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
    public class DatabaseManager : DatabaseManagerInterface
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

                string strstats = "Insert into stats (fk_username,elo,won,lost,played) Values (@username, 100, 0, 0, 0)";
                NpgsqlCommand sqlstats = new NpgsqlCommand(strstats, conn);
                sqlstats.Parameters.AddWithValue("username", username);
                sqlstats.Prepare();
                sqlstats.ExecuteNonQuery();

                string strscoreboard = "Insert into scoreboard (fk_username, elo) Values (@username, 100)";
                NpgsqlCommand sqlscoreboard = new NpgsqlCommand(strscoreboard, conn);
                sqlscoreboard.Parameters.AddWithValue("username", username);
                sqlscoreboard.Prepare();
                sqlscoreboard.ExecuteNonQuery();

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
            int indeck = 0;
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strcomm = "Insert into cards (cardid,name,damage,packageid,indeck) Values (@id, @name, @damage, @packageid, @indeck)";

            NpgsqlCommand sqlcomm = new NpgsqlCommand(strcomm, conn);
            sqlcomm.Parameters.AddWithValue("id", id);
            sqlcomm.Parameters.AddWithValue("name", name);
            sqlcomm.Parameters.AddWithValue("damage", damage);
            sqlcomm.Parameters.AddWithValue("packageid", packageid);
            sqlcomm.Parameters.AddWithValue("indeck", indeck);

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
            conn.Close();
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
            conn.Close();
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
                conn.Close();
                return false;
            }
            else
            {
                conn.Close();
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

            conn.Close();
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
        public string ShowAllCards(string username)
        {
            string cards;

            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strcards = "Select name, damage from cards where fk_username = @username";
            NpgsqlCommand sqlcards = new NpgsqlCommand(strcards, conn);
            sqlcards.Parameters.AddWithValue("username", username);
            sqlcards.Prepare();

            cards = "The cards of " + username + " are: \n\n";
            NpgsqlDataReader reader = sqlcards.ExecuteReader();
            while (reader.Read())
            {
                string row = "Name: " + reader.GetString(0) + " -- Damage: " + reader.GetDouble(1).ToString() + "\n";
                cards += row;
            }

            conn.Close();
            return cards;
        }
        public bool CheckIfDeckConfigured(string username)
        {
            int indeck = 1;
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strcount = "Select count(*) from cards where fk_username = @username and indeck = @indeck";
            NpgsqlCommand sqlcount = new NpgsqlCommand(strcount, conn);
            sqlcount.Parameters.AddWithValue("username", username);
            sqlcount.Parameters.AddWithValue("indeck", indeck);
            sqlcount.Prepare();

            Int32 count = Convert.ToInt32(sqlcount.ExecuteScalar());

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
        public bool CheckIfCardInDeck(string cardsid, string username)
        {
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strcount = "Select count(*) from cards where cardid = @cards and indeck = 1 and fk_username = @username";
            NpgsqlCommand sqlcount = new NpgsqlCommand(strcount, conn);

            sqlcount.Parameters.AddWithValue("cards", cardsid);
            sqlcount.Parameters.AddWithValue("username", username);
            sqlcount.Prepare();
            Int32 count = Convert.ToInt32(sqlcount.ExecuteScalar());

            if (count > 0)
            {
                conn.Close();
                return true;
            }

            conn.Close();
            return false;
        }
        public void InsertCardInDeck(string cardsid, string username)
        {
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string updatedeckcard = "Update cards set indeck = 1 where cardid = @cardsid and fk_username = @username";
            NpgsqlCommand sqlupdatedeckcard = new NpgsqlCommand(updatedeckcard, conn);

            sqlupdatedeckcard.Parameters.AddWithValue("cardsid", cardsid);
            sqlupdatedeckcard.Parameters.AddWithValue("username", username);
            sqlupdatedeckcard.Prepare();
            sqlupdatedeckcard.ExecuteNonQuery();

            conn.Close();
        }
        public void ResetDeck(string username)
        {
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string updatedeck = "Update cards set indeck = 0 where fk_username = @username";
            NpgsqlCommand sqlupdatedeck = new NpgsqlCommand(updatedeck, conn);
            sqlupdatedeck.Parameters.AddWithValue("username", username);
            sqlupdatedeck.Prepare();
            sqlupdatedeck.ExecuteNonQuery();

            conn.Close();
        }
        public bool CheckIfCardBelongs(string cardsid, string username)
        {
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strcount = "Select count(*) from cards where cardid = @cards and fk_username = @username";
            NpgsqlCommand sqlcount = new NpgsqlCommand(strcount, conn);

            sqlcount.Parameters.AddWithValue("cards", cardsid);
            sqlcount.Parameters.AddWithValue("username", username);
            sqlcount.Prepare();
            Int32 count = Convert.ToInt32(sqlcount.ExecuteScalar());

            if (count == 1)
            {
                conn.Close();
                return true;
            }

            conn.Close();
            return false;
        }
        public string ShowDeckCards(string username)
        {
            string cards;

            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strcards = "Select name, damage from cards where fk_username = @username and indeck = 1";
            NpgsqlCommand sqlcards = new NpgsqlCommand(strcards, conn);
            sqlcards.Parameters.AddWithValue("username", username);
            sqlcards.Prepare();

            cards = username + "'s deck: \n\n";
            NpgsqlDataReader reader = sqlcards.ExecuteReader();
            while (reader.Read())
            {
                string row = "Name: " + reader.GetString(0) + " -- Damage: " + reader.GetDouble(1).ToString() + "\n";
                cards += row;
            }

            conn.Close();
            return cards;
        }
        public string ShowDeckCardsDifferent(string username)
        {
            string cards;

            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strcards = "Select cardid, name, damage from cards where fk_username = @username and indeck = 1";
            NpgsqlCommand sqlcards = new NpgsqlCommand(strcards, conn);
            sqlcards.Parameters.AddWithValue("username", username);
            sqlcards.Prepare();

            cards = username + "'s different representation of deck: \n\n";
            NpgsqlDataReader reader = sqlcards.ExecuteReader();
            while (reader.Read())
            {
                string row = "CardId: " + reader.GetString(0) + " >>>> Name: " + reader.GetString(1) + " >>>> Damage: " + reader.GetDouble(2).ToString() + "\n";
                cards += row;
            }

            conn.Close();
            return cards;
        }
        public bool CheckUserAndToken(string username, string token)
        {
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strcount = "Select count(*) from sessions where fk_username = @username and token = @token";
            NpgsqlCommand sqlcount = new NpgsqlCommand(strcount, conn);
            sqlcount.Parameters.AddWithValue("username", username);
            sqlcount.Parameters.AddWithValue("token", token);
            sqlcount.Prepare();

            Int32 count = Convert.ToInt32(sqlcount.ExecuteScalar());

            if (count == 1)
            {
                conn.Close();
                return true;
            }
            else
            {
                conn.Close();
                return false;
            }
        }
        public bool CheckEditedDataExists(string username)
        {
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strcount = "Select count(name) from users where username = @username";
            NpgsqlCommand sqlcount = new NpgsqlCommand(strcount, conn);
            sqlcount.Parameters.AddWithValue("username", username);
            sqlcount.Prepare();

            Int32 count = Convert.ToInt32(sqlcount.ExecuteScalar());

            if (count == 1)
            {
                conn.Close();
                return true;
            }
            else
            {
                conn.Close();
                return false;
            }
        }
        public void EditUserData(string username, string name, string bio, string image)
        {
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strupdate = "Update users set name = @name, bio = @bio, image = @image where username = @username";
            NpgsqlCommand sqlupdate = new NpgsqlCommand(strupdate, conn);

            sqlupdate.Parameters.AddWithValue("name", name);
            sqlupdate.Parameters.AddWithValue("bio", bio);
            sqlupdate.Parameters.AddWithValue("image", image);
            sqlupdate.Parameters.AddWithValue("username", username);
            sqlupdate.Prepare();
            sqlupdate.ExecuteNonQuery();

            conn.Close();
        }
        public string GetUserData(string username)
        {
            string data;

            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strdata = "Select name, bio, image from users where username = @username";
            NpgsqlCommand sqldata = new NpgsqlCommand(strdata, conn);
            sqldata.Parameters.AddWithValue("username", username);
            sqldata.Prepare();
            data = username + "'s user data: \n\n";
            NpgsqlDataReader reader = sqldata.ExecuteReader();

            while (reader.Read())
            {
                string row = "Name: " + reader.GetString(0) + "     Bio: " + reader.GetString(1) + "     Image: " + reader.GetString(2);
                data += row;
            }

            conn.Close();
            return data;
        }
        public int GetElo(string username)
        {
            int elo = 0;
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strgetelo = "Select elo from stats where fk_username = @username";
            NpgsqlCommand sqlgetelo = new NpgsqlCommand(strgetelo, conn);
            sqlgetelo.Parameters.AddWithValue("username", username);
            sqlgetelo.Prepare();

            NpgsqlDataReader reader = sqlgetelo.ExecuteReader();
            while (reader.Read())
            {
                elo = Int32.Parse(reader[0].ToString());
            }
            conn.Close();
            return elo;
        }
        public int GetWon(string username)
        {
            int won = 0;
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strgetwon = "Select won from stats where fk_username = @username";
            NpgsqlCommand sqlgetwon = new NpgsqlCommand(strgetwon, conn);
            sqlgetwon.Parameters.AddWithValue("username", username);
            sqlgetwon.Prepare();

            NpgsqlDataReader reader = sqlgetwon.ExecuteReader();
            while (reader.Read())
            {
                won = Int32.Parse(reader[0].ToString());
            }
            conn.Close();
            return won;
        }
        public int GetLost(string username)
        {
            int lost = 0;
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strgetlost = "Select lost from stats where fk_username = @username";
            NpgsqlCommand sqlgetlost = new NpgsqlCommand(strgetlost, conn);
            sqlgetlost.Parameters.AddWithValue("username", username);
            sqlgetlost.Prepare();

            NpgsqlDataReader reader = sqlgetlost.ExecuteReader();
            while (reader.Read())
            {
                lost = Int32.Parse(reader[0].ToString());
            }
            conn.Close();
            return lost;
        }
        public void ChangeScoreAndStatsIfWon(string username)
        {
            int eloold = GetElo(username);
            int wonold = GetWon(username);
            int lost = GetLost(username);
            int elonew = eloold + 5;
            int wonnew = wonold + 1;
            int played = wonnew + lost;
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strupdatestats = "Update stats set elo = @elonew, won = @wonnew, played = @played where fk_username = @username";
            NpgsqlCommand sqlupdatestats = new NpgsqlCommand(strupdatestats, conn);
            sqlupdatestats.Parameters.AddWithValue("elonew", elonew);
            sqlupdatestats.Parameters.AddWithValue("wonnew", wonnew);
            sqlupdatestats.Parameters.AddWithValue("played", played);
            sqlupdatestats.Parameters.AddWithValue("username", username);
            sqlupdatestats.Prepare();
            sqlupdatestats.ExecuteNonQuery();

            string strupdatescore = "Update scoreboard set elo = @elonew where fk_username = @username";
            NpgsqlCommand sqlupdatescore = new NpgsqlCommand(strupdatescore, conn);
            sqlupdatescore.Parameters.AddWithValue("elonew", elonew);
            sqlupdatescore.Parameters.AddWithValue("username", username);
            sqlupdatescore.Prepare();
            sqlupdatescore.ExecuteNonQuery();

            conn.Close();
        }
        public void ChangeScoreAndStatsIfLost(string username)
        {
            int eloold = GetElo(username);
            int won = GetWon(username);
            int lostold = GetLost(username);
            int elonew = eloold - 3;
            int lostnew = lostold + 1;
            int played = won + lostnew;
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strupdatestats = "Update stats set elo = @elonew, lost = @lostnew, played = @played where fk_username = @username";
            NpgsqlCommand sqlupdatestats = new NpgsqlCommand(strupdatestats, conn);
            sqlupdatestats.Parameters.AddWithValue("elonew", elonew);
            sqlupdatestats.Parameters.AddWithValue("lostnew", lostnew);
            sqlupdatestats.Parameters.AddWithValue("played", played);
            sqlupdatestats.Parameters.AddWithValue("username", username);
            sqlupdatestats.Prepare();
            sqlupdatestats.ExecuteNonQuery();

            string strupdatescore = "Update scoreboard set elo = @elonew where fk_username = @username";
            NpgsqlCommand sqlupdatescore = new NpgsqlCommand(strupdatescore, conn);
            sqlupdatescore.Parameters.AddWithValue("elonew", elonew);
            sqlupdatescore.Parameters.AddWithValue("username", username);
            sqlupdatescore.Prepare();
            sqlupdatescore.ExecuteNonQuery();

            conn.Close();
        }
        public string GetUserStats(string username)
        {
            string stats = null;

            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strstats = "Select elo, won, lost, played from stats where fk_username = @username";
            NpgsqlCommand sqlstats = new NpgsqlCommand(strstats, conn);
            sqlstats.Parameters.AddWithValue("username", username);
            sqlstats.Prepare();
            stats = username + "'s stats: \n\n";
            NpgsqlDataReader reader = sqlstats.ExecuteReader();

            while (reader.Read())
            {
                string row = "Elo: " + reader.GetInt32(0).ToString() + "     Won: " + reader.GetInt32(1).ToString() + "     Lost: " + reader.GetInt32(2).ToString() + "     Played: " + reader.GetInt32(3).ToString();
                stats += row;
            }

            conn.Close();
            return stats;
        }
        public string GetScoreboard()
        {
            string board;
            int count = 1;
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strboard = "Select fk_username, elo from scoreboard order by elo desc";
            NpgsqlCommand sqlboard = new NpgsqlCommand(strboard, conn);
            sqlboard.Prepare();

            board = "Ranking: \n\n";
            NpgsqlDataReader reader = sqlboard.ExecuteReader();
            while (reader.Read())
            {
                string row = count + ".Username: " + reader.GetString(0) + "     Elo: " + reader.GetInt32(1).ToString() + "\n";
                board += row;
                count++;
            }

            conn.Close();
            return board;
        }
        public bool CheckTradingDeals()
        {
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strcount = "Select count(dealid) from store";
            NpgsqlCommand sqlcount = new NpgsqlCommand(strcount, conn);

            Int32 count = Convert.ToInt32(sqlcount.ExecuteScalar());

            if (count > 0)
            {
                conn.Close();
                return true;
            }
            else
            {
                conn.Close();
                return false;
            }
        }
        public void InsertTradingDeal(string dealid, string username, string cardid, string type, double mindamage)
        {
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strcomm = "Insert into store (dealid,fk_username,cardtotrade,type,minimumdamage) Values (@dealid, @username, @cardid, @type, @mindamage)";

            NpgsqlCommand sqlcomm = new NpgsqlCommand(strcomm, conn);
            sqlcomm.Parameters.AddWithValue("dealid", dealid);
            sqlcomm.Parameters.AddWithValue("username", username);
            sqlcomm.Parameters.AddWithValue("cardid", cardid);
            sqlcomm.Parameters.AddWithValue("type", type);
            sqlcomm.Parameters.AddWithValue("mindamage", mindamage);

            sqlcomm.Prepare();
            sqlcomm.ExecuteNonQuery();

            conn.Close();
        }
        public string GetTradingDeals()
        {
            string deals;
            int count = 1;
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strdeals = "Select s.fk_username, c.name, c.damage, s.type, s.minimumdamage from store s join cards c on s.cardtotrade = c.cardid";
            NpgsqlCommand sqldeals = new NpgsqlCommand(strdeals, conn);
            sqldeals.Prepare();

            deals = "Available trading deals: \n\n";
            NpgsqlDataReader reader = sqldeals.ExecuteReader();
            while (reader.Read())
            {
                string row = count + ".Username: " + reader.GetString(0) + "-->Trading: " + reader.GetString(1) + "-->with Damage: "+ reader.GetDouble(2).ToString() + "-->for: " + reader.GetString(3) + "-->with minimum Damage: " + reader.GetDouble(4).ToString() + "\n";
                deals += row;
                count++;
            }

            conn.Close();
            return deals;
        }
        public bool CheckIfDealBelongsToUser(string username, string dealid)
        {
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strcount = "Select count(*) from store where dealid = @dealid and fk_username = @username";
            NpgsqlCommand sqlcount = new NpgsqlCommand(strcount, conn);

            sqlcount.Parameters.AddWithValue("dealid", dealid);
            sqlcount.Parameters.AddWithValue("username", username);
            sqlcount.Prepare();
            Int32 count = Convert.ToInt32(sqlcount.ExecuteScalar());

            if (count == 1)
            {
                conn.Close();
                return true;
            }

            conn.Close();
            return false;
        }
        public void DeleteTradingDeal(string dealid)
        {
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strdelete = "Delete from store where dealid = @dealid";
            NpgsqlCommand sqldelete = new NpgsqlCommand(strdelete, conn);

            sqldelete.Parameters.AddWithValue("dealid", dealid);
            sqldelete.Prepare();
            sqldelete.ExecuteNonQuery();

            conn.Close();
        }
        public double GetMinDamageDeal(string dealid)
        {
            double mindamage;
           
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strgetmin = "Select minimumdamage from store where dealid = @dealid";
            NpgsqlCommand sqlgetmin = new NpgsqlCommand(strgetmin, conn);
            sqlgetmin.Parameters.AddWithValue("dealid", dealid);
            sqlgetmin.Prepare();

            mindamage = Convert.ToDouble(sqlgetmin.ExecuteScalar());
         
            conn.Close();
            return mindamage;
        }
        public double GetDamageCardFromCards(string cardid)
        {
            double damage;

            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strgetdamage = "Select damage from cards where cardid = @cardid";
            NpgsqlCommand sqlgetdamage = new NpgsqlCommand(strgetdamage, conn);
            sqlgetdamage.Parameters.AddWithValue("cardid", cardid);
            sqlgetdamage.Prepare();

            damage = Convert.ToDouble(sqlgetdamage.ExecuteScalar());

            conn.Close();
            return damage;
        }
        public string GetTypeDeal(string dealid)
        {
            string type = null;
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strgettype = "Select type from store where dealid = @dealid";
            NpgsqlCommand sqlgettype = new NpgsqlCommand(strgettype, conn);
            sqlgettype.Parameters.AddWithValue("dealid", dealid);
            sqlgettype.Prepare();

            NpgsqlDataReader reader = sqlgettype.ExecuteReader();
            while (reader.Read())
            {
                type = reader[0].ToString();
            }
            conn.Close();
            return type;
        }
        public string GetNameCardFromCards(string cardid)
        {
            string name = null;
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strgetname = "Select name from cards where cardid = @cardid";
            NpgsqlCommand sqlgetname = new NpgsqlCommand(strgetname, conn);
            sqlgetname.Parameters.AddWithValue("cardid", cardid);
            sqlgetname.Prepare();

            NpgsqlDataReader reader = sqlgetname.ExecuteReader();
            while (reader.Read())
            {
                name = reader[0].ToString();
            }
            conn.Close();
            return name;
        }
        public void ExecuteTransaction(string dealid, string dealuser, string otheruser, string othercard)
        {
            string dealcard = GetCardIdFromDeal(dealid);
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string updatecardother = "Update cards set fk_username = @otheruser where cardid = @dealcard";
            NpgsqlCommand sqlupdateother = new NpgsqlCommand(updatecardother, conn);

            sqlupdateother.Parameters.AddWithValue("otheruser", otheruser);
            sqlupdateother.Parameters.AddWithValue("dealcard", dealcard);
            sqlupdateother.Prepare();
            sqlupdateother.ExecuteNonQuery();

            string updatecarddeal = "Update cards set fk_username = @dealuser where cardid = @othercard";
            NpgsqlCommand sqlupdatedeal = new NpgsqlCommand(updatecarddeal, conn);

            sqlupdatedeal.Parameters.AddWithValue("dealuser", dealuser);
            sqlupdatedeal.Parameters.AddWithValue("othercard", othercard);
            sqlupdatedeal.Prepare();
            sqlupdatedeal.ExecuteNonQuery();

            conn.Close();
        }
        public string GetCardIdFromDeal(string dealid)
        {
            string cardid = null;
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strgetcardid = "Select cardtotrade from store where dealid = @dealid";
            NpgsqlCommand sqlgetcardid = new NpgsqlCommand(strgetcardid, conn);
            sqlgetcardid.Parameters.AddWithValue("dealid", dealid);
            sqlgetcardid.Prepare();

            NpgsqlDataReader reader = sqlgetcardid.ExecuteReader();
            while (reader.Read())
            {
                cardid = reader[0].ToString();
            }
            conn.Close();
            return cardid;
        }
        public string GetUserfromDeal(string dealid)
        {
            string user = null;
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strgetuser = "Select fk_username from store where dealid = @dealid";
            NpgsqlCommand sqlgetuser = new NpgsqlCommand(strgetuser, conn);
            sqlgetuser.Parameters.AddWithValue("dealid", dealid);
            sqlgetuser.Prepare();

            NpgsqlDataReader reader = sqlgetuser.ExecuteReader();
            while (reader.Read())
            {
                user = reader[0].ToString();
            }
            conn.Close();
            return user;
        }
        public void FillDeckOfUser(User user)
        {
            RequestContext req = new RequestContext();
            string id;
            string name;
            double damage;
            string elementType;
            string monsterType;

            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strselectcards = "Select cardid, name, damage from cards where fk_username = @username and indeck = 1";
            NpgsqlCommand sqlselectcards = new NpgsqlCommand(strselectcards, conn);
            sqlselectcards.Parameters.AddWithValue("username", user.Username);
            sqlselectcards.Prepare();
            NpgsqlDataReader reader = sqlselectcards.ExecuteReader();

            while(reader.Read())
            {
                id = reader.GetString(0);
                name = reader.GetString(1);
                damage = reader.GetDouble(2);
                elementType = req.GetElementTypeFromCardName(name);
                monsterType = req.GetMonsterTypeFromCardName(name);
                user.AddCardToDeckOfUser(id, name, damage, elementType, monsterType);
            }

        }
        public void SellCardFromStack(string cardtosell, string username)
        {
            string connstring = "Host=localhost;Username=postgres;Password=password;Database=MCTGdb;Port=5432";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            string strdelete = "Delete from cards where cardid = @cardtosell and fk_username = @username";
            NpgsqlCommand sqldelete = new NpgsqlCommand(strdelete, conn);

            sqldelete.Parameters.AddWithValue("cardtosell", cardtosell);
            sqldelete.Parameters.AddWithValue("username", username);
            sqldelete.Prepare();
            sqldelete.ExecuteNonQuery();

            int coinsold = GetCoinsFromUser(username);
            int coinsnew = coinsold + 1;

            string strupdate = "Update users set coins = @newcoins where username = @username";
            NpgsqlCommand sqlupdate = new NpgsqlCommand(strupdate, conn);
            sqlupdate.Parameters.AddWithValue("newcoins", coinsnew);
            sqlupdate.Parameters.AddWithValue("username", username);
            sqlupdate.Prepare();
            sqlupdate.ExecuteNonQuery();

            conn.Close();
        }
    }
}