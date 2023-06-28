using System;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Gateway.Logger;
using System.Data.SqlServerCe;
using PaymentGateway.methods;
using PaymentGateway.Properties;

namespace PaymentGateway.data
{
    public class db
    {
        //A static string that is used to store the connection string for the database file.
        internal static string ConnectionString { get; set; }
        internal static string dbPasswod { get; set; }

        //The 'CreateConnection' attempts to FIRST check if the database exists by calling the 'buildDatabase' method.
        //If the database exists, a new connection is created using the connection string that will have been previously specified in the 'buildDatabase' method and the connection is returned to the user.
        public static async Task<SqlCeConnection> CreateConnection(string dbassword, bool dbExists = false, SqlCeConnection connection = null)
        {
            //Using a try {} catch {} to properly catch and handle any errors that are generated during method execution.
            try
            {
                //The 'buildDatabase' method will return either a true or false (bool).
                //If return == true, the system will create a new SqlCeConnection using the connection string stored in the static string, returning it to the user.
                dbExists = await buildDatabase(dbassword);

                if (dbExists)
                {
                    MyLogger.GetInstance().Debug("Creating database connection");
                    connection = new SqlCeConnection(db.ConnectionString);
                }
            }

            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Error: " + ex.Message, ex.StackTrace);
            }

            MyLogger.GetInstance().Debug("Returning database connection");
            return connection;
        }

        //The 'buildDatabase' method assigns a connection string, pulling the password the user entered when the program first ran.
        //The method will then check to see if the specified database file exists within the database and if not will create the file using the connection string.
        //The method will then return true as the database has ben created. Should it fail and create an error, the try {} catch {} statement will catch this and log the error accordingly.
        public static async Task<bool> buildDatabase(string dbPassword = null, bool dbExists = false)
        {
            if (dbPassword != null)
            dbPasswod = dbPassword;

            else if (dbPassword == null)
            dbPasswod = Settings.Default.dbPassword;

            try
            {
                if (await databaseLocation())
                    ConnectionString = $"Data Source={internalConfig.internalConfiguration.dbLocation}\\db.sdf;Encrypt Database=True;Password={Settings.Default.dbPassword};File Mode=Exclusive;Persist Security Info=True;";

                if (await databaseLocation())
                    if (File.Exists($"{internalConfig.internalConfiguration.dbLocation}\\db.sdf"))
                    {
                        dbExists = true;
                    }

                    else if (!File.Exists($"{internalConfig.internalConfiguration.dbLocation}\\db.sdf"))
                    {
                        //Creating a new instance of the SqlCeEngine, in order to utilise the CreateDatabase() function.
                        //Initializing the class inside a using statement means that it will be correctly disposed of when no longer needed, freeing up system resources.
                        using (var engine = new SqlCeEngine(ConnectionString))
                        {
                            engine.CreateDatabase();
                        }

                        dbExists = true;
                    }
            }

            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Error: " + ex.Message, ex.StackTrace);

                dbExists = false;
            }

