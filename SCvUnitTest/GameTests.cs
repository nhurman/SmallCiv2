using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SCvLib;

namespace SCvUnitTest
{
    [TestClass]
    public class GameTests
    {
        [TestMethod]
        public void TestGameCreation()
        {
            IGame game = new Game();
            Assert.AreEqual(game.Turn, 0);
            Assert.AreEqual(game.CurrentPlayerId, 0);
            Assert.AreEqual(Game.Instance, game);
        }
       
        [TestMethod]
        public void TestNextTurn()
        {
            IPlayer p1 = new Player { Faction = FactionType.Elves, Name = "A", Score = 0 };
            IPlayer p2 = new Player { Faction = FactionType.Dwarves, Name = "B", Score = 0 };
            IGame game = GameBuilder.New(MapType.Demo, (Player)p1, (Player)p2);

            game.NextTurn();
            Assert.AreEqual(game.CurrentPlayerId, 1);
            Assert.AreEqual(game.Turn,0);
            game.NextTurn();
            Assert.AreEqual(game.CurrentPlayerId, 0);
            Assert.AreEqual(game.Turn, 1);
        }
        
       /* [TestMethod]
        public void TestCalculateUnitStats()
        {
            Game game = new Game();
        }*/

       /* [TestMethod]
        public void TestIsEnded()
        {
            IPlayer p1 = new Player { Faction = FactionType.Elves, Name = "A", Score = 0 };
            IPlayer p2 = new Player { Faction = FactionType.Dwarves, Name = "B", Score = 0 };
            IGame game = GameBuilder.New(MapType.Demo, (Player)p1, (Player)p2);
        }*/
    }
}
