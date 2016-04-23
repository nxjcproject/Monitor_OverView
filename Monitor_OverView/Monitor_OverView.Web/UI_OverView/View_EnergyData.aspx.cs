using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using WebStyleBaseForEnergy;
namespace Monitor_OverView.Web.UI_OverView
{
    public partial class View_EnergyData : WebStyleBaseForEnergy.webToolsStyleBase
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
            }
        }
        [WebMethod]
        public static string GetGlobalComplete(DateTime myDate)
        {
            string m_JsonString = "1";

            //if (mUserId != "")
            {
                List<string> m_OganizationIds = WebStyleBaseForEnergy.webStyleBase.GetDataValidIdGroup("ProductionOrganization");
                IList<string> m_LevelCodes = WebUserControls.Service.OrganizationSelector.OrganisationTree.GetOrganisationLevelCodeById(m_OganizationIds);
                m_JsonString = Monitor_OverView.Service.OverView.View_EnergyData.GetGlobalComplete(myDate, m_LevelCodes.ToArray());
            }
            return m_JsonString;
        }

        [WebMethod]
        public static string GetCompanyComprehensiveComplete(DateTime myDate, string myLevelCode)
        {
            string m_JsonString = "1";

            //if (mUserId != "")
            {
                List<string> m_OganizationIds = WebStyleBaseForEnergy.webStyleBase.GetDataValidIdGroup("ProductionOrganization");
                IList<string> m_LevelCodes = WebUserControls.Service.OrganizationSelector.OrganisationTree.GetOrganisationLevelCodeById(m_OganizationIds);
                m_JsonString = Monitor_OverView.Service.OverView.View_EnergyData.GetCompanyComprehensiveComplete(myDate, myLevelCode,m_LevelCodes.ToArray());
            }
            return m_JsonString;
        }
        [WebMethod]
        public static string GetCompanyProcessComplete(DateTime myDate, string myLevelCode)
        {
            string m_JsonString = "1";

            //if (mUserId != "")
            {
                List<string> m_OganizationIds = WebStyleBaseForEnergy.webStyleBase.GetDataValidIdGroup("ProductionOrganization");
                IList<string> m_LevelCodes = WebUserControls.Service.OrganizationSelector.OrganisationTree.GetOrganisationLevelCodeById(m_OganizationIds);
                m_JsonString = Monitor_OverView.Service.OverView.View_EnergyData.GetCompanyProcessComplete(myDate, myLevelCode, m_LevelCodes.ToArray());
            }
            return m_JsonString;
        }
        [WebMethod]
        public static string GetPlanItems()
        {
            return Monitor_OverView.Service.OverView.View_EnergyData.GetPlanItems();
        }
        [WebMethod]
        public static string GetPlanAndCompleteChart(DateTime myDate, string myVariableId, string myValueType, string myCaculateType, string myDenominator)
        {
            List<string> m_OganizationIds = WebStyleBaseForEnergy.webStyleBase.GetDataValidIdGroup("ProductionOrganization");
            if (myValueType == "ElectricityQuantity")   //电量
            {
                return Monitor_OverView.Service.OverView.View_EnergyData.GetQuntityPlanAndComplete(myDate, myVariableId, myValueType, m_OganizationIds.ToArray());
            }
            else if (myValueType == "MaterialWeight")   //产量
            {
                return Monitor_OverView.Service.OverView.View_EnergyData.GetWeightPlanAndComplete(myDate, myVariableId, myValueType, m_OganizationIds.ToArray());
            }
            else if (myValueType == "ElectricityConsumption" && myCaculateType == "Normal")  //工序电耗
            {
                return Monitor_OverView.Service.OverView.View_EnergyData.GetElectricityConsumptionPlanAndComplete(myDate, myVariableId, myValueType, myDenominator, m_OganizationIds.ToArray());
            }
            else if (myValueType == "ElectricityConsumption" && myCaculateType == "Comprehensive")  //综合电耗
            {
                return Monitor_OverView.Service.OverView.View_EnergyData.GetComprehensivePlanAndComplete(myDate, myVariableId, myValueType, myDenominator, m_OganizationIds.ToArray());
            }
            else if (myValueType == "CoalConsumption" && myCaculateType == "Comprehensive")   //综合煤耗
            {
                return Monitor_OverView.Service.OverView.View_EnergyData.GetComprehensivePlanAndComplete(myDate, myVariableId, myValueType, myDenominator, m_OganizationIds.ToArray());
            }
            else if (myValueType == "EnergyConsumption" && myCaculateType == "Comprehensive")   //综合能耗
            {
                return Monitor_OverView.Service.OverView.View_EnergyData.GetComprehensivePlanAndComplete(myDate, myVariableId, myValueType, myDenominator, m_OganizationIds.ToArray());
            }
            else
            {
                return "1";
            }
        }
    }
}