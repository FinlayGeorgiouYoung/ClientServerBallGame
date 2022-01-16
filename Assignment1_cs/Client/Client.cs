using System;
using System.IO;
using System.Net.Sockets;

namespace Client
{
    class Client : IDisposable
    {

        const int port = 8888;

        private readonly StreamReader reader;
        private readonly StreamWriter writer;
        private string players;

        public Client()
        {
            // Connecting to the server and creating objects for communications
            TcpClient tcpClient = new TcpClient("localhost", port);
            NetworkStream stream = tcpClient.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
        }

        public static void Main(String[] args)
        {

            try
            {
                Client client = new Client();
                string playerID = client.GetPlayerID();
                string playerWithBall;
                Console.WriteLine("You have been assigned player ID: " + playerID);

                while (true)
                {
                    client.players = client.ShowPlayers();
                    //prints player list
                    Console.WriteLine("\n" + client.players);
                    //calls ball location to return the player with the ball. If no-one has the ball,
                    //it will be assigned and the location will be returned after
                    playerWithBall = client.BallLocation();

                    //if player has the ball...
                    if (playerID.Equals(playerWithBall))
                    {
                        Console.WriteLine("You have the ball.");
                        Console.WriteLine("Type 'pass' and a player ID to pass it to");
                        //until the player enters a correct input the loop will continue
                        while (true)
                        {
                            //tries to pass the ball to the player id input by the player
                            //returns whether the pass was successful or not
                            string passBallState = client.PassBall();
                            if (passBallState.Equals("Success"))
                            {
                                break;
                            }
                            else if (passBallState.Equals("No matching player"))
                            {
                                Console.WriteLine("The player ID you entered doesn't exist. Try again!");
                            }
                            else if (passBallState.Equals("Input error"))
                            {
                                Console.WriteLine("Input error. Try again!");
                            }
                        }
                    }

                    //player who has the ball is printed
                    Console.WriteLine("Player " + client.BallLocation() + " has the ball");
                    string updatedPlayers;
                    //updates player list when necessary
                    while (playerWithBall.Equals(client.BallLocation()) && !playerWithBall.Equals(playerID))
                    {
                        updatedPlayers = client.ShowPlayers();
                        if (!client.players.Equals(updatedPlayers))
                        {
                            Console.WriteLine("\nPlayer list update:");
                            Console.WriteLine(updatedPlayers);
                            client.players = updatedPlayers;
                        }

                    }
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }

        }


        //Methods below write commands to the ClientHandler
        public string ShowPlayers()
        {
            writer.WriteLine("showPlayers");
            writer.Flush();
            return reader.ReadLine();
        }

        public string BallLocation()
        {
            writer.WriteLine("ballLocation");
            writer.Flush();
            return reader.ReadLine();
        }

        public string GetPlayerID()
        {
            writer.WriteLine("getPlayerID");
            writer.Flush();
            return reader.ReadLine(); ;
        }

        public string PassBall()
        {
            writer.WriteLine("passBall");
            writer.Flush();
            string playerInput = Console.ReadLine();
            writer.WriteLine(playerInput);
            writer.Flush();
            return reader.ReadLine();
        }

        public void Dispose()
        {
            reader.Close();
            writer.Close();
        }
    }  

}
