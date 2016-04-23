using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
namespace Monitor_OverView.Web.UI_SystemStatus
{
    /// <summary>
    /// NetworkMonitor 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    [System.Web.Script.Services.ScriptService]
    public class NetworkMonitor : System.Web.Services.WebService
    {

        private static Dictionary<string, bool> GroupNetworkStatus = new Dictionary<string, bool>();         //分厂到集团网络
        private static Dictionary<string, DataTable> DataComputerNetworkStatus = new Dictionary<string, DataTable>();           //采集计算机网络状况

        private static Dictionary<string, DateTime> FactorySoftwareUpdateTime = new Dictionary<string, DateTime>();      //记录分厂汇总软件状态
        private static Dictionary<string, DateTime> RealtimeDatetime = new Dictionary<string, DateTime>();               //实时表记录的时间

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
            /*
             以分厂为单位,
             1、记录到集团的网络状况;bool
             2、记录汇总软件更新时间;DateTime
             3、记录分厂数据库电表实时表vDate;DateTime
             2、记录到每个采集计算机的网络状况;bool
             */
        }
        [WebMethod]
        public void SetNetworkStatus(string myOrganizationId, bool myGroupNetworkStatus, DateTime myFactorySoftwareUpdateTime, DateTime myRealtimeDatetime, DataTable myDataComputerNetworkStatus)
        {
            if (GroupNetworkStatus.ContainsKey(myOrganizationId))
            {
                GroupNetworkStatus[myOrganizationId] = myGroupNetworkStatus;
                FactorySoftwareUpdateTime[myOrganizationId] = myFactorySoftwareUpdateTime;
                RealtimeDatetime[myOrganizationId] = myRealtimeDatetime;
                DataComputerNetworkStatus[myOrganizationId] = myDataComputerNetworkStatus;
            }
            else       //如果没有则增加
            {
                GroupNetworkStatus.Add(myOrganizationId, myGroupNetworkStatus);
                FactorySoftwareUpdateTime.Add(myOrganizationId, myFactorySoftwareUpdateTime);
                RealtimeDatetime.Add(myOrganizationId, myRealtimeDatetime);
                DataComputerNetworkStatus.Add(myOrganizationId, myDataComputerNetworkStatus);
            }
        }
        [WebMethod]
        public string GetNetworkStatus()
        {
            string m_StationId = Monitor_OverView.Service.OverView.OverView_Nxjc.GetStationId();
            DataTable m_FactoryServerInfoTable = Monitor_OverView.Service.SystemStatus.NetworkStatus.GetFactoryServerInfo(m_StationId);
            if (m_FactoryServerInfoTable != null)
            {
                string[] m_FactoryDataBaseName = new string[m_FactoryServerInfoTable.Rows.Count];
                string[] m_OrganzationId = new string[m_FactoryServerInfoTable.Rows.Count];
                for (int i = 0; i < m_FactoryServerInfoTable.Rows.Count; i++)
                {
                    m_FactoryDataBaseName[i] = m_FactoryServerInfoTable.Rows[i]["MeterDatabase"].ToString();
                    m_OrganzationId[i] = m_FactoryServerInfoTable.Rows[i]["OrganizationId"].ToString();
                }
                DataTable m_DataCollectionNetValueTable = Monitor_OverView.Service.SystemStatus.NetworkStatus.GetDataCollectionNetInfo(m_OrganzationId);
                Monitor_OverView.Service.SystemStatus.NetworkStatus.GetDataComputerAndOPCValue(ref m_DataCollectionNetValueTable);        //根据网络的对照表查找每个实时表的时间,并返回是否正常
                Monitor_OverView.Service.SystemStatus.NetworkStatus.GetAmmeterValue(m_FactoryDataBaseName, ref m_DataCollectionNetValueTable);        //根据网络的对照表查找每个实时表的时间,并返回是否正常
                DataTable m_FactoryServerStatusTable = Monitor_OverView.Service.SystemStatus.NetworkStatus.GetFactoryNetworkStatus(m_OrganzationId, m_FactoryDataBaseName, GroupNetworkStatus, FactorySoftwareUpdateTime, RealtimeDatetime);
                DataTable m_DataComputerStatusTable = Monitor_OverView.Service.SystemStatus.NetworkStatus.GetDataComputerNetworkStatus(m_DataCollectionNetValueTable, DataComputerNetworkStatus);
                DataTable m_OPCStatusTable = Monitor_OverView.Service.SystemStatus.NetworkStatus.GetOPCStatus(m_DataCollectionNetValueTable);
                DataTable m_AmmeterNetworkStatusTable = Monitor_OverView.Service.SystemStatus.NetworkStatus.GetAmmeterNetworkStatus(m_DataCollectionNetValueTable);

                string m_ReturnString = "";
                m_ReturnString = m_ReturnString + GetStatusString(m_FactoryServerStatusTable, true);
                if (m_ReturnString != "")
                {
                    string m_StringTemp = GetStatusString(m_DataComputerStatusTable, false);
                    if (m_StringTemp != "")
                    {
                        m_ReturnString = m_ReturnString + "," + m_StringTemp;
                    }
                }
                else
                {
                    m_ReturnString = GetStatusString(m_DataComputerStatusTable, false);
                }
                if (m_ReturnString != "")
                {
                    string m_StringTemp = GetStatusString(m_OPCStatusTable, false);
                    if (m_StringTemp != "")
                    {
                        m_ReturnString = m_ReturnString + "," + m_StringTemp;
                    }
                }
                else
                {
                    m_ReturnString = GetStatusString(m_OPCStatusTable, false);
                }
                if (m_ReturnString != "")
                {
                    string m_StringTemp = GetStatusString(m_AmmeterNetworkStatusTable, false);
                    if (m_StringTemp != "")
                    {
                        m_ReturnString = m_ReturnString + "," + m_StringTemp;
                    }
                }
                else
                {
                    m_ReturnString = GetStatusString(m_AmmeterNetworkStatusTable, false);
                }
                return "{" + m_ReturnString + "}";
            }
            else
            {
                return "[]";
            }
        }
        private string GetStatusString(DataTable myStatusDataTable, bool myIsSynchronization)
        {
            string m_ReturnString = "";
            if (myStatusDataTable != null)
            {
                for (int i = 0; i < myStatusDataTable.Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        m_ReturnString = "\"" + myStatusDataTable.Rows[i]["Id"].ToString() + "\":{\"NetworkStatus\":\"" + myStatusDataTable.Rows[i]["NetworkStatus"].ToString() +
                                         "\",\"SoftwareStatus\":\"" + myStatusDataTable.Rows[i]["SoftwareStatus"].ToString() + "\"";
                        if (myIsSynchronization == true)
                        {
                            m_ReturnString = m_ReturnString + ",\"SynchronizationStatus\":\"" + myStatusDataTable.Rows[i]["SynchronizationStatus"].ToString() + "\"}";
                        }
                        else
                        {
                            m_ReturnString = m_ReturnString + "}";
                        }
                    }
                    else
                    {
                        m_ReturnString = m_ReturnString + ",\"" + myStatusDataTable.Rows[i]["Id"].ToString() + "\":{\"NetworkStatus\":\"" + myStatusDataTable.Rows[i]["NetworkStatus"].ToString() +
                                                           "\",\"SoftwareStatus\":\"" + myStatusDataTable.Rows[i]["SoftwareStatus"].ToString() + "\"";
                        if (myIsSynchronization == true)
                        {
                            m_ReturnString = m_ReturnString + ",\"SynchronizationStatus\":\"" + myStatusDataTable.Rows[i]["SynchronizationStatus"].ToString() + "\"}";
                        }
                        else
                        {
                            m_ReturnString = m_ReturnString + "}";
                        }
                    }
                }
            }
            return m_ReturnString;
        }
        private List<Monitor_OverView.Service.SystemStatus.Model_FactoryServer> LoadNetworkStructure()
        {
            List<Monitor_OverView.Service.SystemStatus.Model_FactoryServer> m_FactoryServerList = new List<Service.SystemStatus.Model_FactoryServer>();
            string m_StationId = Monitor_OverView.Service.OverView.OverView_Nxjc.GetStationId();
            string m_StationIpAddress = "";
            //m_StationId = "zc_nxjc";
            DataTable m_FactoryServerInfoTable = Monitor_OverView.Service.SystemStatus.NetworkStatus.GetFactoryServerInfo(m_StationId);

            if (m_FactoryServerInfoTable != null)
            {
                string[] m_FactoryDataBaseName = new string[m_FactoryServerInfoTable.Rows.Count];
                string[] m_OrganzationId = new string[m_FactoryServerInfoTable.Rows.Count];
                for (int i = 0; i < m_FactoryServerInfoTable.Rows.Count; i++)
                {
                    m_FactoryDataBaseName[i] = m_FactoryServerInfoTable.Rows[i]["MeterDatabase"].ToString();
                    m_OrganzationId[i] = m_FactoryServerInfoTable.Rows[i]["OrganizationId"].ToString();
                    m_StationIpAddress = Monitor_OverView.Service.SystemStatus.NetworkStatus.GetStationIpAddress(m_OrganzationId[i], m_FactoryServerInfoTable);    //获取站点IP地址
                    Monitor_OverView.Service.SystemStatus.Model_FactoryServer m_FactoryServerTemp = new Service.SystemStatus.Model_FactoryServer();
                    m_FactoryServerTemp.Id = m_OrganzationId[i];
                    m_FactoryServerTemp.OrganizationId = m_OrganzationId[i];
                    m_FactoryServerTemp.Name = m_FactoryServerInfoTable.Rows[i]["Name"].ToString();
                    m_FactoryServerTemp.IpAddress = m_StationIpAddress;
                    m_FactoryServerList.Add(m_FactoryServerTemp);
                }
                DataTable m_DataCollectionNetInfoTable = Monitor_OverView.Service.SystemStatus.NetworkStatus.GetDataCollectionNetInfo(m_OrganzationId);
                if (m_DataCollectionNetInfoTable != null)
                {
                    for (int i = 0; i < m_FactoryServerList.Count; i++)
                    {
                        //////////////获得每一个分厂的所有数采计算机
                        DataRow[] m_DataComputer = m_DataCollectionNetInfoTable.Select(string.Format("OrganizationID = '{0}' and NodeType = '{1}'", m_FactoryServerList[i].OrganizationId, "DataComputer"));
                        for (int j = 0; j < m_DataComputer.Length; j++)
                        {
                            Monitor_OverView.Service.SystemStatus.Model_DataComputer m_DataComputerTemp = new Service.SystemStatus.Model_DataComputer();
                            m_DataComputerTemp.IpAddress = m_DataComputer[j]["IpAddress"].ToString();
                            m_DataComputerTemp.Id = m_FactoryServerList[i].OrganizationId + "_" + m_DataComputer[j]["NodeId"].ToString();
                            m_DataComputerTemp.Name = m_DataComputer[j]["NodeName"].ToString();
                            string m_ParentNodeId = m_DataComputer[j]["ParentNodeId"].ToString();
                            //获得每一个数采计算机下面的交换机
                            Monitor_OverView.Service.SystemStatus.NetworkStatus.GetUpperSwitchInfo(m_DataCollectionNetInfoTable, ref m_DataComputerTemp, m_ParentNodeId, m_FactoryServerList[i].OrganizationId);
                            m_FactoryServerList[i].DataComputer.Add(m_DataComputerTemp);
                        }
                    }
                }
            }
            return m_FactoryServerList;
        }
        [WebMethod]
        public string GetNetworkStructure()
        {
            List<Monitor_OverView.Service.SystemStatus.Model_FactoryServer> m_FactoryServerList = LoadNetworkStructure(); //刷新一下
            StringBuilder m_ValueString = new StringBuilder();
            m_ValueString.Append("{\"FactoryServers\":");
            if (m_FactoryServerList != null)
            {
                string m_FactoryServerString = "";
                for (int i = 0; i < m_FactoryServerList.Count; i++)                 //检测一共多少个服务器
                {
                    if (i == 0)
                    {
                        m_FactoryServerString = Monitor_OverView.Service.SystemStatus.NetworkStatus.GetFactoryServerValueString(m_FactoryServerList[i]);
                    }
                    else
                    {
                        m_FactoryServerString = m_FactoryServerString + "," + Monitor_OverView.Service.SystemStatus.NetworkStatus.GetFactoryServerValueString(m_FactoryServerList[i]);
                    }
                }
                if (m_FactoryServerString != "")
                {
                    m_ValueString.Append("[" + m_FactoryServerString + "]");
                }
                else
                {
                    m_ValueString.Append("[]");
                }
            }
            else
            {
                m_ValueString.Append("[]");
            }
            m_ValueString.Append("}");
            return m_ValueString.ToString();
        }
    }
}
