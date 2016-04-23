using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SqlServerDataAdapter;
namespace Monitor_OverView.Service.SystemStatus
{
    public class NetworkStatus
    {
        private static readonly ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(Monitor_OverView.Infrastruture.Configuration.ConnectionStringFactory.NXJCConnectionString);
        private const int ValidDelayTime = 20;
        public static DataTable GetFactoryServerInfo(string myStationId)
        {
            string m_Condition = "";
            if (myStationId != "zc_nxjc")
            {
                m_Condition = string.Format(" and A.OrganizationID = '{0}'", myStationId);
            }
            string m_Sql = @"Select A.OrganizationID as OrganizationId, C.Name + A.Name as Name, B.MeterDatabase, D.NodeType, D.IpAddress from system_Organization A
                                left join system_Organization C on C.LevelCode = substring(A.LevelCode, 1,len(A.LevelCode) - 2)
                                ,system_Database B, net_DataCollectionNet D
                                where A.LevelType = 'Factory'
                                and A.DatabaseID = B.DatabaseID
                                and D.NodeType = 'FactoryServer'
                                and D.OrganizationID = A.OrganizationID
                                {0}
                                order by A.LevelCode";
            m_Sql = string.Format(m_Sql, m_Condition);
            try
            {
                DataTable m_Result = _dataFactory.Query(m_Sql);
                return m_Result;
            }
            catch
            {
                return null;
            }
        }
        public static string GetStationIpAddress(string myOrganizationId, DataTable myFacotoryServerTable)
        {
            string m_StationIpAddress = "";
            if (myFacotoryServerTable != null)
            {
                DataRow[] m_FactoryServerRow = myFacotoryServerTable.Select(string.Format("NodeType = 'FactoryServer' and OrganizationID = '{0}'", myOrganizationId));
                if (m_FactoryServerRow != null && m_FactoryServerRow.Length > 0)
                {
                    m_StationIpAddress = m_FactoryServerRow[0]["IpAddress"].ToString();
                }
            }
            return m_StationIpAddress;
        }

