using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.data
{
    internal class hash
    {
        public static async Task<char[]> initKeys(int KeyNum)
        {
            string[] returnValue = new string[6];

            //MyQ UserID
            returnValue[0] = "lrHvB4SBhqEqJSgI4pbw";

            //ParentPay UserID
            returnValue[1] = "joNYipgzSihArmsdxrjL";

            //MyQ Access Token
            returnValue[2] = "i73tNh514FMplj9cybSm";

            //ParentPay PaymentID
            returnValue[3] = "Gh7ZLvyi3mlV9SJxSXzX";

            //UpdatedBalance
            returnValue[4] = "n6TyARAdGOIsJ0S1Ge3M";

            //Database Password (for export)
            returnValue[5] = "jfiosru_fjsoruw3vva843";

            return Task.FromResult(returnValue[KeyNum].ToCharArray()).Result;
        }

        private static TripleDESCryptoServiceProvider GetCryproProvider(int keyNum)
        {
            var md5 = new MD5CryptoServiceProvider();
            var key = md5.ComputeHash(Encoding.UTF8.GetBytes(initKeys(keyNum).Result));
            return new TripleDESCryptoServiceProvider() { Key = key, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 };
        }

        public static async Task<string> dbEncrypt(string plainString, int keyNum)
        {
            var data = Encoding.UTF8.GetBytes(plainString);
            var tripleDes = GetCryproProvider(keyNum);
            var transform = tripleDes.CreateEncryptor();
            var resultsByteArray = transform.TransformFinalBlock(data, 0, data.Length);
            return Convert.ToBase64String(resultsByteArray);
        }

        public static async Task<string> dbDecrypt(string encryptedString, int keyNum)
        {
            var data = Convert.FromBase64String(encryptedString);
            var tripleDes = GetCryproProvider(keyNum);
            var transform = tripleDes.CreateDecryptor();
            var resultsByteArray = transform.TransformFinalBlock(data, 0, data.Length);
            return Encoding.UTF8.GetString(resultsByteArray);
        }
        public static async Task<decimal> decryptBalance(string enryptedBalance)
        {
            string tmp = await hash.dbDecrypt(enryptedBalance.ToString(), 4);
            decimal balance = Convert.ToDecimal(tmp);

            return balance;
        }
    }
}
