var MinDisplayWidth = 1035;
var MinDisplayHeight = 585;
var MaxFullDisplayWidth = 1366;
var MaxFullDisplayHeight = 600;
var FactoryOrganizationId = "";
var RuntimeRefreshInterval = 10000;

var CurrentPreLoadComponents = 0;        //当前已经动态加载了几个
var MaxPreLoadComponents = 1;            //提前动态加载的组件数量

var ElectricPercentageLoadCount;  //用电比例数据加载完成计数(某些数据是两次或者三次回调才生成)
var PlotObjArray = [{"Obj":[],"FirstLoadFlag":true},{"Obj":[],"FirstLoadFlag":true},{"Obj":[],"FirstLoadFlag":true}
                   , { "Obj": [], "FirstLoadFlag": true }, { "Obj": [], "FirstLoadFlag": true }, { "Obj": [], "FirstLoadFlag": true }];   //1、能源比例;2、故障停机比例;3、运转率;4、故障率;5、可靠性;6、分步电耗
var SelectedDateString;             //选择的时间
var SelectedDate;
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
    ////////////////左边区域///////////////////
    GetEnergyQuantityHtml("EnergyQuantityInfoTd");      //获得工序电量结构
    GetEnergyConsumptionHtml("EnergyConsumptionInfoTd");      //获得工序电量结构
    GetEnergyConsumptionComprehensiveHtml("EnergyConsumptionComprehensiveInfoTd");           //获得综合电耗结构

    ///////////////中间区域//////////////////////
    GetMaterialWeightHtml("MaterialWeightOutputTd");
    GetMaterialStorage("MaterialStorageTd");
    GetRunIndicatorsHtml("EquipmentRunIndicatorsTd");

    //GetPieChart("piechart_HaltStatisticalRange", "", null);

    ////////////////////初始化对话框////////////////
    loadElectricityQuantityDetailDialog();          //电量
    loadElectricityConsumptionDetailDialog();       //电耗
    loadElectricityConsumptionDetailYRDialog();     //吨熟料发电量
    loadElectricityConsumptionCDetailDialog();      //综合电耗
    loadMaterialWeightDetailDialog();               //物料量
    loadRunIndictorsDetailDialog();                 //设备运行指标
    loadMasterMachineHaltDetailDialog();            //故障停机
    loadWorkingTeamShiftLogDetailDialog();          //交接班记录
    loadQuickMenuDetailDialog();                    //初始化快捷菜单

    //GetRealTimeData();


});
function AddLoadComponentFlag() {                           //增加一个组件标志
    CurrentPreLoadComponents = CurrentPreLoadComponents + 1;
    if (CurrentPreLoadComponents == MaxPreLoadComponents) {
        ////////////////当前页面所处生产区域///////////////
        GetDisplayFactoryOragization();
    }

}
//////////////////时间选项///////////////////
function InitializationDateTime() {
    var nowDate = new Date();
    nowDate.setDate(nowDate.getDate() - 1);
    starDate = nowDate.getFullYear() + '-' + (nowDate.getMonth() + 1) + '-' + nowDate.getDate();
    $("#dateTime").datebox('setValue', starDate);
    SelectedDate = nowDate;
    SelectedDateString = DateTimeFormat(nowDate, "yyyy-MM-dd");
}
function QueryDataFun(myDate) {
    var m_TodayDate = new Date();
    var m_ViladDate = compareDate(DateTimeFormat(m_TodayDate, "yyyy-MM-dd"), DateTimeFormat(myDate, "yyyy-MM-dd"));
    if (m_ViladDate == true) {
        SelectedDate = myDate;
        SelectedDateString = DateTimeFormat(myDate, "yyyy-MM-dd");
        if (FactoryOrganizationId != "") {
            RefreshFactoryOrganiztion(FactoryOrganizationId);
        }
        else {
            alert("无法确认生产区域!");
        }
    }
    else {
        alert("请选择今天以前的时间!");
    }
}
function compareDate(d1, d2) {  // 时间比较的方法，如果d1时间比d2时间大，则返回true   
    return Date.parse(d1.replace(/-/g, "/")) > Date.parse(d2.replace(/-/g, "/"))
}
///////////////////定位MainTable////////////////////
function SetGlobalSize(myWidth, myHeight) {
    $("#MainTablePosizionLeft").css('width', parseInt((myWidth - MinDisplayWidth) / 2));
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
///////////////////// 判断当前生产区域///////////
function GetDisplayFactoryOragization() {
    var m_VisiabilityBackToGlobalPageButton = $('#HiddenField_ComfromGlobalPage').val();
    var m_Msg = $("#HiddenField_StationOrganizationIds").val();
    var m_FactoryOragization = jQuery.parseJSON(m_Msg);
    if (m_FactoryOragization != null && m_FactoryOragization != undefined) {
        if (m_FactoryOragization["rows"].length > 0) {
            $('#Select_SelectStation').combobox("loadData", m_FactoryOragization["rows"]);
            $('#Select_SelectStation').combobox("setValue", m_FactoryOragization["rows"][0]["OrganizationId"]);
            RefreshFactoryOrganiztion(m_FactoryOragization["rows"][0]["OrganizationId"]);
        }
        else {
            $('#Select_SelectStation').combobox("clear");
            RefreshFactoryOrganiztion("");
            alert("无法确认生产区域!");
        }
        //= "zc_nxjc_qtx_efc"
    }
    else {
        $('#Select_SelectStation').combobox("clear");
        RefreshFactoryOrganiztion("");
        alert("无法确认生产区域!");
    }
    if (m_VisiabilityBackToGlobalPageButton == "True") {
        $('#button_BackToGlobalPage').css("visibility", "visible");
    }
    else {
        $('#button_BackToGlobalPage').css("visibility", "hidden");
    }
}
function ChangeDisplayStation() {
    window.location.href = 'View_OverView_nxjc.aspx';
}
////////////////////左边区域/////////////////////
function GetEnergyQuantityHtml(myRootDomId) {
    var m_EnergyQuantity = '<table>';
    m_EnergyQuantity = m_EnergyQuantity + '<tr><td class = "ElectricFirstTableTitleTd">工序电量</td><td class = "ElectricTableTitleTd"></td><td class = "ElectricTableTitleTd"></td><td class = "ElectricPercentageTitleTd"  colspan = "3">单位(kWh)</td></tr>';
    m_EnergyQuantity = m_EnergyQuantity + '<tr><td class = "ElectricTableColumnTd">工序名称</td><td class = "ElectricTableColumnTd">当日</td><td class = "ElectricTableColumnTd">月累计</td></tr>';
    m_EnergyQuantity = m_EnergyQuantity + '<tr><td class = "ElectricTableForcusColumnTd" onclick ="GetElectricityQuantityDetail(this,\'cementGrind\',\'水泥磨\',\'工序电量\');">1#水泥粉磨</td><td id = "cementGrind_ElectricityQuantity_DayGlobal" class = "ElectricTableTd">0.00</td><td id = "cementGrind_ElectricityQuantity_MonthGlobal" class = "ElectricTableTd">0.00</td></tr>';
    m_EnergyQuantity = m_EnergyQuantity + '<tr><td class = "ElectricTableForcusColumnTd" onclick ="GetElectricityQuantityDetail(this,\'cementGrind\',\'水泥磨\',\'工序电量\');">2#水泥粉磨</td><td id = "cementGrind_ElectricityQuantity_DayGlobal" class = "ElectricTableTd">0.00</td><td id = "cementGrind_ElectricityQuantity_MonthGlobal" class = "ElectricTableTd">0.00</td></tr>';
    m_EnergyQuantity = m_EnergyQuantity + '<tr><td class = "ElectricTableLastRowForcusColumnTd" onclick ="GetElectricityQuantityDetail(this,\'cementPacking\',\'分厂\',\'工序电量\');">水泥包装</td><td id = "cementPacking_ElectricityQuantity_DayGlobal" class = "ElectricTableLastRowTd">0.00</td><td id = "cementPacking_ElectricityQuantity_MonthGlobal" class = "ElectricTableLastRowTd">0.00</td></tr>';
    m_EnergyQuantity = m_EnergyQuantity + '</table>';
    $("#" + myRootDomId).html(m_EnergyQuantity);
}

function GetEnergyConsumptionHtml(myRootDomId) {
    var m_EnergyConsumption = '<table>';
    m_EnergyConsumption = m_EnergyConsumption + '<tr><td class = "ElectricFirstTableTitleTd">工序电耗</td><td class = "ElectricTableTitleTd"></td><td class = "ElectricTableTitleTd"></td><td class = "ElectricFirstTableTitleTd"></td><td class = "ElectricTableTitleTd"></td><td class = "ElectricTableTitleTd">单位(kWh/t)</td></tr>';
    m_EnergyConsumption = m_EnergyConsumption + '<tr><td class = "ElectricTableColumnTd">工序名称</td><td class = "ElectricTableColumnTd">当日</td><td class = "ElectricTableColumnTd">月累计</td></tr>';
    m_EnergyConsumption = m_EnergyConsumption + '<tr><td class = "ElectricTableLastRowForcusColumnTd" onclick ="GetElectricityConsumptionDetail(this,\'cementGrind\',\'cement_CementOutput\',\'水泥磨\',\'工序电耗\');">1#水泥粉磨</td><td id = "cementGrind_ElectricityConsumption_DayGlobal" class = "ElectricTableLastRowTd">0.00</td><td id = "cementGrind_ElectricityConsumption_MonthGlobal" class = "ElectricTableLastRowTd">0.00</td>></tr>';
    m_EnergyConsumption = m_EnergyConsumption + '<tr><td class = "ElectricTableLastRowForcusColumnTd" onclick ="GetElectricityConsumptionDetail(this,\'cementGrind\',\'cement_CementOutput\',\'水泥磨\',\'工序电耗\');">2#水泥粉磨</td><td id = "cementGrind_ElectricityConsumption_DayGlobal" class = "ElectricTableLastRowTd">0.00</td><td id = "cementGrind_ElectricityConsumption_MonthGlobal" class = "ElectricTableLastRowTd">0.00</td>></tr>';
    m_EnergyConsumption = m_EnergyConsumption + '<tr><td class = "ElectricTableLastRowForcusColumnTd" onclick ="GetElectricityConsumptionDetail(this,\'cementPacking\',\'cement_CementOutput\',\'分厂\',\'工序电耗\');">水泥包装</td><td id = "cementPacking_ElectricityConsumption_DayGlobal" class = "ElectricTableLastRowTd">0.00</td><td id = "cementPacking_ElectricityConsumption_MonthGlobal" class = "ElectricTableLastRowTd">0.00</td></tr>';
    m_EnergyConsumption = m_EnergyConsumption + '</table>';
    $("#" + myRootDomId).html(m_EnergyConsumption);
}
function GetEnergyConsumptionComprehensiveHtml(myRootDomId) {
    var m_EnergyConsumptionComprehensive = '<table>';
    m_EnergyConsumptionComprehensive = m_EnergyConsumptionComprehensive + '<tr><td class = "ElectricComprehensiveFirstTableTitleTd">综合电耗(月)</td><td class = "ElectricComprehensiveTableTitleTd"></td><td class = "ElectricComprehensiveTableTitleTd"></td><td class = "ElectricComprehensiveTableTitleTd">单位(kWh/t)</td></tr>';
    m_EnergyConsumptionComprehensive = m_EnergyConsumptionComprehensive + '<tr><td class = "ElectricComprehensiveTableColumnTd">名称</td><td class = "ElectricComprehensiveTableColumnTd">综合电耗</td><td class = "ElectricComprehensiveTableColumnTd">综合煤耗</td><td class = "ElectricComprehensiveTableColumnTd">综合能耗</td></tr>';
    m_EnergyConsumptionComprehensive = m_EnergyConsumptionComprehensive + '<tr><td class = "ElectricComprehensiveTableLastRowForcusTd" onclick ="GetElectricityConsumptionCDetail(this, \'水泥磨\', \'综合能耗\');">水泥</td><td id ="Cementmill_ElectricityConsumptionComprehensive_MonthGlobal" class = "ElectricComprehensiveTableLastRowTd">0.00</td><td class = "ElectricComprehensiveTableLastRowTd">0.00</td><td id = "Cementmill_EnergyConsumptionComprehensive_MonthGlobal" class = "ElectricComprehensiveTableLastRowTd">0.00</td></tr>';
    m_EnergyConsumptionComprehensive = m_EnergyConsumptionComprehensive + '</table>';
    $("#" + myRootDomId).html(m_EnergyConsumptionComprehensive);
}

//////////////////////中间区域/////////////////////
function GetMaterialWeightHtml(myRootDomId) {
    var m_MaterialWeight = '<table>';
    m_MaterialWeight = m_MaterialWeight + '<tr><td class = "MaterialWeightFirstTitleRowTd">产量(t)</td><td class = "MaterialWeightTitleRowForcusTd" onclick ="GetMaterialWeightDetail(this,\'cement_CementOutput\',\'Output\',\'水泥磨\');">1#水泥磨</td><td class = "MaterialWeightTitleRowForcusTd" onclick ="GetMaterialWeightDetail(this,\'cement_CementOutput\',\'Output\',\'水泥磨\');">2#水泥磨</td><td class = "MaterialWeightTitleRowForcusTd">总计</td></tr>';
    m_MaterialWeight = m_MaterialWeight + '<tr><td class = "MaterialWeightFristRowTd">○当日</td><td id = "cement_CementOutput_DayGlobal" class = "MaterialWeightRowTd">0.00</td><td id = "2cement_CementOutput_DayGlobal" class = "MaterialWeightRowTd">0.00</td><td id = "Allcement_CementOutput_DayGlobal" class = "MaterialWeightRowTd">0.00</td></tr>';
    m_MaterialWeight = m_MaterialWeight + '<tr><td class = "MaterialWeightFristRowTd">○月累计</td><td id = "cement_CementOutput_MonthGlobal" class = "MaterialWeightRowTd">0.00</td><td id = "2cement_CementOutput_MonthGlobal" class = "MaterialWeightRowTd">0.00</td><td id = "Allcement_CementOutput_MonthGlobal" class = "MaterialWeightRowTd">0.00</td></tr>';
    //m_MaterialWeight = m_MaterialWeight + '<tr><td class = "MaterialWeightFristRowTd">○月计划</td><td class = "MaterialWeightRowTd">0.00</td><td class = "MaterialWeightRowTd">0.00</td><td class = "MaterialWeightRowTd">0.00</td><td class = "MaterialWeightRowTd">0.00</td><td class = "MaterialWeightRowTd">0.00</td></tr>';
    m_MaterialWeight = m_MaterialWeight + '<tr><td class = "MaterialWeightFirstTitleRowTd">消耗量(t)</td><td class = "MaterialWeightTitleRowForcusTd" onclick ="GetMaterialWeightDetail(this,\'clinker_ClinkerInput\',\'Input\',\'水泥磨\');">1#熟料消耗</td><td class = "MaterialWeightTitleRowForcusTd" onclick ="GetMaterialWeightDetail(this,\'clinker_ClinkerInput\',\'Input\',\'水泥磨\');">2#熟料消耗</td><td class = "MaterialWeightTitleRowForcusTd" onclick ="GetMaterialWeightDetail(this,\'clinker_ClinkerInput\',\'Input\',\'水泥磨\');">总计</td></tr>';
    m_MaterialWeight = m_MaterialWeight + '<tr><td class = "MaterialWeightFristRowTd">○当日</td><td id = "clinker_ClinkerInput_DayGlobal"class = "MaterialWeightRowTd">0.00</td><td class = "MaterialWeightRowTd"></td></tr>';
    m_MaterialWeight = m_MaterialWeight + '<tr><td class = "MaterialWeightFristLastRowTd">○月累计</td><td id = "clinker_ClinkerInput_MonthGlobal" class = "MaterialWeightLastRowTd">0.00</td></tr>';
    m_MaterialWeight = m_MaterialWeight + '</table>';
    $("#" + myRootDomId).html(m_MaterialWeight);
}

function GetMaterialStorage(myRootDomId) {
    var m_Msg = $("#HiddenField_StationOrganizationIds").val();
    var m_FactoryOragization = jQuery.parseJSON(m_Msg);
    if (m_FactoryOragization != null && m_FactoryOragization != undefined) {
        if (m_FactoryOragization["rows"].length > 0) {
            FactoryOrganizationId = m_FactoryOragization["rows"][0]["OrganizationId"];
        }
    }
    //这里库存改成活的库存
    $.ajax({
        type: "POST",
        url: "View_OverView_Factory.aspx/GetMaterialStorageInfo",
        data: '{myFactoryOrganizationId:"' +  FactoryOrganizationId + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MaterialStorage = msg.d;
            $("#" + myRootDomId).html(m_MaterialStorage);
            AddLoadComponentFlag();
        },
        error: function (err) {
            AddLoadComponentFlag();
        }
    });
}

