<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GeneralDrawingTest.aspx.cs" Inherits="Monitor_OverView.Web.UI_OverView.GeneralDrawingTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>分厂总貌</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css" />

    <link rel="stylesheet" type="text/css" href="/lib/pllib/themes/jquery.jqplot.min.css" />
    <link type="text/css" rel="stylesheet" href="/lib/pllib/syntaxhighlighter/styles/shCoreDefault.min.css" />
    <link type="text/css" rel="stylesheet" href="/lib/pllib/syntaxhighlighter/styles/shThemejqPlot.min.css" />
    <link type="text/css" rel="stylesheet" href="/css/common/NormalPage.css" />
    <link rel="stylesheet" type="text/css" href="css/page/Style_OverView_FactoryTest.css" />

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

    <script type="text/javascript" src="js/page/View_OverView_Factory.js" charset="utf-8"></script>
</head>
<body>
    <div id="MainLayout" class="easyui-layout" data-options="fit:true,border:false">
        <div data-options="region:'center',border:false">
            <table id="MainTable">
                <tr>
                    <td id="MainTablePosizionLeft"></td>
                    <td class="MainTableMiddle">
                        <table>
                            <tr>
                                <td>
                                    <table>
                                        <tr>
                                            <td>
                                                <table class="MainTableLeftTop">
                                                    <tr class="MainTableLeftTopTitleTr">
                                                        <td class="MainTableLeftTopTitleTr" colspan = "4">生产
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <th class="MainTableLeftTopTitleTh1">产量(t)</th>
                                                        <th class="MainTableLeftTopTitleTh">1#水泥磨</th>
                                                        <th class="MainTableLeftTopTitleTh">2#水泥磨</th>
                                                        <th class="MainTableLeftTopTitleTh">总计</th>
                                                    </tr>
                                                    <tr>
                                                        <td class="MainTableLeftTopColTitleTd">◇当日</td>
                                                        <td class="MainTableLeftTopTd">0.00</td>
                                                        <td class="MainTableLeftTopTd">0.00</td>
                                                        <td class="MainTableLeftTopTd">0.00</td>
                                                    </tr>
                                                    <tr>
                                                        <td class="MainTableLeftTopColTitleTd">◇月累计</td>
                                                        <td class="MainTableLeftTopTd">0.00</td>
                                                        <td class="MainTableLeftTopTd">0.00</td>
                                                        <td class="MainTableLeftTopTd">0.00</td>
                                                    </tr>
                                                    <tr>
                                                        <th class="MainTableLeftTopTitleTh1">消耗量(t)</th>
                                                        <th class="MainTableLeftTopTitleTh">水泥耗熟料</th>
                                                        <th class="MainTableLeftTopTitleTh">水泥粉煤灰</th>
                                                        <th class="MainTableLeftTopTitleTh">水泥耗石膏</th>
                                                    </tr>
                                                    <tr>
                                                        <td class="MainTableLeftTopColTitleTd">◇当日</td>
                                                        <td class="MainTableLeftTopTd">0.00</td>
                                                        <td class="MainTableLeftTopTd">0.00</td>
                                                        <td class="MainTableLeftTopTd">0.00</td>
                                                    </tr>
                                                    <tr>
                                                        <td class="MainTableLeftTopColTitleTd">◇月累计</td>
                                                        <td class="MainTableLeftTopTd">0.00</td>
                                                        <td class="MainTableLeftTopTd">0.00</td>
                                                        <td class="MainTableLeftTopTd">0.00</td>
                                                    </tr>
                                               </table>
                                            </td>
                                            <td >
                                                <table class="MainTableMiddleTop">
                                                    <tr>
                                                        <td class="MainTableMiddleTopTitleTr" colspan = "3">设备
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <th class="MainTableMiddleTopTitleTh1">设备运行(月)</th>
                                                        <th class="MainTableMiddleTopTitleTh">台时产量</th>
                                                        <th class="MainTableMiddleTopTitleTh">运转时间</th>                       
                                                    </tr>
                                                    <tr>
                                                        <td class="MainTableMiddleTopColTitleTd">1#水泥磨</td>
                                                        <td class="MainTableMiddleTopTd">0.00</td>
                                                        <td class="MainTableMiddleTopTd">0.00</td>
                                                    </tr>
                                                    <tr>
                                                        <td class="MainTableMiddleTopColTitleTd">2#水泥磨</td>
                                                        <td class="MainTableMiddleTopTd">0.00</td>
                                                        <td class="MainTableMiddleTopTd">0.00</td>
                                                    </tr>
                                                    <tr>
                                                        <td class="MainTableMiddleTopColTitleTd">####</td>
                                                        <td class="MainTableMiddleTopTd">0.00</td>
                                                        <td class="MainTableMiddleTopTd">0.00</td>
                                                    </tr>
                                                    <tr>
                                                        <td class="MainTableMiddleTopColTitleTd">####</td>
                                                        <td class="MainTableMiddleTopTd">0.00</td>
                                                        <td class="MainTableMiddleTopTd">0.00</td>
                                                    </tr>
                                                    <tr>
                                                        <td class="MainTableMiddleTopColTitleTd">####</td>
                                                        <td class="MainTableMiddleTopTd">0.00</td>
                                                        <td class="MainTableMiddleTopTd">0.00</td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>                                      
                                    </table>
                                </td>                              
                            </tr>
                            <tr style="height:2px;"></tr>
                            <tr>
                                <td>
                                    <table class="MainTableLeftBottom">
                                        <tr>
                                            <td class="MainTableBottomTitleTr" colspan="6">能源</td>
                                        </tr>
                                        <tr>
                                            <td class="MainTableBottomElecCol3" colspan="3">工序电量</td>
                                            <td class="MainTableBottomElecCol3" colspan="3">工序电耗</td>
                                        </tr>
                                        <tr>
                                            <td class="MainTableBottomTitleTh1">工序名称</td>
                                            <td class="MainTableBottomTitleTh">当日</td>
                                            <td class="MainTableBottomTitleTh">月累计</td>
                                            <td class="MainTableBottomTitleTh1">工序名称</td>
                                            <td class="MainTableBottomTitleTh">当日</td>
                                            <td class="MainTableBottomTitleTh">月累计</td>
                                        </tr>
                                        <tr>
                                            <td class="MainTableBottomColTitleTd">水泥粉磨</td>
                                            <td class="MainTableBottomTd">0.00</td>
                                            <td class="MainTableBottomTd">0.00</td>
                                            <td class="MainTableBottomColTitleTd">水泥粉磨</td>
                                            <td class="MainTableBottomTd">0.00</td>
                                            <td class="MainTableBottomTd">0.00</td>
                                        </tr>

                                        <tr style="height:1px;"></tr>
                                        <tr>
                                            <td class="MainTableBottomElecCol3" colspan="3">工序煤耗</td>
                                            <td class="MainTableBottomElecCol3" colspan="3">工序水量</td>
                                        </tr>
                                        <tr>
                                            <td class="MainTableBottomTitleTh1">工序名称</td>
                                            <td class="MainTableBottomTitleTh">当日</td>
                                            <td class="MainTableBottomTitleTh">月累计</td>
                                            <td class="MainTableBottomTitleTh1">工序名称</td>
                                            <td class="MainTableBottomTitleTh">当日</td>
                                            <td class="MainTableBottomTitleTh">月累计</td>
                                        </tr>
                                        <tr>
                                            <td class="MainTableBottomColTitleTd">水泥粉磨</td>
                                            <td class="MainTableBottomTd">0.00</td>
                                            <td class="MainTableBottomTd">0.00</td>
                                            <td class="MainTableBottomColTitleTd">水泥粉磨</td>
                                            <td class="MainTableBottomTd">0.00</td>
                                            <td class="MainTableBottomTd">0.00</td>
                                        </tr>
                                    </table>
                                </td>                            
                            </tr>
                         </table>
                    </td>                   
                    <td class="MainTableRight">
                        <table>
                            <tr>
                                <td class="RightSelectStationTd">&nbsp;选择生产区域&nbsp;<select id="Select_SelectStation" class="easyui-combobox" name="SelectStation" data-options="panelHeight:'auto', editable:true, valueField: 'OrganizationId',textField: 'Name',onSelect:function(myRecord){RefreshFactoryOrganiztion(myRecord['OrganizationId']);}" style="width: 90px;"></select>
                                    &nbsp;<a id="button_BackToGlobalPage" href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'ext-icon-application_go',plain:true" onclick="ChangeDisplayStation();">返回</a>
                                </td>
                            </tr>
                            <tr>
                                <td class="RightSelectStationTd">&nbsp;选择查询时间&nbsp;<input id="dateTime" type="text" class="easyui-datebox" required="required" data-options="onSelect:function(date){QueryDataFun(date);}" style="width: 90px;" />
                                </td>
                            </tr>
                            <tr>
                                <td class="RightAccordionTd">
                                    <div class="easyui-accordion" style="width: 250px;">
                                        <div title="销售信息" data-options="iconCls:'ext-icon-medal_gold_1',selected:true" style="overflow: hidden; height: 175px;">
                                            <table id="datagrid_SaleInfo" class="easyui-datagrid" data-options="fit:true,striped:true, singleSelect:true, border:false">
                                                <thead>
                                                    <tr>
                                                        <th data-options="field:'Name',width:78">名称</th>
                                                        <th data-options="field:'Clinker',width:80">熟料</th>
                                                        <th data-options="field:'Cement',fitColumns:true,width:80">水泥</th>
                                                    </tr>
                                                </thead>
                                            </table>
                                        </div>
                                    </div>
                                    <div class="easyui-accordion" style="width: 250px;">
                                        <div title="停机/能耗报警" data-options="iconCls:'ext-icon-error',selected:true" style="height: 300px;">
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
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <form id="form1" runat="server">
    </form>
</body>
</html>
