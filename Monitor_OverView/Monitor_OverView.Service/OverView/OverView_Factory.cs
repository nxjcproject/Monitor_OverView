using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SqlServerDataAdapter;

namespace Monitor_OverView.Service.OverView
{
    public class OverView_Factory
    {
        private static readonly ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(Monitor_OverView.Infrastruture.Configuration.ConnectionStringFactory.NXJCConnectionString);
        private static readonly AutoSetParameters.AutoGetEnergyConsumption_V1 _AutoGetEnergyConsumption_V1 = new AutoSetParameters.AutoGetEnergyConsumption_V1(_dataFactory);
        public static string GetFactoryStationList(string myOrganizationId, List<string> myOrganizationIdList)
        {
            ////////////////////////
            string m_OrganizationListCondition = "''";
            string m_Sql = @"Select B.OrganizationID as OrganizationId,
                                B.LevelCode,
                                B.Name,
                                B.Type 
                                from system_Organization A, system_Organization B, system_Organization C
                                where B.LevelCode like A.LevelCode + '%'
                                and B.LevelType = 'Factory'
                                and C.OrganizationID in ({1})
								and A.LevelCode like C.LevelCode + '%'
								and A.OrganizationID = {0}";
            if (myOrganizationIdList != null)
            {

                for (int i = 0; i < myOrganizationIdList.Count; i++)
                {
                    if (i == 0)
                    {
                        m_OrganizationListCondition = string.Format("'{0}'", myOrganizationIdList[i]);
                    }
                    else
                    {
                        m_OrganizationListCondition = m_OrganizationListCondition + string.Format(",'{0}'", myOrganizationIdList[i]);
                    }

                }
            }
            if (myOrganizationId != "")
            {
                m_Sql = string.Format(m_Sql, "'" + myOrganizationId + "'", m_OrganizationListCondition);
            }
            else
            {
                m_Sql = string.Format(m_Sql, "A.OrganizationID", m_OrganizationListCondition);
            }
            
            try
            {
                DataTable result = _dataFactory.Query(m_Sql);
                string m_ResultValue = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(result);
                return m_ResultValue;
                
            }
            catch
            {
                return "{\"rows\":[],\"total\":0}";
            }
        }

