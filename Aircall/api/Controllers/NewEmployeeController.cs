using api.ActionFilters;
using api.App_Start;
using api.Models;
using api.ViewModel;
using AutoMapper;
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
using System.Net.Http.Headers;
using System.IO;
using System.Text.RegularExpressions;
using Stripe;
using Nito.AspNetBackgroundTasks;
using Newtonsoft.Json;
using System.Threading;
using api.Common;
using System.Text;

namespace api.Controllers
{

    [RoutePrefix("v1/employee")]
    public class NewEmployeeController : BaseEmployeeApiController
    {
        private string generateUnitName(Aircall_DBEntities1 db, int ClientId, int cnt = 0)
        {
            var unitCount = (cnt == 0 ? db.ClientUnits.Where(x => x.ClientId == ClientId && x.IsDeleted == false).Count() : cnt) + 1;
            string name = "AC" + unitCount;
            var unitExists = db.ClientUnits.Where(x => x.UnitName == name && x.ClientId == ClientId).FirstOrDefault();
            if (unitExists != null)
            {
                name = generateUnitName(db, ClientId, unitCount);
            }
            return name;
        }

        Aircall_DBEntities1 db;
        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("AddUnitDetails")]
        public async Task<IHttpActionResult> AddUnitDetails([FromBody] EmpClientUnitAddModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            DateTime? mfgDate = new DateTime();
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
                    if (!string.IsNullOrWhiteSpace(request.UnitName))
                    {
                        var unitExists = db.ClientUnits.Where(x => x.UnitName == request.UnitName && x.ClientId == request.ClientId).FirstOrDefault();
                        if (unitExists != null)
                        {

                            res.StatusCode = (int)HttpStatusCode.Ambiguous;
                            res.Message = "Unit Name Already Exists.";
                            res.Data = null;

                            if (updatetoken)
                            {
                                res.Token = accessToken;
                            }
                            else
                            {
                                res.Token = "";
                            }
                            return Ok(res);
                        }
                    }
                    var unitCount = db.ClientUnits.Where(x => x.ClientId == request.ClientId && x.IsDeleted == false).LongCount() + 1;
                    var Unit = db.ClientUnits.Where(x => x.ClientId == request.ClientId).ToList();
                    var cunit = AutoMapper.Mapper.Map<ClientUnit>(request);
                    cunit.IsActive = true;
                    cunit.Notes = request.Notes;
                    if (request.AutoRenewal == true)
                    {
                        cunit.AutoRenewal = true;
                    }
                    if (request.SpecialOffer == true)
                    {
                        cunit.IsSpecialApplied = true;
                    }
                    else
                    {
                        cunit.IsSpecialApplied = false;
                    }
                    if (request.Parts != null)
                    {
                        if (request.Parts.Count == 1)
                        {
                            var op = request.Parts.FirstOrDefault();
                            if (op.UnitType.ToLower() == "packaged")
                            {
                                cunit.UnitTypeId = 1;
                            }
                            else if (op.UnitType.ToLower() == "heating")
                            {
                                cunit.UnitTypeId = 3;
                            }
                        }
                        else if (request.Parts.Count == 2)
                        {
                            cunit.UnitTypeId = 2;
                        }
                    }
                    var year = (request.Parts.FirstOrDefault().ManufactureDate == null ? 1 : DateTime.UtcNow.Year - request.Parts.FirstOrDefault().ManufactureDate.Value.Year);
                    Plan cUnitPlan = new Plan();
                    if (year < 10)
                    {
                        cUnitPlan = db.Plans.Where(x => x.PlanTypeId == request.PlanTypeId && x.PackageType == true && x.IsVisible == true && x.IsDeleted == false).FirstOrDefault();
                    }
                    if (year >= 10)
                    {
                        cUnitPlan = db.Plans.Where(x => x.PlanTypeId == request.PlanTypeId && x.PackageType == false && x.IsVisible == true && x.IsDeleted == false).FirstOrDefault();
                    }
                    cunit.CurrentPaymentMethod = request.CurrentPaymentMethod;
                    cunit.UnitName = (string.IsNullOrWhiteSpace(request.UnitName) ? generateUnitName(db, request.ClientId, 0) : request.UnitName);
                    cunit.PlanTypeId = request.PlanTypeId;
                    cunit.IsDeleted = false;
                    cunit.AddedBy = request.EmployeeId;
                    cunit.AddedByType = (int)Utilities.UserRoles.Employee;
                    cunit.Status = (int)Utilities.UnitStatus.ServiceSoon;
                    cunit.AddedDate = DateTime.UtcNow;
                    cunit.PaymentStatus = Utilities.UnitPaymentTypes.NotReceived.GetEnumDescription();
                    cunit.IsServiceAdded = false;
                    cunit.IsMatched = true;
                    db.ClientUnits.Add(cunit);

                    db.SaveChanges();

                    if (request.Parts != null)
                    {
                        if (request.Parts.Count > 0)
                        {
                            foreach (var ept in request.Parts)
                            {
                                ClientUnitPart cUnitPart = new ClientUnitPart();
                                if (ept.IsMatched)
                                {
                                    var mUnit = db.Units.Where(x => x.ModelNumber == ept.ModelNumber && x.Status == true && x.IsDeleted == false).FirstOrDefault();
                                    foreach (var item in mUnit.UnitManuals)
                                    {
                                        string sourcePath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["unitPartManualPath"].ToString());
                                        string targetPath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["ManualPath"].ToString());
                                        string sourceFile = System.IO.Path.Combine(sourcePath, item.ManualFileName);
                                        string destFile = System.IO.Path.Combine(targetPath, item.ManualFileName);
                                        // To copy a folder's contents to a new location:
                                        // Create a new target folder, if necessary.
                                        if (!System.IO.Directory.Exists(targetPath))
                                        {
                                            System.IO.Directory.CreateDirectory(targetPath);
                                        }
                                        // To copy a file to another location and 
                                        // overwrite the destination file if it already exists.
                                        System.IO.File.Copy(sourceFile, destFile, true);

                                        cunit.ClientUnitManuals.Add(new ClientUnitManual()
                                        {
                                            ManualName = item.ManualFileName,
                                            SplitType = ept.UnitType
                                        });
                                    }
                                    var mUnitP = db.uspa_Units_GetUnitsByModelNumber(mUnit.ModelNumber).FirstOrDefault();
                                    cUnitPart.BlowerMotor = (mUnitP.BlowerMotor > 0 ? mUnitP.BlowerMotor : cUnitPart.BlowerMotor);
                                    cUnitPart.Capacitor = (mUnitP.Capacitor > 0 ? mUnitP.Capacitor : cUnitPart.Capacitor);
                                    cUnitPart.Compressor = (mUnitP.Compressor > 0 ? mUnitP.Compressor : cUnitPart.Compressor);
                                    cUnitPart.Condensingfanmotor = (mUnitP.Condensingfanmotor > 0 ? mUnitP.Condensingfanmotor : cUnitPart.Condensingfanmotor);
                                    cUnitPart.Contactor = (mUnitP.Contactor > 0 ? mUnitP.Contactor : cUnitPart.Contactor);
                                    cUnitPart.Controlboard = (mUnitP.Controlboard > 0 ? mUnitP.Controlboard : cUnitPart.Controlboard);
                                    cUnitPart.Defrostboard = (mUnitP.Defrostboard > 0 ? mUnitP.Defrostboard : cUnitPart.Defrostboard);
                                    cUnitPart.Doorswitch = (mUnitP.Doorswitch > 0 ? mUnitP.Doorswitch : cUnitPart.Doorswitch);
                                    cUnitPart.ElectricalService = mUnitP.ElectricalService;
                                    cUnitPart.Filterdryer = (mUnitP.Filterdryer > 0 ? mUnitP.Filterdryer : cUnitPart.Filterdryer);
                                    cUnitPart.Flamesensor = (mUnitP.Flamesensor > 0 ? mUnitP.Flamesensor : cUnitPart.Flamesensor);
                                    cUnitPart.Gasvalve = (mUnitP.Gasvalve > 0 ? mUnitP.Gasvalve : cUnitPart.Gasvalve);
                                    cUnitPart.Ignitioncontrolboard = (mUnitP.Ignitioncontrolboard > 0 ? mUnitP.Ignitioncontrolboard : cUnitPart.Ignitioncontrolboard);
                                    cUnitPart.Ignitor = (mUnitP.Ignitor > 0 ? mUnitP.Ignitor : cUnitPart.Ignitor);
                                    cUnitPart.Inducerdraftmotor = (mUnitP.Inducerdraftmotor > 0 ? mUnitP.Inducerdraftmotor : cUnitPart.Inducerdraftmotor);
                                    cUnitPart.Limitswitch = (mUnitP.Limitswitch > 0 ? mUnitP.Limitswitch : cUnitPart.Limitswitch);
                                    cUnitPart.ManufactureBrand = mUnitP.ManufactureBrand;
                                    cUnitPart.ManufactureDate = ept.ManufactureDate;
                                    cUnitPart.ModelNumber = mUnitP.ModelNumber;
                                    cUnitPart.Pressureswitch = (mUnitP.Pressureswitch > 0 ? mUnitP.Pressureswitch : cUnitPart.Pressureswitch);
                                    cUnitPart.RefrigerantType = (mUnitP.Refrigerant > 0 ? mUnitP.Refrigerant : cUnitPart.RefrigerantType);
                                    cUnitPart.Relay = (mUnitP.Relay > 0 ? mUnitP.Relay : cUnitPart.Relay);
                                    cUnitPart.ReversingValve = (mUnitP.ReversingValve > 0 ? mUnitP.ReversingValve : cUnitPart.ReversingValve);
                                    cUnitPart.Rolloutsensor = (mUnitP.Rolloutsensor > 0 ? mUnitP.Rolloutsensor : cUnitPart.Rolloutsensor);
                                    cUnitPart.SerialNumber = ept.SerialNumber;
                                    cUnitPart.Transformer = (mUnitP.Transformer > 0 ? mUnitP.Transformer : cUnitPart.Transformer);
                                    cUnitPart.TXVValve = (mUnitP.TXVValve > 0 ? mUnitP.TXVValve : cUnitPart.TXVValve);
                                    cUnitPart.UnitTon = mUnitP.UnitTon;
                                    cUnitPart.Breaker = (mUnitP.Breaker > 0 ? mUnitP.Breaker : cUnitPart.Breaker);
                                    cUnitPart.MaxBreaker = mUnitP.MaxBreaker;
                                    cUnitPart.Coil = (mUnitP.Coil > 0 ? mUnitP.Coil : cUnitPart.Coil);
                                    cUnitPart.Misc = (mUnitP.Misc > 0 ? mUnitP.Misc : cUnitPart.Misc);
                                    cUnitPart.SplitType = ept.UnitType.Replace("Split-", "");
                                    cUnitPart.UnitId = cunit.Id;
                                    db.ClientUnitParts.Add(cUnitPart);
                                    db.SaveChanges();
                                    if (ept.OptionalInformation != null)
                                    {
                                        var oi = ept.OptionalInformation;
                                        if (oi.Filters.Count > 0)
                                        {
                                            var oldUnitExtraInfoes = db.UnitExtraInfoes.Where(x => x.ClientUnitPartId == cUnitPart.Id && x.ExtraInfoType == "Filter").ToList();
                                            db.UnitExtraInfoes.RemoveRange(oldUnitExtraInfoes);
                                            foreach (var uFilter in oi.Filters)
                                            {
                                                UnitExtraInfo uei = new UnitExtraInfo();
                                                uei.UnitId = cunit.Id;
                                                uei.PartId = uFilter.size;
                                                uei.ClientUnitPartId = cUnitPart.Id;
                                                uei.LocationOfPart = uFilter.LocationOfPart;
                                                uei.ExtraInfoType = "Filter";
                                                db.UnitExtraInfoes.Add(uei);
                                            }
                                            db.SaveChanges();
                                        }
                                        if (oi.FuseTypes.Count > 0)
                                        {
                                            var oldUnitExtraInfoes = db.UnitExtraInfoes.Where(x => x.ClientUnitPartId == cUnitPart.Id && x.ExtraInfoType == "Fuses").ToList();
                                            db.UnitExtraInfoes.RemoveRange(oldUnitExtraInfoes);
                                            foreach (var uFuseType in oi.FuseTypes)
                                            {
                                                UnitExtraInfo uei = new UnitExtraInfo();
                                                uei.UnitId = cunit.Id;
                                                uei.ClientUnitPartId = cUnitPart.Id;
                                                uei.PartId = uFuseType.FuseType;
                                                uei.ExtraInfoType = "Fuses";
                                                db.UnitExtraInfoes.Add(uei);
                                            }
                                            db.SaveChanges();
                                        }
                                        cUnitPart.Thermostat = (oi.ThermostatTypes == 0 ? null : oi.ThermostatTypes);
                                    }
                                    db.SaveChanges();
                                }
                                else
                                {
                                    Unit mUnit = new Unit();

                                    mUnit = AutoMapper.Mapper.Map<Unit>(ept);
                                    mUnit.AddedBy = request.EmployeeId;
                                    mUnit.AddedByType = Utilities.UserRoles.Employee.GetEnumValue();
                                    mUnit.AddedDate = DateTime.UtcNow;
                                    mUnit.Status = true;
                                    db.Units.Add(mUnit);
                                    db.SaveChanges();

                                    cUnitPart.BlowerMotor = mUnit.BlowerMotor;
                                    cUnitPart.Capacitor = mUnit.Capacitor;
                                    cUnitPart.Compressor = mUnit.Compressor;
                                    cUnitPart.Condensingfanmotor = mUnit.Condensingfanmotor;
                                    cUnitPart.Contactor = mUnit.Contactor;
                                    cUnitPart.Controlboard = mUnit.Controlboard;
                                    cUnitPart.Defrostboard = mUnit.Defrostboard;
                                    cUnitPart.Doorswitch = mUnit.Doorswitch;
                                    cUnitPart.ElectricalService = mUnit.ElectricalService;
                                    cUnitPart.Filterdryer = mUnit.Filterdryer;
                                    cUnitPart.Flamesensor = mUnit.Flamesensor;
                                    cUnitPart.Gasvalve = mUnit.Gasvalve;
                                    cUnitPart.Ignitioncontrolboard = mUnit.Ignitioncontrolboard;
                                    cUnitPart.Ignitor = mUnit.Ignitor;
                                    cUnitPart.Inducerdraftmotor = mUnit.Inducerdraftmotor;
                                    cUnitPart.Limitswitch = mUnit.Limitswitch;
                                    cUnitPart.ManufactureBrand = mUnit.ManufactureBrand;
                                    cUnitPart.ManufactureDate = ept.ManufactureDate;

                                    cUnitPart.ModelNumber = mUnit.ModelNumber;
                                    cUnitPart.Pressureswitch = mUnit.Pressureswitch;
                                    cUnitPart.RefrigerantType = mUnit.Refrigerant;
                                    cUnitPart.Relay = mUnit.Relay;
                                    cUnitPart.ReversingValve = mUnit.ReversingValve;
                                    cUnitPart.Rolloutsensor = mUnit.Rolloutsensor;
                                    cUnitPart.SerialNumber = ept.SerialNumber;
                                    cUnitPart.Transformer = mUnit.Transformer;
                                    cUnitPart.TXVValve = mUnit.TXVValve;
                                    cUnitPart.UnitTon = mUnit.UnitTon;
                                    cUnitPart.Breaker = (mUnit.Breaker == null ? null : mUnit.Breaker);
                                    cUnitPart.MaxBreaker = mUnit.MaxBreaker;

                                    cUnitPart.Coil = mUnit.Coil;
                                    cUnitPart.Misc = mUnit.Misc;

                                    cUnitPart.SplitType = ept.UnitType;

                                    cUnitPart.UnitId = cunit.Id;
                                    db.ClientUnitParts.Add(cUnitPart);
                                    db.SaveChanges();
                                    if (ept.OptionalInformation != null)
                                    {
                                        var oi = ept.OptionalInformation;
                                        if (oi.Filters.Count > 0)
                                        {
                                            var oldUnitExtraInfoes = db.UnitExtraInfoes.Where(x => x.ClientUnitPartId == cUnitPart.Id && x.ExtraInfoType == "Filter").ToList();
                                            db.UnitExtraInfoes.RemoveRange(oldUnitExtraInfoes);
                                            foreach (var uFilter in oi.Filters)
                                            {
                                                UnitExtraInfo uei = new UnitExtraInfo();
                                                uei.UnitId = cunit.Id;
                                                uei.PartId = uFilter.size;
                                                uei.ClientUnitPartId = cUnitPart.Id;
                                                uei.LocationOfPart = uFilter.LocationOfPart;
                                                uei.ExtraInfoType = "Filter";
                                                db.UnitExtraInfoes.Add(uei);
                                            }
                                             db.SaveChanges();
                                        }
                                        if (oi.FuseTypes.Count > 0)
                                        {
                                            var oldUnitExtraInfoes = db.UnitExtraInfoes.Where(x => x.ClientUnitPartId == cUnitPart.Id && x.ExtraInfoType == "Fuses").ToList();
                                            db.UnitExtraInfoes.RemoveRange(oldUnitExtraInfoes);
                                            foreach (var uFuseType in oi.FuseTypes)
                                            {
                                                UnitExtraInfo uei = new UnitExtraInfo();
                                                uei.UnitId = cunit.Id;
                                                uei.ClientUnitPartId = cUnitPart.Id;
                                                uei.PartId = uFuseType.FuseType;
                                                uei.ExtraInfoType = "Fuses";
                                                db.UnitExtraInfoes.Add(uei);
                                            }
                                            db.SaveChanges();
                                        }
                                        cUnitPart.Thermostat = (oi.ThermostatTypes == 0 ? null : oi.ThermostatTypes);
                                    }
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                    res.StatusCode = (int)HttpStatusCode.OK;
                    res.Message = "Record Saved.";
                    res.Data = cunit.Id;
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
        [Route("getElectricalServices")]
        public async Task<IHttpActionResult> getElectricalServices([FromBody] EmpCommonModel request)
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
                List<object> datas = new List<object>();

                string MaxBreaker = Utilities.GetSiteSettingValue("ElectricalServices", db);
                var splitMaxBreaker = MaxBreaker.Split((",").ToArray());
                foreach (var item in splitMaxBreaker)
                {
                    datas.Add(new { PartId = 0, Size = item, Name = "Electrical Service" });
                }

                //datas.Add(new { PartId = 0, Size = "230/1/60", Name = "Electrical Service" });
                //datas.Add(new { PartId = 0, Size = "230/3/60", Name = "Electrical Service" });
                //datas.Add(new { PartId = 0, Size = "460/3/60", Name = "Electrical Service" });
                //datas.Add(new { PartId = 0, Size = "120/1/60", Name = "Electrical Service" });

                res.Message = "Record Found";
                res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                res.Data = datas;
                if (updatetoken)
                {
                    res.Token = accessToken;
                }
                else
                {
                    res.Token = "";
                }
            }
            db.Dispose();
            return Ok(res);
        }

        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("getMaxBreaker")]
        public async Task<IHttpActionResult> getMaxBreaker([FromBody] EmpCommonModel request)
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
                List<object> datas = new List<object>();
                string MaxBreaker = Utilities.GetSiteSettingValue("MaxBreaker", db);
                var splitMaxBreaker = MaxBreaker.Split((",").ToArray());
                foreach (var item in splitMaxBreaker)
                {
                    datas.Add(new { PartId = 0, Size = item, Name = "Max Breaker" });
                }

