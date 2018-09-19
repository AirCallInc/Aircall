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
using System.Globalization;
using System.Web.Http.Cors;
using System.Data;
using DBUtility;

namespace api.Controllers
{
    public class FileUploadResult
    {
        public string LocalFilePath { get; set; }
        public string FileName { get; set; }
        public long FileLength { get; set; }
    }
    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            return headers.ContentDisposition.FileName.Replace("\"", string.Empty);
        }
    }
    public class UploadMultipartFormProvider : MultipartFormDataStreamProvider
    {
        public UploadMultipartFormProvider(string rootPath) : base(rootPath) { }

        public override string GetLocalFileName(HttpContentHeaders headers)
        {
            if (headers != null &&
                headers.ContentDisposition != null)
            {
                var fname = DateTime.UtcNow.Ticks.ToString() + headers
                    .ContentDisposition
                    .FileName.TrimEnd('"').TrimStart('"');
                if (!fname.Contains("."))
                {
                    fname = fname + ".png";
                }
                return fname;
            }

            return base.GetLocalFileName(headers);
        }
    }

    [RoutePrefix("v1/profile")]
    public class ProfileController : BaseClientApiController
    {
        Aircall_DBEntities1 db;
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("clientlogin")]
        public async Task<IHttpActionResult> Login([FromBody]LoginModel model)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            try
            {
                var user = await db.Clients.Where(x => x.Email == model.Email && x.Password == model.Password && x.IsDeleted == false).FirstOrDefaultAsync();
                if (user != null)
                {
                    TokenDetails objToken = await generatToken(model.Email, model.Password, model.DeviceToken);
                    if (String.IsNullOrEmpty(objToken.error))
                    {
                        if (user.IsActive)
                        {
                            if (string.IsNullOrWhiteSpace(user.CustomerProfileId))
                            {
                                try
                                {
                                    var objClientService = Services.ServiceFactory.ClientService;
                                    string customerProfileId = "";
                                    var errCode = "";
                                    var errText = "";
                                    var email = user.Email;
                                    var description = user.FirstName + " " + user.LastName;
                                    var ret = objClientService.AddClientToAuthorizeNet(email, description, ref customerProfileId, ref errCode, ref errText);
                                    user.CustomerProfileId = customerProfileId;
                                    user.VersionFlag = "2.0";
                                    if (!ret)
                                    {
                                        res.StatusCode = (int)HttpStatusCode.InternalServerError;
                                        res.Message = "Add client to Authorizenet failed. " + errText;
                                        res.Data = null;
                                        db.Dispose();
                                        return Ok(errText);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //StripeErrorLog err = Mapper.Map<StripeErrorLog>(stex.StripeError);
                                    //err.Userid = user.Id;
                                    //db.StripeErrorLogs.Add(err);

                                    res.StatusCode = (int)HttpStatusCode.InternalServerError;
                                    res.Message = "Add client to Authorizenet failed 2. " + ex.Message.ToString().Trim();
                                    res.Data = null;
                                    db.Dispose();
                                    return Ok(ex);
                                }
                            }
                            //db.Entry(user).State = EntityState.Modified;
                            user.DeviceType = model.DeviceType;
                            user.DeviceToken = model.DeviceToken;
                            await db.SaveChangesAsync();
                            var DefaultAddressId = await db.ClientAddresses.Where(x => x.ClientId == user.Id && x.IsDefaultAddress == true).FirstOrDefaultAsync();
                            var clientmodel = new
                            {
                                Id = user.Id,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Email = user.Email,
                                ProfileImage = (user.Image == "" ? "" : ConfigurationManager.AppSettings["ProfileImageURL"] + user.Image),
                                MobileNumber = user.MobileNumber,
                                DefaultAddressId = (DefaultAddressId != null ? DefaultAddressId.Id : 0),
                                Company = user.Company ?? "-"
                            };

                            Add_UpdateToken(user.Id, objToken, 1);

                            res.StatusCode = (int)HttpStatusCode.OK;
                            res.Message = "Login Successful!";
                            res.Token = objToken.access_token;
                            res.Data = clientmodel;
                        }
                        else
                        {
                            res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                            res.Message = "Your account was deactivated by Admin.";
                            res.Token = objToken.access_token;
                            res.Data = null;
                        }
                    }
                    else
                    {
                        res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        res.Message = objToken.error_description;
                        res.Token = objToken.access_token;
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
            catch (Exception ex)
            {
                return Ok(ex);
            }
            db.Dispose();
            return Ok(res);

        }

        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("getClientAccessToken")]
        public async Task<IHttpActionResult> getClientAccessToken([FromBody]LoginModel model)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            var user = await db.Clients.Where(x => x.Email == model.Email && x.Password == model.Password).FirstOrDefaultAsync();
            if (user != null)
            {
                TokenDetails objToken = await generatToken(model.Email, model.Password, model.DeviceToken);
                if (String.IsNullOrEmpty(objToken.error))
                {
                    if (user.IsActive)
                    {
                        if (string.IsNullOrWhiteSpace(user.CustomerProfileId))
                        {
                            try
                            {
                                var myCustomer = new StripeCustomerCreateOptions();

                                myCustomer.Email = user.Email;
                                myCustomer.Description = user.FirstName + ' ' + user.LastName + " (" + user.Email + ")";
                                var customerService = new StripeCustomerService();
                                StripeCustomer stripeCustomer = customerService.Create(myCustomer);
                                user.CustomerProfileId = stripeCustomer.Id;
                            }
                            catch (StripeException stex)
                            {
                                StripeErrorLog err = Mapper.Map<StripeErrorLog>(stex.StripeError);
                                err.Userid = user.Id;
                                db.StripeErrorLogs.Add(err);
                            }
                        }
                        //db.Entry(user).State = EntityState.Modified;
                        user.DeviceType = model.DeviceType;
                        user.DeviceToken = model.DeviceToken;
                        await db.SaveChangesAsync();
                        var DefaultAddressId = await db.ClientAddresses.Where(x => x.ClientId == user.Id && x.IsDefaultAddress == true).FirstOrDefaultAsync();
                        var clientmodel = new
                        {
                            Id = user.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            ProfileImage = (user.Image == "" ? "" : ConfigurationManager.AppSettings["ProfileImageURL"] + user.Image),
                            MobileNumber = user.MobileNumber,
                            DefaultAddressId = (DefaultAddressId != null ? DefaultAddressId.Id : 0)
                        };

                        Add_UpdateToken(user.Id, objToken, 1);

                        res.StatusCode = (int)HttpStatusCode.OK;
                        res.Message = "Login Successfull!";
                        res.Token = objToken.access_token;
                        res.Data = clientmodel;
                    }
                    else
                    {
                        res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        res.Message = "Your account was deactivated by Admin.";
                        res.Token = objToken.access_token;
                        res.Data = null;
                    }
                }
            }
            else
            {
                res.StatusCode = (int)HttpStatusCode.NotFound;
                res.Message = "Incorrect Email or Password";
                res.Data = model;
            }
            db.Dispose();
            return Ok(res);

        }

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("clientUpdateToken")]
        public async Task<IHttpActionResult> clientUpdateToken([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            try
            {
                var UserInfo = db.Clients.Where(x => x.Id == request.ClientId).FirstOrDefault();
                if (UserInfo != null)
                {
                    if (UserInfo.IsActive)
                    {
                        UserInfo.DeviceToken = request.DeviceToken;
                        UserInfo.DeviceType = request.DeviceType;
                        UserInfo.UpdatedBy = request.ClientId;
                        UserInfo.UpdatedByType = Utilities.UserRoles.Client.GetEnumValue();
                        UserInfo.UpdatedDate = DateTime.UtcNow;

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

        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("clientlogout")]
        public async Task<IHttpActionResult> clientlogout([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();


            var user = await db.AppAccessTokens.Where(x => x.UserId == request.ClientId && x.UserType == 4).ToListAsync();
            var UserInfo = db.Clients.Where(x => x.Id == request.ClientId).FirstOrDefault();
            if (user.Count > 0)
            {
                UserInfo.DeviceToken = "";
                UserInfo.DeviceType = "";
                //db.Entry(user).State = EntityState.Modified;
                db.AppAccessTokens.RemoveRange(user);


                await db.SaveChangesAsync();

                res.StatusCode = (int)HttpStatusCode.OK;
                res.Message = "Logout Successful!";
                res.Token = "";
                res.Data = null;
            }
            else
            {
                res.StatusCode = (int)HttpStatusCode.OK;
                res.Message = "Already Logged out";
                res.Data = null;
            }
            db.Dispose();
            return Ok(res);
        }

        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("clientRegister")]
        public async Task<IHttpActionResult> Register([FromBody]RegisterClientModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            try
            {
                var UserInfo = db.Clients.ToList().Where(x => x.Email == request.Email && x.IsDeleted == false).FirstOrDefault();
                if (UserInfo != null)
                {
                    res.StatusCode = (int)HttpStatusCode.Found;
                    res.Message = "Email already registerd.";
                    res.Data = request;
                    db.Dispose();
                    return Ok(res);
                }

                int ClientCnt = db.Clients.Count() + 1;

                int PartnerAffilateId = 0;
                if (!string.IsNullOrEmpty(request.AffiliateId))
                {
                    var PartnerId = db.Partners.ToList().Where(x => x.AssignedAffiliateId == request.AffiliateId && x.IsActive == true && x.IsDeleted == false).FirstOrDefault();
                    if (PartnerId != null)
                    {
                        PartnerAffilateId = PartnerId.Id;
                    }
                    else
                    {
                        res.StatusCode = (int)HttpStatusCode.NotFound;
                        res.Message = "Affiliate Not Found";
                        res.Data = request;
                        db.Dispose();
                        return Ok(res);
                    }
                }

                var objClient = new Client
                {
                    RoleId = (int)Utilities.UserRoles.Client,
                    Company = request.Company,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Password = request.Password,
                    Image = "",
                    MobileNumber = request.MobileNumber,
                    DeviceType = request.DeviceType,
                    DeviceToken = request.DeviceToken,
                    AccountNumber = "C" + ClientCnt.ToString(),
                    IsActive = true,
                    IsDeleted = false,
                    AddedDate = DateTime.UtcNow
                };

                try
                {
                    var objClientService = Services.ServiceFactory.ClientService;
                    string customerProfileId = "";
                    var errCode = "";
                    var errText = "";
                    var email = objClient.Email;
                    var description = objClient.FirstName + " " + objClient.LastName;
                    var ret = objClientService.AddClientToAuthorizeNet(email, description, ref customerProfileId, ref errCode, ref errText);
                    objClient.CustomerProfileId = customerProfileId;
                    objClient.VersionFlag = "2.0";
                    if (!ret)
                    {
                        res.StatusCode = (int)HttpStatusCode.InternalServerError;
                        res.Message = "Add client to Authorizenet failed. " + errText;
                        res.Data = request;
                        db.Dispose();
                        return Ok(errText);
                    }
                }
                catch (Exception ex)
                {
                    //StripeErrorLog err = Mapper.Map<StripeErrorLog>(stex.StripeError);
                    //err.Userid = 0;
                    //db.StripeErrorLogs.Add(err);

                    res.StatusCode = (int)HttpStatusCode.InternalServerError;
                    res.Message = "Add client to Authorizenet failed 2. " + ex.Message.ToString().Trim();
                    res.Data = request;
                    db.Dispose();
                    return Ok(ex);
                }

                if (PartnerAffilateId != 0)
                    objClient.AffiliateId = PartnerAffilateId;

                db.Clients.Add(objClient);
                db.SaveChanges();

                var resp = new
                {
                    Id = objClient.Id,
                    RoleId = (int)Utilities.UserRoles.Client,
                    Company = request.Company,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Password = request.Password,
                    Image = "",
                    MobileNumber = request.MobileNumber,
                    DeviceType = request.DeviceType,
                    DeviceToken = request.DeviceToken,
                    AccountNumber = "C" + ClientCnt.ToString(),
                    IsActive = true,
                    IsDeleted = false,
                    AddedDate = DateTime.UtcNow
                };

                if (request.SendDisclosureEmail)
                {
                    EmailTemplate templateSendDisclosureEmailClient = db.EmailTemplates.Where(x => x.Name == "SendDisclosureEmailClient" && x.Status == true).FirstOrDefault();
                    var strSendDisclosureEmail = templateSendDisclosureEmailClient.EmailBody;

                    var DisclosurePage = db.CMSPages.Where(x => x.Id == 23).FirstOrDefault().Description;
                    strSendDisclosureEmail = strSendDisclosureEmail.Replace("{{Name}}", objClient.FirstName + " " + objClient.FirstName);
                    strSendDisclosureEmail = strSendDisclosureEmail.Replace("{{Message}}", DisclosurePage);

                    Utilities.Send(templateSendDisclosureEmailClient.EmailTemplateSubject, objClient.Email, strSendDisclosureEmail, templateSendDisclosureEmailClient.FromEmail, db);
                }

                EmailTemplate templateAdmin = db.EmailTemplates.Where(x => x.Name == "NewUserAdmin" && x.Status == true).FirstOrDefault();
                var stradmin = templateAdmin.EmailBody;
                stradmin = stradmin.Replace("{{FirstName}}", objClient.FirstName);
                stradmin = stradmin.Replace("{{LastName}}", objClient.LastName);
                stradmin = stradmin.Replace("{{Email}}", objClient.Email);
                stradmin = stradmin.Replace("{{PhoneNumber}}", objClient.MobileNumber);
                stradmin = stradmin.Replace("{{RegisterDate}}", objClient.AddedDate.ToString("MM/dd/yyyy"));
                stradmin = stradmin.Replace("{{Company}}", objClient.Company ?? "-");
                var AdminEmail = Utilities.GetSiteSettingValue("AdminEmail", db);
                Utilities.Send(templateAdmin.EmailTemplateSubject, AdminEmail, stradmin, templateAdmin.FromEmail, db);

                EmailTemplate templateclient = db.EmailTemplates.Where(x => x.Name == "NewUserClient" && x.Status == true).FirstOrDefault();
                var strclient = templateclient.EmailBody;
                var LoginUrl = ConfigurationManager.AppSettings["SiteAddress"].ToString() + "sign-in.aspx";
                strclient = strclient.Replace("{{FirstName}}", objClient.FirstName);
                strclient = strclient.Replace("{{LastName}}", objClient.LastName);
                strclient = strclient.Replace("{{Email}}", objClient.Email);
                strclient = strclient.Replace("{{PhoneNumber}}", objClient.MobileNumber);
                strclient = strclient.Replace("{{RegisterDate}}", objClient.AddedDate.ToString("MM/dd/yyyy"));
                strclient = strclient.Replace("{{LoginUrl}}", LoginUrl);
                strclient = strclient.Replace("{{Company}}", objClient.Company ?? "-");
                Utilities.Send(templateclient.EmailTemplateSubject, objClient.Email, strclient, templateclient.FromEmail, db);

                TokenDetails objToken = await generatToken(request.Email, request.Password, request.DeviceToken);
                if (String.IsNullOrEmpty(objToken.error))
                {
                    Add_UpdateToken(objClient.Id, objToken);
                    res.Token = objToken.access_token;
                }

                res.StatusCode = (int)HttpStatusCode.OK;
                res.Message = "Client Successfully registered!";
                res.Data = resp;
                db.Dispose();
                return Ok(res);
            }
            catch (Exception Ex)
            {
                res.StatusCode = (int)HttpStatusCode.InternalServerError;
                res.Message = Ex.Message.ToString().Trim();
                res.Data = request;
                db.Dispose();
                return Ok(Ex);
            }
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("clientRegister1")]
        public async Task<IHttpActionResult> Register1([FromBody]RegisterClientModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            try
            {
                var UserInfo = db.Clients.ToList().Where(x => x.Email == request.Email && x.IsDeleted == false).FirstOrDefault();
                if (UserInfo != null)
                {
                    res.StatusCode = (int)HttpStatusCode.Found;
                    res.Message = "Email already registerd.";
                    res.Data = request;
                    db.Dispose();
                    return Ok(res);
                }

                int ClientCnt = db.Clients.Count() + 1;

                int PartnerAffilateId = 0;
                if (!string.IsNullOrEmpty(request.AffiliateId))
                {
                    var PartnerId = db.Partners.ToList().Where(x => x.AssignedAffiliateId == request.AffiliateId && x.IsActive == true && x.IsDeleted == false).FirstOrDefault();
                    if (PartnerId != null)
                    {
                        PartnerAffilateId = PartnerId.Id;
                    }
                    else
                    {
                        res.StatusCode = (int)HttpStatusCode.NotFound;
                        res.Message = "Affiliate Not Found";
                        res.Data = request;
                        db.Dispose();
                        return Ok(res);
                    }
                }

                var objClient = new Client
                {

                    RoleId = (int)Utilities.UserRoles.Client,
                    Company = request.Company,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Password = request.Password,
                    Image = "",
                    MobileNumber = request.MobileNumber,
                    DeviceType = request.DeviceType,
                    DeviceToken = request.DeviceToken,
                    AccountNumber = "C" + ClientCnt.ToString(),
                    IsActive = true,
                    IsDeleted = false,
                    AddedDate = DateTime.UtcNow
                };

                try
                {
                    var objClientService = Services.ServiceFactory.ClientService;
                    string customerProfileId = "";
                    var errCode = "";
                    var errText = "";
                    var email = objClient.Email;
                    var description = objClient.FirstName + " " + objClient.LastName;
                    var ret = objClientService.AddClientToAuthorizeNet(email, description, ref customerProfileId, ref errCode, ref errText);
                    objClient.CustomerProfileId = customerProfileId;
                    objClient.VersionFlag = "2.0";
                    if (!ret)
                    {
                        res.StatusCode = (int)HttpStatusCode.InternalServerError;
                        res.Message = errText;
                        res.Data = request;
                        db.Dispose();
                        return Ok(errText);
                    }
                }
                catch (Exception ex)
                {
                    //StripeErrorLog err = Mapper.Map<StripeErrorLog>(stex.StripeError);
                    //err.Userid = 0;
                    //db.StripeErrorLogs.Add(err);
                    res.StatusCode = (int)HttpStatusCode.InternalServerError;
                    res.Message = ex.Message.ToString().Trim();
                    res.Data = request;
                    db.Dispose();
                    return Ok(ex);
                }

                if (PartnerAffilateId != 0)
                    objClient.AffiliateId = PartnerAffilateId;

                db.Clients.Add(objClient);
                db.SaveChanges();

                if (request.SendDisclosureEmail)
                {
                    EmailTemplate templateSendDisclosureEmailClient = db.EmailTemplates.Where(x => x.Name == "SendDisclosureEmailClient" && x.Status == true).FirstOrDefault();
                    var strSendDisclosureEmail = templateSendDisclosureEmailClient.EmailBody;

                    var DisclosurePage = db.CMSPages.Where(x => x.Id == 23).FirstOrDefault().Description;
                    strSendDisclosureEmail = strSendDisclosureEmail.Replace("{{Name}}", objClient.FirstName + " " + objClient.FirstName);
                    strSendDisclosureEmail = strSendDisclosureEmail.Replace("{{Message}}", DisclosurePage);

                    Utilities.Send(templateSendDisclosureEmailClient.EmailTemplateSubject, objClient.Email, strSendDisclosureEmail, templateSendDisclosureEmailClient.FromEmail, db);
                }

                EmailTemplate templateAdmin = db.EmailTemplates.Where(x => x.Name == "NewUserAdmin" && x.Status == true).FirstOrDefault();
                var stradmin = templateAdmin.EmailBody;
                stradmin = stradmin.Replace("{{FirstName}}", objClient.FirstName);
                stradmin = stradmin.Replace("{{LastName}}", objClient.LastName);
                stradmin = stradmin.Replace("{{Email}}", objClient.Email);
                stradmin = stradmin.Replace("{{PhoneNumber}}", objClient.MobileNumber);
                stradmin = stradmin.Replace("{{RegisterDate}}", objClient.AddedDate.ToString("MM/dd/yyyy"));
                stradmin = stradmin.Replace("{{Company}}", objClient.Company ?? "-");
                var AdminEmail = Utilities.GetSiteSettingValue("AdminEmail", db);
                Utilities.Send(templateAdmin.EmailTemplateSubject, AdminEmail, stradmin, templateAdmin.FromEmail, db);

                EmailTemplate templateclient = db.EmailTemplates.Where(x => x.Name == "NewUserClient" && x.Status == true).FirstOrDefault();
                var strclient = templateclient.EmailBody;
                var LoginUrl = ConfigurationManager.AppSettings["SiteAddress"].ToString() + "sign-in.aspx";
                strclient = strclient.Replace("{{FirstName}}", objClient.FirstName);
                strclient = strclient.Replace("{{LastName}}", objClient.LastName);
                strclient = strclient.Replace("{{Email}}", objClient.Email);
                strclient = strclient.Replace("{{PhoneNumber}}", objClient.MobileNumber);
                strclient = strclient.Replace("{{RegisterDate}}", objClient.AddedDate.ToString("MM/dd/yyyy"));
                strclient = strclient.Replace("{{LoginUrl}}", LoginUrl);
                strclient = strclient.Replace("{{Company}}", objClient.Company ?? "-");
                Utilities.Send(templateclient.EmailTemplateSubject, objClient.Email, strclient, templateclient.FromEmail, db);

                res.StatusCode = (int)HttpStatusCode.OK;
                res.Message = "Client Successfully registered!";
                res.Data = "";
                db.Dispose();
                return Ok(res);
            }
            catch (Exception Ex)
            {
                res.StatusCode = (int)HttpStatusCode.InternalServerError;
                res.Message = Ex.Message.ToString().Trim();
                res.Data = request;
                db.Dispose();
                return Ok(Ex);
            }
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [ResponseType(typeof(ResponseModel))]
        [HttpGet]
        [Route("checkClientExists")]
        public async Task<IHttpActionResult> checkClientExists([FromUri]string Email)
        {
            Email = HttpContext.Current.Server.UrlDecode(Email);
            Email = Email.Replace(" ", "+");
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            try
            {
                var UserInfo = db.Clients.ToList().Where(x => x.Email == Email && x.IsDeleted == false).FirstOrDefault();
                var PreRegistration = db.PreRegistrations.Where(x => x.Email == Email).FirstOrDefault();
                if (UserInfo != null)
                {
                    res.StatusCode = (int)HttpStatusCode.Found;
                    res.Message = "Email already registerd.";
                    res.Data = Email;
                    db.Dispose();
                    return Ok(res);
                }
                //string Email = request.Email;
                if (PreRegistration == null)
                {
                    db.PreRegistrations.Add(new PreRegistration() { Email = Email });
                    db.SaveChanges();
                }

                res.StatusCode = (int)HttpStatusCode.OK;
                res.Message = Email;
                res.Data = Email;
                db.Dispose();
                return Ok(res);
            }
            catch (Exception Ex)
            {
                res.StatusCode = (int)HttpStatusCode.InternalServerError;
                res.Message = Ex.Message.ToString().Trim();
                res.Data = Email;
                db.Dispose();
                return Ok(res);
            }
        }

        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("clientForgotPassword")]
        public async Task<IHttpActionResult> ForgotPassword([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            int RandomStringLength = Convert.ToInt32(ConfigurationManager.AppSettings["RandomStringLength"].ToString());

            var UserInfo = db.Clients.Where(x => x.Email == request.Email && x.IsActive == true && x.IsDeleted == false).FirstOrDefault();
            if (UserInfo != null)
            {
                string randomString = Guid.NewGuid().ToString().Substring(0, RandomStringLength);
                UserInfo.PasswordUrl = randomString;
                UserInfo.ResetPasswordLinkExpireDate = DateTime.UtcNow.AddHours(24);
                UserInfo.IsLinkActive = true;
                //db.Entry(UserInfo).State = EntityState.Modified;
                await db.SaveChangesAsync();

                //Email Reset-password link
                //string Emailbody = "Reset Password Link: " + ConfigurationManager.AppSettings["PasswordUrl"].ToString() + randomString;
                //Utilities.Send("testlocalcoding@gmail.com", "this.admin", "Reset Password", request.Email, Emailbody, "smtp.gmail.com", 587, true);
                EmailTemplate templateAdmin = db.EmailTemplates.Where(x => x.Name == "ResetPasswordClient" && x.Status == true).FirstOrDefault();
                var stradmin = templateAdmin.EmailBody;
                stradmin = stradmin.Replace("{{Name}}", UserInfo.FirstName.ToString() + " " + UserInfo.LastName.ToString());
                stradmin = stradmin.Replace("{{Link}}", ConfigurationManager.AppSettings["PasswordUrl"].ToString() + randomString);

                var AdminEmail = Utilities.GetSiteSettingValue("AdminEmail", db);
                Utilities.Send(templateAdmin.EmailTemplateSubject, UserInfo.Email, stradmin, templateAdmin.FromEmail, db);
                res.StatusCode = (int)HttpStatusCode.OK;
                res.Message = "Forgot password link sent to registered email.";
                res.Data = request.Email;
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("clientChangePassword")]
        public async Task<IHttpActionResult> ChangePassword([FromBody]ClientChangePasswordModel ModelData)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            try
            {
                var UserInfo = db.Clients.Where(x => x.Id == ModelData.Id && x.Password == ModelData.OldPassword).FirstOrDefault();
                if (UserInfo != null)
                {
                    if (UserInfo.IsActive)
                    {
                        UserInfo.Password = ModelData.NewPassword;
                        UserInfo.UpdatedDate = DateTime.UtcNow;

                        //db.Entry(UserInfo).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        res.StatusCode = (int)HttpStatusCode.OK;
                        res.Message = "Password changed successfully.";
                        res.Data = ModelData;
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
                    res.Message = "Old Password is invalid.";
                    res.Data = ModelData;
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpGet]
        [Route("GetClientProfile")]
        public async Task<IHttpActionResult> GetClientProfile([FromUri]int ClientId)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            var UserInfo = await db.Clients.Where(x => x.Id == ClientId && x.IsActive == true && x.IsDeleted == false).FirstOrDefaultAsync();
            if (UserInfo != null)
            {

                if (UserInfo.IsActive)
                {
                    var FinalUserInfo = AutoMapper.Mapper.Map<ClientProfileModel>(UserInfo);
                    FinalUserInfo.ProfileImage = (FinalUserInfo.Image != "" ? ConfigurationManager.AppSettings["ProfileImageURL"].ToString() + FinalUserInfo.Image : "");
                    res.StatusCode = (int)HttpStatusCode.OK;
                    res.Message = "Success";
                    res.Data = FinalUserInfo;
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

        [MimeMultipart]
        [HttpPost]
        [Route("PostFile1")]
        public async Task<FileUploadResult> PostFile1()
        {
            var uploadPath = new DirectoryInfo(HttpContext.Current.Server.MapPath("../../")).Parent.FullName + ConfigurationManager.AppSettings["ClientProfilePath"].ToString();//HttpContext.Current.Server.MapPath("~/Uploads");

            var multipartFormDataStreamProvider = new UploadMultipartFormProvider(uploadPath);

            // Read the MIME multipart asynchronously 
            await Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProvider);

            string _localFileName = multipartFormDataStreamProvider
                .FileData.Select(multiPartData => multiPartData.LocalFileName).FirstOrDefault();

            // Create response
            return new FileUploadResult
            {
                LocalFilePath = _localFileName,

                FileName = Path.GetFileName(_localFileName),

                FileLength = new FileInfo(_localFileName).Length
            };
        }

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("UpdateClientProfile")]
        public Task<IHttpActionResult> UpdateClientProfile()
        {
            ResponseModel res = new ResponseModel();

            var uploadPath = new DirectoryInfo(HttpContext.Current.Server.MapPath("../../")).Parent.FullName + ConfigurationManager.AppSettings["ClientProfilePath"].ToString();

            var multipartFormDataStreamProvider = new UploadMultipartFormProvider(uploadPath);
            var task = Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProvider).
            ContinueWith<IHttpActionResult>(t =>
            {
                try
                {
                    db = new Aircall_DBEntities1();
                    ClientProfileModel ModelData = new ClientProfileModel();
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
                    }
                    //await Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProvider);

                    ModelData.Id = int.Parse(multipartFormDataStreamProvider.FormData.GetValues("Id").First());

                    ModelData.FirstName = multipartFormDataStreamProvider.FormData.GetValues("FirstName").First();

                    ModelData.LastName = multipartFormDataStreamProvider.FormData.GetValues("LastName").First();

                    try
                    {
                        ModelData.Company = multipartFormDataStreamProvider.FormData.GetValues("Company").First();
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
                    var UserInfo = db.Clients.Where(x => x.Id == ModelData.Id && x.IsActive == true && x.IsDeleted == false).FirstOrDefault();
                    if (UserInfo != null)
                    {
                        if (UserInfo.IsActive)
                        {
                            //UserInfo.Id = ModelData.Id;
                            UserInfo.FirstName = (ModelData.FirstName.Trim() == "" ? UserInfo.FirstName : ModelData.FirstName);
                            UserInfo.LastName = (ModelData.LastName.Trim() == "" ? UserInfo.LastName : ModelData.LastName);
                            UserInfo.Image = (ModelData.Image.Trim() == "" ? UserInfo.Image : ModelData.Image);
                            UserInfo.Company = ModelData.Company ?? UserInfo.Company;
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
                                catch (StripeException stex)
                                {
                                    StripeErrorLog err = Mapper.Map<StripeErrorLog>(stex.StripeError);
                                    err.Userid = UserInfo.Id;
                                    db.StripeErrorLogs.Add(err);
                                }
                            }
                            else
                            {
                                try
                                {
                                    var objClientService = Services.ServiceFactory.ClientService;
                                    var email = UserInfo.Email;
                                    var description = UserInfo.FirstName + ' ' + UserInfo.LastName;
                                    var errCode = "";
                                    var errText = "";
                                    objClientService.UpdateClientToAuthorizeNet(email, description, UserInfo.CustomerProfileId, ref errCode, ref errText);

                                }
                                catch (Exception ex)
                                {
                                    //StripeErrorLog err = Mapper.Map<StripeErrorLog>(stex.StripeError);
                                    //err.Userid = UserInfo.Id;
                                    //db.StripeErrorLogs.Add(err);
                                }
                            }
                            db.SaveChanges();

                            var FinalUserInfo = AutoMapper.Mapper.Map<ClientProfileModel>(UserInfo);
                            FinalUserInfo.Image = (FinalUserInfo.Image != "" ? ConfigurationManager.AppSettings["ProfileImageURL"].ToString() + FinalUserInfo.Image : "");
                            res.StatusCode = (int)HttpStatusCode.OK;
                            res.Message = "Profile updated successfully.";
                            res.Data = FinalUserInfo;
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
                        res.Message = "User not found.";
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
            });
            return task;
        }

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("UpdateClientContactInfo")]
        public async Task<IHttpActionResult> UpdateClientContactInfo([FromBody]ClientContactInfoModel ModelData)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            try
            {
                var UserInfo = db.Clients.Where(x => x.Id == ModelData.Id && x.IsDeleted == false).FirstOrDefault();
                if (UserInfo != null)
                {
                    if (UserInfo.IsActive)
                    {
                        UserInfo.Id = ModelData.Id;
                        UserInfo.MobileNumber = ModelData.MobileNumber;
                        UserInfo.OfficeNumber = ModelData.OfficeNumber;
                        UserInfo.HomeNumber = ModelData.HomeNumber;
                        UserInfo.UpdatedDate = DateTime.UtcNow;
                        await db.SaveChangesAsync();

                        res.StatusCode = (int)HttpStatusCode.OK;
                        res.Message = "Contact updated successfully.";
                        res.Data = ModelData;
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
                    res.Message = "User not found.";
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

        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("GetClientToken")]
        public async Task<IHttpActionResult> GetClientToken([FromBody]CommonRequest ModelData)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            var chkToken = db.AppAccessTokens.AsEnumerable().Where(top => top.UserId == ModelData.ClientId && top.UserType == Utilities.UserRoles.Client.GetEnumValue()).FirstOrDefault();
            Client model = db.Clients.Where(x => x.Id == ModelData.ClientId).FirstOrDefault();
            if (chkToken != null)
            {
                db.AppAccessTokens.Remove(chkToken);
                db.SaveChanges();
                TokenDetails objToken = api.Helpers.AsyncHelpers.RunSync<TokenDetails>(() => BaseClientApiController.generatToken(model.Email, model.Password, model.DeviceToken));
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
                TokenDetails objToken = api.Helpers.AsyncHelpers.RunSync<TokenDetails>(() => BaseClientApiController.generatToken(model.Email, model.Password, model.DeviceToken));
                if (String.IsNullOrEmpty(objToken.error))
                {
                    res.Token = objToken.access_token;
                    res.Message = "Token Generated";
                    res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                    Add_UpdateToken(model.Id, objToken, 1);
                }
            }
            db.Dispose();
            return Ok(res);
        }

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpGet]
        [Route("GetClientAddress")]
        public async Task<IHttpActionResult> GetClientAddress([FromUri]int ClientId)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            var UserInfo = db.Clients.Where(x => x.Id == ClientId && x.IsDeleted == false).FirstOrDefault();
            if (UserInfo != null)
            {
                if (UserInfo.IsActive)
                {
                    var ClientAddress = await db.ClientAddresses.Where(x => x.ClientId == ClientId && x.IsDeleted == false).ToListAsync();
                    if (ClientAddress.Count > 0)
                    {

                        var FinalClientAddress = AutoMapper.Mapper.Map<List<ClientAddressModel>>(ClientAddress);
                        foreach (ClientAddressModel item in FinalClientAddress)
                        {
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

                            var units = await db.ClientUnits.Where(x => x.AddressId == item.Id).ToListAsync();
                            if (units.Count > 0)
                            {
                                item.AllowDelete = false;
                            }
                            else
                            {
                                //item.AllowDelete = false;
                                //if (item.IsDefaultAddress == true)
                                //{
                                //    //if (ClientAddress.Count > 1)
                                //    //{
                                //    //    item.AllowDelete = false;
                                //    //}
                                //    //else
                                //    //{
                                //        item.AllowDelete = true;
                                //    //}

                                //}
                                //else
                                //{
                                item.AllowDelete = true;
                                //}
                            }
                        }
                        res.StatusCode = (int)HttpStatusCode.OK;
                        res.Message = "Success";
                        res.Data = FinalClientAddress;
                    }
                    else
                    {
                        res.StatusCode = (int)HttpStatusCode.NotFound;
                        res.Message = "Client's address was not found";
                        res.Data = null;
                    }
                }
                else
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("AddClientAddress")]
        public async Task<IHttpActionResult> AddClientAddress([FromBody]ClientAddressModel ModelData)
        {
            db = new Aircall_DBEntities1();
            //db.Configuration.ProxyCreationEnabled = true;
            //db.Configuration.LazyLoadingEnabled = true;
            ResponseModel res = new ResponseModel();
            try
            {
                var UserInfo = db.Clients.Where(x => x.Id == ModelData.ClientId && x.IsDeleted == false).FirstOrDefault();
                if (UserInfo != null)
                {
                    if (UserInfo.IsActive)
                    {
                        //var ActiveZipCodes = db.ZipCodes.Where(x => x.Status == true && x.IsDeleted == false && x.PendingInactive == false).Select(x => x.ZipCode1).ToArray<string>();
                        var adrs = await db.ClientAddresses.Where(x => x.ClientId == ModelData.ClientId && x.IsDeleted == false).ToListAsync();
                        var city = db.Cities.Where(x => x.Id == ModelData.City).Include(x => x.State).FirstOrDefault();
                        HttpClient client = new HttpClient();
                        var dd = await client.GetAsync("https://maps.googleapis.com/maps/api/geocode/json?address=" + ModelData.Address.Replace(" ", "+") + ",+" + city.Name + ",+" + city.State.Name + "&key=" + ConfigurationManager.AppSettings["GeoCodeKey"]);
                        var data = await dd.Content.ReadAsAsync<Example>();
                        var geo = data.results.FirstOrDefault();
                        var zips = db.ZipCodes.Where(x => x.CitiesId == ModelData.City && x.StateId == ModelData.State && x.ZipCode1 == ModelData.ZipCode && x.IsDeleted == false).ToList();
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
                                var Msg = Utilities.GetSiteSettingValue("ServiceInactiveZipCodeMessage", db);
                                res.StatusCode = (int)HttpStatusCode.BadRequest;
                                res.Message = Msg;
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

                        var objClientService = Services.ServiceFactory.ClientService;
                        var customerAddressId = "";
                        string errCode = "";
                        string errText = "";
                        var ret = objClientService.AddClientAddressToAuthorizeNet(ModelData.ClientId.Value, ModelData.StateName, ModelData.CityName, ModelData.ZipCode, ModelData.Address, ref customerAddressId, ref errCode, ref errText);
                        if (!ret)
                        {
                            var ignoreDuplicate = false;
                            if (errCode == "E00039")
                            {
                                if (ModelData.IgnoreDuplicate != null && ModelData.IgnoreDuplicate.Value)
                                {
                                    ignoreDuplicate = true;
                                }
                            }
                            if (!ignoreDuplicate)
                            {
                                LogUtility.LogHelper log = new LogUtility.LogHelper();
                                log.Log("Add address to authorizenet failed. Error Code: " + errCode + " Err Text: " + errText);
                                res.StatusCode = (int)HttpStatusCode.InternalServerError;
                                res.Message = "Add address to authorizenet failed. Error Code: " + errCode + " Err Text: " + errText;
                                res.Data = null;
                                return Ok(res);
                            }
                            else
                            {
                                res.StatusCode = (int)HttpStatusCode.OK;
                                res.Message = "Client address no need to add again.";
                                res.Data = null;
                                return Ok(res);
                            }
                        }

                        ClientAddress objClientAddress = new ClientAddress
                        {
                            ClientId = ModelData.ClientId,
                            Address = ModelData.Address,
                            State = ModelData.State,
                            City = ModelData.City,
                            ZipCode = ModelData.ZipCode,
                            Latitude = (geo != null ? decimal.Parse(geo.geometry.location.lat.ToString()) : 0m),
                            Longitude = (geo != null ? decimal.Parse(geo.geometry.location.lng.ToString()) : 0m),
                            IsDefaultAddress = (adrs.Count == 0 ? true : false),
                            AddedBy = ModelData.ClientId,
                            AddedByType = (int)Utilities.UserRoles.Client,
                            AddedDate = DateTime.UtcNow,
                            IsDeleted = false,
                            CustomerAddressId = customerAddressId
                        };
                        db.ClientAddresses.Add(objClientAddress);
                        db.SaveChanges();
                        var ClientAddress = db.ClientAddresses.Where(x => x.ClientId == ModelData.ClientId && x.IsDeleted == false).Include(x => x.City1).Include(x => x.State1).ToList();

                        var FinalClientAddress = AutoMapper.Mapper.Map<List<ClientAddressModel>>(ClientAddress);
                        foreach (ClientAddressModel item in FinalClientAddress)
                        {
                            var zip = await db.ZipCodes.Where(x => x.ZipCode1 == item.ZipCode && x.StateId == item.State && x.CitiesId == item.City).FirstOrDefaultAsync();
                            var state = db.States.Where(x => x.Id == item.State).FirstOrDefault();
                            var city1 = db.Cities.Where(x => x.Id == item.City).FirstOrDefault();

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
                            else if (city1 == null)
                            {
                                item.ShowAddressInApp = false;
                            }
                            else if (city1.IsDeleted == true || city1.Status == false)
                            {
                                item.ShowAddressInApp = false;
                            }
                            else if (city1.PendingInactive == true)
                            {
                                item.ShowAddressInApp = false;
                            }
                            else
                            {
                                item.ShowAddressInApp = true;
                            }

                            var units = await db.ClientUnits.Where(x => x.AddressId == item.Id).ToListAsync();
                            if (units.Count > 0)
                            {
                                item.AllowDelete = false;
                            }
                            else
                            {
                                //item.AllowDelete = false;
                                //if (item.IsDefaultAddress == true)
                                //{
                                //    if (ClientAddress.Count > 1)
                                //    {
                                //        item.AllowDelete = false;
                                //    }
                                //    else
                                //    {
                                //        item.AllowDelete = true;
                                //    }

                                //}
                                //else
                                //{
                                item.AllowDelete = true;
                                //}
                            }
                        }
                        res.StatusCode = (int)HttpStatusCode.OK;
                        res.Message = "Client address added successfully.";
                        res.Data = FinalClientAddress;
                    }
                    else
                    {
                        res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        res.Message = "Your account was deactivated by Admin.";
                        res.Data = null;
                    }
                }
            }
            catch (Exception Ex)
            {
                LogUtility.LogHelper log = new LogUtility.LogHelper();
                log.LogException(Ex);
                res.StatusCode = (int)HttpStatusCode.InternalServerError;
                res.Message = "Invalid Request";
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("UpdateClientAddress")]
        public async Task<IHttpActionResult> UpdateClientAddress([FromBody]ClientAddressModel ModelData)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            try
            {
                var checkAddress = db.ClientAddresses.Find(ModelData.Id);

                if (checkAddress != null)
                {
                    if (ModelData.IsDefaultAddress == true)
                    {
                        var oldDefaultAddress = db.ClientAddresses.Where(x => x.ClientId == checkAddress.ClientId && x.IsDefaultAddress == true).ToList();
                        if (oldDefaultAddress.Count > 0)
                        {
                            oldDefaultAddress.ForEach(x => x.IsDefaultAddress = false);
                        }
                    }
                    City city = new City();
                    city = db.Cities.Where(x => x.Id == ModelData.City).Include(x => x.State).FirstOrDefault();

                    if (city == null)
                    {
                        city = db.Cities.Where(x => x.Id == checkAddress.City).Include(x => x.State).FirstOrDefault();
                    }

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
                        var zips = db.ZipCodes.Where(x => x.CitiesId == checkAddress.City && x.StateId == checkAddress.State && x.ZipCode1 == checkAddress.ZipCode && x.Status == true).ToList();
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
                    //checkAddress.ClientId = ModelData.ClientId;
                    checkAddress.Address = (string.IsNullOrEmpty(ModelData.Address) ? checkAddress.Address : ModelData.Address);
                    checkAddress.State = (ModelData.State == null ? checkAddress.State : ModelData.State);
                    checkAddress.City = (ModelData.City == null ? checkAddress.City : ModelData.City);
                    checkAddress.ZipCode = (string.IsNullOrEmpty(ModelData.ZipCode) ? checkAddress.ZipCode : ModelData.ZipCode);
                    try
                    {
                        HttpClient client = new HttpClient();
                        var dd = await client.GetAsync("https://maps.googleapis.com/maps/api/geocode/json?address=" + checkAddress.Address.Replace(" ", "+") + ",+" + city.Name + ",+" + city.State.Name + "&key=" + ConfigurationManager.AppSettings["GeoCodeKey"]);
                        var data = await dd.Content.ReadAsAsync<Example>();
                        var geo = data.results.FirstOrDefault();
                        checkAddress.Latitude = (geo != null ? decimal.Parse(geo.geometry.location.lat.ToString()) : 0m);
                        checkAddress.Longitude = (geo != null ? decimal.Parse(geo.geometry.location.lng.ToString()) : 0m);
                    }
                    catch (Exception ex)
                    {
                    }
                    checkAddress.IsDefaultAddress = (ModelData.IsDefaultAddress == null ? checkAddress.IsDefaultAddress : ModelData.IsDefaultAddress);
                    checkAddress.UpdatedBy = checkAddress.ClientId;
                    checkAddress.UpdatedByType = (int)Utilities.UserRoles.Client;
                    checkAddress.UpdatedDate = DateTime.UtcNow;

                    await db.SaveChangesAsync();

                    var ClientAddress = await db.ClientAddresses.Where(x => x.ClientId == checkAddress.ClientId && x.IsDeleted == false).ToListAsync();

                    var FinalClientAddress = AutoMapper.Mapper.Map<List<ClientAddressModel>>(ClientAddress);
                    foreach (ClientAddressModel item in FinalClientAddress)
                    {
                        var zip = await db.ZipCodes.Where(x => x.ZipCode1 == item.ZipCode && x.StateId == item.State && x.CitiesId == item.City).FirstOrDefaultAsync();
                        var state = db.States.Where(x => x.Id == item.State).FirstOrDefault();
                        var city1 = db.Cities.Where(x => x.Id == item.City).FirstOrDefault();

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
                        else if (city1 == null)
                        {
                            item.ShowAddressInApp = false;
                        }
                        else if (city1.IsDeleted == true || city1.Status == false)
                        {
                            item.ShowAddressInApp = false;
                        }
                        else if (city1.PendingInactive == true)
                        {
                            item.ShowAddressInApp = false;
                        }
                        else
                        {
                            item.ShowAddressInApp = true;
                        }

                        var units = await db.ClientUnits.Where(x => x.AddressId == item.Id).ToListAsync();
                        if (units.Count > 0)
                        {
                            item.AllowDelete = false;
                        }
                        else
                        {
                            //item.AllowDelete = false;
                            //if (item.IsDefaultAddress == true)
                            //{
                            //    if (ClientAddress.Count > 1)
                            //    {
                            //        item.AllowDelete = false;
                            //    }
                            //    else
                            //    {
                            //        item.AllowDelete = true;
                            //    }

                            //}
                            //else
                            //{
                            item.AllowDelete = true;
                            //}
                        }
                    }
                    res.StatusCode = (int)HttpStatusCode.OK;
                    res.Message = "Client address updated successfully.";
                    res.Data = FinalClientAddress;
                }
                else
                {
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Message = "Client address not found.";
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

        [AuthorizationRequired]
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("DeleteClientAddress")]
        public async Task<IHttpActionResult> UpdateClientAddress([FromBody]CommonRequest ModelData)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            try
            {
                var UserInfo = await db.Clients.Where(x => x.Id == ModelData.ClientId && x.IsDeleted == false).FirstOrDefaultAsync();
                if (!UserInfo.IsActive)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    var checkAddress = db.ClientAddresses.Find(ModelData.AddressId);

                    if (checkAddress != null)
                    {
                        checkAddress.IsDeleted = true;
                        checkAddress.DeletedBy = ModelData.ClientId;
                        checkAddress.DeletedByType = (int)Utilities.UserRoles.Client;
                        checkAddress.DeletedDate = DateTime.UtcNow;
                        await db.SaveChangesAsync();

                        var ClientAddress = await db.ClientAddresses.Where(x => x.ClientId == checkAddress.ClientId && x.IsDeleted == false).ToListAsync();

                        var FinalClientAddress = AutoMapper.Mapper.Map<List<ClientAddressModel>>(ClientAddress);
                        foreach (ClientAddressModel item in FinalClientAddress)
                        {
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

                            var units = await db.ClientUnits.Where(x => x.AddressId == item.Id).ToListAsync();
                            if (units.Count > 0)
                            {
                                item.AllowDelete = false;
                            }
                            else
                            {
                                //item.AllowDelete = false;
                                //if (item.IsDefaultAddress == true)
                                //{
                                //    if (ClientAddress.Count > 1)
                                //    {
                                //        item.AllowDelete = false;
                                //    }
                                //    else
                                //    {
                                //        item.AllowDelete = true;
                                //    }

                                //}
                                //else
                                //{
                                item.AllowDelete = true;
                                //}
                            }
                        }
                        res.StatusCode = (int)HttpStatusCode.OK;
                        res.Message = "Records Deleted.";
                        res.Data = FinalClientAddress;
                    }
                    else
                    {
                        res.StatusCode = (int)HttpStatusCode.NotFound;
                        res.Message = "Client address not found.";
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
        }

        [HttpPost()]
        public string UploadFiles()
        {
            int iUploadedCnt = 0;

            // DEFINE THE PATH WHERE WE WANT TO SAVE THE FILES.
            string sPath = "";
            sPath = System.Web.Hosting.HostingEnvironment.MapPath("~/locker/");

            System.Web.HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;

            // CHECK THE FILE COUNT.
            for (int iCnt = 0; iCnt <= hfc.Count - 1; iCnt++)
            {
                System.Web.HttpPostedFile hpf = hfc[iCnt];
                if (hpf.ContentLength > 0)
                {
                    // CHECK IF THE SELECTED FILE(S) ALREADY EXISTS IN FOLDER. (AVOID DUPLICATE)
                    if (!File.Exists(sPath + Path.GetFileName(hpf.FileName)))
                    {
                        // SAVE THE FILES IN THE FOLDER.
                        hpf.SaveAs(sPath + Path.GetFileName(hpf.FileName));
                        iUploadedCnt = iUploadedCnt + 1;
                    }
                }
            }

            // RETURN A MESSAGE (OPTIONAL).
            if (iUploadedCnt > 0)
            {
                return iUploadedCnt + " Files Uploaded Successfully";
            }
            else
            {
                return "Upload Failed";
            }
        }

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("ClientDashboard_old")]
        public async Task<IHttpActionResult> ClientDashboard_old([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            try
            {
                var ClientDefaultAddress = "";
                var UserInfo = await db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefaultAsync();
                if (UserInfo != null)
                {
                    if (!UserInfo.IsActive)
                    {
                        res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        res.Message = "Your account was deactivated by Admin.";
                        res.Data = null;
                    }
                    else
                    {
                        var DefaultAddressId = await db.ClientAddresses.Where(x => x.ClientId == UserInfo.Id && x.IsDefaultAddress == true && x.IsDeleted == false).FirstOrDefaultAsync();
                        if (DefaultAddressId != null)
                        {
                            request.AddressId = DefaultAddressId.Id;
                            ClientDefaultAddress = DefaultAddressId.Address;
                        }
                        else
                        {
                            request.AddressId = 0;
                        }
                        List<uspa_ClientPortal_GetNotificationForDashBoardForClient_Result> ClientNotificationsData = new List<uspa_ClientPortal_GetNotificationForDashBoardForClient_Result>();
                        var ClientUnits = db.ClientUnits.Where(x => x.ClientId == request.ClientId && x.IsDeleted == false && x.IsActive == true && x.AddressId == request.AddressId && x.IsPlanRenewedOrCancelled == false).OrderByDescending(x => x.AddedDate).AsEnumerable().Where(x => x.PaymentStatus == Utilities.UnitPaymentTypes.Received.GetEnumDescription()).Select(x => new { UnitId = x.Id, UnitName = x.UnitName, Status = ((Utilities.UnitStatus)x.Status).GetEnumDescription(), HexColor = Utilities.returncolorfromStatusHex((Utilities.UnitStatus)x.Status), R = Utilities.returncolorfromStatusRGB((Utilities.UnitStatus)x.Status, 1), G = Utilities.returncolorfromStatusRGB((Utilities.UnitStatus)x.Status, 2), B = Utilities.returncolorfromStatusRGB((Utilities.UnitStatus)x.Status, 3) }).ToList();
                        var ClientUnitsFailed = db.ClientUnits.Where(x => x.ClientId == request.ClientId && x.IsDeleted == false && x.IsActive == true).AsEnumerable().Where(x => x.PaymentStatus == Utilities.UnitPaymentTypes.PaymentFailed.GetEnumDescription() && x.AddedBy == request.ClientId && x.AddedByType == Utilities.UserRoles.Client.GetEnumValue()).ToList();
                        var ClientUnitsNotProcessed = db.ClientUnits.Where(x => x.ClientId == request.ClientId && x.IsDeleted == false && x.IsActive == true).AsEnumerable().Where(x => x.PaymentStatus == Utilities.UnitPaymentTypes.NotReceived.GetEnumDescription() && x.AddedBy == request.ClientId && x.AddedByType == Utilities.UserRoles.Client.GetEnumValue()).ToList();
                        ClientUnitsFailed.AddRange(ClientUnitsNotProcessed);
                        var ClientUnitsProcessing = db.ClientUnits.Where(x => x.ClientId == request.ClientId && x.IsDeleted == false && x.IsActive == true).AsEnumerable().Where(x => x.PaymentStatus == Utilities.UnitPaymentTypes.Processing.GetEnumDescription()).Select(x => new { UnitName = x.UnitName, Status = ((Utilities.UnitStatus)x.Status).ToString() }).ToList();
                        var ClientNotificationsCnt = db.uspa_ClientPortal_GetNotificationForDashBoardByUserType(request.ClientId, Utilities.UserRoles.Client.GetEnumValue(), "", 0).AsEnumerable().Where(x => x.Status == Utilities.NotificationStatus.UnRead.GetEnumDescription()).Count();//db.UserNotifications.AsEnumerable().Where(x => x.UserId == request.ClientId && x.DeletedBy == null && x.UserTypeId == Utilities.UserRoles.Client.GetEnumValue() && x.Status == Utilities.NotificationStatus.UnRead.GetEnumDescription()).Count();
                        ClientNotificationsData = db.uspa_ClientPortal_GetNotificationForDashBoardForClient(request.ClientId, request.AddressId).ToList();
                        var ClientNotifications = ClientNotificationsData.Where(x => x.Cnt == 1).Take(5).ToList();
                        List<NotificationListModel> NotificationDate = new List<NotificationListModel>();

                        for (int i = 0; i < ClientNotifications.Count(); i++)
                        {
                            var item = ClientNotifications[i];
                            Utilities.NotificationType MessageType = Enum.GetValues(typeof(Utilities.NotificationType)).Cast<Utilities.NotificationType>()
                                                               .FirstOrDefault(v => v.GetEnumDescription() == item.MessageType);
                            var service = db.Services.Where(x => x.Id == item.CommonId).FirstOrDefault();
                            var emp = (service != null ? service.Employee : null);
                            if (item.MessageType == Utilities.NotificationType.ServiceApproval.GetEnumDescription())
                            {
                                if (service != null)
                                {
                                    var d = new NotificationListModel()
                                    {
                                        NotificationId = item.Id,
                                        ClientId = item.UserId,
                                        NotificationType = MessageType.GetEnumValue(),
                                        Message = item.Message,
                                        CommonId = (item.CommonId.HasValue ? item.CommonId.Value : 0),
                                        ScheduleDay = (service.ScheduleDate == null ? "0" : service.ScheduleDate.Value.ToString("dd")),
                                        ScheduleMonth = (service.ScheduleDate == null ? "" : service.ScheduleDate.Value.ToString("MMMM")),
                                        ScheduleYear = (service.ScheduleDate == null ? "" : service.ScheduleDate.Value.ToString("yyyy")),
                                        ScheduleStartTime = service.ScheduleStartTime,
                                        //ScheduleEndTime = service.ScheduleEndTime,
                                        ScheduleEndTime = DateTime.Parse(service.ScheduleEndTime).AddHours(1).ToString("hh:mm tt"),
                                        ProfileImage = (emp != null ? (string.IsNullOrWhiteSpace(emp.Image) ? "" : ConfigurationManager.AppSettings["EMPProfileImageURL"].ToString() + emp.Image) : ""),
                                        Status = item.Status
                                    };
                                    NotificationDate.Add(d);
                                }
                                else
                                {
                                }
                            }
                            else
                            {
                                var d = new NotificationListModel()
                                {
                                    NotificationId = item.Id,
                                    ClientId = item.UserId,
                                    NotificationType = MessageType.GetEnumValue(),
                                    Message = item.Message,
                                    CommonId = (item.CommonId.HasValue ? item.CommonId.Value : 0),
                                    ProfileImage = (emp != null ? (string.IsNullOrWhiteSpace(emp.Image) ? "" : ConfigurationManager.AppSettings["EMPProfileImageURL"].ToString() + emp.Image) : ""),
                                    Status = item.Status
                                };
                                NotificationDate.Add(d);
                            }
                        }
                        var objClient = new
                        {
                            RoleId = (int)Utilities.UserRoles.Client,
                            Company = UserInfo.Company,
                            FirstName = UserInfo.FirstName,
                            LastName = UserInfo.LastName,
                            PhoneNumber = UserInfo.PhoneNumber,
                            DeviceType = UserInfo.DeviceType,
                            DeviceToken = UserInfo.DeviceToken,
                            AccountNumber = UserInfo.AccountNumber,
                            HasPaymentFailedUnit = (ClientUnitsFailed.Count > 0 ? true : false),
                            HasPaymentProcessingUnits = (ClientUnitsProcessing.Count > 0 ? true : false),
                            Units = ClientUnits,
                            Notifications = NotificationDate,
                            NotificationCount = ClientNotificationsCnt,
                            DefaultAddress = ClientDefaultAddress
                        };
                        res.StatusCode = (int)HttpStatusCode.OK;
                        res.Message = "Record Found.";
                        res.Data = objClient;
                    }
                }
                else
                {
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Message = "No record found";
                    res.Data = null;
                }
            }
            catch (Exception ex)
            {
                res.StatusCode = (int)HttpStatusCode.BadRequest;
                res.Message = "Invalid Request.";
                res.Data = null;
                db.Dispose();
                return Ok(ex);
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("ClientDashboard")]
        public async Task<IHttpActionResult> ClientDashboard([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            try
            {
                var ClientDefaultAddress = "";
                var UserInfo = await db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefaultAsync();
                if (UserInfo != null)
                {
                    if (!UserInfo.IsActive)
                    {
                        res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        res.Message = "Your account was deactivated by Admin.";
                        res.Data = null;
                    }
                    else
                    {
                        var DefaultAddressId = await db.ClientAddresses.Where(x => x.ClientId == UserInfo.Id && x.IsDefaultAddress == true && x.IsDeleted == false).FirstOrDefaultAsync();
                        if (DefaultAddressId != null)
                        {
                            request.AddressId = DefaultAddressId.Id;
                            ClientDefaultAddress = DefaultAddressId.Address;
                        }
                        else
                        {
                            request.AddressId = 0;
                        }
                        List<uspa_ClientPortal_GetNotificationForDashBoardForClient_Result> ClientNotificationsData = new List<uspa_ClientPortal_GetNotificationForDashBoardForClient_Result>();
                        var ClientUnits = db.ClientUnits.Where(x => x.IsSubmittedToSubscription == true && x.ClientId == request.ClientId && x.IsDeleted == false && x.IsActive == true).OrderByDescending(x => x.AddedDate).AsEnumerable().Select(x => new { UnitId = x.Id, UnitName = x.UnitName, Status = ((Utilities.UnitStatus)x.Status).GetEnumDescription(), HexColor = Utilities.returncolorfromStatusHex((Utilities.UnitStatus)x.Status), R = Utilities.returncolorfromStatusRGB((Utilities.UnitStatus)x.Status, 1), G = Utilities.returncolorfromStatusRGB((Utilities.UnitStatus)x.Status, 2), B = Utilities.returncolorfromStatusRGB((Utilities.UnitStatus)x.Status, 3) }).ToList();
                        //var ClientUnitsFailed = db.ClientUnits.Where(x => x.ClientId == request.ClientId && x.IsDeleted == false && x.IsActive == true).AsEnumerable().Where(x => x.PaymentStatus == Utilities.UnitPaymentTypes.PaymentFailed.GetEnumDescription() && x.AddedBy == request.ClientId && x.AddedByType == Utilities.UserRoles.Client.GetEnumValue()).ToList();
                        //var ClientUnitsNotProcessed = db.ClientUnits.Where(x => x.ClientId == request.ClientId && x.IsDeleted == false && x.IsActive == true).AsEnumerable().Where(x => x.PaymentStatus == Utilities.UnitPaymentTypes.NotReceived.GetEnumDescription() && x.AddedBy == request.ClientId && x.AddedByType == Utilities.UserRoles.Client.GetEnumValue()).ToList();
                        //ClientUnitsFailed.AddRange(ClientUnitsNotProcessed);
                        // ClientUnitsProcessing = db.ClientUnits.Where(x => x.ClientId == request.ClientId && x.IsDeleted == false && x.IsActive == true).AsEnumerable().Where(x => x.PaymentStatus == Utilities.UnitPaymentTypes.Processing.GetEnumDescription()).Select(x => new { UnitName = x.UnitName, Status = ((Utilities.UnitStatus)x.Status).ToString() }).ToList();
                        var ClientNotificationsCnt = db.uspa_ClientPortal_GetNotificationForDashBoardByUserType(request.ClientId, Utilities.UserRoles.Client.GetEnumValue(), "", 0).AsEnumerable().Where(x => x.Status == Utilities.NotificationStatus.UnRead.GetEnumDescription()).Count();//db.UserNotifications.AsEnumerable().Where(x => x.UserId == request.ClientId && x.DeletedBy == null && x.UserTypeId == Utilities.UserRoles.Client.GetEnumValue() && x.Status == Utilities.NotificationStatus.UnRead.GetEnumDescription()).Count();
                        ClientNotificationsData = db.uspa_ClientPortal_GetNotificationForDashBoardForClient(request.ClientId, request.AddressId).ToList();
                        var ClientNotifications = ClientNotificationsData.Where(x => x.Cnt == 1).Take(5).ToList();
                        List<NotificationListModel> NotificationDate = new List<NotificationListModel>();

                        for (int i = 0; i < ClientNotifications.Count(); i++)
                        {
                            var item = ClientNotifications[i];
                            Utilities.NotificationType MessageType = Enum.GetValues(typeof(Utilities.NotificationType)).Cast<Utilities.NotificationType>()
                                                               .FirstOrDefault(v => v.GetEnumDescription() == item.MessageType);
                            var service = db.Services.Where(x => x.Id == item.CommonId).FirstOrDefault();
                            var emp = (service != null ? service.Employee : null);
                            if (item.MessageType == Utilities.NotificationType.ServiceApproval.GetEnumDescription())
                            {
                                if (service != null)
                                {
                                    var d = new NotificationListModel()
                                    {
                                        NotificationId = item.Id,
                                        ClientId = item.UserId,
                                        NotificationType = MessageType.GetEnumValue(),
                                        Message = item.Message,
                                        CommonId = (item.CommonId.HasValue ? item.CommonId.Value : 0),
                                        ScheduleDay = (service.ScheduleDate == null ? "0" : service.ScheduleDate.Value.ToString("dd")),
                                        ScheduleMonth = (service.ScheduleDate == null ? "" : service.ScheduleDate.Value.ToString("MMMM")),
                                        ScheduleYear = (service.ScheduleDate == null ? "" : service.ScheduleDate.Value.ToString("yyyy")),
                                        ScheduleStartTime = service.ScheduleStartTime,
                                        //ScheduleEndTime = service.ScheduleEndTime,
                                        ScheduleEndTime = DateTime.Parse(service.ScheduleEndTime).AddHours(1).ToString("hh:mm tt"),
                                        ProfileImage = (emp != null ? (string.IsNullOrWhiteSpace(emp.Image) ? "" : ConfigurationManager.AppSettings["EMPProfileImageURL"].ToString() + emp.Image) : ""),
                                        Status = item.Status
                                    };
                                    NotificationDate.Add(d);
                                }
                                else
                                {
                                }
                            }
                            else
                            {
                                var d = new NotificationListModel()
                                {
                                    NotificationId = item.Id,
                                    ClientId = item.UserId,
                                    NotificationType = MessageType.GetEnumValue(),
                                    Message = item.Message,
                                    CommonId = (item.CommonId.HasValue ? item.CommonId.Value : 0),
                                    ProfileImage = (emp != null ? (string.IsNullOrWhiteSpace(emp.Image) ? "" : ConfigurationManager.AppSettings["EMPProfileImageURL"].ToString() + emp.Image) : ""),
                                    Status = item.Status
                                };
                                NotificationDate.Add(d);
                            }
                        }
                        var objClient = new
                        {
                            RoleId = (int)Utilities.UserRoles.Client,
                            Company = UserInfo.Company,
                            FirstName = UserInfo.FirstName,
                            LastName = UserInfo.LastName,
                            PhoneNumber = UserInfo.PhoneNumber,
                            DeviceType = UserInfo.DeviceType,
                            DeviceToken = UserInfo.DeviceToken,
                            AccountNumber = UserInfo.AccountNumber,
                            HasPaymentFailedUnit = false,
                            HasPaymentProcessingUnits = false,
                            Units = ClientUnits,
                            Notifications = NotificationDate,
                            NotificationCount = ClientNotificationsCnt,
                            DefaultAddress = ClientDefaultAddress
                        };
                        res.StatusCode = (int)HttpStatusCode.OK;
                        res.Message = "Record Found.";
                        res.Data = objClient;
                    }
                }
                else
                {
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Message = "No record found";
                    res.Data = null;
                }
            }
            catch (Exception ex)
            {
                res.StatusCode = (int)HttpStatusCode.BadRequest;
                res.Message = "Invalid Request.";
                res.Data = null;
                db.Dispose();
                return Ok(ex);
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseListModel))]
        [HttpPost]
        [Route("GetClientUnit_old")]
        public async Task<IHttpActionResult> GetClientUnit_old([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseListModel res = new ResponseListModel();
            List<ClientUnitResponseModel> data = new List<ClientUnitResponseModel>();
            List<ClientUnit> ClientUnits = new List<ClientUnit>();
            List<ClientUnit> result = new List<ClientUnit>();
            var UserInfo = await db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefaultAsync();
            ClientAddress address = new ClientAddress();
            int totalRecord = 0;
            int pageCnt = 0;
            int totalPageCount = 0;
            var ClientUnitsFailed = db.ClientUnits.Where(x => x.ClientId == request.ClientId && x.IsDeleted == false && x.IsActive == true).AsEnumerable().Where(x => x.PaymentStatus == Utilities.UnitPaymentTypes.PaymentFailed.GetEnumDescription() && x.AddedBy == request.ClientId && x.AddedByType == Utilities.UserRoles.Client.GetEnumValue()).ToList();
            var ClientUnitsNotProcessed = db.ClientUnits.Where(x => x.ClientId == request.ClientId && x.IsDeleted == false && x.IsActive == true).AsEnumerable().Where(x => x.PaymentStatus == Utilities.UnitPaymentTypes.NotReceived.GetEnumDescription() && x.AddedBy == request.ClientId && x.AddedByType == Utilities.UserRoles.Client.GetEnumValue()).ToList();
            ClientUnitsFailed.AddRange(ClientUnitsNotProcessed);
            var ClientUnitsProcessing = db.ClientUnits.Where(x => x.ClientId == request.ClientId && x.IsDeleted == false && x.IsActive == true && x.IsPlanRenewedOrCancelled == false).AsEnumerable().Where(x => x.PaymentStatus == Utilities.UnitPaymentTypes.Processing.GetEnumDescription()).Select(x => new { UnitName = x.UnitName, Status = ((Utilities.UnitStatus)x.Status).ToString() }).ToList();
            if (UserInfo != null)
            {
                if (!UserInfo.IsActive)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    int PeraAddressId = 0;
                    if (request.AddressId == null)
                    {
                        address = UserInfo.ClientAddresses.Where(x => x.IsDefaultAddress == true && x.IsDeleted == false).FirstOrDefault();
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
                        if (request.LastCallDateTime == null)
                        {
                            ClientUnits = db.ClientUnits.Where(x => x.ClientId == request.ClientId && x.IsDeleted == false && x.AddressId == PeraAddressId && x.IsPlanRenewedOrCancelled == false).OrderByDescending(x => x.AddedBy).AsEnumerable().Where(x => x.PaymentStatus == Utilities.UnitPaymentTypes.Received.GetEnumDescription()).ToList();
                        }
                        else
                        {
                            ClientUnits = db.ClientUnits.Where(x => x.ClientId == request.ClientId && x.IsDeleted == false && x.AddressId == PeraAddressId && x.AddedDate >= request.LastCallDateTime && x.IsPlanRenewedOrCancelled == false).OrderByDescending(x => x.AddedBy).AsEnumerable().Where(x => x.PaymentStatus == Utilities.UnitPaymentTypes.Received.GetEnumDescription()).ToList();
                        }
                        var pageSize = int.Parse(Utilities.GetSiteSettingValue("ApplicationPageSize", db));

                        if (request.PageNumber.HasValue)
                        {
                            result = CreatePagedResults<ClientUnit, ClientUnit>(ClientUnits.AsQueryable(), request.PageNumber.Value, pageSize, out totalRecord, out pageCnt, out totalPageCount).ToList();
                        }
                        else
                        {
                            result = ClientUnits;
                        }
                        foreach (var cUnit in result)
                        {
                            var services = db.Services.Where(x => x.ClientId == request.ClientId && x.ServiceUnits.Where(y => y.UnitId == cUnit.Id).Count() > 0).ToList();
                            var LastService = services.Where(x => x.Status == Utilities.ServiceTypes.Completed.GetEnumDescription() && (x.IsNoShow == false || x.IsNoShow == null) && x.ServiceReports.Count > 0).OrderByDescending(x => x.StatusChangeDate).FirstOrDefault();
                            var Upcoming = services.Where(x => x.Status == Utilities.ServiceTypes.Scheduled.GetEnumDescription()).OrderBy(x => x.StatusChangeDate).FirstOrDefault();
                            var CompletedCount = services.Where(x => x.Status == Utilities.ServiceTypes.Completed.GetEnumDescription() && (x.IsNoShow == false || x.IsNoShow == null) && x.ServiceReportUnits.Where(y => y.UnitId == cUnit.Id && y.IsCompleted == true).Count() > 0 && x.RequestedServiceBridges.Count == 0).Count();
                            try
                            {
                                var planTypeId = cUnit.PlanTypeId;
                                var planName = db.SubscriptionPlans.FirstOrDefault(p => p.Id == planTypeId).PlanName;
                                var d = new ClientUnitResponseModel()
                                {
                                    Id = cUnit.Id,
                                    UnitName = cUnit.UnitName,
                                    Status = ((Utilities.UnitStatus)cUnit.Status).GetEnumDescription(),
                                    HexColor = Utilities.returncolorfromStatusHex((Utilities.UnitStatus)cUnit.Status),
                                    PlanName = planName,
                                    LastService = (LastService != null ? LastService.ScheduleDate.Value.ToString("MM/dd/yyyy") + " " + LastService.ScheduleStartTime : "NA"),
                                    UpcomingService = (Upcoming != null ? Upcoming.ScheduleDate.Value.ToString("MM/dd/yyyy") + " " + Upcoming.ScheduleStartTime : "NA"),
                                    EmpFirstName = (Upcoming != null ? Upcoming.Employee.FirstName : "NA"),
                                    EmpLastName = (Upcoming != null ? Upcoming.Employee.LastName : "NA"),
                                    TotalService = cUnit.VisitPerYear.Value,
                                    RemainingService = cUnit.VisitPerYear.Value - CompletedCount,
                                    //UnitAge = Math.Round((cUnit.ManufactureDate == null ? 1 : Convert.ToDecimal(DateTime.UtcNow.TotalMonths(cUnit.ManufactureDate.Value)) / 12m), 2),
                                    UnitAge = (cUnit.ClientUnitParts.FirstOrDefault().ManufactureDate == null ? "1 Year" : (DateTime.UtcNow.Year - cUnit.ClientUnitParts.FirstOrDefault().ManufactureDate.Value.Year) > 0 ? DateTime.UtcNow.Year - cUnit.ClientUnitParts.FirstOrDefault().ManufactureDate.Value.Year + " Year" : DateTime.UtcNow.TotalMonths(cUnit.ClientUnitParts.FirstOrDefault().ManufactureDate.Value) + " Month"),
                                    R = Utilities.returncolorfromStatusRGB((Utilities.UnitStatus)cUnit.Status, 1),
                                    G = Utilities.returncolorfromStatusRGB((Utilities.UnitStatus)cUnit.Status, 2),
                                    B = Utilities.returncolorfromStatusRGB((Utilities.UnitStatus)cUnit.Status, 3),

                                    LastAdded = cUnit.AddedDate
                                };
                                data.Add(d);
                            }
                            catch (Exception ex)
                            {
                            }

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
                        res.LastCallDateTime = data.Last().LastAdded;
                        res.PageNumber = pageCnt;
                        res.TotalNumberOfPages = totalPageCount;
                        res.TotalNumberOfRecords = totalRecord;
                        res.HasPaymentFailedUnit = (ClientUnitsFailed.Count > 0 ? true : false);
                        res.HasPaymentProcessingUnits = (ClientUnitsProcessing.Count > 0 ? true : false);
                    }
                    else
                    {
                        res.StatusCode = (int)HttpStatusCode.NotFound;
                        res.Message = "No record found";
                        res.Data = null;
                        res.PageNumber = pageCnt - 1;
                        res.HasPaymentFailedUnit = false;
                        res.HasPaymentProcessingUnits = false;
                        res.TotalNumberOfPages = totalPageCount;
                        res.TotalNumberOfRecords = totalRecord;
                        res.HasPaymentFailedUnit = (ClientUnitsFailed.Count > 0 ? true : false);
                        res.HasPaymentProcessingUnits = (ClientUnitsProcessing.Count > 0 ? true : false);
                    }
                }
            }
            else
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "You are not authorized to view this data.";
                res.Data = null;
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseListModel))]
        [HttpPost]
        [Route("GetClientUnit")]
        public async Task<IHttpActionResult> GetClientUnit([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseListModel res = new ResponseListModel();
            var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
            var totalPageCount = 0;
            var totalRecord = 0;

            if (UserInfo != null)
            {
                if (!UserInfo.IsActive)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    DataTable dtUnits = new DataTable();
                    var objClientUnitService = Services.ServiceFactory.ClientUnitService;
                    objClientUnitService.GetClientUnitsForPortal(request.ClientId, ref dtUnits);
                    //var data = db.ClientUnits.Where(u => (u.IsDeleted == null || u.IsDeleted.Value == false) && u.ClientId == request.ClientId).ToList();
                    res.StatusCode = (int)HttpStatusCode.OK;
                    res.Message = "Record Found.";
                    res.Data = dtUnits;
                    res.LastCallDateTime = DateTime.Now;
                    res.PageNumber = 1;
                    res.TotalNumberOfPages = 1;
                    res.TotalNumberOfRecords = dtUnits.Rows.Count;
                    res.HasPaymentFailedUnit = false;
                    res.HasPaymentProcessingUnits = false;
                }
            }
            else
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "You are not authorized to view this data.";
                res.Data = null;
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
            //db.Dispose();
            return Ok(res);
        }

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("GetClientUnitDetail")]
        public async Task<IHttpActionResult> GetClientUnitDetail([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            List<ClientUnitResponseModel> data = new List<ClientUnitResponseModel>();
            List<ClientUnit> ClientUnits = new List<ClientUnit>();
            var UserInfo = await db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefaultAsync();
            ClientAddress address = new ClientAddress();
            try
            {
                if (UserInfo != null)
                {
                    if (!UserInfo.IsActive)
                    {
                        res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        res.Message = "Your account was deactivated by Admin.";
                        res.Data = null;
                    }
                    else
                    {
                        ClientUnits = db.ClientUnits.Where(x => x.ClientId == request.ClientId && x.IsDeleted == false && x.Id == request.UnitId).OrderByDescending(x => x.AddedBy).AsEnumerable().ToList();

                        foreach (var cUnit in ClientUnits)
                        {
                            var services = db.Services.Where(x => x.ClientId == request.ClientId && x.ServiceUnits.Where(y => y.UnitId == cUnit.Id).Count() > 0).ToList();
                            var LastService = services.Where(x => x.Status == Utilities.ServiceTypes.Completed.GetEnumDescription() && (x.IsNoShow == false || x.IsNoShow == null) && x.ServiceReports.Count > 0).OrderByDescending(x => x.StatusChangeDate).FirstOrDefault();
                            var Upcoming = services.Where(x => x.Status == Utilities.ServiceTypes.Scheduled.GetEnumDescription()).OrderBy(x => x.StatusChangeDate).FirstOrDefault();
                            //var CompletedCount = services.Where(x => x.Status == Utilities.ServiceTypes.Completed.GetEnumDescription() && (x.IsNoShow == false || x.IsNoShow == null) && x.RequestedServiceBridges.Count == 0).Count();
                            //var CompletedCount = services.Where(x => x.Status == Utilities.ServiceTypes.Completed.GetEnumDescription() && x.ServiceCount != 0 && (x.IsNoShow == false || x.IsNoShow == null) && x.ServiceReportUnits.Where(y => y.UnitId == cUnit.Id && y.IsCompleted == true).Count() > 0 && x.RequestedServiceBridges.Count == 0).Count();
                            var planName = db.SubscriptionPlans.FirstOrDefault(p => p.Id == cUnit.PlanTypeId).PlanName;
                            var d = new ClientUnitResponseModel()
                            {
                                Id = cUnit.Id,
                                UnitName = cUnit.UnitName,
                                Status = ((Utilities.UnitStatus)cUnit.Status).GetEnumDescription(),
                                HexColor = Utilities.returncolorfromStatusHex((Utilities.UnitStatus)cUnit.Status),
                                PlanName = planName,
                                LastService = (LastService != null ? LastService.ScheduleDate.Value.ToString("MM/dd/yyyy") + " " + LastService.ScheduleStartTime : "NA"),
                                UpcomingService = (Upcoming != null ? Upcoming.ScheduleDate.Value.ToString("MM/dd/yyyy") + " " + Upcoming.ScheduleStartTime : "NA"),
                                EmpFirstName = (Upcoming != null ? Upcoming.Employee.FirstName : "NA"),
                                EmpLastName = (Upcoming != null ? Upcoming.Employee.LastName : "NA"),
                                TotalService = cUnit.VisitPerYear.Value,
                                RemainingService = cUnit.VisitPerYear.Value,
                                UnitAge = (cUnit.ClientUnitParts.FirstOrDefault().ManufactureDate == null ? "1 Year" : (DateTime.UtcNow.Year - cUnit.ClientUnitParts.FirstOrDefault().ManufactureDate.Value.Year) > 0 ? DateTime.UtcNow.Year - cUnit.ClientUnitParts.FirstOrDefault().ManufactureDate.Value.Year + " Year" : DateTime.UtcNow.TotalMonths(cUnit.ClientUnitParts.FirstOrDefault().ManufactureDate.Value) + " Month"),
                                R = Utilities.returncolorfromStatusRGB((Utilities.UnitStatus)cUnit.Status, 1),
                                G = Utilities.returncolorfromStatusRGB((Utilities.UnitStatus)cUnit.Status, 2),
                                B = Utilities.returncolorfromStatusRGB((Utilities.UnitStatus)cUnit.Status, 3),

                                LastAdded = cUnit.AddedDate
                            };
                            data.Add(d);
                        }
                        if (data.Count > 0)
                        {
                            res.StatusCode = (int)HttpStatusCode.OK;
                            res.Message = "Record Found.";
                            res.Data = data.FirstOrDefault();
                            res.LastCallDateTime = data.FirstOrDefault().LastAdded;
                        }
                        else
                        {
                            res.StatusCode = (int)HttpStatusCode.NotFound;
                            res.Message = "No record found";
                            res.Data = null;
                        }
                    }
                }
                else
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "You are not authorized to view this data.";
                    res.Data = null;
                }
            }
            catch (Exception ex)
            {
                return Ok(ex);
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseListModel))]
        [HttpPost]
        [Route("GetPaymentFailedUnit")]
        public async Task<IHttpActionResult> GetPaymentFailedUnit([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            List<object> data = new List<object>();

            var UserInfo = await db.Clients.Where(x => x.Id == request.ClientId && x.IsActive == true && x.IsDeleted == false).FirstOrDefaultAsync();
            if (UserInfo != null)
            {
                var PaymentFailedNotifications = db.UserNotifications.AsEnumerable().Where(x => x.UserId == UserInfo.Id && x.UserTypeId == Utilities.UserRoles.Client.GetEnumValue() && x.MessageType == Utilities.NotificationType.PaymentFailed.GetEnumDescription()).ToList();
                if (PaymentFailedNotifications.Count() > 0)
                {
                    PaymentFailedNotifications.ForEach(x => x.Status = Utilities.NotificationStatus.Read.GetEnumDescription());
                    db.SaveChanges();
                }
                var PendingProcessUnit = db.ClientUnits.AsEnumerable().Where(x => x.ClientId == request.ClientId && x.PaymentStatus == Utilities.UnitPaymentTypes.PaymentFailed.GetEnumDescription() && x.AddedBy == request.ClientId && x.AddedByType == Utilities.UserRoles.Client.GetEnumValue()).ToList();
                var PendingProcessUnitNotPayment = db.ClientUnits.AsEnumerable().Where(x => x.ClientId == request.ClientId && x.PaymentStatus == Utilities.UnitPaymentTypes.NotReceived.GetEnumDescription() && x.AddedBy == request.ClientId && x.AddedByType == Utilities.UserRoles.Client.GetEnumValue()).ToList();
                PendingProcessUnit.AddRange(PendingProcessUnitNotPayment);
                decimal total = 0m;
                bool isValid = true;
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
                    var pfUnit = db.ClientUnits.Find(cunit1.Id);
                    pfUnit.PaymentStatus = Utilities.UnitPaymentTypes.NotReceived.GetEnumDescription();
                    db.SaveChanges();
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
            else
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "You are not authorized to view this data.";
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("AddClientUnit")]
        public async Task<IHttpActionResult> AddClientUnit([FromBody]ClientUnitAddModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            try
            {
                for (int i = 0; i < request.Qty; i++)
                {
                    if (!string.IsNullOrWhiteSpace(request.UnitName))
                    {
                        var unitExists = db.ClientUnits.Where(x => x.UnitName == request.UnitName && x.ClientId == request.ClientId && x.IsDeleted == false && x.IsActive == true).FirstOrDefault();
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
                        else
                        {
                            cunit.UnitTypeId = 1;
                        }
                    }
                    else
                    {
                        cunit.UnitTypeId = 1;
                    }

                    var year = (request.ManufactureDate == null ? 1m : Convert.ToDecimal(DateTime.UtcNow.TotalMonths(request.ManufactureDate.Value)) / 12m);
                    cunit.UnitName = (string.IsNullOrWhiteSpace(request.UnitName) ? generateUnitName(db, request.ClientId, 0) : (request.Qty > 1 ? request.UnitName + (i + 1).ToString() : request.UnitName));
                    //cunit.PlanId = cUnitPlan.Id;
                    cunit.IsDeleted = false;
                    cunit.AddedBy = request.ClientId;
                    cunit.AddedByType = (int)Utilities.UserRoles.Client;
                    cunit.Status = (int)Utilities.UnitStatus.ServiceSoon;
                    cunit.AddedDate = DateTime.UtcNow;
                    cunit.PaymentStatus = "NotReceived";
                    cunit.IsServiceAdded = false;
                    cunit.PricePerMonth = request.PricePerMonth;
                    cunit.VisitPerYear = request.VisitPerYear;
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

                var PendingProcessUnit = db.ClientUnits.AsEnumerable().Where(x => x.ClientId == request.ClientId && x.PaymentStatus == "NotReceived" && x.AddedBy == request.ClientId && x.AddedByType == Utilities.UserRoles.Client.GetEnumValue()).ToList();
                List<object> data = new List<object>();
                decimal total = 0m;
                bool isValid = true;
                foreach (var cunit1 in PendingProcessUnit)
                {
                    var planName = db.SubscriptionPlans.FirstOrDefault(p => p.Id == cunit1.PlanTypeId).PlanName;
                    var desc = planName;
                    var PlanSelectedDisplay = new
                    {
                        cunit1.UnitName,
                        PlanName = planName,
                        Description = desc.Substring(0, (desc.Length > 256 ? 256 : desc.Length)),
                        Price = cunit1.PricePerMonth,
                        PlanType = (cunit1.IsSpecialApplied == true ? "Special Offer" : "Recurring"),
                        Id = cunit1.Id
                    };
                    //PlanSelectedDisplay.Description = PlanSelectedDisplay.Description.Substring(0, (PlanSelectedDisplay.Description.Length > 100 ? 100 : PlanSelectedDisplay.Description.Length));
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
                LogUtility.LogHelper log = new LogUtility.LogHelper();
                log.LogException(ex);
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("RemoveUnit")]
        public async Task<IHttpActionResult> RemoveUnit([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            try
            {
                var ClientUnit = await db.ClientUnits.Where(x => x.ClientId == request.ClientId && x.Id == request.UnitId).ToListAsync();

                var notifications = db.UserNotifications.AsEnumerable().Where(x => x.UserId == request.ClientId && x.UserTypeId == Utilities.UserRoles.Client.GetEnumValue() && x.MessageType == Utilities.NotificationType.PaymentFailed.GetEnumDescription()).ToList();
                db.UserNotifications.RemoveRange(notifications);
                db.SaveChanges();

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


                    var PendingProcessUnit = db.ClientUnits.AsEnumerable().Where(x => x.ClientId == request.ClientId && (x.PaymentStatus == Utilities.UnitPaymentTypes.NotReceived.GetEnumDescription() || x.PaymentStatus == Utilities.UnitPaymentTypes.PaymentFailed.GetEnumDescription()) && x.AddedBy == request.ClientId && x.AddedByType == Utilities.UserRoles.Client.GetEnumValue()).ToList();
                    List<object> data = new List<object>();
                    decimal total = 0m;
                    bool isValid = true;
                    if (PendingProcessUnit.Count > 0)
                    {
                        foreach (var cunit1 in PendingProcessUnit)
                        {
                            var planName = db.SubscriptionPlans.FirstOrDefault(p => p.Id == cunit1.PlanTypeId).PlanName;
                            var desc = planName;
                            var PlanSelectedDisplay = new
                            {
                                cunit1.UnitName,
                                PlanName = planName,
                                Description = desc.Substring(0, (desc.Length > 256 ? 256 : desc.Length)),
                                Price = cunit1.PricePerMonth,
                                PlanType = (cunit1.IsSpecialApplied == true ? "Special Offer" : "Recurring"),
                                Id = cunit1.Id
                            };
                            //PlanSelectedDisplay.Description = PlanSelectedDisplay.Description.Substring(0, (PlanSelectedDisplay.Description.Length > 100 ? 100 : PlanSelectedDisplay.Description.Length));
                            total = total + cunit1.PricePerMonth.Value;
                            //isValid = (cunit1.Plan.Status == false || cunit1.Plan.IsVisible == false) ? false : isValid;
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
                        res.Message = "Records Deleted.";
                        res.Data = response;
                    }
                    else
                    {
                        res.StatusCode = (int)HttpStatusCode.NotFound;
                        res.Message = "Unit Not Found";
                        res.Data = null;
                    }
                }
                else
                {
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Message = "Unit Not Found";
                    res.Data = null;
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("DeleteOldData")]
        public async Task<IHttpActionResult> DeleteOldData([FromBody]ClientUnitAddModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            try
            {
                var deleteunits = await db.ClientUnits.Where(x => x.ClientId == request.ClientId && x.PaymentStatus == "NotReceived").ToListAsync();
                var deleteunitsp = await db.ClientUnits.Where(x => x.ClientId == request.ClientId && x.PaymentStatus == "Failed").ToListAsync();
                deleteunits.AddRange(deleteunitsp);
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

                db.ClientUnits.RemoveRange(deleteunits);

                var PaymentFailedNotifications = db.UserNotifications.AsEnumerable().Where(x => x.UserId == request.ClientId && x.UserTypeId == Utilities.UserRoles.Client.GetEnumValue() && x.MessageType == Utilities.NotificationType.PaymentFailed.GetEnumDescription()).ToList();
                if (PaymentFailedNotifications.Count() > 0)
                {
                    db.UserNotifications.RemoveRange(PaymentFailedNotifications);
                }

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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("MyCart")]
        public async Task<IHttpActionResult> MyCart([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            var ClientAddress = await db.ClientAddresses.Where(x => x.ClientId == request.ClientId && x.IsDeleted == false && x.IsDefaultAddress == true).ToListAsync();
            if (ClientAddress.Count > 0)
            {
                var FinalClientAddress = AutoMapper.Mapper.Map<List<ClientAddressModel>>(ClientAddress);
                foreach (ClientAddressModel item in FinalClientAddress)
                {
                    var zip = await db.ZipCodes.Where(x => x.ZipCode1 == item.ZipCode).FirstOrDefaultAsync();
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

                    var units = await db.ClientUnits.Where(x => x.AddressId == item.Id).ToListAsync();
                    if (units.Count > 0)
                    {
                        item.AllowDelete = false;
                    }
                    else
                    {
                        //item.AllowDelete = false;
                        if (item.IsDefaultAddress == true)
                        {
                            item.AllowDelete = false;
                        }
                        else
                        {
                            item.AllowDelete = true;
                        }
                    }
                }
                var AddressData = FinalClientAddress.FirstOrDefault();
                var PDFUrl = Utilities.GetSiteSettingValue("SalesAgreement", db);
                AddressData.PDFUrl = (PDFUrl == "" ? "" : ConfigurationManager.AppSettings["PolicyURL"].ToString() + PDFUrl);
                res.StatusCode = (int)HttpStatusCode.OK;
                res.Message = "Success";
                res.Data = FinalClientAddress.FirstOrDefault();
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("NewMyPayment")]
        public async Task<IHttpActionResult> NewMyPayment([FromBody]PaymentRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            try
            {
                var userInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
                var pendingProcessUnits = await db.ClientUnits.Where(x => x.ClientId == request.ClientId && (x.IsSubmittedToSubscription == null || x.IsSubmittedToSubscription == false) && x.IsDeleted == false).Include(c => c.Client).ToListAsync();

                if (pendingProcessUnits.Count < 0)
                {
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Message = "No data found.";
                    res.Data = null;
                    db.Dispose();
                    return Ok(res);
                }

                if (request.CustomerPaymentProfileId == "")
                {
                    res.StatusCode = (int)HttpStatusCode.BadRequest;
                    res.Message = "Invalid Request.";
                    res.Data = null;
                    db.Dispose();
                    return Ok(res);
                }

                var card = db.ClientPaymentMethods.Where(x => x.CustomerPaymentProfileId == request.CustomerPaymentProfileId).FirstOrDefault();

                var total = pendingProcessUnits.Sum(u => u.PricePerMonth);
                var objClientService = Services.ServiceFactory.ClientService;
                var authorizeNetSubscriptionId = "";
                var errCode = "";
                var errText = "";
                var result = objClientService.AddSubscriptionToAuthorizeNet(request.ClientId, request.CustomerPaymentProfileId, total.Value, ref authorizeNetSubscriptionId, ref errCode, ref errText);

                if (!result)
                {
                    res.StatusCode = (int)HttpStatusCode.BadRequest;
                    res.Message = "Add subscription to authorizenet failed. " + errText;
                    res.Data = null;
                    db.Dispose();
                    LogUtility.LogHelper log = new LogUtility.LogHelper();
                    log.Log("Add subscription to authorizenet failed. " + errText);
                    return Ok(res);
                }

                var objClientAddressService = Services.ServiceFactory.ClientAddressService;
                DataTable dtAddress = new DataTable();
                objClientAddressService.GetClientAddressesByClientId(request.ClientId, true, ref dtAddress);

                int addressId = Convert.ToInt32(dtAddress.Rows[0]["Id"]);
                var orderId = objClientService.AddOrder(request.ClientId, request.CustomerPaymentProfileId, addressId, total.Value, "Charge", "CC");

                UpdateOrderPricePerMonth(total.Value, orderId);

                var arr = pendingProcessUnits.Select(x => x.Id).ToArray();
                var clientUnitIds = string.Join(",", pendingProcessUnits.Select(x => x.Id.ToString()).ToArray());
                var unitId = Convert.ToInt32(arr[0]);
                BizObjects.ClientUnitSubscription objClientUnitSubscription = new BizObjects.ClientUnitSubscription();
                var objClientUnitSubscriptionService = Services.ServiceFactory.ClientUnitSubscriptionService;
                objClientUnitSubscription.ClientId = request.ClientId;
                objClientUnitSubscription.UnitId = unitId;
                objClientUnitSubscription.ClientUnitIds = string.Join(",", pendingProcessUnits.Select(x => x.Id.ToString()).ToArray());
                objClientUnitSubscription.OrderId = orderId;
                objClientUnitSubscription.PaymentMethod = "CC";
                objClientUnitSubscription.CardId = card.Id;
                objClientUnitSubscription.PONumber = string.Empty;
                objClientUnitSubscription.CheckNumbers = string.Empty;
                objClientUnitSubscription.CheckAmounts = string.Empty;
                objClientUnitSubscription.FrontImage = string.Empty;
                objClientUnitSubscription.BackImage = string.Empty;
                objClientUnitSubscription.AccountingNotes = "User pays in the mobile";
                objClientUnitSubscription.PricePerMonth = total.Value;
                objClientUnitSubscription.Amount = total.Value;
                objClientUnitSubscription.AddedDate = DateTime.UtcNow.ToLocalTime();
                objClientUnitSubscription.TotalUnits = Convert.ToInt32(arr.Length);

                var unitSubscriptionId = objClientUnitSubscriptionService.AddClientUnitSubscriptionService(ref objClientUnitSubscription, false);

                UpdateAuthorizeNetSubscriptionId(authorizeNetSubscriptionId, unitSubscriptionId);
                UpdateClientUnitSubscriptionId(request.ClientId, unitSubscriptionId, clientUnitIds);
                UpdateSubmittedFlag(request.ClientId, clientUnitIds);

                List<object> data = new List<object>();

                foreach (var cunit1 in pendingProcessUnits)
                {
                    var PlanSelectedDisplay = new
                    {
                        PlanName = db.SubscriptionPlans.FirstOrDefault(p => p.Id == cunit1.PlanTypeId).PlanName,
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
                    FirstName = userInfo.FirstName,
                    LastName = userInfo.LastName,
                    Email = userInfo.Email,
                    Total = total,
                    Units = data,
                    CardType = card.CardType
                };

                res.Message = "Order Placed";
                res.StatusCode = (int)HttpStatusCode.OK;
                res.Data = objRes;
            }
            catch (Exception ex)
            {
                res.StatusCode = (int)HttpStatusCode.BadRequest;
                res.Message = "Invalid Request. " + ex.Message;
                LogUtility.LogHelper log = new LogUtility.LogHelper();
                log.Log(res.Message);
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

        private void UpdateOrderPricePerMonth(decimal pricePerMonth, int orderId)
        {
            var sql = string.Format("update Orders set PricePerMonth = {0} where Id = {1}", pricePerMonth, orderId);
            var instance = new SQLDBHelper();
            instance.ExecuteSQL(sql, null);
            instance.Close();
        }

        private void UpdateAuthorizeNetSubscriptionId(string authorizeNetSubscriptionId, int unitSubscriptionId)
        {
            var sql = string.Format("update ClientUnitSubscription set AuthorizeNetSubscriptionId = '{0}' where Id = {1}", authorizeNetSubscriptionId, unitSubscriptionId);
            var instance = new SQLDBHelper();
            instance.ExecuteSQL(sql, null);
            instance.Close();
        }

        private void UpdateSubmittedFlag(int clientId, string unitIds)
        {
            var sql = string.Format("update dbo.ClientUnit set IsSubmittedToSubscription = '1' where ClientId = {0} and Id in ({1})", clientId, unitIds);
            var instance = new SQLDBHelper();
            instance.ExecuteSQL(sql, null);
        }

        private void UpdateClientUnitSubscriptionId(int clientId, int clientUnitSubscriptionId, string unitIds)
        {
            var sql = string.Format("update dbo.ClientUnit set ClientUnitSubscriptionId = {2} where ClientId = {0} and Id in ({1})", clientId, unitIds, clientUnitSubscriptionId);
            var instance = new SQLDBHelper();
            instance.ExecuteSQL(sql, null);
        }

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("MyPayment")]
        public async Task<IHttpActionResult> MyPayment([FromBody]PaymentRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            try
            {
                var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
                var PendingProcessUnits = await db.ClientUnits.Where(x => x.ClientId == request.ClientId && x.PaymentStatus == "NotReceived" && x.IsDeleted == false).Include(c => c.Client).ToListAsync();

                var ccDetails = db.ClientPaymentMethods.Where(x => x.ClientId == request.ClientId).ToList();
                if (request.CustomerPaymentProfileId == "")
                {
                    res.StatusCode = (int)HttpStatusCode.BadRequest;
                    res.Message = "Invalid Request.";
                    res.Data = null;
                    db.Dispose();
                    return Ok(res);
                }

                List<object> data = new List<object>();
                try
                {
                    BackgroundTaskManager.Run(async () =>
                    {
                        await NewMyPayment(request);
                    });
                }
                catch (Exception ex1)
                {
                }
                foreach (var cunit1 in PendingProcessUnits)
                {
                    var PlanSelectedDisplay = new
                    {
                        PlanName = db.SubscriptionPlans.FirstOrDefault(p => p.Id == cunit1.PlanTypeId).PlanName,
                        UnitName = cunit1.UnitName,
                        Price = cunit1.PricePerMonth,
                        PlanType = (cunit1.IsSpecialApplied == true ? "Special Offer" : "Recurring"),
                        Id = cunit1.Id,
                        ServiceCaseNumber = "",
                        Status = (cunit1.PaymentStatus.ToLower() == "notreceived" ? "Processing" : cunit1.PaymentStatus)
                    };
                    data.Add(PlanSelectedDisplay);
                }
                var objRes = new
                {
                    ClientId = request.ClientId,
                    FirstName = UserInfo.FirstName,
                    LastName = UserInfo.LastName,
                    Email = UserInfo.Email,
                    Units = data,
                    CardType = request.CardType
                };
                res.Message = "Order Processing";
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("CheckMyPaymentStatus")]
        public async Task<IHttpActionResult> CheckMyPaymentStatus([FromBody]PaymentStatusCheckRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            List<object> UnitResponse = new List<object>();
            try
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("AddCreditCard")]
        public async Task<IHttpActionResult> AddCreditCard([FromBody]ClientPaymentModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();

            if (UserInfo != null)
            {
                var dd = db.ClientPaymentMethods.Where(x => x.ClientId == request.ClientId && x.IsDefaultPayment == true).ToList();

                try
                {
                    ClientPaymentMethod cpm = new ClientPaymentMethod();
                    String s = request.CardNumber.Substring(request.CardNumber.Length - 4);
                    cpm.CardNumber = s.PadLeft(16, '*');
                    var cnt = db.ClientPaymentMethods.Where(x => x.CardNumber == cpm.CardNumber && x.ClientId == request.ClientId).Count();
                    if (cnt > 0)
                    {
                        res.StatusCode = (int)HttpStatusCode.Ambiguous;
                        res.Message = "Card Already Exists.";
                        res.Data = null;
                        db.Dispose();
                        return Ok(res);
                    }
                    if (string.IsNullOrWhiteSpace(request.CardNumber) || string.IsNullOrWhiteSpace(request.CardType) || request.ExpiryMonth == "0" || request.ExpiryYear == 0 || string.IsNullOrWhiteSpace(request.NameOnCard))
                    {
                        res.StatusCode = (int)HttpStatusCode.BadRequest;
                        res.Message = "Invalid Request.";
                        res.Data = null;
                        db.Dispose();
                        return Ok(res);
                    }
                    if (!string.IsNullOrWhiteSpace(UserInfo.CustomerProfileId))
                    {
                        try
                        {
                            var objClientService = Services.ServiceFactory.ClientService;
                            var errCode = "";
                            var errText = "";
                            string customerPaymentProfileId = "";
                            string expirationDate = request.ExpiryMonth.ToString().PadLeft(2, '0') + (request.ExpiryYear.ToString().Length > 2 ? request.ExpiryYear.ToString().Substring(2, 2) : request.ExpiryYear.ToString());
                            var ret = objClientService.CreatePaymentProfile(UserInfo.FirstName, UserInfo.LastName, UserInfo.CustomerProfileId, request.CardNumber, expirationDate, request.CVV.ToString(), ref customerPaymentProfileId, ref errCode, ref errText);

                            cpm.CustomerPaymentProfileId = customerPaymentProfileId;
                            cpm.CardType = request.CardType;
                            cpm.ClientId = request.ClientId;
                            cpm.ExpiryMonth = short.Parse(request.ExpiryMonth);
                            cpm.ExpiryYear = request.ExpiryYear;
                            cpm.AddedBy = request.ClientId;
                            cpm.NameOnCard = request.NameOnCard;
                            cpm.IsDeleted = false;

                            if (dd.Count == 0)
                            {
                                cpm.IsDefaultPayment = true;
                            }
                            else
                            {
                                cpm.IsDefaultPayment = false;
                            }
                            cpm.AddedByType = (int)Utilities.UserRoles.Client;
                            cpm.AddedDate = DateTime.UtcNow;

                            db.ClientPaymentMethods.Add(cpm);
                            db.SaveChanges();

                            res.Message = "Card Saved";
                            res.StatusCode = (int)HttpStatusCode.OK;

                            var ccs = db.ClientPaymentMethods.Where(x => x.ClientId == request.ClientId).ToList();
                            var data = AutoMapper.Mapper.Map<List<ClientPaymentModel>>(ccs);
                            res.Data = data;
                        }
                        catch (Exception ex)
                        {
                            //StripeError = true;
                            //StripeErrorLog err = Mapper.Map<StripeErrorLog>(stex.StripeError);
                            //err.Message = stex.StripeError.Message;
                            //StripeErrMsg = stex.StripeError.Message;
                            //err.Userid = request.ClientId;
                            //db.StripeErrorLogs.Add(err);
                            LogUtility.LogHelper log = new LogUtility.LogHelper();
                            log.LogException(ex);
                            res.StatusCode = (int)HttpStatusCode.BadRequest;
                            res.Message = ex.Message;
                            res.Data = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    res.StatusCode = (int)HttpStatusCode.BadRequest;
                    res.Message = "Invalid Request. " + ex.Message;
                    res.Data = null;
                }
            }
            else
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "Your account was deactivated by Admin.";
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("ValidateCreditCard")]
        public async Task<IHttpActionResult> ValidateCreditCard([FromBody]PaymentRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
            if (updatetoken)
            {
                res.Token = accessToken;
            }
            else
            {
                res.Token = "";
            }
            ClientPaymentMethod cpm1 = new ClientPaymentMethod();
            if (UserInfo != null)
            {
                //var PaymentFailedNotifications = db.UserNotifications.AsEnumerable().Where(x => x.UserId == UserInfo.Id && x.UserTypeId == Utilities.UserRoles.Client.GetEnumValue() && x.MessageType == Utilities.NotificationType.PaymentFailed.GetEnumDescription()).ToList();
                //if (PaymentFailedNotifications.Count() > 0)
                //{
                //    db.UserNotifications.RemoveRange(PaymentFailedNotifications);
                //    db.SaveChanges();
                //}
                if (request.CardNumber.Contains("*"))
                {
                    cpm1 = db.ClientPaymentMethods.Where(x => x.ClientId == request.ClientId && x.CardNumber == request.CardNumber && x.ExpiryMonth == request.ExpiryMonth && x.ExpiryYear == request.ExpiryYear).FirstOrDefault();
                }
                else
                {
                    String s1 = request.CardNumber.Substring(request.CardNumber.Length - 4);
                    s1 = s1.PadLeft(16, '*');
                    var cnt = db.ClientPaymentMethods.Where(x => x.CardNumber == s1 && x.ClientId == UserInfo.Id).Count();
                    if (cnt > 0)
                    {
                        res.StatusCode = (int)HttpStatusCode.Ambiguous;
                        res.Message = "Card Already Exists.";
                        res.Data = null;
                        db.Dispose();
                        return Ok(res);
                    }
                    cpm1 = null;
                }
                try
                {
                    if (string.IsNullOrWhiteSpace(request.CardNumber) || string.IsNullOrWhiteSpace(request.CardType) || request.ExpiryMonth == 0 || request.ExpiryYear == 0 || string.IsNullOrWhiteSpace(request.NameOnCard))
                    {
                        res.StatusCode = (int)HttpStatusCode.BadRequest;
                        res.Message = "Invalid Request.";
                        res.Data = null;
                        db.Dispose();
                        return Ok(res);
                    }

                    if (!string.IsNullOrWhiteSpace(UserInfo.CustomerProfileId))
                    {
                        try
                        {
                            ClientPaymentMethod cpm = new ClientPaymentMethod();
                            if (cpm1 != null)
                            {
                                cpm = cpm1;
                            }
                            else
                            {
                                String s = request.CardNumber.Substring(request.CardNumber.Length - 4);
                                cpm.CardNumber = s.PadLeft(16, '*');
                                var objClientService = Services.ServiceFactory.ClientService;
                                var errCode = "";
                                var errText = "";
                                string customerPaymentProfileId = "";
                                string expirationDate = request.ExpiryMonth.ToString().PadLeft(2, '0') + (request.ExpiryYear.ToString().Length > 2 ? request.ExpiryYear.ToString().Substring(2, 2) : request.ExpiryYear.ToString());
                                var ret = objClientService.CreatePaymentProfile(UserInfo.FirstName, UserInfo.LastName, UserInfo.CustomerProfileId, request.CardNumber, expirationDate, request.CVV.ToString(), ref customerPaymentProfileId, ref errCode, ref errText);

                                if (!ret)
                                {
                                    res.StatusCode = (int)HttpStatusCode.BadRequest;
                                    res.Message = "Add credit card to authorizenet failed. Error Code: " + errCode + " Error Text: " + errText;
                                    LogUtility.LogHelper log = new LogUtility.LogHelper();
                                    log.Log(res.Message);
                                    res.Data = null;
                                    db.Dispose();
                                    return Ok(res);
                                }

                                cpm.CustomerPaymentProfileId = customerPaymentProfileId;
                                cpm.CardType = request.CardType;
                                cpm.ClientId = request.ClientId;
                                cpm.ExpiryMonth = request.ExpiryMonth;
                                cpm.ExpiryYear = request.ExpiryYear;
                                cpm.AddedBy = request.ClientId;
                                cpm.NameOnCard = request.NameOnCard;
                                cpm.IsDeleted = false;

                                cpm.IsDefaultPayment = false;
                                cpm.AddedByType = (int)Utilities.UserRoles.Client;
                                cpm.AddedDate = Convert.ToDateTime(DateTime.Now.ToLocalTime().ToString("MM/dd/yyyy hh:mm:ss tt"));
                                if (UserInfo.ClientPaymentMethods.Count <= 0)
                                {
                                    cpm.IsDefaultPayment = true;
                                }
                                db.ClientPaymentMethods.Add(cpm);

                                db.SaveChanges();
                                cpm1 = cpm;
                            }
                        }
                        catch (Exception ex)
                        {
                            res.StatusCode = (int)HttpStatusCode.BadRequest;
                            res.Message = ex.Message;
                            LogUtility.LogHelper log = new LogUtility.LogHelper();
                            log.Log(ex.Message);
                            res.Data = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    res.StatusCode = (int)HttpStatusCode.BadRequest;
                    res.Message = "Invalid Request. " + ex.Message;
                    LogUtility.LogHelper log = new LogUtility.LogHelper();
                    log.Log(ex.Message);
                    res.Data = null;
                }
            }
            else
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "Your account was deactivated by Admin.";
                res.Data = null;
            }

            request.CustomerPaymentProfileId = cpm1.CustomerPaymentProfileId;

            //if (res.Data != null)
            //{
            //    BackgroundTaskManager.Run(async () =>
            //    {
            //        await NewMyPayment(request);
            //    });
            //}

            db.Dispose();
            var result = await NewMyPayment(request);
            //return Ok(res);
            return result;
        }

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("UpdateCreditCard")]
        public async Task<IHttpActionResult> UpdateCreditCard([FromBody]ClientPaymentModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            var dd = db.ClientPaymentMethods.Where(x => x.ClientId == request.ClientId && x.IsDefaultPayment == true).ToList();
            var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();

            try
            {
                if (!UserInfo.IsActive)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    ClientPaymentMethod cpm = db.ClientPaymentMethods.Where(x => x.Id == request.Id).FirstOrDefault();
                    if (cpm.IsDefaultPayment == true && request.IsDefaultPayment == false)
                    {
                        res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        res.Message = "Please select another Card as Default first.";
                        res.Data = null;
                    }
                    else
                    {
                        if (request.IsDefaultPayment.Value == true && string.IsNullOrWhiteSpace(request.CardNumber))
                        {
                            if (dd.Count > 0)
                            {
                                dd.ForEach(x => x.IsDefaultPayment = false);

                                //Code Added on 07-05-2017
                                //var GetClientUnit = db.ClientUnits.Where(s => s.ClientId == request.ClientId && s.CurrentPaymentMethod == "CC" && s.IsActive == true && s.IsDeleted == false && s.CurrentPaymentMethod != null).ToList();
                                //if(GetClientUnit.Count() > 0)
                                //{
                                //    GetClientUnit.ForEach(k => k.StripeSubscriptionId = cpm.StripeCardId);
                                //    db.SaveChanges();
                                //}
                            }
                            //StripeCustomerUpdateOptions up = new StripeCustomerUpdateOptions();
                            //up.DefaultSource = cpm.CustomerPaymentProfileId;
                            //new StripeCustomerService().Update(UserInfo.CustomerProfileId, up);
                            //cpm.IsDefaultPayment = true;
                            //cpm.UpdatedBy = request.ClientId;
                            //cpm.UpdatedByType = (int)Utilities.UserRoles.Client;
                            //cpm.UpdatedDate = DateTime.UtcNow;
                            //db.SaveChanges();
                        }
                        else
                        {
                            if (request.IsDefaultPayment.Value == true)
                            {
                                if (dd.Count > 0)
                                {
                                    dd.ForEach(x => x.IsDefaultPayment = false);
                                }
                                cpm.IsDefaultPayment = true;
                                cpm.UpdatedBy = request.ClientId;
                                cpm.UpdatedByType = (int)Utilities.UserRoles.Client;
                                cpm.UpdatedDate = DateTime.UtcNow;
                                db.SaveChanges();
                            }
                            if (request.CardNumber == "" || request.ExpiryMonth == "0" || request.ExpiryYear == 0 || request.NameOnCard == "")
                            {
                                res.StatusCode = (int)HttpStatusCode.BadRequest;
                                res.Message = "Invalid Request.";
                                res.Data = null;
                                db.Dispose();
                                return Ok(res);
                            }

                            if (!string.IsNullOrWhiteSpace(cpm.CustomerPaymentProfileId))
                            {
                                try
                                {
                                    var myCard = new StripeCardUpdateOptions();
                                    myCard.Name = request.NameOnCard;
                                    myCard.ExpirationYear = request.ExpiryYear.ToString();
                                    myCard.ExpirationMonth = request.ExpiryMonth.ToString();

                                    var cardService = new StripeCardService();
                                    StripeCard stripeCard = cardService.Update(UserInfo.CustomerProfileId, cpm.CustomerPaymentProfileId, myCard);

                                    if (!string.IsNullOrWhiteSpace(request.CardNumber))
                                    {
                                        if (!request.CardNumber.Contains("*"))
                                        {
                                            string s = request.CardNumber.Substring(request.CardNumber.Length - 4);
                                            s = s.PadLeft(16, '*');
                                            cpm.CardNumber = s;
                                        }
                                    }
                                    cpm.CardType = (stripeCard.Brand.Contains("American") ? request.CardType : stripeCard.Brand);
                                    cpm.ClientId = request.ClientId;
                                    cpm.ExpiryMonth = short.Parse(request.ExpiryMonth);
                                    cpm.ExpiryYear = request.ExpiryYear;
                                    cpm.AddedBy = request.ClientId;
                                    cpm.NameOnCard = request.NameOnCard;
                                    cpm.IsExpirationNotificationSent = false;
                                    cpm.IsDefaultPayment = (request.IsDefaultPayment == null ? false : request.IsDefaultPayment.Value);
                                    cpm.UpdatedBy = request.ClientId;
                                    cpm.UpdatedByType = (int)Utilities.UserRoles.Client;
                                    cpm.UpdatedDate = DateTime.UtcNow;
                                    db.SaveChanges();
                                }
                                catch (StripeException stex)
                                {
                                    StripeErrorLog err = Mapper.Map<StripeErrorLog>(stex.StripeError);
                                    err.Userid = request.ClientId;
                                    db.StripeErrorLogs.Add(err);
                                    res.StatusCode = (int)HttpStatusCode.BadRequest;
                                    res.Message = stex.StripeError.Error;
                                    res.Data = null;
                                }
                            }
                        }
                    }

                    var notifications = db.UserNotifications.AsEnumerable().Where(x => x.CommonId == cpm.Id && x.MessageType == Utilities.NotificationType.CreditCardExpiration.GetEnumDescription()).FirstOrDefault();
                    if (notifications != null)
                    {
                        db.UserNotifications.Remove(notifications);
                        db.SaveChanges();
                    }
                    res.Message = "Card Saved";
                    res.StatusCode = (int)HttpStatusCode.OK;
                    var ccs = db.ClientPaymentMethods.Where(x => x.ClientId == request.ClientId).ToList();
                    var data = AutoMapper.Mapper.Map<List<ClientPaymentModel>>(ccs);
                    res.Data = data;
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("GetCreditCard")]
        public async Task<IHttpActionResult> GetCreditCard([FromBody]ClientPaymentModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            try
            {
                var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
                if (!UserInfo.IsActive)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    //IsDeleted Code added on 06-07-2017
                    var ccs = db.ClientPaymentMethods.Where(x => x.ClientId == request.ClientId && (x.IsDeleted == false || x.IsDeleted == null)).ToList();
                    var data = AutoMapper.Mapper.Map<List<ClientPaymentModel>>(ccs);
                    data.ForEach(x => x.ExpiryMonth = x.ExpiryMonth.ToString().PadLeft(2, '0'));
                    if (data.Count > 0)
                    {
                        res.StatusCode = (int)HttpStatusCode.OK;
                        res.Message = "Record Found";
                        res.Data = data;
                    }
                    else
                    {
                        res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                        res.Message = "No record found";
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("GetPlanListing")]
        public async Task<IHttpActionResult> GetPlanListing([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            try
            {
                var plans = db.Plans.Where(x => x.IsDeleted == false && x.Status == true && x.IsVisible == true)
                    .Include(x => x.PlanType).OrderBy(x => x.PlanType.Order).AsEnumerable();
                var data = plans.Select(x => new
                {
                    x.PlanTypeId,
                    PlanName = x.Name,
                    x.PricePerMonth,
                    x.NumberOfService,
                    x.DurationInMonth,
                    R = x.BackGroundColorRGB.Replace("rgb(", "").Replace(")", "").Split((",").ToArray())[0].Replace(",", ""),
                    G = x.BackGroundColorRGB.Replace("rgb(", "").Replace(")", "").Split((",").ToArray())[1].Replace(",", ""),
                    B = x.BackGroundColorRGB.Replace("rgb(", "").Replace(")", "").Split((",").ToArray())[2].Replace(",", ""),
                    HGS = x.BackGroundColorHGS,
                    Image = (x.Image != "" ? ConfigurationManager.AppSettings["PlanImageURL"] + x.Image : "")
                }).Distinct().ToList();
                if (data.Count > 0)
                {
                    res.StatusCode = (int)HttpStatusCode.OK;
                    res.Message = "Record Found";
                    res.Data = data;
                }
                else
                {
                    res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                    res.Message = "No record found";
                    res.Data = null;
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("GetPlanDetail")]
        public async Task<IHttpActionResult> GetPlanDetail([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            try
            {
                var plans = db.Plans.Where(x => x.IsDeleted == false && x.Status == true && x.PlanTypeId == request.PlanTypeId && x.IsVisible == true).ToList();
                if (plans.Count > 0)
                {
                    var data = plans.Select(x => new
                    {
                        x.Id,
                        PlanName = x.Name,
                        Name = x.PackageDisplayName,
                        x.Description,
                        x.PricePerMonth,
                        x.Image
                    }).Distinct().ToList();

                    res.StatusCode = (int)HttpStatusCode.OK;
                    res.Message = "Record Found";
                    res.Data = data;
                }
                else
                {
                    res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                    res.Message = "No record found";
                    res.Data = null;
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("GetBillingHistory_old")]
        public async Task<IHttpActionResult> GetBillingHistory_old([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            List<object> parthistroy = new List<object>();
            try
            {
                var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
                if (!UserInfo.IsActive)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    var billingHistory = db.BillingHistories.AsEnumerable().Where(x => x.ClientId == request.ClientId && (x.IsDeleted == false || x.IsDeleted == null)).AsEnumerable().Select(x => new
                    {
                        x.Id,
                        PlanName = "PlanName",
                        x.PurchasedAmount,
                        TransactionDate = x.TransactionDate.Value.ToString("s") + "Z",
                        TransactionDateord = x.TransactionDate,
                        x.TransactionId,
                        UnitName = "UnitName",
                        PartName = "",
                        ServiceCaseNumber = (string.IsNullOrWhiteSpace(x.ServiceCaseNumber) ? x.Order.OrderNumber : x.ServiceCaseNumber),
                        PartsHistory = parthistroy,
                        IsPaid = (x.IsPaid.HasValue ? x.IsPaid.Value : true),
                        Reason = (string.IsNullOrWhiteSpace(x.faildesc) ? "Payment Success!" : x.faildesc),
                        ChargeBy = (string.IsNullOrEmpty(x.Order.ChargeBy) ? Utilities.PaymentMethod.CC.GetEnumDescription() : x.Order.ChargeBy),
                        CardNumber = (string.IsNullOrWhiteSpace(x.Order.ChargeBy) ? x.Order.CardNumber : (x.Order.ChargeBy == Utilities.PaymentMethod.CC.GetEnumDescription() ? x.Order.CardNumber : (x.Order.ChargeBy == Utilities.PaymentMethod.Check.GetEnumDescription() ? (string.IsNullOrWhiteSpace(x.Order.ChequeNo) ? "NA" : x.Order.ChequeNo) : (x.Order.ChargeBy == Utilities.PaymentMethod.PO.GetEnumDescription() ? (string.IsNullOrWhiteSpace(x.Order.PONo) ? "NA" : x.Order.PONo) : "NA"))))
                    }).ToList();

                    var billingHistoryPart = db.BillingHistories.AsEnumerable().Where(x => x.ClientId == request.ClientId && (x.ServiceCaseNumber == null || x.ServiceCaseNumber == "") && (x.IsDeleted == false || x.IsDeleted == null)).AsEnumerable();
                    foreach (var x in billingHistoryPart)
                    {
                        var d = new
                        {
                            x.Id,
                            PlanName = "Part Order",
                            x.PurchasedAmount,
                            TransactionDate = x.TransactionDate.Value.ToString("s") + "Z",
                            TransactionDateord = x.TransactionDate,
                            x.TransactionId,
                            UnitName = "",
                            PartName = string.Join(", ", x.Order.OrderItems.Select(y => y.PartName + " " + y.PartSize)),
                            ServiceCaseNumber = (string.IsNullOrWhiteSpace(x.ServiceCaseNumber) ? x.Order.OrderNumber : x.ServiceCaseNumber),
                            PartsHistory = parthistroy,
                            IsPaid = true,
                            Reason = (string.IsNullOrWhiteSpace(x.faildesc) ? "Payment Success!" : x.faildesc),
                            ChargeBy = (string.IsNullOrEmpty(x.Order.ChargeBy) ? Utilities.PaymentMethod.CC.GetEnumDescription() : x.Order.ChargeBy),
                            CardNumber = (string.IsNullOrWhiteSpace(x.Order.ChargeBy) ? x.Order.CardNumber : (x.Order.ChargeBy == Utilities.PaymentMethod.CC.GetEnumDescription() ? x.Order.CardNumber : (x.Order.ChargeBy == Utilities.PaymentMethod.Check.GetEnumDescription() ? (string.IsNullOrWhiteSpace(x.Order.ChequeNo) ? "NA" : x.Order.ChequeNo) : (x.Order.ChargeBy == Utilities.PaymentMethod.PO.GetEnumDescription() ? (string.IsNullOrWhiteSpace(x.Order.PONo) ? "NA" : x.Order.PONo) : "NA"))))
                        };
                        billingHistory.Add(d);
                    }

                    var billingHistoryNoShow = db.BillingHistories.AsEnumerable().Where(x => x.ClientId == request.ClientId && (x.ServiceCaseNumber != null) && (x.IsDeleted == false || x.IsDeleted == null)).AsEnumerable();
                    foreach (var x in billingHistoryNoShow)
                    {
                        var d = new
                        {
                            x.Id,
                            PlanName = "No Show",
                            x.PurchasedAmount,
                            TransactionDate = x.TransactionDate.Value.ToString("s") + "Z",
                            TransactionDateord = x.TransactionDate,
                            x.TransactionId,
                            UnitName = "",
                            PartName = "",
                            ServiceCaseNumber = x.ServiceCaseNumber,
                            PartsHistory = parthistroy,
                            IsPaid = true,
                            Reason = (string.IsNullOrWhiteSpace(x.faildesc) ? "Payment Success!" : x.faildesc),
                            ChargeBy = (string.IsNullOrEmpty(x.Order.ChargeBy) ? Utilities.PaymentMethod.CC.GetEnumDescription() : x.Order.ChargeBy),
                            CardNumber = (string.IsNullOrWhiteSpace(x.Order.ChargeBy) ? x.Order.CardNumber : (x.Order.ChargeBy == Utilities.PaymentMethod.CC.GetEnumDescription() ? x.Order.CardNumber : (x.Order.ChargeBy == Utilities.PaymentMethod.Check.GetEnumDescription() ? (string.IsNullOrWhiteSpace(x.Order.ChequeNo) ? "NA" : x.Order.ChequeNo) : (x.Order.ChargeBy == Utilities.PaymentMethod.PO.GetEnumDescription() ? (string.IsNullOrWhiteSpace(x.Order.PONo) ? "NA" : x.Order.PONo) : "NA"))))
                        };
                        billingHistory.Add(d);
                    }
                    if (billingHistory.Count > 0)
                    {
                        res.Message = "Record Found";
                        res.StatusCode = (int)HttpStatusCode.OK;
                        res.Data = billingHistory.OrderByDescending(x => x.TransactionDateord).ToList();
                    }
                    else
                    {
                        res.Message = "No record found";
                        res.StatusCode = (int)HttpStatusCode.NotFound;
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("GetBillingHistoryDetail_old")]
        public async Task<IHttpActionResult> GetBillingHistoryDetail_Old([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            List<object> parthistroy = new List<object>();
            try
            {
                var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
                if (!UserInfo.IsActive)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    var billingHistory = db.BillingHistories.AsEnumerable().Where(x => x.ClientId == request.ClientId && (x.IsDeleted == false || x.IsDeleted == null)).AsEnumerable().Select(x => new
                    BillingDetails()
                    {
                        Id = x.Id,
                        PlanName = "PlanName",
                        PurchasedAmount = x.PurchasedAmount.Value,
                        TransactionDate = x.TransactionDate.Value.ToString("s") + "Z",
                        TransactionId = (string.IsNullOrWhiteSpace(x.TransactionId) ? "" : x.TransactionId),
                        UnitName = "UnitName",
                        PartName = "",
                        ServiceCaseNumber = (string.IsNullOrWhiteSpace(x.ServiceCaseNumber) ? x.Order.OrderNumber : x.ServiceCaseNumber),
                        PartsHistory = x.Order.OrderItems.Select(item => new ordItem
                        {
                            Quantity = item.Quantity,
                            PartName = item.PartName,
                            Size = item.PartSize ?? "",
                            Amount = (item.Amount.Value * item.Quantity)
                        }).ToList(),
                        IsPaid = (x.IsPaid.HasValue ? x.IsPaid.Value : true),
                        Reason = (string.IsNullOrWhiteSpace(x.faildesc) ? "Payment Success!" : x.faildesc),
                        ChargeBy = (string.IsNullOrEmpty(x.Order.ChargeBy) ? Utilities.PaymentMethod.CC.GetEnumDescription() : x.Order.ChargeBy),
                        CardNumber = (string.IsNullOrWhiteSpace(x.Order.ChargeBy) ? x.Order.CardNumber : ((x.Order.ChargeBy == Utilities.PaymentMethod.CC.GetEnumDescription() || x.Order.ChargeBy == Utilities.ChargeBy.CCOnFile.GetEnumDescription() || x.Order.ChargeBy == Utilities.ChargeBy.NewCC.GetEnumDescription()) ? x.Order.CardNumber : (x.Order.ChargeBy == Utilities.PaymentMethod.Check.GetEnumDescription() ? (string.IsNullOrWhiteSpace(x.Order.ChequeNo) ? "NA" : x.Order.ChequeNo) : (x.Order.ChargeBy == Utilities.PaymentMethod.PO.GetEnumDescription() ? (string.IsNullOrWhiteSpace(x.Order.PONo) ? "NA" : x.Order.PONo) : "NA"))))
                    }).ToList();

                    var billingHistoryPart = db.BillingHistories.AsEnumerable().Where(x => x.ClientId == request.ClientId && (x.ServiceCaseNumber == null || x.ServiceCaseNumber == "") && (x.IsDeleted == false || x.IsDeleted == null)).AsEnumerable();
                    foreach (var x in billingHistoryPart)
                    {
                        var d = new BillingDetails()
                        {
                            Id = x.Id,
                            PlanName = "Part Order",
                            PurchasedAmount = x.PurchasedAmount.Value,
                            TransactionDate = x.TransactionDate.Value.ToString("s") + "Z",
                            TransactionId = (string.IsNullOrWhiteSpace(x.TransactionId) ? "" : x.TransactionId),
                            UnitName = "",
                            PartName = string.Join(", ", x.Order.OrderItems.Select(y => y.PartName + " " + y.PartSize)),
                            ServiceCaseNumber = (string.IsNullOrWhiteSpace(x.ServiceCaseNumber) ? x.Order.OrderNumber : x.ServiceCaseNumber),
                            PartsHistory = x.Order.OrderItems.Select(item => new ordItem
                            {
                                Quantity = item.Quantity,
                                PartName = item.PartName,
                                Size = item.PartSize ?? "",
                                Amount = (item.Amount.Value * item.Quantity)
                            }).ToList(),
                            IsPaid = true,
                            Reason = (string.IsNullOrWhiteSpace(x.faildesc) ? "Payment Success!" : x.faildesc),
                            ChargeBy = (string.IsNullOrEmpty(x.Order.ChargeBy) ? Utilities.PaymentMethod.CC.GetEnumDescription() : x.Order.ChargeBy),
                            CardNumber = (string.IsNullOrWhiteSpace(x.Order.ChargeBy) ? x.Order.CardNumber : ((x.Order.ChargeBy == Utilities.PaymentMethod.CC.GetEnumDescription() || x.Order.ChargeBy == Utilities.ChargeBy.CCOnFile.GetEnumDescription() || x.Order.ChargeBy == Utilities.ChargeBy.NewCC.GetEnumDescription()) ? x.Order.CardNumber : (x.Order.ChargeBy == Utilities.PaymentMethod.Check.GetEnumDescription() ? (string.IsNullOrWhiteSpace(x.Order.ChequeNo) ? "NA" : x.Order.ChequeNo) : (x.Order.ChargeBy == Utilities.PaymentMethod.PO.GetEnumDescription() ? (string.IsNullOrWhiteSpace(x.Order.PONo) ? "NA" : x.Order.PONo) : "NA"))))
                        };
                        billingHistory.Add(d);
                    }

                    var billingHistoryNoShow = db.BillingHistories.AsEnumerable().Where(x => x.ClientId == request.ClientId && x.ServiceCaseNumber != null && (x.IsDeleted == false || x.IsDeleted == null));
                    foreach (var x in billingHistoryNoShow)
                    {
                        var d = new BillingDetails()
                        {
                            Id = x.Id,
                            PlanName = "No Show",
                            PurchasedAmount = x.PurchasedAmount.Value,
                            TransactionDate = x.TransactionDate.Value.ToString("s") + "Z",
                            TransactionId = (string.IsNullOrWhiteSpace(x.TransactionId) ? "" : x.TransactionId),
                            UnitName = "",
                            PartName = "",
                            ServiceCaseNumber = x.ServiceCaseNumber,
                            PartsHistory = x.Order.OrderItems.Select(item => new ordItem
                            {
                                Quantity = item.Quantity,
                                PartName = item.PartName,
                                Size = item.PartSize ?? "",
                                Amount = (item.Amount.Value * item.Quantity)
                            }).ToList(),
                            IsPaid = true,
                            Reason = (string.IsNullOrWhiteSpace(x.faildesc) ? "Payment Success!" : x.faildesc),
                            ChargeBy = (string.IsNullOrEmpty(x.Order.ChargeBy) ? Utilities.PaymentMethod.CC.GetEnumDescription() : x.Order.ChargeBy),
                            CardNumber = (string.IsNullOrWhiteSpace(x.Order.ChargeBy) ? x.Order.CardNumber : ((x.Order.ChargeBy == Utilities.PaymentMethod.CC.GetEnumDescription() || x.Order.ChargeBy == Utilities.ChargeBy.CCOnFile.GetEnumDescription() || x.Order.ChargeBy == Utilities.ChargeBy.NewCC.GetEnumDescription()) ? x.Order.CardNumber : (x.Order.ChargeBy == Utilities.PaymentMethod.Check.GetEnumDescription() ? (string.IsNullOrWhiteSpace(x.Order.ChequeNo) ? "NA" : x.Order.ChequeNo) : (x.Order.ChargeBy == Utilities.PaymentMethod.PO.GetEnumDescription() ? (string.IsNullOrWhiteSpace(x.Order.PONo) ? "NA" : x.Order.PONo) : "NA"))))
                        };
                        billingHistory.Add(d);
                    }

                    if (request.BillingId != null)
                    {
                        request.ServiceCaseNumber = billingHistory.Where(x => x.Id == request.BillingId).FirstOrDefault().ServiceCaseNumber;
                        var notifications = db.UserNotifications.AsEnumerable().Where(x => x.CommonId == request.BillingId && x.UserTypeId == Utilities.UserRoles.Client.GetEnumValue() && (x.MessageType == Utilities.NotificationType.PartPurchased.GetEnumDescription() || x.MessageType == Utilities.NotificationType.SubscriptionInvoicePaymentFailed.GetEnumDescription())).ToList();
                        if (notifications.Count > 0)
                        {
                            notifications.ForEach(x => x.Status = Utilities.NotificationStatus.Read.GetEnumDescription());
                            db.SaveChanges();
                        }
                    }

                    var bh = billingHistory.Where(x => x.Id == request.BillingId).FirstOrDefault();
                    var subscription = db.ClientUnitSubscriptions.Where(y => y.InvoiceNumber == bh.TransactionId).FirstOrDefault();
                    if (subscription != null)
                    {
                        bh.ChargeBy = subscription.PaymentMethod;
                        if (subscription.PaymentMethod == Utilities.PaymentMethod.CC.GetEnumDescription())
                        {
                            var card = db.ClientPaymentMethods.Where(x => x.Id == subscription.CardId).FirstOrDefault();
                            bh.CardNumber = card.CardNumber;
                        }
                        else if (subscription.PaymentMethod == Utilities.PaymentMethod.PO.GetEnumDescription())
                        {
                            bh.CardNumber = (string.IsNullOrWhiteSpace(subscription.PONumber) ? "NA" : subscription.PONumber);
                        }
                        else if (subscription.PaymentMethod == Utilities.PaymentMethod.Check.GetEnumDescription())
                        {
                            bh.CardNumber = (string.IsNullOrWhiteSpace(subscription.CheckNumbers) ? "NA" : subscription.CheckNumbers);
                        }
                    }
                    if (bh != null)
                    {
                        res.Message = "Record Found";
                        res.StatusCode = (int)HttpStatusCode.OK;
                        res.Data = bh;
                    }
                    else
                    {
                        res.Message = "No record found";
                        res.StatusCode = (int)HttpStatusCode.NotFound;
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("GetBillingHistory")]
        public async Task<IHttpActionResult> GetBillingHistory([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            List<object> parthistroy = new List<object>();
            try
            {
                var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
                if (!UserInfo.IsActive)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    var objBillingHistoryService = Services.ServiceFactory.BillingHistoryService;
                    DataTable bh = new DataTable();
                    objBillingHistoryService.GetAllBillingHistoryByClientId(request.ClientId, ref bh);

                    if (bh != null && bh.Rows.Count > 0)
                    {
                        res.Message = "Record Found";
                        res.StatusCode = (int)HttpStatusCode.OK;
                        res.Data = bh;
                    }
                    else
                    {
                        res.Message = "No record found";
                        res.StatusCode = (int)HttpStatusCode.NotFound;
                        res.Data = null;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogHelper log = new LogUtility.LogHelper();
                log.LogException(ex);
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("GetBillingHistoryDetail")]
        public async Task<IHttpActionResult> GetBillingHistoryDetail([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            List<object> parthistroy = new List<object>();
            try
            {
                var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
                if (!UserInfo.IsActive)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    var objBillingHistoryService = Services.ServiceFactory.BillingHistoryService;
                    DataTable bh = new DataTable();
                    objBillingHistoryService.GetBillingHistoryById(request.BillingId.Value, ref bh);

                    if (bh != null && bh.Rows.Count > 0)
                    {
                        res.Message = "Record Found";
                        res.StatusCode = (int)HttpStatusCode.OK;
                        res.Data = bh;
                    }
                    else
                    {
                        res.Message = "No record found";
                        res.StatusCode = (int)HttpStatusCode.NotFound;
                        res.Data = null;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogHelper log = new LogUtility.LogHelper();
                log.LogException(ex);
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("GetUnPaidClientUnitsByClientId")]
        public async Task<IHttpActionResult> GetUnPaidClientUnitsByClientId([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            List<object> parthistroy = new List<object>();
            try
            {
                var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
                if (!UserInfo.IsActive)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    var objClientUnitService = Services.ServiceFactory.ClientUnitService;
                    DataTable dt = new DataTable();
                    objClientUnitService.GetUnPaidClientUnitsByClientId(request.ClientId, ref dt);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        res.Message = "Record Found";
                        res.StatusCode = (int)HttpStatusCode.OK;
                        res.Data = dt;
                    }
                    else
                    {
                        res.Message = "No record found";
                        res.StatusCode = (int)HttpStatusCode.NotFound;
                        res.Data = null;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.LogHelper log = new LogUtility.LogHelper();
                log.LogException(ex);
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("GetPurposeOfVisitTime")]
        public async Task<IHttpActionResult> GetPurposeOfVisitTime([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            List<object> data = new List<object>();
            var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
            if (!UserInfo.IsActive)
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

                var time = await db.PlanTypes.FirstOrDefaultAsync();
                var MaintenanceServicesWithinDays = Utilities.GetSiteSettingValue("MaintenanceServicesWithinDays", db);
                var EmergencyAndOtherServiceWithinDays = Utilities.GetSiteSettingValue("EmergencyAndOtherServiceWithinDays", db);
                var resp = new
                {
                    Purpose = data,
                    TimeSlot1 = time.ServiceSlot1,
                    TimeSlot2 = time.ServiceSlot2,
                    MaintenanceServicesWithinDays,
                    EmergencyAndOtherServiceWithinDays
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("AddRequestForService")]
        public async Task<IHttpActionResult> AddRequestForService([FromBody]ServiceRequestModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
            if (UserInfo != null)
            {
                if (!UserInfo.IsActive)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    var msg = "";

                    int Index = 1;
                    Utilities.PurposeOfVisit p = (Utilities.PurposeOfVisit)Convert.ToInt32(request.PurposeOfVisit);
                    if (request.ServiceRequestedOn.Date == DateTime.Now.Date)
                    {
                        bool IsValidTime = Utilities.CheckTimeValidation(request.ServiceRequestedTime, Index);
                        if (!IsValidTime)
                        {
                            msg = "Please select future time.";
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
                    var requestdService = db.RequestedServices.Where(x => x.ClientId == request.ClientId && x.ServiceRequestedOn == request.ServiceRequestedOn && x.IsDeleted == false).Include(x => x.RequestedServiceUnits).ToList();
                    bool alreadyadd = false;

                    foreach (var item in requestdService)
                    {
                        foreach (var it in item.RequestedServiceUnits)
                        {
                            var d = request.Units.Where(x => x == it.UnitId).ToList();
                            if (d.Count > 0)
                            {
                                alreadyadd = true;
                                msg = it.ClientUnit.UnitName + ",";
                            }
                        }
                    }
                    if (alreadyadd)
                    {
                        msg = "Some of the selected units were already requested for this date";
                        res.StatusCode = HttpStatusCode.Ambiguous.GetEnumValue();
                        res.Message = msg;
                        res.Data = null;
                    }
                    else
                    {
                        var cAddress = db.ClientAddresses.Where(x => x.ClientId == request.ClientId && x.Id == request.AddressId).FirstOrDefault();
                        var sCount = db.RequestedServices.Count(x => x.ClientId == request.ClientId) + 1;
                        var serviceNo = UserInfo.AccountNumber + "-" + cAddress.ZipCode + "-RS" + sCount;
                        Utilities.PurposeOfVisit p1 = (Utilities.PurposeOfVisit)Convert.ToInt32(request.PurposeOfVisit);
                        RequestedService rs = new RequestedService();
                        rs.ServiceCaseNumber = serviceNo;
                        rs.ClientId = request.ClientId;
                        rs.Notes = (string.IsNullOrWhiteSpace(request.Notes) ? "No notes has been added" : request.Notes);
                        rs.PurposeOfVisit = p1.GetEnumDescription();
                        rs.ServiceRequestedTime = request.ServiceRequestedTime;
                        rs.ServiceRequestedOn = request.ServiceRequestedOn;
                        rs.AddressId = request.AddressId;
                        rs.AddedBy = request.ClientId;
                        rs.AddedByType = Utilities.UserRoles.Client.GetEnumValue();
                        rs.AddedDate = DateTime.UtcNow;

                        foreach (var item in request.Units)
                        {
                            RequestedServiceUnit rsu = new RequestedServiceUnit();
                            rsu.UnitId = item;
                            rs.RequestedServiceUnits.Add(rsu);
                        }

                        db.RequestedServices.Add(rs);
                        db.SaveChanges();
                        db.uspa_ClientUnitServiceCount_UpdateByRequestedServiceId(rs.Id);

                        if (rs.PurposeOfVisit == Utilities.PurposeOfVisit.Emergency.GetEnumDescription())
                        {
                            var EmergencyService = db.uspa_EmergencyRequestedServiceSchedule(rs.Id).ToList();

                            if (EmergencyService.Count > 0)
                            {
                                var es = EmergencyService.FirstOrDefault();
                                if (es.EmployeeId > 0)
                                {
                                    //Employee Notification

                                    var service = db.Services.Where(x => x.Id == es.ServiceId).FirstOrDefault();

                                    var EmpNotification = db.NotificationMasters.Where(x => x.Name == "EmployeeSchedule").FirstOrDefault();
                                    var message = EmpNotification.Message;
                                    message = message.Replace("{{ScheduleDate}}", service.ScheduleDate.Value.ToString("MMMM dd, yyyy"));

                                    UserNotification objUserNotification = new UserNotification();
                                    objUserNotification.UserId = service.EmployeeId;
                                    objUserNotification.UserTypeId = Utilities.UserRoles.Employee.GetEnumValue();
                                    objUserNotification.Message = message;
                                    objUserNotification.Status = Utilities.NotificationStatus.UnRead.GetEnumDescription();
                                    objUserNotification.CommonId = es.ServiceId;
                                    objUserNotification.MessageType = Utilities.NotificationType.ServiceScheduled.GetEnumDescription();
                                    objUserNotification.AddedDate = DateTime.UtcNow;
                                    db.UserNotifications.Add(objUserNotification);
                                    db.SaveChanges();

                                    var BadgeCount = db.UserNotifications.AsEnumerable().Where(x => x.UserId == service.EmployeeId && x.UserTypeId == Utilities.UserRoles.Employee.GetEnumValue() && x.Status == Utilities.NotificationStatus.UnRead.GetEnumDescription()).ToList().Count;

                                    Notifications objNotifications = new Notifications { NId = objUserNotification.Id, NType = Utilities.NotificationType.ServiceScheduled.GetEnumValue(), CommonId = es.ServiceId.Value };
                                    List<NotificationModel> notify = new List<NotificationModel>();
                                    notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                                    notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                                    notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });
                                    var EmpInfo = db.Employees.Where(x => x.Id == service.EmployeeId).FirstOrDefault();
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
                                    //Client Notification

                                    var service = db.Services.Find(es.ServiceId);
                                    var ClientNotification = db.NotificationMasters.Where(x => x.Name == "RequestedServiceScheduleSendToClient").FirstOrDefault();
                                    var message = ClientNotification.Message;
                                    message = message.Replace("{{ScheduleDate}}", service.ScheduleDate.Value.ToString("MMMM dd, yyyy"));
                                    UserNotification objUserNotification = new UserNotification();
                                    objUserNotification.UserId = service.ClientId;
                                    objUserNotification.UserTypeId = Utilities.UserRoles.Client.GetEnumValue();
                                    objUserNotification.Message = message;
                                    objUserNotification.Status = Utilities.NotificationStatus.UnRead.GetEnumDescription();
                                    objUserNotification.CommonId = service.Id;
                                    objUserNotification.MessageType = Utilities.NotificationType.ServiceScheduled.GetEnumDescription();
                                    objUserNotification.AddedDate = DateTime.UtcNow;
                                    db.UserNotifications.Add(objUserNotification);
                                    db.SaveChanges();

                                    var BadgeCount = db.uspa_ClientPortal_GetNotificationForDashBoardByUserType(service.ClientId, Utilities.UserRoles.Client.GetEnumValue(), "", 0).AsEnumerable().Where(x => x.Status == Utilities.NotificationStatus.UnRead.GetEnumDescription()).Count();
                                    Notifications objNotifications = new Notifications { NId = objUserNotification.Id, NType = Utilities.NotificationType.ServiceScheduled.GetEnumValue(), CommonId = service.Id };
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

                                    EmailTemplate templateClient = db.EmailTemplates.Where(x => x.Name == "RequestedServiceScheduleClient" && x.Status == true).FirstOrDefault();
                                    var strclient = templateClient.EmailBody;
                                    var strSubject = templateClient.EmailTemplateSubject.Replace("{{PurposeOfVisit}}", Utilities.PurposeOfVisit.Emergency.GetEnumDescription());

                                    strclient = strclient.Replace("{{PurposeOfVisit}}", Utilities.PurposeOfVisit.Emergency.GetEnumDescription());
                                    strclient = strclient.Replace("{{ScheduleDate}}", service.ScheduleDate.Value.ToString("MMMM dd, yyyy"));
                                    strclient = strclient.Replace("{{ScheduleTime}}", service.ScheduleStartTime + " - " + service.ScheduleEndTime);
                                    strclient = strclient.Replace("{{ServiceCaseNumber}}", service.ServiceCaseNumber);
                                    strclient = strclient.Replace("{{Address}}", service.ClientAddress.Address + ", " + service.ClientAddress.City1.Name + ", " + service.ClientAddress.State1.Name + ", " + service.ClientAddress.ZipCode + " ");
                                    strclient = strclient.Replace("{{Technician}}", service.ScheduleDate.Value.ToString("MMMM dd, yyyy"));

                                    Utilities.Send(strSubject, UserInfo.Email, strclient, templateClient.FromEmail, db);

                                }
                            }
                        }

                        res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                        res.Message = "Record Added";
                        res.Data = null;
                    }
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("EditRequestForService")]
        public async Task<IHttpActionResult> EditRequestForService([FromBody]ServiceRequestModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            var requestdService1 = db.RequestedServices.Where(x => x.ClientId == request.ClientId && x.Id == request.ServiceId).FirstOrDefault();
            try
            {
                int Index = 1;
                Utilities.PurposeOfVisit p = (Utilities.PurposeOfVisit)Convert.ToInt32(request.PurposeOfVisit);
                if (p.GetEnumDescription() == Utilities.PurposeOfVisit.Emergency.GetEnumDescription())
                {
                    Index = 0;
                }
                if (request.ServiceRequestedOn.Date == DateTime.Now.Date)
                {
                    bool IsValidTime = Utilities.CheckTimeValidation(request.ServiceRequestedTime, Index);
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
                var notification = db.UserNotifications.AsEnumerable().Where(x => x.CommonId == requestdService1.ServiceId && x.MessageType == Utilities.NotificationType.ServiceApproval.GetEnumDescription()).ToList();
                if (notification.Count > 0)
                {
                    db.UserNotifications.RemoveRange(notification);
                    db.SaveChanges();
                }
                var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
                if (!UserInfo.IsActive)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    var ambiguous = false;
                    if (requestdService1.ServiceRequestedOn == request.ServiceRequestedOn)
                    {
                        if (requestdService1.ServiceRequestedTime == request.ServiceRequestedTime)
                        {
                            ambiguous = true;
                        }
                    }

                    if (ambiguous)
                    {
                        res.StatusCode = HttpStatusCode.Ambiguous.GetEnumValue();
                        res.Message = "You can not reschedule service for same date and time.";
                        res.Data = null;
                    }
                    else
                    {
                        if (requestdService1 != null)
                        {
                            requestdService1.ServiceRequestedTime = request.ServiceRequestedTime;
                            requestdService1.ServiceRequestedOn = request.ServiceRequestedOn;
                            requestdService1.UpdatedBy = request.ClientId;
                            requestdService1.UpdatedByType = Utilities.UserRoles.Client.GetEnumValue();
                            requestdService1.UpdatedDate = DateTime.UtcNow;

                            if (requestdService1.RequestedServiceBridges.Count > 0)
                            {
                                foreach (var item in requestdService1.RequestedServiceBridges)
                                {
                                    db.Services.Remove(item.Service);
                                    db.RequestedServiceBridges.Remove(item);

                                    if (item.Service.RescheduleServices.Count > 0)
                                    {
                                        db.RescheduleServices.RemoveRange(item.Service.RescheduleServices.ToList());
                                        db.SaveChanges();
                                    }
                                }
                            }
                            if (requestdService1.Service != null)
                            {
                                requestdService1.Service.Status = Utilities.ServiceTypes.Pending.GetEnumDescription();
                                requestdService1.Service.UpdatedBy = request.ClientId;
                                requestdService1.Service.UpdatedByType = Utilities.UserRoles.Client.GetEnumValue();
                                requestdService1.Service.UpdatedDate = DateTime.UtcNow;
                            }
                            if (requestdService1.ServiceId.HasValue)
                            {
                                RescheduleService RS = new RescheduleService();
                                RS.ServiceId = requestdService1.ServiceId.Value;
                                RS.Rescheduletime = request.ServiceRequestedTime;
                                RS.RescheduleDate = request.ServiceRequestedOn;
                                RS.Reason = request.Reason;
                                RS.AddedBy = request.ClientId;
                                RS.AddedByType = Utilities.UserRoles.Client.GetEnumValue();
                                RS.AddedDate = DateTime.UtcNow;
                                db.RescheduleServices.Add(RS);
                            }
                            db.SaveChanges();
                            res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                            res.Message = "Record Updated";
                            res.Data = null;
                            var requestdServices = db.RequestedServices.Where(x => x.ClientId == request.ClientId && x.IsDeleted == false).Include(x => x.Service).Include(x => x.RequestedServiceUnits).ToList();

                            List<object> addresses = new List<object>();

                            if (requestdServices.Count > 0)
                            {
                                var Addresses = requestdServices.Select(x => new { x.ClientAddress.Id, x.ClientAddress.Address }).Distinct().ToList();

                                foreach (var Address in Addresses)
                                {
                                    List<object> data = new List<object>();
                                    foreach (var requestdService in requestdServices.Where(x => x.AddressId == Address.Id).OrderByDescending(x => x.ServiceRequestedOn).ToList())
                                    {
                                        bool AllowDelete = false;
                                        if (requestdService.RequestedServiceBridges.Count == 0)
                                        {
                                            AllowDelete = true;
                                        }
                                        else
                                        {
                                            AllowDelete = false;
                                        }
                                        if (requestdService.RequestedServiceBridges.Count == 0)
                                        {
                                            var d = new
                                            {
                                                requestdService.Id,
                                                requestdService.ServiceCaseNumber,
                                                Message = "Requested service on " + requestdService.ServiceRequestedOn.ToString("MM/dd/yyyy") + " at " + requestdService.ServiceRequestedTime.Split(("-").ToArray()).First().Trim() + " For " + (requestdService.PurposeOfVisit),
                                                AllowDelete = AllowDelete,
                                                requestdService.ServiceRequestedTime,
                                                ServiceRequestedOn = requestdService.ServiceRequestedOn.ToString("s") + "Z",
                                                UnitCount = requestdService.RequestedServiceUnits.Count,
                                                PurposeOfVisit = DurationExtensions.GetValueFromDescription<Utilities.PurposeOfVisit>(requestdService.PurposeOfVisit).GetEnumValue(),
                                                requestdService.ClientAddress.Address
                                            };
                                            data.Add(d);
                                        }
                                        else
                                        {
                                            var Completed = requestdService.RequestedServiceBridges.AsEnumerable().Where(x => x.Service.Status == Utilities.ServiceTypes.Completed.GetEnumDescription()).Count();
                                            var Cancelled = requestdService.RequestedServiceBridges.AsEnumerable().Where(x => x.Service.Status == Utilities.ServiceTypes.Cancelled.GetEnumDescription()).Count();
                                            if (requestdService.RequestedServiceBridges.Count >= (Completed + Cancelled))
                                            {
                                                var d = new
                                                {
                                                    requestdService.Id,
                                                    requestdService.ServiceCaseNumber,
                                                    Message = "Requested service on " + requestdService.ServiceRequestedOn.ToString("MM/dd/yyyy") + " at " + requestdService.ServiceRequestedTime.Split(("-").ToArray()).First().Trim() + " For " + (requestdService.PurposeOfVisit),
                                                    AllowDelete = AllowDelete,
                                                    requestdService.ServiceRequestedTime,
                                                    ServiceRequestedOn = requestdService.ServiceRequestedOn.ToString("s") + "Z",
                                                    UnitCount = requestdService.RequestedServiceUnits.Count,
                                                    PurposeOfVisit = DurationExtensions.GetValueFromDescription<Utilities.PurposeOfVisit>(requestdService.PurposeOfVisit).GetEnumValue(),
                                                    requestdService.ClientAddress.Address
                                                };
                                                data.Add(d);
                                            }
                                        }
                                    }
                                    var add = new
                                    {
                                        Address.Address,
                                        Services = data
                                    };
                                    addresses.Add(add);
                                }

                                res.Message = "Record Found";
                                res.StatusCode = (int)HttpStatusCode.OK;
                                res.Data = addresses;
                            }
                        }
                        else
                        {
                            res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                            res.Message = "Record Not Found";
                            res.Data = null;
                        }
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("RequestForServiceList")]
        public async Task<IHttpActionResult> RequestForServiceList([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            List<object> addresses = new List<object>();

            var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
            if (!UserInfo.IsActive)
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "Your account was deactivated by Admin.";
                res.Data = null;
            }
            else
            {
                var requestdServices = db.RequestedServices.Where(x => x.ClientId == request.ClientId && x.IsDeleted == false).Include(x => x.Service).Include(x => x.RequestedServiceUnits).ToList();
                if (requestdServices.Count > 0)
                {
                    var Addresses = requestdServices.Select(x => new { x.ClientAddress.Id, x.ClientAddress.Address }).Distinct().ToList();

                    foreach (var Address in Addresses)
                    {
                        List<object> data = new List<object>();
                        foreach (var requestdService in requestdServices.Where(x => x.AddressId == Address.Id).OrderByDescending(x => x.ServiceRequestedOn).ToList())
                        {
                            bool AllowDelete = false;
                            if (requestdService.RequestedServiceBridges.Count == 0)
                            {
                                AllowDelete = true;
                            }
                            else
                            {
                                AllowDelete = false;
                            }
                            if (requestdService.RequestedServiceBridges.Count == 0)
                            {
                                var d = new
                                {
                                    requestdService.Id,
                                    requestdService.ServiceCaseNumber,
                                    Message = "Requested service on " + requestdService.ServiceRequestedOn.ToString("MM/dd/yyyy") + " at " + requestdService.ServiceRequestedTime.Split(("-").ToArray()).First().Trim() + " For " + (requestdService.PurposeOfVisit),
                                    AllowDelete = AllowDelete,
                                    requestdService.ServiceRequestedTime,
                                    ServiceRequestedOn = requestdService.ServiceRequestedOn.ToString("s") + "Z",
                                    UnitCount = requestdService.RequestedServiceUnits.Count,
                                    PurposeOfVisit = DurationExtensions.GetValueFromDescription<Utilities.PurposeOfVisit>(requestdService.PurposeOfVisit).GetEnumValue(),
                                    requestdService.ClientAddress.Address
                                };
                                data.Add(d);
                            }
                            else
                            {
                                var Completed = requestdService.RequestedServiceBridges.AsEnumerable().Where(x => x.Service.Status == Utilities.ServiceTypes.Completed.GetEnumDescription()).Count();
                                var Cancelled = requestdService.RequestedServiceBridges.AsEnumerable().Where(x => x.Service.Status == Utilities.ServiceTypes.Cancelled.GetEnumDescription()).Count();
                                if (requestdService.RequestedServiceBridges.Count >= (Completed + Cancelled))
                                {
                                    var d = new
                                    {
                                        requestdService.Id,
                                        requestdService.ServiceCaseNumber,
                                        Message = "Requested service on " + requestdService.ServiceRequestedOn.ToString("MM/dd/yyyy") + " at " + requestdService.ServiceRequestedTime.Split(("-").ToArray()).First().Trim() + " For " + (requestdService.PurposeOfVisit),
                                        AllowDelete = AllowDelete,
                                        requestdService.ServiceRequestedTime,
                                        ServiceRequestedOn = requestdService.ServiceRequestedOn.ToString("s") + "Z",
                                        UnitCount = requestdService.RequestedServiceUnits.Count,
                                        PurposeOfVisit = DurationExtensions.GetValueFromDescription<Utilities.PurposeOfVisit>(requestdService.PurposeOfVisit).GetEnumValue(),
                                        requestdService.ClientAddress.Address
                                    };
                                    data.Add(d);
                                }
                            }
                        }
                        var add = new
                        {
                            Address.Address,
                            Services = data
                        };
                        addresses.Add(add);
                    }

                    res.Message = "Record Found";
                    res.StatusCode = (int)HttpStatusCode.OK;
                    res.Data = addresses;
                }
                else
                {
                    res.Message = "No record found";
                    res.StatusCode = (int)HttpStatusCode.NotFound;
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("RequestForServiceDetail")]
        public async Task<IHttpActionResult> RequestForServiceDetail([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
            if (!UserInfo.IsActive)
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "Your account was deactivated by Admin.";
                res.Data = null;
            }
            else
            {
                var requestdServices = db.RequestedServices.Where(x => x.ClientId == request.ClientId && x.Id == request.ServiceId && x.IsDeleted == false).Include(x => x.RequestedServiceUnits).ToList();
                if (requestdServices.Count > 0)
                {
                    var requestdService = requestdServices.First();
                    var unitid = requestdService.RequestedServiceUnits.Select(x => x.UnitId).ToArray();
                    var sUnits = db.ClientUnits.Where(x => unitid.Contains(x.Id)).Select(x => new { x.Id, x.UnitName }).ToList();
                    var address = AutoMapper.Mapper.Map<ClientAddressModel>(requestdService.ClientAddress);
                    var d = new
                    {
                        requestdService.Id,
                        PurposeOfVisit = (requestdService.PurposeOfVisit),
                        requestdService.ServiceCaseNumber,
                        ServiceRequestedOn = requestdService.ServiceRequestedOn.ToString("MM/dd/yyyy"),
                        ServiceRequestedTime = (requestdService.PurposeOfVisit == Utilities.PurposeOfVisit.Emergency.GetEnumDescription() ? requestdService.ServiceRequestedTime.Split(("-").ToArray()).First() : requestdService.ServiceRequestedTime),
                        Notes = string.IsNullOrWhiteSpace(requestdService.Notes) ? "No notes has been added" : requestdService.Notes,
                        Address = address,
                        Units = sUnits
                    };
                    res.Message = "Record Found";
                    res.StatusCode = (int)HttpStatusCode.OK;
                    res.Data = d;
                }
                else
                {
                    res.Message = "No record found";
                    res.StatusCode = (int)HttpStatusCode.NotFound;
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("WaitingServiceDetail")]
        public async Task<IHttpActionResult> WaitingServiceDetail([FromBody]NotificationRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
            if (!UserInfo.IsActive)
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "Your account was deactivated by Admin.";
                res.Data = null;
            }
            else
            {
                var Services = db.Services.Where(x => x.ClientId == request.ClientId && x.Id == request.ServiceId).ToList();
                if (Services.Count > 0)
                {
                    var Service = Services.First();
                    var unitid = Service.ServiceUnits.Select(x => x.UnitId).ToArray();
                    var sUnits = db.ClientUnits.Where(x => unitid.Contains(x.Id)).Select(x => new { x.Id, x.UnitName }).ToList();

                    var LateRescheduleHours = int.Parse(Utilities.GetSiteSettingValue("LateRescheduleHours", db));
                    var diff24Hourdate = DateTime.Parse(Service.ScheduleDate.Value.ToString("MM/dd/yyyy") + " " + Service.ScheduleStartTime) - DateTime.Now;
                    var LateRescheduleDisplayMessage = Utilities.GetSiteSettingValue("LateRescheduleDisplayMessage", db);
                    var LateCancelDisplayMessage = Utilities.GetSiteSettingValue("LateCancelDisplayMessage", db);
                    var address = AutoMapper.Mapper.Map<ClientAddressModel>(Service.ClientAddress);

                    var d = new
                    {
                        Service.Id,
                        PurposeOfVisit = DurationExtensions.GetValueFromDescription<Utilities.PurposeOfVisit>(Service.PurposeOfVisit).GetEnumValue(),
                        Service.ServiceCaseNumber,
                        ScheduleYear = Service.ScheduleDate.Value.Year,
                        ScheduleDate = Service.ScheduleDate.Value.ToString("MMMM dd, yyyy"),
                        ScheduleMonth = Service.ScheduleDate.Value.ToString("MM"),
                        MonthName = Service.ScheduleDate.Value.ToString("MMMM"),
                        ScheduleDay = Service.ScheduleDate.Value.Day,
                        Service.ScheduleStartTime,
                        ScheduleEndTime = DateTime.Parse(Service.ScheduleEndTime).AddHours(1).ToString("hh:mm tt"),
                        CustomerComplaints = (Service.CustomerComplaints == null ? (Service.RequestedServices.Count > 0 ? (string.IsNullOrWhiteSpace(Service.RequestedServices.FirstOrDefault().Notes) ? "" : Service.RequestedServices.FirstOrDefault().Notes) : "") : Service.CustomerComplaints),
                        EMPFirstName = Service.Employee.FirstName,
                        EMPLastName = Service.Employee.LastName,
                        Units = sUnits,
                        EmpProfileImage = (string.IsNullOrWhiteSpace(Service.Employee.Image) ? "" : ConfigurationManager.AppSettings["EMPProfileImageURL"].ToString() + Service.Employee.Image),
                        ServiceRequestedTime = (Service.RequestedServices.Count > 0 ? Service.RequestedServices.FirstOrDefault().ServiceRequestedTime : ""),
                        Is24HourLeft = diff24Hourdate.TotalHours <= LateRescheduleHours,
                        diff24Hourdate.TotalHours,
                        LateRescheduleDisplayMessage = diff24Hourdate.TotalHours <= LateRescheduleHours ? LateRescheduleDisplayMessage : "",
                        LateCancelDisplayMessage = diff24Hourdate.TotalHours <= LateRescheduleHours ? LateCancelDisplayMessage : "",
                        Address = address,
                        IsRequested = (Service.RequestedServiceBridges.Count > 0)
                    };
                    var notificaiton = db.UserNotifications.Where(x => x.Id == request.NotificationId).FirstOrDefault();
                    if (notificaiton != null)
                    {
                        notificaiton.Status = Utilities.NotificationStatus.Read.GetEnumDescription();
                        notificaiton.UpdatedBy = request.ClientId;
                        notificaiton.UpdatedDate = DateTime.UtcNow;
                        notificaiton.UserTypeId = Utilities.UserRoles.Client.GetEnumValue();
                        db.SaveChanges();
                    }
                    if (Service.Status == Utilities.ServiceTypes.Scheduled.GetEnumDescription())
                    {
                        res.Message = "Service Already Approved";
                        res.StatusCode = (int)HttpStatusCode.OK;
                        res.Data = d;
                    }
                    else
                    {
                        res.Message = "Record Found";
                        res.StatusCode = (int)HttpStatusCode.OK;
                        res.Data = d;
                    }
                }
                else
                {
                    res.Message = "No record found";
                    res.StatusCode = (int)HttpStatusCode.NotFound;
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("ApproveWaitingService")]
        public async Task<IHttpActionResult> ApproveWaitingService([FromBody]NotificationRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
            if (!UserInfo.IsActive)
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "Your account was deactivated by Admin.";
                res.Data = null;
            }
            else
            {
                var n = db.UserNotifications.AsEnumerable().Where(x => x.CommonId == request.ServiceId && x.MessageType == Utilities.NotificationType.ServiceApproval.GetEnumDescription()).ToList();
                db.UserNotifications.RemoveRange(n);
                db.SaveChanges();
                var service = db.Services.Where(x => x.Id == request.ServiceId).FirstOrDefault();
                service.Status = Utilities.ServiceTypes.Scheduled.GetEnumDescription();
                foreach (var item in service.ServiceUnits)
                {
                    item.ClientUnit.Status = Utilities.UnitStatus.ServiceSoon.GetEnumValue();
                }
                service.ApprovalEmailUrl = null;
                service.UrlExpireDate = null;
                service.StatusChangeDate = DateTime.UtcNow;
                service.UpdatedBy = request.ClientId;
                service.UpdatedByType = Utilities.UserRoles.Client.GetEnumValue();
                service.UpdatedDate = DateTime.UtcNow;
                var EmpNotification = db.NotificationMasters.Where(x => x.Name == "EmployeeSchedule").FirstOrDefault();
                var message = EmpNotification.Message;
                message = message.Replace("{{ScheduleDate}}", service.ScheduleDate.Value.ToString("MMMM dd, yyyy"));

                UserNotification objUserNotification = new UserNotification();
                objUserNotification.UserId = service.EmployeeId;
                objUserNotification.UserTypeId = Utilities.UserRoles.Employee.GetEnumValue();
                objUserNotification.Message = message;
                objUserNotification.Status = Utilities.NotificationStatus.UnRead.GetEnumDescription();
                objUserNotification.CommonId = request.ServiceId;
                objUserNotification.MessageType = Utilities.NotificationType.ServiceScheduled.GetEnumDescription();
                objUserNotification.AddedDate = DateTime.UtcNow;
                db.UserNotifications.Add(objUserNotification);
                db.SaveChanges();

                var BadgeCount = db.UserNotifications.AsEnumerable().Where(x => x.UserId == service.EmployeeId && x.UserTypeId == Utilities.UserRoles.Employee.GetEnumValue() && x.Status == Utilities.NotificationStatus.UnRead.GetEnumDescription()).ToList().Count;

                Notifications objNotifications = new Notifications { NId = objUserNotification.Id, NType = Utilities.NotificationType.ServiceScheduled.GetEnumValue(), CommonId = request.ServiceId };
                List<NotificationModel> notify = new List<NotificationModel>();
                notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });
                var EmpInfo = db.Employees.Where(x => x.Id == service.EmployeeId).FirstOrDefault();
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
                res.StatusCode = (int)HttpStatusCode.OK;
                res.Message = "Service Request Approved.";
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("DeleteRequestedService")]
        public async Task<IHttpActionResult> DeleteRequestedService([FromBody]ServiceRequestModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            //List<object> data = new List<object>();
            List<object> addresses = new List<object>();

            var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
            if (!UserInfo.IsActive)
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "Your account was deactivated by Admin.";
                res.Data = null;
            }
            else
            {
                var requestdServices = db.RequestedServices.Where(x => x.ClientId == request.ClientId && x.Id == request.ServiceId && x.IsDeleted == false).Include(x => x.RequestedServiceUnits).ToList();
                if (requestdServices.Count > 0)
                {
                    try
                    {
                        foreach (var item in requestdServices)
                        {
                            var notification = db.UserNotifications.AsEnumerable().Where(x => x.CommonId == item.ServiceId && x.MessageType == Utilities.NotificationType.ServiceApproval.GetEnumDescription()).ToList();
                            if (notification.Count > 0)
                            {
                                db.UserNotifications.RemoveRange(notification);
                                db.SaveChanges();
                            }
                            item.IsDeleted = true;
                            item.DeletedBy = request.ClientId;

                            item.DeletedByType = Utilities.UserRoles.Client.GetEnumValue();
                            item.DeletedDate = DateTime.UtcNow;

                            if (item.Service != null)
                            {
                                item.Service.Status = Utilities.ServiceTypes.Deleted.GetEnumDescription();
                                item.Service.StatusChangeDate = DateTime.UtcNow;
                                item.Service.UpdatedBy = request.ClientId;
                                item.Service.UpdatedByType = Utilities.UserRoles.Client.GetEnumValue();
                                item.Service.UpdatedDate = DateTime.UtcNow;
                                item.Notes = "deleted by client";
                            }
                        }

                        db.SaveChanges();

                        var requestdServices1 = db.RequestedServices.Where(x => x.ClientId == request.ClientId && x.IsDeleted == false).Include(x => x.Service).Include(x => x.RequestedServiceUnits).ToList();

                        if (requestdServices1.Count > 0)
                        {
                            var Addresses = requestdServices1.Select(x => new { x.ClientAddress.Id, x.ClientAddress.Address }).Distinct().ToList();

                            foreach (var Address in Addresses)
                            {
                                List<object> data = new List<object>();
                                foreach (var requestdService in requestdServices1.Where(x => x.AddressId == Address.Id).OrderByDescending(x => x.ServiceRequestedOn).ToList())
                                {
                                    bool AllowDelete = false;
                                    if (requestdService.RequestedServiceBridges.Count == 0)
                                    {
                                        AllowDelete = true;
                                    }
                                    else
                                    {
                                        AllowDelete = false;
                                    }
                                    if (requestdService.RequestedServiceBridges.Count == 0)
                                    {
                                        var d = new
                                        {
                                            requestdService.Id,
                                            requestdService.ServiceCaseNumber,
                                            Message = "Requested service on " + requestdService.ServiceRequestedOn.ToString("MM/dd/yyyy") + " at " + requestdService.ServiceRequestedTime.Split(("-").ToArray()).First().Trim() + " For " + (requestdService.PurposeOfVisit),
                                            AllowDelete = AllowDelete,
                                            requestdService.ServiceRequestedTime,
                                            ServiceRequestedOn = requestdService.ServiceRequestedOn.ToString("s") + "Z",
                                            UnitCount = requestdService.RequestedServiceUnits.Count,
                                            PurposeOfVisit = DurationExtensions.GetValueFromDescription<Utilities.PurposeOfVisit>(requestdService.PurposeOfVisit).GetEnumValue(),
                                            requestdService.ClientAddress.Address
                                        };
                                        data.Add(d);
                                    }
                                    else
                                    {
                                        var Completed = requestdService.RequestedServiceBridges.AsEnumerable().Where(x => x.Service.Status == Utilities.ServiceTypes.Completed.GetEnumDescription()).Count();
                                        var Cancelled = requestdService.RequestedServiceBridges.AsEnumerable().Where(x => x.Service.Status == Utilities.ServiceTypes.Cancelled.GetEnumDescription()).Count();
                                        if (requestdService.RequestedServiceBridges.Count >= (Completed + Cancelled))
                                        {
                                            var d = new
                                            {
                                                requestdService.Id,
                                                requestdService.ServiceCaseNumber,
                                                Message = "Requested service on " + requestdService.ServiceRequestedOn.ToString("MM/dd/yyyy") + " at " + requestdService.ServiceRequestedTime.Split(("-").ToArray()).First().Trim() + " For " + (requestdService.PurposeOfVisit),
                                                AllowDelete = AllowDelete,
                                                requestdService.ServiceRequestedTime,
                                                ServiceRequestedOn = requestdService.ServiceRequestedOn.ToString("s") + "Z",
                                                UnitCount = requestdService.RequestedServiceUnits.Count,
                                                PurposeOfVisit = DurationExtensions.GetValueFromDescription<Utilities.PurposeOfVisit>(requestdService.PurposeOfVisit).GetEnumValue(),
                                                requestdService.ClientAddress.Address
                                            };
                                            data.Add(d);
                                        }
                                    }
                                }
                                var add = new
                                {
                                    Address.Address,
                                    Services = data
                                };
                                addresses.Add(add);
                            }

                            res.Message = "Record Deleted";
                            res.StatusCode = (int)HttpStatusCode.OK;
                            res.Data = addresses;
                        }
                        else
                        {
                            res.Message = "Record Deleted";
                            res.StatusCode = (int)HttpStatusCode.NotFound;
                            res.Data = addresses;
                        }
                    }
                    catch (Exception ex)
                    {
                        res.Message = "Internal Server Error";
                        res.StatusCode = (int)HttpStatusCode.InternalServerError;
                        res.Data = addresses;
                    }
                }
                else
                {
                    res.Message = "No record found";
                    res.StatusCode = (int)HttpStatusCode.NotFound;
                    res.Data = addresses;
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseListModel))]
        [HttpPost]
        [Route("PastServiceListing")]
        public async Task<IHttpActionResult> PastServiceListing([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseListModel res = new ResponseListModel();
            List<PastServiceResponseModel> ClientServicesData = new List<PastServiceResponseModel>();
            List<Service> ClientServices = new List<Service>();
            List<Service> result = new List<Service>();
            int totalRecord = 0;
            int pageCnt = 0;
            int totalPageCount = 0;

            try
            {
                var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
                if (UserInfo != null)
                {
                    if (!UserInfo.IsActive)
                    {
                        res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        res.Message = "Your account was deactivated by Admin.";
                        res.Data = null;
                    }
                    else
                    {
                        if (request.LastCallDateTime == null)
                        {
                            ClientServices = UserInfo.Services.AsEnumerable().Where(x => x.Status == Utilities.ServiceTypes.Completed.GetEnumDescription() && x.ServiceReports.Count > 0).OrderByDescending(x => x.StatusChangeDate).ToList();
                            var ClientServices1 = UserInfo.Services.AsEnumerable().Where(x => x.Status == Utilities.ServiceTypes.NoShow.GetEnumDescription()).OrderByDescending(x => x.StatusChangeDate).ToList();
                            ClientServices.AddRange(ClientServices1);


                        }
                        else
                        {
                            ClientServices = UserInfo.Services.AsEnumerable().Where(x => x.Status == Utilities.ServiceTypes.Completed.GetEnumDescription() && x.ServiceReports.Count > 0 && x.AddedDate >= request.LastCallDateTime).OrderByDescending(x => x.StatusChangeDate).ToList();
                            var ClientServices1 = UserInfo.Services.AsEnumerable().Where(x => x.Status == Utilities.ServiceTypes.NoShow.GetEnumDescription()).OrderByDescending(x => x.StatusChangeDate).ToList();
                            ClientServices.AddRange(ClientServices1);
                        }
                        var pageSize = int.Parse(Utilities.GetSiteSettingValue("ApplicationPageSize", db));
                        if (request.PageNumber.HasValue)
                        {
                            result = CreatePagedResults<Service, Service>(ClientServices.AsQueryable(), request.PageNumber.Value, pageSize, out totalRecord, out pageCnt, out totalPageCount).ToList();
                        }
                        else
                        {
                            result = ClientServices;
                        }
                        if (result.Count > 0)
                        {
                            foreach (var ClientService in result)
                            {
                                var ServicePlan = ClientService.ServiceReportUnits.AsEnumerable().Select(x => new
                                {
                                    Name = db.SubscriptionPlans.FirstOrDefault(p => p.Id == x.ClientUnit.PlanTypeId).PlanName,
                                    x.ClientUnit.UnitName
                                });
                                var isnoshow = (ClientService.IsNoShow.HasValue ? ClientService.IsNoShow.Value : false);
                                isnoshow = (ClientService.ServiceReports.Count <= 0 ? true : isnoshow);
                                var cs = new PastServiceResponseModel()
                                {
                                    Id = ClientService.Id,
                                    PlanName = (ServicePlan.FirstOrDefault() != null ? ServicePlan.FirstOrDefault().Name : ""),
                                    UnitName = (ServicePlan.FirstOrDefault() != null ? ServicePlan.FirstOrDefault().UnitName : ""),
                                    ServiceCaseNumber = ClientService.ServiceCaseNumber,
                                    Message = "Service performed on " + ClientService.ScheduleDate.Value.ToString("MM/dd/yyyy") + " at " + ClientService.ScheduleStartTime.ToString() + " by " + ClientService.Employee.FirstName + " " + (string.IsNullOrWhiteSpace(ClientService.Employee.LastName) ? "" : ClientService.Employee.LastName),
                                    LastAdded = ClientService.AddedDate,
                                    IsNoShow = isnoshow,
                                    Address = ClientService.ClientAddress.Address
                                };
                                ClientServicesData.Add(cs);
                            }
                            res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                            res.Message = "Record found";
                            res.PageNumber = pageCnt;
                            res.TotalNumberOfPages = totalPageCount;
                            res.TotalNumberOfRecords = totalRecord;
                        }
                        else
                        {
                            res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                            res.Message = "No record found";
                            res.PageNumber = pageCnt - 1;
                            res.TotalNumberOfPages = totalPageCount;
                            res.TotalNumberOfRecords = totalRecord;
                        }
                    }
                }
                else
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "You are not authorized to view this data.";
                    res.Data = null;
                    res.PageNumber = (request.PageNumber.HasValue ? request.PageNumber.Value : 1);
                    res.TotalNumberOfPages = totalPageCount;
                    res.TotalNumberOfRecords = totalRecord;
                }
                res.Data = ClientServicesData;
                res.LastCallDateTime = (ClientServicesData.Count > 0 ? ClientServicesData.Last().LastAdded : null);
            }
            catch (Exception ex)
            {
                res.Message = ex.Message;
                res.StatusCode = (int)HttpStatusCode.InternalServerError;
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("PastServiceListingDetail")]
        public async Task<IHttpActionResult> PastServiceListingDetail([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            List<object> ClientServicesData = new List<object>();
            try
            {
                var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();

                if (UserInfo != null)
                {
                    if (!UserInfo.IsActive)
                    {
                        res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        res.Message = "Your account was deactivated by Admin.";
                        res.Data = null;
                    }
                    else
                    {
                        var ClientServices = UserInfo.Services.AsEnumerable().Where(x => (x.Status == Utilities.ServiceTypes.Completed.GetEnumDescription() || x.Status == Utilities.ServiceTypes.NoShow.GetEnumDescription()) && x.Id == request.ServiceId).ToList();
                        if (ClientServices.Count > 0)
                        {
                            var ClientService = ClientServices.FirstOrDefault();

                            var ClientUnit = ClientService.ServiceReportUnits.Where(x => x.IsCompleted == true).AsEnumerable().Select(x => new
                            {
                                x.ClientUnit.UnitName,
                                PlanName = db.SubscriptionPlans.FirstOrDefault(p => p.Id == x.ClientUnit.PlanTypeId).PlanName
                            }).ToList();
                            var serviceReport = ClientService.ServiceReports.FirstOrDefault();
                            var Rating = ClientService.ServiceRatingReviews.FirstOrDefault();

                            string msg = "Service performed on " + ClientService.ScheduleDate.Value.ToString("MM/dd/yyyy") + " at " + ClientService.ScheduleStartTime.ToString() + " by " + ClientService.Employee.FirstName + " " + (string.IsNullOrWhiteSpace(ClientService.Employee.LastName) ? "" : ClientService.Employee.LastName);
                            var isnoshow = (ClientService.IsNoShow.HasValue ? ClientService.IsNoShow.Value : false);
                            isnoshow = (ClientService.ServiceReports.Count <= 0 ? true : isnoshow);
                            if ((ClientService.Status == Utilities.ServiceTypes.Completed.GetEnumDescription() || ClientService.Status == Utilities.ServiceTypes.NoShow.GetEnumDescription()) && isnoshow == true)
                            {
                                msg = "Service Attempted on " + ClientService.ScheduleDate.Value.ToString("MM/dd/yyyy") + " at " + ClientService.ScheduleStartTime.ToString() + " by " + ClientService.Employee.FirstName + " " + (string.IsNullOrWhiteSpace(ClientService.Employee.LastName) ? "" : ClientService.Employee.LastName);
                                var noshowunit = ClientService.ServiceUnits.Select(x => new
                                {
                                    x.ClientUnit.UnitName,
                                    PlanName = "PlanName"
                                }).ToList();

                                ClientUnit.AddRange(noshowunit);
                            }

                            var isdifferenttime = true;
                            string WorkStartedTime = (serviceReport == null ? "" : serviceReport.WorkStartedTime);
                            var WorkCompletedTime = (serviceReport == null ? "" : serviceReport.WorkCompletedTime);
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
                                        dateTime3 = DateTime.ParseExact(WorkStartedTime, "h:mm tt", CultureInfo.InvariantCulture);
                                    }

                                    DateTime dateTime4 = new DateTime();
                                    try
                                    {
                                        dateTime4 = DateTime.ParseExact(WorkCompletedTime, "hh:mm tt", CultureInfo.InvariantCulture);
                                    }
                                    catch (Exception ex)
                                    {
                                        dateTime4 = DateTime.ParseExact(WorkCompletedTime, "h:mm tt", CultureInfo.InvariantCulture);
                                    }

                                    TimeSpan time2 = dateTime4.Subtract(dateTime3);

                                    if (isdifferenttime)
                                    {
                                        ExtraTime = time2.TotalMinutes - time1.TotalMinutes;
                                    }
                                }
                            }
                            var address = AutoMapper.Mapper.Map<ClientAddressModel>(ClientService.ClientAddress);
                            var cs = new
                            {
                                ClientService.Id,
                                Units = ClientUnit,
                                ClientService.ServiceCaseNumber,
                                ScheduleDate = ClientService.ScheduleDate.Value.ToString("MM/dd/yyyy"),
                                ClientService.Employee.FirstName,
                                ClientService.Employee.LastName,
                                ClientService.ScheduleStartTime,
                                ClientService.ScheduleEndTime,
                                AssignedTotalTime = AssignedTotalTime.ToString() + " Min",
                                WorkStartedTime = (serviceReport == null ? "" : serviceReport.WorkStartedTime),
                                WorkCompletedTime = (serviceReport == null ? "" : serviceReport.WorkCompletedTime),
                                WorkPerformed = (serviceReport == null ? "" : serviceReport.WorkPerformed),
                                IsWorkDone = (serviceReport == null ? false : (serviceReport.IsWorkDone == null ? false : serviceReport.IsWorkDone)),
                                IsNoShow = isnoshow,
                                Recommendations = (serviceReport == null ? "" : serviceReport.Recommendationsforcustomer),
                                EmpProfileImage = (string.IsNullOrWhiteSpace(ClientService.Employee.Image) ? "" : ConfigurationManager.AppSettings["EMPProfileImageURL"].ToString() + ClientService.Employee.Image),
                                PurposeOfVisit = (ClientService.PurposeOfVisit),
                                Rate = (Rating == null ? 0 : Rating.Rate),
                                Review = (Rating == null ? "" : Rating.Review),
                                Message = msg,
                                IsDifferentTime = isdifferenttime,
                                ExtraTime = (ExtraTime > 0 ? ExtraTime + " Min" : ""),
                                Address = address
                            };
                            res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                            res.Message = "Record found";
                            res.Data = cs;
                        }
                        else
                        {
                            res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                            res.Message = "No Data Found";
                        }
                        if (request.NotificationId != null)
                        {
                            var notificaiton = db.UserNotifications.Where(x => x.Id == request.NotificationId).FirstOrDefault();
                            if (notificaiton != null)
                            {
                                notificaiton.Status = Utilities.NotificationStatus.Read.GetEnumDescription();
                                notificaiton.UpdatedBy = request.ClientId;
                                notificaiton.UpdatedDate = DateTime.UtcNow;
                                notificaiton.UserTypeId = Utilities.UserRoles.Client.GetEnumValue();
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res.StatusCode = HttpStatusCode.InternalServerError.GetEnumValue();
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("ServiceRating")]
        public async Task<IHttpActionResult> ServiceRating([FromBody]ServiceRatingModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            try
            {
                var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
                if (UserInfo != null)
                {
                    if (!UserInfo.IsActive)
                    {
                        res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                        res.Message = "Your account was deactivated by Admin.";
                        res.Data = null;
                    }
                    else
                    {
                        var ClientServices = UserInfo.Services.AsEnumerable().Where(x => x.Status == Utilities.ServiceTypes.Completed.GetEnumDescription() && x.Id == request.ServiceId).ToList();
                        if (ClientServices.Count > 0)
                        {
                            var ClientService = ClientServices.FirstOrDefault();

                            var notification = db.UserNotifications.AsEnumerable().Where(x => x.CommonId == ClientService.Id && x.MessageType == Utilities.NotificationType.RateService.GetEnumDescription()).ToList();
                            if (notification.Count > 0)
                            {
                                db.UserNotifications.RemoveRange(notification);
                                db.SaveChanges();
                            }
                            ClientService.ServiceRatingReviews.Add(new ServiceRatingReview() { Rate = request.Rate, Review = (string.IsNullOrWhiteSpace(request.Review) ? "No Review Added" : request.Review), ReviewDate = DateTime.UtcNow, EmployeeId = ClientService.EmployeeId });
                            db.SaveChanges();

                            res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                            res.Message = "Record Saved";

                            var EmpNotification = db.NotificationMasters.Where(x => x.Name == "ServiceRatingSendToEmployee").FirstOrDefault();
                            var message = EmpNotification.Message;
                            message = message.Replace("{{ClientName}}", UserInfo.FirstName + " " + UserInfo.LastName);
                            UserNotification objUserNotification = new UserNotification();
                            objUserNotification.UserId = ClientService.EmployeeId;
                            objUserNotification.UserTypeId = Utilities.UserRoles.Employee.GetEnumValue();
                            objUserNotification.Message = message;
                            objUserNotification.Status = Utilities.NotificationStatus.UnRead.GetEnumDescription();
                            objUserNotification.CommonId = request.ServiceId;
                            objUserNotification.MessageType = Utilities.NotificationType.RateService.GetEnumDescription();
                            objUserNotification.AddedDate = DateTime.UtcNow;
                            db.UserNotifications.Add(objUserNotification);
                            db.SaveChanges();

                            var BadgeCount = db.UserNotifications.AsEnumerable().Where(x => x.UserId == ClientService.EmployeeId && x.UserTypeId == Utilities.UserRoles.Employee.GetEnumValue()).ToList().Count;

                            Notifications objNotifications = new Notifications { NId = objUserNotification.Id, NType = Utilities.NotificationType.RateService.GetEnumValue(), CommonId = request.ServiceId };
                            List<NotificationModel> notify = new List<NotificationModel>();
                            notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                            notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                            notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });
                            var EmpInfo = db.Employees.Where(x => x.Id == ClientService.EmployeeId).FirstOrDefault();
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
                }
            }
            catch (Exception ex)
            {
                res.StatusCode = HttpStatusCode.InternalServerError.GetEnumValue();
                res.Message = "Internal Server Error";
                throw;
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("UpcomingServices")]
        public async Task<IHttpActionResult> UpcomingServices([FromBody]CommonRequest request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();
            List<object> data = new List<object>();
            List<object> Units = new List<object>();
            var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
            if (!UserInfo.IsActive)
            {
                res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                res.Message = "Your account was deactivated by Admin.";
                res.Data = null;
            }
            else
            {
                var UpcomingServices = db.Services.Where(x => x.ClientId == request.ClientId).AsEnumerable().Where(x => x.Status == Utilities.ServiceTypes.Scheduled.GetEnumDescription() && x.ScheduleDate >= DateTime.UtcNow.Date).OrderBy(x => x.ScheduleDate).ToList();
                var FutureServices = db.Services.Where(x => x.ClientId == request.ClientId).AsEnumerable().Where(x => x.Status == Utilities.ServiceTypes.Pending.GetEnumDescription() && x.PurposeOfVisit == Utilities.PurposeOfVisit.Maintenance.GetEnumDescription() && x.RequestedServiceBridges.Count == 0).OrderBy(x => x.ExpectedStartDate).ToList();
                UpcomingServices.AddRange(FutureServices);
                if (UpcomingServices.Count > 0)
                {
                    foreach (var us in UpcomingServices)
                    {
                        var ServiceUnits = us.ServiceUnits.Select(x => new
                        {
                            x.ClientUnit.Id,
                            x.ClientUnit.UnitName
                        }).ToList();

                        var d = new
                        {
                            Id = us.Id,
                            ScheduleYear = (us.ScheduleDate.HasValue ? us.ScheduleDate.Value.Year : 0),
                            ScheduleMonth = (us.ScheduleDate.HasValue ? us.ScheduleDate.Value.Month : 0),
                            MonthName = (us.ScheduleDate.HasValue ? us.ScheduleDate.Value.ToString("MMMM") : ""),
                            ScheduleDay = (us.ScheduleDate.HasValue ? us.ScheduleDate.Value.Day : 0),
                            ScheduleStartTime = (string.IsNullOrWhiteSpace(us.ScheduleStartTime) ? "" : us.ScheduleStartTime),
                            ScheduleEndTime = (string.IsNullOrWhiteSpace(us.ScheduleEndTime) ? "" : DateTime.Parse(us.ScheduleEndTime).AddHours(1).ToString("hh:mm tt")),
                            PurposeOfVisit = DurationExtensions.GetValueFromDescription<Utilities.PurposeOfVisit>(us.PurposeOfVisit).GetEnumValue(),
                            EmpProfileImage = (us.EmployeeId.HasValue ? (string.IsNullOrWhiteSpace(us.Employee.Image) ? "" : ConfigurationManager.AppSettings["EMPProfileImageURL"].ToString() + us.Employee.Image) : ""),
                            EMPFirstName = (us.EmployeeId.HasValue ? us.Employee.FirstName : ""),
                            EMPLastName = (us.EmployeeId.HasValue ? us.Employee.LastName : ""),
                            ScheduleDate = (us.Status == Utilities.ServiceTypes.Scheduled.GetEnumDescription() ? us.ScheduleDate.HasValue ? us.ScheduleDate.Value.ToString("MM/dd/yyyy dddd") : "" : us.ExpectedStartDate.Value.ToString("MM/dd/yyyy") + " - " + us.ExpectedEndDate.Value.ToString("MM/dd/yyyy")),
                            ServiceTime = (string.IsNullOrWhiteSpace(us.ScheduleStartTime) ? "" : us.ScheduleStartTime + " - " + us.ScheduleEndTime),
                            ServiceCaseNumber = us.ServiceCaseNumber,
                            Units = ServiceUnits,
                            Appoinment = us.ServiceCount.Value.Ordinal() + " Appointment",
                            CustomerComplaints = (string.IsNullOrWhiteSpace(us.CustomerComplaints) ? "" : us.CustomerComplaints),
                            EmpName = (us.Status == Utilities.ServiceTypes.Scheduled.GetEnumDescription() ? ("At " + us.ClientAddress.Address + ", " + (us.PurposeOfVisit) + " will be provided by Technician " + us.Employee.FirstName + " " + (string.IsNullOrWhiteSpace(us.Employee.LastName) ? "" : us.Employee.LastName)) : "At " + us.ClientAddress.Address + ", " + (us.PurposeOfVisit) + " will be scheduled 30 days before this date range period. Notification will be sent with option to reschedule."),
                            us.Status,
                            us.ClientAddress.Address,
                            IsRequested = (us.RequestedServiceBridges.Count > 0)
                        };
                        data.Add(d);
                    }
                    if (data.Count > 0)
                    {
                        res.Message = "Record Found";
                        res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                        res.Data = data;
                    }
                    else
                    {
                        res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                        res.Message = "Not Record Found";
                        res.Data = null;
                    }
                }
                else
                {
                    res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                    res.Message = "Record Not Found";
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

        [AuthorizationRequired]
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
                var UserInfo = db.Clients.Where(x => x.Id == request.ClientId && x.IsDeleted == false).FirstOrDefault();
                if (!UserInfo.IsActive)
                {
                    res.StatusCode = (int)HttpStatusCode.NotAcceptable;
                    res.Message = "Your account was deactivated by Admin.";
                    res.Data = null;
                }
                else
                {
                    if (request.ServiceRequestedOn.Date == DateTime.Now.Date && !request.IsLateReschedule)
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
                    var notification = db.UserNotifications.AsEnumerable().Where(x => x.CommonId == request.ServiceId && x.MessageType == Utilities.NotificationType.ServiceApproval.GetEnumDescription()).ToList();
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

                    var notification2 = db.UserNotifications.AsEnumerable().Where(x => x.CommonId == request.ServiceId && x.MessageType == Utilities.NotificationType.ServiceScheduled.GetEnumDescription()).ToList();
                    if (notification2.Count > 0)
                    {
                        db.UserNotifications.RemoveRange(notification2);
                        db.SaveChanges();
                    }
                    var ClientService = db.Services.Where(x => x.ClientId == request.ClientId && x.Id == request.ServiceId).FirstOrDefault();
                    if (ClientService != null)
                    {
                        var isSchedueled = false;
                        if (request.IsCancelled)
                        {
                            ClientService.Status = Utilities.ServiceTypes.Cancelled.GetEnumDescription();
                        }
                        else
                        {
                            if (ClientService.Status == Utilities.ServiceTypes.Scheduled.GetEnumDescription())
                            {
                                isSchedueled = true;
                            }
                            ClientService.Status = Utilities.ServiceTypes.Rescheduled.GetEnumDescription();
                        }

                        if (isSchedueled)
                        {
                            var EmpNotification = db.NotificationMasters.Where(x => x.Name == "RescheduleServiceSendToEmployee").FirstOrDefault();
                            var message = EmpNotification.Message;
                            message = message.Replace("{{ClientName}}", ClientService.Client.FirstName + " " + ClientService.Client.LastName);
                            message = message.Replace("{{ScheduleDate}}", ClientService.ScheduleDate.Value.ToString("MMMM dd, yyyy"));
                            UserNotification objUserNotification = new UserNotification();
                            objUserNotification.UserId = ClientService.EmployeeId;
                            objUserNotification.UserTypeId = Utilities.UserRoles.Employee.GetEnumValue();
                            objUserNotification.Message = message;
                            objUserNotification.Status = Utilities.NotificationStatus.UnRead.GetEnumDescription();
                            objUserNotification.CommonId = ClientService.Id;
                            objUserNotification.MessageType = Utilities.NotificationType.FriendlyReminder.GetEnumDescription();
                            objUserNotification.AddedDate = DateTime.Now;
                            db.UserNotifications.Add(objUserNotification);
                            db.SaveChanges();

                            var BadgeCount = db.UserNotifications.AsEnumerable().Where(x => x.UserId == ClientService.EmployeeId && x.UserTypeId == Utilities.UserRoles.Employee.GetEnumValue() && x.Status == Utilities.NotificationStatus.UnRead.GetEnumDescription()).ToList().Count;

                            Notifications objNotifications = new Notifications { NId = objUserNotification.Id, NType = Utilities.NotificationType.FriendlyReminder.GetEnumValue(), CommonId = ClientService.Id };
                            List<NotificationModel> notify = new List<NotificationModel>();
                            notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                            notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                            notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });
                            var EmpInfo = db.Employees.Where(x => x.Id == ClientService.EmployeeId).FirstOrDefault();
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

                        //if (isSchedueled)
                        //{
                        //    var ClientNotification = db.NotificationMasters.Where(x => x.Name == "RescheduleServiceSendToClient").FirstOrDefault();
                        //    var message = ClientNotification.Message;
                        //    message = message.Replace("{{EmpName}}", ClientService.Employee.FirstName + " " + ClientService.Employee.LastName);
                        //    message = message.Replace("{{ScheduleDate}}", ClientService.ScheduleDate.Value.ToString("MMMM dd, yyyy"));
                        //    UserNotification objUserNotification = new UserNotification();
                        //    objUserNotification.UserId = ClientService.Client.Id;
                        //    objUserNotification.UserTypeId = Utilities.UserRoles.Employee.GetEnumValue();
                        //    objUserNotification.Message = message;
                        //    objUserNotification.Status = Utilities.NotificationStatus.UnRead.GetEnumDescription();
                        //    objUserNotification.CommonId = ClientService.Id;
                        //    objUserNotification.MessageType = Utilities.NotificationType.FriendlyReminder.GetEnumDescription();
                        //    objUserNotification.AddedDate = DateTime.Now;
                        //    db.UserNotifications.Add(objUserNotification);
                        //    db.SaveChanges();

                        //    var BadgeCount = db.UserNotifications.AsEnumerable().Where(x => x.UserId == ClientService.Client.Id && x.UserTypeId == Utilities.UserRoles.Client.GetEnumValue() && x.Status == Utilities.NotificationStatus.UnRead.GetEnumDescription()).ToList().Count;

                        //    Notifications objNotifications = new Notifications { NId = objUserNotification.Id, NType = Utilities.NotificationType.FriendlyReminder.GetEnumValue(), CommonId = ClientService.Id };
                        //    List<NotificationModel> notify = new List<NotificationModel>();
                        //    notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                        //    notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                        //    notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });
                        //    if (UserInfo.DeviceType != null && UserInfo.DeviceToken != null)
                        //    {
                        //        if (UserInfo.DeviceType.ToLower() == "android")
                        //        {
                        //            string CustomData = "&data.NId=" + objNotifications.NId + "&data.NType=" + objNotifications.NType + "&data.CommonId=" + objNotifications.CommonId;
                        //            SendNotifications.SendAndroidNotification(UserInfo.DeviceToken, message, CustomData, "client");
                        //        }
                        //        else if (UserInfo.DeviceType.ToLower() == "iphone")
                        //        {
                        //            SendNotifications.SendIphoneNotification(BadgeCount, UserInfo.DeviceToken, message, notify, "client", HttpContext.Current);
                        //        }
                        //    }
                        //}

                        ClientService.Status = request.IsLateReschedule ? Utilities.ServiceTypes.NoShow.GetEnumDescription() : ClientService.Status;
                        ClientService.ApprovalEmailUrl = null;
                        ClientService.UrlExpireDate = null;

                        if (request.IsLateReschedule == false)
                        {
                            ClientService.EmployeeId = null;
                            ClientService.WorkAreaId = null;
                            ClientService.ScheduleDate = null;
                            ClientService.ScheduleStartTime = null;
                            ClientService.ScheduleEndTime = null;
                        }
                        ClientService.StatusChangeDate = DateTime.UtcNow;
                        ClientService.UpdatedBy = request.ClientId;
                        ClientService.UpdatedByType = Utilities.UserRoles.Client.GetEnumValue();
                        ClientService.UpdatedDate = DateTime.UtcNow;

                        var es = db.EmployeeSchedules.Where(x => x.ServiceId == request.ServiceId).FirstOrDefault();
                        if (es != null)
                        {
                            db.EmployeeSchedules.Remove(es);
                        }
                        var notifications = db.UserNotifications.AsEnumerable().Where(x => x.CommonId == request.ServiceId && x.MessageType == Utilities.NotificationType.ServiceScheduled.GetEnumDescription()).ToList();
                        if (notifications.Count > 0)
                        {
                            db.UserNotifications.RemoveRange(notifications);
                        }
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

                        if (!request.IsLateReschedule)
                        {
                            var scheduletime = request.ServiceRequestedTime;
                            if (ClientService.PurposeOfVisit == Utilities.PurposeOfVisit.Emergency.GetEnumDescription() && (request.ServiceRequestedOn.DayOfWeek == DayOfWeek.Saturday || request.ServiceRequestedOn.DayOfWeek == DayOfWeek.Sunday))
                            {
                                //scheduletime = request.ServiceRequestedTime;
                            }
                            else if (ClientService.PurposeOfVisit == Utilities.PurposeOfVisit.Emergency.GetEnumDescription())
                            {
                                if (scheduletime.Contains("04:00 PM"))
                                {
                                    scheduletime = scheduletime.Replace("04:00", "06:00");
                                }
                            }
                            else if (ClientService.PurposeOfVisit != Utilities.PurposeOfVisit.Emergency.GetEnumDescription())
                            {
                                if (scheduletime.Contains("06:00 PM"))
                                {
                                    scheduletime = scheduletime.Replace("06:00", "04:00");
                                }
                            };
                            RescheduleService RS = new RescheduleService();
                            RS.ServiceId = ClientService.Id;
                            RS.Rescheduletime = scheduletime; //request.ServiceRequestedTime;
                            RS.RescheduleDate = request.ServiceRequestedOn;
                            RS.Reason = request.Reason;
                            RS.AddedBy = request.ClientId;
                            RS.AddedByType = Utilities.UserRoles.Client.GetEnumValue();
                            RS.AddedDate = DateTime.UtcNow;

                            db.RescheduleServices.Add(RS);
                        }
                        db.SaveChanges();
                        if (ClientService.PurposeOfVisit == Utilities.PurposeOfVisit.Emergency.GetEnumDescription() && !request.IsLateReschedule)
                        {
                            var requestedService = ClientService.RequestedServiceBridges.FirstOrDefault();
                            var EmergencyService = db.uspa_RequestedServiceToServiceScheduler(ClientService.Id, 0, request.ClientId, ClientService.AddressID, ClientService.PurposeOfVisit, request.ServiceRequestedOn, request.ServiceRequestedTime).ToList();

                            if (EmergencyService.Count > 0)
                            {
                                var es1 = EmergencyService.FirstOrDefault();
                                if (es1.EmployeeId > 0 && es1.ServiceId > 0)
                                {
                                    //Employee Notification
                                    db.Dispose();
                                    db = new Aircall_DBEntities1();
                                    var service = db.Services.Where(x => x.Id == es1.ServiceId).FirstOrDefault();

                                    var EmpNotification = db.NotificationMasters.Where(x => x.Name == "EmployeeSchedule").FirstOrDefault();
                                    var message = EmpNotification.Message;
                                    message = message.Replace("{{ScheduleDate}}", service.ScheduleDate.Value.ToString("MMMM dd, yyyy"));

                                    UserNotification objUserNotification = new UserNotification();
                                    objUserNotification.UserId = service.EmployeeId;
                                    objUserNotification.UserTypeId = Utilities.UserRoles.Employee.GetEnumValue();
                                    objUserNotification.Message = message;
                                    objUserNotification.Status = Utilities.NotificationStatus.UnRead.GetEnumDescription();
                                    objUserNotification.CommonId = es1.ServiceId;
                                    objUserNotification.MessageType = Utilities.NotificationType.ServiceScheduled.GetEnumDescription();
                                    objUserNotification.AddedDate = DateTime.UtcNow;
                                    db.UserNotifications.Add(objUserNotification);
                                    db.SaveChanges();

                                    var BadgeCount = db.UserNotifications.AsEnumerable().Where(x => x.UserId == service.EmployeeId && x.UserTypeId == Utilities.UserRoles.Employee.GetEnumValue() && x.Status == Utilities.NotificationStatus.UnRead.GetEnumDescription()).ToList().Count;

                                    Notifications objNotifications = new Notifications { NId = objUserNotification.Id, NType = Utilities.NotificationType.ServiceScheduled.GetEnumValue(), CommonId = es1.ServiceId.Value };
                                    List<NotificationModel> notify = new List<NotificationModel>();
                                    notify.Add(new NotificationModel { Key = "NId", Value = new object[] { objNotifications.NId } });
                                    notify.Add(new NotificationModel { Key = "NType", Value = new object[] { objNotifications.NType } });
                                    notify.Add(new NotificationModel { Key = "CommonId", Value = new object[] { objNotifications.CommonId } });
                                    var EmpInfo = db.Employees.Where(x => x.Id == service.EmployeeId).FirstOrDefault();
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
                                var es2 = EmergencyService.FirstOrDefault();
                                if (es2.EmployeeId > 0 && es2.ServiceId > 0)
                                {
                                    //Client Notification

                                    var service = db.Services.Find(es2.ServiceId);
                                    var ClientNotification = db.NotificationMasters.Where(x => x.Name == "RequestedServiceScheduleSendToClient").FirstOrDefault();
                                    var message = ClientNotification.Message;
                                    message = message.Replace("{{ScheduleDate}}", service.ScheduleDate.Value.ToString("MMMM dd, yyyy"));
                                    UserNotification objUserNotification = new UserNotification();
                                    objUserNotification.UserId = service.ClientId;
                                    objUserNotification.UserTypeId = Utilities.UserRoles.Client.GetEnumValue();
                                    objUserNotification.Message = message;
                                    objUserNotification.Status = Utilities.NotificationStatus.UnRead.GetEnumDescription();
                                    objUserNotification.CommonId = service.Id;
                                    objUserNotification.MessageType = Utilities.NotificationType.ServiceScheduled.GetEnumDescription();
                                    objUserNotification.AddedDate = DateTime.UtcNow;
                                    db.UserNotifications.Add(objUserNotification);
                                    db.SaveChanges();

                                    var BadgeCount = db.uspa_ClientPortal_GetNotificationForDashBoardByUserType(service.ClientId, Utilities.UserRoles.Client.GetEnumValue(), "", 0).AsEnumerable().Where(x => x.Status == Utilities.NotificationStatus.UnRead.GetEnumDescription()).Count();
                                    Notifications objNotifications = new Notifications { NId = objUserNotification.Id, NType = Utilities.NotificationType.ServiceScheduled.GetEnumValue(), CommonId = service.Id };
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
                        }
                        var ServiceRescheduleSuccessMessage = Utilities.GetSiteSettingValue("ServiceRescheduleSuccessMessage", db);
                        res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                        res.Message = ServiceRescheduleSuccessMessage;
                        res.Data = null;
                    }
                    else
                    {
                        res.StatusCode = HttpStatusCode.NotFound.GetEnumValue();
                        res.Message = "Record Not Found";
                        res.Data = null;
                    }
                }
            }
            catch (Exception ex)
            {
                res.StatusCode = HttpStatusCode.InternalServerError.GetEnumValue();
                res.Message = ex.InnerException.Message;
                res.Data = null;
                return Ok(ex);
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

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("ClientContactUs")]
        public async Task<IHttpActionResult> ClientContactUs([FromBody]ClientContactModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();

            ContactRequest CR = new ContactRequest();
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Message) || string.IsNullOrWhiteSpace(request.Name))
                {
                    res.Message = "Please Provide Required Information";
                    res.StatusCode = HttpStatusCode.BadRequest.GetEnumValue();
                }
                else
                {
                    CR.Name = request.Name;
                    CR.PhoneNumber = (string.IsNullOrWhiteSpace(request.PhoneNumber) ? "" : request.PhoneNumber);
                    CR.Email = request.Email;
                    CR.Message = request.Message;
                    CR.RequestDate = DateTime.UtcNow;
                    db.ContactRequests.Add(CR);
                    db.SaveChanges();

                    EmailTemplate template = db.EmailTemplates.Where(x => x.Name == "ContactUs" && x.Status == true).FirstOrDefault();
                    var str = template.EmailBody;
                    str = str.Replace("{{FirstName}}", CR.Name);
                    Utilities.Send(template.EmailTemplateSubject, request.Email, str, template.FromEmail, db);

                    EmailTemplate templateAdmin = db.EmailTemplates.Where(x => x.Name == "ContactUsAdmin" && x.Status == true).FirstOrDefault();
                    var stradmin = templateAdmin.EmailBody;
                    stradmin = stradmin.Replace("{{Name}}", CR.Name);
                    stradmin = stradmin.Replace("{{Email}}", CR.Email);
                    stradmin = stradmin.Replace("{{PhoneNumber}}", CR.PhoneNumber);
                    stradmin = stradmin.Replace("{{Message}}", CR.Message);

                    var AdminEmail = Utilities.GetSiteSettingValue("AdminEmail", db);
                    Utilities.Send(templateAdmin.EmailTemplateSubject, AdminEmail, stradmin, templateAdmin.FromEmail, db);

                    res.Message = "Contact Request Sent Successfully";
                    res.StatusCode = HttpStatusCode.OK.GetEnumValue();
                    res.Data = null;
                }
            }
            catch (Exception ex)
            {
                res.Message = "Internal Server Error";
                res.StatusCode = HttpStatusCode.InternalServerError.GetEnumValue();
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

        [ResponseType(typeof(ResponseModel))]
        [HttpPost]
        [Route("checkdata")]
        public async Task<IHttpActionResult> checkdaterange([FromBody]ClientUnitAddModel request)
        {
            db = new Aircall_DBEntities1();
            ResponseModel res = new ResponseModel();


            HttpClient client = new HttpClient();

            var dd = await client.GetAsync("https://maps.googleapis.com/maps/api/geocode/json?address=714+La+Brea+Suite+230,+Hollywood,+California&key=AIzaSyAqIYKCniqGpTtlp_QSeJPPqRZ1bRt6A9M");
            var data = await dd.Content.ReadAsAsync<Example>();
            db.Dispose();
            return Ok(res);
        }

        [ResponseType(typeof(ResponseModel))]
        [HttpGet]
        [Route("datacheck")]
        public async Task<IHttpActionResult> datacheck()
        {
            ResponseModel res = new ResponseModel();
            var values = DurationExtensions.GetValues<Utilities.PurposeOfVisit>();
            var subscriptionService = new StripeSubscriptionService();
            StripeSubscriptionCreateOptions ssco = new StripeSubscriptionCreateOptions();

            StripeSubscription stripeSubscription = subscriptionService.Create("cus_8m7I1PKQvD9MeM", "residential");

            res.Data = values;

            return Ok(res);
        }
    }
}