function GetRunIndicatorsHtml(myRootDomId) {
    var m_MaterialWeight = '<table>';
    m_MaterialWeight = m_MaterialWeight + '<tr><td class = "MaterialWeightFirstTitleRowTd"><span style = "letter-spacing: -2px;">设备运行(月)</span></td><td class = "MaterialWeightTitleRowTd">台时产量</td><td class = "MaterialWeightTitleRowTd">运转时间</td></tr>';
    m_MaterialWeight = m_MaterialWeight + '<tr><td class = "MaterialWeightForcusRowTd" onclick ="GetRunIndictorsDetail(this, \'CementGrind\',\'\');">※1#水泥磨</td><td id = "CementGrind_台时产量_Global" class = "MaterialWeightRowTd">0.00</td><td id = "CementGrind_运转时间_Global" class = "MaterialWeightRowTd">0.00</td></tr>';
    m_MaterialWeight = m_MaterialWeight + '<tr><td class = "MaterialWeightForcusRowTd" onclick ="GetRunIndictorsDetail(this, \'CementGrind\',\'\');">※2#水泥磨</td><td id = "CementGrind_台时产量_Global" class = "MaterialWeightRowTd">0.00</td><td id = "CementGrind_运转时间_Global" class = "MaterialWeightRowTd">0.00</td></tr>';
    m_MaterialWeight = m_MaterialWeight + '<tr><td class = "MaterialWeightForcusLastRowTd" onclick ="GetRunIndictorsDetail(this, \'CementPacker\',\'\');">※包机</td><td id = "CementPacker_台时产量_Global" class = "MaterialWeightLastRowTd">0.00</td><td id = "CementPacker_运转时间_Global" class = "MaterialWeightLastRowTd">0.00</td></tr>';
    m_MaterialWeight = m_MaterialWeight + '</table>';
    $("#" + myRootDomId).html(m_MaterialWeight);
}

