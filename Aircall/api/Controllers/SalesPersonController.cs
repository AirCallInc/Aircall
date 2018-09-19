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
using PdfSharp.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace api.Controllers
{

    [RoutePrefix("v1/employee")]
    public class SalesPersonController : BaseEmployeeApiController
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
        [Route("AddClientUnitSalesPerson")]
        public async Task<IHttpActionResult> AddClientUnitSalesPerson([FromBody]ClientUnitAddModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            try
            {
                var objClientUnitService = Services.ServiceFactory.ClientUnitService;
                for (int i = 0; i < request.Qty; i++)
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
                    var unitCount = db.ClientUnits.Where(x => x.ClientId == request.ClientId && x.IsDeleted == false).OrderByDescending(x => x.AddedDate).FirstOrDefault();
                    var cunit = AutoMapper.Mapper.Map<ClientUnit>(request);
                    if (request.AutoRenewal == true)
                    {
                        cunit.AutoRenewal = true;
                    }
                    //else 
                    if (request.SpecialOffer == true)
                    {
                        cunit.IsSpecialApplied = true;
                    }
                    cunit.IsActive = true;
                    if (request.OptionalInformation != null)
                    {
                        if (request.OptionalInformation.Count == 1)
                        {
                            ClientUnitOptions op = request.OptionalInformation.FirstOrDefault();
                            if (op.SplitType.ToLower() == "packaged")
                            {
                                cunit.UnitTypeId = 1;
                            }
                            else if (op.SplitType.ToLower() == "heating")
                            {
                                cunit.UnitTypeId = 3;
                            }
                        }
                        else if (request.OptionalInformation.Count == 2)
                        {
                            cunit.UnitTypeId = 2;
                        }
                    }

                    var year = (request.ManufactureDate == null ? 1m : Convert.ToDecimal(DateTime.UtcNow.TotalMonths(request.ManufactureDate.Value)) / 12m);

                    cunit.UnitName = (string.IsNullOrWhiteSpace(request.UnitName) ? generateUnitName(db, request.ClientId, 0) : request.UnitName + (i + 1).ToString());
                    cunit.PlanTypeId = request.PlanTypeId;
                    cunit.IsDeleted = false;
                    cunit.AddedBy = request.EmployeeId;
                    cunit.AddedByType = (int)Utilities.UserRoles.Employee;
                    cunit.Status = (int)Utilities.UnitStatus.ServiceSoon;
                    cunit.AddedDate = DateTime.UtcNow;
                    cunit.PaymentStatus = "NotReceived";
                    cunit.IsServiceAdded = false;
                    cunit.CurrentPaymentMethod = request.CurrentPaymentMethod;
                    cunit.VisitPerYear = request.VisitPerYear;
                    cunit.PricePerMonth = objClientUnitService.GetPricePerMonth(request.PlanTypeId.Value.ToString(), request.VisitPerYear);
                    cunit.IsSubmittedToSubscription = false;

                    db.ClientUnits.Add(cunit);

                    await db.SaveChangesAsync();

                    if (request.OptionalInformation != null)
                    {
                        if (request.OptionalInformation.Count == 1)
                        {
                            ClientUnitOptions op = request.OptionalInformation.FirstOrDefault();

                            ClientUnitPart part = new ClientUnitPart();
                            part.ModelNumber = op.ModelNumber;
                            part.SerialNumber = op.SerialNumber;
                            part.Thermostat = op.ThermostatTypes;
                            part.UnitTon = op.UnitTon;
                            part.ManufactureDate = request.ManufactureDate;
                            part.SplitType = op.SplitType;
                            part.ManufactureBrand = "";
                            part.ElectricalService = "";
                            part.MaxBreaker = "";
                            if (!string.IsNullOrWhiteSpace(op.ModelNumber) && !string.IsNullOrWhiteSpace(op.SerialNumber))
                            {
                                var UnitMatched = db.Units.Where(x => x.ModelNumber == op.ModelNumber && x.SerialNumber == op.SerialNumber).FirstOrDefault();
                                if (UnitMatched != null)
                                {
                                    var u = AutoMapper.Mapper.Map<Unit, UnitView>(UnitMatched);
                                    part = AutoMapper.Mapper.Map<UnitView, ClientUnitPart>(u);
                                    cunit.IsMatched = true;
                                }
                            }
                            part.UnitId = cunit.Id;

                            db.ClientUnitParts.Add(part);
                            var partid = await db.SaveChangesAsync();

                            if (op.Filters != null)
                            {
                                if (op.Filters.Count > 0)
                                {
                                    foreach (var uFilter in op.Filters)
                                    {
                                        UnitExtraInfo uei = new UnitExtraInfo();
                                        uei.UnitId = cunit.Id;
                                        uei.PartId = uFilter.size;
                                        uei.ClientUnitPartId = part.Id;
                                        uei.LocationOfPart = uFilter.LocationOfPart;
                                        uei.ExtraInfoType = "Filter";
                                        db.UnitExtraInfoes.Add(uei);
                                    }
                                    db.SaveChanges();
                                }
                            }
                            if (op.FuseTypes != null)
                            {
                                if (op.FuseTypes.Count > 0)
                                {
                                    foreach (var uFuseType in op.FuseTypes)
                                    {
                                        UnitExtraInfo uei = new UnitExtraInfo();
                                        uei.UnitId = cunit.Id;
                                        uei.ClientUnitPartId = part.Id;
                                        uei.PartId = uFuseType.FuseType;
                                        uei.ExtraInfoType = "Fuses";
                                        db.UnitExtraInfoes.Add(uei);
                                    }
                                    db.SaveChanges();
                                }
                            }
                        }
                        else if (request.OptionalInformation.Count == 2)
                        {
                            foreach (var op in request.OptionalInformation)
                            {
                                ClientUnitPart part = new ClientUnitPart();
                                part.ModelNumber = op.ModelNumber;
                                part.SerialNumber = op.SerialNumber;
                                part.ManufactureDate = request.ManufactureDate;
                                part.UnitTon = request.UnitTon;
                                part.Thermostat = op.ThermostatTypes;
                                part.UnitId = cunit.Id;
                                part.SplitType = op.SplitType.Replace("Split-", "");
                                part.ManufactureBrand = "";
                                part.ElectricalService = "";
                                part.MaxBreaker = "";
                                part.UnitTon = op.UnitTon;
                                if (!string.IsNullOrWhiteSpace(op.ModelNumber) && !string.IsNullOrWhiteSpace(op.SerialNumber))
                                {
                                    var UnitMatched = db.Units.Where(x => x.ModelNumber == op.ModelNumber && x.SerialNumber == op.SerialNumber).FirstOrDefault();
                                    if (UnitMatched != null)
                                    {
                                        var u = AutoMapper.Mapper.Map<Unit, UnitView>(UnitMatched);
                                        part = AutoMapper.Mapper.Map<UnitView, ClientUnitPart>(u);
                                    }
                                }
                                part.SplitType = op.SplitType.Replace("Split-", "");
                                part.UnitId = cunit.Id;
                                db.ClientUnitParts.Add(part);
                                var partid = await db.SaveChangesAsync();

                                if (op.Filters != null)
                                {
                                    if (op.Filters.Count > 0)
                                    {
                                        foreach (var uFilter in op.Filters)
                                        {
                                            UnitExtraInfo uei = new UnitExtraInfo();
                                            uei.UnitId = cunit.Id;
                                            uei.PartId = uFilter.size;
                                            uei.ClientUnitPartId = part.Id;
                                            uei.LocationOfPart = uFilter.LocationOfPart;
                                            uei.ExtraInfoType = "Filter";
                                            db.UnitExtraInfoes.Add(uei);
                                        }
                                        db.SaveChanges();
                                    }
                                }
                                if (op.FuseTypes != null)
                                {
                                    if (op.FuseTypes.Count > 0)
                                    {
                                        foreach (var uFuseType in op.FuseTypes)
                                        {
                                            UnitExtraInfo uei = new UnitExtraInfo();
                                            uei.UnitId = cunit.Id;
                                            uei.ClientUnitPartId = part.Id;
                                            uei.PartId = uFuseType.FuseType;
                                            uei.ExtraInfoType = "Fuses";
                                            db.UnitExtraInfoes.Add(uei);
                                        }
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        ClientUnitPart part = new ClientUnitPart();

                        part.UnitId = cunit.Id;
                        part.SplitType = "Packaged";
                        part.ManufactureBrand = "";
                        part.ManufactureDate = request.ManufactureDate;
                        db.ClientUnitParts.Add(part);
                        db.SaveChanges();
                    } 
                }

                var PendingProcessUnit = db.ClientUnits.AsEnumerable().Where(x => x.ClientId == request.ClientId && (x.IsSubmittedToSubscription == null || x.IsSubmittedToSubscription == false)).ToList();
                List<object> data = new List<object>();
                decimal total = 0m;
                bool isValid = true;
                foreach (var cunit1 in PendingProcessUnit)
                {
                    var desc = db.SubscriptionPlans.FirstOrDefault(p => p.Id == cunit1.PlanTypeId.Value).PlanName;
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
                    ErrMessage = (isValid ? "" : "Some of your unit plan is InActive / Changed Please remove Unit and Add again.")
                };

                res.StatusCode = (int)HttpStatusCode.OK;
                res.Message = "Record Saved.";
                res.Data = response;
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
    }
}
