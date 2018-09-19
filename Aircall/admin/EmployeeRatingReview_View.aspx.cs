using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Services;
using System.Data;

namespace Aircall.admin
{
    public partial class EmployeeRatingReview_View : System.Web.UI.Page
    {
        IServicesService objServicesService;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["ServiceId"]))
                {
                    long ServiceId = Convert.ToInt64(Request.QueryString["ServiceId"].ToString());
                    objServicesService = ServiceFactory.ServicesService;
                    DataTable dtService = new DataTable();
                    objServicesService.GetCompletedServiceById(ServiceId, ref dtService);
                    if (dtService.Rows.Count > 0)
                    {

                        ltrServiceCase.Text = dtService.Rows[0]["ServiceCaseNumber"].ToString();
                        ltrClient.Text = dtService.Rows[0]["ClientName"].ToString();
                        dvRating.Attributes.Add("data-rate", dtService.Rows[0]["Rating"].ToString());
                        if (string.IsNullOrEmpty(dtService.Rows[0]["ClientImage"].ToString()))
                            imgClient.ImageUrl = Application["SiteAddress"] + "uploads/profile/Client/defultimage.jpg";
                        else
                            imgClient.ImageUrl = Application["SiteAddress"] + "uploads/profile/client/" + dtService.Rows[0]["ClientImage"].ToString();
                        ltrClientName2.Text = dtService.Rows[0]["ClientName"].ToString();
                        ltrReviewDate.Text = dtService.Rows[0]["ReviewDate"].ToString();
                        ltrReview.Text = dtService.Rows[0]["Review"].ToString();
                        lnkClient.HRef = Application["SiteAddress"] + "admin/Client_AddEdit.aspx?ClientId=" + dtService.Rows[0]["ClientId"].ToString();

                        if (!string.IsNullOrEmpty(dtService.Rows[0]["EmployeNotes"].ToString()))
                        {
                            if (string.IsNullOrEmpty(dtService.Rows[0]["EmpImage"].ToString()))
                                imgEmployee.ImageUrl = Application["SiteAddress"] + "uploads/profile/employee/defultimage.jpg";
                            else
                                imgEmployee.ImageUrl = Application["SiteAddress"] + "uploads/profile/employee/" + dtService.Rows[0]["EmpImage"].ToString();

                            ltrEmpName.Text = dtService.Rows[0]["EmployeeName"].ToString();
                            ltrNoteDate.Text = dtService.Rows[0]["NotesDate"].ToString();
                            ltrNotes.Text = dtService.Rows[0]["EmployeNotes"].ToString();

                            lnkEmp.HRef=Application["SiteAddress"] + "admin/Employee_AddEdit.aspx?EmployeeId=" + dtService.Rows[0]["EmpId"].ToString();
                        }
                        else
                            Employee.Visible = false;
                    }
                }
            }
        }
    }
}