///////////////获得折线图///////////////////
function ReleasePlotChart(containerId, plot) {
    if (plot) {
        plot.destroy();

        var elementId = '#' + containerId;
        $(elementId).unbind(); // for iexplorer  
        $(elementId).empty();

        plot = null;
    }
}
function GetChartDataTabs(myTabName, myData, myObjArrayIndex, myTabTitle) {
    $('#ChartDataTabs').tabs("select", myTabTitle);
    var m_ChartData = [];
    var m_legendData = [];
    var m_MaxValue = 0;
    for (var i = 0; i < myData["rows"].length; i++) {
        /////////////遍历每一列数据
        var j = 0;
        m_ChartData[i] = [];
        $.each(myData["rows"][i], function (myKey, myValue) {          //遍历采集服务器
            if (j == 0) {

            }
            else if (j == 1) {
                m_legendData[i] = myValue;
            }
            else {
                var m_Date = myKey.split('-');
                var m_PointValue = parseFloat(myValue);
                m_ChartData[i][j - 2] = [m_Date[1] + '/' + '01/' + m_Date[0], m_PointValue];
                if (m_PointValue > m_MaxValue) {
                    m_MaxValue = m_PointValue;        //找出最大的纵坐标
                }
            }
            j = j + 1;
        });
    }
    if (m_MaxValue <= 1) {
        m_MaxValue = 1;
    }
    else if (m_MaxValue > 1 && m_MaxValue < 20) {
        m_MaxValue = 20;
    }
    else if (m_MaxValue >= 20 && m_MaxValue < 40) {
        m_MaxValue = 40;
    }
    else if (m_MaxValue >= 40 && m_MaxValue < 60) {
        m_MaxValue = 60;
    }
    else if (m_MaxValue >= 60 && m_MaxValue < 80) {
        m_MaxValue = 80;
    }
    else if (m_MaxValue >= 80 && m_MaxValue < 100) {
        m_MaxValue = 100;
    }
    else if (m_MaxValue >= 100 && m_MaxValue < 120) {
        m_MaxValue = 120;
    }
    else if (m_MaxValue >= 120 && m_MaxValue < 140) {
        m_MaxValue = 140;
    }
    else if (m_MaxValue >= 140 && m_MaxValue < 160) {
        m_MaxValue = 160;
    }
    else {
        m_MaxValue = m_MaxValue + 10;
    }
    if (PlotObjArray[myObjArrayIndex]["FirstLoadFlag"] == true) {
        GetLineChart(myTabName, m_ChartData, myObjArrayIndex, m_MaxValue);
        PlotObjArray[myObjArrayIndex]["FirstLoadFlag"] = false;
    }
    else {
        PlotObjArray[myObjArrayIndex]["Obj"].data = m_ChartData;
        PlotObjArray[myObjArrayIndex]["Obj"].replot(PlotObjArray[myObjArrayIndex]["Obj"].options);
        //PlotObjArray[myObjArrayIndex]["Obj"].destroy();
        //$('#' + myTabName).empty();
        //GetLineChart(myTabName, m_ChartData, myObjArrayIndex, m_MaxValue);
    }
    GetLineChartLegend(myTabName, m_legendData);
}
function GetLineChartLegend(myTabName, mylegendData) {
    var m_SeriesColors = ["#01b3f9", "#fef102", "#f8000e", "#a400ed", "#aaf900", "#fe0072", "#0c6c92", "#fea002", "#c1020a", "#62008d", "#3c8300"];
    var m_LegendObjId = myTabName.substring(0,myTabName.length - '_Content'.length) + "_Legend";
    var m_LegendObj = $('#' + m_LegendObjId);
    m_LegendObj.empty();
    var m_LegendHtml = '<table id="' + m_LegendObjId + '_DefinedLegendTable" style="bottom: 0px;"><tbody><tr class="jqplot-table-legend"><td id="' + m_LegendObjId + '_DefinedLegendBlankTd"></td>';
    for (var i = 0; i < mylegendData.length; i++) {
        m_LegendHtml = m_LegendHtml + '<td style="text-align: center; padding-top: 0px;" class="jqplot-table-legend jqplot-table-legend-swatch"><div class="jqplot-table-legend-swatch-outline"><div style="background-color: ' + m_SeriesColors[i] + '; border-color:  ' + m_SeriesColors[i] + '" class="jqplot-table-legend-swatch"></div></div></td><td style="padding-top: 0px; padding-right:4px; color:#000000; font-size:8pt; font-family: SimSun;">' + mylegendData[i] + '</td>';
    }
    m_LegendHtml = m_LegendHtml + '</tr></tbody></table>';
    m_LegendObj.append($(m_LegendHtml));
    var m_LegendBlankTdWidth = ($('#' + m_LegendObjId).width() - $('#' + m_LegendObjId + '_DefinedLegendTable').width()) / 2;
    $('#' + m_LegendObjId + '_DefinedLegendBlankTd').css("width", m_LegendBlankTdWidth);
    $.parser.parse('#' + m_LegendObjId);
}
function GetLineChart(myTabName, myData, myObjArrayIndex, myMaxValue) {
    //    var line3 = [[['01/01/2008', 0.42], ['02/01/2008', 0.80], ['03/01/2008', 0.56], ['04/01/2008', 0.68],
    //                ['05/01/2008', 0.43], ['06/01/2008', 0.87]]];
    var m_SeriesColors = ["#01b3f9", "#fef102", "#f8000e", "#a400ed", "#aaf900", "#fe0072", "#0c6c92", "#fea002", "#c1020a", "#62008d", "#3c8300"];
    PlotObjArray[myObjArrayIndex]["Obj"] = $.jqplot(myTabName, myData, {
        animate: true,
        seriesColors: m_SeriesColors,
        title: "",
        animateReplot: true,
        seriesDefaults: {
            lineWidth: 1,
            markerOptions: { size: 0 }
        },
        axes: {
            xaxis: {
                renderer: $.jqplot.DateAxisRenderer,
                tickOptions: {
                    formatString: "%Y-%m",
                    fontFamily: 'Times New Roman',
                    fontSize: '7pt',
                    fontWeight: 'normal',
                    textColor: '#000000'
                },
                labelOptions: {
                    fontFamily: 'Times New Roman',
                    fontSize: '7pt',
                    textColor: '#000000'
                },
            },
            yaxis: {
                tickOptions: {
                    formatString: "%.2f",
                    fontFamily: 'Times New Roman',
                    fontSize: '7pt',
                    fontWeight: 'normal',
                    textColor: '#000000'
                },
                labelOptions: {
                    fontFamily: 'Times New Roman',
                    fontSize: '7pt',
                    textColor: '#000000'
                },
                min: 0,
                max: myMaxValue,
                numberTicks: 5,
            }
        },
        highlighter: {
            show: true,
            sizeAdjust: 15
        },
        //cursor: {
        //    show: true,
        //    tooltipLocation: 'sw'
        //},
        grid: {
            drawGridLines: true, // wether to draw lines across the grid or not.
            gridLineColor: '#cccccc', // 设置整个图标区域网格背景线的颜色
            background: '#eeeeff', // 设置整个图表区域的背景色
            borderColor: '#999999', // 设置图表的(最外侧)边框的颜色
            borderWidth: 2.0, //设置图表的（最外侧）边框宽度
            shadow: false, // 为整个图标（最外侧）边框设置阴影，以突出其立体效果
            shadowAngle: 45, // 设置阴影区域的角度，从x轴顺时针方向旋转
            shadowOffset: 1.5, // 设置阴影区域偏移出图片边框的距离
            shadowWidth: 3, // 设置阴影区域的宽度
            shadowDepth: 3, // 设置影音区域重叠阴影的数量
            shadowAlpha: 0.07, // 设置阴影区域的透明度
            renderer: $.jqplot.CanvasGridRenderer, // renderer to use to draw the grid.
            rendererOptions: {} // options to pass to the renderer. Note, the default
            // CanvasGridRenderer takes no additional options.
        }
    });
}
function GetRunningRateChart(myEquipmentCommonIdList) {
    var m_RunIndictors = "运转率"
    $.ajax({
        type: "POST",
        url: "View_OverView_Factory.aspx/GetMonthLineChartData",
        data: '{myRunIndictors:"' + m_RunIndictors + '",myEquipmentCommonIdList:"' + myEquipmentCommonIdList + '",myOrganizationId:"' + FactoryOrganizationId + '",myDatetime:"' + SelectedDateString + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData != null && m_MsgData != undefined) {

                GetChartDataTabs("chartTab01_Content", m_MsgData, 2, "运转率");
            }
        }
    });
}
function GetHaltRateChart(myEquipmentCommonIdList) {
    var m_RunIndictors = "故障率"
    $.ajax({
        type: "POST",
        url: "View_OverView_Factory.aspx/GetMonthLineChartData",
        data: '{myRunIndictors:"' + m_RunIndictors + '",myEquipmentCommonIdList:"' + myEquipmentCommonIdList + '",myOrganizationId:"' + FactoryOrganizationId + '",myDatetime:"' + SelectedDateString + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData != null && m_MsgData != undefined) {
                GetChartDataTabs("chartTab02_Content", m_MsgData, 3, "故障率");
            }
        }
    });
}
function GetReliabilityChart(myEquipmentCommonIdList) {
    var m_RunIndictors = "可靠性"
    $.ajax({
        type: "POST",
        url: "View_OverView_Factory.aspx/GetMonthLineChartData",
        data: '{myRunIndictors:"' + m_RunIndictors + '",myEquipmentCommonIdList:"' + myEquipmentCommonIdList + '",myOrganizationId:"' + FactoryOrganizationId + '",myDatetime:"' + SelectedDateString + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData != null && m_MsgData != undefined) {
                GetChartDataTabs("chartTab03_Content", m_MsgData, 4, "可靠性");
            }
        }
    });
}
function GetElectricitiyConsumptionChart(myVariableIdList, myOrganizationTypeList) {
    $.ajax({
        type: "POST",
        url: "View_OverView_Factory.aspx/GetElectricitiyConsumptionChartData",
        data: '{myVariableIdList:"' + myVariableIdList + '",myOrganizationId:"' + FactoryOrganizationId + '",myOrganizationTypeList:"' + myOrganizationTypeList + '",myDatetime:"' + SelectedDateString + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData != null && m_MsgData != undefined) {
                GetChartDataTabs("chartTab04_Content", m_MsgData, 5, "分步电耗");
            }
        }
    });
}

