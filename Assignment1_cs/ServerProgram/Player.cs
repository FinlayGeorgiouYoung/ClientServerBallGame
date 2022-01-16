using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class Player
    {
       
        public bool hasBall;

        public Player(int playerID)
        {
            PlayerID = playerID;
        }
        public int PlayerID { get; }
    }
}
