using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Lecture 4
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;


public class client : MonoBehaviour
{

    public GameObject myCube;
    public GameObject otherCube;

    private static byte[] outBuffer = new byte[512];
    private static IPEndPoint remoteEP;
    private static Socket client_socket = null;

    //Lecture 5
    private float[] pos;
    float[] prevPos = new float[3];
    private byte[] bpos;

    bool portAssigned = false;

    public static IPAddress ip;
    public static void RunClient()
    {
        ip = IPAddress.Parse("127.0.0.1");//192.168.2.144");
        remoteEP = new IPEndPoint(ip, 11112);

    }

    // Start is called before the first frame update
    void Start()
    {
        //This must be changed

        RunClient();

        //Lecture 05
        pos = new float[] { myCube.transform.position.x, myCube.transform.position.y, myCube.transform.position.z };
        bpos = new byte[pos.Length * 4];
        Buffer.BlockCopy(pos, 0, bpos, 0, bpos.Length);
        //client_socket.SendTo(bpos, remoteEP);
        StartCoroutine(Downdate());

    }

    private void Update() 
    {
        if(GameManager.newUdpPort != 0 && !portAssigned)
        {
            try {
            //assign the port
            EndPoint remote = new IPEndPoint(ip, GameManager.newUdpPort);
            client_socket = new Socket(ip.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            client_socket.Bind(remote);
            client_socket.Blocking = false;

            portAssigned = true;
            }
            catch{}
        }
    }
    // Update is called once per frame
    // void Update()
    // {
    //     //outBuffer = Encoding.ASCII.GetBytes(myCube.transform.position.x.ToString());

    //     //Debug.Log("Sent X:" + myCube.transform.position.x);
     


        
    // }
    IEnumerator Downdate()
    {
        while(true)
        {
            if (client_socket == null)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }
            pos = new float[] {myCube.transform.position.x, myCube.transform.position.y, myCube.transform.position.z };
            // if(pos != prevPos){
                //prevPos = pos;
                Buffer.BlockCopy(pos, 0, bpos, 0, bpos.Length);
                client_socket.SendTo(bpos, remoteEP);
            //}
            //Attempting to get data from the server
            try
            {
                byte[] buffer = new byte[512];
                int rec = client_socket.Receive(buffer);
                pos = new float[rec / 4];
                Buffer.BlockCopy(buffer, 0, pos, 0, rec);
                otherCube.transform.position = new Vector3(pos[0], pos[1], pos[2]);
            }
			catch (SocketException er)
			{
				if (er.SocketErrorCode != SocketError.WouldBlock)
				{
					//write error
					//Debug.Log("Nothing recieved");
				}
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
