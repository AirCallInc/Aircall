using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Services;
using System.Data;
using Newtonsoft.Json;

namespace Aircall.Common
{
    public class WorkAreas
    {
        public string key { get; set; }
        public string label { get; set; }
        public bool open { get; set; }
        public List<WorkAreaEmployee> children { get; set; }
    }

    public class WorkAreaEmployee
    {
        public string key { get; set; }
        public string label { get; set; }
    }

    public class EmployeeSchedule
    {
        public string section_id { get; set; }
        public string text { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string color { get; set; }
    }

    public class MarkerData
    {
        public string ServiceCaseNumber { get; set; }
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
        public decimal EmpLat { get; set; }
        public decimal EmpLng { get; set; }
        public string EmpName { get; set; }
    }

    public class ReInitializeTimeline
    {
        public string workareaEmployee { get; set; }
        public string employeeSchedule { get; set; }
    }

    public class CalendarData
    {
        public static string GetElementJsonString(int AId, int EId, int CId)
        {
            List<WorkAreas> returnResult = new List<WorkAreas>();
            string JsonString = string.Empty;
            ICalendarService objCalendarService = ServiceFactory.CalendarService;
            DataTable dtWorkAreas = new DataTable();
            objCalendarService.GetCityWorkAreaEmployees(CId, EId, ref dtWorkAreas);

            foreach (DataRow dr in dtWorkAreas.Rows)
            {
                int AreaId = Convert.ToInt32(dr["Id"].ToString());
                string AreaName = dr["Name"].ToString();
                returnResult.Add(new WorkAreas() { key = "W-" + AreaId.ToString(), label = AreaName, open = true });

                DataTable dtEmployee = new DataTable();
                dtEmployee.Rows.Clear();
                objCalendarService.GetWorkAreaEmployees(AreaId, EId, ref dtEmployee);

                var regionObject = returnResult.Find(row => row.key == "W-" + AreaId.ToString());
                if (regionObject.children == null)
                {
                    regionObject.children = new List<WorkAreaEmployee>();
                }
                foreach (DataRow dre in dtEmployee.Rows)
                {
                    int EmployeeId = Convert.ToInt32(dre["EmployeeId"].ToString());
                    string EmployeeName = dre["EmployeeName"].ToString();
                    regionObject.children.Add(new WorkAreaEmployee() { key = "E-" + EmployeeId + "W-" + AreaId, label = EmployeeName });
                }
            }
            if (returnResult.Count > 0)
            {
                JsonString = JsonConvert.SerializeObject(returnResult);
            }
            if (string.IsNullOrEmpty(JsonString))
                JsonString = "[]";
            return JsonString;
        }

        public static string GetEmployeeSchedule(int EId, int CityId)
        {
            List<EmployeeSchedule> returnResult = new List<EmployeeSchedule>();
            string JsonString = string.Empty;
            ICalendarService objCalendarService = ServiceFactory.CalendarService;
            DataTable dtEmployees = new DataTable();
            objCalendarService.GetEmployeeScheduleCityWise(EId, CityId, ref dtEmployees);

            foreach (DataRow dr in dtEmployees.Rows)
            {
                int EmployeeId = Convert.ToInt32(dr["EmployeeId"].ToString());
                DataTable dtSchedule = new DataTable();
                dtSchedule.Rows.Clear();

                objCalendarService.GetEmployeeScheduleCityWise(EmployeeId, CityId, ref dtSchedule);
                foreach (DataRow drs in dtSchedule.Rows)
                {
                    string description = drs["Description"].ToString();
                    string WAreaid = drs["WorkAreaId"].ToString();
                    string start = Convert.ToDateTime(drs["StartDate"].ToString()).ToString("yyyy-MM-dd HH:mm");
                    string color = description.ToLower() == "onleave" ? "red" : "";
                    string end = Convert.ToDateTime(drs["EndDate"].ToString()).ToString("yyyy-MM-dd HH:mm");
                    returnResult.Add(new EmployeeSchedule() { section_id = "E-" + EmployeeId.ToString() + "W-" + WAreaid, text = description, start_date = start, end_date = end, color = color });
                }
            }
            if (returnResult.Count > 0)
            {
                JsonString = JsonConvert.SerializeObject(returnResult);
            }
            if (string.IsNullOrEmpty(JsonString))
                JsonString = "[]";
            return JsonString;
        }

        public static string GetEmployeeScheduleFilter(int EId, int CityId)
        {
            List<EmployeeSchedule> returnResult = new List<EmployeeSchedule>();
            string JsonString = string.Empty;
            ICalendarService objCalendarService = ServiceFactory.CalendarService;
            DataTable dtSchedule = new DataTable();

            objCalendarService.GetEmployeeScheduleCityWise(EId, CityId, ref dtSchedule);
            //objCalendarService.GetEmployeeSchedule(EId, ref dtSchedule);
            foreach (DataRow drs in dtSchedule.Rows)
            {
                string description = drs["Description"].ToString();
                string WAreaid = drs["WorkAreaId"].ToString();
                string start = Convert.ToDateTime(drs["StartDate"].ToString()).ToString("yyyy-MM-dd HH:mm");
                string color = description.ToLower() == "onleave" ? "red" : "";
                string end = Convert.ToDateTime(drs["EndDate"].ToString()).ToString("yyyy-MM-dd HH:mm");
                returnResult.Add(new EmployeeSchedule() { section_id = "E-" + EId.ToString() + "W-" + WAreaid, text = description, start_date = start, end_date = end, color = color });
            }
            if (returnResult.Count > 0)
            {
                JsonString = JsonConvert.SerializeObject(returnResult);
            }
            return JsonString;
        }
    }
}