        public static string GetElectricityQuantitiyDetail(string myVariableId, string myOrganizationId, string myOrganizationType, string myStartTime, string myEndTime, ref DataTable myDataTable)
        {
            string[] m_OrganizationTypeList = myOrganizationType.Split(',');
            string m_OrganizationType = "";

            for (int i = 0; i < m_OrganizationTypeList.Length; i++)
            {
                if(i==0)
                {
                    m_OrganizationType = "'" + m_OrganizationTypeList[i] + "'";
                }
                else
                {
                    m_OrganizationType = m_OrganizationType + ",'" + m_OrganizationTypeList[i] + "'";
                }
            }
            if (m_OrganizationType != "")
            {
                m_OrganizationType = string.Format(" and C.Type in ({0}) ", m_OrganizationType);
            }
            string m_Sql = @"select M.OrganizationID, W.Name + Z.Name as ProcessName, M.BalanceVariableId, M.VariableId, M.Value as DayElectricityQuantity, N.Value as MonthElectricityQuantity from 
                                (Select B.OrganizationID, B.VariableId as BalanceVariableId, A.StaticsCycle, '{0}' as VariableId, sum(B.TotalPeakValleyFlatB) as Value from tz_Balance A, balance_Energy B, system_Organization C, system_Organization D
                                where A.StaticsCycle = 'day'
                                and A.OrganizationID = '{1}'
                                and A.TimeStamp >= '{4}'
                                and A.TimeStamp <= '{4}'
                                and A.BalanceId = B.KeyId
                                and B.VariableId = '{0}_ElectricityQuantity'
                                and D.OrganizationID = A.OrganizationID
                                and C.LevelCode like D.LevelCode + '%'
                                {2}
                                and B.OrganizationID in (C.OrganizationID)
                                group by B.OrganizationID, B.VariableId, A.StaticsCycle) M,
                                (Select B.OrganizationID, B.VariableId as BalanceVariableId, A.StaticsCycle, '{0}' as VariableId, sum(B.TotalPeakValleyFlatB) as Value from tz_Balance A, balance_Energy B, system_Organization C, system_Organization D
                                where A.StaticsCycle = 'day'
                                and A.OrganizationID = '{1}'
                                and A.TimeStamp >= '{3}'
                                and A.TimeStamp <= '{4}'
                                and A.BalanceId = B.KeyId
                                and B.VariableId = '{0}_ElectricityQuantity'
                                and D.OrganizationID = A.OrganizationID
                                and C.LevelCode like D.LevelCode + '%'
                                {2}
                                and B.OrganizationID in (C.OrganizationID)
                                group by B.OrganizationID, B.VariableId, A.StaticsCycle) N, tz_Formula W, formula_FormulaDetail Z
                                where M.OrganizationID = N.OrganizationID
                                and M.BalanceVariableId = N.BalanceVariableId
                                and M.OrganizationID = W.OrganizationID
                                and W.KeyID = Z.KeyID
                                and W.Type = 2
                                and W.State = 0
                                and W.ENABLE = 1
                                and M.VariableId = Z.VariableId";
            m_Sql = string.Format(m_Sql, myVariableId, myOrganizationId, m_OrganizationType, myStartTime, myEndTime);
            try
            {
                myDataTable = _dataFactory.Query(m_Sql);
                string m_ResultValue = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(myDataTable);
                return m_ResultValue;
            }
            catch
            {
                return "{\"rows\":[],\"total\":0}";
            }
        }
        public static string GetElectricityQuantitiy(string myVariableIdList, string myOrganizationId, string myOrganizationType, string myStartTime, string myEndTime, ref DataTable myDataTable)
        {
            string[] m_OrganizationTypeList = myOrganizationType.Split(',');
            string m_OrganizationType = "";
            string[] m_VariableIdList = myVariableIdList.Split(',');
            string m_VariableId = "";
            for (int i = 0; i < m_OrganizationTypeList.Length; i++)
            {
                if (i == 0)
                {
                    m_OrganizationType = "'" + m_OrganizationTypeList[i] + "'";
                }
                else
                {
                    m_OrganizationType = m_OrganizationType + ",'" + m_OrganizationTypeList[i] + "'";
                }
            }
            if (m_OrganizationType != "")
            {
                m_OrganizationType = string.Format(" and C.Type in ({0}) ", m_OrganizationType);
            }
            for (int i = 0; i < m_VariableIdList.Length; i++)
            {
                if (i == 0)
                {
                    m_VariableId = "'" + m_VariableIdList[i] + "_ElectricityQuantity'";
                }
                else
                {
                    m_VariableId = m_VariableId + ",'" + m_VariableIdList[i] + "_ElectricityQuantity'";
                }
            }
            if (m_VariableId != "")
            {
                m_VariableId = string.Format(" and B.VariableId in ({0})", m_VariableId);
            }
            string m_Sql = @"select M.BalanceVariableId, M.Value as DayElectricityQuantity, N.Value as MonthElectricityQuantity from 
                                (Select B.VariableId as BalanceVariableId, A.StaticsCycle, sum(B.TotalPeakValleyFlatB) as Value from tz_Balance A, balance_Energy B, system_Organization C, system_Organization D
                                where A.StaticsCycle = 'day'
                                and A.OrganizationID = '{1}'
                                and A.TimeStamp >= '{4}'
                                and A.TimeStamp <= '{4}'
                                and A.BalanceId = B.KeyId
                                {0}
                                and D.OrganizationID = A.OrganizationID
                                and C.LevelCode like D.LevelCode + '%'
                                {2}
                                and B.OrganizationID in (C.OrganizationID)
                                group by B.VariableId, A.StaticsCycle) M,
                                (Select B.VariableId as BalanceVariableId, A.StaticsCycle, sum(B.TotalPeakValleyFlatB) as Value from tz_Balance A, balance_Energy B, system_Organization C, system_Organization D
                                where A.StaticsCycle = 'day'
                                and A.OrganizationID = '{1}'
                                and A.TimeStamp >= '{3}'
                                and A.TimeStamp <= '{4}'
                                and A.BalanceId = B.KeyId
                                {0}
                                and D.OrganizationID = A.OrganizationID
                                and C.LevelCode like D.LevelCode + '%'
                                {2}
                                and B.OrganizationID in (C.OrganizationID)
                                group by B.VariableId, A.StaticsCycle) N
                                where M.BalanceVariableId = N.BalanceVariableId";
            m_Sql = string.Format(m_Sql, m_VariableId, myOrganizationId, m_OrganizationType, myStartTime, myEndTime);
            try
            {
                myDataTable = _dataFactory.Query(m_Sql);
                string m_ResultValue = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(myDataTable);
                return m_ResultValue;
            }
            catch
            {
                return "{\"rows\":[],\"total\":0}";
            }
        }
        public static string GetMaterialWeightDetail(string myVariableId, string myOrganizationId, string myOrganizationType, string myStartTime, string myEndTime, ref DataTable myDataTable)
        {
            string m_Sql = @"Select M.OrganizationID, W.Name + Z.Name as MaterialName, M.VariableId, M.Value as DayMaterialWeight, N.Value as MonthMaterialWeight from 
                                (SELECT C.OrganizationID, B.VariableId, sum(B.TotalPeakValleyFlatB) as Value
                                  FROM tz_Balance A, balance_Energy B, system_Organization C, system_Organization D
                                  where D.OrganizationID = '{1}'
                                    and C.LevelCode like D.LevelCode + '%'
                                    and C.Type = '{2}'
                                    and A.StaticsCycle = 'day'
	                                and A.OrganizationID = D.OrganizationID
	                                and B.OrganizationID in (C.OrganizationID)
	                                and A.TimeStamp >= '{4}'
	                                and A.TimeStamp <= '{4}'
	                                and A.BalanceId = B.KeyId
	                                and B.VariableId = '{0}'
                                group by C.OrganizationID, B.VariableId) M,
                                (SELECT C.OrganizationID, B.VariableId, sum(B.TotalPeakValleyFlatB) as Value
                                  FROM tz_Balance A, balance_Energy B, system_Organization C, system_Organization D
                                  where D.OrganizationID = '{1}'
                                    and C.LevelCode like D.LevelCode + '%'
                                    and C.Type = '{2}'
                                    and A.StaticsCycle = 'day'
	                                and A.OrganizationID = D.OrganizationID
	                                and B.OrganizationID in (C.OrganizationID)
	                                and A.TimeStamp >= '{3}'
	                                and A.TimeStamp <= '{4}'
	                                and A.BalanceId = B.KeyId
	                                and B.VariableId = '{0}'
                                group by C.OrganizationID, B.VariableId) N, tz_Material W, material_MaterialDetail Z
                                where M.OrganizationID = N.OrganizationID
                                and M.VariableId = N.VariableId
                                and M.OrganizationID = W.OrganizationID
                                and W.KeyID = Z.KeyID
                                and W.Type = 2
                                and W.State = 0
                                and W.ENABLE = 1
                                and M.VariableId = Z.VariableId";
            m_Sql = string.Format(m_Sql, myVariableId, myOrganizationId, myOrganizationType, myStartTime, myEndTime);
            try
            {
                myDataTable = _dataFactory.Query(m_Sql);
                string m_ResultValue = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(myDataTable);
                return m_ResultValue;
            }
            catch
            {
                return "{\"rows\":[],\"total\":0}";
            }
        }
        public static string GetMaterialWeight(string myVariableIdList, string myOrganizationId, string myOrganizationType, string myStartTime, string myEndTime, ref DataTable myDataTable)
        {
            string[] m_OrganizationTypeList = myOrganizationType.Split(',');
            string m_OrganizationType = "";
            string[] m_VariableIdList = myVariableIdList.Split(',');
            string m_VariableId = "";
            for (int i = 0; i < m_OrganizationTypeList.Length; i++)
            {
                if (i == 0)
                {
                    m_OrganizationType = "'" + m_OrganizationTypeList[i] + "'";
                }
                else
                {
                    m_OrganizationType = m_OrganizationType + ",'" + m_OrganizationTypeList[i] + "'";
                }
            }
            if (m_OrganizationType != "")
            {
                m_OrganizationType = string.Format(" and C.Type in ({0}) ", m_OrganizationType);
            }
            for (int i = 0; i < m_VariableIdList.Length; i++)
            {
                if (i == 0)
                {
                    m_VariableId = "'" + m_VariableIdList[i] + "'";
                }
                else
                {
                    m_VariableId = m_VariableId + ",'" + m_VariableIdList[i] + "'";
                }
            }
            if (m_VariableId != "")
            {
                m_VariableId = string.Format(" and B.VariableId in ({0})", m_VariableId);
            }
            string m_Sql = @"Select M.VariableId, M.Value as DayMaterialWeight, N.Value as MonthMaterialWeight from 
                                (SELECT B.VariableId, sum(B.TotalPeakValleyFlatB) as Value
                                  FROM tz_Balance A, balance_Energy B, system_Organization C, system_Organization D
                                  where D.OrganizationID = '{1}'
                                    and C.LevelCode like D.LevelCode + '%'
                                    {2}
                                    and A.StaticsCycle = 'day'
	                                and A.OrganizationID = D.OrganizationID
	                                and B.OrganizationID in (C.OrganizationID)
	                                and A.TimeStamp >= '{4}'
	                                and A.TimeStamp <= '{4}'
	                                and A.BalanceId = B.KeyId
	                                {0}
                                group by B.VariableId) M,
                                (SELECT B.VariableId, sum(B.TotalPeakValleyFlatB) as Value
                                  FROM tz_Balance A, balance_Energy B, system_Organization C, system_Organization D
                                  where D.OrganizationID = '{1}'
                                    and C.LevelCode like D.LevelCode + '%'
                                    {2}
                                    and A.StaticsCycle = 'day'
	                                and A.OrganizationID = D.OrganizationID
	                                and B.OrganizationID in (C.OrganizationID)
	                                and A.TimeStamp >= '{3}'
	                                and A.TimeStamp <= '{4}'
	                                and A.BalanceId = B.KeyId
	                                {0}
                                group by B.VariableId) N
                                where M.VariableId = N.VariableId";
            m_Sql = string.Format(m_Sql, m_VariableId, myOrganizationId, m_OrganizationType, myStartTime, myEndTime);
            try
            {
                myDataTable = _dataFactory.Query(m_Sql);
                string m_ResultValue = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(myDataTable);
                return m_ResultValue;
            }
            catch
            {
                return "{\"rows\":[],\"total\":0}";
            }
        }
        public static DataTable GetOrganizationIdByClinckerContrast(string myOrganizationId, string myOrganizationType)
        {
            string m_Sql = @"Select A.* from analyse_KPI_OrganizationContrast A, system_Organization B, system_Organization C
                                where A.OrganizationID = B.OrganizationID
                                and B.Type = '{1}'
                                and B.LevelType = 'ProductionLine'
                                and B.LevelCode like C.LevelCode + '%'
                                and C.OrganizationID = '{0}'";
            m_Sql = string.Format(m_Sql, myOrganizationId, myOrganizationType);
            try
            {
                DataTable m_OrganizationTDataTable = _dataFactory.Query(m_Sql);
                return m_OrganizationTDataTable;
            }
            catch
            {
                return null;
            }
        }

