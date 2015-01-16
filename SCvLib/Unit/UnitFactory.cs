using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace SCvLib
{
    public interface IUnitFactory
    {
        IUnit Create(FactionType faction);
        IUnit CreateElf();
        IUnit CreateDwarf();
        IUnit CreateOrc();
    }

    [Serializable]
    public class UnitFactory : IUnitFactory
    {
        public UnitFactory()
        {
        }

        public IUnit Create(FactionType faction)
        {
            switch (faction)
            {
                case FactionType.Elves:
                    return CreateElf();
                case FactionType.Dwarves:
                    return CreateDwarf();
                case FactionType.Orcs:
                default:
                    return CreateOrc();
            }
        }

        public IUnit CreateElf()
        {
            return new Elf();
        }
        
        public IUnit CreateDwarf()
        {
            return new Dwarf();
        }
        public IUnit CreateOrc()
        {
            return new Orc();
        }
    }
}
