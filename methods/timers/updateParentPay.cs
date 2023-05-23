using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using PaymentGateway.methods;
using Gateway.Logger;
using NLog;
using Org.BouncyCastle.Security;
using PaymentGateway.request.response;
using PaymentGateway.request;

namespace PaymentGateway.methods.timers
{
    public class updateParentPay
    {
        //Default to 7am - 6pm
        private static System.Timers.Timer uTimer;
        public static xmlUtility.HandleSimplePaymentReportResult xml;

        public static async Task newTillBalance(xmlUtility.HandleSimplePaymentReportResult XML)
        {
            xml = XML;

            uTimer = new System.Timers.Timer(60000);
            uTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent2);
            uTimer.Enabled = true;
            uTimer.Stop();
        }

        internal static async void OnTimedEvent2(object source, ElapsedEventArgs e)
        {
            MyLogger.GetInstance().Info("Updating Till balance");

            var xmlResponse = (xmlUtility.HandleMessageUpdateRequestResult)await soap.httpSoap("handleMessageUpdateRequest");

        }
    }
}