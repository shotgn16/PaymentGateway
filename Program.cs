using Gateway.Logger;
using Microsoft.Graph.Models;
using Newtonsoft.Json;
using PaymentGateway.data;
using PaymentGateway.methods;
using PaymentGateway.Properties;
using PaymentGateway.request;
using PaymentGateway.request.response;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PaymentGateway.request.response.graphUtility;

namespace PaymentGateway
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //Creates an UnhandledExceptionEventHandler for logging crash reports
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(crashLogger.OnException);

            await programIniation.checkArguments(args);

            //Functionality: Checks whether the program has started with administrator rights and prompts to restart if not.
            //If the program is started with admin, the a prompt will appear asking for a password to encrypt the database with. After entry, password is checked to ensure its strong enough.
            await programIniation.bootUp();

            //Functionality: Checks if the file - "/config/config.json" exists and if not creates it. 
            //If the file exists, reads the file and deserializes the content to pre-defined class objects, creating a singleton class to access the data globally across the namespace.
            //If the file dosen't exist, creates the "config" directory and "config.json" file inside of the directory, then serializes a pre-defined class, writing it to the config file as JSON, using format indentation. Will then prompt the user to open the file and edit the configuration accordingly, then close the app after X time.
            await configurationLoader.LoadDetails();

            //Functionality: Will call the 'CreateConnection' method that will firstly try to build the database if it dosen't already exist, then return a new databse connection to connection to this database.
            //Will create two database tables in the newly created database (if they don't already exist).
            await db.setupDatabase();

            //Functionality: Will begin downloading payment data and processing request for each user in downloaded collection.
            await recharger.AuthenticationPack();

            Console.ReadLine();
        }
    }
}
