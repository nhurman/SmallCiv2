using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCvLib
{
    public interface IUnit
    {
        
    }

    public class Unit : IUnit
    {
        public int HP { get; set; }
        public int HPMax { get; set; }
        public int Atk  { get; set; }
        public int Def  { get; set; }
        public int Mvt  { get; set; }
        public string Name { get; set; }
        public FactionName Faction { get; set; }

        public bool CanMoveTo(ITile dest)
        {
            return false;
        }
    }
    
    public class Elf : Unit
    {
        public Elf()
            : base()
        {
            Faction = FactionName.Elves;
        }
    }
    public class Dwarf : Unit
    {
        public Dwarf()
            : base()
        {
            Faction = FactionName.Dwarves;
        }
    }
    public class Orc : Unit
    {
        public Orc()
            : base()
        {
            Faction = FactionName.Orcs;
        }
    }
}
