using Gateway.Logger;
using System.Threading.Tasks;
using System.Timers;

namespace PaymentGateway.methods.timers
{
    public class updateMyQ
    {
        private static System.Timers.Timer aTimer;

        internal static async Task StartTimer()
        {
            //900000
            aTimer = new System.Timers.Timer(60000);
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Enabled = true;
            aTimer.AutoReset = true;
        }

        private static async void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            aTimer.Enabled = false;

            MyLogger.GetInstance().Info("Verifying database integrity...");
            await data.db.setupDatabase();

            await recharger.AuthenticationPack();
        }
    }
}