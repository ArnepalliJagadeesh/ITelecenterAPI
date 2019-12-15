using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITelecenter.Utility
{
    public class DapperUtility
    {
        public static List<T> GetListDataUsingStoreProc<T>(SqlConnection con, string SqlQuery, DynamicParameters p)
        {
            List<T> res = new List<T>();
            res = con.Query<T>(SqlQuery, p, commandType: CommandType.StoredProcedure).ToList();
            return res;
        }

        public static T GetDataUsingStoreProc<T>(SqlConnection con, string SqlQuery, DynamicParameters p = null)
        {
            T res = con.Query<T>(SqlQuery, p, commandType: CommandType.StoredProcedure).FirstOrDefault();
            return res;
        }
        public static bool UpdateUsingStoreProc(SqlConnection con, string SqlQuery, DynamicParameters p = null)
        {
            bool status = false;
            con.Execute(SqlQuery, p, commandType: CommandType.StoredProcedure);
            status = true;
            return status;
        }
    }
}
