// TCP Server (blocking mode)
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TCPServer
{
    private static Dictionary<string, Socket> socketDict = new Dictionary<string, Socket>();

    private static bool isPrinting = false;
    private static string printContent = "";

    private static void taskPrint(string toPrint)
    {
        printContent = toPrint;
        isPrinting = true;
    }

    public static void StartServer()
    {
        Console.WriteLine("[INPUT] Please input the private server IPv4 address:");
        IPAddress ip = IPAddress.Parse(Console.ReadLine());

        Console.WriteLine("[INPUT] Please input port number:");
        IPEndPoint serverEP = new IPEndPoint(ip, Int32.Parse(Console.ReadLine()));

        Socket server = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            server.Bind(serverEP);

            server.Listen(10);

            Console.WriteLine("[SYSTEM] Server has started");

            Task.Run(() => { ServerAccept(server); });

            while(true)
            {
                if (isPrinting)
                {
                    Console.WriteLine(printContent);
                    isPrinting = false;
                }
            }
        }
        catch (Exception exc)
        {
            Console.WriteLine(exc.ToString());
        }
    }

    static void ServerReceive(Socket newSocket, string clientName)
    {
        while (newSocket.Connected)
        {
            byte[] buffer = new byte[1024];
            //int recv = newSocket.Receive(data, 0, data.Length, SocketFlags.None);
            int recv = newSocket.Receive(buffer);

            if (!newSocket.Poll(0, SelectMode.SelectWrite))
            {
                taskPrint("[SYSTEM] " + clientName + ": " + "has left the server");
                socketDict.Remove(newSocket.RemoteEndPoint.ToString());

                newSocket.Shutdown(SocketShutdown.Both);
                newSocket.Close();
                return;
            }

            string msgReceived = Encoding.ASCII.GetString(buffer, 0, recv);

            byte[] msg = Encoding.ASCII.GetBytes(("[" + clientName + "]: " + msgReceived));

            foreach (string socketTag in socketDict.Keys)
            {
                socketDict[socketTag].Send(msg);
            }
        }
    }

    static void ServerAccept(Socket socket)
    {
        while (true)
        {
            Socket newSocket = socket.Accept();
            byte[] buffer = new byte[1024];
            int recv = newSocket.Receive(buffer);
            string clientName = Encoding.ASCII.GetString(buffer, 0, recv);
            socketDict.Add(newSocket.RemoteEndPoint.ToString(), newSocket);

            taskPrint("[SYSTEM] New client connected: " + clientName + " [" + newSocket.RemoteEndPoint.ToString() + "]");

            Task.Run(() => { ServerReceive(newSocket, clientName); });
        }
    }

    public static int Main(String[] args)
    {
        StartServer();
        return 0;
    }

}