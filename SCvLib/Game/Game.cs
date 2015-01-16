using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;

namespace SCvLib
{
    public interface IGame
    {
        int CurrentPlayerId { get; set; }
        void CreateUnits();
    }

    [Serializable]
    public class Game : IGame
    {
        public int CurrentPlayerId { get; set; }
        public Player Player1;
        public Player Player2;
        public int Turn;
        private const string SAVE_PATH = "save.bin";

        public Map Map { get; set; }

        public Game()
        {
            Turn = 0;
            CurrentPlayerId = 0;
        }

        public void CreateUnits()
        {
            for (int p = 0; p < 2; ++p)
            {
                int x = Map._mapBackend.StartTileX(p);
                int y = Map._mapBackend.StartTileY(p);
                ITile t = Map.Tiles[y, x];
                for (int i = 0; i < Map.Units; ++i)
                {
                    IUnit u = new Unit() {Atk = 2, Def = 1, HP = 5, HPMax = 5, Mvt = 1, Faction = (0 == p?Player1.Faction : Player2.Faction), PlayerId = p};
                    u.Tile = t;
                    t.AddUnit(u);
                }
            }
        }

        public void NextTurn()
        {
            ++CurrentPlayerId;
            if (CurrentPlayerId >= 2)
            {
                CurrentPlayerId = 0;
                ++Turn;
            }

            CalculateScore();
        }

        public void CalculateScore()
        {
            Player1.Score = 0;
            Player2.Score = 0;

            foreach (Tile t in Map.Tiles)
            {
                List<IUnit> toRemove = new List<IUnit>();
                foreach (Unit u in t.Units)
                {
                    if (u.HP <= 0)
                    {
                        toRemove.Add(u);
                    }
                    else
                    {
                        u.Mvt = 1;
                    }
                }

                foreach (IUnit u in toRemove)
                {
                    t.Units.Remove(u);
                }

                if (t.Units.Count > 0)
                {
                    IUnit u = t.Units[0];
                    if (u.Faction == FactionType.Dwarves && t.Terrain == TileType.Field) continue;
                    if (u.Faction == FactionType.Orcs && t.Terrain == TileType.Forest) continue;

                    if (u.Faction == Player1.Faction) Player1.Score++;
                    if (u.Faction == Player2.Faction) Player2.Score++;
                }
            }
        }

        public void Start()
        {
            CalculateScore();
        }

        public static void SaveGame(Game g)
        {
            var formatter = new BinaryFormatter();
            try
            {
                FileStream fileStream = new FileStream(SAVE_PATH, FileMode.Create, FileAccess.Write);
                formatter.Serialize(fileStream, g);
                fileStream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] Unable to save game data : [" + e.Source + "]" + e.Message);
                if (File.Exists(SAVE_PATH))
                    File.Delete(SAVE_PATH);
            }
        }


        public static Game LoadGame()
        {
            var formatter = new BinaryFormatter();
            Game g = null;

            if (File.Exists(SAVE_PATH))
            {
                try
                {
                    FileStream fileStream = new FileStream(SAVE_PATH, FileMode.Open, FileAccess.Read);
                    g = (Game)formatter.Deserialize(fileStream);
                    fileStream.Close();
                    g.Map.PostRestore();
                }
                catch (Exception e)
                {
                    Console.WriteLine("[ERROR] Unable to read game data : [" + e.Source + "]" + e.Message);
                    g = null;
                }
            }

            return g;
        }
    }
}
