using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SqlServerDataAdapter;
namespace Monitor_OverView.Service.OverView
{
    public class View_EnergyData
    {
        private static readonly string[] Weekly = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        private static readonly ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(Monitor_OverView.Infrastruture.Configuration.ConnectionStringFactory.NXJCConnectionString);
        private static readonly AutoSetParameters.AutoGetEnergyConsumption_V1 AutoGetEnergyConsumption_V1 = new AutoSetParameters.AutoGetEnergyConsumption_V1(new SqlServerDataAdapter.SqlServerDataFactory(Monitor_OverView.Infrastruture.Configuration.ConnectionStringFactory.NXJCConnectionString));
        
        public View_EnergyData()
        {

        }
        ///////////////////////////////////////////////全局数据GetGlobalComplete///////////////////////////////////////////
        #region GetGlobalComplete
        /// <summary>
        /// 获得左上角的全局数据
        /// </summary>
        /// <param name="myDate"></param>
        /// <param name="myLevelCodes"></param>
        /// <returns></returns>
        public static string GetGlobalComplete(DateTime myDate, string[] myLevelCodes)
        {
            string m_ReturnJsonString = "{\"rows\":[],\"total\":0}";
            string m_Sql = @"Select Z.* from (
                            SELECT A.OrganizationID as OrganizationId,A.LevelCode as LevelCode,A.Name as Name, M.VariableId as VariableId, sum(M.TotalPeakValleyFlatB) as value
                                    FROM system_Organization AS A
	                                left join 
	                                (SELECT D.LevelCode as LevelCode,C.VariableId AS VariableId, SUM(C.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
		                                FROM tz_Balance AS B,balance_Energy AS C, system_Organization D
		                                WHERE B.BalanceId=C.KeyId 
		                                and B.TimeStamp= '{1}'
		                                and B.StaticsCycle = 'day'
		                                and C.VariableId in ('clinkerPreparation_ElectricityQuantity'
										,'rawMaterialsPreparation_ElectricityQuantity'
										,'cementPreparation_ElectricityQuantity'
                                        ,'clinkerElectricityGeneration_ElectricityQuantity'
										,'cementmill_ElectricityQuantity'
										,'clinker_ElectricityQuantity'
                                        ,'clinker_ClinkerOutput'
										,'cement_CementOutput'
										,'clinker_MixtureMaterialsOutput')
										and D.LevelType = 'ProductionLine'
                                        and D.Type in ('熟料','水泥磨')
		                                and C.OrganizationID = D.OrganizationID
		                                GROUP BY D.LevelCode, C.VariableId) M on  M.LevelCode like A.LevelCode + '%'
                                where (A.LevelType = 'Company')
                                {0}
                                group by A.OrganizationID, A.LevelCode, A.Name, M.VariableId
                             union all
                             SELECT A.OrganizationID as OrganizationId,A.LevelCode as LevelCode,A.Name as Name, M.VariableId as VariableId, sum(M.TotalPeakValleyFlatB) as value
                                    FROM system_Organization AS A
	                                left join 
	                                (SELECT D.LevelCode as LevelCode,C.VariableId AS VariableId, SUM(C.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
		                                FROM tz_Balance AS B,balance_Energy AS C, system_Organization D
		                                WHERE B.BalanceId=C.KeyId 
		                                and B.TimeStamp= '{1}'
		                                and B.StaticsCycle = 'day'
		                                and C.VariableId in ('cementPacking_ElectricityQuantity','auxiliaryProduction_ElectricityQuantity')
										and D.LevelType = 'Factory'
		                                and C.OrganizationID = D.OrganizationID
		                                GROUP BY D.LevelCode, C.VariableId) M on  M.LevelCode like A.LevelCode + '%'
                                where (A.LevelType = 'Company')
                                {0}
                                group by A.OrganizationID, A.LevelCode, A.Name, M.VariableId) Z
                                order by Z.LevelCode, Z.Name, Z.VariableId";

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
            m_Sql = string.Format(m_Sql, m_levelCodesParameter, myDate.ToString("yyyy-MM-dd"));
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
            m_GlobalCompleteTable.Columns.Add("Company_ElectricityQuantity", typeof(decimal));
            m_GlobalCompleteTable.Columns.Add("clinkerElectricityGeneration_ElectricityQuantity", typeof(decimal));
            m_GlobalCompleteTable.Columns.Add("auxiliaryProduction_ElectricityQuantity", typeof(decimal));
            m_GlobalCompleteTable.Columns.Add("cementPacking_ElectricityQuantity", typeof(decimal));
            m_GlobalCompleteTable.Columns.Add("rawMaterialsPreparation_ElectricityQuantity", typeof(decimal));
            m_GlobalCompleteTable.Columns.Add("clinkerPreparation_ElectricityQuantity", typeof(decimal));
            m_GlobalCompleteTable.Columns.Add("cementPreparation_ElectricityQuantity", typeof(decimal));
            m_GlobalCompleteTable.Columns.Add("rawMaterialsPreparation_ElectricityConsumption", typeof(decimal));
            m_GlobalCompleteTable.Columns.Add("clinkerPreparation_ElectricityConsumption", typeof(decimal));
            m_GlobalCompleteTable.Columns.Add("cementPreparation_ElectricityConsumption", typeof(decimal));
            m_GlobalCompleteTable.Columns.Add("clinker_MixtureMaterialsOutput", typeof(decimal));
            m_GlobalCompleteTable.Columns.Add("clinker_ClinkerOutput", typeof(decimal));
            m_GlobalCompleteTable.Columns.Add("cement_CementOutput", typeof(decimal));

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
            /////在第一行增加集团合计/////
            DataRow m_GroupDataRow = m_GlobalCompleteTable.NewRow();
            m_GroupDataRow["OrganizationId"] = "zc_nxjc";
            m_GroupDataRow["LevelCode"] = "O";
            m_GroupDataRow["Name"] = "宁夏建材集团";
            m_GroupDataRow["Company_ElectricityQuantity"] = m_GlobalCompleteTable.Compute("sum(Company_ElectricityQuantity)", "TRUE");
            m_GroupDataRow["clinkerElectricityGeneration_ElectricityQuantity"] = m_GlobalCompleteTable.Compute("sum(clinkerElectricityGeneration_ElectricityQuantity)", "TRUE");
            m_GroupDataRow["auxiliaryProduction_ElectricityQuantity"] = m_GlobalCompleteTable.Compute("sum(auxiliaryProduction_ElectricityQuantity)", "TRUE");
            m_GroupDataRow["cementPacking_ElectricityQuantity"] = m_GlobalCompleteTable.Compute("sum(cementPacking_ElectricityQuantity)", "TRUE");
            m_GroupDataRow["rawMaterialsPreparation_ElectricityQuantity"] = m_GlobalCompleteTable.Compute("sum(rawMaterialsPreparation_ElectricityQuantity)", "TRUE");
            m_GroupDataRow["clinkerPreparation_ElectricityQuantity"] = m_GlobalCompleteTable.Compute("sum(clinkerPreparation_ElectricityQuantity)", "TRUE");
            m_GroupDataRow["cementPreparation_ElectricityQuantity"] = m_GlobalCompleteTable.Compute("sum(cementPreparation_ElectricityQuantity)", "TRUE");
            m_GroupDataRow["clinker_MixtureMaterialsOutput"] = m_GlobalCompleteTable.Compute("sum(clinker_MixtureMaterialsOutput)", "TRUE");
            m_GroupDataRow["clinker_ClinkerOutput"] = m_GlobalCompleteTable.Compute("sum(clinker_ClinkerOutput)", "TRUE");
            m_GroupDataRow["cement_CementOutput"] = m_GlobalCompleteTable.Compute("sum(cement_CementOutput)", "TRUE");
            //////////////////////////////////////////////////////////////////////////
            m_GroupDataRow["rawMaterialsPreparation_ElectricityConsumption"] = (decimal)m_GroupDataRow["clinker_MixtureMaterialsOutput"] > 0 ? (decimal)m_GroupDataRow["rawMaterialsPreparation_ElectricityQuantity"] / (decimal)m_GroupDataRow["clinker_MixtureMaterialsOutput"] : 0.00m;
            m_GroupDataRow["clinkerPreparation_ElectricityConsumption"] = (decimal)m_GroupDataRow["clinker_ClinkerOutput"] > 0 ? (decimal)m_GroupDataRow["clinkerPreparation_ElectricityQuantity"] / (decimal)m_GroupDataRow["clinker_ClinkerOutput"] : 0.00m;
            m_GroupDataRow["cementPreparation_ElectricityConsumption"] = (decimal)m_GroupDataRow["cement_CementOutput"] > 0 ? (decimal)m_GroupDataRow["cementPreparation_ElectricityQuantity"] / (decimal)m_GroupDataRow["cement_CementOutput"] : 0.00m;

            m_GlobalCompleteTable.Rows.InsertAt(m_GroupDataRow, 0);
            return m_GlobalCompleteTable;

        }
        private static void GetGlobalCompleteRowData(ref DataRow myNewDataRow, string myOrganizationId, DataTable myResultTable)
        {
            if (myResultTable != null)
            {
                myNewDataRow["Company_ElectricityQuantity"] = 0.0m;
                for (int i = 0; i < myResultTable.Rows.Count; i++)    //找熟料和水泥的日统计
                {
                    if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "clinker_ElectricityQuantity")
                    { //找熟料产线统计,并存入用电总量中
                        myNewDataRow["Company_ElectricityQuantity"] = (decimal)myResultTable.Rows[i]["value"] + (decimal)myNewDataRow["Company_ElectricityQuantity"];
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "cementmill_ElectricityQuantity")
                    { //找水泥磨产线统计,并存入用电总量中
                        myNewDataRow["Company_ElectricityQuantity"] = (decimal)myResultTable.Rows[i]["value"] + (decimal)myNewDataRow["Company_ElectricityQuantity"]; ;
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "clinkerElectricityGeneration_ElectricityQuantity")
                    { //找余热发电电量统计
                        myNewDataRow["clinkerElectricityGeneration_ElectricityQuantity"] = (decimal)myResultTable.Rows[i]["value"];
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "auxiliaryProduction_ElectricityQuantity")
                    { //找辅助电量统计
                        myNewDataRow["auxiliaryProduction_ElectricityQuantity"] = (decimal)myResultTable.Rows[i]["value"];
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "cementPacking_ElectricityQuantity")
                    { //找水泥包装统计
                        myNewDataRow["cementPacking_ElectricityQuantity"] = (decimal)myResultTable.Rows[i]["value"];
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "rawMaterialsPreparation_ElectricityQuantity")
                    { //找生料制备统计
                        myNewDataRow["rawMaterialsPreparation_ElectricityQuantity"] = (decimal)myResultTable.Rows[i]["value"];
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "clinkerPreparation_ElectricityQuantity")
                    { //找熟料制备电量统计
                        myNewDataRow["clinkerPreparation_ElectricityQuantity"] = myResultTable.Rows[i]["value"];
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "cementPreparation_ElectricityQuantity")
                    { //找水泥制备制备统计
                        myNewDataRow["cementPreparation_ElectricityQuantity"] = (decimal)myResultTable.Rows[i]["value"];
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "clinker_MixtureMaterialsOutput")
                    { //找生料产量
                        myNewDataRow["clinker_MixtureMaterialsOutput"] = (decimal)myResultTable.Rows[i]["value"];
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "clinker_ClinkerOutput")
                    { //找熟料产量
                        myNewDataRow["clinker_ClinkerOutput"] = myResultTable.Rows[i]["value"];
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "cement_CementOutput")
                    { //找水泥产量
                        myNewDataRow["cement_CementOutput"] = (decimal)myResultTable.Rows[i]["value"];
                    }

                }
                //////////////////////分步均摊辅助电量//////////////////////
                decimal m_auxiliaryProduction_ElectricityQuantity = myNewDataRow["rawMaterialsPreparation_ElectricityQuantity"] != DBNull.Value ? (decimal)myNewDataRow["auxiliaryProduction_ElectricityQuantity"] : 0.00m;
                decimal m_rawMaterialsPreparation_ElectricityQuantity = myNewDataRow["rawMaterialsPreparation_ElectricityQuantity"] != DBNull.Value ? (decimal)myNewDataRow["rawMaterialsPreparation_ElectricityQuantity"] : 0.00m;
                decimal m_clinkerPreparation_ElectricityQuantity = myNewDataRow["clinkerPreparation_ElectricityQuantity"] != DBNull.Value ? (decimal)myNewDataRow["clinkerPreparation_ElectricityQuantity"] : 0.00m;
                decimal m_cementPreparation_ElectricityQuantity = myNewDataRow["cementPreparation_ElectricityQuantity"] != DBNull.Value ? (decimal)myNewDataRow["cementPreparation_ElectricityQuantity"] : 0.00m;
                decimal m_StepElectricictyQuantityTotal = m_rawMaterialsPreparation_ElectricityQuantity + m_clinkerPreparation_ElectricityQuantity + m_cementPreparation_ElectricityQuantity;
                if (m_StepElectricictyQuantityTotal > 0)
                {
                    m_rawMaterialsPreparation_ElectricityQuantity = m_rawMaterialsPreparation_ElectricityQuantity + m_auxiliaryProduction_ElectricityQuantity * m_rawMaterialsPreparation_ElectricityQuantity / m_StepElectricictyQuantityTotal;
                    if (myNewDataRow["rawMaterialsPreparation_ElectricityQuantity"] != DBNull.Value)
                    {
                        myNewDataRow["rawMaterialsPreparation_ElectricityQuantity"] = m_rawMaterialsPreparation_ElectricityQuantity;
                    }
                    m_clinkerPreparation_ElectricityQuantity = m_clinkerPreparation_ElectricityQuantity + m_auxiliaryProduction_ElectricityQuantity * m_clinkerPreparation_ElectricityQuantity / m_StepElectricictyQuantityTotal;
                    if (myNewDataRow["clinkerPreparation_ElectricityQuantity"] != DBNull.Value)
                    {
                        myNewDataRow["clinkerPreparation_ElectricityQuantity"] = m_clinkerPreparation_ElectricityQuantity;
                    }
                    m_cementPreparation_ElectricityQuantity = m_cementPreparation_ElectricityQuantity + m_auxiliaryProduction_ElectricityQuantity * m_cementPreparation_ElectricityQuantity / m_StepElectricictyQuantityTotal;
                    if (myNewDataRow["cementPreparation_ElectricityQuantity"] != DBNull.Value)
                    {
                        myNewDataRow["cementPreparation_ElectricityQuantity"] = m_cementPreparation_ElectricityQuantity;
                    }
                }
                ///////////////////计算分步电耗////////////////////////
                decimal m_clinker_MixtureMaterialsOutput = myNewDataRow["clinker_MixtureMaterialsOutput"] != DBNull.Value ? (decimal)myNewDataRow["clinker_MixtureMaterialsOutput"] : 0.00m;
                decimal m_clinker_ClinkerOutput = myNewDataRow["clinker_ClinkerOutput"] != DBNull.Value ? (decimal)myNewDataRow["clinker_ClinkerOutput"] : 0.00m;
                decimal m_cement_CementOutput = myNewDataRow["cement_CementOutput"] != DBNull.Value ? (decimal)myNewDataRow["cement_CementOutput"] : 0.00m;
                if (myNewDataRow["rawMaterialsPreparation_ElectricityQuantity"] != DBNull.Value)
                {
                    myNewDataRow["rawMaterialsPreparation_ElectricityConsumption"] = m_clinker_MixtureMaterialsOutput > 0 ? m_rawMaterialsPreparation_ElectricityQuantity / m_clinker_MixtureMaterialsOutput : 0.00m;
                }
                if (myNewDataRow["clinkerPreparation_ElectricityQuantity"] != DBNull.Value)
                {
                    myNewDataRow["clinkerPreparation_ElectricityConsumption"] = m_clinker_ClinkerOutput > 0 ? m_clinkerPreparation_ElectricityQuantity / m_clinker_ClinkerOutput : 0.00m;
                }
                if (myNewDataRow["cementPreparation_ElectricityQuantity"] != DBNull.Value)
                {
                    myNewDataRow["cementPreparation_ElectricityConsumption"] = m_cement_CementOutput > 0 ? m_cementPreparation_ElectricityQuantity / m_cement_CementOutput : 0.00m;
                }
            }
        }
        #endregion GetGlobalComplete

