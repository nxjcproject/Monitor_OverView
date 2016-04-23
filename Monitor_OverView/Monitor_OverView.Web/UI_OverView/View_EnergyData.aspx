<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="View_EnergyData.aspx.cs" Inherits="Monitor_OverView.Web.UI_OverView.View_EnergyData" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>能源数据</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css" />

    <link rel="stylesheet" type="text/css" href="/lib/pllib/themes/jquery.jqplot.min.css" />
    <link type="text/css" rel="stylesheet" href="/lib/pllib/syntaxhighlighter/styles/shCoreDefault.min.css" />
    <link type="text/css" rel="stylesheet" href="/lib/pllib/syntaxhighlighter/styles/shThemejqPlot.min.css" />
    <link type="text/css" rel="stylesheet" href="/css/common/charts.css" />
    <link type="text/css" rel="stylesheet" href="/css/common/NormalPage.css" />
    <%--<link type="text/css" rel="stylesheet" href="/UI_ComprehensiveDailyReport/css/page/DispatchDailyReport.css" />--%>
    <link rel="stylesheet" type="text/css" href="/UI_OverView/css/page/Style_SelectButton.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <!--[if lt IE 9]><script type="text/javascript" src="/lib/pllib/excanvas.js"></script><![endif]-->
    <script type="text/javascript" src="/lib/pllib/jquery.jqplot.min.js"></script>
    <!--<script type="text/javascript" src="/lib/pllib/syntaxhighlighter/scripts/shCore.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/syntaxhighlighter/scripts/shBrushJScript.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/syntaxhighlighter/scripts/shBrushXml.min.js"></script>-->

    <!-- Additional plugins go here -->
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.barRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.pieRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.categoryAxisRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.pointLabels.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.cursor.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.canvasTextRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.canvasAxisTickRenderer.min.js"></script>

    <%--<script type="text/javascript" src="/lib/pllib/themes/jquery.jqplot.js"></script>
    <script type="text/javascript" src="/lib/pllib/themes/jjquery.jqplot.min.js"></script>--%>
    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->

    <script type="text/javascript" src="/js/common/components/Charts.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/components/DataGrid.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/components/WindowsDialog.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/components/GridChart.js" charset="utf-8"></script>
    <script type="text/javascript" src="js/page/View_EnergyData.js"></script>
    <script type="text/javascript" src="js/page/View_SelectButton.js" charset="utf-8"></script>

