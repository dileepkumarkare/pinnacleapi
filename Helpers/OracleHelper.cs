using DevExpress.CodeParser;
using System.Data;

namespace Pinnacle.Helpers
{
    public class OracleHelper
    {
        static Oracle.ManagedDataAccess.Client.OracleConnection con = new Oracle.ManagedDataAccess.Client.OracleConnection();
        static Oracle.ManagedDataAccess.Client.OracleCommand orcl_cmd;
        static Oracle.ManagedDataAccess.Client.OracleDataAdapter _dataAdapter;
        private string _str_con;
        public OracleHelper()
        {
            _str_con = "User Id=MEDINEED;Password=WHIZKIDS;Data Source=10.10.10.21:1521/orcl";

        }
        public DataSet GetLiveInventory()
        {
            con.ConnectionString = _str_con;
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            orcl_cmd = new Oracle.ManagedDataAccess.Client.OracleCommand("WITH CTE AS (SELECT DISTINCT A.ITEMCD,SBATCHNO,PKGST_sTOCK.FUNST_ITEMSTOCK_NEW(A.COMPANYCD,LOCATIONCD,A.STOCKPOINTCD,A.ITEMCD,A.SBATCHNO,TRUNC(SYSDATE)) PRESENT_sTOCK FROM STITEM2 A INNER JOIN VWST_STITEM0 B ON A.ITEMCD=B.ITEMCD WHERE STOCKPOINTCD = 'DE045' AND CURRYEAR = 2025 AND ITEMLEVEL0='MEDICAL') SELECT SUM(PRESENT_sTOCK)OnHandQty,ItemCd FROM CTE WHERE PRESENT_sTOCK >0 GROUP BY ITEMCD ORDER BY ITEMCD", con);
            Oracle.ManagedDataAccess.Client.OracleDataAdapter orcl_da = new Oracle.ManagedDataAccess.Client.OracleDataAdapter(orcl_cmd);
            DataSet dsinventory = new DataSet();
            orcl_da.Fill(dsinventory);
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            return dsinventory;
        }

    }
}
