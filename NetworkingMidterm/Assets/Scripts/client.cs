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

    private bool portAssigned = false;
    private bool firstTime = false;

    public static void RunClient()
    {
        try
            {
            remoteEP = new IPEndPoint(GameManager.ip, 11112);
            client_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            }

        catch 
            {
                Debug.Log("Invalid IP address");
            }
    }

    // Start is called before the first frame update
    void Start()
    {
        //This must be changed

   

        //Lecture 05
        pos = new float[] { myCube.transform.position.x, myCube.transform.position.y, myCube.transform.position.z };
        bpos = new byte[pos.Length * 4];

    }

    private void Update() 
    {
        if (GameManager.isConnected == true)
            {
            RunClient();
            firstTime = true;
            }

        if (GameManager.newUdpPort != 0 && !portAssigned && firstTime==true)
        {
            //assign the port
            EndPoint remote = new IPEndPoint(GameManager.ip, GameManager.newUdpPort);
            client_socket = new Socket(GameManager.ip.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            client_socket.Bind(remote);
            client_socket.Blocking = false;

            firstTime = false;
            portAssigned = true;

            Buffer.BlockCopy(pos, 0, bpos, 0, bpos.Length);
            //client_socket.SendTo(bpos, remoteEP);
            StartCoroutine(Downdate());
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
