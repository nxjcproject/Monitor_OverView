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
    public partial class View_ProductionData : WebStyleBaseForEnergy.webToolsStyleBase
    {
        private const string StandardType = "Production";
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_ychc", "zc_nxjc_tsc" };
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
                m_JsonString = Monitor_OverView.Service.OverView.View_ProductionData.GetGlobalComplete(myDate, m_LevelCodes.ToArray());
            }
            return m_JsonString;
        }

        [WebMethod]
        public static string GetCompanyComplete(DateTime myDate, string myLevelCode)
        {
            string m_JsonString = "1";

            //if (mUserId != "")
            {
                m_JsonString = Monitor_OverView.Service.OverView.View_ProductionData.GetCompanyComplete(myDate, myLevelCode);
            }
            return m_JsonString;
        }
        [WebMethod]
        public static string GetIndicatorItems()
        {
            string myValueJson = Monitor_OverView.Service.OverView.View_ProductionData.GetIndicatorItems();
            return myValueJson;
        }
        [WebMethod]
        public static string GetEquipmentCommonInfo()
        {
            string m_ReturnValue = Monitor_OverView.Service.OverView.View_ProductionData.GetEquipmentCommonInfo();
            return m_ReturnValue;
        }
        [WebMethod]
        public static string GetSpecificationsInfo(string myEquipmentCommonId)
        {
            string m_ReturnValue = Monitor_OverView.Service.OverView.View_ProductionData.GetSpecificationsInfo(myEquipmentCommonId);
            return m_ReturnValue;
        }
        [WebMethod]
        public static string GetPlanAndCompleteChart(string myDate, string myIndicatorId, string myEquipmentCommonId, string mySpecifications)
        {
            List<string> m_OganizationIds = WebStyleBaseForEnergy.webStyleBase.GetDataValidIdGroup("ProductionOrganization");
            string m_ReturnValue = Monitor_OverView.Service.OverView.View_ProductionData.GetProductionPlanAndComplete(myDate, myIndicatorId, myEquipmentCommonId, mySpecifications, m_OganizationIds);
            return m_ReturnValue;
        }
    }
}