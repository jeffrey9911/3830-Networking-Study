using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Net;
using System.Net.Sockets;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AsyncServer
{
    class Program
    {
        private static byte[] buffer = new byte[512];
        private static Socket server;

        private static byte[] outBuffer = new byte[512];

        private static string outMsg = "";

        private static float[] fPos;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting the server...");

            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress ip = IPAddress.Parse("127.0.0.1");

            server.Bind(new IPEndPoint(ip, 8888));

            server.Listen(10);

            server.BeginAccept(new AsyncCallback(AcceptCallBack), null);

            //Console.WriteLine("Client Connected! IP: ");

            

            Console.Read();
        }

        private static void AcceptCallBack(IAsyncResult result)
        {
            Socket client = server.EndAccept(result);

            Console.WriteLine("Client Connected! IP: ");

            client.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(ReceiveCallBack), client);
        }

        private static void ReceiveCallBack(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;
            int recv = socket.EndReceive(result);

            // Lec 05
            //byte[] data = new byte[recv];
            //Array.Copy(buffer, data, recv);

            // LEC 06

            fPos = new float[recv / 4];

            Buffer.BlockCopy(buffer, 0, fPos, 0, recv);

            socket.BeginSend(buffer, 0, buffer.Length, 0, new AsyncCallback(SendCallBack), socket);

            Console.WriteLine("[Server]Received X: {0}, Y: {1}, Z: {2}", fPos[0], fPos[1], fPos[2]);

            //string msg = Encoding.ASCII.GetString(data);

            //Console.WriteLine("Recv: " + msg);

            //outMsg = msg;

            //socket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallBack), socket);


            socket.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(ReceiveCallBack), socket);
        }

        private static void SendCallBack(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;
            socket.EndSend(result);


        }
    }
}