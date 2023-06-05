using Newtonsoft.Json;
using PaymentGateway.methods;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CsvHelper;
using System.Drawing.Text;
using Gateway.Logger;
using System.Globalization;

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
            if (!File.Exists("table1_export.csv")) {
                File.Create("table1_export.csv");                           
            }

            else if (File.Exists("table1_export.csv")) {
                MyLogger.GetInstance().Info("CSV exists! Skipping file creation...");
            }

            if (!File.Exists("table1_export.csv")) {
                File.Create("table2_export.csv");
            }

            else if (File.Exists("table2_export.csv")) {
                MyLogger.GetInstance().Info("CSV exists! Skipping file creation...");
            }
        }

        internal static async Task writerCSV(List<string> row, int tableNumber)
        {
            if (tableNumber == 1)
            {
                using (var writer = new StreamWriter("table1_export.csv"))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(row);
                }
            }

            else if (tableNumber == 2)
            {
                using (var writer = new StreamWriter("table2_export.csv"))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(row);
                }
            }
        }
    }
}
