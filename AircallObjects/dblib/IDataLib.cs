using System;
using System.Data;
using System.Collections.Specialized;
using System.Data.SqlTypes;

namespace ZWT.DbLib
{
	/// <summary>
	/// IPortalDAL is interface prototyping the data access layer for eGrAMS portal
	/// </summary>
	public interface IDataLib : IDisposable
	{
		IDbConnection OpenConnection();
		void CloseConnection();
		IDbTransaction BeginTransaction();
		void CommitTransaction();
		void RollbackTransaction();
		void RunSelect(string vQuery, ref DataSet vDS);
		void RunSelect(string vQuery, ref DataSet vDS, int nStart, int nMax, string tblName);
		void RunSelect(string vQuery, ref DataTable vDT);
		int  ExecScalar(string vQuery);
		int  ExecScalarUseTrans(string vQuery);
		void RunSelectUseTrans(string vQuery, ref DataTable vDT);
		int ExeQuery(string vQuery);

		int  ExeSP(string spName);
		string  RunSP(string spName, ref DataTable vDT);
		string  RunSP(string spName, ref DataTable vDT, int nStart, int nMax);

		string  RunSPUseTrans(string spName, ref DataTable vDT);
		string  RunSPUseTrans(string spName);
		string  RunSPUseTrans(string spName, NameValueCollection nvcOutput);

		void InitParameters();
        IDataParameter AddParameter(string paramName, SqlDbType sqldbType, int nSize, object oValue, ParameterDirection dir);
        IDataParameter AddParameter(string paramName, SqlDbType sqldbType, object oValue);
        IDataParameter AddParameter(string paramName, SqlDbType sqldbType, object oValue, ParameterDirection dir);
		void ClearParameters();
	}//IPortalDAL

}//namespace
