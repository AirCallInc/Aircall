using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aircall.Common
{
    public partial class Plan
    {
        public int Drivetime { get; set; }
        public int ServiceTimeForFirstUnit { get; set; }
        public int ServiceTimeForAdditionalUnits { get; set; }
    }
}