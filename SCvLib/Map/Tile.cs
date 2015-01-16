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
        FactionType Faction();
        List<IUnit> Units { get; set; }

        void AddUnit(IUnit u);
        void DelUnit(IUnit u);
        IUnit GetDefendingUnit();
    }

    [Serializable]
    public class Tile : ITile
    {
        public List<IUnit> Units { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public TileType Terrain { get; set; }

        public FactionType Faction()
        {
            if (Units.Count == 0)
                return FactionType.None;
            return Units[0].Faction;
        }

        public Tile()
        {
            Units = new List<IUnit>();
        }

        public void AddUnit(IUnit u)
        {
            Units.Add(u);
        }

        public void DelUnit(IUnit u)
        {
            Units.Remove(u);
        }

        public IUnit GetDefendingUnit()
        {
            if (Units.Count == 0) return null;

            IUnit unit = Units[0];
            foreach (IUnit u in Units)
            {
                if (u.Def > unit.Def)
                    unit = u;
            }

            return unit;
        }
    }

    public enum TileType
    {
        Desert = 0,
        Forest = 1,
        Field = 2,
        Mountain = 3,
    }
}
