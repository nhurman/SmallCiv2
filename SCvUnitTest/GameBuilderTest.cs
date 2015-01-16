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
            Player p1 = new Player();
            p1.Faction = FactionType.Elves;
            p1.Name = "A";
            p1.Score = 0;
            Player p2 = new Player();
            p1.Faction = FactionType.Dwarves;
            p1.Name = "B";
            p1.Score = 0;
            Player p3 = new Player();
            p1.Faction = FactionType.Orcs;
            p1.Name = "C";
            p1.Score = 0;
            Game g = GameBuilder.New(MapType.Demo, p1, p2);

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