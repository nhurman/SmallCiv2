using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace SCvLib
{
    public static class GameBuilder
    {
        private const string SAVE_PATH = "save.bin";

        public static IGame New(MapType mapType, IPlayer p1, IPlayer p2)
        {
            IGame g = new Game
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

        public static void Save(IGame g, string path = "save.scv")
        {
            var formatter = new BinaryFormatter();
            try
            {
                FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                formatter.Serialize(fileStream, g);
                fileStream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("Could not write save file: {0}", e.Message));
                if (File.Exists(path))
                {
                    try
                    {
                        File.Delete(path);
                    }
                    catch (IOException)
                    {
                    }
                }
            }
        }

        public static IGame Load(string path = "save.scv")
        {
            var formatter = new BinaryFormatter();
            IGame g = null;

            if (File.Exists(path))
            {
                try
                {
                    FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                    g = (IGame)formatter.Deserialize(fileStream);
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
