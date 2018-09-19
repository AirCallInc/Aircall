using Aircall.Common;
using Services;
using System;
using System.Data;
using System.Web;

namespace Aircall.client.controls
{
    public partial class header : System.Web.UI.UserControl
    {
        IUserNotificationService objUserNotificationService = ServiceFactory.UserNotificationService;
        IClientAddressService objClientAddressService = ServiceFactory.ClientAddressService;
        IClientService objClientService = ServiceFactory.ClientService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ClientLoginCookie"] != null)
            {
                LoginModel login = Session["ClientLoginCookie"] as LoginModel;
                lnkUsername.InnerText = login.FullName;//Request.Cookies["ClientLoginCookie"]["ClientName"].ToString();
                int ClientId = login.Id;//Convert.ToInt32(Request.Cookies["ClientLoginCookie"]["ClientId"].ToString());

        DataTable dtClient = new DataTable();
        objClientService.GetClientById(login.Id, ref dtClient);

                if (dtClient.Rows.Count > 0)
                {
                    if (dtClient.Rows[0]["IsActive"].ToString().ToLower() == "false")
                    {
                        lnkLogout_Click(lnkLogout, EventArgs.Empty);
    }
                    if (dtClient.Rows[0]["IsDeleted"].ToString().ToLower() == "true")
                    {
                        lnkLogout_Click(lnkLogout, EventArgs.Empty);
}
                }
                else
                {
                    lnkLogout_Click(lnkLogout, EventArgs.Empty);
                }

                DataTable dtAddress = new DataTable();
var AddressId = 0;
objClientAddressService.GetClientAddressesByClientId(ClientId, true,ref dtAddress);
                if (dtAddress.Rows.Count>0)
                {
                    if (dtAddress.Rows.Count == 1)
                    {

                        AddressId = int.Parse(dtAddress.Rows[0]["Id"].ToString());
                    }
                    else
                    {
                        var rows = dtAddress.Select(" IsDefaultAddress = true ");
                        if (rows.Length > 0)
                        {
                            AddressId = int.Parse(rows[0]["Id"].ToString());
                        }                        
                    }
                    BindNotifications(ClientId, AddressId); 
                }
            }
            else
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx");
        }
        private void BindNotifications(int clientId, int addressId)
{
    DataTable dtService = new DataTable();
    objUserNotificationService.GetAllNotificationByUserId(clientId, ref dtService);
    //objUserNotificationService.GetAllNotificationForDashboardByUserId(clientId, addressId, ref dtService);
    //objServicesService.GetServiceForClient(ClientId, General.ServiceTypes.WaitingApproval.GetEnumDescription(), SelectRow, ref dtService);
    if (dtService.Rows.Count > 0)
    {
        var rows = dtService.Select(" Status ='UnRead'");
        if (rows.Length > 0)
        {
            ltrCnt.Text = rows.Length.ToString(); ;
        }
    }
}
protected void lnkLogout_Click(object sender, EventArgs e)
{
    Session["ClientLoginCookie"] = null;
    //Session.Abandon();
    Response.Redirect(Application["SiteAddress"] + "sign-in.aspx");
}
    }
}