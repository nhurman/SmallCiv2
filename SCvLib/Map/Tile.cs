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
        List<IUnit> Units { get; set; }

        void AddUnit(IUnit u);
    }

    [Serializable]
    public class Tile : ITile
    {
        public List<IUnit> Units { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public TileType Terrain { get; set; }
        public FactionType Faction { get; set; }
        public Tile()
        {
            Units = new List<IUnit>();
        }

        public void AddUnit(IUnit u)
        {
            Units.Add(u);
        }
    }

    public enum TileType
    {
        Desert,
        Forest,
        Field,
        Mountain,
    }
}
