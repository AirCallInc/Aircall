using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace DBUtility
{
    public class SQLDBHelper
    {
        private SqlConnection conn = null;
        private int cmdTimeOut = 0;

        public SQLDBHelper()
        {
            string connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            conn = new SqlConnection(connStr);
            conn.Open();
        }

        public SQLDBHelper(string connStr)
        {
            conn = new SqlConnection(connStr);
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

        public DataSet Query(string sql, SqlParameter[] paramArr)
        {
            DataSet ds = new DataSet();
            if (conn.State != ConnectionState.Open) conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandTimeout = cmdTimeOut;
            if (!object.Equals(paramArr, null))
            {
                foreach (SqlParameter param in paramArr)
                {
                    cmd.Parameters.Add(param);
                }
            }
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            cmd.Parameters.Clear();
            return ds;
        }

        public DataSet QueryBySP(string sp, SqlParameter[] paramArr)
        {
            DataSet ds = new DataSet();
            if (conn.State != ConnectionState.Open) conn.Open();
            SqlCommand cmd = new SqlCommand(sp, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = cmdTimeOut;
            if (!object.Equals(paramArr, null))
            {
                foreach (SqlParameter param in paramArr)
                {
                    cmd.Parameters.Add(param);
                }
            }
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);
            cmd.Parameters.Clear();
            return ds;
        }

        public void ExecuteSQL(string sql, SqlParameter[] paramArr)
        {
            if (conn.State != ConnectionState.Open) conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            if (!object.Equals(paramArr, null))
            {
                foreach (SqlParameter param in paramArr)
                {
                    cmd.Parameters.Add(param);
                }
            }
            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
        }

        public object GetSingle(string sql, SqlParameter[] paramArr)
        {
            object obj = null;
            if (conn.State != ConnectionState.Open) conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandTimeout = cmdTimeOut;
            if (!object.Equals(paramArr, null))
            {
                foreach (SqlParameter param in paramArr)
                {
                    cmd.Parameters.Add(param);
                }
            }
            obj = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return obj;
        }

        public bool Exists(string sql, SqlParameter[] paramArr)
        {
            object obj = GetSingle(sql, paramArr);
            if (object.Equals(obj, null)) return false;
            if (object.Equals(obj, DBNull.Value)) return false;
            int temp = System.Convert.ToInt32(obj);
            if (temp > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int ExecuteSqlTran(List<CommandInfo> cmdList)
        {
            if (conn.State != ConnectionState.Open) conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandTimeout = cmdTimeOut;
            cmd.Connection = conn;
            SqlTransaction tx = conn.BeginTransaction();
            cmd.Transaction = tx;
            try
            {
                int intCount = 0;
                foreach (CommandInfo cmdInfo in cmdList)
                {
                    cmd.CommandText = cmdInfo.CmdText;
                    if (!object.Equals(cmdInfo.ParamArr, null))
                    {
                        foreach (SqlParameter param in cmdInfo.ParamArr)
                        {
                            cmd.Parameters.Add(param);
                        }
                    }
                    intCount += cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
                tx.Commit();
                return intCount;
            }
            catch (Exception ex)
            {
                tx.Rollback();
                throw ex;
            }
        }

        public int ExecuteStoreProcedureTran(List<CommandInfo> cmdList)
        {
            if (conn.State != ConnectionState.Open) conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = cmdTimeOut;
            cmd.Connection = conn;
            SqlTransaction tx = conn.BeginTransaction();
            cmd.Transaction = tx;
            try
            {
                int intCount = 0;
                foreach (CommandInfo cmdInfo in cmdList)
                {
                    cmd.CommandText = cmdInfo.CmdText;
                    if (!object.Equals(cmdInfo.ParamArr, null))
                    {
                        foreach (SqlParameter param in cmdInfo.ParamArr)
                        {
                            cmd.Parameters.Add(param);
                        }
                    }
                    intCount += cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
                tx.Commit();
                return intCount;
            }
            catch (Exception ex)
            {
                tx.Rollback();
                throw ex;
            }
        }
    }

    public class CommandInfo
    {
        public string CmdText = string.Empty;
        public SqlParameter[] ParamArr = null;

        public CommandInfo(string cmd_text, SqlParameter[] param_arr)
        {
            CmdText = cmd_text;
            ParamArr = param_arr;
        }
    }
}
