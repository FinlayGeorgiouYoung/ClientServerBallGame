using System;
using System.Collections;
using System.Threading;

namespace Server
{
    public class Game
    {
        public ArrayList playersInGame = new ArrayList();
        private static int highestPlayerID = 0;

        //creates and adds a new player, which is assigned a new player ID
        public Player AddPlayerAndReturn()
        {           
            Interlocked.Increment(ref highestPlayerID);
            Player player = new Player(highestPlayerID);
            playersInGame.Add(player);
            return player;
        }

        //passes ball from one player to another
        public void PassBall(Player player1, Player player2)
        {
            player1.hasBall = false;
            player2.hasBall = true;
        }


        //returns the player with the ball
        public Player GetPlayerWithBall()
        {
            foreach (Player p in playersInGame)
            {
                if (p.hasBall)
                {
                    return p;
                }
            }
            return null;
        }

        //returns string of all the players in the game
        public string ShowPlayersInGame()
        {
            string players = "";
            if (playersInGame.Count != 0)
            {
                foreach (Player p in playersInGame)
                {
                    players += "Player " + p.PlayerID + ", ";
                }
                return players.Substring(0, players.Length - 2);
            }
            else return "none";
        }

        //removes a player from the playerInGame list
        public void RemovePlayer(Player player)
        {
            playersInGame.Remove(player);
        }

        //if no one has the ball, it reassigns it to a random exiting player
        public bool ReassignBall()
        {
            if (GetPlayerWithBall() == null)
            {
                Random rnd = new Random();
                Player player = (Player)playersInGame[(int)rnd.Next(playersInGame.Count)];
                player.hasBall = true;
                return true;
            }
            else { return false; }
        }
    }
}
