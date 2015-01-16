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
        IPlayer Player1 { get; set; }
        IPlayer Player2 { get; set; }
        int Turn { get; set; }
        Random Random { get; set; }
        int Seed { get; set; }
        IMap Map { get; set; }

        int CurrentPlayerId { get; set; }
        void OnDeserialize();
        void NextTurn();
        void Start();
        bool IsEnded();
    }

    [Serializable]
    public class Game : IGame
    {
        public int CurrentPlayerId { get; set; }
        public IPlayer Player1 { get; set; }
        public IPlayer Player2 { get; set; }
        public int Turn { get; set; }
        public static IGame Instance { get; set; }
        public Random Random { get; set; }
        public int Seed { get; set; }

        public IMap Map { get; set; }

        public Game()
        {
            Turn = 0;
            CurrentPlayerId = 0;
            Instance = this;

            var r = new Random();
            Seed = r.Next();

            OnDeserialize();
        }

        public void OnDeserialize()
        {
            Game.Instance = this;
            Random = new Random(Seed);
            if (Map != null)
                Map.OnDeserialize();
        }

        public void NextTurn()
        {
            ++CurrentPlayerId;
            if (CurrentPlayerId >= 2)
            {
                CurrentPlayerId = 0;
                ++Turn;
            }

            CalculateUnitStats();
        }

        public void CalculateUnitStats()
        {
            Player1.Score = 0;
            Player2.Score = 0;

            foreach (Tile t in Map.Tiles)
            {
                // Remove dead units and reset Mvt
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

                    if (u.Faction == Player1.Faction) Player1.Score += u.Points();
                    if (u.Faction == Player2.Faction) Player2.Score += u.Points();
                }
            }
        }

        public void Start()
        {
            CalculateUnitStats();
        }

        public bool IsEnded()
        {
            if (Turn >= Map.Turns) return true;

            int n1 = 0, n2 = 0;
            foreach (ITile t in Map.Tiles)
            {
                if (t.Units.Count > 0)
                {
                    if (t.Units[0].PlayerId == 0) ++n1;
                    else if (t.Units[0].PlayerId == 1) ++n2;
                }

                if (n1 > 0 && n2 > 0) return false;
            }

            return true;
        }
    }
}
