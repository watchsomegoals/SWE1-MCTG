using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml.Schema;
using System.Threading;

namespace MCTG
{
    class MyTcpListener
    {
        public static void Main()
        {
            TcpListener server = null;
            TcpClient client = default(TcpClient);
            
            Int32 port = 10001;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            server = new TcpListener(localAddr, port);
            try
            {
                server.Start();

                while (true)
                {
                    Console.WriteLine("Waiting for a connection... ");

                    client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!\n");

                    ClientHandler clientHandler = new ClientHandler();
                    clientHandler.startClient(client);

                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                client.Close();
                // Stop listening for new clients.
                server.Stop();
            }
            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
    }

    public class ClientHandler
    {
        TcpClient _client;
        public void startClient(TcpClient client)
        {
            _client = client;
            Thread t = new Thread(exchangeMessages);
            t.Start();
            
        }
        public void exchangeMessages()
        {
            RequestContext context = new RequestContext();
            NetworkStream stream = _client.GetStream();
            Console.WriteLine("waiting to sleep");
            Thread.Sleep(10000);

            byte[] bytes = new byte[256];
            string data = null;
            int i;
            while (true)
            {
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    //Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                    context.ReadContext(data);
                    context.HandleRequest();

                    data = context.ComposeResponse();
                    Console.WriteLine(data);

                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                    //Send back a response.
                    stream.Write(msg, 0, msg.Length);

                    if (!stream.DataAvailable)
                    {
                        break;
                    }
                }
            }
        }
    }
}
