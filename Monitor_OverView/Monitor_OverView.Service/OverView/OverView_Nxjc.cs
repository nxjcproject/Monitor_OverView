using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SqlServerDataAdapter;

namespace Monitor_OverView.Service.OverView
{
    public class OverView_Nxjc
    {
        private static readonly ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(Monitor_OverView.Infrastruture.Configuration.ConnectionStringFactory.NXJCConnectionString);

        public OverView_Nxjc()
        {
        }
        public static string GetStationId()
        {
            return Monitor_OverView.Infrastruture.Configuration.ConnectionStringFactory.StationId;
        }
        public static string GetRealTimeData(string myStartTime, string myEndTime)
        {
            DataTable m_DisplayDataTable = GetDisplayDataTable();
            string[][] m_Organizations = new string[10][];
            m_Organizations[0] = new string[]{"银川水泥","zc_nxjc_ychc"};
            m_Organizations[1] = new string[]{"石嘴山水泥","zc_nxjc_shzhshc"};
            m_Organizations[2] = new string[]{"青铜峡水泥","zc_nxjc_qtx"};
            m_Organizations[3] = new string[]{"中宁水泥","zc_nxjc_zhnc"};
            m_Organizations[4] = new string[]{"六盘山水泥","zc_nxjc_lpsc"};
            m_Organizations[5] = new string[]{"天水水泥","zc_nxjc_tsc"};
            m_Organizations[6] = new string[]{"白银水泥","zc_nxjc_byc"};
            m_Organizations[7] = new string[]{"乌海赛马","zc_nxjc_whsmc"};
            m_Organizations[8] = new string[]{"乌海西水","zc_nxjc_whxsc"};
            m_Organizations[9] = new string[]{"喀喇沁水泥","zc_nxjc_klqc"};
            for (int i = 0; i < 10; i++)
            {
                m_DisplayDataTable.Rows.Add(m_Organizations[i][0], m_Organizations[i][1], 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m, 0.0m);
            }
            string m_JsonString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_DisplayDataTable);
            return m_JsonString;
        }
        private static DataTable GetDisplayDataTable()
        {
            DataTable m_DataTable = new DataTable();
            m_DataTable.Columns.Add("DataZoneTitle", typeof(string));
            m_DataTable.Columns.Add("DataZoneTitleId", typeof(string));
            m_DataTable.Columns.Add("Data1", typeof(decimal));
            m_DataTable.Columns.Add("Data2", typeof(decimal));
            m_DataTable.Columns.Add("Data3", typeof(decimal));
            m_DataTable.Columns.Add("Data4", typeof(decimal));
            m_DataTable.Columns.Add("Data5", typeof(decimal));
            m_DataTable.Columns.Add("Data6", typeof(decimal));
            m_DataTable.Columns.Add("Data7", typeof(decimal));
            m_DataTable.Columns.Add("Data8", typeof(decimal));
            return m_DataTable;
        }
    }
}
