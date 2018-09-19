using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Stripe;
using Services;
using System.Data;

namespace Aircall
{
    public partial class CheckUnitSubscriptionCountWithStripe : System.Web.UI.Page
    {
        IClientUnitServiceCountService objClientUnitServiceCountService;
        IStripeErrorLogService objStripeErrorLogService;

        protected void Page_Load(object sender, EventArgs e)
        {
            objClientUnitServiceCountService = ServiceFactory.ClientUnitServiceCountService;
            DataTable dtUnitSubscription = new DataTable();
            objClientUnitServiceCountService.GetUnitSubscriptionCountWithStripe(ref dtUnitSubscription);
            if (dtUnitSubscription.Rows.Count>0)
            {
                for (int i = 0; i < dtUnitSubscription.Rows.Count; i++)
                {
                    int Count = 0;
                    int UnitId=Convert.ToInt32(dtUnitSubscription.Rows[i]["Id"].ToString());
                    int ClientId = Convert.ToInt32(dtUnitSubscription.Rows[i]["ClientId"].ToString());
                    string StripeCustomerId = dtUnitSubscription.Rows[i]["StripeCustomerId"].ToString();
                    string StripeSubscriptionId = dtUnitSubscription.Rows[i]["StripeSubscriptionId"].ToString();
                    try
                    {
                        var InvoiceService = new StripeInvoiceService();
                        var InvoiceList = InvoiceService.List(new StripeInvoiceListOptions() { CustomerId = StripeCustomerId, Limit = 100 });
                        Count = InvoiceList.Where(top => top.SubscriptionId == StripeSubscriptionId && top.Paid == true && Convert.ToDateTime(top.Date).Date == DateTime.UtcNow.Date).Count();

                        objClientUnitServiceCountService.UpdateUnitStripeSubscriptionCount(UnitId, Count, 1, 1, DateTime.UtcNow);
                    }
                    catch (StripeException stex)
                    {
                        BizObjects.StripeErrorLog objStripeErrorLog = new BizObjects.StripeErrorLog();
                        objStripeErrorLogService = ServiceFactory.StripeErrorLogService;
                        objStripeErrorLog.ChargeId = stex.StripeError.ChargeId;
                        objStripeErrorLog.Code = stex.StripeError.Code;
                        objStripeErrorLog.DeclineCode = stex.StripeError.DeclineCode;
                        objStripeErrorLog.ErrorType = stex.StripeError.ErrorType;
                        objStripeErrorLog.Error = stex.StripeError.Error;
                        objStripeErrorLog.ErrorSubscription = stex.StripeError.ErrorSubscription;
                        objStripeErrorLog.Message = stex.StripeError.Message;
                        objStripeErrorLog.Parameter = stex.StripeError.Parameter;
                        objStripeErrorLog.Userid = ClientId;
                        objStripeErrorLog.UnitId = UnitId;
                        objStripeErrorLog.DateAdded = DateTime.UtcNow;

                        objStripeErrorLogService.AddStripeErrorLog(ref objStripeErrorLog);
                    }
                }
            }
        }
    }
}