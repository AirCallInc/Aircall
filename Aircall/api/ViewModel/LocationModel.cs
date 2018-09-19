using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.ViewModel
{
    public class GetStateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDefault { get; set; }
    }

    public class GetCityModel
    {
        public int Id { get; set; }
        public Nullable<int> StateId { get; set; }
        public string Name { get; set; }
    }
}