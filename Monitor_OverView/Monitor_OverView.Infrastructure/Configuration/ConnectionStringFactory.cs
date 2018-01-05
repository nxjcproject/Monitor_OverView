using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Monitor_OverView.Infrastruture.Configuration
{
    public static class ConnectionStringFactory
    {
        public static string NXJCConnectionString { get { return ConfigurationManager.ConnectionStrings["ConnNXJC"].ToString(); } }
        public static string StationId
        {
            get
            {
                return ConfigurationManager.AppSettings["StationId"] != null ? ConfigurationManager.AppSettings["StationId"].ToString() : "";
            }
        }
        public static string MonthStatisticalDay
        {
            get
            {
                return ConfigurationManager.AppSettings["MonthStatisticalDay"] != null ? ConfigurationManager.AppSettings["MonthStatisticalDay"].ToString() : "1,0";
            }
        }
    }
}
