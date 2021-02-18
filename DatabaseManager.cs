using System;
using Rocket.Core.Logging;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace DudeRPCore
{
    public class DatabaseManager
    {
        public static string table = "DudeRPCore";

        public void Connect()
        {
            string connstring = string.Format("Server={0}; database={1}; UID={2}; password={3}; port={4}", Server, DatabaseName, UserName, Password, Port);
            Connection = new MySqlConnection(connstring);
            Connection.Open();
        }

        public string Server { get; set; }
        public string DatabaseName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Port { get; set; }

        public MySqlConnection Connection { get; set; }

        private static DatabaseManager _instance = null;

        public static DatabaseManager Instance()
        {
            if (_instance == null)
                _instance = new DatabaseManager();
            return _instance;
        }

        public bool IsConnect()
        {
            if (Connection == null)
            {
                if (String.IsNullOrEmpty(DatabaseName))
                    return false;

                string connstring = string.Format("Server={0}; database={1}; UID={2}; password={3}; port={4}", Server, DatabaseName, UserName, Password, Port);
                Connection = new MySqlConnection(connstring);
                Connection.Open();
                Logger.Log("[Database Manager] Connection : Ok");
            }

            return true;
        }

        public void Close()
        {
            Connection.Close();
        }

        public void CheckIfExist()
        {
            if (IsConnect())
            {
                MySqlCommand cmd = new MySqlCommand($"CREATE TABLE IF NOT EXISTS {table}( steamid VARCHAR(17) PRIMARY KEY, name VARCHAR(20))", this.Connection);
                cmd.ExecuteNonQuery();
            }
        }

        
        public void Set(string playerID, string valueName, string newValue)
        {
            if (IsConnect())
            {
                string query = $" UPDATE {table} SET {valueName} = '{newValue}' WHERE steamid = {playerID} ";
                var cmd = new MySqlCommand(query, this.Connection);
                cmd.ExecuteNonQuery();
            }
        }

        public void Add(string _playerID, string _fullname)
        {
            if(IsConnect())
            { 
                string query = $"INSERT INTO {table} (steamid, name) VALUES ('{_playerID}','{_fullname}')";
                var cm = new MySqlCommand(query, this.Connection);
                cm.ExecuteNonQuery();
            }
        }

        public string Get(int _pos, string _name, string _val)
        {
            string x = null;

            if (IsConnect())
            {

                string query = $" SELECT * FROM {table} WHERE {_name} = '{_val}' ";
                var cmd = new MySqlCommand(query, this.Connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    x = reader[_pos].ToString();
                }
                reader.Close();
                return x;
            }
            else
            {
                return null;
            }
        }

        public string Get(string playerID, int pos)
        {
            string x = null;

            if (IsConnect())
            {

                string query = $" SELECT * FROM {table} WHERE steamid = '{playerID}' ";
                var cmd = new MySqlCommand(query, this.Connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    x = reader[pos].ToString();
                }
                reader.Close();
                return x;
            }
            else
            {
                return null;
            }
        }

        public bool GetBool(string playerID, int position)
        {
            bool x = false;

            if (this.IsConnect())
            {

                string query = $" SELECT * FROM {table} WHERE steamid = '{playerID}' ";
                var cmd = new MySqlCommand(query, this.Connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (Convert.ToInt32(reader[position]) == 0)
                    {
                        x = false;
                    }
                    else
                    {
                        x = true;
                    }
                }
                reader.Close();
                return x;
            }
            else
            {
                return true;
            }
        }
    }
}
