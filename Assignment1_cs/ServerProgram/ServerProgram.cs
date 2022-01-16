using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    public class ServerProgram
    {
        private const int port = 8888;
        private static readonly Game game = new Game();

        static void Main(string[] args)
        {
            RunServer();
        }

        private static void RunServer()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();
            Console.WriteLine("Waiting for incoming connections...");
            //loop accepts incoming requests from clients and creates a new Thread with a new instance of ClientHandler to deal with the client
            while (true)
            {
                TcpClient tcpClient = listener.AcceptTcpClient();
                new Thread(new ClientHandler(game).HandleIncomingConnection).Start(tcpClient);
            }
        }
            
    }
    
}
