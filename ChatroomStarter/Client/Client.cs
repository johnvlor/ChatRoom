using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Client
    {
        TcpClient clientSocket;
        NetworkStream stream;

        public Client(string IP, int port)
        {
            clientSocket = new TcpClient();
            clientSocket.Connect(IPAddress.Parse(IP), port);
            stream = clientSocket.GetStream();

        }
        public void Send()
        {
            while (clientSocket.Connected == true)
            {
                string messageString = UI.GetInput();
                byte[] message = Encoding.ASCII.GetBytes(messageString);
                stream.Write(message, 0, message.Count());
                stream.Flush();
            }
        }
        public void Recieve()
        {
            while (true)
            {
                byte[] recievedMessage = new byte[256];
                stream.Read(recievedMessage, 0, recievedMessage.Length);
                UI.DisplayMessage(Encoding.ASCII.GetString(recievedMessage).TrimEnd('\0'));
                stream.Flush();
            }
        }

        public void RunChat()
        {
            //TcpClient clientSocket = new TcpClient();
            //try
            //{
            //    clientSocket.Connect(IPAddress.Parse("127.0.0.1"), 9999);
            //    Console.WriteLine("User Connected");
            //}
            //catch
            //{
            //    Console.WriteLine("Failed to connect to server");
            //    return;
            //}

            //stream = clientSocket.GetStream();

            Thread receiveMessage = new Thread(Recieve);
            receiveMessage.Start();
            Thread sendMessage = new Thread(Send);
            sendMessage.Start();

        }

    }
}
