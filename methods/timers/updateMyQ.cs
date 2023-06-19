using Gateway.Logger;
using PaymentGateway.Properties;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace PaymentGateway.methods.timers
{
    public class updateMyQ
    {
        private static System.Timers.Timer aTimer;
        private static bool isActive = false;

        internal static async Task StartTimer()
        {
            
            if (!isActive) //Updated every 13 minutes
            {
                //5 Minutes
                aTimer = new System.Timers.Timer(Settings.Default.myqTimer);
                aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                isActive = true;
                aTimer.Start();

                MyLogger.GetInstance().Info("Starting Timer #003");
            }

            else if (isActive) {
                MyLogger.GetInstance().Info("Timer #003 Active! Skipping activation...");
            }
        }

        private static async void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            aTimer.Stop();
            isActive = false;

            MyLogger.GetInstance().Info("Timer, ID: #002 Complete!");

            MyLogger.GetInstance().Info("Verifying database integrity...");
            await data.db.setupDatabase();

            var startAuth = Task.Run(async () => { await recharger.AuthenticationPack(); });
            startAuth.Wait();

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