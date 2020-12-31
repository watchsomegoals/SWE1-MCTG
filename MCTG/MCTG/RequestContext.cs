﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using Npgsql;
using Npgsql.Replication.PgOutput.Messages;
using NpgsqlTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MCTG
{
    public class RequestContext
    {
        private string httpVerb;
        private string dirName;
        private string resourceID;
        private string protocol;
        private string payload;
        private IDictionary<string, string> headerData = new Dictionary<string, string>();

        private string statusCode = null;
        private string reasonPhrase = null;
        private string responseBody = null;

        public RequestContext() { }

        public void ReadContext(string data)
        {
            httpVerb = null;
            dirName = null;
            resourceID = null;
            protocol = null;
            payload = null;

            if (headerData.Count() != 0)
            {
                headerData.Clear();
            }

            string[] head;
            string[] resources;
            string[] lines = data.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            string[] firstLine;
            if (lines.Length > 1)
            {
                firstLine = lines[0].Split(new string[] { " " }, StringSplitOptions.None);
                if (firstLine.Length > 1)
                {
                    this.httpVerb = firstLine[0];
                    resources = firstLine[1].Split(new string[] { "/" }, StringSplitOptions.None);
                    if (resources.Length == 2)
                    {
                        this.dirName = resources[1];
                    }
                    else if (resources.Length == 3)
                    {
                        this.dirName = resources[1];
                        this.resourceID = resources[2];
                    }
                    this.protocol = firstLine[2];
                    if (httpVerb == "POST" || httpVerb == "PUT")
                    {
                        //Getting the payload 
                        for (int i = 0; i < lines.Length; i++)
                        {
                            if (lines[i] == "")
                            {
                                int start = Array.IndexOf(lines, "");
                                start++;
                                for (int j = start; j < lines.Length; j++)
                                {
                                    this.payload += lines[j];
                                    if (j != lines.Length - 1)
                                    {
                                        this.payload += "\n";
                                    }
                                }
                            }
                        }
                    }
                    for (int i = 1; i < lines.Length; i++)
                    {
                        if (lines[i] == "")
                        {
                            break;
                        }
                        head = lines[i].Split(new string[] { ": " }, StringSplitOptions.None);
                        headerData.Add(head[0], head[1]);
                    }
                    foreach (KeyValuePair<string, string> kvp in headerData)
                    {
                        Console.WriteLine("Key: {0}, Value: {1}", kvp.Key, kvp.Value);
                    }
                    Console.WriteLine("\n");
                    Console.WriteLine("httpVerb: {0}", this.httpVerb);
                    Console.WriteLine("dirName: {0}", this.dirName);
                    Console.WriteLine("resourceID: {0}", this.resourceID);
                    Console.WriteLine("protocol: {0}", this.protocol);
                    Console.WriteLine("httpPayLoad: {0}\n", this.payload);
                }

            }
        }

        public void HandleRequest()
        {
            statusCode = null;
            reasonPhrase = null;
            responseBody = null;

            if (string.Compare(httpVerb, "GET") != 0 && string.Compare(httpVerb, "POST") != 0 &&
               string.Compare(httpVerb, "PUT") != 0 && string.Compare(httpVerb, "DELETE") != 0)
            {
                statusCode = "501";
                reasonPhrase = "Not Implemented";
                responseBody = "\nRequest Not Implemented\n";
            }
            else if (string.Compare(this.protocol, "HTTP/1.1") != 0)
            {
                statusCode = "500";
                reasonPhrase = "Internal Server Error";
                responseBody = "\nWrong protocol, internal server error\n";
            }
            else if (string.Compare(httpVerb, "POST") == 0)
            {
                if (string.Compare(dirName, "users") == 0)
                {
                    RegisteringUser();
                }
                else if (string.Compare(dirName, "sessions") == 0)
                {
                    LoggingUser();
                }
                else if (string.Compare(dirName, "packages") == 0)
                {
                    CreatingPackages();

                }
                else if (string.Compare(dirName, "transactions") == 0 && string.Compare(resourceID, "packages") == 0)
                {
                    BuyingPackages();
                }
                else
                {
                    statusCode = "404";
                    reasonPhrase = "Not Found";
                    responseBody = "\nNot Found, wrong ressource name\n";
                }
            }
            else if (string.Compare(httpVerb, "GET") == 0)
            {
                if (string.Compare(dirName, "cards") == 0)
                {
                    ShowCards();
                }
                else if (string.Compare(dirName, "deck") == 0)
                {
                    ShowDeck();
                }
                else if(string.Compare(dirName, "deck?format=plain") == 0)
                {
                    ShowDeckDifferent();
                }
                else if(string.Compare(dirName, "users") == 0 && string.Compare(resourceID, null) != 0)
                {
                    DatabaseManager mycon = new DatabaseManager();
                    string key = "Authorization";
                    if (mycon.CheckUserAndToken(resourceID, headerData[key]))
                    {
                        if(mycon.CheckEditedDataExists(resourceID))
                        {
                            statusCode = "200";
                            reasonPhrase = "OK";
                            responseBody = "\n" + mycon.GetUserData(resourceID) + "\n";
                        }
                        else
                        {
                            statusCode = "404";
                            reasonPhrase = "Not Found";
                            responseBody = "\nNo data has been added for " + resourceID + "\n";
                        }
                    }
                    else
                    {
                        statusCode = "404";
                        reasonPhrase = "Not Found";
                        responseBody = "\nNo active session under given username and token\n";
                    }
                }
                else
                {
                    statusCode = "404";
                    reasonPhrase = "Not Found";
                    responseBody = "\nNot Found, wrong ressource name\n";
                }
            }
            else if (string.Compare(httpVerb, "PUT") == 0)
            {
                if (string.Compare(dirName, "deck") == 0)
                {
                    ConfigureDeck();
                }
                else if (string.Compare(dirName, "users") == 0 && string.Compare(resourceID, null) != 0)
                {
                    DatabaseManager mycon = new DatabaseManager();
                    string key = "Authorization";
                    if (mycon.CheckUserAndToken(resourceID, headerData[key]))
                    {
                        User user = JsonConvert.DeserializeObject<User>(payload);
                        mycon.EditUserData(resourceID, user.Name, user.Bio, user.Image);
                        statusCode = "200";
                        reasonPhrase = "OK";
                        responseBody = "\nUser data edited by " + resourceID + "\n";
                    }
                    else
                    {
                        statusCode = "404";
                        reasonPhrase = "Not Found";
                        responseBody = "\nNo active session under given username and token\n";
                    }
                }
                else
                {
                    statusCode = "404";
                    reasonPhrase = "Not Found";
                    responseBody = "\nNot Found, wrong ressource name\n";
                }
            }
            else if (string.Compare(httpVerb, "DELETE") == 0)
            {
                if (string.Compare(dirName, "messages") != 0)
                {
                    statusCode = "404";
                    reasonPhrase = "Not Found";
                    responseBody = "\nNot Found, wrong ressource name\n";
                }
                else if (string.Compare(resourceID, null) == 0)
                {
                    statusCode = "400";
                    reasonPhrase = "Bad Request";
                    responseBody = "\nBad request, resourceID necessary\n";
                }
                else
                {
                    Delete();
                }
            }
        }

        private void ShowDeckDifferent()
        {
            DatabaseManager mycon = new DatabaseManager();
            string key = "Authorization";
            if (headerData.ContainsKey(key))
            {
                if (mycon.CheckLoggedIn(headerData[key]))
                {
                    string username = mycon.GetUserLoggedIn(headerData[key]);
                    if (mycon.CheckIfDeckConfigured(username))
                    {
                        statusCode = "200";
                        reasonPhrase = "OK";
                        responseBody = "\n" + mycon.ShowDeckCardsDifferent(username) + "\n";
                    }
                    else
                    {
                        statusCode = "404";
                        reasonPhrase = "Not Found";
                        responseBody = "\nNo deck configured\n";
                    }
                }
                else
                {
                    statusCode = "404";
                    reasonPhrase = "Not Found";
                    responseBody = "\nLog in as a user to show all acquired cards\n";
                }

            }
            else
            {
                statusCode = "400";
                reasonPhrase = "Bad Request";
                responseBody = "\nSession token is missing\n";
            }
        }

        private void ShowDeck()
        {
            DatabaseManager mycon = new DatabaseManager();
            string key = "Authorization";
            if (headerData.ContainsKey(key))
            {
                if (mycon.CheckLoggedIn(headerData[key]))
                {
                    string username = mycon.GetUserLoggedIn(headerData[key]);
                    if (mycon.CheckIfDeckConfigured(username))
                    {
                        statusCode = "200";
                        reasonPhrase = "OK";
                        responseBody = "\n" + mycon.ShowDeckCards(username) + "\n";
                    }
                    else
                    {
                        statusCode = "404";
                        reasonPhrase = "Not Found";
                        responseBody = "\nNo deck configured\n";
                    }
                }
                else
                {
                    statusCode = "404";
                    reasonPhrase = "Not Found";
                    responseBody = "\nLog in as a user to show all acquired cards\n";
                }

            }
            else
            {
                statusCode = "400";
                reasonPhrase = "Bad Request";
                responseBody = "\nSession token is missing\n";
            }
        }

        public void ConfigureDeck()
        {
            DatabaseManager mycon = new DatabaseManager();
            string key = "Authorization";
            if (headerData.ContainsKey(key))
            {
                if (mycon.CheckLoggedIn(headerData[key]))
                {
                    string username = mycon.GetUserLoggedIn(headerData[key]);
                    string[] cardsid = payload.Split(new string[] { ", " }, StringSplitOptions.None);

                    if (cardsid.Length == 4)
                    {
                        for (int i = 0; i < cardsid.Length; i++)
                        {
                            cardsid[i] = cardsid[i].Replace("[", "");
                            cardsid[i] = cardsid[i].Replace("]", "");
                            cardsid[i] = cardsid[i].Replace("\\", "");
                            cardsid[i] = cardsid[i].Replace("\"", "");
                        }
                        int flag = 0;
                        for (int i = 0; i < cardsid.Length; i++)
                        {
                            if (mycon.CheckIfCardInDeck(cardsid[i], username))
                            {
                                flag = 1;
                            }
                        }
                        if (flag == 1)
                        {
                            statusCode = "400";
                            reasonPhrase = "Bad Request";
                            responseBody = "\nAt least one card already in deck\n";
                        }
                        else
                        {
                            int flag1 = 0;
                            for (int i = 0; i < cardsid.Length; i++)
                            {
                                if (!mycon.CheckIfCardBelongs(cardsid[i], username))
                                {
                                    flag1 = 1;
                                }
                            }
                            if (flag1 == 1)
                            {
                                statusCode = "400";
                                reasonPhrase = "Bad Request";
                                responseBody = "\nAt least one card does not belong to " + username + "\n";
                            }
                            else
                            {
                                mycon.ResetDeck(username);
                                for (int i = 0; i < cardsid.Length; i++)
                                {
                                    mycon.InsertCardInDeck(cardsid[i], username);
                                }
                                statusCode = "200";
                                reasonPhrase = "OK";
                                responseBody = "\n" + username + " successfully configured his deck\n";
                            }
                        }
                    }
                    else
                    {
                        statusCode = "400";
                        reasonPhrase = "Bad Request";
                        responseBody = "\nNot enough cards set\n";
                    }
                }
                else
                {
                    statusCode = "404";
                    reasonPhrase = "Not Found";
                    responseBody = "\nLog in as a user to configure deck\n";
                }
            }
            else
            {
                statusCode = "400";
                reasonPhrase = "Bad Request";
                responseBody = "\nSession token is missing\n";
            }
        }

        public void ShowCards()
        {
            DatabaseManager mycon = new DatabaseManager();
            string key = "Authorization";
            if (headerData.ContainsKey(key))
            {
                if (mycon.CheckLoggedIn(headerData[key]))
                {
                    string username = mycon.GetUserLoggedIn(headerData[key]);

                    statusCode = "200";
                    reasonPhrase = "OK";
                    responseBody = "\n" + mycon.ShowAllCards(username) + "\n";
                }
                else
                {
                    statusCode = "404";
                    reasonPhrase = "Not Found";
                    responseBody = "\nLog in as a user to show all acquired cards\n";
                }

            }
            else
            {
                statusCode = "400";
                reasonPhrase = "Bad Request";
                responseBody = "\nSession token is missing\n";
            }
        }

        public void BuyingPackages()
        {
            DatabaseManager mycon = new DatabaseManager();
            string key = "Authorization";
            if (mycon.CheckLoggedIn(headerData[key]))
            {
                string username = mycon.GetUserLoggedIn(headerData[key]);
                int coins = mycon.GetCoinsFromUser(username);
                if (coins >= 5)
                {
                    if (mycon.CheckPackageAvailability())
                    {
                        int packagedid = mycon.GetPackageidToBuy();
                        mycon.SpendCoins(username, 5);
                        mycon.TransferCardsFromPackageToUser(username, packagedid);
                        statusCode = "200";
                        reasonPhrase = "OK";
                        responseBody = "\nPackage with id " + packagedid + " successfully purchased\n";
                    }
                    else
                    {
                        statusCode = "400";
                        reasonPhrase = "Bad Request";
                        responseBody = "\nNot more packages left\n";
                    }
                }
                else
                {
                    statusCode = "400";
                    reasonPhrase = "Bad Request";
                    responseBody = "\nNot enough coins\n";
                }
            }
            else
            {
                statusCode = "404";
                reasonPhrase = "Not Found";
                responseBody = "\nLog in as a user to purchase packages\n";
            }
        }

        public void CreatingPackages()
        {
            DatabaseManager mycon = new DatabaseManager();
            int check = mycon.CheckUserForInsertPackage(headerData.ElementAt(4).Value);
            if (check == 0)
            {
                statusCode = "400";
                reasonPhrase = "Bad Request";
                responseBody = "\nOnly admin can create packages\n";
            }
            else if (check == 1)
            {
                statusCode = "404";
                reasonPhrase = "Not Found";
                responseBody = "\nLog in as admin before creating packages\n";
            }
            else if (check == 2)
            {
                int packageid = mycon.GetPackageidForInsertPackage();

                string[] cards = payload.Split(new string[] { "}," }, StringSplitOptions.None);
                for (int i = 0; i < cards.Length; i++)
                {
                    if (i != cards.Length - 1)
                    {
                        cards[i] += "}";
                    }
                    cards[i] = cards[i].Replace("[", "");
                    cards[i] = cards[i].Replace("]", "");
                    Monster card = JsonConvert.DeserializeObject<Monster>(cards[i]);
                    mycon.InsertCardPackage(card.Id, card.Name, card.Damage, packageid);
                    Console.WriteLine(cards[i] + "\n");
                }

                statusCode = "200";
                reasonPhrase = "OK";
                responseBody = "\nPackage with id " + packageid + " created\n";
            }
        }

        public void LoggingUser()
        {
            User user = JsonConvert.DeserializeObject<User>(payload);
            DatabaseManager mycon = new DatabaseManager();
            int result = mycon.LogInUser(user.Username, user.Password);
            if (result == 0)
            {
                statusCode = "404";
                reasonPhrase = "Not Found";
                responseBody = "\nInvalid credentials\n";
            }
            else if (result == 1)
            {
                statusCode = "403";
                reasonPhrase = "Forbidden";
                responseBody = "\nAlready logged in\n";
            }
            else if (result == 2)
            {
                statusCode = "200";
                reasonPhrase = "OK";
                responseBody = "\nSuccessfully logged in\n";
            }
        }

        public void RegisteringUser()
        {
            User user = JsonConvert.DeserializeObject<User>(payload);
            DatabaseManager mycon = new DatabaseManager();
            if (mycon.InsertUser(user.Username, user.Password))
            {
                statusCode = "200";
                reasonPhrase = "OK";
                responseBody = "\nUser created\n";
            }
            else
            {
                statusCode = "403";
                reasonPhrase = "Forbidden";
                responseBody = "\nUser already exists\n";
            }
        }

//-----------------------------------------------------------------------------------------------------------------------------

        public void Delete()
        {
            string path = Path.Combine(Environment.CurrentDirectory, dirName);
            if (Directory.Exists(path))
            {
                string filePath = Path.Combine(path, resourceID);
                filePath += ".txt";
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    statusCode = "200";
                    reasonPhrase = "OK";
                    responseBody = "\nFile deleted";
                }
                else
                {
                    statusCode = "404";
                    reasonPhrase = "Not Found";
                    responseBody = "\nNot Found, file does not exist";
                }
            }
            else
            {
                statusCode = "404";
                reasonPhrase = "Not Found";
                responseBody = "\nNot Found, file does not exist";
            }
        }

        public void Put()
        {
            string path = Path.Combine(Environment.CurrentDirectory, dirName);
            if (Directory.Exists(path))
            {
                string filePath = Path.Combine(path, resourceID);
                filePath += ".txt";
                if (File.Exists(filePath))
                {
                    File.WriteAllText(filePath, payload);
                    statusCode = "200";
                    reasonPhrase = "OK";
                    responseBody = "\nFile updated";
                }
                else
                {
                    statusCode = "404";
                    reasonPhrase = "Not Found";
                    responseBody = "\nNot Found, file does not exist";
                }
            }
            else
            {
                statusCode = "404";
                reasonPhrase = "Not Found";
                responseBody = "\nNot Found, file does not exist";
            }
        }

        public void GetByID()
        {
            string path = Path.Combine(Environment.CurrentDirectory, dirName);
            if (Directory.Exists(path))
            {
                string filePath = Path.Combine(path, resourceID);
                filePath += ".txt";
                if (File.Exists(filePath))
                {
                    responseBody = null;
                    responseBody += "\n";
                    responseBody += Path.GetFileName(filePath);
                    responseBody += "\n{";
                    responseBody += File.ReadAllText(filePath);
                    responseBody += "}\n";
                    statusCode = "200";
                    reasonPhrase = "OK";
                }
                else
                {
                    statusCode = "404";
                    reasonPhrase = "Not Found";
                    responseBody = "\nNot Found, file does not exist";
                }
            }
            else
            {
                statusCode = "404";
                reasonPhrase = "Not Found";
                responseBody = "\nNot Found, file does not exist";
            }
        }

        public void GetAll()
        {
            string path = Path.Combine(Environment.CurrentDirectory, dirName);
            if (Directory.Exists(path))
            {
                DirectoryInfo messagesDir = new DirectoryInfo(path);
                if (messagesDir.GetFiles("*.txt").Length > 0)
                {
                    string[] filePaths = Directory.GetFiles(path);
                    responseBody = null;
                    responseBody += "\n";
                    for (int i = 0; i < filePaths.Length; i++)
                    {
                        responseBody += Path.GetFileName(filePaths[i]);
                        responseBody += "\n{";
                        responseBody += File.ReadAllText(filePaths[i]);
                        responseBody += "}\n";
                        statusCode = "200";
                        reasonPhrase = "OK";
                    }
                }
                else
                {
                    statusCode = "404";
                    reasonPhrase = "Not Found";
                    responseBody = "\nNot Found, files do not exist";
                }
            }
            else
            {
                statusCode = "404";
                reasonPhrase = "Not Found";
                responseBody = "\nNot Found, files do not exist";
            }
        }

        public string ComposeResponse()
        {
            string response;
            response = $"{protocol} {statusCode} {reasonPhrase}\r\n";
            response += "Server: Caraba\r\n";
            response += "Content-Type: text/html\r\n";
            response += "Accept-Ranges: bytes\r\n";
            response += $"Content-Length: {responseBody.Length}\r\n";
            response += "\r\n";
            response += $"{responseBody}";
            response += "\r\n\r\n";
            return response;
        }

        public string HttpVerb
        {
            get { return httpVerb; }
            set { httpVerb = value; }
        }

        public string DirName
        {
            get { return dirName; }
            set { dirName = value; }
        }

        public string ResourceID
        {
            get { return resourceID; }
            set { resourceID = value; }
        }

        public string Protocol
        {
            get { return protocol; }
            set { protocol = value; }
        }

        public string Payload
        {
            get { return payload; }
            set { payload = value; }
        }

        public string StatusCode
        {
            get { return statusCode; }
            set { statusCode = value; }
        }

        public string ReasonPhrase
        {
            get { return reasonPhrase; }
            set { reasonPhrase = value; }
        }

        public string ResponseBody
        {
            get { return responseBody; }
            set { responseBody = value; }
        }
    }
}
