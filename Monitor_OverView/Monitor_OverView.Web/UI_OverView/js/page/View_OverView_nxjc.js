var WidthBlankSize = 20;
var HeightBlankSize = 15;
var MinDisplayWidth = 1320;
var MinDisplayHeight = 585;
var MaxFullDisplayWidth = 1366;
var MaxFullDisplayHeight = 600;
$(document).ready(function () {
    //alert($(window).height()); //浏览器时下窗口可视区域高度
    //alert($(document).height()); //浏览器时下窗口文档的高度
    //alert($(document.body).height());//浏览器时下窗口文档body的高度
    //alert($(document.body).outerHeight(true));//浏览器时下窗口文档body的总高度 包括border padding margin
    //alert($(window).width()); //浏览器时下窗口可视区域宽度
    //alert($(document).width());//浏览器时下窗口文档对于象宽度
    //alert($(document.body).width());//浏览器时下窗口文档body的高度
    //alert($(document.body).outerWidth(true));//浏览器时下窗口文档body的总宽度 包括border padding margin 
    var m_DocumentWidth = GetGlobalWidth($(document.body).outerWidth(true));
    var m_DocumentHeight = GetGlobalHeight($(document.body).outerHeight(true));
    SetGlobalSize(m_DocumentWidth, m_DocumentHeight);

    /////////////窗口改变大小跟随
    $(window).resize(function () {
        //setTimeout(ChangeGlobalSize, 5000);
        var m_DocumentWidthC = GetGlobalWidth($(document.body).outerWidth(true));
        var m_DocumentHeightC = GetGlobalHeight($(document.body).outerHeight(true));
        SetGlobalSize(m_DocumentWidthC, m_DocumentHeightC);
    });

    GetRealTimeData();

    $(".DataZoneTable").click(function (myObject) {
        ///跳转页面///
        LinkToSubPage($(this)[0].id);
    });

});

function GetGlobalWidth(myDocumentWidth) {
    if (myDocumentWidth > MinDisplayWidth) {
        return myDocumentWidth;
    }
    else {
        return MinDisplayWidth;
    }
}
function GetGlobalHeight(myDocumentHeight) {
    if (myDocumentHeight > MinDisplayHeight) {
        return myDocumentHeight;
    }
    else {
        return MinDisplayHeight;
    }
}
function SetGlobalSize(myDocumentWidth, myDocumentHeight) {
    var m_DisplayWidth = MinDisplayWidth;
    var m_DisplayHeight = MinDisplayHeight;
    ///////////////////定Div位置
    if (myDocumentWidth > MaxFullDisplayWidth) {
        $('#GlobalBackGroundDiv').css('left', (myDocumentWidth - MaxFullDisplayWidth) / 2);
    }
    else {
        $('#GlobalBackGroundDiv').css('left', 0);
    }
    if (myDocumentHeight > MaxFullDisplayHeight) {
        $('#GlobalBackGroundDiv').css('top', (myDocumentHeight - MaxFullDisplayHeight) / 2);
    }
    else {
        $('#GlobalBackGroundDiv').css('top', 0);
    }
    if (myDocumentWidth >= MaxFullDisplayWidth)   //当前窗口宽度大于设定最大的宽度，则按照设定宽度分配
    {
        m_DisplayWidth = MaxFullDisplayWidth;
    }
    else if (myDocumentWidth > MinDisplayWidth)  //当前窗口宽度小于设定最大宽度但大于设定最小宽度，则按照当前窗口宽度分配
    {
        m_DisplayWidth = myDocumentWidth;
    }
    else {                                       //当前窗口宽度小于最小宽度，则按照最小宽度分配
        m_DisplayWidth = MinDisplayWidth;
    }
    if (myDocumentHeight >= MaxFullDisplayHeight)   //当前窗口高度大于设定最大的高度，则按照设定高度分配
    {
        m_DisplayHeight = MaxFullDisplayHeight;
    }
    else if (myDocumentHeight > MinDisplayHeight)  //当前窗口宽度小于设定最大高度但大于设定最小高度，则按照当前窗口高度分配
    {
        m_DisplayHeight = myDocumentHeight;
    }
    else {                                       //当前窗口高度小于最小高度，则按照最小高度分配
        m_DisplayHeight = MinDisplayHeight;
    }

    $(".DataZoneBlankTd").css('width', parseInt((m_DisplayWidth - 1200) / 6));
    $(".BorderBlankTd1").css('width', parseInt((m_DisplayWidth - 1200) / 6));
    $(".BorderBlankTd1").css('height', parseInt((m_DisplayHeight - 540) / 3));
    $(".BorderBlankTd2").css('height', parseInt((m_DisplayHeight - 540) / 3));

    /////以下设置按钮位置
    $('#SelectButtonDiv').css('left', $('#GlobalBackGroundDiv').offset().left + parseInt(m_DisplayWidth / 2) - 150);
    $('#SelectButtonDiv').css('top', $('#GlobalBackGroundDiv').offset().top + m_DisplayHeight - 18);

}

//////////////////////////获得后台数据//////////////////////
function GetRealTimeData() {
    var m_StartTime = "";  //$('#Datebox_StartTimeF').datebox('getValue');
    var m_EndTime = "";   //$('#Datebox_EndTimeF').datebox('getValue');
    $.ajax({
        type: "POST",
        url: "View_OverView_nxjc.aspx/GetRealTimeData",
        data: "{myStartTime:'" + m_StartTime + "',myEndTime:'" + m_EndTime + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData != null && m_MsgData != undefined) {
                SetDisplayData(m_MsgData);
            }
        }
    });
}
function SetDisplayData(myMsgData) {
    var m_Rows = myMsgData.rows;
    for (var i = 1; i <= m_Rows.length; i++) {
        $("#DataZoneTitle" + i).html(m_Rows[i - 1].DataZoneTitle);
        $("#DataZoneTable" + i).data("options").name = m_Rows[i - 1].DataZoneTitleId;
        for (var j = 1; j <= 8; j++) {
            if (j == 4 || j == 8) {
                $("#DataTextTd" + i + "_" + j).html(m_Rows[i - 1]["Data" + j] + " %");
            }
            else {
                $("#DataTextTd" + i + "_" + j).html(m_Rows[i - 1]["Data" + j] + " 吨");
            }
        }
    }
}
function ChageOtherPage(myOtherPageId) {
    window.location.href = myOtherPageId + '.aspx';
}
function LinkToSubPage(myOtherPageId) {
    //$(myObject).css("border", "1px solid black;");
    var m_OtherPageId = $('#' + myOtherPageId).data("options").name;
    window.location.href = 'View_OverView_Factory.aspx?id=' + m_OtherPageId;
}