            return dbExists;
        }

        //Gets the user profile location and creates a hidden folder called 'PaymentGateway' if none exists...
        internal static async Task<bool> databaseLocation(bool returnValue = false)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(internalConfig.internalConfiguration.dbLocation);

                //If directory DOES NOT exist...
                if (!Directory.Exists(internalConfig.internalConfiguration.dbLocation))
                {
                    //Creates directory...
                    Directory.CreateDirectory(internalConfig.internalConfiguration.dbLocation);

                    //Check: Directory created, does it exist now?
                    if (Directory.Exists(internalConfig.internalConfiguration.dbLocation))
                        returnValue = true;
                }

                //Check 3: Did the directory exist in the first place?
                else if (Directory.Exists(internalConfig.internalConfiguration.dbLocation))
                    returnValue = true;
            }

            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Error: " + ex.Message, ex.StackTrace);
                returnValue = false;
            }

            //Let's inform the others ;)
            return returnValue;
        }

        //This method simply checks if the connection is valid.
        //It does this by creating a new connection, opening it, setting a value and closing it again.
        //Should an error be caught, the try {} catch {} block will handle this appropiately.
        private static async Task<bool> CheckConnection()
        {
            bool establishedConnection = false;

            using (var connection = CreateConnection(db.dbPasswod).Result)
            {
                try
                {
                    MyLogger.GetInstance().Debug("DatabaseConnection opening database connection");
                    await connection.OpenAsync();

                    MyLogger.GetInstance().Debug("DatabaseConnection validating connection");
                    establishedConnection = true;

                    MyLogger.GetInstance().Debug("DatabaseConnection closng database connection");
                    connection.Close();
                }

                catch (Exception ex)
                {
                    establishedConnection = false;

                    MyLogger.GetInstance().Error("Error: " + ex.Message, ex.StackTrace);
                }

                return establishedConnection;
            }
        }

        public static async Task setupDatabase()
        {
            MyLogger.GetInstance().Debug("Verifying database tables");

            //Calling the 'CreateConnection' method that will firstly try to build the database if it dosen't already exist, then return a new databse connection.
            using (var connection = await CreateConnection(db.dbPasswod))
            {
                //Opening the newly established database connection asyncronously.
                await connection.OpenAsync();

                //Using a try {} catch {} block to propely catch and handle errors caught during code execution.
                try
                {
                    //Creating an SQL command to check if the table 'transactionHistory' exists in the current database.
                    var cmd1 = new SqlCeCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'transactionHistory'", connection);

                    //Creating an SQL command to check if the table 'creditTransactions' exists in the current database.
                    var cmd2 = new SqlCeCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'creditTransactions'", connection);

                    //Initilizing a new SqlReader then executing and executing the first command.
                    using (var reader = await cmd1.ExecuteReaderAsync())
                    {
                        //Using a while loop to loop through all the content in the database.
                        while (await reader.ReadAsync())
                        {
                            //If no tables are found, '0' is returned and the 'executeQuery()' method is called, passing in a query to create the 'transaction' table.
                            if (reader.GetInt32(0) == 0)
                            {
                                MyLogger.GetInstance().Debug("Table 'transactionHistory' not found! Creating table...");

                                MyLogger.GetInstance().Debug("Creating database table 'transactionHistory'");

                                await executeQuery("CREATE TABLE transactionHistory (P_UserID nvarchar(50), M_UserID nvarchar(50), P_TransactionID nvarchar(50), M_TransactionID nvarchar(50));", connection);
                            }
                        }
                    }

                    //Creating an SQL command to check if the table 'creditTransactions' exists in the current database.
                    using (var reader = await cmd2.ExecuteReaderAsync())
                    {
                        //Using a while loop to loop through all the content in the database.
                        while (await reader.ReadAsync())
                        {
                            //If no tables are found, '0' is returned and the 'executeQuery()' method is called, passing in a query to create the 'creditTransactions' table.
                            if (reader.GetInt32(0) == 0)
                            {
                                MyLogger.GetInstance().Debug("Table 'creditTransactions' not found! Creating table...");

                                MyLogger.GetInstance().Debug("Creating database table 'creditTransactions'");

                                await executeQuery("CREATE TABLE creditTransactions (P_UserID nvarchar(50), M_UserID nvarchar(50), UpdatedBalance nvarchar(50), TimeOfTransaction datetime)", connection);
                            }
                        }
                    }

                    //Once both requests are complete, the connection is closed and disposed of. 
                    connection.Close();
                    connection.Dispose();
                }

                catch (Exception ex)
                {
                    MyLogger.GetInstance().Error("Error: " + ex.Message, ex.StackTrace);

                    //Should the application crash, the connection is also closed off and disposed of manually, incase the application fails to properly dispose of the connection.
                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        private static async Task executeQuery(string query, SqlCeConnection connection)
        {
            using (SqlCeCommand command = new SqlCeCommand())
            {
                try
                {
                    command.CommandText = query;
                    command.Connection = connection;

                    //BUG - Crashes here when trying to create database tables
                    command.ExecuteNonQuery();

                    //Don't close connection - Connection is shared!
                }

                catch (Exception ex)
                {
                    MyLogger.GetInstance().Error("Error: " + ex.Message, ex.StackTrace);

                    connection.Close();
                    connection.Dispose();
                }
            }
        }

        public static async Task newTransaction(string P_UserID, string P_TransactionID, string M_UserID)
        {
            using (var connection = await CreateConnection(db.dbPasswod))
            {
                await connection.OpenAsync();

                using (SqlCeCommand command = new SqlCeCommand(null, connection))
                {
                    try
                    {
                        command.CommandText = "INSERT INTO transactionHistory (P_UserID, M_UserID, P_TransactionID) VALUES (@UserID, @M_UserID, @P_TransactionID)";

                        command.Parameters.AddWithValue("@UserID", P_UserID);
                        command.Parameters.AddWithValue("@P_TransactionID", P_TransactionID);
                        command.Parameters.AddWithValue("@M_UserID", M_UserID);

                        command.ExecuteNonQuery();
                        connection.Close();
                        connection.Dispose();
                    }

                    catch (Exception ex)
                    {
                        MyLogger.GetInstance().Error("Error: " + ex.Message, ex.StackTrace);

                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }

        //Will check if a transaction with the specified ID already exists for this user. If so, it will return 'TRUE'. If not it will return 'FALSE'.
        public static async Task<bool> getUserTransactions(string P_UserID, string P_TransactionID, string M_UserID, bool transactionExists = true)
        {
            using (var connection = await CreateConnection(db.dbPasswod))
            {
                await connection.OpenAsync();

                using (SqlCeCommand command = new SqlCeCommand(null, connection))
                {
                    try
                    {
                        command.CommandText = "SELECT * FROM transactionHistory WHERE P_UserID = '@P_UserID' AND P_TransactionID = '@P_TransactionID' AND M_UserID = '@M_UserID'";

                        command.Parameters.AddWithValue("@P_UserID", P_UserID);
                        command.Parameters.AddWithValue("@P_TransactionID", P_TransactionID);
                        command.Parameters.AddWithValue("@M_UserID", M_UserID);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            int rowCount = 0;

                            while (reader.Read())
                            {
                                rowCount++;
                            }

                            if (rowCount == 0) { transactionExists = false; MyLogger.GetInstance().Debug("No transactions detected..."); }
                        }

                        connection.Close();
                        connection.Dispose();
                    }

                    catch (Exception ex)
                    {
                        MyLogger.GetInstance().Error("Error: " + ex.Message, ex.StackTrace);

                        connection.Close();
                        connection.Dispose();
                    }
                }
            }

            return Task.FromResult(transactionExists).Result;
        }

        //Error with this request
        public static async Task newTillBalance(string P_UserID, string UpdatedBalance, string M_UserID, DateTime TimeOfTransaction)
        {
            using (var connection = await CreateConnection(db.dbPasswod))
            {
                await connection.OpenAsync();

                using (SqlCeCommand command = new SqlCeCommand(null, connection))
                {
                    try
                    {
                        command.CommandText = "INSERT INTO creditTransactions (P_UserID, M_UserID, UpdatedBalance, TimeOfTransaction) VALUES (@P_UserID, @M_UserID, @UpdatedBalance, @TimeOfTransaction)";

                        command.Parameters.AddWithValue("@P_UserID", P_UserID);
                        command.Parameters.AddWithValue("@M_UserID", M_UserID);
                        command.Parameters.AddWithValue("@UpdatedBalance", UpdatedBalance);
                        command.Parameters.AddWithValue("@TimeOfTransaction", SqlDbType.DateTime).Value = TimeOfTransaction;

                        command.ExecuteNonQuery();
                        connection.Close();
                        connection.Dispose();
                    }

                    catch (Exception ex)
                    {
                        MyLogger.GetInstance().Error("Error: " + ex.Message, ex.StackTrace);

                        connection.Close();
                        connection.Dispose();
                    }
                }
            }
        }

        //TODO - Test this works [Command Parameters]
        public static async Task<decimal> getLatestTillBalance(string P_UserID, string M_UserID, decimal balance = 0.0M)
        {
            List<string> dt = new List<string>();
            List<string> lstBalance = new List<string>();

            using (var connection = await CreateConnection(db.dbPasswod))
            {
                await connection.OpenAsync();

                using (SqlCeCommand command = new SqlCeCommand(null, connection))
                {
                    try
                    {
                        command.CommandText = "SELECT * FROM creditTransactions WHERE M_UserID = '@M_UserID' AND P_UserID = '@P_UserID'";

                        command.Parameters.AddWithValue("@M_UserID", M_UserID);
                        command.Parameters.AddWithValue("@P_UserID", P_UserID);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                dt.Add(reader[3].ToString());
                                lstBalance.Add(reader[2].ToString());

                                string newUserBalance = await parseLateatTransaction(dt, lstBalance);
                                balance = await
                                    hash.decryptBalance(newUserBalance);
                            }
                        }

                        connection.Close();
                        connection.Dispose();
                    }

                    catch (Exception ex)
                    {
                        MyLogger.GetInstance().Error("Error: " + ex.Message, ex.StackTrace);

                        connection.Close();
                        connection.Dispose();
                    }
                }
            }

            return balance;
        }

        //This method is designed to get around some limitation that SqlCE has that restricts me from implementing 
        private static async Task<string> parseLateatTransaction(List<string> dt, List<string> balance, int pos = 0)
        {
            List<DateTime> time = new List<DateTime>();

            try
            {
                foreach (string item in dt)
                {
                    time.Add(Convert.ToDateTime(item));
                }

                var latest = Enumerable.Max(time);
                pos = time.IndexOf(latest);
            }

            catch (Exception ex)
            {
                MyLogger.GetInstance().Error("Error: " + ex.Message, ex.StackTrace);
            }

            return balance[pos];
        }

        //@TODO: Fix statement to allow reading of data and writing one row at a time to csv method
        internal static async Task renameDatabase()
        {
            System.IO.File.Move(internalConfig.internalConfiguration.dbLocation + "/db.sdf", internalConfig.internalConfiguration.dbLocation + "/old_db-" + DateTime.UtcNow.ToString() + ".sdf");
        }

        internal static async Task getTableData()
        {
            //Check if files exist
            await TabledataExport.buildExportFiles();

            MyLogger.GetInstance().Info("Retrieving table data...");

            List<string> Values = new List<string>();

            using (var connection = await CreateConnection(db.dbPasswod))
            {
                await connection.OpenAsync();

                try
                {
                    var getTable1Data = new SqlCeCommand("SELECT * FROM transactionHistory", connection);

                    var getTable2Data = new SqlCeCommand("SELECT * FROM creditTransactions", connection);

                    using (var reader = await getTable1Data.ExecuteReaderAsync())
                    {
                        using (var sw = new StreamWriter(fileHandler.openFile1))
                        {
                            sw.WriteLine("P_UserID; P_TransactionID; M_UserID");

                            while (await reader.ReadAsync())
                            {
                                sw.WriteLine("{0},{1},{2}", reader["P_UserID"], reader["P_TransactionID"], reader["M_UserID"]);
                            }
                        }
                    }

                    using (var reader = await getTable2Data.ExecuteReaderAsync())
                    {
                        using (var sw = new StreamWriter(fileHandler.openFile2))
                        {
                            sw.WriteLine("P_UserID; M_UserID; UpdatedBalance; TimeOfTransaction");

                            while (await reader.ReadAsync())
                            {
                                sw.WriteLine("{0},{1},{2},{3}", reader["P_UserID"], reader["M_UserID"], reader["UpdatedBalance"], reader["TimeOfTransaction"]);
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    MyLogger.GetInstance().Error("Error: " + ex.Message, ex.StackTrace);
                }

                //Closes the OpenFiles to clear memory :)
                fileHandler.Dispose();
            }
        }
    }
}