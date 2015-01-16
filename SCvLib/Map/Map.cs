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

    public class Map : IMap
    {
        private struct Point
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        public int Width { get; set; }
        public int Turns { get; set; }
        public ITile[,] Tiles { get; protected set; }

        private MapBackend _mapBackend;

        public Map(int width, int turns)
        {
            Width = width;
            Turns = turns;
            Tiles = new ITile[Width,Width];
            var tileFactory = new TileFactory();

            _mapBackend = new MapBackend(width);
            _mapBackend.Generate();

            for (int y = 0; y < width; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    _mapBackend.GetTileType(x, y);
                    Tiles[x, y] = tileFactory.Create(x, y, (TileType)_mapBackend.GetTileType(x, y));
                }
            }
        }

        public ITile StartTile(int playerId)
        {
            int x = 0, y = 0;
            x = _mapBackend.StartTileX(playerId);
            y = _mapBackend.StartTileY(playerId);
            return Tiles[x, y];
        }

        public ITile GetTile(int x, int y)
        {
            return Tiles[x, y];
        }

        public bool CanMoveTo(ITile source, ITile dest)
        {
            return _mapBackend.CanMoveTo((int)source.Faction, source.X, source.Y, dest.X, dest.Y);
        }
    }
}
