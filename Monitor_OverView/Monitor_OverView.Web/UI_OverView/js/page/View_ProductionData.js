$(document).ready(function () {
    var nowDate = new Date();
    nowDate.setDate(nowDate.getDate() - 1);
    starDate = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate();
    $("#dateTime").datebox('setValue', starDate);

    //InitChartWindows();
    loadFisrtCompanyData("first");
});
function loadFisrtCompanyData(myLoadFlag) {
    date = $('#dateTime').datebox('getValue');//datebox('setValue', startDate);
    var m_MsgData;
    $.ajax({
        type: "POST",
        url: "View_ProductionData.aspx/GetGlobalComplete",
        data: '{myDate:"' + date + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg == "1") {
                alert("您没有获得授权!");
            }
            else {
                m_MsgData = jQuery.parseJSON(msg.d);
                if (myLoadFlag == "first") {
                    if (m_MsgData != null && m_MsgData != undefined && m_MsgData.rows.length > 0) {
                        InitializeGlobalGrid(m_MsgData);
                        onRowDblClick(m_MsgData.rows[0],"first");
                    }
                }
                else {
                    $('#GlobalCompleteGridId').datagrid('loadData', m_MsgData);
                    onRowDblClick(m_MsgData.rows[0], "last");
                }
            }
        },
        error: handleError
    });
}
function QueryReportFun() {
    loadFisrtCompanyData("last");
}
function InitializeGlobalGrid(myData) {
    //data-options="idField:'id',treeField:'Name',rownumbers:true,singleSelect:true,fit:true,onDblClickRow:onRowDblClick"
    $('#GlobalCompleteGridId').datagrid({
        title: '',
        data: myData,
        dataType: "json",
        striped: true,
        rownumbers: true,
        singleSelect: true,
        fit: true,
        idField: "id",

        toolbar: '#QueryTools',
        onDblClickRow: function (rowIndex, rowData) {
            onRowDblClick(rowData, "last");
        }
    });
}

function onRowDblClick(rowData, myLoadType) {
    date = $('#dateTime').datebox('getValue');
    var m_LevelCode = (rowData["LevelCode"]);
    var companyName = rowData["Name"];
    $.ajax({
        type: "POST",
        url: "View_ProductionData.aspx/GetCompanyComplete",
        data: '{myDate:"' + date + '",myLevelCode:"' + m_LevelCode + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData != null && m_MsgData != undefined) {
                if (myLoadType == "first") {
                    InitializeCompanyGrid(m_MsgData);
                }
                else {
                    $('#CompanyCompleteGridId').datagrid('loadData', m_MsgData);
                }
                $('#CompanyCompleteGridId').datagrid({
                    title: companyName
                });
                for (var i = 0; i < m_MsgData.rows.length / 4; i++) {
                    $('#CompanyCompleteGridId').datagrid('mergeCells', {
                        index: i * 4,
                        field: 'Name',
                        rowspan: 4
                    });
                }
            }
        },
        error: handleError
    });
}
function InitializeCompanyGrid(myData) {
    //data-options="idField:'id',treeField:'Name',rownumbers:true,singleSelect:true,fit:true,onDblClickRow:onRowDblClick"
    $('#CompanyCompleteGridId').datagrid({
        title: '',
        data: myData,
        dataType: "json",
        striped: true,
        rownumbers: true,
        singleSelect: true,
        fit: true,
        idField: "id"
    });
}
function handleError() {
    $('#gridMain_ReportTemplate').datagrid('loadData', []);
    $.messager.alert('失败', '获取数据失败');
}