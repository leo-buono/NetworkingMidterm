using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class GameManager : MonoBehaviour
	{

	//public string username;

	public int maxMessages = 25;

	public GameObject chatPanel, textObject, BeforeConnectionUI, AfterConnectionUI;
	public InputField chatBox, ipAddressBox;


	public Color playerMessage, Info;

	[SerializeField]
	List<Message> messageList = new List<Message>();

	Socket client1 = null;
	byte[] buffer = new byte[512];

	public static bool isConnected = false;
	public static IPAddress ip;
	public static int newUdpPort = 0;

	public void StartClient()
		{

		//Setup our end point (server)
		try
			{

			if (ipAddressBox.text != "")
				{

				Debug.Log("hey there bucko");
				string ipAddress = ipAddressBox.text.TrimEnd(' ');

				//IPAddress ip = Dns.GetHostAddresses("mail.bigpond.com")[0];
				//you got to do this lol
				ip = IPAddress.Parse(ipAddress);
				IPEndPoint server = new IPEndPoint(ip, 11111);

				//create out client socket 
				client1 = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				//attempted a connection
				try
					{
					Debug.Log("Attempting Connection to server...");
					client1.Connect(server);
					Debug.Log("Connected to IP: " + client1.RemoteEndPoint.ToString());
					isConnected = true;

					// //release the resource
					// client1.Shutdown(SocketShutdown.Both);
					// client1.Close();
					}
				catch
					{

					}

				ipAddressBox.text = "";
				BeforeConnectionUI.SetActive(false);
				AfterConnectionUI.SetActive(true);
				client1.Blocking = false;
				}
			else
				{
				if (!ipAddressBox.isFocused)
					{
					ipAddressBox.ActivateInputField();
					}
				}
			}
		catch
			{
			Debug.Log("Invalid IP address");
			ipAddressBox.text = "";
			}

		}

	// Start is called before the first frame update
	void Start()
		{
		BeforeConnectionUI.SetActive(true);
		AfterConnectionUI.SetActive(false);
		}

	// Update is called once per frame
	void Update()
		{

		try
			{
			try
				{
					if(isConnected)
					{
						int recieved = client1.Receive(buffer);
						string message = Encoding.Default.GetString(buffer, 0, recieved);
						if(message[0] == 'p' && message[1] == 'o' && message[2] == 'r' && message[3] == 't')
						{
							string portString = "";
							for (int i = 4; i < message.Length; i++)
							{
								//Get the port data into the thing
								portString += message[i];
								//Debug.Log(portString);
							}
							newUdpPort = int.Parse(portString);
						}
						else{
						SendMessageToChat(message, Message.MessageType.playerMessage);
						}
					}
				}
			catch (SocketException er)
				{
				if (er.SocketErrorCode != SocketError.WouldBlock)
					{
					//write error
					//Debug.Log("Nothing recieved");
					}
				}
			try
				{

				

				if (chatBox.text != "")
					{

					if (Input.GetKeyDown(KeyCode.Return))
						{
						try
							{
								if(isConnected)
								{
									string newMessage = chatBox.text;
									byte[] msg = Encoding.ASCII.GetBytes(newMessage);
									chatBox.text = "";
									Debug.Log(newMessage);
									client1.Send(msg);
										if(newMessage == "quit")
										{
											client1.Close();
											isConnected = false;
										}
								}
							}
						catch (SocketException er) 
							{
								if (er.SocketErrorCode != SocketError.WouldBlock)
								{
									//write error
									Debug.Log("Nothing sent ");
								}
							}
						}

					}
				else
					{

					if (!chatBox.isFocused && Input.GetKeyDown(KeyCode.Return) && isConnected)
						{
							chatBox.ActivateInputField();
						}
					}
				}
			catch (SocketException er)
				{
				if (er.SocketErrorCode != SocketError.WouldBlock)
					{
					//write error
					//Debug.Log("Nothing sent ");
					}
				}
			}
		catch (SocketException socke)
			{
			if (socke.SocketErrorCode != SocketError.WouldBlock)
				{
				//write error
				//Debug.Log("Error");

				}
			}
		}

	public void SendMessageToChat(string text, Message.MessageType messageType)
		{

		if (messageList.Count >= maxMessages)

			{

			Destroy(messageList[0].textObject.gameObject);
			messageList.Remove(messageList[0]);

			}

		Message newMessage = new Message();

		newMessage.text = text;

		GameObject newText = Instantiate(textObject, chatPanel.transform);

		newMessage.textObject = newText.GetComponent<Text>();

		newMessage.textObject.text = newMessage.text;
		
		newMessage.textObject.color = MessageTypeColor(messageType);

		messageList.Add(newMessage);

		}

	Color MessageTypeColor(Message.MessageType messageType)

		{

		Color color = Info;

		switch (messageType)
			{
			case Message.MessageType.playerMessage:
				color = playerMessage;
				break;

			}
		return color;

		}

	[System.Serializable]
	public class Message
		{

		public string text;
		public Text textObject;
		public MessageType messageType;

		public enum MessageType
			{
			playerMessage,
			info
			}

		}

	private void OnApplicationQuit()
		{
		//release the resource
		string sent = "quit";
        byte[] msg = Encoding.ASCII.GetBytes(sent);
		client1.Send(msg);
		client1.Shutdown(SocketShutdown.Both);
		client1.Close();
		}


	}
