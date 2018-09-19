using api.ActionFilters;
using api.App_Start;
using api.Models;
using api.Repository;
using api.ViewModel;
using AutoMapper;
using System.IdentityModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Script.Serialization;
using System.Data;
using DBUtility;

namespace api.Controllers
{
    [RoutePrefix("v1/EmpCommon")]
    public class EmpCommonController : BaseEmployeeApiController
    {
        Aircall_DBEntities1 db;
        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpGet]
        [Route("GetAllPlanType")]
        public async Task<IHttpActionResult> GetAllPlanType([FromUri] int EmployeeId)
        {
            ResponseModel res = new ResponseModel();
            db = new Aircall_DBEntities1();
            var EmpInfo = db.Employees.Where(x => x.Id == EmployeeId).FirstOrDefault();
            if (EmpInfo == null)
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "Your account was deactivated by Admin.";
                res.Data = null;
                db.Dispose();
                return Ok(res);
            }
            else
            {
                if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                    db.Dispose();
                    return Ok(res);
                }
            }
            var plantype = db.Plans.Where(x => x.Status == true && x.IsVisible == true && x.IsDeleted == false).OrderBy(x => x.PlanType.Order).Select(x => new
            {
                Id = x.PlanTypeId,
                Name = x.Name,
                x.PlanType.ServiceSlot1,
                x.PlanType.ServiceSlot2,
                x.PlanType.Order
            }).Distinct().OrderBy(x => x.Order).ToList();
            //var plantype = await db.PlanTypes.Where(x => x.Status == true).Select(x => new { Id = x.Id, Name = x.Name, x.ServiceSlot1, x.ServiceSlot2 }).ToListAsync();
            if (plantype.Count > 0)
            {
                res.StatusCode = (int)HttpStatusCode.OK;
                res.Message = "Success";
                res.Data = plantype;
            }
            else
            {
                res.StatusCode = (int)HttpStatusCode.NotFound;
                res.Message = "PlanType not found.";
                res.Data = "";
            }
            if (updatetoken)
            {
                res.Token = accessToken;
            }
            else
            {
                res.Token = "";
            }
            db.Dispose();
            return Ok(res);
        }

        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("GetSpecialRateByPlanType")]
        public async Task<IHttpActionResult> GetSpecialRateByPlanType([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId).FirstOrDefault();
            if (EmpInfo == null)
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "Your account was deactivated by Admin.";
                res.Data = null;
                db.Dispose();
                return Ok(res);
            }
            else
            {
                if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                    db.Dispose();
                    return Ok(res);
                }
            }
            var Plans = await db.Plans.Where(x => x.PlanTypeId == request.PlanTypeId && x.IsVisible == true && x.IsDeleted == false).ToListAsync();

            if (Plans.Count > 0)
            {
                var sprates = Plans[0];
                if (sprates.ShowSpecialPrice == true)
                {
                    var srate = new
                    {
                        Id = sprates.Id,
                        SpecialText = ("Save $" + ((sprates.PricePerMonth * sprates.DurationInMonth) - sprates.DiscountPrice) + " and Pay only $" + sprates.DiscountPrice + " now!"),
                        Price = (sprates.PricePerMonth * sprates.DurationInMonth),
                        PricePerMonth = sprates.PricePerMonth,
                        ShowAutoRenewal = sprates.ShowAutoRenewal,
                        sprates.DurationInMonth,
                        SpecialPrice = ((sprates.PricePerMonth * sprates.DurationInMonth) - sprates.DiscountPrice),
                        DiscountPrice = sprates.DiscountPrice,
                        Display = true
                    };
                    res.StatusCode = (int)HttpStatusCode.OK;
                    res.Message = "Record Found.";
                    res.Data = srate;
                }
                else
                {
                    var srate = new
                    {
                        Id = sprates.Id,
                        SpecialText = "",
                        Price = (sprates.PricePerMonth * sprates.DurationInMonth),
                        PricePerMonth = sprates.PricePerMonth,
                        ShowAutoRenewal = sprates.ShowAutoRenewal,
                        sprates.DurationInMonth,
                        SpecialPrice = ((sprates.PricePerMonth * sprates.DurationInMonth) - sprates.DiscountPrice),
                        DiscountPrice = sprates.DiscountPrice,
                        Display = false
                    };
                    res.StatusCode = (int)HttpStatusCode.OK;
                    res.Message = "Record Found.";
                    res.Data = srate;
                }
            }
            else
            {
                res.StatusCode = (int)HttpStatusCode.NotFound;
                res.Message = "No record found";
                res.Data = null;
            }
            if (updatetoken)
            {
                res.Token = accessToken;
            }
            else
            {
                res.Token = "";
            }
            db.Dispose();
            return Ok(res);
        }

        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("GetPartsByTypeForUnit")]
        public async Task<IHttpActionResult> GetPartsByTypeForUnit([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId && x.IsDeleted == false).FirstOrDefault();
            if (EmpInfo == null)
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "Your account was deactivated by Admin.";
                res.Data = null;
                db.Dispose();
                return Ok(res);
            }
            else
            {
                if (!EmpInfo.IsActive)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                    db.Dispose();
                    return Ok(res);
                }
            }
            if (request.PartTypeId != null)
            {
                var Parts = from p in db.Parts
                            join dp in db.DailyPartListMasters on p.DailyPartListMasterId equals dp.Id
                            where dp.Id == request.PartTypeId && p.IsDeleted == false && p.Status == true
                            select new { PartId = p.Id, Name = p.Name, Size = p.Size ?? "", SellingPrice = p.SellingPrice, DailyPartListMasterId = p.DailyPartListMasterId };
                if (Parts.Count() > 0)
                {
                    res.StatusCode = (int)HttpStatusCode.OK;
                    res.Message = "Record Found.";
                    res.Data = await Parts.ToListAsync();
                }
                else
                {
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Message = "No record found";
                    res.Data = null;
                }
            }
            else
            {
                var Parts = from p in db.Parts
                            where p.IsDeleted == false && p.Status == true
                            select new { PartId = p.Id, Name = p.Name, Size = p.Size ?? "", SellingPrice = p.SellingPrice, DailyPartListMasterId = (p.DailyPartListMasterId.HasValue ? p.DailyPartListMasterId.Value.ToString() : "") };
                if (Parts.Count() > 0)
                {
                    res.StatusCode = (int)HttpStatusCode.OK;
                    res.Message = "Record Found.";
                    res.Data = await Parts.ToListAsync();
                }
                else
                {
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Message = "No record found";
                    res.Data = null;
                }
            }
            if (updatetoken)
            {
                res.Token = accessToken;
            }
            else
            {
                res.Token = "";
            }
            db.Dispose();
            return Ok(res);
        }

        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("GetCMSPages")]
        public async Task<IHttpActionResult> GetCMSPages([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            List<object> data = new List<object>();
            var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId && x.IsDeleted == false).FirstOrDefault();
            if (EmpInfo == null)
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "Your account was deactivated by Admin.";
                res.Data = null;
                db.Dispose();
                return Ok(res);
            }
            else
            {
                if (!EmpInfo.IsActive)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                    db.Dispose();
                    return Ok(res);
                }
            }
            var MobileScreen = db.MobileScreens.Where(x => x.Id == request.PageId).FirstOrDefault();

            try
            {
                var d = new
                {
                    MobileScreen.PageTitle,
                    MobileScreen.Description
                };
                res.Message = "Record Found";
                res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                res.Data = d;
            }
            catch (Exception ex)
            {
                res.StatusCode = HttpStatusCode.InternalServerError.GetEnumValue();
                res.Message = "Internal Server Error";
                res.Data = null;
            }

            if (updatetoken)
            {
                res.Token = accessToken;
            }
            else
            {
                res.Token = "";
            }
            db.Dispose();
            return Ok(res);
        }

        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("GetPartList")]
        public async Task<IHttpActionResult> GetPartList([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            List<object> data = new List<object>();
            var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId && x.IsDeleted == false).FirstOrDefault();
            if (EmpInfo == null)
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "Your account was deactivated by Admin.";
                res.Data = null;
                db.Dispose();
                return Ok(res);
            }
            else
            {
                if (!EmpInfo.IsActive)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                    db.Dispose();
                    return Ok(res);
                }
            }
            var Parts = db.Parts.Where(x => x.IsDeleted == false && x.Status == true).AsEnumerable().Select(x => new
            {
                PartId = x.Id,
                x.Name,
                Size = (string.IsNullOrWhiteSpace(x.Size) ? "" : x.Size),
                x.SellingPrice,
                x.DailyPartListMasterId
            }).ToList();
            if (Parts.Count > 0)
            {
                res.Message = "Record Found";
                res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                res.Data = Parts;
            }
            else
            {
                res.StatusCode = (int)HttpStatusCode.NotFound;
                res.Message = "No record found";
                res.Data = null;
            }
            if (updatetoken)
            {
                res.Token = accessToken;
            }
            else
            {
                res.Token = "";
            }
            db.Dispose();
            return Ok(res);
        }
        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("GetPartById")]
        public async Task<IHttpActionResult> GetPartById([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            List<object> data = new List<object>();
            var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId && x.IsDeleted == false).FirstOrDefault();
            if (EmpInfo == null)
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "Your account was deactivated by Admin.";
                res.Data = null;
                db.Dispose();
                return Ok(res);
            }
            else
            {
                if (!EmpInfo.IsActive)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                    db.Dispose();
                    return Ok(res);
                }
            }
            var Parts = db.Parts.Where(x => x.IsDeleted == false && x.Status == true && x.Id == request.PartId).AsEnumerable().Select(x => new
            {
                PartId = x.Id,
                x.Name,
                Size = (string.IsNullOrWhiteSpace(x.Size) ? "" : x.Size),
                x.SellingPrice,
                x.Description,
                x.DailyPartListMasterId
            }).ToList();
            if (Parts.Count > 0)
            {
                res.Message = "Record Found";
                res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                res.Data = Parts.FirstOrDefault();
            }
            else
            {
                res.StatusCode = (int)HttpStatusCode.NotFound;
                res.Message = "No record found";
                res.Data = null;
            }
            if (updatetoken)
            {
                res.Token = accessToken;
            }
            else
            {
                res.Token = "";
            }
            db.Dispose();
            return Ok(res);
        }

        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("GetScheduleTimeByServiceId")]
        public async Task<IHttpActionResult> GetScheduleTimeByPlanTypeServiceId([FromBody]CommonRequest request)
        {
            ResponseModel res = new ResponseModel();
            db = new Aircall_DBEntities1();

            var MaintenanceServicesWithinDays = Utilities.GetSiteSettingValue("MaintenanceServicesWithinDays", db);
            var EmergencyAndOtherServiceWithinDays = Utilities.GetSiteSettingValue("EmergencyAndOtherServiceWithinDays", db);
            var EmergencyServiceSlot1 = Utilities.GetSiteSettingValue("EmergencyServiceSlot1", db);
            var EmergencyServiceSlot2 = Utilities.GetSiteSettingValue("EmergencyServiceSlot2", db);

            string lunchtime = Utilities.GetSiteSettingValue("EmployeeLunchTime", db);

            var MaintenanceServiceSubmitMessage = Utilities.GetSiteSettingValue("MaintenanceServiceSubmitMessage", db);
            var RepairServiceSubmitMessage = Utilities.GetSiteSettingValue("RepairServiceSubmitMessage", db);
            var ContinuingPreviousWorkServiceSubmitMessage = Utilities.GetSiteSettingValue("ContinuingPreviousWorkServiceSubmitMessage", db);
            var EmergencyServiceSubmitMessage = Utilities.GetSiteSettingValue("EmergencyServiceSubmitMessage", db);

            var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId && x.IsDeleted == false).FirstOrDefault();
            if (EmpInfo == null)
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "Your account was deactivated by Admin.";
                res.Data = null;
                db.Dispose();
                return Ok(res);
            }
            else
            {
                if (!EmpInfo.IsActive)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                    db.Dispose();
                    return Ok(res);
                }
            }
            if (request.ServiceId == null && request.RequestedServiceId == null)
            {
                List<object> data = new List<object>();
                var values = DurationExtensions.GetValues<Utilities.PurposeOfVisit>();
                foreach (var item in values)
                {
                    Utilities.PurposeOfVisit p = (Utilities.PurposeOfVisit)item;
                    var Message = "";
                    if (p.GetEnumDescription() == Utilities.PurposeOfVisit.Maintenance.GetEnumDescription())
                    {
                        Message = MaintenanceServiceSubmitMessage;
                    }
                    else if (p.GetEnumDescription() == Utilities.PurposeOfVisit.Repairing.GetEnumDescription())
                    {
                        Message = RepairServiceSubmitMessage;
                    }
                    else if (p.GetEnumDescription() == Utilities.PurposeOfVisit.ContinuingPreviousWork.GetEnumDescription())
                    {
                        Message = ContinuingPreviousWorkServiceSubmitMessage;
                    }
                    else if (p.GetEnumDescription() == Utilities.PurposeOfVisit.Emergency.GetEnumDescription())
                    {
                        Message = EmergencyServiceSubmitMessage;
                    }
                    data.Add(new { Id = p.GetEnumValue(), Name = p.GetEnumDisplayName(), Message = Message });
                }
                var plantype = await db.PlanTypes.Where(x => x.Status == true && x.Id == request.PlanTypeId).Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    TimeSlot1 = x.ServiceSlot1,
                    TimeSlot2 = x.ServiceSlot2,
                    PlanDetail = x.Plans.FirstOrDefault(y => y.IsVisible == true && y.Status == true && y.IsDeleted == false)
                }).ToListAsync();
                var Slot1 = Slottime1(plantype.FirstOrDefault().TimeSlot1.Split(("-").ToArray()), plantype.FirstOrDefault().TimeSlot2.Split(("-").ToArray()), plantype.FirstOrDefault().PlanDetail, lunchtime);
                var Slot2 = Slottime1(new List<string>().ToArray(), plantype.FirstOrDefault().TimeSlot2.Split(("-").ToArray()), plantype.FirstOrDefault().PlanDetail, lunchtime);
                if (plantype.Count > 0)
                {
                    res.StatusCode = (int)HttpStatusCode.OK;
                    res.Message = "Success";
                    var resp = new
                    {
                        Purpose = data,
                        TimeSlot1 = plantype.FirstOrDefault().TimeSlot1,
                        TimeSlot2 = plantype.FirstOrDefault().TimeSlot2,
                        TotalUnitSlot1 = Slot1,
                        TotalUnitSlot2 = Slot2,
                        MaintenanceServicesWithinDays,
                        EmergencyAndOtherServiceWithinDays,
                        EmergencyServiceSlot1,
                        EmergencyServiceSlot2
                    };
                    res.Data = resp;
                }
                else
                {
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Message = "PlanType not found.";
                    res.Data = "";
                }
            }
            if (request.ServiceId != null)
            {
                var RequestService = db.Services.Where(x => x.Id == request.ServiceId).FirstOrDefault();
                var Units = RequestService.ServiceUnits.Select(x => x.ClientUnit.PlanTypeId).FirstOrDefault();

                List<object> data = new List<object>();
                var values = DurationExtensions.GetValues<Utilities.PurposeOfVisit>();
                foreach (var item in values)
                {
                    Utilities.PurposeOfVisit p = (Utilities.PurposeOfVisit)item;
                    var Message = "";
                    if (p.GetEnumDescription() == Utilities.PurposeOfVisit.Maintenance.GetEnumDescription())
                    {
                        Message = MaintenanceServiceSubmitMessage;
                    }
                    else if (p.GetEnumDescription() == Utilities.PurposeOfVisit.Repairing.GetEnumDescription())
                    {
                        Message = RepairServiceSubmitMessage;
                    }
                    else if (p.GetEnumDescription() == Utilities.PurposeOfVisit.ContinuingPreviousWork.GetEnumDescription())
                    {
                        Message = ContinuingPreviousWorkServiceSubmitMessage;
                    }
                    else if (p.GetEnumDescription() == Utilities.PurposeOfVisit.Emergency.GetEnumDescription())
                    {
                        Message = EmergencyServiceSubmitMessage;
                    }
                    data.Add(new { Id = p.GetEnumValue(), Name = p.GetEnumDisplayName(), Message = Message });
                }
                var plantype = await db.PlanTypes.Where(x => x.Status == true && x.Id == Units).Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    TimeSlot1 = x.ServiceSlot1,
                    TimeSlot2 = x.ServiceSlot2,
                    PlanDetail = x.Plans.FirstOrDefault(y => y.IsVisible == true && y.Status == true && y.IsDeleted == false)
                }).ToListAsync();
                var Slot1 = Slottime1(plantype.FirstOrDefault().TimeSlot1.Split(("-").ToArray()), plantype.FirstOrDefault().TimeSlot2.Split(("-").ToArray()), plantype.FirstOrDefault().PlanDetail, lunchtime);
                var Slot2 = Slottime1(new List<string>().ToArray(), plantype.FirstOrDefault().TimeSlot2.Split(("-").ToArray()), plantype.FirstOrDefault().PlanDetail, lunchtime);
                if (plantype.Count > 0)
                {
                    res.StatusCode = (int)HttpStatusCode.OK;
                    res.Message = "Success";
                    var resp = new
                    {
                        Purpose = data,
                        TimeSlot1 = plantype.FirstOrDefault().TimeSlot1,
                        TimeSlot2 = plantype.FirstOrDefault().TimeSlot2,
                        TotalUnitSlot1 = Slot1,
                        TotalUnitSlot2 = Slot2,
                        MaintenanceServicesWithinDays,
                        EmergencyAndOtherServiceWithinDays,
                        EmergencyServiceSlot1,
                        EmergencyServiceSlot2
                    };
                    res.Data = resp;
                }
                else
                {
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Message = "PlanType not found.";
                    res.Data = "";
                }
            }
            if (request.RequestedServiceId != null)
            {
                var RequestService = db.RequestedServices.Where(x => x.Id == request.RequestedServiceId).FirstOrDefault();
                var Units = RequestService.RequestedServiceUnits.Select(x => x.ClientUnit.PlanTypeId).FirstOrDefault();

                List<object> data = new List<object>();
                var values = DurationExtensions.GetValues<Utilities.PurposeOfVisit>();
                foreach (var item in values)
                {
                    Utilities.PurposeOfVisit p = (Utilities.PurposeOfVisit)item;
                    var Message = "";
                    if (p.GetEnumDescription() == Utilities.PurposeOfVisit.Maintenance.GetEnumDescription())
                    {
                        Message = MaintenanceServiceSubmitMessage;
                    }
                    else if (p.GetEnumDescription() == Utilities.PurposeOfVisit.Repairing.GetEnumDescription())
                    {
                        Message = RepairServiceSubmitMessage;
                    }
                    else if (p.GetEnumDescription() == Utilities.PurposeOfVisit.ContinuingPreviousWork.GetEnumDescription())
                    {
                        Message = ContinuingPreviousWorkServiceSubmitMessage;
                    }
                    else if (p.GetEnumDescription() == Utilities.PurposeOfVisit.Emergency.GetEnumDescription())
                    {
                        Message = EmergencyServiceSubmitMessage;
                    }
                    data.Add(new { Id = p.GetEnumValue(), Name = p.GetEnumDisplayName(), Message = Message });
                }
                var plantype = await db.PlanTypes.Where(x => x.Status == true && x.Id == Units).Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    TimeSlot1 = x.ServiceSlot1,
                    TimeSlot2 = x.ServiceSlot2,
                    PlanDetail = x.Plans.FirstOrDefault(y => y.IsVisible == true && y.Status == true && y.IsDeleted == false)
                }).ToListAsync();
                var Slot1 = Slottime1(plantype.FirstOrDefault().TimeSlot1.Split(("-").ToArray()), plantype.FirstOrDefault().TimeSlot2.Split(("-").ToArray()), plantype.FirstOrDefault().PlanDetail, lunchtime);
                var Slot2 = Slottime1(new List<string>().ToArray(), plantype.FirstOrDefault().TimeSlot2.Split(("-").ToArray()), plantype.FirstOrDefault().PlanDetail, lunchtime);
                if (plantype.Count > 0)
                {
                    res.StatusCode = (int)HttpStatusCode.OK;
                    res.Message = "Success";
                    var resp = new
                    {
                        Purpose = data,
                        TimeSlot1 = plantype.FirstOrDefault().TimeSlot1,
                        TimeSlot2 = plantype.FirstOrDefault().TimeSlot2,
                        TotalUnitSlot1 = Slot1,
                        TotalUnitSlot2 = Slot2,
                        MaintenanceServicesWithinDays,
                        EmergencyAndOtherServiceWithinDays,
                        EmergencyServiceSlot1,
                        EmergencyServiceSlot2
                    };
                    res.Data = resp;
                }
                else
                {
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Message = "PlanType not found.";
                    res.Data = "";
                }
            }
            if (updatetoken)
            {
                res.Token = accessToken;
            }
            else
            {
                res.Token = "";
            }
            db.Dispose();
            return Ok(res);
        }

        private int Slottime1(string[] slots1, string[] slots2, Plan p, string lunchtime)
        {
            var lunchtimes = lunchtime.Split(("-").ToArray()).Select(x => new { time = DateTime.Parse(x) }).Select(x => x.time);

            DateTime dtlunchstart = lunchtimes.First();
            DateTime dtlunchend = lunchtimes.Last();

            var lunchMinut = dtlunchend.Subtract(dtlunchstart).TotalMinutes;
            int noofunit = 0;
            if (slots2.Count() > 0 && slots1.Count() > 0)
            {
                DateTime dtStart1 = DateTime.Parse(slots1.First());
                DateTime dtEnd1 = DateTime.Parse(slots1.Last());

                var st2 = slots2.Select(x => new { SlotDt = DateTime.Parse(x) }).OrderBy(x => x.SlotDt).Select(x => new { time = x.SlotDt.ToString("HH:mm") }).Select(x => x.time).ToArray();
                DateTime dtStart2 = DateTime.Parse(st2.First());
                DateTime dtEnd2 = DateTime.Parse(st2.Last());

                TimeSpan t = dtEnd2.Subtract(dtStart1);

                int TotalMinutes = int.Parse(t.TotalMinutes.ToString());

                noofunit = (int)((TotalMinutes - (p.Drivetime + p.ServiceTimeForFirstUnit + lunchMinut)) / p.ServiceTimeForAdditionalUnits) + 1;
                noofunit = (noofunit < 0 ? 0 : noofunit);
            }
            else
            {
                var st2 = slots2.Select(x => new { SlotDt = DateTime.Parse(x) }).OrderBy(x => x.SlotDt).Select(x => new { time = x.SlotDt.ToString("HH:mm") }).Select(x => x.time).ToArray();
                DateTime dtStart2 = DateTime.Parse(st2.First());
                DateTime dtEnd2 = DateTime.Parse(st2.Last());
                TimeSpan t = dtEnd2.Subtract(dtStart2);

                int TotalMinutes = int.Parse(t.TotalMinutes.ToString());

                if ((p.Drivetime + p.ServiceTimeForFirstUnit) < TotalMinutes)
                {
                    noofunit = (int)((TotalMinutes - (p.Drivetime + p.ServiceTimeForFirstUnit)) / p.ServiceTimeForAdditionalUnits) + 1;
                    noofunit = (noofunit < 0 ? 0 : noofunit);
                }
                else
                {
                    noofunit = 0;
                }
            }
            return noofunit;
        }

        //public async Task<IHttpActionResult> GetScheduleTimeByPlanTypeServiceId([FromBody]CommonRequest request)
        //{
        //    ResponseModel res = new ResponseModel();
        //    db = new Aircall_DBEntities1();
        //    var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId && x.IsDeleted == false).FirstOrDefault();
        //    if (EmpInfo == null)
        //    {
        //        res.StatusCode = (int)HttpStatusCode.NotAcceptable;
        //        res.Message = "Your account was deactivated by Admin.";
        //        res.Data = null;
        //        db.Dispose();
        //        return Ok(res);
        //    }
        //    else
        //    {
        //        if (!EmpInfo.IsActive)
        //        {
        //            res.StatusCode = (int)HttpStatusCode.NotAcceptable;
        //            res.Message = "Your account was deactivated by Admin.";
        //            res.Data = null;
        //            db.Dispose();
        //            return Ok(res);
        //        }
        //    }
        //    if (request.ServiceId == null)
        //    {
        //        List<string> data = new List<string>();
        //        var values = DurationExtensions.GetValues<Utilities.PurposeOfVisit>();
        //        foreach (var item in values)
        //        {
        //            Utilities.PurposeOfVisit p = (Utilities.PurposeOfVisit)item;
        //            data.Add(p.GetEnumDescription());
        //        }
        //        var plantype = await db.PlanTypes.Where(x => x.Status == true && x.Id == request.PlanTypeId).Select(x => new
        //        {
        //            Id = x.Id,
        //            Name = x.Name,
        //            TimeSlot1 = x.ServiceSlot1,
        //            TimeSlot2 = x.ServiceSlot2
        //        }).ToListAsync();
        //        if (plantype.Count > 0)
        //        {
        //            res.StatusCode = (int)HttpStatusCode.OK;
        //            res.Message = "Success";
        //            var resp = new
        //            {
        //                Purpose = data,
        //                TimeSlot1 = plantype.FirstOrDefault().TimeSlot1,
        //                TimeSlot2 = plantype.FirstOrDefault().TimeSlot2
        //            };
        //            res.Data = resp;
        //        }
        //        else
        //        {
        //            res.StatusCode = (int)HttpStatusCode.NotFound;
        //            res.Message = "PlanType not found.";
        //            res.Data = "";
        //        }
        //    }
        //    if (request.ServiceId != null)
        //    {
        //        List<string> data = new List<string>();
        //        var RequestService = db.Services.Where(x => x.Id == request.ServiceId).FirstOrDefault();
        //        var Units = RequestService.ServiceUnits.Select(x => x.ClientUnit.PlanTypeId).FirstOrDefault();

        //        var values = DurationExtensions.GetValues<Utilities.PurposeOfVisit>();
        //        foreach (var item in values)
        //        {
        //            Utilities.PurposeOfVisit p = (Utilities.PurposeOfVisit)item;
        //            data.Add(p.GetEnumDescription());
        //        }
        //        var plantype = await db.PlanTypes.Where(x => x.Status == true && x.Id == Units).Select(x => new
        //        {
        //            Id = x.Id,
        //            Name = x.Name,
        //            TimeSlot1 = x.ServiceSlot1,
        //            TimeSlot2 = x.ServiceSlot2
        //        }).ToListAsync();
        //        if (plantype.Count > 0)
        //        {
        //            res.StatusCode = (int)HttpStatusCode.OK;
        //            res.Message = "Success";
        //            var resp = new
        //            {
        //                Purpose = data,
        //                TimeSlot1 = plantype.FirstOrDefault().TimeSlot1,
        //                TimeSlot2 = plantype.FirstOrDefault().TimeSlot2
        //            };
        //            res.Data = resp;
        //        }
        //        else
        //        {
        //            res.StatusCode = (int)HttpStatusCode.NotFound;
        //            res.Message = "PlanType not found.";
        //            res.Data = "";
        //        }
        //    }
        //    if (request.RequestedServiceId != null)
        //    {
        //        List<string> data = new List<string>();
        //        var RequestService = db.RequestedServices.Where(x => x.ServiceId == request.ServiceId).FirstOrDefault();
        //        var Units = RequestService.RequestedServiceUnits.Select(x => x.ClientUnit.PlanTypeId).FirstOrDefault();

        //        var values = DurationExtensions.GetValues<Utilities.PurposeOfVisit>();
        //        foreach (var item in values)
        //        {
        //            Utilities.PurposeOfVisit p = (Utilities.PurposeOfVisit)item;
        //            data.Add(p.GetEnumDescription());
        //        }
        //        var plantype = await db.PlanTypes.Where(x => x.Status == true && x.Id == Units).Select(x => new
        //        {
        //            Id = x.Id,
        //            Name = x.Name,
        //            TimeSlot1 = x.ServiceSlot1,
        //            TimeSlot2 = x.ServiceSlot2
        //        }).ToListAsync();
        //        if (plantype.Count > 0)
        //        {
        //            res.StatusCode = (int)HttpStatusCode.OK;
        //            res.Message = "Success";
        //            var resp = new
        //            {
        //                Purpose = data,
        //                TimeSlot1 = plantype.FirstOrDefault().TimeSlot1,
        //                TimeSlot2 = plantype.FirstOrDefault().TimeSlot2
        //            };
        //            res.Data = resp;
        //        }
        //        else
        //        {
        //            res.StatusCode = (int)HttpStatusCode.NotFound;
        //            res.Message = "PlanType not found.";
        //            res.Data = "";
        //        }
        //    }
        //    if (updatetoken)
        //    {
        //        res.Token = accessToken;
        //    }
        //    else
        //    {
        //        res.Token = "";
        //    }
        //    db.Dispose();
        //    return Ok(res);
        //}


        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpGet]
        [Route("GetAllPlanTypeFromAddressID")]
        public async Task<IHttpActionResult> GetAllPlanTypeFromAddressID([FromUri] int AddressId, [FromUri]int EmployeeId)
        {
            ResponseModel res = new ResponseModel();
            db = new Aircall_DBEntities1();
            var EmpInfo = db.Employees.Where(x => x.Id == EmployeeId && x.IsDeleted == false).FirstOrDefault();

            if (EmpInfo == null)
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "Your account was deactivated by Admin.";
                res.Data = null;
                db.Dispose();
                return Ok(res);
            }
            else
            {
                if (!EmpInfo.IsActive)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                    db.Dispose();
                    return Ok(res);
                }
            }

            var plantype = GetSubscriptionPlan();

            if (plantype.Rows.Count > 0)
            {
                res.StatusCode = (int)HttpStatusCode.OK;
                res.Message = "Success";
                res.Data = plantype;
            }
            else
            {
                res.StatusCode = (int)HttpStatusCode.NotFound;
                res.Message = "PlanType not found.";
                res.Data = "";
            }
            if (updatetoken)
            {
                res.Token = accessToken;
            }
            else
            {
                res.Token = "";
            }
            db.Dispose();
            return Ok(res);
        }

        private DataTable GetSubscriptionPlan()
        {
            var sql = "select * from SubscriptionPlans";
            var instance = new SQLDBHelper();
            var ds = instance.Query(sql, null);
            instance.Close();
            return ds.Tables[0];
        }
    }
}
