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
								and A.OrganizationID = {0}
								order by B.LevelCode";
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
                                and W.State = 0
                                and W.ENABLE = 1
                                and M.VariableId = Z.VariableId
                                order by M.OrganizationID";
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
                m_OrganizationType = string.Format(" and (C.Type in ({0}) or B.VariableId in ('cementPacking_ElectricityQuantity')) ", m_OrganizationType);
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
            string m_Type = "";
            string m_OrganizationID = "'" + myOrganizationId + "' as OrganizationID";
            string m_Sql = @"Select {5}, W.Name + Z.Name as MaterialName, M.VariableId, M.Value as DayMaterialWeight, N.Value as MonthMaterialWeight from 
                                (SELECT C.OrganizationID, B.VariableId, sum(B.TotalPeakValleyFlatB) as Value
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
	                                and B.VariableId = '{0}'
                                group by C.OrganizationID, B.VariableId) M,
                                (SELECT C.OrganizationID, B.VariableId, sum(B.TotalPeakValleyFlatB) as Value
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
	                                and B.VariableId = '{0}'
                                group by C.OrganizationID, B.VariableId) N, tz_Material W, material_MaterialDetail Z
                                where M.OrganizationID = N.OrganizationID
                                and M.VariableId = N.VariableId
                                and M.OrganizationID = W.OrganizationID
                                and W.KeyID = Z.KeyID
                                and W.Type = 2
                                and W.State = 0
                                and W.ENABLE = 1
                                and M.VariableId = Z.VariableId
                                order by M.OrganizationID";
            if (myOrganizationType != "分厂")
            {
                m_Type = string.Format(" and  C.Type = '{0}'", myOrganizationType);
                m_OrganizationID = "M.OrganizationID";
            }
            m_Sql = string.Format(m_Sql, myVariableId, myOrganizationId, m_Type, myStartTime, myEndTime, m_OrganizationID);
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
            DataView m_DataView = m_RunIndictorsDetailTable.DefaultView;
            m_DataView.Sort = "EquipmentName asc";
            DataTable m_RunIndictorsDetailTableOrder = m_DataView.ToTable();
            string m_ReturnString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_RunIndictorsDetailTableOrder);
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
            DataView m_DataView = m_RunIndictorsDetailTable.DefaultView;
            m_DataView.Sort = "EquipmentName asc";
            DataTable m_m_RunIndictorsDetailTableOrder = m_DataView.ToTable();
            string m_ReturnString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_m_RunIndictorsDetailTableOrder);
            return m_ReturnString;
        }
        public static string GetEquipmentHalt(string myEquipmentCommonIdList, string myFactoryOrganizationId, string myStatisticalRange, string myStartTime, string myEndTime)
        {
            string[] m_EquipmentCommonIdList = myEquipmentCommonIdList.Split(',');
            DataTable m_RunIndictorsDetailTable = RunIndicators.EquipmentHalt.GetEquipmentHalt(m_EquipmentCommonIdList, myFactoryOrganizationId, myStatisticalRange, myStartTime, myEndTime, _dataFactory);
            string m_ReturnString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_RunIndictorsDetailTable);
            return m_ReturnString;
        }
        public static List<string> GetInventoryData(string[] myMaterialList, string myOrganizationId, DataTable myWareHousingContrastTable, DateTime myTodayTime)
        {
            List<string> m_ReturnString = new List<string>();
            //string[] m_MaterialArray = myMaterialList.Split(',');
            DataTable m_CheckWarehouseTable = GetCheckWarehouseInfo(myMaterialList, myOrganizationId, myTodayTime);
            //DataTable m_WareHousingContrastTable = GetWareHousingContrast(m_MaterialArray,myOrganizationId);
            if (m_CheckWarehouseTable != null && myWareHousingContrastTable != null)
            {
                DataTable m_InventoryDataTable = GetInventoryDataByContrast(myOrganizationId, m_CheckWarehouseTable, myWareHousingContrastTable);
                if (m_InventoryDataTable != null)
                {
                    for (int i = 0; i < m_CheckWarehouseTable.Rows.Count; i++)
                    {
                        string m_ValueTemp = GetInventory(m_CheckWarehouseTable.Rows[i], m_InventoryDataTable);
                        if (m_ValueTemp != "")
                        {
                            string m_ElementTempString = string.Format("\"{0}\":{1}", m_CheckWarehouseTable.Rows[i]["ElementId"].ToString(), m_ValueTemp);
                            m_ReturnString.Add(m_ElementTempString);
                        }
                        // ;
                    }
                }
            }
            return m_ReturnString;
        }
        public static List<string> GetWareHousingData(string[] myMaterialList, string myOrganizationId, DataTable myWareHousingContrastTable, DateTime myTodayTime)
        {
            List<string> m_ReturnString = new List<string>();
            //string[] m_MaterialArray = myMaterialList.Split(',');
            DataTable m_WareHouseInfoTable = GetWareHouseInfo(myMaterialList, myOrganizationId, myTodayTime.ToString("yyyy-MM-dd 00:00:00"), myTodayTime.AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
            //DataTable m_WareHousingContrastTable = GetWareHousingContrast(m_MaterialArray, myOrganizationId);
            if (m_WareHouseInfoTable != null && myWareHousingContrastTable != null)
            {
                DataTable m_InventoryDataTable = GetInventoryDataByContrast(myOrganizationId, m_WareHouseInfoTable, myWareHousingContrastTable);
                if (m_InventoryDataTable != null)
                {
                    for (int i = 0; i < m_WareHouseInfoTable.Rows.Count; i++)
                    {
                        string m_ValueTemp = GetWareHousing(m_WareHouseInfoTable.Rows[i], m_InventoryDataTable, "Input");
                        if (m_ValueTemp != "")
                        {
                            string m_ElementTempString = string.Format("\"{0}\":{1}", m_WareHouseInfoTable.Rows[i]["MaterialId"].ToString() + "_" + "Input_Day", m_ValueTemp);
                            m_ReturnString.Add(m_ElementTempString);
                        }
                        m_ValueTemp = "";
                        m_ValueTemp = GetWareHousing(m_WareHouseInfoTable.Rows[i], m_InventoryDataTable, "Output");
                        if (m_ValueTemp != "")
                        {
                            string m_ElementTempString = string.Format("\"{0}\":{1}", m_WareHouseInfoTable.Rows[i]["MaterialId"].ToString() + "_" + "Output_Day", m_ValueTemp);
                            m_ReturnString.Add(m_ElementTempString);
                        }
                        // ;
                    }
                }
            }
            return m_ReturnString;
        }
        private static DataTable GetWareHouseInfo(string[] myMaterialArray, string myOrganizationId, string myStartTime, string myEndTime)
        {
            string m_Sql = @"SELECT A.Id as WarehouseId
                                  ,A.Code
                                  ,A.Name
                                  ,A.Type
                                  ,A.OrganizationID
                                  ,A.MaterialId
                                  ,A.Spec
                                  ,A.LevelCode
                                  ,convert(datetime,'{2}') as StartTime
                                  ,convert(datetime,'{3}') as EndTime
                              FROM inventory_Warehouse A
                              where A.Enabled = 1
                              and A.OrganizationID = '{0}'
                              and A.MaterialId in ({1})";
            string m_MaterialIdArray = "";
            for (int i = 0; i < myMaterialArray.Length; i++)
            {
                if (i == 0)
                {
                    m_MaterialIdArray = "'" + myMaterialArray[i] + "'";
                }
                else
                {
                    m_MaterialIdArray = m_MaterialIdArray + ",'" + myMaterialArray[i] + "'";
                }
            }
            m_Sql = string.Format(m_Sql, myOrganizationId, m_MaterialIdArray, myStartTime, myEndTime);
            try
            {
                DataTable m_WareHouseInfoTable = _dataFactory.Query(m_Sql);
                return m_WareHouseInfoTable;
            }
            catch
            {
                return null;
            }
        }
        private static DataTable GetCheckWarehouseInfo(string[] myMaterialArray, string myOrganizationId, DateTime myTodayTime)
        {
            string m_Sql = @"Select M.OrganizationID as OrganizationId, 
                                M.WarehouseId,
	                            M.MaterialId,
	                            M.ElementId + '_' + M.Type as ElementId,
	                            M.Type,
	                            M.Value as CheckWarehouseValue,
	                            M.StartTime,
                                M.EndTime
                            from 
	                            (Select C.OrganizationID, C.WarehouseId, C.MaterialId + '_Inventory' as ElementId, C.MaterialId, 'DayF' as Type,  D.Value, C.TimeStamp as StartTime, convert(datetime,'{2}') as EndTime
	                            from
	                                (Select B.OrganizationID, A.WarehouseId, MAX(A.TimeStamp) as TimeStamp, B.MaterialId from inventory_CheckWarehouse A, inventory_Warehouse B
	                                    where A.TimeStamp < '{2}'
			                            and A.WarehouseId = B.Id
			                            and B.MaterialId in ({1})
			                            and B.OrganizationID = '{0}'
	                                    group by B.OrganizationID, A.WarehouseId, B.MaterialId) C, inventory_CheckWarehouse D
	                                 where C.WarehouseId = D.WarehouseId
	                                    and C.TimeStamp = D.TimeStamp
	                            union all
	                            Select C.OrganizationID, C.WarehouseId, C.MaterialId + '_Inventory' as ElementId,C.MaterialId, 'DayL' as Type, D.Value, C.TimeStamp as StartTime, convert(datetime,'{3}') as EndTime
	                            from
	                                (Select B.OrganizationID, A.WarehouseId, MAX(A.TimeStamp) as TimeStamp, B.MaterialId from inventory_CheckWarehouse A, inventory_Warehouse B
	                                    where A.TimeStamp < '{3}'
			                            and A.WarehouseId = B.Id
			                            and B.MaterialId in ({1})
			                            and B.OrganizationID = '{0}'
	                                    group by B.OrganizationID, A.WarehouseId, B.MaterialId) C, inventory_CheckWarehouse D
	                                 where C.WarehouseId = D.WarehouseId
	                                    and C.TimeStamp = D.TimeStamp 
                                union all
                                Select C.OrganizationID, C.WarehouseId, C.MaterialId + '_Inventory' as ElementId, C.MaterialId, 'MonthF' as Type, D.Value, C.TimeStamp as StartTime, convert(datetime,'{4}') as EndTime
	                            from
	                                (Select B.OrganizationID, A.WarehouseId, MAX(A.TimeStamp) as TimeStamp, B.MaterialId from inventory_CheckWarehouse A, inventory_Warehouse B
	                                    where A.TimeStamp < '{4}'
			                            and A.WarehouseId = B.Id
			                            and B.MaterialId in ({1})
			                            and B.OrganizationID = '{0}'
	                                    group by B.OrganizationID, A.WarehouseId, B.MaterialId) C, inventory_CheckWarehouse D
	                                 where C.WarehouseId = D.WarehouseId
	                                    and C.TimeStamp = D.TimeStamp) M
                            order by M.OrganizationID, M.WarehouseId, M.MaterialId, M.Type, M.StartTime";
            string m_MaterialIdArray = "";
            for (int i = 0; i < myMaterialArray.Length; i++)
            {
                if (i == 0)
                {
                    m_MaterialIdArray = "'" + myMaterialArray[i] + "'";
                }
                else
                {
                    m_MaterialIdArray = m_MaterialIdArray + ",'" + myMaterialArray[i] + "'";
                }
            }
            m_Sql = string.Format(m_Sql, myOrganizationId, m_MaterialIdArray, myTodayTime.ToString("yyyy-MM-dd 00:00:00"), myTodayTime.AddDays(1).ToString("yyyy-MM-dd 00:00:00"), myTodayTime.ToString("yyyy-MM-01 00:00:00"));
            try
            {
                DataTable m_WareHousingContrastTable = _dataFactory.Query(m_Sql);
                return m_WareHousingContrastTable;
            }
            catch
            {
                return null;
            }
        }
        public static DataTable GetWareHousingContrast(string[] myMaterialArray, string myOrganizationId)
        {
            string m_Sql = @"SELECT A.ItemId
                                ,A.WarehouseId
	                            ,B.OrganizationID as OrganizationId
                                ,A.VariableId
                                ,A.Specs
                                ,A.DataBaseName + '.dbo.' + A.DataTableName as DataTableName
                                ,A.WarehousingType
                                ,A.Multiple
                                ,A.Offset
                                ,A.Editor
                                ,A.EditTime
                                ,A.Remark
                            FROM inventory_WareHousingContrast A, inventory_Warehouse B
                            where A.WarehouseId = B.Id
                            and B.OrganizationID = '{0}'
                            and B.MaterialId in ({1})
                            order by B.OrganizationID, B.Id, A.DataBaseName + '.dbo.' + A.DataTableName, A.WarehousingType";
            string m_MaterialIdArray = "";
            for (int i = 0; i < myMaterialArray.Length; i++)
            {
                if (i == 0)
                {
                    m_MaterialIdArray = "'" + myMaterialArray[i] + "'";
                }
                else
                {
                    m_MaterialIdArray = m_MaterialIdArray + ",'" + myMaterialArray[i] + "'";
                }
            }
            m_Sql = string.Format(m_Sql, myOrganizationId, m_MaterialIdArray);
            try
            {
                DataTable m_WareHousingContrastTable = _dataFactory.Query(m_Sql);
                return m_WareHousingContrastTable;
            }
            catch
            {
                return null;
            }
        }
        private static DataTable GetInventoryDataByContrast(string myOrganizationId, DataTable myCheckWarehouseTable, DataTable myWareHousingContrastTable)
        {
            DataTable m_InventoryDataTable = new DataTable();
            m_InventoryDataTable.Columns.Add("OrganizationId", typeof(string));
            m_InventoryDataTable.Columns.Add("WarehouseId", typeof(string));
            m_InventoryDataTable.Columns.Add("Value", typeof(decimal));
            m_InventoryDataTable.Columns.Add("VariableId", typeof(string));
            m_InventoryDataTable.Columns.Add("WarehousingType", typeof(string));
            m_InventoryDataTable.Columns.Add("Type", typeof(string));
            if (myCheckWarehouseTable != null && myWareHousingContrastTable != null)
            {
                for (int i = 0; i < myCheckWarehouseTable.Rows.Count; i++)
                {
                    DataRow[] m_WareHousingContrastRows = myWareHousingContrastTable.Select(string.Format("OrganizationId = '{0}' and WarehouseId = '{1}'", myOrganizationId, myCheckWarehouseTable.Rows[i]["WarehouseId"].ToString()));
                    string m_Type = myCheckWarehouseTable.Rows[i]["Type"].ToString();
                    string m_WarehouseId = myCheckWarehouseTable.Rows[i]["WarehouseId"].ToString();
                    DateTime m_StartTime = (DateTime)myCheckWarehouseTable.Rows[i]["StartTime"];
                    DateTime m_EndTime = (DateTime)myCheckWarehouseTable.Rows[i]["EndTime"];

                    string m_DCSDataBaseTable = "";

                    string m_VWB_WeightNYGL_Variables = "";
                    string m_HistoryDCSIncrement_Variables = "";
                    for (int j = 0; j < m_WareHousingContrastRows.Length; j++)
                    {
                        string m_DataTableNameTemp = m_WareHousingContrastRows[j]["DataTableName"].ToString();
                        if (m_DataTableNameTemp.Contains("NXJC.dbo.VWB_WeightNYGL"))
                        {
                            if (m_VWB_WeightNYGL_Variables == "")
                            {
                                m_VWB_WeightNYGL_Variables = "'" + m_WareHousingContrastRows[j]["VariableId"].ToString() + "'";
                            }
                            else
                            {
                                m_VWB_WeightNYGL_Variables = m_VWB_WeightNYGL_Variables + ",'" + m_WareHousingContrastRows[j]["VariableId"].ToString() + "'";
                            }
                        }
                        else if (m_DataTableNameTemp.Contains("dbo.HistoryDCSIncrement"))
                        {
                            m_DCSDataBaseTable = m_DataTableNameTemp;
                            if (m_HistoryDCSIncrement_Variables == "")
                            {
                                m_HistoryDCSIncrement_Variables = string.Format(" sum({0}) as {0} ", m_WareHousingContrastRows[j]["VariableId"].ToString());
                            }
                            else
                            {
                                m_HistoryDCSIncrement_Variables = m_HistoryDCSIncrement_Variables + "," + string.Format(" sum({0}) as {0} ", m_WareHousingContrastRows[j]["VariableId"].ToString());
                            }
                        }
                    }
                    if (m_VWB_WeightNYGL_Variables != "")
                    {
                        string m_Sql = @"Select A.OrganizationID,
		                                            A.VariableId,
		                                            sum(A.Value) as Value
			                                    from NXJC.dbo.VWB_WeightNYGL A
			                                    where A.StatisticalTime >= '{2}'
				                                    and A.StatisticalTime < '{3}'
				                                    and A.OrganizationID = '{0}'
				                                    and A.VariableId in ({1}) 
			                                    group by A.OrganizationID, A.VariableId";
                        m_Sql = string.Format(m_Sql, myOrganizationId, m_VWB_WeightNYGL_Variables, m_StartTime.ToString("yyyy-MM-dd HH:mm:ss"), m_EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        DataTable m_InventoryDataTableTemp = _dataFactory.Query(m_Sql);
                        if (m_InventoryDataTableTemp != null)
                        {
                            for (int m = 0; m < m_InventoryDataTableTemp.Rows.Count; m++)
                            {
                                for (int n = 0; n < m_WareHousingContrastRows.Length; n++)
                                {
                                    if (m_InventoryDataTableTemp.Rows[m]["VariableId"].ToString() == m_WareHousingContrastRows[n]["VariableId"].ToString())
                                    {
                                        m_InventoryDataTable.Rows.Add(new object[]
                                              {
                                              myOrganizationId, 
                                              m_WareHousingContrastRows[n]["WarehouseId"].ToString(),
                                              (decimal)m_InventoryDataTableTemp.Rows[m]["Value"],
                                              m_WareHousingContrastRows[n]["VariableId"].ToString(),
                                              m_WareHousingContrastRows[n]["WarehousingType"].ToString(),
                                              m_Type
                                              });
                                    }
                                }

                            }

                        }
                    }
                    if (m_HistoryDCSIncrement_Variables != "")
                    {
                        string m_Sql = @"Select {1}
			                                    from {0} A
			                                    where A.vDate >= '{2}'
				                                    and A.vDate < '{3}'";
                        m_Sql = string.Format(m_Sql, m_DCSDataBaseTable, m_HistoryDCSIncrement_Variables, m_StartTime.ToString("yyyy-MM-dd HH:mm:ss"), m_EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        DataTable m_InventoryDataTableTemp = _dataFactory.Query(m_Sql);
                        if (m_InventoryDataTableTemp != null)
                        {
                            for (int m = 0; m < m_InventoryDataTableTemp.Columns.Count; m++)
                            {
                                for (int n = 0; n < m_WareHousingContrastRows.Length; n++)
                                {
                                    if (m_InventoryDataTableTemp.Columns[m].ColumnName == m_WareHousingContrastRows[n]["VariableId"].ToString())
                                    {
                                        m_InventoryDataTable.Rows.Add(new object[]
                                              {
                                              myOrganizationId, 
                                              m_WareHousingContrastRows[n]["WarehouseId"].ToString(),
                                              (decimal)m_InventoryDataTableTemp.Rows[0][m_InventoryDataTableTemp.Columns[m].ColumnName],
                                              m_WareHousingContrastRows[n]["VariableId"].ToString(),
                                              m_WareHousingContrastRows[n]["WarehousingType"].ToString(),
                                              m_Type
                                              });
                                    }
                                }
                            }

                        }
                    }
                }
            }
            return m_InventoryDataTable;
        }
        private static string GetInventory(DataRow myCheckWarehouseTableRow, DataTable myInventoryDataTable)
        {
            object m_InputValue = myInventoryDataTable.Compute("sum(Value)", string.Format("OrganizationId = '{0}' and WarehouseId = '{1}' and WarehousingType = '{2}' and Type = '{3}'",
                                                     myCheckWarehouseTableRow["OrganizationId"].ToString()
                                                     , myCheckWarehouseTableRow["WarehouseId"].ToString()
                                                     ,"Input"
                                                     ,myCheckWarehouseTableRow["Type"].ToString()));
            object m_OutputValue = myInventoryDataTable.Compute("sum(Value)", string.Format("OrganizationId = '{0}' and WarehouseId = '{1}' and WarehousingType = '{2}' and Type = '{3}'",
                                                     myCheckWarehouseTableRow["OrganizationId"].ToString()
                                                     , myCheckWarehouseTableRow["WarehouseId"].ToString()
                                                     , "Output"
                                                     , myCheckWarehouseTableRow["Type"].ToString()));
            decimal m_ReturnValue = (decimal)myCheckWarehouseTableRow["CheckWarehouseValue"] + (m_InputValue != DBNull.Value ? (decimal)m_InputValue : 0.0m) - (m_OutputValue != DBNull.Value ? (decimal)m_OutputValue : 0.0m);
            string m_ReturnString = m_ReturnValue.ToString("0.00");

            return m_ReturnString;
        }
        private static string GetWareHousing(DataRow myWareHouseInfoTableRow, DataTable myInventoryDataTable, string myType)
        {
            object m_ObjectValue = myInventoryDataTable.Compute("sum(Value)", string.Format("OrganizationId = '{0}' and WarehouseId = '{1}' and WarehousingType = '{2}' and Type = '{3}'",
                                                     myWareHouseInfoTableRow["OrganizationId"].ToString()
                                                     , myWareHouseInfoTableRow["WarehouseId"].ToString()
                                                     , myType
                                                     , myWareHouseInfoTableRow["Type"].ToString()));
            decimal m_ReturnValue = m_ObjectValue != DBNull.Value ? (decimal)m_ObjectValue : 0.0m;
            string m_ReturnString = m_ReturnValue.ToString("0.00");

            return m_ReturnString;
        }
        //右边区域的数据查询
        public static string GetProductSaleData(string myOrganizationId, string myMaterialIds, DateTime myDatetime)
        {
            if (myMaterialIds != "" && myOrganizationId != "")
            {
                string[] m_MaterialIds = myMaterialIds.Split(',');
                string[] m_RowNameArray = new string[] { "日销售量(t)", "月销售量(t)", "年销售量(t)", "月计划完成率", "年计划完成率" };
                string[] m_MonthName = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
                string m_ValueVariable = "";
                string m_PlanVariable = "";

                string m_Sql = @"Select C.VariableId, 
                                    A.DayValue, 
                                    B.MonthValue, 
                                    C.YearValue, 
                                    case when D.MonthPlanValue > 0 then B.MonthValue / D.MonthPlanValue else D.MonthPlanValue end as MonthCompletionRate,
                                    case when D.YearPlanValue > 0 then C.YearValue / D.YearPlanValue else D.YearPlanValue end as YearCompletionRate
                                from
                                    (Select M.ConstrastVariableId as VariableId, sum(CASE WHEN M.sales_gblx = 'DE' THEN M.Value WHEN M.sales_gblx = 'RD' THEN -M.Value end) as YearValue from 
                                        VWB_WeightNYGL M
                                        where M.StatisticalTime >= '{8}'
                                        and M.StatisticalTime < '{9}'
                                        and M.ConstrastVariableId in ({0})
                                        and M.OrganizationID = '{2}'
		                                and M.Type = 3
                                    group by M.ConstrastVariableId) C
                                    left join 
                                    (Select substring(N.QuotasID,1, len(N.QuotasID) - 5) as VariableId, N.{10} as MonthPlanValue, N.Totals as YearPlanValue from tz_Plan M, plan_PurchaseSalesYearlyPlan N
                                        where M.OrganizationID = '{2}' and M.Date = '{3}' and M.PlanType = '{11}' and M.KeyID = N.KeyID and N.QuotasID in ({1})) D on C.VariableId = D.VariableId
                                    left join 
                                    (Select M.ConstrastVariableId as VariableId, sum(CASE WHEN M.sales_gblx = 'DE' THEN M.Value WHEN M.sales_gblx = 'RD' THEN -M.Value end) as DayValue from 
                                        VWB_WeightNYGL M
                                        where M.StatisticalTime >= '{4}'
                                        and M.StatisticalTime < '{5}'
                                        and M.ConstrastVariableId in ({0})
                                        and M.OrganizationID = '{2}'
		                                and M.Type = 3
                                    group by M.ConstrastVariableId) A on A.VariableId = C.VariableId
                                    left join
                                    (Select M.ConstrastVariableId as VariableId, sum(CASE WHEN M.sales_gblx = 'DE' THEN M.Value WHEN M.sales_gblx = 'RD' THEN -M.Value end) as MonthValue from 
                                        VWB_WeightNYGL M
                                        where M.StatisticalTime >= '{6}'
                                        and M.StatisticalTime < '{7}'
                                        and M.ConstrastVariableId in ({0})
                                        and M.OrganizationID = '{2}'
		                                and M.Type = 3
                                    group by M.ConstrastVariableId) B on B.VariableId = C.VariableId";
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
                m_Sql = m_Sql.Replace("{2}", myOrganizationId);
                m_Sql = m_Sql.Replace("{3}", myDatetime.Year.ToString());
                m_Sql = m_Sql.Replace("{4}", myDatetime.ToString("yyyy-MM-dd 00:00:00"));
                m_Sql = m_Sql.Replace("{5}", myDatetime.AddDays(1).ToString("yyyy-MM-dd 00:00:00"));
                m_Sql = m_Sql.Replace("{6}", myDatetime.ToString("yyyy-MM-01 00:00:00"));
                m_Sql = m_Sql.Replace("{7}", myDatetime.AddMonths(1).ToString("yyyy-MM-01 00:00:00"));
                m_Sql = m_Sql.Replace("{8}", myDatetime.ToString("yyyy-01-01 00:00:00"));
                m_Sql = m_Sql.Replace("{9}", myDatetime.AddYears(1).ToString("yyyy-01-01 00:00:00"));
                m_Sql = m_Sql.Replace("{10}", m_MonthName[myDatetime.Month - 1]);
                m_Sql = m_Sql.Replace("{11}", "PurchaseSales");
                //string m_DayStartTime = myDatetime.ToString("yyyy-MM-dd 00:00:00");
                //string m_DayEndTime = myDatetime.AddDays(1).ToString("yyyy-MM-dd 00:00:00");
                //string m_MonthStartTime = myDatetime.ToString("yyyy-MM-01 00:00:00");
                //string m_MonthEndTime = myDatetime.AddMonths(1).ToString("yyyy-MM-01 00:00:00");
                //string m_YearStartTime = myDatetime.ToString("yyyy-01-01 00:00:00");
                //string m_YearEndTime = myDatetime.AddYears(1).ToString("yyyy-01-01 00:00:00");
                try
                {
                    DataTable m_ProductSaleDataTable = _dataFactory.Query(m_Sql);
                    if (m_ProductSaleDataTable != null)
                    {
                        DataTable m_ResultTable = new DataTable();
                        m_ResultTable.Columns.Add("Name", typeof(string));
                        for (int i = 0; i < m_MaterialIds.Length; i++)
                        {
                            m_ResultTable.Columns.Add(m_MaterialIds[i], typeof(decimal));
                        }
                        for (int i = 1; i < m_ProductSaleDataTable.Columns.Count - 1; i++)
                        {
                            DataRow m_NewDataRowTemp = m_ResultTable.NewRow();
                            m_NewDataRowTemp["Name"] = m_RowNameArray[i - 1];
                            for (int j = 0; j < m_MaterialIds.Length; j++)
                            {
                                m_NewDataRowTemp[m_MaterialIds[j]] = 0.0m;
                            }
                            for (int j = 0; j < m_ProductSaleDataTable.Rows.Count; j++)
                            {
                                m_NewDataRowTemp[m_ProductSaleDataTable.Rows[j]["VariableId"].ToString()] = m_ProductSaleDataTable.Rows[j][i] != DBNull.Value ? m_ProductSaleDataTable.Rows[j][i] : 0.0m;
                            }
                            m_ResultTable.Rows.Add(m_NewDataRowTemp);
                        }
                        string m_ReturnString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_ResultTable);
                        return m_ReturnString;
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
            else
            {
                return "{\"rows\":[],\"total\"0}";
            }
        }
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
        public static string GetWorkingTeamShiftLog(string myOrganizationId, string myStartTime)
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
                              and A.ShiftDate <= '{1}'
                              order by A.ShiftDate desc";
            m_Sql = string.Format(m_Sql, myOrganizationId, myStartTime);
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
            DataTable m_auxiliaryProductionElectricitiyQuantity = GetAuxiliaryProductionElectricitiyQuantity(myOrganizationId, myOrganizationTypeList, myStartMonth, myEndMonth);
            if (m_auxiliaryProductionElectricitiyQuantity != null)  //分步电耗均摊辅助用电
            {
                for (int i = 0; i < m_auxiliaryProductionElectricitiyQuantity.Rows.Count; i++)
                {
                    string m_QueryCondition = string.Format("ValueType = '{0}' and TimeStamp = '{1}'", m_auxiliaryProductionElectricitiyQuantity.Rows[i]["ValueType"].ToString(), m_auxiliaryProductionElectricitiyQuantity.Rows[i]["TimeStamp"].ToString());
                    DataRow[] m_ShareProcessRow = m_SourceData.Select(m_QueryCondition);
                    decimal m_auxiliaryProductionElectricitiyQuantityData = m_auxiliaryProductionElectricitiyQuantity.Rows[i]["Value"] != DBNull.Value ? (decimal)m_auxiliaryProductionElectricitiyQuantity.Rows[i]["Value"] : 0.0m;
                    decimal m_TotalElectricityQuantity = 0.0m;
                    decimal m_SharedElectricityQuantity = 0.0m;
                    for (int j = 0; j < m_ShareProcessRow.Length; j++)          //计算电量总和
                    {
                        decimal m_ElectricityTemp = 0.0m;
                        if (m_ShareProcessRow[j]["Value"] == DBNull.Value)
                        {
                            m_ShareProcessRow[j]["Value"] = 0.0m;
                        }
                        else
                        {
                            m_ElectricityTemp = (decimal)m_ShareProcessRow[j]["Value"];
                        }
                        m_TotalElectricityQuantity = m_TotalElectricityQuantity + m_ElectricityTemp;
                    }
                    for (int j = 0; j < m_ShareProcessRow.Length; j++)          //计算电量总和
                    {
                        if (j < m_ShareProcessRow.Length - 1)
                        {
                            if (m_TotalElectricityQuantity > 0.0m)          //当用电总和大于0,按比例均摊辅助电量
                            {
                                m_SharedElectricityQuantity = m_SharedElectricityQuantity + m_auxiliaryProductionElectricitiyQuantityData * (decimal)m_ShareProcessRow[j]["Value"] / m_TotalElectricityQuantity;
                                m_ShareProcessRow[j]["Value"] = (decimal)m_ShareProcessRow[j]["Value"] + m_auxiliaryProductionElectricitiyQuantityData * (decimal)m_ShareProcessRow[j]["Value"] / m_TotalElectricityQuantity;                             
                            }
                            else                                            //当用电总和等于0,平分辅助电量
                            {
                                m_SharedElectricityQuantity = m_SharedElectricityQuantity + m_auxiliaryProductionElectricitiyQuantityData / m_ShareProcessRow.Length;
                                m_ShareProcessRow[j]["Value"] = m_auxiliaryProductionElectricitiyQuantityData / m_ShareProcessRow.Length;
                            }
                            
                        }
                        else            //当最后一行时,应该是总量减去已分配的量
                        {
                            m_ShareProcessRow[j]["Value"] = (decimal)m_ShareProcessRow[j]["Value"] + m_auxiliaryProductionElectricitiyQuantityData - m_SharedElectricityQuantity;
                        }
                    }
                }
            }
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
        private static DataTable GetAuxiliaryProductionElectricitiyQuantity(string myOrganizationId, string myOrganizationTypeList, string myStartMonth, string myEndMonth)
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
            string m_Sql = @"Select M.TimeStamp, M.VariableId, M.Value, M.ValueType from
                                (Select substring('{0}',0, 8) as TimeStamp, B.VariableId, B.ValueType, sum(B.TotalPeakValleyFlatB) as Value from tz_Balance A, balance_Energy B, system_Organization C, system_Organization D
                                where A.TimeStamp >='{0}'
                                and A.TimeStamp <='{1}'
                                and A.StaticsCycle = 'day'
                                and A.OrganizationID = '{4}'
                                and A.BalanceId = B.KeyId
                                and B.ValueType = 'ElectricityQuantity'
                                and B.VariableId = '{6}'
                                and D.OrganizationID = A.OrganizationID
                                and C.LevelCode like D.LevelCode + '%'
                                and C.Type in ({5})
                                and B.OrganizationID = C.OrganizationID
                                group by B.VariableId, B.ValueType
                             union all
                                Select A.TimeStamp, B.VariableId, B.ValueType, sum(B.TotalPeakValleyFlatB) as Value from tz_Balance A, balance_Energy B, system_Organization C, system_Organization D
                                where A.TimeStamp >='{2}'
                                and A.TimeStamp <='{3}'
                                and A.StaticsCycle = 'month'
                                and A.OrganizationID = '{4}'
                                and A.BalanceId = B.KeyId
                                and B.ValueType = 'ElectricityQuantity'
                                and B.VariableId = '{6}'
                                and D.OrganizationID = A.OrganizationID
                                and C.LevelCode like D.LevelCode + '%'
                                and C.Type in ({5})
                                and B.OrganizationID = C.OrganizationID
                                group by A.TimeStamp, B.VariableId, B.ValueType) M
                                order by M.VariableId, M.TimeStamp";
            m_Sql = string.Format(m_Sql, myEndMonth + "-01", m_EndTime.ToString("yyyy-MM-dd"), myStartMonth, m_LastMonth.ToString("yyyy-MM"), myOrganizationId, m_OrganizationType, "auxiliaryProduction_ElectricityQuantity");
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
            string m_Sql = @"Select M.TimeStamp, M.VariableId, M.Value,M.ValueType from
                                (Select substring('{0}',0, 8) as TimeStamp, B.VariableId, B.ValueType, sum(B.TotalPeakValleyFlatB) as Value from tz_Balance A, balance_Energy B, system_Organization C, system_Organization D
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
                                group by B.VariableId,B.ValueType
                             union all
                                Select A.TimeStamp, B.VariableId, B.ValueType, sum(B.TotalPeakValleyFlatB) as Value from tz_Balance A, balance_Energy B, system_Organization C, system_Organization D
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
                                group by A.TimeStamp, B.VariableId, B.ValueType) M
                                order by M.VariableId, M.TimeStamp";
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
        public static string GetQuickContent(string myGroupKey, string myUserId, string myRoleId)
        {
            string m_ReturnValue = "{\"rows\":[],\"total\":0}";
            DataTable m_QuickContentTable = GetQuickContentTable(myGroupKey, myUserId, myRoleId);
            DataTable m_AllContentTable = GetAllContentTable();
            if (m_QuickContentTable != null && m_AllContentTable != null)
            {
                for (int i = 0; i < m_QuickContentTable.Rows.Count; i++)
                {
                    string m_NodePath = m_QuickContentTable.Rows[i]["Name"].ToString();
                    string m_ParentNodeId = m_QuickContentTable.Rows[i]["ParentNodeId"].ToString();
                    while (m_ParentNodeId != "0")
                    {
                        DataRow[] m_DataRowTemp = m_AllContentTable.Select(string.Format("NodeId = '{0}'", m_ParentNodeId));
                        if (m_DataRowTemp.Length == 0)      //如果没有找到直接结束
                        {
                            m_ParentNodeId = "0";
                        }
                        else
                        {
                            m_ParentNodeId = m_DataRowTemp[0]["ParentNodeId"].ToString();
                            m_NodePath = m_DataRowTemp[0]["NodeContext"].ToString() + ">>" + m_NodePath;
                        }
                    }
                    m_QuickContentTable.Rows[i]["NodePath"] = m_NodePath;
                }
                m_ReturnValue = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_QuickContentTable);
            }
            return m_ReturnValue;
        }
        private static DataTable GetQuickContentTable(string myGroupKey, string myUserId, string myRoleId)
        {
            string m_Sql = @"Select A.NODE_ID as PageId, 
			                        case when A.NODE_NAME is null or A.NODE_NAME = '' then B.NODE_CONTEXT else B.NODE_CONTEXT + '(' + A.NODE_NAME + ')' end as Name,
                                    A.REMARK as [Description],
			                        B.NAVIGATE_URL as NavigateUrl,
                                    B.PARENT_NODE_ID as ParentNodeId, 
			                        case when B.ICON_PATH is null then '' else B.ICON_PATH end as IconPath,
			                        '' as NodePath
	                         from IndustryEnergy_SH.dbo.content_quick A, IndustryEnergy_SH.dbo.content B, IndustryEnergy_SH.dbo.page_role C
	                         where A.NODE_ID = B.NODE_ID
	                         and A.CONTENT_TYPE = '{0}'
	                         and A.GROUP_KEY = '{1}'
	                         and (A.USER_ID is null or A.USER_ID = '{2}')
	                         and C.ROLE_ID = '{3}'
	                         and A.NODE_ID = C.PAGE_ID
	                         order by A.DISPLAY_INDEX";
            m_Sql = string.Format(m_Sql, "OverView", myGroupKey, myUserId, myRoleId);
            try
            {
                DataTable m_QuickContentTableTable = _dataFactory.Query(m_Sql);
                return m_QuickContentTableTable;

            }
            catch
            {
                return null;
            }
        }
        private static DataTable GetAllContentTable()
        {
            string m_Sql = @"SELECT A.NODE_ID as NodeId,
                                   A.NODE_CONTEXT as NodeContext,
                                   A.PARENT_NODE_ID as ParentNodeId
                              FROM IndustryEnergy_SH.dbo.content A
                              order by A.PARENT_NODE_ID, A.NODE_INDEX";
            try
            {
                DataTable m_AllContentTableTable = _dataFactory.Query(m_Sql);
                return m_AllContentTableTable;
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
