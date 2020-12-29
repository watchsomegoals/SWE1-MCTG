using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Npgsql.Replication.PgOutput.Messages;
using NpgsqlTypes;
using Newtonsoft.Json;

namespace DatabaseManager
{
    public class ConnectionDatabase
    {
        public bool InsertUser(string username, string password)
        {

            int coins = 20;

            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strcount = "Select count(*) from users where username = @username";
            NpgsqlCommand sqlcountcmd = new NpgsqlCommand(strcount, conn);
            sqlcountcmd.Parameters.AddWithValue("username", username);
            sqlcountcmd.Prepare();
            Int32 count = Convert.ToInt32(sqlcountcmd.ExecuteScalar());

            if(count == 0)
            {
                string strcomm = "Insert into users (username,password,coins) Values (@username, @password, @coins)";

                NpgsqlCommand sqlcomm = new NpgsqlCommand(strcomm, conn);

                sqlcomm.Parameters.AddWithValue("username", username);
                sqlcomm.Parameters.AddWithValue("password", password);
                sqlcomm.Parameters.AddWithValue("coins", coins);
                sqlcomm.Prepare();
                sqlcomm.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            else
            {
                conn.Close();
                return false;
            }
        }
    }
}
