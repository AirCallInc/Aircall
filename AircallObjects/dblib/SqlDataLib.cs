using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Collections.Specialized;
using NLog;
using LogUtility;

namespace ZWT.DbLib
{
	/// <summary>
	/// SqlPortalDAL : this class implements the IPortalDAL interface
	/// </summary>
	public class SqlDataLib : IDataLib
	{
        private LogHelper _logHelper = new LogHelper();
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        #region "private members of this implementation"
        private static string _connString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();// ConfigurationSettings.AppSettings["ConnectionString"];
        //private static string _connString = ConfigurationManager;
			private SqlConnection _sqlConn;
			private SqlTransaction _sqlTrans;
			private SqlCommand _sqlCmd;						//used only for parameters
			private SqlParameterCollection _sqlParameters;	//will hold only from _sqlCmd
		#endregion

		public SqlDataLib()
		{
			//
			// TODO: Add constructor logic here
			//
		}


		#region IPortalDAL Members

		/// <summary>
		/// this will create a new instance of connection if needed 
		/// and opens the database connection.
		/// this will throw exception if error occurs.
		/// </summary>
		public IDbConnection OpenConnection()
		{
			if(_sqlConn == null)
				_sqlConn = new SqlConnection();
			if(_sqlConn.State != ConnectionState.Closed)
				CloseConnection();
			_sqlConn.ConnectionString = _connString;			
			_sqlConn.Open();
			return _sqlConn;
		}//OpenConnection


		/// <summary>
		/// this will close the connection stored in private _sqlConn
		/// this will never throw exception in error case either 
		/// </summary>
		public void CloseConnection()
		{
			try
			{
				if (_sqlTrans != null)
					RollbackTransaction();
				if (_sqlConn != null)
				{
					if (_sqlConn.State != ConnectionState.Closed)
						_sqlConn.Close();
				}
			}
			catch
			{
				//nothing to do here...
			}
			finally
			{
				_sqlConn.Dispose();
			}//finally
		}//CloseConnection


		/// <summary>
		/// starts a new transaction with current open connection
		/// </summary>
		public IDbTransaction BeginTransaction()
		{
			_sqlTrans = _sqlConn.BeginTransaction();
			return _sqlTrans;
		}//BeginTransaction


		/// <summary>
		/// commits current transaction if its started 
		/// throws exception if error
		/// </summary>
		public void CommitTransaction()
		{
			if(_sqlTrans != null)
				_sqlTrans.Commit();
			_sqlTrans.Dispose();
		}//CommitTransaction


		/// <summary>
		/// rolls back transaction
		/// does not throw any exception whether error or not
		/// </summary>
		public void RollbackTransaction()
		{
			try
			{
				if(_sqlTrans != null)
					_sqlTrans.Rollback();
			}
			catch
			{
				//do nothing 
			}
			finally
			{
				_sqlTrans.Dispose();
			}
		}//RollbackTransaction


		/// <summary>
		/// this will run and return results into a new dataset 
		/// </summary>
		/// <param name="vQuery"> the select query </param>
		/// <param name="vDS"> uninitialized data set parameter </param>
		public void RunSelect(string vQuery, ref DataSet vDS)
		{
			SqlCommand	myCommand;
			DataSet		myDS;
			SqlDataAdapter myDataAdapter;

			try
			{
				OpenConnection();
				if(_sqlCmd != null)
					myCommand = _sqlCmd;
				else
					myCommand = new SqlCommand();

				//myCommand = new SqlCommand(vQuery, _sqlConn);
				myCommand.CommandText = vQuery;
				myCommand.Connection = _sqlConn;

				myDS = new DataSet();
				myDataAdapter = new SqlDataAdapter(myCommand);
				myDataAdapter.Fill(myDS);
				vDS = myDS;
			}
			finally
			{
				CloseConnection();
			}
		}//RunSelect


