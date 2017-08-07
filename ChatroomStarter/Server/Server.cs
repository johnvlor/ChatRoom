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
        //public static Client client;
        TcpListener server;

        public Dictionary<string, TcpClient> clientList;
        public static Queue<string> messageList;
        string userName = null;

        public Server()
        {
            server = new TcpListener(IPAddress.Parse("127.0.0.1"), 9999);
            clientList = new Dictionary<string, TcpClient>();
            messageList = new Queue<string>();
            server.Start();
        }
        public void Run()
        {
            Thread clientThread = new Thread(() => AcceptClient());
            clientThread.Start();
        }
        public void AcceptClient()
        {
            Console.WriteLine("Listening for connections...");
            
            while (true)
            {               
                TcpClient clientSocket = default(TcpClient);
                clientSocket = server.AcceptTcpClient();
                Console.WriteLine("Connected");

                Task.Run(()=>CheckForBroadcast());
                Thread clientThread = new Thread(() => HandleClient(clientSocket));
                clientThread.Start();
            }
        }
        public static void Respond(Client client, string body)
        {
            client.Send(body);
        }

        public void HandleClient(TcpClient clientSocket)
        {
            try
            {
                NetworkStream stream = clientSocket.GetStream();

                userName = GetUserName(stream);
                Client client = new Client(stream, clientSocket, userName);

                AddToClientList(clientSocket);
                string message = "";
                AddMessageToQueue(message = userName + " has joined the chatroom.");

                Thread clientThread = new Thread(() => GetMessage(client, userName));
                clientThread.Start();
            }
            catch (Exception error)
            {
                Console.WriteLine("\n" + error.ToString());
            }

        }

        public void AddToClientList(TcpClient clientSocket)
        {
            clientList.Add(userName, clientSocket);
            Console.WriteLine("{0} has joined the chatroom from server.", userName);
        }

        public void AddMessageToQueue(string message)
        {
            messageList.Enqueue(message);
        }

        public string GetUserName(NetworkStream stream)
        {
            byte[] message = System.Text.Encoding.ASCII.GetBytes("Welcome to the ChatRoom!\nEnter your username?");
            stream.Write(message, 0, message.Length);
            byte[] userName = new Byte[256];
            stream.Read(userName, 0, userName.Length);
            string inputName = System.Text.Encoding.ASCII.GetString(userName).TrimEnd('\0');
            return inputName;
        }

        public void GetMessage(Client client, string userName)
        {
            while (true)
            {
                string message = client.Recieve();
                AddMessageToQueue(message);
                Respond(client, message);
            }
        }

        public void CheckForBroadcast()
        {
            while (true)
            {
                if (messageList.Count > 0 && clientList.Count > 0)
                {
                    BroadcastMessage(userName);
                }
            }
        }

        public void BroadcastMessage(string userName)
        {
            while (messageList.Count > 0)
            {
                //string message = messageList.Dequeue();
                byte[] byteMessage = null;
                try
                {
                    byteMessage = System.Text.Encoding.ASCII.GetBytes(messageList.Dequeue());
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
                foreach (KeyValuePair<string, TcpClient> clientUser in clientList)
                {
                    if (userName != clientUser.Key)
                    {
                        NetworkStream stream = clientUser.Value.GetStream();
                        stream.Write(byteMessage, 0, byteMessage.Length);
                        stream.Flush();
                    }
                }
            }
        }

    }
}
