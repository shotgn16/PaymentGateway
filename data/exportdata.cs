using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.data
{
    internal class exportdata
    {
        internal class dbPassword
        {
            public string databasePassword
            {
                get
                {
                    return DatabaseHash.dbEncrypt(Properties.Settings.Default.dbPassword, 5).Result;
                
                } set {}
            }

            public string appVersion
            {
                get
                {
                    //Returns 1.0.0.0
                    //Major.Minor.Patch.Build
                    return System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
                
                } set {}
            }

            public string databaseVersion
            {
                get
                {
                    return "4.0.8876.1";

                } set {}
            }
        }

        public static async Task createFile()
        {
            dbPassword dbPassword = new dbPassword();
            File.WriteAllText(@"./AppConfig.json", JsonConvert.SerializeObject(dbPassword));
        }
    }
}
