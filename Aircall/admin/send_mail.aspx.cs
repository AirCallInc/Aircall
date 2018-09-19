using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall.admin
{
    public partial class send_mail : System.Web.UI.Page
    {
        IEmailTemplateService objEmailTemplateService;
        IUsersService objUsersService;
        protected void Page_Load(object sender, EventArgs e)
        {
            List<string> data = new List<string>();
            var values = DurationExtensions.GetValues<General.UserRoles>();
            foreach (var item in values)
            {
                General.UserRoles p = (General.UserRoles)item;
                data.Add(p.GetEnumDescription());
                chkRole.Items.Add(new ListItem(p.GetEnumDescription(), p.GetEnumValue().ToString()));

            }
        }

        protected void chkUseTemplate_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUseTemplate.Checked)
            {
                objEmailTemplateService = ServiceFactory.EmailTemplateService;
                DataTable dtEmailTemplate = new DataTable();
                objEmailTemplateService.GetByName("GeneralMailTemplate", ref dtEmailTemplate);
                if (dtEmailTemplate.Rows.Count > 0)
                {
                    txtSubject.Value = dtEmailTemplate.Rows[0]["EmailTemplateSubject"].ToString();
                    txtBody.Value = dtEmailTemplate.Rows[0]["EmailBody"].ToString();
                }
            }
            else
            {
                txtSubject.Value = "";
                txtBody.Value = "";
            }

        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                var checked1 = false;

                foreach (ListViewDataItem item in lstUser.Items)
                {
                    CheckBox chkUser = item.FindControl("chkUser") as CheckBox;
                    HiddenField hdnEmail = item.FindControl("hdnEmail") as HiddenField;
                    HiddenField hdnName = item.FindControl("hdnName") as HiddenField;
                    if (chkUser.Checked)
                    {
                        checked1 = true;
                        break;
                    }
                }
                if (!checked1)
                {
                    dvMessage.InnerHtml = "<strong>Error!</strong> :" + "Selct at least one user to send email.";
                    dvMessage.Attributes.Add("class", "alert alert-error");
                    dvMessage.Visible = true;
                    return;
                }
                foreach (ListViewDataItem item in lstUser.Items)
                {
                    CheckBox chkUser = item.FindControl("chkUser") as CheckBox;
                    HiddenField hdnEmail = item.FindControl("hdnEmail") as HiddenField;
                    HiddenField hdnName = item.FindControl("hdnName") as HiddenField;
                    if (chkUser.Checked)
                    {
                        string htmlbody = txtBody.Value;
                        htmlbody = htmlbody.Replace("{{Name}}", hdnName.Value);
                        Email.SendEmail(txtSubject.Value, hdnEmail.Value, "", "", htmlbody);
                    }
                }
                dvMessage.InnerHtml = "<strong>Email Send Successfully</strong> ";
                dvMessage.Attributes.Add("class", "alert alert-success");
                dvMessage.Visible = true;
            }
            catch (Exception ex)
            {
                dvMessage.InnerHtml = "<strong>Error!</strong> " + ex.Message.Trim();
                dvMessage.Attributes.Add("class", "alert alert-error");
                dvMessage.Visible = true;
            }
        }

        protected void chkClient_CheckedChanged(object sender, EventArgs e)
        {
            objUsersService = ServiceFactory.UsersService;
            bool ClientAdded = false;
            bool EmpAdded = false;
            bool AdminAdded = false;
            bool WarehouseAdded = false;
            bool PartnerAdded = false;

            if (chkClient.Checked)
            {
                ClientAdded = true;
            }
            chkEmployee.Checked = false;
            chkAdmin.Checked = false;
            chkWarehouseUser.Checked = false;
            chkPartner.Checked = false;
            //chkClient.Checked = false;

            DataTable dtUser = new DataTable();
            objUsersService.GetAllUserForEmail(ClientAdded, EmpAdded, AdminAdded, WarehouseAdded, PartnerAdded, ref dtUser);
            lstUser.DataSource = dtUser;
            lstUser.DataBind();
        }
        protected void chkPartner_CheckedChanged(object sender, EventArgs e)
        {
            objUsersService = ServiceFactory.UsersService;
            bool ClientAdded = false;
            bool EmpAdded = false;
            bool AdminAdded = false;
            bool WarehouseAdded = false;
            bool PartnerAdded = false;

            if (chkPartner.Checked)
            {
                ClientAdded = true;
            }
            chkEmployee.Checked = false;
            chkAdmin.Checked = false;
            chkWarehouseUser.Checked = false;            
            chkClient.Checked = false;
            //if (chkEmployee.Checked)
            //{
            //    EmpAdded = true;
            //}
            //if (chkAdmin.Checked)
            //{
            //    AdminAdded = true;
            //}
            //if (chkWarehouseUser.Checked)
            //{
            //    WarehouseAdded = true;
            //}
            //if (chkPartner.Checked)
            //{
            //    PartnerAdded = true;
            //}

            DataTable dtUser = new DataTable();
            objUsersService.GetAllUserForEmail(ClientAdded, EmpAdded, AdminAdded, WarehouseAdded, PartnerAdded, ref dtUser);
            lstUser.DataSource = dtUser;
            lstUser.DataBind();
        }
        protected void chkEmployee_CheckedChanged(object sender, EventArgs e)
        {
            objUsersService = ServiceFactory.UsersService;
            bool ClientAdded = false;
            bool EmpAdded = false;
            bool AdminAdded = false;
            bool WarehouseAdded = false;
            bool PartnerAdded = false;

            //if (chkClient.Checked)
            //{
            //    ClientAdded = true;
            //}
            if (chkEmployee.Checked)
            {
                EmpAdded = true;
            }
            chkAdmin.Checked = false;
            chkWarehouseUser.Checked = false;
            chkPartner.Checked = false;
            chkClient.Checked = false;
            //if (chkAdmin.Checked)
            //{
            //    AdminAdded = true;
            //}
            //if (chkWarehouseUser.Checked)
            //{
            //    WarehouseAdded = true;
            //}
            //if (chkPartner.Checked)
            //{
            //    PartnerAdded = true;
            //}

            DataTable dtUser = new DataTable();
            objUsersService.GetAllUserForEmail(ClientAdded, EmpAdded, AdminAdded, WarehouseAdded, PartnerAdded, ref dtUser);
            lstUser.DataSource = dtUser;
            lstUser.DataBind();
        }
        protected void chkWarehouseUser_CheckedChanged(object sender, EventArgs e)
        {
            objUsersService = ServiceFactory.UsersService;
            bool ClientAdded = false;
            bool EmpAdded = false;
            bool AdminAdded = false;
            bool WarehouseAdded = false;
            bool PartnerAdded = false;

            //if (chkClient.Checked)
            //{
            //    ClientAdded = true;
            //}
            //if (chkEmployee.Checked)
            //{
            //    EmpAdded = true;
            //}
            //if (chkAdmin.Checked)
            //{
            //    AdminAdded = true;
            //}
            if (chkWarehouseUser.Checked)
            {
                WarehouseAdded = true;
            }
            chkEmployee.Checked = false;
            chkAdmin.Checked = false;
            //chkWarehouseUser.Checked = false;
            chkPartner.Checked = false;
            chkClient.Checked = false;
            //if (chkPartner.Checked)
            //{
            //    PartnerAdded = true;
            //}

            DataTable dtUser = new DataTable();
            objUsersService.GetAllUserForEmail(ClientAdded, EmpAdded, AdminAdded, WarehouseAdded, PartnerAdded, ref dtUser);
            lstUser.DataSource = dtUser;
            lstUser.DataBind();
        }
        protected void chkAdmin_CheckedChanged(object sender, EventArgs e)
        {
            objUsersService = ServiceFactory.UsersService;
            bool ClientAdded = false;
            bool EmpAdded = false;
            bool AdminAdded = false;
            bool WarehouseAdded = false;
            bool PartnerAdded = false;

            //if (chkClient.Checked)
            //{
            //    ClientAdded = true;
            //}
            //if (chkEmployee.Checked)
            //{
            //    EmpAdded = true;
            //}
            if (chkAdmin.Checked)
            {
                AdminAdded = true;
            }
            //if (chkWarehouseUser.Checked)
            //{
            //    WarehouseAdded = true;
            //}
            //if (chkPartner.Checked)
            //{
            //    PartnerAdded = true;
            //}
            chkEmployee.Checked = false;
            //chkAdmin.Checked = false;
            chkWarehouseUser.Checked = false;
            chkPartner.Checked = false;
            chkClient.Checked = false;
            DataTable dtUser = new DataTable();
            objUsersService.GetAllUserForEmail(ClientAdded, EmpAdded, AdminAdded, WarehouseAdded, PartnerAdded, ref dtUser);
            lstUser.DataSource = dtUser;
            lstUser.DataBind();
        }
    }
}