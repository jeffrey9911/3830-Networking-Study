using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine.SocialPlatforms;

using System.Threading.Tasks;

using TMPro;

public class Server : MonoBehaviour
{

    public GameObject _sphere;

    public TextMeshProUGUI textF;

    static Vector3 spherePos = new Vector3(0, 0, 0);
    static Socket server;

    static EndPoint remoteClient;

    public static void StartServer()
    {
        

        IPHostEntry hostInfo = Dns.GetHostEntry(Dns.GetHostName());

        IPAddress ip = IPAddress.Parse("192.168.2.29");

        Debug.Log("Server name: " + hostInfo.HostName + ", IP:" + ip);

        IPEndPoint localEP = new IPEndPoint(ip, 12222);

        server = new Socket(ip.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

        remoteClient = new IPEndPoint(IPAddress.Any, 0);

        server.Bind(localEP);

        Debug.Log("Waiting for data...");

        Task.Run(() => { ServerRecieve(); });

    }

    static void ServerRecieve()
    {

        try
        {
            byte[] buffer = new byte[512];
            while (true)
            {
                

                int recv = server.ReceiveFrom(buffer, ref remoteClient);

                string str = Encoding.ASCII.GetString(buffer, 0, recv).Replace("(", "").Replace(")", "");
                string[] components = str.Split(",");

                spherePos = new Vector3(float.Parse(components[0]), float.Parse(components[1]), float.Parse(components[2]));

                
            }
            // Server shutdown

        }
        catch (Exception exc)
        {
            Debug.Log(exc.ToString());
        }
    }

    private void Awake()
    {
        Debug.Log("");
    }
    // Start is called before the first frame update
    void Start()
    {
        StartServer();
    }

    // Update is called once per frame
    void Update()
    {
        textF.text = "Position received: " + spherePos;
        _sphere.transform.position = spherePos;
    }
}
