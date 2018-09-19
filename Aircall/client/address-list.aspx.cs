using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;
using Aircall.Common;

namespace Aircall.client
{
    public partial class address_list : System.Web.UI.Page
    {
        #region "Declaration"
        IClientAddressService objClientAddressService = ServiceFactory.ClientAddressService;
        #endregion

        #region "Page Events"
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["ClientLoginCookie"] != null)
                {
                    BindClientAddress();
                }
                else
                    Response.Redirect(Application["SiteAddress"] + "sign-in.aspx", false);
            }
        }
        #endregion

        #region "Functions"
        private void BindClientAddress()
        {
            int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
            DataTable dtAddress = new DataTable();
            objClientAddressService.GetClientAddressesByClientId(ClientId, true, ref dtAddress);
            if (dtAddress.Rows.Count > 0)
            {
                DataTable dt = dtAddress.Clone();
                var rows = dtAddress.Select(" ShowInList = true ");

                foreach (var row in rows)
                {
                    dt.Rows.Add(row.ItemArray);
                }
                lstAddress.DataSource = dt;
            }
            else
            {
                lstAddress.DataSource = null;
            }
            lstAddress.DataBind();
        }
        #endregion

        #region "Events"

        protected void lstAddress_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (Session["ClientLoginCookie"] != null)
            {
                BizObjects.ClientAddress objClientAddress = new BizObjects.ClientAddress();
                if (e.CommandName == "DeleteAddress" && e.CommandArgument.ToString() != "0")
                {
                    int AddressId = Convert.ToInt32(e.CommandArgument.ToString());
                    DataTable dtAddress = new DataTable();
                    objClientAddressService.GetAddressById(AddressId, ref dtAddress);

                    DataTable dtAddress1 = new DataTable();
                    objClientAddressService.GetClientAddressesByClientId((Session["ClientLoginCookie"] as LoginModel).Id, true, ref dtAddress1);


                    if (dtAddress1.Rows.Count > 1)
                    {
                        if (Convert.ToBoolean(dtAddress.Rows[0]["IsDefaultAddress"].ToString()))
                        {
                            dvMessage.InnerHtml = "Default address can not deleted. Mark other address as default first";
                            dvMessage.Attributes.Add("class", "error");
                            dvMessage.Visible = true;
                            return;
                        }
                    }
                    objClientAddress.Id = AddressId;
                    objClientAddress.DeletedBy = (Session["ClientLoginCookie"] as LoginModel).Id;
                    objClientAddress.DeletedByType = General.UserRoles.Client.GetEnumValue();
                    objClientAddress.DeletedDate = DateTime.Now;

                    objClientAddressService.DeleteClientAddress(ref objClientAddress);
                    dvMessage.InnerHtml = "Address deleted Successfully.";
                    dvMessage.Attributes.Add("class", "success");
                    dvMessage.Visible = true;

                    Response.Redirect(Application["SiteAddress"] + "client/address-list.aspx", false);
                }
                else if (e.CommandName == "Modify" && e.CommandArgument.ToString() != "0")
                {
                    Session["AddressId"] = e.CommandArgument.ToString();
                    Response.Redirect(Application["SiteAddress"] + "client/address-addEdit.aspx", false);
                }
            }
            else
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx", false);
        }
        protected void lstAddress_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var row = (e.Item.DataItem as DataRowView).Row;
                LinkButton lnkDelete = e.Item.FindControl("lnkDelete") as LinkButton;

                if (row["AllowDelete"].ToString().ToLower() == "false")
                {
                    lnkDelete.Visible = false;
                }
            }
        }
        #endregion
    }
}