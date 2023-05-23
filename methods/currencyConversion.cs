using Gateway.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.methods
{
    internal class currencyConversion
    {
        public static async Task<decimal> PenceToPounds(decimal Pence)
        {
            MyLogger.GetInstance().Debug("CurrencyConverter converting {0} pence...", await data.DatabaseHash.dbEncrypt(Pence.ToString(), 2));

            decimal result = Pence / 100;

            MyLogger.GetInstance().Debug("CurrencyConverter conversion complete");

            return Task.FromResult(result).Result;
        }
    }
}
