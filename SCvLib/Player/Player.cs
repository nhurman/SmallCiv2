﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SCvLib
{
    public interface IPlayer
    {

    }
    public class Player : IPlayer
    {
        public FactionName Faction;
        public string Name;
        public int Score;
    }
}