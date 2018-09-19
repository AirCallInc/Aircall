using api.Models;
using AutoMapper;
using api.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Linq.Expressions;

namespace api.Controllers
{
    public class BaseClientApiController : ApiController
    {
        public static bool updatetoken = false;
        public static string accessToken = "";
        public static async Task<TokenDetails> generatToken(string Email, string Password,string deviceid)
        {
            var request = HttpContext.Current.Request;
            var tokenServiceUrl = ConfigurationManager.AppSettings["APIURL"].ToString() + "/oauth/Token";
            using (var client = new HttpClient())
            {
                var requestParams = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", Email),
                    new KeyValuePair<string, string>("password", Password),
                    new KeyValuePair<string, string>("deviceid", deviceid)
                };
                var requestParamsFormUrlEncoded = new FormUrlEncodedContent(requestParams);
                var tokenServiceResponse = await client.PostAsync(tokenServiceUrl, requestParamsFormUrlEncoded);
                var responseString = await tokenServiceResponse.Content.ReadAsStringAsync();
                var responseCode = tokenServiceResponse.StatusCode;
                JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                TokenDetails objToken = jsonSerializer.Deserialize<TokenDetails>(responseString.ToString());
                //detocken(objToken.access_token);
                return objToken;
            }
        }
        public static void detocken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            SecurityToken t = handler.ReadToken(token);
        }
        public static IQueryable<T> CreatePagedResults<T, TResult>(IQueryable<T> query, int pageNum, int pageSize, out int rowsCount,out int page, out int totalPageCount)
        {
            if (pageSize <= 0) pageSize = 20;
            if (pageNum <= 0) pageNum = 1;
            //Total result count
            rowsCount = query.Count();
            var mod = rowsCount % pageSize;
            totalPageCount = (rowsCount / pageSize) + (mod == 0 ? 0 : 1);
            //If page number should be > 0 else set to first page
            //if (rowsCount <= pageSize || pageNum <= 0) pageNum = 1;

            //Calculate nunber of rows to skip on pagesize
            int excludedRows = (pageNum - 1) * pageSize;

            //query = isAscendingOrder ? query.OrderBy(orderByProperty) : query.OrderByDescending(orderByProperty);

            //Skip the required rows for the current page and take the next records of pagesize count
            page = pageNum + 1;
            return query.Skip(excludedRows).Take(pageSize);
        }
        //protected IEnumerable<TEntity> CreatePagedResults(IEnumerable<T> queryable,int page,int pageSize)
        //{
        //    var skipAmount = pageSize * (page - 1);

        //    var projection = queryable.Skip(skipAmount).Take(pageSize).ToList();

        //    var totalNumberOfRecords = queryable.LongCount();
        //    var results = projection.ToList();

        //    var mod = totalNumberOfRecords % pageSize;
        //    var totalPageCount = (totalNumberOfRecords / pageSize) + (mod == 0 ? 0 : 1);        

        //    return new ResponseListModel()
        //    {
        //        Data = results,
        //        PageNumber = page,
        //        PageSize = results.Count,
        //        TotalNumberOfPages = totalPageCount,
        //        TotalNumberOfRecords = totalNumberOfRecords,                
        //    };
        //}
        public static void Add_UpdateToken(int userId, TokenDetails _token,int forceupdate = 1)
        {
            try
            {
                Aircall_DBEntities1 db = new Aircall_DBEntities1();
                var chkToken = db.AppAccessTokens.Where(x => x.UserId == userId && x.UserType == 4).FirstOrDefault();
                if (chkToken != null)
                {
                    if (chkToken.ExpiresOn <= DateTime.UtcNow)
                    {
                        db.Entry(chkToken).State = EntityState.Modified;

                        TimeSpan t = TimeSpan.FromMinutes(Convert.ToInt16(ConfigurationManager.AppSettings["TokenExpireHour"].ToString()));
                        chkToken.UserType = (int)(api.App_Start.Utilities.UserRoles.Client);
                        chkToken.AuthToken = _token.access_token;
                        chkToken.ExpiresOn = DateTime.UtcNow.Add(t);
                        chkToken.IssuedOn = DateTime.UtcNow;
                        db.SaveChanges();
                    }
                    else
                    {
                        if (forceupdate > 0)
                        {
                            db.Entry(chkToken).State = EntityState.Modified;

                            TimeSpan t = TimeSpan.FromMinutes(Convert.ToInt16(ConfigurationManager.AppSettings["TokenExpireHour"].ToString()));
                            chkToken.UserType = (int)(api.App_Start.Utilities.UserRoles.Client);
                            chkToken.AuthToken = _token.access_token;
                            chkToken.ExpiresOn = DateTime.UtcNow.Add(t);
                            chkToken.IssuedOn = DateTime.UtcNow;
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    AppAccessToken _tokenDetails = new AppAccessToken();
                    _tokenDetails.UserId = userId;
                    _tokenDetails.IssuedOn = DateTime.UtcNow;
                    TimeSpan t = TimeSpan.FromMinutes(Convert.ToInt16(ConfigurationManager.AppSettings["TokenExpireHour"].ToString()));
                    _tokenDetails.ExpiresOn = DateTime.UtcNow.Add(t);
                    _tokenDetails.UserType = (int)(api.App_Start.Utilities.UserRoles.Client);
                    _tokenDetails.AuthToken = _token.access_token;
                    db.AppAccessTokens.Add(_tokenDetails);
                    db.SaveChanges();
                }
                db.Dispose();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }

    public class BaseEmployeeApiController : ApiController
    {
        public static bool updatetoken = false;
        public static string accessToken = "";        

        public static async Task<TokenDetails> generatEmpToken(string Email, string Password, string deviceid)
        {
            var request = HttpContext.Current.Request;
            var tokenServiceUrl = ConfigurationManager.AppSettings["APIURL"].ToString() + "/oauth/Token";
            using (var client = new HttpClient())
            {
                var requestParams = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", Email),
                    new KeyValuePair<string, string>("password", Password),
                    new KeyValuePair<string, string>("deviceid", deviceid),
                    new KeyValuePair<string, string>("usertype", "employee")
                };
                var requestParamsFormUrlEncoded = new FormUrlEncodedContent(requestParams);
                var tokenServiceResponse = await client.PostAsync(tokenServiceUrl, requestParamsFormUrlEncoded);
                var responseString = await tokenServiceResponse.Content.ReadAsStringAsync();
                var responseCode = tokenServiceResponse.StatusCode;
                JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                TokenDetails objToken = jsonSerializer.Deserialize<TokenDetails>(responseString.ToString());
                //detocken(objToken.access_token);
                return objToken;
            }
        }
        public static void detocken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            SecurityToken t = handler.ReadToken(token);
        }
        public static IQueryable<T> CreatePagedResults<T, TResult>(IQueryable<T> query, int pageNum, int pageSize, out int rowsCount, out int page, out int totalPageCount)
        {
            if (pageSize <= 0) pageSize = 20;
            if (pageNum <= 0) pageNum = 1;
            //Total result count
            rowsCount = query.Count();
            var mod = rowsCount % pageSize;
            totalPageCount = (rowsCount / pageSize) + (mod == 0 ? 0 : 1);
            int excludedRows = (pageNum - 1) * pageSize;
            page = pageNum + 1;
            return query.Skip(excludedRows).Take(pageSize);
        }
        public static void Add_UpdateToken(int userId, TokenDetails _token, int forceupdate = 0)
        {
            try
            {
                Aircall_DBEntities1 db = new Aircall_DBEntities1();
                var chkToken = db.EmpAppAccessTokens.AsEnumerable().Where(x => x.UserId == userId && x.UserType == 5).FirstOrDefault();
                if (chkToken != null)
                {
                    if (chkToken.ExpiresOn <= DateTime.Now)
                    {
                        db.Entry(chkToken).State = EntityState.Modified;

                        TimeSpan t = TimeSpan.FromHours(Convert.ToInt16(ConfigurationManager.AppSettings["TokenExpireHour"].ToString()));
                        chkToken.UserType = (int)(api.App_Start.Utilities.UserRoles.Employee);
                        chkToken.AuthToken = _token.access_token;
                        chkToken.ExpiresOn = DateTime.UtcNow.Add(t);
                        chkToken.IssuedOn = DateTime.UtcNow;
                        db.SaveChanges();
                    }
                    else
                    {
                        if (forceupdate > 0)
                        {
                            db.Entry(chkToken).State = EntityState.Modified;

                            TimeSpan t = TimeSpan.FromHours(Convert.ToInt16(ConfigurationManager.AppSettings["TokenExpireHour"].ToString()));
                            chkToken.UserType = (int)(api.App_Start.Utilities.UserRoles.Employee);
                            chkToken.AuthToken = _token.access_token;
                            chkToken.ExpiresOn = DateTime.UtcNow.Add(t);
                            chkToken.IssuedOn = DateTime.UtcNow;
                            db.SaveChanges();
                        }
                    }
                }
                else
                {
                    EmpAppAccessToken _tokenDetails = new EmpAppAccessToken();
                    _tokenDetails.UserId = userId;
                    _tokenDetails.IssuedOn = DateTime.Now;
                    TimeSpan t = TimeSpan.FromHours(Convert.ToInt16(ConfigurationManager.AppSettings["TokenExpireHour"].ToString()));
                    _tokenDetails.ExpiresOn = DateTime.Now.Add(t);
                    _tokenDetails.UserType = (int)(api.App_Start.Utilities.UserRoles.Employee);
                    _tokenDetails.AuthToken = _token.access_token;
                    db.EmpAppAccessTokens.Add(_tokenDetails);
                    db.SaveChanges();
                }
                db.Dispose();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}