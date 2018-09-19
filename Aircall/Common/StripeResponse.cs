using Stripe;
using System;

namespace Aircall.Common
{
    public class StripeResponse
    {
        public string TransactionId { get; set; }
        public string PaymentStatus { get; set; }
        public StripeException ex { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime StripeNextPaymentDate { get; set; }
    }
}