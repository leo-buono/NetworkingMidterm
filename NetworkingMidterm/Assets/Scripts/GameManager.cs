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

	public GameObject chatPanel, textObject, BeforeConnectionUI, AfterConnectionUI;
	public InputField chatBox, ipAddressBox;

	public static IPAddress ip;

	public Color playerMessage, Info;

	[SerializeField]
	List<Message> messageList = new List<Message>();

	Socket client1 = null;
	byte[] buffer = new byte[512];

	public static bool isConnected= false;

	public void StartClient()
		{

		//Show server input UI

		try
			{

			//If they are inputting something 
			if (ipAddressBox.text != "")
				{

					Debug.Log("hey there bucko");
					string ipAddress = chatBox.text;
					IPAddress ip = IPAddress.Parse(ipAddress);
					IPEndPoint server = new IPEndPoint(ip, 11111);

					//create out client socket 
					client1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

					//attempted a connection

					Debug.Log("Attempting Connection to server...");
					client1.Connect(server);
					Debug.Log("Connected to IP: " + client1.RemoteEndPoint.ToString());
					client1.Blocking = false;
						// //release the resource
						// client1.Shutdown(SocketShutdown.Both);
						// client1.Close();

					BeforeConnectionUI.SetActive(false);
					AfterConnectionUI.SetActive(true);
					isConnected = true;
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

		if (isConnected == true)

			try
				{
				try
					{
					int recieved = client1.Receive(buffer);
					Debug.Log("Recieved: " + Encoding.ASCII.GetString(buffer, 0, recieved));
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
								string newMessage = username + ": " + chatBox.text;
								byte[] msg = Encoding.ASCII.GetBytes(newMessage);
								chatBox.text = "";
								Debug.Log(newMessage);
								client1.Send(msg);
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