        ////////////////////////////////////////计算工序电耗////////////////////////////////////
        #region GetCompanyProcessComplete
        /// <summary>
        /// ////////////////
        /// </summary>
        /// <param name="myDate"></param>
        /// <param name="myLevelCode"></param>
        /// <param name="myLevelCodes"></param>
        /// <returns></returns>

        public static string GetCompanyProcessComplete(DateTime myDate, string myLevelCode, string[] myLevelCodes)
        {
            string m_ReturnJsonString = "{\"Process\":{\"rows\":[],\"total\":0},\"Comprehensive\":{\"rows\":[],\"total\":0}}";
            string m_Sql = @"select M.OrganizationID, M.LevelCode, M.Type, M.VariableId, M.Name, M.FormulaLevelCode, M.value as Value_Month, N.value as Value_Day from
			                    (select A.OrganizationID, A.LevelCode, E.VariableId, C.Name, A.Type, C.LevelCode as FormulaLevelCode, sum(E.TotalPeakValleyFlatB) as value
			                        from system_Organization A, tz_Formula B, formula_FormulaDetail C, tz_Balance D, balance_Energy E
					                    where (B.Type = 2 or B.Type = 1) and B.ENABLE = 1 and B.State = 0
                                        and A.LevelCode like '{1}%'
					                    {0}
					                    and A.OrganizationID = B.OrganizationID
					                    and C.Visible = 1
					                    and B.keyId = C.KeyId
					                    and ((C.LevelType = 'Process' and len(C.LevelCode) >= 5) or (len(C.LevelCode) = 3 and C.VariableId='cementPacking' and C.LevelType = 'CementCommon'))
					                    and D.BalanceId=E.KeyId 
					                    and D.TimeStamp >= '{3}' and D.TimeStamp <= '{2}'
					                    and D.StaticsCycle = 'day'
					                    and B.OrganizationID = E.OrganizationID
					                    and C.VariableId + '_ElectricityQuantity' = E.VariableId
				                    group by A.LevelCode, A.OrganizationID, A.Type, E.VariableId, C.Name, C.LevelCode
			                    union all
			                    select A.OrganizationID, A.LevelCode, C.VariableId, '' as Name, A.Type, '' as FormulaLevelCode, sum(C.TotalPeakValleyFlatB) as value
				                    from system_Organization A, tz_Balance B, balance_Energy C
					                    where B.BalanceId=C.KeyId 
                                        and A.LevelCode like '{1}%'
					                    {0}
					                    and B.TimeStamp >= '{3}' and B.TimeStamp <= '{2}'
					                    and B.StaticsCycle = 'day'
					                    and A.OrganizationID = C.OrganizationID
					                    and C.ValueType = 'MaterialWeight'
					                    and len(A.LevelCode) = 7
				                    group by A.LevelCode, A.OrganizationID, A.Type, C.VariableId) M
			                    left join 
			                    (select A.OrganizationID, A.LevelCode, E.VariableId, C.Name, C.LevelCode as FormulaLevelCode, sum(E.TotalPeakValleyFlatB) as value
			                        from system_Organization A, tz_Formula B, formula_FormulaDetail C, tz_Balance D, balance_Energy E
					                    where (B.Type = 2 or B.Type = 1) and B.ENABLE = 1 and B.State = 0
                                        and A.LevelCode like '{1}%'
					                    {0}
					                    and A.OrganizationID = B.OrganizationID
					                    and C.Visible = 1
					                    and B.keyId = C.KeyId
					                    and ((C.LevelType = 'Process' and len(C.LevelCode) >= 5) or (len(C.LevelCode) = 3 and C.VariableId='cementPacking' and C.LevelType = 'CementCommon'))
					                    and D.BalanceId=E.KeyId 
					                    and D.TimeStamp = '{2}'
					                    and D.StaticsCycle = 'day'
					                    and B.OrganizationID = E.OrganizationID
					                    and C.VariableId + '_ElectricityQuantity' = E.VariableId
				                    group by A.LevelCode, A.OrganizationID, E.VariableId, C.Name, C.LevelCode
			                    union all
			                    select A.OrganizationID, A.LevelCode, C.VariableId, '' as Name, '' as FormulaLevelCode, sum(C.TotalPeakValleyFlatB) as value
				                    from system_Organization A, tz_Balance B, balance_Energy C
					                    where B.BalanceId=C.KeyId 
                                        and A.LevelCode like '{1}%'
					                    {0}
					                    and B.TimeStamp = '{2}'
					                    and B.StaticsCycle = 'day'
					                    and A.OrganizationID = C.OrganizationID
					                    and C.ValueType = 'MaterialWeight'
					                    and len(A.LevelCode) = 7
				                    group by A.LevelCode, A.OrganizationID, C.VariableId) N on M.LevelCode = N.LevelCode and M.VariableId = N.VariableId
			                    order by M.LevelCode, M.FormulaLevelCode";
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
            m_Sql = string.Format(m_Sql, m_levelCodesParameter, myLevelCode, myDate.ToString("yyyy-MM-dd"), myDate.ToString("yyyy-MM-01"));    //, "replace(M.Name,'公司','')"
            try
            {
                DataTable m_Result = _dataFactory.Query(m_Sql);
                DataTable m_EnergyTemplateTable = GetEnergyTemplateTable(m_levelCodesParameter, myLevelCode, myDate.ToString("yyyy"), Weekly[myDate.Month - 1]);
                if (m_Result != null && m_EnergyTemplateTable != null)
                {
                    DataRow[] m_ProcessRows = m_Result.Select("len(FormulaLevelCode) = 7 or FormulaLevelCode = ''");
                    DataTable m_EnergyProcessTable = m_Result.Clone();
                    for (int i = 0; i < m_ProcessRows.Length; i++)
                    {
                        m_EnergyProcessTable.Rows.Add(m_ProcessRows[i].ItemArray);
                    }
                    string m_ProcessString = "";
                    string m_ComprehensiveString = "";
                    DataTable m_EnergyConsumptionTable = EnergyConsumption.EnergyConsumptionCalculate.CalculateByOrganizationId(m_EnergyProcessTable, m_EnergyTemplateTable, "ValueFormula", new string[] { "Value_Month", "Value_Day" });
                    if (m_EnergyConsumptionTable != null)
                    {
                        m_EnergyConsumptionTable.Columns.Add("Value_Deviation", typeof(decimal));
                        for (int i = 0; i < m_EnergyConsumptionTable.Rows.Count; i++)
                        {
                            decimal m_Value_MonthTemp = m_EnergyConsumptionTable.Rows[i]["Value_Month"] != DBNull.Value ? (decimal)m_EnergyConsumptionTable.Rows[i]["Value_Month"] : 0.00m;
                            decimal m_Value_PlanTemp = m_EnergyConsumptionTable.Rows[i]["Value_Plan"] != DBNull.Value ? (decimal)m_EnergyConsumptionTable.Rows[i]["Value_Plan"] : 0.00m;
                            m_EnergyConsumptionTable.Rows[i]["Value_Deviation"] = m_Value_MonthTemp - m_Value_PlanTemp;
                        }
                        m_ProcessString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_EnergyConsumptionTable);
                    }
                    m_ComprehensiveString = GetCompanyComprehensive(m_Result, myDate, m_levelCodesParameter, myLevelCode);

                    m_ReturnJsonString = "{\"Process\":" + m_ProcessString + ",\"Comprehensive\":" + m_ComprehensiveString + "}";
                }
            }
            catch
            {

            }

