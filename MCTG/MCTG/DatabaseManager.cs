using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Npgsql.Replication.PgOutput.Messages;
using NpgsqlTypes;
using Newtonsoft.Json;

namespace MCTG
{
    public class DatabaseManager
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

            if (count == 0)
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
            if (count == 0)
            {
                conn.Close();
                return 0;
            }

            string strcountsession = "Select count(*) from sessions where token = @token";
            string token = "Basic " + username + "-mtcgToken";
            NpgsqlCommand sqlcountsession = new NpgsqlCommand(strcountsession, conn);
            sqlcountsession.Parameters.AddWithValue("token", token);
            sqlcountsession.Prepare();
            count = Convert.ToInt32(sqlcountsession.ExecuteScalar());
            if (count > 0)
            {
                conn.Close();
                return 1;
            }

            string strcomm = "Insert into sessions (token, fk_username) Values (@token, @username)";
            NpgsqlCommand sqlcomm = new NpgsqlCommand(strcomm, conn);
            sqlcomm.Parameters.AddWithValue("token", token);
            sqlcomm.Parameters.AddWithValue("username", username);
            sqlcomm.Prepare();
            sqlcomm.ExecuteNonQuery();
            conn.Close();
            return 2;
        }
        public int GetPackageidForInsertPackage()
        {
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strmax = "Select max(packageid) from cards";
            string strcount = "Select count(*) from cards";
            NpgsqlCommand sqlmaxcmd = new NpgsqlCommand(strmax, conn);
            NpgsqlCommand sqlcountcmd = new NpgsqlCommand(strcount, conn);
            Int32 count = Convert.ToInt32(sqlcountcmd.ExecuteScalar());

            if (count == 0)
            {
                conn.Close();
                return 1;
            }
            else
            {
                Int32 packageid = Convert.ToInt32(sqlmaxcmd.ExecuteScalar());
                conn.Close();
                packageid++;
                return packageid;
            }
        }
        //return 0 if not admin, 1 for no active session, 2 for successful authorization 
        public int CheckUserForInsertPackage(string token)
        {
            if (string.Compare(token, "Basic admin-mtcgToken") != 0)
            {
                return 0;
            }
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strcounttoken = "Select count(*) from sessions where token = @token";
            NpgsqlCommand sqlcountcmd = new NpgsqlCommand(strcounttoken, conn);
            sqlcountcmd.Parameters.AddWithValue("token", token);
            sqlcountcmd.Prepare();
            Int32 count = Convert.ToInt32(sqlcountcmd.ExecuteScalar());
            if (count == 0)
            {
                conn.Close();
                return 1;
            }
            else
            {
                conn.Close();
                return 2;
            }
        }
        public void InsertCardPackage(string id, string name, double damage, int packageid)
        {
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strcomm = "Insert into cards (cardid,name,damage,packageid) Values (@id, @name, @damage, @packageid)";

            NpgsqlCommand sqlcomm = new NpgsqlCommand(strcomm, conn);
            sqlcomm.Parameters.AddWithValue("id", id);
            sqlcomm.Parameters.AddWithValue("name", name);
            sqlcomm.Parameters.AddWithValue("damage", damage);
            sqlcomm.Parameters.AddWithValue("packageid", packageid);

            sqlcomm.Prepare();
            sqlcomm.ExecuteNonQuery();

            conn.Close();
        }
        public bool CheckLoggedIn(string token)
        {
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strcounttoken = "Select count(*) from sessions where token = @token";
            NpgsqlCommand sqlcountcmd = new NpgsqlCommand(strcounttoken, conn);
            sqlcountcmd.Parameters.AddWithValue("token", token);
            sqlcountcmd.Prepare();
            Int32 count = Convert.ToInt32(sqlcountcmd.ExecuteScalar());
            if (count == 0)
            {
                conn.Close();
                return false;
            }
            else
            {
                conn.Close();
                return true;
            }
        }
        public string GetUserLoggedIn(string token)
        {
            string user = null;
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strgetuser = "Select fk_username from sessions where token = @token";
            NpgsqlCommand sqlgetuser = new NpgsqlCommand(strgetuser, conn);
            sqlgetuser.Parameters.AddWithValue("token", token);
            sqlgetuser.Prepare();

            NpgsqlDataReader reader = sqlgetuser.ExecuteReader();
            while (reader.Read())
            {
                user = reader[0].ToString();
            }
            return user;
        }
        public int GetCoinsFromUser(string username)
        {
            int coins = 0;
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strgetcoins = "Select coins from users where username = @username";
            NpgsqlCommand sqlgetcoins = new NpgsqlCommand(strgetcoins, conn);
            sqlgetcoins.Parameters.AddWithValue("username", username);
            sqlgetcoins.Prepare();

            NpgsqlDataReader reader = sqlgetcoins.ExecuteReader();
            while (reader.Read())
            {
                coins = Int32.Parse(reader[0].ToString());
            }
            return coins;
        }
        public void SpendCoins(string username, int value)
        {
            int coinsold = GetCoinsFromUser(username);
            int coinsnew = coinsold - value;

            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strupdate = "Update users set coins = @newcoins where username = @username";
            NpgsqlCommand sqlupdate = new NpgsqlCommand(strupdate, conn);
            sqlupdate.Parameters.AddWithValue("newcoins", coinsnew);
            sqlupdate.Parameters.AddWithValue("username", username);
            sqlupdate.Prepare();
            sqlupdate.ExecuteNonQuery();

            conn.Close();
        }
        public bool CheckPackageAvailability()
        {
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strmax = "Select max(packageid) from cards";
            NpgsqlCommand sqlmaxcmd = new NpgsqlCommand(strmax, conn);

            Int32 packageid = Convert.ToInt32(sqlmaxcmd.ExecuteScalar());

            if (packageid == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public int GetPackageidToBuy()
        {
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strmax = "Select max(packageid) from cards";
            NpgsqlCommand sqlmaxcmd = new NpgsqlCommand(strmax, conn);

            Int32 packageid = Convert.ToInt32(sqlmaxcmd.ExecuteScalar());

            return packageid;
        }
        public void TransferCardsFromPackageToUser(string username, int packageid)
        {
            //string isbought = "yes";
            int newpackageid = 0;
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strupdateuser = "Update cards set fk_username = @username where packageid = @packageid";
            NpgsqlCommand sqlupdateuser = new NpgsqlCommand(strupdateuser, conn);
            sqlupdateuser.Parameters.AddWithValue("username", username);
            sqlupdateuser.Parameters.AddWithValue("packageid", packageid);
            sqlupdateuser.Prepare();
            sqlupdateuser.ExecuteNonQuery();

            string strupdatepackageid = "Update cards set packageid = @newpackageid where packageid = @packageid";
            NpgsqlCommand sqlupdatepackageid = new NpgsqlCommand(strupdatepackageid, conn);
            sqlupdatepackageid.Parameters.AddWithValue("newpackageid", newpackageid);
            sqlupdatepackageid.Parameters.AddWithValue("packageid", packageid);
            sqlupdatepackageid.Prepare();
            sqlupdatepackageid.ExecuteNonQuery();

            conn.Close();
        }
    }
}
