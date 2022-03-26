using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
//public class UDPServer
//{
//    public static void RunServer()
//    {
//        byte[] buffer = new byte[512];
//        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
//        IPAddress ip = host.AddressList[1];

//        Console.WriteLine("Server name: {0}  IP: {1}", host.HostName, ip);
//        IPEndPoint localEP = new IPEndPoint(ip, 11111);
//        Socket server = new Socket(ip.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
//        IPEndPoint client = new IPEndPoint(IPAddress.Any, 0); // 0 for any available port
//        EndPoint remoteClient = (EndPoint)client;
//        try
//        {
//            server.Bind(localEP);
//            Console.WriteLine("Waiting for data...");
//            while (true)
//            {
//                int rec = server.ReceiveFrom(buffer, ref remoteClient);
//                Console.WriteLine("Client: {0}  | Data: {1}", remoteClient.ToString(), Encoding.ASCII.GetString(buffer, 0, rec));
//                Console.WriteLine("Waiting for data...");
//            }
//            //server.Shutdown(SocketShutdown.Both);
//            //server.Close();
//        }
//        catch (Exception e)
//        {
//            Console.WriteLine(e.ToString());
//        }
//    }
//    public static int Main(String[] args)
//    {
//        RunServer();
//        Console.Read();
//        return 0;
//    }
//}

public class User 
{
    public Socket handler;
    public EndPoint remoteEP;
    public string username;

    public User(Socket handler, string username)
    {
        this.handler = handler;
        this.remoteEP = (EndPoint)handler.RemoteEndPoint;
        this.username = username;
    }
}
public class TCPServer 
{
    List<User> userList = new List<User>();
    public void StartServer() 
    {
        Console.ReadKey();
        byte[] buffer = new byte[512];
        IPHostEntry hostInfo = Dns.GetHostEntry(Dns.GetHostName()); 
        IPAddress ip = IPAddress.Parse("127.0.0.1");

        IPEndPoint localEP = new IPEndPoint(ip, 11111);

        Socket server = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        Socket UDPSocket  = new Socket(ip.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        //UDP
        IPEndPoint client = new IPEndPoint(ip, 11112);
        EndPoint remoteClient = (EndPoint)client;

        try
        {
                server.Bind(localEP);
                UDPSocket.Bind(client);
                server.Listen(10);
                Console.WriteLine("Waiting for Connection...");
                Socket handler = server.Accept(); //I like playing with fire
                Console.WriteLine("Client Connected.");
                IPEndPoint clientEP = (IPEndPoint)handler.RemoteEndPoint;
                Console.WriteLine("Client {0} connected at port {1}", clientEP.Address, clientEP.Port);
                handler.Blocking = false;
                userList.Add(new User(handler, "Player 1"));

            server.Blocking = false;
            UDPSocket.Blocking = false;
            while (true)
            {
                //TCP
                try
                {
                    //only expecting two players
                    if (userList.Count < 2)
                    {
                        handler = server.Accept();
                        Console.WriteLine("Client Connected.");
                        clientEP = (IPEndPoint)handler.RemoteEndPoint;
                        Console.WriteLine("Client {0} connected at port {1}", clientEP.Address, clientEP.Port);
                        //send data 
                        byte[] msg = Encoding.ASCII.GetBytes("Accepted");
                        handler.Send(msg);
                        //Because we are only expecting two players we can get away with this naming scheme
                        handler.Blocking = false;
                        userList.Add(new User(handler, "Player 2"));

                        //server.Blocking = false;
                        //UDPSocket.Blocking = false;
                    }
                }
                catch (SocketException socke)
                {
                    if (socke.SocketErrorCode != SocketError.WouldBlock)
                    {
                        Console.WriteLine("Socket Error");
                    }
                }
                for (int i = 0; i < userList.Count; i++)
                {
                    UDPTCP(ref buffer, ref UDPSocket, ref userList[i].remoteEP, ref userList[i].handler, userList[i]);
                }
            }
            //handler.Shutdown(SocketShutdown.Both);
        }
            catch (SocketException sockeet)
            {
                if (sockeet.SocketErrorCode != SocketError.WouldBlock)
                {
                    Console.WriteLine("Error");
                }
            }
    }

    private void UDPTCP(ref byte[] buffer, ref Socket UDPSocket,
        ref EndPoint remoteClient, ref Socket handler, User player)
    {
        //Create a loop so that you can loop through all the users that are connected and do something about it 
        try
        {
            //Ok so it's waiting here to recieve something. I should make sure the socket is nonblocking because that could be a problem
            int recieved = handler.Receive(buffer);

            Console.WriteLine("Recieved: {0}", Encoding.ASCII.GetString(buffer, 0, recieved));
            //It recieved a thing! Great, now send it off to both clients
            byte[] msg = Encoding.ASCII.GetBytes(player.username + " " + recieved);
            foreach (User t in userList)
            {
                t.handler.Send(msg);
            }
        }
        catch (SocketException socke)
        {
            if (socke.SocketErrorCode != SocketError.WouldBlock)
            {
                Console.WriteLine("Socket Error");
            }
        }
        //UDP
        //Clean the buffer so there is no garbage 
        buffer = new byte[512];
        try
        {
            int rec = UDPSocket.ReceiveFrom(buffer, ref remoteClient);
            if (rec != 0)
            {
                //send the data back to the other computer


            }
        }
        catch (SocketException socke)
        {
            if (socke.SocketErrorCode != SocketError.WouldBlock)
            {
                Console.WriteLine("Socket Error");
            }
        }
    }

    public static int Main(String[] args) 
    {
        TCPServer lmao = new TCPServer();
        lmao.StartServer();
        Console.ReadKey();
        return 0;
    }
}

//namespace ServingYourOwnGamesItsEasierThanYouThink
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            Console.WriteLine("Hello World!");
//        }
//    }
//}
