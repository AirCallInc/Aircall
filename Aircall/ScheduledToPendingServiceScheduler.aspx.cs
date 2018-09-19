using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall
{
    public partial class ScheduledToPendingServiceScheduler : System.Web.UI.Page
    {
        IServicesService objServicesService;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                objServicesService = ServiceFactory.ServicesService;
                objServicesService.RunScheduledServiceToPendingServiceScheduler();
                ltrMessage.Text = "Schedulled Service to Pending Service Scheduler Run Successfuly.";
            }
            catch (Exception Ex)
            {
                ltrMessage.Text = Ex.Message.ToString().Trim();
            }
        }
    }
}