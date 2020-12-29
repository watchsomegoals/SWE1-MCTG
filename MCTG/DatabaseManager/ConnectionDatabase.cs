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
        //returns 0 for invalid credentials, 1 for already logged in, 2 for succesfully logged in
        public int LogInUser(string username, string password)
        {
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strcountuser = "Select count(*) from users where username = @username and password = @password";
            NpgsqlCommand sqlcountcmd = new NpgsqlCommand(strcountuser, conn);
            sqlcountcmd.Parameters.AddWithValue("username", username);
            sqlcountcmd.Parameters.AddWithValue("password", password);
            sqlcountcmd.Prepare();
            Int32 count = Convert.ToInt32(sqlcountcmd.ExecuteScalar());
            if(count == 0)
            {
                conn.Close();
                return 0;
            }

            string strcountsession = "Select count(*) from sessions where token = @token";
            string token = username + "-mctgToken";
            NpgsqlCommand sqlcountsession = new NpgsqlCommand(strcountsession, conn);
            sqlcountsession.Parameters.AddWithValue("token", token);
            sqlcountsession.Prepare();
            count = Convert.ToInt32(sqlcountsession.ExecuteScalar());
            if (count > 0)
            {
                conn.Close();
                return 1;
            }

            string strcomm = "Insert into sessions (token) Values (@token)";
            NpgsqlCommand sqlcomm = new NpgsqlCommand(strcomm, conn);
            sqlcomm.Parameters.AddWithValue("token", token);
            sqlcomm.Prepare();
            sqlcomm.ExecuteNonQuery();
            conn.Close();
            return 2;
        }
    }
}
