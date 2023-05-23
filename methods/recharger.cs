using System;
using System.Threading.Tasks;
using Gateway.Logger;
using PaymentGateway.exceptions;
using PaymentGateway.methods.timers;
using PaymentGateway.request;
using PaymentGateway.request.response;

namespace PaymentGateway.methods
{
    public class recharger
    {

        public static async Task AuthenticationPack(xmlUtility.HandleSimplePaymentReportResult PaymentReport = null)
        {
            //Will use try {} catch {} to handle any exceptions caught during code execution. 
            try
            {
                MyLogger.GetInstance().Info("Starting transactions...");
                MyLogger.GetInstance().Info("Downloading ParentPay transactions");

                //Creating a new instance of the soap class, used to retrieve the ParentPay bulkpaymentdata

                soap Soap = new soap();

                //Calling the 'httpSoap' request and returning an 'Object' but casting it as a type: 'xmlUtility.HandleSimplePaymentReportResult', which is the PaymentReport class
                PaymentReport = (xmlUtility.HandleSimplePaymentReportResult)await soap.httpSoap("handleSimplePaymentReport");

                //Calling the REST API to retrieve the raw certificate from the MyQ server and storing it in the 'cert' variable.
                var cert = await flurlRest.getCertificate().Result.ResponseMessage.Content.ReadAsStringAsync();

                //passing the 'cert' variable into a method designed to check if it's null or not then pass to be converted to an X509 type certificate for installiation.
                await restUtility.root8Parse(cert);

                //Calling the REST API to generate a new MyQ access_token and passing the immediate result into a method to validate and deserialize the result into a pre-defined class.
                await restUtility.root1Parse(await flurlRest.getToken().Result.ResponseMessage.Content.ReadAsStringAsync());

                //Calling a new Task factory to run start a timer in the background. This timer, once finished will run the current method again, retarting the timer again.
                await Task.Factory.StartNew(async () => { await updateMyQ.StartTimer(); });

                //Looping through the array of users download from the bulkPaymentData and performing the 
                foreach (xmlUtility.PaymentVO User in PaymentReport.PaymentArray)
                {
                    MyLogger.GetInstance().Info("Syncing Credit for {0}", PaymentReport.PaymentArray.PaymentVO[parentpayConfiguration.ParentPay.GlobalNum].ConsumerName);

                    //Calls the 'PreRechargePack', passing in the paymentData and soap class instance. This method is designed to set required values for the user currently in iteration, including username, amount and their ParentPay Identifier. 
                    //This method also calls the request to get userInformation from the MyQ API.
                    await PreRechargePack(PaymentReport, Soap);
                }

                //Setting the incremental and Global values to '0' so that when the incremental method is called, the loop will not miss user '0'.
                parentpayConfiguration.ParentPay.IncrementThroughUsers = 0;
                parentpayConfiguration.ParentPay.GlobalNum = 0;
            }

            catch (Exception ex)
            {
                var exception = new RechargeException(ex.Message + ex.StackTrace + ex.TargetSite + ex.Source);
                MyLogger.GetInstance().Error("Error: " + exception);
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
                await IterateUser(parentpayConfiguration.ParentPay.GlobalNum);

                //Assigning the username to be used in MyQ to be the same as the consumer name downloaded from ParentPay
                myqConfiguration.MyQ.Username = XML.PaymentArray.PaymentVO[parentpayConfiguration.ParentPay.GlobalNum].ConsumerName;

                //Balance is downloaded from ParentPay in pence, therefore converting it to pounds (decimal) using the 'currencyConversion' method and saving the result in the 'myqConfiguration.MyQ.Amount' variable.
                myqConfiguration.MyQ.Amount = currencyConversion.PenceToPounds(XML.PaymentArray.PaymentVO[parentpayConfiguration.ParentPay.GlobalNum].Amount).Result;

                //Assigning the ParentPay identifer to be the same as the ID downloaded from the bulkPaymentData.
                parentpayConfiguration.ParentPay.UserIdentifier = XML.PaymentArray.PaymentVO[parentpayConfiguration.ParentPay.GlobalNum].Identifier;

                //Calling the REST API to get user information from MyQ regarding the current user. - Passing in their usernamename(ConsumerName) and Identifier(UserID)
                await restUtility.rootParse(await flurlRest.getUserInfo(
                    XML.PaymentArray.PaymentVO[parentpayConfiguration.ParentPay.GlobalNum].ConsumerName, 
                    XML.PaymentArray.PaymentVO[parentpayConfiguration.ParentPay.GlobalNum].Identifier.ToString()).Result.ResponseMessage.Content.ReadAsStringAsync());
            }

            catch (Exception ex)
            {
                var exception = new RechargeException(ex.ToString());
                MyLogger.GetInstance().Error("Error: ", exception);
            }

            //Calling the 'TransactionCheckPack', which will verify whether or not transactions have already been performed for this user.
            await TransactionCheckPack(XML, Soap);
        }