function GetPieChart(myObjId, myObjArrayIndex, myData) {

    PlotObjArray[myObjArrayIndex]["Obj"] = jQuery.jqplot(myObjId, [myData],
      {
          // title:'Exponential Line',
          // title: {
          //     text: 'aaaa',  //设置当前图的标题
          //     show: true,//设置当前图的标题是否显示
          //     textAlign:'right',
          //     textColor:'red',
          // },
          animate: true,
          animateReplot: true,
          seriesColors: ["#01b3f9", "#fef102", "#f8000e", "#a400ed", "#aaf900", "#fe0072", "#0c6c92", "#fea002", "#c1020a", "#62008d", "#3c8300"],   //设置饼图颜色
          gridPadding: { top: 0, bottom: 0, left: 0, right: 70 },
          grid: {
              drawBorder: false,
              drawGridlines: false,
              background: '#eeeeff',
              shadow: false
          },
          seriesDefaults: {
              // Make this a pie chart.
              renderer: jQuery.jqplot.PieRenderer,

              rendererOptions: {
                  // Put data labels on the pie slices.
                  // By default, labels show the percentage of the slice.
                  showDataLabels: true,
                  startAngle: 180,       //设置旋转角度
                  diameter: 100, // 设置饼的直径  
                  padding: 20,        // 饼距离其分类名称框或者图表边框的距离，变相该表饼的直径  
                  sliceMargin: 2,     // 饼的每个部分之间的距离  

                  fill: true,         // 设置饼的每部分被填充的状态  
                  // background:["red","yellow","green"],
                  shadow: false,       //为饼的每个部分的边框设置阴影，以突出其立体效果    让饼图变成平面
                  // shadowOffset: 2,    //设置阴影区域偏移出饼的每部分边框的距离  
                  // shadowDepth: 5,     // 设置阴影区域的深度  
                  // shadowAlpha: 0.07,   // 设置阴影区域的透明度 
                  //highlightColors: ["rgb(129,0,0)", "rgb(240,189,104)", "rgb(214,202,165)", "rgb(137,180,158)", "rgb(168,180,137)", "rgb(0,0,0)"]    //修改高亮的部分背景颜色
              }
          },
          legend: {
              renderer: $.jqplot.EnhancedLegendRenderer,
              show: true,
              location: 'e',
              placement: 'outsideGrid',
              disableIEFading: true,
              seriesToggle: 'normal',
              seriesToggleReplot: true,
              rowSpacing: '2px',
              rendererOptions: {
                  numberColumns: 1
              }
          }

          // markerOptions:{color: '#000'},

      }
);
    $('.jqplot-target').css('font-size', '10pt');

    var labels = $('table.jqplot-table-legend');
    labels.each(function (index) {
        //turn the label's text color to the swatch's color  
        $(this).css('border', '0px');
        $(this).css('font-size', '8pt');
        //set type name as the label's text  

    });
}

