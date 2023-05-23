using System;
using System.DirectoryServices.ActiveDirectory;
using System.Threading.Tasks;
using Gateway.Logger;
using PaymentGateway.request;
using PaymentGateway.exceptions;

namespace PaymentGateway.methods
{
    public class activeDirectory
    {
        internal static async Task<string> UserInfoType(string userName, string personalCode)
        {
            MyLogger.GetInstance().Debug("ADAttributeChecker checking values");

            bool attributeExists = AttributeExists().Result; string returnValue = "";

            if (myqConfiguration.MyQ.reqAzureCID == 1)
            {
                var result = await microsoftGraph.getUserList(myqConfiguration.MyQ.Username);

                //Need to return the extension attribute value. 
                //Search by: 'Consumer Name' - ParentPay
                returnValue = $"code={result}";
            }

            else if (attributeExists == true)
            {

                MyLogger.GetInstance().Debug("ADAttributeChecker active directory attribute exists!");

                returnValue = $"code={personalCode}";
            }

            else if (attributeExists == false)
            {

                MyLogger.GetInstance().Debug("ADAttributeChecker attribute not found! Switching to username");

                returnValue = $"username={userName}";
            }

            return returnValue;
        }

        private static Task<bool> AttributeExists()
        {
            bool returnValue = false;

            try
            {
                MyLogger.GetInstance().Debug("ADAttributeChecker checking for attribute");

                if (!string.IsNullOrWhiteSpace(applicationConfiguration.Credentials.ADName) || !string.IsNullOrWhiteSpace(applicationConfiguration.Credentials.ParentPayIDAttribute))
                {
                    MyLogger.GetInstance().Debug("ADAttributeChecker attribute found!");

                    var context = new DirectoryContext(DirectoryContextType.Forest, applicationConfiguration.Credentials.ADName);

                    MyLogger.GetInstance().Debug("ADAttributeChecker new directory context");

                    using (var schema = System.DirectoryServices.ActiveDirectory.ActiveDirectorySchema.GetSchema(context))
                    {
                        MyLogger.GetInstance().Debug("ADAttributeChecker retriving schema");

                        var userClass = schema.FindClass("user");

                        MyLogger.GetInstance().Debug("ADAttributeChecker searching for class {0}", "user");

                        foreach (ActiveDirectorySchemaProperty property in userClass.GetAllProperties())
                        {
                            if (property.Name == applicationConfiguration.Credentials.ParentPayIDAttribute)
                            {
                                returnValue = true;
                                MyLogger.GetInstance().Info("ADAttributeChecker Attribute Found!");
                            }
                        }
                    }
                }

                else
                { MyLogger.GetInstance().Info("ADAttributeChecker Attribute Null, ActiveDirectory"); }
            }

            catch (Exception ex)
            {
                var exception = new ActiveDirectoryException(ex.Message);
                MyLogger.GetInstance().Error("Error: ", exception);
            }

            return Task.FromResult(returnValue);
        }
    }
}