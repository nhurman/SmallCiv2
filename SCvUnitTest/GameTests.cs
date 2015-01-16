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
            Game game = new Game();
            Assert.AreEqual(game.Turn, 0);
            Assert.AreEqual(game.CurrentPlayerId, 0);
            Assert.AreEqual(Game.Instance, game);
        }
       
        /*[TestMethod]
        public void TestNextTurn()
        {
            Game game = new Game();
            game.NextTurn();
            Assert.AreEqual(game.CurrentPlayerId, 1);
            Assert.AreEqual(game.Turn,0);
            game.NextTurn();
            Assert.AreEqual(game.CurrentPlayerId, 1);
            Assert.AreEqual(game.Turn, 1);
        }*/
        
       /* [TestMethod]
        public void TestCalculateUnitStats()
        {
            Game game = new Game();
        }*/

        /*[TestMethod]
        public void TestIsEnded()
        {

        }*/
    }
}
