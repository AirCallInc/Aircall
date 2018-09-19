using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace api.Common
{
    public class Notifications
    {
        public long NId { get; set; }
        public int NType { get; set; }
        public long CommonId { get; set; }
    }
    public class NotificationModel
    {
        public string Key { get; set; }
        public object[] Value { get; set; }
    }
}