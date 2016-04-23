using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monitor_OverView.Service.SystemStatus
{
    public class Model_FactoryServer
    {
        private string _Id;
        private string _OrganizationId;
        private string _Name;
        private string _PropertyName;                           //属性名称
        private string _IpAddress;
        private bool _NetworkStatus;
        private bool _SynchronizationStatus;
        private bool _NodeSoftwareStatus;
        private List<Model_DataComputer> _DataComputer;
        public Model_FactoryServer()
        {
            _Id = "";
            _OrganizationId = "";
            _Name = "";
            _PropertyName = "FactoryServer";
            _IpAddress = "";
            _NetworkStatus = false;
            _SynchronizationStatus = false;
            _NodeSoftwareStatus = false;
            _DataComputer = new List<Model_DataComputer>();
        }
        public string Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }
        public string OrganizationId
        {
            get
            {
                return _OrganizationId;
            }
            set
            {
                _OrganizationId = value;
            }

        }
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }
        public string PropertyName
        {
            get
            {
                return _PropertyName;
            }
        }
        public string IpAddress
        {
            get
            {
                return _IpAddress;
            }
            set
            {
                _IpAddress = value;
            }
        }
        public bool NetworkStatus
        {
            get
            {
                return _NetworkStatus;
            }
            set
            {
                _NetworkStatus = value;
            }
        }
        public bool SynchronizationStatus
        {
            get
            {
                return _SynchronizationStatus;
            }
            set
            {
                _SynchronizationStatus = value;
            }
        }
        public bool NodeSoftwareStatus
        {
            get
            {
                return _NodeSoftwareStatus;
            }
            set
            {
                _NodeSoftwareStatus = value;
            }
        }
        public List<Model_DataComputer> DataComputer
        {
            get
            {
                return _DataComputer;
            }
            set
            {
                _DataComputer = value;
            }
        }
    }
    public class Model_DataComputer
    {
        private string _Id;
        private string _Name;
        private string _PropertyName;
        private string _DataSourceType;               //数据源类型(OPC\ELC串口服务器\EAS易思\DLQ电力需求侧)
        private string _IpAddress;
        private bool _NetworkStatus;
        private bool _NodeSoftwareStatus;       //数据采集软件
        private Model_Switch _Switch;     //连接的交换机
        public Model_DataComputer()
        {
            _Id = "";
            _Name = "";
            _PropertyName = "DataComputer";
            _DataSourceType = "";
            _IpAddress = "";
            _NetworkStatus = false;
            _NodeSoftwareStatus = false;       //数据采集软件
            _Switch = new Model_Switch();
        }
        public string Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }
        public string PropertyName
        {
            get
            {
                return _PropertyName;
            }
        }
        public string DataSourceType
        {
            get
            {
                return _DataSourceType;
            }
            set
            {
                _DataSourceType = value;
            }
        }
        public string IpAddress
        {
            get
            {
                return _IpAddress;
            }
            set
            {
                _IpAddress = value;
            }
        }
        public bool NetworkStatus
        {
            get
            {
                return _NetworkStatus;
            }
            set
            {
                _NetworkStatus = value;
            }
        }
        public bool NodeSoftwareStatus
        {
            get
            {
                return _NodeSoftwareStatus;
            }
            set
            {
                _NodeSoftwareStatus = value;
            }
        }
        public Model_Switch Switch
        {
            get
            {
                return _Switch;
            }
            set
            {
                _Switch = value;
            }
        }
    }
    public class Model_Switch
    {
        private string _Id;
        private string _Name;
        private string _PropertyName;                           //属性名称
        private int _DepthIndex;
        private int _MaxDepth;
        private int _CollectorCount;
        private bool _NodeSoftwareStatus;
        private List<Model_Switch> _Switch;     //连接的交换机
        private List<Model_Collector> _Collector;
        public Model_Switch()
        {
            _Id = "";
            _Name = "";
            _PropertyName = "Switch";
            _DepthIndex = 1;
            _MaxDepth = 1;
            _CollectorCount = 0;
            _NodeSoftwareStatus = true;
        }
        public string Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }
        public string PropertyName
        {
            get
            {
                return _PropertyName;
            }
        }
        public int DepthIndex
        {
            get
            {
                return _DepthIndex;
            }
            set
            {
                _DepthIndex = value;
            }
        }
        public int MaxDepth
        {
            get
            {
                return _MaxDepth;
            }
            set
            {
                _MaxDepth = value;
            }
        }
        public int CollectorCount
        {
            get
            {
                return _CollectorCount;
            }
            set
            {
                _CollectorCount = value;
            }
        }
        public bool NodeSoftwareStatus
        {
            get
            {
                return _NodeSoftwareStatus;
            }
            set
            {
                _NodeSoftwareStatus = value;
            }
        }
        public List<Model_Switch> Switch
        {
            get
            {
                return _Switch;
            }
            set
            {
                _Switch = value;
            }
        }
        public List<Model_Collector> Collector
        {
            get
            {
                return _Collector;
            }
            set
            {
                _Collector = value;
            }
        }
    }
    public class Model_Collector
    {
        private string _Id;
        private string _Name;
        private string _PropertyName;
        private string _Type;                           //数据源类型(OPC\ELC串口服务器\EAS易思\DLQ电力需求侧)
        private string _IpAddress;
        private bool _NetworkStatus;
        public Model_Collector()
        {
            _Id = "";
            _Name = "";
            _PropertyName = "Collector";
            _Type = "";
            _IpAddress = "";
            _NetworkStatus = false;
        }
        public string Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }
        public string PropertyName
        {
            get
            {
                return _PropertyName;
            }
        }
        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                _Type = value;
            }
        }
        public string IpAddress
        {
            get
            {
                return _IpAddress;
            }
            set
            {
                _IpAddress = value;
            }
        }
        public bool NetworkStatus
        {
            get
            {
                return _NetworkStatus;
            }
            set
            {
                _NetworkStatus = value;
            }
        }
    }
}