		/// <summary>
		/// this will return the data set containing the records asked for
		/// it will throw exception in case of error
		/// </summary>
		/// <param name="vQuery"> select query </param>
		/// <param name="vDS"> data set with results </param>
		/// <param name="nStart"> starting record number </param>
		/// <param name="nMax"> how many records to return </param>
		/// <param name="tblName"> which table to read </param>
		public void RunSelect(string vQuery, ref DataSet vDS, int nStart, int nMax, string tblName)
		{
			SqlCommand	myCommand;
			DataSet		myDS;
			SqlDataAdapter myDataAdapter;

			try
			{
				OpenConnection();
				if(_sqlCmd != null)
					myCommand = _sqlCmd;
				else
					myCommand = new SqlCommand();

				//myCommand = new SqlCommand(vQuery, _sqlConn);
				myCommand.CommandText = vQuery;
				myCommand.Connection = _sqlConn;

				myDS = new DataSet();
				myDataAdapter = new SqlDataAdapter(myCommand);
				myDataAdapter.Fill(myDS, nStart, nMax, tblName);
				vDS = myDS;
			}
			finally
			{
				CloseConnection();
			}
		}//RunSelect


		/// <summary>
		/// returns the results in a data table instead of dataset
		/// useful when dealing with a single table
		/// </summary>
		/// <param name="vQuery"> select query </param>
		/// <param name="vDT"> results in data table </param>
		public void RunSelect(string vQuery, ref DataTable vDT)
		{
			SqlCommand	myCommand;
			DataTable	myDT;
			SqlDataAdapter myDataAdapter;

			try
			{
				OpenConnection();
				if(_sqlCmd != null)
					myCommand = _sqlCmd;
				else
					myCommand = new SqlCommand();

				//myCommand = new SqlCommand(vQuery, _sqlConn);
				myCommand.CommandText = vQuery;
				myCommand.Connection = _sqlConn;

				myDT = new DataTable();
				myDataAdapter = new SqlDataAdapter(myCommand);
				myDataAdapter.Fill(myDT);
				vDT = myDT;
			}
			finally
			{
				CloseConnection();
			}
		}//RunSelect


		/// <summary>
		/// Runs the select query in the current transaction
		/// </summary>
		/// <param name="vQuery"></param>
		/// <param name="vDT"></param>
		public void RunSelectUseTrans(string vQuery, ref DataTable vDT)
		{
			SqlCommand	myCommand;
			DataTable	myDT;
			SqlDataAdapter myDataAdapter;

			if(_sqlCmd != null)
				myCommand = _sqlCmd;
			else
				myCommand = new SqlCommand();

			//myCommand = new SqlCommand(vQuery, _sqlConn);
			myCommand.CommandText = vQuery;
			myCommand.Connection = _sqlConn;
			myCommand.Transaction = _sqlTrans;

			myDT = new DataTable();
			myDataAdapter = new SqlDataAdapter(myCommand);
			myDataAdapter.Fill(myDT);
			vDT = myDT;
		}//RunSelectUseTrans


		/// <summary>
		/// queries like insert, update, delete to be executed with this
		/// </summary>
		/// <param name="vQuery"></param>
		public int ExeQuery(string vQuery)
		{
			SqlCommand myCommand;
			int rowsAffected;

			//myCommand = new SqlCommand(vQuery, _sqlConn, _sqlTrans);
			if (_sqlCmd != null)
				myCommand = _sqlCmd;
			else
				myCommand = new SqlCommand();
			
			myCommand.CommandText = vQuery;
			myCommand.Connection = _sqlConn;
			myCommand.Transaction = _sqlTrans;
	
			rowsAffected = myCommand.ExecuteNonQuery();
			return rowsAffected;
		}//ExeQuery


		public int ExecScalarUseTrans(string vQuery)
		{
			SqlCommand	myCommand;

			if(_sqlCmd != null)
				myCommand = _sqlCmd;
			else
				myCommand = new SqlCommand();

			//myCommand = new SqlCommand(vQuery, _sqlConn);
			myCommand.CommandText = vQuery;
			myCommand.Connection = _sqlConn;
			myCommand.Transaction = _sqlTrans;
			
			int nRet;
			nRet = Convert.ToInt32(myCommand.ExecuteScalar());
			return nRet;
		}//ExecScalarUseTrans

		
		/// <summary>
		/// runs the query that returns a single int value e.g. count(*)
		/// </summary>
		/// <param name="vQuery"> the query </param>
		/// <returns> integer value </returns>
		public int ExecScalar(string vQuery)
		{
			int nRet = 0;
			SqlCommand myCommand;
			if (_sqlCmd != null)
				myCommand = _sqlCmd;
			else
				myCommand = new SqlCommand();
			try
			{
				OpenConnection();
				myCommand.CommandText = vQuery;
				myCommand.Connection = _sqlConn;
				nRet = Convert.ToInt32(myCommand.ExecuteScalar());
			}
			finally
			{
				CloseConnection();
			}
			return nRet;
		}//ExecScalar

