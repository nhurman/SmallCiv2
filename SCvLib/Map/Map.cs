using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Wrapper;

namespace SCvLib
{
    public interface IMap
    {

    }

    public enum MapType
    {
        Demo,
        Small,
        Normal,
    }

    [Serializable]
    public class Map : IMap
    {
        private struct Point
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        public int Width { get; set; }
        public int Turns { get; set; }
        public int Units { get; set; }
        public int Seed { get; set; }
        public ITile[,] Tiles { get; protected set; }

        [NonSerialized] public MapBackend _mapBackend;
        [NonSerialized] public Random _random;

        public Map(int width, int turns, int units)
        {
            Width = width;
            Turns = turns;
            Units = units;
            Tiles = new ITile[Width,Width];
            var r = new Random();
            Seed = r.Next();
            _random = new Random(Seed);
            var tileFactory = new TileFactory();

            _mapBackend = new MapBackend(width, Seed);
            _mapBackend.Generate();

            for (int y = 0; y < width; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    _mapBackend.GetTileType(x, y);
                    Tiles[y, x] = tileFactory.Create(x, y, (TileType)_mapBackend.GetTileType(x, y));
                }
            }
        }

        public void PostRestore()
        {
            _mapBackend = new MapBackend(Width, Seed);
            _mapBackend.Generate();
            _random = new Random(Seed);
        }

        public ITile StartTile(int playerId)
        {
            int x = 0, y = 0;
            x = _mapBackend.StartTileX(playerId);
            y = _mapBackend.StartTileY(playerId);
            return Tiles[y, x];
        }

        public ITile GetTile(int x, int y)
        {
            return Tiles[y, x];
        }

        public double MoveCost(ITile source, ITile dest)
        {
            return _mapBackend.MoveCost((int)source.Faction, source.X, source.Y, dest.X, dest.Y);
        }

        public void AttackOrMoveTo(IUnit unit, ITile dstTile)
        {
            if (unit.Tile == dstTile) return;

            // Is it in range
            double moveCost = MoveCost(unit.Tile, dstTile);
            if (moveCost > unit.Mvt)
            {
                Console.WriteLine("Not enough Mvt");
                return;
            }

            unit.Mvt -= moveCost;

            // Is it free or ours
            if (dstTile.Units.Count == 0 || dstTile.Units[0].PlayerId == unit.PlayerId)
            {
                unit.Tile.Units.Remove(unit);
                dstTile.Units.Add(unit);
                unit.Tile = dstTile;
                return;
            }

            // Attack
            if (moveCost == 0)
            {
                if (unit.Mvt <= 0) return;
                unit.Mvt--;
            }

            IUnit defendingUnit = dstTile.Units[0];
            foreach (IUnit u in dstTile.Units)
            {
                if (u.Def > defendingUnit.Def)
                    defendingUnit = u;
            }

            var nbCombats = _random.Next(3, defendingUnit.Def + 2);
            int rnd;
            for (var i = 0; i < nbCombats && unit.HP > 0 && defendingUnit.HP > 0; ++i)
            {
                double winProba = 100 * (double)unit.Atk/(unit.Atk + defendingUnit.Def);
                rnd = _random.Next(1, 100);
                if (rnd >= winProba)
                {
                    unit.HP--;
                }
                else
                {
                    defendingUnit.HP--;
                }
            }

            if (defendingUnit.HP <= 0 && unit.Faction == FactionType.Orcs)
                unit.Atk++;
            else if (unit.HP <= 0 && defendingUnit.Faction == FactionType.Orcs)
                defendingUnit.Atk++;

            rnd = _random.Next(0, 2);

            if (defendingUnit.HP <= 0 && unit.Faction == FactionType.Elves && rnd == 1)
                defendingUnit.HP = 1;
            else if (unit.HP <= 0 && unit.Faction == FactionType.Elves && rnd == 1)
                unit.HP = 1;

            if (defendingUnit.HP <= 0)
            {
                dstTile.Units.Remove(defendingUnit);

                if (dstTile.Units.Count == 0)
                {
                    unit.Tile.Units.Remove(unit);
                    dstTile.Units.Add(unit);
                    unit.Tile = dstTile;
                }
                return;
            }

        }
    }
}
