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
        int Width { get; set; }
        int Turns { get; set; }
        int Units { get; set; }
        ITile[,] Tiles { get; set; }
        void CreateUnits();
        void OnDeserialize();
        double MoveCost(ITile source, ITile dest);
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
        public ITile[,] Tiles { get; set; }

        [NonSerialized] private MapBackend _mapBackend;

        public Map(int width, int turns, int units)
        {
            Width = width;
            Turns = turns;
            Units = units;
            Tiles = new ITile[Width,Width];
            var tileFactory = new TileFactory();

            OnDeserialize();

            for (int y = 0; y < width; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    _mapBackend.GetTileType(x, y);
                    Tiles[y, x] = tileFactory.Create(x, y, (TileType)_mapBackend.GetTileType(x, y));
                }
            }
        }

        public void OnDeserialize()
        {
            _mapBackend = new MapBackend(Width, Game.Instance.Seed);
            _mapBackend.Generate();
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
            return _mapBackend.MoveCost((int)source.Units[0].Faction, source.X, source.Y, dest.X, dest.Y);
        }

        public void CreateUnits()
        {
            IUnitFactory unitFactory = new UnitFactory();
            for (int p = 0; p < 2; ++p)
            {
                int x = _mapBackend.StartTileX(p);
                int y = _mapBackend.StartTileY(p);
                ITile t = Tiles[y, x];
                for (int i = 0; i < Units; ++i)
                {
                    IUnit u = unitFactory.Create(0 == p ? Game.Instance.Player1.Faction : Game.Instance.Player2.Faction);
                    u.PlayerId = p;
                    u.ForceMoveTo(t);
                }
            }
        }
    }
}