                res.Message = "Record Found";
                res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                res.Data = datas;
                if (updatetoken)
                {
                    res.Token = accessToken;
                }
                else
                {
                    res.Token = "";
                }
            }
            db.Dispose();
            return Ok(res);
        }
        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("SubmitUnitImage")]
        public Task<IHttpActionResult> SubmitUnitImage()
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

            var uploadPath = new DirectoryInfo(HttpContext.Current.Server.MapPath("../../")).Parent.FullName + ConfigurationManager.AppSettings["UnitImagePath"].ToString();

            var multipartFormDataStreamProvider = new UploadMultipartFormProvider(uploadPath);
            var task = Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProvider).
            ContinueWith<IHttpActionResult>(t =>
            {
                db = new Aircall_DBEntities1();

                ServiceReporRequestModel emp = new ServiceReporRequestModel();
                if (t.IsFaulted || t.IsCanceled)
                {
                    //Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
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
                    var cId = multipartFormDataStreamProvider.FormData.GetValues("ClientId").FirstOrDefault();
                    var EmpId = multipartFormDataStreamProvider.FormData.GetValues("EmployeeId").FirstOrDefault();
                    int EmployeeId = int.Parse(EmpId);
                    {
                        if (IdValue != null)
                        {
                            int id = int.Parse(IdValue);

                            int ClientId = int.Parse(cId);
                            var rpt = db.ClientUnits.Find(id);
                            foreach (var file in filedata)
                            {
                                rpt.ClientUnitPictures.Add(new ClientUnitPicture() { UnitImage = new FileInfo(file.LocalFileName).Name, SplitType = file.Headers.ContentDisposition.Name.Replace("\"", "").Trim(), AddedBy = EmployeeId, AddedByType = Utilities.UserRoles.Employee.GetEnumValue(), AddedDate = DateTime.UtcNow });
                            }
                            db.SaveChanges();
                            var PendingProcessUnit = db.ClientUnits.Where(x => x.ClientId == ClientId && x.PaymentStatus == "NotReceived").AsEnumerable().Where(x => x.AddedByType == Utilities.UserRoles.Employee.GetEnumValue() && x.AddedBy == EmployeeId).ToList();
                            List<object> data1 = new List<object>();
                            decimal total = 0m;

                            foreach (var cunit1 in PendingProcessUnit)
                            {
                                var desc = db.SubscriptionPlans.FirstOrDefault(p => p.Id == cunit1.PlanTypeId).PlanName;
                                var PlanSelectedDisplay = new
                                {
                                    cunit1.UnitName,
                                    PlanName = desc,
                                    Description = desc.Substring(0, (desc.Length > 256 ? 256 : desc.Length)),
                                    Price = cunit1.PricePerMonth,
                                    PlanType = (cunit1.IsSpecialApplied == true ? "Special Offer" : "Recurring"),
                                    Id = cunit1.Id,
                                    cunit1.AddressId,
                                    cunit1.ClientId,
                                    ClientName = cunit1.Client.FirstName + " " + cunit1.Client.LastName,
                                    cunit1.CurrentPaymentMethod
                                };
                                total = total + cunit1.PricePerMonth.Value;
                                data1.Add(PlanSelectedDisplay);
                            }
                            var response = new
                            {
                                Units = data1,
                                Total = total,
                                Message = (PendingProcessUnit.Count(x => x.IsSpecialApplied == true) == PendingProcessUnit.Count ? "" : "(Recurring Billing occur every month)"),
                            };
                            res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                            res.Data = response;
                            res.Message = "Unit Added";
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
                db.Dispose();
                return Ok(res);
            });
            return task;
        }

        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("UpdateUnitImage")]
        public Task<IHttpActionResult> UpdateUnitImage()
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

            var uploadPath = new DirectoryInfo(HttpContext.Current.Server.MapPath("../../")).Parent.FullName + ConfigurationManager.AppSettings["UnitImagePath"].ToString();

            var multipartFormDataStreamProvider = new UploadMultipartFormProvider(uploadPath);
            var task = Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProvider).
            ContinueWith<IHttpActionResult>(t =>
            {
                db = new Aircall_DBEntities1();
                ServiceReporRequestModel emp = new ServiceReporRequestModel();
                if (t.IsFaulted || t.IsCanceled)
                {
                    //Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
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
                    var cId = multipartFormDataStreamProvider.FormData.GetValues("ClientId").FirstOrDefault();
                    var EmpId = multipartFormDataStreamProvider.FormData.GetValues("EmployeeId").FirstOrDefault();
                    int EmployeeId = int.Parse(EmpId);
                    {
                        if (IdValue != null)
                        {
                            int id = int.Parse(IdValue);

                            int ClientId = int.Parse(cId);
                            var rpt = db.ClientUnits.Find(id);
                            if (rpt.ClientUnitPictures.Count > 0)
                            {
                                db.ClientUnitPictures.RemoveRange(rpt.ClientUnitPictures);
                                db.SaveChanges();
                            }
                            foreach (var file in filedata)
                            {
                                rpt.ClientUnitPictures.Add(new ClientUnitPicture() { UnitImage = new FileInfo(file.LocalFileName).Name, SplitType = file.Headers.ContentDisposition.Name.Replace("\"", "").Trim(), AddedBy = EmployeeId, AddedByType = Utilities.UserRoles.Employee.GetEnumValue(), AddedDate = DateTime.UtcNow });
                            }
                            db.SaveChanges();

                            res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                            res.Data = null;
                            res.Message = "Unit Updated";
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
                db.Dispose();
                return Ok(res);
            });
            return task;
        }

        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("SubmitUnitPayment")]
        public Task<IHttpActionResult> SubmitUnitPayment()
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
            var uploadPath = new DirectoryInfo(HttpContext.Current.Server.MapPath("../../")).Parent.FullName + ConfigurationManager.AppSettings["ClientSignaturePath"].ToString();//HttpContext.Current.Server.MapPath("~/Uploads");

            var multipartFormDataStreamProvider = new UploadMultipartFormProvider(uploadPath);
            var task = Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProvider).
            ContinueWith<IHttpActionResult>(t =>
            {
                PaymentRequest request = new PaymentRequest();
                bool StripeError = false;
                string StripeErrMsg = "";
                db = new Aircall_DBEntities1();
                ServiceReporRequestModel emp = new ServiceReporRequestModel();
                ClientPaymentMethod cpm1 = new ClientPaymentMethod();
                if (t.IsFaulted || t.IsCanceled)
                {
                    //Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
                    res.StatusCode = HttpStatusCode.InternalServerError.GetEnumValue();
                    res.Data = null;
                    res.Message = "Internal Server Error";
                    db.Dispose();
                    return Ok(res);
                }
                try
                {
                    int ClientId = 0;
                    int EmployeeId = 0;
                    string Address = "";
                    int State = 0;
                    int City = 0;
                    string ZipCode = "";
                    string PhoneNumber = "";
                    string MobileNumber = "";
                    string FirstName = "";
                    string LastName = "";
                    string Company = "";
                    string CardType = "";
                    string NameOnCard = "";
                    string CardNumber = "";
                    int CVV = 0;
                    short ExpiryMonth = 0;
                    int ExpiryYear = 0;
                    string PaymentMethod = "";


                    try
                    {
                        PhoneNumber = multipartFormDataStreamProvider.FormData.GetValues("PhoneNumber").FirstOrDefault();
                        request.PhoneNumber = PhoneNumber;
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        MobileNumber = multipartFormDataStreamProvider.FormData.GetValues("MobileNumber").FirstOrDefault();
                        request.CCEmail = MobileNumber;
                    }
                    catch (Exception)
                    {
                    }

                    string CCEmail;
                    try
                    {
                        CCEmail = multipartFormDataStreamProvider.FormData.GetValues("CCEmail").FirstOrDefault();
                        request.CCEmail = CCEmail;
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        FirstName = multipartFormDataStreamProvider.FormData.GetValues("FirstName").FirstOrDefault();
                        request.FirstName = FirstName;
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        LastName = multipartFormDataStreamProvider.FormData.GetValues("LastName").FirstOrDefault();
                        request.LastName = LastName;
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        Company = multipartFormDataStreamProvider.FormData.GetValues("Company").FirstOrDefault();
                        request.Company = Company;
                    }
                    catch (Exception)
                    {
                    }

                    var cAddress = multipartFormDataStreamProvider.FormData.GetValues("Address").FirstOrDefault();
                    try
                    {
                        Address = cAddress;
                        request.Address = Address;
                    }
                    catch (Exception)
                    {
                    }

                    var cZipCode = multipartFormDataStreamProvider.FormData.GetValues("ZipCode").FirstOrDefault();
                    try
                    {
                        ZipCode = cZipCode;
                        request.ZipCode = ZipCode;
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        var cCardType = multipartFormDataStreamProvider.FormData.GetValues("CardType").FirstOrDefault();
                        CardType = cCardType;
                        request.CardType = CardType;
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        var cNameOnCard = multipartFormDataStreamProvider.FormData.GetValues("NameOnCard").FirstOrDefault();
                        NameOnCard = cNameOnCard;
                        request.NameOnCard = NameOnCard;
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        var cCardNumber = multipartFormDataStreamProvider.FormData.GetValues("CardNumber").FirstOrDefault();
                        string s = cCardNumber.Substring(cCardNumber.Length - 4);
                        CardNumber = s.PadLeft(16, '*');
                        request.CardNumber = CardNumber;
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        var cId = multipartFormDataStreamProvider.FormData.GetValues("ClientId").FirstOrDefault();
                        ClientId = int.Parse(cId);
                        request.ClientId = ClientId;
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        var EmpId = multipartFormDataStreamProvider.FormData.GetValues("EmployeeId").FirstOrDefault();
                        EmployeeId = int.Parse(EmpId);
                        request.EmployeeId = EmployeeId;
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        var CVVs = multipartFormDataStreamProvider.FormData.GetValues("CVV").FirstOrDefault();
                        CVV = int.Parse(CVVs);
                        request.CVV = CVV;
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        var ExpMonth = multipartFormDataStreamProvider.FormData.GetValues("ExpiryMonth").FirstOrDefault();
                        ExpiryMonth = short.Parse(ExpMonth);
                        request.ExpiryMonth = ExpiryMonth;
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        var ExpYear = multipartFormDataStreamProvider.FormData.GetValues("ExpiryYear").FirstOrDefault();
                        ExpiryYear = int.Parse(ExpYear);
                        request.ExpiryYear = ExpiryYear;
                    }
                    catch (Exception)
                    {
                    }

                    var cPaymentMethod = multipartFormDataStreamProvider.FormData.GetValues("PaymentMethod").FirstOrDefault();
                    try
                    {
                        PaymentMethod = cPaymentMethod;
                        request.PaymentMethod = PaymentMethod;
                    }
                    catch (Exception)
                    {
                    }

                    string ChequeNo = "";
                    try
                    {
                        ChequeNo = multipartFormDataStreamProvider.FormData.GetValues("ChequeNo").FirstOrDefault();
                        request.ChequeNo = ChequeNo;
                    }
                    catch (Exception ex)
                    {

                    }

                    string PONo = "";
                    try
                    {
                        PONo = multipartFormDataStreamProvider.FormData.GetValues("PONo").FirstOrDefault();
                        request.PONo = PONo;
                    }
                    catch (Exception ex)
                    {

                    }

                    string AccountingNotes = "";
                    try
                    {
                        AccountingNotes = multipartFormDataStreamProvider.FormData.GetValues("AccountingNotes").FirstOrDefault();
                        request.AccountingNotes = AccountingNotes;
                    }
                    catch (Exception ex)
                    {
                    }

                    string ChequeDate = "";
                    try
                    {
                        ChequeDate = multipartFormDataStreamProvider.FormData.GetValues("ChequeDate").FirstOrDefault();
                        request.ChequeDate = DateTime.Parse(ChequeDate);
                    }
                    catch (Exception ex)
                    {
                    }

                    var StateId = multipartFormDataStreamProvider.FormData.GetValues("State").FirstOrDefault();
                    try
                    {
                        State = int.Parse(StateId);
                        request.State = State;
                    }
                    catch (Exception)
                    {
                    }

                    var CityId = multipartFormDataStreamProvider.FormData.GetValues("City").FirstOrDefault();
                    try
                    {
                        City = int.Parse(CityId);
                        request.City = City;
                    }
                    catch (Exception)
                    {
                    }
                    string ChqueImageFront = "";
                    string ChequeImageBack = "";

                    try
                    {
                        ChqueImageFront = multipartFormDataStreamProvider.FileData.Where(x => x.Headers.ContentDisposition.Name.ToLower().Contains("chqueimagefront")).Select(multiPartData => multiPartData.LocalFileName).FirstOrDefault();
                        request.ChqueImageFront = new FileInfo(ChqueImageFront).Name;
                        File.Move(ChqueImageFront, System.Web.Hosting.HostingEnvironment.MapPath("/") + ConfigurationManager.AppSettings["CheckImagePath"].ToString() + request.ChqueImageFront);
                    }
                    catch (Exception)
                    {
                    }
                    try
                    {
                        ChequeImageBack = multipartFormDataStreamProvider.FileData.Where(x => x.Headers.ContentDisposition.Name.ToLower().Contains("chequeimageback")).Select(multiPartData => multiPartData.LocalFileName).FirstOrDefault();
                        request.ChequeImageBack = new FileInfo(ChequeImageBack).Name;
                        File.Move(ChequeImageBack, System.Web.Hosting.HostingEnvironment.MapPath("/") + ConfigurationManager.AppSettings["CheckImagePath"].ToString() + request.ChequeImageBack);
                    }
                    catch (Exception)
                    {
                    }

                    var EmpInfo = db.Employees.Where(x => x.Id == EmployeeId).FirstOrDefault();
                    if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
                    {
                        res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        res.Message = "Your account was deactivated by Admin.";
                        res.Data = null;
                    }
                    else
                    {
                        var UserInfo = db.Clients.Find(ClientId);

                        if (PaymentMethod.ToLower() == Utilities.PaymentMethod.CC.GetEnumDescription().ToLower())
                        {
                            if (CardNumber == "" || ExpiryMonth == 0 || ExpiryYear == 0 || NameOnCard == "")
                            {
                                res.StatusCode = HttpStatusCode.BadRequest.GetEnumValue();
                                res.Message = "Invalid Request";
                                res.Data = "";
                                db.Dispose();
                                return Ok(res);
                            }
                            else
                            {
                                try
                                {
                                    if (CardNumber.Contains("*"))
                                    {
                                        cpm1 = db.ClientPaymentMethods.Where(x => x.ClientId == ClientId && x.CardNumber == CardNumber).FirstOrDefault();
                                    }
                                    else
                                    {
                                        String s1 = request.CardNumber.Substring(request.CardNumber.Length - 4);
                                        s1 = s1.PadLeft(16, '*');
                                        var cnt = db.ClientPaymentMethods.Where(x => x.CardNumber == s1 && x.ClientId == UserInfo.Id).Count();
                                        if (cnt > 0)
                                        {
                                            res.StatusCode = (int)HttpStatusCode.Ambiguous;
                                            res.Message = "Card already exists, go back and select CC on file.";
                                            res.Data = null;
                                            db.Dispose();
                                            return Ok(res);
                                        }
                                        cpm1 = null;
                                    }
                                    ClientPaymentMethod cpm = new ClientPaymentMethod();
                                    if (cpm1 != null)
                                    {
                                        cpm = cpm1;
                                    }
                                    else
                                    {
                                        var objClientService = Services.ServiceFactory.ClientService;
                                        var errCode = "";
                                        var errText = "";
                                        string customerPaymentProfileId = "";
                                        string expirationDate = request.ExpiryMonth.ToString().PadLeft(2, '0') + (request.ExpiryYear.ToString().Length > 2 ? request.ExpiryYear.ToString().Substring(2, 2) : request.ExpiryYear.ToString());
                                        var ret = objClientService.CreatePaymentProfile(UserInfo.FirstName, UserInfo.LastName, UserInfo.CustomerProfileId, request.CardNumber, expirationDate, request.CVV.ToString(), ref customerPaymentProfileId, ref errCode, ref errText);

                                        string s = CardNumber.Substring(CardNumber.Length - 4);
                                        cpm.CardNumber = s.PadLeft(16, '*');
                                        //cpm.StripeCardId = stripeCard.Id;
                                        cpm.CardType = request.CardType;
                                        cpm.ClientId = ClientId;
                                        cpm.ExpiryMonth = ExpiryMonth;
                                        cpm.ExpiryYear = ExpiryYear;
                                        cpm.AddedBy = ClientId;
                                        cpm.NameOnCard = NameOnCard;
                                        cpm.IsDefaultPayment = false;
                                        cpm.AddedByType = (int)Utilities.UserRoles.Client;
                                        cpm.AddedDate = DateTime.UtcNow;
                                        if (UserInfo.ClientPaymentMethods.Count <= 0)
                                        {
                                            cpm.IsDefaultPayment = true;
                                        }
                                        db.ClientPaymentMethods.Add(cpm);

                                        db.SaveChanges();
                                        cpm1 = cpm;
                                    }
                                    if (string.IsNullOrWhiteSpace(cpm.CustomerPaymentProfileId))
                                    {
                                        StripeErrMsg = "Invalid Credit Card";
                                        res.StatusCode = (int)HttpStatusCode.BadRequest;
                                        res.Message = StripeErrMsg;
                                        res.Data = null;
                                    }
                                    else
                                    {
                                        res.Message = "Card Saved";
                                        res.StatusCode = (int)HttpStatusCode.OK;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //StripeError = true;
                                    //StripeErrorLog err = Mapper.Map<StripeErrorLog>(stex.StripeError);
                                    //StripeErrMsg = stex.StripeError.Message;
                                    //err.Userid = ClientId;
                                    //db.StripeErrorLogs.Add(err);
                                    res.StatusCode = (int)HttpStatusCode.BadRequest;
                                    res.Message = StripeErrMsg;
                                    res.Data = null;
                                }
                            }
                        }
                        else
                        {
                            res.Message = "Card Saved";
                            res.StatusCode = (int)HttpStatusCode.OK;
                        }
                        List<object> data = new List<object>();
                        var PendingProcessUnits = db.ClientUnits.Where(x => x.ClientId == ClientId && x.PaymentStatus == "NotReceived" && x.IsDeleted == false).AsEnumerable().Where(x => x.AddedByType == Utilities.UserRoles.Employee.GetEnumValue() && x.AddedBy == EmployeeId).ToList();
                        decimal total = 0m;
                        foreach (var cunit1 in PendingProcessUnits)
                        {
                            var PlanSelectedDisplay = new
                            {
                                PlanName = db.SubscriptionPlans.FirstOrDefault(p=>p.Id==cunit1.PlanTypeId).PlanName,
                                UnitName = cunit1.UnitName,
                                Price = cunit1.PricePerMonth,
                                PlanType = (cunit1.IsSpecialApplied == true ? "Special Offer" : "Recurring"),
                                Id = cunit1.Id,
                                ServiceCaseNumber = "",
                                Status = (cunit1.PaymentStatus.ToLower() == "notreceived" ? "Processing" : cunit1.PaymentStatus)
                            };
                            data.Add(PlanSelectedDisplay);
                            total = total + cunit1.PricePerMonth.Value;
                        }
                        PendingProcessUnits.ForEach(x => x.PaymentStatus = "Processing");
                        db.SaveChanges();
                        var d = new
                        {
                            ClientId = UserInfo.Id,
                            UserInfo.Email,
                            UserInfo.FirstName,
                            UserInfo.LastName,
                            Total = total,
                            Units = data
                        };
                        res.Data = d;
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

                if (!StripeError)
                {
                    request.CustomerPaymentProfileId = cpm1.CustomerPaymentProfileId;
                    var filedata = multipartFormDataStreamProvider.FileData;

                    foreach (var file in filedata)
                    {
                        if (file.Headers.ContentDisposition.Name.ToLower().Contains("clientsignature"))
                        {
                            request.ClientSignature = new FileInfo(file.LocalFileName).Name;
                        }
                        else
                        {
                            request.ClientSignature = "";
                        }
                    }                    
                    //if (string.IsNullOrWhiteSpace(filename))
                    //{
                    //    request.ClientSignature = "";
                    //}
                    //else
                    //{
                    //    request.ClientSignature = new FileInfo(filename).Name;
                    //}
                    BackgroundTaskManager.Run(async () =>
                    {
                        await NewMyPayment(request);
                    });
                }
                db.Dispose();
                return Ok(res);
            });
            return task;
        }

        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        public async Task<IHttpActionResult> NewMyPayment(PaymentRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            try
            {
                var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
                var PendingProcessUnits = db.ClientUnits.Where(x => x.ClientId == request.ClientId && x.PaymentStatus == "Processing" && x.IsDeleted == false).AsEnumerable().Where(x => x.AddedByType == Utilities.UserRoles.Employee.GetEnumValue() && x.AddedBy == request.EmployeeId).ToList();
                if (PendingProcessUnits.Count < 0)
                {
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Message = "No data found.";
                    res.Data = null;
                    db.Dispose();
                    return Ok(res);
                }
                string customerPaymentProfileId = "";
                string customerProfileId = "";
                if (request.PaymentMethod.ToLower() == Utilities.PaymentMethod.CC.GetEnumDescription().ToLower())
                {
                    var ccDetails = db.ClientPaymentMethods.Where(x => x.ClientId == request.ClientId).ToList();
                    if (request.CustomerPaymentProfileId == "")
                    {
                        res.StatusCode = (int)HttpStatusCode.BadRequest;
                        res.Message = "Invalid Request.";
                        res.Data = null;
                        db.Dispose();
                        return Ok(res);
                    }
                    customerPaymentProfileId = request.CustomerPaymentProfileId;
                    customerProfileId = UserInfo.CustomerProfileId;
                }
                var card = db.ClientPaymentMethods.Where(x => x.CustomerPaymentProfileId == request.CustomerPaymentProfileId).FirstOrDefault();
                List<BillingHistory> bhs = new List<BillingHistory>();
                var total = 0m;
                foreach (var PendingProcessUnit in PendingProcessUnits)
                {
                    var planName = db.SubscriptionPlans.FirstOrDefault(p => p.Id == PendingProcessUnit.PlanTypeId).PlanName;
                    var StripeError = false;
                    BillingHistory bh = new BillingHistory();
                    bh.BillingAddress = request.Address;
                    bh.BillingCity = request.City;
                    bh.BillingState = request.State;
                    bh.ClientId = request.ClientId;
                    bh.BillingZipcode = request.ZipCode;
                    bh.BillingFirstName = UserInfo.FirstName;
                    bh.BillingLastName = UserInfo.LastName;
                    bh.Company = request.Company;
                    bh.BillingMobileNumber = request.MobileNumber;
                    bh.BillingPhoneNumber = request.PhoneNumber;
                    bh.PackageName = planName;
                    //bh.UnitId = PendingProcessUnit.Id;
                    bh.BillingType = (PendingProcessUnit.IsSpecialApplied == false ? Utilities.BillingTypes.Recurringpurchase.GetEnumDescription() : Utilities.BillingTypes.FixedCost.GetEnumDescription());
                    bh.IsSpecialOffer = (PendingProcessUnit.IsSpecialApplied == null ? false : PendingProcessUnit.IsSpecialApplied);
                    bh.OriginalAmount = PendingProcessUnit.PricePerMonth;
                    PendingProcessUnit.IsSpecialApplied = (PendingProcessUnit.IsSpecialApplied == null ? false : PendingProcessUnit.IsSpecialApplied);
                    bh.PurchasedAmount = PendingProcessUnit.PricePerMonth;

                    if (PendingProcessUnit.Client.Partner != null)
                    {
                        var commission = PendingProcessUnit.Client.Partner.SalesCommission.Value;
                        bh.PartnerSalesCommisionAmount = (bh.PurchasedAmount * (decimal.Parse(commission.ToString()) / 100));
                    }
                    else
                    {
                        bh.PartnerSalesCommisionAmount = 0;
                    }
                    if (PendingProcessUnit.IsSpecialApplied.Value == false)
                    {
                        if (request.PaymentMethod.ToLower() == Utilities.PaymentMethod.CC.GetEnumDescription().ToLower())
                        {
                            var Description = "Charge For Plan" + planName;
                            //var sr = Utilities.StripeCharge(true, "", customerProfileId, customerPaymentProfileId, (int)(PendingProcessUnit.Plan.PricePerMonth * 100), Description, PendingProcessUnit.Plan.PlanType.Name, db, request.ClientId, PendingProcessUnit.Id);
                            //bh.TransactionId = sr.TransactionId;
                            //PendingProcessUnit.PaymentStatus = sr.PaymentStatus;
                            //PendingProcessUnit.StripeSubscriptionId = customerPaymentProfileId;
                            //bh.StripeNextPaymentDate = null;
                            //if (sr.ex != null)
                            //{
                            //    StripeError = true;
                            //}
                        }
                    }
                    else
                    {
                        if (request.PaymentMethod.ToLower() == Utilities.PaymentMethod.CC.GetEnumDescription().ToLower())
                        {
                            //var Description = "Charge For Plan" + PendingProcessUnit.Plan.PlanType.Name;
                            //var sr = Utilities.StripeCharge(true, "", customerProfileId, customerPaymentProfileId, (int)(PendingProcessUnit.Plan.DiscountPrice * 100), Description, PendingProcessUnit.Plan.PlanType.Name, db, request.ClientId, PendingProcessUnit.Id);
                            //bh.TransactionId = sr.TransactionId;
                            //PendingProcessUnit.PaymentStatus = sr.PaymentStatus;
                            //PendingProcessUnit.StripeSubscriptionId = customerPaymentProfileId;
                            //bh.StripeNextPaymentDate = null;
                            //if (sr.ex != null)
                            //{
                            //    StripeError = true;
                            //}
                        }
                    }
                    if (request.PaymentMethod.ToLower() != Utilities.PaymentMethod.CC.GetEnumDescription().ToLower())
                    {
                        PendingProcessUnit.PaymentStatus = Utilities.UnitPaymentTypes.Received.GetEnumDescription();
                        if (request.PaymentMethod.ToLower() == Utilities.PaymentMethod.Check.GetEnumDescription().ToLower())
                        {
                            if (!string.IsNullOrWhiteSpace(request.ChequeNo))
                            {
                                bh.TransactionId = "TRN-" + Utilities.PaymentMethod.Check.GetEnumDescription() + "-" + request.ChequeNo;
                            }
                        }
                        if (request.PaymentMethod.ToLower() == Utilities.PaymentMethod.PO.GetEnumDescription().ToLower())
                        {
                            if (!string.IsNullOrWhiteSpace(request.ChequeNo))
                            {
                                bh.TransactionId = "TRN-" + Utilities.PaymentMethod.Check.GetEnumDescription() + "-" + request.ChequeNo;
                            }
                            else if (!string.IsNullOrWhiteSpace(request.PONo))
                            {
                                bh.TransactionId = "TRN-" + Utilities.PaymentMethod.PO.GetEnumDescription() + "-" + request.PONo;
                            }
                        }
                    }
                    // bh.TransactionDate = DateTime.UtcNow; Code Commented on 19-07-2017
                    bh.TransactionDate = DateTime.UtcNow.ToLocalTime();

                    bh.IsDeleted = false;
                    bh.AddedBy = request.EmployeeId;
                    //bh.AddedDate = DateTime.UtcNow;  Code Commented on 19-07-2017
                    bh.AddedDate = DateTime.UtcNow.ToLocalTime();
                    if (!StripeError)
                    {
                        total = total + bh.PurchasedAmount.Value;
                        bhs.Add(bh);

                        //var PlanCount = db.ClientUnits.Find(bh.UnitId.Value);
                        ClientUnitServiceCount objServiceCount = new ClientUnitServiceCount();
                        objServiceCount.ClientId = bh.ClientId.Value;
                        objServiceCount.UnitId = PendingProcessUnit.Id;
                        objServiceCount.TotalPlanService = PendingProcessUnit.VisitPerYear;
                        objServiceCount.TotalDonePlanService = 0;
                        objServiceCount.TotalRequestService = 0;
                        objServiceCount.TotalDoneRequestService = 0;
                        if ((request.PaymentMethod.ToLower() == Utilities.PaymentMethod.CC.GetEnumDescription().ToLower()) || (request.PaymentMethod.ToLower() != Utilities.PaymentMethod.CC.GetEnumDescription().ToLower() && !string.IsNullOrWhiteSpace(request.ChequeNo)))
                        {
                            objServiceCount.TotalBillsGenerated = 1;
                            objServiceCount.StripeUnitSubscriptionCount = (PendingProcessUnit.IsSpecialApplied.Value ? 0 : 1);
                        }
                        else
                        {
                            objServiceCount.TotalBillsGenerated = 0;
                            objServiceCount.StripeUnitSubscriptionCount = 0;
                        }
                        objServiceCount.IsDeleted = false;
                        objServiceCount.AddedBy = bh.AddedBy;
                        objServiceCount.AddedByType = Utilities.UserRoles.Employee.GetEnumValue();
                        objServiceCount.AddedDate = bh.AddedDate;
                        db.ClientUnitServiceCounts.Add(objServiceCount);
                        //db.SaveChanges();
                    }
                    else
                    {
                    }
                }
                db.SaveChanges();
                if (bhs.Count > 0)
                {
                    Order cUnitOrder = new Order();

                    if (request.PaymentMethod.ToLower() == Utilities.PaymentMethod.CC.GetEnumDescription().ToLower())
                    {
                        cUnitOrder.CardNumber = card.CardNumber;
                        cUnitOrder.NameOnCard = card.NameOnCard;
                        cUnitOrder.ExpirationMonth = card.ExpiryMonth;
                        cUnitOrder.ExpirationYear = card.ExpiryYear;
                        cUnitOrder.CardType = card.CardType;
                    }
                    else
                    {
                        cUnitOrder.ChequeNo = request.ChequeNo;
                        cUnitOrder.PONo = request.PONo;
                    }
                    cUnitOrder.ChargeBy = request.PaymentMethod;
                    cUnitOrder.CCEmail = UserInfo.Email;
                    cUnitOrder.ClientId = request.ClientId;
                    cUnitOrder.AddedBy = request.EmployeeId;
                    cUnitOrder.AddedByType = (int)Utilities.UserRoles.Employee;
                    cUnitOrder.AddedDate = DateTime.UtcNow;
                    cUnitOrder.IsDeleted = false;
                    cUnitOrder.OrderType = "Charge";
                    cUnitOrder.OrderAmount = total;

                    cUnitOrder.ClientSignature = request.ClientSignature;

                    var orderCount = db.Orders.Where(x => x.ClientId == request.ClientId && x.IsDeleted == false).Count();
                    var ordernumber = UserInfo.AccountNumber + "-" + request.ZipCode + "-O" + (orderCount + 1).ToString();
                    cUnitOrder.OrderNumber = ordernumber;
                    db.Orders.Add(cUnitOrder);
                    db.SaveChanges();

                    if ((request.PaymentMethod.ToLower() == Utilities.PaymentMethod.CC.GetEnumDescription().ToLower()) || (request.PaymentMethod.ToLower() != Utilities.PaymentMethod.CC.GetEnumDescription().ToLower() && !string.IsNullOrWhiteSpace(request.ChequeNo)))
                    {
                        bhs.ForEach(x => x.OrderId = cUnitOrder.Id);
                        bhs.ForEach(x => x.IsPaid = true);
                        bhs.ForEach(x => x.failcode = "");
                        bhs.ForEach(x => x.faildesc = "Payment Success!");


                        db.BillingHistories.AddRange(bhs);
                        db.SaveChanges();
                    }

                    foreach (var bhUnit in bhs)
                    {
                        if (request.PaymentMethod.ToLower() == Utilities.PaymentMethod.CC.GetEnumDescription().ToLower())
                        {
                            //Code Commented on 19-07-2017
                            //db.uspa_ClientUnitSubscription_Insert(UserInfo.Id, bhUnit.UnitId, Utilities.PaymentMethod.CC.GetEnumDescription(), bhUnit.IsSpecialOffer.Value, bhUnit.OrderId, card.Id, "", "", "", "", "", bhUnit.PurchasedAmount, request.EmployeeId, Utilities.UserRoles.Employee.GetEnumValue(), DateTime.UtcNow);
                            //db.uspa_ClientUnitSubscription_Insert(UserInfo.Id, bhUnit.UnitId, Utilities.PaymentMethod.CC.GetEnumDescription(), bhUnit.IsSpecialOffer.Value, bhUnit.OrderId, card.Id, "", "", "", "", "", bhUnit.PurchasedAmount, request.EmployeeId, Utilities.UserRoles.Employee.GetEnumValue(), DateTime.UtcNow.ToLocalTime());
                        }
                        else
                        {
                            //Code Commented on 19-07-2017
                            //db.uspa_ClientUnitSubscription_Insert(UserInfo.Id, bhUnit.UnitId, request.PaymentMethod, bhUnit.IsSpecialOffer.Value, cUnitOrder.Id, 0, request.PONo, request.ChequeNo, request.ChqueImageFront, request.ChequeImageBack, request.AccountingNotes, bhUnit.PurchasedAmount, request.EmployeeId, Utilities.UserRoles.Employee.GetEnumValue(), DateTime.UtcNow);
                            //db.uspa_ClientUnitSubscription_Insert(UserInfo.Id, bhUnit.UnitId, request.PaymentMethod, bhUnit.IsSpecialOffer.Value, cUnitOrder.Id, 0, request.PONo, request.ChequeNo, request.ChqueImageFront, request.ChequeImageBack, request.AccountingNotes, bhUnit.PurchasedAmount, request.EmployeeId, Utilities.UserRoles.Employee.GetEnumValue(), DateTime.UtcNow.ToLocalTime());
                        }
                    }

                    StringBuilder sb = new StringBuilder();

                    EmailTemplate templateclient = db.EmailTemplates.Where(x => x.Name == "UnitOrderClient" && x.Status == true).FirstOrDefault();
                    var strclient = templateclient.EmailBody;
                    var sub = templateclient.EmailTemplateSubject;
                    var city = db.Cities.Find(bhs.FirstOrDefault().BillingCity);
                    var state = db.States.Find(bhs.FirstOrDefault().BillingState);
                    //var ClientAddress = PendingProcessUnits.FirstOrDefault().ClientAddress;
                    strclient = strclient.Replace("{{ClientName}}", UserInfo.FirstName + " " + UserInfo.LastName);
                    strclient = strclient.Replace("{{Address}}", bhs.FirstOrDefault().BillingAddress + ",<br/>" + city.Name + ", " + state.Name + ",<br/>" + bhs.FirstOrDefault().BillingZipcode);
                    //strclient = strclient.Replace("{{Address}}", ClientAddress.Address + ",<br/>" + ClientAddress.City1.Name + ", " + ClientAddress.State1.Name + ",<br/>" + ClientAddress.ZipCode);
                    strclient = strclient.Replace("{{PurchasedDate}}", cUnitOrder.AddedDate.ToString("MM/dd/yyyy"));


                    sb.Append("<table border='1' style='border-collapse: collapse;'>");
                    sb.Append("<tr>");
                    sb.Append("<th>");
                    sb.Append("Unit Name");
                    sb.Append("</th>");
                    sb.Append("<th>");
                    sb.Append("Unit Location Address");
                    sb.Append("</th>");
                    sb.Append("<th>");
                    sb.Append("Plan");
                    sb.Append("</th>");
                    sb.Append("<th>");
                    sb.Append("Payment Type");
                    sb.Append("</th>");
                    sb.Append("<th>");
                    sb.Append("Rate");
                    sb.Append("</th>");

                    sb.Append("</tr>");
                    foreach (var item in bhs)
                    {
                        var unitId = PendingProcessUnits.FirstOrDefault().Id;
                        var clientunit = db.ClientUnits.Where(x => x.Id == unitId).FirstOrDefault();
                        var ClientAddress = clientunit.ClientAddress;
                        sb.Append("<tr>");
                        sb.Append("<td>");
                        sb.Append(clientunit.UnitName);
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(ClientAddress.Address + ",<br/>" + ClientAddress.City1.Name + ", " + ClientAddress.State1.Name + ",<br/>" + ClientAddress.ZipCode);
                        sb.Append("</td>");
                        sb.Append("<td>");
                        var planName = db.SubscriptionPlans.FirstOrDefault(p => p.Id == clientunit.PlanTypeId).PlanName;
                        sb.Append(planName);
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append((item.IsSpecialOffer.Value ? "Special Offer" : "Recurring"));
                        sb.Append("</td>");
                        sb.Append("<td> $");
                        sb.Append(item.PurchasedAmount);
                        sb.Append("</td>");
                        sb.Append("</tr>");
                    }
                    sb.Append("<tr>");
                    sb.Append("<td colspan='5'>");
                    sb.Append("Total: $");
                    sb.Append(total.ToString("0.00"));
                    sb.Append("</td>");
                    sb.Append("</tr>");
                    sb.Append("</table>");
                    //strclient = strclient.Replace("{{Amount}}", total.ToString("0.00"));
                    strclient = strclient.Replace("{{UnitsPurchased}}", sb.ToString());
                    Utilities.Send(templateclient.EmailTemplateSubject, UserInfo.Email, strclient, templateclient.FromEmail, db, "", request.CCEmail);
                    //Utilities.Send("Unit Order Success", UserInfo.Email, sb.ToString(), "testlocalcoding@gmail.com", db);
                }

                List<object> data = new List<object>();
                foreach (var cunit1 in PendingProcessUnits)
                {
                    var PlanSelectedDisplay = new
                    {
                        PlanName = db.SubscriptionPlans.FirstOrDefault(p=>p.Id==cunit1.PlanTypeId).PlanName,
                        UnitName = cunit1.UnitName,
                        Price = cunit1.PricePerMonth,
                        PlanType = (cunit1.IsSpecialApplied == true ? "Special Offer" : "Recurring"),
                        Id = cunit1.Id,
                        Status = (cunit1.PaymentStatus.ToLower() == "processing" ? "Processing" : cunit1.PaymentStatus)
                    };
                    data.Add(PlanSelectedDisplay);
                }
                var objRes = new
                {
                    ClientId = request.ClientId,
                    FirstName = UserInfo.FirstName,
                    LastName = UserInfo.LastName,
                    Email = UserInfo.Email,
                    Total = total,
                    Units = data
                };
                res.Message = "Order Placed";
                res.StatusCode = (int)HttpStatusCode.OK;
                res.Data = objRes;
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
        [Route("CheckClientPaymentStatus")]
        public async Task<IHttpActionResult> CheckClientPaymentStatus([FromBody]PaymentStatusCheckRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            List<object> UnitResponse = new List<object>();
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
                    var ApiError = db.StripeErrorLogs.Where(x => x.ChargeId == "999" && x.Userid == request.ClientId).ToList();
                    if (ApiError.Count > 0)
                    {
                        res.StatusCode = 999;
                        res.Message = "Api Call Failed.";
                        res.Data = null;
                        db.Dispose();
                        return Ok(res);
                    }
                    var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
                    var PendingProcessUnits = db.ClientUnits.Include(c => c.Client).AsEnumerable().Where(x => x.ClientId == request.ClientId && x.IsDeleted == false && request.UnitId == x.Id).ToList();

                    foreach (var PendingProcessUnit in PendingProcessUnits)
                    {
                        StripeErrorLog err = db.StripeErrorLogs.Where(x => x.Userid == request.ClientId && x.UnitId == PendingProcessUnit.Id).OrderByDescending(x => x.Id).FirstOrDefault();
                        var objRes = new
                        {
                            ClientId = request.ClientId,
                            Id = PendingProcessUnit.Id,
                            Status = (PendingProcessUnit.PaymentStatus.ToLower() == "processing" ? "Processing" : PendingProcessUnit.PaymentStatus),
                            StripeError = (err != null ? err.Message : "")
                        };
                        UnitResponse.Add(objRes);
                    }
                    res.Message = "Order Process Report";
                    res.StatusCode = (int)HttpStatusCode.OK;
                    res.Data = UnitResponse.FirstOrDefault();
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
        [Route("GetPurposeOfVisitTime")]
        public async Task<IHttpActionResult> GetPurposeOfVisitTime([FromBody]EmpCommonModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            //var values = DurationExtensions.GetValues<Utilities.PurposeOfVisit>();
            List<object> data = new List<object>();
            var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId).FirstOrDefault();
            if (!EmpInfo.IsActive || EmpInfo.IsDeleted)
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "Your account was deactivated by Admin.";
                res.Data = null;
            }
            else
            {
                var values = DurationExtensions.GetValues<Utilities.PurposeOfVisit>();
                foreach (var item in values)
                {
                    Utilities.PurposeOfVisit p = (Utilities.PurposeOfVisit)item;
                    data.Add(new { Id = p.GetEnumValue(), Name = p.GetEnumDisplayName() });
                }
                foreach (var item in values)
                {
                    Utilities.PurposeOfVisit p = (Utilities.PurposeOfVisit)item;
                    data.Add(p.GetEnumDescription());
                }
                var time = await db.PlanTypes.FirstOrDefaultAsync();

                var resp = new
                {
                    Purpose = data,
                    TimeSlot1 = time.ServiceSlot1,
                    TimeSlot2 = time.ServiceSlot2
                };

                res.Message = "Record Found";
                res.StatusCode = (int)HttpStatusCode.OK;
                res.Data = resp;
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
        [Route("ServiceRecheduleRequest")]
        public async Task<IHttpActionResult> ServiceRecheduleRequest([FromBody]ServiceRequestModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            List<object> data = new List<object>();

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
                    if (request.ServiceRequestedOn.Date == DateTime.Now.Date)
                    {
                        bool IsValidTime = Utilities.CheckTimeValidation(request.ServiceRequestedTime, 1);
                        if (!IsValidTime)
                        {
                            var msg = "Please select future time.";
                            res.StatusCode = HttpStatusCode.Ambiguous.GetEnumValue();
                            res.Message = msg;
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
                    var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
                    var ClientService = db.Services.Where(x => x.ClientId == request.ClientId && x.Id == request.ServiceId).FirstOrDefault();
                    if (ClientService != null)
                    {
                        var notification = db.UserNotifications.AsEnumerable().Where(x => x.CommonId == request.ServiceId && x.MessageType == Utilities.NotificationType.ServiceScheduled.GetEnumDescription()).ToList();
                        if (notification.Count > 0)
                        {
                            db.UserNotifications.RemoveRange(notification);
                            db.SaveChanges();
                        }

                        var notification1 = db.UserNotifications.AsEnumerable().Where(x => x.CommonId == request.ServiceId && x.MessageType == Utilities.NotificationType.PeriodicServiceReminder.GetEnumDescription()).ToList();
                        if (notification1.Count > 0)
                        {
                            db.UserNotifications.RemoveRange(notification1);
                            db.SaveChanges();
                        }
                        ClientService.Status = Utilities.ServiceTypes.Rescheduled.GetEnumDescription();

                        if (request.NotificationId > 0)
                        {
                            var n = db.UserNotifications.Find(request.NotificationId);
                            db.UserNotifications.Remove(n);
                            db.SaveChanges();
                        }
                        var EmpNotification = db.NotificationMasters.Where(x => x.Name == "RescheduleServiceSendToClient").FirstOrDefault();

                        //var message = "your service at " + ClientService.Client.FirstName + " " + ClientService.Client.LastName + "’s address on " + ClientService.ScheduleDate.Value.ToString("MMMM dd, yyyy") + " has been rescheduled";
                        var message = EmpNotification.Message;
                        message = message.Replace("{{EmpName}}", EmpInfo.FirstName + " " + EmpInfo.LastName);
                        message = message.Replace("{{ScheduleDate}}", ClientService.ScheduleDate.Value.ToString("MMMM dd, yyyy"));
                        UserNotification objUserNotification = new UserNotification();
                        objUserNotification.UserId = ClientService.ClientId;
                        objUserNotification.UserTypeId = Utilities.UserRoles.Client.GetEnumValue();
                        objUserNotification.Message = message;
                        objUserNotification.Status = Utilities.NotificationStatus.UnRead.GetEnumDescription();
                        objUserNotification.CommonId = ClientService.Id;
                        objUserNotification.MessageType = Utilities.NotificationType.FriendlyReminder.GetEnumDescription();
                        objUserNotification.AddedDate = DateTime.UtcNow;
                        db.UserNotifications.Add(objUserNotification);
                        db.SaveChanges();

                        var BadgeCount = db.UserNotifications.AsEnumerable().Where(x => x.UserId == ClientService.ClientId && x.UserTypeId == Utilities.UserRoles.Client.GetEnumValue() && x.Status == Utilities.NotificationStatus.UnRead.GetEnumDescription()).ToList().Count;

                        Notifications objNotifications = new Notifications { NId = objUserNotification.Id, NType = Utilities.NotificationType.FriendlyReminder.GetEnumValue(), CommonId = ClientService.Id };
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
                            else if (EmpInfo.DeviceType.ToLower() == "iphone")
                            {
                                SendNotifications.SendIphoneNotification(BadgeCount, UserInfo.DeviceToken, message, notify, "client", HttpContext.Current);
                            }
                        }

                        ClientService.EmployeeId = null;
                        ClientService.WorkAreaId = null;
                        ClientService.ScheduleDate = null;
                        ClientService.ScheduleStartTime = null;
                        ClientService.ScheduleEndTime = null;
                        ClientService.StatusChangeDate = DateTime.UtcNow;
                        ClientService.UpdatedBy = request.EmployeeId;
                        ClientService.UpdatedByType = Utilities.UserRoles.Employee.GetEnumValue();
                        ClientService.UpdatedDate = DateTime.UtcNow;
                        foreach (var parts in ClientService.ServicePartLists)
                        {
                            var pt = parts;
                            if (pt != null)
                            {
                                pt.Part.ReservedQuantity = pt.Part.ReservedQuantity - parts.PartQuantity;
                            }
                        }
                        var removeParts = db.ServicePartLists.Where(x => x.ServiceId == ClientService.Id).ToList();
                        if (removeParts.Count > 0)
                        {
                            db.ServicePartLists.RemoveRange(removeParts);
                            db.SaveChanges();
                        }
                        var es = db.EmployeeSchedules.Where(x => x.ServiceId == request.ServiceId).FirstOrDefault();
                        if (es != null)
                        {
                            db.EmployeeSchedules.Remove(es);
                        }
                        if (ClientService.RequestedServices.Count > 0)
                        {
                            var requestedService = ClientService.RequestedServices.FirstOrDefault();
                            var rs1 = db.RequestedServices.Where(x => x.Id == requestedService.Id).FirstOrDefault();
                            rs1.ServiceRequestedOn = request.ServiceRequestedOn;
                            rs1.ServiceRequestedTime = request.ServiceRequestedTime;
                        }
                        if (ClientService.EmployeeSchedules.Count > 0)
                        {
                            db.EmployeeSchedules.RemoveRange(ClientService.EmployeeSchedules);
                            db.SaveChanges();
                        }
                        RescheduleService RS = new RescheduleService();
                        RS.ServiceId = ClientService.Id;
                        RS.Rescheduletime = request.ServiceRequestedTime;
                        RS.RescheduleDate = request.ServiceRequestedOn;
                        RS.Reason = request.Reason;
                        RS.AddedBy = request.EmployeeId;
                        RS.AddedByType = Utilities.UserRoles.Employee.GetEnumValue();
                        RS.AddedDate = DateTime.UtcNow;

                        db.RescheduleServices.Add(RS);

                        db.SaveChanges();
                        if (ClientService.PurposeOfVisit == Utilities.PurposeOfVisit.Emergency.GetEnumDescription())
                        {
                            var requestedService = ClientService.RequestedServiceBridges.FirstOrDefault();
                            var EmergencyService = db.uspa_RequestedServiceToServiceScheduler(ClientService.Id, 0, request.ClientId, ClientService.AddressID, ClientService.PurposeOfVisit, request.ServiceRequestedOn, request.ServiceRequestedTime).ToList();

                            if (EmergencyService.Count > 0)
                            {
                                var es1 = EmergencyService.FirstOrDefault();
                                if (es1.EmployeeId > 0 && es1.ServiceId > 0)
                                {
                                    //Employee Notification

                                    var service = db.Services.Where(x => x.Id == es1.ServiceId).FirstOrDefault();

                                    var EmpNotification1 = db.NotificationMasters.Where(x => x.Name == "EmployeeSchedule").FirstOrDefault();
                                    var message1 = EmpNotification1.Message;
                                    message1 = message1.Replace("{{ScheduleDate}}", service.ScheduleDate.Value.ToString("MMMM dd, yyyy"));

                                    UserNotification objUserNotification1 = new UserNotification();
                                    objUserNotification1.UserId = service.EmployeeId;
                                    objUserNotification1.UserTypeId = Utilities.UserRoles.Employee.GetEnumValue();
                                    objUserNotification1.Message = message1;
                                    objUserNotification1.Status = Utilities.NotificationStatus.UnRead.GetEnumDescription();
                                    objUserNotification1.CommonId = es1.ServiceId;
                                    objUserNotification1.MessageType = Utilities.NotificationType.ServiceScheduled.GetEnumDescription();
                                    objUserNotification1.AddedDate = DateTime.UtcNow;
                                    db.UserNotifications.Add(objUserNotification1);
                                    db.SaveChanges();

                                    var BadgeCount1 = db.UserNotifications.AsEnumerable().Where(x => x.UserId == service.EmployeeId && x.UserTypeId == Utilities.UserRoles.Employee.GetEnumValue() && x.Status == Utilities.NotificationStatus.UnRead.GetEnumDescription()).ToList().Count;

                                    Notifications objNotifications1 = new Notifications { NId = objUserNotification1.Id, NType = Utilities.NotificationType.ServiceScheduled.GetEnumValue(), CommonId = es1.ServiceId.Value };
                                    List<NotificationModel> notify1 = new List<NotificationModel>();
                                    notify1.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications1.NId } });
                                    notify1.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications1.NType } });
                                    notify1.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications1.CommonId } });
                                    //var EmpInfo = db.Employees.Where(x => x.Id == service.EmployeeId).FirstOrDefault();
                                    if (EmpInfo.DeviceType != null && EmpInfo.DeviceToken != null)
                                    {
                                        if (EmpInfo.DeviceType.ToLower() == "android")
                                        {
                                        }
                                        else if (EmpInfo.DeviceType.ToLower() == "iphone")
                                        {
                                            SendNotifications.SendIphoneNotification(BadgeCount1, EmpInfo.DeviceToken, message1, notify1, "employee", HttpContext.Current);
                                        }
                                    }
                                }
                            }
                            if (EmergencyService.Count > 0)
                            {
                                var es2 = EmergencyService.FirstOrDefault();
                                if (es2.EmployeeId > 0 && es2.ServiceId > 0)
                                {
                                    //Client Notification

                                    var service = db.Services.Find(es2.ServiceId);
                                    var ClientNotification = db.NotificationMasters.Where(x => x.Name == "RequestedServiceScheduleSendToClient").FirstOrDefault();
                                    var message1 = ClientNotification.Message;
                                    message1 = message1.Replace("{{ScheduleDate}}", service.ScheduleDate.Value.ToString("MMMM dd, yyyy"));
                                    UserNotification objUserNotification1 = new UserNotification();
                                    objUserNotification1.UserId = service.ClientId;
                                    objUserNotification1.UserTypeId = Utilities.UserRoles.Client.GetEnumValue();
                                    objUserNotification1.Message = message1;
                                    objUserNotification1.Status = Utilities.NotificationStatus.UnRead.GetEnumDescription();
                                    objUserNotification1.CommonId = service.Id;
                                    objUserNotification1.MessageType = Utilities.NotificationType.ServiceScheduled.GetEnumDescription();
                                    objUserNotification1.AddedDate = DateTime.UtcNow;
                                    db.UserNotifications.Add(objUserNotification1);
                                    db.SaveChanges();

                                    var BadgeCount1 = db.uspa_ClientPortal_GetNotificationForDashBoardByUserType(service.ClientId, Utilities.UserRoles.Client.GetEnumValue(), "", 0).AsEnumerable().Where(x => x.Status == Utilities.NotificationStatus.UnRead.GetEnumDescription()).Count();
                                    Notifications objNotifications1 = new Notifications { NId = objUserNotification1.Id, NType = Utilities.NotificationType.ServiceScheduled.GetEnumValue(), CommonId = service.Id };
                                    List<NotificationModel> notify1 = new List<NotificationModel>();
                                    notify1.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications1.NId } });
                                    notify1.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications1.NType } });
                                    notify1.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications1.CommonId } });
                                    if (UserInfo.DeviceType != null && UserInfo.DeviceToken != null)
                                    {
                                        if (UserInfo.DeviceType.ToLower() == "android")
                                        {
                                            string CustomData = "&data.NId=" + objNotifications1.NId + "&data.NType=" + objNotifications1.NType + "&data.CommonId=" + objNotifications1.CommonId;
                                            SendNotifications.SendAndroidNotification(UserInfo.DeviceToken, message1, CustomData, "client");
                                        }
                                        else if (UserInfo.DeviceType.ToLower() == "iphone")
                                        {
                                            SendNotifications.SendIphoneNotification(BadgeCount1, UserInfo.DeviceToken, message1, notify1, "client", HttpContext.Current);
                                        }
                                    }
                                }
                            }
                        }
                        res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                        res.Message = "Record Updated";
                        res.Data = null;
                        //res.StatusCode = HttpStatusCode.OK.GetEnumValue();


                    }
                    else
                    {
                        res.StatusCode = (int)HttpStatusCode.BadRequest;
                        res.Message = "Invalid Request.";
                        res.Data = null;
                    }
                }
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
        [ResponseType(typeof(ResponseListModel))]
        [HttpPost]
        [Route("NotificationList")]
        public async Task<IHttpActionResult> NotificationList([FromBody]EmpCommonModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseListModel res = new ResponseListModel();
            int totalRecord = 0;
            int pageCnt = 0;
            int totalPageCount = 0;
            try
            {
                var EmpInfo = await db.Employees.Where(x => x.Id == request.EmployeeId).FirstOrDefaultAsync();
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
                        var ClientNotificationsData = db.uspa_ClientPortal_GetNotificationForDashBoardByUserType(request.EmployeeId, Utilities.UserRoles.Employee.GetEnumValue(), "", 0).ToList();
                        List<NotificationListModel> NotificationData = new List<NotificationListModel>();
                        List<NotificationListModel> result = new List<NotificationListModel>();

                        if (request.LastCallDateTime == null)
                        {
                        }
                        else
                        {
                            ClientNotificationsData = ClientNotificationsData.Where(x => x.AddedDate >= request.LastCallDateTime).ToList();
                        }
                        foreach (var item in ClientNotificationsData)
                        {

                            string Date = "";
                            if (item.AddedDate == null)
                            {

                            }
                            if (item.AddedDate.Date == DateTime.UtcNow.Date)
                            {
                                Date = "Today " + item.AddedDate.ToLocalTime().ToString("hh:mm tt");
                            }
                            else
                            {
                                Date = item.AddedDate.ToLocalTime().ToString("dd MMMM yyyy");
                            }
                            var service = db.Services.Where(x => x.Id == item.CommonId).FirstOrDefault();
                            var emp = (service != null ? service.Employee : null);
                            Utilities.NotificationType MessageType = Enum.GetValues(typeof(Utilities.NotificationType)).Cast<Utilities.NotificationType>()
                                                               .FirstOrDefault(v => v.GetEnumDescription() == item.MessageType);
                            if (!EmpInfo.IsSalesPerson)
                            {
                                NotificationData.Add(new NotificationListModel()
                                {
                                    ClientId = item.UserId,
                                    CommonId = (item.CommonId.HasValue ? item.CommonId.Value : 0),
                                    NotificationId = item.Id.Value,
                                    NotificationType = MessageType.GetEnumValue(),
                                    Message = item.Message,
                                    DateTime = Date,
                                    ProfileImage = (emp != null ? (string.IsNullOrWhiteSpace(emp.Image) ? "" : ConfigurationManager.AppSettings["EMPProfileImageURL"].ToString() + emp.Image) : ""),
                                    LastAdded = item.AddedDate,
                                    ScheduleDay = (service == null || !service.ScheduleDate.HasValue ? "0" : service.ScheduleDate.Value.ToString("dd")),
                                    ScheduleMonth = (service == null || !service.ScheduleDate.HasValue ? "" : service.ScheduleDate.Value.ToString("MMMM")),
                                    ScheduleYear = (service == null || !service.ScheduleDate.HasValue ? "" : service.ScheduleDate.Value.ToString("yyyy")),
                                    ScheduleStartTime = (service == null || !string.IsNullOrWhiteSpace(service.ScheduleStartTime) ? "" : service.ScheduleStartTime),
                                    ScheduleEndTime = (service == null || !string.IsNullOrWhiteSpace(service.ScheduleEndTime) ? "" : service.ScheduleEndTime),
                                    Status = item.Status
                                });
                            }
                            else
                            {
                                NotificationData.Add(new NotificationListModel()
                                {
                                    ClientId = item.UserId,
                                    CommonId = (item.CommonId.HasValue ? item.CommonId.Value : 0),
                                    NotificationId = item.Id.Value,
                                    NotificationType = MessageType.GetEnumValue(),
                                    Message = item.Message,
                                    DateTime = Date,
                                    ProfileImage = (emp != null ? (string.IsNullOrWhiteSpace(emp.Image) ? "" : ConfigurationManager.AppSettings["EMPProfileImageURL"].ToString() + emp.Image) : ""),
                                    LastAdded = item.AddedDate,
                                    ScheduleDay = "0",
                                    ScheduleMonth = "",
                                    ScheduleYear = "",
                                    ScheduleStartTime = "",
                                    ScheduleEndTime = "",
                                    Status = item.Status
                                });
                            }
                        }
                        if (EmpInfo.IsSalesPerson)
                        {
                            NotificationData = NotificationData.ToList();
                        }
                        else
                        {
                            NotificationData = NotificationData.Where(x => x.NotificationType != Utilities.NotificationType.SalesPersonVisit.GetEnumValue()).ToList();
                        }
                        var pageSize = int.Parse(Utilities.GetSiteSettingValue("ApplicationPageSize", db));
                        if (request.PageNumber.HasValue)
                        {
                            result = CreatePagedResults<NotificationListModel, NotificationListModel>(NotificationData.AsQueryable(), request.PageNumber.Value, pageSize, out totalRecord, out pageCnt, out totalPageCount).ToList();
                        }
                        else
                        {
                            result = CreatePagedResults<NotificationListModel, NotificationListModel>(NotificationData.AsQueryable(), 1, pageSize, out totalRecord, out pageCnt, out totalPageCount).ToList();
                        }
                        var ClientUnitsFailed = db.ClientUnits.AsEnumerable().Where(x => x.AddedBy == request.EmployeeId && x.AddedByType == Utilities.UserRoles.Employee.GetEnumValue() && x.IsDeleted == false && x.IsActive == true && x.PaymentStatus == Utilities.UnitPaymentTypes.PaymentFailed.GetEnumDescription()).ToList();
                        var ClientUnitsNotProcessed = db.ClientUnits.AsEnumerable().Where(x => x.AddedBy == request.EmployeeId && x.AddedByType == Utilities.UserRoles.Employee.GetEnumValue() && x.IsDeleted == false && x.IsActive == true && x.PaymentStatus == Utilities.UnitPaymentTypes.NotReceived.GetEnumDescription()).ToList();
                        var ClientUnitsProcessing = db.ClientUnits.AsEnumerable().Where(x => x.AddedBy == request.EmployeeId && x.AddedByType == Utilities.UserRoles.Employee.GetEnumValue() && x.IsDeleted == false && x.IsActive == true && x.PaymentStatus == Utilities.UnitPaymentTypes.Processing.GetEnumDescription()).ToList();
                        ClientUnitsFailed.AddRange(ClientUnitsNotProcessed);
                        if (result.Count > 0)
                        {
                            res.Data = result;
                            res.Message = "Records Found.";
                            res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                            res.LastCallDateTime = result.Last().LastAdded;
                            res.PageNumber = pageCnt;
                            res.TotalNumberOfPages = totalPageCount;
                            res.TotalNumberOfRecords = totalRecord;
                            res.HasPaymentFailedUnit = (ClientUnitsFailed.Count > 0 ? true : false);
                            res.HasPaymentProcessingUnits = (ClientUnitsProcessing.Count > 0 ? true : false);
                            foreach (var item in result)
                            {
                                Utilities.NotificationType n = (Utilities.NotificationType)item.NotificationType;
                                switch (n)
                                {
                                    case Utilities.NotificationType.FriendlyReminder:
                                    case Utilities.NotificationType.PartPurchased:
                                    case Utilities.NotificationType.AdminNotification:
                                    case Utilities.NotificationType.PlanRenewed:
                                    case Utilities.NotificationType.UnitPlanRenew:
                                    case Utilities.NotificationType.UnitPlanCancelled:
                                    case Utilities.NotificationType.PeriodicServiceReminder:
                                        var oldNotification = db.UserNotifications.Find(item.NotificationId);
                                        oldNotification.Status = Utilities.NotificationStatus.Read.GetEnumDescription();
                                        db.SaveChanges();
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        else
                        {
                            res.Data = NotificationData;
                            res.Message = "No record found";
                            res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                            res.PageNumber = pageCnt - 1;
                            res.TotalNumberOfPages = totalPageCount;
                            res.TotalNumberOfRecords = totalRecord;
                            res.HasPaymentFailedUnit = (ClientUnitsFailed.Count > 0 ? true : false);
                            res.HasPaymentProcessingUnits = (ClientUnitsProcessing.Count > 0 ? true : false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res.Data = null;
                res.Message = "Internal Server Error.";
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

        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("GetEmployeeToken")]
        public async Task<IHttpActionResult> GetEmployeeToken([FromBody]EmpCommonModel request)
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
                var chkToken = db.AppAccessTokens.AsEnumerable().Where(top => top.UserId == request.EmployeeId && top.UserType == Utilities.UserRoles.Employee.GetEnumValue()).FirstOrDefault();
                var model = db.Employees.Where(x => x.Id == request.EmployeeId).FirstOrDefault();
                if (chkToken != null)
                {
                    db.AppAccessTokens.Remove(chkToken);
                    db.SaveChanges();
                    TokenDetails objToken = api.Helpers.AsyncHelpers.RunSync<TokenDetails>(() => generatEmpToken(model.Email, model.Password, model.DeviceToken));
                    if (String.IsNullOrEmpty(objToken.error))
                    {
                        res.Token = objToken.access_token;
                        res.Message = "Token Generated";
                        res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                        Add_UpdateToken(model.Id, objToken, 1);
                    }
                }
                else
                {
                    TokenDetails objToken = api.Helpers.AsyncHelpers.RunSync<TokenDetails>(() => generatEmpToken(model.Email, model.Password, model.DeviceToken));
                    if (String.IsNullOrEmpty(objToken.error))
                    {
                        res.Token = objToken.access_token;
                        res.Message = "Token Generated";
                        res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                        Add_UpdateToken(model.Id, objToken, 1);
                    }
                }
            }
            db.Dispose();
            return Ok(res);
        }

        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("employeeUpdateToken")]
        public async Task<IHttpActionResult> employeeUpdateToken([FromBody]EmpCommonModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            try
            {
                var EmpInfo = db.Employees.Where(x => x.Id == request.EmployeeId).FirstOrDefault();
                if (EmpInfo != null)
                {
                    if (EmpInfo.IsActive)
                    {
                        EmpInfo.DeviceToken = request.DeviceToken;
                        EmpInfo.DeviceType = request.DeviceType;
                        EmpInfo.UpdatedBy = request.EmployeeId;
                        EmpInfo.UpdatedByType = Utilities.UserRoles.Employee.GetEnumValue();
                        EmpInfo.UpdatedDate = DateTime.UtcNow;

                        //db.Entry(UserInfo).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        res.StatusCode = (int)HttpStatusCode.OK;
                        res.Message = "Token Updated Successfully.";
                        res.Data = null;
                    }
                    else
                    {
                        res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        res.Message = "Your account was deactivated by Admin.";
                        res.Data = null;
                    }
                }
                else
                {
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Message = "Invalid User Details.";
                    res.Data = null;
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
        [HttpPost]
        [Route("salesPersonVisitDetail")]
        public async Task<IHttpActionResult> salesPersonVisitDetail([FromBody]EmpCommonModel request)
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
                var SalesVisit = db.SalesVisitRequests.Where(x => x.Id == request.SalesVisitRequestId).FirstOrDefault();
                if (SalesVisit != null)
                {
                    var ClientAddress = db.ClientAddresses.Where(x => x.Id == SalesVisit.AddressId).FirstOrDefault();
                    var objRes = new
                    {
                        SalesVisit.Client.FirstName,
                        SalesVisit.Client.LastName,
                        MobileNumber = (string.IsNullOrWhiteSpace(SalesVisit.Client.MobileNumber) ? "" : SalesVisit.Client.MobileNumber),
                        OfficeNumber = (string.IsNullOrWhiteSpace(SalesVisit.Client.OfficeNumber) ? "" : SalesVisit.Client.OfficeNumber),
                        HomeNumber = (string.IsNullOrWhiteSpace(SalesVisit.Client.HomeNumber) ? "" : SalesVisit.Client.HomeNumber),
                        PhoneNumber = (string.IsNullOrWhiteSpace(SalesVisit.Client.PhoneNumber) ? "" : SalesVisit.Client.PhoneNumber),
                        Email = (string.IsNullOrWhiteSpace(SalesVisit.Client.Email) ? "" : SalesVisit.Client.Email),
                        Address = ClientAddress.Address + ", " + ClientAddress.City1.Name + ", " + ClientAddress.State1.Name + ", " + ClientAddress.ZipCode,
                        ClientAddress.Latitude,
                        ClientAddress.Longitude,
                        SalesVisit.Notes
                    };
                    res.Message = "Data Found";
                    res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                    res.Data = objRes;

                    var notifications = db.UserNotifications.Where(x => x.UserId == EmpInfo.Id && x.UserTypeId == 5 && x.CommonId == request.SalesVisitRequestId).FirstOrDefault();
                    if (notifications != null)
                    {
                        notifications.Status = Utilities.NotificationStatus.Read.GetEnumDescription();
                        //db.UserNotifications.Remove(notifications);
                        db.SaveChanges();
                    }
                }
                else
                {
                    res.Message = "No Data Found";
                    res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                    res.Data = null;
                }
            }

            db.Dispose();
            return Ok(res);
        }

        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("GetClientUnitByAddressIdPlanType")]
        public async Task<IHttpActionResult> GetClientUnit([FromBody]CommonRequest request)
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
                var UserInfo = await db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefaultAsync();
                ClientAddress address = new ClientAddress();
                List<object> data = new List<object>();
                if (UserInfo != null)
                {
                    int PeraAddressId = 0;
                    if (request.AddressId == null)
                    {
                        address = UserInfo.ClientAddresses.Where(x => x.IsDefaultAddress == true).FirstOrDefault();
                    }
                    if (address != null || request.AddressId != null)
                    {
                        if (request.AddressId == null)
                        {
                            PeraAddressId = address.Id;
                        }
                        else
                        {
                            PeraAddressId = request.AddressId.Value;
                        }

                        var ClientUnits = db.ClientUnits.Where(x => x.ClientId == request.ClientId && x.IsDeleted == false && x.AddressId == PeraAddressId && x.PlanTypeId == request.PlanTypeId && x.IsPlanRenewedOrCancelled == false).AsEnumerable().Where(x => x.PaymentStatus == Utilities.UnitPaymentTypes.Received.GetEnumDescription()).ToList();
                        foreach (var cUnit in ClientUnits)
                        {
                            var d = new
                            {
                                Id = cUnit.Id,
                                UnitName = cUnit.UnitName,
                            };
                            data.Add(d);
                        }
                    }
                    else
                    {
                        res.StatusCode = (int)HttpStatusCode.NotFound;
                        res.Message = "No record found";
                        res.Data = null;
                    }

                    if (data.Count > 0)
                    {
                        res.StatusCode = (int)HttpStatusCode.OK;
                        res.Message = "Record Found.";
                        res.Data = data;
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
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Message = "Client Not Available.";
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
        [Route("RemoveClientUnit")]
        public async Task<IHttpActionResult> RemoveClientUnit([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            List<object> d = new List<object>();
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
                    var ClientUnit = await db.ClientUnits.Where(x => x.ClientId == request.ClientId && x.Id == request.UnitId).ToListAsync();

                    if (ClientUnit.Count > 0)
                    {
                        foreach (ClientUnit unit in ClientUnit)
                        {
                            var ueis = db.UnitExtraInfoes.Where(x => x.UnitId == unit.Id).ToList();
                            db.UnitExtraInfoes.RemoveRange(ueis);
                            var ClientUnitParts = await db.ClientUnitParts.Where(x => x.UnitId == unit.Id).ToListAsync();
                            db.ClientUnitParts.RemoveRange(ClientUnitParts);

                            var um = unit.ClientUnitManuals.ToList();

                            db.ClientUnitManuals.RemoveRange(um);

                            var unitImage = unit.ClientUnitPictures.ToList();
                            db.ClientUnitPictures.RemoveRange(unitImage);

                        }
                        db.ClientUnits.RemoveRange(ClientUnit);
                        db.SaveChanges();

                        //var PendingProcessUnit = db.ClientUnits.Where(x => x.ClientId == request.ClientId && x.PaymentStatus == "NotReceived").AsEnumerable().Where(x => x.AddedByType == Utilities.UserRoles.Employee.GetEnumValue() && x.AddedBy == request.EmployeeId).ToList();
                        var PendingProcessUnit = db.ClientUnits.AsEnumerable().Where(x => x.ClientId == request.ClientId && x.AddedByType == Utilities.UserRoles.Employee.GetEnumValue() && x.AddedBy == request.EmployeeId && (x.PaymentStatus == Utilities.UnitPaymentTypes.NotReceived.GetEnumDescription() || x.PaymentStatus == Utilities.UnitPaymentTypes.PaymentFailed.GetEnumDescription())).ToList();
                        List<object> data = new List<object>();
                        decimal total = 0m;
                        if (PendingProcessUnit.Count > 0)
                        {

                            foreach (var cunit1 in PendingProcessUnit)
                            {
                                var desc = db.SubscriptionPlans.FirstOrDefault(p => p.Id == cunit1.PlanTypeId).PlanName;
                                var PlanSelectedDisplay = new
                                {
                                    cunit1.UnitName,
                                    PlanName = desc,
                                    Description = desc.Substring(0, (desc.Length > 256 ? 256 : desc.Length)),
                                    Price = cunit1.PricePerMonth,
                                    PlanType = (cunit1.IsSpecialApplied == true ? "Special Offer" : "Recurring"),
                                    Id = cunit1.Id
                                };
                                total = total + cunit1.PricePerMonth.Value;
                                data.Add(PlanSelectedDisplay);
                            }
                            var response = new
                            {
                                Units = data,
                                Total = total,
                                Message = (PendingProcessUnit.Count(x => x.IsSpecialApplied == true) == PendingProcessUnit.Count ? "" : "(Recurring Billing occur every month)"),
                            };
                            res.StatusCode = (int)HttpStatusCode.OK;
                            res.Message = "Records Deleted.";
                            res.Data = response;
                        }
                        else
                        {
                            res.StatusCode = (int)HttpStatusCode.OK;
                            res.Message = "Records Deleted.";
                            res.Data = d;
                        }
                    }
                    else
                    {
                        res.StatusCode = (int)HttpStatusCode.BadRequest;
                        res.Message = "Unit Not Found";
                        res.Data = null;
                    }
                }
            }
            catch (Exception ex)
            {
                res.StatusCode = (int)HttpStatusCode.BadRequest;
                res.Message = "Invalid Request.";
                res.Data = new { };
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
        [Route("GetPaymentFailedUnit")]
        public async Task<IHttpActionResult> GetPaymentFailedUnit([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            List<object> d = new List<object>();
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
                    var PendingProcessUnit = db.ClientUnits.AsEnumerable().Where(x => x.AddedBy == request.EmployeeId && x.AddedByType == Utilities.UserRoles.Employee.GetEnumValue() && (x.PaymentStatus == Utilities.UnitPaymentTypes.NotReceived.GetEnumDescription() || x.PaymentStatus == Utilities.UnitPaymentTypes.PaymentFailed.GetEnumDescription())).ToList();
                    List<object> data = new List<object>();
                    decimal total = 0m;
                    if (PendingProcessUnit.Count > 0)
                    {
                        var PaymentFailedNotifications = db.UserNotifications.AsEnumerable().Where(x => x.UserId == PendingProcessUnit[0].ClientId && x.UserTypeId == Utilities.UserRoles.Client.GetEnumValue() && x.MessageType == Utilities.NotificationType.PaymentFailed.GetEnumDescription()).ToList();
                        if (PaymentFailedNotifications.Count() > 0)
                        {
                            PaymentFailedNotifications.ForEach(x => x.Status = Utilities.NotificationStatus.Read.GetEnumDescription());
                            db.SaveChanges();
                        }
                        var CurrentPaymentMethod = "";
                        foreach (var cunit1 in PendingProcessUnit)
                        {
                            var desc = db.SubscriptionPlans.FirstOrDefault(p => p.Id == cunit1.PlanTypeId).PlanName;
                            var PlanSelectedDisplay = new
                            {
                                cunit1.UnitName,
                                PlanName = desc,
                                Description = desc.Substring(0, (desc.Length > 256 ? 256 : desc.Length)),
                                Price = cunit1.PricePerMonth,
                                PlanType = (cunit1.IsSpecialApplied == true ? "Special Offer" : "Recurring"),
                                Id = cunit1.Id,
                                cunit1.AddressId,
                                cunit1.ClientId,
                                ClientName = cunit1.Client.FirstName + " " + cunit1.Client.LastName
                                //CurrentPaymentMethod = string.IsNullOrWhiteSpace(cunit1.CurrentPaymentMethod) ? "" : cunit1.CurrentPaymentMethod
                            };
                            CurrentPaymentMethod = string.IsNullOrWhiteSpace(cunit1.CurrentPaymentMethod) ? "" : cunit1.CurrentPaymentMethod;
                            total = total + cunit1.PricePerMonth.Value;
                            data.Add(PlanSelectedDisplay);
                            var pfUnit = db.ClientUnits.Find(cunit1.Id);
                            pfUnit.PaymentStatus = Utilities.UnitPaymentTypes.NotReceived.GetEnumDescription();
                            db.SaveChanges();
                        }
                        var response = new
                        {
                            Units = data,
                            Total = total,
                            CurrentPaymentMethod = CurrentPaymentMethod,
                            Message = (PendingProcessUnit.Count(x => x.IsSpecialApplied == true) == PendingProcessUnit.Count ? "" : "(Recurring Billing occur every month)"),
                        };
                        res.StatusCode = (int)HttpStatusCode.OK;
                        res.Message = "Records Found.";
                        res.Data = response;
                    }
                    else
                    {
                        res.StatusCode = (int)HttpStatusCode.OK;
                        res.Message = "Unit Not Found";
                        res.Data = d;
                    }
                }
            }
            catch (Exception ex)
            {
                res.StatusCode = (int)HttpStatusCode.BadRequest;
                res.Message = "Invalid Request.";
                res.Data = new { };
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
        [Route("DeleteOldData")]
        public async Task<IHttpActionResult> DeleteOldData([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            try
            {
                var deleteunits = db.ClientUnits.AsEnumerable().Where(x => x.AddedBy == request.EmployeeId && x.AddedByType == Utilities.UserRoles.Employee.GetEnumValue() && (x.PaymentStatus == Utilities.UnitPaymentTypes.NotReceived.GetEnumDescription() || x.PaymentStatus == Utilities.UnitPaymentTypes.PaymentFailed.GetEnumDescription())).ToList();
                foreach (ClientUnit unit in deleteunits)
                {
                    var ueis = db.UnitExtraInfoes.Where(x => x.UnitId == unit.Id).ToList();
                    db.UnitExtraInfoes.RemoveRange(ueis);

                    var ClientUnitParts = await db.ClientUnitParts.Where(x => x.UnitId == unit.Id).ToListAsync();
                    db.ClientUnitParts.RemoveRange(ClientUnitParts);

                    var ClientUnitServiceCount = db.ClientUnitServiceCounts.Where(x => x.UnitId == unit.Id).ToList();
                    db.ClientUnitServiceCounts.RemoveRange(ClientUnitServiceCount);

                    var um = unit.ClientUnitManuals.ToList();
                    db.ClientUnitManuals.RemoveRange(um);

                    var unitImage = unit.ClientUnitPictures.ToList();
                    db.ClientUnitPictures.RemoveRange(unitImage);
                }

                var PaymentFailedNotifications = db.UserNotifications.AsEnumerable().Where(x => x.UserId == deleteunits[0].ClientId && x.UserTypeId == Utilities.UserRoles.Client.GetEnumValue() && x.MessageType == Utilities.NotificationType.PaymentFailed.GetEnumDescription()).ToList();
                if (PaymentFailedNotifications.Count() > 0)
                {
                    db.UserNotifications.RemoveRange(PaymentFailedNotifications);
                }

                db.ClientUnits.RemoveRange(deleteunits);
                db.SaveChanges();
                res.StatusCode = (int)HttpStatusCode.OK;
                res.Message = "Records Deleted.";
                res.Data = null;
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
        [Route("SubmitPartQuote")]
        public async Task<IHttpActionResult> SubmitPartQuote(OrderModel request)
        {
            ResponseModel res = new ResponseModel();
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
                string EmployeeId = request.EmployeeId;
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
                    cId = request.ClientId;
                    string Address = request.Address;
                    int City = request.City;
                    int State = request.State;
                    string ZipCode = request.ZipCode;
                    string Email = request.Email;
                    string ChargeBy = request.ChargeBy;

                    string Recommendation = request.Recommendation;
                    bool EmailToClientEmail = request.EmailToClientEmail;
                    string CCEmail = "";
                    try
                    {
                        CCEmail = request.CCEmail;
                    }
                    catch (Exception ex)
                    {
                    }
                    var Parts = request.OrderItems;

                    var PartsList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<OrderPartModel>>(Parts);
                    var Amount = 0m;
                    var ClientInfo = db.Clients.Find(cId);
                    Order ord = new Order();
                    ord.ClientId = cId;
                    ord.OrderType = Utilities.BillingTypes.FixedCost.GetEnumDescription();
                    ord.ChargeBy = ChargeBy;
                    ord.CCEmail = CCEmail;
                    ord.CustomerRecommendation = Recommendation;
                    ord.AddedBy = int.Parse(EmployeeId);
                    ord.AddedByType = Utilities.UserRoles.Employee.GetEnumValue();
                    ord.AddedDate = DateTime.UtcNow;
                    foreach (var item in PartsList)
                    {
                        var part = db.Parts.Find(item.PartId);
                        Amount = Amount + (part.SellingPrice.Value * item.Quantity);
                        ord.OrderItems.Add(new OrderItem()
                        {
                            Amount = part.SellingPrice,
                            PartId = part.Id,
                            PartSize = part.Size,
                            Quantity = item.Quantity,
                            PartName = part.Name
                        });
                    }
                    ord.OrderAmount = Amount;
                    var cords = db.Orders.Where(x => x.ClientId == ClientInfo.Id).Count();
                    var ordernumber = ClientInfo.AccountNumber.ToString() + "-" + ZipCode + "-Q" + (cords + 1).ToString();
                    ord.OrderNumber = ordernumber;
                    var eId = int.Parse(EmployeeId);
                    //var EmpInfo = db.Employees.Find(eId);
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
                    res.Message = "Quote sent successfully";
                    res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                    //var eId = int.Parse(EmployeeId);
                    //var EmpInfo = db.Employees.Find(eId);

                    try
                    {
                        //var orderMail = Orders.FirstOrDefault();
                        EmailTemplate templateclient = db.EmailTemplates.Where(x => x.Name == "QuotationEmailClient" && x.Status == true).FirstOrDefault();
                        var strclient = templateclient.EmailBody;
                        EmailTemplate templateEmp = db.EmailTemplates.Where(x => x.Name == "QuotationEmailEmployee" && x.Status == true).FirstOrDefault();
                        var strEmp = templateEmp.EmailBody;
                        EmailTemplate templateAdmin = db.EmailTemplates.Where(x => x.Name == "QuotationEmailAdmin" && x.Status == true).FirstOrDefault();
                        var strAdmin = templateAdmin.EmailBody;
                        //client
                        strclient = strclient.Replace("{{FirstName}}", ClientInfo.FirstName);
                        strclient = strclient.Replace("{{LastName}}", ClientInfo.LastName);
                        strclient = strclient.Replace("{{Email}}", ClientInfo.Email);
                        strclient = strclient.Replace("{{PhoneNumber}}", ClientInfo.MobileNumber);
                        strclient = strclient.Replace("{{OrderDate}}", ord.AddedDate.ToString("MM/dd/yyyy"));
                        strclient = strclient.Replace("{{EmpName}}", EmpInfo.FirstName + " " + EmpInfo.LastName);
                        //emp
                        strEmp = strEmp.Replace("{{FirstName}}", ClientInfo.FirstName);
                        strEmp = strEmp.Replace("{{LastName}}", ClientInfo.LastName);
                        strEmp = strEmp.Replace("{{Email}}", ClientInfo.Email);
                        strEmp = strEmp.Replace("{{PhoneNumber}}", ClientInfo.MobileNumber);
                        strEmp = strEmp.Replace("{{OrderDate}}", ord.AddedDate.ToString("MM/dd/yyyy"));
                        strEmp = strEmp.Replace("{{EmpName}}", EmpInfo.FirstName + " " + EmpInfo.LastName);
                        //admin
                        strAdmin = strAdmin.Replace("{{FirstName}}", ClientInfo.FirstName);
                        strAdmin = strAdmin.Replace("{{LastName}}", ClientInfo.LastName);
                        strAdmin = strAdmin.Replace("{{Email}}", ClientInfo.Email);
                        strAdmin = strAdmin.Replace("{{PhoneNumber}}", ClientInfo.MobileNumber);
                        strAdmin = strAdmin.Replace("{{OrderDate}}", ord.AddedDate.ToString("MM/dd/yyyy"));
                        strAdmin = strAdmin.Replace("{{EmpName}}", EmpInfo.FirstName + " " + EmpInfo.LastName);
                        //"<tr><td>SR</td><td>Part</td><td>Qty</td><td>Rate</td></tr>"
                        var stritems = "";
                        var total = 0m;
                        for (int i = 0; i < ord.OrderItems.Count; i++)
                        {
                            var item = ord.OrderItems.ToList()[i];
                            total += item.Quantity * item.Amount.Value;
                            stritems = stritems + "<tr><td>" + (i + 1).ToString() + "</td><td>" + item.PartName + " - " + item.PartSize + "</td><td>" + item.Quantity + "</td><td align='right'>" + item.Amount + "</td><td align='right'>" + item.Quantity * item.Amount.Value + "</td></tr>";
                        }
                        stritems = "<table style='width:100%;font-family:Arial,sans-serif;font-size:14px;line-height:21px;color:#333;border-color: #e2e2e2;border-collapse: collapse;' border='1'><tr><th>SR</th><th>Part</th><th>Qty</th><th>Rate</th><th>Total</th></tr>" + stritems + "<tr><td colspan='4' align='right'>Total</td><td align='right'>$" + total.ToString("0.00") + "</td></tr></table>";
                        strclient = strclient.Replace("{{orderitems}}", stritems);
                        strEmp = strEmp.Replace("{{orderitems}}", stritems);
                        strAdmin = strAdmin.Replace("{{orderitems}}", stritems);

                        //client
                        Utilities.Send(templateclient.EmailTemplateSubject, ClientInfo.Email, strclient, templateclient.FromEmail, db, "", CCEmail);
                        //emp
                        Utilities.Send(templateEmp.EmailTemplateSubject, EmpInfo.Email, strEmp, templateEmp.FromEmail, db, "", "");
                        //admin
                        Utilities.Send(templateAdmin.EmailTemplateSubject, Utilities.GetSiteSettingValue("AdminEmail", db), strAdmin, templateAdmin.FromEmail, db, "", templateAdmin.CCEmails);
                    }
                    catch (Exception ex1)
                    {

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
        }

        [EMPAuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("ValidateClientAddress")]
        public async Task<IHttpActionResult> ValidateClientAddress([FromBody]ClientAddressModel ModelData)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            try
            {
                if (!string.IsNullOrWhiteSpace(ModelData.ZipCode))
                {
                    var zips = db.ZipCodes.Where(x => x.CitiesId == ModelData.City && x.StateId == ModelData.State && x.ZipCode1 == ModelData.ZipCode && x.Status == true && x.PendingInactive == false).ToList();
                    if (zips.Count <= 0)
                    {
                        res.StatusCode = (int)HttpStatusCode.BadRequest;
                        res.Message = "Please enter a valid Zip Code.";
                        res.Data = null;
                        if (updatetoken)
                        {
                            res.Token = accessToken;
                        }
                        else
                        {
                            res.Token = "";
                        }
                        return Ok(res);
                    }
                    else
                    {
                        var zip = zips.FirstOrDefault();
                        if (zip.Status == false || zip.PendingInactive == true)
                        {
                            res.StatusCode = (int)HttpStatusCode.BadRequest;
                            res.Message = Utilities.GetSiteSettingValue("ServiceInactiveZipCodeMessage", db); //"Service in this zip code is inactivate.";
                            res.Data = null;
                            if (updatetoken)
                            {
                                res.Token = accessToken;
                            }
                            else
                            {
                                res.Token = "";
                            }
                            return Ok(res);
                        }
                    }
                }
                else
                {
                    var zips = db.ZipCodes.Where(x => x.CitiesId == ModelData.City && x.StateId == ModelData.State && x.ZipCode1 == ModelData.ZipCode && x.Status == true).ToList();
                    if (zips.Count <= 0)
                    {
                        res.StatusCode = (int)HttpStatusCode.BadRequest;
                        res.Message = "Please enter valid Zipcode.";
                        res.Data = null;
                        if (updatetoken)
                        {
                            res.Token = accessToken;
                        }
                        else
                        {
                            res.Token = "";
                        }
                        return Ok(res);
                    }
                }
                res.StatusCode = (int)HttpStatusCode.OK;
                res.Message = "";
                res.Data = null;
                if (updatetoken)
                {
                    res.Token = accessToken;
                }
                else
                {
                    res.Token = "";
                }
                return Ok(res);
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
    }
}