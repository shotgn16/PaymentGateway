using Gateway.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.data
{
    internal class fileHandler
    {
        internal static Stream openFile1;
        internal static Stream openFile2;

        internal static async Task checkFileExists(string fullFilePath)
        {
            if (File.Exists(fullFilePath)) {
                MyLogger.GetInstance().Info("File: " + fullFilePath + " exists! Skipping file creation...");
            }

            else if (!File.Exists(fullFilePath)) {
                MyLogger.GetInstance().Info("File: " + fullFilePath + " not found! Creating file...");
                File.Create(fullFilePath).Close();
            }
        }

        internal static async Task OpenLockFile(string exportFile1, string exportFile2)
        {
            if (!string.IsNullOrEmpty(exportFile1))
            {
                MyLogger.GetInstance().Info("Opening file: " + exportFile1);
                openFile1 = File.Open(exportFile1, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }

            if (!string.IsNullOrEmpty(exportFile2))
            {
                MyLogger.GetInstance().Info("Opening file: " + exportFile2);
                openFile2 = File.Open(exportFile2, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
        }

        public static void Dispose()
        {
            openFile1.Dispose();
            openFile2.Dispose();
            GC.Collect();
        }
    }
}
