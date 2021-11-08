using System.Collections.Generic;
using eventula_entrance_client.Models;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System;

namespace eventula_entrance_client.Services
{
    public class Database
    {
        private SQLiteConnection _DbConnection;
        private bool _Disposed;

        protected SQLiteConnection DbConnection
        {
            get => _DbConnection;
            private set
            {
                if (_DbConnection != null)
                {
                    _DbConnection.Dispose();
                }

                _DbConnection = value;
            }
        }

        public void InitializeDB()
        {
            string cs = "Data Source=:memory:";
            DbConnection = new SQLiteConnection(cs);
            DbConnection.Open();

            SQLiteCommand cmd;
            cmd = DbConnection.CreateCommand();

            cmd.CommandText = "DROP TABLE IF EXISTS users";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE users(UserId INTEGER PRIMARY KEY,
                        Name TEXT, Seat TEXT, Counter INT, New INT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO users(UserId, Name, Seat, Counter, New) VALUES(1,'test','C3', 900, 1)";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "INSERT INTO users(UserId, Name, Seat, Counter, New) VALUES(2,'test2','C4', 900, 0)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO users(UserId, Name, Seat, Counter, New) VALUES(3,'test3','C5', 900, 0)";
            cmd.ExecuteNonQuery();

        }
  

        public IEnumerable<User> GetNewUsers()
        {
            SQLiteCommand cmd;
            cmd = DbConnection.CreateCommand();
            cmd.CommandText = "SELECT * FROM users where New = true";
            
            SQLiteDataReader sqlite_datareader;
            sqlite_datareader = cmd.ExecuteReader();

            List<User> list = new List<User>();

            while (sqlite_datareader.Read())
            {
            list.Add(new User{UserId = sqlite_datareader.GetInt32(0), Name = sqlite_datareader.GetString(1), Seat = sqlite_datareader.GetString(2), Counter = sqlite_datareader.GetInt32(3), New = sqlite_datareader.GetBoolean(4)});
            }
            return list.AsEnumerable();

        }

        public IEnumerable<User> GetSelectedUsers()
        {
            SQLiteCommand cmd;
            cmd = DbConnection.CreateCommand();
            cmd.CommandText = "SELECT * FROM users where New = false";
            
            SQLiteDataReader sqlite_datareader;
            sqlite_datareader = cmd.ExecuteReader();

            List<User> list = new List<User>();

            while (sqlite_datareader.Read())
            {
            list.Add(new User{UserId = sqlite_datareader.GetInt32(0), Name = sqlite_datareader.GetString(1), Seat = sqlite_datareader.GetString(2), Counter = sqlite_datareader.GetInt32(3), New = sqlite_datareader.GetBoolean(4)});
            }

            return list.AsEnumerable();

        }
                    




        

    }
}