            return m_ReturnJsonString;
        }
        private static DataTable GetEnergyTemplateTable(string mylevelCodesParameter, string myLevelCode, string myYear, string WeeklyName)
        {
            string m_Sql = @"Select Z.Name as Company, M.OrganizationID, replace(replace(replace(replace(replace(replace(OrganizationName,'号',''),'#',''),'窑',''),'熟料',''),'线',''),'水泥磨','') + '#' as Name, M.VariableName as DataItem, M.VariableId, M.ValueFormula, N.value as Value_Plan from 
                                (SELECT A.OrganizationID
                                  ,A.Name as OrganizationName
                                  ,D.VariableId
                                  ,C.Name as VariableName
                                  ,D.ValueFormula
	                              ,C.LevelCode
                                  ,A.Type
                                  FROM system_Organization A, tz_Formula B, formula_FormulaDetail C, balance_Energy_Template D
                                  where B.Type = 2 and B.ENABLE = 1 and B.State = 0 
                                  and A.LevelCode like '{1}%'
                                  {0}
                                  and A.OrganizationID = B.OrganizationID
                                  and C.Visible = 1
                                  and B.keyId = C.KeyId
                                  and C.LevelType = 'Process'
                                  and len(C.LevelCode) = 7
                                  and D.TemplateType = 'Process'
                                  and D.[Enabled] = 1
                                  and C.VariableId + '_ElectricityConsumption' = D.VariableId) M
                                  ,(select A.OrganizationID, A.LevelCode, D.VariableId + '_ElectricityConsumption' as VariableId, D.ValueType, C.{3} as value
			                                    from system_Organization A, tz_Plan B, plan_EnergyConsumptionYearlyPlan C, plan_EnergyConsumptionPlan_Template D
					                                where A.OrganizationID = B.OrganizationID
                                                    and A.LevelCode like '{1}%'
					                                {0}
					                                and B.PlanType = 'Energy'
					                                and B.Statue = 1
					                                and B.Date = '{2}'
					                                and B.KeyId = C.KeyId
					                                and C.QuotasID = D.QuotasID
					                                and B.PlanType = D.PlanType
					                                and D.ValueType = 'ElectricityConsumption') N 
                                    left join system_Organization Z on CHARINDEX(Z.LevelCode, N.LevelCode)>0 and Z.LevelType = 'Company'
                                where M.OrganizationID = N.OrganizationID and M.VariableId = N.VariableId
                                order by Z.LevelCode, M.Type, N.LevelCode, M.LevelCode";
            try
            {
                m_Sql = string.Format(m_Sql, mylevelCodesParameter, myLevelCode, myYear, WeeklyName);
                DataTable m_Result = _dataFactory.Query(m_Sql);
                return m_Result;
            }
            catch
            {
                return null;
            }
        }
        #endregion GetCompanyProcessComplete
        /////////////////////////////////////////////综合电耗GetCompanyComprehensiveComplete//////////////////////////////////////////////
        /////////////调用时通过工序电耗方法调用,利用了工序电耗的数据,以增加数据访问速度///////////////
        #region GetCompanyComprehensiveComplete
        /// <summary>
        /// 
        /// </summary>
        /// <param name="myValue"></param>
        /// <param name="myDate"></param>
        /// <returns></returns>
        private static string GetCompanyComprehensive(DataTable myValue, DateTime myDate, string mylevelCodesParameter, string myLevelCode)
        {
            DataTable m_CompanyComprehensiveTable = GetComprehensiveDataTable();
            DataTable m_CompanyComprehensivePlanTable = GetCompanyComprehensivePlanTable(myDate, mylevelCodesParameter, myLevelCode);
            Dictionary<string, decimal[]> m_ClinkerProcessValue = new Dictionary<string, decimal[]>();
            Dictionary<string, decimal[]> m_CementProcessValue = new Dictionary<string, decimal[]>();
            for (int i = 0; i < myValue.Rows.Count; i++)
            {
                string m_VariableId = myValue.Rows[i]["VariableId"].ToString();
                if (myValue.Rows[i]["Type"].ToString() == "熟料")
                {
                    if (m_ClinkerProcessValue.ContainsKey(m_VariableId))
                    {
                        m_ClinkerProcessValue[m_VariableId][0] = m_ClinkerProcessValue[m_VariableId][0] + (myValue.Rows[i]["Value_Month"] != DBNull.Value ? (decimal)myValue.Rows[i]["Value_Month"] : 0.00m);
                        m_ClinkerProcessValue[m_VariableId][1] = m_ClinkerProcessValue[m_VariableId][1] + (myValue.Rows[i]["Value_Day"] != DBNull.Value ? (decimal)myValue.Rows[i]["Value_Day"] : 0.00m);
                    }
                    else
                    {
                        m_ClinkerProcessValue.Add(m_VariableId, new decimal[] { (myValue.Rows[i]["Value_Month"] != DBNull.Value ? (decimal)myValue.Rows[i]["Value_Month"] : 0.00m)
                                                                      ,(myValue.Rows[i]["Value_Day"] != DBNull.Value ? (decimal)myValue.Rows[i]["Value_Day"] : 0.00m)});
                    }
                }
                if (myValue.Rows[i]["Type"].ToString() == "水泥磨" || myValue.Rows[i]["Type"].ToString() == "分厂")
                {
                    if (m_CementProcessValue.ContainsKey(m_VariableId))
                    {
                        m_CementProcessValue[m_VariableId][0] = m_CementProcessValue[m_VariableId][0] + (myValue.Rows[i]["Value_Month"] != DBNull.Value ? (decimal)myValue.Rows[i]["Value_Month"] : 0.00m);
                        m_CementProcessValue[m_VariableId][1] = m_CementProcessValue[m_VariableId][1] + (myValue.Rows[i]["Value_Day"] != DBNull.Value ? (decimal)myValue.Rows[i]["Value_Day"] : 0.00m);
                    }
                    else
                    {
                        m_CementProcessValue.Add(m_VariableId, new decimal[] { (myValue.Rows[i]["Value_Month"] != DBNull.Value ? (decimal)myValue.Rows[i]["Value_Month"] : 0.00m)
                                                                      ,(myValue.Rows[i]["Value_Day"] != DBNull.Value ? (decimal)myValue.Rows[i]["Value_Day"] : 0.00m)});
                    }
                }
            }
            ////////////////////修正水泥包装变量名称//////////////////////
            if (m_CementProcessValue.ContainsKey("cementPacking_ElectricityQuantity") && !m_CementProcessValue.ContainsKey("cementPacking_ElectricityQuantity_All"))
            {
                m_CementProcessValue.Add("cementPacking_ElectricityQuantity_All", new decimal[] { m_CementProcessValue["cementPacking_ElectricityQuantity"][0], m_CementProcessValue["cementPacking_ElectricityQuantity"][1] });
            }
            if (m_CementProcessValue.ContainsKey("cement_CementOutput") && !m_CementProcessValue.ContainsKey("cement_CementOutput_All"))
            {
                m_CementProcessValue.Add("cement_CementOutput_All", new decimal[] { m_CementProcessValue["cement_CementOutput"][0], m_CementProcessValue["cement_CementOutput"][1] });
            }
            ///////当以集团角度统计,公司间、分厂间倒运视为内倒；当以分公司角度统计，公司间倒运视为外倒，分厂间为内倒
            string m_StatisticalRange = GetClinkerInputStatics(myLevelCode);
            //clinker_ClinkerOutsourcingInput    clinker_ClinkerInput
            if (!m_CementProcessValue.ContainsKey("clinker_ClinkerInput"))
            {
                m_CementProcessValue.Add("clinker_ClinkerInput", new decimal[] { 0.00m, 0.00m });
            }
            if (!m_CementProcessValue.ContainsKey("clinker_ClinkerOutsourcingInput"))
            {
                m_CementProcessValue.Add("clinker_ClinkerOutsourcingInput", new decimal[] { 0.00m, 0.00m });
            }
            if (m_StatisticalRange == "Group")    //集团
            {
                m_CementProcessValue["clinker_ClinkerInput"][0] = m_CementProcessValue["clinker_ClinkerInput"][0] + m_CementProcessValue["clinker_ClinkerFactoryTransportInput"][0] + m_CementProcessValue["clinker_ClinkerCompanyTransportInput"][0];
                m_CementProcessValue["clinker_ClinkerInput"][1] = m_CementProcessValue["clinker_ClinkerInput"][1] + m_CementProcessValue["clinker_ClinkerFactoryTransportInput"][1] + m_CementProcessValue["clinker_ClinkerCompanyTransportInput"][1];
            }
            else if (m_StatisticalRange == "Company")
            {
                m_CementProcessValue["clinker_ClinkerInput"][0] = m_CementProcessValue["clinker_ClinkerInput"][0] + m_CementProcessValue["clinker_ClinkerFactoryTransportInput"][0];
                m_CementProcessValue["clinker_ClinkerInput"][1] = m_CementProcessValue["clinker_ClinkerInput"][1] + m_CementProcessValue["clinker_ClinkerFactoryTransportInput"][1];
                m_CementProcessValue["clinker_ClinkerOutsourcingInput"][0] = m_CementProcessValue["clinker_ClinkerOutsourcingInput"][0] + m_CementProcessValue["clinker_ClinkerCompanyTransportInput"][0];
                m_CementProcessValue["clinker_ClinkerOutsourcingInput"][1] = m_CementProcessValue["clinker_ClinkerOutsourcingInput"][1] + m_CementProcessValue["clinker_ClinkerCompanyTransportInput"][1];
            }
            else
            {
                m_CementProcessValue["clinker_ClinkerOutsourcingInput"][0] = m_CementProcessValue["clinker_ClinkerOutsourcingInput"][0] + m_CementProcessValue["clinker_ClinkerFactoryTransportInput"][0] + m_CementProcessValue["clinker_ClinkerCompanyTransportInput"][0];
                m_CementProcessValue["clinker_ClinkerOutsourcingInput"][1] = m_CementProcessValue["clinker_ClinkerOutsourcingInput"][1] + m_CementProcessValue["clinker_ClinkerFactoryTransportInput"][1] + m_CementProcessValue["clinker_ClinkerCompanyTransportInput"][1];
            }

            Standard_GB16780_2012.Function_EnergyConsumption_V1 m_EnergyConsumption_V1 = new Standard_GB16780_2012.Function_EnergyConsumption_V1();
            Standard_GB16780_2012.Parameters_ComprehensiveData m_ComprehensiveDataParameters = GetComprehensiveDataParameters(myValue, myDate);
            if (m_ComprehensiveDataParameters != null)
            {
                string m_OnlyCementmill = Monitor_OverView.Infrastruture.Configuration.ConnectionStringFactory.OnlyCementmil;
                /////////////////////计算月综合电耗//////////////////////
                DataTable m_ClinkerProcessDataTable_Month = GetProcessDataTable(m_ClinkerProcessValue, 0);
                m_EnergyConsumption_V1.LoadComprehensiveData(m_ClinkerProcessDataTable_Month, m_ComprehensiveDataParameters, "VarialbeId", "Value", "");

                decimal m_ClinkerPowerConsumption_Month = m_EnergyConsumption_V1.GetClinkerPowerConsumption();
                decimal m_ClinkerCoalConsumption_Month = m_EnergyConsumption_V1.GetClinkerCoalConsumption();
                decimal m_ClinkerEnergyConsumption_Month = m_EnergyConsumption_V1.GetClinkerEnergyConsumption(m_ClinkerPowerConsumption_Month, m_ClinkerCoalConsumption_Month);
                
                m_EnergyConsumption_V1.ClearPropertiesList();
                DataTable m_CementProcessDataTable_Month = GetProcessDataTable(m_CementProcessValue, 0);
                if (m_OnlyCementmill.Substring(0, 3) == myLevelCode)
                {
                    decimal m_ClinkerOutsourcing_PowerConsumptionTemp = m_ComprehensiveDataParameters.ClinkerOutsourcing_PowerConsumption;
                    decimal m_ClinkerOutsourcing_CoalConsumptionTemp = m_ComprehensiveDataParameters.ClinkerOutsourcing_CoalConsumption;
                    m_ComprehensiveDataParameters.ClinkerOutsourcing_PowerConsumption = 0.00m;
                    m_ComprehensiveDataParameters.ClinkerOutsourcing_CoalConsumption = 0.00m;
                    m_EnergyConsumption_V1.LoadComprehensiveData(m_CementProcessDataTable_Month, m_ComprehensiveDataParameters, "VarialbeId", "Value", "");
                    m_ComprehensiveDataParameters.ClinkerOutsourcing_PowerConsumption = m_ClinkerOutsourcing_PowerConsumptionTemp;
                    m_ComprehensiveDataParameters.ClinkerOutsourcing_CoalConsumption = m_ClinkerOutsourcing_CoalConsumptionTemp;
                }
                else
                {
                    m_EnergyConsumption_V1.LoadComprehensiveData(m_CementProcessDataTable_Month, m_ComprehensiveDataParameters, "VarialbeId", "Value", "");
                }
                //Standard_GB16780_2012.Model_CaculateValue m_aaaa_Temp = m_EnergyConsumption_V1.GetCementPowerConsumptionWithFormula(m_ClinkerPowerConsumption_Month);

                decimal m_CementPowerConsumption_Month = m_EnergyConsumption_V1.GetCementPowerConsumption(m_ClinkerPowerConsumption_Month);
                decimal m_CementCoalConsumption_Month = m_EnergyConsumption_V1.GetCementCoalConsumption(m_ClinkerCoalConsumption_Month);
                decimal m_CementEnergyConsumption_Month = m_EnergyConsumption_V1.GetCementEnergyConsumption(m_CementPowerConsumption_Month, m_CementCoalConsumption_Month);
                /////////////////////计算日综合电耗//////////////////////
                m_EnergyConsumption_V1.ClearPropertiesList();
                DataTable m_ClinkerProcessDataTable_Day = GetProcessDataTable(m_ClinkerProcessValue, 1);
                m_EnergyConsumption_V1.LoadComprehensiveData(m_ClinkerProcessDataTable_Day, m_ComprehensiveDataParameters, "VarialbeId", "Value", "");

                decimal m_ClinkerPowerConsumption_Day = m_EnergyConsumption_V1.GetClinkerPowerConsumption();
                decimal m_ClinkerCoalConsumption_Day = m_EnergyConsumption_V1.GetClinkerCoalConsumption();
                decimal m_ClinkerEnergyConsumption_Day = m_EnergyConsumption_V1.GetClinkerEnergyConsumption(m_ClinkerPowerConsumption_Day, m_ClinkerCoalConsumption_Day);

                m_EnergyConsumption_V1.ClearPropertiesList();
                DataTable m_CementProcessDataTable_Day = GetProcessDataTable(m_CementProcessValue, 1);
                if (m_OnlyCementmill.Substring(0,3) == myLevelCode)
                {
                    decimal m_ClinkerOutsourcing_PowerConsumptionTemp = m_ComprehensiveDataParameters.ClinkerOutsourcing_PowerConsumption;
                    decimal m_ClinkerOutsourcing_CoalConsumptionTemp = m_ComprehensiveDataParameters.ClinkerOutsourcing_CoalConsumption;
                    m_ComprehensiveDataParameters.ClinkerOutsourcing_PowerConsumption = 0.00m;
                    m_ComprehensiveDataParameters.ClinkerOutsourcing_CoalConsumption = 0.00m;
                    m_EnergyConsumption_V1.LoadComprehensiveData(m_CementProcessDataTable_Day, m_ComprehensiveDataParameters, "VarialbeId", "Value", "");
                    m_ComprehensiveDataParameters.ClinkerOutsourcing_PowerConsumption = m_ClinkerOutsourcing_PowerConsumptionTemp;
                    m_ComprehensiveDataParameters.ClinkerOutsourcing_CoalConsumption = m_ClinkerOutsourcing_CoalConsumptionTemp;
                }
                else
                {
                    m_EnergyConsumption_V1.LoadComprehensiveData(m_CementProcessDataTable_Day, m_ComprehensiveDataParameters, "VarialbeId", "Value", "");
                }
                decimal m_CementPowerConsumption_Day = m_EnergyConsumption_V1.GetCementPowerConsumption(m_ClinkerPowerConsumption_Day);
                decimal m_CementCoalConsumption_Day = m_EnergyConsumption_V1.GetCementCoalConsumption(m_ClinkerCoalConsumption_Day);
                decimal m_CementEnergyConsumption_Day = m_EnergyConsumption_V1.GetCementEnergyConsumption(m_CementPowerConsumption_Day, m_CementCoalConsumption_Day);

                object m_PlanValueTemp = m_CompanyComprehensivePlanTable.Compute("sum(value)", "VariableId = 'clinker_ElectricityConsumption'");
                decimal m_ClinkerPowerConsumption_Plan = m_PlanValueTemp != null && m_PlanValueTemp != DBNull.Value ? (decimal)m_PlanValueTemp : 0.00m;
                m_PlanValueTemp = m_CompanyComprehensivePlanTable.Compute("sum(value)", "VariableId = 'clinker_CoalConsumption'");
                decimal m_ClinkerCoalConsumption_Plan = m_PlanValueTemp != null && m_PlanValueTemp != DBNull.Value ? (decimal)m_PlanValueTemp : 0.00m;
                m_PlanValueTemp = m_CompanyComprehensivePlanTable.Compute("sum(value)", "VariableId = 'clinker_EnergyConsumption'");
                decimal m_ClinkerEnergyConsumption_Plan = m_PlanValueTemp != null && m_PlanValueTemp != DBNull.Value ? (decimal)m_PlanValueTemp : 0.00m;
                m_PlanValueTemp = m_CompanyComprehensivePlanTable.Compute("sum(value)", "VariableId = 'cementmill_ElectricityConsumption'");
                decimal m_CementPowerConsumption_Plan = m_PlanValueTemp != null && m_PlanValueTemp != DBNull.Value ? (decimal)m_PlanValueTemp : 0.00m;
                m_PlanValueTemp = m_CompanyComprehensivePlanTable.Compute("sum(value)", "VariableId = 'cementmill_EnergyConsumption'");
                decimal m_CementEnergyConsumption_Plan = m_PlanValueTemp != null && m_PlanValueTemp != DBNull.Value ? (decimal)m_PlanValueTemp : 0.00m;

                m_CompanyComprehensiveTable.Rows.Add("熟料", "综合电耗", "clinker_ElectricityConsumption", m_ClinkerPowerConsumption_Day, m_ClinkerPowerConsumption_Month, m_ClinkerPowerConsumption_Plan, m_ClinkerPowerConsumption_Month - m_ClinkerPowerConsumption_Plan);
                m_CompanyComprehensiveTable.Rows.Add("熟料", "综合煤耗", "clinker_CoalConsumption", m_ClinkerCoalConsumption_Day, m_ClinkerCoalConsumption_Month, m_ClinkerCoalConsumption_Plan, m_ClinkerCoalConsumption_Month - m_ClinkerCoalConsumption_Plan);
                m_CompanyComprehensiveTable.Rows.Add("熟料", "综合能耗", "clinker_EnergyConsumption", m_ClinkerEnergyConsumption_Day, m_ClinkerEnergyConsumption_Month, m_ClinkerEnergyConsumption_Plan, m_ClinkerEnergyConsumption_Month - m_ClinkerEnergyConsumption_Plan);
                m_CompanyComprehensiveTable.Rows.Add("水泥", "综合电耗", "cementmill_ElectricityConsumption", m_CementPowerConsumption_Day, m_CementPowerConsumption_Month, m_CementPowerConsumption_Plan, m_CementPowerConsumption_Month - m_CementPowerConsumption_Plan);
                m_CompanyComprehensiveTable.Rows.Add("水泥", "综合能耗", "cementmill_EnergyConsumption", m_CementEnergyConsumption_Day, m_CementEnergyConsumption_Month, m_CementEnergyConsumption_Plan, m_CementEnergyConsumption_Month - m_CementEnergyConsumption_Plan);
            }
            return EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_CompanyComprehensiveTable);
        }
        private static string GetClinkerInputStatics(string myLevelCode)
        {
             string m_SqlStatisticalRange = @"select A.LevelType as LevelType from system_Organization A where A.LevelCode = '{0}'";
            m_SqlStatisticalRange = string.Format(m_SqlStatisticalRange, myLevelCode);
            try
            {

                DataTable mDataTable_StatisticalRange = _dataFactory.Query(m_SqlStatisticalRange);
                string m_StatisticalRange = "Group";
                if (mDataTable_StatisticalRange != null && mDataTable_StatisticalRange.Rows.Count > 0)
                {
                    m_StatisticalRange = mDataTable_StatisticalRange.Rows[0]["LevelType"].ToString();
                }
                return m_StatisticalRange;
            }
            catch
            {
                return "";
            }
        }
        private static DataTable GetProcessDataTable(Dictionary<string, decimal[]> myProcessData, int myValueIndex)
        {
            DataTable m_ProcessDataTable = new DataTable();
            m_ProcessDataTable.Columns.Add("VarialbeId", typeof(string));
            m_ProcessDataTable.Columns.Add("Value", typeof(decimal));
            foreach (string myKeyId in myProcessData.Keys)
            {
                m_ProcessDataTable.Rows.Add(myKeyId, myProcessData[myKeyId][myValueIndex]);
            }
            return m_ProcessDataTable;
        }
        private static Standard_GB16780_2012.Parameters_ComprehensiveData GetComprehensiveDataParameters(DataTable myValueTable, DateTime myDate)
        {
            List<string> m_FactoryOrganizationIds = new List<string>();
            List<string> m_OrganizationsTemp = new List<string>();
            for (int i = 0; i < myValueTable.Rows.Count; i++)
            {
                string m_OrganizationID = myValueTable.Rows[i]["OrganizationID"].ToString();
                if (!m_OrganizationsTemp.Contains(m_OrganizationID))
                {
                    m_OrganizationsTemp.Add(m_OrganizationID);
                }
            }
            string m_Sql = @"SELECT distinct A.OrganizationID
                                FROM system_Organization A, system_Organization B
                                where (A.LevelCode like B.LevelCode + '%' or CHARINDEX(A.LevelCode, B.LevelCode) > 0)
                                {0}
                                and A.LevelType = 'Factory'";
            string m_Conditions = "";
            for (int i = 0; i < m_OrganizationsTemp.Count; i++)
            {
                if (i == 0)
                {
                    m_Conditions = "'" + m_OrganizationsTemp[i] + "'";
                }
                else
                {
                    m_Conditions = m_Conditions + ",'" + m_OrganizationsTemp[i] + "'";
                }
            }
            if (m_Conditions == "")
            {
                m_Conditions = "and A.OrganizationID <> A.OrganizationID";
            }
            else
            {
                m_Conditions = " and B.OrganizationID in (" + m_Conditions + ")";
            }
            try
            {
                m_Sql = string.Format(m_Sql, m_Conditions);
                DataTable m_Result = _dataFactory.Query(m_Sql);
                if (m_Result != null)
                {
                    for(int i=0;i< m_Result.Rows.Count;i++)
                    {
                        m_FactoryOrganizationIds.Add(m_Result.Rows[i]["OrganizationID"].ToString());
                    }
                }
                Standard_GB16780_2012.Parameters_ComprehensiveData m_Parameters_ComprehensiveData = AutoSetParameters.AutoSetParameters_V1.SetComprehensiveParametersFromSql("day", myDate.AddMonths(-1).ToString("yyyy-MM-dd HH:mm:ss"), myDate.ToString("yyyy-MM-dd HH:mm:ss"), m_FactoryOrganizationIds, _dataFactory);
                return m_Parameters_ComprehensiveData;
            }
            catch
            {
                return null;
            }
        }
        private static DataTable GetComprehensiveDataTable()
        {
            DataTable m_ComprehensiveDataTable = new DataTable();
            m_ComprehensiveDataTable.Columns.Add("Name", typeof(string));
            m_ComprehensiveDataTable.Columns.Add("DataItem", typeof(string));
            m_ComprehensiveDataTable.Columns.Add("VariableId", typeof(string));
            m_ComprehensiveDataTable.Columns.Add("Value_Day", typeof(decimal));
            m_ComprehensiveDataTable.Columns.Add("Value_Month", typeof(decimal));
            m_ComprehensiveDataTable.Columns.Add("Value_Plan", typeof(decimal));
            m_ComprehensiveDataTable.Columns.Add("Value_Deviation", typeof(decimal));
            return m_ComprehensiveDataTable;
        }
        private static DataTable GetCompanyComprehensivePlanTable(DateTime myDate, string mylevelCodesParameter, string myLevelCode)
        {
            string m_Sql = @"select M.VariableId, case when sum(N.value) is not null and sum(N.value) <> 0 then sum(M.value * N.value) / sum(N.value) else 0.00 end as value from 
                                (select A.OrganizationID, A.LevelCode, D.VariableId + '_' + ValueType as VariableId, C.{3} as value, D.Denominator
	                                from system_Organization A, tz_Plan B, plan_EnergyConsumptionYearlyPlan C, plan_EnergyConsumptionPlan_Template D
		                                where A.OrganizationID = B.OrganizationID
                                        and A.LevelCode like '{1}%'
		                                {0}
		                                and B.PlanType = 'Energy'
		                                and B.Statue = 1
		                                and B.Date = '{2}'
		                                and B.KeyId = C.KeyId
		                                and C.QuotasID = D.QuotasID
		                                and B.PlanType = D.PlanType
		                                and D.CaculateType = 'Comprehensive') M,
                                (select A.OrganizationID, A.LevelCode, D.VariableId as VariableId, C.{3} as value
	                                from system_Organization A, tz_Plan B, plan_EnergyConsumptionYearlyPlan C, plan_EnergyConsumptionPlan_Template D
		                                where A.OrganizationID = B.OrganizationID
                                        and A.LevelCode like '{1}%'
		                                {0}
		                                and B.PlanType = 'Energy'
		                                and B.Statue = 1
		                                and B.Date = '{2}'
		                                and B.KeyId = C.KeyId
		                                and C.QuotasID = D.QuotasID
		                                and B.PlanType = D.PlanType
		                                and D.ValueType = 'MaterialWeight') N
                                where M.OrganizationID = N.OrganizationID
                                and M.Denominator = N.VariableId
                                group by M.VariableId";
            try
            {
                m_Sql = string.Format(m_Sql, mylevelCodesParameter, myLevelCode, myDate.ToString("yyyy"), Weekly[myDate.Month - 1]);
                DataTable m_Result = _dataFactory.Query(m_Sql);
                return m_Result;
            }
            catch
            {
                return null;
            }
        }
        #endregion GetCompanyComprehensiveComplete
        ////////////////////////////////////////柱状图显示/////////////////////////////////
        #region GetPlanAndCompleteChart
        /// <summary>
        /// //////
        /// </summary>
        /// <returns></returns>
        public static string GetPlanItems()
        {
            string m_Sql = @"Select QuotasID as QuotasId
                                  ,QuotasName
                                  ,ProductionLineType
                                  ,VariableId
                                  ,ValueType
                                  ,CaculateType
                                  ,Denominator
                              from plan_EnergyConsumptionPlan_Template
                              order by ProductionLineType, DisplayIndex";
            try
            {
                DataTable m_Result = _dataFactory.Query(m_Sql);
                string m_ReturnValue = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_Result);
                return m_ReturnValue;
            }
            catch
            {
                return "{\"rows\":[],\"total\":0}";
            }
        }
        public static string GetSpecificationsInfo(string myVariableId)
        {

            string m_Sql = @"SELECT distinct A.Specifications as id, 
                                  A.Specifications as text
                                  FROM equipment_EquipmentDetail A
                                  where A.Enabled = 1
                                  and A.Specifications is not null
                                  and A.Specifications <> ''
                                  and A.VariableId = '{0}'";
            m_Sql = string.Format(m_Sql, myVariableId);
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
        public static string GetQuntityPlanAndComplete(DateTime myDate, string myVariableId, string myValueType, string mySpecifications, string[] myOganizationIds)
        {
            DataTable m_CompleteTable = GetNormalQuantityCompleteData(myDate, myVariableId + "_" + myValueType, myValueType, myOganizationIds);
            DataTable m_PlanTable = GetQuantityPlanData(myDate, myVariableId, myValueType, myOganizationIds, "Normal");
            DataTable m_SpecificationsTable = GetSpecificationsViariable(mySpecifications, myVariableId, myOganizationIds);
            string m_ReturnValue = GetChartString(m_CompleteTable, m_PlanTable, m_SpecificationsTable, myValueType);
            return m_ReturnValue;
        }
        public static string GetWeightPlanAndComplete(DateTime myDate, string myVariableId, string myValueType, string mySpecifications, string[] myOganizationIds)
        {
            DataTable m_CompleteTable = GetNormalQuantityCompleteData(myDate, myVariableId, myValueType, myOganizationIds);
            DataTable m_PlanTable = GetQuantityPlanData(myDate, myVariableId, myValueType, myOganizationIds, "Normal");
            DataTable m_SpecificationsTable = GetSpecificationsViariable(mySpecifications, myVariableId, myOganizationIds);
            string m_ReturnValue = GetChartString(m_CompleteTable, m_PlanTable, m_SpecificationsTable, myValueType);
            return m_ReturnValue;
        }
        public static string GetElectricityConsumptionPlanAndComplete(DateTime myDate, string myVariableId, string myValueType, string myDenominator, string mySpecifications, string[] myOganizationIds)
        {
            DataTable m_CompleteTable = GetNormalConsumptionCompleteData(myDate, myVariableId + "_" + myValueType, myValueType, myDenominator, myOganizationIds);
            DataTable m_PlanTable = GetConsumptionPlanData(myDate, myVariableId, myValueType, myDenominator, myOganizationIds, "Normal");
            DataTable m_SpecificationsTable = GetSpecificationsViariable(mySpecifications, myVariableId, myOganizationIds);
            string m_ReturnValue = GetChartString(m_CompleteTable, m_PlanTable, m_SpecificationsTable, myValueType);
            return m_ReturnValue;
        }
        public static string GetComprehensivePlanAndComplete(DateTime myDate, string myVariableId, string myProductionLineType, string myValueType, string myDenominator, string mySpecifications, string[] myOganizationIds)
        {
            DataTable m_CompleteTable = GetComprehensiveCompleteData(myDate, myVariableId, myProductionLineType, myValueType, myOganizationIds);
            DataTable m_PlanTable = GetConsumptionPlanData(myDate, myVariableId, myValueType, myDenominator, myOganizationIds, "Comprehensive");
            DataTable m_SpecificationsTable = GetSpecificationsViariable(mySpecifications, myVariableId, myOganizationIds);
            string m_ReturnValue = GetChartString(m_CompleteTable, m_PlanTable, m_SpecificationsTable, myValueType);
            return m_ReturnValue;
        }
        private static DataTable GetSpecificationsViariable(string mySpecifications, string myVariableId, string[] myOganizationIds)
        {
            if (mySpecifications == "All")
            {
                return null;
            }
            else
            {
                string m_Sql = @"SELECT  A.VariableId
                                      ,A.ProductionLineId as OrganizationID
                                  FROM equipment_EquipmentDetail A, system_Organization B, system_Organization C
                                  where A.VariableId = '{0}'
                                  and A.Specifications = '{1}'
                                  and A.Enabled = 1
                                  {2}
                                  and B.LevelCode like C.LevelCode + '%'
                                  and B.LevelType ='ProductionLine'
                                  and A.ProductionLineId = B.OrganizationID";
                string m_OrganizationIds = "";
                string m_OrganizationCondition = "";
                if (myOganizationIds != null)
                {
                    for (int i = 0; i < myOganizationIds.Length; i++)
                    {
                        if (i == 0)
                        {
                            m_OrganizationIds = "'" + myOganizationIds[i] + "'";
                        }
                        else
                        {
                            m_OrganizationIds = m_OrganizationIds + ",'" + myOganizationIds[i] + "'";
                        }
                    }
                    if (m_OrganizationIds != "")
                    {
                        m_OrganizationCondition = string.Format(" and C.OrganizationID in ({0}) ", m_OrganizationIds);
                    }
                    else
                    {
                        m_OrganizationCondition = "and C.OrganizationID <> C.OrganizationID ";
                    }
                }
                else
                {
                    m_OrganizationCondition = "and C.OrganizationID <> C.OrganizationID ";
                }
                m_Sql = string.Format(m_Sql, myVariableId, mySpecifications, m_OrganizationCondition);
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
        }
        private static DataTable GetLevelCodeByOrganizationIds(string[] myOrganizationIds, string myProductionLineType)
        {
            string m_Sql = @"Select B.OrganizationID, A.Name + B.Name as Name, B.LevelCode, '' as VariableId, 0 as Value
                                from system_Organization A, system_Organization B
                                where B.LevelCode like A.LevelCode + '%'
                                and B.LevelType = 'ProductionLine'
                                and B.Type = '{1}'
                                {0}
                                order by B.OrganizationID";
            string m_OrganizationIds = "";
            string m_OrganizationCondition = "";
            if (myOrganizationIds != null)
            {
                for (int i = 0; i < myOrganizationIds.Length; i++)
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
                if (m_OrganizationIds != "")
                {
                    m_OrganizationCondition = string.Format(" and A.OrganizationID in ({0}) ", m_OrganizationIds);
                }
                else
                {
                    m_OrganizationCondition = "and A.OrganizationID <> A.OrganizationID ";
                }
            }
            else
            {
                m_OrganizationCondition = "and A.OrganizationID <> A.OrganizationID ";
            }
            m_Sql = string.Format(m_Sql, m_OrganizationCondition, myProductionLineType);
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
        /// <summary>
        /// 获取电量、发电量实绩数据
        /// </summary>
        /// <param name="myDate">时间</param>
        /// <param name="myVariableId">变量</param>
        /// <param name="myValueType">数据类型(电量)</param>
        /// <param name="myOganizationIds">授权的组织机构</param>
        /// <returns>数据表</returns>
        private static DataTable GetNormalQuantityCompleteData(DateTime myDate, string myVariableId, string myValueType, string[] myOganizationIds)
        {
            string m_Sql = @"select D.OrganizationID, C.Name + D.Name as Name, B.VariableId, sum(B.TotalPeakValleyFlat) as Value
                                from tz_Balance A, balance_Energy B, system_Organization C, system_Organization D
                                where C.LevelType = 'Company'
                                {4}
                                and D.LevelCode like C.LevelCode + '%'
                                and D.LevelType = 'ProductionLine'
                                and A.TimeStamp >= '{2}'
                                and A.TimeStamp <= '{3}'
                                and A.StaticsCycle ='day'
                                and A.BalanceId = B.KeyId
                                and B.OrganizationID = D.OrganizationID
                                and B.VariableId = '{0}'
                                and B.ValueType = '{1}'
                                and D.Type <> '余热发电'
                                group by D.OrganizationID, C.Name + D.Name, B.VariableId
                                order by D.OrganizationID";
            //and ((replace('{0}', '_{1}','') = 'clinkerElectricityGeneration' and D.Type = '余热发电') or (replace('{0}', '_{1}','') <> 'clinkerElectricityGeneration'))
            string m_OrganizationIds = "";
            string m_OrganizationCondition = "";
            if(myOganizationIds != null)
            {
                for(int i=0;i< myOganizationIds.Length;i++)
                {
                    if( i==0)
                    {
                        m_OrganizationIds = "'" + myOganizationIds[i] + "'";
                    }
                    else
                    {
                        m_OrganizationIds = m_OrganizationIds + ",'" + myOganizationIds[i] + "'";
                    }
                }
                if (m_OrganizationIds != "")
                {
                    m_OrganizationCondition = string.Format(" and C.OrganizationID in ({0}) ", m_OrganizationIds);
                }
                else
                {
                    m_OrganizationCondition = "and C.OrganizationID <> C.OrganizationID ";
                }
            }
            else
            {
                m_OrganizationCondition = "and C.OrganizationID <> C.OrganizationID ";
            }
            m_Sql = string.Format(m_Sql, myVariableId, myValueType, myDate.ToString("yyyy-MM-01"), myDate.ToString("yyyy-MM-dd"), m_OrganizationCondition);
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

        private static DataTable GetNormalConsumptionCompleteData(DateTime myDate, string myVariableId, string myValueType, string myDenominator, string[] myOganizationIds)
        {
            string m_Sql = @"Select M.OrganizationID, M.Name, M.VariableId, (case when sum(N.Value) <> 0 and sum(N.Value) is not null then sum(M.Value * N.Value) / sum(N.Value) end ) as Value from (
                                select D.OrganizationID, C.Name + D.Name as Name, B.VariableId, '{5}' as Denominator, B.TotalPeakValleyFlat as Value
                                from tz_Balance A, balance_Energy B, system_Organization C, system_Organization D
                                where C.LevelType = 'Company'
                                {4}
                                and D.LevelCode like C.LevelCode + '%'
                                and D.LevelType = 'ProductionLine'
                                and A.TimeStamp >= '{2}'
                                and A.TimeStamp <= '{3}'
                                and A.StaticsCycle ='day'
                                and A.BalanceId = B.KeyId
                                and B.OrganizationID = D.OrganizationID
                                and B.VariableId = '{0}'
                                and B.ValueType = '{1}') M,
	                       (select D.OrganizationID, C.Name + D.Name as Name, B.VariableId, B.TotalPeakValleyFlat as Value
                                from tz_Balance A, balance_Energy B, system_Organization C, system_Organization D
                                where C.LevelType = 'Company'
                                {4}
                                and D.LevelCode like C.LevelCode + '%'
                                and D.LevelType = 'ProductionLine'
                                and A.TimeStamp >= '{2}'
                                and A.TimeStamp <= '{3}'
                                and A.StaticsCycle ='day'
                                and A.BalanceId = B.KeyId
                                and B.OrganizationID = D.OrganizationID
                                and B.VariableId = '{5}'
                                and B.ValueType = 'MaterialWeight') N
								where M.OrganizationID = N.OrganizationID
                                and M.Denominator = N.VariableId
                                group by M.OrganizationID, M.Name, M.VariableId
                                order by M.OrganizationID";
            string m_OrganizationIds = "";
            string m_OrganizationCondition = "";
            if (myOganizationIds != null)
            {
                for (int i = 0; i < myOganizationIds.Length; i++)
                {
                    if (i == 0)
                    {
                        m_OrganizationIds = "'" + myOganizationIds[i] + "'";
                    }
                    else
                    {
                        m_OrganizationIds = m_OrganizationIds + ",'" + myOganizationIds[i] + "'";
                    }
                }
                if (m_OrganizationIds != "")
                {
                    m_OrganizationCondition = string.Format(" and C.OrganizationID in ({0}) ", m_OrganizationIds);
                }
                else
                {
                    m_OrganizationCondition = "and C.OrganizationID <> C.OrganizationID ";
                }
            }
            else
            {
                m_OrganizationCondition = "and C.OrganizationID <> C.OrganizationID ";
            }
            m_Sql = string.Format(m_Sql, myVariableId, myValueType, myDate.ToString("yyyy-MM-01"), myDate.ToString("yyyy-MM-dd"), m_OrganizationCondition, myDenominator);
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
        private static DataTable GetComprehensiveCompleteData(DateTime myDate, string myVariableId, string myProductionLineType, string myValueType, string[] myOganizationIds)
        {
            DataTable m_CompleteTable = GetLevelCodeByOrganizationIds(myOganizationIds, myProductionLineType);
            if (m_CompleteTable != null)
            {
                decimal ComprehensiveConsumption = 0.0m;
                for (int i = 0; i < m_CompleteTable.Rows.Count; i++)
                {
                    if (myVariableId == "clinker" && myValueType == "ElectricityConsumption")
                    {
                        ComprehensiveConsumption = (AutoGetEnergyConsumption_V1.GetClinkerPowerConsumptionWithFormula("day", myDate.ToString("yyyy-MM-01"), myDate.ToString("yyyy-MM-dd"), m_CompleteTable.Rows[i]["LevelCode"].ToString())).CaculateValue;
                    }
                    else if (myVariableId == "clinker" && myValueType == "CoalConsumption")
                    {
                        ComprehensiveConsumption = (AutoGetEnergyConsumption_V1.GetClinkerCoalConsumptionWithFormula("day", myDate.ToString("yyyy-MM-01"), myDate.ToString("yyyy-MM-dd"), m_CompleteTable.Rows[i]["LevelCode"].ToString())).CaculateValue;
                    }
                    else if (myVariableId == "clinker" && myValueType == "EnergyConsumption")
                    {
                        ComprehensiveConsumption = (AutoGetEnergyConsumption_V1.GetClinkerEnergyConsumptionWithFormula("day", myDate.ToString("yyyy-MM-01"), myDate.ToString("yyyy-MM-dd"), m_CompleteTable.Rows[i]["LevelCode"].ToString())).CaculateValue;
                    }
                    else if (myVariableId == "cementmill" && myValueType == "ElectricityConsumption")
                    {
                        ComprehensiveConsumption = (AutoGetEnergyConsumption_V1.GetCementPowerConsumptionWithFormula("day", myDate.ToString("yyyy-MM-01"), myDate.ToString("yyyy-MM-dd"), m_CompleteTable.Rows[i]["LevelCode"].ToString())).CaculateValue;
                    }
                    else if (myVariableId == "cementmill" && myValueType == "EnergyConsumption")
                    {
                        ComprehensiveConsumption = (AutoGetEnergyConsumption_V1.GetCementEnergyConsumptionWithFormula("day", myDate.ToString("yyyy-MM-01"), myDate.ToString("yyyy-MM-dd"), m_CompleteTable.Rows[i]["LevelCode"].ToString())).CaculateValue;
                    }
                    m_CompleteTable.Rows[i]["VariableId"] = myVariableId + "_" + myValueType;
                    m_CompleteTable.Rows[i]["Value"] = ComprehensiveConsumption;
                }
            }
            return m_CompleteTable;
        }

        private static DataTable GetQuantityPlanData(DateTime myDate, string myVariableId, string myValueType, string[] myOganizationIds, string myCaculateType)
        {
            string m_Sql = @"select E.OrganizationID, D.Name + E.Name as Name, C.VariableId, B.QuotasName, sum(B.{4}) as Value
                                from tz_Plan A, plan_EnergyConsumptionYearlyPlan B, plan_EnergyConsumptionPlan_Template C, system_Organization D, system_Organization E
                                where D.LevelType = 'Company'
                                {3}
                                and E.LevelCode like D.LevelCode + '%'
                                and E.LevelType = 'ProductionLine'
                                and A.Date = '{2}'
                                and A.Statue = 1
                                and A.KeyID = B.KeyID
		                        and B.QuotasID = C.QuotasID
                                and A.OrganizationID = E.OrganizationID
                                and C.VariableId = '{0}'
                                and C.ValueType = '{1}'
                                and C.CaculateType = '{5}'
                                and E.Type <> '余热发电'
                                group by E.OrganizationID, D.Name + E.Name, C.VariableId, B.QuotasName
                                order by E.OrganizationID";
            //and (('{0}' = 'clinkerElectricityGeneration' and E.Type = '余热发电') or ('{0}' <> 'clinkerElectricityGeneration'))
            string m_OrganizationIds = "";
            string m_OrganizationCondition = "";
            if (myOganizationIds != null)
            {
                for (int i = 0; i < myOganizationIds.Length; i++)
                {
                    if (i == 0)
                    {
                        m_OrganizationIds = "'" + myOganizationIds[i] + "'";
                    }
                    else
                    {
                        m_OrganizationIds = m_OrganizationIds + ",'" + myOganizationIds[i] + "'";
                    }
                }
                if (m_OrganizationIds != "")
                {
                    m_OrganizationCondition = string.Format(" and D.OrganizationID in ({0}) ", m_OrganizationIds);
                }
                else
                {
                    m_OrganizationCondition = "and D.OrganizationID <> D.OrganizationID ";
                }
            }
            else
            {
                m_OrganizationCondition = "and D.OrganizationID <> D.OrganizationID ";
            }
            m_Sql = string.Format(m_Sql, myVariableId, myValueType, myDate.ToString("yyyy"), m_OrganizationCondition, Weekly[myDate.Month - 1], myCaculateType);
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

        private static DataTable GetConsumptionPlanData(DateTime myDate, string myVariableId, string myValueType, string myDenominator, string[] myOganizationIds, string myCaculateType)
        {
            string m_Sql = @"Select M.OrganizationID, M.Name, M.VariableId, M.QuotasName, (case when sum(N.Value) <> 0  and sum(N.Value) is not null then sum(M.Value * N.Value) / sum(N.Value) else 0 end) as Value from (
                                    select E.OrganizationID, D.Name + E.Name as Name, C.VariableId, B.{5} as Value, C.Denominator, B.QuotasName
                                        from tz_Plan A, plan_EnergyConsumptionYearlyPlan B, plan_EnergyConsumptionPlan_Template C, system_Organization D, system_Organization E
                                        where D.LevelType = 'Company'
                                        {4}
                                        and E.LevelCode like D.LevelCode + '%'
                                        and E.LevelType = 'ProductionLine'
                                        and A.Date = '{3}'
                                        and A.Statue = 1
                                        and A.KeyID = B.KeyID
		                                and B.QuotasID = C.QuotasID
                                        and A.OrganizationID = E.OrganizationID
                                        and C.VariableId = '{0}'
                                        and C.ValueType = '{1}'
                                        and C.CaculateType = '{6}') M,
	                                (select E.OrganizationID, D.Name + E.Name as Name, C.VariableId, B.{5} as Value
                                        from tz_Plan A, plan_EnergyConsumptionYearlyPlan B, plan_EnergyConsumptionPlan_Template C, system_Organization D, system_Organization E
                                        where D.LevelType = 'Company'
                                        {4}
                                        and E.LevelCode like D.LevelCode + '%'
                                        and E.LevelType = 'ProductionLine'
                                        and A.Date = '{3}'
                                        and A.Statue = 1
                                        and A.KeyID = B.KeyID
		                                and B.QuotasID = C.QuotasID
                                        and A.OrganizationID = E.OrganizationID
                                        and C.VariableId = '{2}'
                                        and C.ValueType = 'MaterialWeight'
                                        and C.CaculateType = 'Normal') N
                                where M.OrganizationID = N.OrganizationID
                                and M.Denominator = N.VariableId
                                group by M.OrganizationID, M.Name, M.VariableId, M.QuotasName
                                order by M.OrganizationID";
            string m_OrganizationIds = "";
            string m_OrganizationCondition = "";
            if (myOganizationIds != null)
            {
                for (int i = 0; i < myOganizationIds.Length; i++)
                {
                    if (i == 0)
                    {
                        m_OrganizationIds = "'" + myOganizationIds[i] + "'";
                    }
                    else
                    {
                        m_OrganizationIds = m_OrganizationIds + ",'" + myOganizationIds[i] + "'";
                    }
                }
                if (m_OrganizationIds != "")
                {
                    m_OrganizationCondition = string.Format(" and D.OrganizationID in ({0}) ", m_OrganizationIds);
                }
                else
                {
                    m_OrganizationCondition = "and D.OrganizationID <> D.OrganizationID ";
                }
            }
            else
            {
                m_OrganizationCondition = "and D.OrganizationID <> D.OrganizationID ";
            }
            m_Sql = string.Format(m_Sql, myVariableId, myValueType, myDenominator, myDate.ToString("yyyy"), m_OrganizationCondition, Weekly[myDate.Month - 1], myCaculateType);
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
        private static string GetChartString(DataTable myPerfomance, DataTable myPlanData, DataTable mySpecificationsTable, string myValueType)
        {
            DataTable m_ChartDataTableStruct = new DataTable();
            List<string> m_ColumnNameList = new List<string>();
            string[] m_RowsName = new string[]{"实绩","计划"};
            string m_UnitX = "";  // "单位";
            string m_UnitY = "";
            if (myPerfomance != null && myPlanData != null)
            {
                if (mySpecificationsTable != null)
                {
                    int m_TableRowIndex = 0;
                    bool m_ExistVariableItem = false;
                    while (m_TableRowIndex < myPerfomance.Rows.Count)
                    {
                        m_ExistVariableItem = false;
                        for (int i = 0; i < mySpecificationsTable.Rows.Count; i++)
                        {
                            if (myPerfomance.Rows[m_TableRowIndex]["OrganizationID"].ToString() == mySpecificationsTable.Rows[i]["OrganizationID"].ToString()
                                  && myPerfomance.Rows[m_TableRowIndex]["VariableId"].ToString().Replace("_" + myValueType, "") == mySpecificationsTable.Rows[i]["VariableId"].ToString())
                            {
                                m_ExistVariableItem = true;
                                break;
                            }
                        }
                        if (m_ExistVariableItem == false)
                        {
                            myPerfomance.Rows.RemoveAt(m_TableRowIndex);
                        }
                        else
                        {
                            m_TableRowIndex = m_TableRowIndex + 1;
                        }
                    }

                    m_TableRowIndex = 0;
                    while (m_TableRowIndex < myPlanData.Rows.Count)
                    {
                        m_ExistVariableItem = false;
                        for (int i = 0; i < mySpecificationsTable.Rows.Count; i++)
                        {
                            if (myPlanData.Rows[m_TableRowIndex]["OrganizationID"].ToString() == mySpecificationsTable.Rows[i]["OrganizationID"].ToString()
                                  && myPlanData.Rows[m_TableRowIndex]["VariableId"].ToString() == mySpecificationsTable.Rows[i]["VariableId"].ToString())
                            {
                                m_ExistVariableItem = true;
                                break;
                            }
                        }
                        if (m_ExistVariableItem == false)
                        {
                            myPlanData.Rows.RemoveAt(m_TableRowIndex);
                        }
                        else
                        {
                            m_TableRowIndex = m_TableRowIndex + 1;
                        }
                    }
                }

                if (myPlanData.Rows.Count > 0)
                {
                    m_UnitY = myPlanData.Rows[0]["QuotasName"].ToString();
                }
                for(int i=0;i<myPerfomance.Rows.Count;i++)
                {
                    m_ChartDataTableStruct.Columns.Add(myPerfomance.Rows[i]["OrganizationID"].ToString(), typeof(decimal));
                    m_ColumnNameList.Add(myPerfomance.Rows[i]["Name"].ToString().Replace("公司",""));
                }
                for (int i = 0; i < myPlanData.Rows.Count; i++)
                {
                    string m_ColumnName = myPlanData.Rows[i]["OrganizationID"].ToString();
                    if (!m_ChartDataTableStruct.Columns.Contains(m_ColumnName))
                    {
                        m_ChartDataTableStruct.Columns.Add(m_ColumnName, typeof(decimal));
                        m_ColumnNameList.Add(m_ColumnName);
                    }
                }
                m_ChartDataTableStruct.Rows.Add(m_ChartDataTableStruct.NewRow());
                m_ChartDataTableStruct.Rows.Add(m_ChartDataTableStruct.NewRow());

                for (int i = 0; i < m_ChartDataTableStruct.Columns.Count; i++)
                {
                    string m_ColumnName = m_ChartDataTableStruct.Columns[i].ColumnName;
                    m_ChartDataTableStruct.Rows[0][m_ColumnName] = 0.0m;     //初始化
                    m_ChartDataTableStruct.Rows[1][m_ColumnName] = 0.0m;     //初始化
                    for (int j = 0; j < myPlanData.Rows.Count; j++)
                    {
                        if (myPlanData.Rows[j]["OrganizationID"].ToString() == m_ColumnName)
                        {
                            m_ChartDataTableStruct.Rows[1][m_ColumnName] = myPlanData.Rows[j]["Value"] != DBNull.Value ? myPlanData.Rows[j]["Value"] : 0.0m;
                        }
                    }
                    for (int j = 0; j < myPerfomance.Rows.Count; j++)
                    {
                        if (myPerfomance.Rows[j]["OrganizationID"].ToString() == m_ColumnName)
                        {
                            m_ChartDataTableStruct.Rows[0][m_ColumnName] = myPerfomance.Rows[i]["Value"] != DBNull.Value ? myPerfomance.Rows[j]["Value"] : 0.0m;
                        }
                    }
                }

                //for (int i = 0; i < myPerfomance.Rows.Count; i++)
                //{
                //    for (int j = 0; j < myPlanData.Rows.Count; j++)
                //    {
                //        if (myPlanData.Rows[j]["OrganizationID"].ToString() == myPerfomance.Rows[i]["OrganizationID"].ToString()
                //            && myPerfomance.Rows[i]["VariableId"].ToString().Contains(myPlanData.Rows[j]["VariableId"].ToString()))
                //        {
                //            m_ChartDataTableStruct.Rows[0][myPerfomance.Rows[i]["OrganizationID"].ToString()] = myPerfomance.Rows[i]["Value"];
                //            m_ChartDataTableStruct.Rows[1][myPerfomance.Rows[i]["OrganizationID"].ToString()] = myPlanData.Rows[j]["Value"];
                //        }
                //    }
                //    m_ColumnNameList.Add(myPerfomance.Rows[i]["Name"].ToString());
                //}
            }
            else if (myPerfomance != null)
            {
                for (int i = 0; i < myPerfomance.Rows.Count; i++)
                {
                    m_ChartDataTableStruct.Rows[0][myPerfomance.Rows[i]["OrganizationID"].ToString()] = myPerfomance.Rows[i]["Value"];
                    m_ChartDataTableStruct.Rows[1][myPerfomance.Rows[i]["OrganizationID"].ToString()] = 0.0m;
                    m_ColumnNameList.Add(myPerfomance.Rows[i]["Name"].ToString());
                }
            }
            else if (myPlanData != null)
            {
                for (int i = 0; i < myPlanData.Rows.Count; i++)
                {
                    m_ChartDataTableStruct.Rows[0][myPlanData.Rows[i]["OrganizationID"].ToString()] = 0.0m;
                    m_ChartDataTableStruct.Rows[1][myPlanData.Rows[i]["OrganizationID"].ToString()] = myPlanData.Rows[i]["Value"];
                    m_ColumnNameList.Add(myPlanData.Rows[i]["Name"].ToString());
                }
            }
            string m_ChartData = EasyUIJsonParser.ChartJsonParser.GetGridChartJsonString(m_ChartDataTableStruct, m_ColumnNameList.ToArray(), m_RowsName, m_UnitX, m_UnitY, 1);
            return m_ChartData;
        }
        #endregion GetPlanAndCompleteChart
    }
}
