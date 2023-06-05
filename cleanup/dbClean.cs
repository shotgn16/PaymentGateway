using PaymentGateway.data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Gateway.Logger;
using PaymentGateway.methods;
using Microsoft.Kiota.Abstractions;

namespace PaymentGateway.cleanup
{
    internal class dbClean
    {
        private static FileInfo fileInfo;

        internal static async Task CleanUp()
        {
            if (File.Exists(internalConfig.internalConfiguration.dbLocation + "/db.sdf")) 
            {
                var location = internalConfig.internalConfiguration.dbLocation + "/db.sdf";

                fileInfo = new FileInfo(location);
                long fileSize = fileInfo.Length;

                await databaseClean(internalConfig.internalConfiguration.dbLocation, fileSize);
            }

            else if (!File.Exists(internalConfig.internalConfiguration.dbLocation + "/db.sdf"))
            {
                MyLogger.GetInstance().Warning("Warning: Unable to detect database location. CleanUp paused!");
            }
        }

        internal static async Task databaseClean(string dbLocation, long dbSize)
        {
            //Copy last two rows from old old table into new table...
            // (1) Get count of users [Array of users in memory?]
            // (2) Query table for data based on 'count' of users
            // (3) Hold data in memory
            // (4) Query 2nd table for data [At the same time]
            // (5) Hold data in memory
            // (6) Insert into new table...

            if (dbSize >= 536870912) {
                await db.renameDatabase();
            }
        }

        public void Dispose()
        {
            fileInfo = null;
            GC.Collect();
        }
    }
}
