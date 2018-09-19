using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.ViewModel
{
    public class TokenDetails
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string error { get; set; }
        public string refresh_token { get; set; }
        public string error_description { get; set; }
    }
}