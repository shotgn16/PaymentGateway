using Gateway.Logger;
using PaymentGateway.request;
using PaymentGateway.request.response;
using System.Threading.Tasks;
using System.Timers;

namespace PaymentGateway.methods.timers
{
    public class generateToken
    {
        private static System.Timers.Timer aTimer;

        internal static async Task AuthRefresh()
        {
            //Interval is in seconds by default (MyQ Server) - Converting it to milliseconds then taking away 5 minutes to cause token refresh every 25 minutes.
            int msInterval = myqConfiguration.MyQ.TokenExpire * 1000 - 300000;

            aTimer = new System.Timers.Timer(30000);
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent2);
            aTimer.Enabled = true;
        }

        private static async void OnTimedEvent2(object source, ElapsedEventArgs e)
        {
            MyLogger.GetInstance().Info("Authentication Generating New Token");
            await restUtility.root1Parse(await flurlRest.getToken().Result.ResponseMessage.Content.ReadAsStringAsync());
        }
    }
}