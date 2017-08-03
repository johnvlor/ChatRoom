using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
        public static Client client;
        TcpListener server;

        public Dictionary<string, TcpClient> clientList;
        string userName = null;

        public Server()
        {
            server = new TcpListener(IPAddress.Parse("127.0.0.1"), 9999);
            clientList = new Dictionary<string, TcpClient>();
            server.Start();
        }
        public void Run()
        {
            while (true)
            {
                AcceptClient();
                //Thread getMessageThread = new Thread(() => GetMessage());
                //getMessageThread.Start();
                //string message = client.Recieve();
                //Respond(message);
            }
        }
        private void AcceptClient()
        {
            Console.WriteLine("Listening for connections...");
            TcpClient clientSocket = default(TcpClient);
            
            while (true)
            {
                clientSocket = server.AcceptTcpClient();
                Console.WriteLine("Connected");
                Thread clientThread = new Thread(() => HandleClient(clientSocket));
                clientThread.Start();

            }
        }
        private void Respond(string body)
        {
             client.Send(body);
        }

        public void HandleClient(TcpClient clientSocket)
        {
            try
            {
                NetworkStream stream = clientSocket.GetStream();
                userName = GetUserName(stream);
                client = new Client(stream, clientSocket);
                AddClient(clientSocket);

                foreach (KeyValuePair<string, TcpClient> pair in clientList)
                {
                    Console.WriteLine(pair.Key + " - " + pair.Value);
                }

            }
            catch (Exception error)
            {
                Console.WriteLine("\n" + error.ToString());
            }

            Thread getMessageThread = new Thread(() => GetMessage());
            getMessageThread.Start();

            //Task.Run(() => GetMessage());
        }

        public void AddClient(TcpClient newClient)
        {
            clientList.Add(userName, newClient);
            Console.WriteLine("{0} has joined the chatroom.", userName);
        }

        public string GetUserName(NetworkStream stream)
        {
            byte[] message = System.Text.Encoding.ASCII.GetBytes("Enter your username?");
            stream.Write(message, 0, message.Length);
            byte[] userName = new Byte[256];
            stream.Read(userName, 0, userName.Length);
            string inputName = System.Text.Encoding.ASCII.GetString(userName).TrimEnd('\0');
            return inputName;
        }

        public void GetMessage()
        {
            while (true)
            {
                string message = client.Recieve();
                Respond(message);
            }
        }

    }
}
