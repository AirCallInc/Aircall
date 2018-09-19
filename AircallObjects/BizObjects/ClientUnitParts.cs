using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZWT.DbLib;

namespace BizObjects
{
    public class ClientUnitParts : BizObject
    {
        public long Id { get; set; }
        public int UnitId { get; set; }
        public string SplitType { get; set; }
        public string ModelNumber { get; set; }
        public string SerialNumber { get; set; }
        public string ManufactureBrand { get; set; }
        public DateTime ManufactureDate{ get; set; }
        public string UnitTon { get; set; }
        public int Thermostat { get; set; }
        public int RefrigerantType { get; set; }
        public string ElectricalService { get; set; }
        public string MaxBreaker { get; set; }
        public int Breaker { get; set; }
        public int Compressor { get; set; }
        public int Capacitor { get; set; }
        public int Contactor { get; set; }
        public int Filterdryer { get; set; }
        public int Defrostboard { get; set; }
        public int Relay { get; set; }
        public int TXVValve { get; set; }
        public int ReversingValve { get; set; }
        public int BlowerMotor { get; set; }
        public int Condensingfanmotor { get; set; }
        public int Inducerdraftmotor { get; set; }
        public int Transformer { get; set; }
        public int Controlboard { get; set; }
        public int Limitswitch { get; set; }
        public int Ignitor { get; set; }
        public int Gasvalve { get; set; }
        public int Pressureswitch { get; set; }
        public int Flamesensor { get; set; }
        public int Rolloutsensor { get; set; }
        public int Doorswitch { get; set; }
        public int Ignitioncontrolboard { get; set; }
        public int Coil { get; set; }
        public int Misc { get; set; }

        #region "Constructors"
        public ClientUnitParts()
        {
        }
        public ClientUnitParts(ref DataRow drRow)
        {
            _LoadFromDb(ref drRow);
        }
        #endregion

        #region "Methods overridden of BizObjects"
        protected override void _LoadFromDb(ref DataRow drRow)
        {
            DBUtils dbUtil = new DBUtils(drRow);
        }

        public override void AddInsertParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@UnitId", SqlDbType.Int, this.UnitId);
            dataLib.AddParameter("@SplitType", SqlDbType.NVarChar, this.SplitType);
            dataLib.AddParameter("@ModelNumber", SqlDbType.NVarChar, this.ModelNumber);
            dataLib.AddParameter("@SerialNumber", SqlDbType.NVarChar, this.SerialNumber);
            dataLib.AddParameter("@ManufactureBrand", SqlDbType.NVarChar, this.ManufactureBrand);
            dataLib.AddParameter("@ManufactureDate", SqlDbType.DateTime, this.ManufactureDate);
            dataLib.AddParameter("@UnitTon", SqlDbType.NVarChar, this.UnitTon);
            dataLib.AddParameter("@Thermostat", SqlDbType.Int, this.Thermostat);
            dataLib.AddParameter("@RefrigerantType", SqlDbType.Int, this.RefrigerantType);
            dataLib.AddParameter("@ElectricalService", SqlDbType.NVarChar, this.ElectricalService);
            dataLib.AddParameter("@MaxBreaker", SqlDbType.NVarChar, this.MaxBreaker);
            dataLib.AddParameter("@Breaker", SqlDbType.Int, this.Breaker);
            dataLib.AddParameter("@Compressor", SqlDbType.Int, this.Compressor);
            dataLib.AddParameter("@Capacitor", SqlDbType.Int, this.Capacitor);
            dataLib.AddParameter("@Contactor", SqlDbType.Int, this.Contactor);
            dataLib.AddParameter("@Filterdryer", SqlDbType.Int, this.Filterdryer);
            dataLib.AddParameter("@Defrostboard", SqlDbType.Int, this.Defrostboard);
            dataLib.AddParameter("@Relay", SqlDbType.Int, this.Relay);
            dataLib.AddParameter("@TXVValve", SqlDbType.Int, this.TXVValve);
            dataLib.AddParameter("@ReversingValve", SqlDbType.Int, this.ReversingValve);
            dataLib.AddParameter("@BlowerMotor", SqlDbType.Int, this.BlowerMotor);
            dataLib.AddParameter("@Condensingfanmotor", SqlDbType.Int, this.Condensingfanmotor);
            dataLib.AddParameter("@Inducerdraftmotor", SqlDbType.Int, this.Inducerdraftmotor);
            dataLib.AddParameter("@Transformer", SqlDbType.Int, this.Transformer);
            dataLib.AddParameter("@Controlboard", SqlDbType.Int, this.Controlboard);
            dataLib.AddParameter("@Limitswitch", SqlDbType.Int, this.Limitswitch);
            dataLib.AddParameter("@Ignitor", SqlDbType.Int, this.Ignitor);
            dataLib.AddParameter("@Gasvalve", SqlDbType.Int, this.Gasvalve);
            dataLib.AddParameter("@Pressureswitch", SqlDbType.Int, this.Pressureswitch);
            dataLib.AddParameter("@Flamesensor", SqlDbType.Int, this.Flamesensor);
            dataLib.AddParameter("@Rolloutsensor", SqlDbType.Int, this.Rolloutsensor);
            dataLib.AddParameter("@Doorswitch", SqlDbType.Int, this.Doorswitch);
            dataLib.AddParameter("@Ignitioncontrolboard", SqlDbType.Int, this.Ignitioncontrolboard);
            dataLib.AddParameter("@Coil", SqlDbType.Int, this.Coil);
            dataLib.AddParameter("@Misc", SqlDbType.Int, this.Misc);
        }

