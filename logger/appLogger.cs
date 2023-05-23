using ApiTest.Logger;
using NLog;
using NLog.Targets;
using PaymentGateway.data;
using System;
using System.IO;
using System.Security;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace Gateway.Logger
{
    internal class MyLogger : ILogger
    {
        //Singleton pattern design
        private static MyLogger instance;
        private static NLog.Logger logger;
        private MyLogger() { }
        public static MyLogger GetInstance()
        {
            if (instance == null)
                instance = new MyLogger();
            return instance;
        }

        private NLog.Logger GetLogger(string theLogger)
        {
            if (MyLogger.logger == null)
                MyLogger.logger = LogManager.GetLogger(theLogger);

            return MyLogger.logger;
        }

        public void Debug(string message, object arg = null)
        {
            GetLogger("appLogger").Debug(message, arg);
        }

        public void Error(string message, object arg = null)
        {
            if (arg == null)
            {
                GetLogger("errorLogger").Error(message);
            }
            else
            {
                GetLogger("errorLogger").Error(message, arg);
            }
        }

        public void Info(string message, object arg = null)
        {
            GetLogger("appLogger").Info(message, arg);
        }

        public void Warning(string message, object arg = null)
        {
            if (arg == null)
            {
                GetLogger("errorLogger").Warn(message);
            }
            else
            {
                GetLogger("errorLogger").Warn(message, arg);
            }
        }
    }

    public class crashLogger
    {
        public static async void OnException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;

            using (StreamWriter file = File.AppendText(@"logs\appCrash.log"))
            {
                file.WriteLine("\n" + System.DateTime.Now.ToString() + "\n" + e.Message + "\n" + args.IsTerminating + "\n" + e.StackTrace + "\n" + e.Source);
            }

            await supportData.generateDataFile();
        }
    }
}
