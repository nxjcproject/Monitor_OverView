using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SqlServerDataAdapter;

namespace Monitor_OverView.Service.OverView
{
    public class OverView_Nxjc
    {
        private static readonly ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(Monitor_OverView.Infrastruture.Configuration.ConnectionStringFactory.NXJCConnectionString);

        public OverView_Nxjc()
        {
        }
        public static string GetStationId()
        {
            return Monitor_OverView.Infrastruture.Configuration.ConnectionStringFactory.StationId;
        }
        public static string GetRealTimeData(string myDateTime)
        {
            DateTime m_DateTime = DateTime.Parse(myDateTime);
            DataTable m_DisplayDataTable = GetDisplayDataTable();
            DataTable m_ProductSaleDataTable = GetProductSaleData("Clinker,Cement", m_DateTime);
            string[][] m_Organizations = new string[10][];
            m_Organizations[0] = new string[] { "银川水泥", "zc_nxjc_ychc" };
            m_Organizations[1] = new string[] { "青铜峡水泥", "zc_nxjc_qtx" };
            m_Organizations[2] = new string[] { "白银水泥", "zc_nxjc_byc" };
            m_Organizations[3] = new string[] { "天水水泥", "zc_nxjc_tsc" };
            m_Organizations[4] = new string[] { "中宁水泥", "zc_nxjc_znc" };
            m_Organizations[5] = new string[] { "六盘山水泥", "zc_nxjc_lpsc" };
            m_Organizations[6] = new string[] { "乌海赛马", "zc_nxjc_whsmc" };
            m_Organizations[7] = new string[] { "喀喇沁水泥", "zc_nxjc_klqc" };
            m_Organizations[8] = new string[] { "石嘴山水泥", "zc_nxjc_szsc" };
            m_Organizations[9] = new string[] { "乌海西水", "zc_nxjc_whxsc" };
            for (int i = 0; i < 10; i++)
            {
                object[] m_RowArray = GetFactorySaleData(m_ProductSaleDataTable, m_Organizations[i][0], m_Organizations[i][1]);
                //m_DisplayDataTable.Rows.Add(m_Organizations[i][0], m_Organizations[i][1], 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m);
                m_DisplayDataTable.Rows.Add(m_RowArray);
            }
            string m_JsonString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_DisplayDataTable);
            return m_JsonString;
        }
        private static DataTable GetDisplayDataTable()
        {
            DataTable m_DataTable = new DataTable();
            m_DataTable.Columns.Add("DataZoneTitle", typeof(string));
            m_DataTable.Columns.Add("DataZoneTitleId", typeof(string));
            m_DataTable.Columns.Add("Data1", typeof(long));
            m_DataTable.Columns.Add("Data2", typeof(long));
            m_DataTable.Columns.Add("Data3", typeof(long));
            m_DataTable.Columns.Add("Data4", typeof(decimal));
            m_DataTable.Columns.Add("Data5", typeof(long));
            m_DataTable.Columns.Add("Data6", typeof(long));
            m_DataTable.Columns.Add("Data7", typeof(long));
            m_DataTable.Columns.Add("Data8", typeof(decimal));
            return m_DataTable;
        }
        private static object[] GetFactorySaleData(DataTable myProductSaleDataTable, string myOrganizationName, string myOrganizationId)
        {
            object[] m_SaleResultRowItemArray = new object[10];
            m_SaleResultRowItemArray[0] = myOrganizationName;
            m_SaleResultRowItemArray[1] = myOrganizationId;
            for (int i = 2; i < m_SaleResultRowItemArray.Length; i++)
            {
                m_SaleResultRowItemArray[i] = 0.0m;
            }
            DataRow[] m_SaleDataRows = myProductSaleDataTable.Select(string.Format("OrganizationId = '{0}'", myOrganizationId));
            for (int i = 0; i < m_SaleDataRows.Length; i++)
            {
                if (m_SaleDataRows[i]["VariableId"].ToString() == "Clinker")
                {
                    if (m_SaleDataRows[i]["DayValue"] != DBNull.Value)
                    {
                        m_SaleResultRowItemArray[2] = (decimal)m_SaleDataRows[i]["DayValue"];
                    }
                    if (m_SaleDataRows[i]["MonthValue"] != DBNull.Value)
                    {
                        m_SaleResultRowItemArray[3] = (decimal)m_SaleDataRows[i]["MonthValue"];
                    }
                    if (m_SaleDataRows[i]["YearValue"] != DBNull.Value)
                    {
                        m_SaleResultRowItemArray[4] = (decimal)m_SaleDataRows[i]["YearValue"];
                    }
                    if (m_SaleDataRows[i]["YearCompletionRate"] != DBNull.Value)
                    {
                        m_SaleResultRowItemArray[5] = (decimal)m_SaleDataRows[i]["YearCompletionRate"];
                    }
                }
                else if (m_SaleDataRows[i]["VariableId"].ToString() == "Cement")
                {
                    if (m_SaleDataRows[i]["DayValue"] != DBNull.Value)
                    {
                        m_SaleResultRowItemArray[6] = (decimal)m_SaleDataRows[i]["DayValue"];
                    }
                    if (m_SaleDataRows[i]["MonthValue"] != DBNull.Value)
                    {
                        m_SaleResultRowItemArray[7] = (decimal)m_SaleDataRows[i]["MonthValue"];
                    }
                    if (m_SaleDataRows[i]["YearValue"] != DBNull.Value)
                    {
                        m_SaleResultRowItemArray[8] = (decimal)m_SaleDataRows[i]["YearValue"];
                    }
                    if (m_SaleDataRows[i]["YearCompletionRate"] != DBNull.Value)
                    {
                        m_SaleResultRowItemArray[9] = (decimal)m_SaleDataRows[i]["YearCompletionRate"];
                    }
                }
            }
            return m_SaleResultRowItemArray;
        }
        private static DataTable GetProductSaleData(string myMaterialIds, DateTime myDatetime)
        {
            if (myMaterialIds != "")
            {
                string[] m_MaterialIds = myMaterialIds.Split(',');
                //string[] m_RowNameArray = new string[] { "日销售量(t)", "月销售量(t)", "年销售量(t)", "月计划完成率", "年计划完成率" };
                string[] m_MonthName = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
                string m_ValueVariable = "";
                string m_PlanVariable = "";

                string m_Sql = @"Select C.OrganizationID as OrganizationId, 
                                    C.VariableId, 
                                    A.DayValue, 
                                    B.MonthValue, 
                                    C.YearValue, 
                                    case when D.MonthPlanValue > 0 then 100 * B.MonthValue / D.MonthPlanValue else D.MonthPlanValue end as MonthCompletionRate,
                                    case when D.YearPlanValue > 0 then 100 * C.YearValue / D.YearPlanValue else D.YearPlanValue end as YearCompletionRate
                                from
                                    (Select D.OrganizationID, A.ConstrastVariableId as VariableId, sum(CASE WHEN A.sales_gblx = 'DE' THEN A.Value WHEN A.sales_gblx = 'RD' THEN -A.Value end) as YearValue from 
				                        VWB_WeightNYGL A, system_Organization C, system_Organization D
                                        where A.StatisticalTime >= '{7}'
                                        and A.StatisticalTime < '{8}'
                                        and A.ConstrastVariableId in ({0})
			                            and A.Type = 3
                                        and A.OrganizationID = C.OrganizationID
				                        and D.LevelType = 'Company'
				                        and charindex(D.LevelCode, C.LevelCode) > 0
                                    group by D.OrganizationID, A.ConstrastVariableId) C
                                    left join 
                                    (Select P.OrganizationID, substring(N.QuotasID,1, len(N.QuotasID) - 5) as VariableId, N.{9} as MonthPlanValue, N.Totals as YearPlanValue from tz_Plan M, plan_PurchaseSalesYearlyPlan N, system_Organization P, system_Organization Q
                                        where M.Date = '{2}' and M.PlanType = '{10}' and M.KeyID = N.KeyID and N.QuotasID in ({1})
                                        and M.OrganizationID = Q.OrganizationID and P.LevelType = 'Company' and CHARINDEX(P.LevelCode, Q.LevelCode) > 0) D on C.VariableId = D.VariableId and D.OrganizationID = C.OrganizationID
                                    left join 
                                    (Select D.OrganizationID, A.ConstrastVariableId as VariableId, sum(CASE WHEN A.sales_gblx = 'DE' THEN A.Value WHEN A.sales_gblx = 'RD' THEN -A.Value end) as DayValue from 
				                        VWB_WeightNYGL A, system_Organization C, system_Organization D
                                        where A.StatisticalTime >= '{3}'
                                        and A.StatisticalTime < '{4}'
                                        and A.ConstrastVariableId in ({0})
			                            and A.Type = 3
                                        and A.OrganizationID = C.OrganizationID
				                        and D.LevelType = 'Company'
				                        and charindex(D.LevelCode, C.LevelCode) > 0
                                    group by D.OrganizationID, A.ConstrastVariableId) A on A.VariableId = C.VariableId and A.OrganizationID = C.OrganizationID
                                    left join 
                                   (Select D.OrganizationID, A.ConstrastVariableId as VariableId, sum(CASE WHEN A.sales_gblx = 'DE' THEN A.Value WHEN A.sales_gblx = 'RD' THEN -A.Value end) as MonthValue from 
				                        VWB_WeightNYGL A, system_Organization C, system_Organization D
                                        where A.StatisticalTime >= '{5}'
                                        and A.StatisticalTime < '{6}'
                                        and A.ConstrastVariableId in ({0})
			                            and A.Type = 3
                                        and A.OrganizationID = C.OrganizationID
				                        and D.LevelType = 'Company'
				                        and charindex(D.LevelCode, C.LevelCode) > 0
                                    group by D.OrganizationID, A.ConstrastVariableId) B on B.VariableId = C.VariableId and B.OrganizationID = C.OrganizationID";
                for (int i = 0; i < m_MaterialIds.Length; i++)
                {
                    if (i == 0)
                    {
                        m_ValueVariable = "'" + m_MaterialIds[i] + "'";
                        m_PlanVariable = "'" + m_MaterialIds[i] + "Sales'";
                    }
                    else
                    {
                        m_ValueVariable = m_ValueVariable + ",'" + m_MaterialIds[i] + "'";
                        m_PlanVariable = m_PlanVariable + ",'" + m_MaterialIds[i] + "Sales'";
                    }
                }
                m_Sql = m_Sql.Replace("{0}", m_ValueVariable);
                m_Sql = m_Sql.Replace("{1}", m_PlanVariable);
                m_Sql = m_Sql.Replace("{2}", myDatetime.Year.ToString());
                m_Sql = m_Sql.Replace("{3}", myDatetime.ToString("yyyy-MM-dd 00:00:00"));
                m_Sql = m_Sql.Replace("{4}", myDatetime.AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
                m_Sql = m_Sql.Replace("{5}", myDatetime.ToString("yyyy-MM-01 00:00:00"));
                m_Sql = m_Sql.Replace("{6}", myDatetime.AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
                m_Sql = m_Sql.Replace("{7}", myDatetime.ToString("yyyy-01-01 00:00:00"));
                m_Sql = m_Sql.Replace("{8}", myDatetime.AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
                m_Sql = m_Sql.Replace("{9}", m_MonthName[myDatetime.Month - 1]);
                m_Sql = m_Sql.Replace("{10}", "PurchaseSales");
                //string m_DayStartTime = myDatetime.ToString("yyyy-MM-dd 00:00:00");
                //string m_DayEndTime = myDatetime.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
                //string m_MonthStartTime = myDatetime.ToString("yyyy-MM-01 00:00:00");
                //string m_MonthEndTime = myDatetime.AddMonths(1).ToString("yyyy-MM-01 00:00:00");
                //string m_YearStartTime = myDatetime.ToString("yyyy-01-01 00:00:00");
                //string m_YearEndTime = myDatetime.AddYears(1).ToString("yyyy-01-01 00:00:00");
                try
                {
                    DataTable m_ProductSaleDataTable = _dataFactory.Query(m_Sql);
                    return m_ProductSaleDataTable;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
