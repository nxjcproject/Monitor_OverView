$(document).ready(function () {
    var nowDate = new Date();
    nowDate.setDate(nowDate.getDate() - 1);
    starDate = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate();
    $("#dateTime").datebox('setValue', starDate);

    //InitChartWindows();
    loadFirstCompanyData("first");
    loadPlanItem();
});
function loadFirstCompanyData(myLoadFlag) {
    date = $('#dateTime').datebox('getValue');//datebox('setValue', startDate);
    var m_MsgData;
    $.ajax({
        type: "POST",
        url: "View_EnergyData.aspx/GetGlobalComplete",
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
                        onRowDblClick(m_MsgData.rows[0], "first");
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
function loadPlanItem() {
    $.ajax({
        type: "POST",
        url: "View_EnergyData.aspx/GetPlanItems",
        data: '',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg == "1") {
                alert("您没有获得授权!");
            }
            else {
                m_MsgData = jQuery.parseJSON(msg.d);
                InitPlanItems(m_MsgData);
            }
        },
        error: handleError
    });
}
function QueryReportFun() {
    loadFirstCompanyData("last");
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
    $.messager.progress({
        title: 'Please waiting',
        msg: 'Loading data...'
    });
    date = $('#dateTime').datebox('getValue');
    var m_LevelCode = (rowData["LevelCode"]);
    var companyName = rowData["Name"];
    /////////////////////////综合能耗/////////////////////////
    $.ajax({
        type: "POST",
        url: "View_EnergyData.aspx/GetCompanyComprehensiveComplete",
        data: '{myDate:"' + date + '",myLevelCode:"' + m_LevelCode + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData != null && m_MsgData != undefined) {
                if (myLoadType == "first") {
                    InitializeCompanyGrid(m_MsgData, 'CompanyComprehensiveCompleteGridId','公司');
                }
                else {
                    //InitializeCompanyGrid(m_MsgData, 'CompanyComprehensiveCompleteGridId');
                    $('#CompanyComprehensiveCompleteGridId').datagrid('loadData', m_MsgData);
                }

                $('#CompanyComprehensiveCompleteGridId').datagrid("getPanel").panel("setTitle", companyName);
                var m_Index = 0;
                var m_Name = "";
                for (var i = 0; i < m_MsgData.rows.length; i++) {
                    if (i == 0 && m_Name != m_MsgData.rows[i]["Name"]) {
                        m_Index = i;
                        m_Name = m_MsgData.rows[i]["Name"];
                    }
                    else if (m_Name != m_MsgData.rows[i]["Name"]) {
                        $('#CompanyComprehensiveCompleteGridId').datagrid('mergeCells', {
                            index: m_Index,
                            field: 'Name',
                            rowspan: i - m_Index
                        });
                        m_Index = i;
                        m_Name = m_MsgData.rows[i]["Name"];
                    }

                    if (i + 1 == m_MsgData.rows.length && m_Index != i) {
                        $('#CompanyComprehensiveCompleteGridId').datagrid('mergeCells', {
                            index: m_Index,
                            field: 'Name',
                            rowspan: i - m_Index + 1
                        });
                    }
                    
                }
            }
            $.messager.progress('close');
        },
        error: handleError
    });
    ///////////////////////////分步电耗////////////////////////
    $.ajax({
        type: "POST",
        url: "View_EnergyData.aspx/GetCompanyProcessComplete",
        data: '{myDate:"' + date + '",myLevelCode:"' + m_LevelCode + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData != null && m_MsgData != undefined) {
                if (myLoadType == "first") {
                    InitializeCompanyGrid(m_MsgData, 'CompanyProcessCompleteGridId','');
                }
                else {
                    $('#CompanyProcessCompleteGridId').datagrid('loadData', m_MsgData);
                }
                var m_Index = 0;
                var m_Name = "";
                for (var i = 0; i < m_MsgData.rows.length; i++) {
                    if (i == 0 && m_Name != m_MsgData.rows[i]["Name"]) {
                        m_Index = i;
                        m_Name = m_MsgData.rows[i]["Name"];
                    }
                    else if (m_Name != m_MsgData.rows[i]["Name"]) {
                        $('#CompanyProcessCompleteGridId').datagrid('mergeCells', {
                            index: m_Index,
                            field: 'Name',
                            rowspan: i - m_Index
                        });
                        m_Index = i;
                        m_Name = m_MsgData.rows[i]["Name"];
                    }

                    if (i + 1 == m_MsgData.rows.length && m_Index != i) {
                        $('#CompanyProcessCompleteGridId').datagrid('mergeCells', {
                            index: m_Index,
                            field: 'Name',
                            rowspan: i - m_Index + 1
                        });
                    }

                }
            }
        },
        error: handleError
    });
}
function InitializeCompanyGrid(myData, myObjectId, myTitle) {
    //data-options="idField:'id',treeField:'Name',rownumbers:true,singleSelect:true,fit:true,onDblClickRow:onRowDblClick"
    $('#' + myObjectId).datagrid({
        title: myTitle,
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
    $.messager.progress('close');
}

function InitPlanItems(myData) {
    $('#Combobox_StandardF').combobox({
        data: myData.rows,
        valueField: 'QuotasId',
        textField: 'QuotasName',
        panelHeight:'auto',
        onSelect: function (myRecord) {
            LoadPlanAndCompleteChart(myRecord.VariableId, myRecord.ValueType, myRecord.CaculateType, myRecord.Denominator);
        }
    });

    var val = $('#Combobox_StandardF').combobox("getData");
    for (var item in val[0]) {
        if (item == "QuotasId") {
            $('#Combobox_StandardF').combobox("select", val[0][item]);
        }
    }
}
function LoadPlanAndCompleteChart(myVariableId, myValueType, myCaculateType, myDenominator) {
    date = $('#dateTime').datebox('getValue');//datebox('setValue', startDate);
    var m_MsgData;
    $.ajax({
        type: "POST",
        url: "View_EnergyData.aspx/GetPlanAndCompleteChart",
        data: '{myDate:"' + date + '",myVariableId:"' + myVariableId + '",myValueType:"' + myValueType + '",myCaculateType:"' + myCaculateType + '",myDenominator:"' + myDenominator + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg == "1") {
                alert("您没有获得授权!");
            }
            else {
                m_MsgData = jQuery.parseJSON(msg.d);
                if (m_MsgData.rows != null && m_MsgData.rows != undefined && m_MsgData.rows.length > 0) {
                    var m_WindowContainerId = 'Windows_Container';
                    var m_ChartType = "Bar";
                    var m_ShowType = "SingleScreen";

                    var m_Maximizable = false;
                    var m_Maximized = true;

                    var m_WindowsIdArray = GetWindowsIdArray();
                    if (m_ShowType == 'SingleScreen') {
                        for (var i = 0; i < m_WindowsIdArray.length; i++) {
                            if (m_WindowsIdArray[i] != "") {
                                ReleaseAllGridChartObj(m_WindowsIdArray[i]);
                            }
                        }
                        CloseAllWindows();
                    }

                    //////////////////////////////计算当前windows数量/////////////////////////
                    var m_WindowsCount = 0;
                    var m_EmptyIndex = -1;              //找到第一个空位置放置
                    m_WindowsIdArray = GetWindowsIdArray();
                    for (var i = 0; i < m_WindowsIdArray.length; i++) {
                        if (m_WindowsIdArray[i] != "") {
                            m_WindowsCount = m_WindowsCount + 1;
                        }
                        else {
                            if (m_EmptyIndex == -1) {
                                m_EmptyIndex = i;
                            }
                        }
                    }
                    /////////////////////判断超数量的图表///////////////////////
                    if (m_ShowType == 'SingleScreen' && m_WindowsCount > 0) {
                        alert("请先关闭图表!");
                    }
                    else if (m_ShowType == 'MultiScreen' && m_WindowsCount >= 4) {
                        alert("请先关闭图表!");
                    }
                    else {
                        var m_Postion = GetWindowPostion(m_EmptyIndex, m_WindowContainerId);
                        WindowsDialogOpen(m_MsgData, m_WindowContainerId, false, m_ChartType, m_Postion[0], m_Postion[1], m_Postion[2], m_Postion[3], false, m_Maximizable, m_Maximized);
                        //WindowsDialogOpen(m_MsgData, "Windows_Container", true, "Bar", 100, 100, 0, 0, false, true, false);
                    }
                }
            }
        },
        error: handleError
    });
}

