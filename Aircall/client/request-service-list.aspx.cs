using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.client
{
    public partial class request_service_list : System.Web.UI.Page
    {
        IRequestServicesService objRequestServicesService;
        protected void Page_Load(object sender, EventArgs e)
        {
            objRequestServicesService = ServiceFactory.RequestServicesService;
            if (Session["success"] != null)
            {
                if (Session["success"].ToString() == "1")
                {
                    dvMessage.InnerText = "Record Added.";
                    dvMessage.Attributes.Add("class", "success");
                    dvMessage.Visible = true;
                }
                if (Session["success"].ToString() == "2")
                {
                    dvMessage.InnerText = "Record Updated.";
                    dvMessage.Attributes.Add("class", "success");
                    dvMessage.Visible = true;
                }
                Session["success"] = null;
            }
            if (Session["ClientLoginCookie"] != null)
            {
                if (!IsPostBack)
                {
                    BindRequest();
                }
            }
            else
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx", false);
        }

        private void BindRequest()
        {
            DataTable dt = new DataTable();
            int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
            objRequestServicesService.GetRequestedServiceAddressByClientId(ClientId, ref dt);
            lstAddresses.DataSource = dt;
            lstAddresses.DataBind();
        }

        private void BindRequestList(ListView lstRequest, int AddressId)
        {
            DataTable dt = new DataTable();
            int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
            objRequestServicesService.GetRequestedServiceByClientId(ClientId, ref dt);
            DataTable dtClone = new DataTable();

            dtClone = dt.Clone();

            DataRow[] rows = dt.Select(" AddressId =" + AddressId.ToString());

            foreach (DataRow dr in rows)
            {
                dtClone.Rows.Add(dr.ItemArray);
            }


            lstRequest.DataSource = dtClone;
            lstRequest.DataBind();
        }

        protected void lstRequest_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "EditRequest")
            {
                Response.Redirect("request-services.aspx?rid=" + e.CommandArgument.ToString());
            }
            if (e.CommandName == "DeleteRequest" && e.CommandArgument.ToString() != "0")
            {
                if (Session["ClientLoginCookie"] != null)
                {
                    long RequestedServiceId = Convert.ToInt64(e.CommandArgument.ToString());
                    objRequestServicesService = ServiceFactory.RequestServicesService;
                    int ClientId = (Session["ClientLoginCookie"] as LoginModel).Id;
                    objRequestServicesService.DeleteRequestedService(RequestedServiceId, ClientId, General.UserRoles.Client.GetEnumValue(), DateTime.UtcNow);
                    dvMessage.InnerText = "Record Deleted.";
                    dvMessage.Attributes.Add("class", "success");
                    dvMessage.Visible = true;
                    BindRequest();
                }
            }
        }

        protected void dataPagerRequest_PreRender(object sender, EventArgs e)
        {
            BindRequest();
        }

        protected void lstRequest_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                LinkButton lnkEdit = e.Item.FindControl("lnkEdit") as LinkButton;
                LinkButton lnkDelete = e.Item.FindControl("lnkDelete") as LinkButton;
                DataRow row = (e.Item.DataItem as DataRowView).Row;
                if (row["AllowEdit"].ToString().ToLower() == "false")
                {
                    lnkEdit.Visible = false;
                }
                if (row["AllowDelete"].ToString().ToLower() == "false")
                {
                    lnkDelete.Visible = false;
                }
            }
        }

        protected void lstAddresses_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                ListView lstRequest = e.Item.FindControl("lstRequest") as ListView;
                Literal ltrAddress = e.Item.FindControl("ltrAddress") as Literal;

                DataRow dr = (e.Item.DataItem as DataRowView).Row;
                ltrAddress.Text = dr["Address"].ToString();
                BindRequestList(lstRequest, int.Parse(dr["AddressId"].ToString()));
            }
        }
    }
}