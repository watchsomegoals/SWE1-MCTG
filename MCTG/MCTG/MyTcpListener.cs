﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Xml.Schema;

namespace MCTG
{
    class MyTcpListener
    {
        public static void Main()
        {
            RequestContext context = new RequestContext();
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                byte[] bytes = new byte[256];
                string data = null;

                //Enter the listening loop.
                while (true)
                {
                    Console.WriteLine("Waiting for a connection... ");

                    // Perform a blocking call to accept requests
                    // You could also use server.AcceptTcpClient();
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!\n");

                    data = null;

                    //Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
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
                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
    }
}
