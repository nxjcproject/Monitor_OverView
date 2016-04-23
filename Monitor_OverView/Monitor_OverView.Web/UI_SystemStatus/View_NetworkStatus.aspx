<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="View_NetworkStatus.aspx.cs" Inherits="Monitor_OverView.Web.UI_SystemStatus.OverView_NetworkStatus" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>网络状态监控</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/css/common/NormalPage.css" />
    <link rel="stylesheet" type="text/css" href="css/page/View_NetworkStatus.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <!--[if lt IE 9]><script type="text/javascript" src="/lib/pllib/excanvas.js"></script><![endif]-->

    <script type="text/javascript" src="js/page/View_NetworkStatus.js"></script>
</head>
<body>
    <div id="MainLayout" class="easyui-layout" data-options="fit:true,border:false">
        <div data-options="region:'center',border:false" style="overflow: auto;">
            <table id="StatusMainTable">
            </table>
        </div>
    </div>
    <div id="dlg_FactoryServer" class="easyui-dialog" data-options="iconCls:'icon-search',resizable:false,modal:true">
        <fieldset>
            <legend>分厂服务器信息</legend>
            <table class="table" style="width: 100%;">
                <tr>
                    <th style="height: 30px;">名称</th>
                    <td>
                        <input id="TextBox_FactoryName" class="easyui-textbox" style="width: 120px" readonly="readonly" />
                    </td>
                    <th>IP地址</th>
                    <td>
                        <input id="TextBox_FactoryAddress" class="easyui-textbox" style="width: 120px" readonly="readonly" />
                    </td>
                </tr>
                <tr>
                    <th style="height: 30px;">网络状态</th>
                    <td>
                        <input id="Text_FactoryNetworkStatus" class="easyui-textbox" style="width: 120px" readonly="readonly" />
                    </td>
                    <th>同步状态</th>
                    <td>
                        <input id="Text_SynchronizationStatus" class="easyui-textbox" style="width: 120px" readonly="readonly" />
                    </td>
                </tr>
                <tr>
                    <th style="height: 30px;">汇总软件状态</th>
                    <td>
                        <input id="Text_FactoryCollectionSoftStatus" class="easyui-textbox" style="width: 120px" readonly="readonly" />
                    </td>
                    <th></th>
                    <td>
                    </td>
                </tr>
            </table>
        </fieldset>
    </div>
    <div id="dlg_CollectionComputerServer" class="easyui-dialog" data-options="iconCls:'icon-search',resizable:false,modal:true">
        <fieldset>
            <legend>采集计算机信息</legend>
            <table class="table" style="width: 100%;">
                <tr>
                    <th style="height: 30px;">名称</th>
                    <td>
                        <input id="Text_CollectionComputer" class="easyui-textbox" style="width: 120px" readonly="readonly" />
                    </td>
                    <th>IP地址</th>
                    <td>
                        <input id="Text_CollectionComputerAddress" class="easyui-textbox" style="width: 120px" readonly="readonly" />
                    </td>
                </tr>
                <tr>
                    <th style="height: 30px;">网络状态</th>
                    <td>
                        <input id="Text_CollectionComputerNetworkStatus" class="easyui-textbox" style="width: 120px" readonly="readonly" />
                    </td>
                    <th>采集软件状态</th>
                    <td>
                        <input id="Text_CollectionComputerSoftStatus" class="easyui-textbox" style="width: 120px" readonly="readonly" />

                    </td>
                </tr>
            </table>
        </fieldset>
    </div>
    <div id="dlg_AmmeterStatus" class="easyui-dialog" data-options="iconCls:'icon-search',resizable:false,modal:true" style ="height:500px">
        <div id="Div_AmmeterStatusLayout" class="easyui-layout" data-options="fit:true,border:false">
            <div data-options="region:'north',border:false" style="overflow: auto; height:100px;">
                <fieldset>
                    <legend>采集器信息</legend>
                    <table class="table" style="width: 100%;">
                        <tr>
                            <th style="height: 30px;">名称</th>
                            <td>
                                <input id="Text_CollectorName" class="easyui-textbox" style="width: 130px" readonly="readonly" />
                            </td>
                            <th>IP地址</th>
                            <td>
                                <input id="Text_CollectorIpAddress" class="easyui-textbox" style="width: 130px" readonly="readonly" />
                            </td>
                        </tr>
                        <tr>
                            <th style="height: 30px;">类别</th>
                            <td>
                                <input id="Text_CollectorType" class="easyui-textbox" style="width: 130px" readonly="readonly" />
                            </td>
                            <th>网络状态</th>
                            <td>
                                <input id="Text_CollectorNetworkStatus" class="easyui-textbox" style="width: 130px" readonly="readonly" />
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </div>
            <div data-options="region:'center',border:false" style="overflow: auto;">
                <table id="grid_AmmeterStatus" data-options="fit:true,border:true"></table>
            </div>
        </div>
    </div>
    <div id="dlg_OPCStatus" class="easyui-dialog" data-options="iconCls:'icon-search',resizable:false,modal:true">
        <fieldset>
            <legend>DCS采集信息</legend>
            <table class="table" style="width: 100%;">
                <tr>
                    <th style="height: 30px;">名称</th>
                    <td>
                        <input id="Text_OPCName" class="easyui-textbox" style="width: 140px" readonly="readonly" />
                    </td>
                    <th>IP地址</th>
                    <td>
                        <input id="Text_OPCIpAddress" class="easyui-textbox" style="width: 120px" readonly="readonly" />
                    </td>
                </tr>
                <tr>
                    <th style="height: 30px;">类别</th>
                    <td>
                        <input id="Text_OPCCollectorType" class="easyui-textbox" style="width: 140px" readonly="readonly" />
                    </td>
                    <th>网络状态</th>
                    <td>
                        <input id="Text_OPCStatus" class="easyui-textbox" style="width: 120px" readonly="readonly" />
                    </td>
                </tr>
            </table>
        </fieldset>
    </div>
    <form id="form1" runat="server">
        <div>
        </div>
    </form>
</body>
</html>
