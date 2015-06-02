using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBookingSystem {
    class OpheliaDatabase {
        public OpheliaInterface Interface;
        private static OpheliaDatabase instance;

        private MySqlConnection connection;
        private MySqlDataAdapter adapter;
        public static OpheliaDatabase Instance {
            get {
                if (instance == null) {
                    instance = new OpheliaDatabase();
                }
                return instance;
            }
        }

        private string connectStr = "datasource=localhost;port=3306;username=root;password=usbw";

        private OpheliaDatabase() {
            this.connection = new MySqlConnection(this.connectStr);
        }

        private void ConnectToResourceDB() {
            try {
                this.connection.Close();
                this.adapter = new MySqlDataAdapter();
                this.connection.OpenAsync();
                var set = new DataSet();
                this.connection.CloseAsync();
            }
            catch (MySqlException e) {
                this.Interface.ComposeMessage(OpheliaData.DBConnectionFail + e);
            }
        }

        private string SendQuery(string query) {
            try {
                this.connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, this.connection)) {
                    MySqlDataReader reader = command.ExecuteReader();
                    if (reader.Read()) {
                        return reader.GetString(0);
                    }
                }
                //this.connection.CloseAsync();
            }
            catch (MySqlException) {
                this.ConnectToResourceDB();
                this.connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, this.connection)) {
                    MySqlDataReader reader = command.ExecuteReader();
                    if (reader.Read()) {
                        return reader.GetString(0);
                    }
                }
            }
            return String.Empty;
        }

        public static string GetCity(string input) {
            foreach (var s in input.Split(' ')) {
                char[] oldName = s.ToCharArray();
                char[] newName = new char[oldName.Length - 3];
                for(int i=0; i<oldName.Length; i++) {
                    if (i < oldName.Length - 3) {
                        newName[i] = oldName[i];
                    }
                    else break;
                }
                string country = new string(newName);
                string q = "SELECT combined FROM cities WHERE combined LIKE '" + s +"%'";
                string answer = OpheliaDatabase.Instance.SendQuery(country);
                if (answer != String.Empty) return answer;
            }
            return String.Empty;
        }
    }
}
