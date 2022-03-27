using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Lecture 4
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;

public class client : MonoBehaviour
	{

	public GameObject myCube, BeforeConnectionUI, AfterConnectionUI;

	private static byte[] outBuffer = new byte[512];
	private static IPEndPoint remoteEP;
	private static Socket client_socket;

	//Lecture 5
	private float[] pos;
	float[] prevPos = new float[3];
	private byte[] bpos;

	private bool firstTime = false;


	public void RunClient()
		{

		try
			{
			remoteEP = new IPEndPoint(GameManager.ip, 11112);
			client_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			}

		catch (SocketException er)
			{
			if (er.SocketErrorCode != SocketError.WouldBlock)
				{
				Debug.Log("Invalid IP address");
				}
			}


		}



	// Start is called before the first frame update
	void Start()
		{
		myCube = GameObject.Find("Cube");
		pos = new float[] { myCube.transform.position.x, myCube.transform.position.y, myCube.transform.position.z };
		bpos = new byte[pos.Length * 4];
		}

	// Update is called once per frame

	void Update()
		{

		if (GameManager.isConnected == true)
			{
			RunClient();
			firstTime = true;
			}
		else
			{

			if (firstTime) {
				//Lecture 05
				Buffer.BlockCopy(pos, 0, bpos, 0, bpos.Length);
				client_socket.SendTo(bpos, remoteEP);
				StartCoroutine(Downdate());
				firstTime = false;
				}	
			}
		}

	//outBuffer = Encoding.ASCII.GetBytes(myCube.transform.position.x.ToString());
	//Debug.Log("Sent X:" + myCube.transform.position.x);


	IEnumerator Downdate()
		{
		while (true)
			{
			pos = new float[] { myCube.transform.position.x, myCube.transform.position.y, myCube.transform.position.z };
			if (pos != prevPos)
				{
				prevPos = pos;
				Buffer.BlockCopy(pos, 0, bpos, 0, bpos.Length);
				client_socket.SendTo(bpos, remoteEP);
				}
			yield return new WaitForSeconds(0.01f);
			}
		}
	}
