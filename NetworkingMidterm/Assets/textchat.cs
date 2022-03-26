using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class textchat : MonoBehaviour
{
    Socket client1 = null;
    byte[] buffer = new byte[512];
    public void StartClient() 
    {

        //Setup our end point (server)
        try
        {
            //IPAddress ip = Dns.GetHostAddresses("mail.bigpond.com")[0];
            //you got to do this lol
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint server = new IPEndPoint(ip, 11111);

            //create out client socket 
            client1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //attempted a connection
            try
            {
                Debug.Log("Attempting Connection to server...");
                client1.Connect(server);
                Debug.Log("Connected to IP: " + client1.RemoteEndPoint.ToString());

                // //release the resource
                // client1.Shutdown(SocketShutdown.Both);
                // client1.Close();
            }
            catch
            {

            }
            client1.Blocking = false;
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
        try
        {
            try
            {
                int recieved = client1.Receive(buffer);
                Debug.Log("Recieved: " + Encoding.ASCII.GetString(buffer, 0, recieved));
            }
            catch(SocketException er)
            {
                if (er.SocketErrorCode != SocketError.WouldBlock) 
                {
                    //write error
                    Debug.Log("Nothing recieved");
                }
            }
            try
            {
                string sent = " ";
                byte[] msg = Encoding.ASCII.GetBytes(sent);

                Debug.Log("Sent: " + sent);
                client1.Send(msg);
            }
             catch(SocketException er)
            {
                if (er.SocketErrorCode != SocketError.WouldBlock) 
                {
                    //write error
                    Debug.Log("Nothing sent ");
                }
            }
        }
        catch (SocketException socke)
        {
            if (socke.SocketErrorCode != SocketError.WouldBlock) 
            {
              //write error
                Debug.Log("Error");

            }
        }
    }
    private void OnApplicationQuit() 
    {
        //release the resource
        client1.Shutdown(SocketShutdown.Both);
        client1.Close();
    }
}
