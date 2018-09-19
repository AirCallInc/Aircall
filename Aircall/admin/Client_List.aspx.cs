using Aircall.Common;
using Services;
using Stripe;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Aircall.admin
{
    public partial class Client_List : System.Web.UI.Page
    {
        IClientService objClientService;
        IStripeErrorLogService objStripeErrorLogService;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["msg"] != null)
                {
                    if (Session["msg"].ToString() == "edit")
                    {
                        dvMessage.InnerHtml = "<strong>Client updated successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    else if (Session["msg"].ToString() == "add")
                    {
                        dvMessage.InnerHtml = "<strong>Client added successfully.</strong>";
                        dvMessage.Attributes.Add("class", "alert alert-success");
                        dvMessage.Visible = true;
                    }
                    Session["msg"] = null;
                }
                BindClients();
            }
        }

        private void BindClients()
        {
            string ClientName = string.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString["ClientName"]))
            {
                ClientName = Request.QueryString["ClientName"].ToString();
                txtClient.Text = ClientName;
            }
            objClientService = ServiceFactory.ClientService;
            DataTable dtClients = new DataTable();
            objClientService.GetAllClientByName(ClientName, ListViewSortExpression, ListViewSortDirection.ToString(), ref dtClients);
            if (dtClients.Rows.Count > 0)
            {
                lstClients.DataSource = dtClients;
            }
            lstClients.DataBind();
        }

        protected void Page_PreRender(object sender, System.EventArgs e)
        {
            lnkActive.Attributes.Add("onclick", "javascript:return checkActive('Are you sure want to activate selected record?','Please select atleast one record')");
            lnkInactive.Attributes.Add("onclick", "javascript:return checkInactive('Are you sure want to inactivate selected record?','Please select atleast one record')");
            lnkDelete.Attributes.Add("onclick", "javascript:return checkDelete('Services for selected clients will be deleted. Are you sure want to delete selected record?','Please select atleast one record')");
        }
       
        protected void lnkActive_Click(object sender, EventArgs e)
        {
            try
            {
                bool Active = false;
                dvMessage.InnerHtml = "";
                dvMessage.Visible = false;
                LoginModel objLoginModel = new LoginModel();
                objLoginModel = Session["LoginSession"] as LoginModel;

                BizObjects.Client objClient = new BizObjects.Client();
                objClientService = ServiceFactory.ClientService;

                int UserId = objLoginModel.Id;
                int RoleId = objLoginModel.RoleId;

                for (int i = 0; i <= lstClients.Items.Count - 1; i++)
                {
                    HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstClients.Items[i].FindControl("chkcheck");
                    if (chkUsers.Checked)
                    {
                        HiddenField hdnClientId = (HiddenField)lstClients.Items[i].FindControl("hdnClientId");
                        if (!string.IsNullOrEmpty(hdnClientId.Value))
                        {
                            objClient.Id = Convert.ToInt32(hdnClientId.Value);
                            objClient.IsActive = true;
                            objClient.UpdatedBy = UserId;
                            objClient.UpdatedByType = RoleId;
                            objClient.UpdatedDate = DateTime.UtcNow;
                            objClientService.SetStatus(ref objClient);
                            Active = true;
                        }
                    }
                }
                if (Active)
                {
                    dvMessage.InnerHtml = "<strong>Record updated successfully.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-success");
                    dvMessage.Visible = true;
                }
                BindClients();
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = "<strong>Error!</strong>" + Ex.Message.ToString().Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }

        protected void lnkInactive_Click(object sender, EventArgs e)
        {
            try
            {
                bool InActive = false;
                dvMessage.InnerHtml = "";
                dvMessage.Visible = false;
                int Cnt = 0;

                LoginModel objLoginModel = new LoginModel();
                objLoginModel = Session["LoginSession"] as LoginModel;

                BizObjects.Client objClient = new BizObjects.Client();
                objClientService = ServiceFactory.ClientService;

                int UserId = objLoginModel.Id;
                int RoleId = objLoginModel.RoleId;

                for (int i = 0; i <= lstClients.Items.Count - 1; i++)
                {
                    HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstClients.Items[i].FindControl("chkcheck");
                    if (chkUsers.Checked)
                    {
                        HiddenField hdnClientId = (HiddenField)lstClients.Items[i].FindControl("hdnClientId");
                        if (!string.IsNullOrEmpty(hdnClientId.Value))
                        {
                            objClient.Id = Convert.ToInt32(hdnClientId.Value);
                            objClient.IsActive = false;
                            objClient.UpdatedBy = UserId;
                            objClient.UpdatedByType = RoleId;
                            objClient.UpdatedDate = DateTime.UtcNow;
                            Cnt = objClientService.SetStatus(ref objClient);
                            if (Cnt == 0)
                            {
                                dvMessage.InnerHtml = "<strong>You can not InActive client record because service is scheduled for client.</strong>";
                                dvMessage.Attributes.Add("class", "alert alert-error");
                                dvMessage.Visible = true;
                                BindClients();
                                return;
                            }
                            InActive = true;
                        }
                    }
                }
                if (InActive)
                {
                    dvMessage.InnerHtml = "<strong>Record updated successfully.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-success");
                    dvMessage.Visible = true;
                }
                BindClients();
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = "<strong>Error!</strong>" + Ex.Message.ToString().Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }

        protected void lnkDelete_Click(object sender, EventArgs e)
        {
            try
            {
                bool Delete = false;
                dvMessage.InnerHtml = "";
                dvMessage.Visible = false;

                LoginModel objLoginModel = new LoginModel();
                objLoginModel = Session["LoginSession"] as LoginModel;

                BizObjects.Client objClient = new BizObjects.Client();
                objClientService = ServiceFactory.ClientService;

                int UserId = objLoginModel.Id;
                int RoleId = objLoginModel.RoleId;

                for (int i = 0; i <= lstClients.Items.Count - 1; i++)
                {
                    HtmlInputCheckBox chkUsers = (HtmlInputCheckBox)lstClients.Items[i].FindControl("chkcheck");
                    if (chkUsers.Checked)
                    {
                        HiddenField hdnClientId = (HiddenField)lstClients.Items[i].FindControl("hdnClientId");
                        if (!string.IsNullOrEmpty(hdnClientId.Value))
                        {
                            var clientId = Convert.ToInt32(hdnClientId.Value);
                            objClient.Id = clientId;
                            objClient.DeletedBy = UserId;
                            objClient.DeletedByType = RoleId;
                            objClient.DeletedDate = DateTime.UtcNow;
                            DataTable dtClientUnits = new DataTable();
                            DataTable dtClient = new DataTable();

                            objClientService.AllowToDeleteClient(Convert.ToInt32(hdnClientId.Value), ref dtClient);
                            if (dtClient.Rows.Count > 0)
                            {
                                dvMessage.InnerHtml = "<strong>You can not delete some of the client record because service is scheduled for that client.</strong>";
                                dvMessage.Attributes.Add("class", "alert alert-error");
                                dvMessage.Visible = true;
                                BindClients();
                                return;
                            }
                            else
                            {
                                objClientService.DeleteClient(ref objClient, ref dtClientUnits);
                                Delete = true;
                                CancelSubscription(clientId);
                            }
                        }
                    }
                }
                if (Delete)
                {
                    dvMessage.InnerHtml = "<strong>Record deleted successfully.</strong>";
                    dvMessage.Attributes.Add("class", "alert alert-success");
                    dvMessage.Visible = true;
                }
                BindClients();
            }
            catch (Exception Ex)
            {
                dvMessage.InnerHtml = "<strong>Error!</strong>" + Ex.Message.ToString().Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }

        private void CancelSubscription(int clientId)
        {
            var sql = "select A.AuthorizeNetSubscriptionId, B.CustomerProfileId from ClientUnitSubscription A inner join Client B on A.ClientId = B.Id where A.ClientId = {0} and isnull(A.AuthorizeNetSubscriptionId, '') <> '' and isnull(B.CustomerProfileId, '') <> ''";
            var instance = new DBUtility.SQLDBHelper();
            var ds = instance.Query(sql, null);
            instance.Close();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string subscriptionId = dr["AuthorizeNetSubscriptionId"].ToString();
                bool isSuccess = false;
                string errCode = "";
                string errText = "";
                var helper = new AuthorizeNetLib.AuthorizeNetHelper();
                helper.CancelSubscription(subscriptionId, ref isSuccess, ref errCode, ref errText);
            }
        }

        protected void dataPagerClients_PreRender(object sender, EventArgs e)
        {
            dataPagerClients.PageSize = Convert.ToInt32(Application["PageSize"].ToString());
            BindClients();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string Param = string.Empty;

            if (!string.IsNullOrEmpty(txtClient.Text.Trim()))
                Param = "?ClientName=" + txtClient.Text.Trim();

            Response.Redirect(Application["SiteAddress"] + "admin/Client_List.aspx" + Param);
        }

        //changes by PP start 
        protected void SortByServiceCase_Click(object sender, EventArgs e)
        {

        }

        protected void lstClients_Sorting(object sender, ListViewSortEventArgs e)
        {
            LinkButton lb = lstClients.FindControl(e.SortExpression) as LinkButton;
            HtmlTableCell th = lb.Parent as HtmlTableCell;
            HtmlTableRow tr = th.Parent as HtmlTableRow;
            List<HtmlTableCell> ths = new List<HtmlTableCell>();
            foreach (HtmlTableCell item in tr.Controls)
            {
                try
                {
                    if (item.ID.Contains("th"))
                    {
                        item.Attributes["class"] = "sorting";
                    }
                }
                catch (Exception ex)
                {
                }
            }
            ListViewSortExpression = e.SortExpression;
            if (ListViewSortDirection == SortDirection.Ascending)
            {
                ListViewSortDirection = SortDirection.Descending;
                th.Attributes["class"] = "sorting_desc";
            }
            else
            {
                ListViewSortDirection = SortDirection.Ascending;
                th.Attributes["class"] = "sorting_asc";
            }
        }

        protected string ListViewSortExpression
        {
            get
            {
                if (ViewState["SortExpression"] == null)
                    ViewState["SortExpression"] = "AddedDate";
                return (string)ViewState["SortExpression"];
            }
            set { ViewState["SortExpression"] = value; }
        }

        protected SortDirection ListViewSortDirection
        {
            get
            {
                if (ViewState["sortDirection"] == null)
                    ViewState["sortDirection"] = SortDirection.Descending;
                return (SortDirection)ViewState["sortDirection"];
            }
            set { ViewState["sortDirection"] = value; }
        }
        //changes by PP end
    }
}