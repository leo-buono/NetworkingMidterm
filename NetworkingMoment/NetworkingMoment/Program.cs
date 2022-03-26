//Sync client
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
//public class UDPClient
//{
//    public static void StartClient()
//    {
//        byte[] buffer = new byte[512];
//        try
//        {
//            // REPLACE THE IP BELOW WITH YOUR AWS SERVER IP
//            IPAddress ip = IPAddress.Parse("34.227.43.65");
//            IPEndPoint remote = new IPEndPoint(ip, 11111);
//            Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
//            try
//            {
//                Console.Write("Enter data to send: ");

//                byte[] msg = Encoding.ASCII.GetBytes(Console.ReadLine());
//                sender.SendTo(msg, remote);
//                Console.WriteLine("Data sent to Server... IP: {0}",
//                remote.Address.ToString());
//                // Release the socket
//                sender.Shutdown(SocketShutdown.Both);
//                sender.Close();
//            }
//            catch (ArgumentNullException anexc)
//            {
//                Console.WriteLine("ArgumentNullException: {0}", anexc.ToString());
//            }
//            catch (SocketException sexc)
//            {
//                Console.WriteLine("SocketException: {0}", sexc.ToString());
//            }
//            catch (Exception exc)
//            {
//                Console.WriteLine("Unexpected exception: {0}", exc.ToString());
//            }
//        }
//        catch (Exception e)
//        {
//            Console.WriteLine("Exception: {0}", e.ToString());
//        }
//    }
//    public static int Main(String[] args)
//    {
//        StartClient();
//        Console.ReadKey();
//        return 0;
//    }
//}
//using System;
//using System.Text;
//using System.Net;
//using System.Net.Sockets;

public class SyncClient 
{
    public static void StartClient() 
    {
        byte[] buffer = new byte[512];

        //Setup our end point (server)
        try
        {
            //IPAddress ip = Dns.GetHostAddresses("mail.bigpond.com")[0];
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint server = new IPEndPoint(ip, 11111);

            //create out client socket 
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //attempted a connection
            try
            {
                Console.WriteLine("Attempting Connection to server...");
                client.Connect(server);
                Console.WriteLine("Connected to IP: {0}", client.RemoteEndPoint.ToString());
                int recieved = client.Receive(buffer);

                Console.WriteLine("Recieved: {0}", Encoding.ASCII.GetString(buffer, 0, recieved));

                string sent = "Literally Gaming";
                byte[] msg = Encoding.ASCII.GetBytes(sent);

                Console.WriteLine("Sent: {0}", sent);
                client.Send(msg);

                //release the resource
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
            catch (ArgumentNullException argExc)
            {
                Console.WriteLine("ArgumentNullException: {0|", argExc.ToString());
            }
            catch (SocketException SockExc)
            {
                Console.WriteLine("SocketException : {0}", SockExc.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception! : {0}", e.ToString());
            }
        }
        catch (Exception e)
        { 
                Console.WriteLine("Exception! : {0}", e.ToString());
        }

    }
    public static int Main(String[] args) 
    {
        StartClient();
        Console.ReadKey();
        return 0;
    }
}


//namespace NetworkingMoment
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            Console.WriteLine("Goodbye World!");
//        }
//    }
//}