        public static string GetElectricityConsumptionDetailData(DataTable myElectricityConsumptionTable)
        {
             string  m_ValueString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(myElectricityConsumptionTable);
             return m_ValueString;
        }
        public static string GetEnergyConsumptionComprehensiveDetail(string myOrganizationId, string myOrganizationType, string myStartTime, string myEndTime)
        {
            string m_Sql = @"SELECT 
                                A.OrganizationID as OrganizationId,
                                A.Name as Name,
                                A.LevelCode as LevelCode
                                from system_Organization A,system_Organization B 
                                where B.OrganizationID = '{0}'
                                and A.LevelCode like B.LevelCode + '%'
                                and A.Type = '{1}'
                                order by A.LevelCode";
            m_Sql = string.Format(m_Sql, myOrganizationId, myOrganizationType);
            try
            {
                DataTable m_OrganizationInfoTable = _dataFactory.Query(m_Sql);
                if (m_OrganizationInfoTable != null)
                {
                    m_OrganizationInfoTable.Columns.Add("ElectricityConsumption", typeof(decimal));
                    m_OrganizationInfoTable.Columns.Add("CoalConsumption", typeof(decimal));
                    m_OrganizationInfoTable.Columns.Add("EnergyConsumption", typeof(decimal));

                    if (myOrganizationType == "熟料")
                    {
                        for (int i = 0; i < m_OrganizationInfoTable.Rows.Count; i++)
                        {
                            Standard_GB16780_2012.Model_CaculateValue m_CaculateValue = _AutoGetEnergyConsumption_V1.GetClinkerEnergyConsumptionWithFormula("day", myStartTime, myEndTime, m_OrganizationInfoTable.Rows[i]["LevelCode"].ToString());
                            if (m_CaculateValue != null)
                            {
                                for (int j = 0; j < m_CaculateValue.CaculateFactor.Count; j++)
                                {
                                    if (m_CaculateValue.CaculateFactor[j].FactorName == "熟料综合电耗")
                                    {
                                        m_OrganizationInfoTable.Rows[i]["ElectricityConsumption"] = m_CaculateValue.CaculateFactor[j].FactorValue;
                                    }
                                    else if (m_CaculateValue.CaculateFactor[j].FactorName == "熟料综合煤耗")
                                    {
                                        m_OrganizationInfoTable.Rows[i]["CoalConsumption"] = m_CaculateValue.CaculateFactor[j].FactorValue;
                                    }
                                }
                                m_OrganizationInfoTable.Rows[i]["EnergyConsumption"] = m_CaculateValue.CaculateValue;
                            }

                        }
                    }
                    else if (myOrganizationType == "水泥磨")
                    {
                        for (int i = 0; i < m_OrganizationInfoTable.Rows.Count; i++)
                        {
                            Standard_GB16780_2012.Model_CaculateValue m_CaculateValue = _AutoGetEnergyConsumption_V1.GetCementEnergyConsumptionWithFormula("day", myStartTime, myEndTime, m_OrganizationInfoTable.Rows[i]["LevelCode"].ToString());
                            if (m_CaculateValue != null)
                            {
                                for (int j = 0; j < m_CaculateValue.CaculateFactor.Count; j++)
                                {
                                    if (m_CaculateValue.CaculateFactor[j].FactorName == "水泥综合电耗")
                                    {
                                        m_OrganizationInfoTable.Rows[i]["ElectricityConsumption"] = m_CaculateValue.CaculateFactor[j].FactorValue;
                                    }
                                }
                                m_OrganizationInfoTable.Rows[i]["EnergyConsumption"] = m_CaculateValue.CaculateValue;
                            }

                        }
                    }
                    string m_ReturnValue = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_OrganizationInfoTable);
                    return m_ReturnValue;
                }
                else
                {
                    return "{\"rows\":[],\"total\"0}";
                }
                    
            }
            catch
            {
                return "{\"rows\":[],\"total\"0}";
            }

        }
        public static string GetEnergyConsumptionComprehensive(string myOrganizationId, string myStartTime, string myEndTime)
        {
            string m_Sql = @"SELECT 
                                A.OrganizationID as OrganizationId,
                                A.Name as Name,
                                A.LevelCode as LevelCode
                                from system_Organization A
                                where A.OrganizationID = '{0}'";
            m_Sql = string.Format(m_Sql, myOrganizationId);
            try
            {
                DataTable m_OrganizationInfoTable = _dataFactory.Query(m_Sql);
                if (m_OrganizationInfoTable != null && m_OrganizationInfoTable.Rows.Count > 0)
                {
                    m_OrganizationInfoTable.Columns.Add("ElectricityConsumption", typeof(decimal));
                    m_OrganizationInfoTable.Columns.Add("CoalConsumption", typeof(decimal));
                    m_OrganizationInfoTable.Columns.Add("EnergyConsumption", typeof(decimal));

                    Standard_GB16780_2012.Model_CaculateValue m_CaculateValueClincker = _AutoGetEnergyConsumption_V1.GetClinkerEnergyConsumptionWithFormula("day", myStartTime, myEndTime, m_OrganizationInfoTable.Rows[0]["LevelCode"].ToString());
                    if (m_CaculateValueClincker != null)
                    {
                        for (int j = 0; j < m_CaculateValueClincker.CaculateFactor.Count; j++)
                        {
                            if (m_CaculateValueClincker.CaculateFactor[j].FactorName == "熟料综合电耗")
                            {
                                m_OrganizationInfoTable.Rows[0]["ElectricityConsumption"] = m_CaculateValueClincker.CaculateFactor[j].FactorValue;
                            }
                            else if (m_CaculateValueClincker.CaculateFactor[j].FactorName == "熟料综合煤耗")
                            {
                                m_OrganizationInfoTable.Rows[0]["CoalConsumption"] = m_CaculateValueClincker.CaculateFactor[j].FactorValue;
                            }
                        }
                        m_OrganizationInfoTable.Rows[0]["EnergyConsumption"] = m_CaculateValueClincker.CaculateValue;
                    }
                    Standard_GB16780_2012.Model_CaculateValue m_CaculateValueCementmill = _AutoGetEnergyConsumption_V1.GetCementEnergyConsumptionWithFormula("day", myStartTime, myEndTime, m_OrganizationInfoTable.Rows[0]["LevelCode"].ToString());
                    if (m_CaculateValueCementmill != null)
                    {
                        DataRow m_CementmillRows = m_OrganizationInfoTable.NewRow();
                        m_CementmillRows["OrganizationId"] = m_OrganizationInfoTable.Rows[0]["OrganizationId"];
                        m_CementmillRows["Name"] = m_OrganizationInfoTable.Rows[0]["Name"];
                        m_CementmillRows["LevelCode"] = m_OrganizationInfoTable.Rows[0]["LevelCode"];
                        for (int j = 0; j < m_CaculateValueCementmill.CaculateFactor.Count; j++)
                        {
                            if (m_CaculateValueCementmill.CaculateFactor[j].FactorName == "水泥综合电耗")
                            {
                                m_CementmillRows["ElectricityConsumption"] = m_CaculateValueCementmill.CaculateFactor[j].FactorValue;
                            }
                        }
                        m_CementmillRows["EnergyConsumption"] = m_CaculateValueCementmill.CaculateValue;
                        m_OrganizationInfoTable.Rows.Add(m_CementmillRows);
                    }
                    string m_ReturnValue = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_OrganizationInfoTable);
                    return m_ReturnValue;
                }
                else
                {
                    return "{\"rows\":[],\"total\"0}";
                }

            }
            catch
            {
                return "{\"rows\":[],\"total\"0}";
            }

        }
        public static string GetRunIndictorsDetail(string myEquipmentCommonId, string myFactoryOrganizationId, string myRunIndictorsList, string myStartTime, string myEndTime)
        {
            string[] m_RunIndictorsList = myRunIndictorsList.Split(',');
            DataTable m_RunIndictorsDetailTable = RunIndicators.EquipmentRunIndicators.GetEquipmentUtilizationByCommonId(m_RunIndictorsList, myEquipmentCommonId, myFactoryOrganizationId, myStartTime, myEndTime, _dataFactory);
            string m_ReturnString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_RunIndictorsDetailTable);
            return m_ReturnString;
        }
        public static string GetRunIndictors(string myRunIndictorsList, string myEquipmentCommonIdList, string myFactoryOrganizationId, string myStartTime, string myEndTime)
        {
            string[] m_RunIndictorsList = myRunIndictorsList.Split(',');
            string[] m_EquipmentCommonIdList = myEquipmentCommonIdList.Split(',');
            DataTable m_RunIndictorsDetailTable = RunIndicators.EquipmentRunIndicators.GetEquipmentCommonUtilization(m_RunIndictorsList, m_EquipmentCommonIdList, myFactoryOrganizationId, myStartTime, myEndTime, _dataFactory);
            string m_ReturnString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_RunIndictorsDetailTable);
            return m_ReturnString;
        }
        public static string GetEquipmentHaltDetail(string myEquipmentCommonId, string myFactoryOrganizationId, string myStatisticalRange, string myStartTime, string myEndTime)
        {
            DataTable m_RunIndictorsDetailTable = RunIndicators.EquipmentHalt.GetEquipmentHaltDetail(myEquipmentCommonId, myFactoryOrganizationId, myStatisticalRange, myStartTime, myEndTime, _dataFactory);
            string m_ReturnString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_RunIndictorsDetailTable);
            return m_ReturnString;
        }
        public static string GetEquipmentHalt(string myEquipmentCommonIdList, string myFactoryOrganizationId, string myStatisticalRange, string myStartTime, string myEndTime)
        {
            string[] m_EquipmentCommonIdList = myEquipmentCommonIdList.Split(',');
            DataTable m_RunIndictorsDetailTable = RunIndicators.EquipmentHalt.GetEquipmentHalt(m_EquipmentCommonIdList, myFactoryOrganizationId, myStatisticalRange, myStartTime, myEndTime, _dataFactory);
            string m_ReturnString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_RunIndictorsDetailTable);
            return m_ReturnString;
        }
        //右边区域的数据查询
        public static string GetEquipmentHaltAlarm(string myOrganizationId)
        {
            string m_Sql = @"SELECT A.AlarmItemId
                                  ,(case when A.AlarmType = 'MachineHalt' then '停机' when A.AlarmType = 'EnergyConsumption' then '能耗' else  A.AlarmType end ) as AlarmType
                                  ,A.AlarmDateTime
                                  ,B.Name + A.AlarmDescription as Name
                              FROM system_TenDaysRealtimeAlarm A, system_Organization B, system_Organization C
                              where A.OrganizationID = B.OrganizationID
                              and B.OrganizationID like C.OrganizationID + '%'
                              and C.OrganizationID = '{0}'
                              order by AlarmDateTime desc";
            m_Sql = string.Format(m_Sql, myOrganizationId);
            try
            {
                DataTable m_EquipmentHaltAlarmTable = _dataFactory.Query(m_Sql);
                string m_ReturnString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_EquipmentHaltAlarmTable);
                return m_ReturnString;

            }
            catch
            {
                return "{\"rows\":[],\"total\"0}";
            }
        }
        public static string GetWorkingTeamShiftLog(string myOrganizationId)
        {
            string m_Sql = @"SELECT top 6 A.WorkingTeamShiftLogID as WorkingTeamShiftLogId
                                  ,A.UpdateDate
                                  ,A.Shifts
                                  ,A.WorkingTeam
                                  ,B.Name as WorkingTeamShiftMonitor
                              FROM shift_WorkingTeamShiftLog A,system_StaffInfo B
                              where A.OrganizationID = '{0}'
                              and A.OrganizationID = B.OrganizationID
                              and A.ChargeManID = B.StaffInfoId
                              order by A.ShiftDate desc";
            m_Sql = string.Format(m_Sql, myOrganizationId);
            try
            {
                DataTable m_WorkingTeamShiftLogTable = _dataFactory.Query(m_Sql);
                string m_ReturnString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_WorkingTeamShiftLogTable);
                return m_ReturnString;

            }
            catch
            {
                return "{\"rows\":[],\"total\"0}";
            }
        }
        public static string GetWorkingTeamShiftLogDetail(string myOrganizationId, string myWorkingTeamShiftLogId)
        {
            string m_Sql = @"SELECT A.PerformToObjectives
                                  ,A.ProblemsAndSettlements
                                  ,A.EquipmentSituation
                                  ,A.AdvicesToNextShift
                              FROM shift_WorkingTeamShiftLog A
                              where A.OrganizationID = '{0}'
                              and A.WorkingTeamShiftLogID = '{1}'";
            m_Sql = string.Format(m_Sql, myOrganizationId, myWorkingTeamShiftLogId);
            try
            {
                DataTable m_WorkingTeamShiftLogDetailTable = _dataFactory.Query(m_Sql);
                string m_ReturnString = "";
                if (m_WorkingTeamShiftLogDetailTable != null && m_WorkingTeamShiftLogDetailTable.Rows.Count > 0)
                {
                    m_ReturnString = m_ReturnString + "本班生产计划完成情况:\r\n" + m_WorkingTeamShiftLogDetailTable.Rows[0]["PerformToObjectives"].ToString() + "\r\n";
                    m_ReturnString = m_ReturnString + "本班出现的问题及处理情况:\r\n" + m_WorkingTeamShiftLogDetailTable.Rows[0]["ProblemsAndSettlements"].ToString() + "\r\n";
                    m_ReturnString = m_ReturnString + "本班设备运行情况:\r\n" + m_WorkingTeamShiftLogDetailTable.Rows[0]["EquipmentSituation"].ToString() + "\r\n";
                    m_ReturnString = m_ReturnString + "下班工作重点及建议:\r\n" + m_WorkingTeamShiftLogDetailTable.Rows[0]["AdvicesToNextShift"].ToString() + "\r\n";
                }
                return m_ReturnString;

            }
            catch
            {
                return "";
            }
        }

        public static string GetMonthLineChartData(string myRunIndictors, string myEquipmentCommonIdList, string myOrganizationId, string myStartMonth, string myEndMonth)
        {
            DataTable m_MonthLineChartTable = RunIndicators.EquipmentRunIndicators.GetEquipmentCommonUtilizationPerMonth(myRunIndictors, myEquipmentCommonIdList, myOrganizationId, myStartMonth, myEndMonth, _dataFactory);
            string m_ReturnValue = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_MonthLineChartTable);
            return m_ReturnValue;
        }

        public static string GetElectricitiyConsumptionChartData(string myVariableIdList, string myOrganizationId, string myOrganizationTypeList, string myStartMonth, string myEndMonth)
        {        
            DataTable m_SourceData = GetElectricitiyConsumptionChartSourceData(myVariableIdList, myOrganizationId, myOrganizationTypeList, myStartMonth, myEndMonth);
            DataTable m_CaculateTemplate = GetCaculateTemplate(myVariableIdList, myOrganizationTypeList);
            if (m_SourceData != null && m_CaculateTemplate != null)
            {
                DataTable m_PerMonthSourceTable = new DataTable();
                m_PerMonthSourceTable.Columns.Add("VariableId", typeof(string));
                m_PerMonthSourceTable.Columns.Add("VariableName", typeof(string));

                List<string> m_MonthList = new List<string>();
                DateTime m_StartTime = DateTime.Parse(myStartMonth);
                DateTime m_EndMonth = DateTime.Parse(myEndMonth);
                int m_MonthIndex = 0;
                while (m_StartTime.AddMonths(m_MonthIndex) <= m_EndMonth)
                {
                    string m_MonthName = m_StartTime.AddMonths(m_MonthIndex).ToString("yyyy-MM");
                    m_MonthList.Add(m_MonthName);
                    m_PerMonthSourceTable.Columns.Add(m_MonthName, typeof(decimal));
                    m_MonthIndex = m_MonthIndex + 1;
                    //////////////填充数据表///////////
                    DataRow[] m_DataRowsTemp = m_SourceData.Select(string.Format("TimeStamp = '{0}'", m_MonthName));
                    for (int i = 0; i < m_DataRowsTemp.Length; i++)
                    {
                        bool m_HaveVariable = false;
                        for (int j = 0; j < m_PerMonthSourceTable.Rows.Count; j++)
                        {
                            //如果找到匹配的变量名,则写入数据
                            if (m_PerMonthSourceTable.Rows[j]["VariableId"].ToString() == m_DataRowsTemp[i]["VariableId"].ToString())
                            {
                                m_HaveVariable = true;
                                m_PerMonthSourceTable.Rows[j][m_MonthName] = m_DataRowsTemp[i]["Value"];
                            }
                        }
                        if (m_HaveVariable == false)        //如果当前没有找到匹配的变量名,则添加一行
                        {
                            DataRow m_NewDataRow = m_PerMonthSourceTable.NewRow();
                            m_NewDataRow["VariableId"] = m_DataRowsTemp[i]["VariableId"];
                            m_NewDataRow[m_MonthName] = m_DataRowsTemp[i]["Value"];
                            m_PerMonthSourceTable.Rows.Add(m_NewDataRow);
                        }
                    }
                }
                DataTable m_ElectricitiyConsumptionTable = EnergyConsumption.EnergyConsumptionCalculate.Calculate(m_PerMonthSourceTable, m_CaculateTemplate, "ValueFormula", m_MonthList.ToArray());
                m_ElectricitiyConsumptionTable.Columns.RemoveAt(2);
                for (int i = 0; i < m_ElectricitiyConsumptionTable.Rows.Count; i++)
                {
                    string m_VariableNameTemp = m_ElectricitiyConsumptionTable.Rows[i]["VariableName"].ToString();
                   
                    m_ElectricitiyConsumptionTable.Rows[i]["VariableName"] = m_VariableNameTemp.Substring(0, m_VariableNameTemp.Length - 2);
                }
                string m_ReturnValue = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_ElectricitiyConsumptionTable);
                return m_ReturnValue;
            }
            else
            {
                return "{\"rows\":[],\"total\"0}";
            }

        }
        private static DataTable GetCaculateTemplate(string myVariableIdList, string myOrganizationTypeList)
        {
            string[] m_VariableIdList = myVariableIdList.Split(',');
            string m_VariableId = "";
            for (int i = 0; i < m_VariableIdList.Length; i++)
            {
                if (i == 0)
                {
                    m_VariableId = "'" + m_VariableIdList[i] + "_ElectricityConsumption'";
                }
                else
                {
                    m_VariableId = m_VariableId + ",'" + m_VariableIdList[i] + "_ElectricityConsumption'";
                }
            }
            string[] m_OrganizationTypeList = myOrganizationTypeList.Split(',');
            string m_OrganizationType = "";
            for (int i = 0; i < m_OrganizationTypeList.Length; i++)
            {
                if (i == 0)
                {
                    m_OrganizationType = "'" + m_OrganizationTypeList[i] + "'";
                }
                else
                {
                    m_OrganizationType = m_OrganizationType + ",'" + m_OrganizationTypeList[i] + "'";
                }
            }
            string m_Sql = @"Select A.VariableId, A.VariableName,A.ValueFormula from balance_Energy_Template A
                                where A.VariableId in ({0})
                                and A.ValueType = 'ElectricityConsumption'
                                and A.ProductionLineType in ({1})
                                and A.Enabled = 1";
            m_Sql = string.Format(m_Sql, m_VariableId, m_OrganizationType);
            try
            {
                DataTable m_ElectricityQuantityTable = _dataFactory.Query(m_Sql);
                return m_ElectricityQuantityTable;

            }
            catch
            {
                return null;
            }
        }

        private static DataTable GetElectricitiyConsumptionChartSourceData(string myVariableIdList, string myOrganizationId, string myOrganizationTypeList, string myStartMonth, string myEndMonth)
        {
            DateTime m_LastMonth = DateTime.Parse(myEndMonth + "-01").AddMonths(-1);
            DateTime m_EndTime = DateTime.Parse(myEndMonth + "-01").AddMonths(1).AddDays(-1);
            string[] m_VariableIdList = myVariableIdList.Split(',');
            string m_VariableId = "";
            for (int i = 0; i < m_VariableIdList.Length; i++)
            {
                if (i == 0)
                {
                    m_VariableId = "'" + m_VariableIdList[i] + "_ElectricityQuantity'";
                }
                else
                {
                    m_VariableId = m_VariableId + ",'" + m_VariableIdList[i] + "_ElectricityQuantity'";
                }
            }
            string[] m_OrganizationTypeList = myOrganizationTypeList.Split(',');
            string m_OrganizationType = "";
            for (int i = 0; i < m_OrganizationTypeList.Length; i++)
            {
                if (i == 0)
                {
                    m_OrganizationType = "'" + m_OrganizationTypeList[i] + "'";
                }
                else
                {
                    m_OrganizationType = m_OrganizationType + ",'" + m_OrganizationTypeList[i] + "'";
                }
            }
            string m_Sql = @"Select M.TimeStamp, M.VariableId, M.Value from
                                (Select substring('{0}',0, 8) as TimeStamp, B.VariableId, sum(B.TotalPeakValleyFlatB) as Value from tz_Balance A, balance_Energy B, system_Organization C, system_Organization D
                                where A.TimeStamp >='{0}'
                                and A.TimeStamp <='{1}'
                                and A.StaticsCycle = 'day'
                                and A.OrganizationID = '{4}'
                                and A.BalanceId = B.KeyId
                                and ((B.ValueType = 'ElectricityQuantity'
                                and B.VariableId in ({6})) or B.ValueType = 'MaterialWeight')
                                and D.OrganizationID = A.OrganizationID
                                and C.LevelCode like D.LevelCode + '%'
                                and C.Type in ({5})
                                and B.OrganizationID = C.OrganizationID
                                group by B.VariableId
                             union all
                                Select A.TimeStamp, B.VariableId, sum(B.TotalPeakValleyFlatB) as Value from tz_Balance A, balance_Energy B, system_Organization C, system_Organization D
                                where A.TimeStamp >='{2}'
                                and A.TimeStamp <='{3}'
                                and A.StaticsCycle = 'month'
                                and A.OrganizationID = '{4}'
                                and A.BalanceId = B.KeyId
                                and ((B.ValueType = 'ElectricityQuantity'
                                and B.VariableId in ({6})) or B.ValueType = 'MaterialWeight')
                                and D.OrganizationID = A.OrganizationID
                                and C.LevelCode like D.LevelCode + '%'
                                and C.Type in ({5})
                                and B.OrganizationID = C.OrganizationID
                                group by A.TimeStamp, B.VariableId) M
                                order by M.VariableId";
            m_Sql = string.Format(m_Sql, myEndMonth + "-01", m_EndTime.ToString("yyyy-MM-dd"), myStartMonth, m_LastMonth.ToString("yyyy-MM"), myOrganizationId, m_OrganizationType, m_VariableId);
            try
            {
                DataTable m_ElectricityQuantityTable = _dataFactory.Query(m_Sql);
                return m_ElectricityQuantityTable;

            }
            catch
            {
                return null;
            }
        }
        private static DataTable GetElectricitiyConsumptionChart_MaterialWeight(string myOrganizationId, string myOrganizationTypeList, string myStartMonth, string myEndMonth)
        {
            DateTime m_LastMonth = DateTime.Parse(myEndMonth + "-01").AddMonths(-1);
            DateTime m_EndTime = DateTime.Parse(myEndMonth + "-01").AddMonths(1).AddDays(-1);
            string[] m_OrganizationTypeList = myOrganizationTypeList.Split(',');
            string m_OrganizationType = "";
            for (int i = 0; i < m_OrganizationTypeList.Length; i++)
            {
                if (i == 0)
                {
                    m_OrganizationType = "'" + m_OrganizationTypeList[i] + "'";
                }
                else
                {
                    m_OrganizationType = m_OrganizationType + ",'" + m_OrganizationTypeList[i] + "'";
                }
            }
            string m_Sql = @"Select M.VariableId, M.Value + N.Value as Value from 
                                (Select B.VariableId, sum(B.TotalPeakValleyFlatB) as Value from tz_Balance A, balance_Energy B, system_Organization C, system_Organization D
                                where A.TimeStamp >='{0}'
                                and A.TimeStamp <='{1}'
                                and A.StaticsCycle = 'day'
                                and A.OrganizationID = '{4}'
                                and A.BalanceId = B.KeyId
                                and B.ValueType = 'MaterialWeight'
                                and D.OrganizationID = A.OrganizationID
                                and C.LevelCode like D.LevelCode + '%'
                                and C.Type in ({5})
                                and B.OrganizationID = C.OrganizationID
                                group by B.VariableId) M,
                                (Select B.VariableId, sum(B.TotalPeakValleyFlatB) as Value from tz_Balance A, balance_Energy B, system_Organization C, system_Organization D
                                where A.TimeStamp >='{2}'
                                and A.TimeStamp <='{3}'
                                and A.StaticsCycle = 'month'
                                and A.OrganizationID = '{4}'
                                and A.BalanceId = B.KeyId
                                and B.ValueType = 'MaterialWeight'
                                and D.OrganizationID = A.OrganizationID
                                and C.LevelCode like D.LevelCode + '%'
                                and C.Type in ({5})
                                and B.OrganizationID = C.OrganizationID
                                group by B.VariableId) N
                                where M.VariableId = N.VariableId";
            m_Sql = string.Format(m_Sql, myEndMonth + "-01", m_EndTime.ToString("yyyy-MM-dd"), myStartMonth, m_LastMonth.ToString("yyyy-MM"), myOrganizationId, m_OrganizationType);
            try
            {
                DataTable m_ElectricityQuantityTable = _dataFactory.Query(m_Sql);
                return m_ElectricityQuantityTable;

            }
            catch
            {
                return null;
            }
        }
        /*
        public static DataTable GetFactoryByCompany(string myCompanyOrganizationId, string[] myLevelCodes)
        {
            string m_Sql = @"Select A.OrganizationID as OrganizationId,
                                A.LevelCode,
                                A.Name,
                                A.Type 
                                from system_Organization A, system_Organization B
                                where B.OrganizationID = '{0}'
                                and A.LevelCode like B.LevelCode + '%'
                                and A.LevelType = 'Factory'
                                {1}";

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
            m_Sql = string.Format(m_Sql, myCompanyOrganizationId, m_levelCodesParameter);
            try
            {
                DataTable result = _dataFactory.Query(m_Sql);
                return result;
            }
            catch
            {
                return null;
            }
        }

        public static string GetOrganizationInfo(string myFactoryOrganizationId)
        {
            string m_Sql = @"Select A.OrganizationID as OrganizationId,
                                A.LevelCode,
                                A.Name,
                                A.Type 
                                from system_Organization A, system_Organization B
                                where B.OrganizationID = '{0}'
                                and A.LevelCode like B.LevelCode + '%'
                                and B.LevelType = 'Factory'
                                and A.LevelType = 'ProductionLine'
                                order by A.LevelCode";
            m_Sql = string.Format(m_Sql, myFactoryOrganizationId);
            try
            {
                DataTable result = _dataFactory.Query(m_Sql);
                string m_ResultValue = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(result);
                return m_ResultValue;
            }
            catch
            {
                return "{\"rows\":[],\"total\":0}";
            }
        }
         * 
         * */
    }
}
