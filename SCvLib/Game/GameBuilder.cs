using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCvLib
{
    public static class GameBuilder
    {
        public static Game New(MapType mapType, List<Tuple<string, FactionType>> players)
        {
            if (2 != players.Count) return null;

            Map map;
            if (MapType.Demo == mapType)
            {
                map = new Map(6, 5, 1);
            }
            else if (MapType.Demo == mapType)
            {
                map = new Map(10, 20, 6);
            }
            else
            {
                map = new Map(14, 30, 8);
            }

            var g = new Game
            {
                Player1 = new Player() {Name = players[0].Item1, Faction = players[0].Item2, Score = 0},
                Player2 = new Player() {Name = players[1].Item1, Faction = players[1].Item2, Score = 0},
                Map = map,
            };

            g.CreateUnits();

            return g;
        }
    }
}
