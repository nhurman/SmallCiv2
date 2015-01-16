using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCvLib
{
    public interface ITile
    {
        int X { get; set; }
        int Y { get; set; }
        TileType Terrain { get; set; }
        FactionType Faction { get; set; }
    }
    public class Tile : ITile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TileType Terrain { get; set; }
        public FactionType Faction { get; set; }
    }

    public enum TileType
    {
        Desert,
        Forest,
        Field,
        Mountain,
    }
}
