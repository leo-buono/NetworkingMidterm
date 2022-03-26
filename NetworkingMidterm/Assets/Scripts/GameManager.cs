using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class GameManager : MonoBehaviour
	{

	public string username;

	public int maxMessages = 25;

	public GameObject chatPanel, textObject;
	public InputField chatBox;


	public Color playerMessage, Info;

	[SerializeField]
	List<Message> messageList = new List<Message>();

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
				//Debug.Log("Recieved: " + Encoding.ASCII.GetString(buffer, 0, recieved));
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

						string newMessage = username + ": " + chatBox.text;
						byte[] msg = Encoding.ASCII.GetBytes(newMessage);
						chatBox.text = "";
						Debug.Log(newMessage);
						client1.Send(msg);
						}

					}
				else
					{

					if (!chatBox.isFocused && Input.GetKeyDown(KeyCode.Return))
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
		client1.Shutdown(SocketShutdown.Both);
		client1.Close();
		}


	}
