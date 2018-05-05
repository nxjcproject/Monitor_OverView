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
        private const string Ammeter = "Ammeter";
        private const string AmmeterS = "AmmeterS";   //单块电表
        private const string Network = "Network";
        private const string OPC = "OPC";
        private const string Software = "Software";
        public static DataTable GetFactoryServerInfo(string myStationId)
        {
            string m_Condition = "";
            if (myStationId != "zc_nxjc")
            {
                m_Condition = string.Format(" and E.OrganizationID = '{0}'", myStationId);
            }
            else
            {
                m_Condition = "and A.OrganizationID = E.OrganizationID";
            }
            string m_Sql = @"Select A.OrganizationID as OrganizationId, case when C.Name is null then '' else C.Name end + case when A.Name is null then '' else A.Name end as Name
                                    , B.MeterDatabase, D.NodeId, D.NodeType, D.IpAddress from system_Organization A
                                left join system_Organization C on C.LevelCode = substring(A.LevelCode, 1,len(A.LevelCode) - 2)
                                ,system_Database B, net_DataCollectionNet D, system_Organization E
                                where A.LevelType = 'Factory'
                                and A.DatabaseID = B.DatabaseID
                                and D.NodeType = 'FactoryServer'
                                and D.OrganizationID = A.OrganizationID
                                and CHARINDEX(E.LevelCode,A.LevelCode) > 0
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
        public static DataSet GetStatusDataSet(Dictionary<string, List<string>> myStatusBuffer, Dictionary<string, string> myTimeStampBuffer)
        {
            DataSet m_StatusDataSet = new DataSet();
            DataTable m_FactoryNodeStatusTable = GetFactorySynchronizationStatusTable(myTimeStampBuffer);
            DataTable m_NormalNodeStatusTable = GetNormalNodeStatusTable();
            if (myStatusBuffer != null)
            {
                foreach (string myKey in myStatusBuffer.Keys)
                {
                    if (myStatusBuffer[myKey] != null)
                    {
                        for (int i = 0; i < myStatusBuffer[myKey].Count; i++)
                        {
                            string[] m_SuatusDataTemp = myStatusBuffer[myKey][i].Split(';');
                            if (m_SuatusDataTemp.Length == 3)
                            {
                                int m_FactoryServerInex = ContainIdInTable(m_SuatusDataTemp[0], m_FactoryNodeStatusTable);
                                int m_NormalNodeIndex = ContainIdInTable(m_SuatusDataTemp[0], m_NormalNodeStatusTable);
                                if (m_FactoryServerInex != -1)
                                {
                                    if (m_SuatusDataTemp[1] == Software)
                                    {
                                        m_FactoryNodeStatusTable.Rows[m_FactoryServerInex]["SoftwareStatus"] = m_SuatusDataTemp[0] == "1" ? true : false;
                                    }
                                    else if (m_SuatusDataTemp[1] == Network)
                                    {
                                        m_FactoryNodeStatusTable.Rows[m_FactoryServerInex]["NetworkStatus"] = m_SuatusDataTemp[0] == "1" ? true : false;
                                    }
                                }
                                else if (m_NormalNodeIndex != -1)
                                {
                                    if (m_SuatusDataTemp[1] == Software || m_SuatusDataTemp[1] == Ammeter || m_SuatusDataTemp[1] == OPC)
                                    {
                                        m_NormalNodeStatusTable.Rows[m_NormalNodeIndex]["SoftwareStatus"] = m_SuatusDataTemp[0] == "1" ? true : false;
                                    }
                                    else if (m_SuatusDataTemp[1] == AmmeterS)
                                    {
                                        m_NormalNodeStatusTable.Rows[m_NormalNodeIndex]["SubNodeStatus"] = m_SuatusDataTemp[0] == "1" ? true : false;
                                    }
                                    else if (m_SuatusDataTemp[1] == Network)
                                    {
                                        m_NormalNodeStatusTable.Rows[m_NormalNodeIndex]["NetworkStatus"] = m_SuatusDataTemp[0] == "1" ? true : false;
                                    }
                                }
                                else if (m_NormalNodeIndex == -1)
                                {
                                    DataRow m_NewDataRow = m_NormalNodeStatusTable.NewRow();
                                    m_NewDataRow["Id"] = m_SuatusDataTemp[0];
                                    m_NewDataRow["SoftwareStatus"] = true;
                                    m_NewDataRow["NetworkStatus"] = true;
                                    m_NewDataRow["SubNodeStatus"] = true;
                                    if (m_SuatusDataTemp[1] == Software || m_SuatusDataTemp[1] == Ammeter || m_SuatusDataTemp[1] == OPC)
                                    {
                                        m_NewDataRow["SoftwareStatus"] = m_SuatusDataTemp[0] == "1" ? true : false;
                                    }
                                    else if (m_SuatusDataTemp[1] == AmmeterS)
                                    {
                                        m_NewDataRow["SubNodeStatus"] = m_SuatusDataTemp[0] == "1" ? true : false;
                                    }
                                    else if (m_SuatusDataTemp[1] == Network)
                                    {
                                        m_NewDataRow["NetworkStatus"] = m_SuatusDataTemp[0] == "1" ? true : false;
                                    }
                                    m_NormalNodeStatusTable.Rows.Add(m_NewDataRow);
                                }
                            }
                        }
                    }
                }
            }
            m_StatusDataSet.Tables.Add(m_FactoryNodeStatusTable);
            m_StatusDataSet.Tables.Add(m_NormalNodeStatusTable);
            return m_StatusDataSet;
        }
        private static DataTable GetFactorySynchronizationStatusTable(Dictionary<string, string> myTimeStampBuffer)
        {
            DataTable m_FactorySynchronizationStatusTable = GetFactoryStatusTable();
            if (myTimeStampBuffer != null)
            {
                DataTable m_FactoryDataBaseTable = GetFactoryDataBase(myTimeStampBuffer);
                if (m_FactoryDataBaseTable != null)
                {
                    DataTable m_FactoryUpateTimeTable = GetFactoryUpateTime(m_FactoryDataBaseTable);
                    if (myTimeStampBuffer != null && m_FactoryUpateTimeTable != null)
                    {
                        for (int i = 0; i < m_FactoryUpateTimeTable.Rows.Count; i++)
                        {
                            string m_OrganizationId = m_FactoryUpateTimeTable.Rows[i]["OrganizationId"].ToString();
                            if (myTimeStampBuffer.ContainsKey(m_OrganizationId))             //如果传过来的信息有该
                            {
                                DateTime m_LocalUpdateTime = (DateTime)m_FactoryUpateTimeTable.Rows[i]["vDate"];
                                DateTime m_RemoteUpateTime = DateTime.Parse(myTimeStampBuffer[m_OrganizationId]);
                                bool m_SynchronizationStatus = true;
                                if (m_RemoteUpateTime > m_LocalUpdateTime.AddMinutes(30))
                                {
                                    m_SynchronizationStatus = false;
                                }
                                m_FactorySynchronizationStatusTable.Rows.Add(new object[] { m_FactoryUpateTimeTable.Rows[i]["OrganizationId"].ToString() + "_" + m_FactoryUpateTimeTable.Rows[i]["NodeId"].ToString(), true, true, m_SynchronizationStatus });
                            }
                        }
                    }
                }
            }
            return m_FactorySynchronizationStatusTable;
        }
        private static DataTable GetFactoryDataBase(Dictionary<string, string> myTimeStampBuffer)
        {
            string m_OrganizationIds = "''";
            foreach (string myKey in myTimeStampBuffer.Keys)
            {
                if (m_OrganizationIds == "''")
                {
                    m_OrganizationIds = "'" + myKey + "'";
                }
                else
                {
                    m_OrganizationIds = m_OrganizationIds + ",'" + myKey + "'";
                }
            }

            string m_Sql = @"Select A.OrganizationID as OrganizationId, B.MeterDatabase, C.NodeId as Id from system_Organization A, system_Database B, net_DataCollectionNet C
                                where A.OrganizationID in ({0})
                                and A.DatabaseID = B.DatabaseID
                                and A.OrganizationID = C.OrganizationID
                                and C.NodeType = 'FactoryServer'";
            m_Sql = string.Format(m_Sql, m_OrganizationIds);
            try
            {
                DataTable m_FactoryDataBaseResult = _dataFactory.Query(m_Sql);
                return m_FactoryDataBaseResult;               
            }
            catch
            {
                return null;
            }
        }
        private static DataTable GetFactoryUpateTime(DataTable myFactoryDataBaseTable)
        {
            string m_Sql = "";
            string m_DataBaseUpdateTimeTemplate = @"select A.vDate, '{0}' as OrganizationId, B.NodeId from {1}.dbo.RealtimeAmmeter A, net_DataCollectionNet B 
                                                   where B.OrganizationID = '{0}' and B.NodeType = 'FactoryServer'";
            for (int i = 0; i < myFactoryDataBaseTable.Rows.Count; i++)
            {
                if (i == 0)
                {
                    m_Sql = string.Format(m_DataBaseUpdateTimeTemplate, myFactoryDataBaseTable.Rows[i]["OrganizationId"].ToString(), myFactoryDataBaseTable.Rows[i]["MeterDatabase"].ToString());
                }
                else
                {
                    m_Sql = m_Sql + " Union all " + string.Format(m_DataBaseUpdateTimeTemplate, myFactoryDataBaseTable.Rows[i]["OrganizationId"].ToString(), myFactoryDataBaseTable.Rows[i]["MeterDatabase"].ToString());
                }
            }
            if (m_Sql != "")
            {
                try
                {
                    DataTable m_FactoryUpdateTimeTable = _dataFactory.Query(m_Sql);
                    return m_FactoryUpdateTimeTable;
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
        private static int ContainIdInTable(string myKeyId, DataTable myDataTable)
        {
            int m_FindIndex = -1;
            int m_Index = 0;
            while (m_Index < myDataTable.Rows.Count)
            {
                if (myDataTable.Rows[m_Index]["Id"].ToString() == myKeyId)
                {
                    m_FindIndex = m_Index;
                    break;
                }
                else
                {
                    m_Index = m_Index + 1;
                }
            }
            return m_FindIndex;
        }
        
        private static DataTable GetFactoryStatusTable()
        {
            //////分厂到集团网络状态和分厂采集软件状态
            DataTable m_FactoryNodeStatus = new DataTable("FactoryNodeStatus");
            m_FactoryNodeStatus.Columns.Add("Id", typeof(string));
            m_FactoryNodeStatus.Columns.Add("NetworkStatus", typeof(bool));
            m_FactoryNodeStatus.Columns.Add("SoftwareStatus", typeof(bool));
            m_FactoryNodeStatus.Columns.Add("SynchronizationStatus", typeof(bool));
            return m_FactoryNodeStatus;

        }
        private static DataTable GetNormalNodeStatusTable()
        {
            //////采集计算机到分厂的网络状态
            DataTable m_NormalNodeStatusStatus = new DataTable("NormalNodeStatusStatus");
            m_NormalNodeStatusStatus.Columns.Add("Id", typeof(string));
            m_NormalNodeStatusStatus.Columns.Add("NetworkStatus", typeof(bool));
            m_NormalNodeStatusStatus.Columns.Add("SoftwareStatus", typeof(bool));
            m_NormalNodeStatusStatus.Columns.Add("SubNodeStatus", typeof(bool));
            return m_NormalNodeStatusStatus;
        }
        public static string[] GetStatusString(byte[] myStatusBuffer)
        {
            if (myStatusBuffer != null)
            {
                string[] m_StatusString = DataCompression.Function_DefaultCompressionArray.DecompressString(myStatusBuffer);
                return m_StatusString;
            }
            else
            {
                return null;
            }

        }

    }

}
