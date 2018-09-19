using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.ViewModel
{
    public class EmpClientUnitParts
    {
        public bool IsMatched { get; set; }
        public string ModelNumber { get; set; }
        public string SerialNumber { get; set; }
        public string UnitType { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public string ManufactureBrand { get; set; }
        public string UnitTon { get; set; }
        public int? Refrigerant { get; set; }
        public string ElectricalService { get; set; }
        public int? Breaker { get; set; }
        public string MaxBreaker { get; set; }
        public int? Compressor { get; set; }
        public int? Capacitor { get; set; }
        public int? Contactor { get; set; }
        public int? Filterdryer { get; set; }
        public int? Defrostboard { get; set; }
        public int? Relay { get; set; }
        public int? TXVValve { get; set; }
        public int? ReversingValve { get; set; }
        public int? BlowerMotor { get; set; }
        public int? Condensingfanmotor { get; set; }
        public int? Inducerdraftmotor { get; set; }
        public int? Transformer { get; set; }
        public int? Controlboard { get; set; }
        public int? Limitswitch { get; set; }
        public int? Ignitor { get; set; }
        public int? Gasvalve { get; set; }
        public int? Pressureswitch { get; set; }
        public int? Flamesensor { get; set; }
        public int? Rolloutsensor { get; set; }
        public int? Doorswitch { get; set; }
        public int? Ignitioncontrolboard { get; set; }
        public int? Coil { get; set; }
        public int? Misc { get; set; }
        public EmpUnitOptions OptionalInformation { get; set; }
    }

    public class EmpUnitOptions
    {        
        public int QuantityOfFilter { get; set; }
        public List<FilterDetails> Filters { get; set; }
        public int QuantityOfFuses { get; set; }
        public List<FuseDetails> FuseTypes { get; set; }
        public int? ThermostatTypes { get; set; }
    }

    public class EmpClientUnitModel
    {
        public int EmployeeId { get; set; }
        public int ClientId { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public string Notes { get; set; }
        public int AddressId { get; set; }
        
        public List<EmpClientUnitParts> Parts { get; set; }
    }

    public class EmpClientUnitAddModel
    {
        public int EmployeeId { get; set; }
        public int ClientId { get; set; }
        public string UnitName { get; set; }
        public Nullable<int> PlanTypeId { get; set; }
        public Nullable<int> AddressId { get; set; }
        public Nullable<int> UnitTypeId { get; set; }
        public Nullable<bool> AutoRenewal { get; set; }
        public Nullable<bool> SpecialOffer { get; set; }
        public string CurrentPaymentMethod { get; set; }
        public bool IsMatched { get; set; }
        public string Notes { get; set; }
        public List<EmpClientUnitParts> Parts { get; set; }
    }
}