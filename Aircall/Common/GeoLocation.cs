using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aircall.Common
{
    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Geometry
    {
        public Location location { get; set; }
        public string location_type { get; set; }
        public object viewport { get; set; }
    }

    public class Result
    {
        public object address_components { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public bool partial_match { get; set; }
        public string place_id { get; set; }
        public IList<string> types { get; set; }
    }

    public class Example
    {
        public IList<Result> results { get; set; }
        public string status { get; set; }
    }
}