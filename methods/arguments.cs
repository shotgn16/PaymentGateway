using ApiTest.Logger;
using Gateway.Logger;
using PaymentGateway.data;
using PaymentGateway.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaymentGateway.methods
{
    internal class arguments
    {
        internal static async Task generate_SuportData()
        {
            //Setup the logger so you can log the output to the console
            await loggerConfig.SetupNLog();

            MyLogger.GetInstance().Info("Generating support file");

            await supportData.generateDataFile();

            MyLogger.GetInstance().Info("Support file generated successfully!");

            System.Environment.Exit(1);
        }

        internal static async Task sets_DatabasePassword()
        {
            char[] charArray = "!@#$%^&*()".ToCharArray();

            Console.WriteLine("Please provide a passowrd for the database of the following requirements...\n- At least 12 characters long\n- At least 1 special character\n- No whitespaces\n\n\nPlease note this password down as you may need it in the future. The database CANNOT be accessed without it!\n\nPassword: ");
            string pwd = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(pwd) || pwd.Length >= 12 || pwd.IndexOfAny(charArray) != -1) {
                Properties.Settings.Default.dbPassword = pwd;
                Properties.Settings.Default.Save();
                
                //Generates a new 'AppConfig.Json' file after setting the database password
                await exportdata.createFile();

                //Exit the application
                System.Environment.Exit(1);

                Console.WriteLine("\nThanks! Password saved!");

                //Run the program all over again with the updated password
                await programIniation.bootUp();

                Console.Clear();
            }

            else if (string.IsNullOrWhiteSpace(pwd) || pwd.Length < 12 || pwd.IndexOfAny(charArray) == -1) {
                Console.WriteLine("Error: Invalid Password");
                Thread.Sleep(4000);
                Console.Clear();

                //Restart the process all over again should the password be incorrect
                await programIniation.bootUp();
            }

            //Disposing
            programIniation.Dispose();
        }

        internal static async Task create_NewDatabase()
        {
            Console.Write("Please enter database password: ");
            var password = Console.ReadLine();

            if (password == Settings.Default.dbPassword)
            {
                //Assign tmp password equal to new password value.
                Settings.Default.dbPassword = null;
                Settings.Default.dbPassword = password;

                //Rename the old database and create a new one with the new database password
                await db.renameDatabase();

                await db.CreateConnection(Properties.Settings.Default.dbPassword);

                Console.WriteLine("Database password reset successfully!");
                Console.WriteLine("New database sucessfully created!");

                //Create new database password
                await exportdata.createFile();

                System.Environment.Exit(1);
            }

            else if (password != Settings.Default.dbPassword)
                System.Environment.Exit(1);
        }

        internal static async Task export_DatabaseTables()
        {
            await db.getTableData();
            Environment.Exit(0);
        }
    }
}