		public string RunSP(string spName, ref DataTable vDT)
		{
			return RunSP(spName, ref vDT, 0, -1);
		}//RunSP

		/// <summary>
		/// This method uses transaction to run a stored procedure which is
		/// of SELECT type. It runs the SP and fills the return datatable with
		/// results
		/// </summary>
		/// <param name="spName"> stored procedure name </param>
		/// <param name="vDT"> return resultant data table </param>
		/// <returns>the @@ROWCOUNT value </returns>
		public string RunSPUseTrans(string spName, ref DataTable vDT)
		{
			SqlCommand	myCommand;
			DataSet	myDS;
			SqlDataAdapter myDataAdapter;
			
			if(_sqlCmd != null)
				myCommand = _sqlCmd;
			else
				myCommand = new SqlCommand();

			myCommand.CommandType = CommandType.StoredProcedure; 
			myCommand.CommandText = spName;
			myCommand.Connection = _sqlConn;
			myCommand.Transaction = _sqlTrans;

			myDS = new DataSet();
			myDataAdapter = new SqlDataAdapter(myCommand);
			myDataAdapter.Fill(myDS);
			if(myDS != null && myDS.Tables.Count > 0)
				vDT = myDS.Tables[0];

			//find the @@ROWCOUNT return parameter
			foreach (SqlParameter param in myCommand.Parameters)
			{
				if(param.Direction == ParameterDirection.ReturnValue)
				{
					return param.Value.ToString();
				}//if
			}//foreach

			return "";
		}//RunSPUseTrans

		public string RunSP(string spName, ref DataTable vDT, int nStart, int nMax)
		{
			SqlCommand	myCommand;
			DataSet	myDS;
			SqlDataAdapter myDataAdapter;

			try
			{
				OpenConnection();
				if(_sqlCmd != null)
					myCommand = _sqlCmd;
				else
					myCommand = new SqlCommand();

				myCommand.CommandType = CommandType.StoredProcedure; 
				myCommand.CommandText = spName;
				myCommand.Connection = _sqlConn;

				myDS = new DataSet();
				myDataAdapter = new SqlDataAdapter(myCommand);
				if(nStart >= 0 && nMax > 0)
					myDataAdapter.Fill(myDS, nStart, nMax, "Table");
				else
					myDataAdapter.Fill(myDS);
				if(myDS != null && myDS.Tables.Count > 0)
					vDT = myDS.Tables[0];

				//find the @@ROWCOUNT return parameter
				foreach (SqlParameter param in myCommand.Parameters)
				{
					if(param.Direction == ParameterDirection.ReturnValue)
					{
						return param.Value.ToString();
					}//if
				}//foreach
			}//try
			finally
			{
				CloseConnection();
			}
			return "";
		}//RunSP

		public int ExeSP(string spName)
		{
			SqlCommand	myCommand;

            try
            {
                OpenConnection();
                if (_sqlCmd != null)
                    myCommand = _sqlCmd;
                else
                    myCommand = new SqlCommand();

                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.CommandText = spName;
                myCommand.Connection = _sqlConn;

                SqlParameter prmReturnValue = myCommand.Parameters.Add("@RETURN_VALUE", SqlDbType.Int);
                prmReturnValue.Direction = ParameterDirection.ReturnValue;

                myCommand.ExecuteNonQuery();
                //int id = Convert.ToInt32(myCommand.ExecuteScalar());

                /*if (prmReturnValue.Value != null && prmReturnValue.Value != DBNull.Value)
				{
					return (int)prmReturnValue.Value;
				}
				else
					return 2;*/

                //find the @@ROWCOUNT return parameter
                //return Convert.ToInt32(myCommand.Parameters["RETURN_VALUE"].Value);
                try
                {
                    return (int)prmReturnValue.Value;
                }
                catch (Exception ex)
                {
                    string errMsg = _logHelper.GetFullErrorMessage(ex);
                    _logger.Error(errMsg);

                    System.Collections.Generic.Dictionary<string, object> dic = new System.Collections.Generic.Dictionary<string, object>();
                    dic["prmReturnValue.Value"] = prmReturnValue.Value;

                    object[] arr = { dic };

                    _logger.Error(ex, "Error when execute store procudure " + spName, arr);

                    throw ex;
                }
                //return id;
            }//try
            catch (Exception ex2)
            {
                _logger.Error(ex2, "Error when execute store procudure " + spName);
            }
            finally
            {
                CloseConnection();
            }
			return -1;
		}//ExeSP


