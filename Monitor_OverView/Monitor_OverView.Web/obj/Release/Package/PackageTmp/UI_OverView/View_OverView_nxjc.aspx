<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="View_OverView_nxjc.aspx.cs" Inherits="Monitor_OverView.Web.UI_OverView.View_OverView_nxjc" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head_OverView_nxjc" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>宁夏建材能源中心系统总貌</title>

    <link rel="stylesheet" type="text/css" href="/css/common/NormalPage.css" />
    <link rel="stylesheet" type="text/css" href="/UI_OverView/css/page/Style_OverView_nxjc.css" />
    <link rel="stylesheet" type="text/css" href="/UI_OverView/css/page/Style_SelectButton.css" />
    
    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->
    <script type="text/javascript" src="js/page/View_OverView_nxjc.js" charset="utf-8"></script>
    

</head>
<body>
    <div id="GlobalBackGroundDiv">
        <table class="MainTable">
            <tr>
                <td class="BorderBlankTd1"></td>
                <td class="BorderBlankTd2"></td>
                <td class="BorderBlankTd1"></td>
                <td class="BorderBlankTd2"></td>
                <td class="BorderBlankTd1"></td>
                <td class="BorderBlankTd2"></td>
                <td class="BorderBlankTd1"></td>
                <td class="BorderBlankTd2"></td>
                <td class="BorderBlankTd1"></td>
                <td class="BorderBlankTd2"></td>
                <td class="BorderBlankTd1"></td>
            </tr>
            <tr>
                <td class="DataZoneBlankTd"></td>
                <td class="DataZoneTd">
                    <table id ="DataZoneTable1" class="DataZoneTable" data-options ='{"name":""}'>
                        <tr>
                            <td id="DataZoneTitle1" class="DataZoneTitleTd1"></td>
                            <td class="DataZoneTitleTd2"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料日销售量:</td>
                            <td id ="DataTextTd1_1" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料月销售量:</td>
                            <td id ="DataTextTd1_2" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料年销售量:</td>
                            <td id ="DataTextTd1_3"class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料计划完成率:</td>
                            <td id ="DataTextTd1_4" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥日销售量:</td>
                            <td id ="DataTextTd1_5" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥月销售量:</td>
                            <td id ="DataTextTd1_6" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥年销售量:</td>
                            <td id ="DataTextTd1_7" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥计划完成率:</td>
                            <td id ="DataTextTd1_8" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataBottomBlankTd"></td>
                            <td></td>
                        </tr>
                    </table>
                </td>
                <td class="DataZoneBlankTd"></td>
                <td class="DataZoneTd">
                    <table id ="DataZoneTable2" class="DataZoneTable" data-options ='{"name":""}'>
                        <tr>
                            <td id="DataZoneTitle2" class="DataZoneTitleTd1"></td>
                            <td class="DataZoneTitleTd2"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料日销售量:</td>
                            <td id ="DataTextTd2_1" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料月销售量:</td>
                            <td id ="DataTextTd2_2" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料年销售量:</td>
                            <td id ="DataTextTd2_3" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料计划完成率:</td>
                            <td id ="DataTextTd2_4" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥日销售量:</td>
                            <td id ="DataTextTd2_5" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥月销售量:</td>
                            <td id ="DataTextTd2_6" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥年销售量:</td>
                            <td id ="DataTextTd2_7" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥计划完成率:</td>
                            <td id ="DataTextTd2_8" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataBottomBlankTd"></td>
                            <td></td>
                        </tr>
                    </table>
                </td>
                <td class="DataZoneBlankTd"></td>
                <td class="DataZoneTd">
                    <table id ="DataZoneTable3" class="DataZoneTable" data-options ='{"name":""}'>
                        <tr>
                            <td id="DataZoneTitle3" class="DataZoneTitleTd1"></td>
                            <td class="DataZoneTitleTd2"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料日销售量:</td>
                            <td id ="DataTextTd3_1" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料月销售量:</td>
                            <td id ="DataTextTd3_2" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料年销售量:</td>
                            <td id ="DataTextTd3_3" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料计划完成率:</td>
                            <td id ="DataTextTd3_4" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥日销售量:</td>
                            <td id ="DataTextTd3_5" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥月销售量:</td>
                            <td id ="DataTextTd3_6" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥年销售量:</td>
                            <td id ="DataTextTd3_7" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥计划完成率:</td>
                            <td id ="DataTextTd3_8" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataBottomBlankTd"></td>
                            <td></td>
                        </tr>
                    </table>
                </td>
                <td class="DataZoneBlankTd"></td>
                <td class="DataZoneTd">
                    <table id ="DataZoneTable4" class="DataZoneTable" data-options ='{"name":""}'>
                        <tr>
                            <td id="DataZoneTitle4" class="DataZoneTitleTd1"></td>
                            <td class="DataZoneTitleTd2"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料日销售量:</td>
                            <td id ="DataTextTd4_1" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料月销售量:</td>
                            <td id ="DataTextTd4_2" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料年销售量:</td>
                            <td id ="DataTextTd4_3" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料计划完成率:</td>
                            <td id ="DataTextTd4_4" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥日销售量:</td>
                            <td id ="DataTextTd4_5" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥月销售量:</td>
                            <td id ="DataTextTd4_6" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥年销售量:</td>
                            <td id ="DataTextTd4_7" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥计划完成率:</td>
                            <td id ="DataTextTd4_8" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataBottomBlankTd"></td>
                            <td></td>
                        </tr>
                    </table>
                </td>
                <td class="DataZoneBlankTd"></td>
                <td class="DataZoneTd">
                    <table id ="DataZoneTable5" class="DataZoneTable" data-options ='{"name":""}'>
                        <tr>
                            <td id="DataZoneTitle5" class="DataZoneTitleTd1"></td>
                            <td class="DataZoneTitleTd2"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料日销售量:</td>
                            <td id ="DataTextTd5_1" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料月销售量:</td>
                            <td id ="DataTextTd5_2" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料年销售量:</td>
                            <td id ="DataTextTd5_3" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料计划完成率:</td>
                            <td id ="DataTextTd5_4" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥日销售量:</td>
                            <td id ="DataTextTd5_5" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥月销售量:</td>
                            <td id ="DataTextTd5_6" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥年销售量:</td>
                            <td id ="DataTextTd5_7" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥计划完成率:</td>
                            <td id ="DataTextTd5_8" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataBottomBlankTd"></td>
                            <td></td>
                        </tr>
                    </table>
                </td>
                <td class="DataZoneBlankTd"></td>
            </tr>
            <tr>
                <td class="BorderBlankTd1"></td>
                <td class="BorderBlankTd2"></td>
                <td class="BorderBlankTd1"></td>
                <td class="BorderBlankTd2"></td>
                <td class="BorderBlankTd1"></td>
                <td class="BorderBlankTd2"></td>
                <td class="BorderBlankTd1"></td>
                <td class="BorderBlankTd2"></td>
                <td class="BorderBlankTd1"></td>
                <td class="BorderBlankTd2"></td>
                <td class="BorderBlankTd1"></td>
            </tr>
            <tr>
                <td class="DataZoneBlankTd"></td>
                <td class="DataZoneTd">
                    <table id ="DataZoneTable6" class="DataZoneTable" data-options ='{"name":""}'>
                        <tr>
                            <td id="DataZoneTitle6" class="DataZoneTitleTd1"></td>
                            <td class="DataZoneTitleTd2"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料日销售量:</td>
                            <td id ="DataTextTd6_1" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料月销售量:</td>
                            <td id ="DataTextTd6_2" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料年销售量:</td>
                            <td id ="DataTextTd6_3" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料计划完成率:</td>
                            <td id ="DataTextTd6_4" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥日销售量:</td>
                            <td id ="DataTextTd6_5" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥月销售量:</td>
                            <td id ="DataTextTd6_6" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥年销售量:</td>
                            <td id ="DataTextTd6_7" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥计划完成率:</td>
                            <td id ="DataTextTd6_8" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataBottomBlankTd"></td>
                            <td></td>
                        </tr>
                    </table>
                </td>
                <td class="DataZoneBlankTd"></td>
                <td class="DataZoneTd">
                    <table id ="DataZoneTable7" class="DataZoneTable" data-options ='{"name":""}'>
                        <tr>
                            <td id="DataZoneTitle7" class="DataZoneTitleTd1"></td>
                            <td class="DataZoneTitleTd2"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料日销售量:</td>
                            <td id ="DataTextTd7_1" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料月销售量:</td>
                            <td id ="DataTextTd7_2" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料年销售量:</td>
                            <td id ="DataTextTd7_3" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料计划完成率:</td>
                            <td id ="DataTextTd7_4" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥日销售量:</td>
                            <td id ="DataTextTd7_5" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥月销售量:</td>
                            <td id ="DataTextTd7_6" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥年销售量:</td>
                            <td id ="DataTextTd7_7" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥计划完成率:</td>
                            <td id ="DataTextTd7_8" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataBottomBlankTd"></td>
                            <td></td>
                        </tr>
                    </table>
                </td>
                <td class="DataZoneBlankTd"></td>
                <td class="DataZoneTd">
                    <table id ="DataZoneTable8" class="DataZoneTable" data-options ='{"name":""}'>
                        <tr>
                            <td id="DataZoneTitle8" class="DataZoneTitleTd1"></td>
                            <td class="DataZoneTitleTd2"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料日销售量:</td>
                            <td id ="DataTextTd8_1" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料月销售量:</td>
                            <td id ="DataTextTd8_2" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料年销售量:</td>
                            <td id ="DataTextTd8_3" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料计划完成率:</td>
                            <td id ="DataTextTd8_4" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥日销售量:</td>
                            <td id ="DataTextTd8_5" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥月销售量:</td>
                            <td id ="DataTextTd8_6" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥年销售量:</td>
                            <td id ="DataTextTd8_7" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥计划完成率:</td>
                            <td id ="DataTextTd8_8" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataBottomBlankTd"></td>
                            <td></td>
                        </tr>
                    </table>
                </td>
                <td class="DataZoneBlankTd"></td>
                <td class="DataZoneTd">
                    <table id ="DataZoneTable9" class="DataZoneTable" data-options ='{"name":""}'>
                        <tr>
                            <td id="DataZoneTitle9" class="DataZoneTitleTd1"></td>
                            <td class="DataZoneTitleTd2"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料日销售量:</td>
                            <td id ="DataTextTd9_1" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料月销售量:</td>
                            <td id ="DataTextTd9_2" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料年销售量:</td>
                            <td id ="DataTextTd9_3" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料计划完成率:</td>
                            <td id ="DataTextTd9_4" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥日销售量:</td>
                            <td id ="DataTextTd9_5" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥月销售量:</td>
                            <td id ="DataTextTd9_6" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥年销售量:</td>
                            <td id ="DataTextTd9_7" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥计划完成率:</td>
                            <td id ="DataTextTd9_8" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataBottomBlankTd"></td>
                            <td></td>
                        </tr>
                    </table>
                </td>
                <td class="DataZoneBlankTd"></td>
                <td class="DataZoneTd">
                    <table id ="DataZoneTable10" class="DataZoneTable" data-options ='{"name":""}'>
                        <tr>
                            <td id="DataZoneTitle10" class="DataZoneTitleTd1"></td>
                            <td class="DataZoneTitleTd2"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料日销售量:</td>
                            <td id ="DataTextTd10_1" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料月销售量:</td>
                            <td id ="DataTextTd10_2" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料年销售量:</td>
                            <td id ="DataTextTd10_3" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">熟料计划完成率:</td>
                            <td id ="DataTextTd10_4" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥日销售量:</td>
                            <td id ="DataTextTd10_5" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥月销售量:</td>
                            <td id ="DataTextTd10_6" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥年销售量:</td>
                            <td id ="DataTextTd10_7" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataTitleTd">水泥计划完成率:</td>
                            <td id ="DataTextTd10_8" class="DataTextTd"></td>
                        </tr>
                        <tr>
                            <td class="DataBottomBlankTd"></td>
                            <td></td>
                        </tr>
                    </table>
                </td>
                <td class="DataZoneBlankTd"></td>
            </tr>
            <tr>
                <td class="BorderBlankTd1"></td>
                <td class="BorderBlankTd2"></td>
                <td class="BorderBlankTd1"></td>
                <td class="BorderBlankTd2"></td>
                <td class="BorderBlankTd1"></td>
                <td class="BorderBlankTd2"></td>
                <td class="BorderBlankTd1"></td>
                <td class="BorderBlankTd2"></td>
                <td class="BorderBlankTd1"></td>
                <td class="BorderBlankTd2"></td>
                <td class="BorderBlankTd1"></td>
            </tr>
        </table>
    </div>
    <div id ="SelectButtonDiv">
        <table>
            <tr>
                <td id="ComprehensiveDailyTd" class ="SelectedButtonTd" onclick ="ChageOtherPage('View_OverView_nxjc');">综合日报</td>
                <td id="ProductionDataTd" class ="SelectButtonTd" onclick = "ChageOtherPage('View_ProductionData');">生产数据</td>
                <td id="EnergyDataTd" class ="SelectButtonTd" onclick = "ChageOtherPage('View_EnergyData');">能源数据</td>
            </tr>
        </table>
    </div>
    <form id="form_Main" runat="server">
        <div>
        </div>
    </form>
</body>
</html>
