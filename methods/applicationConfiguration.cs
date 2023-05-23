using ApiTest.Logger;
using Gateway.Logger;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using PaymentGateway.exceptions;
using Formatting = Newtonsoft.Json.Formatting;
using System.Xml;
using System.Windows.Forms;

namespace PaymentGateway.methods
{
    public class applicationConfiguration
    {
        public static applicationConfiguration Credentials = new applicationConfiguration();
        public string ____________________________Please_Enter_Your_MyQ_Server_Details_Below____________________________ { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string ServerAddress { get; set; }
        public string ____________________________Please_Enter_Your_ParentPay_Details_Below____________________________ { get; set; }
        public string OrgIdentifier { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public long ServiceID { get; set; }
        public string ____________________________Please_Enter_Your_Organisation_Name_Below____________________________ { get; set; }
        public string InstallationName { get; set; }
        public string ____________________________Please_Fill_In_Security_Info_Below____________________________ { get; set; }
        public decimal TLSVersion { get; set; }
        public string Certificate_DistinguishedName { get; set; }
        public string _______________________Active_Directory_Authentication_Details_______________________ { get; set; }
        public string ADName { get; set; }
        public string ParentPayIDAttribute { get; set; }
        public string _________________________Please_Enter_Your_AzureAD_ClietID_AND_Authority_Below_________________________ { get; set; }
        public string AzureClientID { get; set; }
        public string AzureTenantID { get; set; }
        public string AzureClientSecret { get; set; }
        public string AzureCustomAttribute { get; set; }
        public string ____________________________Please_Enter_Your_Logging_and_SMTP_Config_Below____________________________ { get; set; }
        public bool DebugEnabled { get; set; }
        public string smtpServer { get; set; }
        public int smtpPort { get; set; }
        public string smtpUsername { get; set; }
        public string smtpPassword { get; set; }
        public string smtpFromAddress { get; set; }
        public string smtpToAddress { get; set; }
        public bool UseSSL { get; set; }
        public string emailSubject { get; set; }
        public string ____________________________Please_Dont_touch_unless_instructed____________________________ { get; set; }
        public string[] whitelistedAddresses { get; set; }

        public IEnumerator GetEnumerator()
        {
            return (Credentials as IEnumerable).GetEnumerator();
        }
    }
    public class configurationLoader
    {
        private static string config1File = @"./config/config.json";

        public static async Task LoadDetails()
        {
            if (!Directory.Exists("./config"))
            {
                MessageBox.Show("Error! The application has not yet been configured.\n\n Please close the application and configure the 'config\\config.json' file then proceed to run again...", "Applicaion Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            MyLogger.GetInstance().Debug("ConfigManager creating payload instance");

            applicationConfiguration config1 = new applicationConfiguration();

            try
            {
                MyLogger.GetInstance().Debug("ConfigManager locating configuration file");

                if (File.Exists(config1File))
                {
                    MyLogger.GetInstance().Debug("ConfigManager configuration file found!");
                    MyLogger.GetInstance().Debug("ConfigManager reading configuration file");

                    using (StreamReader file = File.OpenText(config1File))
                    {
                        MyLogger.GetInstance().Debug("ConfigManager deserializing payload");

                        var payload = JsonConvert.DeserializeObject<applicationConfiguration>(File.ReadAllText(config1File));
                        MyLogger.GetInstance().Info("ConfigFileLoaded {0}", config1File);

                        MyLogger.GetInstance().Debug("ConfigManager assigning payload");

                        //Loads deserialized JSON into static class.
                        applicationConfiguration.Credentials = payload;

                        MyLogger.GetInstance().Info("DataReadFromFile {0}", payload);

                        MyLogger.GetInstance().Debug("ConfigManager getting ip address");

                        await loggerConfig.SetupNLog();

                        MyLogger.GetInstance().Info("ServerAddress, {0}", networkManagement.HostToIp(applicationConfiguration.Credentials.ServerAddress).Result);
                    }
                }

                else if (!File.Exists(config1File))
                {
                    MyLogger.GetInstance().Debug("ConfigManager configuration file not found!");
                    MyLogger.GetInstance().Debug("ConfigManager converting payload");

                    string currentPath = Directory.GetCurrentDirectory();

                    if (!Directory.Exists(currentPath + "/config"))
                    {
                        Directory.CreateDirectory(currentPath + "/config");
                    }

                    File.WriteAllText(config1File, JsonConvert.SerializeObject(config1));
                    MyLogger.GetInstance().Info("File: {0}", config1File);

                    MyLogger.GetInstance().Debug("ConfigManager creating configuration file");

                    using (StreamWriter file = File.CreateText(config1File))
                    {
                        MyLogger.GetInstance().Debug("ConfigManager creating serializer");

                        JsonSerializer serializer = new JsonSerializer();

                        MyLogger.GetInstance().Debug("ConfigManager configuring serializer");

                        serializer.Formatting = Formatting.Indented;

                        config1.whitelistedAddresses = new string[] { "PLEASE REPLACE ME WITH YOUR MYQ SERVER ADDRESS", "45.60.65.76", "45.60.72.226", "45.60.65.226" };

                        MyLogger.GetInstance().Debug("ConfigManager serializing payload to configuration file");

                        serializer.Serialize(file, config1);
                    }

                    Console.Clear();
                    Console.WriteLine("\n\n" + "Error loading config file! - Please ensure the information provided is valid and try again!");
                }
            }

            catch (Exception ex)
            {
                var exception = new ApplicationConfiguration(ex.ToString());
                MyLogger.GetInstance().Error("Error: ", exception);
            }
        }
    }

    public class myqConfiguration
    {
        public static myqConfiguration MyQ = new myqConfiguration();
        public int AuthenticationType { get { return 2; } set { } }
        public string GrantType { get { return "client_credentials"; } set { } }
        public string Scope
        {
            get
            {
                return "credit users";

            }
            set { }
        }

        public string AutomaticType
        {
            get
            {
                return "pin";
            }
            set { }
        }
        public string reference
        {
            get
            {
                return "external";
            }
            set { }
        }
        public decimal UserNewBalance { get; set; }
        public string Token { get; set; }
        public string Url { get; set; }
        public decimal Amount { get; set; }
        public string UserID { get; set; }
        public string Username { get; set; }
        public string PaymentID { get; set; }
        public decimal PaymentAmount { get; set; }
        public string TLS { get; set; }
        public int TokenExpire { get; set; }
        public string reqUsername { get; set; }
        public string reqCode { get; set; }
        public int reqAzureCID
        {
            get
            {
                var returnValue = 0;

                if (applicationConfiguration.Credentials.AzureClientID != null && applicationConfiguration.Credentials.AzureClientSecret != null && applicationConfiguration.Credentials.AzureCustomAttribute != null && applicationConfiguration.Credentials.AzureTenantID != null)
                    returnValue = 1;

                return returnValue;
            }

            set { }
        }
        public string TokenAuth_Body
        {
            get
            {
                return "{\"grant_type\":\"" + myqConfiguration.MyQ.GrantType + "\",\"scope\":\"" + myqConfiguration.MyQ.Scope + "\",\"client_id\":\"" + applicationConfiguration.Credentials.ClientID + "\",\"client_secret\":\"" + applicationConfiguration.Credentials.ClientSecret + "\"}";

            }
            set { }
        }

        public string RechargeRequest_Body
        {
            get
            {
                return "{\"userId\": " + Convert.ToInt32(UserID) + " ,\"description\": \"ParentPay Transaction\",\"params\": {\"amount\": " + Amount + " }}";
            }
        }
    }

    public class parentpayConfiguration
    {
        public static parentpayConfiguration ParentPay = new parentpayConfiguration();
        public int UserIdentifier { get; set; }
        public XmlDocument XmlBody = new XmlDocument();
        public int IncrementThroughUsers { get; set; }
        public int GlobalNum { get; set; }
        public string handleSimplePaymentReport { get { return "http://www.parentpay.com/P247WS/PubMethods/handleSimplePaymentReport"; } }
        public string handleMessageUpdateRequest { get { return "http://www.parentpay.com/P247WS/PubMethods/handleMessageUpdateRequest"; } }
    }

    public class iterationConfig
    {
        public static iterationConfig Other = new iterationConfig();
        public int GlobalVariable { get; set; }
    }
}