		public string RunSPUseTrans(string spName)
		{
			return RunSPUseTrans(spName, null);
		}//RunSPUseTrans


		public string RunSPUseTrans(string spName, NameValueCollection nvcOutput)
		{
			SqlCommand	myCommand;

			if(_sqlCmd != null)
				myCommand = _sqlCmd;
			else
				myCommand = new SqlCommand();

			myCommand.CommandType = CommandType.StoredProcedure; 
			myCommand.CommandText = spName;
			myCommand.Connection = _sqlConn;
			myCommand.Transaction = _sqlTrans;

			myCommand.ExecuteNonQuery();

			string strRetval = "";
			//get output parameters if any
			foreach (SqlParameter param in myCommand.Parameters)
			{
				if(param.Direction == ParameterDirection.InputOutput 
						|| param.Direction == ParameterDirection.Output 
						|| param.Direction == ParameterDirection.ReturnValue )
				{
					if (nvcOutput != null)
						nvcOutput[param.ParameterName] = param.Value.ToString();
					if (param.Direction == ParameterDirection.ReturnValue)
						strRetval = param.Value.ToString();
				}//if
			}//foreach
			return strRetval;
		}//RunSPUseTrans


		public void InitParameters()
		{
			if(_sqlCmd == null)
			{
				_sqlCmd = new SqlCommand();
			}//if
			_sqlCmd.Parameters.Clear();
			_sqlParameters = _sqlCmd.Parameters;
		}//InitParameters


        public IDataParameter AddParameter(string paramName, SqlDbType sqldbType, int nSize, object oValue, ParameterDirection dir)
		{
            SqlParameter oParam = _sqlParameters.Add(paramName, sqldbType, nSize);
			oParam.Direction = dir; 
			if(oValue == null)
				oParam.Value = System.DBNull.Value;
			else
			{
                if (sqldbType == SqlDbType.DateTime)
					if(DateTime.Compare((DateTime)oValue, new DateTime(1900,1,1)) <= 0)
						oValue = DateTime.Now;
				oParam.Value = oValue;
			}
			return (IDataParameter)oParam;
		}//AddParameter


        public IDataParameter AddParameter(string paramName, SqlDbType sqldbType, object oValue)
		{
            return AddParameter(paramName, sqldbType, oValue, ParameterDirection.Input);
		}//AddParameter        

        public IDataParameter AddParameter(string paramName, SqlDbType sqldbType, object oValue, ParameterDirection dir)
		{
            SqlParameter oParam = _sqlParameters.Add(paramName, sqldbType);
			oParam.Direction = dir;
			if (oValue == null)
				oParam.Value = System.DBNull.Value;
			else
			{
                if (sqldbType == SqlDbType.DateTime)
					if(DateTime.Compare((DateTime)oValue, new DateTime(1900,1,1)) <= 0)
						oValue = new DateTime(1900,1,1);

                if (sqldbType == SqlDbType.VarChar)
					oParam.Size = ((String)oValue).Length;
				oParam.Value = oValue;

			}
			return (IDataParameter)oParam; 
		}//AddParameter

		
		public void ClearParameters()
		{
			if(_sqlParameters != null)
				_sqlParameters.Clear();
			_sqlParameters = null;
			_sqlCmd = null;
		}//ClearParameters

		
		public SqlDbType GetDbType(DbType dbType)
		{
			switch(dbType)
			{
				case DbType.AnsiString: return SqlDbType.VarChar;
				case DbType.AnsiStringFixedLength: return SqlDbType.Char;
				case DbType.String: return SqlDbType.NText;
				case DbType.StringFixedLength: return SqlDbType.Char;
				case DbType.Int32: return SqlDbType.Int;
				case DbType.Int16: return SqlDbType.SmallInt; 
				case DbType.Decimal: return SqlDbType.Decimal;
				case DbType.DateTime: return SqlDbType.DateTime;
				case DbType.Binary: return SqlDbType.Image;               
				default:
					return SqlDbType.Variant;
			}//switch
		}//
		
		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			// TODO:  Add SqlPortalDAL.Dispose implementation
		}

		#endregion
	}//SqlPortalDAL

}//namespace PortalMiddleWare
