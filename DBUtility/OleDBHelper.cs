using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Configuration;

namespace DBUtility
{
    public class OleDBHelper
    {
        private OleDbConnection conn = null;
        private int cmdTimeOut = 0;

        public OleDBHelper()
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            conn = new OleDbConnection(connStr);
            conn.Open();
        }

        public void Close()
        {
            if (!object.Equals(conn, null))
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        public DataSet Query(string sql, OleDbParameter[] paramArr)
        {
            DataSet ds = new DataSet();
            if (conn.State != ConnectionState.Open) conn.Open();
            OleDbCommand cmd = new OleDbCommand(sql, conn);
            cmd.CommandTimeout = cmdTimeOut;
            if (!object.Equals(paramArr, null))
            {
                foreach (OleDbParameter param in paramArr)
                {
                    cmd.Parameters.Add(param);
                }
            }
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
            da.Fill(ds);
            cmd.Parameters.Clear();
            return ds;
        }

        public void ExecuteSQL(string sql, OleDbParameter[] paramArr)
        {
            if (conn.State != ConnectionState.Open) conn.Open();
            OleDbCommand cmd = new OleDbCommand(sql, conn);
            if (!object.Equals(paramArr, null))
            {
                foreach (OleDbParameter param in paramArr)
                {
                    cmd.Parameters.Add(param);
                }
            }
            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
        }

        public object GetSingle(string sql, OleDbParameter[] paramArr)
        {
            object obj = null;
            if (conn.State != ConnectionState.Open) conn.Open();
            OleDbCommand cmd = new OleDbCommand(sql, conn);
            cmd.CommandTimeout = cmdTimeOut;
            if (!object.Equals(paramArr, null))
            {
                foreach (OleDbParameter param in paramArr)
                {
                    cmd.Parameters.Add(param);
                }
            }
            obj = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return obj;
        }
    }
}
