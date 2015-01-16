using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace SCvLib
{
    public interface ITileFactory
    {
        ITile Create(int x, int y, TileType type);
        ITile CreateDesertTile(int x, int y);
        ITile CreateForestTile(int x, int y);
        ITile CreateFieldTile(int x, int y);
        ITile CreateMountainTile(int x, int y);
    }

    public class TileFactory : ITileFactory
    {
        public TileFactory()
        {
        }

        public ITile Create(int x, int y, TileType type)
        {
            switch (type)
            {
                case TileType.Desert:
                    return CreateDesertTile(x, y);
                case TileType.Forest:
                    return CreateForestTile(x, y);
                case TileType.Mountain:
                    return CreateMountainTile(x, y);
                case TileType.Field:
                default:
                    return CreateFieldTile(x, y);
            }
        }

        public ITile CreateDesertTile(int x, int y)
        {
            return new Tile() {X = x, Y = y, Terrain = TileType.Desert};
        }

        public ITile CreateForestTile(int x, int y)
        {
            return new Tile() { X = x, Y = y, Terrain = TileType.Forest };
        }
        public ITile CreateFieldTile(int x, int y)
        {
            return new Tile() { X = x, Y = y, Terrain = TileType.Field };
        }
        public ITile CreateMountainTile(int x, int y)
        {
            return new Tile() { X = x, Y = y, Terrain = TileType.Mountain };
        }
    }
}
