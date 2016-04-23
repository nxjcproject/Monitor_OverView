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
                                group by A.OrganizationID, A.LevelCode,A.Name, M.VariableId) Z 
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
            m_Sql = string.Format(m_Sql, m_levelCodesParameter, myDate.ToString("yyyy-MM-dd"), myDate.ToString("yyyy-MM-01"), myDate.ToString("yyyy-MM"), myDate.ToString("yyyy-01"));
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
                        myNewDataRow["clinker_ClinkerOutput_day"] = (decimal)myResultTable.Rows[i]["value"];
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "cement_CementOutput_day")
                    { //找水泥日统计
                        myNewDataRow["cement_CementOutput_day"] = (decimal)myResultTable.Rows[i]["value"];
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "clinker_ClinkerOutput_month")
                    { //找熟料月统计
                        myNewDataRow["clinker_ClinkerOutput_month"] = (decimal)myResultTable.Rows[i]["value"];
                        myNewDataRow["clinker_ClinkerOutput_year"] = (decimal)myNewDataRow["clinker_ClinkerOutput_month"];
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "cement_CementOutput_month")
                    { //找水泥月统计
                        myNewDataRow["cement_CementOutput_month"] = myResultTable.Rows[i]["value"];
                        myNewDataRow["cement_CementOutput_year"] = (decimal)myNewDataRow["cement_CementOutput_month"];
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "clinker_ClinkerOutput_year")
                    { //找熟料年统计
                        myNewDataRow["clinker_ClinkerOutput_year"] = (decimal)myResultTable.Rows[i]["value"] + (decimal)myNewDataRow["clinker_ClinkerOutput_year"];
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "cement_CementOutput_year")
                    { //找水泥年统计
                        myNewDataRow["cement_CementOutput_year"] = (decimal)myResultTable.Rows[i]["value"] + (decimal)myNewDataRow["cement_CementOutput_year"];
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
            string m_ReturnJsonString = "";
            string m_Sql = @"select A.OrganizationID as OrganizationId, 
                                C.Name as ProcessName, 
                                substring(B.Name,1,2) + C.Name as Name,
                                '' as DataItem,
                                C.VariableId as VariableId,
                                0 as Value_Day,
                                0 as Value_Month,
                                0 as Value_Plan,
                                0 as Value_Deviation
                                from system_Organization A, tz_Formula B, formula_FormulaDetail C
                                where A.LevelCode like '{0}%'
                                and A.OrganizationID = B.OrganizationID
                                and B.KeyID = C.KeyID
                                and C.LevelType = 'Process'
                                and C.VariableId in ('rawMaterialsPreparation','coalPreparation', 'clinkerBurning','cementGrind')
                                order by C.Name, B.Name";
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
                        m_CompanyCompleteTable.Rows[i * 4 + 1]["DataItem"] = "运行时间";
                        m_CompanyCompleteTable.Rows[i * 4 + 2]["DataItem"] = "台时";
                        m_CompanyCompleteTable.Rows[i * 4 + 3]["DataItem"] = "运转率";
                    }
                }
                m_ReturnJsonString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_CompanyCompleteTable);
            }
            catch
            {

            }

            return m_ReturnJsonString;
        }
    }
}
