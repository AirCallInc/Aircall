using api.Models;
using api.ViewModel;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Configuration;
using System.Data.Entity;
using api.ActionFilters;
using api.App_Start;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Web;
using Stripe;
using AutoMapper;
using api.Common;
using System.Threading;
using PdfSharp.Pdf;
using PdfSharp;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using System.Text;
using ImageResizer;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Nito.AspNetBackgroundTasks;
using System.Data.Entity.Core.Objects;

namespace api.Controllers
{
    [RoutePrefix("v1/employee")]
    public class EmployeeController : BaseEmployeeApiController
    {
        Aircall_DBEntities1 db;

        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login([FromBody]LoginModel model)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            TokenDetails objToken = await generatEmpToken(model.Email, model.Password, model.DeviceToken);
            if (String.IsNullOrEmpty(objToken.error))
            {
                var user = await db.Employees.Where(x => x.Email == model.Email && x.Password == model.Password && x.IsDeleted == false).FirstOrDefaultAsync();
                if (user != null)
                {
                    if (user.IsActive)
                    {
                        //db.Entry(user).State = EntityState.Modified;
                        user.DeviceType = model.DeviceType;
                        user.DeviceToken = model.DeviceToken;
                        user.AppLoginStatus = true;
                        await db.SaveChangesAsync();

                        var clientmodel = new
                        {
                            Id = user.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            ProfileImage = (user.Image == "" ? "" : ConfigurationManager.AppSettings["EMPProfileImageURL"] + user.Image),
                            user.IsSalesPerson
                        };
                        EmployeeLocation loc = new EmployeeLocation();
                        loc.EmployeeId = user.Id;
                        loc.LastUpdatedOn = DateTime.UtcNow;
                        loc.Latitude = model.Latitude;
                        loc.Longitude = model.Longitude;
                        db.EmployeeLocations.Add(loc);
                        db.SaveChanges();
                        Add_UpdateToken(user.Id, objToken, 1);

                        res.StatusCode = (int)HttpStatusCode.OK;
                        res.Message = "Login Successfull!";
                        res.Token = objToken.access_token;
                        res.Data = clientmodel;
                    }
                    else
                    {
                        res.StatusCode = HttpStatusCode.NotAcceptable.GetEnumValue();
                        res.Message = "Your account was deactivated by Admin.";
                        res.Data = null;
                    }
                }
                else
                {
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Message = "Incorrect Email or Password";
                    res.Data = model;
                }
            }
            db.Dispose();
            return Ok(res);
        }

        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("logout")]
        public async Task<IHttpActionResult> logout([FromBody]EmpCommonModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            var user = db.EmpAppAccessTokens.Where(x => x.UserId == request.EmployeeId).ToList();
            if (user.Count > 0)
            {
                var emp = db.Employees.Find(request.EmployeeId);
                emp.AppLoginStatus = false;
                db.EmpAppAccessTokens.RemoveRange(user);
                await db.SaveChangesAsync();

                res.StatusCode = (int)HttpStatusCode.OK;
                res.Message = "Logout Successfull!";
                res.Token = "";
                res.Data = null;
            }
            else
            {
                res.StatusCode = (int)HttpStatusCode.OK;
                res.Message = "Logout Successfull!";
                res.Token = "";
                res.Data = null;
            }
            db.Dispose();
            return Ok(res);
        }

        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("ForgotPassword")]
        public async Task<IHttpActionResult> ForgotPassword([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            int RandomStringLength = Convert.ToInt32(ConfigurationManager.AppSettings["RandomStringLength"].ToString());

            var EmpInfo = db.Employees.Where(x => x.Email == request.Email).FirstOrDefault();
            if (EmpInfo != null)
            {

                if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    string randomString = Guid.NewGuid().ToString().Substring(0, RandomStringLength);
                    EmpInfo.PasswordUrl = randomString;
                    EmpInfo.ResetPasswordLinkExpireDate = DateTime.UtcNow.AddHours(24);
                    EmpInfo.IsLinkActive = true;
                    await db.SaveChangesAsync();
                    EmailTemplate templateAdmin = db.EmailTemplates.Where(x => x.Name == "ResetPasswordEmployee" && x.Status == true).FirstOrDefault();
                    var stradmin = templateAdmin.EmailBody;
                    stradmin = stradmin.Replace("{{Name}}", EmpInfo.FirstName.ToString() + " " + EmpInfo.LastName.ToString());
                    stradmin = stradmin.Replace("{{Link}}", ConfigurationManager.AppSettings["PasswordEmpUrl"].ToString() + randomString);
                    //string Emailbody = "Reset Password Link: <" + ConfigurationManager.AppSettings["PasswordEmpUrl"].ToString() + randomString;
                    api.App_Start.Utilities.Send("Reset Password", request.Email, stradmin, templateAdmin.FromEmail, db);
                    res.StatusCode = (int)HttpStatusCode.OK;
                    res.Message = "Forgot password link sent to registered email.";
                    res.Data = request.Email;
                }
            }
            else
            {
                res.StatusCode = (int)HttpStatusCode.OK;
                res.Message = "You are not registered with this email.";
                res.Data = request.Email;
            }
            db.Dispose();
            return Ok(res);
        }

        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword([FromBody]ClientChangePasswordModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            try
            {

                var EmpInfo = db.Employees.Where(x => x.Id == request.Id && x.Password == request.OldPassword).FirstOrDefault();
                if (EmpInfo != null)
                {
                    if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
                    {
                        res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        res.Message = "Your account was deactivated by Admin.";
                        res.Data = null;
                    }
                    else
                    {
                        EmpInfo.Password = request.NewPassword;
                        EmpInfo.UpdatedDate = DateTime.UtcNow;

                        await db.SaveChangesAsync();

                        res.StatusCode = (int)HttpStatusCode.OK;
                        res.Message = "Password changed successfully.";
                        res.Data = request;
                    }
                }
                else
                {
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Message = "Old Password is invalid.";
                    res.Data = request;
                }

            }
            catch (Exception Ex)
            {
                res.StatusCode = (int)HttpStatusCode.InternalServerError;
                res.Message = Ex.Message.ToString().Trim();
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
        [HttpGet]
        [Route("GetProfile")]
        public async Task<IHttpActionResult> GetClientProfile([FromUri]int EmployeeId)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            var EmpInfo = await db.Employees.Where(x => x.Id == EmployeeId).FirstOrDefaultAsync();
            if (EmpInfo != null)
            {
                if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    var FinalUserInfo = AutoMapper.Mapper.Map<EmpProfileModel>(EmpInfo);
                    FinalUserInfo.Image = (string.IsNullOrWhiteSpace(FinalUserInfo.Image) ? "" : ConfigurationManager.AppSettings["EMPProfileImageURL"] + FinalUserInfo.Image);
                    FinalUserInfo.WorkingHours = FinalUserInfo.WorkStartTime + " - " + FinalUserInfo.WorkEndTime;
                    res.StatusCode = (int)HttpStatusCode.OK;
                    res.Message = "Success";
                    res.Data = FinalUserInfo;
                }
            }
            else
            {
                res.StatusCode = (int)HttpStatusCode.NotFound;
                res.Message = "User not found.";
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
        [Route("UpdateProfile")]
        public Task<IHttpActionResult> UpdateProfile()
        {
            ResponseModel res = new ResponseModel();

            var uploadPath = new DirectoryInfo(HttpContext.Current.Server.MapPath("../../")).Parent.FullName + ConfigurationManager.AppSettings["EMPProfilePath"].ToString();//HttpContext.Current.Server.MapPath("~/Uploads");

            var multipartFormDataStreamProvider = new UploadMultipartFormProvider(uploadPath);
            var task = Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProvider).
            ContinueWith<IHttpActionResult>(t =>
            {
                try
                {
                    db = new Aircall_DBEntities1();
                    EmpProfileModel ModelData = new EmpProfileModel();
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
                    }
                    //await Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProvider);

                    ModelData.Id = int.Parse(multipartFormDataStreamProvider.FormData.GetValues("Id").First());
                    var EmpInfo = db.Employees.Where(x => x.Id == ModelData.Id).FirstOrDefault();
                    if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
                    {
                        res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        res.Message = "Your account was deactivated by Admin.";
                        res.Data = null;
                    }
                    else
                    {
                        try
                        {
                            ModelData.FirstName = multipartFormDataStreamProvider.FormData.GetValues("FirstName").First();
                        }
                        catch (Exception ex)
                        {
                        }
                        try
                        {
                            ModelData.LastName = multipartFormDataStreamProvider.FormData.GetValues("LastName").First();
                        }
                        catch (Exception ex)
                        {
                        }
                        try
                        {
                            ModelData.MobileNumber = multipartFormDataStreamProvider.FormData.GetValues("MobileNumber").First();
                        }
                        catch (Exception ex)
                        {
                        }
                        try
                        {
                            ModelData.PhoneNumber = multipartFormDataStreamProvider.FormData.GetValues("PhoneNumber").First();
                        }
                        catch (Exception ex)
                        {
                        }
                        var filename = multipartFormDataStreamProvider.FileData.Select(multiPartData => multiPartData.LocalFileName).FirstOrDefault();
                        if (string.IsNullOrWhiteSpace(filename))
                        {
                            ModelData.Image = "";
                        }
                        else
                        {
                            ModelData.Image = new FileInfo(filename).Name;
                        }
                        var UserInfo = db.Employees.Where(x => x.Id == ModelData.Id && x.IsActive == true && x.IsDeleted == false).FirstOrDefault();
                        if (UserInfo != null)
                        {
                            UserInfo.Id = ModelData.Id;
                            UserInfo.FirstName = (string.IsNullOrWhiteSpace(ModelData.FirstName) ? UserInfo.FirstName : ModelData.FirstName);
                            UserInfo.LastName = (string.IsNullOrWhiteSpace(ModelData.LastName) ? UserInfo.LastName : ModelData.LastName);
                            UserInfo.Image = (string.IsNullOrWhiteSpace(ModelData.Image) ? UserInfo.Image : ModelData.Image);
                            UserInfo.Email = (string.IsNullOrWhiteSpace(ModelData.Email) ? UserInfo.Email : ModelData.Email);
                            UserInfo.MobileNumber = (string.IsNullOrWhiteSpace(ModelData.MobileNumber) ? UserInfo.MobileNumber : ModelData.MobileNumber);
                            UserInfo.PhoneNumber = (string.IsNullOrWhiteSpace(ModelData.PhoneNumber) ? UserInfo.PhoneNumber : ModelData.PhoneNumber);

                            db.SaveChanges();

                            var FinalUserInfo = AutoMapper.Mapper.Map<EmpProfileModel>(UserInfo);
                            FinalUserInfo.Image = (FinalUserInfo.Image != "" ? ConfigurationManager.AppSettings["EMPProfileImageURL"].ToString() + FinalUserInfo.Image : "");
                            res.StatusCode = (int)HttpStatusCode.OK;
                            res.Message = "Profile Updated Successfully.";
                            res.Data = FinalUserInfo;
                        }
                        else
                        {
                            res.StatusCode = (int)HttpStatusCode.NotFound;
                            res.Message = "User not found.";
                            res.Data = null;
                        }
                    }
                }
                catch (Exception Ex)
                {
                    res.StatusCode = (int)HttpStatusCode.InternalServerError;
                    res.Message = Ex.Message.ToString().Trim();
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
            });


            return task;


        }

        private List<object> dates(EmpCommonModel request)
        {
            db = new Aircall_DBEntities1();
            List<object> data = new List<object>();

            int d = 0;
            bool isPreviousMonth = false;
            if (DateTime.UtcNow.Month == request.Month && DateTime.UtcNow.Year == request.Year)
            {
                d = (DateTime.UtcNow.Day == 1 ? DateTime.UtcNow.Day : DateTime.UtcNow.Day - 1);
            }
            else
            {
                d = 1;
                if (new DateTime(request.Year, request.Month, 1) < DateTime.UtcNow)
                {
                    isPreviousMonth = true;

                }
            }

            int days = DateTime.DaysInMonth(request.Year, request.Month);
            if (!isPreviousMonth)
            {
                for (int day = d; day <= days; day++)
                {
                    DateTime dt = new DateTime(request.Year, request.Month, day);
                    //var service = db.Services.AsEnumerable().Where(x => x.ScheduleDate == dt && ((x.Status == api.App_Start.Utilities.ServiceTypes.Completed.GetEnumDescription() && x.ServiceReports.Count > 0) || x.Status == api.App_Start.Utilities.ServiceTypes.NoShow.GetEnumDescription() || x.Status == api.App_Start.Utilities.ServiceTypes.Scheduled.GetEnumDescription()) && x.EmployeeId == request.EmployeeId).LongCount();

                    var service = db.Services.AsEnumerable().Where(x => x.ScheduleDate == dt && (x.Status == api.App_Start.Utilities.ServiceTypes.NoShow.GetEnumDescription() || x.Status == api.App_Start.Utilities.ServiceTypes.Scheduled.GetEnumDescription()) && x.EmployeeId == request.EmployeeId).LongCount();

                    if (service > 0)
                    {
                        var d1 = new { ScheduleDate = dt, Count = service };
                        data.Add(d1);
                    }
                }
                for (int day = 1; day <= d + 1; day++)
                {
                    DateTime dt = new DateTime(request.Year, request.Month, day);
                    var service = db.Services.AsEnumerable().Where(x => x.ScheduleDate == dt && ((x.Status == api.App_Start.Utilities.ServiceTypes.Completed.GetEnumDescription() && x.ServiceReports.Count > 0) || x.Status == api.App_Start.Utilities.ServiceTypes.NoShow.GetEnumDescription()) && x.EmployeeId == request.EmployeeId).LongCount();

                    if (service > 0)
                    {
                        var d1 = new { ScheduleDate = dt, Count = service };
                        data.Add(d1);
                    }
                }
            }
            else
            {
                for (int day = 1; day <= days; day++)
                {
                    DateTime dt = new DateTime(request.Year, request.Month, day);
                    var service = db.Services.AsEnumerable().Where(x => x.ScheduleDate == dt && ((x.Status == api.App_Start.Utilities.ServiceTypes.Completed.GetEnumDescription() && x.ServiceReports.Count > 0) || x.Status == api.App_Start.Utilities.ServiceTypes.NoShow.GetEnumDescription()) && x.EmployeeId == request.EmployeeId).LongCount();
                    if (service > 0)
                    {
                        var d1 = new { ScheduleDate = dt, Count = service };
                        data.Add(d1);
                    }
                }
            }

            return data;
        }

        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("GetDashBoardMonthWiseServiceCount")]
        public async Task<IHttpActionResult> GetDashBoardMonthWiseServiceCount([FromBody] EmpCommonModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            try
            {
                var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId).FirstOrDefault();
                if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    List<object> data = new List<object>();
                    List<object> data2 = new List<object>();
                    data = dates(request);

                    var nextmonth = new DateTime(request.Year, request.Month, 1).AddMonths(1);

                    request.Month = nextmonth.Month;
                    request.Year = nextmonth.Year;

                    data2 = dates(request);

                    data.AddRange(data2);

                    var NotificationsCnt = db.UserNotifications.AsEnumerable().Where(x => x.UserId == request.EmployeeId && x.DeletedBy == null && x.UserTypeId == api.App_Start.Utilities.UserRoles.Employee.GetEnumValue() && x.Status == api.App_Start.Utilities.NotificationStatus.UnRead.GetEnumDescription()).Count();
                    var objRes = new
                    {
                        UnReadCount = NotificationsCnt,
                        data = data
                    };
                    res.StatusCode = (int)HttpStatusCode.OK;
                    res.Message = "Data Found";
                    res.Data = objRes;
                }
            }
            catch (Exception ex)
            {
                res.StatusCode = (int)HttpStatusCode.BadRequest;
                res.Message = "Invalid Request.";
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
        [Route("GetDashBoardMonthDayWiseServiceList")]
        public async Task<IHttpActionResult> GetDashBoardMonthDayWiseServiceList([FromBody] EmpCommonModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            try
            {
                var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId).FirstOrDefault();
                if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    List<object> data = new List<object>();

                    DateTime dt = new DateTime(request.Year, request.Month, request.Day);
                    long ServiceId = 0;
                    var ServiceReportData = db.ServiceReports.Where(s => EntityFunctions.TruncateTime(s.AddedDate.Value) == dt.Date).FirstOrDefault();
                    if (ServiceReportData != null)
                    {
                        ServiceId = Convert.ToInt64(ServiceReportData.ServiceId);
                    }


                    var service1 = db.Services.AsEnumerable().Where(s => s.Id == ServiceId && s.EmployeeId == request.EmployeeId).Select(x => new
                    {
                        x.Id,
                        x.Client.FirstName,
                        x.Client.LastName,
                        x.PurposeOfVisit,
                        x.ScheduleStartTime,
                        x.ScheduleEndTime,
                        x.ClientAddress.Latitude,
                        x.ClientAddress.Longitude,
                        x.Status,
                        ordDate = DateTime.ParseExact(x.ScheduleDate.Value.ToString("MM/dd/yyyy") + " " + x.ScheduleStartTime, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture),
                        Address = x.ClientAddress.Address + ", " + x.ClientAddress.City1.Name + ", " + x.ClientAddress.State1.Name + " - " + x.ClientAddress.ZipCode
                    }).OrderBy(x => x.ordDate).ToList();

                    bool IsFutureDate = false;

                    var CurrentDate = DateTime.UtcNow;

                    if (dt > CurrentDate)
                        IsFutureDate = true;
                    else
                        IsFutureDate = false;

                    if (IsFutureDate == true)
                    {
                        var service4 = db.Services.AsEnumerable().Where(x => x.ScheduleDate == dt && x.EmployeeId == request.EmployeeId && (x.Status == api.App_Start.Utilities.ServiceTypes.NoShow.GetEnumDescription() || x.Status == api.App_Start.Utilities.ServiceTypes.Scheduled.GetEnumDescription())).Select(x => new
                        {
                            x.Id,
                            x.Client.FirstName,
                            x.Client.LastName,
                            x.PurposeOfVisit,
                            x.ScheduleStartTime,
                            x.ScheduleEndTime,
                            x.ClientAddress.Latitude,
                            x.ClientAddress.Longitude,
                            x.Status,
                            ordDate = DateTime.ParseExact(x.ScheduleDate.Value.ToString("MM/dd/yyyy") + " " + x.ScheduleStartTime, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture),
                            Address = x.ClientAddress.Address + ", " + x.ClientAddress.City1.Name + ", " + x.ClientAddress.State1.Name + " - " + x.ClientAddress.ZipCode
                        }).OrderBy(x => x.ordDate).ToList();

                        var service = service1.Union(service4);
                        var serviceOrderBy = service.OrderByDescending(s => s.ordDate);
                        if (serviceOrderBy.Count() > 0)
                        {
                            var d = new { ScheduleDate = dt, Services = serviceOrderBy };
                            data.Add(d);
                        }


                    }
                    else
                    {
                        var service2 = db.Services.AsEnumerable().Where(x => x.ScheduleDate == dt && x.EmployeeId == request.EmployeeId && ((x.Status == api.App_Start.Utilities.ServiceTypes.Completed.GetEnumDescription() && x.ServiceReports.Count > 0) || x.Status == api.App_Start.Utilities.ServiceTypes.NoShow.GetEnumDescription() || x.Status == api.App_Start.Utilities.ServiceTypes.Scheduled.GetEnumDescription())).Select(x => new
                        {
                            x.Id,
                            x.Client.FirstName,
                            x.Client.LastName,
                            x.PurposeOfVisit,
                            x.ScheduleStartTime,
                            x.ScheduleEndTime,
                            x.ClientAddress.Latitude,
                            x.ClientAddress.Longitude,
                            x.Status,
                            ordDate = DateTime.ParseExact(x.ScheduleDate.Value.ToString("MM/dd/yyyy") + " " + x.ScheduleStartTime, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture),
                            Address = x.ClientAddress.Address + ", " + x.ClientAddress.City1.Name + ", " + x.ClientAddress.State1.Name + " - " + x.ClientAddress.ZipCode
                        }).OrderBy(x => x.ordDate).ToList();


                        var service3 = db.Services.AsEnumerable().Where(x => x.AddedDate.Day == dt.Day && x.AddedDate.Month == dt.Month && x.AddedDate.Year == dt.Year && x.EmployeeId == request.EmployeeId && x.Status == "Completed").Select(x => new
                        {
                            x.Id,
                            x.Client.FirstName,
                            x.Client.LastName,
                            x.PurposeOfVisit,
                            x.ScheduleStartTime,
                            x.ScheduleEndTime,
                            x.ClientAddress.Latitude,
                            x.ClientAddress.Longitude,
                            x.Status,
                            ordDate = DateTime.ParseExact(x.ScheduleDate.Value.ToString("MM/dd/yyyy") + " " + x.ScheduleStartTime, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture),
                            Address = x.ClientAddress.Address + ", " + x.ClientAddress.City1.Name + ", " + x.ClientAddress.State1.Name + " - " + x.ClientAddress.ZipCode
                        }).OrderBy(x => x.ordDate).ToList();

                        var service = service1.Union(service2).Union(service3);
                        var serviceOrderBy = service.OrderByDescending(s => s.ordDate);
                        if (serviceOrderBy.Count() > 0)
                        {
                            var d = new { ScheduleDate = dt, Services = serviceOrderBy };
                            data.Add(d);
                        }
                    }
                    if (data.Count > 0)
                    {
                        res.StatusCode = (int)HttpStatusCode.OK;
                        res.Message = "Data Found";
                        res.Data = data;
                    }
                    else
                    {
                        res.StatusCode = (int)HttpStatusCode.NotFound;
                        res.Message = "Data not found.";
                        res.Data = null;
                    }
                }
            }
            catch (Exception ex)
            {
                res.StatusCode = (int)HttpStatusCode.BadRequest;
                res.Message = "Invalid Request.";
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
        [Route("GetServiceDetails")]
        public async Task<IHttpActionResult> GetServiceDetails([FromBody] EmpCommonModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            try
            {
                var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId).FirstOrDefault();
                if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    if (request.NotificationId > 0)
                    {
                        var n = db.UserNotifications.Find(request.NotificationId);
                        if (n != null)
                        {
                            n.Status = api.App_Start.Utilities.NotificationStatus.Read.GetEnumDescription();
                            db.SaveChanges();
                        }
                    }
                    var service = await db.Services.Where(x => x.Id == request.ServiceId && x.EmployeeId == request.EmployeeId).FirstOrDefaultAsync();
                    if (service != null)
                    {
                        var StartTime = Convert.ToDateTime(service.ScheduleStartTime, CultureInfo.InvariantCulture);
                        var EndTime = Convert.ToDateTime(service.ScheduleEndTime, CultureInfo.InvariantCulture);
                        //var TimeAlloted = EndTime.Subtract(StartTime).Hours + " Hours " + (EndTime.Subtract(StartTime).Minutes > 0 ? EndTime.Subtract(StartTime).Minutes + " Minutes" : "");
                        var TimeAlloted = EndTime.Subtract(StartTime).ToString(@"hh\:mm") + " Hour";
                        List<object> units = new List<object>();
                        List<object> DailyPart = new List<object>();
                        List<ReportResponce> Reports = new List<ReportResponce>();
                        List<object> UnitManuals = new List<object>();

                        var ServiceType = service.PurposeOfVisit;
                        List<DailyPartListMaster> sDailyPartList = new List<DailyPartListMaster>();
                        if (ServiceType == api.App_Start.Utilities.PurposeOfVisit.Maintenance.GetEnumDescription())
                        {
                            sDailyPartList = db.DailyPartListMasters.Where(x => x.IsIncludeInService == true).ToList();
                        }
                        foreach (var item in service.ServiceUnits)
                        {
                            DailyPart = new List<object>();

                            var CompletedUnits = service.ServiceReportUnits.Where(x => x.UnitId == item.UnitId && x.IsCompleted == true).FirstOrDefault();
                            var objServiceParts = service.ServicePartLists.Where(x => x.UnitId == item.UnitId).ToList();

                            foreach (var osp in objServiceParts)
                            {
                                var dp = new
                                {
                                    osp.PartId,
                                    osp.Part.Name,
                                    Size = osp.Part.Size ?? "",
                                    osp.PartQuantity,
                                    item.ClientUnit.UnitName
                                };
                                DailyPart.Add(dp);
                            }
                            if (item.ClientUnit.ClientUnitManuals.Count > 0)
                            {
                                for (int i = 0; i < item.ClientUnit.ClientUnitManuals.Count; i++)
                                {
                                    var manual = item.ClientUnit.ClientUnitManuals.ToList()[i];
                                    var m = new
                                    {
                                        manual.Id,
                                        ManualName = item.ClientUnit.UnitName + " - Manual " + i.ToString(),
                                        ManualURL = ConfigurationManager.AppSettings["ManualURL"].ToString() + manual.ManualName
                                    };
                                    UnitManuals.Add(m);
                                }
                            }
                            var u = new
                            {
                                item.ClientUnit.Id,
                                item.ClientUnit.UnitName,
                                ServiceCompleted = (CompletedUnits != null ? true : false),
                                DailyPartList = DailyPart
                            };
                            units.Add(u);
                            var reports1 = (from r in db.ServiceReports
                                            join ru in db.ServiceReportUnits on r.Id equals ru.ServiceReportId
                                            where ru.UnitId == item.UnitId
                                            select new
                                            {
                                                r.ServiceReportNumber,
                                                r.AddedDate,
                                                r.Id,
                                                r.EmployeeNotes
                                            }).ToList();
                            foreach (var rpt in reports1)
                            {
                                var report = new ReportResponce
                                {
                                    Id = rpt.Id,
                                    ServiceReportNumber = rpt.ServiceReportNumber,
                                    ReportDate = rpt.AddedDate.Value.ToString("dd MMMM yyyy"),
                                    EmployeeNotes = string.IsNullOrWhiteSpace(rpt.EmployeeNotes) ? "" : rpt.EmployeeNotes
                                };
                                if (Reports.Where(x => x.ServiceReportNumber == rpt.ServiceReportNumber).Count() <= 0)
                                {
                                    Reports.Add(report);
                                }

                            }
                        }

                        var d = new
                        {
                            service.Id,
                            service.ServiceCaseNumber,
                            service.Client.AccountNumber,
                            service.ClientId,
                            TimeAlloted,
                            Company = (service.Client.Company ?? "-"),
                            service.Client.FirstName,
                            service.Client.LastName,
                            PurposeOfVisit = (service.PurposeOfVisit),
                            service.Client.MobileNumber,
                            service.Client.OfficeNumber,
                            service.Client.HomeNumber,
                            service.Client.PhoneNumber,
                            service.Client.Email,
                            service.Status,
                            Address = new
                            {
                                Address = service.ClientAddress.Address,
                                City = service.ClientAddress.City1.Name,
                                State = service.ClientAddress.State1.Name,
                                ZipCode = service.ClientAddress.ZipCode,
                            },
                            service.ScheduleStartTime,
                            service.ScheduleEndTime,
                            ScheduleDate = (service.ScheduleDate != null ? service.ScheduleDate.Value.ToString("MMMM dd, yyyy") : ""),
                            service.ClientAddress.Latitude,
                            service.ClientAddress.Longitude,
                            FixedContractName = "",
                            CustomerComplaints = (service.CustomerComplaints == null ? "" : service.CustomerComplaints),
                            DispatcherNotes = (service.DispatcherNotes == null ? "" : service.DispatcherNotes),
                            TechnicianNotes = (Reports.Distinct().Count() <= 0 ? "" : Reports.Distinct().LastOrDefault().EmployeeNotes),
                            ServiceUnits = units,
                            ServiceReports = Reports.Distinct(),
                            UnitManuals = UnitManuals,
                            PurposeOfVisitId = DurationExtensions.GetValueFromDescription<api.App_Start.Utilities.PurposeOfVisit>(service.PurposeOfVisit).GetEnumValue()
                        };
                        res.Data = d;
                        res.Message = "Data Found";
                        res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                    }
                    else
                    {
                        res.Data = null;
                        res.Message = "No Data Found";
                        res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                    }
                }
            }
            catch (Exception ex)
            {
                res.StatusCode = (int)HttpStatusCode.BadRequest;
                res.Message = ex.Message;
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
        [Route("GetServiceUnitDetails")]
        public async Task<IHttpActionResult> GetServiceUnitDetails([FromBody] EmpCommonModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId).FirstOrDefault();
            if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "Your account was deactivated by Admin.";
                res.Data = null;
            }
            else
            {
                var cUnit = db.ClientUnits.Find(request.UnitId);
                if (cUnit != null)
                {
                    var UnitAddress = new
                    {
                        cUnit.ClientAddress.Address,
                        CityName = cUnit.ClientAddress.City1.Name,
                        StateName = cUnit.ClientAddress.State1.Name,
                        cUnit.ClientAddress.ZipCode
                    };

                    List<object> objUnitParts = new List<object>();

                    foreach (var cUnitPart in cUnit.ClientUnitParts)
                    {
                        List<object> objUnitFilters = new List<object>();
                        List<object> objUnitFuses = new List<object>();
                        var filterExtra = cUnitPart.UnitExtraInfoes.Where(x => x.ExtraInfoType == "Filter").ToList();
                        int FilterQty = filterExtra.Count;

                        foreach (var item in filterExtra)
                        {
                            var d = new
                            {
                                PartId = item.Part.Id,
                                FilterSize = item.Part.Name + " " + item.Part.Size ?? "",
                                FilterLocation = item.LocationOfPart.Value
                            };
                            objUnitFilters.Add(d);
                        }

                        var FusesExtra = cUnitPart.UnitExtraInfoes.Where(x => x.ExtraInfoType == "Fuses").ToList();
                        int FusesQty = FusesExtra.Count;

                        foreach (var item in FusesExtra)
                        {
                            var d = new
                            {
                                PartId = item.Part.Id,
                                FuseType = item.Part.Name
                            };
                            objUnitFuses.Add(d);
                        }
                        var splitTp = (cUnit.UnitType.Id == 2 ? cUnitPart.SplitType : cUnit.UnitType.UnitTypeName);
                        int pid = (cUnitPart.Breaker.HasValue ? cUnitPart.Breaker.Value : 0);
                        var BreakerPart = db.Parts.Where(x => x.Id == pid).FirstOrDefault();


                        var objUnitPart = new
                        {
                            cUnitPart.ModelNumber,
                            cUnitPart.SerialNumber,
                            UnitType = (cUnit.UnitType.Id == 2 ? cUnitPart.SplitType : cUnit.UnitType.UnitTypeName),
                            cUnitPart.ManufactureBrand,
                            ManufactureDate = (cUnitPart.ManufactureDate != null ? cUnitPart.ManufactureDate.Value.ToString("MMMM, yyyy") : ""),
                            UnitTon = (cUnitPart.UnitTon != null ? cUnitPart.UnitTon : "NA"),

                            //Blower Part2
                            ThermostatId = (cUnitPart.Thermostat == null ? 0 : cUnitPart.Part3.IsDeleted ? 0 : cUnitPart.Thermostat),
                            Thermostat = (cUnitPart.Thermostat != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.Thermostat.Value, db) : "NA"),

                            //Rolloutsensor Part23
                            BreakerId = (cUnitPart.Breaker == null ? 0 : cUnitPart.Part1.IsDeleted ? 0 : cUnitPart.Breaker),
                            Breaker = (cUnitPart.Breaker != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.Breaker.Value, db) : "NA"),

                            // Breaker Part1
                            BlowerMotorId = (cUnitPart.BlowerMotor == null ? 0 : cUnitPart.Part2.IsDeleted ? 0 : cUnitPart.BlowerMotor),
                            BlowerMotor = (cUnitPart.BlowerMotor != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.BlowerMotor.Value, db) : "NA"),

                            // Limitswitch Part17
                            RefrigerantTypeId = (cUnitPart.RefrigerantType == null ? 0 : cUnitPart.Part20.IsDeleted ? 0 : cUnitPart.RefrigerantType),
                            Refrigerant = (cUnitPart.RefrigerantType != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.RefrigerantType.Value, db) : "NA"),

                            // Capacitor Part4
                            CompressorId = (cUnitPart.Compressor == null ? 0 : cUnitPart.Part6.IsDeleted ? 0 : cUnitPart.Compressor),
                            Compressor = (cUnitPart.Compressor != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.Compressor.Value, db) : "NA"),

                            // Booster Part3
                            CapacitorId = (cUnitPart.Capacitor == null ? 0 : cUnitPart.Part4.IsDeleted ? 0 : cUnitPart.Capacitor),
                            Capacitor = (cUnitPart.Capacitor != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.Capacitor.Value, db) : "NA"),

                            // Compressor Part6
                            ContactorId = (cUnitPart.Contactor == null ? 0 : cUnitPart.Part8.IsDeleted ? 0 : cUnitPart.Contactor),
                            Contactor = (cUnitPart.Contactor != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.Contactor.Value, db) : "NA"),

                            // Defrostboard Part10
                            FilterdryerId = (cUnitPart.Filterdryer == null ? 0 : cUnitPart.Part12.IsDeleted ? 0 : cUnitPart.Filterdryer),
                            Filterdryer = (cUnitPart.Filterdryer != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.Filterdryer.Value, db) : "NA"),

                            // Contactor Part8
                            DefrostboardId = (cUnitPart.Defrostboard == null ? 0 : cUnitPart.Part10.IsDeleted ? 0 : cUnitPart.Defrostboard),
                            Defrostboard = (cUnitPart.Defrostboard != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.Defrostboard.Value, db) : "NA"),

                            //  Misc Part18
                            RelayId = (cUnitPart.Relay == null ? 0 : cUnitPart.Part21.IsDeleted ? 0 : cUnitPart.Relay),
                            Relay = (cUnitPart.Relay != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.Relay.Value, db) : "NA"),

                            // ReversingValve Part25
                            TXVValveId = (cUnitPart.TXVValve == null ? 0 : cUnitPart.Part25.IsDeleted ? 0 : cUnitPart.TXVValve),
                            TXVValve = (cUnitPart.TXVValve != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.TXVValve.Value, db) : "NA"),

                            // Pressureswitch Part19
                            ReversingValveId = (cUnitPart.ReversingValve == null ? 0 : cUnitPart.Part22.IsDeleted ? 0 : cUnitPart.ReversingValve),
                            ReversingValve = (cUnitPart.ReversingValve != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.ReversingValve.Value, db) : "NA"),

                            // Coil Part5
                            CondensingfanmotorId = (cUnitPart.Condensingfanmotor == null ? 0 : cUnitPart.Part7.IsDeleted ? 0 : cUnitPart.Condensingfanmotor),
                            Condensingfanmotor = (cUnitPart.Condensingfanmotor != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.Condensingfanmotor.Value, db) : "NA"),

                            InducerdraftmotorId = (cUnitPart.Inducerdraftmotor == null ? 0 : cUnitPart.Part.IsDeleted ? 0 : cUnitPart.Inducerdraftmotor),
                            Inducerdraftmotor = (cUnitPart.Inducerdraftmotor != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.Inducerdraftmotor.Value, db) : "NA"),

                            // Relay Part21
                            TransformerId = (cUnitPart.Transformer == null ? 0 : cUnitPart.Part24.IsDeleted ? 0 : cUnitPart.Transformer),
                            Transformer = (cUnitPart.Transformer != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.Transformer.Value, db) : "NA"),

                            // Condensingfanmotor Part7
                            ControlboardId = (cUnitPart.Controlboard == null ? 0 : cUnitPart.Part9.IsDeleted ? 0 : cUnitPart.Controlboard),
                            Controlboard = (cUnitPart.Controlboard != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.Controlboard.Value, db) : "NA"),

                            // Ignitioncontrolboard Part15
                            LimitswitchId = (cUnitPart.Limitswitch == null ? 0 : cUnitPart.Part17.IsDeleted ? 0 : cUnitPart.Limitswitch),
                            Limitswitch = (cUnitPart.Limitswitch != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.Limitswitch.Value, db) : "NA"),

                            // Gasvalve Part14
                            IgnitorId = (cUnitPart.Ignitor == null ? 0 : cUnitPart.Part16.IsDeleted ? 0 : cUnitPart.Ignitor),
                            Ignitor = (cUnitPart.Ignitor != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.Ignitor.Value, db) : "NA"),

                            // Filterdryer Part12
                            GasvalveId = (cUnitPart.Gasvalve == null ? 0 : cUnitPart.Part14.IsDeleted ? 0 : cUnitPart.Gasvalve),
                            Gasvalve = (cUnitPart.Gasvalve != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.Gasvalve.Value, db) : "NA"),

                            // Ignitor Part16
                            PressureswitchId = (cUnitPart.Pressureswitch == null ? 0 : cUnitPart.Part19.IsDeleted ? 0 : cUnitPart.Pressureswitch),
                            Pressureswitch = (cUnitPart.Pressureswitch != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.Pressureswitch.Value, db) : "NA"),

                            // Doorswitch Part13
                            FlamesensorId = (cUnitPart.Flamesensor == null ? 0 : cUnitPart.Part13.IsDeleted ? 0 : cUnitPart.Flamesensor),
                            Flamesensor = (cUnitPart.Flamesensor != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.Flamesensor.Value, db) : "NA"),

                            //Refrigerant Part20
                            RolloutsensorId = (cUnitPart.Rolloutsensor == null ? 0 : cUnitPart.Part23.IsDeleted ? 0 : cUnitPart.Rolloutsensor),
                            Rolloutsensor = (cUnitPart.Rolloutsensor != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.Rolloutsensor.Value, db) : "NA"),

                            // Controlboard Part9
                            DoorswitchId = (cUnitPart.Doorswitch == null ? 0 : cUnitPart.Part11.IsDeleted ? 0 : cUnitPart.Doorswitch),
                            Doorswitch = (cUnitPart.Doorswitch != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.Doorswitch.Value, db) : "NA"),

                            //Flamesensor Part13
                            IgnitioncontrolboardId = (cUnitPart.Ignitioncontrolboard == null ? 0 : cUnitPart.Part15.IsDeleted ? 0 : cUnitPart.Ignitioncontrolboard),
                            Ignitioncontrolboard = (cUnitPart.Ignitioncontrolboard != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.Ignitioncontrolboard.Value, db) : "NA"),

                            ElectricalService = (string.IsNullOrWhiteSpace(cUnitPart.ElectricalService) != true ? cUnitPart.ElectricalService : "NA"),

                            MaxBreaker = (string.IsNullOrWhiteSpace(cUnitPart.MaxBreaker) != true ? cUnitPart.MaxBreaker : "NA"),

                            //BreakerId = (cUnitPart.Breaker.HasValue ? (BreakerPart != null ? (BreakerPart.IsDeleted ? 0 : pid) : 0) : 0),
                            //Breaker = (BreakerPart != null ? BreakerPart.IsDeleted ? "NA" : BreakerPart.Name + "-" + BreakerPart.Size ?? "" : "NA"),


                            // transformer Part24
                            CoilId = (cUnitPart.Coil == null ? 0 : cUnitPart.Part5.IsDeleted ? 0 : cUnitPart.Coil),
                            Coil = (cUnitPart.Coil != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.Coil.Value, db) : "NA"),

                            // TXVValve Part25
                            MiscId = (cUnitPart.Misc == null ? 0 : cUnitPart.Part18.IsDeleted ? 0 : cUnitPart.Misc),
                            Misc = (cUnitPart.Misc != null ? api.App_Start.Utilities.GetPartDetails(cUnitPart.Misc.Value, db) : "NA"),

                            FilterQty,
                            Filters = objUnitFilters,
                            FusesQty,
                            Fuses = objUnitFuses,
                            UnitManuals = cUnit.ClientUnitManuals.AsEnumerable().Where(x => x.SplitType == cUnitPart.SplitType).Select(x => new
                            {
                                x.ManualName,
                                ManualURL = ConfigurationManager.AppSettings["ManualURL"].ToString() + x.ManualName
                            }).ToList(),
                            UnitPictures = db.ClientUnitPictures.Where(x => x.ClientUnitId == cUnit.Id && x.SplitType == splitTp).AsEnumerable().Select(x => new
                            {
                                UnitImageUrl = (x.UnitImage != null ? ConfigurationManager.AppSettings["UnitImageURL"].ToString() + x.UnitImage : ""),

                            }).ToList(),
                            UnitAge = (cUnitPart.ManufactureDate == null ? false : (DateTime.UtcNow.Year - cUnitPart.ManufactureDate.Value.Year) >= 10 ? true : false),
                        };
                        objUnitParts.Add(objUnitPart);

                    }
                    //var ClientUnitPictures = db.ClientUnitPictures.Where(x => x.ClientUnitId == cUnit.Id).AsEnumerable().Select(x => new
                    //{
                    //    UnitImageUrl = (x.UnitImage != null ? ConfigurationManager.AppSettings["UnitImageURL"].ToString() + x.UnitImage : "")
                    //}).ToList();

                    //var ManualURLs = cUnit.ClientUnitManuals.AsEnumerable().Select(x => new
                    //{
                    //    x.ManualName,
                    //    ManualURL = ConfigurationManager.AppSettings["ManualURL"].ToString() + x.ManualUrl
                    //});
                    List<object> ServiceHistory = new List<object>();
                    var report = cUnit.ServiceReportUnits.AsEnumerable();
                    foreach (var item in report)
                    {
                        if (item.ServiceReport != null)
                        {
                            var d = new
                            {
                                ServiceReportNumber = "Service Report No - " + item.ServiceReport.ServiceReportNumber
                            };
                            ServiceHistory.Add(d);
                        }
                    }

                    var UnitDetail = new
                    {
                        cUnit.Client.FirstName,
                        cUnit.Client.LastName,
                        cUnit.UnitName,
                        Address = UnitAddress,
                        Name = db.SubscriptionPlans.FirstOrDefault(p => p.Id == cUnit.PlanTypeId).PlanName,
                        UnitParts = objUnitParts,
                        Notes = (cUnit.Notes == null ? "" : cUnit.Notes),
                        ServiceHistory = ServiceHistory
                    };
                    res.Data = UnitDetail;
                    res.Message = "Data Found";
                    res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                }
                else
                {
                    res.Data = null;
                    res.Message = "No Data Found";
                    res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
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
        [Route("SubmitServiceReport")]
        public async Task<IHttpActionResult> SubmitServiceReport([FromBody] ServiceReporRequestModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            try
            {
                var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId).FirstOrDefault();
                if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    var service = db.Services.Find(request.ServiceId);
                    if (service != null)
                    {
                        var notification = db.UserNotifications.AsEnumerable().Where(x => x.CommonId == request.ServiceId && x.MessageType == api.App_Start.Utilities.NotificationType.ServiceScheduled.GetEnumDescription()).ToList();
                        if (notification.Count > 0)
                        {
                            db.UserNotifications.RemoveRange(notification);
                            db.SaveChanges();
                        }
                        var notification1 = db.UserNotifications.AsEnumerable().Where(x => x.CommonId == request.ServiceId && x.MessageType == api.App_Start.Utilities.NotificationType.PeriodicServiceReminder.GetEnumDescription()).ToList();
                        if (notification1.Count > 0)
                        {
                            db.UserNotifications.RemoveRange(notification1);
                            db.SaveChanges();
                        }
                        var client = service.Client;
                        var cnt = service.ClientAddress.Services.Select(x => x.ServiceReports.Count).FirstOrDefault() + 1;
                        var ReportNumber = service.ServiceCaseNumber + "-R" + cnt.ToString();
                        ServiceReport rpt = new ServiceReport();
                        rpt.BillingType = api.App_Start.Utilities.ReportBilling.FixedCostContract.GetEnumDescription();
                        rpt.ServiceReportNumber = ReportNumber;
                        rpt.ServiceId = request.ServiceId;
                        rpt.CCEmail = request.CCEmail;
                        try
                        {
                            rpt.WorkStartedTime = DateTime.ParseExact(request.ScheduleStartTime, "HH:mm", CultureInfo.InvariantCulture).ToString("hh:mm tt");
                        }
                        catch (Exception)
                        {
                            rpt.WorkStartedTime = DateTime.ParseExact(request.ScheduleStartTime, "hh:mm tt", CultureInfo.InvariantCulture).ToString("hh:mm tt");
                        }
                        try
                        {
                            rpt.WorkCompletedTime = DateTime.ParseExact(request.ScheduleEndTime, "HH:mm", CultureInfo.InvariantCulture).ToString("hh:mm tt");
                        }
                        catch (Exception)
                        {
                            rpt.WorkCompletedTime = DateTime.ParseExact(request.ScheduleEndTime, "hh:mm tt", CultureInfo.InvariantCulture).ToString("hh:mm tt");
                        }
                        rpt.IsWorkDone = !request.IsWorkNotDone;
                        rpt.WorkPerformed = request.WorkPerformed;
                        rpt.Recommendationsforcustomer = request.Recommendationsforcustomer;
                        rpt.EmployeeNotes = request.EmployeeNotes;
                        rpt.AddedBy = request.EmployeeId;
                        rpt.AddedDate = DateTime.UtcNow;
                        rpt.StartLatitude = request.StartLatitude;
                        rpt.StartLongitude = request.StartLongitude;
                        rpt.EndLatitude = request.EndLatitude;
                        rpt.EndLongitude = request.EndLongitude;

                        rpt.AddedByType = api.App_Start.Utilities.UserRoles.Employee.GetEnumValue();
                        if (request.Units.Count > 0)
                        {
                            foreach (var item in request.Units)
                            {
                                rpt.ServiceReportUnits.Add(new ServiceReportUnit()
                                {
                                    ServiceId = request.ServiceId,
                                    UnitId = item.Id,
                                    IsCompleted = item.IsCompleted,
                                });
                                var cunit = db.ClientUnits.Find(item.Id);
                                if (item.IsCompleted)
                                {
                                    cunit.Status = api.App_Start.Utilities.UnitStatus.Serviced.GetEnumValue();
                                    db.SaveChanges();

                                    if (service.RequestedServiceBridges.Count > 0)
                                    {
                                        db.uspa_ClientUnitServiceCount_Update(cunit.ClientId, cunit.Id, 0, 0, 0, 1, 0, 0, request.EmployeeId, api.App_Start.Utilities.UserRoles.Employee.GetEnumValue(), DateTime.UtcNow);
                                    }
                                    else
                                    {
                                        db.uspa_ClientUnitServiceCount_Update(cunit.ClientId, cunit.Id, 0, 1, 0, 0, 0, 0, request.EmployeeId, api.App_Start.Utilities.UserRoles.Employee.GetEnumValue(), DateTime.UtcNow);
                                    }
                                }
                                else
                                {
                                    cunit.Status = api.App_Start.Utilities.UnitStatus.NeedRepair.GetEnumValue();
                                    db.SaveChanges();
                                }
                                if (item.ServiceUnitParts.Count > 0)
                                {
                                    foreach (var parts in item.ServiceUnitParts)
                                    {
                                        var pt = service.ServicePartLists.Where(x => x.ServiceId == request.ServiceId && x.PartId == parts.PartId && x.UnitId == item.Id).FirstOrDefault();
                                        if (pt != null)
                                        {
                                            //pt.PartQuantity = pt.PartQuantity.Value - parts.Qty;
                                            pt.IsUsed = true;
                                            pt.UsedQuantity = parts.Qty;
                                            pt.Part.InStockQuantity = pt.Part.InStockQuantity - parts.Qty;
                                            pt.Part.ReservedQuantity = pt.Part.ReservedQuantity - parts.Qty;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (request.IsWorkNotDone)
                            {
                                service.IsNoShow = true;
                                foreach (var item in service.ServiceUnits)
                                {
                                    rpt.ServiceReportUnits.Add(new ServiceReportUnit()
                                    {
                                        ServiceId = request.ServiceId,
                                        UnitId = item.UnitId,
                                        IsCompleted = false,
                                    });
                                    var cunit = db.ClientUnits.Find(item.UnitId);
                                    cunit.Status = api.App_Start.Utilities.UnitStatus.NeedRepair.GetEnumValue();
                                    db.SaveChanges();
                                }
                                //if (service.ServicePartLists.Count > 0)
                                //{
                                //    foreach (var parts in service.ServicePartLists)
                                //    {
                                //        parts.IsUsed = true;
                                //        parts.UsedQuantity = 0;
                                //        parts.Part.ReservedQuantity = parts.Part.ReservedQuantity - parts.PartQuantity;
                                //    }
                                //}
                            }
                            else
                            {
                                service.IsNoShow = false;
                            }
                        }
                        if (request.RequestedParts == null)
                        {
                            request.RequestedParts = new List<RequestedParts>();
                        }
                        if (request.RequestedParts.Count > 0)
                        {
                            EmployeePartRequestMaster eprm = new EmployeePartRequestMaster();
                            eprm.EmployeeId = request.EmployeeId;
                            eprm.ClientId = client.Id;
                            eprm.ClientAddressId = service.AddressID;
                            eprm.Status = "";
                            eprm.AddedBy = request.EmployeeId;
                            eprm.AddedByType = api.App_Start.Utilities.UserRoles.Employee.GetEnumValue();
                            eprm.AddedDate = DateTime.UtcNow;
                            eprm.EmpNotes = request.RequestedParts[0].EmpNotes;
                            foreach (var rp in request.RequestedParts)
                            {
                                eprm.EmployeePartRequests.Add(new EmployeePartRequest()
                                {
                                    UnitId = rp.UnitId,
                                    RequestedQuantity = rp.Quantity,
                                    ArrangedQuantity = rp.Quantity,
                                    PartId = rp.PartId,
                                    PartName = rp.PartName,
                                    PartSize = rp.PartSize,
                                    Description = rp.Description
                                });
                            }
                            eprm.Status = api.App_Start.Utilities.EmpPartRequestStatus.NeedToOrder.GetEnumDescription();
                            rpt.EmployeePartRequestMasters.Add(eprm);
                        }
                        if (request.EmailToClient.Count > 0)
                        {

                        }
                        if (service.IsNoShow == true)
                        {
                            service.Status = api.App_Start.Utilities.ServiceTypes.NoShow.GetEnumDescription();
                        }
                        else
                        {
                            service.Status = api.App_Start.Utilities.ServiceTypes.Completed.GetEnumDescription();
                        }
                        service.StatusChangeDate = DateTime.UtcNow;
                        service.UpdatedBy = EmpInfo.Id;
                        service.UpdatedByType = api.App_Start.Utilities.UserRoles.Employee.GetEnumValue();
                        service.UpdatedDate = DateTime.UtcNow;
                        db.ServiceReports.Add(rpt);
                        db.SaveChanges();

                        if (rpt.ServiceReportUnits.Count(x => x.IsCompleted == false) > 0 && rpt.Service.Status == api.App_Start.Utilities.ServiceTypes.Completed.GetEnumDescription())
                        {
                            var result = db.uspa_ScheduleServiceForUnDoneUnit(request.ServiceId, EmpInfo.Id, api.App_Start.Utilities.UserRoles.Employee.GetEnumValue(), DateTime.UtcNow).ToList();
                        }

                        if (request.RequestedParts.Count > 0)
                        {
                            var eprm1 = rpt.EmployeePartRequestMasters.FirstOrDefault();
                            var ret = db.uspa_CheckInStockAndScheduleService(eprm1.Id, eprm1.AddedBy, eprm1.AddedByType, eprm1.AddedDate).ToList();
                            if (ret.FirstOrDefault().ServiceId > 0)
                            {
                                var EmpNotification = db.NotificationMasters.Where(x => x.Name == "EmployeeSchedule").FirstOrDefault();
                                var message = EmpNotification.Message; //var message = "System has scheduled a service for you on " + ret.FirstOrDefault().ScheduleDate.ToString("MMMM dd, yyyy") + ".";
                                message = message.Replace("{{ScheduleDate}}", ret.FirstOrDefault().ScheduleDate.ToString("MMMM dd, yyyy"));
                                UserNotification objUserNotification = new UserNotification();
                                objUserNotification.UserId = eprm1.AddedBy;
                                objUserNotification.UserTypeId = api.App_Start.Utilities.UserRoles.Employee.GetEnumValue();
                                objUserNotification.Message = message;
                                objUserNotification.Status = api.App_Start.Utilities.NotificationStatus.UnRead.GetEnumDescription();
                                objUserNotification.CommonId = ret.FirstOrDefault().ServiceId;
                                objUserNotification.MessageType = api.App_Start.Utilities.NotificationType.ServiceScheduled.GetEnumDescription();
                                objUserNotification.AddedDate = DateTime.UtcNow;
                                db.UserNotifications.Add(objUserNotification);
                                db.SaveChanges();

                                var BadgeCount = db.UserNotifications.AsEnumerable().Where(x => x.UserId == eprm1.AddedBy && x.UserTypeId == api.App_Start.Utilities.UserRoles.Employee.GetEnumValue() && x.Status == api.App_Start.Utilities.NotificationStatus.UnRead.GetEnumDescription()).ToList().Count;

                                Notifications objNotifications = new Notifications { NId = objUserNotification.Id, NType = api.App_Start.Utilities.NotificationType.ServiceScheduled.GetEnumValue(), CommonId = objUserNotification.CommonId.Value };
                                List<NotificationModel> notify = new List<NotificationModel>();
                                notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                                notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                                notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });
                                if (EmpInfo.DeviceType != null && EmpInfo.DeviceToken != null)
                                {
                                    if (EmpInfo.DeviceType.ToLower() == "android")
                                    {
                                        //string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType + "&data.CommonId=" + objNotifications.CommonId;
                                        //SendNotifications.SendAndroidNotification(UserInfo.DeviceToken, message, CustomData, "employee");
                                    }
                                    else if (EmpInfo.DeviceType.ToLower() == "iphone")
                                    {
                                        SendNotifications.SendIphoneNotification(BadgeCount, EmpInfo.DeviceToken, message, notify, "employee", HttpContext.Current);
                                    }
                                }
                            }
                            if (ret.FirstOrDefault().ServiceId > 0)
                            {
                                var service1 = db.Services.Find(ret.FirstOrDefault().ServiceId);
                                var ClientNotification = db.NotificationMasters.Where(x => x.Name == "PartRequestServiceScheduleSendToClient").FirstOrDefault();
                                var message = ClientNotification.Message; //var message = "Service " + service1.ServiceCaseNumber + " for your requested parts has been scheduled on " + service1.ScheduleDate.Value.ToString("MMMM dd, yyyy") + ".";
                                //message = message.Replace("{{ServiceCaseNumber}}", service1.ServiceCaseNumber);
                                message = message.Replace("{{Address}}", service1.ClientAddress.Address);
                                message = message.Replace("{{ScheduleDate}}", service1.ScheduleDate.Value.ToString("MMMM dd, yyyy"));

                                UserNotification objUserNotification = new UserNotification();
                                objUserNotification.UserId = service1.ClientId;
                                objUserNotification.UserTypeId = api.App_Start.Utilities.UserRoles.Client.GetEnumValue();
                                objUserNotification.Message = message;
                                objUserNotification.Status = api.App_Start.Utilities.NotificationStatus.UnRead.GetEnumDescription();
                                objUserNotification.CommonId = service1.Id;
                                objUserNotification.MessageType = api.App_Start.Utilities.NotificationType.PeriodicServiceReminder.GetEnumDescription();
                                objUserNotification.AddedDate = DateTime.UtcNow;
                                db.UserNotifications.Add(objUserNotification);
                                db.SaveChanges();

                                //var BadgeCount = db.UserNotifications.AsEnumerable().Where(x => x.UserId == service.ClientId && x.UserTypeId == api.App_Start.Utilities.UserRoles.Client.GetEnumValue()).ToList().Count;
                                var BadgeCount = db.uspa_ClientPortal_GetNotificationForDashBoardByUserType(service1.ClientId, api.App_Start.Utilities.UserRoles.Client.GetEnumValue(), "", 0).AsEnumerable().Where(x => x.Status == api.App_Start.Utilities.NotificationStatus.UnRead.GetEnumDescription()).Count();
                                Notifications objNotifications = new Notifications { NId = objUserNotification.Id, NType = api.App_Start.Utilities.NotificationType.PeriodicServiceReminder.GetEnumValue(), CommonId = service1.Id };
                                List<NotificationModel> notify = new List<NotificationModel>();
                                notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                                notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                                notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });
                                var UserInfo = db.Clients.Where(x => x.Id == service1.ClientId).FirstOrDefault();
                                if (UserInfo.DeviceType != null && UserInfo.DeviceToken != null)
                                {
                                    if (UserInfo.DeviceType.ToLower() == "android")
                                    {
                                        string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType + "&data.CommonId=" + objNotifications.CommonId;
                                        SendNotifications.SendAndroidNotification(UserInfo.DeviceToken, message, CustomData, "client");
                                    }
                                    else if (UserInfo.DeviceType.ToLower() == "iphone")
                                    {
                                        SendNotifications.SendIphoneNotification(BadgeCount, UserInfo.DeviceToken, message, notify, "client", HttpContext.Current);
                                    }
                                }
                            }
                        }

                        var notifications = db.UserNotifications.AsEnumerable().Where(x => x.CommonId == service.Id && x.MessageType == api.App_Start.Utilities.NotificationType.ServiceScheduled.GetEnumDescription()).FirstOrDefault();
                        if (notifications != null)
                        {
                            db.UserNotifications.Remove(notifications);
                            db.SaveChanges();
                        }

                        res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                        res.Data = rpt.Id;
                        res.Message = "Report Created";


                    }
                    else
                    {
                        res.StatusCode = HttpStatusCode.BadRequest.GetEnumValue();
                        res.Data = null;
                        res.Message = "Invalid Request";
                    }
                }

            }
            catch (Exception ex)
            {
                res.StatusCode = HttpStatusCode.InternalServerError.GetEnumValue();
                res.Data = null;
                res.Message = "Internal Server Error";

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
        [Route("SubmitServiceReportImage")]
        public Task<IHttpActionResult> SubmitServiceReportImage()
        {
            ResponseModel res = new ResponseModel();
            if (updatetoken)
            {
                res.Token = accessToken;
            }
            else
            {
                res.Token = "";
            }

            var uploadPath = new DirectoryInfo(HttpContext.Current.Server.MapPath("../../")).Parent.FullName + ConfigurationManager.AppSettings["ReportImagePath"].ToString();//HttpContext.Current.Server.MapPath("~/Uploads");

            var multipartFormDataStreamProvider = new UploadMultipartFormProvider(uploadPath);
            var task = Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProvider).
            ContinueWith<IHttpActionResult>(t =>
            {
                db = new Aircall_DBEntities1();
                ServiceReporRequestModel emp = new ServiceReporRequestModel();
                if (t.IsFaulted || t.IsCanceled)
                {
                    res.StatusCode = HttpStatusCode.InternalServerError.GetEnumValue();
                    res.Data = null;
                    res.Message = "Internal Server Error";
                    db.Dispose();
                    return Ok(res);
                }
                var data = multipartFormDataStreamProvider.FormData;
                var filedata = multipartFormDataStreamProvider.FileData;

                try
                {
                    var IdValue = multipartFormDataStreamProvider.FormData.GetValues("Id").FirstOrDefault();
                    if (IdValue != null)
                    {
                        int id = int.Parse(IdValue);
                        var rpt = db.ServiceReports.Find(id);
                        foreach (var file in filedata)
                        {
                            if (file.Headers.ContentDisposition.Name.ToLower().Contains("clientsignature"))
                            {
                                var signaturefile = System.Web.Hosting.HostingEnvironment.MapPath("/") + ConfigurationManager.AppSettings["ClientSignaturePath"].ToString() + new FileInfo(file.LocalFileName).Name;
                                File.Move(file.LocalFileName, signaturefile);
                                rpt.ClientSignature = new FileInfo(signaturefile).Name;
                                string extension = System.IO.Path.GetExtension(signaturefile);
                                string extensionwithoutdot = extension.Remove(0, 1);

                                Instructions rsiphnWxH = new Instructions();
                                rsiphnWxH.Width = 175;
                                rsiphnWxH.Mode = FitMode.Stretch;
                                rsiphnWxH.Format = extensionwithoutdot;

                                ImageJob imjob = new ImageJob(signaturefile, System.Web.Hosting.HostingEnvironment.MapPath(ConfigurationManager.AppSettings["ClientEmailSignaturePath"] + new FileInfo(signaturefile).Name), rsiphnWxH);
                                imjob.CreateParentDirectory = true;
                                imjob.Build();
                            }
                            else
                            {
                                string extension = System.IO.Path.GetExtension(file.LocalFileName);
                                string extensionwithoutdot = extension.Remove(0, 1);

                                Instructions rsiphnWxH = new Instructions();
                                rsiphnWxH.Width = 175;
                                rsiphnWxH.Mode = FitMode.Stretch;
                                rsiphnWxH.Format = extensionwithoutdot;

                                ImageJob imjob = new ImageJob(file.LocalFileName, System.Web.Hosting.HostingEnvironment.MapPath(ConfigurationManager.AppSettings["ReportEmailImagePath"] + new FileInfo(file.LocalFileName).Name), rsiphnWxH);
                                imjob.CreateParentDirectory = true;
                                imjob.Build();

                                rpt.ServiceReportImages.Add(new ServiceReportImage() { ServiceImage = new FileInfo(file.LocalFileName).Name });
                            }
                        }
                        db.SaveChanges();
                        var UserInfo = db.Clients.Find(rpt.Service.ClientId);
                        var service = rpt.Service;
                        try
                        {
                            //if (service.IsNoShow == false)
                            {
                                var Report = rpt;

                                var ClientAddress = Report.ServiceReportUnits.FirstOrDefault().ClientUnit.ClientAddress;
                                EmailTemplate templateclient = db.EmailTemplates.Where(x => x.Name == "ServiceReportClient" && x.Status == true).FirstOrDefault();
                                var strclient = templateclient.EmailBody;
                                var sub = templateclient.EmailTemplateSubject.Replace("{{Address}}", Report.Service.ClientAddress.Address + (service.IsNoShow == true ? "(No Show Service)" : ""));
                                sub = sub.Replace("{{ServiceCaseNumber}}", service.ServiceCaseNumber);
                                strclient = strclient.Replace("{{ServiceReport}}", Report.ServiceReportNumber + (service.IsNoShow == true ? "(No Show Service)" : ""));
                                strclient = strclient.Replace("{{ContactName}}", UserInfo.FirstName + " " + UserInfo.LastName);
                                strclient = strclient.Replace("{{Company}}", UserInfo.Company ?? "-");
                                strclient = strclient.Replace("{{Address}}", ClientAddress.Address + ",<br/>" + ClientAddress.City1.Name + ", " + ClientAddress.State1.Name + ",<br/>" + ClientAddress.ZipCode);
                                strclient = strclient.Replace("{{PurposeOfVisit}}", (Report.Service.PurposeOfVisit));
                                strclient = strclient.Replace("{{Date}}", Report.Service.ScheduleDate.Value.ToString("MM/dd/yyyy"));
                                strclient = strclient.Replace("{{Technician}}", Report.Service.Employee.FirstName + " " + Report.Service.Employee.LastName);
                                //strclient = strclient.Replace("{{ActualStartTime}}", Report.WorkStartedTime);
                                //strclient = strclient.Replace("{{ActualEndTime}}", Report.WorkCompletedTime);
                                strclient = strclient.Replace("{{WorkPerformed}}", Report.WorkPerformed);
                                strclient = strclient.Replace("{{RecommendationsForCustomer}}", (string.IsNullOrWhiteSpace(Report.Recommendationsforcustomer) ? "NA" : Report.Recommendationsforcustomer));
                                strclient = strclient.Replace("{{EmployeeNotes}}", (string.IsNullOrWhiteSpace(Report.EmployeeNotes) ? "NA" : Report.EmployeeNotes));

                                var strUnits = "";
                                if (service.IsNoShow == false)
                                {

                                    foreach (var item in Report.ServiceReportUnits.Where(x => x.IsCompleted == true).ToList())
                                    {
                                        strUnits = strUnits + "<tr><td>" + item.ClientUnit.UnitName + " " + db.SubscriptionPlans.FirstOrDefault(p => p.Id == item.ClientUnit.PlanTypeId).PlanName + "</td></tr>";
                                    }
                                }
                                else
                                {
                                    foreach (var item in service.ServiceUnits.ToList())
                                    {
                                        strUnits = strUnits + "<tr><td>" + item.ClientUnit.UnitName + " " + db.SubscriptionPlans.FirstOrDefault(p => p.Id == item.ClientUnit.PlanTypeId).PlanName + "</td></tr>";
                                    }
                                }
                                if (!string.IsNullOrWhiteSpace(strUnits))
                                {
                                    strUnits = @"<table style='width: 330px;border-collapse: collapse;border: gray 1px solid;'>" + strUnits + "</table>";
                                }
                                else
                                {
                                    strUnits = "NA";
                                }
                                strclient = strclient.Replace("{{UnitServiced}}", strUnits);
                                var strPartsUsed = "";

                                if (service.IsNoShow == false)
                                {
                                    var prts = Report.Service.ServicePartLists.Where(x => x.IsUsed == true).ToList();
                                    for (int i = 0; i < prts.Count; i++)
                                    {
                                        var item = prts[i];
                                        strPartsUsed = strPartsUsed + "<tr><td style='border: gray 1px solid;'>" + (i + 1).ToString() + "</td><td style='border: gray 1px solid;'>" + item.Part.Name + " - " + (item.Part.Size ?? "") + "</td><td style='border: gray 1px solid;'>" + item.UsedQuantity + "</td></tr>";
                                    }
                                }
                                if (strPartsUsed != "")
                                {
                                    strPartsUsed = @"<table style='width: 330px;border-collapse: collapse;border: gray 1px solid;'>
                                                    <tr><td style='border: gray 1px solid;'>SR</td><td style='border: gray 1px solid;'>Part</td><td style='border: gray 1px solid;'>Qty</td></tr>" + strPartsUsed + "</table>";
                                }
                                else
                                {
                                    strPartsUsed = "NA";
                                }
                                strclient = strclient.Replace("{{MaterialUsed}}", strPartsUsed);

                                var strPartRequested = "";

                                var PartRequest = Report.EmployeePartRequestMasters.Select(x => new { Parts = x.EmployeePartRequests.Select(y => new { y.Part, y.RequestedQuantity }) }).ToList();

                                int k = 0;
                                for (int i = 0; i < PartRequest.Count; i++)
                                {
                                    var item = PartRequest[i];
                                    for (int j = 0; j < item.Parts.Count(); j++)
                                    {
                                        var itm = item.Parts.ToList()[j];
                                        strPartRequested = strPartRequested + "<tr><td>" + (k + 1).ToString() + "</td><td>" + itm.Part.Name + " - " + (itm.Part.Size ?? "") + "</td><td>" + itm.RequestedQuantity + "</td></tr>";
                                    }
                                }
                                if (strPartRequested != "")
                                {
                                    strPartRequested = @"<table style='width: 330px;border-collapse: collapse;border: gray 1px solid;'>
                                                    <tr><td style='border: gray 1px solid;'>SR</td><td style='border: gray 1px solid;'>Part</td><td style='border: gray 1px solid;'>Qty</td></tr>" + strPartRequested + "</table>";
                                }
                                else
                                {
                                    strPartRequested = "NA";
                                }

                                strclient = strclient.Replace("{{PartRequested}}", strPartRequested);

                                var strImage = "";
                                if (rpt.ServiceReportImages.Count > 0)
                                {
                                    strImage += "<tr>";
                                    int j = 1;
                                    foreach (var item in rpt.ServiceReportImages)
                                    {
                                        strImage = strImage + "<td style='width:175px;'><img style='width:175px' src='" + ConfigurationManager.AppSettings["ReportEmailImageURL"] + item.ServiceImage + "' /></td>";
                                        if ((j % 2) == 0)
                                        {
                                            strImage += "</tr>";
                                            if (rpt.ServiceReportImages.Count > j)
                                            {
                                                strImage += "<tr>";
                                            }
                                        }
                                        j++;
                                    }
                                    if (!strImage.EndsWith("</tr>"))
                                        strImage += "</tr>";

                                    strImage = "<table style='width: 350px;'>" + strImage + "</table>";
                                }
                                else
                                {
                                    strImage = "NA";
                                }
                                strclient = strclient.Replace("{{Images}}", strImage);
                                string strClientSignature = "";
                                if (!string.IsNullOrWhiteSpace(rpt.ClientSignature))
                                {
                                    strClientSignature = "<img style='width:175px' src='" + ConfigurationManager.AppSettings["ClientEmailSignatureURL"] + rpt.ClientSignature + "' />";
                                }
                                strclient = strclient.Replace("{{ClientSignature}}", strClientSignature);

                                strclient = @"<html>
                                                <head>
                                                    <title></title>    
                                                </head>
                                                <body style='width: 540px;'>" + strclient + @"</body></html>";
                                //var pdf = PdfGenerator.GeneratePdf(strclient, new PdfGenerateConfig() { MarginBottom = 0, MarginLeft = 0, MarginTop = 0, MarginRight = 0, PageSize = PdfSharp.PageSize.A4, PageOrientation = PageOrientation.Portrait });

                                //string fname = (ConfigurationManager.AppSettings["reportsFilePathFull"].ToString()) +
                                //    Report.ServiceReportNumber + "-" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".pdf";
                                //pdf.Save(fname);

                                try
                                {
                                    //api.App_Start.Utilities.Send(sub, UserInfo.Email, strclient, templateclient.FromEmail, db, fname, Report.CCEmail);
                                    BackgroundTaskManager.Run(async () =>
                                    {
                                        await api.App_Start.Utilities.SendAsync(sub, UserInfo.Email, strclient, templateclient.FromEmail, db, PrintOrdersToPdf(Report), Report.CCEmail);
                                    });
                                }
                                catch (Exception ex)
                                {
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                        }

                        if (service.IsNoShow == false)
                        {
                            var ClientNotification = db.NotificationMasters.Where(x => x.Name == "ServiceCompletedSendToClient").FirstOrDefault();
                            var message = ClientNotification.Message; //var message = "Employee " + service.Employee.FirstName + " " + service.Employee.LastName + " has completed your service.";
                            message = message.Replace("{{EmployeeName}}", service.Employee.FirstName + " " + service.Employee.LastName);
                            UserNotification objUserNotification = new UserNotification();
                            objUserNotification.UserId = service.ClientId;
                            objUserNotification.UserTypeId = api.App_Start.Utilities.UserRoles.Client.GetEnumValue();
                            objUserNotification.Message = message;
                            objUserNotification.Status = api.App_Start.Utilities.NotificationStatus.UnRead.GetEnumDescription();
                            objUserNotification.CommonId = service.Id;
                            objUserNotification.MessageType = api.App_Start.Utilities.NotificationType.RateService.GetEnumDescription();
                            objUserNotification.AddedDate = DateTime.UtcNow;
                            db.UserNotifications.Add(objUserNotification);
                            db.SaveChanges();

                            //var BadgeCount = db.UserNotifications.AsEnumerable().Where(x => x.UserId == service.ClientId && x.UserTypeId == api.App_Start.Utilities.UserRoles.Client.GetEnumValue()).ToList().Count;
                            var BadgeCount = db.uspa_ClientPortal_GetNotificationForDashBoardByUserType(service.ClientId, api.App_Start.Utilities.UserRoles.Client.GetEnumValue(), "", 0).AsEnumerable().Where(x => x.Status == api.App_Start.Utilities.NotificationStatus.UnRead.GetEnumDescription()).Count();
                            Notifications objNotifications = new Notifications { NId = objUserNotification.Id, NType = api.App_Start.Utilities.NotificationType.RateService.GetEnumValue(), CommonId = service.Id };
                            List<NotificationModel> notify = new List<NotificationModel>();
                            notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                            notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                            notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });
                            if (UserInfo.DeviceType != null && UserInfo.DeviceToken != null)
                            {
                                if (UserInfo.DeviceType.ToLower() == "android")
                                {
                                    string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType + "&data.CommonId=" + objNotifications.CommonId;
                                    SendNotifications.SendAndroidNotification(UserInfo.DeviceToken, message, CustomData, "client");
                                }
                                else if (UserInfo.DeviceType.ToLower() == "iphone")
                                {
                                    SendNotifications.SendIphoneNotification(BadgeCount, UserInfo.DeviceToken, message, notify, "client", HttpContext.Current);
                                }
                            }

                            //Send Email To admin for not done units
                            var serviceUnitCount = service.ServiceUnits.Count();
                            var CompletedServiceUnitCount = rpt.ServiceReportUnits.Where(x => x.IsCompleted == true).Count();

                            if (CompletedServiceUnitCount < serviceUnitCount)
                            {

                                var NotCompletedReport = rpt;
                                var ClientAddr = NotCompletedReport.ServiceReportUnits.FirstOrDefault().ClientUnit.ClientAddress;

                                EmailTemplate templateNotCompleted = db.EmailTemplates.Where(x => x.Name == "ServiceReportNotCompletedUnits" && x.Status == true).FirstOrDefault();
                                var strclient = templateNotCompleted.EmailBody;
                                //var sub = templateNotCompleted.EmailTemplateSubject + " - " + service.ServiceCaseNumber;
                                var sub = templateNotCompleted.EmailTemplateSubject.Replace("{{Address}}", service.ClientAddress.Address);
                                sub = sub.Replace("{{ServiceCaseNumber}}", service.ServiceCaseNumber);
                                strclient = strclient.Replace("{{ServiceCase}}", service.ServiceCaseNumber);
                                strclient = strclient.Replace("{{ServiceReport}}", NotCompletedReport.ServiceReportNumber);
                                strclient = strclient.Replace("{{ContactName}}", UserInfo.FirstName + " " + UserInfo.LastName);
                                strclient = strclient.Replace("{{Company}}", UserInfo.Company ?? "-");
                                strclient = strclient.Replace("{{Address}}", ClientAddr.Address + ",<br/>" + ClientAddr.City1.Name + ", " + ClientAddr.State1.Name + ",<br/>" + ClientAddr.ZipCode);
                                strclient = strclient.Replace("{{PurposeOfVisit}}", (NotCompletedReport.Service.PurposeOfVisit));
                                strclient = strclient.Replace("{{Date}}", NotCompletedReport.Service.ScheduleDate.Value.ToString("MM/dd/yyyy"));
                                strclient = strclient.Replace("{{Technician}}", NotCompletedReport.Service.Employee.FirstName + " " + NotCompletedReport.Service.Employee.LastName);
                                strclient = strclient.Replace("{{EmployeeNotes}}", (string.IsNullOrWhiteSpace(NotCompletedReport.EmployeeNotes) ? "NA" : NotCompletedReport.EmployeeNotes));

                                var strNotCompletedUnits = "";

                                foreach (var item in NotCompletedReport.ServiceReportUnits.Where(x => x.IsCompleted == false).ToList())
                                {
                                    strNotCompletedUnits = strNotCompletedUnits + "<tr><td>" + item.ClientUnit.UnitName + " " + db.SubscriptionPlans.FirstOrDefault(p => p.Id == item.ClientUnit.PlanTypeId).PlanName + "</td></tr>";
                                }
                                if (!string.IsNullOrWhiteSpace(strNotCompletedUnits))
                                {
                                    strNotCompletedUnits = @"<table style='width:100%;font-family:Arial,sans-serif;font-size:14px;line-height:21px;color:#333;
                                            border-color: #e2e2e2;border-collapse: collapse;'>" + strNotCompletedUnits + "</table>";
                                }
                                else
                                {
                                    strNotCompletedUnits = "NA";
                                }
                                strclient = strclient.Replace("{{UnitNotServiced}}", strNotCompletedUnits);

                                api.App_Start.Utilities.Send(sub, api.App_Start.Utilities.GetSiteSettingValue("AdminEmail", db), strclient, templateNotCompleted.FromEmail, db, "", "");
                            }
                        }

                        res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                        res.Data = null;
                        res.Message = "Report submitted successfully";
                    }
                    else
                    {
                        res.StatusCode = HttpStatusCode.BadRequest.GetEnumValue();
                        res.Data = null;
                        res.Message = "Invalid Request";
                    }
                }
                catch (Exception ex)
                {
                    res.StatusCode = HttpStatusCode.InternalServerError.GetEnumValue();
                    res.Data = null;
                    res.Message = "Internal Server Error";
                }
                db.Dispose();
                return Ok(res);
            });
            return task;
        }


        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("UpdateEmployeeLocation")]
        public async Task<IHttpActionResult> UpdateEmployeeLocation([FromBody] EmpCommonModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            try
            {
                var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId).FirstOrDefault();
                if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    EmployeeLocation loc = new EmployeeLocation();
                    loc.EmployeeId = request.EmployeeId;
                    loc.LastUpdatedOn = DateTime.UtcNow;
                    loc.Latitude = request.Latitude;
                    loc.Longitude = request.Longitude;
                    db.EmployeeLocations.Add(loc);
                    db.SaveChanges();
                    res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                    res.Data = null;
                    res.Message = "Location Updated";
                }
            }
            catch (Exception ex)
            {
                res.StatusCode = HttpStatusCode.InternalServerError.GetEnumValue();
                res.Data = null;
                res.Message = "Internal Server Error";
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
        [ResponseType(typeof(ResponseListModel))]
        [HttpPost]
        [Route("GetAllCompletedReports")]
        public async Task<IHttpActionResult> GetAllCompletedReports([FromBody] EmpCommonModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseListModel res = new ResponseListModel();
            List<object> data = new List<object>();
            int totalRecord = 0;
            int pageCnt = 0;
            int totalPageCount = 0;
            List<CompletedServiceReportModel> EmpServices = new List<CompletedServiceReportModel>();
            List<CompletedServiceReportModel> result = new List<CompletedServiceReportModel>();
            List<CompletedServiceReportModel> sv = new List<CompletedServiceReportModel>();
            var pageSize = int.Parse(api.App_Start.Utilities.GetSiteSettingValue("ApplicationPageSize", db));

            var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId).FirstOrDefault();
            if (EmpInfo != null)
            {
                if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    var dtStart = DateTime.UtcNow;
                    var dtEnd = dtStart.AddDays(-7);
                    sv = (from sr in db.ServiceReports
                          join s in db.Services on sr.ServiceId equals s.Id
                          join c in db.Clients on s.ClientId equals c.Id
                          where s.EmployeeId == request.EmployeeId
                          select new CompletedServiceReportModel()
                          {
                              ReportId = sr.Id,
                              ServiceReportNumber = sr.ServiceReportNumber,
                              ClientName = c.FirstName + " " + c.LastName,
                              PurposeOfVisit = s.PurposeOfVisit,
                              PhoneNumber = c.PhoneNumber,
                              MobileNumber = c.MobileNumber,
                              OfficeNumber = c.OfficeNumber,
                              ReportDateTime = sr.AddedDate,
                              WorkCompletedTime = sr.WorkCompletedTime,
                              Status = s.Status,
                              StatusChangeDate = s.StatusChangeDate
                          }).AsEnumerable()
                          .Select(c => new CompletedServiceReportModel()
                          {
                              ReportId = c.ReportId,
                              ServiceReportNumber = c.ServiceReportNumber,
                              ClientName = c.ClientName,
                              PurposeOfVisit = c.PurposeOfVisit,
                              PhoneNumber = (string.IsNullOrWhiteSpace(c.PhoneNumber) ? "" : c.PhoneNumber),
                              MobileNumber = (string.IsNullOrWhiteSpace(c.MobileNumber) ? "" : c.MobileNumber),
                              OfficeNumber = (string.IsNullOrWhiteSpace(c.OfficeNumber) ? "" : c.OfficeNumber),
                              ReportDateTime = c.ReportDateTime,
                              WorkCompletedTime = c.WorkCompletedTime,
                              Status = c.Status,
                              StatusChangeDate = c.StatusChangeDate
                          }).Where(x => x.ServiceReportNumber.ToLower().Contains(request.SearchTerm) || x.ClientName.ToLower().Contains(request.SearchTerm) || x.PhoneNumber.Contains(request.SearchTerm) || x.MobileNumber.Contains(request.SearchTerm) || x.OfficeNumber.Contains(request.SearchTerm)).ToList();
                    if (request.LastCallDateTime == null)
                    {
                        EmpServices = sv.Where(x => (x.Status == api.App_Start.Utilities.ServiceTypes.Completed.GetEnumDescription() || x.Status == api.App_Start.Utilities.ServiceTypes.NoShow.GetEnumDescription())).OrderByDescending(x => x.StatusChangeDate).ToList();
                    }
                    else
                    {
                        EmpServices = sv.Where(x => (x.Status == api.App_Start.Utilities.ServiceTypes.Completed.GetEnumDescription() || x.Status == api.App_Start.Utilities.ServiceTypes.NoShow.GetEnumDescription()) && x.StatusChangeDate >= request.LastCallDateTime).OrderByDescending(x => x.StatusChangeDate).ToList();
                    }
                    if (request.PageNumber.HasValue)
                    {
                        result = CreatePagedResults<CompletedServiceReportModel, CompletedServiceReportModel>(EmpServices.AsQueryable(), request.PageNumber.Value, pageSize, out totalRecord, out pageCnt, out totalPageCount).ToList();
                    }
                    else
                    {
                        result = EmpServices;
                    }
                    var Services = result;
                    if (Services.Count > 0)
                    {
                        foreach (CompletedServiceReportModel service in Services)
                        {
                            var d = new
                            {
                                Id = service.ReportId,
                                ServiceReportNumber = (string.IsNullOrWhiteSpace(service.ServiceReportNumber) ? "" : service.ServiceReportNumber),
                                service.ClientName,
                                PurposeOfVisit = (service.PurposeOfVisit),
                                PhoneNumber = (string.IsNullOrWhiteSpace(service.PhoneNumber) ? "" : service.PhoneNumber),
                                MobileNumber = (string.IsNullOrWhiteSpace(service.MobileNumber) ? "" : service.MobileNumber),
                                OfficeNumber = (string.IsNullOrWhiteSpace(service.OfficeNumber) ? "" : service.OfficeNumber),
                                ReportDateTime = service.ReportDateTime.Value.ToString("MMMM dd, yyyy") + " " + service.WorkCompletedTime
                            };
                            data.Add(d);
                        }
                        res.Data = data;
                        res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                        res.Message = "Records Found";
                        res.PageNumber = pageCnt;
                        res.TotalNumberOfPages = totalPageCount;
                        res.TotalNumberOfRecords = totalRecord;
                    }
                    else
                    {
                        res.Data = null;
                        res.Message = "No Data Found";
                        res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                        res.PageNumber = pageCnt - 1;
                        res.TotalNumberOfPages = totalPageCount;
                        res.TotalNumberOfRecords = totalRecord;
                    }
                }
            }
            else
            {
                res.Data = null;
                res.Message = "You are not authorized to view this details";
                res.StatusCode = HttpStatusCode.Unauthorized.GetEnumValue();
                res.PageNumber = (request.PageNumber.HasValue ? request.PageNumber.Value : 1);
                res.TotalNumberOfPages = totalPageCount;
                res.TotalNumberOfRecords = totalRecord;
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
        [Route("CompletedReportsDetail")]
        public async Task<IHttpActionResult> CompletedReportsDetail([FromBody] EmpCommonModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            ServiceReporModel sr = new ServiceReporModel();
            var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId).FirstOrDefault();
            if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "Your account was deactivated by Admin.";
                res.Data = null;
            }
            else
            {
                var ServiceReport = db.ServiceReports.Find(request.ReportId);
                try
                {
                    if (ServiceReport != null)
                    {
                        var ClientService = ServiceReport.Service;
                        var isnoshow = (ClientService.IsNoShow.HasValue ? ClientService.IsNoShow.Value : false);
                        var isdifferenttime = true;
                        string WorkStartedTime = (ServiceReport == null ? "" : ServiceReport.WorkStartedTime);
                        var WorkCompletedTime = (ServiceReport == null ? "" : ServiceReport.WorkCompletedTime);
                        var ExtraTime = 0d;
                        var AssignedTotalTime = 0d;
                        if (!isnoshow)
                        {
                            if (ClientService.ScheduleStartTime == WorkStartedTime && ClientService.ScheduleEndTime == WorkCompletedTime)
                            {
                                isdifferenttime = false;
                            }
                            if (WorkStartedTime != "" && WorkCompletedTime != "")
                            {
                                DateTime dateTime = DateTime.ParseExact(ClientService.ScheduleStartTime, "hh:mm tt", CultureInfo.InvariantCulture);

                                DateTime dateTime2 = DateTime.ParseExact(ClientService.ScheduleEndTime, "hh:mm tt", CultureInfo.InvariantCulture);

                                TimeSpan time1 = dateTime2.Subtract(dateTime);
                                AssignedTotalTime = time1.TotalMinutes;
                                DateTime dateTime3 = new DateTime();
                                try
                                {
                                    dateTime3 = DateTime.ParseExact(WorkStartedTime, "hh:mm tt", CultureInfo.InvariantCulture);
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        dateTime3 = DateTime.ParseExact(WorkStartedTime, "h:mm tt", CultureInfo.InvariantCulture);
                                    }
                                    catch (Exception)
                                    {

                                        DateTime.ParseExact(WorkStartedTime, "HH:mm", CultureInfo.InvariantCulture);
                                    }
                                }

                                DateTime dateTime4 = new DateTime();
                                try
                                {
                                    dateTime4 = DateTime.ParseExact(WorkCompletedTime, "hh:mm tt", CultureInfo.InvariantCulture);
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        dateTime4 = DateTime.ParseExact(WorkCompletedTime, "h:mm tt", CultureInfo.InvariantCulture);
                                    }
                                    catch (Exception)
                                    {
                                        dateTime4 = DateTime.ParseExact(WorkCompletedTime, "HH:mm", CultureInfo.InvariantCulture);
                                    }
                                }

                                TimeSpan time2 = dateTime4.Subtract(dateTime3);

                                if (isdifferenttime)
                                {
                                    ExtraTime = time2.TotalMinutes - time1.TotalMinutes;
                                }
                            }
                        }


                        sr.ReportNumber = ServiceReport.ServiceReportNumber;
                        sr.ClientName = ServiceReport.Service.Client.FirstName + " " + ServiceReport.Service.Client.LastName;
                        sr.Company = ServiceReport.Service.Client.Company ?? "-";
                        sr.AccountNo = ServiceReport.Service.Client.AccountNumber;
                        sr.PurposeOfVisit = (ServiceReport.Service.PurposeOfVisit);
                        var planTypeId = ServiceReport.ServiceReportUnits.FirstOrDefault().ClientUnit.PlanTypeId;
                        var planName = db.SubscriptionPlans.FirstOrDefault(p => p.Id == planTypeId).PlanName;
                        sr.BillingType = planName;

                        sr.ReportDate = ServiceReport.AddedDate.Value.ToString("dd MMMM yyyy");
                        sr.AssignedTotalTime = AssignedTotalTime.ToString() + " Min";
                        sr.ScheduleStartTime = ClientService.ScheduleStartTime;
                        sr.ScheduleEndTime = ClientService.ScheduleEndTime;
                        sr.WorkStartedTime = ServiceReport.WorkStartedTime;
                        sr.WorkCompletedTime = ServiceReport.WorkCompletedTime;
                        sr.IsDifferentTime = isdifferenttime;

                        sr.ExtraTime = (ExtraTime > 0 ? ExtraTime + " Min" : "");
                        sr.IsWorkNotDone = (ServiceReport.IsWorkDone != null ? !ServiceReport.IsWorkDone.Value : true);
                        sr.Units = new List<ServiceUnitsResponse>();
                        var Units = ServiceReport.ServiceReportUnits.Where(x => x.IsCompleted == true).ToList();
                        foreach (var item in Units)
                        {
                            ServiceUnitsResponse unit = new ServiceUnitsResponse();
                            unit.Id = item.UnitId.Value;
                            unit.UnitName = item.ClientUnit.UnitName;
                            unit.IsCompleted = true;
                            unit.ServiceUnitParts = new List<ServiceUnitPartsResponse>();
                            var spunitParts = ServiceReport.Service.ServicePartLists.Where(x => x.UnitId == item.UnitId && x.IsUsed == true).ToList();
                            foreach (var part in spunitParts)
                            {
                                ServiceUnitPartsResponse pt = new ServiceUnitPartsResponse();
                                pt.PartName = part.Part.Name;
                                pt.Qty = (part.UsedQuantity == null ? 0 : part.UsedQuantity.Value);
                                unit.ServiceUnitParts.Add(pt);
                            }
                            sr.Units.Add(unit);
                        }
                        sr.WorkPerformed = ServiceReport.WorkPerformed;
                        sr.RequestedParts = new List<RequestedPartsResponse>();
                        foreach (var item in ServiceReport.EmployeePartRequestMasters)
                        {
                            foreach (var epr in item.EmployeePartRequests)
                            {
                                RequestedPartsResponse rpr = new RequestedPartsResponse();
                                rpr.PartName = (string.IsNullOrWhiteSpace(epr.PartName) && epr.Part != null ? epr.Part.Name : epr.PartName);
                                rpr.PartSize = (string.IsNullOrWhiteSpace(epr.PartSize) && epr.Part != null ? epr.Part.Size ?? "" : epr.PartSize);
                                rpr.UnitId = epr.UnitId;
                                sr.RequestedParts.Add(rpr);
                            }
                        }
                        sr.Images = new List<string>();
                        foreach (var item in ServiceReport.ServiceReportImages)
                        {
                            sr.Images.Add(ConfigurationManager.AppSettings["ReportImageURL"] + item.ServiceImage);
                        }
                        sr.Recommendationsforcustomer = ServiceReport.Recommendationsforcustomer;
                        if (!string.IsNullOrWhiteSpace(ServiceReport.ClientSignature))
                        {
                            sr.ClientSignature = ConfigurationManager.AppSettings["ClientSignatureURL"] + ServiceReport.ClientSignature;
                        }
                        sr.EmailToClient = new List<string>();
                        sr.EmailToClient.Add(ServiceReport.Service.Client.Email);
                        if (!string.IsNullOrWhiteSpace(ServiceReport.CCEmail))
                        {
                            sr.CCEmail = ServiceReport.CCEmail;
                        }
                        else
                        {
                            sr.CCEmail = "";
                        }
                        sr.EmployeeNotes = ServiceReport.EmployeeNotes;

                        res.Data = sr;
                        res.Message = "Record Found";
                        res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                    }
                    else
                    {
                        res.Data = null;
                        res.Message = "No Data Found";
                        res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                    }
                }
                catch (Exception ex)
                {
                    res.StatusCode = HttpStatusCode.InternalServerError.GetEnumValue();
                    res.Data = null;
                    res.Message = "Internal Server Error";
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
        [ResponseType(typeof(ResponseListModel))]
        [HttpPost]
        [Route("GetAllScheduledServices")]
        public async Task<IHttpActionResult> GetAllScheduledServices([FromBody] EmpCommonModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseListModel res = new ResponseListModel();
            List<object> data = new List<object>();
            int totalRecord = 0;
            int pageCnt = 0;
            int totalPageCount = 0;
            List<CompletedServiceModel> EmpServices = new List<CompletedServiceModel>();
            List<CompletedServiceModel> result = new List<CompletedServiceModel>();
            List<CompletedServiceModel> sv = new List<CompletedServiceModel>();
            List<CompletedServiceModel> svrpt = new List<CompletedServiceModel>();
            List<CompletedServiceModel> svFinal = new List<CompletedServiceModel>();
            var pageSize = int.Parse(api.App_Start.Utilities.GetSiteSettingValue("ApplicationPageSize", db));

            var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId).FirstOrDefault();
            if (EmpInfo != null)
            {
                if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    DateTime dtStart = new DateTime();
                    DateTime dtEnd = new DateTime();

                    dtStart = request.StatDate.Value.Date;
                    dtEnd = (request.EndDate == null ? dtStart : request.EndDate.Value.Date);

                    sv = (from s in db.Services
                          join c in db.Clients on s.ClientId equals c.Id
                          join su in db.ServiceUnits on s.Id equals su.ServiceId
                          join cu in db.ClientUnits on su.UnitId equals cu.Id
                          join ut in db.UnitTypes on cu.UnitTypeId equals ut.Id
                          where s.EmployeeId == request.EmployeeId
                          select new CompletedServiceModel()
                          {
                              ServiceId = s.Id,
                              ClientName = c.FirstName + " " + c.LastName,
                              ServiceDate = s.ScheduleDate,
                              ServiceTime = s.ScheduleStartTime,
                              UnitName = cu.UnitName,
                              Status = s.Status,
                              UnitId = cu.Id,
                              UnitType = ut.UnitTypeName,
                              ManufactureDate = cu.ManufactureDate,
                              IsMatched = (cu.IsMatched == null ? false : cu.IsMatched.Value)
                          }).AsEnumerable().Where(x => x.Status == api.App_Start.Utilities.ServiceTypes.Scheduled.GetEnumDescription())
                          .Select(c => new CompletedServiceModel()
                          {
                              ServiceId = c.ServiceId,
                              ServiceDate = c.ServiceDate,
                              ServiceTime = c.ServiceTime,
                              UnitName = c.UnitName,
                              IsMatched = c.IsMatched,
                              Status = c.Status,
                              UnitId = c.UnitId,
                              UnitType = string.IsNullOrWhiteSpace(c.UnitType) ? "" : c.UnitType,
                              ManufactureDate = c.ManufactureDate,
                              ClientName = c.ClientName,
                              UnitAge = (c.ManufactureDate == null ? false : (DateTime.UtcNow.Year - c.ManufactureDate.Value.Year) >= 10 ? true : false)
                              //UnitAge = (c.ManufactureDate == null ? "1 Year" : (DateTime.UtcNow.Year - c.ManufactureDate.Value.Year) > 0 ? DateTime.UtcNow.Year - c.ManufactureDate.Value.Year + " Year" : DateTime.UtcNow.TotalMonths(c.ManufactureDate.Value) + " Month"),
                          })
                          .Where(x => (x.ServiceDate.Value.Date >= dtStart && x.ServiceDate.Value.Date <= dtEnd)).Distinct().ToList();

                    svrpt = (from s in db.Services
                             join c in db.Clients on s.ClientId equals c.Id
                             join su in db.ServiceReportUnits on s.Id equals su.ServiceId
                             join cu in db.ClientUnits on su.UnitId equals cu.Id
                             join ut in db.UnitTypes on cu.UnitTypeId equals ut.Id
                             where s.EmployeeId == request.EmployeeId
                             select new CompletedServiceModel()
                             {
                                 ServiceId = s.Id,
                                 ClientName = c.FirstName + " " + c.LastName,
                                 ServiceDate = s.ScheduleDate,
                                 ServiceTime = s.ScheduleStartTime,
                                 UnitName = cu.UnitName,
                                 Status = s.Status,
                                 UnitId = cu.Id,
                                 UnitType = ut.UnitTypeName,
                                 ManufactureDate = cu.ManufactureDate,
                                 IsMatched = (cu.IsMatched == null ? false : cu.IsMatched.Value)
                             }).AsEnumerable().Where(x => x.Status == api.App_Start.Utilities.ServiceTypes.Completed.GetEnumDescription())
                          .Select(c => new CompletedServiceModel()
                          {
                              ServiceId = c.ServiceId,
                              ServiceDate = c.ServiceDate,
                              ServiceTime = c.ServiceTime,
                              UnitName = c.UnitName,
                              IsMatched = c.IsMatched,
                              Status = c.Status,
                              UnitId = c.UnitId,
                              UnitType = string.IsNullOrWhiteSpace(c.UnitType) ? "" : c.UnitType,
                              ManufactureDate = c.ManufactureDate,
                              ClientName = c.ClientName,
                              UnitAge = (c.ManufactureDate == null ? false : (DateTime.UtcNow.Year - c.ManufactureDate.Value.Year) >= 10 ? true : false)
                          })
                          .Where(x => (x.ServiceDate.Value.Date >= dtStart && x.ServiceDate.Value.Date <= dtEnd)).Distinct().ToList();

                    var ids = svrpt.Select(x => x.ServiceId).ToArray();

                    svFinal.AddRange(svrpt);

                    var svs = sv.Where(x => !ids.Contains(x.ServiceId)).ToList();
                    svFinal.AddRange(svs);

                    EmpServices = svFinal.OrderByDescending(x => x.ServiceDate).Distinct().ToList();

                    if (request.PageNumber.HasValue)
                    {
                        result = CreatePagedResults<CompletedServiceModel, CompletedServiceModel>(EmpServices.AsQueryable(), request.PageNumber.Value, pageSize, out totalRecord, out pageCnt, out totalPageCount).ToList();
                    }
                    else
                    {
                        result = EmpServices;
                    }
                    var Services = result;
                    if (Services.Count > 0)
                    {
                        foreach (CompletedServiceModel service in Services)
                        {
                            List<object> objUnitParts = new List<object>();
                            var c = service;
                            var ClientUnit = db.ClientUnits.Where(x => x.Id == c.UnitId).FirstOrDefault();
                            var add = new
                            {
                                ClientUnit.ClientAddress.Address,
                                CityName = ClientUnit.ClientAddress.City1.Name,
                                StateName = ClientUnit.ClientAddress.State1.Name,
                                ZipCode = ClientUnit.ClientAddress.ZipCode,
                            };
                            foreach (var cUnitPart in ClientUnit.ClientUnitParts)
                            {
                                cUnitPart.SplitType = string.IsNullOrWhiteSpace(cUnitPart.SplitType) ? "" : cUnitPart.SplitType;
                                var objUnitPart = new
                                {
                                    ModelNumber = (string.IsNullOrWhiteSpace(cUnitPart.ModelNumber) ? "" : cUnitPart.ModelNumber),
                                    SerialNumber = (string.IsNullOrWhiteSpace(cUnitPart.SerialNumber) ? "" : cUnitPart.SerialNumber),
                                    ManufactureDate = (cUnitPart.ManufactureDate == null ? "" : cUnitPart.ManufactureDate.Value.ToString("MMMM, yyyy")),
                                    UnitType = (ClientUnit.UnitType.Id == 2 ? cUnitPart.SplitType : ClientUnit.UnitType.UnitTypeName)
                                };
                                objUnitParts.Add(objUnitPart);
                            }
                            var unit = db.ClientUnits.FirstOrDefault(u => u.Id == c.UnitId);
                            var planTypeId = unit.PlanTypeId;
                            var planName = db.SubscriptionPlans.FirstOrDefault(p => p.Id == planTypeId).PlanName;

                            var d = new
                            {
                                ServiceId = c.ServiceId,
                                ServiceDate = c.ServiceDate,
                                ServiceTime = c.ServiceTime,
                                UnitName = c.UnitName,
                                IsMatched = c.IsMatched,
                                ClientName = c.ClientName,
                                UnitType = c.UnitType,
                                UnitId = c.UnitId,
                                Address = add,
                                PlanType = planName,
                                ClientUnitPart = objUnitParts,
                                c.UnitAge
                            };
                            data.Add(d);
                        }
                        var ClientUnitsFailed = db.ClientUnits.AsEnumerable().Where(x => x.AddedBy == request.EmployeeId && x.AddedByType == api.App_Start.Utilities.UserRoles.Employee.GetEnumValue() && x.IsDeleted == false && x.IsActive == true && x.PaymentStatus == api.App_Start.Utilities.UnitPaymentTypes.PaymentFailed.GetEnumDescription()).ToList();
                        var ClientUnitsNotProcessed = db.ClientUnits.AsEnumerable().Where(x => x.AddedBy == request.EmployeeId && x.AddedByType == api.App_Start.Utilities.UserRoles.Employee.GetEnumValue() && x.IsDeleted == false && x.IsActive == true && x.PaymentStatus == api.App_Start.Utilities.UnitPaymentTypes.NotReceived.GetEnumDescription()).ToList();
                        var ClientUnitsProcessing = db.ClientUnits.AsEnumerable().Where(x => x.AddedBy == request.EmployeeId && x.AddedByType == api.App_Start.Utilities.UserRoles.Employee.GetEnumValue() && x.IsDeleted == false && x.IsActive == true && x.PaymentStatus == api.App_Start.Utilities.UnitPaymentTypes.Processing.GetEnumDescription()).ToList();
                        ClientUnitsFailed.AddRange(ClientUnitsNotProcessed);
                        res.Data = data;
                        res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                        res.Message = "Records Found";
                        res.PageNumber = pageCnt;
                        res.TotalNumberOfPages = totalPageCount;
                        res.TotalNumberOfRecords = totalRecord;
                        res.HasPaymentFailedUnit = (ClientUnitsFailed.Count > 0 ? true : false);
                        res.HasPaymentProcessingUnits = (ClientUnitsProcessing.Count > 0 ? true : false);
                    }
                    else
                    {
                        var ClientUnitsFailed = db.ClientUnits.AsEnumerable().Where(x => x.AddedBy == request.EmployeeId && x.AddedByType == api.App_Start.Utilities.UserRoles.Employee.GetEnumValue() && x.IsDeleted == false && x.IsActive == true && x.PaymentStatus == api.App_Start.Utilities.UnitPaymentTypes.PaymentFailed.GetEnumDescription()).ToList();
                        var ClientUnitsNotProcessed = db.ClientUnits.AsEnumerable().Where(x => x.AddedBy == request.EmployeeId && x.AddedByType == api.App_Start.Utilities.UserRoles.Employee.GetEnumValue() && x.IsDeleted == false && x.IsActive == true && x.PaymentStatus == api.App_Start.Utilities.UnitPaymentTypes.NotReceived.GetEnumDescription()).ToList();
                        var ClientUnitsProcessing = db.ClientUnits.AsEnumerable().Where(x => x.AddedBy == request.EmployeeId && x.AddedByType == api.App_Start.Utilities.UserRoles.Employee.GetEnumValue() && x.IsDeleted == false && x.IsActive == true && x.PaymentStatus == api.App_Start.Utilities.UnitPaymentTypes.Processing.GetEnumDescription()).ToList();
                        ClientUnitsFailed.AddRange(ClientUnitsNotProcessed);

                        res.Data = null;
                        res.Message = "No Data Found";
                        res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                        res.PageNumber = pageCnt - 1;
                        res.TotalNumberOfPages = totalPageCount;
                        res.TotalNumberOfRecords = totalRecord;
                        res.HasPaymentFailedUnit = (ClientUnitsFailed.Count > 0 ? true : false);
                        res.HasPaymentProcessingUnits = (ClientUnitsProcessing.Count > 0 ? true : false);
                    }
                }
            }
            else
            {

                res.Data = null;
                res.Message = "You are not authorized to view this details";
                res.StatusCode = HttpStatusCode.Unauthorized.GetEnumValue();
                res.PageNumber = (request.PageNumber.HasValue ? request.PageNumber.Value : 1);
                res.TotalNumberOfPages = totalPageCount;
                res.TotalNumberOfRecords = totalRecord;
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
        [Route("GetScheduledServiceUnitDetails")]
        public async Task<IHttpActionResult> GetScheduledServiceUnitDetails([FromBody] EmpCommonModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId).FirstOrDefault();
            if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "Your account was deactivated by Admin.";
                res.Data = null;
            }
            else
            {
                var cUnit = await db.ClientUnits.FindAsync(request.UnitId);
                if (cUnit != null)
                {
                    var UnitAddress = new
                    {
                        cUnit.ClientAddress.Address,
                        CityName = cUnit.ClientAddress.City1.Name,
                        StateName = cUnit.ClientAddress.State1.Name,
                        cUnit.ClientAddress.ZipCode
                    };

                    List<object> objUnitParts = new List<object>();

                    foreach (var cUnitPart in cUnit.ClientUnitParts)
                    {
                        List<object> objUnitFilters = new List<object>();
                        List<object> objUnitFuses = new List<object>();
                        var filterExtra = cUnitPart.UnitExtraInfoes.Where(x => x.ExtraInfoType == "Filter").ToList();
                        int FilterQty = filterExtra.Count;

                        foreach (var item in filterExtra)
                        {
                            var d = new
                            {
                                PartId = item.Part.Id,
                                FilterSize = item.Part.Name + " " + item.Part.Size ?? "",
                                FilterLocation = item.LocationOfPart.Value
                            };
                            objUnitFilters.Add(d);
                        }

                        var FusesExtra = cUnitPart.UnitExtraInfoes.Where(x => x.ExtraInfoType == "Fuses").ToList();
                        int FusesQty = FusesExtra.Count;

                        foreach (var item in FusesExtra)
                        {
                            var d = new
                            {
                                PartId = item.Part.Id,
                                FuseType = item.Part.Name
                            };
                            objUnitFuses.Add(d);
                        }
                        int pid = (cUnitPart.Breaker.HasValue ? cUnitPart.Breaker.Value : 0);
                        var BreakerPart = db.Parts.Where(x => x.Id == pid).FirstOrDefault();
                        var splitTp = (cUnit.UnitType.Id == 2 ? cUnitPart.SplitType : cUnit.UnitType.UnitTypeName);
                        //try
                        //{
                        var objUnitPart = new
                        {
                            cUnitPart.ModelNumber,
                            cUnitPart.SerialNumber,
                            UnitType = (cUnit.UnitType.Id == 2 ? cUnitPart.SplitType : cUnit.UnitType.UnitTypeName),
                            cUnitPart.ManufactureBrand,
                            ManufactureDate = (cUnitPart.ManufactureDate != null ? cUnitPart.ManufactureDate.Value.ToString("MMMM, yyyy") : ""),
                            UnitTon = (cUnitPart.UnitTon != null ? cUnitPart.UnitTon : "NA"),

                            //Blower Part2
                            ThermostatId = (cUnitPart.Thermostat == null ? 0 : cUnitPart.Part3.IsDeleted ? 0 : cUnitPart.Thermostat),
                            Thermostat = (cUnitPart.Part3 != null ? cUnitPart.Part3.IsDeleted ? "NA" : cUnitPart.Part3.Name + "-" + cUnitPart.Part3.Size ?? "" : "NA"),

                            //Rolloutsensor Part23
                            BreakerId = (cUnitPart.Breaker == null ? 0 : cUnitPart.Part1.IsDeleted ? 0 : cUnitPart.Breaker),
                            Breaker = (cUnitPart.Part1 != null ? cUnitPart.Part1.IsDeleted ? "NA" : cUnitPart.Part1.Name + "-" + cUnitPart.Part1.Size ?? "" : "NA"),

                            // Breaker Part1
                            BlowerMotorId = (cUnitPart.BlowerMotor == null ? 0 : cUnitPart.Part2.IsDeleted ? 0 : cUnitPart.BlowerMotor),
                            BlowerMotor = (cUnitPart.Part2 != null ? cUnitPart.Part2.IsDeleted ? "NA" : cUnitPart.Part2.Name + "-" + cUnitPart.Part2.Size ?? "" : "NA"),

                            // Limitswitch Part17
                            RefrigerantTypeId = (cUnitPart.RefrigerantType == null ? 0 : cUnitPart.Part20.IsDeleted ? 0 : cUnitPart.RefrigerantType),
                            Refrigerant = (cUnitPart.Part20 != null ? cUnitPart.Part20.IsDeleted ? "NA" : cUnitPart.Part20.Name + "-" + cUnitPart.Part20.Size ?? "" : "NA"),

                            // Capacitor Part4
                            CompressorId = (cUnitPart.Compressor == null ? 0 : cUnitPart.Part6.IsDeleted ? 0 : cUnitPart.Compressor),
                            Compressor = (cUnitPart.Part6 != null ? cUnitPart.Part6.IsDeleted ? "NA" : cUnitPart.Part6.Name + "-" + cUnitPart.Part6.Size ?? "" : "NA"),

                            // Booster Part3
                            CapacitorId = (cUnitPart.Capacitor == null ? 0 : cUnitPart.Part4.IsDeleted ? 0 : cUnitPart.Capacitor),
                            Capacitor = (cUnitPart.Part4 != null ? cUnitPart.Part4.IsDeleted ? "" : cUnitPart.Part4.Name + "-" + cUnitPart.Part4.Size ?? "" : "NA"),

                            // Compressor Part6
                            ContactorId = (cUnitPart.Contactor == null ? 0 : cUnitPart.Part8.IsDeleted ? 0 : cUnitPart.Contactor),
                            Contactor = (cUnitPart.Part8 != null ? cUnitPart.Part8.IsDeleted ? "" : cUnitPart.Part8.Name + "-" + cUnitPart.Part8.Size ?? "" : "NA"),

                            // Defrostboard Part10
                            FilterdryerId = (cUnitPart.Filterdryer == null ? 0 : cUnitPart.Part12.IsDeleted ? 0 : cUnitPart.Filterdryer),
                            Filterdryer = (cUnitPart.Part12 != null ? cUnitPart.Part12.IsDeleted ? "NA" : cUnitPart.Part12.Name + "-" + cUnitPart.Part12.Size ?? "" : "NA"),

                            // Contactor Part8
                            DefrostboardId = (cUnitPart.Defrostboard == null ? 0 : cUnitPart.Part10.IsDeleted ? 0 : cUnitPart.Defrostboard),
                            Defrostboard = (cUnitPart.Part10 != null ? cUnitPart.Part10.IsDeleted ? "NA" : cUnitPart.Part10.Name + "-" + cUnitPart.Part10.Size ?? "" : "NA"),

                            //  Misc Part18
                            RelayId = (cUnitPart.Relay == null ? 0 : cUnitPart.Part21.IsDeleted ? 0 : cUnitPart.Relay),
                            Relay = (cUnitPart.Part21 != null ? cUnitPart.Part21.IsDeleted ? "NA" : cUnitPart.Part21.Name + "-" + cUnitPart.Part21.Size ?? "" : "NA"),

                            // ReversingValve Part25
                            TXVValveId = (cUnitPart.TXVValve == null ? 0 : cUnitPart.Part25.IsDeleted ? 0 : cUnitPart.TXVValve),
                            TXVValve = (cUnitPart.Part25 != null ? cUnitPart.Part25.IsDeleted ? "NA" : cUnitPart.Part25.Name + "-" + cUnitPart.Part25.Size ?? "" : "NA"),

                            // Pressureswitch Part19
                            ReversingValveId = (cUnitPart.ReversingValve == null ? 0 : cUnitPart.Part22.IsDeleted ? 0 : cUnitPart.ReversingValve),
                            ReversingValve = (cUnitPart.Part25 != null ? cUnitPart.Part22.IsDeleted ? "NA" : cUnitPart.Part22.Name + "-" + cUnitPart.Part22.Size ?? "" : "NA"),

                            // Coil Part5
                            CondensingfanmotorId = (cUnitPart.Condensingfanmotor == null ? 0 : cUnitPart.Part7.IsDeleted ? 0 : cUnitPart.Condensingfanmotor),
                            Condensingfanmotor = (cUnitPart.Part7 != null ? cUnitPart.Part7.IsDeleted ? "NA" : cUnitPart.Part7.Name + "-" + cUnitPart.Part7.Size ?? "" : "NA"),

                            InducerdraftmotorId = (cUnitPart.Inducerdraftmotor == null ? 0 : cUnitPart.Part.IsDeleted ? 0 : cUnitPart.Inducerdraftmotor),
                            Inducerdraftmotor = (cUnitPart.Part != null ? cUnitPart.Part.IsDeleted ? "NA" : cUnitPart.Part.Name + "-" + cUnitPart.Part.Size ?? "" : "NA"),

                            // Relay Part21
                            TransformerId = (cUnitPart.Transformer == null ? 0 : cUnitPart.Part24.IsDeleted ? 0 : cUnitPart.Transformer),
                            Transformer = (cUnitPart.Part24 != null ? cUnitPart.Part24.IsDeleted ? "NA" : cUnitPart.Part24.Name + "-" + cUnitPart.Part24.Size ?? "" : "NA"),

                            // Condensingfanmotor Part7
                            ControlboardId = (cUnitPart.Controlboard == null ? 0 : cUnitPart.Part9.IsDeleted ? 0 : cUnitPart.Controlboard),
                            Controlboard = (cUnitPart.Part9 != null ? cUnitPart.Part9.IsDeleted ? "NA" : cUnitPart.Part9.Name + "-" + cUnitPart.Part9.Size ?? "" : "NA"),

                            // Ignitioncontrolboard Part15
                            LimitswitchId = (cUnitPart.Limitswitch == null ? 0 : cUnitPart.Part17.IsDeleted ? 0 : cUnitPart.Limitswitch),
                            Limitswitch = (cUnitPart.Part17 != null ? cUnitPart.Part17.IsDeleted ? "NA" : cUnitPart.Part17.Name + "-" + cUnitPart.Part17.Size ?? "" : "NA"),

                            // Gasvalve Part14
                            IgnitorId = (cUnitPart.Ignitor == null ? 0 : cUnitPart.Part16.IsDeleted ? 0 : cUnitPart.Ignitor),
                            Ignitor = (cUnitPart.Part16 != null ? cUnitPart.Part16.IsDeleted ? "NA" : cUnitPart.Part16.Name + "-" + cUnitPart.Part16.Size ?? "" : "NA"),

                            // Filterdryer Part12
                            GasvalveId = (cUnitPart.Gasvalve == null ? 0 : cUnitPart.Part14.IsDeleted ? 0 : cUnitPart.Gasvalve),
                            Gasvalve = (cUnitPart.Part14 != null ? cUnitPart.Part14.IsDeleted ? "NA" : cUnitPart.Part14.Name + "-" + cUnitPart.Part14.Size ?? "" : "NA"),

                            // Ignitor Part16
                            PressureswitchId = (cUnitPart.Pressureswitch == null ? 0 : cUnitPart.Part19.IsDeleted ? 0 : cUnitPart.Pressureswitch),
                            Pressureswitch = (cUnitPart.Part19 != null ? cUnitPart.Part19.IsDeleted ? "NA" : cUnitPart.Part19.Name + "-" + cUnitPart.Part19.Size ?? "" : "NA"),

                            // Doorswitch Part13
                            FlamesensorId = (cUnitPart.Flamesensor == null ? 0 : cUnitPart.Part13.IsDeleted ? 0 : cUnitPart.Flamesensor),
                            Flamesensor = (cUnitPart.Part13 != null ? cUnitPart.Part13.IsDeleted ? "NA" : cUnitPart.Part13.Name + "-" + cUnitPart.Part13.Size ?? "" : "NA"),

                            //Refrigerant Part20
                            RolloutsensorId = (cUnitPart.Rolloutsensor == null ? 0 : cUnitPart.Part23.IsDeleted ? 0 : cUnitPart.Rolloutsensor),
                            Rolloutsensor = (cUnitPart.Part23 != null ? cUnitPart.Part23.IsDeleted ? "NA" : cUnitPart.Part23.Name + "-" + cUnitPart.Part23.Size ?? "" : "NA"),

                            // Controlboard Part9
                            DoorswitchId = (cUnitPart.Doorswitch == null ? 0 : cUnitPart.Part11.IsDeleted ? 0 : cUnitPart.Doorswitch),
                            Doorswitch = (cUnitPart.Part11 != null ? cUnitPart.Part11.IsDeleted ? "NA" : cUnitPart.Part11.Name + "-" + cUnitPart.Part11.Size ?? "" : "NA"),

                            //Flamesensor Part13
                            IgnitioncontrolboardId = (cUnitPart.Ignitioncontrolboard == null ? 0 : cUnitPart.Part15.IsDeleted ? 0 : cUnitPart.Ignitioncontrolboard),
                            Ignitioncontrolboard = (cUnitPart.Part15 != null ? cUnitPart.Part15.IsDeleted ? "NA" : cUnitPart.Part15.Name + "-" + cUnitPart.Part15.Size ?? "" : "NA"),

                            ElectricalService = (string.IsNullOrWhiteSpace(cUnitPart.ElectricalService) != true ? cUnitPart.ElectricalService : "NA"),

                            MaxBreaker = (string.IsNullOrWhiteSpace(cUnitPart.MaxBreaker) != true ? cUnitPart.MaxBreaker : "NA"),

                            //BreakerId = (cUnitPart.Breaker.HasValue ? (BreakerPart != null ? (BreakerPart.IsDeleted ? 0 : pid) : 0) : 0),
                            //Breaker = (BreakerPart != null ? BreakerPart.IsDeleted ? "NA" : BreakerPart.Name + "-" + BreakerPart.Size ?? "" : "NA"),


                            // transformer Part24
                            CoilId = (cUnitPart.Coil == null ? 0 : cUnitPart.Part5.IsDeleted ? 0 : cUnitPart.Coil),
                            Coil = (cUnitPart.Part5 != null ? cUnitPart.Part5.IsDeleted ? "NA" : cUnitPart.Part5.Name + "-" + cUnitPart.Part5.Size ?? "" : "NA"),

                            // TXVValve Part25
                            MiscId = (cUnitPart.Misc == null ? 0 : cUnitPart.Part18.IsDeleted ? 0 : cUnitPart.Misc),
                            Misc = (cUnitPart.Part18 != null ? cUnitPart.Part18.IsDeleted ? "NA" : cUnitPart.Part18.Name + "-" + cUnitPart.Part18.Size ?? "" : "NA"),

                            FilterQty,
                            Filters = objUnitFilters,
                            FusesQty,
                            Fuses = objUnitFuses,
                            UnitManuals = cUnit.ClientUnitManuals.AsEnumerable().Where(x => x.SplitType == cUnitPart.SplitType).Select(x => new
                            {
                                x.ManualName,
                                ManualURL = ConfigurationManager.AppSettings["ManualURL"].ToString() + x.ManualName
                            }).ToList(),
                            UnitPictures = db.ClientUnitPictures.Where(x => x.ClientUnitId == cUnit.Id && x.SplitType == splitTp).AsEnumerable().Select(x => new
                            {
                                UnitImageUrl = (x.UnitImage != null ? ConfigurationManager.AppSettings["UnitImageURL"].ToString() + x.UnitImage : ""),

                            }).ToList()
                        };
                        objUnitParts.Add(objUnitPart);
                        //}
                        //catch (Exception ex)
                        //{
                        //}
                    }
                    var planName = db.SubscriptionPlans.FirstOrDefault(p => p.Id == cUnit.PlanTypeId).PlanName;
                    var UnitDetail = new
                    {
                        cUnit.Client.FirstName,
                        cUnit.Client.LastName,
                        cUnit.UnitName,
                        Address = UnitAddress,
                        Name = planName,
                        PackageDisplayName = planName,
                        UnitParts = objUnitParts,
                        IsMatched = (cUnit.IsMatched == null ? false : cUnit.IsMatched),
                        Notes = (cUnit.Notes == null ? "" : cUnit.Notes)

                        //ServiceHistory = ServiceHistory,
                        //UnitManuals = ManualURLs
                    };
                    res.Data = UnitDetail;
                    res.Message = "Data Found";
                    res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                }
                else
                {
                    res.Data = null;
                    res.Message = "No Data Found";
                    res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
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
        [Route("CheckUnitMatchDetails")]
        public async Task<IHttpActionResult> CheckUnitMatchDetails([FromBody] ClientUnitMatchRequestModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            List<object> objUnitParts = new List<object>();
            try
            {
                var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId).FirstOrDefault();
                if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    var cUParts = db.ClientUnits.Where(x => x.Id == request.UnitId && x.IsMatched == true).FirstOrDefault();
                    if (cUParts == null)
                    {
                        foreach (var item in request.Parts)
                        {
                            //var cUnitPart = db.Units.Where(x => x.ModelNumber == item.ModelNumber && x.SerialNumber == item.SerialNumber && x.Status == true && x.IsDeleted == false).FirstOrDefault();
                            var cUnitPart = db.Units.Where(x => x.ModelNumber == item.ModelNumber && x.Status == true && x.IsDeleted == false).FirstOrDefault();
                            if (cUnitPart != null)
                            {
                                var objUnitPart = new
                                {
                                    cUnitPart.ModelNumber,
                                    cUnitPart.SerialNumber,
                                    UnitType = item.UnitType,
                                    cUnitPart.ManufactureBrand,
                                    ManufactureDate = (cUnitPart.ManufactureDate != null ? cUnitPart.ManufactureDate.Value.ToString("dd MMMM yyyy") : ""),
                                    UnitTon = (cUnitPart.UnitTon != null ? cUnitPart.UnitTon : "NA"),

                                    InducerdraftmotorId = (cUnitPart.Inducerdraftmotor == null ? 0 : cUnitPart.Part5.IsDeleted ? 0 : cUnitPart.Inducerdraftmotor),
                                    Inducerdraftmotor = (cUnitPart.Part5 != null ? cUnitPart.Part5.IsDeleted ? "NA" : cUnitPart.Part5.Name + "-" + cUnitPart.Part5.Size ?? "" : "NA"),

                                    BlowerMotorId = (cUnitPart.BlowerMotor == null ? 0 : cUnitPart.Part3.IsDeleted ? 0 : cUnitPart.BlowerMotor),
                                    BlowerMotor = (cUnitPart.Part3 != null ? cUnitPart.Part3.IsDeleted ? "NA" : cUnitPart.Part3.Name + "-" + cUnitPart.Part3.Size ?? "" : "NA"),

                                    ThermostatId = (cUnitPart.Thermostat == null ? 0 : cUnitPart.Part.IsDeleted ? 0 : cUnitPart.Thermostat),
                                    Thermostat = (cUnitPart.Part != null ? cUnitPart.Part.IsDeleted ? "NA" : cUnitPart.Part.Name + "-" + cUnitPart.Part.Size ?? "" : "NA"),
                                    //MISC Part18
                                    CapacitorId = (cUnitPart.Capacitor == null ? 0 : cUnitPart.Part20.IsDeleted ? 0 : cUnitPart.Capacitor),
                                    Capacitor = (cUnitPart.Part20 != null ? cUnitPart.Part20.IsDeleted ? "NA" : cUnitPart.Part20.Name + "-" + cUnitPart.Part20.Size ?? "" : "NA"),

                                    //coil Part17
                                    CompressorId = (cUnitPart.Compressor == null ? 0 : cUnitPart.Part19.IsDeleted ? 0 : cUnitPart.Compressor),
                                    Compressor = (cUnitPart.Part19 != null ? cUnitPart.Part19.IsDeleted ? "NA" : cUnitPart.Part19.Name + "-" + cUnitPart.Part19.Size ?? "" : "NA"),

                                    CondensingfanmotorId = (cUnitPart.Condensingfanmotor == null ? 0 : cUnitPart.Part4.IsDeleted ? 0 : cUnitPart.Condensingfanmotor),
                                    Condensingfanmotor = (cUnitPart.Part4 != null ? cUnitPart.Part4.IsDeleted ? "NA" : cUnitPart.Part4.Name + "-" + cUnitPart.Part4.Size ?? "" : "NA"),

                                    ContactorId = (cUnitPart.Contactor == null ? 0 : cUnitPart.Part21.IsDeleted ? 0 : cUnitPart.Contactor),
                                    Contactor = (cUnitPart.Part21 != null ? cUnitPart.Part21.IsDeleted ? "NA" : cUnitPart.Part21.Name + "-" + cUnitPart.Part21.Size ?? "" : "NA"),

                                    ControlboardId = (cUnitPart.Controlboard == null ? 0 : cUnitPart.Part7.IsDeleted ? 0 : cUnitPart.Controlboard),
                                    Controlboard = (cUnitPart.Part7 != null ? cUnitPart.Part7.IsDeleted ? "NA" : cUnitPart.Part7.Name + "-" + cUnitPart.Part7.Size ?? "" : "NA"),

                                    //Breaker
                                    DefrostboardId = (cUnitPart.Defrostboard == null ? 0 : cUnitPart.Part23.IsDeleted ? 0 : cUnitPart.Defrostboard),
                                    Defrostboard = (cUnitPart.Part23 != null ? cUnitPart.Part23.IsDeleted ? "NA" : cUnitPart.Part23.Name + "-" + cUnitPart.Part23.Size ?? "" : "NA"),

                                    DoorswitchId = (cUnitPart.Doorswitch == null ? 0 : cUnitPart.Part14.IsDeleted ? 0 : cUnitPart.Doorswitch),
                                    Doorswitch = (cUnitPart.Part14 != null ? cUnitPart.Part14.IsDeleted ? "NA" : cUnitPart.Part14.Name + "-" + cUnitPart.Part14.Size ?? "" : "NA"),

                                    FilterdryerId = (cUnitPart.Filterdryer == null ? 0 : cUnitPart.Part22.IsDeleted ? 0 : cUnitPart.Filterdryer),
                                    Filterdryer = (cUnitPart.Part22 != null ? cUnitPart.Part22.IsDeleted ? "NA" : cUnitPart.Part22.Name + "-" + cUnitPart.Part22.Size ?? "" : "NA"),

                                    FlamesensorId = (cUnitPart.Flamesensor == null ? 0 : cUnitPart.Part11.IsDeleted ? 0 : cUnitPart.Flamesensor),
                                    Flamesensor = (cUnitPart.Part11 != null ? cUnitPart.Part11.IsDeleted ? "NA" : cUnitPart.Part11.Name + "-" + cUnitPart.Part11.Size ?? "" : "NA"),

                                    GasvalveId = (cUnitPart.Gasvalve == null ? 0 : cUnitPart.Part9.IsDeleted ? 0 : cUnitPart.Gasvalve),
                                    Gasvalve = (cUnitPart.Part9 != null ? cUnitPart.Part9.IsDeleted ? "NA" : cUnitPart.Part9.Name + "-" + cUnitPart.Part9.Size ?? "" : "NA"),

                                    IgnitorId = (cUnitPart.Ignitor == null ? 0 : cUnitPart.Part16.IsDeleted ? 0 : cUnitPart.Ignitor),
                                    Ignitor = (cUnitPart.Part16 != null ? cUnitPart.Part16.IsDeleted ? "NA" : cUnitPart.Part16.Name + "-" + cUnitPart.Part16.Size ?? "" : "NA"),

                                    IgnitioncontrolboardId = (cUnitPart.Ignitioncontrolboard == null ? 0 : cUnitPart.Part15.IsDeleted ? 0 : cUnitPart.Ignitioncontrolboard),
                                    Ignitioncontrolboard = (cUnitPart.Part15 != null ? cUnitPart.Part15.IsDeleted ? "NA" : cUnitPart.Part15.Name + "-" + cUnitPart.Part15.Size ?? "" : "NA"),

                                    LimitswitchId = (cUnitPart.Limitswitch == null ? 0 : cUnitPart.Part8.IsDeleted ? 0 : cUnitPart.Limitswitch),
                                    Limitswitch = (cUnitPart.Part8 != null ? cUnitPart.Part8.IsDeleted ? "NA" : cUnitPart.Part8.Name + "-" + cUnitPart.Part8.Size ?? "" : "NA"),

                                    PressureswitchId = (cUnitPart.Pressureswitch == null ? 0 : cUnitPart.Part10.IsDeleted ? 0 : cUnitPart.Pressureswitch),
                                    Pressureswitch = (cUnitPart.Part10 != null ? cUnitPart.Part10.IsDeleted ? "NA" : cUnitPart.Part10.Name + "-" + cUnitPart.Part10.Size ?? "" : "NA"),

                                    RefrigerantTypeId = (cUnitPart.Refrigerant == null ? 0 : cUnitPart.Part1.IsDeleted ? 0 : cUnitPart.Refrigerant),
                                    Refrigerant = (cUnitPart.Part1 != null ? cUnitPart.Part1.IsDeleted ? "NA" : cUnitPart.Part1.Name + "-" + cUnitPart.Part1.Size ?? "" : "NA"),

                                    //filter Part22
                                    RelayId = (cUnitPart.Relay == null ? 0 : cUnitPart.Part24.IsDeleted ? 0 : cUnitPart.Relay),
                                    Relay = (cUnitPart.Part24 != null ? cUnitPart.Part24.IsDeleted ? "NA" : cUnitPart.Part24.Name + "-" + cUnitPart.Part24.Size ?? "" : "NA"),

                                    ReversingValveId = (cUnitPart.ReversingValve == null ? 0 : cUnitPart.Part2.IsDeleted ? 0 : cUnitPart.ReversingValve),
                                    ReversingValve = (cUnitPart.Part2 != null ? cUnitPart.Part2.IsDeleted ? "NA" : cUnitPart.Part2.Name + "-" + cUnitPart.Part2.Size ?? "" : "NA"),

                                    RolloutsensorId = (cUnitPart.Rolloutsensor == null ? 0 : cUnitPart.Part13.IsDeleted ? 0 : cUnitPart.Rolloutsensor),
                                    Rolloutsensor = (cUnitPart.Part13 != null ? cUnitPart.Part13.IsDeleted ? "NA" : cUnitPart.Part13.Name + "-" + cUnitPart.Part13.Size ?? "" : "NA"),

                                    TransformerId = (cUnitPart.Transformer == null ? 0 : cUnitPart.Part6.IsDeleted ? 0 : cUnitPart.Transformer),
                                    Transformer = (cUnitPart.Part6 != null ? cUnitPart.Part6.IsDeleted ? "NA" : cUnitPart.Part6.Name + "-" + cUnitPart.Part6.Size ?? "" : "NA"),
                                    //Breaker Part12
                                    TXVValveId = (cUnitPart.TXVValve == null ? 0 : cUnitPart.Part25.IsDeleted ? 0 : cUnitPart.TXVValve),
                                    TXVValve = (cUnitPart.Part25 != null ? cUnitPart.Part25.IsDeleted ? "NA" : cUnitPart.Part25.Name + "-" + cUnitPart.Part25.Size ?? "" : "NA"),

                                    ElectricalService = (cUnitPart.ElectricalService != null ? cUnitPart.ElectricalService : "NA"),
                                    MaxBreaker = (cUnitPart.MaxBreaker != null ? cUnitPart.MaxBreaker : "NA"),
                                    //defrost Part23
                                    BreakerId = (cUnitPart.Breaker != null ? cUnitPart.Part12.IsDeleted ? 0 : cUnitPart.Breaker : 0),
                                    Breaker = (cUnitPart.Part12 != null ? cUnitPart.Part12.IsDeleted ? "NA" : cUnitPart.Part12.Name + "-" + cUnitPart.Part12.Size ?? "" : "NA"),

                                    //Relay Part24
                                    CoilId = (cUnitPart.Coil == null ? 0 : cUnitPart.Part17.IsDeleted ? 0 : cUnitPart.Coil),
                                    Coil = (cUnitPart.Part17 != null ? cUnitPart.Part17.IsDeleted ? "NA" : cUnitPart.Part17.Name + "-" + cUnitPart.Part17.Size ?? "" : "NA"),

                                    MiscId = (cUnitPart.Misc == null ? 0 : cUnitPart.Part18.IsDeleted ? 0 : cUnitPart.Misc),
                                    Misc = (cUnitPart.Part18 != null ? cUnitPart.Part18.IsDeleted ? "NA" : cUnitPart.Part18.Name + "-" + cUnitPart.Part18.Size ?? "" : "NA"),

                                    UnitManuals = cUnitPart.UnitManuals.Select(x => new
                                    {
                                        x.ManualFileName,
                                        ManualURL = ConfigurationManager.AppSettings["unitPartManualsURL"].ToString() + x.ManualFileName
                                    }).ToList()
                                };
                                objUnitParts.Add(objUnitPart);
                            }
                        }
                    }
                    else
                    {
                        foreach (var parts in request.Parts)
                        {
                            foreach (var item in cUParts.ClientUnitParts)
                            {

                                var CheckModelNumber = db.Units.Where(x => x.ModelNumber == parts.ModelNumber && x.Status == true && x.IsDeleted == false).Any();
                                if (CheckModelNumber == true)
                                {
                                    #region ClientUnitPart
                                    var cUnitPart = item;
                                    //var cUnitPart = db.Units.Where(x => x.ModelNumber == item.ModelNumber && x.SerialNumber == item.SerialNumber && x.Status == true && x.IsDeleted == false).FirstOrDefault();
                                    //var cUnitPart = db.Units.Where(x => x.ModelNumber == item.ModelNumber && x.Status == true && x.IsDeleted == false).FirstOrDefault();

                                    if (cUnitPart != null)
                                    {
                                        var objUnitPart = new
                                        {
                                            cUnitPart.ModelNumber,
                                            cUnitPart.SerialNumber,
                                            UnitType2 = (cUParts.UnitType.Id == 2 ? cUnitPart.SplitType : cUParts.UnitType.UnitTypeName),
                                            //UnitType = (cUParts.UnitType.Id == 1 ? "Cooling" : cUParts.UnitType.UnitTypeName),
                                            UnitType = (cUParts.UnitType.Id == 2 ? cUnitPart.SplitType : (cUParts.UnitType.Id == 1 ? "Heating" : "Cooling")),
                                            cUnitPart.ManufactureBrand,
                                            ManufactureDate = (cUnitPart.ManufactureDate != null ? cUnitPart.ManufactureDate.Value.ToString("MMMM, yyyy") : ""),
                                            UnitTon = (cUnitPart.UnitTon != null ? cUnitPart.UnitTon : "NA"),

                                            //Blower Part2
                                            ThermostatId = (cUnitPart.Thermostat == null ? 0 : cUnitPart.Part3.IsDeleted ? 0 : cUnitPart.Thermostat),
                                            Thermostat = (cUnitPart.Part3 != null ? cUnitPart.Part3.IsDeleted ? "NA" : cUnitPart.Part3.Name + "-" + cUnitPart.Part3.Size ?? "" : "NA"),

                                            //Rolloutsensor Part23
                                            BreakerId = (cUnitPart.Breaker == null ? 0 : cUnitPart.Part1.IsDeleted ? 0 : cUnitPart.Breaker),
                                            Breaker = (cUnitPart.Part1 != null ? cUnitPart.Part1.IsDeleted ? "NA" : cUnitPart.Part1.Name + "-" + cUnitPart.Part1.Size ?? "" : "NA"),

                                            // Breaker Part1
                                            BlowerMotorId = (cUnitPart.BlowerMotor == null ? 0 : cUnitPart.Part2.IsDeleted ? 0 : cUnitPart.BlowerMotor),
                                            BlowerMotor = (cUnitPart.Part2 != null ? cUnitPart.Part2.IsDeleted ? "NA" : cUnitPart.Part2.Name + "-" + cUnitPart.Part2.Size ?? "" : "NA"),

                                            // Limitswitch Part17
                                            RefrigerantTypeId = (cUnitPart.RefrigerantType == null ? 0 : cUnitPart.Part20.IsDeleted ? 0 : cUnitPart.RefrigerantType),
                                            Refrigerant = (cUnitPart.Part20 != null ? cUnitPart.Part20.IsDeleted ? "NA" : cUnitPart.Part20.Name + "-" + cUnitPart.Part20.Size ?? "" : "NA"),

                                            // Capacitor Part4
                                            CompressorId = (cUnitPart.Compressor == null ? 0 : cUnitPart.Part6.IsDeleted ? 0 : cUnitPart.Compressor),
                                            Compressor = (cUnitPart.Part6 != null ? cUnitPart.Part6.IsDeleted ? "NA" : cUnitPart.Part6.Name + "-" + cUnitPart.Part6.Size ?? "" : "NA"),

                                            // Booster Part3
                                            CapacitorId = (cUnitPart.Capacitor == null ? 0 : cUnitPart.Part4.IsDeleted ? 0 : cUnitPart.Capacitor),
                                            Capacitor = (cUnitPart.Part4 != null ? cUnitPart.Part4.IsDeleted ? "" : cUnitPart.Part4.Name + "-" + cUnitPart.Part4.Size ?? "" : "NA"),

                                            // Compressor Part6
                                            ContactorId = (cUnitPart.Contactor == null ? 0 : cUnitPart.Part8.IsDeleted ? 0 : cUnitPart.Contactor),
                                            Contactor = (cUnitPart.Part8 != null ? cUnitPart.Part8.IsDeleted ? "" : cUnitPart.Part8.Name + "-" + cUnitPart.Part8.Size ?? "" : "NA"),

                                            // Defrostboard Part10
                                            FilterdryerId = (cUnitPart.Filterdryer == null ? 0 : cUnitPart.Part12.IsDeleted ? 0 : cUnitPart.Filterdryer),
                                            Filterdryer = (cUnitPart.Part12 != null ? cUnitPart.Part12.IsDeleted ? "NA" : cUnitPart.Part12.Name + "-" + cUnitPart.Part12.Size ?? "" : "NA"),

                                            // Contactor Part8
                                            DefrostboardId = (cUnitPart.Defrostboard == null ? 0 : cUnitPart.Part10.IsDeleted ? 0 : cUnitPart.Defrostboard),
                                            Defrostboard = (cUnitPart.Part10 != null ? cUnitPart.Part10.IsDeleted ? "NA" : cUnitPart.Part10.Name + "-" + cUnitPart.Part10.Size ?? "" : "NA"),

                                            //  Misc Part18
                                            RelayId = (cUnitPart.Relay == null ? 0 : cUnitPart.Part21.IsDeleted ? 0 : cUnitPart.Relay),
                                            Relay = (cUnitPart.Part21 != null ? cUnitPart.Part21.IsDeleted ? "NA" : cUnitPart.Part21.Name + "-" + cUnitPart.Part21.Size ?? "" : "NA"),

                                            // ReversingValve Part25
                                            TXVValveId = (cUnitPart.TXVValve == null ? 0 : cUnitPart.Part25.IsDeleted ? 0 : cUnitPart.TXVValve),
                                            TXVValve = (cUnitPart.Part25 != null ? cUnitPart.Part25.IsDeleted ? "NA" : cUnitPart.Part25.Name + "-" + cUnitPart.Part25.Size ?? "" : "NA"),

                                            // Pressureswitch Part19
                                            ReversingValveId = (cUnitPart.ReversingValve == null ? 0 : cUnitPart.Part22.IsDeleted ? 0 : cUnitPart.ReversingValve),
                                            ReversingValve = (cUnitPart.Part25 != null ? cUnitPart.Part22.IsDeleted ? "NA" : cUnitPart.Part22.Name + "-" + cUnitPart.Part22.Size ?? "" : "NA"),

                                            // Coil Part5
                                            CondensingfanmotorId = (cUnitPart.Condensingfanmotor == null ? 0 : cUnitPart.Part7.IsDeleted ? 0 : cUnitPart.Condensingfanmotor),
                                            Condensingfanmotor = (cUnitPart.Part7 != null ? cUnitPart.Part7.IsDeleted ? "NA" : cUnitPart.Part7.Name + "-" + cUnitPart.Part7.Size ?? "" : "NA"),

                                            InducerdraftmotorId = (cUnitPart.Inducerdraftmotor == null ? 0 : cUnitPart.Part.IsDeleted ? 0 : cUnitPart.Inducerdraftmotor),
                                            Inducerdraftmotor = (cUnitPart.Part != null ? cUnitPart.Part.IsDeleted ? "NA" : cUnitPart.Part.Name + "-" + cUnitPart.Part.Size ?? "" : "NA"),

                                            // Relay Part21
                                            TransformerId = (cUnitPart.Transformer == null ? 0 : cUnitPart.Part24.IsDeleted ? 0 : cUnitPart.Transformer),
                                            Transformer = (cUnitPart.Part24 != null ? cUnitPart.Part24.IsDeleted ? "NA" : cUnitPart.Part24.Name + "-" + cUnitPart.Part24.Size ?? "" : "NA"),

                                            // Condensingfanmotor Part7
                                            ControlboardId = (cUnitPart.Controlboard == null ? 0 : cUnitPart.Part9.IsDeleted ? 0 : cUnitPart.Controlboard),
                                            Controlboard = (cUnitPart.Part9 != null ? cUnitPart.Part9.IsDeleted ? "NA" : cUnitPart.Part9.Name + "-" + cUnitPart.Part9.Size ?? "" : "NA"),

                                            // Ignitioncontrolboard Part15
                                            LimitswitchId = (cUnitPart.Limitswitch == null ? 0 : cUnitPart.Part17.IsDeleted ? 0 : cUnitPart.Limitswitch),
                                            Limitswitch = (cUnitPart.Part17 != null ? cUnitPart.Part17.IsDeleted ? "NA" : cUnitPart.Part17.Name + "-" + cUnitPart.Part17.Size ?? "" : "NA"),

                                            // Gasvalve Part14
                                            IgnitorId = (cUnitPart.Ignitor == null ? 0 : cUnitPart.Part16.IsDeleted ? 0 : cUnitPart.Ignitor),
                                            Ignitor = (cUnitPart.Part16 != null ? cUnitPart.Part16.IsDeleted ? "NA" : cUnitPart.Part16.Name + "-" + cUnitPart.Part16.Size ?? "" : "NA"),

                                            // Filterdryer Part12
                                            GasvalveId = (cUnitPart.Gasvalve == null ? 0 : cUnitPart.Part14.IsDeleted ? 0 : cUnitPart.Gasvalve),
                                            Gasvalve = (cUnitPart.Part14 != null ? cUnitPart.Part14.IsDeleted ? "NA" : cUnitPart.Part14.Name + "-" + cUnitPart.Part14.Size ?? "" : "NA"),

                                            // Ignitor Part16
                                            PressureswitchId = (cUnitPart.Pressureswitch == null ? 0 : cUnitPart.Part19.IsDeleted ? 0 : cUnitPart.Pressureswitch),
                                            Pressureswitch = (cUnitPart.Part19 != null ? cUnitPart.Part19.IsDeleted ? "NA" : cUnitPart.Part19.Name + "-" + cUnitPart.Part19.Size ?? "" : "NA"),

                                            // Doorswitch Part13
                                            FlamesensorId = (cUnitPart.Flamesensor == null ? 0 : cUnitPart.Part13.IsDeleted ? 0 : cUnitPart.Flamesensor),
                                            Flamesensor = (cUnitPart.Part13 != null ? cUnitPart.Part13.IsDeleted ? "NA" : cUnitPart.Part13.Name + "-" + cUnitPart.Part13.Size ?? "" : "NA"),

                                            //Refrigerant Part20
                                            RolloutsensorId = (cUnitPart.Rolloutsensor == null ? 0 : cUnitPart.Part23.IsDeleted ? 0 : cUnitPart.Rolloutsensor),
                                            Rolloutsensor = (cUnitPart.Part23 != null ? cUnitPart.Part23.IsDeleted ? "NA" : cUnitPart.Part23.Name + "-" + cUnitPart.Part23.Size ?? "" : "NA"),

                                            // Controlboard Part9
                                            DoorswitchId = (cUnitPart.Doorswitch == null ? 0 : cUnitPart.Part11.IsDeleted ? 0 : cUnitPart.Doorswitch),
                                            Doorswitch = (cUnitPart.Part11 != null ? cUnitPart.Part11.IsDeleted ? "NA" : cUnitPart.Part11.Name + "-" + cUnitPart.Part11.Size ?? "" : "NA"),

                                            //Flamesensor Part13
                                            IgnitioncontrolboardId = (cUnitPart.Ignitioncontrolboard == null ? 0 : cUnitPart.Part15.IsDeleted ? 0 : cUnitPart.Ignitioncontrolboard),
                                            Ignitioncontrolboard = (cUnitPart.Part15 != null ? cUnitPart.Part15.IsDeleted ? "NA" : cUnitPart.Part15.Name + "-" + cUnitPart.Part15.Size ?? "" : "NA"),

                                            ElectricalService = (string.IsNullOrWhiteSpace(cUnitPart.ElectricalService) != true ? cUnitPart.ElectricalService : "NA"),

                                            MaxBreaker = (string.IsNullOrWhiteSpace(cUnitPart.MaxBreaker) != true ? cUnitPart.MaxBreaker : "NA"),

                                            //BreakerId = (cUnitPart.Breaker.HasValue ? (BreakerPart != null ? (BreakerPart.IsDeleted ? 0 : pid) : 0) : 0),
                                            //Breaker = (BreakerPart != null ? BreakerPart.IsDeleted ? "NA" : BreakerPart.Name + "-" + BreakerPart.Size ?? "" : "NA"),


                                            // transformer Part24
                                            CoilId = (cUnitPart.Coil == null ? 0 : cUnitPart.Part5.IsDeleted ? 0 : cUnitPart.Coil),
                                            Coil = (cUnitPart.Part5 != null ? cUnitPart.Part5.IsDeleted ? "NA" : cUnitPart.Part5.Name + "-" + cUnitPart.Part5.Size ?? "" : "NA"),

                                            // TXVValve Part25
                                            MiscId = (cUnitPart.Misc == null ? 0 : cUnitPart.Part18.IsDeleted ? 0 : cUnitPart.Misc),
                                            Misc = (cUnitPart.Part18 != null ? cUnitPart.Part18.IsDeleted ? "NA" : cUnitPart.Part18.Name + "-" + cUnitPart.Part18.Size ?? "" : "NA"),

                                            UnitManuals = cUParts.ClientUnitManuals.AsEnumerable().Where(x => x.SplitType == cUnitPart.SplitType).Select(x => new
                                            {
                                                x.ManualName,
                                                ManualURL = ConfigurationManager.AppSettings["ManualURL"].ToString() + x.ManualName
                                            }).ToList()
                                        };
                                        objUnitParts.Add(objUnitPart);
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                    if (objUnitParts.Count > 0)
                    {
                        if (objUnitParts.Count < request.Parts.Count)
                        {
                            res.StatusCode = HttpStatusCode.PartialContent.GetEnumValue();
                        }
                        else
                        {
                            res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                        }

                        res.Data = objUnitParts;
                        res.Message = "Matched Record Found.";
                    }
                    else
                    {
                        res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                        res.Data = null;
                        res.Message = "No Matched Record Found.";
                    }
                }
            }
            catch (Exception ex)
            {
                res.StatusCode = HttpStatusCode.InternalServerError.GetEnumValue();
                res.Data = null;
                res.Message = "Internal Server Error.";
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
        [Route("GetUnitExtraInfo")]
        public async Task<IHttpActionResult> GetUnitExtraInfo([FromBody] EmpClientUnitModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId).FirstOrDefault();
            if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "Your account was deactivated by Admin.";
                res.Data = null;
            }
            else
            {
                var Unit = db.ClientUnits.Where(x => x.Id == request.UnitId).FirstOrDefault();
                if (Unit != null)
                {

                    List<object> objUnitParts = new List<object>();

                    foreach (var cup in Unit.ClientUnitParts)
                    {
                        List<object> objUnitFilters = new List<object>();
                        List<object> objUnitFuses = new List<object>();
                        var filterExtra = cup.UnitExtraInfoes.Where(x => x.ExtraInfoType == "Filter").ToList();
                        int FilterQty = filterExtra.Count;
                        if (Unit.IsMatched == true)
                        {
                            cup.SplitType = (Unit.UnitType.Id == 2 ? cup.SplitType : (Unit.UnitType.Id == 1 ? "Heating" : "Cooling"));
                        }
                        else
                        {
                            cup.SplitType = (Unit.UnitType.Id == 2 ? cup.SplitType : Unit.UnitType.UnitTypeName);//string.IsNullOrWhiteSpace(cup.SplitType) ? "" : cup.SplitType;
                        }

                        var CheckUnitImageType = cup.SplitType = (Unit.UnitType.Id == 2 ? cup.SplitType : Unit.UnitType.UnitTypeName);
                        // cup.SplitType = (Unit.UnitType.Id == 2 ? cup.SplitType : (Unit.UnitType.Id == 1 ? "Heating" : "Cooling"));
                        foreach (var item in filterExtra)
                        {
                            var d = new
                            {
                                PartId = item.PartId,
                                FilterSize = item.Part.Name + "-" + item.Part.Size ?? "",
                                FilterLocation = item.LocationOfPart.Value
                            };
                            objUnitFilters.Add(d);
                        }

                        var FusesExtra = cup.UnitExtraInfoes.Where(x => x.ExtraInfoType == "Fuses").ToList();
                        int FusesQty = FusesExtra.Count;

                        foreach (var item in FusesExtra)
                        {
                            var d = new
                            {
                                PartId = item.PartId,
                                FuseType = item.Part.Name + "-" + item.Part.Size ?? ""
                            };
                            objUnitFuses.Add(d);
                        }
                        var objUnitPart = new
                        {
                            UnitType = cup.SplitType,
                            Notes = string.IsNullOrEmpty(cup.ClientUnit.Notes) ? "" : cup.ClientUnit.Notes,
                            FilterQty = objUnitFilters.Count,
                            Filters = objUnitFilters,
                            FusesQty = objUnitFuses.Count,
                            Fuses = objUnitFuses,
                            ThermostatId = (cup.Thermostat.HasValue ? cup.Thermostat : 0),
                            Thermostat = (cup.Part3 != null ? cup.Part3.Name + "-" + cup.Part3.Size ?? "" : "NA"),
                            UnitPictures = db.ClientUnitPictures.Where(x => x.ClientUnitId == Unit.Id && x.SplitType == CheckUnitImageType).AsEnumerable().Select(x => new
                            {
                                UnitImageUrl = (x.UnitImage != null ? ConfigurationManager.AppSettings["UnitImageURL"].ToString() + x.UnitImage : ""),

                            }).ToList()
                        };
                        objUnitParts.Add(objUnitPart);
                    }
                    if (objUnitParts.Count > 0)
                    {
                        res.Data = objUnitParts;
                        res.Message = "Data Found";
                        res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                    }
                    else
                    {
                        res.Data = null;
                        res.Message = "No Data Found";
                        res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                    }
                }
                else
                {
                    res.Data = null;
                    res.Message = "No Data Found";
                    res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                }
            }
            db.Dispose();
            return Ok(res);
        }

        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("UpdateUnitDetails")]
        public async Task<IHttpActionResult> UpdateUnitDetails([FromBody] EmpClientUnitModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            try
            {
                var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId).FirstOrDefault();
                if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    var Unit = db.ClientUnits.Where(x => x.Id == request.UnitId).FirstOrDefault();
                    var deleteUnitParts = db.ClientUnitParts.Where(x => x.UnitId == Unit.Id).ToList();
                    foreach (var item in deleteUnitParts)
                    {
                        db.UnitExtraInfoes.RemoveRange(item.UnitExtraInfoes);
                    }
                    if (Unit.ClientUnitManuals.Count > 0)
                    {
                        db.ClientUnitManuals.RemoveRange(Unit.ClientUnitManuals);
                    }
                    db.ClientUnitParts.RemoveRange(deleteUnitParts);
                    db.SaveChanges();

                    if (Unit != null)
                    {
                        foreach (var ept in request.Parts)
                        {
                            Unit mUnit = new Unit();

                            if (!ept.IsMatched)
                            {
                                mUnit = AutoMapper.Mapper.Map<Unit>(ept);
                                mUnit.ManufactureDate = ept.ManufactureDate;
                                mUnit.AddedBy = request.EmployeeId;
                                mUnit.AddedByType = api.App_Start.Utilities.UserRoles.Employee.GetEnumValue();
                                mUnit.AddedDate = DateTime.UtcNow;
                                mUnit.Status = true;
                                db.Units.Add(mUnit);
                                db.SaveChanges();
                            }

                            var unitparts = db.ClientUnitParts.Where(x => x.UnitId == Unit.Id && x.SplitType == ept.UnitType).FirstOrDefault();
                            if (unitparts == null)
                            {
                                unitparts = AutoMapper.Mapper.Map<ClientUnitPart>(ept);
                                unitparts.MaxBreaker = ept.MaxBreaker;
                                unitparts.UnitId = Unit.Id;
                                unitparts.ManufactureDate = ept.ManufactureDate;
                                db.ClientUnitParts.Add(unitparts);
                                db.SaveChanges();
                                if (ept.OptionalInformation != null)
                                {
                                    var oi = ept.OptionalInformation;
                                    if (oi.Filters.Count > 0)
                                    {
                                        var oldUnitExtraInfoes = db.UnitExtraInfoes.Where(x => x.ClientUnitPartId == unitparts.Id && x.ExtraInfoType == "Filter").ToList();
                                        db.UnitExtraInfoes.RemoveRange(oldUnitExtraInfoes);
                                        foreach (var uFilter in oi.Filters)
                                        {
                                            UnitExtraInfo uei = new UnitExtraInfo();
                                            uei.UnitId = Unit.Id;
                                            uei.PartId = uFilter.size;
                                            uei.ClientUnitPartId = unitparts.Id;
                                            uei.LocationOfPart = uFilter.LocationOfPart;
                                            uei.ExtraInfoType = "Filter";
                                            db.UnitExtraInfoes.Add(uei);
                                        }
                                        db.SaveChanges();
                                    }
                                    if (oi.FuseTypes.Count > 0)
                                    {
                                        var oldUnitExtraInfoes = db.UnitExtraInfoes.Where(x => x.ClientUnitPartId == unitparts.Id && x.ExtraInfoType == "Fuses").ToList();
                                        db.UnitExtraInfoes.RemoveRange(oldUnitExtraInfoes);
                                        foreach (var uFuseType in oi.FuseTypes)
                                        {
                                            UnitExtraInfo uei = new UnitExtraInfo();
                                            uei.UnitId = Unit.Id;
                                            uei.ClientUnitPartId = unitparts.Id;
                                            uei.PartId = uFuseType.FuseType;
                                            uei.ExtraInfoType = "Fuses";
                                            db.UnitExtraInfoes.Add(uei);
                                        }
                                        db.SaveChanges();
                                    }
                                    unitparts.Thermostat = (oi.ThermostatTypes == 0 ? null : oi.ThermostatTypes);
                                }
                            }
                        }
                        if (request.Parts.Count > 0)
                        {
                            if (request.Parts.Count == 1)
                            {
                                var op = request.Parts.FirstOrDefault();
                                if (op.UnitType.ToLower() == "packaged")
                                {
                                    Unit.UnitTypeId = 1;
                                }
                                else if (op.UnitType.ToLower() == "heating")
                                {
                                    Unit.UnitTypeId = 3;
                                }
                            }
                            else if (request.Parts.Count == 2)
                            {
                                Unit.UnitTypeId = 2;
                            }
                        }
                        Unit.Notes = request.Notes;
                        Unit.IsMatched = true;
                        Unit.UpdatedBy = request.EmployeeId;
                        Unit.UpdatedByType = api.App_Start.Utilities.UserRoles.Employee.GetEnumValue();
                        Unit.UpdatedDate = DateTime.UtcNow;
                        db.SaveChanges();
                        res.Message = "Unit Updated";
                        res.Data = new
                        {
                            UnitId = Unit.Id,
                            Unit.ClientId
                        };
                        res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                    }
                }
            }
            catch (Exception ex)
            {
                res.Message = "Internal Server Error";
                res.Data = null;
                res.StatusCode = HttpStatusCode.InternalServerError.GetEnumValue();
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
        [Route("AddClientUnit")]
        public async Task<IHttpActionResult> AddClientUnit([FromBody] EmpClientUnitModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            try
            {
                var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId).FirstOrDefault();
                if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    var Unit = db.ClientUnits.Where(x => x.Id == request.UnitId).FirstOrDefault();
                    if (Unit != null)
                    {
                        foreach (var ept in request.Parts)
                        {
                            var unitparts = db.ClientUnitParts.Where(x => x.UnitId == Unit.Id && x.SplitType == ept.UnitType).FirstOrDefault();
                            unitparts = AutoMapper.Mapper.Map<ClientUnitPart>(ept);
                            unitparts.MaxBreaker = (ept.MaxBreaker == null ? null : ept.MaxBreaker.ToString());
                            db.SaveChanges();
                        }
                        Unit.UpdatedBy = request.EmployeeId;
                        Unit.UpdatedByType = api.App_Start.Utilities.UserRoles.Employee.GetEnumValue();
                        Unit.UpdatedDate = DateTime.UtcNow;

                        res.Message = "Unit Updated";
                        res.Data = null;
                        res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                    }
                }
            }
            catch (Exception ex)
            {
                res.Message = "Internal Server Error";
                res.Data = null;
                res.StatusCode = HttpStatusCode.InternalServerError.GetEnumValue();
            }
            db.Dispose();
            return Ok(res);
        }

        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("SubmitPartOrder")]
        public Task<IHttpActionResult> SubmitPartOrder()
        {
            ResponseModel res = new ResponseModel();

            var uploadPath = new DirectoryInfo(HttpContext.Current.Server.MapPath("../../")).Parent.FullName + ConfigurationManager.AppSettings["ClientSignaturePath"].ToString();//HttpContext.Current.Server.MapPath("~/Uploads");

            var multipartFormDataStreamProvider = new UploadMultipartFormProvider(uploadPath);
            var task = Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProvider).
            ContinueWith<IHttpActionResult>(t =>
            {
                int cId = 0;
                var StripeError = false;
                db = new Aircall_DBEntities1();
                Order order = new Order();
                try
                {
                    if (updatetoken)
                    {
                        res.Token = accessToken;
                    }
                    else
                    {
                        res.Token = "";
                    }
                    string EmployeeId = multipartFormDataStreamProvider.FormData.GetValues("EmployeeId").FirstOrDefault();
                    var eid = int.Parse(EmployeeId);
                    var EmpInfo = db.Employees.Where(x => x.Id == eid).FirstOrDefault();
                    if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
                    {
                        res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        res.Message = "Your account was deactivated by Admin.";
                        res.Data = null;
                    }
                    else
                    {
                        string ClientId = multipartFormDataStreamProvider.FormData.GetValues("ClientId").FirstOrDefault();
                        cId = int.Parse(ClientId);
                        string Address = multipartFormDataStreamProvider.FormData.GetValues("Address").FirstOrDefault();
                        string City = multipartFormDataStreamProvider.FormData.GetValues("City").FirstOrDefault();
                        string State = multipartFormDataStreamProvider.FormData.GetValues("State").FirstOrDefault();
                        string ZipCode = multipartFormDataStreamProvider.FormData.GetValues("ZipCode").FirstOrDefault();
                        string Email = multipartFormDataStreamProvider.FormData.GetValues("Email").FirstOrDefault();
                        string ChargeBy = multipartFormDataStreamProvider.FormData.GetValues("ChargeBy").FirstOrDefault();
                        string Company = multipartFormDataStreamProvider.FormData.GetValues("Company").FirstOrDefault();
                        string Recommendation = multipartFormDataStreamProvider.FormData.GetValues("Recommendation").FirstOrDefault();
                        string EmailToClientEmail = multipartFormDataStreamProvider.FormData.GetValues("EmailToClientEmail").FirstOrDefault();
                        string CCEmail = "";
                        try
                        {
                            CCEmail = multipartFormDataStreamProvider.FormData.GetValues("CCEmail").FirstOrDefault();
                        }
                        catch (Exception ex)
                        {
                        }
                        string ChequeNo = "";
                        try
                        {
                            ChequeNo = multipartFormDataStreamProvider.FormData.GetValues("ChequeNo").FirstOrDefault();
                        }
                        catch (Exception ex)
                        {

                        }
                        string AccountingNotes = "";
                        try
                        {
                            AccountingNotes = multipartFormDataStreamProvider.FormData.GetValues("AccountingNotes").FirstOrDefault();
                        }
                        catch (Exception ex)
                        {
                        }

                        string ChequeDate = "";
                        try
                        {
                            ChequeDate = multipartFormDataStreamProvider.FormData.GetValues("ChequeDate").FirstOrDefault();
                        }
                        catch (Exception ex)
                        {
                        }
                        string CardType = "";
                        try
                        {
                            CardType = multipartFormDataStreamProvider.FormData.GetValues("CardType").FirstOrDefault();
                        }
                        catch (Exception ex)
                        {

                        }
                        string NameOnCard = "";
                        try
                        {
                            NameOnCard = multipartFormDataStreamProvider.FormData.GetValues("NameOnCard").FirstOrDefault();
                        }
                        catch (Exception ex)
                        {
                        }
                        string CardNumber = "";
                        try
                        {
                            CardNumber = multipartFormDataStreamProvider.FormData.GetValues("CardNumber").FirstOrDefault();
                        }
                        catch (Exception ex)
                        {
                        }
                        string CVV = "";
                        try
                        {
                            CVV = multipartFormDataStreamProvider.FormData.GetValues("CVV").FirstOrDefault();
                        }
                        catch (Exception ex)
                        {

                        }
                        string ExpiryMonth = "";
                        try
                        {
                            ExpiryMonth = multipartFormDataStreamProvider.FormData.GetValues("ExpiryMonth").FirstOrDefault();
                        }
                        catch (Exception ex)
                        {
                        }
                        string ExpiryYear = "";
                        try
                        {
                            ExpiryYear = multipartFormDataStreamProvider.FormData.GetValues("ExpiryYear").FirstOrDefault();
                        }
                        catch (Exception ex)
                        {
                        }

                        if (ChargeBy.ToLower() == api.App_Start.Utilities.ChargeBy.NewCC.GetEnumDescription().ToLower())
                        {
                            String s1 = CardNumber.Substring(CardNumber.Length - 4);
                            s1 = s1.PadLeft(16, '*');
                            var cnt = db.ClientPaymentMethods.Where(x => x.CardNumber == s1 && x.ClientId == cId).Count();
                            if (cnt > 0)
                            {
                                res.StatusCode = (int)HttpStatusCode.Ambiguous;
                                res.Message = "Card already exists, go back and select CC on file";
                                res.Data = null;
                                db.Dispose();
                                return Ok(res);
                            }
                        }
                        string Parts = multipartFormDataStreamProvider.FormData.GetValues("OrderItems").FirstOrDefault();
                        if ((CardNumber == null || ExpiryMonth == null || ExpiryYear == null || NameOnCard == null) && ChargeBy.ToLower() != api.App_Start.Utilities.ChargeBy.Check.GetEnumDescription().ToLower())
                        {
                            res.StatusCode = HttpStatusCode.BadRequest.GetEnumValue();
                            res.Message = "Invalid Request";
                            res.Data = "";
                            db.Dispose();
                            return Ok(res);
                        }
                        else
                        {

                            //s = s.PadLeft(16, '*');

                            var PartsList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OrderPartModel>>(Parts);

                            var Amount = 0m;

                            Order ord = new Order();
                            ord.ClientId = int.Parse(ClientId);
                            ord.OrderType = api.App_Start.Utilities.BillingTypes.FixedCost.GetEnumDescription();
                            ord.ChargeBy = ChargeBy;
                            ord.CCEmail = CCEmail;
                            ord.CustomerRecommendation = Recommendation;
                            ord.AddedBy = int.Parse(EmployeeId);
                            ord.AddedByType = api.App_Start.Utilities.UserRoles.Employee.GetEnumValue();
                            ord.AddedDate = DateTime.UtcNow;
                            ord.IsDeleted = false;
                            var filename = multipartFormDataStreamProvider.FileData.Where(x => x.Headers.ContentDisposition.Name.ToLower().Contains("clientsignature")).Select(multiPartData => multiPartData.LocalFileName).FirstOrDefault();
                            if (string.IsNullOrWhiteSpace(filename))
                            {
                                ord.ClientSignature = "";
                            }
                            else
                            {
                                ord.ClientSignature = new FileInfo(filename).Name;
                            }

                            foreach (var item in PartsList)
                            {
                                var part = db.Parts.Find(item.PartId);
                                Amount = Amount + (part.SellingPrice.Value * item.Quantity);
                                ord.OrderItems.Add(new OrderItem()
                                {
                                    Amount = part.SellingPrice,
                                    PartId = part.Id,
                                    PartSize = part.Size ?? "",
                                    Quantity = item.Quantity,
                                    PartName = part.Name
                                });
                            }
                            ord.OrderAmount = Amount;
                            var UserInfo = db.Clients.Find(ord.ClientId);
                            var cords = db.Orders.Where(x => x.ClientId == UserInfo.Id).Count();
                            var ordernumber = UserInfo.AccountNumber.ToString() + "-" + ZipCode + "-O" + (cords + 1).ToString();
                            ord.OrderNumber = ordernumber;
                            var ClientCC = new ClientPaymentMethod();
                            if (ChargeBy.ToLower() == api.App_Start.Utilities.ChargeBy.Check.GetEnumDescription().ToLower())
                            {
                                var ChqueImageFront = multipartFormDataStreamProvider.FileData.Where(x => x.Headers.ContentDisposition.Name.ToLower().Contains("chqueimagefront")).Select(multiPartData => multiPartData.LocalFileName).FirstOrDefault();
                                if (string.IsNullOrWhiteSpace(ChqueImageFront))
                                {
                                    ord.ChqueImageFront = "";
                                }
                                else
                                {
                                    File.Move(ChqueImageFront, System.Web.Hosting.HostingEnvironment.MapPath("/") + ConfigurationManager.AppSettings["CheckImagePath"].ToString() + new FileInfo(ChqueImageFront).Name);
                                    ord.ChqueImageFront = new FileInfo(ChqueImageFront).Name;
                                }

                                var ChequeImageBack = multipartFormDataStreamProvider.FileData.Where(x => x.Headers.ContentDisposition.Name.ToLower().Contains("chequeimageback")).Select(multiPartData => multiPartData.LocalFileName).FirstOrDefault();
                                if (string.IsNullOrWhiteSpace(ChequeImageBack))
                                {
                                    ord.ChequeImageBack = "";
                                }
                                else
                                {
                                    File.Move(ChequeImageBack, System.Web.Hosting.HostingEnvironment.MapPath("/") + ConfigurationManager.AppSettings["CheckImagePath"].ToString() + new FileInfo(ChequeImageBack).Name);
                                    ord.ChequeImageBack = new FileInfo(ChequeImageBack).Name;
                                }

                                ord.ChequeNo = ChequeNo;
                                ord.ChequeDate = DateTime.Parse(ChequeDate);
                                ord.AccountingNotes = AccountingNotes;
                            }
                            else if (ChargeBy.ToLower() == api.App_Start.Utilities.ChargeBy.NewCC.GetEnumDescription().ToLower())
                            {
                                string s = "";
                                s = CardNumber.Substring(CardNumber.Length - 4).PadLeft(16, '*');
                                ord.CardNumber = s;
                                ord.NameOnCard = NameOnCard;
                                ord.ExpirationMonth = short.Parse(ExpiryMonth);
                                ord.ExpirationYear = int.Parse(ExpiryYear);
                                ord.CardType = CardType;

                                ClientCC = new ClientPaymentMethod();
                                ClientCC.CardNumber = s;
                                ClientCC.IsDefaultPayment = false;
                                ClientCC.ExpiryYear = int.Parse(ExpiryYear);
                                ClientCC.ExpiryMonth = short.Parse(ExpiryMonth);
                                ClientCC.NameOnCard = NameOnCard;
                                ClientCC.ClientId = ord.ClientId;
                                ClientCC.AddedBy = EmpInfo.Id;
                                ClientCC.AddedByType = api.App_Start.Utilities.UserRoles.Employee.GetEnumValue();
                                ClientCC.AddedDate = DateTime.UtcNow;
                                //var myCard = new StripeCardCreateOptions();
                                //myCard.SourceCard = new SourceCard()
                                //{
                                //    Number = CardNumber.ToString(),
                                //    ExpirationYear = ExpiryYear.ToString(),
                                //    ExpirationMonth = ExpiryMonth.ToString(),
                                //    Name = NameOnCard,
                                //    Cvc = CVV.ToString()
                                //};

                                //var cardService = new StripeCardService();
                                if (string.IsNullOrWhiteSpace(UserInfo.CustomerProfileId))
                                {
                                    try
                                    {
                                        var objClientService = Services.ServiceFactory.ClientService;
                                        string customerProfileId = "";
                                        var errCode = "";
                                        var errText = "";
                                        var email = UserInfo.Email;
                                        var description = UserInfo.FirstName + ' ' + UserInfo.LastName;
                                        var ret = objClientService.AddClientToAuthorizeNet(email, description, ref customerProfileId, ref errCode, ref errText);

                                        UserInfo.CustomerProfileId = customerProfileId;
                                    }
                                    catch (Exception ex)
                                    {
                                        //StripeErrorLog err = Mapper.Map<StripeErrorLog>(stex.StripeError);
                                        //err.Userid = UserInfo.Id;
                                        //db.StripeErrorLogs.Add(err);
                                    }
                                }

                                var objClientService2 = Services.ServiceFactory.ClientService;
                                var errCode2 = "";
                                var errText2 = "";
                                string customerPaymentProfileId = "";
                                string expirationDate = ExpiryMonth.ToString().PadLeft(2, '0') + (ExpiryYear.ToString().Length > 2 ? ExpiryYear.ToString().Substring(2, 2) : ExpiryYear.ToString());
                                var ret2 = objClientService2.CreatePaymentProfile(UserInfo.FirstName, UserInfo.LastName, UserInfo.CustomerProfileId, CardNumber, expirationDate, CVV.ToString(), ref customerPaymentProfileId, ref errCode2, ref errText2);

                                //StripeCard stripeCard = cardService.Create(UserInfo.StripeCustomerId, myCard);
                                ClientCC.CustomerPaymentProfileId = customerPaymentProfileId;
                                ClientCC.CardType = CardType;
                                db.ClientPaymentMethods.Add(ClientCC);
                                db.SaveChanges();
                            }
                            else
                            {
                                string s = "";
                                s = CardNumber.Substring(CardNumber.Length - 4).PadLeft(16, '*');
                                ord.CardNumber = s;
                                ord.NameOnCard = NameOnCard;
                                ord.ExpirationMonth = short.Parse(ExpiryMonth);
                                ord.ExpirationYear = int.Parse(ExpiryYear);
                                ord.CardType = CardType;
                                ClientCC = db.ClientPaymentMethods.Where(x => x.ClientId == ord.ClientId && x.CardNumber == s).FirstOrDefault();

                                if (string.IsNullOrWhiteSpace(ClientCC.CustomerPaymentProfileId))
                                {
                                    var myCard = new StripeCardCreateOptions();
                                    myCard.SourceCard = new SourceCard()
                                    {
                                        Number = CardNumber.ToString(),
                                        ExpirationYear = ExpiryYear.ToString(),
                                        ExpirationMonth = ExpiryMonth.ToString(),
                                        Name = NameOnCard,
                                        Cvc = CVV.ToString()
                                    };

                                    var cardService = new StripeCardService();
                                    if (string.IsNullOrWhiteSpace(UserInfo.CustomerProfileId))
                                    {
                                        try
                                        {
                                            var myCustomer = new StripeCustomerCreateOptions();

                                            myCustomer.Email = UserInfo.Email;
                                            myCustomer.Description = UserInfo.FirstName + ' ' + UserInfo.LastName + " (" + UserInfo.Email + ")";
                                            var customerService = new StripeCustomerService();
                                            StripeCustomer stripeCustomer = customerService.Create(myCustomer);
                                            UserInfo.CustomerProfileId = stripeCustomer.Id;
                                        }
                                        catch (StripeException stex)
                                        {
                                            StripeErrorLog err = Mapper.Map<StripeErrorLog>(stex.StripeError);
                                            err.Userid = UserInfo.Id;
                                            db.StripeErrorLogs.Add(err);
                                        }
                                    }
                                    StripeCard stripeCard = cardService.Create(UserInfo.CustomerProfileId, myCard);
                                    ClientCC.CustomerPaymentProfileId = stripeCard.Id;
                                    ClientCC.CardType = stripeCard.Brand;
                                    db.SaveChanges();
                                }
                            }
                            BillingHistory bh = new BillingHistory();
                            bh.BillingAddress = Address;
                            bh.BillingCity = int.Parse(City);
                            bh.BillingState = int.Parse(State);
                            bh.ClientId = int.Parse(ClientId);
                            bh.BillingZipcode = ZipCode;
                            bh.BillingFirstName = UserInfo.FirstName;
                            bh.BillingLastName = UserInfo.LastName;
                            bh.Company = Company;
                            bh.BillingType = api.App_Start.Utilities.BillingTypes.FixedCost.GetEnumDescription();
                            bh.IsSpecialOffer = false;
                            bh.OriginalAmount = Amount;
                            bh.PurchasedAmount = Amount;
                            bh.IsPaid = true;
                            bh.failcode = "";
                            bh.faildesc = "Payment Success!";
                            var StripeErrMsg = "";
                            if (ChargeBy.ToLower() != api.App_Start.Utilities.ChargeBy.Check.GetEnumDescription().ToLower())
                            {
                                var Description = "Charge for Parts Purchase";
                                var sr = api.App_Start.Utilities.StripeCharge(true, "", UserInfo.CustomerProfileId, ClientCC.CustomerPaymentProfileId, (int)(Amount * 100), Description, " Parts Purchase", db, int.Parse(ClientId), 0);
                                bh.TransactionId = sr.TransactionId;
                                if (sr.ex != null)
                                {
                                    StripeError = true;
                                }
                                StripeErrMsg = sr.ErrorMessage;
                                bh.TransactionDate = DateTime.UtcNow;
                            }
                            else
                            {
                                bh.TransactionId = ChequeNo;
                                bh.TransactionDate = DateTime.Parse(ChequeDate);
                                StripeError = false;
                            }
                            if (!StripeError)
                            {
                                ord.BillingHistories.Add(bh);
                                db.Orders.Add(ord);
                                db.SaveChanges();
                                order = ord;
                            }
                            else
                            {
                                res.Message = StripeErrMsg;
                                res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                                res.Data = null;
                            }
                        }
                        var eId = int.Parse(EmployeeId);
                        var Orders = db.Orders.Where(x => x.Id == order.Id).ToList();
                        List<object> data = new List<object>();
                        foreach (Order item in Orders)
                        {
                            var d = new
                            {
                                OrderId = item.Id,
                                item.OrderNumber,
                                ChargeBy = (string.IsNullOrWhiteSpace(item.ChargeBy) ? "CC on File" : item.ChargeBy),
                                ClientName = item.Client.FirstName + " " + item.Client.LastName,
                                item.Client.Email,
                                CardType = CardType,
                                Parts = item.OrderItems.Select(x => new
                                {
                                    x.PartName,
                                    x.PartSize,
                                    x.Quantity,
                                    x.Amount
                                }).ToList(),
                                Total = item.OrderAmount
                            };
                            data.Add(d);
                        }
                        if (data.Count > 0)
                        {
                            res.Data = data.FirstOrDefault();
                            if (StripeError)
                            {
                                res.Message = "Payment Failed";
                                res.StatusCode = HttpStatusCode.BadGateway.GetEnumValue();
                            }
                            else
                            {
                                res.Message = "Order Placed";
                                res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                                try
                                {
                                    var orderMail = Orders.FirstOrDefault();
                                    EmailTemplate templateclient = db.EmailTemplates.Where(x => x.Name == "PartOrderReceiptChequeClient" && x.Status == true).FirstOrDefault();
                                    var strclient = templateclient.EmailBody;
                                    strclient = strclient.Replace("{{FirstName}}", orderMail.Client.FirstName);
                                    strclient = strclient.Replace("{{LastName}}", orderMail.Client.LastName);
                                    strclient = strclient.Replace("{{Email}}", orderMail.Client.Email);
                                    strclient = strclient.Replace("{{PhoneNumber}}", orderMail.Client.MobileNumber);
                                    strclient = strclient.Replace("{{OrderDate}}", orderMail.AddedDate.ToString("MM/dd/yyyy"));
                                    var stritems = "";
                                    var total = 0m;
                                    for (int i = 0; i < orderMail.OrderItems.Count; i++)
                                    {
                                        var item = orderMail.OrderItems.ToList()[i];
                                        total += item.Quantity * item.Amount.Value;
                                        stritems = stritems + "<tr><td>" + (i + 1).ToString() + "</td><td>" + item.PartName + " - " + item.PartSize + "</td><td>" + item.Quantity + "</td><td align='right'>" + item.Amount + "</td><td align='right'>" + item.Quantity * item.Amount.Value + "</td></tr>";
                                    }
                                    stritems = "<table style='width:100%;font-family:Arial,sans-serif;font-size:14px;line-height:21px;color:#333;border-color: #e2e2e2;border-collapse: collapse;'><tr><td>SR</td><td>Part</td><td>Qty</td><td>Rate</td><td>Total</td></tr>" + stritems + "<tr><td colspan='4' align='right'>Total</td><td align='right'>$" + total.ToString("0.00") + "</td></tr></table>";
                                    strclient = strclient.Replace("{{orderitems}}", stritems);
                                    if (EmailToClientEmail.ToLower() == "1")
                                    {
                                        api.App_Start.Utilities.Send(templateclient.EmailTemplateSubject, orderMail.Client.Email, strclient, templateclient.FromEmail, db, "", CCEmail);
                                    }
                                }
                                catch (Exception ex1)
                                {
                                }
                                var ClientNotification = db.NotificationMasters.Where(x => x.Name == "PartPurchasedSendToClient").FirstOrDefault();
                                var message = ClientNotification.Message;
                                message = message.Replace("{{EmployeeName}}", EmpInfo.FirstName + " " + EmpInfo.LastName);
                                var cid = int.Parse(ClientId);
                                var UserInfo = db.Clients.Find(cid);
                                var billingId = Orders.FirstOrDefault().BillingHistories.FirstOrDefault().Id;
                                UserNotification objUserNotification = new UserNotification();
                                objUserNotification.UserId = UserInfo.Id;
                                objUserNotification.UserTypeId = api.App_Start.Utilities.UserRoles.Client.GetEnumValue();
                                objUserNotification.Message = message;
                                objUserNotification.CommonId = billingId;
                                objUserNotification.Status = api.App_Start.Utilities.NotificationStatus.UnRead.GetEnumDescription();
                                objUserNotification.MessageType = api.App_Start.Utilities.NotificationType.PartPurchased.GetEnumDescription();
                                objUserNotification.AddedDate = DateTime.UtcNow;
                                db.UserNotifications.Add(objUserNotification);
                                db.SaveChanges();

                                var BadgeCount = db.uspa_ClientPortal_GetNotificationForDashBoardByUserType(UserInfo.Id, api.App_Start.Utilities.UserRoles.Client.GetEnumValue(), "", 0).AsEnumerable().Where(x => x.Status == api.App_Start.Utilities.NotificationStatus.UnRead.GetEnumDescription()).Count();
                                Notifications objNotifications = new Notifications { NId = objUserNotification.Id, NType = api.App_Start.Utilities.NotificationType.PartPurchased.GetEnumValue(), CommonId = objUserNotification.CommonId.Value };
                                List<NotificationModel> notify = new List<NotificationModel>();
                                notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                                notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                                notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });

                                if (UserInfo.DeviceType != null && UserInfo.DeviceToken != null)
                                {
                                    if (UserInfo.DeviceType.ToLower() == "android")
                                    {
                                        string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType + "&data.CommonId=" + objNotifications.CommonId;
                                        SendNotifications.SendAndroidNotification(UserInfo.DeviceToken, message, CustomData, "client");
                                    }
                                    else if (UserInfo.DeviceType.ToLower() == "iphone")
                                    {
                                        SendNotifications.SendIphoneNotification(BadgeCount, UserInfo.DeviceToken, message, notify, "client", HttpContext.Current);
                                    }
                                }
                            }
                        }
                        else
                        {
                            res.Message = "No record found";
                            res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                            res.Data = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    res.Message = ex.Message;
                    res.StatusCode = HttpStatusCode.InternalServerError.GetEnumValue();
                    res.Data = null;
                }
                db.Dispose();
                return Ok(res);
            });
            return task;
        }

        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpGet]
        [Route("GetServiceRatingReviewList")]
        public async Task<IHttpActionResult> GetServiceRatingReviewList([FromUri]int EmployeeId)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            List<RatingReviewModel> data = new List<RatingReviewModel>();
            decimal Rating = 0;
            string Status = api.App_Start.Utilities.ServiceTypes.Completed.GetEnumDescription();
            var EmpInfo = db.Employees.Find(EmployeeId);
            var service = EmpInfo.Services.Where(x => x.Status == Status && x.ServiceRatingReviews.Count > 0).ToList();
            if (service.Count > 0)
            {
                foreach (var item in service)
                {
                    var planTypeId = item.ServiceUnits.FirstOrDefault().ClientUnit.PlanTypeId;
                    var planName = db.SubscriptionPlans.FirstOrDefault(p => p.Id == planTypeId).PlanName;
                    RatingReviewModel d = new RatingReviewModel
                    {
                        RatingReviewId = item.ServiceRatingReviews.FirstOrDefault().Id,
                        ClientName = item.Client.FirstName + " " + item.Client.LastName,
                        ServiceCaseNumber = item.ServiceCaseNumber,
                        PackageName = planName,
                        Rating = Convert.ToDecimal(item.ServiceRatingReviews.FirstOrDefault().Rate),
                        Review = item.ServiceRatingReviews.FirstOrDefault().Review,
                        EmpNotes = string.IsNullOrEmpty(item.ServiceRatingReviews.FirstOrDefault().EmployeNotes) ? "" : item.ServiceRatingReviews.FirstOrDefault().EmployeNotes
                    };
                    Rating = Rating + Convert.ToDecimal(d.Rating);
                    data.Add(d);
                }
                RatingReviewListModel objReturn = new RatingReviewListModel();
                objReturn.AverageRating = Rating / service.Count();
                objReturn.RatingReview = data;
                res.Data = objReturn;
                res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                res.Message = "Records Found";
            }
            else
            {
                res.Data = null;
                res.Message = "No Rating Found";
                res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
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
        [Route("GetServiceRatingReviewDetail")]
        public async Task<IHttpActionResult> GetServiceRatingReviewDetail([FromBody]EmpCommonModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            List<RatingReviewModel> data = new List<RatingReviewModel>();
            decimal Rating = 0;
            string Status = api.App_Start.Utilities.ServiceTypes.Completed.GetEnumDescription();
            var EmpInfo = db.Employees.Find(request.EmployeeId);
            //var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId && x.IsDeleted == false).FirstOrDefault();
            if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "Your account was deactivated by Admin.";
                res.Data = null;
            }
            else
            {
                var service = EmpInfo.Services.Where(x => x.Status == Status && x.ServiceRatingReviews.Count > 0 && x.Id == request.ServiceId).ToList();
                if (request.NotificationId > 0)
                {
                    var n = db.UserNotifications.Find(request.NotificationId);
                    if (n != null)
                    {
                        n.Status = api.App_Start.Utilities.NotificationStatus.Read.GetEnumDescription();
                        db.SaveChanges();
                    }
                }
                if (service.Count > 0)
                {
                    foreach (var item in service)
                    {
                        var planTypeId = item.ServiceUnits.FirstOrDefault().ClientUnit.PlanTypeId;
                        var planName = db.SubscriptionPlans.FirstOrDefault(p => p.Id == planTypeId).PlanName;

                        RatingReviewModel d = new RatingReviewModel
                        {
                            RatingReviewId = item.ServiceRatingReviews.FirstOrDefault().Id,
                            ClientName = item.Client.FirstName + " " + item.Client.LastName,
                            ServiceCaseNumber = item.ServiceCaseNumber,
                            PackageName = planName,
                            Rating = Convert.ToDecimal(item.ServiceRatingReviews.FirstOrDefault().Rate),
                            Review = item.ServiceRatingReviews.FirstOrDefault().Review,
                            EmpNotes = string.IsNullOrEmpty(item.ServiceRatingReviews.FirstOrDefault().EmployeNotes) ? "" : item.ServiceRatingReviews.FirstOrDefault().EmployeNotes
                        };
                        Rating = Rating + Convert.ToDecimal(d.Rating);
                        data.Add(d);
                    }
                    RatingReviewListModel objReturn = new RatingReviewListModel();
                    objReturn.AverageRating = Rating / service.Count();
                    objReturn.RatingReview = data;
                    res.Data = data.FirstOrDefault();
                    res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                    res.Message = "Records Found";
                }
                else
                {
                    res.Data = null;
                    res.Message = "No Rating Found";
                    res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
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
        [Route("GetOrderList")]
        public async Task<IHttpActionResult> GetOrderList([FromBody] EmpCommonModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            var EmpInfo = db.Employees.Find(request.EmployeeId);
            var Orders = db.Orders.Where(x => x.AddedBy == request.EmployeeId && x.AddedByType == 5 && x.BillingHistories.Count > 0 && (x.IsDeleted == false || x.IsDeleted.HasValue == false)).ToList();
            if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "Your account was deactivated by Admin.";
                res.Data = null;
            }
            else
            {
                List<object> data = new List<object>();
                foreach (Order item in Orders)
                {
                    var d = new
                    {
                        OrderId = item.Id,
                        item.OrderNumber,
                        ChargeBy = (string.IsNullOrWhiteSpace(item.ChargeBy) ? "CC on File" : item.ChargeBy),
                        ClientName = item.Client.FirstName + " " + item.Client.LastName,
                        OrderDate = item.AddedDate.ToString("MMMM dd, yyyy hh:mm tt"),
                        OrderAmount = item.OrderAmount
                    };
                    data.Add(d);
                }
                if (data.Count > 0)
                {
                    res.Data = data;
                    res.Message = "Record Found";
                    res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                }
                else
                {
                    res.Message = "No record found";
                    res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
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
        [Route("GetOrderDetails")]
        public async Task<IHttpActionResult> GetOrderDetails([FromBody] EmpCommonModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            var EmpInfo = db.Employees.Find(request.EmployeeId);
            //var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId && x.IsDeleted == false).FirstOrDefault();
            if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "Your account was deactivated by Admin.";
                res.Data = null;
            }

            else
            {
                var Orders = db.Orders.AsEnumerable().Where(x => x.AddedBy == request.EmployeeId && x.AddedByType == 5 && x.Id == request.OrderId && x.OrderItems.Count > 0).ToList();
                List<object> data = new List<object>();
                foreach (Order item in Orders)
                {
                    var billingHistory = item.BillingHistories.ToList().FirstOrDefault();
                    var orderItems = db.OrderItems.Where(x => x.OrderId == item.Id).Select(x => new
                    {
                        x.PartName,
                        PartSize = x.PartSize ?? "",
                        x.Quantity,
                        x.Amount
                    }).ToList();
                    var d = new
                    {
                        OrderId = item.Id,
                        item.OrderNumber,
                        ChargeBy = (string.IsNullOrWhiteSpace(item.ChargeBy) ? "CC on File" : item.ChargeBy),
                        ClientName = item.Client.FirstName + " " + item.Client.LastName,
                        OrderDate = item.AddedDate.ToString("MMMM dd, yyyy hh:mm tt"),
                        OrderAmount = item.OrderAmount,
                        Address = (billingHistory != null ? billingHistory.BillingAddress : ""),
                        City = (billingHistory != null ? billingHistory.City.Name : ""),
                        State = (billingHistory != null ? billingHistory.State.Name : ""),
                        ZipCode = (billingHistory != null ? billingHistory.BillingZipcode : ""),
                        Email = item.Client.Email,
                        item.CCEmail,
                        Recommendation = item.CustomerRecommendation,
                        ClientSignature = (string.IsNullOrWhiteSpace(item.ClientSignature) ? "" : ConfigurationManager.AppSettings["ClientSignatureURL"].ToString() + item.ClientSignature),
                        OrderItems = orderItems
                    };
                    data.Add(d);
                }

                var Orders1 = db.Orders.AsEnumerable().Where(x => x.AddedBy == request.EmployeeId && x.AddedByType == 5 && x.Id == request.OrderId && x.OrderItems.Count == 0).ToList();
                foreach (Order item in Orders1)
                {
                    var billingHistory = item.BillingHistories.ToList().FirstOrDefault();
                    var orderItems = item.BillingHistories.Select(x => new
                    {
                        PartName = "",
                        PartSize = 1,
                        Quantity = 1,
                        Amount = x.PurchasedAmount
                    }).ToList();

                    var d = new
                    {
                        OrderId = item.Id,
                        item.OrderNumber,
                        ChargeBy = (string.IsNullOrWhiteSpace(item.ChargeBy) ? "CC on File" : item.ChargeBy),
                        ClientName = item.Client.FirstName + " " + item.Client.LastName,
                        OrderDate = item.AddedDate.ToString("MMMM dd, yyyy hh:mm tt"),
                        OrderAmount = item.OrderAmount,
                        Address = (billingHistory != null ? billingHistory.BillingAddress : ""),
                        City = (billingHistory != null ? billingHistory.City.Name : ""),
                        State = (billingHistory != null ? billingHistory.State.Name : ""),
                        ZipCode = (billingHistory != null ? billingHistory.BillingZipcode : ""),
                        Email = item.Client.Email,
                        item.CCEmail,
                        Recommendation = item.CustomerRecommendation,
                        ClientSignature = (string.IsNullOrWhiteSpace(item.ClientSignature) ? "" : ConfigurationManager.AppSettings["ClientSignatureURL"].ToString() + item.ClientSignature),
                        OrderItems = orderItems
                    };
                    data.Add(d);
                }
                if (data.Count > 0)
                {
                    res.Data = data.FirstOrDefault();
                    res.Message = "Record Found";
                    res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                }
                else
                {
                    res.Message = "No record found";
                    res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
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
        [Route("GetAllCompletedReports1")]
        public async Task<IHttpActionResult> GetAllCompletedReports1([FromBody] EmpCommonModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            List<object> data = new List<object>();
            var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId).FirstOrDefault();
            if (EmpInfo != null)
            {
                if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    var Services = EmpInfo.Services.AsEnumerable().Where(x => x.Status == api.App_Start.Utilities.ServiceTypes.Completed.GetEnumDescription()).ToList();
                    if (Services.Count > 0)
                    {
                        foreach (Service service in Services)
                        {
                            foreach (ServiceReport report in service.ServiceReports)
                            {
                                var d = new
                                {
                                    report.Id,
                                    ServiceReportNumber = (string.IsNullOrWhiteSpace(report.ServiceReportNumber) ? "" : report.ServiceReportNumber),
                                    service.Client.FirstName,
                                    service.Client.LastName,
                                    PurposeOfVisit = (service.PurposeOfVisit),
                                    PhoneNumber = (string.IsNullOrWhiteSpace(service.Client.PhoneNumber) ? "" : service.Client.PhoneNumber),
                                    MobileNumber = (string.IsNullOrWhiteSpace(service.Client.MobileNumber) ? "" : service.Client.MobileNumber),
                                    OfficeNumber = (string.IsNullOrWhiteSpace(service.Client.OfficeNumber) ? "" : service.Client.OfficeNumber),
                                    ReportDateTime = report.AddedDate.Value.ToString("MMMM dd, yyyy") + " " + report.WorkCompletedTime
                                };
                                data.Add(d);
                            }

                        }
                        res.Data = data;
                        res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                        res.Message = "Records Found";
                    }
                    else
                    {
                        res.Data = null;
                        res.Message = "No Data Found";
                        res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                    }
                }
            }
            else
            {
                res.Data = null;
                res.Message = "You are not authorized to view this details";
                res.StatusCode = HttpStatusCode.Unauthorized.GetEnumValue();
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
        [Route("SubmitEmployeeNotesOnRating")]
        public async Task<IHttpActionResult> SubmitEmployeeNotesOnRating([FromBody]SubmitEmployeeNotesModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            try
            {
                var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId).FirstOrDefault();
                if (EmpInfo == null)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
                    {
                        res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        res.Message = "Your account was deactivated by Admin.";
                        res.Data = null;
                    }
                    else
                    {
                        var ServiceRatingInfo = db.ServiceRatingReviews.Find(request.RatingReviewId);
                        if (ServiceRatingInfo != null)
                        {
                            ServiceRatingInfo.EmployeNotes = request.EmployeeNotes;
                            ServiceRatingInfo.NotesAddedDate = DateTime.Now;
                            db.Entry(ServiceRatingInfo).State = EntityState.Modified;
                            db.SaveChanges();

                            var notification = db.UserNotifications.AsEnumerable().Where(x => x.CommonId == ServiceRatingInfo.ServiceId && x.MessageType == api.App_Start.Utilities.NotificationType.RateService.GetEnumDescription()).FirstOrDefault();
                            if (notification != null)
                            {
                                db.UserNotifications.Remove(notification);
                                db.SaveChanges();
                            }


                            res.Data = "";
                            res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                            res.Message = "Employee Notes Added Successfully.";
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                res.StatusCode = HttpStatusCode.InternalServerError.GetEnumValue();
                res.Data = null;
                res.Message = "Internal Server Error";
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
        [Route("SubmitSalesVisitRequest")]
        public async Task<IHttpActionResult> SubmitSalesVisitRequest([FromBody]SubmitSalesVisitRequestModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            try
            {
                var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId).FirstOrDefault();
                if (EmpInfo == null)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {

                    if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
                    {
                        res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        res.Message = "Your account was deactivated by Admin.";
                        res.Data = null;
                    }
                    else
                    {
                        SalesVisitRequest objData = new SalesVisitRequest();
                        objData.ClientId = request.ClientId;
                        objData.Notes = request.EmployeeNotes;
                        objData.AddressId = request.AddressId;
                        objData.AddedBy = request.EmployeeId;
                        objData.AddedByType = api.App_Start.Utilities.UserRoles.Employee.GetEnumValue();
                        objData.AddedDate = DateTime.Now;
                        db.SalesVisitRequests.Add(objData);
                        db.SaveChanges();

                        res.Data = "";
                        res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                        res.Message = "Sales request for client added Successfully.";
                    }
                }
            }
            catch (Exception Ex)
            {
                res.StatusCode = HttpStatusCode.InternalServerError.GetEnumValue();
                res.Data = null;
                res.Message = Ex.Message.ToString().Trim();
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
        [ResponseType(typeof(ResponseListModel))]
        [HttpPost]
        [Route("GetSalesVisitRequestList")]
        public async Task<IHttpActionResult> GetSalesVisitRequestList([FromBody]EmpCommonModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseListModel res = new ResponseListModel();
            int totalRecord = 0;
            int pageCnt = 0;
            int totalPageCount = 0;
            List<SalesVisitRequestModel> result = new List<SalesVisitRequestModel>();
            var pageSize = int.Parse(api.App_Start.Utilities.GetSiteSettingValue("ApplicationPageSize", db));
            var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId).FirstOrDefault();
            if (EmpInfo != null)
            {
                //var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId && x.IsDeleted == false).FirstOrDefault();
                if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    int EmpRoleId = api.App_Start.Utilities.UserRoles.Employee.GetEnumValue();
                    var SalesRequest = EmpInfo.SalesVisitRequests1.Where(top => top.AddedByType == EmpRoleId && top.AddedBy == request.EmployeeId).OrderByDescending(x => x.AddedDate).ToList();
                    if (SalesRequest.Count > 0)
                    {
                        var objResult = AutoMapper.Mapper.Map<List<SalesVisitRequestModel>>(SalesRequest);
                        if (request.PageNumber.HasValue)
                        {
                            result = CreatePagedResults<SalesVisitRequestModel, SalesVisitRequestModel>(objResult.AsQueryable(), request.PageNumber.Value, pageSize, out totalRecord, out pageCnt, out totalPageCount).ToList();
                        }
                        else
                        {
                            result = objResult;
                        }
                        if (result.Count > 0)
                        {
                            res.Data = result;
                            res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                            res.Message = "Records Found";
                            res.PageNumber = pageCnt;
                            res.TotalNumberOfPages = totalPageCount;
                            res.TotalNumberOfRecords = totalRecord;
                        }
                        else
                        {
                            res.Data = result;
                            res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                            res.Message = "Records Not Found.";
                            res.PageNumber = pageCnt - 1;
                            res.TotalNumberOfPages = totalPageCount;
                            res.TotalNumberOfRecords = totalRecord;
                        }

                    }
                    else
                    {
                        res.Data = null;
                        res.Message = "Records Not Found.";
                        res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                        res.PageNumber = pageCnt - 1;
                        res.TotalNumberOfPages = totalPageCount;
                        res.TotalNumberOfRecords = totalRecord;
                    }
                }
            }
            else
            {
                res.Data = null;
                res.Message = "You are not authorized to view this details";
                res.StatusCode = HttpStatusCode.Unauthorized.GetEnumValue();
                res.PageNumber = (request.PageNumber.HasValue ? request.PageNumber.Value : 1);
                res.TotalNumberOfPages = totalPageCount;
                res.TotalNumberOfRecords = totalRecord;
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
        [HttpGet]
        [Route("GetAllClients")]
        public async Task<IHttpActionResult> GetAllClients([FromUri]int EmployeeId, [FromUri] bool ShowOnlyWorkAreaClient)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            var EmpInfo = db.Employees.Where(x => x.Id == EmployeeId).FirstOrDefault();
            if (updatetoken)
            {
                res.Token = accessToken;
            }
            else
            {
                res.Token = "";
            }
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
            if (!EmpInfo.IsWorkAreaAssigned && EmpInfo.IsSalesPerson == false)
            {
                res.StatusCode = (int)HttpStatusCode.NotFound;
                res.Message = "You have not assigned any work area. Please contact Admin.";
                res.Data = null;
                db.Dispose();
                return Ok(res);
            }
            if (ShowOnlyWorkAreaClient)
            {

            }
            List<Client> ClientInfo = new List<Client>();
            if (ShowOnlyWorkAreaClient)
            {
                var AreaIds = EmpInfo.EmployeeWorkareas.Select(x => x.AreaId).ToArray();
                var Zipcodes = db.WorkAreas.Where(x => AreaIds.Contains(x.AreaId)).Select(x => x.ZipCode.ZipCode1).ToList();
                ClientInfo = db.Clients.Where(top => top.IsActive == true && top.IsDeleted == false && top.ClientAddresses.Where(y => Zipcodes.Contains(y.ZipCode)).Count() > 0).ToList();
            }
            else
            {
                ClientInfo = db.Clients.Where(top => top.IsActive == true && top.IsDeleted == false).ToList();
            }
            if (ClientInfo.Count > 0)
            {
                var ClientData = AutoMapper.Mapper.Map<List<ClientListModel>>(ClientInfo);

                res.Data = ClientData;
                res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                res.Message = "Success";
            }
            else
            {
                res.Data = null;
                res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                res.Message = "No Data Found";
            }

            db.Dispose();
            return Ok(res);
        }

        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpGet]
        [Route("GetClientAddress")]
        public async Task<IHttpActionResult> GetClientAddress([FromUri]int ClientId, [FromUri] int EmployeeId)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

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
            var ClientInfo = db.Clients.Find(ClientId);
            if (ClientInfo != null)
            {
                var ClientAddress = ClientInfo.ClientAddresses.Where(x => x.IsDeleted == false).ToList();
                if (ClientAddress.Count > 0)
                {
                    var objResult = AutoMapper.Mapper.Map<List<ClientAddressModel>>(ClientAddress);
                    foreach (ClientAddressModel item in objResult)
                    {
                        item.Email = ClientInfo.Email;

                        var zip = await db.ZipCodes.Where(x => x.ZipCode1 == item.ZipCode && x.StateId == item.State && x.CitiesId == item.City).FirstOrDefaultAsync();
                        var state = db.States.Where(x => x.Id == item.State).FirstOrDefault();
                        var city = db.Cities.Where(x => x.Id == item.City).FirstOrDefault();

                        if (zip == null)
                        {
                            item.ShowAddressInApp = false;
                        }
                        else if (zip.IsDeleted == true || zip.Status == false)
                        {
                            item.ShowAddressInApp = false;
                        }
                        else if (zip.PendingInactive == true)
                        {
                            item.ShowAddressInApp = false;
                        }
                        else if (state == null)
                        {
                            item.ShowAddressInApp = false;
                        }
                        else if (state.IsDeleted == true || state.Status == false)
                        {
                            item.ShowAddressInApp = false;
                        }
                        else if (state.PendingInactive == true)
                        {
                            item.ShowAddressInApp = false;
                        }
                        else if (city == null)
                        {
                            item.ShowAddressInApp = false;
                        }
                        else if (city.IsDeleted == true || city.Status == false)
                        {
                            item.ShowAddressInApp = false;
                        }
                        else if (city.PendingInactive == true)
                        {
                            item.ShowAddressInApp = false;
                        }
                        else
                        {
                            item.ShowAddressInApp = true;
                        }
                    }
                    res.Data = objResult;
                    res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                    res.Message = "Success";
                }
                else
                {
                    res.Data = "";
                    res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                    res.Message = "Client Address Not Found.";
                }
            }
            else
            {
                res.Data = "";
                res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                res.Message = "Client Not Found.";
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
        [HttpGet]
        [Route("GetClientDefaultAddress")]
        public async Task<IHttpActionResult> GetClientDefaultAddress([FromUri]int ClientId, [FromUri] int EmployeeId)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

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
            var ClientInfo = db.Clients.Find(ClientId);
            if (ClientInfo != null)
            {
                var ClientAddress = ClientInfo.ClientAddresses.Where(x => x.IsDefaultAddress == true).ToList();
                var clientCC = ClientInfo.ClientPaymentMethods.Where(x => x.IsDefaultPayment == true).FirstOrDefault();
                if (ClientAddress.Count > 0)
                {
                    var objResult = AutoMapper.Mapper.Map<List<ClientAddressModel>>(ClientAddress);
                    foreach (ClientAddressModel item in objResult)
                    {
                        item.Email = ClientInfo.Email;

                        var zip = await db.ZipCodes.Where(x => x.ZipCode1 == item.ZipCode && x.StateId == item.State && x.CitiesId == item.City).FirstOrDefaultAsync();
                        if (zip == null)
                        {
                            item.ShowAddressInApp = false;
                        }
                        else
                        {
                            if (zip.IsDeleted == true || zip.Status == false)
                            {
                                item.ShowAddressInApp = false;
                            }
                            else
                            {
                                item.ShowAddressInApp = true;
                            }
                        }
                    }

                    var d = new
                    {
                        objResult.FirstOrDefault().Id,
                        objResult.FirstOrDefault().Address,
                        objResult.FirstOrDefault().AllowDelete,
                        objResult.FirstOrDefault().City,
                        objResult.FirstOrDefault().CityName,
                        objResult.FirstOrDefault().ClientId,
                        objResult.FirstOrDefault().Email,
                        objResult.FirstOrDefault().HomeNumber,
                        objResult.FirstOrDefault().IsDefaultAddress,
                        objResult.FirstOrDefault().Latitude,
                        objResult.FirstOrDefault().Longitude,
                        objResult.FirstOrDefault().MobileNumber,
                        objResult.FirstOrDefault().OfficeNumber,
                        objResult.FirstOrDefault().PhoneNumber,
                        objResult.FirstOrDefault().ShowAddressInApp,
                        objResult.FirstOrDefault().State,
                        objResult.FirstOrDefault().StateName,
                        objResult.FirstOrDefault().ZipCode,
                        ClientInfo.Company,
                        NameOnCard = (clientCC != null ? clientCC.NameOnCard : ""),
                        CardNumber = (clientCC != null ? clientCC.CardNumber : ""),
                        CardType = (clientCC != null ? (string.IsNullOrWhiteSpace(clientCC.CardType) ? "" : clientCC.CardType) : ""),
                        ExpiryMonth = (clientCC != null ? clientCC.ExpiryMonth : 0),
                        ExpiryYear = (clientCC != null ? clientCC.ExpiryYear : 0),
                        StripeCardId = (clientCC != null ? clientCC.CustomerPaymentProfileId : "")
                    };
                    res.Data = d;
                    res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                    res.Message = "Success";
                }
                else
                {
                    var d = new
                    {
                        Id = 0,
                        Address = "",
                        AllowDelete = false,
                        City = 0,
                        CityName = "",
                        ClientId = ClientInfo.Id,
                        ClientInfo.Email,
                        ClientInfo.HomeNumber,
                        IsDefaultAddress = false,
                        Latitude = 0m,
                        Longitude = 0m,
                        ClientInfo.MobileNumber,
                        ClientInfo.OfficeNumber,
                        ClientInfo.PhoneNumber,
                        ShowAddressInApp = false,
                        State = 0,
                        StateName = "",
                        ZipCode = "",
                        NameOnCard = (clientCC != null ? clientCC.NameOnCard : ""),
                        CardNumber = (clientCC != null ? clientCC.CardNumber : ""),
                        CardType = (clientCC != null ? (string.IsNullOrWhiteSpace(clientCC.CardType) ? "" : clientCC.CardType) : ""),
                        ExpiryMonth = (clientCC != null ? clientCC.ExpiryMonth : 0),
                        ExpiryYear = (clientCC != null ? clientCC.ExpiryYear : 0),
                        StripeCardId = (clientCC != null ? clientCC.CustomerPaymentProfileId : "")
                    };
                    res.Data = d;
                    res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                    res.Message = "Client Address Not Found.";
                }
            }
            else
            {
                res.Data = "";
                res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                res.Message = "Client Not Found.";
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
        [HttpGet]
        [Route("GetClientUnitsByAddressId")]
        public async Task<IHttpActionResult> GetClientUnitsByAddressId([FromUri]int ClientId, int AddressId, [FromUri] int EmployeeId)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

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
            var ClientInfo = db.Clients.Find(ClientId);
            if (ClientInfo != null)
            {
                var ClientUnits = db.ClientUnits.Where(top => top.ClientId == ClientId && top.AddressId == AddressId && top.IsDeleted == false && top.IsPlanRenewedOrCancelled == false).ToList();
                if (ClientUnits.Count > 0)
                {
                    var objResult = AutoMapper.Mapper.Map<List<ClientUnitsModel>>(ClientUnits);
                    res.Data = objResult;
                    res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                    res.Message = "Client Units Found.";
                }
                else
                {
                    res.Data = "";
                    res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                    res.Message = "Client Unit Not Found.";
                }
            }
            else
            {
                res.Data = "";
                res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                res.Message = "Client Not Found.";
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
        [HttpGet]
        [Route("GetTimeAndPurposeOfRequest")]
        public async Task<IHttpActionResult> GetTimeAndPurposeOfRequest([FromUri] int EmployeeId)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

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

            TimeAndPurposeModel objResult = new TimeAndPurposeModel();
            var time = await db.PlanTypes.FirstOrDefaultAsync();
            objResult.ServiceSlot1 = time.ServiceSlot1;
            objResult.ServiceSlot2 = time.ServiceSlot2;
            //objResult.PurposeOfVisit = Enum.GetNames(typeof(api.App_Start.Utilities.PurposeOfVisit)).Cast<object>().ToList();
            objResult.PurposeOfVisit = typeof(api.App_Start.Utilities.PurposeOfVisit).GetEnumValuesWithDescription<api.App_Start.Utilities.PurposeOfVisit>().Select(x => x.Key.GetEnumDescription()).ToList();
            res.Data = objResult;
            res.StatusCode = HttpStatusCode.OK.GetEnumValue();
            res.Message = "Success";

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
        [Route("SubmitRequestServiceForClient")]
        public async Task<IHttpActionResult> SubmitRequestServiceForClient([FromBody]SubmitRequestService request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            RequestedService objReqService = new RequestedService();

            try
            {
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
                int Index = 1;

                api.App_Start.Utilities.PurposeOfVisit p = (api.App_Start.Utilities.PurposeOfVisit)Convert.ToInt32(request.PurposeOfVisit);
                //if (p.GetEnumDescription() == api.App_Start.Utilities.PurposeOfVisit.Emergency.GetEnumDescription())
                //{
                //    Index = 0;
                //}
                if (request.ServiceDate.Date == DateTime.Now.Date)
                {
                    bool IsValidTime = api.App_Start.Utilities.CheckTimeValidation(request.TimeSlot, Index);
                    if (!IsValidTime)
                    {
                        res.StatusCode = HttpStatusCode.Ambiguous.GetEnumValue();
                        res.Message = "Please select future time.";
                        res.Data = null;
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
                }
                var requestdService = db.RequestedServices.Where(x => x.ClientId == request.ClientId && x.ServiceRequestedOn == request.ServiceDate && x.IsDeleted == false).Include(x => x.RequestedServiceUnits).ToList();
                bool alreadyadd = false;
                var msg = "";


                foreach (var item in requestdService)
                {
                    foreach (var it in item.RequestedServiceUnits)
                    {
                        var d = request.UnitIds.Where(x => x == it.UnitId).ToList();
                        if (d.Count > 0)
                        {
                            alreadyadd = true;
                            msg = it.ClientUnit.UnitName + ",";
                        }
                    }
                }
                var userinfo = db.Clients.Find(request.ClientId);

                if (alreadyadd)
                {
                    msg = "Some of the selected units were already requested for this date.";
                    res.StatusCode = HttpStatusCode.Ambiguous.GetEnumValue();
                    res.Message = msg;
                    res.Data = null;
                }
                else
                {
                    var cAddress = db.ClientAddresses.Where(x => x.ClientId == request.ClientId && x.Id == request.AddressId).FirstOrDefault();
                    var sCount = db.RequestedServices.Count(x => x.ClientId == request.ClientId) + 1;
                    var serviceNo = userinfo.AccountNumber + "-" + cAddress.ZipCode + "-RS" + sCount;
                    api.App_Start.Utilities.PurposeOfVisit p1 = (api.App_Start.Utilities.PurposeOfVisit)Convert.ToInt32(request.PurposeOfVisit);
                    objReqService.ServiceCaseNumber = serviceNo;
                    objReqService.ClientId = request.ClientId;
                    objReqService.AddressId = request.AddressId;
                    objReqService.PurposeOfVisit = p1.GetEnumDescription();
                    objReqService.ServiceRequestedTime = request.TimeSlot;
                    objReqService.ServiceRequestedOn = request.ServiceDate;
                    objReqService.Notes = (string.IsNullOrWhiteSpace(request.Notes) ? "No notes has been added" : request.Notes);
                    objReqService.AddedBy = request.EmployeeId;
                    objReqService.AddedByType = api.App_Start.Utilities.UserRoles.Employee.GetEnumValue();
                    objReqService.AddedDate = DateTime.UtcNow;

                    db.RequestedServices.Add(objReqService);
                    db.SaveChanges();

                    if (objReqService.Id != 0)
                    {
                        RequestedServiceUnit objReqServiceUnit = new RequestedServiceUnit();
                        foreach (var item in request.UnitIds)
                        {
                            objReqServiceUnit.ServiceId = objReqService.Id;
                            objReqServiceUnit.UnitId = item;
                            db.RequestedServiceUnits.Add(objReqServiceUnit);
                            db.SaveChanges();

                            //db.uspa_ClientUnitServiceCount_Update(request.ClientId, item, 0, 0, 1, 0, 0, 0, request.EmployeeId, api.App_Start.Utilities.UserRoles.Employee.GetEnumValue(), DateTime.UtcNow);
                        }
                        db.uspa_ClientUnitServiceCount_UpdateByRequestedServiceId(objReqService.Id);
                    }
                    if (objReqService.PurposeOfVisit == api.App_Start.Utilities.PurposeOfVisit.Emergency.GetEnumDescription())
                    {
                        var EmergencyService = db.uspa_EmergencyRequestedServiceSchedule(objReqService.Id).ToList();

                        if (EmergencyService.Count > 0)
                        {
                            var es = EmergencyService.FirstOrDefault();
                            if (es.EmployeeId > 0)
                            {
                                var service = db.Services.Where(x => x.Id == es.ServiceId).FirstOrDefault();

                                var EmpNotification = db.NotificationMasters.Where(x => x.Name == "EmployeeSchedule").FirstOrDefault();
                                var message = EmpNotification.Message;
                                message = message.Replace("{{ScheduleDate}}", service.ScheduleDate.Value.ToString("MMMM dd, yyyy"));

                                UserNotification objUserNotification = new UserNotification();
                                objUserNotification.UserId = service.EmployeeId;
                                objUserNotification.UserTypeId = api.App_Start.Utilities.UserRoles.Employee.GetEnumValue();
                                objUserNotification.Message = message;
                                objUserNotification.Status = api.App_Start.Utilities.NotificationStatus.UnRead.GetEnumDescription();
                                objUserNotification.CommonId = es.ServiceId;
                                objUserNotification.MessageType = api.App_Start.Utilities.NotificationType.ServiceScheduled.GetEnumDescription();
                                objUserNotification.AddedDate = DateTime.UtcNow;
                                db.UserNotifications.Add(objUserNotification);
                                db.SaveChanges();

                                var BadgeCount = db.UserNotifications.AsEnumerable().Where(x => x.UserId == service.EmployeeId && x.UserTypeId == api.App_Start.Utilities.UserRoles.Employee.GetEnumValue() && x.Status == api.App_Start.Utilities.NotificationStatus.UnRead.GetEnumDescription()).ToList().Count;

                                Notifications objNotifications = new Notifications { NId = objUserNotification.Id, NType = api.App_Start.Utilities.NotificationType.ServiceScheduled.GetEnumValue(), CommonId = es.ServiceId.Value };
                                List<NotificationModel> notify = new List<NotificationModel>();
                                notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                                notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                                notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });
                                if (EmpInfo.DeviceType != null && EmpInfo.DeviceToken != null)
                                {
                                    if (EmpInfo.DeviceType.ToLower() == "android")
                                    {
                                    }
                                    else if (EmpInfo.DeviceType.ToLower() == "iphone")
                                    {
                                        SendNotifications.SendIphoneNotification(BadgeCount, EmpInfo.DeviceToken, message, notify, "employee", HttpContext.Current);
                                    }
                                }
                            }
                        }
                        if (EmergencyService.Count > 0)
                        {
                            var es = EmergencyService.FirstOrDefault();
                            if (es.EmployeeId > 0)
                            {
                                var service = db.Services.Find(es.ServiceId);
                                var ClientNotification = db.NotificationMasters.Where(x => x.Name == "RequestedServiceScheduleSendToClient").FirstOrDefault();
                                var message = ClientNotification.Message;
                                message = message.Replace("{{ScheduleDate}}", service.ScheduleDate.Value.ToString("MMMM dd, yyyy"));
                                UserNotification objUserNotification = new UserNotification();
                                objUserNotification.UserId = service.ClientId;
                                objUserNotification.UserTypeId = api.App_Start.Utilities.UserRoles.Client.GetEnumValue();
                                objUserNotification.Message = message;
                                objUserNotification.Status = api.App_Start.Utilities.NotificationStatus.UnRead.GetEnumDescription();
                                objUserNotification.CommonId = service.Id;
                                objUserNotification.MessageType = api.App_Start.Utilities.NotificationType.ServiceScheduled.GetEnumDescription();
                                objUserNotification.AddedDate = DateTime.UtcNow;
                                db.UserNotifications.Add(objUserNotification);
                                db.SaveChanges();

                                var BadgeCount = db.uspa_ClientPortal_GetNotificationForDashBoardByUserType(service.ClientId, api.App_Start.Utilities.UserRoles.Client.GetEnumValue(), "", 0).AsEnumerable().Where(x => x.Status == api.App_Start.Utilities.NotificationStatus.UnRead.GetEnumDescription()).Count();
                                Notifications objNotifications = new Notifications { NId = objUserNotification.Id, NType = api.App_Start.Utilities.NotificationType.ServiceScheduled.GetEnumValue(), CommonId = service.Id };
                                List<NotificationModel> notify = new List<NotificationModel>();
                                notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                                notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                                notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });
                                if (userinfo.DeviceType != null && userinfo.DeviceToken != null)
                                {
                                    if (userinfo.DeviceType.ToLower() == "android")
                                    {
                                        string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType + "&data.CommonId=" + objNotifications.CommonId;
                                        SendNotifications.SendAndroidNotification(userinfo.DeviceToken, message, CustomData, "client");
                                    }
                                    else if (userinfo.DeviceType.ToLower() == "iphone")
                                    {
                                        SendNotifications.SendIphoneNotification(BadgeCount, userinfo.DeviceToken, message, notify, "client", HttpContext.Current);
                                    }
                                }
                            }
                        }
                    }
                    res.Data = "";
                    res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                    res.Message = "Service added Successfully.";
                }
            }
            catch (Exception Ex)
            {
                res.Data = "";
                res.StatusCode = HttpStatusCode.InternalServerError.GetEnumValue();
                res.Message = Ex.Message.Trim().ToString();
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
        [ResponseType(typeof(ResponseListModel))]
        [HttpPost]
        [Route("ServicePartList")]
        public async Task<IHttpActionResult> ServicePartList([FromBody] EmpCommonModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseListModel res = new ResponseListModel();
            List<object> data = new List<object>();
            int totalRecord = 0;
            int pageCnt = 0;
            int totalPageCount = 0;
            List<ServicePartModel> EmpServices = new List<ServicePartModel>();
            List<ServicePartModel> result = new List<ServicePartModel>();
            List<ServicePartModel> sv = new List<ServicePartModel>();
            var pageSize = int.Parse(api.App_Start.Utilities.GetSiteSettingValue("ApplicationPageSize", db));

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
            if (EmpInfo != null)
            {
                var dtStart = request.StatDate.Value.Date;
                var dtEnd = (request.EndDate == null ? dtStart : request.EndDate.Value.Date);
                var dbfinal = db.Services.Where(x => (x.ScheduleDate >= dtStart && x.ScheduleDate <= dtEnd)).AsEnumerable()
                    .Where(x => x.Status == api.App_Start.Utilities.ServiceTypes.Scheduled.GetEnumDescription() && x.EmployeeId == EmpInfo.Id).Select(s => new ServicePartModel()
                    {
                        ServiceId = s.Id,
                        ServiceNumber = s.ServiceCaseNumber,
                        ClientName = s.Client.FirstName + " " + s.Client.LastName,
                        ServiceDate = s.ScheduleDate,
                        Status = s.Status,
                        ServicePartLists = s.ServicePartLists,
                    }).ToList();
                EmpServices = dbfinal;// sv.Where(x => x.Status == api.App_Start.Utilities.ServiceTypes.Scheduled.GetEnumDescription()).OrderByDescending(x => x.ServiceDate).ToList();

                if (request.PageNumber.HasValue)
                {
                    result = CreatePagedResults<ServicePartModel, ServicePartModel>(EmpServices.AsQueryable(), request.PageNumber.Value, pageSize, out totalRecord, out pageCnt, out totalPageCount).ToList();
                }
                else
                {
                    result = EmpServices;
                }
                var Services = result;
                if (Services.Count > 0)
                {
                    foreach (ServicePartModel service in Services)
                    {
                        var c = service;
                        //var Parts = new List<PartDetails>();
                        var sp = service.ServicePartLists.GroupBy(x => new { x.PartId, x.Part.Name, x.Part.Size }).Select(u => new
                        {
                            PartName = u.Key.Name + " - " + u.Key.Size ?? "",
                            Quantity = u.Sum(x => x.PartQuantity)
                        }).ToList();
                        var d = new
                        {
                            ServiceId = c.ServiceId,
                            ServiceDate = c.ServiceDate,
                            ServiceNumber = c.ServiceNumber,
                            ClientName = c.ClientName,
                            Parts = sp
                        };
                        data.Add(d);
                    }
                    res.Data = data;
                    res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                    res.Message = "Records Found";
                    res.PageNumber = pageCnt;
                    res.TotalNumberOfPages = totalPageCount;
                    res.TotalNumberOfRecords = totalRecord;
                }
                else
                {
                    res.Data = null;
                    res.Message = "No Data Found";
                    res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                    res.PageNumber = pageCnt - 1;
                    res.TotalNumberOfPages = totalPageCount;
                    res.TotalNumberOfRecords = totalRecord;
                }
            }
            else
            {
                res.Data = null;
                res.Message = "You are not authorized to view this details";
                res.StatusCode = HttpStatusCode.Unauthorized.GetEnumValue();
                res.PageNumber = (request.PageNumber.HasValue ? request.PageNumber.Value : 1);
                res.TotalNumberOfPages = totalPageCount;
                res.TotalNumberOfRecords = totalRecord;
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
        [ResponseType(typeof(ResponseListModel))]
        [HttpPost]
        [Route("RequestedPartList")]
        public async Task<IHttpActionResult> RequestedPartList([FromBody] EmpCommonModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseListModel res = new ResponseListModel();
            List<object> data = new List<object>();
            int totalRecord = 0;
            int pageCnt = 0;
            int totalPageCount = 0;
            List<RequstedPartList> EmpServices = new List<RequstedPartList>();
            List<RequstedPartList> result = new List<RequstedPartList>();
            List<RequstedPartList> sv = new List<RequstedPartList>();
            var pageSize = int.Parse(api.App_Start.Utilities.GetSiteSettingValue("ApplicationPageSize", db));

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
            if (EmpInfo != null)
            {
                var dbfinal = db.EmployeePartRequestMasters.Where(x => x.EmployeeId == EmpInfo.Id).OrderByDescending(x => x.AddedDate).AsEnumerable()
                    .Select(s => new RequstedPartList()
                    {
                        EmployeePartRequestId = s.Id,
                        ClientName = s.Client.FirstName + " " + s.Client.LastName,
                        RequestedDate = s.AddedDate.ToString("MMMM dd, yyyy hh:mm tt"),
                        Status = s.Status,
                        EmployeePartRequests = s.EmployeePartRequests,
                    }).ToList();


                EmpServices = dbfinal;// sv.Where(x => x.Status == api.App_Start.Utilities.ServiceTypes.Scheduled.GetEnumDescription()).OrderByDescending(x => x.ServiceDate).ToList();

                if (request.PageNumber.HasValue)
                {
                    result = CreatePagedResults<RequstedPartList, RequstedPartList>(EmpServices.AsQueryable(), request.PageNumber.Value, pageSize, out totalRecord, out pageCnt, out totalPageCount).ToList();
                }
                else
                {
                    result = EmpServices;
                }
                var Services = result;
                if (Services.Count > 0)
                {
                    foreach (RequstedPartList service in Services)
                    {
                        var c = service;
                        //var Parts = new List<PartDetails>();
                        var sp = service.EmployeePartRequests.GroupBy(x => new { x.PartId, x.PartName, x.Part }).Select(u => new
                        {
                            PartName = (u.Key.PartId != null ? u.Key.Part.Name : u.Key.PartName),
                            Quantity = u.Sum(x => x.RequestedQuantity),
                            u.Key.PartId
                        }).ToList();
                        if (service.EmployeePartRequests.Count > 0)
                        {
                            var d = new
                            {
                                EmployeePartRequestId = service.EmployeePartRequestId,
                                PartId = (sp.FirstOrDefault().PartId == null ? 0 : sp.FirstOrDefault().PartId),
                                PartName = sp.FirstOrDefault().PartName,
                                Quantity = sp.FirstOrDefault().Quantity,
                                c.RequestedDate,
                                c.Status,
                                ClientName = c.ClientName
                            };
                            data.Add(d);
                        }
                    }
                    res.Data = data;
                    res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                    res.Message = "Records Found";
                    res.PageNumber = pageCnt;
                    res.TotalNumberOfPages = totalPageCount;
                    res.TotalNumberOfRecords = totalRecord;
                }
                else
                {
                    res.Data = null;
                    res.Message = "No Data Found";
                    res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                    res.PageNumber = pageCnt - 1;
                    res.TotalNumberOfPages = totalPageCount;
                    res.TotalNumberOfRecords = totalRecord;
                }
            }
            else
            {
                res.Data = null;
                res.Message = "You are not authorized to view this details";
                res.StatusCode = HttpStatusCode.Unauthorized.GetEnumValue();
                res.PageNumber = (request.PageNumber.HasValue ? request.PageNumber.Value : 1);
                res.TotalNumberOfPages = totalPageCount;
                res.TotalNumberOfRecords = totalRecord;
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
        [Route("RequestedPartDetail")]
        public async Task<IHttpActionResult> RequestedPartDetail([FromBody] EmpPartRequestModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();


            EmployeePartRequestMaster eprm = db.EmployeePartRequestMasters.Find(request.EmployeePartRequestId);

            var d = new
            {
                EmployeePartRequestId = eprm.Id,
                eprm.EmployeePartRequests.FirstOrDefault().PartName,
                eprm.EmployeePartRequests.FirstOrDefault().PartSize,
                eprm.ClientId,
                EmpNotes = string.IsNullOrWhiteSpace(eprm.EmpNotes) ? "" : eprm.EmpNotes,
                ClientName = eprm.Client.FirstName + " " + eprm.Client.LastName,
                AddressId = eprm.ClientAddressId,
                Units = eprm.EmployeePartRequests.Select(x => new
                {
                    x.UnitId,
                    x.RequestedQuantity
                })
            };
            res.StatusCode = HttpStatusCode.OK.GetEnumValue();
            res.Message = "Recored Found";
            res.Data = d;


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
        [Route("CancelRequestedPartDetail")]
        public async Task<IHttpActionResult> CancelRequestedPartDetail([FromBody] EmpPartRequestModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            EmployeePartRequestMaster eprm = db.EmployeePartRequestMasters.Find(request.EmployeePartRequestId);

            eprm.Status = api.App_Start.Utilities.EmpPartRequestStatus.Cancelled.GetEnumDescription();
            eprm.StatusUpdatedDate = DateTime.UtcNow;
            eprm.EmpNotes = request.EmpNotes;
            eprm.UpdatedBy = request.EmployeeId;
            eprm.UpdatedByType = api.App_Start.Utilities.UserRoles.Employee.GetEnumValue();
            eprm.UpdatedDate = DateTime.UtcNow;

            db.SaveChanges();

            res.StatusCode = HttpStatusCode.OK.GetEnumValue();
            res.Message = "Request Cancelled Successfully";
            res.Data = null;


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
        [Route("EmpPartRequeste")]
        public async Task<IHttpActionResult> EmpPartRequeste([FromBody] EmpPartRequestModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            try
            {
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
                EmployeePartRequestMaster eprm = new EmployeePartRequestMaster();
                if (request.EmployeePartRequestId.HasValue)
                {
                    if (request.EmployeePartRequestId.Value > 0)
                    {
                        eprm = db.EmployeePartRequestMasters.Find(request.EmployeePartRequestId);
                        eprm.UpdatedBy = request.EmployeeId;
                        eprm.UpdatedByType = api.App_Start.Utilities.UserRoles.Employee.GetEnumValue();
                        eprm.UpdatedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        eprm.AddedBy = request.EmployeeId;
                        eprm.AddedByType = api.App_Start.Utilities.UserRoles.Employee.GetEnumValue();
                        eprm.AddedDate = DateTime.UtcNow;
                    }
                }
                else
                {
                    eprm.AddedBy = request.EmployeeId;
                    eprm.AddedByType = api.App_Start.Utilities.UserRoles.Employee.GetEnumValue();
                    eprm.AddedDate = DateTime.UtcNow;
                }
                eprm.ClientId = request.ClientID;
                eprm.EmployeeId = request.EmployeeId;
                eprm.ClientAddressId = request.AddressId;
                eprm.EmpNotes = request.EmpNotes;
                if (request.EmployeePartRequestId.HasValue)
                {
                    if (request.EmployeePartRequestId.Value > 0)
                    {
                        var eprd = eprm.EmployeePartRequests.ToList();

                        db.EmployeePartRequests.RemoveRange(eprd);
                    }
                }
                foreach (var item in request.RequestedParts)
                {
                    EmployeePartRequest epr = new EmployeePartRequest();
                    epr.PartId = item.PartId;
                    epr.PartName = item.PartName;
                    epr.PartSize = item.PartSize;
                    epr.RequestedQuantity = item.Quantity;
                    epr.ArrangedQuantity = item.Quantity;
                    epr.UnitId = item.UnitId;
                    epr.Description = item.Description;
                    eprm.EmployeePartRequests.Add(epr);
                }
                if (request.EmployeePartRequestId.HasValue)
                {
                    if (request.EmployeePartRequestId.Value > 0)
                    {

                    }
                    else
                    {
                        db.EmployeePartRequestMasters.Add(eprm);
                    }
                }
                else
                {
                    db.EmployeePartRequestMasters.Add(eprm);
                }
                eprm.Status = api.App_Start.Utilities.EmpPartRequestStatus.NeedToOrder.GetEnumDescription();
                db.SaveChanges();

                var ret = db.uspa_CheckInStockAndScheduleService(eprm.Id, eprm.AddedBy, eprm.AddedByType, eprm.AddedDate).ToList();
                if (ret.FirstOrDefault().ServiceId > 0)
                {

                    var EmpNotification = db.NotificationMasters.Where(x => x.Name == "EmployeeSchedule").FirstOrDefault();
                    var message = EmpNotification.Message; //var message = "System has scheduled a service for you on " + ret.FirstOrDefault().ScheduleDate.ToString("MMMM dd, yyyy") + ".";
                    message = message.Replace("{{ScheduleDate}}", ret.FirstOrDefault().ScheduleDate.ToString("MMMM dd, yyyy"));
                    UserNotification objUserNotification = new UserNotification();
                    objUserNotification.UserId = eprm.AddedBy;
                    objUserNotification.UserTypeId = api.App_Start.Utilities.UserRoles.Employee.GetEnumValue();
                    objUserNotification.Message = message;
                    objUserNotification.Status = api.App_Start.Utilities.NotificationStatus.UnRead.GetEnumDescription();
                    objUserNotification.CommonId = ret.FirstOrDefault().ServiceId;
                    objUserNotification.MessageType = api.App_Start.Utilities.NotificationType.ServiceScheduled.GetEnumDescription();
                    objUserNotification.AddedDate = DateTime.UtcNow;
                    db.UserNotifications.Add(objUserNotification);
                    db.SaveChanges();

                    var BadgeCount = db.UserNotifications.AsEnumerable().Where(x => x.UserId == eprm.AddedBy && x.UserTypeId == api.App_Start.Utilities.UserRoles.Employee.GetEnumValue() && x.Status == api.App_Start.Utilities.NotificationStatus.UnRead.GetEnumDescription()).ToList().Count;

                    Notifications objNotifications = new Notifications { NId = objUserNotification.Id, NType = api.App_Start.Utilities.NotificationType.ServiceScheduled.GetEnumValue(), CommonId = objUserNotification.CommonId.Value };
                    List<NotificationModel> notify = new List<NotificationModel>();
                    notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                    notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                    notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });
                    if (EmpInfo.DeviceType != null && EmpInfo.DeviceToken != null)
                    {
                        if (EmpInfo.DeviceType.ToLower() == "android")
                        {
                            //string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType + "&data.CommonId=" + objNotifications.CommonId;
                            //SendNotifications.SendAndroidNotification(UserInfo.DeviceToken, message, CustomData, "employee");
                        }
                        else if (EmpInfo.DeviceType.ToLower() == "iphone")
                        {
                            SendNotifications.SendIphoneNotification(BadgeCount, EmpInfo.DeviceToken, message, notify, "employee", HttpContext.Current);
                        }
                    }
                }
                if (ret.FirstOrDefault().ServiceId > 0)
                {
                    var service = db.Services.Find(ret.FirstOrDefault().ServiceId);
                    var ClientNotification = db.NotificationMasters.Where(x => x.Name == "PartRequestServiceScheduleSendToClient").FirstOrDefault();
                    var message = ClientNotification.Message; //var message = "Service " + service.ServiceCaseNumber + " for your requested parts has been scheduled on " + service.ScheduleDate.Value.ToString("MMMM dd, yyyy") + ".";
                    //message = message.Replace("{{ServiceCaseNumber}}", service.ServiceCaseNumber);
                    message = message.Replace("{{Address}}", service.ClientAddress.Address);
                    message = message.Replace("{{ScheduleDate}}", service.ScheduleDate.Value.ToString("MMMM dd, yyyy"));
                    UserNotification objUserNotification = new UserNotification();
                    objUserNotification.UserId = service.ClientId;
                    objUserNotification.UserTypeId = api.App_Start.Utilities.UserRoles.Client.GetEnumValue();
                    objUserNotification.Message = message;
                    objUserNotification.Status = api.App_Start.Utilities.NotificationStatus.UnRead.GetEnumDescription();
                    objUserNotification.CommonId = service.Id;
                    objUserNotification.MessageType = api.App_Start.Utilities.NotificationType.PeriodicServiceReminder.GetEnumDescription();
                    objUserNotification.AddedDate = DateTime.UtcNow;
                    db.UserNotifications.Add(objUserNotification);
                    db.SaveChanges();

                    //var BadgeCount = db.UserNotifications.AsEnumerable().Where(x => x.UserId == service.ClientId && x.UserTypeId == api.App_Start.Utilities.UserRoles.Client.GetEnumValue()).ToList().Count;
                    var BadgeCount = db.uspa_ClientPortal_GetNotificationForDashBoardByUserType(service.ClientId, api.App_Start.Utilities.UserRoles.Client.GetEnumValue(), "", 0).AsEnumerable().Where(x => x.Status == api.App_Start.Utilities.NotificationStatus.UnRead.GetEnumDescription()).Count();
                    Notifications objNotifications = new Notifications { NId = objUserNotification.Id, NType = api.App_Start.Utilities.NotificationType.PeriodicServiceReminder.GetEnumValue(), CommonId = service.Id };
                    List<NotificationModel> notify = new List<NotificationModel>();
                    notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                    notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                    notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });
                    var UserInfo = db.Clients.Where(x => x.Id == service.ClientId).FirstOrDefault();
                    if (UserInfo.DeviceType != null && UserInfo.DeviceToken != null)
                    {
                        if (UserInfo.DeviceType.ToLower() == "android")
                        {
                            string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType + "&data.CommonId=" + objNotifications.CommonId;
                            SendNotifications.SendAndroidNotification(UserInfo.DeviceToken, message, CustomData, "client");
                        }
                        else if (UserInfo.DeviceType.ToLower() == "iphone")
                        {
                            SendNotifications.SendIphoneNotification(BadgeCount, UserInfo.DeviceToken, message, notify, "client", HttpContext.Current);
                        }
                    }
                }
                res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                res.Message = "Request Added";
                res.Data = null;
            }
            catch (Exception ex)
            {
                res.StatusCode = HttpStatusCode.InternalServerError.GetEnumValue();
                res.Message = "Internal Server Error";
                res.Data = null;
            }
            db.Dispose();
            return Ok(res);
        }

        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("ResendReport")]
        public async Task<IHttpActionResult> ResendReport([FromBody] EmpCommonModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            try
            {
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

                var Report = db.ServiceReports.Find(request.ReportId);
                var service = Report.Service;
                var UserInfo = Report.Service.Client;//db.Clients.Find(Report.Service.ClientId);

                var ClientAddress = Report.ServiceReportUnits.FirstOrDefault().ClientUnit.ClientAddress;
                EmailTemplate templateclient = db.EmailTemplates.Where(x => x.Name == "ServiceReportClient" && x.Status == true).FirstOrDefault();
                var strclient = templateclient.EmailBody;
                //var sub = templateclient.EmailTemplateSubject + " - " + Report.Service.ServiceCaseNumber + (service.IsNoShow == true ? "(No Show Service)" : "");
                var sub = templateclient.EmailTemplateSubject.Replace("{{Address}}", Report.Service.ClientAddress.Address + (service.IsNoShow == true ? "(No Show Service)" : ""));
                sub = sub.Replace("{{ServiceCaseNumber}}", service.ServiceCaseNumber);
                strclient = strclient.Replace("{{ServiceReport}}", Report.ServiceReportNumber + (service.IsNoShow == true ? "(No Show Service)" : ""));
                strclient = strclient.Replace("{{ContactName}}", UserInfo.FirstName + " " + UserInfo.LastName);
                strclient = strclient.Replace("{{Company}}", UserInfo.Company ?? "-");
                strclient = strclient.Replace("{{Address}}", ClientAddress.Address + ",<br/>" + ClientAddress.City1.Name + ", " + ClientAddress.State1.Name + ",<br/>" + ClientAddress.ZipCode);
                strclient = strclient.Replace("{{PurposeOfVisit}}", (Report.Service.PurposeOfVisit));
                strclient = strclient.Replace("{{Date}}", Report.Service.ScheduleDate.Value.ToString("MM/dd/yyyy"));
                strclient = strclient.Replace("{{Technician}}", Report.Service.Employee.FirstName + " " + Report.Service.Employee.LastName);
                //strclient = strclient.Replace("{{ActualStartTime}}", Report.WorkStartedTime);
                //strclient = strclient.Replace("{{ActualEndTime}}", Report.WorkCompletedTime);
                strclient = strclient.Replace("{{WorkPerformed}}", Report.WorkPerformed);
                strclient = strclient.Replace("{{RecommendationsForCustomer}}", (string.IsNullOrWhiteSpace(Report.Recommendationsforcustomer) ? "NA" : Report.Recommendationsforcustomer));
                strclient = strclient.Replace("{{EmployeeNotes}}", (string.IsNullOrWhiteSpace(Report.EmployeeNotes) ? "NA" : Report.EmployeeNotes));

                var strUnits = "";
                if (service.IsNoShow == false)
                {
                    foreach (var item in Report.ServiceReportUnits.Where(x => x.IsCompleted == true).ToList())
                    {
                        var planTypeId = item.ClientUnit.PlanTypeId;
                        var planName = db.SubscriptionPlans.FirstOrDefault(p => p.Id == planTypeId).PlanName;
                        strUnits = strUnits + "<tr><td>" + item.ClientUnit.UnitName + " " + planName + "</td></tr>";
                    }
                }
                else
                {
                    foreach (var item in service.ServiceUnits.ToList())
                    {
                        var planTypeId = item.ClientUnit.PlanTypeId;
                        var planName = db.SubscriptionPlans.FirstOrDefault(p => p.Id == planTypeId).PlanName;
                        strUnits = strUnits + "<tr><td>" + item.ClientUnit.UnitName + " " + planName + "</td></tr>";
                    }
                }
                if (!string.IsNullOrWhiteSpace(strUnits))
                {
                    strUnits = @"<table style='width: 330px;border-collapse: collapse;border: gray 1px solid;'>" + strUnits + "</table>";
                }
                else
                {
                    strUnits = "NA";
                }
                strclient = strclient.Replace("{{UnitServiced}}", strUnits);
                var strPartsUsed = "";

                if (service.IsNoShow == false)
                {
                    var prts = Report.Service.ServicePartLists.Where(x => x.IsUsed == true).ToList();
                    for (int i = 0; i < prts.Count; i++)
                    {
                        var item = prts[i];
                        strPartsUsed = strPartsUsed + "<tr><td style='border: gray 1px solid;'>" + (i + 1).ToString() + "</td><td style='border: gray 1px solid;'>" + item.Part.Name + " - " + (item.Part.Size ?? "") + "</td><td style='border: gray 1px solid;'>" + item.UsedQuantity + "</td></tr>";
                    }
                }
                if (strPartsUsed != "")
                {
                    strPartsUsed = @"<table style='width: 330px;border-collapse: collapse;border: gray 1px solid;'>
                                                    <tr><td style='border: gray 1px solid;'>SR</td><td style='border: gray 1px solid;'>Part</td><td style='border: gray 1px solid;'>Qty</td></tr>" + strPartsUsed + "</table>";
                }
                else
                {
                    strPartsUsed = "NA";
                }
                strclient = strclient.Replace("{{MaterialUsed}}", strPartsUsed);

                var strPartRequested = "";

                var PartRequest = Report.EmployeePartRequestMasters.Select(x => new { Parts = x.EmployeePartRequests.Select(y => new { y.Part, y.RequestedQuantity }) }).ToList();

                int k = 0;
                for (int i = 0; i < PartRequest.Count; i++)
                {
                    var item = PartRequest[i];
                    for (int j = 0; j < item.Parts.Count(); j++)
                    {
                        var itm = item.Parts.ToList()[j];
                        strPartRequested = strPartRequested + "<tr><td>" + (k + 1).ToString() + "</td><td>" + itm.Part.Name + " - " + (itm.Part.Size ?? "") + "</td><td>" + itm.RequestedQuantity + "</td></tr>";
                    }
                }
                if (strPartRequested != "")
                {
                    strPartRequested = @"<table style='width: 330px;border-collapse: collapse;border: gray 1px solid;'>
                                                    <tr><td>SR</td><td>Part</td><td>Qty</td></tr>" + strPartRequested + "</table>";
                }
                else
                {
                    strPartRequested = "NA";
                }

                strclient = strclient.Replace("{{PartRequested}}", strPartRequested);

                var strImage = "";
                if (Report.ServiceReportImages.Count > 0)
                {
                    strImage += "<tr>";
                    int j = 1;
                    foreach (var item in Report.ServiceReportImages)
                    {
                        strImage = strImage + "<td style='width:175px;'><img style='width:175px' src='" + ConfigurationManager.AppSettings["ReportEmailImageURL"] + item.ServiceImage + "' /></td>";
                        if ((j % 2) == 0)
                        {
                            strImage += "</tr>";
                            if (Report.ServiceReportImages.Count > j)
                            {
                                strImage += "<tr>";
                            }
                        }
                        j++;
                    }
                    if (!strImage.EndsWith("</tr>"))
                        strImage += "</tr>";

                    strImage = "<table style='width: 350px;'>" + strImage + "</table>";
                }
                else
                {
                    strImage = "NA";
                }
                strclient = strclient.Replace("{{Images}}", strImage);

                string strClientSignature = "";
                if (!string.IsNullOrWhiteSpace(Report.ClientSignature))
                {
                    strClientSignature = "<img style='width:175px' src='" + ConfigurationManager.AppSettings["ClientEmailSignatureURL"] + Report.ClientSignature + "' />";
                }
                strclient = strclient.Replace("{{ClientSignature}}", strClientSignature);

                strclient = @"<html>
                            <head>
                                <title></title>    
                            </head>
                            <body style='width: 540px;'>" + strclient + @"</body></html>";

                //var pdf = PdfGenerator.GeneratePdf(strclient, new PdfGenerateConfig() { MarginBottom = 0, MarginLeft = 0, MarginTop = 0, MarginRight = 0, PageSize = PdfSharp.PageSize.A4, PageOrientation = PageOrientation.Portrait });
                //string fname = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["reportsFilePath"].ToString()) +
                //    Report.ServiceReportNumber + "-" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".pdf";
                //pdf.Save(fname);
                if (request.ClientEmails.Count > 0)
                {
                    Report.CCEmail = string.Join(";", request.ClientEmails);
                }
                //api.App_Start.Utilities.Send(sub, UserInfo.Email, strclient, templateclient.FromEmail, db, fname, Report.CCEmail);

                BackgroundTaskManager.Run(async () =>
                {
                    await api.App_Start.Utilities.SendAsync(sub, Report.CCEmail, strclient, templateclient.FromEmail, db, PrintOrdersToPdf(Report), "");
                });

                res.Message = "Report sent successfully";
                res.Data = null;
                res.StatusCode = HttpStatusCode.OK.GetEnumValue();
            }
            catch (Exception ex)
            {
                res.Data = null;
                res.Message = ex.Message;
                res.StatusCode = HttpStatusCode.InternalServerError.GetEnumValue();
            }
            db.Dispose();
            return Ok(res);
        }

        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("ResendOrder")]
        public async Task<IHttpActionResult> ResendOrder([FromBody] EmpCommonModel request)
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
            var orderMail = db.Orders.Find(request.OrderId);
            var BillingHistory1 = orderMail.BillingHistories.ToList();
            if (BillingHistory1.Count <= 0)
            {
                EmailTemplate templateclient = db.EmailTemplates.Where(x => x.Name == "PartOrderReceiptChequeClient" && x.Status == true).FirstOrDefault();
                var strclient = templateclient.EmailBody;
                var LoginUrl = ConfigurationManager.AppSettings["SiteAddress"].ToString() + "sign-in.aspx";
                strclient = strclient.Replace("{{FirstName}}", orderMail.Client.FirstName);
                strclient = strclient.Replace("{{LastName}}", orderMail.Client.LastName);
                strclient = strclient.Replace("{{Email}}", orderMail.Client.Email);
                strclient = strclient.Replace("{{PhoneNumber}}", orderMail.Client.MobileNumber);
                strclient = strclient.Replace("{{OrderDate}}", orderMail.AddedDate.ToString("MM/dd/yyyy"));
                var stritems = "";
                var total = 0m;
                if (orderMail.OrderItems.Count > 0)
                {
                    for (int i = 0; i < orderMail.OrderItems.Count; i++)
                    {
                        var item = orderMail.OrderItems.ToList()[i];
                        total += item.Quantity * item.Amount.Value;
                        stritems = stritems + "<tr><td>" + (i + 1).ToString() + "</td><td>" + item.PartName + " - " + item.PartSize + "</td><td>" + item.Quantity + "</td><td align='right'>" + item.Amount + "</td><td align='right'>" + item.Quantity * item.Amount.Value + "</td></tr>";
                    }
                    stritems = "<table style='width:100%;font-family:Arial,sans-serif;font-size:14px;line-height:21px;color:#333;border-color: #e2e2e2;border-collapse: collapse;'><tr><td>SR</td><td>Part</td><td>Qty</td><td>Rate</td><td>Total</td></tr>" + stritems + "<tr><td colspan='4' align='right'>Total</td><td align='right'>$" + total.ToString("0.00") + "</td></tr></table>";
                }
                else
                {
                    var billingHistory = orderMail.BillingHistories.ToList().FirstOrDefault();
                    var orderItems = orderMail.BillingHistories.Select(x => new
                    {
                        PartName = "",
                        PartSize = 1,
                        Quantity = 1,
                        Amount = x.PurchasedAmount
                    }).ToList();
                    for (int i = 0; i < orderMail.OrderItems.Count; i++)
                    {
                        var item = orderMail.OrderItems.ToList()[i];
                        total += item.Quantity * item.Amount.Value;
                        stritems = stritems + "<tr><td>" + (i + 1).ToString() + "</td><td>" + item.PartName + " - " + item.PartSize + "</td><td>" + item.Quantity + "</td><td align='right'>" + item.Amount + "</td></tr>";
                    }
                    stritems = @"<table style='width:100%;font-family:Arial,sans-serif;font-size:14px;line-height:21px;color:#333;
                             border-color: #e2e2e2;border-collapse: collapse;'><tr><td>SR</td><td>Unit Name</td><td>Unit Type</td><td>Amount</td></tr>" + stritems + "<tr><td colspan='3' align='right'>Total</td><td align='right'>$" + total.ToString("0.00") + "</td></tr></table>";
                }
                strclient = strclient.Replace("{{orderitems}}", stritems);
                if (request.ClientEmails.Count > 0)
                {
                    request.CCEmail = string.Join(";", request.ClientEmails);
                }
                api.App_Start.Utilities.Send(templateclient.EmailTemplateSubject, orderMail.Client.Email, strclient, templateclient.FromEmail, db, "", request.CCEmail);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                var total = 0m;
                //var PendingProcessUnits = BillingHistory1.Select(x => x.ClientUnit).ToList();
                EmailTemplate templateclient = db.EmailTemplates.Where(x => x.Name == "UnitOrderClient" && x.Status == true).FirstOrDefault();
                var strclient = templateclient.EmailBody;
                var sub = templateclient.EmailTemplateSubject;
                //var ClientAddress = PendingProcessUnits.FirstOrDefault().ClientAddress;
                var UserInfo = orderMail.Client;
                strclient = strclient.Replace("{{ClientName}}", UserInfo.FirstName + " " + UserInfo.LastName);
                strclient = strclient.Replace("{{Address}}", BillingHistory1.FirstOrDefault().BillingAddress + ",<br/>" + BillingHistory1.FirstOrDefault().City.Name + ", " + BillingHistory1.FirstOrDefault().State.Name + ",<br/>" + BillingHistory1.FirstOrDefault().BillingZipcode);
                strclient = strclient.Replace("{{PurchasedDate}}", orderMail.AddedDate.ToString("MM/dd/yyyy"));


                sb.Append("<table style='border-collapse: collapse;'>");
                sb.Append("<tr>");
                sb.Append("<td>");
                sb.Append("Unit Name");
                sb.Append("</td>");
                sb.Append("<td>");
                sb.Append("Address");
                sb.Append("</td>");
                sb.Append("<td>");
                sb.Append("Plan");
                sb.Append("</td>");
                sb.Append("<td>");
                sb.Append("Rate");
                sb.Append("</td>");

                sb.Append("</tr>");
                foreach (var item in BillingHistory1)
                {
                    total = total + item.PurchasedAmount.Value;
                    var clientunit = db.ClientUnits.Where(x => x.Id == 1).FirstOrDefault();
                    var ClientAddress = clientunit.ClientAddress;
                    sb.Append("<tr>");
                    sb.Append("<td>");
                    sb.Append(clientunit.UnitName);
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(ClientAddress.Address + ",<br/>" + ClientAddress.City1.Name + ", " + ClientAddress.State1.Name + ",<br/>" + ClientAddress.ZipCode);
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append("");
                    sb.Append("</td>");
                    sb.Append("<td>");
                    sb.Append(item.PurchasedAmount);
                    sb.Append("</td>");
                    sb.Append("</tr>");
                }
                sb.Append("<tr>");
                sb.Append("<td colspan='3'>");
                sb.Append("Total");
                sb.Append("</td>");
                sb.Append("<td>");
                sb.Append(total.ToString("0.00"));
                sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("</table>");
                //strclient = strclient.Replace("{{Amount}}", total.ToString("0.00"));
                strclient = strclient.Replace("{{UnitsPurchased}}", sb.ToString());
                if (request.ClientEmails.Count > 0)
                {
                    request.CCEmail = string.Join(";", request.ClientEmails);
                }
                api.App_Start.Utilities.Send(templateclient.EmailTemplateSubject, request.CCEmail, strclient, templateclient.FromEmail, db, "", "");
            }
            res.Message = "Order sent successfully";
            res.Data = null;
            res.StatusCode = HttpStatusCode.OK.GetEnumValue();
            db.Dispose();
            return Ok(res);
        }

        [HttpGet]
        [Route("ResendReport1")]
        public async Task<IHttpActionResult> ResendReport1([FromUri] int ReportId, [FromUri] string CCEmail)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            var Report = db.ServiceReports.Find(ReportId);
            var service = Report.Service;
            var UserInfo = Report.Service.Client;//db.Clients.Find(Report.Service.ClientId);

            var ClientAddress = Report.ServiceReportUnits.FirstOrDefault().ClientUnit.ClientAddress;
            EmailTemplate templateclient = db.EmailTemplates.Where(x => x.Name == "ServiceReportClient" && x.Status == true).FirstOrDefault();
            var strclient = templateclient.EmailBody;
            //var sub = templateclient.EmailTemplateSubject + " - " + Report.Service.ServiceCaseNumber + (service.IsNoShow == true ? "(No Show Service)" : "");
            var sub = templateclient.EmailTemplateSubject.Replace("{{Address}}", Report.Service.ClientAddress.Address + (service.IsNoShow == true ? "(No Show Service)" : ""));
            sub = sub.Replace("{{ServiceCaseNumber}}", service.ServiceCaseNumber);
            strclient = strclient.Replace("{{ServiceReport}}", Report.ServiceReportNumber + (service.IsNoShow == true ? "(No Show Service)" : ""));
            strclient = strclient.Replace("{{ContactName}}", UserInfo.FirstName + " " + UserInfo.LastName);
            strclient = strclient.Replace("{{Company}}", UserInfo.Company ?? "-");
            strclient = strclient.Replace("{{Address}}", ClientAddress.Address + ",<br/>" + ClientAddress.City1.Name + ", " + ClientAddress.State1.Name + ",<br/>" + ClientAddress.ZipCode);
            strclient = strclient.Replace("{{PurposeOfVisit}}", (Report.Service.PurposeOfVisit));
            strclient = strclient.Replace("{{Date}}", Report.Service.ScheduleDate.Value.ToString("MM/dd/yyyy"));
            strclient = strclient.Replace("{{Technician}}", Report.Service.Employee.FirstName + " " + Report.Service.Employee.LastName);
            //strclient = strclient.Replace("{{ActualStartTime}}", Report.WorkStartedTime);
            //strclient = strclient.Replace("{{ActualEndTime}}", Report.WorkCompletedTime);
            strclient = strclient.Replace("{{WorkPerformed}}", Report.WorkPerformed);
            strclient = strclient.Replace("{{RecommendationsForCustomer}}", (string.IsNullOrWhiteSpace(Report.Recommendationsforcustomer) ? "NA" : Report.Recommendationsforcustomer));
            strclient = strclient.Replace("{{EmployeeNotes}}", (string.IsNullOrWhiteSpace(Report.EmployeeNotes) ? "NA" : Report.EmployeeNotes));

            var strUnits = "";
            if (service.IsNoShow == false)
            {
                foreach (var item in Report.ServiceReportUnits.Where(x => x.IsCompleted == true).ToList())
                {
                    var planTypeId = item.ClientUnit.PlanTypeId;
                    var planName = db.SubscriptionPlans.FirstOrDefault(p => p.Id == planTypeId).PlanName;
                    strUnits = strUnits + "<tr><td>" + item.ClientUnit.UnitName + " " + planName + "</td></tr>";
                }
            }
            else
            {
                foreach (var item in service.ServiceUnits.ToList())
                {
                    var planTypeId = item.ClientUnit.PlanTypeId;
                    var planName = db.SubscriptionPlans.FirstOrDefault(p => p.Id == planTypeId).PlanName;
                    strUnits = strUnits + "<tr><td>" + item.ClientUnit.UnitName + " " + planName + "</td></tr>";
                }
            }
            if (!string.IsNullOrWhiteSpace(strUnits))
            {
                strUnits = @"<table style='width: 330px;border-collapse: collapse;border: gray 1px solid;'>" + strUnits + "</table>";
            }
            else
            {
                strUnits = "NA";
            }
            strclient = strclient.Replace("{{UnitServiced}}", strUnits);
            var strPartsUsed = "";

            if (service.IsNoShow == false)
            {
                var prts = Report.Service.ServicePartLists.Where(x => x.IsUsed == true).ToList();
                for (int i = 0; i < prts.Count; i++)
                {
                    var item = prts[i];
                    strPartsUsed = strPartsUsed + "<tr><td style='border: gray 1px solid;'>" + (i + 1).ToString() + "</td><td style='border: gray 1px solid;'>" + item.Part.Name + " - " + (item.Part.Size ?? "") + "</td><td style='border: gray 1px solid;'>" + item.UsedQuantity + "</td></tr>";
                }
            }
            if (strPartsUsed != "")
            {
                strPartsUsed = @"<table style='width: 330px;border-collapse: collapse;border: gray 1px solid;'>
                                                    <tr><td style='border: gray 1px solid;'>SR</td><td style='border: gray 1px solid;'>Part</td><td style='border: gray 1px solid;'>Qty</td></tr>" + strPartsUsed + "</table>";
            }
            else
            {
                strPartsUsed = "NA";
            }
            strclient = strclient.Replace("{{MaterialUsed}}", strPartsUsed);

            var strPartRequested = "";

            var PartRequest = Report.EmployeePartRequestMasters.Select(x => new { Parts = x.EmployeePartRequests.Select(y => new { y.Part, y.RequestedQuantity }) }).ToList();

            int k = 0;
            for (int i = 0; i < PartRequest.Count; i++)
            {
                var item = PartRequest[i];
                for (int j = 0; j < item.Parts.Count(); j++)
                {
                    var itm = item.Parts.ToList()[j];
                    strPartRequested = strPartRequested + "<tr><td style='border: gray 1px solid;'>" + (k + 1).ToString() + "</td><td style='border: gray 1px solid;'>" + itm.Part.Name + " - " + (itm.Part.Size ?? "") + "</td><td style='border: gray 1px solid;'>" + itm.RequestedQuantity + "</td></tr>";
                }
            }
            if (strPartRequested != "")
            {
                strPartRequested = @"<table style='width: 330px;border-collapse: collapse;border: gray 1px solid;'>
                                                    <tr><td style='border: gray 1px solid;'>SR</td><td style='border: gray 1px solid;'>Part</td><td style='border: gray 1px solid;'>Qty</td></tr>" + strPartRequested + "</table>";
            }
            else
            {
                strPartRequested = "NA";
            }

            strclient = strclient.Replace("{{PartRequested}}", strPartRequested);

            var strImage = "";
            if (Report.ServiceReportImages.Count > 0)
            {
                strImage += "<tr>";
                int j = 1;
                foreach (var item in Report.ServiceReportImages)
                {
                    strImage = strImage + "<td style='width:175px;'><img style='width:175px' src='" + ConfigurationManager.AppSettings["ReportEmailImageURL"] + item.ServiceImage + "' /></td>";
                    if ((j % 2) == 0)
                    {
                        strImage += "</tr>";
                        if (Report.ServiceReportImages.Count > j)
                        {
                            strImage += "<tr>";
                        }
                    }
                    j++;
                }
                if (!strImage.EndsWith("</tr>"))
                    strImage += "</tr>";

                strImage = "<table style='width: 350px;'>" + strImage + "</table>";
            }
            else
            {
                strImage = "NA";
            }
            strclient = strclient.Replace("{{Images}}", strImage);
            string strClientSignature = "";
            if (!string.IsNullOrWhiteSpace(Report.ClientSignature))
            {
                strClientSignature = "<img style='width:175px' src='" + ConfigurationManager.AppSettings["ClientEmailSignatureURL"] + Report.ClientSignature + "' />";
            }
            strclient = strclient.Replace("{{ClientSignature}}", strClientSignature);
            strclient = @"<html>
                            <head>
                                <title></title>    
                            </head>
                            <body style='width: 540px;'>" + strclient + @"</body></html>";

            //PdfDocument pdf = PdfGenerator.GeneratePdf(strclient, PageSize.A4);
            //PdfSharp.Pdf.PdfDocument pdf = PdfGenerator.GeneratePdf(strclient, new PdfGenerateConfig() { MarginBottom = 0, MarginLeft = 0, MarginTop = 0, MarginRight = 0, PageSize = PdfSharp.PageSize.A4, PageOrientation = PageOrientation.Portrait });

            //try
            //{
            //    api.App_Start.Utilities.Send(sub, UserInfo.Email, strclient, templateclient.FromEmail, db, PrintOrdersToPdf(Report), CCEmail);
            //}
            //catch (Exception ex)
            //{
            //    return Ok(ex);
            //}
            BackgroundTaskManager.Run(async () =>
            {
                await api.App_Start.Utilities.SendAsync(sub, CCEmail, strclient, templateclient.FromEmail, db, PrintOrdersToPdf(Report), "");
            });


            res.Message = "Report sent successfully";
            res.Data = null;
            res.StatusCode = HttpStatusCode.OK.GetEnumValue();
            db.Dispose();
            return Ok(res);
        }

        [HttpGet]
        [Route("ResendOrder1")]
        public string ResendOrder1([FromUri] int OrderId, [FromUri] string CCEmail)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            var orderMail = db.Orders.Find(OrderId);
            EmailTemplate templateclient = db.EmailTemplates.Where(x => x.Name == "PartOrderReceiptChequeClient" && x.Status == true).FirstOrDefault();
            var strclient = templateclient.EmailBody;
            var LoginUrl = ConfigurationManager.AppSettings["SiteAddress"].ToString() + "sign-in.aspx";
            strclient = strclient.Replace("{{FirstName}}", orderMail.Client.FirstName);
            strclient = strclient.Replace("{{LastName}}", orderMail.Client.LastName);
            strclient = strclient.Replace("{{Email}}", orderMail.Client.Email);
            strclient = strclient.Replace("{{PhoneNumber}}", orderMail.Client.MobileNumber);
            strclient = strclient.Replace("{{OrderDate}}", orderMail.AddedDate.ToString("MM/dd/yyyy"));
            //"<tr><td>SR</td><td>Part</td><td>Qty</td><td>Rate</td></tr>"
            var stritems = "";
            var total = 0m;
            if (orderMail.OrderItems.Count > 0)
            {
                for (int i = 0; i < orderMail.OrderItems.Count; i++)
                {
                    var item = orderMail.OrderItems.ToList()[i];
                    total += item.Quantity * item.Amount.Value;
                    stritems = stritems + "<tr><td>" + (i + 1).ToString() + "</td><td>" + item.PartName + " - " + item.PartSize + "</td><td>" + item.Quantity + "</td><td align='right'>" + item.Amount + "</td><td align='right'>" + item.Quantity * item.Amount.Value + "</td></tr>";
                }
                stritems = "<table style='width:100%;font-family:Arial,sans-serif;font-size:14px;line-height:21px;color:#333;border-color: #e2e2e2;border-collapse: collapse;'><tr><td>SR</td><td>Part</td><td>Qty</td><td>Rate</td><td>Total</td></tr>" + stritems + "<tr><td colspan='4' align='right'>Total</td><td align='right'>$" + total.ToString("0.00") + "</td></tr></table>";
            }
            else
            {
                var billingHistory = orderMail.BillingHistories.ToList().FirstOrDefault();
                var orderItems = orderMail.BillingHistories.Select(x => new
                {
                    PartName = "",
                    PartSize = 1,
                    Quantity = 1,
                    Amount = x.PurchasedAmount
                }).ToList();
                for (int i = 0; i < orderMail.OrderItems.Count; i++)
                {
                    var item = orderMail.OrderItems.ToList()[i];
                    total += item.Quantity * item.Amount.Value;
                    stritems = stritems + "<tr><td>" + (i + 1).ToString() + "</td><td>" + item.PartName + " - " + item.PartSize + "</td><td>" + item.Quantity + "</td><td align='right'>" + item.Amount + "</td></tr>";
                }
                stritems = @"<table style='width:100%;font-family:Arial,sans-serif;font-size:14px;line-height:21px;color:#333;
                             border-color: #e2e2e2;border-collapse: collapse;'><tr><td>SR</td><td>Unit Name</td><td>Unit Type</td><td>Amount</td></tr>" + stritems + "<tr><td colspan='3' align='right'>Total</td><td align='right'>$" + total.ToString("0.00") + "</td></tr></table>";
            }
            strclient = strclient.Replace("{{orderitems}}", stritems);
            api.App_Start.Utilities.Send(templateclient.EmailTemplateSubject, CCEmail, strclient, templateclient.FromEmail, db, "", "");
            res.Message = "Order sent successfully";
            res.Data = null;
            res.StatusCode = HttpStatusCode.OK.GetEnumValue();
            db.Dispose();
            return "ok";
        }

        [HttpGet]
        [Route("sendfiles")]
        public string sendfiles()
        {
            db = new Aircall_DBEntities1();
            var Report = db.ServiceReports.Find(1);
            //PdfDocument pdf = PdfGenerator.GeneratePdf(str, new PdfGenerateConfig() { MarginBottom = 0, MarginLeft = 0, MarginTop = 0, MarginRight = 0, PageSize = PageSize.A4, PageOrientation = PageOrientation.Portrait });
            PrintOrdersToPdf(Report);
            //pdf.Save(fname);
            return "Upload Failed";
        }
        public string PrintOrdersToPdf(ServiceReport Report)
        {
            ResponseModel res = new ResponseModel();
            var service = Report.Service;
            var UserInfo = Report.Service.Client;

            string fname = ConfigurationManager.AppSettings["reportsFilePathFull"].ToString() + Report.ServiceReportNumber + "-" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".pdf";
            using (var fileStream = new FileStream(fname, FileMode.Create))
            {
                var pageSize = iTextSharp.text.PageSize.A4;

                var doc = new Document(pageSize);
                var pdfWriter = PdfWriter.GetInstance(doc, fileStream);
                doc.Open();
                //fonts
                var titleFont = GetFont();
                titleFont.SetStyle(Font.BOLD);
                titleFont.Color = BaseColor.BLACK;

                var titleFont1 = GetFont();
                titleFont1.SetStyle(Font.BOLD);
                titleFont1.Size = 18;
                titleFont1.Color = BaseColor.BLACK;


                var font = GetFont();

                #region Header

                //header
                var headerTable = new PdfPTable(2);
                headerTable.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                headerTable.DefaultCell.Border = Rectangle.NO_BORDER;

                var cellHeader = new PdfPCell(new Phrase("Service Report", titleFont1));
                cellHeader.Phrase.Add(new Phrase(Environment.NewLine));
                cellHeader.Phrase.Add(new Phrase(Environment.NewLine));
                cellHeader.HorizontalAlignment = Element.ALIGN_CENTER;
                cellHeader.VerticalAlignment = Element.ALIGN_TOP;
                cellHeader.Border = Rectangle.NO_BORDER;

                headerTable.AddCell(cellHeader);

                headerTable.SetWidths(new[] { 0.8f, 0.2f });
                headerTable.WidthPercentage = 100f;

                //logo
                var logoFilePath = ConfigurationManager.AppSettings["LogoPath"].ToString();
                var logo = Image.GetInstance(logoFilePath);
                logo.Alignment = Element.ALIGN_LEFT;
                logo.ScaleToFit(100f, 100f);

                var cellLogo = new PdfPCell();
                cellLogo.Border = Rectangle.NO_BORDER;
                cellLogo.AddElement(logo);
                cellLogo.VerticalAlignment = Element.ALIGN_TOP;
                headerTable.AddCell(cellLogo);
                doc.Add(headerTable);

                #endregion

                #region Addresses

                var addressTable = new PdfPTable(1);
                addressTable.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                addressTable.DefaultCell.Border = Rectangle.NO_BORDER;
                addressTable.WidthPercentage = 100f;

                //billing info
                var ReportDetailTable = new PdfPTable(2);
                ReportDetailTable.SetWidths(new[] { 30, 70 });
                ReportDetailTable.DefaultCell.Border = Rectangle.NO_BORDER;
                ReportDetailTable.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                var ClientAddress = Report.ServiceReportUnits.FirstOrDefault().ClientUnit.ClientAddress;

                // other info
                ReportDetailTable.AddCell(new Paragraph("Report Number : ", titleFont));
                ReportDetailTable.AddCell(new Paragraph(Report.ServiceReportNumber + (service.IsNoShow == true ? "(No Show Service)" : ""), font));

                ReportDetailTable.AddCell(new Paragraph("Client Name : ", titleFont));
                ReportDetailTable.AddCell(new Paragraph(UserInfo.FirstName + " " + UserInfo.LastName, font));

                ReportDetailTable.AddCell(new Paragraph("Company : ", titleFont));
                ReportDetailTable.AddCell(new Paragraph(UserInfo.Company ?? "-", font));

                ReportDetailTable.AddCell(new Paragraph("Service Address : ", titleFont));

                var addSub = new PdfPTable(1);
                addSub.DefaultCell.Border = Rectangle.NO_BORDER;
                addSub.RunDirection = PdfWriter.RUN_DIRECTION_LTR;

                addSub.AddCell(new Paragraph(ClientAddress.Address, font));
                addSub.AddCell(new Paragraph(ClientAddress.City1.Name + ", " + ClientAddress.State1.Name, font));
                addSub.AddCell(new Paragraph(ClientAddress.ZipCode, font));
                ReportDetailTable.AddCell(addSub);

                ReportDetailTable.AddCell(new Paragraph("Purpose Of Visit : ", titleFont));
                ReportDetailTable.AddCell(new Paragraph(Report.Service.PurposeOfVisit, font));

                ReportDetailTable.AddCell(new Paragraph("Service Date : ", titleFont));
                ReportDetailTable.AddCell(new Paragraph(Report.Service.ScheduleDate.Value.ToString("MM/dd/yyyy"), font));

                ReportDetailTable.AddCell(new Paragraph("Technician : ", titleFont));
                ReportDetailTable.AddCell(new Paragraph(Report.Service.Employee.FirstName + " " + Report.Service.Employee.LastName, font));

                //ReportDetailTable.AddCell(new Paragraph("Actual Start Time : ", titleFont));
                //ReportDetailTable.AddCell(new Paragraph(Report.WorkStartedTime, font));

                //ReportDetailTable.AddCell(new Paragraph("Actual End Time : ", titleFont));
                //ReportDetailTable.AddCell(new Paragraph(Report.WorkCompletedTime, font));

                ReportDetailTable.AddCell(new Paragraph("Work Performed : ", titleFont));
                ReportDetailTable.AddCell(new Paragraph(Report.WorkPerformed, font));

                ReportDetailTable.AddCell(new Paragraph("Recommendations for customer : ", titleFont));
                ReportDetailTable.AddCell(new Paragraph((string.IsNullOrWhiteSpace(Report.Recommendationsforcustomer) ? "NA" : Report.Recommendationsforcustomer), font));

                addressTable.AddCell(ReportDetailTable);
                doc.Add(addressTable);
                doc.Add(new Paragraph(" "));

                #endregion

                #region Units

                //Untis
                var productsHeader = new PdfPTable(1);
                productsHeader.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                productsHeader.WidthPercentage = 100f;
                var cellProducts = new PdfPCell(new Phrase("Unit Serviced", titleFont));
                cellProducts.Border = Rectangle.NO_BORDER;
                productsHeader.AddCell(cellProducts);
                doc.Add(productsHeader);

                var productsTable = new PdfPTable(1);
                productsTable.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                productsTable.WidthPercentage = 100f;

                //SR
                var cellProductItemunits = new PdfPCell(new Phrase("Unit Name", font));
                cellProductItemunits.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellProductItemunits.HorizontalAlignment = Element.ALIGN_CENTER;
                productsTable.AddCell(cellProductItemunits);

                List<string> units = new List<string>();
                if (service.IsNoShow == false)
                {
                    foreach (var item in Report.ServiceReportUnits.Where(x => x.IsCompleted == true).ToList())
                    {
                        var planTypeId = item.ClientUnit.PlanTypeId;
                        var planName = db.SubscriptionPlans.FirstOrDefault(p => p.Id == planTypeId).PlanName;
                        units.Add(item.ClientUnit.UnitName + " " + planName);
                    }
                }
                else
                {
                    foreach (var item in service.ServiceUnits.ToList())
                    {
                        var planTypeId = item.ClientUnit.PlanTypeId;
                        var planName = db.SubscriptionPlans.FirstOrDefault(p => p.Id == planTypeId).PlanName;
                        units.Add(item.ClientUnit.UnitName + " " + planName);
                    }
                }
                foreach (var orderItem in units)
                {
                    cellProductItemunits = new PdfPCell(new Phrase(orderItem, font));
                    cellProductItemunits.HorizontalAlignment = Element.ALIGN_LEFT;
                    productsTable.AddCell(cellProductItemunits);
                }
                doc.Add(productsTable);
                doc.Add(new Paragraph(" "));
                #endregion

                #region MaterialUsed

                //MaterialUsed
                var MaterialUsedHeader = new PdfPTable(1);
                MaterialUsedHeader.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                MaterialUsedHeader.WidthPercentage = 100f;
                var cellMaterialUsed = new PdfPCell(new Phrase("Material Used", titleFont));
                cellMaterialUsed.Border = Rectangle.NO_BORDER;
                MaterialUsedHeader.AddCell(cellMaterialUsed);
                doc.Add(MaterialUsedHeader);

                var MaterialUsedTable = new PdfPTable(3);
                MaterialUsedTable.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                MaterialUsedTable.WidthPercentage = 100f;

                MaterialUsedTable.SetWidths(new[] { 20, 60, 20 });
                //SR
                var cellProductItem = new PdfPCell(new Phrase("SR", font));
                cellProductItem.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
                MaterialUsedTable.AddCell(cellProductItem);

                //Parts
                cellProductItem = new PdfPCell(new Phrase("Part", font));
                cellProductItem.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
                MaterialUsedTable.AddCell(cellProductItem);

                //Qty
                cellProductItem = new PdfPCell(new Phrase("Qty", font));
                cellProductItem.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
                MaterialUsedTable.AddCell(cellProductItem);

                if (service.IsNoShow == false)
                {
                    var prts = Report.Service.ServicePartLists.Where(x => x.IsUsed == true).ToList();
                    for (int i = 0; i < prts.Count; i++)
                    {
                        var item = prts[i];

                        //SR
                        cellProductItem = new PdfPCell(new Phrase((i + 1).ToString(), font));
                        cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
                        MaterialUsedTable.AddCell(cellProductItem);

                        //Part
                        cellProductItem = new PdfPCell(new Phrase(item.Part.Name + " - " + (item.Part.Size ?? ""), font));
                        cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                        MaterialUsedTable.AddCell(cellProductItem);

                        //Qty
                        cellProductItem = new PdfPCell(new Phrase(item.UsedQuantity.Value.ToString(), font));
                        cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
                        MaterialUsedTable.AddCell(cellProductItem);
                    }
                    if (prts.Count == 0)
                    {
                        cellProductItem = new PdfPCell(new Phrase("NA", font));
                        cellProductItem.Colspan = 3;
                        MaterialUsedTable.AddCell(cellProductItem);
                    }
                }
                else
                {
                    cellProductItem = new PdfPCell(new Phrase("NA", font));
                    cellProductItem.Colspan = 3;
                    MaterialUsedTable.AddCell(cellProductItem);
                }
                doc.Add(MaterialUsedTable);
                doc.Add(new Paragraph(" "));
                #endregion

                #region PartRequest

                //PartRequest
                var PartRequestHeader = new PdfPTable(1);
                PartRequestHeader.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                PartRequestHeader.WidthPercentage = 100f;
                var cellPartRequestHead = new PdfPCell(new Phrase("Part Requested", titleFont));
                cellPartRequestHead.Border = Rectangle.NO_BORDER;
                PartRequestHeader.AddCell(cellPartRequestHead);
                doc.Add(PartRequestHeader);

                var PartRequestTable = new PdfPTable(3);
                PartRequestTable.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                PartRequestTable.WidthPercentage = 100f;

                PartRequestTable.SetWidths(new[] { 20, 60, 20 });
                //SR
                var cellPartRequest = new PdfPCell(new Phrase("SR", font));
                cellPartRequest.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellPartRequest.HorizontalAlignment = Element.ALIGN_CENTER;
                PartRequestTable.AddCell(cellPartRequest);

                //Parts
                cellPartRequest = new PdfPCell(new Phrase("Part", font));
                cellPartRequest.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellPartRequest.HorizontalAlignment = Element.ALIGN_CENTER;
                PartRequestTable.AddCell(cellPartRequest);

                //Qty
                cellPartRequest = new PdfPCell(new Phrase("Qty", font));
                cellPartRequest.BackgroundColor = BaseColor.LIGHT_GRAY;
                cellPartRequest.HorizontalAlignment = Element.ALIGN_CENTER;
                PartRequestTable.AddCell(cellPartRequest);


                var prts1 = Report.EmployeePartRequestMasters.Select(x => new { Parts = x.EmployeePartRequests.Select(y => new { y.Part, y.RequestedQuantity }) }).ToList();
                int k = 0;
                if (prts1.Count > 0)
                {
                    for (int i = 0; i < prts1.Count; i++)
                    {
                        var item = prts1[i];
                        for (int j = 0; j < item.Parts.Count(); j++)
                        {
                            var itm = item.Parts.ToList()[j];
                            //SR
                            cellPartRequest = new PdfPCell(new Phrase((k + 1).ToString(), font));
                            cellPartRequest.HorizontalAlignment = Element.ALIGN_CENTER;
                            PartRequestTable.AddCell(cellPartRequest);

                            //Part
                            cellPartRequest = new PdfPCell(new Phrase(itm.Part.Name + " - " + (itm.Part.Size ?? ""), font));
                            cellPartRequest.HorizontalAlignment = Element.ALIGN_LEFT;
                            PartRequestTable.AddCell(cellPartRequest);

                            //Qty
                            cellPartRequest = new PdfPCell(new Phrase(itm.RequestedQuantity.Value.ToString(), font));
                            cellPartRequest.HorizontalAlignment = Element.ALIGN_CENTER;
                            PartRequestTable.AddCell(cellPartRequest);
                        }
                    }
                }
                else
                {
                    cellPartRequest = new PdfPCell(new Phrase("NA", font));
                    cellPartRequest.Colspan = 3;
                    PartRequestTable.AddCell(cellPartRequest);
                }
                doc.Add(PartRequestTable);
                doc.Add(new Paragraph(" "));
                #endregion

                #region ReportImage

                var ReportImageHeader = new PdfPTable(1);
                ReportImageHeader.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                ReportImageHeader.WidthPercentage = 100f;
                var cellReportImageHead = new PdfPCell(new Phrase("Report Image", titleFont));
                cellReportImageHead.Border = Rectangle.NO_BORDER;
                ReportImageHeader.AddCell(cellReportImageHead);
                doc.Add(ReportImageHeader);

                var ReportImageTable = new PdfPTable(4);
                ReportImageTable.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                ReportImageTable.WidthPercentage = 100f;
                ReportImageTable.SetWidths(new[] { 25, 25, 25, 25 });

                if (Report.ServiceReportImages.Count > 0)
                {
                    //int j = Report.ServiceReportImages.Count - 4;
                    int mod = int.Parse(Math.Ceiling((double)(Report.ServiceReportImages.Count / 4m)).ToString());
                    //int finalcount = 4 / mod;
                    int dtemp = mod * 4;
                    int finalcount = dtemp - Report.ServiceReportImages.Count;
                    foreach (var item in Report.ServiceReportImages)
                    {
                        try
                        {
                            var reportImagePath = (ConfigurationManager.AppSettings["ReportEmailImagePathFull"].ToString()) + item.ServiceImage;
                            var reportImage = Image.GetInstance(reportImagePath);
                            reportImage.Alignment = Element.ALIGN_LEFT;

                            var cellreportImage = new PdfPCell();
                            cellreportImage.Border = Rectangle.NO_BORDER;
                            cellreportImage.AddElement(reportImage);
                            ReportImageTable.AddCell(cellreportImage);
                        }
                        catch (Exception ee)
                        {
                        }
                    }
                    for (int i = 0; i < finalcount; i++)
                    {
                        var cellreportImage = new PdfPCell(new Phrase("", font));
                        cellreportImage.Border = Rectangle.NO_BORDER;
                        ReportImageTable.AddCell(cellreportImage);
                    }
                }
                else
                {
                    var cellreportImage = new PdfPCell(new Phrase("NA", font));
                    cellreportImage.Colspan = 2;
                    ReportImageTable.AddCell(cellreportImage);
                }
                doc.Add(ReportImageTable);
                doc.Add(new Paragraph(" "));

                ReportImageHeader = new PdfPTable(1);
                ReportImageHeader.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                ReportImageHeader.WidthPercentage = 100f;
                cellReportImageHead = new PdfPCell(new Phrase("Client Signature", titleFont));
                cellReportImageHead = new PdfPCell(new Phrase("You acknowledge that the unit(s) listed are covered by the Plan Services as described in our agreement, that each unit was personally examined by you and that the unit(s) is/are in good and serviceable condition and that equipment described (hereinafter referred to as the “Equipment”) comprised the entirety of the unit.", font));

                cellReportImageHead.Border = Rectangle.NO_BORDER;
                ReportImageHeader.AddCell(cellReportImageHead);
                doc.Add(ReportImageHeader);

                var ClientSignatureTable = new PdfPTable(2);
                ClientSignatureTable.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
                ClientSignatureTable.WidthPercentage = 100f;
                ClientSignatureTable.SetWidths(new[] { 25, 75 });

                if (!string.IsNullOrWhiteSpace(Report.ClientSignature))
                {
                    var reportImagePath = (ConfigurationManager.AppSettings["ClientEmailSignaturePathFull"].ToString()) + Report.ClientSignature;
                    var reportImage = Image.GetInstance(reportImagePath);
                    reportImage.Alignment = Element.ALIGN_LEFT;

                    var cellreportImage = new PdfPCell();
                    cellreportImage.Border = Rectangle.NO_BORDER;
                    cellreportImage.AddElement(reportImage);
                    ClientSignatureTable.AddCell(cellreportImage);

                    cellreportImage = new PdfPCell(new Phrase("", font));
                    cellreportImage.Border = Rectangle.NO_BORDER;
                    ClientSignatureTable.AddCell(cellreportImage);
                    //strClientSignature = "<img style='width:175px' src='" + ConfigurationManager.AppSettings["ClientEmailSignatureURL"] + Report.ClientSignature + "' />";
                }
                doc.Add(ClientSignatureTable);
                doc.Add(new Paragraph(" "));
                #endregion
                doc.Close();
                return fname;
            }
        }
        protected virtual Font GetFont()
        {
            //nopCommerce supports unicode characters
            //nopCommerce uses Free Serif font by default (~/App_Data/Pdf/FreeSerif.ttf file)
            //It was downloaded from http://savannah.gnu.org/projects/freefont
            Font font = new Font();
            try
            {
                string fontPath = ConfigurationManager.AppSettings["PDFFontPath"].ToString();
                var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                font = new Font(baseFont, 10, Font.NORMAL);
            }
            catch (Exception ex)
            {
            }
            return font;
        }
    }
}
