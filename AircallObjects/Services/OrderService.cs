using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizObjects;
using ZWT.DbLib;
using System.Data.SqlClient;

namespace Services
{
    public interface IOrderService
    {
        void GetAllOrders(string ClientName,string EmpName,string StartDate,string EndDate, ref DataTable dtOrders);
        void GetOrderById(int OrderId, ref DataTable dtOrders);
        void GetOrderByClientId(int ClientId, ref DataTable dtClientOrder);
        void GetOrderItemByOrderId(int OrderId, ref DataTable dtClientOrder);
        int AddClientOrder(ref Orders objOrder);
        int AddClientUnitOrder(ref BizObjects.Orders Orders, string customerPaymentProfileId, int AddressId);
        int AddClientUnitOrderForSchedular(ref BizObjects.Orders Orders, string StripeCardId, int AddressId);
        int DeleteOrder(int OrderId);
        int DeleteOrderById(ref BizObjects.Orders Orders);
    }
    public class OrderService : IOrderService
    {
        IDataLib dbLib;

        public void GetAllOrders(string ClientName, string EmpName, string StartDate, string EndDate, ref DataTable dtOrders)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Orders_SelectAll";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientName", SqlDbType.NVarChar, ClientName);
                dbLib.AddParameter("@EmpName", SqlDbType.NVarChar, EmpName);
                dbLib.AddParameter("@StartDate", SqlDbType.NVarChar, StartDate);
                dbLib.AddParameter("@EndDate", SqlDbType.NVarChar, EndDate);
                dbLib.RunSP(strsql, ref dtOrders);
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

        public void GetOrderById(int OrderId, ref DataTable dtOrders)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Orders_SelectByID";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Id", SqlDbType.Int, OrderId);
                dbLib.RunSP(strsql, ref dtOrders);
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

        public int AddClientOrder(ref Orders objOrder)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Orders_Insert";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                objOrder.AddInsertParams(ref dbLib);
                rtn = dbLib.ExeSP(strsql);
                objOrder.Id = rtn;
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
        public void GetOrderItemByOrderId(int OrderId, ref DataTable dtClientOrder)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_ClientPortal_GetOrderItem_ByOrderId";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@OrderId", SqlDbType.Int, OrderId);
                dbLib.RunSP(strsql, ref dtClientOrder);
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

        public int AddClientUnitOrder(ref BizObjects.Orders Order, string customerPaymentProfileId, int AddressId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Orders_AddClientUnitOrder";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@OrderType", SqlDbType.NVarChar, Order.OrderType);
                dbLib.AddParameter("@ClientId", SqlDbType.Int, Order.ClientId);
                dbLib.AddParameter("@OrderAmount", SqlDbType.Decimal, Order.OrderAmount);
                dbLib.AddParameter("@ChargeBy", SqlDbType.NVarChar, Order.ChargeBy);
                dbLib.AddParameter("@AddedBy", SqlDbType.Int, Order.AddedBy);
                dbLib.AddParameter("@AddedByType", SqlDbType.Int, Order.AddedByType);
                dbLib.AddParameter("@AddedDate", SqlDbType.DateTime, Order.AddedDate);
                dbLib.AddParameter("@CustomerPaymentProfileId", SqlDbType.NVarChar, customerPaymentProfileId);
                dbLib.AddParameter("@AddressId", SqlDbType.Int, AddressId);
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

        public int AddClientUnitOrderForSchedular(ref BizObjects.Orders Order, string StripeCardId, int AddressId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Orders_AddClientUnitOrderForSchedular";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@OrderType", SqlDbType.NVarChar, Order.OrderType);
                dbLib.AddParameter("@ClientId", SqlDbType.Int, Order.ClientId);
                dbLib.AddParameter("@OrderAmount", SqlDbType.Decimal, Order.OrderAmount);
                dbLib.AddParameter("@ChargeBy", SqlDbType.NVarChar, Order.ChargeBy);
                dbLib.AddParameter("@AddedBy", SqlDbType.Int, Order.AddedBy);
                dbLib.AddParameter("@AddedByType", SqlDbType.Int, Order.AddedByType);
                dbLib.AddParameter("@AddedDate", SqlDbType.DateTime, Order.AddedDate);
                dbLib.AddParameter("@StripeCardId", SqlDbType.NVarChar, StripeCardId);
                dbLib.AddParameter("@AddressId", SqlDbType.Int, AddressId);
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
        public int DeleteOrder(int OrderId)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Orders_DeleteByID";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Id", SqlDbType.Int, OrderId);
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

        public int DeleteOrderById(ref BizObjects.Orders Orders)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            int rtn;
            strsql = "uspa_Orders_Delete";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@Id", SqlDbType.Int, Orders.Id);
                dbLib.AddParameter("@DeletedBy", SqlDbType.Int, Orders.DeletedBy);
                dbLib.AddParameter("@DeletedByType", SqlDbType.Int, Orders.DeletedByType);
                dbLib.AddParameter("@DeletedDate", SqlDbType.DateTime, Orders.DeletedDate);
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

        public void GetOrderByClientId(int ClientId, ref DataTable dtClientOrder)
        {
            string strsql = null;
            dbLib = DataLibFactory.CreateDAL();
            strsql = "uspa_Orders_SelectByClientID";
            try
            {
                dbLib.OpenConnection();
                dbLib.BeginTransaction();
                dbLib.InitParameters();
                dbLib.AddParameter("@ClientId", SqlDbType.Int, ClientId);
                dbLib.RunSP(strsql, ref dtClientOrder);
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
