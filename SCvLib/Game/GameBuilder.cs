using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCvLib
{
    public static class GameBuilder
    {
        public static Game New(MapName map, List<Tuple<string, FactionName>> players)
        {
            if (2 != players.Count) return null;

            var g = new Game
            {
                Player1 = new Player() {Name = players[0].Item1, Faction = players[0].Item2, Score = 0},
                Player2 = new Player() {Name = players[1].Item1, Faction = players[1].Item2, Score = 0}
            };

            return g;
        }
    }
}
