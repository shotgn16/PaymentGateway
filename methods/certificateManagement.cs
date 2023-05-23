using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Gateway.Logger;
using Org.BouncyCastle.Security.Certificates;
using PaymentGateway.exceptions;

namespace PaymentGateway.methods
{
    public class certificateManagement
    {
        public static async Task StringToCertificate(string cert)
        {
            MyLogger.GetInstance().Debug("SelfCert parsing certificate string...");

            cert = cert.Replace("-----BEGIN CERTIFICATE-----", null);
            cert = cert.Replace("-----END CERTIFICATE-----", null);

            MyLogger.GetInstance().Debug("SelfCert converting to bytes...");
            byte[] rawData = Convert.FromBase64String(cert);

            MyLogger.GetInstance().Debug("SelfCert converting to certificate");
            await isInstalled(new X509Certificate2(rawData));
        }

        private static async Task isInstalled(X509Certificate2 cert, bool isValidCert = false)
        {
            MyLogger.GetInstance().Debug("SelfCert checking if certificate already exists");

            try
            {
                MyLogger.GetInstance().Debug("SelfCert opening certificate store");

                X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);

                MyLogger.GetInstance().Debug("SelfCert locating certificate");
                var certificates = store.Certificates.Find(X509FindType.FindByIssuerDistinguishedName, CustomDistinguishedName().Result, false);

                applicationConfiguration.Credentials.Certificate_DistinguishedName = "CN=MyQ Certificate Authority";

                foreach (var item in certificates)
                {
                    if (item.Issuer == "CN=MyQ Certificate Authority")
                    {
                        isValidCert = item.Verify();

                        if (!isValidCert)
                        {
                            var exception = new exceptions.CertificateException("Certificate validation failed... Invalid certificate detected!");
                            MyLogger.GetInstance().Error("Error: ", exception);
                        }
                    }
                }

                if (isValidCert) {
                    MyLogger.GetInstance().Warning("Certificate valid!");
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
                var excepton = new exceptions.CertificateException(ex.Message);
                MyLogger.GetInstance().Error("Error: ", excepton);
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
                X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadWrite);
                store.Add(cert);
                store.Close();

                MyLogger.GetInstance().Info("Certificate Installed!");
            }

            catch (Exception ex)
            {
                var exception = new exceptions.CertificateException("Please ensure you run the Gateway with Administrator rights to be able to install the MyQ Root certificate");
                MyLogger.GetInstance().Error("Error: ", exception);
            }
        }

        public static async Task<string> CustomDistinguishedName(string returnValue = "")
        {
            if (!string.IsNullOrEmpty(applicationConfiguration.Credentials.Certificate_DistinguishedName))
            {
                returnValue = applicationConfiguration.Credentials.Certificate_DistinguishedName;
            }
            else
            {
                returnValue = "CN=MyQ Certificate Authority";
            }

            return Task.FromResult(returnValue).Result;
        }
    }
}