function LoadElectricPercentageData() {

    var m_limestoneMine = parseFloat($('#limestoneMine_ElectricityQuantity_MonthGlobal').text());
    var m_rawMaterialsHomogenize = parseFloat($('#rawMaterialsHomogenize_ElectricityQuantity_MonthGlobal').text());
    var m_rawMaterialsGrind = parseFloat($('#rawMaterialsGrind_ElectricityQuantity_MonthGlobal').text());
    var m_coalPreparation = parseFloat($('#coalPreparation_ElectricityQuantity_MonthGlobal').text());
    var m_clinkerBurning = parseFloat($('#clinkerBurning_ElectricityQuantity_MonthGlobal').text());
    var m_kilnSystem = parseFloat($('#kilnSystem_ElectricityQuantity_MonthGlobal').text());
    var m_hybridMaterialsPreparation = parseFloat($('#hybridMaterialsPreparation_ElectricityQuantity_MonthGlobal').text());
    var m_clinkerTransport = parseFloat($('#clinkerTransport_ElectricityQuantity_MonthGlobal').text());
    var m_cementGrind = parseFloat($('#cementGrind_ElectricityQuantity_MonthGlobal').text());
    var m_cementPacking = parseFloat($('#cementPacking_ElectricityQuantity_MonthGlobal').text());
    var m_auxiliaryProduction = parseFloat($('#auxiliaryProduction_ElectricityQuantity_MonthGlobal').text());

    var m_ElectricPercentageData = [['矿山', m_limestoneMine], ['原料调配', m_rawMaterialsHomogenize], ['生料粉磨', m_rawMaterialsGrind], ['煤粉制备', m_coalPreparation],   //设置数据名称和值
                                    ['熟料烧成', m_clinkerBurning], ['废气处理', m_kilnSystem], ['混合材制备', m_hybridMaterialsPreparation],
                                    ['熟料储送', m_clinkerTransport], ['水泥粉磨', m_cementGrind], ['水泥包装', m_cementPacking], ['辅助用电', m_auxiliaryProduction]];
    if (PlotObjArray[0]["FirstLoadFlag"] == true) {
        GetPieChart("piechart_ElectricPercentage", 0, m_ElectricPercentageData);
        PlotObjArray[0]["FirstLoadFlag"] = false;
    }
    else {
        PlotObjArray[0]["Obj"].series[0].data = m_ElectricPercentageData;
        PlotObjArray[0]["Obj"].replot();
    }
}

