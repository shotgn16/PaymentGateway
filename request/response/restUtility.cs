using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gateway.Logger;
using PaymentGateway.exceptions;
using PaymentGateway.methods;
using PaymentGateway.methods.timers;

namespace PaymentGateway.request.response
{
    public class Root1 /* api/auth/token */
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
    }

    public class Account
    {
        public string name { get; set; }
        public string reference { get; set; }
        public string type { get; set; }
        public Limits limits { get; set; }
        public List<RechargeProvider> rechargeProviders { get; set; }
        public int? groupId { get; set; }
    }

    public class Limits
    {
        public decimal price { get; set; }
        public int? total { get; set; }
        public int? color { get; set; }
        public int? mono { get; set; }
        public int? scan { get; set; }
        public int? denyAccess { get; set; }
    }

    public class Policies
    {
        public string accessLevel { get; set; }
        public bool copy { get; set; }
        public bool copyColor { get; set; }
        public bool fax { get; set; }
        public bool forceBW { get; set; }
        public bool forceDuplex { get; set; }
        public bool forceTonerSave { get; set; }
        public bool print { get; set; }
        public bool scan { get; set; }
        public bool allowJobHistory { get; set; }
        public bool allowJobModification { get; set; }
        public bool allowRemoteJobs { get; set; }
    }

    public class RechargeProvider
    {
        public string name { get; set; }
        public string reference { get; set; }
    }

    public class Root
    {
        public List<User> users { get; set; }
        public int count { get; set; }
    }

    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string fullname { get; set; }
        public string email { get; set; }
        public string scanStorage { get; set; }
        public string code { get; set; }
        public string syncSource { get; set; }
        public string lang { get; set; }
        public string phone { get; set; }
        public string ldapDomain { get; set; }
        public string notes { get; set; }
        public List<string> aliases { get; set; }
        public List<string> delegates { get; set; }
        public Policies policies { get; set; }
        public List<Account> accounts { get; set; }
        public List<string> permissions { get; set; }
    }

    public class Balance2
    {
        public decimal amount { get; set; }
    }

    public class Root2
    {
        public Balance2 balance { get; set; }
    }

    public class Payment3 /* /v3/rechargeProviders/{reference}/payments */
    {
        public string id { get; set; }
        public int userId { get; set; }
        public decimal amount { get; set; }
        public object currency { get; set; }
        public string state { get; set; }
        public DateTime created { get; set; }
        public DateTime updated { get; set; }
        public object expires { get; set; }
        public string paymentNo { get; set; }
        public string provider { get; set; }
        public string description { get; set; }
    }

    public class Root3
    {
        public List<Payment3> payments { get; set; }
        public int count { get; set; }
    }

    public class Provider4 /* /v3/rechargeProviders */
    {
        public string name { get; set; }
        public string reference { get; set; }
        public string description { get; set; }
        public List<object> @params { get; set; }
    }

    public class Root4
    {
        public List<Provider4> providers { get; set; }
        public int count { get; set; }
    }

    public class Params5 /* /v3/rechargeProviders/{reference}/payments */
    {
        public decimal amount { get; set; }
        public int createdBy { get; set; }
    }

    public class Root5
    {
        public string id { get; set; }
        public int userId { get; set; }
        public decimal amount { get; set; }
        public string description { get; set; }
        public Params5 @params { get; set; }
    }

    public class Root6
    {
        public decimal amount { get; set; }
    }

    // api/v3/users/{userid}}/accounts
    public class Account7
    {
        public string name { get; set; }
        public string reference { get; set; }
        public string type { get; set; }
        public Limits limits { get; set; }
        public List<RechargeProvider7> rechargeProviders { get; set; }
        public int? groupId { get; set; }
    }

    public class Credit7
    {
        public bool enabled { get; set; }
    }

    public class Limits7
    {
        public int price { get; set; }
        public int? total { get; set; }
        public int? color { get; set; }
        public int? mono { get; set; }
        public int? scan { get; set; }
        public int? denyAccess { get; set; }
    }

    public class Quota7
    {
        public bool enabled { get; set; }
    }

    public class RechargeProvider7
    {
        public string name { get; set; }
        public string reference { get; set; }
    }

    public class Root7
    {
        public Credit7 credit { get; set; }
        public Quota7 quota { get; set; }
        public List<Account7> accounts { get; set; }
    }


    public class restUtility
    {
        public static async Task root1Parse(string Input)
        {
            try
            {
                MyLogger.GetInstance().Debug("Creating object class");

                Root1 myClass = JsonConvert.DeserializeObject<Root1>(Input);

                MyLogger.GetInstance().Debug("Checking class values");

                if (myClass.access_token == null)
                {
                    new DeserializationException("An error occurred while authenticating with MyQ");
                }

                myqConfiguration.MyQ.Token = myClass.access_token;
                myqConfiguration.MyQ.TokenExpire = myClass.expires_in;

                Task.Factory.StartNew(async () => { await generateToken.AuthRefresh(); });

                MyLogger.GetInstance().Info("Deserialization Complete, Access Token: {0}", await data.DatabaseHash.dbEncrypt(myClass.access_token, 2));
            }

            catch (Exception ex)
            {
                var exception = new DeserializationException(ex.Message);
                MyLogger.GetInstance().Error("Error: ", exception);
            }
        }

        public static async Task rootParse(string Input)
        {
            try
            {
                MyLogger.GetInstance().Debug("Creating object class");

                Root myClass = JsonConvert.DeserializeObject<Root>(Input);

                MyLogger.GetInstance().Debug("Checking class values");

                if (myClass.users[0].id.ToString() == null)
                {
                    var exception = new DeserializationException("an Error occurred while deserializing user data from MyQ");
                    MyLogger.GetInstance().Error("Error: ", exception);
                }

                myqConfiguration.MyQ.UserID = myClass.users[0].id.ToString();
                myqConfiguration.MyQ.Username = myClass.users[0].username;
                MyLogger.GetInstance().Info("Deserialization Complete, UserID: {0}", await data.DatabaseHash.dbEncrypt(myClass.users[0].id.ToString(), 2));
            }

            catch (Exception ex)
            {
                var exception = new DeserializationException(ex.Message);
                MyLogger.GetInstance().Error("Error: ", exception);
            }
        }

        public static async Task root3Parse(string Input)
        {
            try
            {
                MyLogger.GetInstance().Debug("Creating object class");

                Root3 myClass = JsonConvert.DeserializeObject<Root3>(Input);

                MyLogger.GetInstance().Debug("Checking user payments");

                //Validates if the latest payment is newer than the previous
                bool result = await restValidation.CheckPayments(myClass);

                if (result == false)
                {
                    MyLogger.GetInstance().Debug("PaymentValidator, No Payments detected for user");
                }

                else if (result == true)
                {
                    MyLogger.GetInstance().Debug("PaymentValidator, Payments found!");
                }

                MyLogger.GetInstance().Info("Deserialization Complete, Payments: {0}", await data.DatabaseHash.dbEncrypt(myClass.count.ToString(), 2));
            }

            catch (Exception ex)
            {
                var exception = new DeserializationException(ex.Message);
                MyLogger.GetInstance().Error("Error: ", exception);
            }
        }

        public static async Task root4Parse(string Input)
        {
            try
            {
                MyLogger.GetInstance().Debug("Creating object class");

                Root4 myClass = JsonConvert.DeserializeObject<Root4>(Input);

                MyLogger.GetInstance().Debug("Checking payment providers");

                //Method to check if 'External' Payment provider is enabled HERE
                bool result = await restValidation.CheckProviders(myClass);

                if (result == false)
                {
                    var exception = new DeserializationException("an Error occurred. Please ensure you have enabled 'External Payment Providers' in MyQ > Accounting > Credit > Credit recharge");
                    MyLogger.GetInstance().Error("Error: ", exception);
                }

                MyLogger.GetInstance().Info("Deserialization Complete!");
            }

            catch (Exception ex)
            {
                var exception = new DeserializationException("Error: " + ex.Message);
                MyLogger.GetInstance().Error("Error: ", exception);
            }
        }

        public static async Task root5Parse(string Input)
        {
            try
            {
                MyLogger.GetInstance().Debug("Creating object class");

                Root5 myClass = JsonConvert.DeserializeObject<Root5>(Input);

                MyLogger.GetInstance().Debug("Checking transaction data");

                //Method to check if the Recharge amount is the same as ParentPay
                int result = await restValidation.CheckPaymentAmount(myClass);

                if (result == 0)
                {
                    var exception = new DeserializationException("an Error occurred while creating recharge request, please try again later.");
                    MyLogger.GetInstance().Error("Error: ", exception);
                }

                else if (result == 2)
                {
                    MyLogger.GetInstance().Info("PaymentAmountValidator, No Payments found! ");
                }

                else if (result == 1)
                {
                    MyLogger.GetInstance().Info("Deserialization Complete!, Value {0}", await data.DatabaseHash.dbEncrypt(myClass.amount.ToString(), 2));
                }

                myqConfiguration.MyQ.PaymentAmount = myClass.amount;
                myqConfiguration.MyQ.PaymentID = myClass.id;
            }

            catch (Exception ex)
            {
                var exception = new DeserializationException(ex.Message);
                MyLogger.GetInstance().Error("Error: ", exception);
            }
        }

        public static async Task root6Parse(string Input)
        {
            try
            {
                MyLogger.GetInstance().Debug("Creating object class");

                Root6 myClass = JsonConvert.DeserializeObject<Root6>(Input);

                MyLogger.GetInstance().Info("Deserialization Complete, {0}", await data.DatabaseHash.dbEncrypt(myClass.amount.ToString(), 2));
            }

            catch (Exception ex)
            {
                var exception = new DeserializationException(ex.Message);
                MyLogger.GetInstance().Error("Error: ", exception);
            }
        }

        public static async Task root7Parse(string Input)
        {
            try
            {
                MyLogger.GetInstance().Debug("Creating object class");

                Root7 myClass = JsonConvert.DeserializeObject<Root7>(Input);

                MyLogger.GetInstance().Debug("Checking user credit");

                bool result = await restValidation.CheckCredit(myClass);
                if (result == false) { throw new DeserializationException($"Error - failed to retrieve credit for user: '{myqConfiguration.MyQ.Username}'. Please check credit is enabled and try again!"); }
                MyLogger.GetInstance().Info("Deserialization Complete, Credit accounts: {0}", myClass.accounts.Count);
            }

            catch (Exception ex)
            {
                var exception = new DeserializationException(ex.Message);
                MyLogger.GetInstance().Error("Error: ", exception);
            }
        }

        public static async Task root8Parse(string Input)
        {
            try
            {
                if (!string.IsNullOrEmpty(Input))
                {
                    await certificateManagement.StringToCertificate(Input);
                }
            }

            catch (Exception ex)
            {
                var exception = new CertificateException("Error: " + ex.Message);
                MyLogger.GetInstance().Error("Error: ", exception);
            }
        }
    }
}
