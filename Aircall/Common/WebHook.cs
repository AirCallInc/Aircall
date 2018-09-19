using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aircall.Common.Stripe
{
    public class Metadata
    {
    }

    public class Period
    {

        [JsonProperty("start")]
        public int start { get; set; }

        [JsonProperty("end")]
        public int end { get; set; }
    }

    public class Plan
    {

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("object")]
        public string object1 { get; set; }

        [JsonProperty("amount")]
        public int amount { get; set; }

        [JsonProperty("created")]
        public int created { get; set; }

        [JsonProperty("currency")]
        public string currency { get; set; }

        [JsonProperty("interval")]
        public string interval { get; set; }

        [JsonProperty("interval_count")]
        public int interval_count { get; set; }

        [JsonProperty("livemode")]
        public bool livemode { get; set; }

        [JsonProperty("metadata")]
        public Metadata metadata { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("statement_descriptor")]
        public object statement_descriptor { get; set; }

        [JsonProperty("trial_period_days")]
        public object trial_period_days { get; set; }
    }

    public class Datum
    {

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("object")]
        public string object1 { get; set; }

        [JsonProperty("amount")]
        public int amount { get; set; }

        [JsonProperty("currency")]
        public string currency { get; set; }

        [JsonProperty("description")]
        public object description { get; set; }

        [JsonProperty("discountable")]
        public bool discountable { get; set; }

        [JsonProperty("livemode")]
        public bool livemode { get; set; }

        [JsonProperty("metadata")]
        public Metadata metadata { get; set; }

        [JsonProperty("period")]
        public Period period { get; set; }

        [JsonProperty("plan")]
        public Plan plan { get; set; }

        [JsonProperty("proration")]
        public bool proration { get; set; }

        [JsonProperty("quantity")]
        public int quantity { get; set; }

        [JsonProperty("subscription")]
        public object subscription { get; set; }

        [JsonProperty("type")]
        public string type { get; set; }
    }

    public class Lines
    {

        [JsonProperty("object")]
        public string object1 { get; set; }

        [JsonProperty("data")]
        public IList<Datum> data { get; set; }

        [JsonProperty("has_more")]
        public bool has_more { get; set; }

        [JsonProperty("total_count")]
        public int total_count { get; set; }

        [JsonProperty("url")]
        public string url { get; set; }
    }

    public class Object
    {

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("object")]
        public string object1 { get; set; }

        [JsonProperty("amount_due")]
        public int amount_due { get; set; }

        [JsonProperty("application_fee")]
        public object application_fee { get; set; }

        [JsonProperty("attempt_count")]
        public int attempt_count { get; set; }

        [JsonProperty("attempted")]
        public bool attempted { get; set; }

        [JsonProperty("charge")]
        public string charge { get; set; }

        [JsonProperty("closed")]
        public bool closed { get; set; }

        [JsonProperty("currency")]
        public string currency { get; set; }

        [JsonProperty("customer")]
        public string customer { get; set; }

        [JsonProperty("date")]
        public int date { get; set; }

        [JsonProperty("description")]
        public object description { get; set; }

        [JsonProperty("discount")]
        public object discount { get; set; }

        [JsonProperty("ending_balance")]
        public int ending_balance { get; set; }

        [JsonProperty("forgiven")]
        public bool forgiven { get; set; }

        [JsonProperty("lines")]
        public Lines lines { get; set; }

        [JsonProperty("livemode")]
        public bool livemode { get; set; }

        [JsonProperty("metadata")]
        public Metadata metadata { get; set; }

        [JsonProperty("next_payment_attempt")]
        public object next_payment_attempt { get; set; }

        [JsonProperty("paid")]
        public bool paid { get; set; }

        [JsonProperty("period_end")]
        public int period_end { get; set; }

        [JsonProperty("period_start")]
        public int period_start { get; set; }

        [JsonProperty("receipt_number")]
        public object receipt_number { get; set; }

        [JsonProperty("starting_balance")]
        public int starting_balance { get; set; }

        [JsonProperty("statement_descriptor")]
        public object statement_descriptor { get; set; }

        [JsonProperty("subscription")]
        public string subscription { get; set; }

        [JsonProperty("subtotal")]
        public int subtotal { get; set; }

        [JsonProperty("tax")]
        public object tax { get; set; }

        [JsonProperty("tax_percent")]
        public object tax_percent { get; set; }

        [JsonProperty("total")]
        public int total { get; set; }

        [JsonProperty("webhooks_delivered_at")]
        public object webhooks_delivered_at { get; set; }
    }

    public class Data
    {

        [JsonProperty("object")]
        public Object object1 { get; set; }
    }

    public class WebHook
    {

        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("object")]
        public string object1 { get; set; }

        [JsonProperty("api_version")]
        public string api_version { get; set; }

        [JsonProperty("created")]
        public int created { get; set; }

        [JsonProperty("data")]
        public Data data { get; set; }

        [JsonProperty("livemode")]
        public bool livemode { get; set; }

        [JsonProperty("pending_webhooks")]
        public int pending_webhooks { get; set; }

        [JsonProperty("request")]
        public string request { get; set; }

        [JsonProperty("type")]
        public string type { get; set; }
    }

}