</head>
<body>
    <div id="MainLayout" class="easyui-layout" data-options="fit:true,border:false">
        <div data-options="region:'center',border:false">
            <div class="easyui-layout" data-options="fit:true,border:false">
                <div data-options="region:'center',border:false" style="padding-bottom: 5px;">
                    <div id="QueryTools">
                        <input id="dateTime" type="text" class="easyui-datebox" required="required" />
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true"
                            onclick="QueryReportFun();">查询</a>
                    </div>
                    <table id="GlobalCompleteGridId" class="easyui-datagrid" data-options="fit:true,border:true">
                        <thead data-options="frozen:true">
                            <tr>
                                <th data-options="field:'OrganizationId',width:120,hidden: true">组织机构</th>
                                <th data-options="field:'LevelCode',width:100,hidden: true">层次码</th>
                                <th data-options="field:'Name',width:100,fit:true,align:'center',headalign:'center'">名称</th>
                            </tr>
                        </thead>
                        <thead>
                            <tr>

                                <th data-options="field:'Company_ElectricityQuantity',width:85, align:'right',headalign:'center'">总用电量(kWh)</th>
                                <th data-options="field:'clinkerElectricityGeneration_ElectricityQuantity',width:100, align:'right',headalign:'center'">余热发电量(kWh)</th>
                                <th data-options="field:'auxiliaryProduction_ElectricityQuantity',width:100, align:'right',headalign:'center'">辅助电量(kWh)</th>
                                <th data-options="field:'clinkerBurning_ElectricityQuantity',width:110, align:'right',headalign:'center'">熟料烧成电量(kWh)</th>
                                <th data-options="field:'rawMaterialsPreparation_ElectricityQuantity',width:85, align:'right',headalign:'center'">生料制备电量(kWh)</th>
                                <th data-options="field:'coalPreparation_ElectricityQuantity',width:100, align:'right',headalign:'center'">煤粉制备电量(kWh)</th>
                                <th data-options="field:'cementPreparation_ElectricityQuantity',width:100, align:'right',headalign:'center'">水泥制备电量(kWh)</th>
                                <th data-options="field:'cementPacking_ElectricityQuantity',width:110, align:'right',headalign:'center'">水泥包装电量(kWh)</th>
                            </tr>
                        </thead>
                    </table>
                </div>
                <div data-options="region:'south',border:false" style="height: 270px;">
                    <div class="easyui-layout" data-options="fit:true,border:false">
                        <div class="easyui-panel" data-options="region:'north', border:false, collapsible:false, split:false" style="height: 25px;">
                            <table>
                                <tr>
                                    <th>选择指标</th>
                                    <td>
                                        <input id="Combobox_StandardF" class="easyui-combobox" style="height: 23px; width: 160px" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div id="Windows_Container" class="easyui-panel" data-options="region:'center', border:false, collapsible:false, split:false">
                        </div>
                    </div>

                </div>
            </div>
        </div>
        <div data-options="region:'east',border:false" style="width: 440px; padding-left: 5px;">
            <div class="easyui-layout" data-options="fit:true,border:false">
                <div data-options="region:'north',border:false" style="height: 230px;">
                    <table id="CompanyComprehensiveCompleteGridId" data-options="fit:true,border:true" class="easyui-datagrid">
                        <thead>
                            <tr>
                                <th data-options="width:70, align:'center',headalign:'center'" colspan="2">能源消耗</th>
                                <th data-options="field:'Value_Day',width:60, align:'right',headalign:'center'" rowspan="2">日完成</th>
                                <th data-options="field:'Value_Month',width:60, align:'right',headalign:'center'" rowspan="2">月完成</th>
                                <th data-options="field:'Value_Plan',width:60, align:'right',headalign:'center'" rowspan="2">月计划</th>
                                <th data-options="field:'Value_Deviation',width:60, align:'right',headalign:'center'" rowspan="2">月差值</th>
                            </tr>
                            <tr>
                                <th data-options="field:'Name',width:70,align:'center',headalign:'center'">产品</th>
                                <th data-options="field:'DataItem',width:70,align:'center',headalign:'center'">项目</th>
                            </tr>
                        </thead>
                    </table>
                </div>
                <div data-options="region:'center',border:false" style="padding-top: 5px">
                    <table id="CompanyProcessCompleteGridId" data-options="fit:true,border:true" class="easyui-datagrid">
                        <thead>
                            <tr>
                                <th data-options="width:70, align:'center',headalign:'center'" colspan="2">分步电耗</th>
                                <th data-options="field:'Value_Day',width:60, align:'right',headalign:'center'" rowspan="2">日完成</th>
                                <th data-options="field:'Value_Month',width:60, align:'right',headalign:'center'" rowspan="2">月完成</th>
                                <th data-options="field:'Value_Plan',width:60, align:'right',headalign:'center'" rowspan="2">月计划</th>
                                <th data-options="field:'Value_Deviation',width:60, align:'right',headalign:'center'" rowspan="2">月差值</th>
                            </tr>
                            <tr>
                                <th data-options="field:'Name',width:70,align:'center',headalign:'center'">名称</th>
                                <th data-options="field:'DataItem',width:70,align:'center',headalign:'center'">项目</th>
                            </tr>
                        </thead>
                    </table>
                </div>
            </div>
        </div>
        <div data-options="region:'south',border:false" style="height: 20px; text-align: center; vertical-align: middle;">
            <table>
                <tr>
                    <td id="LeftBlankWidth"></td>
                    <td id="ComprehensiveDailyTd" class="SelectButtonTd" onclick="ChageOtherPage('View_OverView_nxjc');">综合日报</td>
                    <td id="ProductionDataTd" class="SelectButtonTd" onclick="ChageOtherPage('View_ProductionData');">生产数据</td>
                    <td id="EnergyDataTd" class="SelectedButtonTd" onclick="ChageOtherPage('View_EnergyData');">能源数据</td>
                </tr>
            </table>
        </div>
    </div>
    <form id="form1" runat="server">
        <div>
        </div>
    </form>
</body>
</html>
