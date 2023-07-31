using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Gateway.Logger;
using Microsoft.Graph.Models;
using PaymentGateway.cleanup;
using PaymentGateway.methods.timers;
using PaymentGateway.request;
using PaymentGateway.request.response;

namespace PaymentGateway.methods
{
    public class recharger
    {
        public static soap soapInstance;
        public static soapv3 _instance;

        public static async Task AuthenticationPack(xmlUtility.HandleSimplePaymentReportResult PaymentReport = null)
        {
            //Will use try {} catch {} to handle any exceptions caught during code execution. 
            try
            {
                MyLogger.GetInstance().Info("Starting transactions...");
                MyLogger.GetInstance().Info("Downloading ParentPay transactions");

                //Creating a new instance of the soap class, used to retrieve the ParentPay bulkpaymentdata

                soapInstance = new soap();
                _instance = new soapv3();

                //Calling the 'httpSoap' request and returning an 'Object' but casting it as a type: 'xmlUtility.HandleSimplePaymentReportResult', which is the PaymentReport class
                var result = await _instance.QueryParentPay("handleSimplePaymentReport");
                PaymentReport = (xmlUtility.HandleSimplePaymentReportResult)result;

                //Calling the REST API to retrieve the raw certificate from the MyQ server and storing it in the 'cert' variable.
                //passing the 'cert' variable into a method designed to check if it's null or not then pass to be converted to an X509 type certificate for installiation.
                await restUtility.root8Parse(await flurlRest.getCertificate());

                //Calling the REST API to generate a new MyQ access_token and passing the immediate result into a method to validate and deserialize the result into a pre-defined class.
                await restUtility.root1Parse(await flurlRest.getToken());

                //Calling a new Task factory to run start a timer in the background. This timer, once finished will run the current method again, retarting the timer again.
                //Need to check if a timer is already running...
                Task.Factory.StartNew(async () => { await updateMyQ.StartTimer(); });

                //Looping through the array of users download from the bulkPaymentData and performing the 
                foreach (xmlUtility.PaymentVO User in PaymentReport.PaymentArray)
                {
                    MyLogger.GetInstance().Info("Syncing Credit for {0}", PaymentReport.PaymentArray.PaymentVO[internalConfig.internalConfiguration.GlobalNum].ConsumerName);

                    //Calls the 'PreRechargePack', passing in the paymentData and soap class instance. This method is designed to set required values for the user currently in iteration, including username, amount and their ParentPay Identifier. 
                    //This method also calls the request to get userInformation from the MyQ API.
                    await PreRechargePack(PaymentReport, soapInstance);
                }

                //Setting the incremental and Global values to '0' so that when the incremental method is called, the loop will not miss user '0'.
                internalConfig.internalConfiguration.IncrementThroughUsers = 0;
                internalConfig.internalConfiguration.GlobalNum = 0;
            }

            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Error: " + ex.Message, ex.StackTrace);
            }

            //A method that when the value is met, starts a timer of x minutes. When the timer finishes updates ParentPay user till balances with the current MyQ credit balances. 
            await startMessageUpdateTimer(PaymentReport);
        }

        private static async Task PreRechargePack(xmlUtility.HandleSimplePaymentReportResult XML, soap Soap)
        {
            //Will use try {} catch {} to handle any exceptions caught during code execution. 
            try
            {
                //Calling a special method for iteration, this method is designed to increment the 'parentpayConfiguration.ParentPay.GlobalNum' value each time, creating a value that can increment through users in the array.
              await IterateUser(internalConfig.internalConfiguration.GlobalNum);

                //Assigning the username to be used in MyQ to be the same as the consumer name downloaded from ParentPay
                internalConfig.internalConfiguration.Username = XML.PaymentArray.PaymentVO[internalConfig.internalConfiguration.GlobalNum].ConsumerName;

                //Balance is downloaded from ParentPay in pence, therefore converting it to pounds (decimal) using the 'currencyConversion' method and saving the result in the 'myqConfiguration.MyQ.Amount' variable.
                internalConfig.internalConfiguration.Amount = currencyConversion.PenceToPounds(XML.PaymentArray.PaymentVO[internalConfig.internalConfiguration.GlobalNum].Amount).Result;

                //Assigning the ParentPay identifer to be the same as the ID downloaded from the bulkPaymentData.
                internalConfig.internalConfiguration.UserIdentifier = XML.PaymentArray.PaymentVO[internalConfig.internalConfiguration.GlobalNum].Identifier;

                //Calling the REST API to get user information from MyQ regarding the current user. - Passing in their usernamename(ConsumerName) and Identifier(UserID)
                await restUtility.rootParse(await flurlRest.getUserInfo(
                XML.PaymentArray.PaymentVO[internalConfig.internalConfiguration.GlobalNum].ConsumerName,
                XML.PaymentArray.PaymentVO[internalConfig.internalConfiguration.GlobalNum].Identifier.ToString()));
            }

            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Error: " + ex.InnerException.ToString());
            }

