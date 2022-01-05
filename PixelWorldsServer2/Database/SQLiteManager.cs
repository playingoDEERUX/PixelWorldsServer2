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

        public SQLiteConnection GetConnection() => sqliteConn;
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

        public long GetLastInsertID()
        {
            if (sqliteConn == null)
                return 0;

            if (sqliteConn.State != System.Data.ConnectionState.Open)
                return 0;

            return sqliteConn.LastInsertRowId;
        }

        public SQLiteCommand Make(string q)
        {
            if (sqliteConn == null)
                return null;

            if (sqliteConn.State != System.Data.ConnectionState.Open)
            {
                if (!Connect())
                    return null;
            }

            // ensure its OPENED now:
            if (sqliteConn.State == System.Data.ConnectionState.Open)
            {
                var sqliteCmd = sqliteConn.CreateCommand();
                sqliteCmd.CommandText = q;
                return sqliteCmd;
            }

            return null;
        }

        public int PreparedQuery(SQLiteCommand cmd)
        {
            cmd.Prepare();
            return cmd.ExecuteNonQuery();
        }

        public SQLiteDataReader PreparedFetchQuery(SQLiteCommand cmd)
        {
            cmd.Prepare();

            try
            {
                return cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                Util.Log("EXCEPTION during SQLiteManager Query: " + ex.Message);
            }

            return null;
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

        public SQLiteDataReader FetchQuery(string q)
        {
            if (sqliteConn == null)
                return null;

            if (sqliteConn.State != System.Data.ConnectionState.Open)
            {
                if (!Connect())
                    return null;
            }

            // ensure its OPENED now:
            if (sqliteConn.State == System.Data.ConnectionState.Open)
            {
                var sqliteCmd = sqliteConn.CreateCommand();
                sqliteCmd.CommandText = q;

                try
                {
                    return sqliteCmd.ExecuteReader();
                }
                catch (Exception ex)
                {
                    Util.Log("EXCEPTION during SQLiteManager Query: " + ex.Message);
                }
            }

            return null;
        }

        public void Close()
        {
            sqliteConn.Close();
        }

        public static bool HasIllegalChar(string q) => !Regex.IsMatch(q, @"^[a-zA-Z0-9]+$");
    }
}