function LoadDownTimeCountPercentageData(myData) {

    //var m_DownTimeCountPercentageData = [['生料制备', 22], ['熟料制备', 22], ['水泥制备', 22],   //设置数据名称和值
    //                                ['水泥包装', 22], ['辅助用电', 22]];
    var m_DownTimeCountPercentageData = [];
    for (var i = 0; i < myData.rows.length; i++) {
        m_DownTimeCountPercentageData[i] = [myData.rows[i]["EquipmentName"], parseInt(myData.rows[i]["DowntimeCount"])];
    }
    if (PlotObjArray[1]["FirstLoadFlag"] == true) {
        GetPieChart("piechart_HaltStatisticalRange", 1, m_DownTimeCountPercentageData);
        PlotObjArray[1]["FirstLoadFlag"] = false;
    }
    else {
        PlotObjArray[1]["Obj"].series[0].data = m_DownTimeCountPercentageData;
        PlotObjArray[1]["Obj"].replot();
    }
}
//////////////////////初始化对话框//////////////////////
function loadElectricityQuantityDetailDialog() {
    $('#dlg_ElectricityQuantityDetail').dialog({
        title: '数据项查询',
        width: 360,
        height: 220,
        left: 300,
        top: 20,
        closed: true,
        cache: false,
        modal: true,
        iconCls: 'icon-search',
        resizable: false
    });
}
function loadElectricityConsumptionDetailDialog() {
    $('#dlg_ElectricityConsumptionDetail').dialog({
        title: '数据项查询',
        width: 760,
        height: 220,
        left: 300,
        top: 20,
        closed: true,
        cache: false,
        modal: true,
        iconCls: 'icon-search',
        resizable: false
    });
}
function loadElectricityConsumptionDetailYRDialog() {
    $('#dlg_ElectricityConsumptionDetailYR').dialog({
        title: '数据项查询',
        width: 760,
        height: 220,
        left: 300,
        top: 20,
        closed: true,
        cache: false,
        modal: true,
        iconCls: 'icon-search',
        resizable: false
    });
}
function loadElectricityConsumptionCDetailDialog() {

    $('#dlg_ElectricityConsumptionCDetail').dialog({
        title: '数据项查询',
        width: 430,
        height: 220,
        left: 300,
        top: 20,
        closed: true,
        cache: false,
        modal: true,
        iconCls: 'icon-search',
        resizable: false
    });
}
function loadMaterialWeightDetailDialog() {

    $('#dlg_MaterialWeightDetail').dialog({
        title: '数据项查询',
        width: 340,
        height: 220,
        left: 300,
        top: 20,
        closed: true,
        cache: false,
        modal: true,
        iconCls: 'icon-search',
        resizable: false
    });
}
function loadRunIndictorsDetailDialog() {
    $('#dlg_RunIndictorsDetail').dialog({
        title: '数据项查询',
        width: 550,
        height: 220,
        left: 300,
        top: 20,
        closed: true,
        cache: false,
        modal: true,
        iconCls: 'icon-search',
        resizable: false
    });
}
function loadMasterMachineHaltDetailDialog() {
    $('#dlg_MasterMachineHaltDetail').dialog({
        title: '数据项查询',
        width: 750,
        height: 220,
        left: 300,
        top: 20,
        closed: true,
        cache: false,
        modal: true,
        iconCls: 'icon-search',
        resizable: false
    });
}
function loadWorkingTeamShiftLogDetailDialog() {
    $('#dlg_WorkingTeamShiftLogDetail').dialog({
        title: '数据项查询',
        width: 500,
        height: 400,
        left: 300,
        top: 20,
        closed: true,
        cache: false,
        modal: true,
        iconCls: 'icon-search',
        resizable: false
    });
}
function loadQuickMenuDetailDialog() {
    $('#dlg_QuickMenuDetail').dialog({
        title: '快捷菜单',
        width: 410,
        height: 400,
        left: 300,
        top: 20,
        closed: true,
        cache: false,
        modal: true,
        iconCls: 'icon-search',
        resizable: false
    });
}
//////////////////////数据查询////////////////////
function GetElectricityQuantityDetail(myObject, myVariableId, myOrganizationType, myDialogTitle) {
    GetElectricityQuantityDetailData(myVariableId, myOrganizationType);
    $('#dlg_ElectricityQuantityDetail').panel("setTitle", $(myObject).text() + myDialogTitle);
    $('#dlg_ElectricityQuantityDetail').dialog('open');
}
function GetElectricityConsumptionDetail(myObject, myVariableId, myOutputVariableId, myOrganizationType, myDialogTitle) {
    GetElectricityConsumptionDetailData(myVariableId, myOutputVariableId, myOrganizationType);
    $('#dlg_ElectricityConsumptionDetail').panel("setTitle", $(myObject).text() + myDialogTitle);
    $('#dlg_ElectricityConsumptionDetail').dialog('open');
}
//专为计算余热吨熟料发电量
function GetElectricityConsumptionDetailYR(myObject, myVariableId, myOutputVariableId, myOrganizationType, myDialogTitle) {
    GetElectricityConsumptionDetailDataYR(myVariableId, myOutputVariableId, myOrganizationType);
    $('#dlg_ElectricityConsumptionDetailYR').panel("setTitle", $(myObject).text());
    $('#dlg_ElectricityConsumptionDetailYR').dialog('open');
}
function GetElectricityConsumptionCDetail(myObject, myOrganizationType, myDialogTitle) {
    GetElectricityConsumptionCDetailData(myOrganizationType);
    $('#dlg_ElectricityConsumptionCDetail').panel("setTitle", $(myObject).text() + myDialogTitle);
    $('#dlg_ElectricityConsumptionCDetail').dialog('open');
}
function GetMaterialWeightDetail(myObject, myVariableId, myValueType, myOrganizationType) {
    GetMaterialWeightDetailData(myVariableId, myOrganizationType);
    if (myValueType == "Output") {
        $('#dlg_MaterialWeightDetail').panel("setTitle", $(myObject).text() + "产量");
    }
    else if (myValueType == "Input") {
        $('#dlg_MaterialWeightDetail').panel("setTitle", $(myObject).text() + "量");
    }
    $('#dlg_MaterialWeightDetail').dialog('open');
}
function GetRunIndictorsDetail(myObject, myEquipmentCommonId, myDialogTitle) {
    GetRunIndictorsDetailData(myEquipmentCommonId, "台时产量,运转率,可靠性,故障率,运转时间");
    $('#dlg_RunIndictorsDetail').panel("setTitle", $(myObject).text().substring(1) + "运行指标");
    $('#dlg_RunIndictorsDetail').dialog('open');
}
function GetMasterMachineHaltDetail(myObject, myEquipmentCommonId, myDialogTitle) {
    GetEquipmentHaltDetailData(myEquipmentCommonId);
    $('#dlg_MasterMachineHaltDetail').panel("setTitle", $(myObject).text().substring(1) + "设备停机");
    $('#dlg_MasterMachineHaltDetail').dialog('open');
}
function GetWorkingTeamShiftLogDetail(myRowData) {
    var m_WorkingTeamShiftLogId = myRowData["WorkingTeamShiftLogId"];
    $('#input_WorkingTeamShiftName').textbox("setValue", myRowData["WorkingTeam"]);
    $('#input_WorkingTeamShiftMonitor').textbox("setValue", myRowData["WorkingTeamShiftMonitor"]);
    $('#input_WorkingTeamShiftUpdateTime').textbox("setValue", myRowData["UpdateDate"]);
    GetWorkingTeamShiftLogDetailData(m_WorkingTeamShiftLogId);
    $('#dlg_WorkingTeamShiftLogDetail').panel("setTitle", "交接班记录");
    $('#dlg_WorkingTeamShiftLogDetail').dialog('open');
}
///////////////快捷菜单/////////////////
function GetQuickMenuDetail(myPageType) {
    //alert(myPageType);

    $.ajax({
        type: "POST",
        url: "View_OverView_Factory.aspx/GetQuickContent",
        data: '{myGroupKey:"' + myPageType + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData != null && m_MsgData != undefined) {
                $('#grid_QuickMenuDetail').datagrid('loadData', m_MsgData);
                $('#dlg_QuickMenuDetail').dialog('open');
            }
        }
    });
}
function AddNewPage(rowIndex, rowData) {
    window.parent.frames.LeftButtonNavigator = rowData["NodePath"];
    window.parent.frames.AddTabFrame(rowData["NavigateUrl"], rowData["PageId"], rowData["Name"], rowData["IconPath"]);
}
////////////页面数据查询/////////////
function GetElectricityQuantityData(myVarialbeIdList, myOrganizationType, myChartName) {
    //var m_VariableId = "'rawMaterialsPreparation','clinkerPreparation','cementPreparation','cementPacking','auxiliaryProduction'";
    $.ajax({
        type: "POST",
        url: "View_OverView_Factory.aspx/GetElectricityQuantitiy",
        data: '{myVariableIdList:"' + myVarialbeIdList + '",myOrganizationId:"' + FactoryOrganizationId + '",myOrganizationType:"' + myOrganizationType + '",myDatetime:"' + SelectedDateString + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData != null && m_MsgData != undefined) {
                for (var i = 0; i < m_MsgData["rows"].length; i++) {
                    var m_DomDayObj = $("#" + m_MsgData["rows"][i].BalanceVariableId + "_DayGlobal");
                    var m_DomMonthObj = $("#" + m_MsgData["rows"][i].BalanceVariableId + "_MonthGlobal");
                    if (m_DomDayObj != null && m_DomDayObj != undefined) {
                        m_DomDayObj.text(m_MsgData["rows"][i].DayElectricityQuantity);
                    }
                    if (m_DomMonthObj != null && m_DomMonthObj != undefined) {
                        m_DomMonthObj.text(m_MsgData["rows"][i].MonthElectricityQuantity);
                    }
                }
            }
            if (myChartName == "ElectricPercentageChart") {
                ElectricPercentageLoadCount = ElectricPercentageLoadCount + 1;
                if (ElectricPercentageLoadCount == 2) {
                    LoadElectricPercentageData();                  //获取饼图数据
                }
            }
        }
    });
}

