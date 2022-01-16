using System;
using System.Collections;
using System.IO;
using System.Net.Sockets;

namespace Server
{
    public class ClientHandler
    {
        private Game game;
        private Player player;

        public ClientHandler(Game game)
        {
            this.game = game;
        }
               

        public void HandleIncomingConnection(object param) 
        {
            TcpClient tcpClient = (TcpClient)param;
            using (Stream stream = tcpClient.GetStream()) 
            {
                StreamWriter writer = new StreamWriter(stream);
                StreamReader reader = new StreamReader(stream);

                try
                {
                    ArrayList players = game.playersInGame;
                    Player playerWithBall;
                    //creates a new player with a new player ID
                    player = game.AddPlayerAndReturn();
                    //updates the server console when a new player joins
                    Console.WriteLine("\nNew connection; player ID " + player.PlayerID);
                    Console.WriteLine("Players: " + game.ShowPlayersInGame());

                    //loop waits for commands to be sent from the client
                    while (true)
                    {

                        string line = reader.ReadLine();
                        string[] substrings = line.Split(" ");
                        switch (substrings[0])
                        {
                            //sends the player ID to the client
                            case "getPlayerID":
                                writer.WriteLine(player.PlayerID);
                                writer.Flush();
                                break;

                            //sends a presentable string of the current players in the game to the client
                            case "showPlayers":
                                writer.WriteLine("Players: " + game.ShowPlayersInGame());
                                writer.Flush();
                                break;

                            //sends the player ID of the player which has the ball to the client.
                            //if no one has it, it is reassigned to a random player
                            case "ballLocation":
                                playerWithBall = game.GetPlayerWithBall();
                                if (playerWithBall == null)
                                {
                                    //locked so when if multiple Client threads try to reassign the ball, 
                                    //only one can implement the method, giving the ball to at most one Client
                                    lock (game)
                                    {
                                        game.ReassignBall();
                                    }
                                    playerWithBall = game.GetPlayerWithBall();
                                    Console.WriteLine("\nThe ball has been assigned to Player " + playerWithBall.PlayerID);
                                }
                                else { playerWithBall = game.GetPlayerWithBall(); }


                                writer.WriteLine(playerWithBall.PlayerID);
                                writer.Flush();
                                break;

                            //tries to pass the ball to the player requested by the client
                            //if the input isn't valid an appropriate message will be sent to the client
                            //when the ball is passed, the server is notified
                            //who passed to who and who now has the ball is printed to the server console
                            case "passBall":
                                Player passToPlayer = null;
                                string input = reader.ReadLine();
                                string[] lineSplit = input.Split(" ");

                                if (lineSplit[0].ToLower().Equals("pass"))
                                {
                                    foreach (Player p in game.playersInGame)
                                    {
                                        try
                                        {
                                            if (p.PlayerID == int.Parse(lineSplit[1]))
                                            {
                                                passToPlayer = p;
                                                game.PassBall(player, passToPlayer);
                                                writer.WriteLine("Success");
                                                writer.Flush();

                                                //updates server on who passed to who and who has the ball
                                                Console.WriteLine("\nPlayer " + player.PlayerID + " passed to Player " + passToPlayer.PlayerID);
                                                playerWithBall = passToPlayer;
                                                Console.WriteLine("Player " + playerWithBall.PlayerID + " has the ball");
                                                break;
                                            }
                                        }
                                        catch (Exception e) { break; }
                                    }
                                    if (passToPlayer == null) { 
                                        writer.WriteLine("No matching player");
                                        writer.Flush();
                                    }
                                }
                                else { 
                                    writer.WriteLine("Input error");
                                    writer.Flush();
                                }
                                break;

                        }

                        //server info printing
                        if (game.playersInGame.Count != players.Count)
                        {
                            Console.WriteLine(game.ShowPlayersInGame());
                            players = game.playersInGame;
                        }
                    }

                }
                catch (Exception e)
                {
                    try
                    {
                        writer.WriteLine("Error: " + e.Message);
                        writer.Flush();
                        tcpClient.Close();
                    }
                    catch { }
                   
                }

                finally 
                {
                    //if a player leaves/disconnects, the server is notified.
                    //Who left and the list of players in the game will be printed to the server console
                    Console.WriteLine("\nPlayer " + player.PlayerID + " has left the game");
                    game.RemovePlayer(player);
                    Console.WriteLine("Players: " + game.ShowPlayersInGame());
                }

            }

        }
    }
}
