var NetworkNodeArray = [];           //需要初始化的连接线
$(document).ready(function () {
    loadDialog();
    InitAmmeterStatusDataGrid();
    GetNetworkStructure();
    //ResizeNetworkStructure();
});

function ResizeNetworkStructure() {
    var m_DisplayWidth = $('#MainLayout').layout('panel', 'center').panel('options').width -20;
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
        }
    });
}

function InitializeNetworkStructure(myData) {
    $('#StatusMainTable').empty();         //清空框架表格

    var m_HtmlString = "";
    var m_GroupLineString = '<td id ="GroupTitle1" class="GroupTitle" rowspan ="' + myData.length + '">宁夏建材</td>' +
          '<td class="GroupNetwork" rowspan ="' + myData.length + '"></td>';
    for (var i = 0; i < myData.length; i++) {             //生成框架Html
        if (i == 0) {
            m_HtmlString = m_HtmlString + '<tr>' + m_GroupLineString + '<td class = "MainTableTd"><table class = "FactoryTable" id = "Table_' + myData[i]["OrganzaitonId"] + '"></table></td></tr>';
            //$('#Table_' + myData[i]["OrganzaitonId"]).css("width", 12 * 50);
        }
        else {
            m_HtmlString = m_HtmlString + '<tr><td  class = "MainTableTd"><table class = "FactoryTable" id = "Table_' + myData[i]["OrganzaitonId"] + '"></table></td></tr>';
            //$('#Table_' + myData[i]["OrganzaitonId"]).css("width", 12 * 50);
        }
        //ResizeNetworkStructure();
    }
    $('#StatusMainTable').html(m_HtmlString);

    for (var i = 0; i < myData.length; i++) {          //判断有多少个服务器
        var m_MaxExtendColumnCount = 0;                   //获得最多的一组采集器数量
        var m_ExtendColumnCount = 0;
        for (var j = 0; j < myData[i]["DataComputer"].length; j++) {
            if (myData[i]["DataComputer"][j]["Switch"]["CollectorCount"] != null && myData[i]["DataComputer"][j]["Switch"]["CollectorCount"] != undefined) {
                var m_CollectorCount = parseInt(myData[i]["DataComputer"][j]["Switch"]["CollectorCount"]);
                if (j == myData[i]["DataComputer"].length - 1) {             //当最后一行
                    //前面最多有11个空余位置
                    if (m_CollectorCount > 22) {
                        m_ExtendColumnCount = m_CollectorCount - 11;         //当大于12个格的情况,需要向右边补格
                    }
                    else {
                        m_ExtendColumnCount = parseInt(m_CollectorCount / 2);
                    }

                    if (m_ExtendColumnCount > m_MaxExtendColumnCount) {
                        m_MaxExtendColumnCount = m_ExtendColumnCount;
                    }
                }
                else {
                    //前面最多有6个空余位置
                    if (m_CollectorCount > 12) {
                        m_ExtendColumnCount = m_CollectorCount - 6;         //当大于12个格的情况,需要向右边补格
                    }
                    else {
                        m_ExtendColumnCount = parseInt(m_CollectorCount / 2);
                    }
                    if (m_ExtendColumnCount > m_MaxExtendColumnCount) {
                        m_MaxExtendColumnCount = m_ExtendColumnCount;
                    }
                }
            }
        }

        InitializeServerNode(myData[i], m_MaxExtendColumnCount);
    }
}
function InitializeServerNode(myData, myExtendColumnCount) {
    $('#Table_' + myData["OrganzaitonId"]).empty();         //清空框架表格
    var m_HtmlString = "";
    var m_ExtendColumnHtml = "";
    for (var i = 0; i < myExtendColumnCount; i++) {
        m_ExtendColumnHtml = m_ExtendColumnHtml + '<td class = "Network"></td>';
    }
    if (myData["DataComputer"] != null) {
        ///////////////判断数采计算机位置///////////////
        var m_FactoryServerRowIndex = 0;                     //表示与第几个数据采集计算机在一条线上
        var m_CrossPointStyle = 1;
        NetworkNodeArray.push(myData["Id"]);          //设置默认状态线
        if (myData["DataComputer"].length == 1) {                  //当只有一个数据采集计算机时,没有交叉线
            m_CrossPointStyle = 1;
            m_FactoryServerRowIndex = 0;
        }
        else if (myData["DataComputer"].length == 2) {                  //当只有2个数据采集计算机时,显示上交叉线
            m_CrossPointStyle = 4;
            m_FactoryServerRowIndex = 1;
        }
        else {                                              //当大于一个数据采集计算机时,显示十字交叉线
            m_CrossPointStyle = 3;
            m_FactoryServerRowIndex = parseInt(myData["DataComputer"].length / 2);
        }
        ////////////////////////生成每个数采计算机Html////////////////////////
        for (var j = 0; j < myData["DataComputer"].length; j++) {              //统计有多少个数采计算机
            NetworkNodeArray.push(myData["DataComputer"][j]["Id"]);          //设置默认状态线
            m_HtmlString = m_HtmlString + '<tr>';
            ///////////////////////生成服务器Html///////////////////////
            if (m_FactoryServerRowIndex == j)              //如果该数采计算机与分厂服务器在同一行
            {
                m_HtmlString = m_HtmlString + '<td class = "Network" name = "' + myData["Id"] + '" data-options=\'{"CrossPointStyle":"1"}\'></td>';    //横线
                m_HtmlString = m_HtmlString + '<td class = "Network" name = "' + myData["Id"] + '" data-options=\'{"CrossPointStyle":"1"}\'></td>';    //横线
                m_HtmlString = m_HtmlString + '<td id = "' + myData["Id"] +
                               '" class = "FactoryServer" data-options=\'{"Name":"' + myData["Name"] +
                               '", "IpAddress": "' + myData["IpAddress"] +
                               '", "OrganzaitonId": "' + myData["OrganzaitonId"] +
                               '", "PropertyName": "' + myData["PropertyName"] +
                               '"}\' colspan = 2>' + myData["Name"] + '</td>';
            }
            else {
                m_HtmlString = m_HtmlString + '<td class = "Network"></td>';    //长横线
                m_HtmlString = m_HtmlString + '<td class = "Network"></td>';    //长横线
                m_HtmlString = m_HtmlString + '<td class = "Network"></td>';    //长横线
                m_HtmlString = m_HtmlString + '<td class = "Network"></td>';    //长横线
            }
            ////////////////////////生成数采计算机和下面一级交换机Html//////////////////////////
            if (m_FactoryServerRowIndex == j)              //如果该数采计算机与分厂服务器在同一行
            {
                m_HtmlString = m_HtmlString + '<td class = "Network" name = "' + myData["DataComputer"][j]["Id"] + '" data-options=\'{"CrossPointStyle":"1"}\'></td>';    //长横线
                m_HtmlString = m_HtmlString + '<td class = "Network" name = "' + myData["DataComputer"][j]["Id"] + '" data-options=\'{"CrossPointStyle":"' + m_CrossPointStyle + '"}\'></td>';  //交叉线
                m_HtmlString = m_HtmlString + '<td class = "Network" name = "' + myData["DataComputer"][j]["Id"] + '" data-options=\'{"CrossPointStyle":"1"}\'></td>';    //长横线
            }
            else if (j == 0)        //第一行并且是非服务器所在行
            {
                m_HtmlString = m_HtmlString + '<td class = "Network"></td>';    //长横线
                m_HtmlString = m_HtmlString + '<td class = "Network" name = "' + myData["DataComputer"][j]["Id"] + '" data-options=\'{"CrossPointStyle":"8"}\'></td>';  //交叉线
                m_HtmlString = m_HtmlString + '<td class = "Network" name = "' + myData["DataComputer"][j]["Id"] + '" data-options=\'{"CrossPointStyle":"1"}\'></td>';    //长横线
            }
            else if (j == myData["DataComputer"].length - 1) {                              //随后一行并且是非服务器所在行
                m_HtmlString = m_HtmlString + '<td class = "Network"></td>';    //长横线
                m_HtmlString = m_HtmlString + '<td class = "Network" name = "' + myData["DataComputer"][j]["Id"] + '" data-options=\'{"CrossPointStyle":"10"}\'></td>';  //交叉线
                m_HtmlString = m_HtmlString + '<td class = "Network" name = "' + myData["DataComputer"][j]["Id"] + '" data-options=\'{"CrossPointStyle":"1"}\'></td>';    //长横线
            }
            else {
                m_HtmlString = m_HtmlString + '<td class = "Network"></td>';    //长横线
                m_HtmlString = m_HtmlString + '<td class = "Network" name = "' + myData["DataComputer"][j]["Id"] + '" data-options=\'{"CrossPointStyle":"7"}\'></td>';  //交叉线
                m_HtmlString = m_HtmlString + '<td class = "Network" name = "' + myData["DataComputer"][j]["Id"] + '" data-options=\'{"CrossPointStyle":"1"}\'></td>';    //长横线
            }
            m_HtmlString = m_HtmlString + '<td id = "' + myData["DataComputer"][j]["Id"] +
                           '" class = "DataComputer" data-options=\'{"Name":"' + myData["DataComputer"][j]["Name"] +
                           '", "IpAddress": "' + myData["DataComputer"][j]["IpAddress"] +
                           '", "PropertyName": "' + myData["DataComputer"][j]["PropertyName"] +
                           '"}\' colspan = 2>' + myData["DataComputer"][j]["Name"] + '</td>';
            ///第一级交换机
            NetworkNodeArray.push(myData["DataComputer"][j]["Switch"]["Id"]);          //设置默认状态线
            m_HtmlString = m_HtmlString + '<td class = "Network" name = "' + myData["DataComputer"][j]["Switch"]["Id"] + '" data-options=\'{"CrossPointStyle":"1"}\'></td>';    //长横线
            m_HtmlString = m_HtmlString + '<td class = "Network" name = "' + myData["DataComputer"][j]["Switch"]["Id"] + '" data-options=\'{"CrossPointStyle":"1"}\'></td>';    //长横线
            var m_SwitchName = myData["DataComputer"][j]["Switch"]["Name"];
            if (m_SwitchName.length > 8)
            {
                m_SwitchName = m_SwitchName.substring(0, 7) + '...';
            }
            else if (m_SwitchName.length < 5)
            {
                m_SwitchName = m_SwitchName + "&nbsp;&nbsp;&nbsp;&nbsp;";
            }
            m_HtmlString = m_HtmlString + '<td id = "' + myData["DataComputer"][j]["Switch"]["Id"] +
                       '" class = "Switch" data-options=\'{"Name":"' + myData["DataComputer"][j]["Switch"]["Name"] +
                       '", "PropertyName": "' + myData["DataComputer"][j]["Switch"]["PropertyName"] +
                       '"}\'>' + m_SwitchName + '</td>';
            m_HtmlString = m_HtmlString + m_ExtendColumnHtml + '</tr>';
            ///////////////////////////////////////////////////////////////////////////////////////
            ////////////////////////找下级交换机以及数采装置///////////////////////
            if (j != myData["DataComputer"].length - 1) {   //非最后一个数采计算机
                m_HtmlString = m_HtmlString + GetSubSwitchHtmlString(myData["DataComputer"][j]["Switch"], myExtendColumnCount, myData["DataComputer"][j]["Id"], false);
            }
            else {
                m_HtmlString = m_HtmlString + GetSubSwitchHtmlString(myData["DataComputer"][j]["Switch"], myExtendColumnCount, myData["DataComputer"][j]["Id"], true);
            }
            
        }
    }
    $('#Table_' + myData["OrganzaitonId"]).html(m_HtmlString);
    LoadStatusDefaultColor(NetworkNodeArray);
}
function GetSubSwitchHtmlString(mySwitch, myExtendColumnCount, myDataComputerId, myLastDataComputer) {
    var m_DefaultHtmlString = '<td class = "Network"></td><td class = "Network"></td><td class = "Network"></td>' +
                               '<td class = "Network"></td><td class = "Network"></td>' +                                      //长横线;
                               '<td class = "Network" name = "' + myDataComputerId + '" data-options=\'{"CrossPointStyle":"2"}\'></td>';  //交叉线
    var m_DefaultLastRowHtmlString = '<td class = "Network"></td>';                                    //长横线;
    var m_HtmlString = "";
    var m_ExtendColumnHtml = "";
    var m_RowArray = [];
    for (var i = 0; i < mySwitch["MaxDepth"] * 2; i++) {
        var m_ColumnArray = [];
        if (myLastDataComputer == true) {
            for (var j = 0; j < 11 + myExtendColumnCount; j++) {
                m_ColumnArray.push([]);
            }
        }
        else {
            for (var j = 0; j < 6 + myExtendColumnCount; j++) {
                m_ColumnArray.push([]);
            }
        }
        m_RowArray.push(m_ColumnArray);
    }
    ///////////////////////解析网络结构////////////////////
    if(myLastDataComputer == false)        //不是最后一个数采计算机
    {
        var m_FirstNodeColumnIndex = GetFristColumnIndexSubZone(mySwitch, 0, myLastDataComputer);
        var m_NodeArray = GetSubNodeInfo(mySwitch, 0, m_FirstNodeColumnIndex, 5, myLastDataComputer);           //第一个交换机默认位置都是6
        for (var i = 0; i < m_NodeArray.length; i++) {
            m_RowArray[m_NodeArray[i]["Y"]][m_NodeArray[i]["X"]] = { "NodeName": m_NodeArray[i]["NodeName"], "NodeType": m_NodeArray[i]["NodeType"], "CrossPointStyle": m_NodeArray[i]["CrossPointStyle"] };
        }
    }
    else
    {
        var m_FirstNodeColumnIndex = GetFristColumnIndexSubZone(mySwitch, 0, myLastDataComputer);
        var m_NodeArray = GetSubNodeInfo(mySwitch, 0, m_FirstNodeColumnIndex, 10, myLastDataComputer);           //第一个交换机默认位置都是11
        for (var i = 0; i < m_NodeArray.length; i++) {
            m_RowArray[m_NodeArray[i]["Y"]][m_NodeArray[i]["X"]] = { "NodeName": m_NodeArray[i]["NodeName"], "NodeType": m_NodeArray[i]["NodeType"], "CrossPointStyle": m_NodeArray[i]["CrossPointStyle"] };
        }
    }
    ///////////////////////生成Html////////////////////////
    for (var i = 0; i < m_RowArray.length; i++) {            //生成Html
        if (myLastDataComputer == true) {
            m_HtmlString = m_HtmlString + '<tr>' + m_DefaultLastRowHtmlString;
        }
        else {
            m_HtmlString = m_HtmlString + '<tr>' + m_DefaultHtmlString;
        }
        for (var j = 0; j < m_RowArray[i].length; j++) {
            if (m_RowArray[i][j]["NodeName"] != null && m_RowArray[i][j]["NodeName"] != undefined) {
                if (m_RowArray[i][j]["NodeType"] == "Network") {
                    m_HtmlString = m_HtmlString + '<td class = "Network" name = "' + m_RowArray[i][j]["NodeName"] + '" data-options=\'{"CrossPointStyle":"' + m_RowArray[i][j]["CrossPointStyle"] + '"}\'></td>';
                }
                else {
                    var m_CollectorName = m_RowArray[i][j]["NodeName"];
                    var m_StringLength = GetByteLen(m_CollectorName);
                    if (m_StringLength > 16) {
                        m_CollectorName = m_CollectorName.substring(0, 7) + '...';
                    }
                    else if (m_StringLength < 8) {
                        m_CollectorName = m_CollectorName + "&nbsp;&nbsp;";
                    }
                    m_HtmlString = m_HtmlString + '<td id = "' + m_RowArray[i][j]["NodeId"] + '" class = "' + m_RowArray[i][j]["NodeType"] +
                      '" data-options=\'' + m_RowArray[i][j]["data-options"] + '\'>' + m_CollectorName + '</td>'
                }
            }
            else {
                m_HtmlString = m_HtmlString + '<td class = "Network"></td>';
            }
        }
        m_HtmlString = m_HtmlString + '</tr>';
    }
    return m_HtmlString;
}
function GetFristColumnIndexSubZone(mySwitch, myFirstNodeColumnIndex, myLastDataComputer) {
    var m_FristNodeColumnIndex = 0;
    var m_NodeCount = mySwitch["CollectorCount"];           //用来计算初始位置
    ////////////////////////////得到树形结构的列范围////////////////////////
    if (myLastDataComputer == true)           //最后一个数采计算机
    {
        if (m_NodeCount > 22) {
            m_FristNodeColumnIndex = myFirstNodeColumnIndex;
        }
        else {
            m_FristNodeColumnIndex = myFirstNodeColumnIndex + (10 - parseInt(m_NodeCount / 2));
        }
    }
    else {                                   //其它计算机
        if (m_NodeCount > 12) {
            m_FristNodeColumnIndex = myFirstNodeColumnIndex;
        }
        else {
            m_FristNodeColumnIndex = myFirstNodeColumnIndex + (5 - parseInt(m_NodeCount / 2));
        }
    }
    return m_FristNodeColumnIndex;
}
function GetSubNodeInfo(mySwitch, myFirstNodeRowIndex, myFirstNodeColumnIndex, myParentNodeColumnIndex, myLastDataComputer) {
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
            if (m_CollectorCount == 1) {
                m_NodeArray.push({ "NodeName": mySwitch["Collector"][i]["Id"], "NodeType": "Network", "CrossPointStyle": "2", "X": myParentNodeColumnIndex, "Y": m_CurrentNodeRowIndex });
            }
            else if (i == 0) {       //如果只有一个节点
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



            NetworkNodeArray.push(mySwitch["Collector"][i]["Id"]);          //设置默认状态线
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
            NetworkNodeArray.push(mySwitch["Switch"][i]["Id"]);          //设置默认状态线

            var m_SubNodeArray = GetSubNodeInfo(mySwitch["Switch"][i], m_CurrentNodeRowIndex + 2, m_CurrentNodeColumnIndex, m_CurrentNodeColumnIndex + m_SwitchNodeOffsetX, myLastDataComputer);
            if (m_SubNodeArray != null && m_SubNodeArray != undefined) {
                for (var j = 0; j < m_SubNodeArray.length; j++) {
                    m_NodeArray.push(m_SubNodeArray[j]);
                }
            }
                

            m_CurrentNodeColumnIndex = m_CurrentNodeColumnIndex + mySwitch["Switch"][i]["CollectorCount"];
        }
    }
    return m_NodeArray;
    /*
    
    if (m_CollectorCount > 0) {           //交换机下有叶子节点
        m_CurrentColumnIndex = parseInt(m_CollectorCount / 2) + m_CurrentColumnIndex;
        m_CrossPointArray.push([m_CurrentColumnIndex,m_CollectorCount]);       
    }
    for (var i = 0; i < mySwitch["Switch"].length; i++) {
        m_CollectorCount = mySwitch["Switch"][i]["CollectorCount"];
        m_CurrentColumnIndex = parseInt(m_CollectorCount / 2) + m_CurrentColumnIndex;
        m_CrossPointArray.push([m_CurrentColumnIndex,m_CollectorCount]);
    }
    if (m_CrossPointArray.length > 0) {
        for (var i = 0; i <= (m_CrossPointArray[m_CrossPointArray.length - 1][0] - m_CrossPointArray[0][0]) ; i++) {
            for (var j = 0; j < m_CrossPointArray.length; j++) {
                if(m_CrossPointArray[j] == 
            }
        }
    }

    for (var i = 0; i < mySwitch["Collector"].length; i++) {
        if (m_MaxDepth == 1) {
            if (i == 0) {

            }

            m_NodeArray.push({ "NodeName": mySwitch["Id"], "NodeType": "NetLine", "CrossPointStyle": "2", "X": myParentNodeColumnIndex + 5, "Y": 0 });
        }
        else {          //如果深度大于1,则为竖线
            m_NodeArray.push({ "NodeName": mySwitch["Id"], "NodeType": "NetLine", "CrossPointStyle": "2", "X": myParentNodeColumnIndex + 5, "Y": 0 });
        }
    }
    if (mySwitch["Switch"].length > 0) {

    }






    if (mySwitch["Collector"].length > 0) {                //如果该级节点有直接叶子节点,则视为一个子节点
        m_NodeCount = m_NodeCount + mySwitch["Switch"].length + 1;
    }
    else {
        m_NodeCount = m_NodeCount + mySwitch["Switch"].length;
    }
    if (m_NodeCount == 1) {              //根据分支个数选择网络线
        if (myLastDataComputer == false) {
            m_NodeArray.push({ "NodeName": mySwitch["Id"], "NodeType": "NetLine", "CrossPointStyle": "2", "X": myParentNodeColumnIndex, "Y": 0 });
        }
        else {
            m_NodeArray.push({ "NodeName": mySwitch["Id"], "NodeType": "NetLine", "CrossPointStyle": "2", "X": myParentNodeColumnIndex + 5, "Y": 0 });
        }
    }
    else if (m_NodeCount == 2) {
        if (myLastDataComputer == false) {
            m_NodeArray.push({ "NodeName": mySwitch["Id"], "NodeType": "NetLine", "CrossPointStyle": "2", "X": myParentNodeColumnIndex, "Y": 0 });
        }
        else {
            m_NodeArray.push({ "NodeName": mySwitch["Id"], "NodeType": "NetLine", "CrossPointStyle": "2", "X": myParentNodeColumnIndex + 5, "Y": 0 });
        }
    }
    else {

    }
        */
}
function LoadStatusDefaultColor(myNetworkNodeArray) {
    for (var i = 0; i < myNetworkNodeArray.length; i++) {
        var m_DomObjList = $('td[name="' + myNetworkNodeArray[i] + '"]');
        if (m_DomObjList != null && m_DomObjList != undefined) {
            for (var j = 0; j < m_DomObjList.length; j++) {           //查找相同名字的每一个dom
                var aa = GetBackgroudImage(1, $(m_DomObjList[j]).data("options").CrossPointStyle)
                $(m_DomObjList[j]).css('background-image', GetBackgroudImage(1, $(m_DomObjList[j]).data("options").CrossPointStyle));
                //$(m_DomObjList[j]).css('background-color', 'red');
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




////////////////////////////////////////////////////////////////////////////////////
function GetNodeArray(myData) {
    var m_OrganizationId = "";
    var m_CollectionComputerName = "";
    var m_CollectorName = "";
    var m_CollectionComputerCount = 0;
    for (var i = 0; i < myData.rows.length; i++) {
        if (m_OrganizationId != myData.rows[i]["OrganizationID"]) {
            m_OrganizationId = myData.rows[i]["OrganizationID"];
            m_CollectionComputerName = myData.rows[i]["CollectionComputer"];
            m_CollectorName = myData.rows[i]["CollectionName"] + myData.rows[i]["IpAddress"];
            NodeArray[m_OrganizationId] = {};
            NodeArray[m_OrganizationId][m_CollectionComputerName] = {};
            NodeArray[m_OrganizationId][m_CollectionComputerName][m_CollectorName] = [];
        }
        else if (m_CollectionComputerName != myData.rows[i]["CollectionComputer"]) {
            m_CollectionComputerName = myData.rows[i]["CollectionComputer"];
            m_CollectorName = myData.rows[i]["CollectionName"] + myData.rows[i]["IpAddress"];
            NodeArray[m_OrganizationId][m_CollectionComputerName] = {};
            NodeArray[m_OrganizationId][m_CollectionComputerName][m_CollectorName] = [];
        }
        else {
            m_CollectorName = myData.rows[i]["CollectionName"] + myData.rows[i]["IpAddress"];
            NodeArray[m_OrganizationId][m_CollectionComputerName][m_CollectorName] = [];
        }
        if (myData.rows[i]["FactoryLine"] == "True") {
            NodeArray[m_OrganizationId]["FactoryAddress"] = myData.rows[i]["FactoryAddress"];
            NodeArray[m_OrganizationId]["FactoryName"] = myData.rows[i]["FactoryName"];
            NodeArray[m_OrganizationId]["RowIndex"] = i;
            NodeArray[m_OrganizationId]["NetStatus"] = true;         //分厂到集团端网络
            NodeArray[m_OrganizationId]["SynchronizationStatus"] = true;    //同步状态
            NodeArray[m_OrganizationId]["NodeSoftStatus"] = true;                 //汇总软件状态
        }
        if (myData.rows[i]["CollectionLine"] == "True") {
            NodeArray[m_OrganizationId][m_CollectionComputerName]["CollectionComputerAddress"] = myData.rows[i]["CollectionComputerAddress"];
            NodeArray[m_OrganizationId][m_CollectionComputerName]["CollectionName"] = myData.rows[i]["CollectionComputer"];
            NodeArray[m_OrganizationId][m_CollectionComputerName]["RowIndex"] = i;
            NodeArray[m_OrganizationId][m_CollectionComputerName]["NetStatus"] = true;   //数采计算机到分厂服务器端网路
            NodeArray[m_OrganizationId][m_CollectionComputerName]["NodeSoftStatus"] = true;    //数采软件状态
        }
        NodeArray[m_OrganizationId][m_CollectionComputerName][m_CollectorName]["IpAddress"] = myData.rows[i]["IpAddress"];
        NodeArray[m_OrganizationId][m_CollectionComputerName][m_CollectorName]["OPCAddress"] = myData.rows[i]["OPCAddress"];
        NodeArray[m_OrganizationId][m_CollectionComputerName][m_CollectorName]["CollectorName"] = myData.rows[i]["CollectionName"];
        NodeArray[m_OrganizationId][m_CollectionComputerName][m_CollectorName]["CollectionType"] = myData.rows[i]["CollectionType"];
        NodeArray[m_OrganizationId][m_CollectionComputerName][m_CollectorName]["RowIndex"] = i;
        NodeArray[m_OrganizationId][m_CollectionComputerName][m_CollectorName]["NetStatus"] = true;   //采集终端到数采计算机状态
        NodeArray[m_OrganizationId][m_CollectionComputerName][m_CollectorName]["NodeSoftStatus"] = true;    //电表状态集合(只要有一块电表读不上来,就人为有问题)
    }
}
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
                SetNetworkArray(m_MsgData)
            }
        }
    });
}
function SetNetworkArray(myData) {

    ////////////////////初始化网络数组,默认为所有都不正常
    $.each(NodeArray, function (myFactoryKey, myFactoryValue) {             //遍历分厂服务器       
        myFactoryValue["NetStatus"] = true;         //分厂到集团端网络
        myFactoryValue["SynchronizationStatus"] = true;    //同步状态
        myFactoryValue["NodeSoftStatus"] = true;                 //汇总软件状态
        /////////////遍历数据采集计算机
        $.each(myFactoryValue, function (myCollectionComputerKey, myCollectionComputerValue) {          //遍历采集服务器
            if (myCollectionComputerValue instanceof Object) {      //必须是对象  
                myCollectionComputerValue["NetStatus"] = true;   //数采计算机到分厂服务器端网路
                myCollectionComputerValue["NodeSoftStatus"] = false;    //数采软件状态
                /////遍历数据采集器(串口服务器或者OPC)
                $.each(myCollectionComputerValue, function (myCollectorKey, myCollectorValue) {       //遍历采集器
                    if (myCollectorValue instanceof Object) {      //必须是对象
                        myCollectorValue["NetStatus"] = false;   //采集终端到数采计算机状态
                        myCollectorValue["NodeSoftStatus"] = false;    //电表状态集合(只要有一块电表读不上来,就人为有问题)
                    }
                });
            }
        });      
    });
    ///////////////////加载数据采集器和网络状态/////////////////////
    for (var i = 0; i < myData["CollectorNodeAndLineStatus"]["rows"].length; i++) {
        if (myData["CollectorNodeAndLineStatus"]["rows"][i]["NetworkStatus"] == "True") {
            if (NodeArray[myData["CollectorNodeAndLineStatus"]["rows"][i]["OrganizationID"]][myData["CollectorNodeAndLineStatus"]["rows"][i]["CollectionComputer"]][myData["CollectorNodeAndLineStatus"]["rows"][i]["CollectionName"] + myData["CollectorNodeAndLineStatus"]["rows"][i]["IpAddress"]]["NetStatus"] != undefined &&
                NodeArray[myData["CollectorNodeAndLineStatus"]["rows"][i]["OrganizationID"]][myData["CollectorNodeAndLineStatus"]["rows"][i]["CollectionComputer"]][myData["CollectorNodeAndLineStatus"]["rows"][i]["CollectionName"] + myData["CollectorNodeAndLineStatus"]["rows"][i]["IpAddress"]]["NetStatus"] != null) {
                NodeArray[myData["CollectorNodeAndLineStatus"]["rows"][i]["OrganizationID"]][myData["CollectorNodeAndLineStatus"]["rows"][i]["CollectionComputer"]][myData["CollectorNodeAndLineStatus"]["rows"][i]["CollectionName"] + myData["CollectorNodeAndLineStatus"]["rows"][i]["IpAddress"]]["NetStatus"] = true;
            }
        }
        if (myData["CollectorNodeAndLineStatus"]["rows"][i]["Status"] == "True") {
            if (NodeArray[myData["CollectorNodeAndLineStatus"]["rows"][i]["OrganizationID"]][myData["CollectorNodeAndLineStatus"]["rows"][i]["CollectionComputer"]][myData["CollectorNodeAndLineStatus"]["rows"][i]["CollectionName"] + myData["CollectorNodeAndLineStatus"]["rows"][i]["IpAddress"]]["NodeSoftStatus"] != undefined &&
                NodeArray[myData["CollectorNodeAndLineStatus"]["rows"][i]["OrganizationID"]][myData["CollectorNodeAndLineStatus"]["rows"][i]["CollectionComputer"]][myData["CollectorNodeAndLineStatus"]["rows"][i]["CollectionName"] + myData["CollectorNodeAndLineStatus"]["rows"][i]["IpAddress"]]["NodeSoftStatus"] != null) {
                NodeArray[myData["CollectorNodeAndLineStatus"]["rows"][i]["OrganizationID"]][myData["CollectorNodeAndLineStatus"]["rows"][i]["CollectionComputer"]][myData["CollectorNodeAndLineStatus"]["rows"][i]["CollectionName"] + myData["CollectorNodeAndLineStatus"]["rows"][i]["IpAddress"]]["NodeSoftStatus"] = true;
            }
        }
    }
    ///////////////////加载采集计算机状态
    for (var i = 0; i < myData["CollectionComputerNodeStatus"]["rows"].length; i++) {
        if (myData["CollectionComputerNodeStatus"]["rows"][i]["Status"] == "True") {
            if (NodeArray[myData["CollectionComputerNodeStatus"]["rows"][i]["OrganizationID"]][myData["CollectionComputerNodeStatus"]["rows"][i]["CollectionComputer"]]["NodeSoftStatus"] != undefined &&
                NodeArray[myData["CollectionComputerNodeStatus"]["rows"][i]["OrganizationID"]][myData["CollectionComputerNodeStatus"]["rows"][i]["CollectionComputer"]]["NodeSoftStatus"] != null) {
                NodeArray[myData["CollectionComputerNodeStatus"]["rows"][i]["OrganizationID"]][myData["CollectionComputerNodeStatus"]["rows"][i]["CollectionComputer"]]["NodeSoftStatus"] = true;
            }
        }
    }
    //////////////////加载数采计算机到分厂的网络状态
    for (var i = 0; i < myData["CollectionComputerLineStatus"]["rows"].length; i++) {
        NodeArray[myData["CollectionComputerLineStatus"]["rows"][i]["OrganizationID"]][myData["CollectionComputerLineStatus"]["rows"][i]["CollectionComputer"]]["NetStatus"] = myData["FactoryNodeStatus"]["rows"][i]["NetworkStatus"] == "True" ? true : false;
    }
    //////////////////加载分厂到集团网络,软件运行状态和同步状态
    for (var i = 0; i < myData["FactoryNodeStatus"]["rows"].length; i++) {
        NodeArray[myData["FactoryNodeStatus"]["rows"][i]["OrganizationID"]]["NetStatus"] = myData["FactoryNodeStatus"]["rows"][i]["NetworkStatus"] == "True" ? true : false;
        NodeArray[myData["FactoryNodeStatus"]["rows"][i]["OrganizationID"]]["NodeSoftStatus"] = myData["FactoryNodeStatus"]["rows"][i]["Status"] == "True" ? true : false;
        NodeArray[myData["FactoryNodeStatus"]["rows"][i]["OrganizationID"]]["SynchronizationStatus"] = myData["FactoryNodeStatus"]["rows"][i]["SynchronizationStatus"] == "True" ? true : false;
    }
    SetNetworkStatus();
}
function SetNetworkStatus() {
    var m_CollectionComputerKey = "";
    var m_CollectorKey = "";

    var m_RowIndex = 0;
    var m_LastFactoryRowIndex = -1;
    var m_LastCollectionComputerRowIndex = -1;
    var m_LastCollectorRowIndex = -1;
    var m_FirstCollectionComputerFlag = true;
    var m_FirstCollectorFlag = true;

    var m_CollectionAllStatusFlag = true;
    var m_CollectorAllStatusFlag = true;
    $.each(NodeArray, function (myFactoryKey, myFactoryValue) {             //遍历分厂服务器
        ////////////////生成分厂一级状态
        m_CollectionAllStatusFlag = true;
        m_CollectorAllStatusFlag = true;
        m_RowIndex = NodeArray[myFactoryKey]["RowIndex"];
        m_LastFactoryRowIndex = m_RowIndex;
        if (myFactoryValue["NetStatus"] == true) {
            $('#FactoryNetworkLine_' + m_RowIndex).css("background-image", "url('images/page/LineG1.png')");
        }
        else {
            $('#FactoryNetworkLine_' + m_RowIndex).css("background-image", "url('images/page/LineR1.png')");
        }  
        if (myFactoryValue["SynchronizationStatus"] == true && myFactoryValue["NodeSoftStatus"] == true) {
            $('#FactoryNodeTitle_' + m_RowIndex).css("color", "black");
        }
        else {
            $('#FactoryNodeTitle_' + m_RowIndex).css("color", "red");
        }
        $('#FactoryNodeTitle_' + m_RowIndex).css("background-image", "url('images/page/Title1.png')");
        m_LastCollectionComputerRowIndex = -1;
        m_FirstCollectionComputerFlag = true;
        m_CollectionAllStatusFlag = true;
        /////////////遍历
        $.each(myFactoryValue, function (myCollectionComputerKey, myCollectionComputerValue) {          //遍历采集服务器
            if (myCollectionComputerValue instanceof Object) {      //必须是对象
                m_CollectionComputerKey = myCollectionComputerKey;
                ///////////生成数采计算机一级状态
                m_RowIndex = myFactoryValue[myCollectionComputerKey]["RowIndex"];
                if (myCollectionComputerValue["NetStatus"] == true) {
                    $('#CollectionComputerNetworkNodeLine_' + m_RowIndex).css("background-image", "url('images/page/LineG1.png')");
                }
                else {
                    m_CollectionAllStatusFlag = false;
                    $('#CollectionComputerNetworkNodeLine_' + m_RowIndex).css("background-image", "url('images/page/LineR1.png')");
                }
                if (myCollectionComputerValue["NodeSoftStatus"] == true) {
                    $('#CollectionNodeTitle_' + m_RowIndex).css("color", "black");
                }
                else {
                    $('#CollectionNodeTitle_' + m_RowIndex).css("color", "red");
                }
                $('#CollectionNodeTitle_' + m_RowIndex).css("background-image", "url('images/page/Title1.png')");
                if (m_LastCollectionComputerRowIndex == -1) {       //当第一个分叉,即分叉顶部
                    if (myCollectionComputerValue["NetStatus"] == true) {
                        $('#CollectionComputerNetworkNode_' + m_RowIndex).css("background-image", "url('images/page/LineG4.png')");
                    }
                    else {
                        $('#CollectionComputerNetworkNode_' + m_RowIndex).css("background-image", "url('images/page/LineR4.png')");
                    }
                }
                else {
                    //if (m_FirstCollectionComputerFlag != true) {
                        if (myCollectionComputerValue["NetStatus"] == true) {       //处于中间的分叉节点
                            $('#CollectionComputerNetworkNode_' + m_RowIndex).css("background-image", "url('images/page/LineG3.png')");
                        }
                        else {
                            $('#CollectionComputerNetworkNode_' + m_RowIndex).css("background-image", "url('images/page/LineR3.png')");
                        }
                    //}
                   // else {
                        m_FirstCollectionComputerFlag = false;
                   // }
                    for (var i = m_LastCollectionComputerRowIndex + 1; i < m_RowIndex; i++) {
                        if (m_LastFactoryRowIndex != i) {
                            $('#CollectionComputerNetworkNode_' + i).css("background-image", "url('images/page/LineG2.png')");
                        }
                        else {
                            if (m_CollectionAllStatusFlag == true) {              //分厂汇集节点,根据总状态来确认
                                $('#CollectionComputerNetworkLine_' + i).css("background-image", "url('images/page/LineG1.png')");
                                $('#CollectionComputerNetworkNode_' + i).css("background-image", "url('images/page/LineG3.png')");
                            }
                            else {
                                $('#CollectionComputerNetworkLine_' + i).css("background-image", "url('images/page/LineR1.png')");
                                $('#CollectionComputerNetworkNode_' + i).css("background-image", "url('images/page/LineR3.png')");
                            }
                        }
                    }
                }
                m_LastCollectionComputerRowIndex = m_RowIndex;
                /////////////遍历
                m_FirstCollectorFlag = true;
                m_LastCollectorRowIndex = -1;
                m_CollectorAllStatusFlag = true;
                $.each(myCollectionComputerValue, function (myCollectorKey, myCollectorValue) {       //遍历采集器
                    if (myCollectorValue instanceof Object) {      //必须是对象
                        m_CollectorKey = myCollectorKey;
                        m_RowIndex = myCollectionComputerValue[myCollectorKey]["RowIndex"];
                        if (myCollectorValue["NetStatus"] == true) {
                            $('#CollectorNetworkNodeLine_' + m_RowIndex).css("background-image", "url('images/page/LineG1.png')");
                        }
                        else {
                            m_CollectorAllStatusFlag = false;
                            $('#CollectorNetworkNodeLine_' + m_RowIndex).css("background-image", "url('images/page/LineR1.png')");
                        }
                        if (myCollectorValue["NodeSoftStatus"] == true) {
                            $('#CollectorNodeTitle_' + m_RowIndex).css("color", "black");
                        }
                        else {
                            $('#CollectorNodeTitle_' + m_RowIndex).css("color", "red");
                        }
                        $('#CollectorNodeTitle_' + m_RowIndex).css("background-image", "url('images/page/Title1.png')");
                        if (m_LastCollectorRowIndex == -1) {
                            if (myCollectorValue["NetStatus"] == true) {
                                $('#CollectorNetworkNode_' + m_RowIndex).css("background-image", "url('images/page/LineG4.png')");
                            }
                            else {
                                $('#CollectorNetworkNode_' + m_RowIndex).css("background-image", "url('images/page/LineR4.png')");
                            }
                        }
                        else {
                            //if (m_FirstCollectorFlag != true) {         //处于中间分叉点
                                if (myCollectorValue["NetStatus"] == true) {
                                    $('#CollectorNetworkNode_' + m_RowIndex).css("background-image", "url('images/page/LineG3.png')");
                                }
                                else
                                {
                                    $('#CollectorNetworkNode_' + m_RowIndex).css("background-image", "url('images/page/LineR3.png')");
                                }
                            //}
                            //else {
                                m_FirstCollectorFlag = false;
                            //}
                            for (var i = m_LastCollectorRowIndex + 1; i < m_RowIndex; i++) {
                                $('#CollectorNetworkNode_' + i).css("background-image", "url('images/page/LineG3.png')");
                            }
                        }
                        m_LastCollectorRowIndex = m_RowIndex;
                    }
                });
                if (m_CollectorAllStatusFlag == true) {
                    $('#CollectorNetworkLine_' + m_LastCollectionComputerRowIndex).css("background-image", "url('images/page/LineG1.png')");
                }
                else {
                    $('#CollectorNetworkLine_' + m_LastCollectionComputerRowIndex).css("background-image", "url('images/page/LineR1.png')");
                }
                if (m_FirstCollectorFlag == false) {
                    if (myCollectionComputerValue[m_CollectorKey]["NetStatus"] == true)
                    {
                        $('#CollectorNetworkNode_' + m_LastCollectorRowIndex).css("background-image", "url('images/page/LineG5.png')");
                    }
                    else
                    {
                        $('#CollectorNetworkNode_' + m_LastCollectorRowIndex).css("background-image", "url('images/page/LineR5.png')");
                    }
                }
            }
        });
        if (m_FirstCollectionComputerFlag == false) {
            if (myFactoryValue[m_CollectionComputerKey]["NetStatus"] == true) {
                $('#CollectionComputerNetworkNode_' + m_LastCollectionComputerRowIndex).css("background-image", "url('images/page/LineG5.png')");
            }
            else {
                $('#CollectionComputerNetworkNode_' + m_LastCollectionComputerRowIndex).css("background-image", "url('images/page/LineR5.png')");
            }

        }
        //alert(n+' '+value);  
        //var trs = "";  
        //trs += "<tr><td>" +value.name+"</td> <td>" + value.password +"</td></tr>";  
        //tbody += trs;         
    });
}
function loadDialog() {
    //loading 分厂节点dialog
    $('#dlg_FactoryServer').dialog({
        title: '网络信息',
        width: 450,
        height: 200,
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
        height: 400,
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
function FactoryInfo(myFactoryId) {
    $('#TextBox_FactoryName').textbox('setValue', NodeArray[myFactoryId]["FactoryName"]);
    $('#TextBox_FactoryAddress').textbox('setValue', NodeArray[myFactoryId]["FactoryAddress"]);
    $('#Text_FactoryNetworkStatus').textbox('setValue', NodeArray[myFactoryId]["NetStatus"] == true ? "正常" : "不正常");
    $('#Text_SynchronizationStatus').textbox('setValue', NodeArray[myFactoryId]["SynchronizationStatus"] == true ? "正常" : "不正常");
    $('#Text_FactoryCollectionSoftStatus').textbox('setValue', NodeArray[myFactoryId]["NodeSoftStatus"] == true ? "正常" : "不正常");
    $('#dlg_FactoryServer').dialog('open');
    //alert(myFactoryId);
}
function CollectionComputerInfo(myFactoryId, myCollectionComputerId)
{
    $('#Text_CollectionComputer').textbox('setValue', NodeArray[myFactoryId][myCollectionComputerId]["CollectionName"]);
    $('#Text_CollectionComputerAddress').textbox('setValue', NodeArray[myFactoryId][myCollectionComputerId]["CollectionComputerAddress"]);
    $('#Text_CollectionComputerNetworkStatus').textbox('setValue', NodeArray[myFactoryId][myCollectionComputerId]["NetStatus"] == true ? "正常" : "不正常");
    $('#Text_CollectionComputerSoftStatus').textbox('setValue', NodeArray[myFactoryId][myCollectionComputerId]["NodeSoftStatus"] == true ? "正常" : "不正常");
    $('#dlg_CollectionComputerServer').dialog('open');
}
function CollectorInfo(myFactoryId, myCollectionComputerId, myCollectorId) {

    var m_CollectorName = NodeArray[myFactoryId][myCollectionComputerId][myCollectorId]["CollectorName"];
    var m_IpAddress = NodeArray[myFactoryId][myCollectionComputerId][myCollectorId]["IpAddress"];
    var m_CollectorType = NodeArray[myFactoryId][myCollectionComputerId][myCollectorId]["CollectionType"];
    var m_NetStatus = NodeArray[myFactoryId][myCollectionComputerId][myCollectorId]["NetStatus"] == true ? "正常" : "不正常";
    $('#Text_CollectorName').textbox('setValue', m_CollectorName);
    $('#Text_CollectorIpAddress').textbox('setValue', m_IpAddress);
    $('#Text_CollectorType').textbox('setValue', m_CollectorType);
    $('#Text_CollectorNetworkStatus').textbox('setValue', m_NetStatus);
    if (m_CollectorType == "Ammeter") {
        SetAmmterStatus(myFactoryId, m_CollectorName, m_IpAddress);
    }
    else
    {
        SetOPCStatus(myFactoryId, myCollectionComputerId, myCollectorId, m_NetStatus);
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
        $('#dlg_AmmeterStatus').dialog('open');
}
function SetOPCStatus(myFactoryId, myCollectionComputerId, myCollectorId, myNetStatus) {
    $('#Text_OPCName').textbox('setValue', NodeArray[myFactoryId][myCollectionComputerId][myCollectorId]["CollectorName"]);
    $('#Text_OPCIpAddress').textbox('setValue', NodeArray[myFactoryId][myCollectionComputerId][myCollectorId]["IpAddress"]);
    $('#Text_OPCAddress').textbox('setValue', NodeArray[myFactoryId][myCollectionComputerId][myCollectorId]["OPCAddress"]);
    $('#Text_OPCStatus').textbox('setValue', myNetStatus);
    $('#dlg_OPCStatus').dialog('open');
}

