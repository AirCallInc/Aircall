using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace Services
{
    public interface IStateService
    {
        void GetAllStates(bool IsActive, bool IncludeInactive,ref DataTable dtStates);
        void GetStateByName(string StateName,int StateId ,ref DataTable dtState);
        void GetStateById(int StateId, ref DataTable dtState);
        int SetStatus(bool IsActive, int StateId);
        int AddStates(ref BizObjects.State States);
        int UpdateState(ref BizObjects.State States);
        int DeleteState(ref BizObjects.State States);
    }

    public class StateService : IStateService
    {
        IDataLib dbLib;

        public void GetAllStates(bool IsActive, bool IncludeInactive,ref DataTable dtStates)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_State_GetAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, IsActive);
                dbLib.AddParameter("@IncludeInactive", SqlDbType.Bit, IncludeInactive);
                dbLib.RunSP(strsql, ref dtStates);
            }
            catch (Exception ex)
            {
                dbLib.RollbackTransaction();
                throw ex;
            }
            finally
            {
                dbLib.CloseConnection();
            }
        }

        public void GetStateByName(string StateName,int StateId, ref DataTable dtState)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_State_GetByName";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@StateId", SqlDbType.Int, StateId);
                dbLib.AddParameter("@StateName", SqlDbType.NVarChar, StateName);
                dbLib.RunSP(strsql, ref dtState);
            }
            catch (Exception ex)
            {
                dbLib.RollbackTransaction();
                throw ex;
            }
            finally
            {
                dbLib.CloseConnection();
            }
        }

        public void GetStateById(int StateId, ref DataTable dtState)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_State_GetById";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@StateId", SqlDbType.NVarChar, StateId);
                dbLib.RunSP(strsql, ref dtState);
            }
            catch (Exception ex)
            {
                dbLib.RollbackTransaction();
                throw ex;
            }
            finally
            {
                dbLib.CloseConnection();
            }
        }

        public int SetStatus(bool IsActive, int StateId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_State_SetStatus";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@IsActive", SqlDbType.Bit, IsActive);
                dbLib.AddParameter("@StateId", SqlDbType.Int, StateId);
                rtn = dbLib.ExeSP(strsql);
                return rtn;
            }
            catch (Exception ex)
            {
                dbLib.RollbackTransaction();
                throw ex;
            }
            finally
            {
                dbLib.CloseConnection();
            }
        }

        public int AddStates(ref BizObjects.State States)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_State_Add";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                States.AddInsertParams(ref dbLib);
                rtn = dbLib.ExeSP(strsql);
                return rtn;
            }
            catch (Exception ex)
            {
                dbLib.RollbackTransaction();
                throw ex;
            }
            finally
            {
                dbLib.CloseConnection();
            }
        }

        public int UpdateState(ref BizObjects.State States)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_State_Update";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                States.AddUpdateParams(ref dbLib);
                rtn = dbLib.ExeSP(strsql);
                return rtn;
            }
            catch (Exception ex)
            {
                dbLib.RollbackTransaction();
                throw ex;
            }
            finally
            {
                dbLib.CloseConnection();
            }
        }

        public int DeleteState(ref BizObjects.State States)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_State_Delete";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@StateId", SqlDbType.Int, States.Id);
                dbLib.AddParameter("@DeletedBy", SqlDbType.Int, States.DeletedBy);
                dbLib.AddParameter("@DeletedByType", SqlDbType.Int, States.DeletedByType);
                dbLib.AddParameter("@DeletedDate", SqlDbType.DateTime, States.DeletedDate);
                rtn = dbLib.ExeSP(strsql);
                return rtn;
            }
            catch (Exception ex)
            {
                dbLib.RollbackTransaction();
                throw ex;
            }
            finally
            {
                dbLib.CloseConnection();
            }
        }
    }
}
