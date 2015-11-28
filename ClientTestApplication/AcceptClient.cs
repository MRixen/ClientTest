using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Net.Sockets;

namespace ConsoleApplication3
{
    class AcceptClient
    {
        public bool isRunning = true;
        static NetworkStream networkStream;
        static TcpClient clientSocket;
        static TcpListener serverSocket;
        int port = 8888;

        public void run()
        {
            IPAddress[] ipAddress = Dns.GetHostAddresses("192.168.137.1");
            serverSocket = new TcpListener(ipAddress[0], port);
            serverSocket.Start();
            Console.WriteLine(" >> Server Started");

            while (isRunning)
            {
                clientSocket = serverSocket.AcceptTcpClient();
                Console.WriteLine(" >> Accept connection from client");
                networkStream = clientSocket.GetStream();
            }

        }

        public void stopThread()
        {
            isRunning = false;
        }
    }
}
