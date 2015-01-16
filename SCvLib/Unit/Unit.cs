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

    }
    
    public class Elf : Unit
    {
        public Elf()
            : base()
        {
            Faction = FactionType.Elves;
        }
    }
    public class Dwarf : Unit
    {
        public Dwarf()
            : base()
        {
            Faction = FactionType.Dwarves;
        }
    }
    public class Orc : Unit
    {
        public Orc()
            : base()
        {
            Faction = FactionType.Orcs;
        }
    }
}
