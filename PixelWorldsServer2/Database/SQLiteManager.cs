using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace PixelWorldsServer2.Database
{
    public class SQLiteManager
    {
        private SQLiteConnection sqliteConn = null;

        public SQLiteConnection GetConnection() { return sqliteConn; }
        public bool Connect()
        {
            sqliteConn =
                new SQLiteConnection(@"
                Data Source=pixelworlds.db;
                Version = 3;
                New = True;
                Compress = True;");
                
           
            try
            {
                sqliteConn.Open();
            }
            catch (Exception ex)
            {
                Util.Log("EXCEPTION upon SQLiteManager: " + ex.Message);
                return false;
            }

            return true;
        }

        

        public int Query(string q)
        {
            if (sqliteConn == null)
                return -1;

            if (sqliteConn.State != System.Data.ConnectionState.Open)
            {
                if (!Connect())
                    return -1;
            }

            // ensure its OPENED now:
            if (sqliteConn.State == System.Data.ConnectionState.Open)
            {
                var sqliteCmd = sqliteConn.CreateCommand();
                sqliteCmd.CommandText = q;

                try
                {
                    return sqliteCmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Util.Log("EXCEPTION during SQLiteManager Query: " + ex.Message);
                }
            }

            return -1;
        }

        public void Close()
        {
            sqliteConn.Close();
        }

        public static bool HasIllegalChar(string q) => !Regex.IsMatch(q, @"^[a-zA-Z0-9{}:\-.=/+]+$");
    }
}
