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
    public partial class View_OverView_nxjc : WebStyleBaseForEnergy.webToolsStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Monitor_OverView.Service.OverView.OverView_Nxjc.GetStationId() != "zc_nxjc")
            {
                Response.Redirect("View_OverView_Factory.aspx", false); 
            }
            else
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
        }
        [WebMethod]
        public static string GetRealTimeData(string myStartTime, string myEndTime)
        {
            string m_JsonString = "";
            m_JsonString = Monitor_OverView.Service.OverView.OverView_Nxjc.GetRealTimeData(myStartTime, myEndTime);
            return m_JsonString;
        }
    }
}