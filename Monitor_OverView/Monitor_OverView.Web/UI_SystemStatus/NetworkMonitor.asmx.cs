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
        private static Dictionary<string, List<string>> m_StatusBuffer;
        private static Dictionary<string, string> m_TimeStampBuffer;

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
        public void SetGroupNetworkStatus(string myOrganizationId, string myTimeStamp, byte[] myStatusBuffer)
        {
            if (m_StatusBuffer == null)
            {
                m_StatusBuffer = new Dictionary<string, List<string>>();
            }
            if (myStatusBuffer != null)
            {
                string[] m_StatusArray = Monitor_OverView.Service.SystemStatus.NetworkStatus.GetStatusString(myStatusBuffer);
                if (m_StatusArray != null)
                {
                    if (!m_StatusBuffer.ContainsKey(myOrganizationId))            //如果不包括该组织机构
                    {
                        List<string> m_StatusArrayTemp = new List<string>();
                        for (int i = 0; i < m_StatusArray.Length; i++)
                        {
                            m_StatusArrayTemp.Add(m_StatusArray[i]);
                        }
                        m_StatusBuffer.Add(myOrganizationId, m_StatusArrayTemp);
                    }
                    else
                    {
                        if (m_StatusBuffer[myOrganizationId] != null)
                        {
                            //m_StatusBuffer[myOrganizationId].Clear();
                            for (int i = 0; i < m_StatusArray.Length; i++)
                            {
                                m_StatusBuffer[myOrganizationId].Add(m_StatusArray[i]);
                            }
                        }
                        else
                        {
                            List<string> m_StatusArrayTemp = new List<string>();
                            for (int i = 0; i < m_StatusArray.Length; i++)
                            {
                                m_StatusArrayTemp.Add(m_StatusArray[i]);
                            }
                            m_StatusBuffer[myOrganizationId] = m_StatusArrayTemp;
                        }

                    }

                }
            }
        }
        [WebMethod]
        public void SetNetworkStatus(string myOrganizationId, string myTimeStamp, byte[] myStatusBuffer)
        {
            if (m_StatusBuffer == null)
            {
                m_StatusBuffer = new Dictionary<string, List<string>>();
            }
            if (myStatusBuffer != null)
            {
                string[] m_StatusArray = Monitor_OverView.Service.SystemStatus.NetworkStatus.GetStatusString(myStatusBuffer);
                if (m_StatusArray != null)
                {
                    if (!m_StatusBuffer.ContainsKey(myOrganizationId))            //如果不包括该组织机构
                    {
                        List<string> m_StatusArrayTemp = new List<string>();
                        for (int i = 0; i < m_StatusArray.Length; i++)
                        {
                            m_StatusArrayTemp.Add(m_StatusArray[i]);
                        }
                        m_StatusBuffer.Add(myOrganizationId, m_StatusArrayTemp);
                    }
                    else
                    {
                        if (m_StatusBuffer[myOrganizationId] != null)
                        {
                            m_StatusBuffer[myOrganizationId].Clear();
                            for (int i = 0; i < m_StatusArray.Length; i++)
                            {
                                m_StatusBuffer[myOrganizationId].Add(m_StatusArray[i]);
                            }
                        }
                        else
                        {
                            List<string> m_StatusArrayTemp = new List<string>();
                            for (int i = 0; i < m_StatusArray.Length; i++)
                            {
                                m_StatusArrayTemp.Add(m_StatusArray[i]);
                            }
                            m_StatusBuffer[myOrganizationId] = m_StatusArrayTemp;
                        }

                    }

                }
            }
            else
            {
                if (!m_StatusBuffer.ContainsKey(myOrganizationId))            //如果不包括该组织机构
                {
                    List<string> m_StatusArrayTemp = new List<string>();
                    m_StatusBuffer.Add(myOrganizationId, m_StatusArrayTemp);
                }
                else
                {
                    if (m_StatusBuffer[myOrganizationId] != null)
                    {
                        m_StatusBuffer[myOrganizationId].Clear();
                    }
                    else
                    {
                        List<string> m_StatusArrayTemp = new List<string>();
                        m_StatusBuffer[myOrganizationId] = m_StatusArrayTemp;
                    }

                }

            }

            ////////////////////////更新上传时间
            if (m_TimeStampBuffer == null)
            {
                m_TimeStampBuffer = new Dictionary<string, string>();
            }
            if (!m_TimeStampBuffer.ContainsKey(myOrganizationId))            //如果不包括该组织机构
            {
                m_TimeStampBuffer.Add(myOrganizationId, myTimeStamp);
            }
            else
            {
                m_TimeStampBuffer[myOrganizationId] = myTimeStamp;
            }
        }
        /// <summary>
        /// /////////////////////////获得更新时间戳///////////////
        /// </summary>
        /// <returns>所有分厂的时间戳用逗号和分号分开</returns>
        [WebMethod]
        public string GetTimeStampGroup()
        {
            string m_TimeStampGroup = "";
            if (m_TimeStampBuffer != null)
            {
                foreach (string myKey in m_TimeStampBuffer.Keys)
                {
                    if (m_TimeStampGroup == "")
                    {
                        m_TimeStampGroup = myKey + "," + m_TimeStampBuffer[myKey];
                    }
                    else
                    {
                        m_TimeStampGroup = m_TimeStampGroup + ";" + myKey + "," + m_TimeStampBuffer[myKey];
                    }
                }
            }
            return m_TimeStampGroup;
        }
        /// <summary>
        /// 获得某个分厂的更新时间戳
        /// </summary>
        /// <param name="myOrganizationId">组织机构ID</param>
        /// <returns>某分厂的时间戳</returns>
        [WebMethod]
        public string GetTimeStamp(string myOrganizationId)
        {
            if (m_TimeStampBuffer != null && m_TimeStampBuffer.ContainsKey(myOrganizationId))
            {
                return m_TimeStampBuffer[myOrganizationId];
            }
            else
            {
                return "";
            }
        }
        [WebMethod]
        public string GetNetworkStatus()
        {
            /////////////////////////测试用/////////////////////
            //m_StatusBuffer = new Dictionary<string, List<string>>();
            //List<string> m_TestList = new List<string>();
            //m_TestList.Add("zc_nxjc_byc_byf_zc_nxjc_byc_byf_ELCCollector1;AmmeterS;0");
            //m_TestList.Add("zc_nxjc_byc_byf_zc_nxjc_byc_byf_ELCCollector1;Network;0");
            //m_TestList.Add("zc_nxjc_byc_byf_zc_nxjc_byc_byf_OPCCollector1;Network;0");
            //m_TestList.Add("zc_nxjc_byc_byf_zc_nxjc_byc_byf_OPCCollector1;OPC;0");
            //m_TestList.Add("zc_nxjc_byc_byf_zc_nxjc_byc_byf_FactoryServer;Network;0");
            //m_StatusBuffer.Add("zc_nxjc_byc_byf", m_TestList);


            //m_TimeStampBuffer = new Dictionary<string, string>();
            //m_TimeStampBuffer.Add("zc_nxjc_byc_byf", "2016-07-29 21:10:20");

            ////////////////////////////////////////////////////

            DataSet m_FactoryServerStatusDataSet = Monitor_OverView.Service.SystemStatus.NetworkStatus.GetStatusDataSet(m_StatusBuffer, m_TimeStampBuffer);

            string m_ReturnString = "";
            m_ReturnString = m_ReturnString + GetStatusString(m_FactoryServerStatusDataSet.Tables["FactoryNodeStatus"]);
            if (m_ReturnString != "")
            {
                string m_StringTemp = GetStatusString(m_FactoryServerStatusDataSet.Tables["NormalNodeStatusStatus"]);
                if (m_StringTemp != "")
                {
                    m_ReturnString = m_ReturnString + "," + m_StringTemp;
                }
            }
            else
            {
                m_ReturnString = GetStatusString(m_FactoryServerStatusDataSet.Tables["NormalNodeStatusStatus"]);
            }
            
            return "{" + m_ReturnString + "}";
        }

        private string GetStatusString(DataTable myStatusDataTable)
        {
            string m_ReturnString = "";
            if (myStatusDataTable != null)
            {
                for (int i = 0; i < myStatusDataTable.Rows.Count; i++)
                {
                    string m_RowDataString = "";
                    for (int j = 1; j < myStatusDataTable.Columns.Count; j++)
                    {
                        if (j == 1)
                        {
                            m_RowDataString = "\"" + myStatusDataTable.Rows[i][0].ToString() + "\":{\"" + myStatusDataTable.Columns[j].ColumnName + "\":\"" + myStatusDataTable.Rows[i][j].ToString() + "\"";
                        }
                        else
                        {
                            m_RowDataString = m_RowDataString + ",\"" + myStatusDataTable.Columns[j].ColumnName + "\":\"" + myStatusDataTable.Rows[i][j].ToString() + "\"";
                        }
                    }
                    if (i == 0)
                    {
                        if (m_RowDataString != "")
                        {
                            m_ReturnString = m_RowDataString + "}";
                        }
                    }
                    else
                    {
                        if (m_RowDataString != "")
                        {
                            m_ReturnString = m_ReturnString + "," + m_RowDataString + "}";
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
                    m_FactoryServerTemp.Id = m_OrganzationId[i] + "_" + m_FactoryServerInfoTable.Rows[i]["NodeId"].ToString();
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
