using Gateway.Logger;
using PaymentGateway.request;
using PaymentGateway.request.response;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace PaymentGateway.methods.timers
{
    public class generateToken
    {
        private static System.Timers.Timer aTimer;
        private static bool isActive = false;

        internal static async Task AuthRefresh()
        {
            if (!isActive) //Updated every 25 minutes
            {
                int msInterval = internalConfig.internalConfiguration.TokenExpire * 1000 - 300000;
                aTimer = new System.Timers.Timer(msInterval);
                aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent2);
                isActive = true;
                aTimer.Start();

                MyLogger.GetInstance().Info("Starting Timer #001");
            }

            else if (isActive)
            {
                MyLogger.GetInstance().Info("Timer #001 Active! Skipping activation...");
            }
        }

        private static async void OnTimedEvent2(object source, ElapsedEventArgs e)
        {
            aTimer.Stop();
            isActive = false;
            
            MyLogger.GetInstance().Info("Timer, ID: #001 Complete!");
            MyLogger.GetInstance().Info("Authentication Generating New Token");
            await restUtility.root1Parse(await flurlRest.getToken().Result.ResponseMessage.Content.ReadAsStringAsync());

            //Clean up resources
            GC.Collect();
        }

        public static void Dispose()
        {
            aTimer.Dispose();
            GC.Collect();
        }
    }
}