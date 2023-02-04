using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class Client : MonoBehaviour
{
    public GameObject _cube;
    private static byte[] outBuffer = new byte[512];
    private static IPEndPoint remoteEP;
    private static Socket clientSocket;

    public static void StartClient()
    {
        try
        {
            IPAddress ip = IPAddress.Parse("192.168.2.29");
            remoteEP = new IPEndPoint(ip, 12222);

            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);


        }
        catch(Exception exc)
        {
            Debug.Log(exc.ToString());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _cube = GameObject.Find("Cube");
        StartClient();
    }

    // Update is called once per frame
    void Update()
    {
        outBuffer = Encoding.ASCII.GetBytes(_cube.transform.position.ToString());
        clientSocket.SendTo(outBuffer, remoteEP);
    }
}
