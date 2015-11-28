using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// -------------------
// Client application
// -------------------

namespace ConsoleApplication3
{
    class Program
    {

        static NetworkStream networkStream;
        static Worker worker;
            static int cntr = 0;
            static bool firstStart = true;
            static int port = 8008;
            static string ip = "127.0.0.1";

        static void Main(string[] args)
        {
                if (firstStart)
                {
                    initSocket(ip, port);
                    firstStart = false;
                }          

            while (true)
            {
                try
                {
                    //receiveData();
                    sendData();
                    Thread.Sleep(1000);
                }
                catch (NullReferenceException e)
                {
                        worker.closeClient();
                        Environment.Exit(0);                   
                }
                
            }
        }

       static private void initSocket(String ip, int port){
            worker = new Worker(ip, port);
            Thread workerThread = new Thread(worker.DoWork);
            workerThread.Start();
        }

       static private void receiveData()
        {
            networkStream = worker.getNetworkStream();
            if (networkStream != null)
            {
                TcpClient tcpClient = worker.getClient();
                if(tcpClient != null){
                    Byte[] receiveBytes = new byte[10025];
                    networkStream.Read(receiveBytes, 0, (int)tcpClient.ReceiveBufferSize);
                    Console.WriteLine(" >> " + receiveBytes.Length + " bytes received.");
                    string returndata = System.Text.Encoding.ASCII.GetString(receiveBytes);
                    Console.Write(returndata);
                }

           }
        }

       static private void sendData()
       {
           networkStream = worker.getNetworkStream();
           if (networkStream != null)
           {
               TcpClient tcpClient = worker.getClient();
               if (tcpClient != null)
               {
                   string stringToSend = "TestMessage;1:1:3;";
                   Byte[] sendBytes = Encoding.ASCII.GetBytes(stringToSend);
                   networkStream.Write(sendBytes, 0, sendBytes.Length);
                   networkStream.Flush();
               }

           }
       }

    }
}