function GetElectricityConsumptionData(myVarialbeIdList, myOutputVariableIdList, myOrganizationType) {
    //var m_VariableId = "'rawMaterialsPreparation','clinkerPreparation','cementPreparation','cementPacking','auxiliaryProduction'";
    $.ajax({
        type: "POST",
        url: "View_OverView_Factory.aspx/GetElectricityConsumption",
        data: '{myVariableIdList:"' + myVarialbeIdList + '",myOutputVariableIdList:"' + myOutputVariableIdList + '",myOrganizationId:"' + FactoryOrganizationId + '",myOrganizationType:"' + myOrganizationType + '",myDatetime:"' + SelectedDateString + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData != null && m_MsgData != undefined) {
                for (var i = 0; i < m_MsgData["rows"].length; i++) {
                    var m_DomDayObj = $("#" + m_MsgData["rows"][i].BalanceVariableId + "_ElectricityConsumption_DayGlobal");
                    var m_DomMonthObj = $("#" + m_MsgData["rows"][i].BalanceVariableId + "_ElectricityConsumption_MonthGlobal");
                    if (m_DomDayObj != null && m_DomDayObj != undefined) {
                        m_DomDayObj.text(m_MsgData["rows"][i].DayElectricityConsumption);
                    }
                    if (m_DomMonthObj != null && m_DomMonthObj != undefined) {
                        m_DomMonthObj.text(m_MsgData["rows"][i].MonthElectricityConsumption);
                    }
                }
            }
        }
    });
}
//计算综合能耗
function GetElectricityConsumptionCData() {
    $.ajax({
        type: "POST",
        url: "View_OverView_Factory.aspx/GetEnergyConsumptionComprehensive",
        data: '{myOrganizationId:"' + FactoryOrganizationId + '",myDatetime:"' + SelectedDateString + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData != null && m_MsgData != undefined) {
                if (m_MsgData["rows"].length > 0) {
                    $('#Clincker_ElectricityConsumptionComprehensive_MonthGlobal').text(m_MsgData["rows"][0].ElectricityConsumption);
                    $('#Clincker_CoalConsumptionComprehensive_MonthGlobal').text(m_MsgData["rows"][0].CoalConsumption);
                    $('#Clincker_EnergyConsumptionComprehensive_MonthGlobal').text(m_MsgData["rows"][0].EnergyConsumption);
                    $('#Cementmill_ElectricityConsumptionComprehensive_MonthGlobal').text(m_MsgData["rows"][1].ElectricityConsumption);
                    //$('#Cementmill_CoalConsumptionComprehensive_MonthGlobal').text(m_MsgData["rows"][0].CoalConsumption);
                    $('#Cementmill_EnergyConsumptionComprehensive_MonthGlobal').text(m_MsgData["rows"][1].EnergyConsumption);
                }
            }
        }
    });
}
function GetMaterialWeightData(myVarialbeIdList, myOrganizationType) {
    //var m_VariableId = "'rawMaterialsPreparation','clinkerPreparation','cementPreparation','cementPacking','auxiliaryProduction'";
    $.ajax({
        type: "POST",
        url: "View_OverView_Factory.aspx/GetMaterialWeightData",
        data: '{myVariableIdList:"' + myVarialbeIdList + '",myOrganizationId:"' + FactoryOrganizationId + '",myOrganizationType:"' + myOrganizationType + '",myDatetime:"' + SelectedDateString + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData != null && m_MsgData != undefined) {
                for (var i = 0; i < m_MsgData["rows"].length; i++) {
                    var m_DomDayObj = $("#" + m_MsgData["rows"][i].VariableId + "_DayGlobal");
                    var m_DomMonthObj = $("#" + m_MsgData["rows"][i].VariableId + "_MonthGlobal");
                    if (m_DomDayObj != null && m_DomDayObj != undefined) {
                        m_DomDayObj.text(m_MsgData["rows"][i].DayMaterialWeight);
                    }
                    if (m_DomMonthObj != null && m_DomMonthObj != undefined) {
                        m_DomMonthObj.text(m_MsgData["rows"][i].MonthMaterialWeight);
                    }
                }
            }
        }
    });
}
function GetInventoryData()
{
    $.ajax({
        type: "POST",
        url: "View_OverView_Factory.aspx/GetInventoryData",
        data: '{myOrganizationId:"' + FactoryOrganizationId + '",myDatetime:"' + SelectedDateString + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData != null && m_MsgData != undefined) {
                for (var key in m_MsgData) {
                    var m_ObjTemp = $('#' + key);
                    if (m_ObjTemp != null && m_ObjTemp != undefined) {
                        m_ObjTemp.html(m_MsgData[key].toFixed(2));
                        if(m_ObjTemp.data("options") != undefined)   // 设置上下限背景色
                        {
                            //先设置背景色为无色
                            m_ObjTemp.css("background-Color", "transparent");
                            if (m_ObjTemp.data("options").HighLimit != undefined && m_ObjTemp.data("options").HighLimit != "null") {   //设置了上限
                                if (parseFloat(m_MsgData[key]) > parseFloat(m_ObjTemp.data("options").HighLimit)) {
                                    //上限报警红色
                                    m_ObjTemp.css("background-Color", "red");
                                }
                            }
                            if (m_ObjTemp.data("options").LowLimit != undefined && m_ObjTemp.data("options").LowLimit != "null") {     //设置了下限
                                if (parseFloat(m_MsgData[key]) < parseFloat(m_ObjTemp.data("options").LowLimit)) {
                                    //下限报警红色
                                    m_ObjTemp.css("background-Color", "yellow");
                                }
                            }
                            
                        }
                        else{
                            //设置背景色为无色
                            m_ObjTemp.css("background-Color", "transparent");
                        }

                    }
                }

            }
        }
    });
}
function GetRunIndictorsData(myRunIndictorsList, myEquipmentCommonIdList) {

    $.ajax({
        type: "POST",
        url: "View_OverView_Factory.aspx/GetRunIndictors",
        data: '{myRunIndictorsList:"' + myRunIndictorsList + '",myEquipmentCommonIdList:"' + myEquipmentCommonIdList + '",myFactoryOrganizationId:"' + FactoryOrganizationId + '",myDatetime:"' + SelectedDateString + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            var m_RunIndictorsList = myRunIndictorsList.split(',');
            if (m_MsgData != null && m_MsgData != undefined) {
                for (var i = 0; i < m_MsgData["rows"].length; i++) {
                    for (var j = 0; j < m_RunIndictorsList.length; j++) {
                        var m_DomObj = $("#" + m_MsgData["rows"][i]["EquipmentCommonId"] + "_" + m_RunIndictorsList[j] + "_Global");
                        if (m_DomObj != null && m_DomObj != undefined) {
                            m_DomObj.text(m_MsgData["rows"][i][m_RunIndictorsList[j]]);
                        }
                    }
                    
                }
            }
        }
    });
}
function GetEquipmentHaltData(myEquipmentCommonIdList) {
    var m_StatisticalRange = $('#Select_StatisticalRangeF').combobox('getValue');
    $.ajax({
        type: "POST",
        url: "View_OverView_Factory.aspx/GetEquipmentHalt",
        data: '{myEquipmentCommonIdList:"' + myEquipmentCommonIdList + '",myFactoryOrganizationId:"' + FactoryOrganizationId + '",myStatisticalRange:"' + m_StatisticalRange + '",myDatetime:"' + SelectedDateString + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData != null && m_MsgData != undefined) {
                for (var i = 0; i < m_MsgData["rows"].length; i++) {
                    var m_DomCountObj = $("#" + m_MsgData["rows"][i].EquipmentCommonId + "_CountGlobal");
                    var m_DomTimeObj = $("#" + m_MsgData["rows"][i].EquipmentCommonId + "_TimeGlobal");
                    if (m_DomCountObj != null && m_DomCountObj != undefined) {
                        m_DomCountObj.text(m_MsgData["rows"][i].DowntimeCount);
                    }
                    if (m_DomTimeObj != null && m_DomTimeObj != undefined) {
                        m_DomTimeObj.text(m_MsgData["rows"][i].DowntimeTime);
                    }
                }
                LoadDownTimeCountPercentageData(m_MsgData);
            }
        }
    });
}
function GetProductSaleData(myMaterialIds)
{
    $.ajax({
        type: "POST",
        url: "View_OverView_Factory.aspx/GetProductSaleData",
        data: '{myOrganizationId:"' + FactoryOrganizationId + '",myMaterialIds:"' + myMaterialIds + '",myDatetime:"' + SelectedDateString + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            $('#datagrid_SaleInfo').datagrid('loadData', m_MsgData);
        },
        error: function (e) {
        }
    });
}

function GetEquipmentHaltAlarm() {
    $.ajax({
        type: "POST",
        url: "View_OverView_Factory.aspx/GetEquipmentHaltAlarm",
        data: '{myOrganizationId:"' + FactoryOrganizationId + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            $('#datagrid_EquipmentHaltAlarm').datagrid('loadData', m_MsgData);
            setTimeout(GetEquipmentHaltAlarm, RuntimeRefreshInterval);
        },
        error: function (e) {
            setTimeout(GetEquipmentHaltAlarm, RuntimeRefreshInterval);
        }
    });
}

function GetWorkingTeamShiftLog() {
    $.ajax({
        type: "POST",
        url: "View_OverView_Factory.aspx/GetWorkingTeamShiftLog",
        data: '{myOrganizationId:"' + FactoryOrganizationId + '",myDatetime:"' + SelectedDateString + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            $('#datagrid_WorkingTeamShiftLogList').datagrid('loadData', m_MsgData);
            setTimeout(GetWorkingTeamShiftLog, RuntimeRefreshInterval);
        },
        error: function (e) {
            setTimeout(GetWorkingTeamShiftLog, RuntimeRefreshInterval);
        }
    });
}
///////////////////////////数据查询查询后台方法////////////////////

