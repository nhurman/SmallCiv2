using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCvLib
{
    public interface IPlayer
    {
        FactionType Faction { get; set; }
        string Name { get; set; }
        int Score { get; set; }
    }

    [Serializable]
    public class Player : IPlayer
    {
        public FactionType Faction { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
    }
}
