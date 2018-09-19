using api.Models;
using api.ViewModel;
using AutoMapper;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Mappers
{
    public class GlobalMappers
    {
        public GlobalMappers()
        {
            //Mapper.CreateMap<ClientAddress, ClientAddressModel>().ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.State1.Name))
            //                                                         .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City1.Name))
            //                                                         .ForMember(dest => dest.HomeNumber, opt => opt.MapFrom(src => src.Client.HomeNumber))
            //                                                         .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.Client.MobileNumber))
            //                                                         .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Client.PhoneNumber))
            //                                                         .ForMember(dest => dest.OfficeNumber, opt => opt.MapFrom(src => src.Client.OfficeNumber));


            //Mapper.CreateMap<Client, ClientProfileModel>().ForMember(dest => dest.MobileNumber, opt => opt.NullSubstitute(string.Empty))
            //                                              .ForMember(dest => dest.OfficeNumber, opt => opt.NullSubstitute(string.Empty))
            //                                              .ForMember(dest => dest.HomeNumber, opt => opt.NullSubstitute(string.Empty));

        }
        public static void Init()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ClientPaymentMethod, ClientPaymentModel>().ForMember(dest => dest.NameOnCard, opt => opt.NullSubstitute(string.Empty))
                                                                        .ForMember(dest => dest.CardNumber, opt => opt.NullSubstitute(string.Empty))
                                                                        .ForMember(dest => dest.IsDefaultPayment, opt => opt.NullSubstitute(false))
                                                                        .ForMember(dest => dest.ExpiryMonth, opt => opt.MapFrom(src => src.ExpiryMonth.ToString()));

                cfg.CreateMap<ClientUnitAddModel, ClientUnit>().ForMember(dest => dest.IsSpecialApplied, opt => opt.MapFrom(src => src.SpecialOffer))
                                                               .ForMember(dest => dest.IsSpecialApplied, opt => opt.NullSubstitute(false));
                cfg.CreateMap<Employee, EmpProfileModel>().ForMember(dest => dest.Address, opt => opt.NullSubstitute(string.Empty))
                                                              .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                                                              .ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.State.Name))
                                                              .ForMember(dest => dest.City, opt => opt.NullSubstitute(string.Empty))
                                                              .ForMember(dest => dest.StateName, opt => opt.NullSubstitute(string.Empty));

                cfg.CreateMap<Client, ClientProfileModel>().ForMember(dest => dest.MobileNumber, opt => opt.NullSubstitute(string.Empty))
                                                          .ForMember(dest => dest.OfficeNumber, opt => opt.NullSubstitute(string.Empty))
                                                          .ForMember(dest => dest.HomeNumber, opt => opt.NullSubstitute(string.Empty));

                cfg.CreateMap<ClientAddress, ClientAddressModel>().ForMember(dest => dest.StateName, opt => opt.MapFrom(src => src.State1.Name))
                                                                     .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City1.Name))
                                                                     .ForMember(dest => dest.HomeNumber, opt => opt.MapFrom(src => src.Client.HomeNumber))
                                                                     .ForMember(dest => dest.MobileNumber, opt => opt.MapFrom(src => src.Client.MobileNumber))
                                                                     .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Client.PhoneNumber))
                                                                     .ForMember(dest => dest.OfficeNumber, opt => opt.MapFrom(src => src.Client.OfficeNumber))
                                                                     .ForMember(dest => dest.MobileNumber, opt => opt.NullSubstitute(string.Empty))
                                                                     .ForMember(dest => dest.OfficeNumber, opt => opt.NullSubstitute(string.Empty))
                                                                     .ForMember(dest => dest.HomeNumber, opt => opt.NullSubstitute(string.Empty))
                                                                     .ForMember(dest => dest.PhoneNumber, opt => opt.NullSubstitute(string.Empty))
                                                                     .ForMember(dest => dest.Latitude, opt => opt.NullSubstitute(string.Empty))
                                                                     .ForMember(dest => dest.Longitude, opt => opt.NullSubstitute(string.Empty));
                cfg.CreateMap<StripeError, StripeErrorLog>();
                cfg.CreateMap<Client, ClientListModel>().ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));
                cfg.CreateMap<SalesVisitRequest, SalesVisitRequestModel>().ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.FirstName + " " + src.Client.LastName))
                                                                        .ForMember(dest => dest.RequestedDate, opt => opt.MapFrom(src => src.AddedDate.ToString("MMM dd,yyyy hh:mm tt")));
                cfg.CreateMap<ClientUnit, ClientUnitsModel>().ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => src.Id));
                cfg.CreateMap<ClientUnit, EmpClientUnitAddModel>().ReverseMap();
                cfg.CreateMap<Unit, UnitView>();
                cfg.CreateMap<UnitView, ClientUnitPart>().ForMember(dest => dest.RefrigerantType, opt => opt.MapFrom(src => src.Refrigerant)).ReverseMap();

                cfg.CreateMap<ClientUnitPart, EmpClientUnitParts>();
                cfg.CreateMap<EmpClientUnitParts, Unit>().ReverseMap();
                cfg.CreateMap<EmpClientUnitParts, ClientUnitPart>()
                    .ForMember(dest => dest.Id, opt => opt.UseDestinationValue())
                    .ForMember(dest => dest.SplitType, opt => opt.MapFrom(src => src.UnitType))
                    .ForMember(dest => dest.MaxBreaker, opt => opt.Ignore())
                    .ForMember(dest => dest.BlowerMotor, opt => opt.MapFrom(x => x.BlowerMotor == 0 ? null : x.BlowerMotor))
                    .ForMember(dest => dest.Breaker, opt => opt.MapFrom(x => x.Breaker == 0 ? null : x.Breaker))
                    .ForMember(dest => dest.Capacitor, opt => opt.MapFrom(x => x.Capacitor == 0 ? null : x.Capacitor))
                    .ForMember(dest => dest.Coil, opt => opt.MapFrom(x => x.Coil == 0 ? null : x.Coil))
                    .ForMember(dest => dest.Compressor, opt => opt.MapFrom(x => x.Compressor == 0 ? null : x.Compressor))
                    .ForMember(dest => dest.Condensingfanmotor, opt => opt.MapFrom(x => x.Condensingfanmotor == 0 ? null : x.Condensingfanmotor))
                    .ForMember(dest => dest.Contactor, opt => opt.MapFrom(x => x.Contactor == 0 ? null : x.Contactor))
                    .ForMember(dest => dest.Controlboard, opt => opt.MapFrom(x => x.Controlboard == 0 ? null : x.Controlboard))
                    .ForMember(dest => dest.Defrostboard, opt => opt.MapFrom(x => x.Defrostboard == 0 ? null : x.Defrostboard))
                    .ForMember(dest => dest.Doorswitch, opt => opt.MapFrom(x => x.Doorswitch == 0 ? null : x.Doorswitch))
                    .ForMember(dest => dest.Filterdryer, opt => opt.MapFrom(x => x.Filterdryer == 0 ? null : x.Filterdryer))
                    .ForMember(dest => dest.Flamesensor, opt => opt.MapFrom(x => x.Flamesensor == 0 ? null : x.Flamesensor))
                    .ForMember(dest => dest.Gasvalve, opt => opt.MapFrom(x => x.Gasvalve == 0 ? null : x.Gasvalve))
                    .ForMember(dest => dest.Ignitioncontrolboard, opt => opt.MapFrom(x => x.Ignitioncontrolboard == 0 ? null : x.Ignitioncontrolboard))
                    .ForMember(dest => dest.Ignitor, opt => opt.MapFrom(x => x.Ignitor == 0 ? null : x.Ignitor))
                    .ForMember(dest => dest.Inducerdraftmotor, opt => opt.MapFrom(x => x.Inducerdraftmotor == 0 ? null : x.Inducerdraftmotor))
                    .ForMember(dest => dest.Limitswitch, opt => opt.MapFrom(x => x.Limitswitch == 0 ? null : x.Limitswitch))
                    .ForMember(dest => dest.Misc, opt => opt.MapFrom(x => x.Misc == 0 ? null : x.Misc))
                    .ForMember(dest => dest.Pressureswitch, opt => opt.MapFrom(x => x.Pressureswitch == 0 ? null : x.Pressureswitch))
                    .ForMember(dest => dest.RefrigerantType, opt => opt.MapFrom(x => x.Refrigerant == 0 ? null : x.Refrigerant))
                    .ForMember(dest => dest.Relay, opt => opt.MapFrom(x => x.Relay == 0 ? null : x.Relay))
                    .ForMember(dest => dest.ReversingValve, opt => opt.MapFrom(x => x.ReversingValve == 0 ? null : x.ReversingValve))
                    .ForMember(dest => dest.Rolloutsensor, opt => opt.MapFrom(x => x.Rolloutsensor == 0 ? null : x.Rolloutsensor))
                    .ForMember(dest => dest.Transformer, opt => opt.MapFrom(x => x.Transformer == 0 ? null : x.Transformer))
                    .ForMember(dest => dest.TXVValve, opt => opt.MapFrom(x => x.TXVValve == 0 ? null : x.TXVValve));
            });
        }
    }
}