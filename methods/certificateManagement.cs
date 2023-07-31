using System;
using System.Management.Instrumentation;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Gateway.Logger;
using Org.BouncyCastle.Security.Certificates;

namespace PaymentGateway.methods
{
    public class certificateManagement
    {
        private static X509Store store;
        public static async Task StringToCertificate(string cert, string distinguishedName = null)
        {
            MyLogger.GetInstance().Debug("SelfCert parsing certificate string...");

            cert = cert.Replace("-----BEGIN CERTIFICATE-----", null);
            cert = cert.Replace("-----END CERTIFICATE-----", null);

            MyLogger.GetInstance().Debug("SelfCert converting to bytes...");
            byte[] rawData = Convert.FromBase64String(cert);

            MyLogger.GetInstance().Debug("SelfCert converting to certificate");
            await isInstalled(new X509Certificate2(rawData));
        }

        private static async Task isInstalled(X509Certificate2 cert, bool isValidCert = false, string distinguishedName = null)
        {
            MyLogger.GetInstance().Debug("SelfCert checking if certificate already exists");

            try
            {
                MyLogger.GetInstance().Debug("SelfCert opening certificate store");

                store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);

                MyLogger.GetInstance().Debug("SelfCert locating certificate");
                var certificates = store.Certificates.Find(X509FindType.FindByIssuerDistinguishedName, CustomDistinguishedName(distinguishedName).Result, false);

                applicationConfiguration.Credentials.Certificate_DistinguishedName = "CN=MyQ Certificate Authority";

                foreach (var item in certificates)
                {
                    if (item.Issuer == "CN=MyQ Certificate Authority")
                    {
                        isValidCert = item.Verify();

                        if (!isValidCert)
                        {
                            MyLogger.GetInstance().Error("Error: ", "Certificate validation failed... Invalid certificate detected!");
                        }
                    }
                }

                if (isValidCert) {
                    MyLogger.GetInstance().Info("Certificate Valid!");
                }

                if (certificates.Count > 0)
                {
                    MyLogger.GetInstance().Info("Certificate Located");
                    await isValid(cert);
                }

                else if (certificates.Count == 0)
                {
                    MyLogger.GetInstance().Info("No certificate found!");
                    MyLogger.GetInstance().Info("Installing certificate...");
                    await installCertificate(cert);
                }
            }

            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Error: " + ex.Message, ex.StackTrace);
            }
        }

        private static async Task isValid(X509Certificate2 cert, bool isValid = false)
        {
            MyLogger.GetInstance().Debug("Certificate detected");
            MyLogger.GetInstance().Debug("Checking validity");

            isValid = cert.Verify();

            if (isValid == true)
            {
                MyLogger.GetInstance().Info("Certificate valid!!");
            }

            if (isValid == false)
            {
                MyLogger.GetInstance().Warning("Certificate invalid!");
            }
        }

        private static async Task installCertificate(X509Certificate2 cert)
        {
            //Needs to be run as admin!
            try
            {
                using (var store = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
                {
                    store.Open(OpenFlags.ReadWrite);
                    store.Add(cert);
                    store.Close();
                }

                MyLogger.GetInstance().Info("Certificate Installed!");
            }

            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Error: " + ex.Message, ex.StackTrace);
            }
        }

        public static async Task<string> CustomDistinguishedName(string returnValue = "", string distinguishedName = null)
        {
            if (!string.IsNullOrEmpty(distinguishedName))
            {
                returnValue = distinguishedName;
            }
            else
            {
                returnValue = "CN=MyQ Certificate Authority";
            }

            return Task.FromResult(returnValue).Result;
        }

        public static void Dispose()
        {
            store.Dispose();
            GC.Collect();
        }
    }
}
