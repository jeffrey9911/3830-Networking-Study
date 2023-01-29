// Blocking client
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public class TCPClientMessenger
{
    private static string _ipAddress = "";
    private static string _ipPort = "";
    public static void StartClient()
    {
        Console.WriteLine("[SYSTEM] This application is only for sending message. \n[SYSTEM] Use Chat Room to receive message.");
        byte[] buffer = new byte[1024];

        // Setup our end point (Server)

        try
        {

            IPAddress ip;
            IPEndPoint serverEP;
            if (_ipAddress != "" && _ipPort != "")
            {
                Console.WriteLine("[SYSTEM] Server end point detected: {0}:{1}", _ipAddress, _ipPort);
                ip = IPAddress.Parse(_ipAddress);
                serverEP = new IPEndPoint(ip, Int32.Parse(_ipPort));
            }
            else
            {
                Console.WriteLine("[INPUT] Please input the server IP address:");
                //IPAddress ip = Dns.GetHostAddresses("mail.bigpond.com")[0]; // DNS will translate a URL to an IP
                ip = IPAddress.Parse(Console.ReadLine());
                Console.WriteLine("[INPUT] Please input the server port:");
                serverEP = new IPEndPoint(ip, Int32.Parse(Console.ReadLine()));
            }



            Console.WriteLine("[INPUT] Please input your name:");
            string clientName = Console.ReadLine();

            // Setup our client socket
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


            try
            {
                // Atttempt a connection
                Console.WriteLine("[SYSTEM] Connecting to chat room");
                client.Connect(serverEP);
                client.Send(Encoding.ASCII.GetBytes(clientName));
                Console.WriteLine("[SYSTEM] {0}, Welcome to chat room. Connected to server: {1}", clientName, client.RemoteEndPoint.ToString());

                while (true)
                {
                    Console.WriteLine("[INPUT] {0}, Please edit your message below:", clientName);
                    string content = Console.ReadLine();
                    client.Send(Encoding.ASCII.GetBytes(content));
                    Console.Clear();
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
        if (args.Length != 0)
        {
            _ipAddress = args[0];
            _ipPort = args[1];
        }
        StartClient();
        //Console.ReadKey();
        return 0;
    }

}