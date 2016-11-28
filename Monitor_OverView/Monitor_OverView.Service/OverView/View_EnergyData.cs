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
        /// <summary>
        /// 获得左上角的全局数据
        /// </summary>
        /// <param name="myDate"></param>
        /// <param name="myLevelCodes"></param>
        /// <returns></returns>
        public static string GetGlobalComplete(DateTime myDate, string[] myLevelCodes)
        {
            string m_ReturnJsonString = "";
            string m_Sql = @"Select Z.* from (
                            SELECT A.OrganizationID as OrganizationId,A.LevelCode as LevelCode,A.Name as Name, M.VariableId as VariableId, sum(M.TotalPeakValleyFlatB) as value
                                    FROM system_Organization AS A
	                                left join 
	                                (SELECT D.LevelCode as LevelCode,C.VariableId AS VariableId, SUM(C.TotalPeakValleyFlatB) AS TotalPeakValleyFlatB
		                                FROM tz_Balance AS B,balance_Energy AS C, system_Organization D
		                                WHERE B.BalanceId=C.KeyId 
		                                and B.TimeStamp= '{1}'
		                                and B.StaticsCycle = 'day'
		                                and C.VariableId in ('clinkerElectricityGeneration_ElectricityQuantity'
										,'clinkerBurning_ElectricityQuantity'
										,'rawMaterialsPreparation_ElectricityQuantity'
										,'coalPreparation_ElectricityQuantity'
										,'cementPreparation_ElectricityQuantity'
										,'cementmill_ElectricityQuantity'
										,'clinker_ElectricityQuantity')
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
            m_GlobalCompleteTable.Columns.Add("clinkerBurning_ElectricityQuantity", typeof(decimal));
            m_GlobalCompleteTable.Columns.Add("rawMaterialsPreparation_ElectricityQuantity", typeof(decimal));
            m_GlobalCompleteTable.Columns.Add("coalPreparation_ElectricityQuantity", typeof(decimal));
            m_GlobalCompleteTable.Columns.Add("cementPreparation_ElectricityQuantity", typeof(decimal));
            m_GlobalCompleteTable.Columns.Add("cementPacking_ElectricityQuantity", typeof(decimal));

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
            m_GroupDataRow["clinkerBurning_ElectricityQuantity"] = m_GlobalCompleteTable.Compute("sum(clinkerBurning_ElectricityQuantity)", "TRUE");
            m_GroupDataRow["rawMaterialsPreparation_ElectricityQuantity"] = m_GlobalCompleteTable.Compute("sum(rawMaterialsPreparation_ElectricityQuantity)", "TRUE");
            m_GroupDataRow["coalPreparation_ElectricityQuantity"] = m_GlobalCompleteTable.Compute("sum(coalPreparation_ElectricityQuantity)", "TRUE");
            m_GroupDataRow["cementPreparation_ElectricityQuantity"] = m_GlobalCompleteTable.Compute("sum(cementPreparation_ElectricityQuantity)", "TRUE");
            m_GroupDataRow["cementPacking_ElectricityQuantity"] = m_GlobalCompleteTable.Compute("sum(cementPacking_ElectricityQuantity)", "TRUE");

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
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "clinkerBurning_ElectricityQuantity")
                    { //找熟料烧成电量统计
                        myNewDataRow["clinkerBurning_ElectricityQuantity"] = myResultTable.Rows[i]["value"];
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "rawMaterialsPreparation_ElectricityQuantity")
                    { //找生料制备统计
                        myNewDataRow["rawMaterialsPreparation_ElectricityQuantity"] = (decimal)myResultTable.Rows[i]["value"];
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "coalPreparation_ElectricityQuantity")
                    { //找煤粉制备统计
                        myNewDataRow["coalPreparation_ElectricityQuantity"] = (decimal)myResultTable.Rows[i]["value"];
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "cementPreparation_ElectricityQuantity")
                    { //找水泥制备制备统计
                        myNewDataRow["cementPreparation_ElectricityQuantity"] = (decimal)myResultTable.Rows[i]["value"];
                    }
                    else if (myResultTable.Rows[i]["OrganizationID"].ToString() == myOrganizationId && myResultTable.Rows[i]["VariableId"].ToString() == "cementPacking_ElectricityQuantity")
                    { //找水泥包装统计
                        myNewDataRow["cementPacking_ElectricityQuantity"] = (decimal)myResultTable.Rows[i]["value"];
                    }
                }

            }
        }
        public static string GetCompanyComprehensiveComplete(DateTime myDate, string myLevelCode, string[] myLevelCodes)
        {
            string m_Sql = @"Select A.LevelCode 
                                from 
                                system_Organization A 
                                where A.LevelCode like '{0}%' and A.LevelType = '{1}'
                                {2}
                                order by A.LevelCode";
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
            m_Sql = string.Format(m_Sql, myLevelCode, "Company", m_levelCodesParameter);

            try
            {
                DataTable m_Result = _dataFactory.Query(m_Sql);
                List<DataTable> m_ResultTableList = new List<DataTable>();
                if (m_Result != null)
                {
                    for (int i = 0; i < m_Result.Rows.Count; i++)
                    {
                        string m_LevelCode = m_Result.Rows[i]["LevelCode"].ToString();
                        m_ResultTableList.Add(GetCompanyComprehensiveCompleteSingle(myDate, m_LevelCode, myLevelCodes));
                    }
                    DataTable m_ComprehensiveDataTable = null;
                    for (int i = 0; i < m_ResultTableList.Count; i++)
                    {
                        if (i == 0)
                        {
                            m_ComprehensiveDataTable = m_ResultTableList[i].Copy();
                            for (int j = 0; j < m_ComprehensiveDataTable.Rows.Count; j++)
                            {
                                m_ComprehensiveDataTable.Rows[j]["Value_Day"] = (decimal)m_ComprehensiveDataTable.Rows[j]["Value_Day"] * (decimal)m_ComprehensiveDataTable.Rows[j]["Output_Day"];
                                m_ComprehensiveDataTable.Rows[j]["Value_Month"] = (decimal)m_ComprehensiveDataTable.Rows[j]["Value_Month"] * (decimal)m_ComprehensiveDataTable.Rows[j]["Output_Month"];
                                m_ComprehensiveDataTable.Rows[j]["Value_Plan"] = (decimal)m_ComprehensiveDataTable.Rows[j]["Value_Plan"] * (decimal)m_ComprehensiveDataTable.Rows[j]["Output_Plan"];
                            }
                        }
                        else
                        {
                            for (int j = 0; j < m_ComprehensiveDataTable.Rows.Count; j++)
                            {
                                m_ComprehensiveDataTable.Rows[j]["Value_Day"] = (decimal)m_ComprehensiveDataTable.Rows[j]["Value_Day"] + (decimal)m_ResultTableList[i].Rows[j]["Value_Day"] * (decimal)m_ResultTableList[i].Rows[j]["Output_Day"];
                                m_ComprehensiveDataTable.Rows[j]["Value_Month"] = (decimal)m_ComprehensiveDataTable.Rows[j]["Value_Month"] + (decimal)m_ResultTableList[i].Rows[j]["Value_Month"] * (decimal)m_ResultTableList[i].Rows[j]["Output_Month"];
                                m_ComprehensiveDataTable.Rows[j]["Value_Plan"] = (decimal)m_ComprehensiveDataTable.Rows[j]["Value_Plan"] + (decimal)m_ResultTableList[i].Rows[j]["Value_Plan"] * (decimal)m_ResultTableList[i].Rows[j]["Output_Plan"];

                                m_ComprehensiveDataTable.Rows[j]["Output_Day"] = (decimal)m_ComprehensiveDataTable.Rows[j]["Output_Day"] + (decimal)m_ResultTableList[i].Rows[j]["Output_Day"];
                                m_ComprehensiveDataTable.Rows[j]["Output_Month"] = (decimal)m_ComprehensiveDataTable.Rows[j]["Output_Month"] + (decimal)m_ResultTableList[i].Rows[j]["Output_Month"];
                                m_ComprehensiveDataTable.Rows[j]["Output_Plan"] = (decimal)m_ComprehensiveDataTable.Rows[j]["Output_Plan"] + (decimal)m_ResultTableList[i].Rows[j]["Output_Plan"];
                            }
                        }
                    }
                    for (int j = 0; j < m_ComprehensiveDataTable.Rows.Count; j++)
                    {
                        if ((decimal)m_ComprehensiveDataTable.Rows[j]["Output_Day"] != 0)
                        {
                            m_ComprehensiveDataTable.Rows[j]["Value_Day"] = (decimal)m_ComprehensiveDataTable.Rows[j]["Value_Day"] / (decimal)m_ComprehensiveDataTable.Rows[j]["Output_Day"];
                        }
                        else
                        {
                            m_ComprehensiveDataTable.Rows[j]["Value_Day"] = 0.0m;
                        }
                        if ((decimal)m_ComprehensiveDataTable.Rows[j]["Output_Month"] != 0)
                        {
                            m_ComprehensiveDataTable.Rows[j]["Value_Month"] = (decimal)m_ComprehensiveDataTable.Rows[j]["Value_Month"] / (decimal)m_ComprehensiveDataTable.Rows[j]["Output_Month"];
                        }
                        else
                        {
                            m_ComprehensiveDataTable.Rows[j]["Value_Month"] = 0.0m;
                        }
                        if ((decimal)m_ComprehensiveDataTable.Rows[j]["Output_Plan"] != 0)
                        {
                            m_ComprehensiveDataTable.Rows[j]["Value_Plan"] = (decimal)m_ComprehensiveDataTable.Rows[j]["Value_Plan"] / (decimal)m_ComprehensiveDataTable.Rows[j]["Output_Plan"];
                        }
                        else
                        {
                            m_ComprehensiveDataTable.Rows[j]["Value_Plan"] = 0.0m;
                        }
                    }
                    string m_ReturnJsonString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_ComprehensiveDataTable);
                    return m_ReturnJsonString;
                }
                else
                {
                    return "{\"rows\":[],\"total\":0}";
                }
            }
            catch
            {
                return "{\"rows\":[],\"total\":0}";
            }
        }
        /// <summary>
        /// 统计综合电耗
        /// </summary>
        /// <param name="myDate"></param>
        /// <param name="myLevelCode"></param>
        /// <returns></returns>
        private static DataTable GetCompanyComprehensiveCompleteSingle(DateTime myDate, string myLevelCode, string[] myLevelCodes)
        {
            DataTable m_ComprehensiveDataTable = new DataTable();
            m_ComprehensiveDataTable.Columns.Add("OrganizationId", typeof(string));
            m_ComprehensiveDataTable.Columns.Add("ProcessName", typeof(string));
            m_ComprehensiveDataTable.Columns.Add("Name", typeof(string));
            m_ComprehensiveDataTable.Columns.Add("DataItem", typeof(string));
            m_ComprehensiveDataTable.Columns.Add("VariableId", typeof(string));
            m_ComprehensiveDataTable.Columns.Add("Value_Day", typeof(decimal));
            m_ComprehensiveDataTable.Columns.Add("Value_Month", typeof(decimal));
            m_ComprehensiveDataTable.Columns.Add("Value_Plan", typeof(decimal));
            m_ComprehensiveDataTable.Columns.Add("Value_Deviation", typeof(decimal));
            m_ComprehensiveDataTable.Columns.Add("Output_Day", typeof(decimal));
            m_ComprehensiveDataTable.Columns.Add("Output_Month", typeof(decimal));
            m_ComprehensiveDataTable.Columns.Add("Output_Plan", typeof(decimal));

            Standard_GB16780_2012.Model_CaculateValue m_ClinkerPowerConsumptionValue_Day = AutoGetEnergyConsumption_V1.GetClinkerPowerConsumptionWithFormula("day", myDate.ToString("yyyy-MM-dd"), myDate.ToString("yyyy-MM-dd"), myLevelCode);
            Standard_GB16780_2012.Model_CaculateValue m_ClinkerPowerConsumptionValue_Month = AutoGetEnergyConsumption_V1.GetClinkerPowerConsumptionWithFormula("day", myDate.ToString("yyyy-MM-01"), myDate.ToString("yyyy-MM-dd"), myLevelCode);
            decimal ClinkerPowerConsumption_Day = m_ClinkerPowerConsumptionValue_Day.CaculateValue;
            decimal ClinkerPowerOutput_Day = m_ClinkerPowerConsumptionValue_Day.CaculateFactor.Count > 0 ? m_ClinkerPowerConsumptionValue_Day.CaculateFactor[m_ClinkerPowerConsumptionValue_Day.CaculateFactor.Count - 1].FactorValue :0.0m;
            decimal ClinkerPowerConsumption_Month = m_ClinkerPowerConsumptionValue_Month.CaculateValue;
            decimal ClinkerPowerOutput_Month = m_ClinkerPowerConsumptionValue_Month.CaculateFactor.Count > 0 ? m_ClinkerPowerConsumptionValue_Month.CaculateFactor[m_ClinkerPowerConsumptionValue_Month.CaculateFactor.Count - 1].FactorValue : 0.0m;
            decimal ClinkerPowerConsumption_Plan = 0.0m;
            decimal ClinkerPowerOutput_Plan = 0.0m;

            Standard_GB16780_2012.Model_CaculateValue m_ClinkerCoalConsumptionValue_Day = AutoGetEnergyConsumption_V1.GetClinkerCoalConsumptionWithFormula("day", myDate.ToString("yyyy-MM-dd"), myDate.ToString("yyyy-MM-dd"), myLevelCode);
            Standard_GB16780_2012.Model_CaculateValue m_ClinkerCoalConsumptionValue_Month = AutoGetEnergyConsumption_V1.GetClinkerCoalConsumptionWithFormula("day", myDate.ToString("yyyy-MM-01"), myDate.ToString("yyyy-MM-dd"), myLevelCode);
            decimal ClinkerCoalConsumption_Day = m_ClinkerCoalConsumptionValue_Day.CaculateValue;
            decimal ClinkerCoalOutput_Day = m_ClinkerCoalConsumptionValue_Day.CaculateFactor.Count > 0 ? m_ClinkerCoalConsumptionValue_Day.CaculateFactor[m_ClinkerCoalConsumptionValue_Day.CaculateFactor.Count - 1].FactorValue : 0.0m;
            decimal ClinkerCoalConsumption_Month = m_ClinkerCoalConsumptionValue_Month.CaculateValue;
            decimal ClinkerCoalOutput_Month = m_ClinkerCoalConsumptionValue_Month.CaculateFactor.Count > 0 ? m_ClinkerCoalConsumptionValue_Month.CaculateFactor[m_ClinkerCoalConsumptionValue_Month.CaculateFactor.Count - 1].FactorValue : 0.0m;
            decimal ClinkerCoalConsumption_Plan = 0.0m;
            decimal ClinkerCoalOutput_Plan = 0.0m;

            Standard_GB16780_2012.Model_CaculateValue m_ClinkerEnergyConsumptionValue_Day = AutoGetEnergyConsumption_V1.GetClinkerEnergyConsumptionWithFormula("day", myDate.ToString("yyyy-MM-dd"), myDate.ToString("yyyy-MM-dd"), myLevelCode);
            Standard_GB16780_2012.Model_CaculateValue m_ClinkerEnergyConsumptionValue_Month = AutoGetEnergyConsumption_V1.GetClinkerEnergyConsumptionWithFormula("day", myDate.ToString("yyyy-MM-01"), myDate.ToString("yyyy-MM-dd"), myLevelCode);
            decimal ClinkerEnergyConsumption_Day = m_ClinkerEnergyConsumptionValue_Day.CaculateValue;
            decimal ClinkerEnergyOutput_Day = m_ClinkerEnergyConsumptionValue_Day.CaculateFactor.Count > 0 ? m_ClinkerEnergyConsumptionValue_Day.CaculateFactor[m_ClinkerEnergyConsumptionValue_Day.CaculateFactor.Count - 1].FactorValue : 0.0m;
            decimal ClinkerEnergyConsumption_Month = m_ClinkerEnergyConsumptionValue_Month.CaculateValue;
            decimal ClinkerEnergyOutput_Month = m_ClinkerEnergyConsumptionValue_Month.CaculateFactor.Count > 0 ? m_ClinkerEnergyConsumptionValue_Month.CaculateFactor[m_ClinkerEnergyConsumptionValue_Month.CaculateFactor.Count - 1].FactorValue : 0.0m;
            decimal ClinkerEnergyConsumption_Plan = 0.0m;
            decimal ClinkerEnergyOutput_Plan = 0.0m;

            Standard_GB16780_2012.Model_CaculateValue m_CementPowerConsumptionValue_Day = AutoGetEnergyConsumption_V1.GetCementPowerConsumptionWithFormula("day", myDate.ToString("yyyy-MM-dd"), myDate.ToString("yyyy-MM-dd"), myLevelCode);
            Standard_GB16780_2012.Model_CaculateValue m_CementPowerConsumptionValue_Month = AutoGetEnergyConsumption_V1.GetCementPowerConsumptionWithFormula("day", myDate.ToString("yyyy-MM-01"), myDate.ToString("yyyy-MM-dd"), myLevelCode);
            decimal CementPowerConsumption_Day = m_CementPowerConsumptionValue_Day.CaculateValue;
            decimal CementPowerOutput_Day = m_CementPowerConsumptionValue_Day.CaculateFactor.Count > 0 ? m_CementPowerConsumptionValue_Day.CaculateFactor[m_CementPowerConsumptionValue_Day.CaculateFactor.Count - 1].FactorValue : 0.0m;
            decimal CementPowerConsumption_Month = m_CementPowerConsumptionValue_Month.CaculateValue;
            decimal CementPowerOutput_Month = m_CementPowerConsumptionValue_Month.CaculateFactor.Count > 0 ? m_CementPowerConsumptionValue_Month.CaculateFactor[m_CementPowerConsumptionValue_Month.CaculateFactor.Count - 1].FactorValue : 0.0m;
            decimal CementPowerConsumption_Plan = 0.0m;
            decimal CementPowerOutput_Plan = 0.0m;

            Standard_GB16780_2012.Model_CaculateValue m_CementEnergyConsumptionValue_Day = AutoGetEnergyConsumption_V1.GetCementEnergyConsumptionWithFormula("day", myDate.ToString("yyyy-MM-dd"), myDate.ToString("yyyy-MM-dd"), myLevelCode);
            Standard_GB16780_2012.Model_CaculateValue m_CementEnergyConsumptionValue_Month = AutoGetEnergyConsumption_V1.GetCementEnergyConsumptionWithFormula("day", myDate.ToString("yyyy-MM-01"), myDate.ToString("yyyy-MM-dd"), myLevelCode);
            decimal CementEnergyConsumption_Day = m_CementEnergyConsumptionValue_Day.CaculateValue;
            decimal CementEnergyOutput_Day = m_CementEnergyConsumptionValue_Day.CaculateFactor.Count > 0 ? m_CementEnergyConsumptionValue_Day.CaculateFactor[m_CementEnergyConsumptionValue_Day.CaculateFactor.Count - 1].FactorValue : 0.0m;
            decimal CementEnergyConsumption_Month = m_CementEnergyConsumptionValue_Month.CaculateValue;
            decimal CementEnergyOutput_Month = m_CementEnergyConsumptionValue_Month.CaculateFactor.Count > 0 ? m_CementEnergyConsumptionValue_Month.CaculateFactor[m_CementEnergyConsumptionValue_Month.CaculateFactor.Count - 1].FactorValue : 0.0m;
            decimal CementEnergyConsumption_Plan = 0.0m;
            decimal CementEnergyOutput_Plan = 0.0m;


            string m_Sql = @"Select A.OrganizationID, D.Name, D.Type, D.LevelCode, C.VariableId + C.ValueType +  '_Plan' as VariableId, B.{3} as Value from tz_Plan A, plan_EnergyConsumptionYearlyPlan B, plan_EnergyConsumptionPlan_Template C, system_Organization D
                                        where D.LevelCode like '{1}%'
                                        {0}
                                        and D.Type in ('熟料','水泥磨')
                                        and A.OrganizationID = D.OrganizationID
                                        and A.Date = '{2}'
                                        and A.ProductionLineType = D.Type
                                        and A.Statue = 1
                                        and A.KeyID = B.KeyID
                                        and B.QuotasID = C.QuotasID
                                        and C.ValueType in ('ElectricityConsumption', 'CoalConsumption','EnergyConsumption')
                                        and C.CaculateType = 'Comprehensive'
                                        and C.VariableId in ('clinker','cementmill')
										order by D.Type, D.LevelCode, C.VariableId";
            StringBuilder levelCodesParameter = new StringBuilder();
            foreach (var levelCode in myLevelCodes)
            {
                levelCodesParameter.Append("D.LevelCode like ");
                levelCodesParameter.Append("'");
                levelCodesParameter.Append(levelCode + "%");
                levelCodesParameter.Append("'");
                levelCodesParameter.Append(" OR ");
                levelCodesParameter.Append(string.Format("CHARINDEX(D.LevelCode,'{0}')>0", levelCode));
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
                m_levelCodesParameter = " and D.OrganizationID <> D.OrganizationID";
            }

            m_Sql = string.Format(m_Sql, m_levelCodesParameter, myLevelCode, myDate.ToString("yyyy"), Weekly[myDate.Month - 1]);
            try
            {
                DataTable m_Result = _dataFactory.Query(m_Sql);
                if (m_Result != null)
                {
                    for (int i = 0; i < m_Result.Rows.Count; i++)
                    {
                        if (m_Result.Rows[i]["VariableId"].ToString() == "clinkerElectricityConsumption_Plan")
                        {
                            ClinkerPowerConsumption_Plan = (decimal)m_Result.Rows[i]["value"];
                        }
                        else if (m_Result.Rows[i]["VariableId"].ToString() == "clinkerCoalConsumption_Plan")
                        {
                            ClinkerCoalConsumption_Plan = (decimal)m_Result.Rows[i]["value"];
                        }
                        else if (m_Result.Rows[i]["VariableId"].ToString() == "clinkerEnergyConsumption_Plan")
                        {
                            ClinkerEnergyConsumption_Plan = (decimal)m_Result.Rows[i]["value"];
                        }
                        else if (m_Result.Rows[i]["VariableId"].ToString() == "cementmillElectricityConsumption_Plan")
                        {
                            CementPowerConsumption_Plan = (decimal)m_Result.Rows[i]["value"];
                        }
                        else if (m_Result.Rows[i]["VariableId"].ToString() == "cementmillEnergyConsumption_Plan")
                        {
                            CementEnergyConsumption_Plan = (decimal)m_Result.Rows[i]["value"];
                        }
                    }
                }
            }
            catch
            {

            }

            m_ComprehensiveDataTable.Rows.Add("", "", "熟料", "综合电耗", "", ClinkerPowerConsumption_Day, ClinkerPowerConsumption_Month, ClinkerPowerConsumption_Plan, ClinkerPowerConsumption_Month - ClinkerPowerConsumption_Plan, ClinkerPowerOutput_Day, ClinkerPowerOutput_Month, ClinkerPowerOutput_Plan);
            m_ComprehensiveDataTable.Rows.Add("", "", "熟料", "综合煤耗", "", ClinkerCoalConsumption_Day, ClinkerCoalConsumption_Month, ClinkerCoalConsumption_Plan, ClinkerCoalConsumption_Month - ClinkerCoalConsumption_Plan, ClinkerCoalOutput_Day, ClinkerCoalOutput_Month, ClinkerCoalOutput_Plan);
            m_ComprehensiveDataTable.Rows.Add("", "", "熟料", "综合能耗", "", ClinkerEnergyConsumption_Day, ClinkerEnergyConsumption_Month, ClinkerEnergyConsumption_Plan, ClinkerEnergyConsumption_Month - ClinkerEnergyConsumption_Plan, ClinkerEnergyOutput_Day, ClinkerEnergyOutput_Month, ClinkerEnergyOutput_Plan);
            m_ComprehensiveDataTable.Rows.Add("", "", "水泥", "综合电耗", "", CementPowerConsumption_Day, CementPowerConsumption_Month, CementPowerConsumption_Plan, CementPowerConsumption_Month - CementPowerConsumption_Plan, CementPowerOutput_Day, CementPowerOutput_Month, CementPowerOutput_Plan);
            m_ComprehensiveDataTable.Rows.Add("", "", "水泥", "综合能耗", "", CementEnergyConsumption_Day, CementEnergyConsumption_Month, CementEnergyConsumption_Plan, CementEnergyConsumption_Month - CementEnergyConsumption_Plan, CementEnergyOutput_Day, CementEnergyOutput_Month, CementEnergyOutput_Plan);

            return m_ComprehensiveDataTable;
        }

        public static string GetCompanyProcessComplete(DateTime myDate, string myLevelCode, string[] myLevelCodes)
        {
            string m_ReturnJsonString = "";
            string m_Sql = @"Select Z.OrganizationID, {6} + Z.Name as Name, Z.Type, Z.LevelCode,Z.VariableId, Z.value from (
                                   SELECT D.OrganizationID, D.Name, D.Type, D.LevelCode as LevelCode,C.VariableId + '_day' AS VariableId, SUM(C.TotalPeakValleyFlatB) AS value
		                                FROM tz_Balance AS B,balance_Energy AS C, system_Organization D
		                                WHERE B.BalanceId=C.KeyId 
		                                and B.TimeStamp= '{2}'
		                                and B.StaticsCycle = 'day'
		                                and C.VariableId in ('clinkerPreparation_ElectricityQuantity'
										,'rawMaterialsPreparation_ElectricityQuantity'
										,'cementPreparation_ElectricityQuantity'
										,'clinker_MixtureMaterialsOutput'
										,'clinker_ClinkerOutput'
										,'cement_CementOutput')
										and D.LevelType = 'ProductionLine'
                                        and D.Type in ('熟料','水泥磨')
										and D.LevelCode like '{1}%'
                                        {0}
		                                and C.OrganizationID = D.OrganizationID
		                                GROUP BY D.OrganizationID, D.LevelCode, D.Name, D.Type, C.VariableId
								   union all
                                   SELECT D.OrganizationID, D.Name, D.Type, D.LevelCode as LevelCode,C.VariableId + '_month' AS VariableId, SUM(C.TotalPeakValleyFlatB) AS value
		                                FROM tz_Balance AS B,balance_Energy AS C, system_Organization D
		                                WHERE B.BalanceId=C.KeyId 
		                                and B.TimeStamp >= '{3}' and B.TimeStamp <= '{2}'
		                                and B.StaticsCycle = 'day'
		                                and C.VariableId in ('clinkerPreparation_ElectricityQuantity'
										,'rawMaterialsPreparation_ElectricityQuantity'
										,'cementPreparation_ElectricityQuantity'
										,'clinker_MixtureMaterialsOutput'
										,'clinker_ClinkerOutput'
										,'cement_CementOutput')
										and D.LevelType = 'ProductionLine'
                                        and D.Type in ('熟料','水泥磨')
										and D.LevelCode like '{1}%'
                                        {0}
		                                and C.OrganizationID = D.OrganizationID
		                                GROUP BY D.OrganizationID, D.LevelCode, D.Name, D.Type, C.VariableId
                                   union all
                                   Select A.OrganizationID, D.Name, D.Type, D.LevelCode, C.VariableId + C.ValueType +  '_Plan' as VariableId, B.{5} as Value from tz_Plan A, plan_EnergyConsumptionYearlyPlan B, plan_EnergyConsumptionPlan_Template C, system_Organization D
                                        where D.LevelCode like '{1}%'
                                        {0}
                                        and D.Type in ('熟料','水泥磨')
                                        and A.OrganizationID = D.OrganizationID
                                        and A.Date = '{4}'
                                        and A.ProductionLineType = D.Type
                                        and A.Statue = 1
                                        and A.KeyID = B.KeyID
                                        and B.QuotasID = C.QuotasID
                                        and C.ValueType = 'ElectricityConsumption'
                                        and C.CaculateType = 'Normal'
                                        and C.VariableId in ('clinkerPreparation','rawMaterialsPreparation','cementPreparation')) Z, system_Organization W
										where Z.LevelCode like W.LevelCode + '%'
										and W.LevelType = 'Company'
										order by Z.Type, Z.LevelCode, Z.VariableId";
            StringBuilder levelCodesParameter = new StringBuilder();
            foreach (var levelCode in myLevelCodes)
            {
                levelCodesParameter.Append("D.LevelCode like ");
                levelCodesParameter.Append("'");
                levelCodesParameter.Append(levelCode + "%");
                levelCodesParameter.Append("'");
                levelCodesParameter.Append(" OR ");
                levelCodesParameter.Append(string.Format("CHARINDEX(D.LevelCode,'{0}')>0", levelCode));
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
                m_levelCodesParameter = " and D.OrganizationID <> D.OrganizationID";
            }
            if (myLevelCode.Length < 3)
            {
                m_Sql = string.Format(m_Sql, m_levelCodesParameter, myLevelCode, myDate.ToString("yyyy-MM-dd"), myDate.ToString("yyyy-MM-01"), myDate.ToString("yyyy"), Weekly[myDate.Month - 1], "replace(W.Name,'公司','')");
            }
            else
            {
                m_Sql = string.Format(m_Sql, m_levelCodesParameter, myLevelCode, myDate.ToString("yyyy-MM-dd"), myDate.ToString("yyyy-MM-01"), myDate.ToString("yyyy"), Weekly[myDate.Month - 1], "''");
            }
            try
            {
                DataTable m_Result = _dataFactory.Query(m_Sql);
                DataTable m_CompanyCompleteTable = GetCompanyCompleteTable(m_Result, myDate);

                m_ReturnJsonString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_CompanyCompleteTable);
            }
            catch
            {

            }

            return m_ReturnJsonString;
        }
        private static DataTable GetCompanyCompleteTable(DataTable myResult, DateTime myDate)
        {
            DataTable m_CompanyCompleteTable = new DataTable();
            m_CompanyCompleteTable.Columns.Add("Name", typeof(string));
            m_CompanyCompleteTable.Columns.Add("DataItem", typeof(string));
            m_CompanyCompleteTable.Columns.Add("Value_Day", typeof(decimal));
            m_CompanyCompleteTable.Columns.Add("Value_Month", typeof(decimal));
            m_CompanyCompleteTable.Columns.Add("Value_Plan", typeof(decimal));
            m_CompanyCompleteTable.Columns.Add("Value_Deviation", typeof(decimal));
            if (myResult != null)
            {
                string m_LevelCode = "";
                string m_Name = "";
                string m_Type = "";
                decimal m_rawMaterialsElectricityQuantity_day = 0.0m;
                decimal m_rawMaterialsElectricityQuantity_month = 0.0m;
                decimal m_clinkerPreparationElectricityQuantity_day = 0.0m;
                decimal m_clinkerPreparationElectricityQuantity_month = 0.0m;
                decimal m_CementPreparationElectricityQuantity_day = 0.0m;
                decimal m_CementPreparationElectricityQuantity_month = 0.0m;

                decimal m_clinker_MixtureMaterialsOutput_day = 0.0m;
                decimal m_clinker_MixtureMaterialsOutput_month = 0.0m;
                decimal m_clinker_ClinkerOutput_day = 0.0m;
                decimal m_clinker_ClinkerOutput_month = 0.0m;
                decimal m_cement_CementOutput_day = 0.0m;
                decimal m_cement_CementOutput_month = 0.0m;

                decimal m_rawMaterialsElectricityConsumption_Plan = 0.0m;
                decimal m_clinkerPreparationElectricityConsumption_Plan = 0.0m;
                decimal m_CementElectricityConsumption_Plan = 0.0m;
                /////////////////////增加分步电耗均摊辅助电量的功能/////////////////////
                List<string> m_OrganizationIds = new List<string>();
                for (int i = 0; i < myResult.Rows.Count; i++)
                {
                    if (!m_OrganizationIds.Contains(myResult.Rows[i]["OrganizationID"].ToString()))
                    {
                        m_OrganizationIds.Add(myResult.Rows[i]["OrganizationID"].ToString());
                    }
                }              
                DataTable m_AuxiliaryProductionElectricityQuantityTable = GetAuxiliaryProductionElectricityQuantity(m_OrganizationIds, myDate);
                DataTable m_Result = SetAuxiliaryProductionElectricityQuantity(myResult, m_AuxiliaryProductionElectricityQuantityTable);
                /////////////////////////////////////////////////////////////////////////////////////////////


                for (int i = 0; i < m_Result.Rows.Count; i++)
                {
                    if (i != 0 && m_Result.Rows[i]["LevelCode"].ToString() != m_LevelCode)
                    {
                        if (m_Type == "熟料")
                        {
                            decimal m_rawMaterialsElectricityConsumption_day = m_clinker_MixtureMaterialsOutput_day != 0 ? m_rawMaterialsElectricityQuantity_day / m_clinker_MixtureMaterialsOutput_day : 0.0m;
                            decimal m_rawMaterialsElectricityConsumption_month = m_clinker_MixtureMaterialsOutput_month != 0 ? m_rawMaterialsElectricityQuantity_month / m_clinker_MixtureMaterialsOutput_month : 0.0m;
                            decimal m_clinkerPreparationElectricityConsumption_day = m_clinker_ClinkerOutput_day != 0 ? m_clinkerPreparationElectricityQuantity_day / m_clinker_ClinkerOutput_day : 0.0m;
                            decimal m_clinkerPreparationElectricityConsumption_month = m_clinker_ClinkerOutput_month != 0 ? m_clinkerPreparationElectricityQuantity_month / m_clinker_ClinkerOutput_month : 0.0m;
                            m_CompanyCompleteTable.Rows.Add(m_Name, "生料制备", m_rawMaterialsElectricityConsumption_day, m_rawMaterialsElectricityConsumption_month, m_rawMaterialsElectricityConsumption_Plan, m_rawMaterialsElectricityConsumption_month - m_rawMaterialsElectricityConsumption_Plan);
                            m_CompanyCompleteTable.Rows.Add(m_Name, "熟料制备", m_clinkerPreparationElectricityConsumption_day, m_clinkerPreparationElectricityConsumption_month, m_clinkerPreparationElectricityConsumption_Plan, m_clinkerPreparationElectricityConsumption_Plan - m_clinkerPreparationElectricityConsumption_Plan);
                        }
                        else if (m_Type == "水泥磨")
                        {
                            decimal m_CementElectricityConsumption_day = m_cement_CementOutput_day != 0 ? m_CementPreparationElectricityQuantity_day / m_cement_CementOutput_day : 0.0m;
                            decimal m_CementElectricityConsumption_month = m_cement_CementOutput_month != 0 ? m_CementPreparationElectricityQuantity_month / m_cement_CementOutput_month : 0.0m;
                            m_CompanyCompleteTable.Rows.Add(m_Name, "水泥制备", m_CementElectricityConsumption_day, m_CementElectricityConsumption_month, m_CementElectricityConsumption_Plan, m_CementElectricityConsumption_month - m_CementElectricityConsumption_Plan);
                        }

                        m_rawMaterialsElectricityQuantity_day = 0.0m;
                        m_rawMaterialsElectricityQuantity_month = 0.0m;
                        m_clinkerPreparationElectricityQuantity_day = 0.0m;
                        m_clinkerPreparationElectricityQuantity_month = 0.0m;
                        m_CementPreparationElectricityQuantity_day = 0.0m;
                        m_CementPreparationElectricityQuantity_month = 0.0m;

                        m_clinker_MixtureMaterialsOutput_day = 0.0m;
                        m_clinker_MixtureMaterialsOutput_month = 0.0m;
                        m_clinker_ClinkerOutput_day = 0.0m;
                        m_clinker_ClinkerOutput_month = 0.0m;
                        m_cement_CementOutput_day = 0.0m;
                        m_cement_CementOutput_month = 0.0m;
                    }

                    m_LevelCode = m_Result.Rows[i]["LevelCode"].ToString();
                    m_Name = m_Result.Rows[i]["Name"].ToString();
                    m_Type = m_Result.Rows[i]["Type"].ToString();
                    /////////////////生料制备/////////////////
                    if (m_Result.Rows[i]["LevelCode"].ToString() == m_LevelCode && m_Result.Rows[i]["VariableId"].ToString() == "rawMaterialsPreparation_ElectricityQuantity_day")
                    {
                        m_rawMaterialsElectricityQuantity_day = m_Result.Rows[i]["value"] != DBNull.Value ? (decimal)m_Result.Rows[i]["value"] : 0.0m;
                    }
                    else if (m_Result.Rows[i]["LevelCode"].ToString() == m_LevelCode && m_Result.Rows[i]["VariableId"].ToString() == "clinker_MixtureMaterialsOutput_day")
                    {
                        m_clinker_MixtureMaterialsOutput_day = m_Result.Rows[i]["value"] != DBNull.Value ? (decimal)m_Result.Rows[i]["value"] : 0.0m;
                    }
                    if (m_Result.Rows[i]["LevelCode"].ToString() == m_LevelCode && m_Result.Rows[i]["VariableId"].ToString() == "rawMaterialsPreparation_ElectricityQuantity_month")
                    {
                        m_rawMaterialsElectricityQuantity_month = m_Result.Rows[i]["value"] != DBNull.Value ? (decimal)m_Result.Rows[i]["value"] : 0.0m;
                    }
                    else if (m_Result.Rows[i]["LevelCode"].ToString() == m_LevelCode && m_Result.Rows[i]["VariableId"].ToString() == "clinker_MixtureMaterialsOutput_month")
                    {
                        m_clinker_MixtureMaterialsOutput_month = m_Result.Rows[i]["value"] != DBNull.Value ? (decimal)m_Result.Rows[i]["value"] : 0.0m;
                    }
                    /////////////////熟料烧成/////////////////
                    if (m_Result.Rows[i]["LevelCode"].ToString() == m_LevelCode && m_Result.Rows[i]["VariableId"].ToString() == "clinkerPreparation_ElectricityQuantity_day")
                    {
                        m_clinkerPreparationElectricityQuantity_day = m_Result.Rows[i]["value"] != DBNull.Value ? (decimal)m_Result.Rows[i]["value"] : 0.0m;
                    }
                    else if (m_Result.Rows[i]["LevelCode"].ToString() == m_LevelCode && m_Result.Rows[i]["VariableId"].ToString() == "clinker_ClinkerOutput_day")
                    {
                        m_clinker_ClinkerOutput_day = m_Result.Rows[i]["value"] != DBNull.Value ? (decimal)m_Result.Rows[i]["value"] : 0.0m;
                    }
                    if (m_Result.Rows[i]["LevelCode"].ToString() == m_LevelCode && m_Result.Rows[i]["VariableId"].ToString() == "clinkerPreparation_ElectricityQuantity_month")
                    {
                        m_clinkerPreparationElectricityQuantity_month = m_Result.Rows[i]["value"] != DBNull.Value ? (decimal)m_Result.Rows[i]["value"] : 0.0m;
                    }
                    else if (m_Result.Rows[i]["LevelCode"].ToString() == m_LevelCode && m_Result.Rows[i]["VariableId"].ToString() == "clinker_ClinkerOutput_month")
                    {
                        m_clinker_ClinkerOutput_month = m_Result.Rows[i]["value"] != DBNull.Value ? (decimal)m_Result.Rows[i]["value"] : 0.0m;
                    }
                    /////////////////水泥制备/////////////////
                    if (m_Result.Rows[i]["LevelCode"].ToString() == m_LevelCode && m_Result.Rows[i]["VariableId"].ToString() == "cementPreparation_ElectricityQuantity_day")
                    {
                        m_CementPreparationElectricityQuantity_day = m_Result.Rows[i]["value"] != DBNull.Value ? (decimal)m_Result.Rows[i]["value"] : 0.0m;
                    }
                    else if (m_Result.Rows[i]["LevelCode"].ToString() == m_LevelCode && m_Result.Rows[i]["VariableId"].ToString() == "cement_CementOutput_day")
                    {
                        m_cement_CementOutput_day = m_Result.Rows[i]["value"] != DBNull.Value ? (decimal)m_Result.Rows[i]["value"] : 0.0m;
                    }
                    if (m_Result.Rows[i]["LevelCode"].ToString() == m_LevelCode && m_Result.Rows[i]["VariableId"].ToString() == "cementPreparation_ElectricityQuantity_month")
                    {
                        m_CementPreparationElectricityQuantity_month = m_Result.Rows[i]["value"] != DBNull.Value ? (decimal)m_Result.Rows[i]["value"] : 0.0m;
                    }
                    else if (m_Result.Rows[i]["LevelCode"].ToString() == m_LevelCode && m_Result.Rows[i]["VariableId"].ToString() == "cement_CementOutput_month")
                    {
                        m_cement_CementOutput_month = m_Result.Rows[i]["value"] != DBNull.Value ? (decimal)m_Result.Rows[i]["value"] : 0.0m;
                    }
                    else if (m_Result.Rows[i]["LevelCode"].ToString() == m_LevelCode && m_Result.Rows[i]["VariableId"].ToString() == "rawMaterialsPreparationElectricityConsumption_Plan")
                    {
                        m_rawMaterialsElectricityConsumption_Plan = m_Result.Rows[i]["value"] != DBNull.Value ? (decimal)m_Result.Rows[i]["value"] : 0.0m;
                    }
                    else if (m_Result.Rows[i]["LevelCode"].ToString() == m_LevelCode && m_Result.Rows[i]["VariableId"].ToString() == "clinkerPreparationElectricityConsumption_Plan")
                    {
                        m_clinkerPreparationElectricityConsumption_Plan = m_Result.Rows[i]["value"] != DBNull.Value ? (decimal)m_Result.Rows[i]["value"] : 0.0m;
                    }
                    else if (m_Result.Rows[i]["LevelCode"].ToString() == m_LevelCode && m_Result.Rows[i]["VariableId"].ToString() == "cementPreparationElectricityConsumption_Plan")
                    {
                        m_CementElectricityConsumption_Plan = m_Result.Rows[i]["value"] != DBNull.Value ? (decimal)m_Result.Rows[i]["value"] : 0.0m;
                    }
                    /////////////////////////////////////////////
                    
                    if (i + 1 == m_Result.Rows.Count)
                    {
                        if (m_Type == "熟料")
                        {
                            decimal m_rawMaterialsElectricityConsumption_day = m_clinker_MixtureMaterialsOutput_day != 0 ? m_rawMaterialsElectricityQuantity_day / m_clinker_MixtureMaterialsOutput_day : 0.0m;
                            decimal m_rawMaterialsElectricityConsumption_month = m_clinker_MixtureMaterialsOutput_month != 0 ? m_rawMaterialsElectricityQuantity_month / m_clinker_MixtureMaterialsOutput_month : 0.0m;
                            decimal m_clinkerPreparationElectricityConsumption_day = m_clinker_ClinkerOutput_day != 0 ? m_clinkerPreparationElectricityQuantity_day / m_clinker_ClinkerOutput_day : 0.0m;
                            decimal m_clinkerPreparationElectricityConsumption_month = m_clinker_ClinkerOutput_month != 0 ? m_clinkerPreparationElectricityQuantity_month / m_clinker_ClinkerOutput_month : 0.0m;
                            m_CompanyCompleteTable.Rows.Add(m_Name, "生料制备", m_rawMaterialsElectricityConsumption_day, m_rawMaterialsElectricityConsumption_month, m_rawMaterialsElectricityConsumption_Plan, m_rawMaterialsElectricityConsumption_month - m_rawMaterialsElectricityConsumption_Plan);
                            m_CompanyCompleteTable.Rows.Add(m_Name, "熟料制备", m_clinkerPreparationElectricityConsumption_day, m_clinkerPreparationElectricityConsumption_month, m_clinkerPreparationElectricityConsumption_Plan, m_clinkerPreparationElectricityConsumption_Plan - m_clinkerPreparationElectricityConsumption_Plan);
                        }
                        else if (m_Type == "水泥磨")
                        {
                            decimal m_CementElectricityConsumption_day = m_cement_CementOutput_day != 0 ? m_CementPreparationElectricityQuantity_day / m_cement_CementOutput_day : 0.0m;
                            decimal m_CementElectricityConsumption_month = m_cement_CementOutput_month != 0 ? m_CementPreparationElectricityQuantity_month / m_cement_CementOutput_month : 0.0m;
                            m_CompanyCompleteTable.Rows.Add(m_Name, "水泥制备", m_CementElectricityConsumption_day, m_CementElectricityConsumption_month, m_CementElectricityConsumption_Plan, m_CementElectricityConsumption_month - m_CementElectricityConsumption_Plan);
                        }
                    }
                }
            }

            return m_CompanyCompleteTable;
        }
        private static DataTable GetAuxiliaryProductionElectricityQuantity(List<string> myOrganizationIds, DateTime myDate)
        {
            string m_Sql = @"Select M.OrganizationId
                                ,M.VariableId
                                ,M.DayValue
                                ,N.MonthValue 
                                from 
                                (SELECT B.OrganizationID as OrganizationId
                                      ,B.VariableId
	                                  ,sum(B.TotalPeakValleyFlatB) as DayValue
                                  FROM tz_Balance A, balance_Energy B
                                  where A.StaticsCycle = 'day'
                                  and A.TimeStamp >='{1}'
                                  and A.TimeStamp <='{1}'
                                  and B.OrganizationID in ({2})
                                  and A.BalanceId = B.KeyId
                                  and B.VariableId = 'auxiliaryProduction_ElectricityQuantity'
                                  group by B.OrganizationID, B.VariableId) M,
                                  (SELECT B.OrganizationID as OrganizationId
                                      ,B.VariableId
	                                  ,sum(B.TotalPeakValleyFlatB) as MonthValue
                                  FROM tz_Balance A, balance_Energy B
                                  where A.StaticsCycle = 'day'
                                  and A.TimeStamp >='{0}'
                                  and A.TimeStamp <='{1}'
                                  and B.OrganizationID in ({2})
                                  and A.BalanceId = B.KeyId
                                  and B.VariableId = 'auxiliaryProduction_ElectricityQuantity'
                                  group by B.OrganizationID, B.VariableId) N
                                  where M.OrganizationID = N.OrganizationID
                                  and M.VariableId = N.VariableId";
            string m_Condition = "''";
            for (int i = 0; i < myOrganizationIds.Count; i++)
            {
                if (i == 0)
                {
                    m_Condition = "'" + myOrganizationIds[i] + "'";
                }
                else
                {
                    m_Condition = m_Condition + ",'" + myOrganizationIds[i] + "'";
                }
            }
            m_Sql = string.Format(m_Sql, myDate.ToString("yyyy-MM-01"), myDate.ToString("yyyy-MM-dd"), m_Condition);
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
        private static DataTable SetAuxiliaryProductionElectricityQuantity(DataTable myResultTable, DataTable myAuxiliaryProductionElectricityQuantityTable)
        {
            if (myAuxiliaryProductionElectricityQuantityTable != null && myResultTable != null)
            {
                for (int i = 0; i < myAuxiliaryProductionElectricityQuantityTable.Rows.Count; i++)
                {
                    decimal m_AuxiliaryProductionElectricityQuantityDay = myAuxiliaryProductionElectricityQuantityTable.Rows[i]["DayValue"] != DBNull.Value?(decimal)myAuxiliaryProductionElectricityQuantityTable.Rows[i]["DayValue"]:0.0m;
                    decimal m_AuxiliaryProductionElectricityQuantityMonth = myAuxiliaryProductionElectricityQuantityTable.Rows[i]["MonthValue"] != DBNull.Value?(decimal)myAuxiliaryProductionElectricityQuantityTable.Rows[i]["MonthValue"]:0.0m;

                    if (m_AuxiliaryProductionElectricityQuantityDay > 0.0m)
                    {
                        decimal m_TotalValue = 0.0m;
                        DataRow[] m_CompanyCompleteRowsDay = myResultTable.Select(string.Format("OrganizationID = '{0}' and VariableId in ('rawMaterialsPreparation_ElectricityQuantity_day','clinkerPreparation_ElectricityQuantity_day','CementPreparation_ElectricityQuantity_day')", myAuxiliaryProductionElectricityQuantityTable.Rows[i]["OrganizationID"].ToString()));
                        for (int j = 0; j < m_CompanyCompleteRowsDay.Length; j++)
                        {
                            m_TotalValue = m_TotalValue + (m_CompanyCompleteRowsDay[j]["value"] != DBNull.Value ? (decimal)m_CompanyCompleteRowsDay[j]["value"] : 0.0m);
                        }
                        if (m_TotalValue != 0.0m)
                        {
                            decimal m_SharedValue = 0.0m;
                            for (int j = 0; j < m_CompanyCompleteRowsDay.Length; j++)
                            {
                                decimal m_RowValueTemp = m_CompanyCompleteRowsDay[j]["value"] != DBNull.Value ? (decimal)m_CompanyCompleteRowsDay[j]["value"] : 0.0m;
                                if (j < m_CompanyCompleteRowsDay.Length - 1)
                                {
                                    m_SharedValue = m_SharedValue + m_AuxiliaryProductionElectricityQuantityDay * m_RowValueTemp / m_TotalValue;
                                    m_CompanyCompleteRowsDay[j]["value"] = m_RowValueTemp + m_AuxiliaryProductionElectricityQuantityDay * m_RowValueTemp / m_TotalValue;
                                }
                                else
                                {
                                    m_CompanyCompleteRowsDay[j]["value"] = m_RowValueTemp + m_AuxiliaryProductionElectricityQuantityDay - m_SharedValue;
                                }
                            }
                        }
                        else
                        {
                            decimal m_SharedValue = 0.0m;
                            for (int j = 0; j < m_CompanyCompleteRowsDay.Length; j++)
                            {
                                if (j < m_CompanyCompleteRowsDay.Length - 1)
                                {
                                    m_SharedValue = m_SharedValue + m_AuxiliaryProductionElectricityQuantityDay / m_CompanyCompleteRowsDay.Length;
                                    m_CompanyCompleteRowsDay[j]["value"] = m_AuxiliaryProductionElectricityQuantityDay / m_CompanyCompleteRowsDay.Length;
                                }
                                else
                                {
                                    m_CompanyCompleteRowsDay[j]["value"] = m_AuxiliaryProductionElectricityQuantityDay - m_SharedValue;
                                }
                            }
                        }
                    }
                    if (m_AuxiliaryProductionElectricityQuantityMonth > 0.0m)
                    {
                        decimal m_TotalValue = 0.0m;
                        DataRow[] m_CompanyCompleteRowsMonth = myResultTable.Select(string.Format("OrganizationID = '{0}' and VariableId in ('rawMaterialsPreparation_ElectricityQuantity_month','clinkerPreparation_ElectricityQuantity_month','CementPreparation_ElectricityQuantity_month')", myAuxiliaryProductionElectricityQuantityTable.Rows[i]["OrganizationID"].ToString()));
                        for (int j = 0; j < m_CompanyCompleteRowsMonth.Length; j++)
                        {
                            m_TotalValue = m_TotalValue + (m_CompanyCompleteRowsMonth[j]["value"] != DBNull.Value ? (decimal)m_CompanyCompleteRowsMonth[j]["value"] : 0.0m);
                        }
                        if (m_TotalValue != 0.0m)
                        {
                            decimal m_SharedValue = 0.0m;
                            for (int j = 0; j < m_CompanyCompleteRowsMonth.Length; j++)
                            {
                                decimal m_RowValueTemp = m_CompanyCompleteRowsMonth[j]["value"] != DBNull.Value ? (decimal)m_CompanyCompleteRowsMonth[j]["value"] : 0.0m;
                                if (j < m_CompanyCompleteRowsMonth.Length - 1)
                                {
                                    m_SharedValue = m_SharedValue + m_AuxiliaryProductionElectricityQuantityMonth * m_RowValueTemp / m_TotalValue;
                                    m_CompanyCompleteRowsMonth[j]["value"] = m_RowValueTemp + m_AuxiliaryProductionElectricityQuantityMonth * m_RowValueTemp / m_TotalValue;
                                }
                                else
                                {
                                    m_CompanyCompleteRowsMonth[j]["value"] = m_RowValueTemp + m_AuxiliaryProductionElectricityQuantityMonth - m_SharedValue;
                                }
                            }
                        }
                        else
                        {
                            decimal m_SharedValue = 0.0m;
                            for (int j = 0; j < m_CompanyCompleteRowsMonth.Length; j++)
                            {
                                if (j < m_CompanyCompleteRowsMonth.Length - 1)
                                {
                                    m_SharedValue = m_SharedValue + m_AuxiliaryProductionElectricityQuantityMonth / m_CompanyCompleteRowsMonth.Length;
                                    m_CompanyCompleteRowsMonth[j]["value"] = m_AuxiliaryProductionElectricityQuantityMonth / m_CompanyCompleteRowsMonth.Length;
                                }
                                else
                                {
                                    m_CompanyCompleteRowsMonth[j]["value"] = m_AuxiliaryProductionElectricityQuantityMonth - m_SharedValue;
                                }
                            }
                        }
                    }
                }
            }
            return myResultTable;
        }
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
                              order by DisplayIndex";
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

        public static string GetQuntityPlanAndComplete(DateTime myDate, string myVariableId, string myValueType, string[] myOganizationIds)
        {
            DataTable m_CompleteTable = GetNormalQuantityCompleteData(myDate, myVariableId + "_" + myValueType, myValueType, myOganizationIds);
            DataTable m_PlanTable = GetQuantityPlanData(myDate, myVariableId, myValueType, myOganizationIds, "Normal");
            string m_ReturnValue = GetChartString(m_CompleteTable, m_PlanTable);
            return m_ReturnValue;
        }
        public static string GetWeightPlanAndComplete(DateTime myDate, string myVariableId, string myValueType, string[] myOganizationIds)
        {
            DataTable m_CompleteTable = GetNormalQuantityCompleteData(myDate, myVariableId, myValueType, myOganizationIds);
            DataTable m_PlanTable = GetQuantityPlanData(myDate, myVariableId, myValueType, myOganizationIds, "Normal");
            string m_ReturnValue = GetChartString(m_CompleteTable, m_PlanTable);
            return m_ReturnValue;
        }
        public static string GetElectricityConsumptionPlanAndComplete(DateTime myDate, string myVariableId, string myValueType, string myDenominator, string[] myOganizationIds)
        {
            DataTable m_CompleteTable = GetNormalConsumptionCompleteData(myDate, myVariableId + "_" + myValueType, myValueType, myDenominator, myOganizationIds);
            DataTable m_PlanTable = GetConsumptionPlanData(myDate, myVariableId, myValueType, myDenominator, myOganizationIds, "Normal");
            string m_ReturnValue = GetChartString(m_CompleteTable, m_PlanTable);
            return m_ReturnValue;
        }
        public static string GetComprehensivePlanAndComplete(DateTime myDate, string myVariableId, string myValueType, string myDenominator, string[] myOganizationIds)
        {
            DataTable m_CompleteTable = GetComprehensiveCompleteData(myDate, myVariableId, myValueType, myOganizationIds);
            DataTable m_PlanTable = GetConsumptionPlanData(myDate, myVariableId, myValueType, myDenominator, myOganizationIds, "Comprehensive");
            string m_ReturnValue = GetChartString(m_CompleteTable, m_PlanTable);
            return m_ReturnValue;
        }
        private static DataTable GetLevelCodeByOrganizationIds(string[] myOrganizationIds)
        {
            string m_Sql = @"Select A.OrganizationID, A.Name, A.LevelCode, '' as VariableId, 0 as Value
                                from system_Organization A
                                where A.OrganizationID = A.OrganizationID
                                {0}
                                order by A.OrganizationID";
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
            m_Sql = string.Format(m_Sql, m_OrganizationCondition);
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
            string m_Sql = @"select C.OrganizationID, C.Name, B.VariableId, sum(B.TotalPeakValleyFlat) as Value
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
                                group by C.OrganizationID, C.Name, B.VariableId
                                order by C.OrganizationID";
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
                                select C.OrganizationID, C.Name, B.VariableId, '{5}' as Denominator, B.TotalPeakValleyFlat as Value
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
	                       (select C.OrganizationID, C.Name, B.VariableId, B.TotalPeakValleyFlat as Value
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
        private static DataTable GetComprehensiveCompleteData(DateTime myDate, string myVariableId, string myValueType, string[] myOganizationIds)
        {
            DataTable m_CompleteTable = GetLevelCodeByOrganizationIds(myOganizationIds);
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
            string m_Sql = @"select D.OrganizationID, D.Name, C.VariableId, B.QuotasName, sum(B.{4}) as Value
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
                                group by D.OrganizationID, D.Name, C.VariableId, B.QuotasName
                                order by D.OrganizationID";
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
                                    select D.OrganizationID, D.Name, C.VariableId, B.{5} as Value, C.Denominator, B.QuotasName
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
	                                (select D.OrganizationID, D.Name, C.VariableId, B.{5} as Value
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
        private static string GetChartString(DataTable myPerfomance, DataTable myPlanData)
        {
            DataTable m_ChartDataTableStruct = new DataTable();
            List<string> m_ColumnNameList = new List<string>();
            string[] m_RowsName = new string[]{"实绩","计划"};
            string m_UnitX = "";  // "单位";
            string m_UnitY = "";
            if (myPerfomance != null && myPlanData != null)
            {
                if (myPlanData.Rows.Count > 0)
                {
                    m_UnitY = myPlanData.Rows[0]["QuotasName"].ToString();
                }
                for(int i=0;i<myPerfomance.Rows.Count;i++)
                {
                    m_ChartDataTableStruct.Columns.Add(myPerfomance.Rows[i]["OrganizationID"].ToString(), typeof(decimal));
                    m_ColumnNameList.Add(myPerfomance.Rows[i]["Name"].ToString());
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
    }
}
