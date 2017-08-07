using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Server
{
    class Client
    {
        NetworkStream stream;
        TcpClient client;
        public string UserId;
        string userName;

        public Client(NetworkStream Stream, TcpClient Client, string userName)
        {
            stream = Stream;
            client = Client;
            UserId = "495933b6-1762-47a1-b655-483510072e73";
            this.userName = userName;
            Thread receiveMessage = new Thread(() => Recieve());
            receiveMessage.Start();
        }

        public void Send(string Message)
        {
            byte[] message = Encoding.ASCII.GetBytes(Message);
            stream.Write(message, 0, message.Count());
            stream.Flush();
        }

        public string Recieve()
        {
            while (true)
            {
                byte[] recievedMessage = new byte[256];
                stream.Read(recievedMessage, 0, recievedMessage.Length);
                string recievedMessageString = Encoding.ASCII.GetString(recievedMessage).TrimEnd('\0');
                Console.WriteLine(userName + ": " + recievedMessageString);
                //Server.messageList.Enqueue(recievedMessageString);
                return (userName + ": " + recievedMessageString);
            }
        }

    }
}