        private static async Task TransactionCheckPack(xmlUtility.HandleSimplePaymentReportResult XML, soap Soap)
        {
            //Will use try {} catch {} to handle any exceptions caught during code execution. 
            try
            {
                //Assining the ParentPayUserID to a local variable named 'parentpayUserID'
                var parentpayUserID = await data.DatabaseHash.dbEncrypt(XML.PaymentArray.PaymentVO[parentpayConfiguration.ParentPay.GlobalNum].Identifier.ToString(), 1);

                //Assigning the ParentPay TransactionID to a local variable named 'transactionID'.
                var transactionID = await data.DatabaseHash.dbEncrypt(XML.PaymentArray.PaymentVO[parentpayConfiguration.ParentPay.GlobalNum].PaymentId.ToString(), 3);

                //Assigning the MyQ UserID to a local variable named 'myqUserID'.
                var myqUserID = await data.DatabaseHash.dbEncrypt(myqConfiguration.MyQ.UserID, 0);

                //Each variable asigned, is hashed using a predefiend hash-key. Each hash-key is different per set of data. The hash is performed because all data stored in the database is hashed.
                //Therefore, in order to retrive data the equivilent needs to be hashed as well.

                //Performing a database request to get a list of all transactions where the 'ParentPayUserID', 'ParentPayTransactionID' and 'MyQUserID' match. The result is returned as a true or false as to whether any payments exist (bool)
                bool TransactionExists = await data.db.getUserTransactions(parentpayUserID, transactionID, myqUserID);

                //If the payment already exists, the system will skip getting new payment data.
                if (TransactionExists == true)
                    await UpdateCredit(Soap, XML);

                //If the payment does NOT already exist, the system will download the nessessary data for a new one for this specific user.
                else if (TransactionExists == false)
                    await NewTransaction(Soap, XML);
            }

            catch (Exception ex)
            {
                var exception = new RechargeException(ex.ToString());
                MyLogger.GetInstance().Error("Error: ", exception);
            }
        }

        private static async Task NewTransaction(soap Soap, xmlUtility.HandleSimplePaymentReportResult xml)
        {
            try
            {
                //Calling on the REST API to get a list of recharge providers available for this user on the MyQ server and passing the immediate result into a deserialization method.
                await restUtility.root4Parse(await flurlRest.getRechargeProviders().Result.ResponseMessage.Content.ReadAsStringAsync());

                //Calling on the REST API to create a new recharge request with a specific balance that will have been previously specified, passing the returned body, including a request ID, into a deserialization method.
                await restUtility.root5Parse(await flurlRest.createRechargeRequest().Result.ResponseMessage.Content.ReadAsStringAsync());

                //Calling on the REST API to confirm the initial recharge request and commit it into the system, passing the returned data into a deserialization method where it will be validated.
                await restUtility.root6Parse(await flurlRest.commitRechargeRequest().Result.ResponseMessage.Content.ReadAsStringAsync());

                //Calling on the REST API to get a list of all past payments for the current user, where it's response is passed into a deserialization where the data is validated as to whether or not the latest payment went through or not.
                await restUtility.root3Parse(await flurlRest.getPayments().Result.ResponseMessage.Content.ReadAsStringAsync());
            }

            catch (Exception ex)
            {
                var exception = new RechargeException(ex.ToString());
                MyLogger.GetInstance().Error("Error: ", exception);
            }

            //Calling the 'UpdateCredit' method, that 
            await UpdateCredit(Soap, xml);
        }

        private static async Task UpdateCredit(soap Soap, xmlUtility.HandleSimplePaymentReportResult xml)
        {
            //Calling on the REST API to get the users updated credit value, passing the retuned request response into a deserialization method that will validate it then store the new balance for further use.
            await restUtility.root7Parse(await flurlRest.getUsersCredit().Result.ResponseMessage.Content.ReadAsStringAsync());

            //Storing the ParentPay UserID in a local variable called 'parentpayUserID'.
            var parentpayUserID = await data.DatabaseHash.dbEncrypt(xml.PaymentArray.PaymentVO[parentpayConfiguration.ParentPay.GlobalNum].Identifier.ToString(), 1);

            //Storing the ParentPay TransactionID in a local variable called 'transactionID'.
            var transactionID = await data.DatabaseHash.dbEncrypt(xml.PaymentArray.PaymentVO[parentpayConfiguration.ParentPay.GlobalNum].PaymentId.ToString(), 3);

            //Storing the MyQ UserID in a local variable called 'myqUserID'.
            var myqUserID = await data.DatabaseHash.dbEncrypt(myqConfiguration.MyQ.UserID, 0);

            //Getting the new balance returned from the request, encrypting it and storing it in a local variable called 'newUserBalance'.
            var newUserBalance = await data.DatabaseHash.dbEncrypt(myqConfiguration.MyQ.UserNewBalance.ToString(), 4);

            //Performing a database operation to insert the current transaction into the database.
            await data.db.newTransaction(parentpayUserID, transactionID, myqUserID); 

            //Performing a databse operation to insert the transaction into the paymentUpdates table, with the current dateTime.
            await data.db.newTillBalance(parentpayUserID, newUserBalance, myqUserID, System.DateTime.Now);
        }

        //An iteration method designed to loop through an array of values and increment the value each time it's called.
        public static Task IterateUser(int globalNum)
        {
            if (parentpayConfiguration.ParentPay.IncrementThroughUsers == 0)
            {
                parentpayConfiguration.ParentPay.IncrementThroughUsers++;
                parentpayConfiguration.ParentPay.GlobalNum = 0;
            }

            else if (parentpayConfiguration.ParentPay.IncrementThroughUsers > 0)
            {
                parentpayConfiguration.ParentPay.IncrementThroughUsers++;
                parentpayConfiguration.ParentPay.GlobalNum++;
            }

            return Task.CompletedTask;
        }
        
        private static async Task startMessageUpdateTimer(xmlUtility.HandleSimplePaymentReportResult XML)
        {
            if (iterationConfig.Other.GlobalVariable == 0)
            {
                await updateParentPay.newTillBalance(XML);
                iterationConfig.Other.GlobalVariable = 1;
            }
        }
    }
}