///////////////////////获取window初始位置////////////////////////////
function GetWindowPostion(myWindowIndex, myWindowContainerId) {
    var m_ParentObj = $('#' + myWindowContainerId);
    var m_ParentWidth = m_ParentObj.width();
    var m_ParentHeight = m_ParentObj.height();
    var m_ZeroLeft = 0;
    var m_ZeroTop = 0;
    var m_Padding = 5;
    var m_Width = (m_ParentWidth - m_Padding) / 2;
    var m_Height = (m_ParentHeight - m_Padding) / 2;
    var m_Left = 0;
    var m_Top = 0;
    if (myWindowIndex == 0) {
        m_Left = m_ZeroLeft;
        m_Top = m_ZeroTop;
    }
    else if (myWindowIndex == 1) {
        m_Left = m_ZeroLeft + m_Width + m_Padding;
        m_Top = m_ZeroTop;
    }
    else if (myWindowIndex == 2) {
        m_Left = m_ZeroLeft;
        m_Top = m_ZeroTop + m_Height + m_Padding;
    }
    else if (myWindowIndex == 3) {
        m_Left = m_ZeroLeft + m_Width + m_Padding;
        m_Top = m_ZeroTop + m_Height + m_Padding;
    }

    return [m_Width, m_Height, m_Left, m_Top]
}
///////////////////////////////////////////打开window窗口//////////////////////////////////////////
function WindowsDialogOpen(myData, myContainerId, myIsShowGrid, myChartType, myWidth, myHeight, myLeft, myTop, myDraggable, myMaximizable, myMaximized) {
    ;
    var m_WindowId = OpenWindows(myContainerId, '数据分析', myWidth, myHeight, myLeft, myTop, myDraggable, myMaximizable, myMaximized); //弹出windows
    var m_WindowObj = $('#' + m_WindowId);
    if (myMaximized != true) {
        CreateGridChart(myData, m_WindowId, myIsShowGrid, myChartType);               //生成图表
    }

    m_WindowObj.window({
        onBeforeClose: function () {
            ///////////////////////释放图形空间///////////////
            //var m_ContainerId = GetWindowIdByObj($(this));
            ReleaseGridChartObj(m_WindowId);
            CloseWindow($(this))
        },
        onMaximize: function () {
            TopWindow(m_WindowId);
            ChangeSize(m_WindowId);
            CreateGridChart(myData, m_WindowId, myIsShowGrid, myChartType);

        },
        onRestore: function () {
            //TopWindow(m_WindowId);
            ChangeSize(m_WindowId);
            CreateGridChart(myData, m_WindowId, myIsShowGrid, myChartType);
        }
    });
}