using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCvLib;

namespace SCvUnitTest
{
    [TestClass]
    public class GameBuilderTests
    {
        [TestMethod]
        void TestNew(){
            IPlayer p1 = new Player {Faction = FactionType.Elves, Name = "A", Score = 0};
            IPlayer p2 = new Player {Faction = FactionType.Dwarves, Name = "B", Score = 0};
            IPlayer p3 = new Player {Faction = FactionType.Orcs, Name = "C", Score = 0};
            IGame g = GameBuilder.New(MapType.Demo, (Player)p1, (Player)p2);

            Assert.AreEqual(g.Map.Width, 6);
            Assert.AreEqual(g.Map.Turns, 5);
            Assert.AreEqual(g.Map.Units, 4);
            Assert.AreEqual(g.Player1, p1);
            Assert.AreEqual(g.Player2, p2);
            
        }

        /*[TestMethod]
        void TestLoad(){

        }*/

        /*[TestMethod]
        void TestSave()
        {

        }*/
    }
}