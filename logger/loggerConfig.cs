using NLog;
using NLog.Config;
using NLog.Layouts;
using PaymentGateway.methods;
using System.Threading.Tasks;

namespace ApiTest.Logger
{
    public class loggerConfig
    {
        //Class-Wide instance of NLog Logger
        public static LoggingConfiguration config = new NLog.Config.LoggingConfiguration();

        private static Layout jsonLogLayout()
        {
            var layout = new JsonLayout
            {
                Attributes =
               {
                   new JsonAttribute("time", "${longdate}"),
                   new JsonAttribute("thread", "${threadid}"),
                   new JsonAttribute("level", "${level:upperCase=true}"),
                   new JsonAttribute("type", "${exception:format=Type}"),
                   new JsonAttribute("message", "${message}"),
                   new JsonAttribute("exception", new JsonLayout
                   {
                       Attributes =
                       {
                           new JsonAttribute("type", "${exception:format=:innerFormat=Type:MaxInnerExceptionLevel=1:InnerExceptionSeparator=}"),
                           new JsonAttribute("message", "${exception:format=:Type}"),
                           new JsonAttribute("version", "${assembly-version}"),
                           new JsonAttribute("properties", "${all-event-properties}")
                       },
                       RenderEmptyObject = false
                   },

                   false),

                   new JsonAttribute("eventProperties", "${all-event-properties}")
               }
            };

            return layout;
        }

        private static async Task<NLog.LogLevel> logDebug(NLog.LogLevel level = null)
        {
            if (applicationConfiguration.Credentials.DebugEnabled == true)
                level = NLog.LogLevel.Debug;

            else if (applicationConfiguration.Credentials.DebugEnabled == false)
                level = NLog.LogLevel.Info;

            return level;
        }

        public static async Task SetupNLog()
        {
            var eMail = new NLog.Targets.MailTarget("eMail")
            {
                Layout = jsonLogLayout(),
                SmtpAuthentication = NLog.Targets.SmtpAuthenticationMode.Basic,
                SmtpServer = applicationConfiguration.Credentials.smtpServer,
                SmtpPort = applicationConfiguration.Credentials.smtpPort,
                SmtpUserName = applicationConfiguration.Credentials.Username,
                SmtpPassword = applicationConfiguration.Credentials.smtpPassword,
                EnableSsl = applicationConfiguration.Credentials.UseSSL,
                Subject = applicationConfiguration.Credentials.emailSubject,
                From = applicationConfiguration.Credentials.smtpFromAddress,
                To = applicationConfiguration.Credentials.smtpToAddress,
                Body = "A Warning or Error has occurred on the PaymentGateway. Please see the attached for details. ${newline}${newline}${longdate} ${level:upperCase=true} ${message}${newline}"
            };

            var configuration = LogManager.Configuration;
            configuration.AddTarget(eMail);
            configuration.AddRule(NLog.LogLevel.Warn, NLog.LogLevel.Fatal, eMail);
            LogManager.ReconfigExistingLoggers();
        }
    }
}