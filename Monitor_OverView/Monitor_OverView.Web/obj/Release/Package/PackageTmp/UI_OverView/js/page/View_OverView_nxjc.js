var WidthBlankSize = 20;
var HeightBlankSize = 15;
var MinDisplayWidth = 1320;
var MinDisplayHeight = 600;
var MaxFullDisplayWidth = 1366;
var MaxFullDisplayHeight = 610;
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

    ///////////////初始化时间//////////////////
    InitializationDateTime();

    $(".DataZoneTable").click(function (myObject) {
        ///跳转页面///
        LinkToSubPage($(this)[0].id);
    });

});

//////////////////时间选项///////////////////
function InitializationDateTime() {


    $('#Datebox_StartTimeF').datebox({
        formatter: function (date) {
            var years = date.getFullYear();//获取年
            var months = date.getMonth() + 1;//获取日
            var dates = date.getDate();//获取月

            if (months < 10) {//当月份不满10的时候前面补0，例如09
                months = '0' + months;
            }

            if (dates < 10) {//当日期不满10的时候前面补0，例如09
                dates = '0' + dates;
            }
            return years + "-" + months + "-" + dates;//根据自己需求进行改动
        },
        parser: function (date)
        {
            return new Date(Date.parse(date.replace(/-/g, "/")));
        },
        onSelect: function (date) { GetRealTimeData(date); }
    });


    var nowDate = new Date();
    nowDate.setDate(nowDate.getDate() - 1);
    starDate = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate();
    $("#Datebox_StartTimeF").datebox('setValue', starDate);
    GetRealTimeData(nowDate);
}

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
function compareDate(d1, d2) {  // 时间比较的方法，如果d1时间比d2时间大，则返回true   
    return Date.parse(d1.replace(/-/g, "/")) > Date.parse(d2.replace(/-/g, "/"))
}
//////////////////////////获得后台数据//////////////////////
function GetRealTimeData(myDate) {
    var m_TomorrowDate = new Date();
    m_TomorrowDate.setDate(m_TomorrowDate.getDate() + 1);
    //nowDate.setDate(nowDate.getDate() - 1)
    var m_ViladDate = compareDate(DateTimeFormat(m_TomorrowDate, "yyyy-MM-dd"), DateTimeFormat(myDate, "yyyy-MM-dd"));
    if (m_ViladDate == true) {
        var SelectedDate = myDate;
        var SelectedDateString = DateTimeFormat(myDate, "yyyy-MM-dd");
        $.ajax({
            type: "POST",
            url: "View_OverView_nxjc.aspx/GetRealTimeData",
            data: "{myDateTime:'" + SelectedDateString + "'}",
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
    else {
        alert("请选择今天以前的时间!");
    }
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