using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class textchat : MonoBehaviour
{

     public static void StartClient() 
    {
        byte[] buffer = new byte[512];

        //Setup our end point (server)
        try
        {
            //IPAddress ip = Dns.GetHostAddresses("mail.bigpond.com")[0];
            //you got to do this lol
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint server = new IPEndPoint(ip, 11111);

            //create out client socket 
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //attempted a connection
            try
            {
                Debug.Log("Attempting Connection to server...");
                client.Connect(server);
                Debug.Log("Connected to IP: " + client.RemoteEndPoint.ToString());
                int recieved = client.Receive(buffer);

                Debug.Log("Recieved: " + Encoding.ASCII.GetString(buffer, 0, recieved));

                string sent = "Literally Gaming";
                byte[] msg = Encoding.ASCII.GetBytes(sent);

                Debug.Log("Sent: " + sent);
                client.Send(msg);

                //release the resource
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
            catch 
            {
            }
        }
        catch 
        { 
            
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        //Change this to only when IP is inputed
        StartClient();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
