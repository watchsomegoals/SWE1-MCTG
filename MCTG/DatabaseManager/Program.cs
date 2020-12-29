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
    class Program
    {
        static void Main(string[] args)
        {
            /*
            ConnectionDatabase mycon = new ConnectionDatabase();
            string user = "caraba";
            string pw = "password";
            if(!mycon.InsertUser(user, pw))
            {
                Console.WriteLine("User exists already!");
            }
            Console.ReadLine();
 
            ConnectionDatabase mycon = new ConnectionDatabase();
            string user = "kienboec";
            string pw = "daniel";
            Console.WriteLine(mycon.LogInUser(user, pw));
            Console.ReadLine();
            */

            ConnectionDatabase mycon = new ConnectionDatabase();
            Console.WriteLine(mycon.GetPackageidForInsertPackage());
            Console.ReadLine();

            /*
            ConnectionDatabase mycon = new ConnectionDatabase();
            Console.WriteLine(mycon.CheckUserForInsertPackage("Basic admin-mctgToken"));
            Console.ReadLine();
            

            ConnectionDatabase mycon = new ConnectionDatabase();
            string id = "firstid";
            string name = "BlueEyesWhiteDragon";
            double damage = 5.0;
            int packageid = 1;
            mycon.InsertCardPackage(id, name, damage, packageid);
            */
        }
    }
}
