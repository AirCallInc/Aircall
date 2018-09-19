using api.ActionFilters;
using api.Models;
using api.ViewModel;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace api.Controllers
{
    [RoutePrefix("v1/Location")]
    public class LocationController : BaseClientApiController
    {
        Aircall_DBEntities1 db = new Aircall_DBEntities1();

        [AuthorizationRequired]
        [ResponseType(typeof(ResponseModel))]
        [HttpGet]
        [Route("GetAllState")]
        public async Task<IHttpActionResult> GetAllState()
        {
            ResponseModel res = new ResponseModel();
            var LoadStates = db.States.Where(x => x.Status == true && x.IsDeleted == false && x.PendingInactive == false).ToList();
            if (LoadStates.Count > 0)
            {
                Mapper.CreateMap<State, GetStateModel>();
                var FinalLoadStates = AutoMapper.Mapper.Map<List<GetStateModel>>(LoadStates);

                res.StatusCode = (int)HttpStatusCode.OK;
                res.Message = "Success";
                res.Data = FinalLoadStates;
            }
            else
            {
                res.StatusCode = (int)HttpStatusCode.NotFound;
                res.Message = "States not found";
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
        [Route("GetAllCityByStateId")]
        public async Task<IHttpActionResult> GetAllCityByStateId([FromUri]int StateId)
        {
            ResponseModel res = new ResponseModel();
            var LoadCity = db.Cities.Where(x => x.StateId == StateId && x.Status == true && x.IsDeleted == false && x.PendingInactive == false).ToList();
            if (LoadCity.Count > 0)
            {
                Mapper.CreateMap<City, GetCityModel>();
                var FinalLoadCity = AutoMapper.Mapper.Map<List<GetCityModel>>(LoadCity);

                res.StatusCode = (int)HttpStatusCode.OK;
                res.Message = "Success";
                res.Data = FinalLoadCity;
            }
            else
            {
                res.StatusCode = (int)HttpStatusCode.NotFound;
                res.Message = "Cities not found for selected state";
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
