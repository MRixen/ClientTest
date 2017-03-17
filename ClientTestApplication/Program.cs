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

        static NetworkStream networkStream_server_send, networkStream_server_receive;
        static Worker worker_server_send, worker_server_receive;
        static int cntr = 0;
        static bool firstStart = true;
        static int port_server_send = 4001;
        static int port_server_receive = 4002;
        static string ip = "127.0.0.1";
        static Byte[] sendBytes = new Byte[4];
        static int DELAY_TIME_THREAD = 10; // ms
        static Stopwatch timer;
        static long MAX_ELAPSED_TIME = 3000;

        static Byte[] recBytes = new Byte[8];

        static void Main(string[] args)
        {
            initSocket(ip, port_server_send, port_server_receive);

            timer = new Stopwatch();

            while (true)
            {
                try
                {
                    recBytes = receiveData();
                    // ----------------
                    // FOR TESTING
                    // ----------------
                    //if ((timer.ElapsedMilliseconds >= MAX_ELAPSED_TIME) & (recBytes[0] != 0))
                    //{
                    //    // Reset task number to 0 after completing task
                    //    // To simulate this behaviour this is done after a specific amount of time
                    //    sendBytes[0] = 0x00;
                    //    sendBytes[1] = recBytes[1];
                    //    sendData(sendBytes);
                    //}
                    if (recBytes[0] == 2) // Save ref pos
                    {
                        //if (!timer.IsRunning) timer.Start();

                        // Send back motor angle when task 2 comes in
                        sendBytes[0] = recBytes[0]; // Task number
                        sendBytes[1] = recBytes[1]; // Motor id
                        byte[] motorAngle = BitConverter.GetBytes(360);
                        sendBytes[2] = motorAngle[0]; // Motor angle
                        sendBytes[3] = motorAngle[1]; // Motor angle
                        // Send data back to server only when the task number is not 0
                        sendData(sendBytes);
                    }
                    //else if ((timer.ElapsedMilliseconds >= MAX_ELAPSED_TIME))
                    //{
                    //    timer.Reset();
                    //}
                    // ----------------
                    // ----------------
                    
                    Thread.Sleep(DELAY_TIME_THREAD);
                }
                catch (NullReferenceException e)
                {
                    worker_server_send.closeClient();
                    Environment.Exit(0);
                }

            }

            if (timer.IsRunning) timer.Stop();
        }

        static private void initSocket(String ip, int port_sever_send, int port_sever_receive)
        {
            worker_server_send = new Worker(ip, port_sever_send);
            Thread workerThread_server_send = new Thread(worker_server_send.DoWork);
            workerThread_server_send.Start();

            worker_server_receive = new Worker(ip, port_sever_receive);
            Thread workerThread_server_receive = new Thread(worker_server_receive.DoWork);
            workerThread_server_receive.Start();
        }

        static private Byte[] receiveData()
        {
            networkStream_server_send = worker_server_send.getNetworkStream();
            Byte[] receiveBytes = new byte[8];
            if (networkStream_server_send != null)
            {
                TcpClient tcpClient = worker_server_send.getClient();
                if (tcpClient != null)
                {
                    networkStream_server_send.Read(receiveBytes, 0, 8);
                    //Console.WriteLine(" >> " + receiveBytes.Length + " bytes received.");
                    short tasknumber = receiveBytes[0];
                    short motorId = receiveBytes[1];
                    short motorVel = BitConverter.ToInt16(receiveBytes, 2);
                    short motorPos = BitConverter.ToInt16(receiveBytes, 4);
                    //string returndata = System.Text.Encoding.ASCII.GetString(receiveBytes);
                    Console.WriteLine("tasknumber (receive): " + tasknumber);
                    //Console.WriteLine("motorId: " + motorId);
                    //Console.WriteLine("motorVel: " + motorVel);
                    //Console.WriteLine("motorPos: " + motorPos);

                }

            }
            return receiveBytes;
        }

        static private void sendData(Byte[] data)
        {
            networkStream_server_receive = worker_server_receive.getNetworkStream();
            if (networkStream_server_receive != null)
            {
                TcpClient tcpClient = worker_server_receive.getClient();
                if (tcpClient != null)
                {
                    //string stringToSend = "TestMessage;1:1:3;";
                    //Byte[] sendBytes = Encoding.ASCII.GetBytes(stringToSend);
                    networkStream_server_receive.Write(data, 0, data.Length);
                    networkStream_server_receive.Flush();
                }

            }
        }

    }
}
