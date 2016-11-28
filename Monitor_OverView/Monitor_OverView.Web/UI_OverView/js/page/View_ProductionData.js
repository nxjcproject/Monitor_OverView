var MaxChartPageItemsCount = 10;
var ChartMsgData;
$(document).ready(function () {
    var nowDate = new Date();
    nowDate.setDate(nowDate.getDate() - 1);
    starDate = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate();
    $("#dateTime").datebox('setValue', starDate);

    //InitChartWindows();
    loadFirstCompanyData("first");
    LoadIndicatorItems();
    LoadEquipmentCommonInfo();
});
function LoadIndicatorItems() {
    $.ajax({
        type: "POST",
        url: "View_ProductionData.aspx/GetIndicatorItems",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            $('#ComboTree_StandardF').combotree({
                data: m_MsgData,
                dataType: "json",
                valueField: 'id',
                textField: 'text',
                required: false,
                panelHeight: 200,
                editable: false
            });

            $('#ComboTree_StandardF').combotree('tree').tree("collapseAll");
            if (m_MsgData != null && m_MsgData != undefined && m_MsgData.length > 0) {
                $('#ComboTree_StandardF').combotree("setValue", m_MsgData[0].id);
            }
        }
    });
}
function LoadEquipmentCommonInfo() {
    $.ajax({
        type: "POST",
        url: "View_ProductionData.aspx/GetEquipmentCommonInfo",
        data: '',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData != null && m_MsgData != undefined) {
                $('#Combobox_EquipmentCommonF').combobox('loadData', m_MsgData.rows);
                if (m_MsgData.rows.length > 0) {
                    $('#Combobox_EquipmentCommonF').combobox("select", m_MsgData.rows[0].EquipmentCommonId);
                    //LoadSpecifications(m_MsgData.rows[0].EquipmentCommonId);     //当选中设备类时,填充数据
                }
            }
        }
    });
}
function LoadSpecifications(myEquipmentCommonId) {
    $.ajax({
        type: "POST",
        url: "View_ProductionData.aspx/GetSpecificationsInfo",
        data: '{myEquipmentCommonId:"' + myEquipmentCommonId +  '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            var m_FinalData = [];
            m_FinalData.push({ "id": "All", "text": "全部" });
            if (m_MsgData != null && m_MsgData != undefined) {
                for (var i = 0; i < m_MsgData.rows.length; i++) {
                    m_FinalData.push(m_MsgData.rows[i]);
                }
            }
            $('#Combobox_SpecificationsF').combobox('loadData', m_FinalData);
            $('#Combobox_SpecificationsF').combobox("select", "All");
        }
    });
}
function loadFirstCompanyData(myLoadFlag) {
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
                $('#CompanyCompleteGridId').datagrid("getPanel").panel("setTitle", companyName);
                //$('#CompanyCompleteGridId').datagrid({
                //    title: companyName
                //});
                for (var i = 0; i < m_MsgData.rows.length / 4; i++) {
                    $('#CompanyCompleteGridId').datagrid('mergeCells', {
                        index: i * 4,
                        field: 'Name',
                        rowspan: 4
                    });
                }
            }
            $.messager.progress('close');
        },
        error: handleError
    });
}
function InitializeCompanyGrid(myData) {
    //data-options="idField:'id',treeField:'Name',rownumbers:true,singleSelect:true,fit:true,onDblClickRow:onRowDblClick"
    $('#CompanyCompleteGridId').datagrid({
        title: '公司',
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
    $.messager.progress('close');
    $('#gridMain_ReportTemplate').datagrid('loadData', { "rows": [], "total": 0 });
    $.messager.alert('失败', '获取数据失败');
}
function GetShowChartData() {
    if (ChartMsgData != undefined && ChartMsgData != null) {
        var m_PageIndexData = [];
        var m_PageCount = Math.floor((ChartMsgData.columns.length - 1) / MaxChartPageItemsCount);    //向下取整
        if (m_PageCount == 0) {
            $('#Th_ChartPageIndexF').css("visibility", "hidden");
            $('#Td_ChartPageIndexF').css("visibility", "hidden");
            return ChartMsgData;
        }
        else {
            $('#Th_ChartPageIndexF').css("visibility", "visible");
            $('#Td_ChartPageIndexF').css("visibility", "visible");
            for (var i = 1; i <= m_PageCount + 1; i++) {
                m_PageIndexData.push({ "id": i.toString(), "text": i.toString() });
            }
            $('#Combobox_PageIndexF').combobox('loadData', m_PageIndexData);
            $('#Combobox_PageIndexF').combobox('setValue', "1");
            return GetShowChartDataByPage("1");
        }
    }
    
}
function GetShowChartDataByPage(myPageIndex) {
    var m_ChartData = [];
    var m_Columns = [];
    var m_Rows = [[], []];
    m_Columns.push({ "title": ChartMsgData.columns[0].title, "field": ChartMsgData.columns[0].field, "width": ChartMsgData.columns[0].width });
    m_Rows[0]["RowName"] = ChartMsgData["rows"][0]["RowName"];
    m_Rows[1]["RowName"] = ChartMsgData["rows"][1]["RowName"];
    for (var i = 1; i <= MaxChartPageItemsCount; i++) {
        var m_ChartMsgDataIndex = (myPageIndex - 1) * MaxChartPageItemsCount + i;
        if ( m_ChartMsgDataIndex < ChartMsgData.columns.length) {          //当前索引在数据数组的范围内
            m_Columns.push({ "title": ChartMsgData.columns[m_ChartMsgDataIndex].title, "field": ChartMsgData.columns[m_ChartMsgDataIndex].field, "width": ChartMsgData.columns[m_ChartMsgDataIndex].width });
            m_Rows[0][ChartMsgData.columns[m_ChartMsgDataIndex].field] = ChartMsgData.rows[0][ChartMsgData.columns[m_ChartMsgDataIndex].field];
            m_Rows[1][ChartMsgData.columns[m_ChartMsgDataIndex].field] = ChartMsgData.rows[1][ChartMsgData.columns[m_ChartMsgDataIndex].field];
        }
        else {                                                                                       //如果超出，则用0补齐,使柱状图好看
            m_Columns.push({ "title": "Blank" + i.toString(), "field": "Blank" + i.toString(), "width": 100 });
            m_Rows[0]["Blank" + i.toString()] = "0.00";
            m_Rows[1]["Blank" + i.toString()] = "0.00";
        }
    }

    m_ChartData["columns"] = m_Columns;
    m_ChartData["rows"] = m_Rows;
    m_ChartData["total"] = 2;
    m_ChartData["FrozenCount"] = 1;
    m_ChartData["Units"] = [];
    m_ChartData["Units"]["UnitX"] = ChartMsgData["Units"]["UnitX"];
    m_ChartData["Units"]["UnitY"] = ChartMsgData["Units"]["UnitY"];

    return m_ChartData;
}
function ChangeChartPageIndex(myRecord) {
    var m_PageIndex = myRecord.id;
    var m_ShowChartData = GetShowChartDataByPage(m_PageIndex);
    DisplayChart(m_ShowChartData);
}
function LoadPlanAndCompleteChart() {
    var date = $('#dateTime').datebox('getValue');//datebox('setValue', startDate);
    var m_IndicatorId = $('#ComboTree_StandardF').combotree("getValue");
    var m_EquipmentCommonId = $('#Combobox_EquipmentCommonF').combobox("getValue");
    var m_Specifications = $('#Combobox_SpecificationsF').combobox("getValue");
    var m_MsgData;
    $.ajax({
        type: "POST",
        url: "View_ProductionData.aspx/GetPlanAndCompleteChart",
        data: '{myDate:"' + date + '",myIndicatorId:"' + m_IndicatorId + '",myEquipmentCommonId:"' + m_EquipmentCommonId + '",mySpecifications:"' + m_Specifications + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg == "1") {
                alert("您没有获得授权!");
            }
            else {
                m_MsgData = jQuery.parseJSON(msg.d);
                if (m_MsgData.rows != null && m_MsgData.rows != undefined && m_MsgData.rows.length > 0) {
                    ChartMsgData = m_MsgData;
                    var m_ShowChartData = GetShowChartData();
                    DisplayChart(m_ShowChartData);
                }
            }
        },
        error: handleError
    });
}
function DisplayChart(myShowChartData) {
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
        WindowsDialogOpen(myShowChartData, m_WindowContainerId, false, m_ChartType, m_Postion[0], m_Postion[1], m_Postion[2], m_Postion[3], false, m_Maximizable, m_Maximized);
        //WindowsDialogOpen(myShowChartData, "Windows_Container", true, "Bar", 100, 100, 0, 0, false, true, false);
    }
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