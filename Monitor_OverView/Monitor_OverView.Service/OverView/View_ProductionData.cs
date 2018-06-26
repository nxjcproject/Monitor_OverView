using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SqlServerDataAdapter;

namespace Monitor_OverView.Service.OverView
{
    public class View_ProductionData
    {
        private static readonly ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(Monitor_OverView.Infrastruture.Configuration.ConnectionStringFactory.NXJCConnectionString);

        public View_ProductionData()
        {

        }
        /// <summary>
        /// 获得左上角的全局数据
        /// </summary>
        /// <param name="myDate"></param>
        /// <param name="myLevelCodes"></param>
        /// <returns></returns>
        public static string GetGlobalComplete(DateTime myDate, string[] myLevelCodes)
        {
            string[] m_Month = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            string m_CurrentMonth = m_Month[myDate.Month - 1];
            string m_ReturnJsonString = "";
            string m_Sql = @"SELECT Z.* FROM (
                              SELECT A.OrganizationID as OrganizationId,A.LevelCode as LevelCode,A.Name as Name, M.VariableId + '_day' as VariableId, sum(M.TotalPeakValleyFlatB) as value
                                    FROM system_Organization AS A
	                                left join 
	                                (SELECT D.LevelCode as LevelCode,C.VariableId AS VariableId, SUM(C.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
		                                FROM tz_Balance AS B,balance_Energy AS C, system_Organization D
		                                WHERE B.BalanceId=C.KeyId 
		                                and B.TimeStamp= '{1}'
		                                and B.StaticsCycle = 'day'
		                                and (C.VariableId='clinker_ClinkerOutput'
                                            or C.VariableId='cement_CementOutput')
		                                and C.OrganizationID = D.OrganizationID
		                                GROUP BY D.LevelCode, C.VariableId) M on  M.LevelCode like A.LevelCode + '%'
                                where (A.LevelType = 'Company')
                                {0}
                                group by A.OrganizationID, A.LevelCode,A.Name, M.VariableId
                                union all
                                SELECT A.OrganizationID as OrganizationId,A.LevelCode as LevelCode,A.Name as Name, M.VariableId + '_month' as VariableId, sum(M.TotalPeakValleyFlatB) as value
                                    FROM system_Organization AS A
	                                left join 
	                                (SELECT D.LevelCode as LevelCode,C.VariableId AS VariableId, SUM(C.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
		                                FROM tz_Balance AS B,balance_Energy AS C, system_Organization D
		                                WHERE B.BalanceId=C.KeyId 
		                                and B.TimeStamp >= '{2}' and B.TimeStamp <= '{1}'
		                                and B.StaticsCycle = 'day'
		                                and (C.VariableId='clinker_ClinkerOutput'
                                            or C.VariableId='cement_CementOutput')
		                                and C.OrganizationID = D.OrganizationID
		                                GROUP BY D.LevelCode, C.VariableId) M on  M.LevelCode like A.LevelCode + '%'
                                where (A.LevelType = 'Company')
                                {0}
                                group by A.OrganizationID, A.LevelCode,A.Name, M.VariableId
                              union all
                              SELECT A.OrganizationID as OrganizationId,A.LevelCode as LevelCode,A.Name as Name, M.VariableId + '_year' as VariableId, sum(M.TotalPeakValleyFlatB) as value
                                    FROM system_Organization AS A
	                                left join 
	                                (SELECT D.LevelCode as LevelCode,C.VariableId AS VariableId, SUM(C.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
		                                FROM tz_Balance AS B,balance_Energy AS C, system_Organization D
		                                WHERE B.BalanceId=C.KeyId 
		                                and B.TimeStamp < '{3}' and B.TimeStamp >= '{4}' 
		                                and B.StaticsCycle = 'month'
		                                and (C.VariableId='clinker_ClinkerOutput'
                                            or C.VariableId='cement_CementOutput')
		                                and C.OrganizationID = D.OrganizationID
		                                GROUP BY D.LevelCode, C.VariableId) M on  M.LevelCode like A.LevelCode + '%'
                                where (A.LevelType = 'Company')
                                {0}
                                group by A.OrganizationID, A.LevelCode,A.Name, M.VariableId
                            union all
                            SELECT A.OrganizationID as OrganizationId,A.LevelCode as LevelCode,A.Name as Name, M.VariableId + '_plan' as VariableId, sum(M.value) as value
                                    FROM system_Organization AS A
	                                left join 
	                                (select A.LevelCode 
							              ,D.VariableId as VariableId
								          ,C.{6} as value
                                        from system_Organization A, tz_Plan B, plan_ProductionYearlyPlan C, plan_ProductionPlan_Template D
                                        where A.OrganizationID = B.OrganizationID
								        and B.Date ='{5}'
								        and B.PlanType = 'Production'
								        and B.Statue = 1
								        and B.KeyID =C.KeyID
								        and C.QuotasID = D.QuotasID
								        and C.QuotasID in ('产量_回转窑','产量_水泥磨')) M on  M.LevelCode like A.LevelCode + '%'
                                where (A.LevelType = 'Company')
                                {0}
                                group by A.OrganizationID, A.LevelCode,A.Name, M.VariableId
                            ) Z 
                            ORDER BY Z.LevelCode,Z.VariableId";

            StringBuilder levelCodesParameter = new StringBuilder();
            foreach (var levelCode in myLevelCodes)
            {
                levelCodesParameter.Append("A.LevelCode like ");
                levelCodesParameter.Append("'");
                levelCodesParameter.Append(levelCode + "%");
                levelCodesParameter.Append("'");
                levelCodesParameter.Append(" OR ");
                levelCodesParameter.Append(string.Format("CHARINDEX(A.LevelCode,'{0}')>0", levelCode));
                levelCodesParameter.Append(" OR ");
            }
            levelCodesParameter.Remove(levelCodesParameter.Length - 4, 4);
            string m_levelCodesParameter = levelCodesParameter.ToString();
            if (m_levelCodesParameter != "")
            {
                m_levelCodesParameter = " and (" + m_levelCodesParameter + ")";
            }
            else
            {
                m_levelCodesParameter = " and A.OrganizationID <> A.OrganizationID";
            }
            m_Sql = string.Format(m_Sql, m_levelCodesParameter, myDate.ToString("yyyy-MM-dd"), myDate.ToString("yyyy-MM-01"), myDate.ToString("yyyy-MM"), myDate.ToString("yyyy-01"), myDate.ToString("yyyy"), m_CurrentMonth);
            try
            {
                DataTable result = _dataFactory.Query(m_Sql);
                m_ReturnJsonString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(GetGlobalCompleteData(result));
            }
            catch
            {

            }

            return m_ReturnJsonString;
        }
        /// <summary>
        /// 构造显示表结构
        /// </summary>
        /// <param name="myResultTable"></param>
        /// <returns></returns>
        private static DataTable GetGlobalCompleteData(DataTable myResultTable)
        {
            DataTable m_GlobalCompleteTable = new DataTable();
            m_GlobalCompleteTable.Columns.Add("OrganizationId", typeof(string));
            m_GlobalCompleteTable.Columns.Add("LevelCode", typeof(string));
            m_GlobalCompleteTable.Columns.Add("Name", typeof(string));
            m_GlobalCompleteTable.Columns.Add("clinker_ClinkerOutput_day", typeof(decimal));
            m_GlobalCompleteTable.Columns.Add("clinker_ClinkerOutput_month", typeof(decimal));
            m_GlobalCompleteTable.Columns.Add("clinker_ClinkerOutput_year", typeof(decimal));
            m_GlobalCompleteTable.Columns.Add("clinker_ClinkerOutput_plan", typeof(decimal));
            m_GlobalCompleteTable.Columns.Add("cement_CementOutput_day", typeof(decimal));
            m_GlobalCompleteTable.Columns.Add("cement_CementOutput_month", typeof(decimal));
            m_GlobalCompleteTable.Columns.Add("cement_CementOutput_year", typeof(decimal));
            m_GlobalCompleteTable.Columns.Add("cement_CementOutput_plan", typeof(decimal));

            string m_OrganizationId = "";
            if (myResultTable != null)
            {
                for (int i = 0; i < myResultTable.Rows.Count; i++)
                {
                    if (myResultTable.Rows[i]["OrganizationID"].ToString() != m_OrganizationId)
                    {
                        m_OrganizationId = myResultTable.Rows[i]["OrganizationID"].ToString();
                        DataRow m_NewDataRow = m_GlobalCompleteTable.NewRow();
                        m_NewDataRow["OrganizationId"] = m_OrganizationId;
                        m_NewDataRow["LevelCode"] = myResultTable.Rows[i]["LevelCode"].ToString();
                        m_NewDataRow["Name"] = myResultTable.Rows[i]["Name"].ToString();
                        GetGlobalCompleteRowData(ref m_NewDataRow, m_OrganizationId, myResultTable);
                        m_GlobalCompleteTable.Rows.Add(m_NewDataRow);
                    }
                }
            }
            return m_GlobalCompleteTable;

        }
        private static void GetGlobalCompleteRowData(ref DataRow myNewDataRow, string myOrganizationId, DataTable myResultTable)
        {
            if (myResultTable != null)
            {
                for (int i = 0; i < myResultTable.Rows.Count; i++)    //找熟料和水泥的日统计
                {
                    if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "clinker_ClinkerOutput_day")
                    { //找熟料日统计
                        decimal m_ValueTemp = myResultTable.Rows[i]["value"] != DBNull.Value ? (decimal)myResultTable.Rows[i]["value"] : 0.0m;
                        myNewDataRow["clinker_ClinkerOutput_day"] = m_ValueTemp;
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "cement_CementOutput_day")
                    { //找水泥日统计
                        decimal m_ValueTemp = myResultTable.Rows[i]["value"] != DBNull.Value ? (decimal)myResultTable.Rows[i]["value"] : 0.0m;
                        myNewDataRow["cement_CementOutput_day"] = m_ValueTemp;
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "clinker_ClinkerOutput_month")
                    { //找熟料月统计
                        decimal m_ValueTemp = myResultTable.Rows[i]["value"] != DBNull.Value ? (decimal)myResultTable.Rows[i]["value"] : 0.0m;
                        decimal m_NewDataRowValueTemp = myNewDataRow["clinker_ClinkerOutput_month"] != DBNull.Value ? (decimal)myNewDataRow["clinker_ClinkerOutput_month"] : 0.0m;

                        myNewDataRow["clinker_ClinkerOutput_month"] = m_ValueTemp;
                        myNewDataRow["clinker_ClinkerOutput_year"] = m_NewDataRowValueTemp;
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "cement_CementOutput_month")
                    { //找水泥月统计
                        decimal m_ValueTemp = myResultTable.Rows[i]["value"] != DBNull.Value ? (decimal)myResultTable.Rows[i]["value"] : 0.0m;
                        decimal m_NewDataRowValueTemp = myNewDataRow["cement_CementOutput_month"] != DBNull.Value ? (decimal)myNewDataRow["cement_CementOutput_month"] : 0.0m;

                        myNewDataRow["cement_CementOutput_month"] = m_ValueTemp;
                        myNewDataRow["cement_CementOutput_year"] = m_NewDataRowValueTemp;
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "clinker_ClinkerOutput_year")
                    { //找熟料年统计
                        decimal m_ValueTemp = myResultTable.Rows[i]["value"] != DBNull.Value ? (decimal)myResultTable.Rows[i]["value"] : 0.0m;
                        decimal m_NewDataRowValueTemp = myNewDataRow["clinker_ClinkerOutput_year"] != DBNull.Value ? (decimal)myNewDataRow["clinker_ClinkerOutput_year"] : 0.0m;

                        myNewDataRow["clinker_ClinkerOutput_year"] = m_ValueTemp + m_NewDataRowValueTemp;
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "cement_CementOutput_year")
                    { //找水泥年统计
                        decimal m_ValueTemp = myResultTable.Rows[i]["value"] != DBNull.Value ? (decimal)myResultTable.Rows[i]["value"] : 0.0m;
                        decimal m_NewDataRowValueTemp = myNewDataRow["cement_CementOutput_year"] != DBNull.Value ? (decimal)myNewDataRow["cement_CementOutput_year"] : 0.0m;

                        myNewDataRow["cement_CementOutput_year"] = m_ValueTemp + m_NewDataRowValueTemp;
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "clinker_ClinkerOutput_plan")
                    { //找熟料月计划
                        decimal m_ValueTemp = myResultTable.Rows[i]["value"] != DBNull.Value ? (decimal)myResultTable.Rows[i]["value"] : 0.0m;
                        decimal m_NewDataRowValueTemp = myNewDataRow["clinker_ClinkerOutput_plan"] != DBNull.Value ? (decimal)myNewDataRow["clinker_ClinkerOutput_plan"] : 0.0m;

                        myNewDataRow["clinker_ClinkerOutput_plan"] = m_ValueTemp + m_NewDataRowValueTemp;
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "cement_CementOutput_plan")
                    { //找水泥月计划
                        decimal m_ValueTemp = myResultTable.Rows[i]["value"] != DBNull.Value ? (decimal)myResultTable.Rows[i]["value"] : 0.0m;
                        decimal m_NewDataRowValueTemp = myNewDataRow["cement_CementOutput_plan"] != DBNull.Value ? (decimal)myNewDataRow["cement_CementOutput_plan"] : 0.0m;

                        myNewDataRow["cement_CementOutput_plan"] = m_ValueTemp + m_NewDataRowValueTemp;
                    }
                }

            }
        }
        /// <summary>
        /// 获得分公司计划完成信息
        /// </summary>
        /// <param name="myDate"></param>
        /// <param name="myLevelCode"></param>
        /// <returns></returns>
        public static string GetCompanyComplete(DateTime myDate, string myLevelCode)
        {
            string m_ReturnJsonString = "{\"rows\":[],\"total\":0}";
            DataTable m_CompanyCompleteTable = GetCompanyCompleteStruct(myLevelCode);
            if (m_CompanyCompleteTable != null)
            {
                GetMaterialWeightCompleteResult(myDate, myLevelCode, "Value_Day", ref m_CompanyCompleteTable);
                GetMaterialWeightCompleteResult(myDate, myLevelCode, "Value_Month", ref m_CompanyCompleteTable);
                GetIndicatorCompleteResult(myDate, myLevelCode, "Value_Day", ref m_CompanyCompleteTable);
                GetIndicatorCompleteResult(myDate, myLevelCode, "Value_Month", ref m_CompanyCompleteTable);
                GetProductionPlan(myDate, myLevelCode, "Value_Plan", ref m_CompanyCompleteTable);
                GetProductionPlanResultDifference(ref m_CompanyCompleteTable);
                m_ReturnJsonString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_CompanyCompleteTable);
            }     
            return m_ReturnJsonString;
        }
        private static DataTable GetCompanyCompleteStruct(string myLevelCode)
        {
            string m_Sql = @"select A.OrganizationID as OrganizationId, 
                                C.Name as ProcessName, 
                                B.EquipmentName as Name,
                                '' as DataItem,
                                C.VariableId as VariableId,
                                B.EquipmentCommonId as EquipmentCommonId,
                                B.EquipmentId as EquipmentId, 
                                convert(decimal(18,4),'0.0') as Value_Day,
                                convert(decimal(18,4),'0.0') as Value_Month,
                                convert(decimal(18,4),'0.0') as Value_Plan,
                                convert(decimal(18,4),'0.0') as Value_Deviation
                                from system_Organization A, equipment_EquipmentDetail B, equipment_EquipmentCommonInfo C, system_MasterMachineDescription D
                                where A.LevelCode like '{0}%'
                                and A.OrganizationID = B.OrganizationID
                                and B.EquipmentCommonId = C.EquipmentCommonId
                                and B.EquipmentId = D.ID
                                order by C.DisplayIndex, B.EquipmentName";
            //and C.EquipmentCommonId in ('CementGrind','CementPacker','CoalGrind','MineCrusher','RawMaterialsGrind','RotaryKiln')
            m_Sql = string.Format(m_Sql, myLevelCode);
            try
            {
                DataTable m_Result = _dataFactory.Query(m_Sql);
                DataTable m_CompanyCompleteTable = m_Result.Clone();
                if (m_Result != null)
                {
                    for (int i = 0; i < m_Result.Rows.Count; i++)
                    {
                        m_CompanyCompleteTable.Rows.Add(m_Result.Rows[i].ItemArray);
                        m_CompanyCompleteTable.Rows.Add(m_Result.Rows[i].ItemArray);
                        m_CompanyCompleteTable.Rows.Add(m_Result.Rows[i].ItemArray);
                        m_CompanyCompleteTable.Rows.Add(m_Result.Rows[i].ItemArray);

                        m_CompanyCompleteTable.Rows[i * 4]["DataItem"] = "产量";
                        m_CompanyCompleteTable.Rows[i * 4 + 1]["DataItem"] = "运转时间";
                        m_CompanyCompleteTable.Rows[i * 4 + 2]["DataItem"] = "台时产量";
                        m_CompanyCompleteTable.Rows[i * 4 + 3]["DataItem"] = "运转率";
                    }
                }
                return m_CompanyCompleteTable;
            }
            catch
            {
                return null;
            }
        }
        private static void GetMaterialWeightCompleteResult(DateTime myDate, string myLevelCode, string myColumnName, ref DataTable myCompanyCompleteStructTable)
        {
            string m_SatartTime = "";
            string m_EndTime = "";
            if (myColumnName == "Value_Day")
            {
                m_SatartTime = myDate.ToString("yyyy-MM-dd");
                m_EndTime = myDate.ToString("yyyy-MM-dd");
            }
            else if (myColumnName == "Value_Month")
            {
                m_SatartTime = myDate.ToString("yyyy-MM-01");
                if (myDate.ToString("yyyyMM") == DateTime.Now.ToString("yyyyMM"))
                {
                    m_EndTime = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                }
                else
                {
                    m_EndTime = DateTime.Parse(m_SatartTime).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
                }
            } 
            string m_Sql = @" select C.VariableId as EquipmentId,
                                sum(C.TotalPeakValleyFlatB) as Value
                                from system_Organization A, tz_Balance B, balance_Production C
                                where A.LevelCode like '{2}%'
                                and A.OrganizationID = B.OrganizationID
								and B.TimeStamp >='{0}'
								and B.TimeStamp <='{1}'
								and B.StaticsCycle = 'day'
								and B.BalanceId = C.KeyId
								and C.ValueType = 'MaterialWeight'
								and C.VariableType = 'EquipmentOutput'
								group by C.VariableId";
            m_Sql = string.Format(m_Sql, m_SatartTime, m_EndTime, myLevelCode);
            try
            {
                DataTable m_MaterialWeightTable = _dataFactory.Query(m_Sql);
                if (m_MaterialWeightTable != null && myCompanyCompleteStructTable != null)
                {
                    for (int i = 0; i < myCompanyCompleteStructTable.Rows.Count; i++)
                    {
                        if (myCompanyCompleteStructTable.Rows[i]["DataItem"].ToString() == "产量")
                        {
                            for (int j = 0; j < m_MaterialWeightTable.Rows.Count; j++)
                            {
                                if (myCompanyCompleteStructTable.Rows[i]["EquipmentId"].ToString() == m_MaterialWeightTable.Rows[j]["EquipmentId"].ToString())
                                {
                                    decimal m_ResultValue = m_MaterialWeightTable.Rows[j]["Value"] != DBNull.Value?(decimal)m_MaterialWeightTable.Rows[j]["Value"]:0.0m;
                                    myCompanyCompleteStructTable.Rows[i][myColumnName] = m_ResultValue;
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception)
            {
            }
        }
        private static void GetIndicatorCompleteResult(DateTime myDate, string myLevelCode, string myColumnName, ref DataTable myCompanyCompleteStructTable)
        {
            string m_SatartTime = "";
            string m_EndTime = "";
            if (myColumnName == "Value_Day")
            {
                m_SatartTime = myDate.ToString("yyyy-MM-dd");
                m_EndTime = myDate.ToString("yyyy-MM-dd");
            }
            else if (myColumnName == "Value_Month")
            {
                m_SatartTime = myDate.ToString("yyyy-MM-01");
                m_EndTime = myDate.ToString("yyyy-MM-dd");
            }

            List<string> m_EquipmentCommonIdList = new List<string>();
            List<string> m_OrganizationIdList = new List<string>();
            string m_EquipmentCommonIdTemp = "";
            string m_OrganizationIdTemp = "";

            DataView m_OrderEquipmentCommonViewTemp = myCompanyCompleteStructTable.Copy().DefaultView;
            m_OrderEquipmentCommonViewTemp.Sort = "EquipmentCommonId";
            DataTable m_OrderEquipmentCommonTableTemp = m_OrderEquipmentCommonViewTemp.ToTable();
            for (int i = 0; i < m_OrderEquipmentCommonTableTemp.Rows.Count; i++)
            {
                if (m_EquipmentCommonIdTemp != m_OrderEquipmentCommonTableTemp.Rows[i]["EquipmentCommonId"].ToString())
                {
                    m_EquipmentCommonIdTemp = m_OrderEquipmentCommonTableTemp.Rows[i]["EquipmentCommonId"].ToString();
                    m_EquipmentCommonIdList.Add(m_EquipmentCommonIdTemp);
                }
            }
            DataView m_OrderOrganizationIdViewTemp = myCompanyCompleteStructTable.Copy().DefaultView;
            m_OrderOrganizationIdViewTemp.Sort = "OrganizationId";
            DataTable m_OrderOrganizationIdTableTemp = m_OrderOrganizationIdViewTemp.ToTable();
            for (int i = 0; i < m_OrderOrganizationIdTableTemp.Rows.Count; i++)
            {
                if (m_OrganizationIdTemp != m_OrderOrganizationIdTableTemp.Rows[i]["OrganizationId"].ToString())
                {
                    m_OrganizationIdTemp = m_OrderOrganizationIdTableTemp.Rows[i]["OrganizationId"].ToString();
                    m_OrganizationIdList.Add(m_OrganizationIdTemp);
                }

            }
            for (int i = 0; i < m_EquipmentCommonIdList.Count; i++)
            {
                for (int w = 0; w < m_OrganizationIdList.Count; w++)
                {
                    DataTable m_CompleteResultTable = RunIndicators.EquipmentRunIndicators.GetEquipmentUtilizationByCommonId(new string[] { "运转时间", "台时产量", "运转率" }, m_EquipmentCommonIdList[i], m_OrganizationIdList[w], m_SatartTime, m_EndTime, _dataFactory);
                    if (m_CompleteResultTable != null)
                    {
                        for (int j = 0; j < m_CompleteResultTable.Rows.Count; j++)
                        {
                            for (int z = 0; z < myCompanyCompleteStructTable.Rows.Count; z++)
                            {
                                if (m_CompleteResultTable.Rows[j]["EquipmentId"].ToString() == myCompanyCompleteStructTable.Rows[z]["EquipmentId"].ToString()
                                    && myCompanyCompleteStructTable.Rows[z]["DataItem"].ToString() == "运转时间")
                                {
                                    myCompanyCompleteStructTable.Rows[z][myColumnName] = (decimal)m_CompleteResultTable.Rows[j]["运转时间"];
                                }
                                else if (m_CompleteResultTable.Rows[j]["EquipmentId"].ToString() == myCompanyCompleteStructTable.Rows[z]["EquipmentId"].ToString()
                                    && myCompanyCompleteStructTable.Rows[z]["DataItem"].ToString() == "台时产量")
                                {
                                    myCompanyCompleteStructTable.Rows[z][myColumnName] = (decimal)m_CompleteResultTable.Rows[j]["台时产量"];
                                }
                                else if (m_CompleteResultTable.Rows[j]["EquipmentId"].ToString() == myCompanyCompleteStructTable.Rows[z]["EquipmentId"].ToString()
                                    && myCompanyCompleteStructTable.Rows[z]["DataItem"].ToString() == "运转率")
                                {
                                    myCompanyCompleteStructTable.Rows[z][myColumnName] = (decimal)m_CompleteResultTable.Rows[j]["运转率"];
                                }
                            }
                        }
                    }
                }
            }
        }
        private static void GetProductionPlan(DateTime myDate, string myLevelCode, string myColumnName, ref DataTable myCompanyCompleteStructTable)
        {
            string[] m_Month = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            string m_CurrentYear = myDate.Year.ToString();
            string m_CurrentMonth = m_Month[myDate.Month - 1];
            string m_Sql = @"select B.OrganizationID,C.*
                                from system_Organization A, tz_Plan B, plan_ProductionYearlyPlan C
                                where A.LevelCode like '{0}%'
                                and A.OrganizationID = B.OrganizationID
								and B.Date >='{1}'
								and B.PlanType = 'Production'
								and B.Statue = 1
								and B.KeyID =C.KeyID";
            m_Sql = string.Format(m_Sql, myLevelCode, m_CurrentYear);
            try
            {
                DataTable m_ProductionPlanTable = _dataFactory.Query(m_Sql);
                if (m_ProductionPlanTable != null && myCompanyCompleteStructTable != null)
                {
                    for (int i = 0; i < myCompanyCompleteStructTable.Rows.Count; i++)
                    {
                        for (int j = 0; j < m_ProductionPlanTable.Rows.Count; j++)
                        {
                            if (myCompanyCompleteStructTable.Rows[i]["EquipmentId"].ToString() == m_ProductionPlanTable.Rows[j]["EquipmentId"].ToString()
                                && myCompanyCompleteStructTable.Rows[i]["DataItem"].ToString() + "_" + myCompanyCompleteStructTable.Rows[i]["ProcessName"].ToString() == m_ProductionPlanTable.Rows[j]["QuotasID"].ToString())
                            {
                                decimal m_ResultValue = m_ProductionPlanTable.Rows[j][m_CurrentMonth] != DBNull.Value ? (decimal)m_ProductionPlanTable.Rows[j][m_CurrentMonth] : 0.0m;
                                myCompanyCompleteStructTable.Rows[i][myColumnName] = m_ResultValue;
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
            }
        }
        private static void GetProductionPlanResultDifference(ref DataTable m_CompanyCompleteTable)
        {
            if (m_CompanyCompleteTable != null)
            {
                for (int i = 0; i < m_CompanyCompleteTable.Rows.Count; i++)
                {
                    decimal m_PlanValue = m_CompanyCompleteTable.Rows[i]["Value_Plan"] != DBNull.Value ? (decimal)m_CompanyCompleteTable.Rows[i]["Value_Plan"] : 0.0m;
                    decimal m_MonthCompleteResult = m_CompanyCompleteTable.Rows[i]["Value_Month"] != DBNull.Value ? (decimal)m_CompanyCompleteTable.Rows[i]["Value_Month"] : 0.0m;
                    m_CompanyCompleteTable.Rows[i]["Value_Deviation"] = m_PlanValue - m_MonthCompleteResult;
                }
            }
        }
        public static List<string> GetAllParentIdAndChildrenIdByIds(List<string> myOrganizations)
        {
            string m_Sql = @"Select 
                                distinct A.OrganizationID as OrganizationId, 
                                A.Name as Name,
					            A.LevelCode as LeveCode
                                from system_Organization A, system_Organization B
					            where A.Enabled = 1 
                                and (A.LevelCode like B.LevelCode + '%' or CHARINDEX(A.LevelCode, B.LevelCode) > 0)
							    and B.OrganizationID in ({0})";
            string m_SqlCondition = "''";
            if (myOrganizations != null)
            {
                for (int i = 0; i < myOrganizations.Count; i++)
                {
                    if (i == 0)
                    {
                        m_SqlCondition = string.Format("'{0}'", myOrganizations[i]);
                    }
                    else
                    {
                        m_SqlCondition = m_SqlCondition + string.Format(",'{0}'", myOrganizations[i]);
                    }
                }
            }
            m_Sql = string.Format(m_Sql, m_SqlCondition);
            List<string> m_OrganizationIdList = new List<string>();
            try
            {
                DataTable m_OrganizationIdTable = _dataFactory.Query(m_Sql);
                if (m_OrganizationIdTable != null)
                {
                    for (int i = 0; i < m_OrganizationIdTable.Rows.Count; i++)
                    {
                        m_OrganizationIdList.Add(m_OrganizationIdTable.Rows[i]["OrganizationId"].ToString());
                    }
                }
                return m_OrganizationIdList;
            }
            catch (Exception)
            {
                return m_OrganizationIdList;
            }
        }
        public static string GetIndicatorItems()
        {
            DataTable m_RunIndicatorsItemsTableTemp = RunIndicators.RunIndicatorsItems.GetRunIndicatorsItemsTable();
            DataRow[] m_EquipmentUtilization = m_RunIndicatorsItemsTableTemp.Select(string.Format("IndicatorType = '{0}' or IndicatorType = '{1}'", "MaterialWeight", "EquipmentUtilization"));
            DataTable m_RunIndicatorsItemsTable = m_RunIndicatorsItemsTableTemp.Clone();
            for(int i=0;i< m_EquipmentUtilization.Length;i++)
            {
                m_RunIndicatorsItemsTable.Rows.Add(m_EquipmentUtilization[i].ItemArray);
            }
            
            string ValueString = EasyUIJsonParser.TreeJsonParser.DataTableToJsonByLevelCodeWithIdColumn(m_RunIndicatorsItemsTable, "LevelCode", "IndicatorId", "IndicatorName");
            return ValueString;
        }

        public static string GetEquipmentCommonInfo()
        {
            string m_Sql = @"SELECT distinct A.EquipmentCommonId as EquipmentCommonId
                            ,A.Name as Name
                            ,A.VariableId as VariableId
							,A.DisplayIndex
                        FROM equipment_EquipmentCommonInfo A, equipment_EquipmentDetail B,system_MasterMachineDescription C
                        where A.EquipmentCommonId = B.EquipmentCommonId
	                    and B.Enabled = 1
                        and B.EquipmentId = C.Id
                        order by A.DisplayIndex";
            try
            {
                DataTable m_EquipmentCommonInfoTable = _dataFactory.Query(m_Sql);
                string m_ReturnString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_EquipmentCommonInfoTable);
                return m_ReturnString;

            }
            catch
            {
                return "{\"rows\":[],\"total\":0}";
            }
        }
        public static string GetSpecificationsInfo(string myEquipmentCommonId)
        {
            string m_Sql = @"SELECT distinct A.Specifications as id, 
                                  A.Specifications as text
                                  FROM equipment_EquipmentDetail A,equipment_EquipmentCommonInfo B
                                  where A.Enabled = 1
                                  and A.Specifications is not null
                                  and A.Specifications <> ''
                                  and A.EquipmentCommonId = B.EquipmentCommonId
                                  and B.EquipmentCommonId = '{0}'";
            m_Sql = string.Format(m_Sql, myEquipmentCommonId);
            try
            {
                DataTable m_SpecificationsInfoTable = _dataFactory.Query(m_Sql);
                string m_ReturnString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_SpecificationsInfoTable);
                return m_ReturnString;

            }
            catch
            {
                return "{\"rows\":[],\"total\":0}";
            }
        }
        /////////////////////////////以下是为获取柱状图///////////////////////////////
        /// <summary>
        /// 
        /// </summary>
        /// <param name="myDate"></param>
        /// <param name="myIndicatorId"></param>
        /// <param name="myIndicatorName"></param>
        /// <param name="myOrganizationIdList"></param>
        /// <returns></returns>
        public static string GetProductionPlanAndComplete(string myDate, string myIndicatorId, string myEquipmentCommonId, string mySpecifications, List<string> myOrganizationIds)
        {
            string m_Specifications = "";
            if (mySpecifications != "All")
            {
                m_Specifications = mySpecifications;
            }
            DataTable m_CompleteTable = GetProductionCompleteData(myDate, myIndicatorId, myEquipmentCommonId, m_Specifications, myOrganizationIds);
            DataTable m_PlanTable = GetProductionPlanData(myDate, myIndicatorId, myEquipmentCommonId, m_Specifications, myOrganizationIds);
            string m_ReturnValue = GetChartString(m_CompleteTable, m_PlanTable, myIndicatorId, myEquipmentCommonId, m_Specifications, myOrganizationIds);
            return m_ReturnValue;
        }
        
        private static DataTable GetProductionCompleteData(string myDate, string myIndicatorId, string myEquipmentCommonId, string mySpecifications, List<string> myOrganizationIds)
        {
            DateTime m_CurrentDate = DateTime.Parse(myDate);
            string m_StartDate = m_CurrentDate.ToString("yyyy-MM") + "-01";
            string m_EndDate = m_CurrentDate.ToString("yyyy-MM-dd");
            DataTable m_ProductionCompleteTable = new DataTable();
            m_ProductionCompleteTable.Columns.Add("OrganizationID", typeof(string));
            m_ProductionCompleteTable.Columns.Add("EquipmentName", typeof(string));
            m_ProductionCompleteTable.Columns.Add("EquipmentId", typeof(string));
            m_ProductionCompleteTable.Columns.Add("QuotasName", typeof(string));
            m_ProductionCompleteTable.Columns.Add("Value", typeof(decimal));

            DataTable m_IndicatorItems = RunIndicators.RunIndicatorsItems.GetRunIndicatorsItemsTable();
            DataRow[] m_IndicatorItemById = m_IndicatorItems.Select(string.Format("IndicatorId = '{0}'", myIndicatorId));
            if (m_IndicatorItemById.Length > 0)
            {
                for (int i = 0; i < myOrganizationIds.Count; i++)
                {
                    if (m_IndicatorItemById[0]["IndicatorType"].ToString() == "EquipmentUtilization" || m_IndicatorItemById[0]["IndicatorType"].ToString() == "MaterialWeight")
                    {
                        DataTable m_EquipmentUtilizationTable = RunIndicators.EquipmentRunIndicators.GetEquipmentUtilizationByCommonId(new string[] { myIndicatorId }, myEquipmentCommonId, mySpecifications, myOrganizationIds[i], m_StartDate, m_EndDate, _dataFactory);
                        if (m_EquipmentUtilizationTable != null)
                        {
                            for (int j = 0; j < m_EquipmentUtilizationTable.Rows.Count; j++)
                            {
                                if (m_EquipmentUtilizationTable.Rows[j][myIndicatorId] != DBNull.Value)
                                {
                                    m_ProductionCompleteTable.Rows.Add(myOrganizationIds[i], m_EquipmentUtilizationTable.Rows[j]["EquipmentName"].ToString(), m_EquipmentUtilizationTable.Rows[j]["EquipmentId"].ToString(), "", (decimal)m_EquipmentUtilizationTable.Rows[j][myIndicatorId]);
                                }
                            }
                        }
                    }
                    else if (m_IndicatorItemById[0]["IndicatorType"].ToString() == "MachineHalt")
                    {
                        int m_StaticsTime = 0;
                        if (RunIndicators.RunIndicatorsItems.MachineHaltItemDetail.ContainsKey(myIndicatorId))
                        {
                            m_StaticsTime = RunIndicators.RunIndicatorsItems.MachineHaltItemDetail[myIndicatorId].StatisticsTime;
                        }
                        DataTable m_RunIndictorsDetailTable = RunIndicators.EquipmentHalt.GetEquipmentHaltDetail(myEquipmentCommonId, mySpecifications, myOrganizationIds[i], m_StaticsTime.ToString(), m_StartDate, m_EndDate, _dataFactory);
                        if (m_RunIndictorsDetailTable != null)
                        {
                            decimal m_ValueTemp = 0.0m;
                            for (int j = 0; j < m_RunIndictorsDetailTable.Rows.Count; j++)
                            {
                                try
                                {
                                    string m_ValueTempString = m_RunIndictorsDetailTable.Rows[j][RunIndicators.RunIndicatorsItems.MachineHaltItemDetail[myIndicatorId].HaltType] != DBNull.Value ? m_RunIndictorsDetailTable.Rows[j][RunIndicators.RunIndicatorsItems.MachineHaltItemDetail[myIndicatorId].HaltType].ToString() : "0";
                                    m_ValueTemp = m_ValueTemp + decimal.Parse(m_ValueTempString);
                                    m_ProductionCompleteTable.Rows.Add(myOrganizationIds[i], RunIndicators.RunIndicatorsItems.MachineHaltItemDetail[myIndicatorId].IndicatorName, m_RunIndictorsDetailTable.Rows[j]["EquipmentId"].ToString(), "", m_ValueTemp);
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                }
            }
            return m_ProductionCompleteTable;
        }
        private static DataTable GetProductionPlanData(string myDate, string myIndicatorId, string myEquipmentCommonId, string mySpecifications, List<string> myOrganizationIds)
        {
            string[] m_Month = new string[]{"January","February","March","April","May","June","July","August","September","October","November","December"};
            DateTime m_CurrentDate = DateTime.Parse(myDate);
            string m_CurrentYear = m_CurrentDate.Year.ToString();
            string m_CurrentMonth = m_Month[m_CurrentDate.Month - 1];
            string m_Sql = @"Select A.OrganizationID as OrganizationID, 
                                C.EquipmentName as EquipmentName, 
                                C.EquipmentId as EquipmentId, 
                                B.QuotasName as QuotasName, 
                                (case when B.{3} is null then 0 else B.{3} end) as Value 
                                from tz_Plan A, plan_ProductionYearlyPlan B, equipment_EquipmentDetail C, system_Organization D, system_Organization E
                                where  A.Date = '{4}'
                                and E.OrganizationID in ({0})
                                and D.LevelCode like E.LevelCode + '%'
                                and D.LevelType = 'Factory'
                                and A.OrganizationID = D.OrganizationID
                                and A.KeyID = B.KeyID
                                and C.EquipmentCommonId = '{2}'
                                and C.EquipmentId = B.EquipmentId
                                and B.QuotasID like '{1}%'
                                {5}";
            string m_OrganizationIds = "";
            if (myOrganizationIds != null)
            {
                for (int i = 0; i < myOrganizationIds.Count; i++)
                {
                    if (i == 0)
                    {
                        m_OrganizationIds = "'" + myOrganizationIds[i] + "'";
                    }
                    else
                    {
                        m_OrganizationIds = m_OrganizationIds + ",'" + myOrganizationIds[i] + "'";
                    }
                }
            }
            if (mySpecifications != "")
            {
                string m_SpecificationsCondition = string.Format(" and C.Specifications = '{0}'", mySpecifications);
                m_Sql = string.Format(m_Sql, m_OrganizationIds, myIndicatorId, myEquipmentCommonId, m_CurrentMonth, m_CurrentYear, m_SpecificationsCondition);
            }
            else
            {
                m_Sql = string.Format(m_Sql, m_OrganizationIds, myIndicatorId, myEquipmentCommonId, m_CurrentMonth, m_CurrentYear, "");
            }
           
            try
            {
                DataTable m_Result = _dataFactory.Query(m_Sql);
                return m_Result;
            }
            catch
            {
                return null;
            }
        }
        private static string GetChartString(DataTable myPerformance, DataTable myPlanData, string myIndicatorId, string myEquipmentCommonId, string mySpecifications, List<string> myOrganizationIds)
        {
            DataTable m_ChartDataTableStruct = new DataTable();
            List<string> m_ColumnNameList = new List<string>();
            string[] m_RowsName = new string[] { "实绩", "计划" };
            string m_UnitX = "";  // "单位";
            string m_UnitY = myIndicatorId;
            string m_Sql = @"Select distinct A.OrganizationID as OrganizationID, 
                                replace(D.Name,'公司','') + A.EquipmentName as Name, 
                                A.EquipmentId as EquipmentId
                                from equipment_EquipmentDetail A, system_Organization B
                                left join system_Organization D on (D.LevelCode like B.LevelCode + '%' or CHARINDEX(D.LevelCode, B.LevelCode) > 0) and D.LevelType = 'Company'
                                , system_Organization C
                                where C.OrganizationID in ({0})
                                and B.LevelCode like C.LevelCode + '%'
                                and B.LevelType = 'Factory'
                                and A.OrganizationID = B.OrganizationID
                                and A.EquipmentCommonId = '{1}'
                                {2}
								order by A.OrganizationID, replace(D.Name,'公司','') + A.EquipmentName";
            string m_OrganizationIds = "";
            if (myOrganizationIds != null)
            {
                for (int i = 0; i < myOrganizationIds.Count; i++)
                {
                    if (i == 0)
                    {
                        m_OrganizationIds = "'" + myOrganizationIds[i] + "'";
                    }
                    else
                    {
                        m_OrganizationIds = m_OrganizationIds + ",'" + myOrganizationIds[i] + "'";
                    }
                }
            }
            if (mySpecifications != "")
            {
                string m_SpecificationsCondition = string.Format(" and A.Specifications = '{0}'", mySpecifications);
                m_Sql = string.Format(m_Sql, m_OrganizationIds, myEquipmentCommonId, m_SpecificationsCondition);
            }
            else
            {
                m_Sql = string.Format(m_Sql, m_OrganizationIds, myEquipmentCommonId, "");
            }
             
            try
            {
                DataTable m_Result = _dataFactory.Query(m_Sql);
                if (m_Result != null)
                {
                    m_ChartDataTableStruct.Rows.Add(m_ChartDataTableStruct.NewRow());
                    m_ChartDataTableStruct.Rows.Add(m_ChartDataTableStruct.NewRow());
                    for (int i = 0; i < m_Result.Rows.Count; i++)
                    {
                        string m_ColumnName = m_Result.Rows[i]["Name"].ToString();
                        m_ChartDataTableStruct.Columns.Add(m_ColumnName, typeof(decimal));
                        m_ColumnNameList.Add(m_ColumnName);

                        m_ChartDataTableStruct.Rows[0][m_ColumnName] = 0.0m;
                        m_ChartDataTableStruct.Rows[1][m_ColumnName] = 0.0m;

                        if (myPerformance != null)
                        {
                            for (int j = 0; j < myPerformance.Rows.Count; j++)
                            {
                                if (myPerformance.Rows[j]["EquipmentId"].ToString() == m_Result.Rows[i]["EquipmentId"].ToString())
                                {
                                    m_ChartDataTableStruct.Rows[0][m_ColumnName] = myPerformance.Rows[j]["Value"];
                                }
                            }
                        }
                        if (myPlanData != null)
                        {
                            for (int j = 0; j < myPlanData.Rows.Count; j++)
                            {
                                if (myPlanData.Rows[j]["EquipmentId"].ToString() == m_Result.Rows[i]["EquipmentId"].ToString())
                                {
                                    m_ChartDataTableStruct.Rows[1][m_ColumnName] = myPlanData.Rows[j]["Value"];
                                }
                            }
                        }
                    }

                }
                string m_ChartData = EasyUIJsonParser.ChartJsonParser.GetGridChartJsonString(m_ChartDataTableStruct, m_ColumnNameList.ToArray(), m_RowsName, m_UnitX, m_UnitY, 1);
                return m_ChartData;
            }
            catch
            {
                return "{\"rows\":[],\"total\":0}";
            }
            


        }
        //////////////////////////////////////////////////////////////
    }
}
