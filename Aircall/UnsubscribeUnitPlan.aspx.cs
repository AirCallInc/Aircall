using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using Stripe;

namespace Aircall
{
    public partial class UnsubscribeUnitPlan : System.Web.UI.Page
    {
        IClientUnitService objClientUnitService;
        IStripeErrorLogService objStripeErrorLogService;

        protected void Page_Load(object sender, EventArgs e)
        {
            objClientUnitService = ServiceFactory.ClientUnitService;
            DataTable dtUnits = new DataTable();
            objClientUnitService.UnsubscribeUnitPlan(ref dtUnits);
            if (dtUnits.Rows.Count > 0)
            {
                for (int i = 0; i < dtUnits.Rows.Count; i++)
                {
                    int ClientId = Convert.ToInt32(dtUnits.Rows[i]["ClientId"].ToString());
                    int UnitId = Convert.ToInt32(dtUnits.Rows[i]["Id"].ToString());
                    string StripeCustomerId = dtUnits.Rows[i]["StripeCustomerId"].ToString();
                    string StripeSubscriptionId = dtUnits.Rows[i]["StripeSubscriptionId"].ToString();

                    if (!string.IsNullOrEmpty(StripeCustomerId) && !string.IsNullOrEmpty(StripeSubscriptionId))
                    {
                        try
                        {
                            var SubscriptionService = new StripeSubscriptionService();
                            StripeSubscription UnitSubscription = SubscriptionService.Cancel(StripeCustomerId, StripeSubscriptionId, true);
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
            ltrMessage.Text = "Unit Plan Unsubscribe Scheduler Run Successfully.";
        }
    }
}