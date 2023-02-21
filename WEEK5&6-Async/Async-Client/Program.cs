using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace AsyncClient
{
    class Program
    {
        private static Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private static byte[] buffer = new byte[512];

        // LEC6
        private static byte[] byPos;
        private static float[] fPos;

        static void Main(string[] args)
        {
            client.Connect(IPAddress.Parse("127.0.0.1"), 8888);

            Console.WriteLine("Connected to server");

            client.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(ReceiveCallBack), client);

            SyncSend();

            Console.Read();
        }

        private static void ReceiveCallBack(IAsyncResult results)
        {
            Socket socket = (Socket)results.AsyncState;
            int recv = socket.EndReceive(results);

            // LEC05
            //byte[] data = new byte[recv];
            //Array.Copy(buffer, data, recv);

            //String msg = Encoding.ASCII.GetString(data);
            //Console.WriteLine("Recv: " + msg);

            // LEC 06
            fPos = new float[recv / 4];
            Buffer.BlockCopy(buffer, 0, fPos, 0, recv);

            Console.WriteLine("[Client] Received X: {0}, Y: {1}, Z: {2}", fPos[0], fPos[1], fPos[2]);

            socket.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(ReceiveCallBack), socket);
        }

        private static void SyncSend()
        {
            int c = 0;

            // LEC 06
            float x, y, z = 0f;

            while (true)
            {
                // LEC 6
                x = y = ++z;

                fPos = new float[] { x, y, z };

                byPos = new byte[fPos.Length * 4];

                Buffer.BlockCopy(fPos, 0, byPos, 0, byPos.Length);

                // LEC 05
                //byte[] buffer = Encoding.ASCII.GetBytes(c.ToString());
                //client.Send(buffer);
                //c++;

                client.Send(byPos);
                Thread.Sleep(1000);
            }
        }

    }

}