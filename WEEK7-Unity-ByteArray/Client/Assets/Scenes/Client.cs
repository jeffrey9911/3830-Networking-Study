using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
using TMPro;

public class Client : MonoBehaviour
{
    public GameObject _cube;
    private static byte[] byPos;
    private static IPEndPoint remoteEP;
    private static Socket clientSocket;

    private float[] fPos;

    public Toggle moveDetect;
    public Toggle timeInterval;
    public TMP_InputField timeText;
    private float timer;

    public static void StartClient()
    {
        try
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            remoteEP = new IPEndPoint(ip, 8888);

            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);


        }
        catch(Exception exc)
        {
            Debug.Log(exc.ToString());
        }
    }

    private void SendPosition()
    {
        fPos = new float[] {_cube.transform.position.x, _cube.transform.position.y, _cube.transform.position.z };
        byPos = new byte[fPos.Length * 4];
        Buffer.BlockCopy(fPos, 0, byPos, 0, byPos.Length);

        clientSocket.SendTo(byPos, remoteEP);

        Debug.Log("Position Sent!");
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
        if(moveDetect.isOn)
        {
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                if(timeInterval.isOn)
                {
                    if(timer <= 0)
                    {
                        Debug.Log("Move Detected. Send position: " + _cube.transform.position);
                        SendPosition();
                        timer += float.Parse(timeText.text);
                    }
                    else
                    {
                        timer -= Time.deltaTime;
                    }
                }
                else
                {
                    Debug.Log("Move Detected. Send position: " + _cube.transform.position);
                    SendPosition();
                }

                
            }
        }
        else
        {
            if (timeInterval.isOn)
            {
                if (timer <= 0)
                {
                    Debug.Log("Send position: " + _cube.transform.position);
                    SendPosition();
                    timer += float.Parse(timeText.text);
                }
                else
                {
                    timer -= Time.deltaTime;
                }
            }
            else
            {
                Debug.Log("Send position: " + _cube.transform.position);
                SendPosition();
            }
        }
    }
}