            //Calling the 'TransactionCheckPack', which will verify whether or not transactions have already been performed for this user.
            await TransactionCheckPack(XML, soapInstance);
        }

        private static async Task TransactionCheckPack(xmlUtility.HandleSimplePaymentReportResult XML, soap Soap)
        {
            //Will use try {} catch {} to handle any exceptions caught during code execution. 
            try
            {
                //Assining the ParentPayUserID to a local variable named 'parentpayUserID'
                string parentpayUserID = await data.hash.dbEncrypt(XML.PaymentArray.PaymentVO[internalConfig.internalConfiguration.GlobalNum].Identifier.ToString(), 1);

                //Assigning the ParentPay TransactionID to a local variable named 'transactionID'.
                string transactionID = await data.hash.dbEncrypt(XML.PaymentArray.PaymentVO[internalConfig.internalConfiguration.GlobalNum].PaymentId.ToString(), 3);

                //Assigning the MyQ UserID to a local variable named 'myqUserID'.
                string myqUserID = await data.hash.dbEncrypt(internalConfig.internalConfiguration.UserID, 0);

                //Performing a database request to get a list of all transactions where the 'ParentPayUserID', 'ParentPayTransactionID' and 'MyQUserID' match. The result is returned as a true or false as to whether any payments exist (bool)
                bool transactionExists = await data.db.getUserTransactions(parentpayUserID, transactionID, myqUserID);

                //If the payment already exists, the system will skip getting new payment data.
                if (transactionExists == true)
                    await UpdateCredit(Soap, XML);

                //If the payment does NOT already exist, the system will download the nessessary data for a new one for this specific user.
                else if (transactionExists == false)
                    await NewTransaction(Soap, XML);
            }

            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Error: " + ex.InnerException);
            }
        }

        private static async Task NewTransaction(soap Soap, xmlUtility.HandleSimplePaymentReportResult xml)
        {
            try
            {
                //Calling on the REST API to get a list of recharge providers available for this user on the MyQ server and passing the immediate result into a deserialization method.
                await restUtility.root4Parse(await flurlRest.getRechargeProviders());

                //Calling on the REST API to create a new recharge request with a specific balance that will have been previously specified, passing the returned body, including a request ID, into a deserialization method.
                await restUtility.root5Parse(await flurlRest.createRechargeRequest());

                //Calling on the REST API to confirm the initial recharge request and commit it into the system, passing the returned data into a deserialization method where it will be validated.
                await restUtility.root6Parse(await flurlRest.commitRechargeRequest());

                //Calling on the REST API to get a list of all past payments for the current user, where it's response is passed into a deserialization where the data is validated as to whether or not the latest payment went through or not.
                await restUtility.root3Parse(await flurlRest.getPayments());


                //Performing a database operation to insert the NEW transaction into the database.
                await data.db.newTransaction(
                    await data.hash.dbEncrypt(xml.PaymentArray.PaymentVO[internalConfig.internalConfiguration.GlobalNum].Identifier.ToString(), 1), 
                    await data.hash.dbEncrypt(xml.PaymentArray.PaymentVO[internalConfig.internalConfiguration.GlobalNum].PaymentId.ToString(), 3),
                    await data.hash.dbEncrypt(internalConfig.internalConfiguration.UserID, 0));
            }

            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Error: " + ex.Message, ex.StackTrace);
            }

            //Calling the 'UpdateCredit' method, that 
            await UpdateCredit(soapInstance, xml);
        }

        private static async Task UpdateCredit(soap Soap, xmlUtility.HandleSimplePaymentReportResult xml)
        {
            //Calling on the REST API to get the users updated credit value, passing the retuned request response into a deserialization method that will validate it then store the new balance for further use.
            await restUtility.root7Parse(await flurlRest.getUsersCredit());

            //Storing the ParentPay UserID in a local variable called 'parentpayUserID'.
            string parentpayUserID = await data.hash.dbEncrypt(xml.PaymentArray.PaymentVO[internalConfig.internalConfiguration.GlobalNum].Identifier.ToString(), 1);

            //Storing the ParentPay TransactionID in a local variable called 'transactionID'.
            string transactionID = await data.hash.dbEncrypt(xml.PaymentArray.PaymentVO[internalConfig.internalConfiguration.GlobalNum].PaymentId.ToString(), 3);

            //Storing the MyQ UserID in a local variable called 'myqUserID'.
            string myqUserID = await data.hash.dbEncrypt(internalConfig.internalConfiguration.UserID, 0);

            //Getting the new balance returned from the request, encrypting it and storing it in a local variable called 'newUserBalance'.
            string newUserBalance = await data.hash.dbEncrypt(internalConfig.internalConfiguration.UserNewBalance.ToString(), 4);

            //Performing a databse operation to insert the transaction into the creditTransactions table, with the current dateTime.
            await data.db.newTillBalance(parentpayUserID, newUserBalance, myqUserID, System.DateTime.Now);
        }

        //An iteration method designed to loop through an array of values and increment the value each time it's called.
        public static Task IterateUser(int globalNum)
        {
            if (internalConfig.internalConfiguration.IncrementThroughUsers == 0)
            {
                internalConfig.internalConfiguration.IncrementThroughUsers++;
                internalConfig.internalConfiguration.GlobalNum = 0;
            }

            else if (internalConfig.internalConfiguration.IncrementThroughUsers > 0)
            {
                internalConfig.internalConfiguration.IncrementThroughUsers++;
                internalConfig.internalConfiguration.GlobalNum++;
            }

            return Task.CompletedTask;
        }
        
        private static async Task startMessageUpdateTimer(xmlUtility.HandleSimplePaymentReportResult XML)
        {
            if (internalConfig.internalConfiguration.GlobalVariable == 0)
            {
                await Task.Run(async () => { await updateParentPay.newTillBalance(XML); });

                internalConfig.internalConfiguration.GlobalVariable = 1;
            }
        }

        public static void Dispose()
        {
            soapInstance = null;
            GC.Collect();
        }
    }
}