using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gateway.Logger;
using ICSharpCode.SharpZipLib.Zip;

namespace PaymentGateway.data
{
    public class supportData
    {
        private static ZipEntry entry;
        //A static string containing the location of a Zip file to be created.
        private static string zipPath = "Support\\SupportFile.zip";

        //A method that will create a zip file using the specified path above. The file will contain all the logs located in the 'logs' directory of the application.
        public static async Task generateDataFile()
        {
            //Checks if the DIR exists and if not creates it.
            await createDirectory();

            //Getting content of the 'logs' folder.
            string[] filenames = Directory.GetFiles("logs");

            using (ZipOutputStream OutputStream = new ZipOutputStream(File.Create(zipPath)))
            {
                OutputStream.SetLevel(5);

                byte[] buffer = new byte[4096];

                foreach (string file in filenames)
                {
                    entry = new ZipEntry(Path.GetFileName(file));

                    entry.DateTime = DateTime.Now;
                    OutputStream.PutNextEntry(entry);

                    using (FileStream fs = File.OpenRead(file))
                    {
                        int sourceBytes;
                        do
                        {
                            sourceBytes = fs.Read(buffer, 0, buffer.Length);
                            OutputStream.Write(buffer, 0, sourceBytes);
                        } while (sourceBytes > 0);
                    }
                }

                OutputStream.Finish();
                OutputStream.Close();
            }
        }

        //Checks if the directory '/Support' exists and if not creates it inside the application running folder.
        private static async Task createDirectory()
        {
            string currentPath = Directory.GetCurrentDirectory();

            if (!Directory.Exists(currentPath + " /Support"))
                Directory.CreateDirectory(currentPath + "/Support");
        }

        public static void Dispose()
        {   
            entry = null;
            GC.Collect();
        }
    }
}
