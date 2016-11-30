<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="View_OverView_Factory.aspx.cs" Inherits="Monitor_OverView.Web.UI_OverView.View_OverView_Factory" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>分厂总貌</title>
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
    <link rel="stylesheet" type="text/css" href="css/page/Style_OverView_Factory.css" />

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
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.dateAxisRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.highlighter.min.js"></script>

    



    <%--<script type="text/javascript" src="/lib/pllib/themes/jquery.jqplot.js"></script>
    <script type="text/javascript" src="/lib/pllib/themes/jjquery.jqplot.min.js"></script>--%>
    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->

    <script type="text/javascript" src="/js/common/format/DateTimeFormat.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/components/Charts.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/components/DataGrid.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/components/WindowsDialog.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/components/GridChart.js" charset="utf-8"></script>
    <script type="text/javascript" src="js/page/View_OverView_Factory.js" charset="utf-8"></script>
</head>
<body>
    <div id="MainLayout" class="easyui-layout" data-options="fit:true,border:false">
        <div data-options="region:'center',border:false">
            <table id="MainTable">
                <tr>
                    <td id="MainTablePosizionLeft"></td>
                    <td class="MainTableLeft">
                        <table>
                            <tr>
                                <td id="ChartDataTd" class="LeftTableTd">
                                    <div id="ChartDataTabs" class="easyui-tabs" data-options="fit:false, tabPosition:'top'">
                                        <div title="运转率">
                                            <div id="chartTab01_Content" class="DataChartContent">
                                            </div>
                                            <div id="chartTab01_Legend" class="DataChartLegend">
                                            </div>
                                        </div>
                                        <div title="故障率">
                                            <div id="chartTab02_Content" class="DataChartContent">
                                            </div>
                                            <div id="chartTab02_Legend" class="DataChartLegend">
                                            </div>
                                        </div>
                                        <div title="可靠性">
                                            <div id="chartTab03_Content" class="DataChartContent">
                                            </div>
                                            <div id="chartTab03_Legend" class="DataChartLegend">
                                            </div>
                                        </div>
                                        <div title="分步电耗">
                                            <div id= "chartTab04_Content" class="DataChartContent">
                                            </div>
                                            <div id="chartTab04_Legend" class="DataChartLegend">
                                            </div>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td id="EnergyZoneTitle" class="LeftTableTd">
                                    <table>
                                        <tr>
                                            <td class="LeftMiddleZoneTitleText">能源
                                            </td>
                                            <td class="LeftMiddleZoneTitleLine"><span class ="ExtendPageDetail" onclick ="GetQuickMenuDetail('Energy');">More>>&nbsp;&nbsp;</span></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td id="EnergyQuantityInfoTd" class="LeftTableTd"></td>
                            </tr>
                            <tr>
                                <td id="EnergyConsumptionInfoTd" class="LeftTableTd"></td>
                            </tr>
                            <tr>
                                <td id="EnergyConsumptionComprehensiveInfoTd" class="LeftTableTd"></td>
                            </tr>
                            <tr>
                                <td id="CogenerationInfoTd" class="LeftTableTd"></td>
                            </tr>
                            <tr>
                                <td id="EnergyAlarmTd" class="LeftTableTd"></td>
                            </tr>

                        </table>
                    </td>
                    <td class="MainTableMiddle">
                        <table>
                            <tr>
                                <td class="LeftTableTd">
                                    <table>
                                        <tr>
                                            <td class="LeftMiddleZoneTitleText">生产
                                            </td>
                                            <td class="LeftMiddleZoneTitleLine"><span class ="ExtendPageDetail" onclick ="GetQuickMenuDetail('Production');">More>>&nbsp;&nbsp;</span></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td id="MaterialWeightOutputTd" class="LeftTableTd"></td>
                            </tr>
                            <tr>
                                <td id="MaterialStorageTd" class="LeftTableTd"></td>
                            </tr>
                            <tr>
                                <td class="LeftTableTd">
                                    <table>
                                        <tr>
                                            <td class="LeftMiddleZoneTitleText">设备
                                            </td>
                                            <td class="LeftMiddleZoneTitleLine"><span class ="ExtendPageDetail" onclick ="GetQuickMenuDetail('Equipment');">More>>&nbsp;&nbsp;</span></td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td id="EquipmentRunIndicatorsTd" class="LeftTableTd"></td>
                            </tr>
                            <tr>
                                <td id="MachineHaltRecordTd" class="LeftTableTd"></td>
                            </tr>
                        </table>
                    </td>
                    <td class="MainTableRight">
                        <table>
                            <tr>
                                <td class="RightSelectStationTd">
                                    &nbsp;&nbsp;选择生产区域&nbsp;<select id="Select_SelectStation" class="easyui-combobox" name="SelectStation" data-options="panelHeight:'auto', editable:true, valueField: 'OrganizationId',textField: 'Name',onSelect:function(myRecord){RefreshFactoryOrganiztion(myRecord['OrganizationId']);}" style="width: 100px;"></select>
                                    &nbsp;&nbsp;<a id ="button_BackToGlobalPage" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'ext-icon-application_go',plain:true" onclick="ChangeDisplayStation();">返回</a>
                                </td>
                            </tr>
                            <tr>
                                <td class="RightSelectStationTd">
                                    &nbsp;&nbsp;选择查询时间&nbsp;<input id="dateTime" type="text" class="easyui-datebox" required="required" data-options ="onSelect:function(date){QueryDataFun(date);}" style ="width:100px;"/>
                                </td>
                            </tr>
                            <tr>
                                <td class="RightAccordionTd">
                                    <div class="easyui-accordion" style="width: 250px;">
                                        <div title="销售信息" data-options="iconCls:'ext-icon-medal_gold_1',selected:true" style="overflow: auto; height: 170px;">
                                            <table id="datagrid_SaleInfo" class="easyui-datagrid" data-options="fit:true,striped:true, singleSelect:true, border:false">
                                                <thead>
                                                    <tr>
                                                        <th data-options="field:'Name',width:78">名称</th>
                                                        <th data-options="field:'Clinker',width:82">熟料</th>
                                                        <th data-options="field:'Cement',fitColumns:true,width:82">水泥</th>
                                                    </tr>
                                                </thead>  
                                            </table>
                                        </div>
                                    </div>
                                    <div class="easyui-accordion" style="width: 250px;">
                                        <div title="停机/能耗报警" data-options="iconCls:'ext-icon-error',selected:true" style="height: 290px;">
                                            <table id="datagrid_EquipmentHaltAlarm" class="easyui-datagrid" data-options="fit:true,striped:true, singleSelect:true, border:false">
                                                <thead>
                                                    <tr>
                                                        <th data-options="field:'AlarmItemId',width:20,hidden:true">Id</th>
                                                        <th data-options="field:'Name',width:100">名称</th>
                                                        <th data-options="field:'AlarmDateTime',width:110">时间</th>
                                                        <th data-options="field:'AlarmType',width:38">类型</th>
                                                    </tr>
                                                </thead>
                                            </table>
                                        </div>
                                    </div>
                                    <div class="easyui-accordion" style="width: 250px;">
                                        <div title="交接班记录" data-options="iconCls:'ext-icon-note',selected:true" style="height: 237px;">
                                            <table id="datagrid_WorkingTeamShiftLogList" class="easyui-datagrid" data-options="fit:true,striped:true, singleSelect:true, border:false, onDblClickRow:function(myRowIndex, myRowData){GetWorkingTeamShiftLogDetail(myRowData);}">
                                                <thead>
                                                    <tr>
                                                        <th data-options="field:'WorkingTeamShiftLogId', width:58, hidden:true">Id</th>
                                                        <th data-options="field:'Shifts',width:38">班次</th>
                                                        <th data-options="field:'WorkingTeam',width:38">班组</th>
                                                        <th data-options="field:'WorkingTeamShiftMonitor',width:52">提交人</th>
                                                        <th data-options="field:'UpdateDate',width:120">提交时间</th>
                                                    </tr>
                                                </thead>
                                            </table>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <!--弹出电量、余热发电具体信息-->
    <div id="dlg_ElectricityQuantityDetail" class="easyui-dialog">
        <table id="grid_ElectricityQuantityDetail" class="easyui-datagrid" data-options="fit:true, rownumbers: true,striped:true, singleSelect:true, border:false">
            <thead>
                <tr>
                    <th data-options="field:'OrganizationID',width:120, hidden: true">组织机构ID</th> 
                    <th data-options="field:'BalanceVariableId',width:120,hidden: true">电耗ID</th>
                    <th data-options="field:'VariableId',width:120,hidden: true">工序ID</th>
                    <th data-options="field:'ProcessName',width:120">工序名称</th>
                    <th data-options="field:'DayElectricityQuantity',width:90">昨日</th>
                    <th data-options="field:'MonthElectricityQuantity',width:110">月累计</th>
                </tr>
            </thead>
        </table>
    </div>
    <!--工序电耗-->
    <div id="dlg_ElectricityConsumptionDetail" class="easyui-dialog">
        <table id="grid_ElectricityConsumptionDetail" class="easyui-datagrid" data-options="fit:true, rownumbers: true,striped:true, singleSelect:true, border:false">
            <thead>
                <tr>
                    <th data-options="field:'ProcessName',width:120">工序名称</th>
                    <th data-options="field:'DayElectricityQuantity',width:90">昨日电量</th>
                    <th data-options="field:'DayMaterialWeight',width:90">昨日产量</th>
                    <th data-options="field:'DayElectricityConsumption',width:90">昨日电耗</th>
                    <th data-options="field:'MonthElectricityQuantity',width:110">月累计电量</th>
                    <th data-options="field:'MonthMaterialWeight',width:110">月累计产量</th>
                    <th data-options="field:'MonthElectricityConsumption',width:110">月累计电耗</th>
                </tr>
            </thead>
        </table>
    </div>
    <!--余热吨熟料发电量-->
    <div id="dlg_ElectricityConsumptionDetailYR" class="easyui-dialog">
        <table id="grid_ElectricityConsumptionDetailYR" class="easyui-datagrid" data-options="fit:true, rownumbers: true,striped:true, singleSelect:true, border:false">
            <thead>
                <tr>
                    <th data-options="field:'ProcessName',width:120">工序名称</th>
                    <th data-options="field:'DayElectricityQuantity',width:90">昨日发电量</th>
                    <th data-options="field:'DayMaterialWeight',width:90">昨日产量</th>
                    <th data-options="field:'DayElectricityConsumption',width:90">昨日吨熟料发电量</th>
                    <th data-options="field:'MonthElectricityQuantity',width:110">月累计发电量</th>
                    <th data-options="field:'MonthMaterialWeight',width:110">月累计产量</th>
                    <th data-options="field:'MonthElectricityConsumption',width:110">月累计吨熟料发电量</th>
                </tr>
            </thead>
        </table>
    </div>
    <!--综合电耗-->
    <div id="dlg_ElectricityConsumptionCDetail" class="easyui-dialog">
        <table id="grid_ElectricityConsumptionCDetail" class="easyui-datagrid" data-options="fit:true, rownumbers: true,striped:true, singleSelect:true, border:false">
            <thead>
                <tr>
                    <th data-options="field:'Name',width:120">产线名称</th>
                    <th data-options="field:'ElectricityConsumption',width:90">综合电耗</th>
                    <th data-options="field:'CoalConsumption',width:90">综合煤耗</th>
                    <th data-options="field:'EnergyConsumption',width:90">综合能耗</th>
                </tr>
            </thead>
        </table>
    </div>
    <!--产量与消耗量-->
    <div id="dlg_MaterialWeightDetail" class="easyui-dialog">
        <table id="grid_MaterialWeightDetail" class="easyui-datagrid" data-options="fit:true, rownumbers: true,striped:true, singleSelect:true, border:false">
            <thead>
                <tr>
                    <th data-options="field:'MaterialName',width:120">物料名称</th>
                    <th data-options="field:'DayMaterialWeight',width:90">昨日</th>
                    <th data-options="field:'MonthMaterialWeight',width:90">本月累计</th>
                    <%--<th data-options="field:'MonthMaterialPlan',width:90">本月计划</th>--%>
                </tr>
            </thead>
        </table>
    </div>
    <!--设备运转率-->
    <div id="dlg_RunIndictorsDetail" class="easyui-dialog">
        <table id="grid_RunIndictorsDetail" class="easyui-datagrid" data-options="fit:true, rownumbers: true,striped:true, singleSelect:true, border:false">
            <thead>
                <tr>    
                    <th data-options="field:'EquipmentId',width:110, hidden:true">设备名称</th> 
                    <th data-options="field:'EquipmentName',width:110">设备名称</th>
                    <th data-options="field:'运转率',width:80">运转率(%)</th>
                    <th data-options="field:'可靠性',width:80">可靠性(%)</th>
                    <th data-options="field:'故障率',width:80">故障率(%)</th>
                    <th data-options="field:'台时产量',width:80">台时产量(t/h)</th>
                    <th data-options="field:'运转时间',width:80">运转时间(h)</th>
                </tr>
            </thead>
        </table>
    </div>
    <!--故障停机-->
    <div id="dlg_MasterMachineHaltDetail" class="easyui-dialog">
        <table id="grid_MasterMachineHaltDetail" class="easyui-datagrid" data-options="fit:true, rownumbers: true,striped:true, singleSelect:true, border:false">
            <thead>
                <tr> 
                    <th data-options="field:'EquipmentId',width:110, hidden:true">设备ID</th>    
                    <th data-options="field:'EquipmentName',width:110">设备名称</th>
                    <th data-options="field:'DowntimeCount',width:60">总次数</th>
                    <th data-options="field:'ProcessDowntimeCount',width:60">工艺故障</th>
                    <th data-options="field:'MechanicalDowntimeCount',width:60">机械故障</th>
                    <th data-options="field:'ElectricalDowntimeCount',width:60">电气故障</th>
                    <th data-options="field:'EnvironmentDowntimeCount',width:60">环境停机</th>
                    <th data-options="field:'DowntimeTime',width:60">总时间</th>
                    <th data-options="field:'ProcessDowntimeTime',width:60">工艺故障</th>
                    <th data-options="field:'MechanicalDowntimeTime',width:60">机械故障</th>
                    <th data-options="field:'ElectricalDowntimeTime',width:60">电气故障</th>
                    <th data-options="field:'EnvironmentDowntimeTime',width:60">环境停机</th>
                </tr>
            </thead>
        </table>
    </div>
    <!--交接班日志-->
    <div id="dlg_WorkingTeamShiftLogDetail" class="easyui-dialog">
        <table class ="WorkingTeamShiftLogDetailTable">
             <tr>
                 <td class ="WorkingTeamShiftNameTd">
                     &nbsp;&nbsp;班组&nbsp;
                     <input id="input_WorkingTeamShiftName" class="easyui-textbox" data-options="readonly:true"  />
                 </td>
                 <td class ="WorkingTeamShiftMonitorTd">
                     班长&nbsp;
                     <input id="input_WorkingTeamShiftMonitor" class="easyui-textbox"  data-options="readonly:true" />
                 </td>
                 <td class ="WorkingTeamShiftUpdateTimeTd">
                     提交时间&nbsp;
                     <input id="input_WorkingTeamShiftUpdateTime" class="easyui-textbox"  data-options="readonly:true" />
                 </td>
             </tr>
            <tr>
                 <td colspan="3" class ="WorkingTeamShiftLogTd">
                     <textarea id="TextArea_WorkingTeamShiftLog" cols="20" name="S1" rows="6"  readonly="readonly" ></textarea>
                 </td>
             </tr>
        </table>
    </div>
    <!--快捷菜单-->
    <div id="dlg_QuickMenuDetail" class="easyui-dialog">
        <table id="grid_QuickMenuDetail" class="easyui-datagrid" data-options="fit:true, rownumbers: true,striped:true, singleSelect:true, border:false, onDblClickRow:function(rowIndex, rowData){AddNewPage(rowIndex, rowData);}">
            <thead>
                <tr>     
                    <th data-options="field:'PageId',width:60, hidden:true">页面ID</th>
                    <th data-options="field:'Name',width:150">页面名称</th>
                    <th data-options="field:'Description',width:190">描述</th>
                    <th data-options="field:'NodePath',width:110, hidden:true">页面</th>
                    <th data-options="field:'NavigateUrl',width:60, hidden:true">地址</th>
                </tr>
            </thead>
        </table>
    </div>
    <form id="form_Main" runat="server">
        <div>
            <asp:HiddenField ID="HiddenField_StationOrganizationIds" runat="server" />
            <asp:HiddenField ID="HiddenField_ComfromGlobalPage" runat="server" />
        </div>
    </form>
</body>
</html>
