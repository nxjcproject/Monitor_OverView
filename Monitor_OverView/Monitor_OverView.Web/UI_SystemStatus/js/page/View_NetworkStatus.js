var NetworkNodeArray = [];           //需要初始化的连接线
var TableRowArray;
var StatusArray = [];
$(document).ready(function () {
    loadDialog();
    InitAmmeterStatusDataGrid();
    GetNetworkStructure();
    //ResizeNetworkStructure();
    GetStatusValue();
});

function ResizeNetworkStructure() {
    var m_DisplayWidth = $('#MainLayout').layout('panel', 'center').panel('options').width - 20;
    var m_DisplayHeight = $('#MainLayout').layout('panel', 'center').panel('options').height;
    var m_GroupTitlesWidth = 40;   //集团标题
    var m_GroupNetworkWidth = 20;   //集团网络竖线
    var m_NodeTitleWidth = 140 * 3;      //节点名称
    var m_NetwordNodeWidth = 20 * 2;       //   网络节点 
    //var m_NetworkLineWidth = $('.NetworkLine').width() * 6;       //   网络线

    //var m_NetworkNodeLineWidth = $('.NetworkNodeLine').width() * 4;       //节点分叉线名称
    var m_FloatingWidth = m_DisplayWidth - m_GroupTitlesWidth - m_GroupNetworkWidth - m_NodeTitleWidth - m_NetwordNodeWidth;
    $('.NetworkLine').width(parseInt(m_FloatingWidth / 5));
    $('.NetworkNodeLine').width(parseInt(m_FloatingWidth / 5));

    var m_BlankWidth = m_FloatingWidth - 5 * parseInt(m_FloatingWidth / 5);
    $("#StatusMainTable").width(m_DisplayWidth - m_BlankWidth);

    //var bb = parseInt(m_FloatingWidth / 5);
    //var cc = $("#StatusMainTable").width();
    //var dd = 3 * $('.NetworkLine').width();
    //var ee = 2 * $(".NetworkNodeLine").width();
    //var ff = $(".NodeTitle").width();
    //var gg = 2 * $(".NetworkNode").width();
    //var mm = $(".GroupNetwork").width();
    //var nn = $(".GroupTitle").width();
    //var aa = nn + mm + ff + gg + dd + ee;
    //alert(ff);
}
function GetNetworkStructure() {
    $.ajax({
        type: "POST",
        url: "NetworkMonitor.asmx/GetNetworkStructure",
        data: '',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData["FactoryServers"] != null && m_MsgData["FactoryServers"] != undefined) {
                InitializeNetworkStructure(m_MsgData["FactoryServers"]);
            }
        },
        error:function(msg)
        {
            var m_Masg = msg;
        }
    });
}
function InitializeNetworkStructure(myData) {
    $('#StatusMainTable').empty();         //清空框架表格

    var m_HtmlString = "";
    //var m_GroupLineString = "";
    var m_GroupLineString = '<td id ="GroupTitle1" class="GroupTitle" rowspan ="' + myData.length * 2 + '">宁<br/>夏<br/>建<br/>材</td>' +
          '<td class="GroupNetwork" rowspan ="' + myData.length * 2 + '"></td>';
    for (var i = 0; i < myData.length; i++) {             //生成框架Html
        if (i == 0) {
            m_HtmlString = m_HtmlString + '<tr>' + m_GroupLineString + '<td class = "MainTableTd"><table class = "FactoryTable" id = "Table_' + myData[i]["OrganizationId"] + '"></table></td></tr>';
            m_HtmlString = m_HtmlString + '<tr><td class = "FactoryServerBlank"></td></tr>'
            //$('#Table_' + myData[i]["OrganizationId"]).css("width", 12 * 50);
        }
        else {
            m_HtmlString = m_HtmlString + '<tr><td  class = "MainTableTd"><table class = "FactoryTable" id = "Table_' + myData[i]["OrganizationId"] + '"></table></td></tr>';
            m_HtmlString = m_HtmlString + '<tr><td class = "FactoryServerBlank"></td></tr>'
            //$('#Table_' + myData[i]["OrganizationId"]).css("width", 12 * 50);
        }
        //ResizeNetworkStructure();
    }
    $('#StatusMainTable').html(m_HtmlString);
    NetworkNodeArray = [];
    var m_MaxSubTableWidth = 0;
    for (var i = 0; i < myData.length; i++) {          //多个服务器
        TableRowArray = [];

        var m_TableColumnCount = GetServerAllCollectorCount(myData[i]);
        var m_TableRowCount = GetServerDepth(myData[i]);
        for (var m = 0; m < m_TableRowCount; m++) {                 //生成等行等列的表格
            TableRowArray.push([]);
            for (var n = 0; n < m_TableColumnCount; n++) {
                TableRowArray[m].push([]);
            }
        }
        GetServerInfoInsertTable(myData[i], 0, 0, m_TableColumnCount);              //获得Server信息并在相应的位置存入信息
        var m_MaxSubTableWidthTemp = GetStructHtml(myData[i]["OrganizationId"]);
        if (m_MaxSubTableWidth < m_MaxSubTableWidthTemp) {
            m_MaxSubTableWidth = m_MaxSubTableWidthTemp;
        }
    }
    $('#StatusMainTable').css('width', m_MaxSubTableWidth + 50);
}

function GetServerAllCollectorCount(myServer) {          //计算这个server下所有的采集设备
    var m_CollectorCount = 0;
    if (myServer != null && myServer["DataComputer"] != null) {
        for (var i = 0; i < myServer["DataComputer"].length; i++) {
            if (myServer["DataComputer"][i]["Switch"] != null) {
                m_CollectorCount = m_CollectorCount + myServer["DataComputer"][i]["Switch"]["CollectorCount"];
            }
        }
    }
    return m_CollectorCount;
}
function GetServerDepth(myServer) {
    var m_MaxDepth = 0;
    if (myServer != null && myServer["DataComputer"] != null) {
        for (var i = 0; i < myServer["DataComputer"].length; i++) {
            if (myServer["DataComputer"][i]["Switch"] != null) {
                if(m_MaxDepth < myServer["DataComputer"][i]["Switch"]["MaxDepth"])
                {
                    m_MaxDepth = myServer["DataComputer"][i]["Switch"]["MaxDepth"];
                }
            }
        }
    }
    m_MaxDepth = m_MaxDepth * 2 + 5;         //5表示：服务器1行、连接线1行、数采计算机1行、连接线1行、交换机1行
    return m_MaxDepth;
}

