using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Services;
using WebStyleBaseForEnergy;

namespace Monitor_OverView.Web.UI_SystemStatus
{
    public partial class OverView_NetworkStatus : WebStyleBaseForEnergy.webToolsStyleBase
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
        public static string GetTermimalStatus(string myOrganizationId, string myCollectorName, string myIpAddress)
        {
            string m_FactoryDataBaseName = Monitor_OverView.Service.SystemStatus.NetworkStatus.GetFactoryDataBase(myOrganizationId);
            string m_TermimalStatusTableString = Monitor_OverView.Service.SystemStatus.NetworkStatus.GetTermimalStatus(m_FactoryDataBaseName, myOrganizationId, myCollectorName, myIpAddress);

            return m_TermimalStatusTableString;
        }
        [WebMethod]
        public static string GetNetworkStructure()
        {
            return "";
        }
    }
}