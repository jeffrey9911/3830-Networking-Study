// Blocking client
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

public class TCPClient
{
    static Socket clientSocket;

    private static bool isPrinting = false;
    private static string printContent = "";

    private static void taskPrint(string toPrint)
    {
        printContent = toPrint;
        isPrinting = true;
    }

    static void ClientReceive(Socket socketClient)
    {
        byte[] buffer = new byte[1024];

        while (true)
        {
            int recv = socketClient.Receive(buffer);

            if (recv == 0)
            {
                socketClient.Shutdown(SocketShutdown.Both);
                socketClient.Close();
                return;
            }

            taskPrint(Encoding.ASCII.GetString(buffer, 0, recv));
        }
    }

    public static void StartClient()
    {
        Console.WriteLine("[SYSTEM] This application is only for receiving message. \n[SYSTEM] Use Chat Messenger to send message.");
        byte[] buffer = new byte[1024];

        // Setup our end point (Server)

        try
        {
            Console.WriteLine("[INPUT] Please input the server IP / DNS:");
            string userConnect = Console.ReadLine();
            IPAddress ip;
            if(!IPAddress.TryParse(userConnect, out ip))
            {
                Console.WriteLine("[INPUT] DNS detected. Please input the index number:");
                ip = Dns.GetHostAddresses(userConnect)[Int16.Parse(Console.ReadLine())];
            }

            
            Console.WriteLine("[INPUT] Please input the server port:");
            int portNumber = Int32.Parse(Console.ReadLine());
            IPEndPoint serverEP = new IPEndPoint(ip, portNumber);

            // Setup our client socket
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


            try
            {
                // Atttempt a connection
                Console.WriteLine("[SYSTEM] Connecting to chat room");
                clientSocket.Connect(serverEP);
                clientSocket.Send(Encoding.ASCII.GetBytes("CHATROOM"));
                Console.WriteLine("[SYSTEM] Connected to server: {0}", clientSocket.RemoteEndPoint.ToString());

                try
                {
                    var messengerProcess = new Process();
                    messengerProcess.StartInfo = new ProcessStartInfo("TCP-Messenger.exe", ip.ToString().Trim() + " " + portNumber.ToString().Trim());
                    messengerProcess.Start();
                }
                catch(Exception exc)
                {
                    Console.WriteLine("[SYSTEM] Messenger not found. Please open it manually.");
                }
                

                Task.Run(() => { ClientReceive(clientSocket); });

                while (true)
                {
                    if (isPrinting)
                    {
                        Console.WriteLine(printContent);
                        isPrinting = false;
                    }
                }


            }
            catch (ArgumentNullException anExc)
            {
                Console.WriteLine("ArgumentNullException: {0}", anExc.ToString());
            }
            catch (SocketException sExc)
            {
                Console.WriteLine("SocketException: {0}", sExc);
            }
            catch (Exception exc)
            {
                Console.WriteLine("Unexpected exception: {0}", exc);
            }
        }
        catch (Exception exc)
        {
            Console.WriteLine("Exception: {0}", exc);
        }
    }



    public static int Main(String[] args)
    {
        StartClient();
        //Console.ReadKey();
        return 0;
    }
}