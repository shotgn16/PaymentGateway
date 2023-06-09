using System;
using System.Threading.Tasks;
using System.Timers;
using Gateway.Logger;
using PaymentGateway.request.response;
using PaymentGateway.request;
using PaymentGateway.Properties;

namespace PaymentGateway.methods.timers
{
    public class updateParentPay
    {
        //Default to 7am - 6pm
        private static System.Timers.Timer uTimer;
        private static bool isActive = false;
        public static xmlUtility.HandleSimplePaymentReportResult xml;

        public static async Task newTillBalance(xmlUtility.HandleSimplePaymentReportResult XML)
        {
            xml = XML;

            if (!isActive) //Updated every 15 minutes
            {
                //10 Minutes
                uTimer = new System.Timers.Timer(Settings.Default.parentpayTimer);
                uTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent2);
                isActive = true;
                uTimer.Start();

                MyLogger.GetInstance().Info("Starting Timer #002");
            }

            else if (isActive)
            {
                MyLogger.GetInstance().Info("Timer #002 Active! Skipping activation...");
            }
        }

        internal static async void OnTimedEvent2(object source, ElapsedEventArgs e)
        {
            uTimer.Stop();
            isActive = false;

            var xmlResponse = (xmlUtility.HandleMessageUpdateRequestResult)await soap.httpSoap("handleMessageUpdateRequest");

            //Clearing the response data from memory as its not needed...
            xmlResponse = null;
            GC.Collect();
        }

        public static void Dispose()
        {
            uTimer.Dispose();
            xml = null;
            GC.Collect();
        }
    }
}