using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Services;
using WebStyleBaseForEnergy;
namespace Monitor_OverView.Web.UI_OverView
{
    public partial class View_OverView_Factory : WebStyleBaseForEnergy.webToolsStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_qtx", "zc_nxjc_byc" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#endif
                string m_OrganizationId = Request.QueryString["id"] != null ? Request.QueryString["id"].ToString() : "";
                if (m_OrganizationId == "")
                {
                    HiddenField_ComfromGlobalPage.Value = "False";
                    m_OrganizationId = Monitor_OverView.Service.OverView.OverView_Nxjc.GetStationId();
                }
                else
                {
                    HiddenField_ComfromGlobalPage.Value = "True";
                }
                GetFactoryStationList(m_OrganizationId);
            }
        }
        private void GetFactoryStationList(string myOrganizationId)
        {
            HiddenField_StationOrganizationIds.Value = Monitor_OverView.Service.OverView.OverView_Factory.GetFactoryStationList(myOrganizationId, GetDataValidIdGroup("ProductionOrganization"));
        }
        [WebMethod]
        public static string GetElectricityQuantitiyDetail(string myVariableId, string myOrganizationId, string myOrganizationType)
        {
            string m_ReturnJson = "{\"rows\":[],\"total\":0}";
            if (myOrganizationId != "")
            {
                DataTable m_ElectricityQuantityTable = null;
                m_ReturnJson = Monitor_OverView.Service.OverView.OverView_Factory.GetElectricityQuantitiyDetail(myVariableId, myOrganizationId, myOrganizationType, DateTime.Now.AddDays(-1).ToString("yyyy-MM") + "-01", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), ref m_ElectricityQuantityTable);
            }
            return m_ReturnJson;
        }
        [WebMethod]
        public static string GetElectricityQuantitiy(string myVariableIdList, string myOrganizationId, string myOrganizationType)
        {
            string m_ReturnJson = "{\"rows\":[],\"total\":0}";
            if (myOrganizationId != "")
            {
                DataTable m_ElectricityQuantityTable = null;
                m_ReturnJson = Monitor_OverView.Service.OverView.OverView_Factory.GetElectricityQuantitiy(myVariableIdList, myOrganizationId, myOrganizationType, DateTime.Now.AddDays(-1).ToString("yyyy-MM") + "-01", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), ref m_ElectricityQuantityTable);
            }
            return m_ReturnJson;
        }

        [WebMethod]
        public static string GetElectricityConsumptionDetail(string myVariableId, string myOutputVariableId, string myOrganizationId, string myOrganizationType)
        {
            string m_ReturnJson = "{\"rows\":[],\"total\":0}";
            if (myOrganizationId != "")
            {
                DataTable m_ElectricityQuantityTable = null;
                DataTable m_MartieialWeightTable = null;
                Monitor_OverView.Service.OverView.OverView_Factory.GetElectricityQuantitiyDetail(myVariableId, myOrganizationId, myOrganizationType, DateTime.Now.AddDays(-1).ToString("yyyy-MM") + "-01", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), ref m_ElectricityQuantityTable);
                Monitor_OverView.Service.OverView.OverView_Factory.GetMaterialWeightDetail(myOutputVariableId, myOrganizationId, myOrganizationType, DateTime.Now.AddDays(-1).ToString("yyyy-MM") + "-01", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), ref m_MartieialWeightTable);
                if (m_ElectricityQuantityTable != null && m_MartieialWeightTable != null)
                {
                    m_ElectricityQuantityTable.Columns.Add(m_MartieialWeightTable.Columns["DayMaterialWeight"].ToString(), m_MartieialWeightTable.Columns["DayMaterialWeight"].DataType);
                    m_ElectricityQuantityTable.Columns.Add(m_MartieialWeightTable.Columns["MonthMaterialWeight"].ToString(), m_MartieialWeightTable.Columns["MonthMaterialWeight"].DataType);
                    m_ElectricityQuantityTable.Columns.Add("DayElectricityConsumption", m_MartieialWeightTable.Columns["DayMaterialWeight"].DataType);
                    m_ElectricityQuantityTable.Columns.Add("MonthElectricityConsumption", m_MartieialWeightTable.Columns["DayMaterialWeight"].DataType);
                    for (int i = 0; i < m_ElectricityQuantityTable.Rows.Count; i++)
                    {
                        for (int j = 0; j < m_MartieialWeightTable.Rows.Count; j++)
                        {
                            if (m_ElectricityQuantityTable.Rows[i]["OrganizationID"].ToString() == m_MartieialWeightTable.Rows[j]["OrganizationID"].ToString())
                            {
                                decimal m_DayMaterialWeight = (decimal)m_MartieialWeightTable.Rows[j]["DayMaterialWeight"];
                                decimal m_MonthMaterialWeight = (decimal)m_MartieialWeightTable.Rows[j]["MonthMaterialWeight"];
                                m_ElectricityQuantityTable.Rows[i]["DayMaterialWeight"] = m_MartieialWeightTable.Rows[j]["DayMaterialWeight"];
                                m_ElectricityQuantityTable.Rows[i]["MonthMaterialWeight"] = m_MartieialWeightTable.Rows[j]["MonthMaterialWeight"];
                                m_ElectricityQuantityTable.Rows[i]["DayElectricityConsumption"] = m_DayMaterialWeight > 0 ? (decimal)m_ElectricityQuantityTable.Rows[i]["DayElectricityQuantity"] / m_DayMaterialWeight : 0;
                                m_ElectricityQuantityTable.Rows[i]["MonthElectricityConsumption"] = m_MonthMaterialWeight > 0 ? (decimal)m_ElectricityQuantityTable.Rows[i]["MonthElectricityQuantity"] / m_MonthMaterialWeight : 0;
                            }
                        }
                    }
                    m_ReturnJson = Monitor_OverView.Service.OverView.OverView_Factory.GetElectricityConsumptionDetailData(m_ElectricityQuantityTable);
                }

            }
            return m_ReturnJson;
        }

        /// <summary>
        /// 专为余热发电计算吨熟料发电量
        /// </summary>
        /// <param name="myVariableId"></param>
        /// <param name="myOutputVariableId"></param>
        /// <param name="myOrganizationId"></param>
        /// <param name="myOrganizationType"></param>
        /// <returns></returns>
        [WebMethod]
        public static string GetElectricityConsumptionDetailYR(string myVariableId, string myOutputVariableId, string myOrganizationId, string myOrganizationType)
        {
            string m_ReturnJson = "{\"rows\":[],\"total\":0}";
            if (myOrganizationId != "")
            {
                DataTable m_OrganizationIdYR = Monitor_OverView.Service.OverView.OverView_Factory.GetOrganizationIdByClinckerContrast(myOrganizationId, myOrganizationType);
                DataTable m_ElectricityQuantityTable = null;
                DataTable m_MartieialWeightTable = null;
                Monitor_OverView.Service.OverView.OverView_Factory.GetElectricityQuantitiyDetail(myVariableId, myOrganizationId, myOrganizationType, DateTime.Now.AddDays(-1).ToString("yyyy-MM") + "-01", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), ref m_ElectricityQuantityTable);
                Monitor_OverView.Service.OverView.OverView_Factory.GetMaterialWeightDetail(myOutputVariableId, myOrganizationId, "熟料", DateTime.Now.AddDays(-1).ToString("yyyy-MM") + "-01", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), ref m_MartieialWeightTable);


                if (m_ElectricityQuantityTable != null && m_MartieialWeightTable != null && m_OrganizationIdYR != null)
                {
                    m_ElectricityQuantityTable.Columns.Add(m_MartieialWeightTable.Columns["DayMaterialWeight"].ToString(), m_MartieialWeightTable.Columns["DayMaterialWeight"].DataType);
                    m_ElectricityQuantityTable.Columns.Add(m_MartieialWeightTable.Columns["MonthMaterialWeight"].ToString(), m_MartieialWeightTable.Columns["MonthMaterialWeight"].DataType);
                    m_ElectricityQuantityTable.Columns.Add("DayElectricityConsumption", m_MartieialWeightTable.Columns["DayMaterialWeight"].DataType);
                    m_ElectricityQuantityTable.Columns.Add("MonthElectricityConsumption", m_MartieialWeightTable.Columns["DayMaterialWeight"].DataType);
                    DataTable m_MartieialWeightTableYR = m_MartieialWeightTable.Clone();
                    for (int i = 0; i < m_OrganizationIdYR.Rows.Count; i++)          //根据余热发电的组织机构ID,找到对应的熟料
                    {
                        decimal m_DayMartieralWeightValue = 0.0m;
                        decimal m_MonthMartieralWeightValue = 0.0m;
                        string[] m_OrganizationIdList = m_OrganizationIdYR.Rows[i]["ClinkerOrganizationID"].ToString().Split(',');
                        for (int j = 0; j < m_OrganizationIdList.Length; j++)
                        {
                            for (int z = 0; z < m_MartieialWeightTable.Rows.Count; z++)
                            {
                                if (m_OrganizationIdList[j] == m_MartieialWeightTable.Rows[z]["OrganizationID"].ToString())    //如果对应的熟料组织机构相同则累加
                                {
                                    m_DayMartieralWeightValue = m_DayMartieralWeightValue + (decimal)m_MartieialWeightTable.Rows[z]["DayMaterialWeight"];
                                    m_MonthMartieralWeightValue = m_MonthMartieralWeightValue + (decimal)m_MartieialWeightTable.Rows[z]["MonthMaterialWeight"];
                                    break;
                                }
                            }
                        }
                        m_MartieialWeightTableYR.Rows.Add(m_OrganizationIdYR.Rows[i]["OrganizationID"].ToString(), "", myOutputVariableId, m_DayMartieralWeightValue, m_MonthMartieralWeightValue);
                    }

                    for (int i = 0; i < m_ElectricityQuantityTable.Rows.Count; i++)
                    {
                        for (int j = 0; j < m_MartieialWeightTableYR.Rows.Count; j++)
                        {
                            if (m_ElectricityQuantityTable.Rows[i]["OrganizationID"].ToString() == m_MartieialWeightTableYR.Rows[j]["OrganizationID"].ToString())
                            {
                                decimal m_DayMaterialWeight = (decimal)m_MartieialWeightTableYR.Rows[j]["DayMaterialWeight"];
                                decimal m_MonthMaterialWeight = (decimal)m_MartieialWeightTableYR.Rows[j]["MonthMaterialWeight"];
                                m_ElectricityQuantityTable.Rows[i]["DayMaterialWeight"] = m_MartieialWeightTableYR.Rows[j]["DayMaterialWeight"];
                                m_ElectricityQuantityTable.Rows[i]["MonthMaterialWeight"] = m_MartieialWeightTableYR.Rows[j]["MonthMaterialWeight"];
                                m_ElectricityQuantityTable.Rows[i]["DayElectricityConsumption"] = m_DayMaterialWeight > 0 ? (decimal)m_ElectricityQuantityTable.Rows[i]["DayElectricityQuantity"] / m_DayMaterialWeight : 0;
                                m_ElectricityQuantityTable.Rows[i]["MonthElectricityConsumption"] = m_MonthMaterialWeight > 0 ? (decimal)m_ElectricityQuantityTable.Rows[i]["MonthElectricityQuantity"] / m_MonthMaterialWeight : 0;
                            }
                        }
                    }
                    m_ReturnJson = Monitor_OverView.Service.OverView.OverView_Factory.GetElectricityConsumptionDetailData(m_ElectricityQuantityTable);
                }

            }
            return m_ReturnJson;
        }
        [WebMethod]
        public static string GetElectricityConsumption(string myVariableIdList, string myOutputVariableIdList, string myOrganizationId, string myOrganizationType)
        {
            string m_ReturnJson = "{\"rows\":[],\"total\":0}";
            if (myOrganizationId != "")
            {
                DataTable m_ElectricityConsumptionTable = new DataTable();
                m_ElectricityConsumptionTable.Columns.Add("BalanceVariableId", typeof(string));
                m_ElectricityConsumptionTable.Columns.Add("DayElectricityConsumption", typeof(decimal));
                m_ElectricityConsumptionTable.Columns.Add("MonthElectricityConsumption", typeof(decimal));

                DataTable m_ElectricityQuantityTable = null;
                DataTable m_MartieialWeightTable = null;
                Monitor_OverView.Service.OverView.OverView_Factory.GetElectricityQuantitiy(myVariableIdList, myOrganizationId, myOrganizationType, DateTime.Now.AddDays(-1).ToString("yyyy-MM") + "-01", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), ref m_ElectricityQuantityTable);
                Monitor_OverView.Service.OverView.OverView_Factory.GetMaterialWeight(myOutputVariableIdList, myOrganizationId, myOrganizationType, DateTime.Now.AddDays(-1).ToString("yyyy-MM") + "-01", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), ref m_MartieialWeightTable);
                if (m_ElectricityQuantityTable != null && m_MartieialWeightTable != null)
                {

                    string[] m_VariableIdListGroup = myVariableIdList.Split(',');
                    string[] m_OutputVariableIdListGroup = myOutputVariableIdList.Split(',');
                    for (int i = 0; i < m_VariableIdListGroup.Length; i++)
                    {
                        decimal m_DayMaterialWeight = 0.0m;
                        decimal m_MonthMaterialWeight = 0.0m;
                        for (int j = 0; j < m_MartieialWeightTable.Rows.Count; j++)
                        {
                            if (m_MartieialWeightTable.Rows[j]["VariableId"].ToString() == m_OutputVariableIdListGroup[i])
                            {
                                m_DayMaterialWeight = (decimal)m_MartieialWeightTable.Rows[j]["DayMaterialWeight"];
                                m_MonthMaterialWeight = (decimal)m_MartieialWeightTable.Rows[j]["MonthMaterialWeight"];
                                break;
                            }
                        }
                        for (int j = 0; j < m_ElectricityQuantityTable.Rows.Count; j++)
                        {
                            if (m_ElectricityQuantityTable.Rows[j]["BalanceVariableId"].ToString() == m_VariableIdListGroup[i] + "_ElectricityQuantity")
                            {
                                decimal m_DayElectricityConsumption = m_DayMaterialWeight > 0 ? (decimal)m_ElectricityQuantityTable.Rows[j]["DayElectricityQuantity"] / m_DayMaterialWeight : 0;
                                decimal m_MonthElectricityConsumption = m_MonthMaterialWeight > 0 ? (decimal)m_ElectricityQuantityTable.Rows[j]["MonthElectricityQuantity"] / m_MonthMaterialWeight : 0;
                                m_ElectricityConsumptionTable.Rows.Add(m_VariableIdListGroup[i], m_DayElectricityConsumption, m_MonthElectricityConsumption);
                                break;
                            }
                        }
                    }

                    m_ReturnJson = Monitor_OverView.Service.OverView.OverView_Factory.GetElectricityConsumptionDetailData(m_ElectricityConsumptionTable);
                }

            }
            return m_ReturnJson;
        }
        [WebMethod]
        public static string GetEnergyConsumptionComprehensiveDetail(string myOrganizationId, string myOrganizationType)
        {
            string m_GetEnergyConsumptionComprehensiveValue = Monitor_OverView.Service.OverView.OverView_Factory.GetEnergyConsumptionComprehensiveDetail(myOrganizationId, myOrganizationType, DateTime.Now.AddDays(-1).ToString("yyyy-MM") + "-01", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
            return m_GetEnergyConsumptionComprehensiveValue;
        }
        [WebMethod]
        public static string GetEnergyConsumptionComprehensive(string myOrganizationId)
        {
            string m_GetEnergyConsumptionComprehensiveValue = Monitor_OverView.Service.OverView.OverView_Factory.GetEnergyConsumptionComprehensive(myOrganizationId, DateTime.Now.AddDays(-1).ToString("yyyy-MM") + "-01", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
            return m_GetEnergyConsumptionComprehensiveValue;
        }
        [WebMethod]
        public static string GetMaterialWeightDetail(string myVariableId, string myOrganizationId, string myOrganizationType)
        {
            DataTable m_MartieialWeightTable = null;
            string m_OrganizationInfoValue = Monitor_OverView.Service.OverView.OverView_Factory.GetMaterialWeightDetail(myVariableId, myOrganizationId, myOrganizationType, DateTime.Now.AddDays(-1).ToString("yyyy-MM") + "-01", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), ref m_MartieialWeightTable);
            return m_OrganizationInfoValue;
        }
        [WebMethod]
        public static string GetMaterialWeightData(string myVariableIdList, string myOrganizationId, string myOrganizationType)
        {
            DataTable m_MartieialWeightTable = null;
            string m_OrganizationInfoValue = Monitor_OverView.Service.OverView.OverView_Factory.GetMaterialWeight(myVariableIdList, myOrganizationId, myOrganizationType, DateTime.Now.AddDays(-1).ToString("yyyy-MM") + "-01", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), ref m_MartieialWeightTable);
            return m_OrganizationInfoValue;
        }
        [WebMethod]
        public static string GetEquipmentHaltAlarm(string myOrganizationId)
        {
            string m_OrganizationInfoValue = Monitor_OverView.Service.OverView.OverView_Factory.GetEquipmentHaltAlarm(myOrganizationId);
            return m_OrganizationInfoValue;
        }
        [WebMethod]
        public static string GetRunIndictorsDetail(string myEquipmentCommonId, string myFactoryOrganizationId, string myRunIndictorsList)
        {
            string m_RunIndictorsDetailValue = Monitor_OverView.Service.OverView.OverView_Factory.GetRunIndictorsDetail(myEquipmentCommonId, myFactoryOrganizationId, myRunIndictorsList, DateTime.Now.AddDays(-1).ToString("yyyy-MM") + "-01", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
            return m_RunIndictorsDetailValue;
        }
        [WebMethod]
        public static string GetRunIndictors(string myRunIndictorsList, string myEquipmentCommonIdList, string myFactoryOrganizationId)
        {
            string m_RunIndictorsValue = Monitor_OverView.Service.OverView.OverView_Factory.GetRunIndictors(myRunIndictorsList, myEquipmentCommonIdList, myFactoryOrganizationId, DateTime.Now.AddDays(-1).ToString("yyyy-MM") + "-01", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
            return m_RunIndictorsValue;
        }
        [WebMethod]
        public static string GetEquipmentHaltDetail(string myEquipmentCommonId, string myFactoryOrganizationId, string myStatisticalRange)
        {
            string m_OrganizationInfoValue = Monitor_OverView.Service.OverView.OverView_Factory.GetEquipmentHaltDetail(myEquipmentCommonId, myFactoryOrganizationId, myStatisticalRange, DateTime.Now.AddDays(-1).ToString("yyyy-MM") + "-01", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
            return m_OrganizationInfoValue;
        }
        [WebMethod]
        public static string GetEquipmentHalt(string myEquipmentCommonIdList, string myFactoryOrganizationId, string myStatisticalRange)
        {
            string m_OrganizationInfoValue = Monitor_OverView.Service.OverView.OverView_Factory.GetEquipmentHalt(myEquipmentCommonIdList, myFactoryOrganizationId, DateTime.Now.AddDays(-1).ToString("yyyy-MM") + "-01", DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), myStatisticalRange);
            return m_OrganizationInfoValue;
        }
        [WebMethod]
        public static string GetWorkingTeamShiftLog(string myOrganizationId)
        {
            string m_OrganizationInfoValue = Monitor_OverView.Service.OverView.OverView_Factory.GetWorkingTeamShiftLog(myOrganizationId);
            return m_OrganizationInfoValue;
        }
        [WebMethod]
        public static string GetWorkingTeamShiftLogDetail(string myOrganizationId, string myWorkingTeamShiftLogId)
        {
            string m_OrganizationInfoValue = Monitor_OverView.Service.OverView.OverView_Factory.GetWorkingTeamShiftLogDetail(myOrganizationId, myWorkingTeamShiftLogId);
            return m_OrganizationInfoValue;
        }

        [WebMethod]
        public static string GetMonthLineChartData(string myRunIndictors, string myEquipmentCommonIdList, string myOrganizationId)
        {
            string m_MonthLineChartDataValue = Monitor_OverView.Service.OverView.OverView_Factory.GetMonthLineChartData(myRunIndictors, myEquipmentCommonIdList, myOrganizationId, DateTime.Now.AddDays(-1).AddMonths(-6).ToString("yyyy-MM-01"), DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
            return m_MonthLineChartDataValue;
        }
        [WebMethod]
        public static string GetElectricitiyConsumptionChartData(string myVariableIdList, string myOrganizationId, string myOrganizationTypeList)
        {
            string m_MonthElectricitiyConsumptionChartDataValue = Monitor_OverView.Service.OverView.OverView_Factory.GetElectricitiyConsumptionChartData(myVariableIdList, myOrganizationId, myOrganizationTypeList, DateTime.Now.AddDays(-1).AddMonths(-6).ToString("yyyy-MM"), DateTime.Now.AddDays(-1).ToString("yyyy-MM"));
            return m_MonthElectricitiyConsumptionChartDataValue;
        }
        //private static DataTable GetFactoryByCompany(string myCompanyOrganizationId)
        //{
        //    List<string> m_OganizationIds = WebStyleBaseForEnergy.webStyleBase.GetDataValidIdGroup("ProductionOrganization");
        //    IList<string> m_LevelCodes = WebUserControls.Service.OrganizationSelector.OrganisationTree.GetOrganisationLevelCodeById(m_OganizationIds);
        //    return Monitor_OverView.Service.OverView.OverView_Factory.GetFactoryByCompany(myCompanyOrganizationId, m_LevelCodes.ToArray());
        //}
        //private static string GetPageData(DataTable myOrganizations)
        //{
        //    if (myOrganizations != null)
        //    {

        //    }
        //    return "1111";
        //}
    }
}