        public override void AddUpdateParams(ref ZWT.DbLib.IDataLib dataLib)
        {
            dataLib.AddParameter("@Id", SqlDbType.BigInt, this.Id);
            dataLib.AddParameter("@UnitId", SqlDbType.Int, this.UnitId);
            dataLib.AddParameter("@SplitType", SqlDbType.NVarChar, this.SplitType);
            dataLib.AddParameter("@ModelNumber", SqlDbType.NVarChar, this.ModelNumber);
            dataLib.AddParameter("@SerialNumber", SqlDbType.NVarChar, this.SerialNumber);
            dataLib.AddParameter("@ManufactureBrand", SqlDbType.NVarChar, this.ManufactureBrand);
            dataLib.AddParameter("@ManufactureDate", SqlDbType.DateTime, this.ManufactureDate);
            dataLib.AddParameter("@UnitTon", SqlDbType.NVarChar, this.UnitTon);
            dataLib.AddParameter("@Thermostat", SqlDbType.Int, this.Thermostat);
            dataLib.AddParameter("@RefrigerantType", SqlDbType.Int, this.RefrigerantType);
            dataLib.AddParameter("@ElectricalService", SqlDbType.NVarChar, this.ElectricalService);
            dataLib.AddParameter("@MaxBreaker", SqlDbType.NVarChar, this.MaxBreaker);
            dataLib.AddParameter("@Breaker", SqlDbType.Int, this.Breaker);
            dataLib.AddParameter("@Compressor", SqlDbType.Int, this.Compressor);
            dataLib.AddParameter("@Capacitor", SqlDbType.Int, this.Capacitor);
            dataLib.AddParameter("@Contactor", SqlDbType.Int, this.Contactor);
            dataLib.AddParameter("@Filterdryer", SqlDbType.Int, this.Filterdryer);
            dataLib.AddParameter("@Defrostboard", SqlDbType.Int, this.Defrostboard);
            dataLib.AddParameter("@Relay", SqlDbType.Int, this.Relay);
            dataLib.AddParameter("@TXVValve", SqlDbType.Int, this.TXVValve);
            dataLib.AddParameter("@ReversingValve", SqlDbType.Int, this.ReversingValve);
            dataLib.AddParameter("@BlowerMotor", SqlDbType.Int, this.BlowerMotor);
            dataLib.AddParameter("@Condensingfanmotor", SqlDbType.Int, this.Condensingfanmotor);
            dataLib.AddParameter("@Inducerdraftmotor", SqlDbType.Int, this.Inducerdraftmotor);
            dataLib.AddParameter("@Transformer", SqlDbType.Int, this.Transformer);
            dataLib.AddParameter("@Controlboard", SqlDbType.Int, this.Controlboard);
            dataLib.AddParameter("@Limitswitch", SqlDbType.Int, this.Limitswitch);
            dataLib.AddParameter("@Ignitor", SqlDbType.Int, this.Ignitor);
            dataLib.AddParameter("@Gasvalve", SqlDbType.Int, this.Gasvalve);
            dataLib.AddParameter("@Pressureswitch", SqlDbType.Int, this.Pressureswitch);
            dataLib.AddParameter("@Flamesensor", SqlDbType.Int, this.Flamesensor);
            dataLib.AddParameter("@Rolloutsensor", SqlDbType.Int, this.Rolloutsensor);
            dataLib.AddParameter("@Doorswitch", SqlDbType.Int, this.Doorswitch);
            dataLib.AddParameter("@Ignitioncontrolboard", SqlDbType.Int, this.Ignitioncontrolboard);
            dataLib.AddParameter("@Coil", SqlDbType.Int, this.Coil);
            dataLib.AddParameter("@Misc", SqlDbType.Int, this.Misc);
        }

        public override void AddSearchParams(ref ZWT.DbLib.IDataLib dataLib)
        {

        }
        #endregion
    }
}