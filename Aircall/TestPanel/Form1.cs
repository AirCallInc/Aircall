using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNetLib;

namespace TestPanel
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnCreateCustomerProfile_Click(object sender, EventArgs e)
        {
            try
            {
                string ApiLoginID = "6Mj5Z6aMh";
                string ApiTransactionKey = "3q85g7TS43U6qNUu";
                bool isSandBox = true;
                var helper = new AuthorizeNetHelper(ApiLoginID, ApiTransactionKey, isSandBox);
                string email = this.textBox1.Text, customerId = "CUS_2018_00001", customerProfileId = "", errCode = "", errText = "";
                bool isSuccess = false;
                customerId = "";
                helper.CreateCustomerProfile(email, "Test123", customerId, ref isSuccess, ref customerProfileId, ref errCode, ref errText);
                MessageBox.Show("customerProfileId is " + customerProfileId);
                MessageBox.Show("Done");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAddAddress_Click(object sender, EventArgs e)
        {
            customerAddressType address = new customerAddressType();
            address.firstName = "Chris";
            address.lastName = "brown";
            address.address = "1200 148th AVE NE";
            address.city = "NorthBend";
            address.zip = "92101";

            string ApiLoginID = "6Mj5Z6aMh";
            string ApiTransactionKey = "3q85g7TS43U6qNUu";
            bool isSandBox = true;
            var helper = new AuthorizeNetHelper(ApiLoginID, ApiTransactionKey, isSandBox);
            string customerProfileId = "1504706322";
            string errCode = "", errText = "";
            string customerAddressId = "";
            bool isSuccess = false;
            helper.CreateCustomerAddress(customerProfileId, address, ref isSuccess, ref customerAddressId, ref errCode, ref errText);
            MessageBox.Show("Done");
        }

        private void btnUpdateAddress_Click(object sender, EventArgs e)
        {
            var address = new customerAddressExType
            {
                firstName = "Newfirstname",
                lastName = "Doe",
                address = "123 Main St.",
                city = "Bellevue",
                state = "WA",
                zip = "98004",
                country = "USA",
                phoneNumber = "000-000-000",
                customerAddressId = "1871993045"
            };

            string ApiLoginID = "6Mj5Z6aMh";
            string ApiTransactionKey = "3q85g7TS43U6qNUu";
            bool isSandBox = true;
            var helper = new AuthorizeNetHelper(ApiLoginID, ApiTransactionKey, isSandBox);
            string customerProfileId = "1504706322";
            string errCode = "", errText = "";
            bool isSuccess = false;
            helper.UpdateCustomerAddress(customerProfileId, address, ref isSuccess, ref errCode, ref errText);
            MessageBox.Show("Done");
        }

        private void btnCreateSubscription_Click(object sender, EventArgs e)
        {
            paymentScheduleTypeInterval interval = new paymentScheduleTypeInterval();

            interval.length = 1;
            interval.unit = ARBSubscriptionUnitEnum.months;

            paymentScheduleType schedule = new paymentScheduleType
            {
                interval = interval,
                startDate = DateTime.Now.AddDays(1),
                totalOccurrences = 12,
            };

            string ApiLoginID = "6Mj5Z6aMh";
            string ApiTransactionKey = "3q85g7TS43U6qNUu";
            bool isSandBox = true;
            var helper = new AuthorizeNetHelper(ApiLoginID, ApiTransactionKey, isSandBox);

            string customerProfileId = "1504706322";
            string customerAddressId = "1871993045";
            string customerPaymentProfileId = "1504009290";
            string errCode = "", errText = "";
            bool isSuccess = false;
            string subscriptionId = "";

            helper.CreateSubscriptionFromCustomerProfile(customerProfileId, customerPaymentProfileId, customerAddressId, schedule, 35.55m, ref isSuccess, ref subscriptionId, ref errCode, ref errText);
            MessageBox.Show("Done");
        }

        private void btnCreatePaymentProfile_Click(object sender, EventArgs e)
        {
            var creditCard = new creditCardType
            {
                cardNumber = "4111111111111111",
                expirationDate = "0720",
                cardCode = "012",
            };

            paymentType echeck = new paymentType { Item = creditCard };

            var billTo = new customerAddressType
            {
                firstName = "John",
                lastName = "Snow"
            };

            string ApiLoginID = "6Mj5Z6aMh";
            string ApiTransactionKey = "3q85g7TS43U6qNUu";
            bool isSandBox = true;
            var helper = new AuthorizeNetHelper(ApiLoginID, ApiTransactionKey, isSandBox);

            string customerProfileId = "1504706322";
            string customerPaymentProfileId = "";
            string errCode = "", errText = "";
            bool isSuccess = false;
            helper.CreateCustomerPaymentProfile(customerProfileId, echeck, billTo, ref isSuccess, ref customerPaymentProfileId, ref errCode, ref errText);
            MessageBox.Show("Done");
        }

        private void btnChargeCustomerProfile_Click(object sender, EventArgs e)
        {
            string ApiLoginID = "6Mj5Z6aMh";
            string ApiTransactionKey = "3q85g7TS43U6qNUu";
            bool isSandBox = true;
            var helper = new AuthorizeNetHelper(ApiLoginID, ApiTransactionKey, isSandBox);

            string customerProfileId = "1504706322";
            string customerPaymentProfileId = "1504009290";
            string errCode = "", errText = "";
            bool isSuccess = false;
            string transId = "";
            string authCode = "";

            helper.ChargeCustomerProfile(customerProfileId, customerPaymentProfileId, 50.02m, ref isSuccess, ref transId, ref authCode, ref errCode, ref errText);
            MessageBox.Show("Done");
        }

        private void btnUpdateSubscription_Click(object sender, EventArgs e)
        {
            paymentScheduleTypeInterval interval = new paymentScheduleTypeInterval();

            interval.length = 1;
            interval.unit = ARBSubscriptionUnitEnum.months;

            string ApiLoginID = "6Mj5Z6aMh";
            string ApiTransactionKey = "3q85g7TS43U6qNUu";
            bool isSandBox = true;
            var helper = new AuthorizeNetHelper(ApiLoginID, ApiTransactionKey, isSandBox);

            string customerProfileId = "1504706322";
            string customerAddressId = "1871993045";
            string customerPaymentProfileId = "1504009290";
            string errCode = "", errText = "";
            bool isSuccess = false;
            string subscriptionId = "5225374";
            string refId = "";

            helper.UpdateSubscription(subscriptionId, customerProfileId, customerPaymentProfileId, customerAddressId, 45.55m, ref isSuccess, ref refId, ref errCode, ref errText);
            MessageBox.Show("Done");
        }

        private void btnGetSubscription_Click(object sender, EventArgs e)
        {
            string ApiLoginID = "6Mj5Z6aMh";
            string ApiTransactionKey = "3q85g7TS43U6qNUu";
            bool isSandBox = true;
            var helper = new AuthorizeNetHelper(ApiLoginID, ApiTransactionKey, isSandBox);
            string errCode = "", errText = "";
            bool isSuccess = false;
            var res = helper.GetSubscriptionById("5245350", ref isSuccess, ref errCode, ref errText);
            MessageBox.Show("Done");
        }

        private void btnGetTransaction_Click(object sender, EventArgs e)
        {
            string ApiLoginID = "6Mj5Z6aMh";
            string ApiTransactionKey = "3q85g7TS43U6qNUu";
            bool isSandBox = true;
            var helper = new AuthorizeNetHelper(ApiLoginID, ApiTransactionKey, isSandBox);
            string errCode = "", errText = "";
            bool isSuccess = false;
            var res = helper.GetCustomerProfileTransactionList("1504801452", ref isSuccess, ref errCode, ref errText);
            MessageBox.Show("Done");
        }

        private void btnGetSubscriptionTransaction_Click(object sender, EventArgs e)
        {
            string ApiLoginID = "6Mj5Z6aMh";
            string ApiTransactionKey = "3q85g7TS43U6qNUu";
            bool isSandBox = true;
            var helper = new AuthorizeNetHelper(ApiLoginID, ApiTransactionKey, isSandBox);
            string errCode = "", errText = "";
            bool isSuccess = false;
            var res = helper.GetSubscriptionTransactions("5245350", ref isSuccess, ref errCode, ref errText);
            MessageBox.Show("Done");
        }

        private void btnStartService_Click(object sender, EventArgs e)
        {
            var s = new AuthorizeNetScheduler.AuthorizeNetService();
            s.StartWork();
        }
    }
}
