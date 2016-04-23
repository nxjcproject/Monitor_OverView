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
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_qtx_efc", "zc_nxjc_byc_byf" };
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
    }
}