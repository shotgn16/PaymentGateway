using Newtonsoft.Json;
using System;
using System.IO;
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
                    return hash.dbEncrypt(Properties.Settings.Default.dbPassword, 5).Result;
                
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

            //Dispose automatically after creating the file
            dbPassword = null;
            GC.Collect();
        }
    }

    internal class TabledataExport
    {
        internal static async Task buildExportFiles()
        {
            string exportFile_1 = @"table1.csv";
            string exportFile_2 = @"table2.csv";

            await fileHandler.checkFileExists(exportFile_1);
            await fileHandler.checkFileExists(exportFile_2);

            await fileHandler.OpenLockFile(exportFile_1, exportFile_2);
        }
    }
}
