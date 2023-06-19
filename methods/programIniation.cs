using Microsoft.Graph.Models;
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
        private static WindowsIdentity identity = WindowsIdentity.GetCurrent();
        private static WindowsPrincipal principle = new WindowsPrincipal(identity);
        public static async Task<bool> isAdministrator(bool isAdmin = false)
        {
            isAdmin = principle.IsInRole(WindowsBuiltInRole.Administrator);

            return isAdmin;
        }

        public static async Task bootUp()
        {
            //Verify if program is being run as admin...
            bool isAdmin = await isAdministrator(true);

            if (isAdmin == false) {
                MessageBox.Show("Please run the application with administrator rights to continue...", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information); Environment.Exit(0);
            }

            //Checks if Database password setting has been filled or not!
            await DatabasePasswordExists();
        }

        private static async Task DatabasePasswordExists()
        {
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.dbPassword))
                await arguments.sets_DatabasePassword();
        }

        public static async Task checkArguments(string[] args)
        {
            if (args.Contains("-s"))
            {
                await arguments.generate_SuportData();
            }

            else if (args.Contains("-e"))
            {
                await createFile();
            }

            else if (args.Contains("-db"))
            {
                await DatabasePasswordExists();
                await db.buildDatabase(Properties.Settings.Default.dbPassword);
            }

            else if (args.Contains("-r"))
            {

            }

            else if (args.Contains("-o"))
            {
                Console.WriteLine("The database password is: " + Settings.Default.dbPassword);

                Thread.Sleep(7500);
                Console.Clear(); Environment.Exit(0);
            }

            else if (args.Contains("-dbe"))
            {
                await arguments.export_DatabaseTables();
            }

            else if (args.Contains("-t"))
            {
                await arguments.setTimer();
            }

            //Arguments:
            // (-e) Create AppConfig.json
            // (-s) Generates a support file
            // (-r) Prompts for a new database password. Once done, will rename the old database and create a brand new one!
            // (-o) Outputs the database password to the console
            // (-dbe) Exports each database table as a .csv file
            // (-t) Set time for timers

        }

        public static void Dispose()
        {
            identity.Dispose();
            principle = null;

            GC.Collect();
        }
    }
}
