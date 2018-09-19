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
    public partial class schedule : System.Web.UI.Page
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ClientLoginCookie"] == null)
            {
                Response.Redirect(Application["SiteAddress"] + "sign-in.aspx");
            }

            if (Session["success"] != null)
            {
                if (Session["success"].ToString() == "1")
                {
                    dvMessage.InnerHtml = General.GetSitesettingsValue("ServiceRescheduleSuccessMessage");
                    dvMessage.Attributes.Add("class", "success");
                    dvMessage.Visible = true;
                }
                Session["success"] = null;
            }
        }
        [System.Web.Services.WebMethod]
        public static List<string> GetClientSchedulesDate()
        {
            List<string> clientdateList = new List<string>();
            IClientService objClientService = ServiceFactory.ClientService;
            DataTable dtClientScheduleDate = new DataTable();
            int ClientId = (HttpContext.Current.Session["ClientLoginCookie"] as LoginModel).Id;
            objClientService.GetClientScheduleDates(ClientId, ref dtClientScheduleDate);
            if (dtClientScheduleDate.Rows.Count > 0)
            {
                for (int i = 0; i < dtClientScheduleDate.Rows.Count; i++)
                {
                    if (clientdateList.Count == 0)
                    {
                        clientdateList.Add(dtClientScheduleDate.Rows[i]["ScheduleDate"].ToString());
                    }
                    else
                    {
                        var checkexists = clientdateList.Where(s => s.Equals(dtClientScheduleDate.Rows[i]["ScheduleDate"].ToString())).FirstOrDefault();
                        if (checkexists == null)
                        {
                            clientdateList.Add(dtClientScheduleDate.Rows[i]["ScheduleDate"].ToString());
                        }
                    }
                }
            }
            return clientdateList;
        }
        [System.Web.Services.WebMethod]
        public static string GetSelectedScheduleService(string dateText)
        {
            var finalstring = string.Empty;
            List<ScheduledServiceResult> result = new List<ScheduledServiceResult>();

            IClientService objClientService = ServiceFactory.ClientService;
            DataTable dtClientServiceDetails = new DataTable();
            int ClientId = (HttpContext.Current.Session["ClientLoginCookie"] as LoginModel).Id;
            objClientService.GetClientScheduleServicesByDate(ClientId, dateText, ref dtClientServiceDetails);
            if (dtClientServiceDetails.Rows.Count > 0)
            {
                for (int i = 0; i < dtClientServiceDetails.Rows.Count; i++)
                {
                    if (result.Count == 0)
                    {
                        result.Add(new ScheduledServiceResult { Id = dtClientServiceDetails.Rows[i]["Id"].ToString(), ScheduleCDate = dtClientServiceDetails.Rows[i]["ScheduleCDate"].ToString(), ScheduleStartTime = dtClientServiceDetails.Rows[i]["ScheduleStartTime"].ToString(), ScheduleEndTime = dtClientServiceDetails.Rows[i]["ScheduleEndTime"].ToString(), PurposeMessage = dtClientServiceDetails.Rows[i]["PurposeMessage"].ToString(), Status = dtClientServiceDetails.Rows[i]["Status"].ToString(), ServiceCount = int.Parse(dtClientServiceDetails.Rows[i]["ServiceCount"].ToString()) });
                    }
                    else
                    {
                        var checkexists = result.Where(s => s.ScheduleStartTime.Equals(dtClientServiceDetails.Rows[i]["ScheduleStartTime"].ToString()) && s.ScheduleEndTime.Equals(dtClientServiceDetails.Rows[i]["ScheduleEndTime"].ToString()) && s.ScheduleCDate.Equals(dtClientServiceDetails.Rows[i]["ScheduleCDate"].ToString())).FirstOrDefault();
                        //if (checkexists == null)
                        {
                            result.Add(new ScheduledServiceResult { Id = dtClientServiceDetails.Rows[i]["Id"].ToString(), ScheduleCDate = dtClientServiceDetails.Rows[i]["ScheduleCDate"].ToString(), ScheduleStartTime = dtClientServiceDetails.Rows[i]["ScheduleStartTime"].ToString(), ScheduleEndTime = dtClientServiceDetails.Rows[i]["ScheduleEndTime"].ToString(), PurposeMessage = dtClientServiceDetails.Rows[i]["PurposeMessage"].ToString(), Status = dtClientServiceDetails.Rows[i]["Status"].ToString(), ServiceCount = int.Parse(dtClientServiceDetails.Rows[i]["ServiceCount"].ToString()) });
                        }
                    }
                }
            }
            finalstring = BindHTML(finalstring, result);
            return finalstring;
        }

        private static string BindHTML(string finalstring, List<ScheduledServiceResult> result)
        {
            foreach (var item in result)
            {
                //complete date block
                //finalstring = finalstring + "<div class='dis-table schedule-row-visits'>" + "<div class='dis-table-cell date-block'>" + "<span class='date'>" + item.ScheduleDate + "</span>" + "<span class='month'>" + item.ScheduleMonth + "</span>" + "<span class='year'>" + item.ScheduleYear + "</span>" + "</div>";

                //complete time block
                if (item.Status == General.ServiceTypes.Scheduled.GetEnumDescription())
                {
                    finalstring = finalstring + "<div class='dis-table schedule-row-visits'>" + "<div class='dis-table-cell date-block'>" + item.ScheduleCDate + "</div>";
                    finalstring = finalstring + "<div class='dis-table-cell time'>" + item.ScheduleStartTime + " - " + DateTime.Parse(item.ScheduleEndTime).AddHours(1).ToString(" hh:mm tt") + "</div>";
                    //complete message part
                    finalstring = finalstring + "<div class='dis-table-cell message'><a href = 'ServiceScheduleDetail.aspx?Id=" + item.Id + "'>" + item.PurposeMessage + "</a></div>";
                    //complete edit block
                    finalstring = finalstring + "<div class='dis-table-cell edit-icon mobile'><a href = 'reschedule.aspx?Id=" + item.Id + "'></a></div>";
                }
                else
                {
                    finalstring = finalstring + "<div class='dis-table schedule-row-visits'>" + "<div class='dis-table-cell date-block'>" + item.ServiceCount.Ordinal() + " Appointment" + "</div>";
                    finalstring = finalstring + "<div class='dis-table-cell time'>" + item.ScheduleCDate + "</div>";

                    //complete message part
                    finalstring = finalstring + "<div class='dis-table-cell message'>" + item.PurposeMessage + "</div>";
                    //complete edit block
                    finalstring = finalstring + "<div class='dis-table-cell edit-icon mobile'></div>";
                }


                finalstring = finalstring + "</div>";

            }
            if (result.Count == 0)
            {
                finalstring = finalstring + "<div class='dis-table schedule-row-visits'>" + "<div class='dis-table-cell message' style='text-align: center;'>No records found.</div>";
                finalstring = finalstring + "</div>";
            }
            return finalstring;
        }
    }
}