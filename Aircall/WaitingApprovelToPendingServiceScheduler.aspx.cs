using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;

namespace Aircall
{
    public partial class WaitingApprovelToPendingServiceScheduler : System.Web.UI.Page
    {
        IServicesService objServicesService;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                objServicesService = ServiceFactory.ServicesService;
                objServicesService.RunWaitingApprovalToPendingServiceScheduler();
                ltrMessage.Text = "From WaitingApproval To PendingService Scheduler run successfully.";
            }
            catch (Exception Ex)
            {
                ltrMessage.Text = Ex.Message.ToString().Trim();
            }
        }
    }
}