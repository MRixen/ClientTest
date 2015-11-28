using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication3
{
    public class Worker
    {
        string ip;
        int port;
        public bool isRunning = true;
        bool createStream = true;
        static TcpClient tcpClient = new TcpClient();
        static NetworkStream networkStream;   
        private IPAddress[] ipAddress;

        public Worker(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            ipAddress = Dns.GetHostAddresses(ip);    
        }

        public void DoWork()
        {
            try
            {
                Console.WriteLine(" >> Start client");
                tcpClient.Connect(ip, port);
                Console.WriteLine(" >> Connected");
                networkStream = tcpClient.GetStream();
            }
            catch (System.Net.Sockets.SocketException e)
            {
                Console.WriteLine(" >> Cant connect to server.\n");
                Console.WriteLine(" >> " + e.ToString());
            }
        }
        public void RequestStop()
        {
            isRunning = false;
        }

        public void closeClient()
        {
            tcpClient.Close();
            Console.WriteLine(" >> Client closed.");
        }

        public NetworkStream getNetworkStream()
        {
            return networkStream;
        }

        public TcpClient getClient()
        {
            return tcpClient;
        }
    }
}
