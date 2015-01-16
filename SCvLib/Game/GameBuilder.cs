using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace SCvLib
{
    public static class GameBuilder
    {
        private const string SAVE_PATH = "save.bin";

        public static Game New(MapType mapType, Player p1, Player p2)
        {
            var g = new Game
            {
                Player1 = p1,
                Player2 = p2,
            };

            IMap map;
            if (MapType.Demo == mapType)
            {
                map = new Map(6, 5, 4);
            }
            else if (MapType.Small == mapType)
            {
                map = new Map(10, 20, 6);
            }
            else
            {
                map = new Map(14, 30, 8);
            }


            g.Map = map;
            g.Map.CreateUnits();

            return g;
        }

        public static void Save(Game g)
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
                Console.WriteLine(string.Format("Could not write save file: {0}", e.Message));
                if (File.Exists(SAVE_PATH))
                {
                    try
                    {
                        File.Delete(SAVE_PATH);
                    }
                    catch (IOException f)
                    {
                    }
                }
            }
        }

        public static Game Load()
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
                    g.OnDeserialize();
                }
                catch (Exception e)
                {
                    Console.WriteLine(string.Format("Could not read from save file: {0}", e.Message));
                    g = null;
                }
            }

            return g;
        }
    }
}
