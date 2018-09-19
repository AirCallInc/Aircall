using Aircall.Common;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Aircall
{
    public partial class lowstockreminder : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            IPartsService objPartsService = ServiceFactory.PartsService;
            DataTable dtParts = new DataTable();
            objPartsService.GetLowStockDetails(ref dtParts);

            IUsersService objUsersService = ServiceFactory.UsersService;
            DataTable dtUsers = new DataTable();
            objUsersService.GetAllWareHouseUser(ref dtUsers);

            DataTable dtUsers1 = new DataTable();
            objUsersService.GetAllAdminUsers(ref dtUsers1);

            DataTable dtEmailtemplate = new DataTable();
            IEmailTemplateService objEmailTemplateService = ServiceFactory.EmailTemplateService;
            objEmailTemplateService.GetByName("LowStockReminder", ref dtEmailtemplate);
            if (dtParts.Rows.Count > 0)
            {
                if (dtEmailtemplate.Rows.Count > 0)
                {
                    string Emailbody = dtEmailtemplate.Rows[0]["EmailBody"].ToString();

                    string Subject = dtEmailtemplate.Rows[0]["EmailTemplateSubject"].ToString();

                    string CCEmail = dtEmailtemplate.Rows[0]["CCEmails"].ToString();

                    var strPartsUsed = "";

                    foreach (DataRow partRow in dtParts.Rows)
                    {
                        strPartsUsed = strPartsUsed + "<tr><td>" + partRow["SrNo"].ToString() + "</td><td>" + partRow["Name"].ToString() + " - " + partRow["Size"].ToString() + " </td><td>" + partRow["RemainingQty"].ToString() + "</td><td>" + partRow["ReorderQuantity"].ToString() + "</td></tr>";
                    }

                    if (strPartsUsed != "")
                    {
                        strPartsUsed = @"<table style='width:100%;font-family:Arial,sans-serif;font-size:14px;line-height:21px;color:#333;border-color: #e2e2e2;border-collapse: collapse;' border='1'>
                                         <tr><th>Sr No.</th><th>Part Name</th><th>Remaining Quantity</th><th>Minimum Re-order Quantity</th></tr>" + strPartsUsed + "</table>";
                    }
                    Emailbody = Emailbody.Replace("{{Parts}}", strPartsUsed);
                    foreach (DataRow row in dtUsers.Rows)
                    {
                        Emailbody = Emailbody.Replace("{{UserName}}", row["FullName"].ToString());
                        Email.SendEmail(Subject, row["Email"].ToString(), CCEmail, "", Emailbody);
                    }

                    foreach (DataRow row in dtUsers1.Rows)
                    {
                        Emailbody = Emailbody.Replace("{{UserName}}", row["FullName"].ToString());
                        Email.SendEmail(Subject, row["Email"].ToString(), CCEmail, "", Emailbody);
                    }
                }
            }
        }
    }
}