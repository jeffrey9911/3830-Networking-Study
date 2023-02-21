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

    static Vector3 spherePos = new Vector3(0, 0, 0);

    static Socket serverSocket;

    static EndPoint remoteClientEP;

    private static byte[] buffer = new byte[1024];

    private static float[] fPos;

    public static void StartServer()
    {

        IPAddress ip = IPAddress.Parse("127.0.0.1");

        IPEndPoint localEP = new IPEndPoint(ip, 8888);

        serverSocket = new Socket(ip.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

        remoteClientEP = new IPEndPoint(IPAddress.Any, 0);

        serverSocket.Bind(localEP);

        serverSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref remoteClientEP, new AsyncCallback(ServerRecieve), serverSocket);

        Debug.Log("Waiting for connect...");
    }

    static void ServerRecieve(IAsyncResult result)
    {
        Socket socket = (Socket)result.AsyncState;

        int recv = socket.EndReceive(result);

        fPos = new float[recv / 4];

        Buffer.BlockCopy(buffer, 0, fPos, 0, recv);

        spherePos.x = fPos[0];
        spherePos.y = fPos[1];
        spherePos.z = fPos[2];

        Debug.Log("Received Position: " + fPos[0] + " " + fPos[1] + " " + fPos[2]);

        socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref remoteClientEP, new AsyncCallback(ServerRecieve), socket);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartServer();
    }

    private void Awake()
    {
        Debug.Log(" ");
    }

    // Update is called once per frame
    void Update()
    {
        _sphere.transform.position = spherePos;
    }
}
