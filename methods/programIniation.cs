using PaymentGateway.data;
using PaymentGateway.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PaymentGateway.data.exportdata;

namespace PaymentGateway.methods
{
    internal class programIniation
    {
        public static char[] charArray = "!@#$%^&*()".ToCharArray();

        public static async Task<bool> isAdministrator(bool isAdmin = false)
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principle = new WindowsPrincipal(identity);
            isAdmin = principle.IsInRole(WindowsBuiltInRole.Administrator);

            return isAdmin;
        }

        public static async Task bootUp()
        {
            //Verify if program is being run as admin...
            bool isAdmin = await isAdministrator(true);

            if (isAdmin == false)
            {
                MessageBox.Show("Please run the application with administrator rights to continue...", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information); Environment.Exit(0);
            }

            //Checks if Database password setting has been filled or not!
            await CheckDatabasePassword();
        }

        private static async Task CheckDatabasePassword()
        {
            //Checks if Database password setting has been filled or not!
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.dbPassword))
            {
                Console.WriteLine("Please provide a passowrd for the database of the following requirements...\n- At least 12 characters long\n- At least 1 special character\n- No whitespaces\n\n\nPlease note this password down as you may need it in the future. The database CANNOT be accessed without it!");
                Console.Write("\n" + "Password: ");

                //Checks if password is valid - If so, stores it in app settings
                await verifyDatabasePassword(Console.ReadLine());
            }
        }

        public static async Task verifyDatabasePassword(string pwd)
        {
            //If Password IS Valid!
            if (!string.IsNullOrWhiteSpace(pwd) || pwd.Length >= 12 || pwd.IndexOfAny(charArray) != -1)
            {
                Properties.Settings.Default.dbPassword = pwd;
                Properties.Settings.Default.Save();

                //Creates a JSON file with the database password specified - Can be imported to other applicaitions for use with accessing the database.
                await exportdata.createFile();
                Environment.Exit(0);

                Console.WriteLine("\n" + "Thanks! Password saved!");

                Thread.Sleep(4000);
                Console.Clear();
            }

            else if (string.IsNullOrWhiteSpace(pwd) || pwd.Length < 12 || pwd.IndexOfAny(charArray) == -1)
            {
                Console.WriteLine("Error Invalid Password!");
                Thread.Sleep(4000); Console.Clear();

                await bootUp();//Re-Runs the whole process
            }
        }

        public static async Task checkArguments(string[] args)
        {
            if (args.Contains("-s"))
            {
                await supportData.generateDataFile();
                MessageBox.Show("Support file generated successfully!", "Application Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Environment.Exit(0);
            }

            else if (args.Contains("-a"))
            {
                await createFile();
                Environment.Exit(0);
            }

            else if (args.Contains("-db"))
            {
                await CheckDatabasePassword();
                await db.buildDatabase(Properties.Settings.Default.dbPassword);
            }

            else if (args.Contains("-r"))
            {
                Console.Write("Please enter database password: ");
                var password = Console.ReadLine();

                if (password == Settings.Default.dbPassword)
                {
                    //Assign tmp password equal to new password value.
                    Settings.Default.dbPassword = null;
                    Settings.Default.dbPassword = password;

                    //Rename the old database and create a new one with the new database password
                    System.IO.File.Move("db.sdf", "old-db.sdf");
                    await db.CreateConnection(Properties.Settings.Default.dbPassword);

                    Console.WriteLine("Database password reset successfully!");
                    Console.WriteLine("New database sucessfully created!");
                    Environment.Exit(0);
                }

                else if (password != Settings.Default.dbPassword)
                {
                    Environment.Exit(0);
                }
            }

            //Arguments:
            // (-a) Create AppConfig.json
            // (-s) Generates a support file
            // (-r) Prompts for a new database password. Once done, will rename the old database and create a brand new one!
        }
    }
}
