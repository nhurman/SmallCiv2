using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCvLib
{
    public interface IUnit
    {
        int HP { get; set; }
        int HPMax { get; set; }
        int Atk { get; set; }
        int Def { get; set; }
        double Mvt { get; set; }
        int PlayerId { get; set; }
        string Name { get; set; }
        FactionType Faction { get; set; }
        int Points();
        double MoveCost(ITile dest);

        void OnFightWon();
        void OnFightLost();
        void ForceMoveTo(ITile dest);
        void AttackOrMoveTo(ITile dest);

        ITile Tile { get; set; }
    }

    [Serializable]
    public class Unit : IUnit
    {
        public int HP { get; set; }
        public int HPMax { get; set; }
        public int Atk  { get; set; }
        public int Def  { get; set; }
        public double Mvt { get; set; }
        public int PlayerId { get; set; }
        public string Name { get; set; }
        public FactionType Faction { get; set; }

        public ITile Tile { get; set; }

        public Unit()
        {
            Atk = 2;
            Def = 1;
            HP = 5;
            HPMax = 5;
            Mvt = 1;
        }

        virtual public int Points()
        {
            return 1;
        }

        public void ForceMoveTo(ITile dest)
        {
            if (Tile != null)
                Tile.DelUnit(this);
            if (dest != null)
                dest.AddUnit(this);
            Tile = dest;
        }

        virtual public void OnFightWon()
        {
        }

        virtual public void OnFightLost()
        {
            
        }

        public double MoveCost(ITile dest)
        {
            return Game.Instance.Map.MoveCost(Tile, dest);
        }

        public void AttackOrMoveTo(ITile dest)
        {
            if (Game.Instance.CurrentPlayerId != PlayerId) return;

            if (Tile == dest) return;

            // Is it in range
            double moveCost = Game.Instance.Map.MoveCost(Tile, dest);
            if (moveCost > Mvt)
            {
                Console.WriteLine(string.Format("Not enough Mvt {0} / {1}", moveCost, Mvt));
                return;
            }

            Mvt -= moveCost;

            // Is it free or ours
            if (dest.Faction() == FactionType.None || dest.Faction() == Faction)
            {
                ForceMoveTo(dest);
                return;
            }

            // Attack
            if (moveCost == 0) // TP for dwarves
            {
                if (Mvt <= 0) return;
                Mvt--;
            }

            IUnit defendingUnit = dest.GetDefendingUnit();
            
            var nbFights = Game.Instance.Random.Next(3, defendingUnit.Def + 2);
            int rnd;
            for (var i = 0; i < nbFights && HP > 0 && defendingUnit.HP > 0; ++i)
            {
                double winProba = 100 * (double)Atk / (Atk + defendingUnit.Def);
                rnd = Game.Instance.Random.Next(1, 100);
                if (rnd >= winProba)
                {
                    HP--;
                }
                else
                {
                    defendingUnit.HP--;
                }
            }

            if (defendingUnit.HP <= 0)
            {
                OnFightWon();
                defendingUnit.OnFightLost();
            }
            else if (HP <= 0)
            {
                defendingUnit.OnFightWon();
                OnFightLost();
            }

            if (defendingUnit.HP <= 0)
            {
                dest.DelUnit(defendingUnit);

                if (dest.Units.Count == 0)
                {
                    ForceMoveTo(dest);
                }
            }
        }
    }
    [Serializable]
    public class Elf : Unit, IUnit
    {
        public Elf()
            : base()
        {
            Faction = FactionType.Elves;
        }

        virtual public void OnFightLost()
        {
            int rnd = Game.Instance.Random.Next(0, 2);
            if (1 == rnd)
            {
                HP = 1;
            }
        }
    }

    [Serializable]
    public class Dwarf : Unit, IUnit
    {
        public Dwarf()
            : base()
        {
            Faction = FactionType.Dwarves;
        }

        override public int Points()
        {
            if (Tile.Terrain == TileType.Field) return 0;
            return 1;
        }
    }

    [Serializable]
    public class Orc : Unit, IUnit
    {
        public Orc()
            : base()
        {
            Faction = FactionType.Orcs;
        }

        override public void OnFightWon()
        {
            Atk++;
        }
        override public int Points()
        {
            if (Tile.Terrain == TileType.Forest) return 0;
            return 1;
        }
    }
}
