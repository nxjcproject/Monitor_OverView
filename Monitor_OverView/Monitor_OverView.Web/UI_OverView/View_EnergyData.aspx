﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="View_EnergyData.aspx.cs" Inherits="Monitor_OverView.Web.UI_OverView.View_EnergyData" %>

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
    <link type="text/css" rel="stylesheet" href="/css/common/NormalPage.css" />

    <link rel="stylesheet" type="text/css" href="/UI_OverView/css/page/Style_SelectButton.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <!--[if gt IE 8]><script type="text/javascript" src="/lib/ealib/extend/easyUI.WindowsOverrange.js" charset="utf-8"></script>-->
    <!--[if !IE]>
    <script type="text/javascript" src="/lib/ealib/extend/easyUI.WindowsOverrange.js" charset="utf-8"></script>
    <![endif]-->

    <!--[if lt IE 9]><script type="text/javascript" src="/lib/pllib/excanvas.js"></script><![endif]-->
    <script type="text/javascript" src="/lib/pllib/jquery.jqplot.min.js"></script>

    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.trendline.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.barRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.pieRenderer.min.js"></script>

    <!-- Additional plugins go here -->
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.canvasAxisTickRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.categoryAxisRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.canvasTextRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.canvasAxisLabelRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.dateAxisRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.pointLabels.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.enhancedLegendRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.enhancedPieLegendRenderer.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.canvasOverlay.min.js"></script> 
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.cursor.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.highlighter.min.js"></script>
    <!--[if lt IE 8 ]><script type="text/javascript" src="/lib/pllib/plugins/jqplot.json2.min"></script><![endif]-->


    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->

    <script type="text/javascript" src="/js/common/components/Charts.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/components/DataGrid.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/components/WindowsDialog.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/components/GridChart.js" charset="utf-8"></script>

    <script type="text/javascript" src="/js/common/format/DateTimeFormat.js" charset="utf-8"></script>

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
                    <table id="GlobalCompleteGridId" class="easyui-datagrid" data-options="fit:true,border:true,title: '', striped: true, rownumbers: true, singleSelect: true, toolbar: '#QueryTools'
                        ,onDblClickRow: function (rowIndex, rowData) {onRowDblClick(rowData);}">
                        <thead data-options="frozen:true">
                            <tr>
                                <th data-options="field:'OrganizationId',width:120,hidden: true">组织机构</th>
                                <th data-options="field:'LevelCode',width:100,hidden: true">层次码</th>
                                <th data-options="field:'Name',width:100,fit:true,align:'center',headalign:'center'" rowspan="2">名称</th>
                            </tr>
                            <tr>
                                <th data-options="field:'OrganizationId',width:120,hidden: true"></th>
                                <th data-options="field:'LevelCode',width:100,hidden: true"></th>
                            </tr>
                        </thead>
                        <thead>
                            <tr>
                                <th data-options="align:'right',headalign:'center'," colspan="7">电量(kWh)</th>
                                <th data-options="align:'right',headalign:'center'" colspan="3">分步电耗(kWh/t)</th>
                            </tr>
                            <tr>
                                <th data-options="field:'Company_ElectricityQuantity',width:80, align:'right',headalign:'center'">总用电量</th>
                                <th data-options="field:'clinkerElectricityGeneration_ElectricityQuantity',width:80, align:'right',headalign:'center'">余热发电量</th>
                                <th data-options="field:'auxiliaryProduction_ElectricityQuantity',width:80, align:'right',headalign:'center'">辅助电量</th>
                                <th data-options="field:'cementPacking_ElectricityQuantity',width:80, align:'right',headalign:'center'">水泥包装</th>
                                <th data-options="field:'rawMaterialsPreparation_ElectricityQuantity',width:80, align:'right',headalign:'center'">生料制备</th>
                                <th data-options="field:'clinkerPreparation_ElectricityQuantity',width:80, align:'right',headalign:'center'">熟料制备</th>
                                <th data-options="field:'cementPreparation_ElectricityQuantity',width:80, align:'right',headalign:'center'">水泥制备</th>
                                <th data-options="field:'rawMaterialsPreparation_ElectricityConsumption',width:70, align:'right',headalign:'center'">生料制备</th>
                                <th data-options="field:'clinkerPreparation_ElectricityConsumption',width:70, align:'right',headalign:'center'">熟料制备</th>
                                <th data-options="field:'cementPreparation_ElectricityConsumption',width:70, align:'right',headalign:'center'">水泥制备</th>
                            </tr>
                        </thead>
                    </table>
                </div>
                <div data-options="region:'south',border:false" style="height: 280px;">
                    <div class="easyui-layout" data-options="fit:true,border:false">
                        <div class="easyui-panel" data-options="region:'north', border:false, collapsible:false, split:false" style="height: 27px;">
                            <table>
                                <tr>
                                    <th>选择指标</th>
                                    <td>
                                        <input id="Combobox_StandardF" class="easyui-combobox" style="height: 23px; width: 150px" />
                                    </td>
                                    <th style="width: 60px;">规格型号</th>
                                    <td style="width: 80px;">
                                        <select id="Combobox_SpecificationsF" class="easyui-combobox" name="Combobox_SpecificationsF" data-options="panelHeight:'auto', editable:false, valueField: 'id',textField: 'text'" style="width: 70px;"></select>
                                    </td>
                                    <td style="width: 60px;">
                                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true"
                                            onclick="LoadPlanAndCompleteChartButton();">查询</a>
                                    </td>
                                    <th id="Th_ChartPageIndexF" style="visibility:hidden;">页码</th>
                                    <td id="Td_ChartPageIndexF" style="visibility:hidden;">
                                        <select id="Combobox_PageIndexF" class="easyui-combobox" name="Combobox_PageIndexF" data-options="panelHeight:'auto', editable:false, valueField: 'id',textField: 'text',onSelect:function(myRecord){ChangeChartPageIndex(myRecord);}" style="width: 50px;"></select>
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
                    <table id="CompanyComprehensiveCompleteGridId" data-options="fit:true,border:true, title: '公司名称',striped: true, rownumbers: true, singleSelect: true, idField: 'id'" class="easyui-datagrid">
                        <thead>
                            <tr>
                                <th data-options="width:165, align:'center',headalign:'center'" colspan="2">能源消耗</th>
                                <th data-options="field:'Value_Day',width:55, align:'right',headalign:'center'" rowspan="2">日完成</th>
                                <th data-options="field:'Value_Month',width:55, align:'right',headalign:'center'" rowspan="2">月完成</th>
                                <th data-options="field:'Value_Plan',width:55, align:'right',headalign:'center'" rowspan="2">月计划</th>
                                <th data-options="field:'Value_Deviation',width:55, align:'right',headalign:'center'" rowspan="2">月差值</th>
                            </tr>
                            <tr>
                                <th data-options="field:'Name',width:70,align:'center',headalign:'center'">产品</th>
                                <th data-options="field:'DataItem',width:95,align:'center',headalign:'center'">项目</th>
                            </tr>
                        </thead>
                    </table>
                </div>
                <div data-options="region:'center',border:false" style="padding-top: 5px">
                    <table id="CompanyProcessCompleteGridId" data-options="fit:true,border:true, title: '',striped: true, rownumbers: true, singleSelect: true" class="easyui-datagrid">
                        <thead>
                            <tr>
                                <th data-options="width:165, align:'center',headalign:'center'" colspan="3">工序电耗</th>
                                <th data-options="field:'Value_Day',width:55, align:'right',headalign:'center'" rowspan="2">日完成</th>
                                <th data-options="field:'Value_Month',width:55, align:'right',headalign:'center'" rowspan="2">月完成</th>
                                <th data-options="field:'Value_Plan',width:55, align:'right',headalign:'center'" rowspan="2">月计划</th>
                                <th data-options="field:'Value_Deviation',width:55, align:'right',headalign:'center'" rowspan="2">月差值</th>
                            </tr>
                            <tr>
                                <th data-options="field:'Company',width:70,align:'center',headalign:'center'">公司</th>
                                <th data-options="field:'Name',width:35,align:'center',headalign:'center'">产线</th>
                                <th data-options="field:'DataItem',width:60,align:'center',headalign:'center'">项目</th>
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
