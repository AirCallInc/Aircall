using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.ViewModel
{
    public class UnitView
    {
        public string ModelNumber { get; set; }
        public string SerialNumber { get; set; }
        public Nullable<System.DateTime> ManufactureDate { get; set; }
        public string ManufactureBrand { get; set; }
        public string UnitTon { get; set; }
        public Nullable<int> Booster { get; set; }
        public Nullable<int> Refrigerant { get; set; }
        public string ElectricalService { get; set; }
        public Nullable<int> Breaker { get; set; }
        public string MaxBreaker { get; set; }
        public Nullable<int> Compressor { get; set; }
        public Nullable<int> Capacitor { get; set; }
        public Nullable<int> Contactor { get; set; }
        public Nullable<int> Filterdryer { get; set; }
        public Nullable<int> Defrostboard { get; set; }
        public Nullable<int> Relay { get; set; }
        public Nullable<int> TXVValve { get; set; }
        public Nullable<int> ReversingValve { get; set; }
        public Nullable<int> BlowerMotor { get; set; }
        public Nullable<int> Condensingfanmotor { get; set; }
        public Nullable<int> Inducerdraftmotor { get; set; }
        public Nullable<int> Transformer { get; set; }
        public Nullable<int> Controlboard { get; set; }
        public Nullable<int> Limitswitch { get; set; }
        public Nullable<int> Ignitor { get; set; }
        public Nullable<int> Gasvalve { get; set; }
        public Nullable<int> Pressureswitch { get; set; }
        public Nullable<int> Flamesensor { get; set; }
        public Nullable<int> Rolloutsensor { get; set; }
        public Nullable<int> Doorswitch { get; set; }
        public Nullable<int> Ignitioncontrolboard { get; set; }
    }
}