using Gateway.Logger;
using PaymentGateway.request.response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.methods
{
    internal class restValidation
    {
        public static async Task<bool> CheckPayments(Root3 Payments)
        {
            bool returnValue = new bool();

            try
            {
                if (Payments.count > 0)
                {
                    //Payments Exist!
                    //Call Next Method
                    returnValue = await isLatestPayment(Payments);
                }

                else if (Payments.count == 0)
                {
                    //Payments don't exist!
                    returnValue = false;
                }
            }

            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Error: " + ex.Message, ex.StackTrace);
            }

            return Task.FromResult(returnValue).Result;
        }

        private static async Task<bool> isLatestPayment(Root3 Payments)
        {
            bool returnValue = false;

            try
            {
                //If latest payment created date is newer than previous
                if (Payments.count > 1)
                {
                    if (Payments.payments[0].created > Payments.payments[1].created)
                    {
                        //Payment is the latest one...
                        return returnValue = true;
                    }

                    else if (Payments.payments[0].created <= Payments.payments[1].created)
                    {
                        returnValue = false;
                    }
                }
            }

            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Error: " + ex.Message, ex.StackTrace);
            }

            return Task.FromResult(returnValue).Result;
        }

        //Root4
        public static async Task<bool> CheckProviders(Root4 providers)
        {
            bool ProviderExists = false;

            try
            {
                if (providers.providers.Count == 0)
                {
                    ProviderExists = false;
                }

                else if (providers.providers.Count > 0)
                {
                    foreach (var provider in providers.providers)
                    {
                        if (provider.reference == "external")
                        {
                            ProviderExists = true;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Error: " + ex.Message, ex.StackTrace);
            }

            return Task.FromResult(ProviderExists).Result;
        }

        //Root5
        public static async Task<int> CheckPaymentAmount(Root5 Transaction)
        {
            // 0 - Not Equal // 1 - Equal Amount // 2 - No Payment Found

            int ValidPayment = 0;

            try
            {
                if (Transaction.id == null || Transaction.userId == 0)
                {
                    ValidPayment = 2;
                }

                else if (Transaction.id != null && Transaction.userId != 0)
                {
                    if (Transaction.amount == internalConfig.internalConfiguration.Amount)
                    {
                        ValidPayment = 1;
                    }
                    else
                    {
                        ValidPayment = 0;
                    }
                }
            }

            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Error: " + ex.Message, ex.StackTrace);
            }

            return Task.FromResult(ValidPayment).Result;
        }

        //Root7
        public static async Task<bool> CheckCredit(Root7 accounts)
        {
            bool returnValue = false;

            try
            {
                if (accounts != null)
                {
                    foreach (var item in accounts.accounts)
                    {
                        if (item.reference == "internalaccount")
                        {
                            internalConfig.internalConfiguration.UserNewBalance = item.limits.price;
                            returnValue = true;
                            break;
                        }
                        else
                        {
                            returnValue = false;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Error: " + ex.Message, ex.StackTrace);
            }

            return Task.FromResult(returnValue).Result;
        }
    }
}
