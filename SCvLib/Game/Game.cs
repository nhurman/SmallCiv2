using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCvLib
{
    public interface IGame
    {
        
    }

    public class Game : IGame
    {
        public Player Player1;
        public Player Player2;
        public int Turn;
        public int LastTurn;

        public Game()
        {
            Turn = 0;
            LastTurn = 10;
        }
    }
}