function GetElectricityQuantityDetailData(myVariableId, myOrganizationType) {
    $.ajax({
        type: "POST",
        url: "View_OverView_Factory.aspx/GetElectricityQuantitiyDetail",
        data: '{myVariableId:"' + myVariableId + '",myOrganizationId:"' + FactoryOrganizationId + '",myOrganizationType:"' + myOrganizationType + '",myDatetime:"' + SelectedDateString + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            $('#grid_ElectricityQuantityDetail').datagrid('loadData', m_MsgData);
        }
    });
}
function GetElectricityConsumptionDetailData(myVariableId, myOutputVariableId, myOrganizationType) {
    $.ajax({
        type: "POST",
        url: "View_OverView_Factory.aspx/GetElectricityConsumptionDetail",
        data: '{myVariableId:"' + myVariableId + '",myOutputVariableId:"' + myOutputVariableId + '",myOrganizationId:"' + FactoryOrganizationId + '",myOrganizationType:"' + myOrganizationType + '",myDatetime:"' + SelectedDateString + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            $('#grid_ElectricityConsumptionDetail').datagrid('loadData', m_MsgData);
        }
    });
}
//计算综合能耗
function GetElectricityConsumptionCDetailData(myOrganizationType) {
    $.ajax({
        type: "POST",
        url: "View_OverView_Factory.aspx/GetEnergyConsumptionComprehensiveDetail",
        data: '{myOrganizationId:"' + FactoryOrganizationId + '",myOrganizationType:"' + myOrganizationType + '",myDatetime:"' + SelectedDateString + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            $('#grid_ElectricityConsumptionCDetail').datagrid('loadData', m_MsgData);
        }
    });
}
//专为余热计算吨熟料发电量
function GetElectricityConsumptionDetailDataYR(myVariableId, myOutputVariableId, myOrganizationType) {
    $.ajax({
        type: "POST",
        url: "View_OverView_Factory.aspx/GetElectricityConsumptionDetailYR",
        data: '{myVariableId:"' + myVariableId + '",myOutputVariableId:"' + myOutputVariableId + '",myOrganizationId:"' + FactoryOrganizationId + '",myOrganizationType:"' + myOrganizationType + '",myDatetime:"' + SelectedDateString + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            $('#grid_ElectricityConsumptionDetailYR').datagrid('loadData', m_MsgData);
        }
    });
}

function GetMaterialWeightDetailData(myVariableId, myOrganizationType) {
    $.ajax({
        type: "POST",
        url: "View_OverView_Factory.aspx/GetMaterialWeightDetail",
        data: '{myVariableId:"' + myVariableId + '",myOrganizationId:"' + FactoryOrganizationId + '",myOrganizationType:"' + myOrganizationType + '",myDatetime:"' + SelectedDateString + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            $('#grid_MaterialWeightDetail').datagrid('loadData', m_MsgData);
        }
    });
}

function GetRunIndictorsDetailData(myEquipmentCommonId, myRunIndictorsList) {
    $.ajax({
        type: "POST",
        url: "View_OverView_Factory.aspx/GetRunIndictorsDetail",
        data: '{myEquipmentCommonId:"' + myEquipmentCommonId + '",myFactoryOrganizationId:"' + FactoryOrganizationId + '",myRunIndictorsList:"' + myRunIndictorsList + '",myDatetime:"' + SelectedDateString + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            $('#grid_RunIndictorsDetail').datagrid('loadData', m_MsgData);
        }
    });
}
function GetEquipmentHaltDetailData(myEquipmentCommonId) {
    var m_StatisticalRange = $('#Select_StatisticalRangeF').combobox('getValue');
    $.ajax({
        type: "POST",
        url: "View_OverView_Factory.aspx/GetEquipmentHaltDetail",
        data: '{myEquipmentCommonId:"' + myEquipmentCommonId + '",myFactoryOrganizationId:"' + FactoryOrganizationId + '",myStatisticalRange:"' + m_StatisticalRange + '",myDatetime:"' + SelectedDateString + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            $('#grid_MasterMachineHaltDetail').datagrid('loadData', m_MsgData);
        }
    });
}

function GetWorkingTeamShiftLogDetailData(myWorkingTeamShiftLogId) {
    $.ajax({
        type: "POST",
        url: "View_OverView_Factory.aspx/GetWorkingTeamShiftLogDetail",
        data: '{myOrganizationId:"' + FactoryOrganizationId + '",myWorkingTeamShiftLogId:"' + myWorkingTeamShiftLogId + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $('#TextArea_WorkingTeamShiftLog').text(msg.d);
        }
    });
}
///////////////////////////选择下拉框后执行的方法//////////////////
function GetEquipmentHaltDataFunction() {
    GetEquipmentHaltData("MineCrusher,RawMaterialsGrind,CoalGrind,RotaryKiln,CementGrind,CementPacker");
}
function RefreshStatisticalRange(myRecord) {
    //alert(myRecord);
    GetEquipmentHaltDataFunction();
}
function RefreshFactoryOrganiztion(myOrganizationId) {
    ElectricPercentageLoadCount = 0;
    if (myOrganizationId != "" && myOrganizationId != undefined) {
        FactoryOrganizationId = $('#Select_SelectStation').combobox('getValue');

        GetElectricityQuantityData("rawMaterialsHomogenize,rawMaterialsGrind,coalPreparation,clinkerBurning,kilnSystem,hybridMaterialsPreparation,clinkerTransport,cementGrind", "熟料,水泥磨", "ElectricPercentageChart");
        GetElectricityQuantityData("limestoneMine,auxiliaryProduction,cementPacking", "分厂", "ElectricPercentageChart");
        GetElectricityQuantityData("clinkerElectricityGeneration,electricityOutput,electricityOwnDemand", "余热发电");
        GetElectricityConsumptionData("clinkerElectricityGeneration", "clinker_ClinkerOutput", "熟料");       //余热发电吨熟料发电量
        GetElectricityConsumptionData("rawMaterialsHomogenize,rawMaterialsGrind,coalPreparation,clinkerBurning,kilnSystem,hybridMaterialsPreparation,clinkerTransport,cementGrind,limestoneMine,auxiliaryProduction,cementPacking",
                                      "clinker_MixtureMaterialsOutput,clinker_MixtureMaterialsOutput,clinker_PulverizedCoalOutput,clinker_ClinkerOutput,clinker_ClinkerOutput,cement_CementOutput,cement_CementOutput,cement_CementOutput,clinker_MixtureMaterialsOutput,cement_CementOutput,cement_CementOutput", "熟料,水泥磨");
        GetElectricityConsumptionCData();      //计算综合电耗
        GetMaterialWeightData("clinker_ClinkerOutput,cement_CementOutput,clinker_PulverizedCoalOutput,clinker_MixtureMaterialsOutput,clinker_LimestoneInput,clinker_MixtureMaterialsInput,clinker_PulverizedCoalInput,clinker_ClinkerInput", "熟料,水泥磨");

        GetRunIndictorsData("台时产量,运转率,可靠性,故障率,运转时间", "MineCrusher,RawMaterialsGrind,CoalGrind,RotaryKiln,CementGrind,CementPacker");                            //运转率等指标
        GetEquipmentHaltDataFunction();           //设备停机记录

        GetEquipmentHaltAlarm();          //报警记录
        GetWorkingTeamShiftLog();         //交接班记录

        GetElectricitiyConsumptionChart("rawMaterialsPreparation,clinkerPreparation,cementPreparation", "熟料,水泥磨");
        GetReliabilityChart("MineCrusher,RawMaterialsGrind,CoalGrind,RotaryKiln,CementGrind,CementPacker");
        GetHaltRateChart("MineCrusher,RawMaterialsGrind,CoalGrind,RotaryKiln,CementGrind,CementPacker");
        GetRunningRateChart("MineCrusher,RawMaterialsGrind,CoalGrind,RotaryKiln,CementGrind,CementPacker");
        GetProductSaleData("Clinker,Cement");               //产品销售
        GetInventoryData();                         //库存记录
    }
    else {
        FactoryOrganizationId = "";
        ClearFactoryData();
    }
}
function ClearFactoryData() {

}


function handleError() {
    $('#gridMain_ReportTemplate').datagrid('loadData', []);
    $.messager.alert('失败', '获取数据失败');
}