function GetServerInfoInsertTable(myServer, myFirstNodeRowIndex, myFirstNodeColumnIndex, myTableColumnCount) {
    var m_NodeColumnIndex = myFirstNodeColumnIndex + parseInt(myTableColumnCount / 2);
    TableRowArray[myFirstNodeRowIndex][m_NodeColumnIndex - 1] = {
        "NodeId": myServer["Id"], "NodeName": myServer["Name"], "NodeType": "FactoryServer", "OrganizationId": myServer["OrganizationId"], "data-options": '{"Name":"' + myServer["Name"] +
   '", "IpAddress":"' + myServer["IpAddress"] + '", "OrganizationId":"' + myServer["OrganizationId"] + '", "PropertyName": "' + myServer["PropertyName"] + '"}',
        "X": m_NodeColumnIndex - 1, "Y": myFirstNodeRowIndex
    };
    for (var i = 0; i < m_NodeColumnIndex - 1; i++) {
        TableRowArray[myFirstNodeRowIndex][i] = {          //连接线
            "NodeId": myServer["Id"], "NodeName": myServer["Id"], "NodeType": "Network", "CrossPointStyle": "1",
            "X": m_NodeColumnIndex + i, "Y": myFirstNodeRowIndex
        };
    }
    var m_ServerNetworkTemp = [];
    m_ServerNetworkTemp["Id"] = myServer["Id"];
    m_ServerNetworkTemp["SynchronizationStatus"] = "true";
    m_ServerNetworkTemp["SoftwareStatus"] = "true";
    m_ServerNetworkTemp["NetworkStatus"] = "true";
    NetworkNodeArray.push(m_ServerNetworkTemp);          //设置默认状态线


    if (myServer["DataComputer"] != null && myServer["DataComputer"] != undefined) {                //找下级数采计算机
        var m_FirstNodeColumnIndex = 0;
        var m_LastDataComputerNodexColumnIndex = 0;
        for (var i = 0; i < myServer["DataComputer"].length; i++) {
            if (myServer["DataComputer"][i]["Switch"] != null && myServer["DataComputer"][i]["Switch"] != undefined) {
                var m_CollectorCount = myServer["DataComputer"][i]["Switch"]["CollectorCount"];
                var m_DataComputerNodexColumnIndex = 0;
                m_DataComputerNodexColumnIndex = GetDataComputerInfoInsertTable(myServer["DataComputer"][i], myFirstNodeRowIndex + 2, m_FirstNodeColumnIndex, m_CollectorCount);
                if (myServer["DataComputer"].length == 1) {       //只有一台数采计算机
                    TableRowArray[myFirstNodeRowIndex + 1][m_NodeColumnIndex] = { "NodeName": myServer["DataComputer"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "2", "X": m_NodeColumnIndex, "Y": myFirstNodeRowIndex + 1 };
                }
                else if (i == 0) {
                    TableRowArray[myFirstNodeRowIndex + 1][m_DataComputerNodexColumnIndex] = { "NodeName": myServer["DataComputer"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "8", "X": m_DataComputerNodexColumnIndex, "Y": myFirstNodeRowIndex + 1 };
                }
                else if (i == myServer["DataComputer"].length - 1 && m_DataComputerNodexColumnIndex == m_NodeColumnIndex) {  //如果是最后一个数采电脑,并且数采电脑的坐标与父节点一样
                    TableRowArray[myFirstNodeRowIndex + 1][m_DataComputerNodexColumnIndex] = { "NodeName": myServer["DataComputer"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "6", "X": m_DataComputerNodexColumnIndex, "Y": myFirstNodeRowIndex + 1 };
                }
                else if (m_DataComputerNodexColumnIndex == m_NodeColumnIndex)               //
                {
                    TableRowArray[myFirstNodeRowIndex + 1][m_DataComputerNodexColumnIndex] = { "NodeName": myServer["DataComputer"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "3", "X": m_DataComputerNodexColumnIndex, "Y": myFirstNodeRowIndex + 1 };
                }
                else if (i == myServer["DataComputer"].length - 1) {
                    TableRowArray[myFirstNodeRowIndex + 1][m_DataComputerNodexColumnIndex] = { "NodeName": myServer["DataComputer"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "9", "X": m_DataComputerNodexColumnIndex, "Y": myFirstNodeRowIndex + 1 };
                }
                else {
                    TableRowArray[myFirstNodeRowIndex + 1][m_DataComputerNodexColumnIndex] = { "NodeName": myServer["DataComputer"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "5", "X": m_DataComputerNodexColumnIndex, "Y": myFirstNodeRowIndex + 1 };
                }

                if (i != 0) {
                    if (m_DataComputerNodexColumnIndex <= m_NodeColumnIndex)    //两个节点都在左边的情况
                    {
                        for (var j = m_LastDataComputerNodexColumnIndex + 1; j < m_DataComputerNodexColumnIndex; j++) {
                            if (j != m_NodeColumnIndex) {
                                TableRowArray[myFirstNodeRowIndex + 1][j] = { "NodeName": myServer["DataComputer"][i - 1]["Id"], "NodeType": "Network", "CrossPointStyle": "1", "X": j, "Y": myFirstNodeRowIndex + 1 };
                            }
                            else {
                                TableRowArray[myFirstNodeRowIndex + 1][j] = { "NodeName": myServer["DataComputer"][i - 1]["Id"], "NodeType": "Network", "CrossPointStyle": "4", "X": j, "Y": myFirstNodeRowIndex + 1 };
                            }
                        }
                    }
                    else if (m_LastDataComputerNodexColumnIndex >= m_NodeColumnIndex)    //两个节点都在右边的情况
                    {
                        for (var j = m_LastDataComputerNodexColumnIndex + 1; j < m_DataComputerNodexColumnIndex; j++) {
                            if (j != m_NodeColumnIndex) {
                                TableRowArray[myFirstNodeRowIndex + 1][j] = { "NodeName": myServer["DataComputer"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "1", "X": j, "Y": myFirstNodeRowIndex + 1 };
                            }
                            else {
                                TableRowArray[myFirstNodeRowIndex + 1][j] = { "NodeName": myServer["DataComputer"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "4", "X": j, "Y": myFirstNodeRowIndex + 1 };
                            }
                        }
                    }
                    else {                                                      //一左一右的情况
                        for (var j = m_LastDataComputerNodexColumnIndex + 1; j < m_NodeColumnIndex; j++) {
                            if (j != m_NodeColumnIndex) {
                                TableRowArray[myFirstNodeRowIndex + 1][j] = { "NodeName": myServer["DataComputer"][i - 1]["Id"], "NodeType": "Network", "CrossPointStyle": "1", "X": j, "Y": myFirstNodeRowIndex + 1 };
                            }
                            else {
                                TableRowArray[myFirstNodeRowIndex + 1][j] = { "NodeName": myServer["DataComputer"][i - 1]["Id"], "NodeType": "Network", "CrossPointStyle": "4", "X": j, "Y": myFirstNodeRowIndex + 1 };
                            }
                        }
                        for (var j = m_NodeColumnIndex; j < m_DataComputerNodexColumnIndex; j++) {
                            if (j != m_NodeColumnIndex) {
                                TableRowArray[myFirstNodeRowIndex + 1][j] = { "NodeName": myServer["DataComputer"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "1", "X": j, "Y": myFirstNodeRowIndex + 1 };
                            }
                            else {
                                TableRowArray[myFirstNodeRowIndex + 1][j] = { "NodeName": myServer["DataComputer"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "4", "X": j, "Y": myFirstNodeRowIndex + 1 };
                            }
                        }
                    }

                }
                m_LastDataComputerNodexColumnIndex = m_DataComputerNodexColumnIndex;

                var m_DataComputerNetworkTemp = [];
                m_DataComputerNetworkTemp["Id"] = myServer["DataComputer"][i]["Id"];
                m_DataComputerNetworkTemp["SoftwareStatus"] = "true";
                m_DataComputerNetworkTemp["NetworkStatus"] = "true";
                NetworkNodeArray.push(m_DataComputerNetworkTemp);          //设置默认状态线
            }


            //m_DataComputerNodeColumnIndexArray.push(m_DataComputerNodexColumnIndex);        //用户生成连接线




            m_FirstNodeColumnIndex = m_FirstNodeColumnIndex + m_CollectorCount;
        }
    }


    //NetworkNodeArray.push(myDataComputer["Switch"]["Id"]);          //设置默认状态线
}
function GetDataComputerInfoInsertTable(myDataComputer, myFirstNodeRowIndex, myFirstNodeColumnIndex, myTableColumnCount) {
    var m_NodeColumnIndex = myFirstNodeColumnIndex + parseInt(myTableColumnCount / 2);
    TableRowArray[myFirstNodeRowIndex][m_NodeColumnIndex] = {          //数采计算机
        "NodeId": myDataComputer["Id"], "NodeName": myDataComputer["Name"], "NodeType": "DataComputer", "data-options": '{"Name":"' + myDataComputer["Name"] +
   '", "IpAddress":"' + myDataComputer["IpAddress"] + '", "PropertyName": "' + myDataComputer["PropertyName"] + '"}',
        "X": m_NodeColumnIndex, "Y": myFirstNodeRowIndex
    };
    TableRowArray[myFirstNodeRowIndex + 1][m_NodeColumnIndex] = {          //连接线
        "NodeId": myDataComputer["Switch"]["Id"], "NodeName": myDataComputer["Switch"]["Id"], "NodeType": "Network", "CrossPointStyle": "2",
        "X": m_NodeColumnIndex, "Y": myFirstNodeRowIndex + 1
    };
    TableRowArray[myFirstNodeRowIndex + 2][m_NodeColumnIndex] = {          //第一级交换机
        "NodeId": myDataComputer["Switch"]["Id"], "NodeName": myDataComputer["Switch"]["Name"], "NodeType": "Switch", "data-options": '{"Name":"' + myDataComputer["Switch"]["Name"] +
   '", "PropertyName": "' + myDataComputer["PropertyName"] + '"}',
        "X": m_NodeColumnIndex, "Y": myFirstNodeRowIndex + 2
    };

    var m_SwitchNetworkTemp = [];
    m_SwitchNetworkTemp["Id"] = myDataComputer["Switch"]["Id"];
    m_SwitchNetworkTemp["SoftwareStatus"] = "true";
    m_SwitchNetworkTemp["NetworkStatus"] = "true";
    NetworkNodeArray.push(m_SwitchNetworkTemp);          //设置默认状态线


    var m_SubNodeArray = GetSubNodeInfo(myDataComputer["Switch"], myFirstNodeRowIndex + 3, myFirstNodeColumnIndex, m_NodeColumnIndex);    //获得下级节点
    if (m_SubNodeArray != null && m_SubNodeArray != undefined) {
        for (var i = 0; i < m_SubNodeArray.length; i++) {
            TableRowArray[m_SubNodeArray[i]["Y"]][m_SubNodeArray[i]["X"]] = m_SubNodeArray[i];
        }
    }
    return m_NodeColumnIndex;
}

function GetSubNodeInfo(mySwitch, myFirstNodeRowIndex, myFirstNodeColumnIndex, myParentNodeColumnIndex) {
    var m_CurrentNodeRowIndex = myFirstNodeRowIndex;
    var m_CurrentNodeColumnIndex = myFirstNodeColumnIndex;
    var m_NodeArray = [];
    //var m_CrossPointArray = [];
    var m_MaxDepth = mySwitch["MaxDepth"];
    var m_CollectorCount = mySwitch["Collector"].length;
    var m_SubSwitchCount = mySwitch["Switch"].length;

    ////////////////////////////////////////////////////////////////////////
    ///////////////////////得到分支结构///////////////////////////

    if (m_CollectorCount > 0) {           //交换机下有叶子节点
        for (var i = 0; i < m_CollectorCount; i++) {
            if (m_CollectorCount == 1) {      //如果只有一个节点
                m_NodeArray.push({ "NodeName": mySwitch["Collector"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "2", "X": myParentNodeColumnIndex, "Y": m_CurrentNodeRowIndex });
            }
            else if (i == 0) { 
                m_NodeArray.push({ "NodeName": mySwitch["Collector"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "8", "X": m_CurrentNodeColumnIndex, "Y": m_CurrentNodeRowIndex });
            }
            else if (m_CurrentNodeColumnIndex == myParentNodeColumnIndex && m_CollectorCount == 2) {
                m_NodeArray.push({ "NodeName": mySwitch["Collector"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "6", "X": m_CurrentNodeColumnIndex, "Y": m_CurrentNodeRowIndex });
            }
            else if (m_CurrentNodeColumnIndex == myParentNodeColumnIndex) {
                m_NodeArray.push({ "NodeName": mySwitch["Collector"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "3", "X": m_CurrentNodeColumnIndex, "Y": m_CurrentNodeRowIndex });
            }
            else if (i == m_CollectorCount - 1) {
                m_NodeArray.push({ "NodeName": mySwitch["Collector"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "9", "X": m_CurrentNodeColumnIndex, "Y": m_CurrentNodeRowIndex });
            }
            else {
                m_NodeArray.push({ "NodeName": mySwitch["Collector"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "5", "X": m_CurrentNodeColumnIndex, "Y": m_CurrentNodeRowIndex });
            }
            m_NodeArray.push({
                "NodeId": mySwitch["Collector"][i]["Id"], "NodeName": mySwitch["Collector"][i]["Name"], "NodeType": mySwitch["Collector"][i]["Type"], "data-options": '{"Name":"' + mySwitch["Collector"][i]["Name"] +
           '", "IpAddress":"' + mySwitch["Collector"][i]["IpAddress"] + '", "Type":"' + mySwitch["Collector"][i]["Type"] + '", "PropertyName": "' + mySwitch["Collector"][i]["PropertyName"] + '"}',
                "X": m_CurrentNodeColumnIndex, "Y": m_CurrentNodeRowIndex + 1
            });


            var m_CollectorNetworkTemp = [];
            m_CollectorNetworkTemp["Id"] = mySwitch["Collector"][i]["Id"];
            m_CollectorNetworkTemp["SoftwareStatus"] = "true";
            m_CollectorNetworkTemp["NetworkStatus"] = "true";
            m_CollectorNetworkTemp["SubNodeStatus"] = "true";
            NetworkNodeArray.push(m_CollectorNetworkTemp);          //设置默认状态线
            m_CurrentNodeColumnIndex = m_CurrentNodeColumnIndex + 1;
        }
    }
    if (m_SubSwitchCount > 0) {
        for (var i = 0; i < m_SubSwitchCount; i++) {
            var m_SwitchNodeOffsetX = parseInt(mySwitch["Switch"][i]["CollectorCount"] / 2);
            if (m_SubSwitchCount == 1) {
                m_NodeArray.push({ "NodeName": mySwitch["Switch"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "2", "X": myParentNodeColumnIndex, "Y": m_CurrentNodeRowIndex });
            }
            else if (i == 0) {       //如果只有一个节点
                m_NodeArray.push({ "NodeName": mySwitch["Switch"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "8", "X": m_CurrentNodeColumnIndex + m_SwitchNodeOffsetX, "Y": m_CurrentNodeRowIndex });
            }
            else if (m_CurrentNodeColumnIndex + m_SwitchNodeOffsetX == myParentNodeColumnIndex && m_SubSwitchCount == 2) {
                m_NodeArray.push({ "NodeName": mySwitch["Switch"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "6", "X": m_CurrentNodeColumnIndex + m_SwitchNodeOffsetX, "Y": m_CurrentNodeRowIndex });
            }
            else if (m_CurrentNodeColumnIndex + m_SwitchNodeOffsetX == myParentNodeColumnIndex) {
                m_NodeArray.push({ "NodeName": mySwitch["Switch"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "3", "X": m_CurrentNodeColumnIndex + m_SwitchNodeOffsetX, "Y": m_CurrentNodeRowIndex });
            }
            else if (i == m_SubSwitchCount - 1) {
                m_NodeArray.push({ "NodeName": mySwitch["Switch"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "9", "X": m_CurrentNodeColumnIndex + m_SwitchNodeOffsetX, "Y": m_CurrentNodeRowIndex });
            }
            else {
                m_NodeArray.push({ "NodeName": mySwitch["Switch"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "5", "X": m_CurrentNodeColumnIndex + m_SwitchNodeOffsetX, "Y": m_CurrentNodeRowIndex });
            }
            m_NodeArray.push({
                "NodeId": mySwitch["Switch"][i]["Id"], "NodeName": mySwitch["Switch"][i]["Name"], "NodeType": "Switch", "data-options": '{"Name":"' + mySwitch["Switch"][i]["Name"] +
           '", "Type":"' + "Switch" + '", "PropertyName": "' + mySwitch["Switch"][i]["PropertyName"] + '"}',
                "X": m_CurrentNodeColumnIndex + m_SwitchNodeOffsetX, "Y": m_CurrentNodeRowIndex + 1
            });

            if (i == 0) {         //第一个子交换机节点,只有右边连接线
                for (var j = m_SwitchNodeOffsetX; j < mySwitch["Switch"][i]["CollectorCount"]; j++) {           //当有子节点时,需要补上两个边缘连接线之间的横线
                    if (j != m_SwitchNodeOffsetX && m_CurrentNodeColumnIndex + j != myParentNodeColumnIndex) {
                        m_NodeArray.push({ "NodeName": mySwitch["Switch"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "1", "X": m_CurrentNodeColumnIndex + j, "Y": m_CurrentNodeRowIndex });
                    }
                    else if (j != m_SwitchNodeOffsetX && m_CurrentNodeColumnIndex + j == myParentNodeColumnIndex) {
                        m_NodeArray.push({ "NodeName": mySwitch["Id"], "NodeType": "Network", "CrossPointStyle": "4", "X": m_CurrentNodeColumnIndex + j, "Y": m_CurrentNodeRowIndex });
                    }
                }
            }
            else if (i == m_SubSwitchCount - 1) {  //第一个子交换机节点,只有左边边连接线
                for (var j = 0; j < m_SwitchNodeOffsetX; j++) {           //当有子节点时,需要补上两个边缘连接线之间的横线
                    if (m_CurrentNodeColumnIndex + j != myParentNodeColumnIndex) {
                        m_NodeArray.push({ "NodeName": mySwitch["Switch"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "1", "X": m_CurrentNodeColumnIndex + j, "Y": m_CurrentNodeRowIndex });
                    }
                    else if (m_CurrentNodeColumnIndex + j == myParentNodeColumnIndex) {
                        m_NodeArray.push({ "NodeName": mySwitch["Id"], "NodeType": "Network", "CrossPointStyle": "4", "X": m_CurrentNodeColumnIndex + j, "Y": m_CurrentNodeRowIndex });
                    }
                }
            }
            else {                                 //中间时两边都有连接线 
                for (var j = 0; j < mySwitch["Switch"][i]["CollectorCount"]; j++) {           //当有子节点时,需要补上两个边缘连接线之间的横线
                    if (j != m_SwitchNodeOffsetX && m_CurrentNodeColumnIndex + j != myParentNodeColumnIndex) {
                        m_NodeArray.push({ "NodeName": mySwitch["Switch"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "1", "X": m_CurrentNodeColumnIndex + j, "Y": m_CurrentNodeRowIndex });
                    }
                    else if (j != m_SwitchNodeOffsetX && m_CurrentNodeColumnIndex + j == myParentNodeColumnIndex) {
                        m_NodeArray.push({ "NodeName": mySwitch["Id"], "NodeType": "Network", "CrossPointStyle": "4", "X": m_CurrentNodeColumnIndex + j, "Y": m_CurrentNodeRowIndex });
                    }
                }
            }

            var m_SubSwitchNetworkTemp = [];
            m_SubSwitchNetworkTemp["Id"] = mySwitch["Switch"][i]["Id"];
            m_SubSwitchNetworkTemp["SoftwareStatus"] = "true";
            m_SubSwitchNetworkTemp["NetworkStatus"] = "true";
            NetworkNodeArray.push(m_SubSwitchNetworkTemp);          //设置默认状态线

            var m_SubNodeArray = GetSubNodeInfo(mySwitch["Switch"][i], m_CurrentNodeRowIndex + 2, m_CurrentNodeColumnIndex, m_CurrentNodeColumnIndex + m_SwitchNodeOffsetX);
            if (m_SubNodeArray != null && m_SubNodeArray != undefined) {
                for (var j = 0; j < m_SubNodeArray.length; j++) {
                    m_NodeArray.push(m_SubNodeArray[j]);
                }
            }


            m_CurrentNodeColumnIndex = m_CurrentNodeColumnIndex + mySwitch["Switch"][i]["CollectorCount"];
        }
    }
    return m_NodeArray;
}
function GetStructHtml(myOrganizationId) {
    ///////////////////////生成Html////////////////////////
    var m_OrganizationId = "";
    var m_HtmlString = "";
    for (var i = 0; i < TableRowArray.length; i++) {            //生成Html
        var m_BlanckHtmlString = "";
        m_HtmlString = m_HtmlString + '<tr>'
        for (var j = 0; j < TableRowArray[i].length; j++) {
            if (TableRowArray[i][j]["NodeName"] != null && TableRowArray[i][j]["NodeName"] != undefined) {
                if (TableRowArray[i][j]["NodeType"] == "Network") {
                    m_HtmlString = m_HtmlString + m_BlanckHtmlString + '<td class = "Network" name = "' + TableRowArray[i][j]["NodeName"] + '" data-options=\'{"CrossPointStyle":"' + TableRowArray[i][j]["CrossPointStyle"] + '"}\'></td>';
                    m_BlanckHtmlString = "";
                }
                else if (TableRowArray[i][j]["NodeType"] == "FactoryServer") {
                    m_OrganizationId = TableRowArray[i][j]["OrganizationId"];
                    var m_NodeName = TableRowArray[i][j]["NodeName"];
                    var m_StringLength = GetByteLen(m_NodeName);
                    if (m_StringLength > 30) {
                        m_NodeName = m_NodeName.substring(0, 26) + '...';
                    }
                    m_HtmlString = m_HtmlString + m_BlanckHtmlString + '<td id = "' + TableRowArray[i][j]["NodeId"] + '" class = "' + TableRowArray[i][j]["NodeType"] +
                      '" data-options=\'' + TableRowArray[i][j]["data-options"] + '\' colspan = 3 onclick = "FactoryInfo(\'' + TableRowArray[i][j]["NodeId"] + '\')">' + m_NodeName + '</td>'
                    j = j + 2;
                    m_BlanckHtmlString = "";
                }
                else if(TableRowArray[i][j]["NodeType"] == "DataComputer")
                {
                    var m_Colspan = 3;
                    var m_BackImageNameSuffix = "";
                    if (j == 0) {                     //当第一个格就是数采电脑时
                        m_Colspan = 1;
                        m_BackImageNameSuffix = "_S";
                    }
                    else if (j == TableRowArray[i].length - 1) { //当最后一个格就是数采电脑时
                        m_Colspan = 1;
                        m_BackImageNameSuffix = "_S";
                    }
                    else { 
                        if (TableRowArray[i][j - 1]["NodeType"] != undefined && TableRowArray[i][j - 1]["NodeType"] != "") {
                            m_Colspan = 1;
                            m_BackImageNameSuffix = "_S";
                        }
                        else if (TableRowArray[i][j + 1]["NodeType"] != undefined && TableRowArray[i][j + 1]["NodeType"] != "") {
                            m_Colspan = 1;
                            m_BackImageNameSuffix = "_S";
                        }
                        else {
                            m_BlanckHtmlString = "";
                        }
                    }
                    var m_NodeName = TableRowArray[i][j]["NodeName"];
                    var m_StringLength = GetByteLen(m_NodeName);
                    if (m_StringLength > 30) {
                        m_NodeName = m_NodeName.substring(0, 26) + '...';
                    }
                    m_HtmlString = m_HtmlString + m_BlanckHtmlString + '<td id = "' + TableRowArray[i][j]["NodeId"] + '" class = "' + TableRowArray[i][j]["NodeType"] + m_BackImageNameSuffix + 
                      '" data-options=\'' + TableRowArray[i][j]["data-options"] + '\' colspan = ' + m_Colspan + ' onclick = "DataComputerInfo(\'' + TableRowArray[i][j]["NodeId"] + '\')">' + m_NodeName + '</td>'
                    if (m_BackImageNameSuffix != "_S") {
                        j = j + 1;
                    }
                    m_BlanckHtmlString = "";
                }
                else if (TableRowArray[i][j]["NodeType"] == "Switch") {
                    var m_NodeName = TableRowArray[i][j]["NodeName"];
                    if (m_NodeName.length > 7) {
                        m_NodeName = m_NodeName.substring(0, 6) + '...';
                    }
                    m_HtmlString = m_HtmlString + m_BlanckHtmlString + '<td id = "' + TableRowArray[i][j]["NodeId"] + '" class = "' + TableRowArray[i][j]["NodeType"] +
                      '" data-options=\'' + TableRowArray[i][j]["data-options"] + '\' onclick = "alert(\'' + TableRowArray[i][j]["NodeName"] + '\')">' + m_NodeName + '</td>'
                    m_BlanckHtmlString = "";
                }
                else {
                    var m_NodeName = TableRowArray[i][j]["NodeName"];
                    if (m_NodeName.length > 7) {
                        m_NodeName = m_NodeName.substring(0, 6) + '...';
                    }
                    m_HtmlString = m_HtmlString + m_BlanckHtmlString + '<td id = "' + TableRowArray[i][j]["NodeId"] + '" class = "' + TableRowArray[i][j]["NodeType"] +
                      '" data-options=\'' + TableRowArray[i][j]["data-options"] + '\' onclick = "CollectorInfo(\'' + TableRowArray[i][j]["NodeId"] + '\',\'' + m_OrganizationId + '\')">' + m_NodeName + '</td>'
                    m_BlanckHtmlString = "";
                }
            }
            else {
                if (m_BlanckHtmlString != "") {
                    m_HtmlString = m_HtmlString + '<td class = "Network"></td>';
                }
                m_BlanckHtmlString = '<td class = "Network"></td>';
            }
        }
        m_HtmlString = m_HtmlString + '</tr>';
    }
    $('#Table_' + myOrganizationId).html(m_HtmlString);

    var m_ColumnCount = 0;
    if (TableRowArray.length > 0) {
        m_ColumnCount = 50 * TableRowArray[0].length;
    }
    $('#Table_' + myOrganizationId).css('width', m_ColumnCount);
    SetStatusColor();             //显示状态颜色
    return m_ColumnCount;
}

function GetByteLen(myString) {
    var len = 0;
    for (var i = 0; i < myString.length; i++) {
        if (myString[i].match(/[^\x00-\xff]/ig) != null) //全角
            len += 2;
        else
            len += 1;
    }
    return len;
}

function FactoryInfo(myFactoryId) {
    var m_FactoryServerInfoObj = $('#' + myFactoryId).data("options");
    $('#TextBox_FactoryName').textbox('setValue', m_FactoryServerInfoObj.Name);
    $('#TextBox_FactoryAddress').textbox('setValue', m_FactoryServerInfoObj.IpAddress);
    for (var i = 0; i < NetworkNodeArray.length; i++) {
        if (NetworkNodeArray[i]["Id"] == myFactoryId) {
            $('#Text_FactoryNetworkStatus').textbox('setValue', NetworkNodeArray[i]["NetworkStatus"].toLowerCase() == "true" ? "正常" : "不正常");
            $('#Text_SynchronizationStatus').textbox('setValue', NetworkNodeArray[i]["SynchronizationStatus"].toLowerCase() == "true" ? "正常" : "不正常");
            $('#Text_FactoryCollectionSoftStatus').textbox('setValue', NetworkNodeArray[i]["SoftwareStatus"].toLowerCase() == "true" ? "正常" : "不正常");
        }
    }

    $('#dlg_FactoryServer').dialog('open');
    //alert(myFactoryId);
}
function DataComputerInfo(myCollectionComputerId) {
    var m_DataComputerInfoObj = $('#' + myCollectionComputerId).data("options");
    $('#Text_CollectionComputer').textbox('setValue', m_DataComputerInfoObj.Name);
    $('#Text_CollectionComputerAddress').textbox('setValue', m_DataComputerInfoObj.IpAddress);
    for (var i = 0; i < NetworkNodeArray.length; i++) {
        if (NetworkNodeArray[i]["Id"] == myCollectionComputerId) {
            $('#Text_CollectionComputerNetworkStatus').textbox('setValue', NetworkNodeArray[i]["NetworkStatus"].toLowerCase() == "true" ? "正常" : "不正常");
            $('#Text_CollectionComputerSoftStatus').textbox('setValue', NetworkNodeArray[i]["SoftwareStatus"].toLowerCase() == "true" ? "正常" : "不正常");
        }
    }
    $('#dlg_CollectionComputerServer').dialog('open');
}
function CollectorInfo(myCollectorId, myOrganizationId) {
    var m_CollectorInfoObj = $('#' + myCollectorId).data("options");
    var m_CollectorType = m_CollectorInfoObj.Type;



    if (m_CollectorType == "Ammeter") {
        $('#Text_CollectorName').textbox('setValue', m_CollectorInfoObj.Name);
        $('#Text_CollectorIpAddress').textbox('setValue', m_CollectorInfoObj.IpAddress);
        $('#Text_CollectorType').textbox('setValue', m_CollectorInfoObj.Type);
        SetAmmterStatus(myOrganizationId, m_CollectorInfoObj.Name, m_CollectorInfoObj.IpAddress);
        for (var i = 0; i < NetworkNodeArray.length; i++) {
            if (NetworkNodeArray[i]["Id"] == myCollectorId) {
                $('#Text_CollectorNetworkStatus').textbox('setValue', NetworkNodeArray[i]["NetworkStatus"].toLowerCase() == "true" ? "正常" : "不正常");
            }
        }
        $('#dlg_AmmeterStatus').dialog('open');
    }
    else if (m_CollectorType == "OPC") {
        $('#Text_OPCName').textbox('setValue', m_CollectorInfoObj.Name);
        $('#Text_OPCIpAddress').textbox('setValue', m_CollectorInfoObj.IpAddress);
        $('#Text_OPCCollectorType').textbox('setValue', m_CollectorInfoObj.Type);       
        for (var i = 0; i < NetworkNodeArray.length; i++) {
            if (NetworkNodeArray[i]["Id"] == myCollectorId) {
                $('#Text_OPCStatus').textbox('setValue', NetworkNodeArray[i]["NetworkStatus"].toLowerCase() == "true" ? "正常" : "不正常");
            }
        }
        $('#dlg_OPCStatus').dialog('open');
    }
    //alert(myFactoryId + "||||" + myCollectionComputerId + "||||" + myCollectorId);
}
function SetAmmterStatus(myOrganizationId, myCollectorName, myIpAddress) {

    $.ajax({
        type: "POST",
        url: "View_NetworkStatus.aspx/GetTermimalStatus",
        data: "{myOrganizationId:'" + myOrganizationId + "',myCollectorName:'" + myCollectorName + "',myIpAddress:'" + myIpAddress + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData != null && m_MsgData != undefined) {
                $('#grid_AmmeterStatus').datagrid('loadData', m_MsgData);
            }
        }
    });
}

///////////////////////////////////////获得状态数据///////////////////////////////////////
function GetStatusValue() {
    $.ajax({
        type: "POST",
        url: "NetworkMonitor.asmx/GetNetworkStatus",
        data: '',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData != null && m_MsgData != undefined) {
                SetStatus(m_MsgData)
            }
            setTimeout('GetStatusValue()', 10000);
        },
        error: function (msg) {
            setTimeout('GetStatusValue()', 10000);
        }
    });
}
function SetStatus(myAlarmArray) {
    var m_Status = 0;
    for (var i = 0; i < NetworkNodeArray.length; i++) {
        var m_NodeObjTemp = NetworkNodeArray[i];
        if (m_NodeObjTemp["NetworkStatus"] != null && m_NodeObjTemp["NetworkStatus"] != undefined) {      //网络状态
            if (myAlarmArray[m_NodeObjTemp["Id"]] != null && myAlarmArray[m_NodeObjTemp["Id"]] != undefined &&
                myAlarmArray[m_NodeObjTemp["Id"]]["NetworkStatus"] != null && myAlarmArray[m_NodeObjTemp["Id"]]["NetworkStatus"] != undefined)   //如果有该节点信息
            {
                m_NodeObjTemp["NetworkStatus"] = myAlarmArray[m_NodeObjTemp["Id"]]["NetworkStatus"];
            }
            else {
                NetworkNodeArray[i]["NetworkStatus"] = "true";
            }
        }
        if (m_NodeObjTemp["SynchronizationStatus"] != null && m_NodeObjTemp["SynchronizationStatus"] != undefined) {      //同步状态
            if (myAlarmArray[m_NodeObjTemp["Id"]] != null && myAlarmArray[m_NodeObjTemp["Id"]] != undefined &&
                myAlarmArray[m_NodeObjTemp["Id"]]["SynchronizationStatus"] != null && myAlarmArray[m_NodeObjTemp["Id"]]["SynchronizationStatus"] != undefined)   //如果有该节点信息
            {
                m_NodeObjTemp["SynchronizationStatus"] = myAlarmArray[m_NodeObjTemp["Id"]]["SynchronizationStatus"];
            }
            else {
                NetworkNodeArray[i]["SynchronizationStatus"] = "true";
            }
        }
        if (m_NodeObjTemp["SubNodeStatus"] != null && m_NodeObjTemp["SubNodeStatus"] != undefined) {      //下级设备状态,针对电表采集
            if (myAlarmArray[m_NodeObjTemp["Id"]] != null && myAlarmArray[m_NodeObjTemp["Id"]] != undefined &&
                myAlarmArray[m_NodeObjTemp["Id"]]["SubNodeStatus"] != null && myAlarmArray[m_NodeObjTemp["Id"]]["SubNodeStatus"] != undefined)   //如果有该节点信息
            {
                m_NodeObjTemp["SubNodeStatus"] = myAlarmArray[m_NodeObjTemp["Id"]]["SubNodeStatus"];
            }
            else {
                NetworkNodeArray[i]["SubNodeStatus"] = "true";
            }
        }
        if (m_NodeObjTemp["SoftwareStatus"] != null && m_NodeObjTemp["SoftwareStatus"] != undefined) {      //软件自身
            if (myAlarmArray[m_NodeObjTemp["Id"]] != null && myAlarmArray[m_NodeObjTemp["Id"]] != undefined &&
                myAlarmArray[m_NodeObjTemp["Id"]]["SoftwareStatus"] != null && myAlarmArray[m_NodeObjTemp["Id"]]["SoftwareStatus"] != undefined)   //如果有该节点信息
            {
                m_NodeObjTemp["SoftwareStatus"] = myAlarmArray[m_NodeObjTemp["Id"]]["SoftwareStatus"];
            }
            else {
                NetworkNodeArray[i]["SoftwareStatus"] = "true";
            }
        }
    }
    SetStatusColor();
}
function SetStatusColor() {
    for (var i = 0; i < NetworkNodeArray.length; i++) {
        var m_NodeObjTemp = NetworkNodeArray[i];
        //画网络线
        var m_DomObjList = $('td[name="' + m_NodeObjTemp["Id"] + '"]');
        if (m_DomObjList != null && m_DomObjList != undefined) {
            for (var j = 0; j < m_DomObjList.length; j++) {           //查找相同名字的每一个dom
                //var m_BackBackgroudImage = GetBackgroudImage(1, $(m_DomObjList[j]).data("options").CrossPointStyle)
                if (m_NodeObjTemp["NetworkStatus"].toLowerCase() == "true") {
                    $(m_DomObjList[j]).css('background-image', GetBackgroudImage(1, $(m_DomObjList[j]).data("options").CrossPointStyle));
                }
                else {
                    $(m_DomObjList[j]).css('background-image', GetBackgroudImage(0, $(m_DomObjList[j]).data("options").CrossPointStyle));
                }
                //$(m_DomObjList[j]).css('background-color', 'red');
            }
        }

        //画结点状态
        var m_NodeDomObj = $('#' + m_NodeObjTemp["Id"]);
        if (m_NodeDomObj != null && m_NodeDomObj != undefined) {
            var m_BackgroundColor = "transparent";
            if (m_NodeObjTemp["SynchronizationStatus"] != null && m_NodeObjTemp["SynchronizationStatus"] != undefined ) {   //同步
                if (m_NodeObjTemp["SynchronizationStatus"].toLowerCase() != "true") {
                    m_BackgroundColor = "yellow";
                }
            }
            if (m_NodeObjTemp["SubNodeStatus"] != null && m_NodeObjTemp["SubNodeStatus"] != undefined) {        //下级节点,主要针对电表
                if (m_NodeObjTemp["SubNodeStatus"].toLowerCase() != "true") {
                    m_BackgroundColor = "yellow";
                }
            }
            if (m_NodeObjTemp["SoftwareStatus"] != null && m_NodeObjTemp["SoftwareStatus"] != undefined ) {             //软件运行,优先级大的放在后面
                if (m_NodeObjTemp["SoftwareStatus"].toLowerCase() != "true") {
                    m_BackgroundColor = "red";
                }
            }
            $(m_NodeDomObj).css('background-color', m_BackgroundColor);
            if (m_BackgroundColor != "transparent") {
                $(m_NodeDomObj).css('border', "1px solid #666666");
            }
            else {
                $(m_NodeDomObj).css('border', "none");
            }

        }
    }
}
function GetBackgroudImage(myStatus, myCrossPointStyle) {
    if (myStatus == 1) {           //网络正常状态
        return 'url("images/page/Net' + myCrossPointStyle + '_Green.png")';
    }
    else if (myStatus == 0) {                            //网络异常状态
        return 'url("images/page/Net' + myCrossPointStyle + '_Red.png")';
    }
    else {
        return 'url("images/page/Net' + myCrossPointStyle + '_Green.png")';
    }
}
function loadDialog() {
    //loading 分厂节点dialog
    $('#dlg_FactoryServer').dialog({
        title: '网络信息',
        width: 450,
        height: 220,
        closed: true,
        cache: false,
        modal: true,
        buttons: [{
            text: '关闭',
            width: 80,
            handler: function () {
                $('#dlg_FactoryServer').dialog('close');
            }
        }]
    });
    //采集计算机节点dialog
    $('#dlg_CollectionComputerServer').dialog({
        title: '网络信息',
        width: 450,
        height: 170,
        closed: true,
        cache: false,
        modal: true,
        buttons: [{
            text: '关闭',
            width: 80,
            handler: function () {
                $('#dlg_CollectionComputerServer').dialog('close');
            }
        }]
    });
    //电表采集器状态
    $('#dlg_AmmeterStatus').dialog({
        title: '网络信息',
        width: 460,
        height: 500,
        closed: true,
        cache: false,
        modal: true,
        buttons: [{
            text: '关闭',
            width: 80,
            handler: function () {
                $('#dlg_AmmeterStatus').dialog('close');
            }
        }]
    });
    /////OPC状态
    $('#dlg_OPCStatus').dialog({
        title: '网络信息',
        width: 460,
        height: 170,
        closed: true,
        cache: false,
        modal: true,
        buttons: [{
            text: '关闭',
            width: 80,
            handler: function () {
                $('#dlg_OPCStatus').dialog('close');
            }
        }]
    });

}
function InitAmmeterStatusDataGrid() {
    $('#grid_AmmeterStatus').datagrid({
        title: '',
        data: { "rows": [], "total": 0 },
        dataType: "json",
        striped: true,
        //loadMsg: '',   //设置本身的提示消息为空 则就不会提示了的。这个设置很关键的
        rownumbers: true,
        singleSelect: true,
        idField: 'AmmeterNumber',
        columns: [[
        { field: 'AmmeterName', title: '名称', width: 130 },
        { field: 'AmmeterNumber', title: '电表名称', width: 60 },
        { field: 'CommPort', title: '端口', width: 40 },
        { field: 'Status', title: '状态', width: 60 },
        { field: 'TimeStatusChange', title: '更新时间', width: 110 }]],
        rowStyler: function (myIndex, myRow) {
            if (myRow.Status == "不能读取") {
                return 'background-color:red;';
            }
        }
    });
}



