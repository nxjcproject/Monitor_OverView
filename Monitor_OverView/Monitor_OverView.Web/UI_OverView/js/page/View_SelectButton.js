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

    ////////////////////////////以下是按钮位置/////////////////////////////////
    if ($('#GlobalBackGroundDiv').offset() != null && $('#GlobalBackGroundDiv').offset() != undefined)   //表示第一个总貌页面
    {
        $('#SelectButtonDiv').css('left', $('#GlobalBackGroundDiv').offset().left + parseInt(m_DisplayWidth / 2) - 150);
        $('#SelectButtonDiv').css('top', $('#GlobalBackGroundDiv').offset().top + m_DisplayHeight - 18);
    }
    else if ($('#LeftBlankWidth') != null && $('#LeftBlankWidth') != undefined) {
        $('#LeftBlankWidth').css('width', parseInt(m_DisplayWidth / 2) - 150);
    }
}


function ChageOtherPage(myOtherPageId) {
    window.location.href = myOtherPageId + '.aspx';
}