        public static DataTable GetDataCollectionNetInfo(string[] myOrganzationId)
        {
            string m_Condition = "";
            if (myOrganzationId != null)
            {
                for (int i = 0; i < myOrganzationId.Length; i++)
                {
                    if (i == 0)
                    {
                        m_Condition = "'" + myOrganzationId[i] + "'";
                    }
                    else
                    {
                        m_Condition = m_Condition + ",'" + myOrganzationId[i] + "'";
                    }
                }
                string m_Sql = @"Select A.* from net_DataCollectionNet A where A.OrganizationID in ({0}) order by A.OrganizationID, A.ParentNodeId";
                m_Sql = string.Format(m_Sql, m_Condition);

                try
                {
                    DataTable m_Result = _dataFactory.Query(m_Sql);
                    return m_Result;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        public static void GetUpperSwitchInfo(DataTable myDataCollectionNetInfoTable, ref Service.SystemStatus.Model_DataComputer myDataComputer, string myParentNodeId, string myOrganizationId)
        {
            ////////////////////查找下层交换机/////////////////////
            DataRow[] m_DataRow = myDataCollectionNetInfoTable.Select(string.Format("OrganizationID = '{0}' and NodeId = '{1}' and NodeType = '{2}'", myOrganizationId, myParentNodeId, "Switch"));
            if (m_DataRow.Length > 0)
            {
                myDataComputer.Switch.Id = myOrganizationId + "_" + m_DataRow[0]["NodeId"].ToString();
                myDataComputer.Switch.Name = m_DataRow[0]["NodeName"].ToString();
                myDataComputer.Switch.DepthIndex = 1;
                myDataComputer.Switch.Switch = GetSwitchInfo(myDataCollectionNetInfoTable, myDataComputer.Switch.DepthIndex, myParentNodeId, myOrganizationId);           //查找下级交换机
                myDataComputer.Switch.Collector = GetDeviceInfo(myDataCollectionNetInfoTable, myDataComputer.Switch.DepthIndex, myParentNodeId, myOrganizationId);

                if (myDataComputer.Switch.Switch != null)
                {
                    int m_MaxDepthIndex = 0;      //找出子节点最深的深度数
                    int m_CollectorCount = 0;     //汇总所有的采集器的数量
                    for (int j = 0; j < myDataComputer.Switch.Switch.Count; j++)
                    {
                        if (myDataComputer.Switch.Switch[j].MaxDepth > m_MaxDepthIndex)
                        {
                            m_MaxDepthIndex = myDataComputer.Switch.Switch[j].MaxDepth;
                        }
                        m_CollectorCount = myDataComputer.Switch.Switch[j].CollectorCount + m_CollectorCount;
                    }
                    myDataComputer.Switch.MaxDepth = m_MaxDepthIndex + 1;
                    myDataComputer.Switch.CollectorCount = myDataComputer.Switch.Collector.Count + m_CollectorCount;
                }
                else
                {
                    myDataComputer.Switch.MaxDepth = 1;
                    myDataComputer.Switch.CollectorCount = myDataComputer.Switch.Collector.Count;
                }
            }
        }
        public static List<Service.SystemStatus.Model_Switch> GetSwitchInfo(DataTable myDataCollectionNetInfoTable, int myIndex, string myNodeId, string myOrganizationId)
        {
            List<Service.SystemStatus.Model_Switch> m_SubSwitchInfoList = new List<Model_Switch>();
            DataRow[] m_DataRow = myDataCollectionNetInfoTable.Select(string.Format("OrganizationID = '{0}' and ParentNodeId = '{1}' and NodeType = '{2}'", myOrganizationId, myNodeId, "Switch"));
            for (int i = 0; i < m_DataRow.Length; i++)
            {
                Service.SystemStatus.Model_Switch m_Switch = new Model_Switch();
                m_Switch.Id = myOrganizationId + "_" + m_DataRow[0]["NodeId"].ToString();
                m_Switch.Name = m_DataRow[i]["NodeName"].ToString();
                m_Switch.DepthIndex = myIndex + 1;
                m_Switch.Switch = GetSwitchInfo(myDataCollectionNetInfoTable, m_Switch.DepthIndex, m_DataRow[i]["NodeId"].ToString(), myOrganizationId);           //查找下级交换机
                m_Switch.Collector = GetDeviceInfo(myDataCollectionNetInfoTable, m_Switch.DepthIndex, m_DataRow[i]["NodeId"].ToString(), myOrganizationId);

                if (m_Switch.Switch != null)
                {
                    int m_MaxDepthIndex = 0;      //找出子节点最深的深度数
                    int m_CollectorCount = 0;     //汇总所有的采集器的数量
                    for (int j = 0; j < m_Switch.Switch.Count; j++)
                    {
                        if (m_Switch.Switch[j].MaxDepth > m_MaxDepthIndex)
                        {
                            m_MaxDepthIndex = m_Switch.Switch[j].MaxDepth;
                        }
                        m_CollectorCount = m_Switch.Switch[j].CollectorCount + m_CollectorCount;
                    }
                    m_Switch.MaxDepth = m_MaxDepthIndex + 1;
                    m_Switch.CollectorCount = m_Switch.Collector.Count + m_CollectorCount;
                }
                else
                {
                    m_Switch.MaxDepth = 1;
                    m_Switch.CollectorCount = m_Switch.Collector.Count;
                }
                m_SubSwitchInfoList.Add(m_Switch);
            }
            return m_SubSwitchInfoList;
        }
        public static List<Service.SystemStatus.Model_Collector> GetDeviceInfo(DataTable myDataCollectionNetInfoTable, int myIndex, string myNodeId, string myOrganizationId)
        {
            ////////////////////查找下层设备//////////////////////
            List<Service.SystemStatus.Model_Collector> m_SubCollectorInfoList = new List<Service.SystemStatus.Model_Collector>();
            DataRow[] m_DataRowDevice = myDataCollectionNetInfoTable.Select(string.Format("OrganizationID = '{0}' and ParentNodeId = '{1}' and NodeType in {2}", myOrganizationId, myNodeId, "('OPC','Ammeter','EAS','DLQ')"));
            for (int i = 0; i < m_DataRowDevice.Length; i++)
            {
                Monitor_OverView.Service.SystemStatus.Model_Collector m_CollectorTemp = new Model_Collector();
                m_CollectorTemp.Id = myOrganizationId + "_" + m_DataRowDevice[i]["NodeId"].ToString();
                m_CollectorTemp.Name = m_DataRowDevice[i]["NodeName"].ToString();
                m_CollectorTemp.IpAddress = m_DataRowDevice[i]["IpAddress"].ToString();
                m_CollectorTemp.Type = m_DataRowDevice[i]["NodeType"].ToString();
                m_SubCollectorInfoList.Add(m_CollectorTemp);
            }
            return m_SubCollectorInfoList;
        }
        //获取每个服务器下面的数据采集电脑
        public static string GetFactoryServerValueString(Service.SystemStatus.Model_FactoryServer myFactoryServer)
        {
            StringBuilder m_ValueString = new StringBuilder();
            if (myFactoryServer != null && myFactoryServer.DataComputer != null)
            {
                m_ValueString.Append("{\"Id\":\"" + myFactoryServer.Id +
                                                 "\",\"Name\":\"" + myFactoryServer.Name +
                                                 "\",\"IpAddress\":\"" + myFactoryServer.IpAddress +
                                                 "\",\"OrganizationId\":\"" + myFactoryServer.OrganizationId +
                                                 "\",\"PropertyName\":\"" + myFactoryServer.PropertyName +
                                                 "\",\"DataComputer\":");
                string m_ComputerValueTemp = "";
                for (int i = 0; i < myFactoryServer.DataComputer.Count; i++)
                {
                    if (i == 0)
                    {
                        m_ComputerValueTemp = GetDataComputerValueString(myFactoryServer.DataComputer[i]);
                    }
                    else
                    {
                        m_ComputerValueTemp = m_ComputerValueTemp + "," + GetDataComputerValueString(myFactoryServer.DataComputer[i]);
                    }
                }
                if (m_ComputerValueTemp != "")
                {
                    m_ValueString.Append("[" + m_ComputerValueTemp + "]");
                }
                else
                {
                    m_ValueString.Append("[]");
                }
                m_ValueString.Append("}");
            }
            else
            {
                m_ValueString.Append("[]");
            }
            return m_ValueString.ToString();
        }
        public static string GetDataComputerValueString(Service.SystemStatus.Model_DataComputer myDataComputer)
        {
            StringBuilder m_ValueString = new StringBuilder();
            if (myDataComputer != null && myDataComputer.Switch != null)
            {
                m_ValueString.Append("{\"Id\":\"" + myDataComputer.Id +
                                                 "\",\"Name\":\"" + myDataComputer.Name +
                                                 "\",\"IpAddress\":\"" + myDataComputer.IpAddress +
                                                 "\",\"PropertyName\":\"" + myDataComputer.PropertyName +
                                                 "\",\"Switch\":");

                string m_SwitchValueTemp = "{\"Id\":\"" + myDataComputer.Switch.Id +
                                                "\",\"Name\":\"" + myDataComputer.Switch.Name +
                                                "\",\"DepthIndex\":" + myDataComputer.Switch.DepthIndex +
                                                ",\"MaxDepth\":" + myDataComputer.Switch.MaxDepth +
                                                ",\"CollectorCount\":" + myDataComputer.Switch.CollectorCount +
                                                ",\"PropertyName\":\"" + myDataComputer.PropertyName +
                                                "\",\"Switch\":";
                /////////////////////////////////获得子交换机//////////////////////////
                string m_SubSwitchValueTemp = "";
                for (int i = 0; i < myDataComputer.Switch.Switch.Count; i++)
                {
                    if (i == 0)
                    {
                        m_SubSwitchValueTemp = GetSwitchValueString(myDataComputer.Switch.Switch[i]);
                    }
                    else
                    {
                        m_SubSwitchValueTemp = m_SubSwitchValueTemp + "," + GetSwitchValueString(myDataComputer.Switch.Switch[i]);
                    }
                }
                if (m_SubSwitchValueTemp != "")
                {
                    m_SwitchValueTemp = m_SwitchValueTemp + "[" + m_SubSwitchValueTemp + "]";
                }
                else
                {
                    m_SwitchValueTemp = m_SwitchValueTemp + "[]";
                }
                ////////////////////////////////获得设备////////////////////////////////
                string m_CollectorValueTemp = GetCollectorValueString(myDataComputer.Switch);
                if (m_CollectorValueTemp != "")
                {
                    m_SwitchValueTemp = m_SwitchValueTemp + ",\"Collector\":" + "[" + m_CollectorValueTemp + "]";
                }
                else
                {
                    m_SwitchValueTemp = m_SwitchValueTemp + ",\"Collector\":[]";
                }

                ///////////////////////////////////////////////////////////////////////
                m_SwitchValueTemp = m_SwitchValueTemp + "}";
                m_ValueString.Append(m_SwitchValueTemp);
                m_ValueString.Append("}");
            }
            else
            {
                m_ValueString.Append("[]");
            }
            return m_ValueString.ToString();
        }

        public static string GetSwitchValueString(Service.SystemStatus.Model_Switch mySwitch)
        {
            StringBuilder m_ValueString = new StringBuilder();
            if (mySwitch != null && mySwitch.Switch != null)
            {
                m_ValueString.Append("{\"Id\":\"" + mySwitch.Id +
                                                "\",\"Name\":\"" + mySwitch.Name +
                                                "\",\"DepthIndex\":" + mySwitch.DepthIndex +
                                                ",\"MaxDepth\":" + mySwitch.MaxDepth +
                                                ",\"CollectorCount\":" + mySwitch.CollectorCount +
                                                ",\"PropertyName\":\"" + mySwitch.PropertyName +
                                                "\",\"Switch\":");
                /////////////////////////////////获得子交换机//////////////////////////
                string m_SubSwitchValueTemp = "";
                for (int i = 0; i < mySwitch.Switch.Count; i++)
                {
                    if (i == 0)
                    {
                        m_SubSwitchValueTemp = GetSwitchValueString(mySwitch.Switch[i]);
                    }
                    else
                    {
                        m_SubSwitchValueTemp = m_SubSwitchValueTemp + "," + GetSwitchValueString(mySwitch.Switch[i]);
                    }
                }
                if (m_SubSwitchValueTemp != "")
                {
                    m_ValueString.Append("[" + m_SubSwitchValueTemp + "]");
                }
                else
                {
                    m_ValueString.Append("[]");
                }
                ////////////////////////////////获得设备////////////////////////////////
                string m_CollectorValueTemp = GetCollectorValueString(mySwitch);
                if (m_CollectorValueTemp != "")
                {
                    m_ValueString.Append(",\"Collector\":" + "[" + m_CollectorValueTemp + "]");
                }
                else
                {
                    m_ValueString.Append(",\"Collector\":[]");
                }

                ///////////////////////////////////////////////////////////////////////
                m_ValueString.Append("}");
            }
            else
            {
                m_ValueString.Append("[]");
            }
            return m_ValueString.ToString();
        }
        public static string GetCollectorValueString(Service.SystemStatus.Model_Switch mySwitch)
        {
            StringBuilder m_ValueString = new StringBuilder();
            if (mySwitch != null && mySwitch.Collector != null)
            {
                for (int i = 0; i < mySwitch.Collector.Count; i++)
                {
                    if (i == 0)
                    {
                        m_ValueString.Append("{\"Id\":\"" + mySwitch.Collector[i].Id +
                                                   "\",\"Name\":\"" + mySwitch.Collector[i].Name +
                                                   "\",\"IpAddress\":\"" + mySwitch.Collector[i].IpAddress +
                                                   "\",\"PropertyName\":\"" + mySwitch.Collector[i].PropertyName +
                                                   "\",\"Type\":\"" + mySwitch.Collector[i].Type + "\"}");
                    }
                    else
                    {
                        m_ValueString.Append(",{\"Id\":\"" + mySwitch.Collector[i].Id +
                                                   "\",\"Name\":\"" + mySwitch.Collector[i].Name +
                                                   "\",\"IpAddress\":\"" + mySwitch.Collector[i].IpAddress +
                                                   "\",\"PropertyName\":\"" + mySwitch.Collector[i].PropertyName +
                                                   "\",\"Type\":\"" + mySwitch.Collector[i].Type + "\"}");
                    }
                }
            }
            else
            {
                m_ValueString.Append("[]");
            }
            return m_ValueString.ToString();
        }

        public static string[] GetFactoryDataBaseArray(string myStationId)
        {
            string m_Condition = "";
            if (myStationId != "zc_nxjc")
            {
                m_Condition = string.Format(" and A.OrganizationID = '{0}'", myStationId);
            }
            string m_Sql = @"Select A.OrganizationID, A.Name, B.MeterDatabase from system_Organization A,system_Database B
                                where A.LevelType = 'Factory'
                                and A.DatabaseID = B.DatabaseID
                                {0}
                                order by A.LevelCode";
            m_Sql = string.Format(m_Sql, m_Condition);
            try
            {
                DataTable m_Result = _dataFactory.Query(m_Sql);
                if (m_Result != null)
                {
                    string[] m_DataBaseName = new string[m_Result.Rows.Count];
                    for (int i = 0; i < m_Result.Rows.Count; i++)
                    {
                        m_DataBaseName[i] = m_Result.Rows[i]["MeterDatabase"].ToString();
                    }
                    return m_DataBaseName;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        public static string GetFactoryDataBase(string myOrganizationId)
        {
            string m_Sql = @"Select A.OrganizationID, A.Name, B.MeterDatabase from system_Organization A,system_Database B
                                where A.LevelType = 'Factory'
                                and A.DatabaseID = B.DatabaseID
                                and A.OrganizationID = '{0}'";
            m_Sql = string.Format(m_Sql, myOrganizationId);
            try
            {
                DataTable m_Result = _dataFactory.Query(m_Sql);
                if (m_Result != null && m_Result.Rows.Count == 1)
                {
                    return m_Result.Rows[0]["MeterDatabase"].ToString();
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        //////////////////////////////////////////////以下是获取数据/////////////////////////////////////////////
        /// <summary>
        /// //////////
        /// </summary>
        /// <param name="myFactoryDataBaseName"></param>
        /// <param name="myOrganizationId"></param>
        /// <param name="myCollectorName"></param>
        /// <param name="myIpAddress"></param>
        /// <returns></returns>
        public static string GetTermimalStatus(string myFactoryDataBaseName, string myOrganizationId, string myCollectorName, string myIpAddress)
        {
            string m_ReturnValueString = "{\"rows\":[],\"total\":0}";
            string m_Sql = @"SELECT A.OrganizationID
                              ,A.AmmeterNumber
                              ,A.AmmeterName
                              ,A.AmmeterAddress
                              ,A.CommPort
	                          ,A.Status
                              ,A.TimeStatusChange
                          FROM {0}.dbo.AmmeterContrast A
                          where A.ElectricRoom = '{1}' 
                          and A.IpAddress = '{2}'
                          and A.OrganizationID = '{3}'
                          and A.EnabledFlag = 1
                          order by A.AmmeterNumber";
            m_Sql = string.Format(m_Sql, myFactoryDataBaseName, myCollectorName, myIpAddress, myOrganizationId);
            try
            {
                DataTable m_Result = _dataFactory.Query(m_Sql);
                m_ReturnValueString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_Result);
            }
            catch
            {
            }
            return m_ReturnValueString;
        }
        public static DataTable GetFactoryNetworkStatus(string[] myOrganizationId, string[] myFactoryDataBaseName, Dictionary<string, bool> myGroupNetworkStatus, Dictionary<string, DateTime> myFactorySoftwareUpdateTime, Dictionary<string, DateTime> myRealtimeDatetime)
        {
            DataTable m_FactoryNodeStatusTable = GetFactoryStatusTable();
            DataTable m_DataBaseRealtimeTable = GetDataBaseRealtime(myOrganizationId, myFactoryDataBaseName);
            GetFactoryNetAndSoftStatus(m_DataBaseRealtimeTable, myGroupNetworkStatus, myFactorySoftwareUpdateTime, myRealtimeDatetime, ref m_FactoryNodeStatusTable);
            return m_FactoryNodeStatusTable;
        }
        public static DataTable GetDataComputerNetworkStatus(DataTable myDataCollectionNetValueTable, Dictionary<string, DataTable> myDataComputerNetworkStatus)
        {
            DataTable m_DataComputerStatusTable = GetDataComputerStatusTable();
            GetDataComputerNetAndSoftStatus(myDataCollectionNetValueTable, myDataComputerNetworkStatus, ref m_DataComputerStatusTable);
            return m_DataComputerStatusTable;
        }
        public static DataTable GetOPCStatus(DataTable myDataCollectionNetValueTable)
        {
            DataTable m_OPCStatusTable = GetOPCStatusTable();
            GetOPCNetAndSoftStatus(myDataCollectionNetValueTable, ref m_OPCStatusTable);
            return m_OPCStatusTable;
        }
        public static DataTable GetAmmeterNetworkStatus(DataTable myDataCollectionNetValueTable)
        {
            DataTable m_AmmeterStatusTable = GetAmmeterStatusTable();
            GetAmmeterNetAndSoftStatus(myDataCollectionNetValueTable, ref m_AmmeterStatusTable);
            return m_AmmeterStatusTable;
        }
        public static void GetDataComputerAndOPCValue(ref DataTable myDataCollectionNetValueTable)
        {
            if(myDataCollectionNetValueTable != null)
            {
                string m_Sql = "";
                myDataCollectionNetValueTable.Columns.Add("SoftwareStatus", typeof(bool));
                myDataCollectionNetValueTable.Columns.Add("NetworkStatus", typeof(bool));
                ///////////////////////////////DataComputer和OPC////////////////////////////////
                DataRow[] m_RealtimeDataTable = myDataCollectionNetValueTable.Select("NodeType in ('DataComputer','OPC')");
                for (int i = 0; i < m_RealtimeDataTable.Length; i++)
                {
                    m_RealtimeDataTable[i]["SoftwareStatus"] = false;
                    m_RealtimeDataTable[i]["NetworkStatus"] = false;
                    string m_RealtimeDataTableItem = m_RealtimeDataTable[i]["RealtimeDataTable"].ToString();
                    if (m_RealtimeDataTableItem != "")
                    {
                        string[] m_RealtimeTableTemp = m_RealtimeDataTableItem.Split(',');
                        for (int j = 0; j < m_RealtimeTableTemp.Length; j++)
                        {
                            if (i == 0 && j == 0)
                            {
                                m_Sql = m_Sql + string.Format(@" Select '{1}' as NodeId, '{2}' as NodeType, '{3}' as OrganizationID, (case when DATEDIFF (minute, getdate(),A.vDate) < 30 then 1 else 0 end) as vDateStatus from {0} A ", m_RealtimeTableTemp[j], m_RealtimeDataTable[i]["NodeId"].ToString(), m_RealtimeDataTable[i]["NodeType"].ToString(), m_RealtimeDataTable[i]["OrganizationID"].ToString());
                            }
                            else
                            {
                                m_Sql = m_Sql + string.Format(@" union all Select '{1}' as NodeId, '{2}' as NodeType, '{3}' as OrganizationID, (case when DATEDIFF (minute, getdate(),A.vDate) < 30 then 1 else 0 end) as vDateStatus from {0} A ", m_RealtimeTableTemp[j], m_RealtimeDataTable[i]["NodeId"].ToString(), m_RealtimeDataTable[i]["NodeType"].ToString(), m_RealtimeDataTable[i]["OrganizationID"].ToString());
                            }
                        }   
                    }
                }
                try
                {
                    DataTable m_Result = _dataFactory.Query(m_Sql);
                    if (m_Result != null)
                    {
                        for (int i = 0; i < m_RealtimeDataTable.Length; i++)
                        {
                            DataRow[] m_RealtimeValueDataFailtRow = m_Result.Select(string.Format("NodeId = '{0}' and NodeType = '{1}' and OrganizationID = '{2}' and vDateStatus = 0", m_RealtimeDataTable[i]["NodeId"].ToString(), m_RealtimeDataTable[i]["NodeType"].ToString(), m_RealtimeDataTable[i]["OrganizationID"].ToString()));
                            DataRow[] m_RealtimeValueDataAllRow = m_Result.Select(string.Format("NodeId = '{0}' and NodeType = '{1}' and OrganizationID = '{2}'", m_RealtimeDataTable[i]["NodeId"].ToString(), m_RealtimeDataTable[i]["NodeType"].ToString(), m_RealtimeDataTable[i]["OrganizationID"].ToString()));
                            if (m_RealtimeValueDataFailtRow.Length == 0 && m_RealtimeValueDataAllRow.Length != 0)       //说明该节点内的数据采集都正常
                            {
                                m_RealtimeDataTable[i]["SoftwareStatus"] = true;
                            }
                            if(m_RealtimeValueDataFailtRow.Length < m_RealtimeValueDataAllRow.Length)      //当不是所有的都不正常,说明网络没问题
                            {
                                m_RealtimeDataTable[i]["NetworkStatus"] = true;
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }
        public static void GetAmmeterValue(string[] myDataBaseName, ref DataTable myDataCollectionNetValueTable)
        {
            ///////////////////////////////Ammeter////////////////////////////////Table.Rows[i]["Status"].ToString() == "正常读取")  
            string m_Sql = "";
            string m_ConditionTemplate = @"Select A.OrganizationID, A.IpAddress, A.ElectricRoom, A.AmmeterNumber, A.AmmeterName, A.Status, A.TimeStatusChange
                                                        from {0}.dbo.AmmeterContrast A where A.EnabledFlag = 1";
            if (myDataBaseName != null && myDataBaseName.Length > 0)
            {
                for (int i = 0; i < myDataBaseName.Length; i++)
                {
                    if (i == 0)
                    {
                        m_Sql = string.Format(m_ConditionTemplate, myDataBaseName[i]);
                    }
                    else
                    {
                        m_Sql = m_Sql + " union all " + string.Format(m_ConditionTemplate, myDataBaseName[i]);
                    }
                }
                m_Sql = string.Format(m_Sql);
                try
                {
                    DataTable m_Result = _dataFactory.Query(m_Sql);
                    if (m_Result != null)
                    {
                        DataRow[] m_TerminalTable = myDataCollectionNetValueTable.Select("NodeType = 'Ammeter'");
                        for (int i = 0; i < m_TerminalTable.Length; i++)
                        {
                            m_TerminalTable[i]["SoftwareStatus"] = false;
                            m_TerminalTable[i]["NetworkStatus"] = false;
                            DataRow[] m_AmmeterItemTerminalRows = m_Result.Select(string.Format("OrganizationID = '{0}' and IpAddress = '{1}' and ElectricRoom = '{2}'"
                                                                                    , m_TerminalTable[i]["OrganizationID"].ToString(), m_TerminalTable[i]["IpAddress"].ToString(), m_TerminalTable[i]["RealtimeDataTable"].ToString()));
                            int m_RightRunCount = 0;
                            for (int j = 0; j < m_AmmeterItemTerminalRows.Length; j++)
                            {
                                if (m_AmmeterItemTerminalRows[j]["Status"].ToString() == "正常读取")
                                {
                                    m_RightRunCount = m_RightRunCount + 1;
                                }
                            }
                            if (m_RightRunCount == m_AmmeterItemTerminalRows.Length && m_AmmeterItemTerminalRows.Length != 0)      //当所有电表都可以读取
                            {
                                m_TerminalTable[i]["SoftwareStatus"] = true;
                            }
                            if (m_RightRunCount > 0)                    //只要有一块表正常读取,就认为网络正常
                            {
                                m_TerminalTable[i]["NetworkStatus"] = true;
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }
        private static DataTable GetDataBaseRealtime(string[] myOrganizationId, string[] myFactoryDataBaseName)
        {
            string m_Sql = "";
            string m_ConditionTemplate = @"SELECT A.vDate, '{1}' as OrganizationID FROM {0} A";
            if (myFactoryDataBaseName != null && myFactoryDataBaseName.Length > 0)
            {
                for (int i = 0; i < myFactoryDataBaseName.Length; i++)
                {
                    if (i == 0)
                    {
                        m_Sql = string.Format(m_ConditionTemplate, myFactoryDataBaseName[i], myOrganizationId[i]);
                    }
                    else
                    {
                        m_Sql = m_Sql + " union all " + string.Format(m_ConditionTemplate, myFactoryDataBaseName[i], myOrganizationId[i]);
                    }
                }
                m_Sql = string.Format(m_Sql);
                try
                {
                    DataTable m_Result = _dataFactory.Query(m_Sql);
                    return m_Result;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        private static void GetFactoryNetAndSoftStatus(DataTable m_DataBaseRealtimeTable, Dictionary<string, bool> myGroupNetworkStatus, Dictionary<string, DateTime> myFactorySoftwareUpdateTime, Dictionary<string, DateTime> myRealtimeDatetime, ref DataTable myFactoryNodeStatus)
        {
            foreach (KeyValuePair<string, DateTime> myItem in myFactorySoftwareUpdateTime)
            {
                bool m_FactoryNetworkStatus = false;
                bool m_FactorySoftwareStatus = false;
                bool m_FactorySynchronizationStatus = false;
                if (DateTime.Now <= myItem.Value.AddMinutes(ValidDelayTime))         //如果数采软件不更新了,说明网络断了,那么
                {
                    m_FactoryNetworkStatus = true;
                    if (myGroupNetworkStatus.ContainsKey(myItem.Key))
                    {
                        m_FactorySoftwareStatus = myGroupNetworkStatus[myItem.Key];
                    }
                    if (myRealtimeDatetime.ContainsKey(myItem.Key))
                    {
                        m_FactorySynchronizationStatus = GetFactorySynchronizationStatus(m_DataBaseRealtimeTable, myItem.Key, myRealtimeDatetime[myItem.Key]);
                    }
                }
                myFactoryNodeStatus.Rows.Add(myItem.Key, m_FactoryNetworkStatus, m_FactorySoftwareStatus, m_FactorySynchronizationStatus);
            }
            //myFactoryNodeStatus.Rows.Add("zc_nxjc_qtx_efc", false, true, true);
        }
        
        private static bool GetFactorySynchronizationStatus(DataTable myDataBaseRealtimeTable, string myOrganizationId, DateTime myRealtimeDatetime)
        {
            DataRow[] m_DataBaseRealtimeRow = myDataBaseRealtimeTable.Select(string.Format("OrganizationID = '{0}'", myOrganizationId));
            if (m_DataBaseRealtimeRow.Length > 0)
            {
                if ((DateTime)m_DataBaseRealtimeRow[0]["DataUpdateTime"] > myRealtimeDatetime.AddMinutes(ValidDelayTime))     //通过对比集团和分厂数据库实时数据的更新时间判断同步是否正常,如果在分厂，则同步显示始终正常
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        private static void GetDataComputerNetAndSoftStatus(DataTable myDataCollectionNetValueTable, Dictionary<string, DataTable> myDataComputerNetworkStatus, ref DataTable myDataComputerNetworkStatusTable)
        {
            foreach (KeyValuePair<string, DataTable> myItem in myDataComputerNetworkStatus)
            {
                if (myItem.Value != null)
                {
                    bool m_SoftwareStatus = false;
                    if (myDataCollectionNetValueTable != null)
                    {
                        for (int i = 0; i < myDataCollectionNetValueTable.Rows.Count; i++)
                        {
                            if (myItem.Key == myDataCollectionNetValueTable.Rows[i]["OrganizationID"].ToString() + "_" + myDataCollectionNetValueTable.Rows[i]["NodeId"].ToString())
                            {
                                m_SoftwareStatus = (bool)myDataCollectionNetValueTable.Rows[i]["SoftwareStatus"];
                                break;
                            }
                        }
                    }
                    for (int i = 0; i < myItem.Value.Rows.Count; i++)
                    {
                        myDataComputerNetworkStatusTable.Rows.Add(myItem.Key, (bool)myItem.Value.Rows[i]["NetworkStatus"], m_SoftwareStatus);
                    }
                }
            }
        }
        private static void GetOPCNetAndSoftStatus(DataTable myDataCollectionNetValueTable, ref DataTable m_OPCStatusTable)
        {
            if(myDataCollectionNetValueTable != null)
            {
                bool m_SoftwareStatus = false;
                bool m_NetworkStatus = false;
                DataRow[] m_OPCNetRows = myDataCollectionNetValueTable.Select(string.Format("NodeType = 'OPC'"));
                for (int i = 0; i < m_OPCNetRows.Length; i++)
                {
                    m_SoftwareStatus = (bool)m_OPCNetRows[i]["SoftwareStatus"];
                    m_NetworkStatus = (bool)m_OPCNetRows[i]["NetworkStatus"];
                    m_OPCStatusTable.Rows.Add(m_OPCNetRows[i]["OrganizationID"].ToString() + "_" + m_OPCNetRows[i]["NodeId"].ToString(), m_NetworkStatus, m_SoftwareStatus);
                }
            }
        }
        private static void GetAmmeterNetAndSoftStatus(DataTable myDataCollectionNetValueTable, ref DataTable m_AmmeterStatusTable)
        {
            if (myDataCollectionNetValueTable != null)
            {
                bool m_SoftwareStatus = false;
                bool m_NetworkStatus = false;
                DataRow[] m_AmmeterNetRows = myDataCollectionNetValueTable.Select(string.Format("NodeType = 'Ammeter'"));
                for (int i = 0; i < m_AmmeterNetRows.Length; i++)
                {
                    if (m_AmmeterNetRows[i]["SoftwareStatus"] != DBNull.Value)
                    {
                        m_SoftwareStatus = (bool)m_AmmeterNetRows[i]["SoftwareStatus"];
                    }
                    if (m_AmmeterNetRows[i]["NetworkStatus"] != DBNull.Value)
                    {
                        m_NetworkStatus = (bool)m_AmmeterNetRows[i]["NetworkStatus"];
                    }
                    m_AmmeterStatusTable.Rows.Add(m_AmmeterNetRows[i]["OrganizationID"].ToString() + "_" + m_AmmeterNetRows[i]["NodeId"].ToString(), m_NetworkStatus, m_SoftwareStatus);
                }
            }
        }
        private static DataTable GetFactoryStatusTable()
        {
            //////分厂到集团网络状态和分厂采集软件状态
            DataTable m_FactoryNodeStatus = new DataTable();
            m_FactoryNodeStatus.Columns.Add("Id", typeof(string));
            m_FactoryNodeStatus.Columns.Add("NetworkStatus", typeof(bool));
            m_FactoryNodeStatus.Columns.Add("SoftwareStatus", typeof(bool));
            m_FactoryNodeStatus.Columns.Add("SynchronizationStatus", typeof(bool));
            return m_FactoryNodeStatus;

        }
        private static DataTable GetDataComputerStatusTable()
        {
            //////采集计算机到分厂的网络状态
            DataTable m_DataComputerNetworkStatus = new DataTable();
            m_DataComputerNetworkStatus.Columns.Add("Id", typeof(string));
            m_DataComputerNetworkStatus.Columns.Add("NetworkStatus", typeof(bool));
            m_DataComputerNetworkStatus.Columns.Add("SoftwareStatus", typeof(bool));
            return m_DataComputerNetworkStatus;
        }
        private static DataTable GetOPCStatusTable()
        {
            //////采集计算机到分厂的网络状态
            DataTable m_OPCNetworkStatus = new DataTable();
            m_OPCNetworkStatus.Columns.Add("Id", typeof(string));
            m_OPCNetworkStatus.Columns.Add("NetworkStatus", typeof(bool));
            m_OPCNetworkStatus.Columns.Add("SoftwareStatus", typeof(bool));
            return m_OPCNetworkStatus;
        }
        private static DataTable GetAmmeterStatusTable()
        {
            DataTable m_AmmeterNetworkStatus = new DataTable();
            m_AmmeterNetworkStatus.Columns.Add("Id", typeof(string));
            m_AmmeterNetworkStatus.Columns.Add("NetworkStatus", typeof(bool));
            m_AmmeterNetworkStatus.Columns.Add("SoftwareStatus", typeof(bool));
            return m_AmmeterNetworkStatus;
        }

    }

}
