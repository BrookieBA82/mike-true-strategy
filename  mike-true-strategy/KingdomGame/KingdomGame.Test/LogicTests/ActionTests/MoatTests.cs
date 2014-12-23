using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame.Test
{
    [TestClass]
    public class MoatTests {

        #region Test Setup

        [TestInitialize]
        public void MoatTestsSetup() {
            TestSetup.InitializeTypes();
        }

        #endregion

        #region Tests

        [TestCategory("MoatTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestStandardMoatActionApplication() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);
            game.PlayStep();
            game.CurrentStrategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<Card>(), new List<Player>() { game.CurrentPlayer });
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.CurrentPlayer.RemainingActions, 
              "No actions should remain after a moat is played."
            );
            Assert.AreEqual(
              6,
              game.CurrentPlayer.Hand.Count,
              "A hand with an extra card should remain after a moat is played."
            );
            Assert.AreEqual(
              1, 
              game.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a moat is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeMoat, 
              game.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a moat after one is played."
            );
        }

        [TestCategory("MoatTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestInsufficentCardsMoatActionApplication() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);
            game.PlayStep();
            game.CurrentStrategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<Card>(), new List<Player>() { game.CurrentPlayer });
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.CurrentPlayer.RemainingActions, 
              "No actions should remain after a moat is played."
            );
            Assert.AreEqual(
              5,
              game.CurrentPlayer.Hand.Count,
              "A full hand should remain after a moat is played with only one card available to draw."
            );
            Assert.AreEqual(
              1, 
              game.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a moat is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeMoat, 
              game.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a moat after one is played."
            );
        }

        [TestCategory("MoatTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestScriptedEmptyMoatActionTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);
            game.PlayStep();
            game.CurrentStrategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<Card>(), new List<Player>());
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.CurrentPlayer.RemainingActions, 
              "No actions should remain after a moat is played on no target."
            );
            Assert.AreEqual(
              4,
              game.CurrentPlayer.Hand.Count,
              "A hand without a moat should remain after a moat is played with no target."
            );
            Assert.AreEqual(
              1, 
              game.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a moat is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeMoat, 
              game.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a moat after one is played."
            );
        }

        [TestCategory("MoatTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestInvalidMoatTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);
            game.PlayStep();

            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(
              null, 
              new List<Player>() {game.Players[1]}
            );
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.CurrentPlayer.RemainingActions, 
              "No actions should remain after a moat is played on an invalid target."
            );
            Assert.AreEqual(
              4,
              game.CurrentPlayer.Hand.Count,
              "A card should be missing after a moat is played on an invalid target."
            );
            Assert.AreEqual(
              5,
              game.Players[1].Hand.Count,
              "The other player's hand should remain unchanged despite being illegally targeted by a moat."
            );
            Assert.AreEqual(
              1, 
              game.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a moat is played on an invalid target."
            );
            Assert.AreEqual(
              TestSetup.CardTypeMoat, 
              game.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a moat after one is played."
            );
        }

        [TestCategory("MoatTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMultipleMoatTargets() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);
            game.PlayStep();

            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(
              null, 
              new List<Player>(game.Players)
            );
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.CurrentPlayer.RemainingActions, 
              "No actions should remain after a moat is played on a multiple target set."
            );
            Assert.AreEqual(
              4,
              game.CurrentPlayer.Hand.Count,
              "A card should be missing after a moat is played on a multiple target set."
            );
            Assert.AreEqual(
              5,
              game.Players[1].Hand.Count,
              "The other player's hand should remain unchanged despite being illegally targeted by a moat."
            );
            Assert.AreEqual(
              1, 
              game.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a moat is played a multiple target set."
            );
            Assert.AreEqual(
              TestSetup.CardTypeMoat, 
              game.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a moat after one is played."
            );
        }

        [TestCategory("MoatTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestDuplicateMoatTargets() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);
            game.PlayStep();

            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(
              null,
              new List<Player>() { game.CurrentPlayer, game.CurrentPlayer}
            );
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.CurrentPlayer.RemainingActions, 
              "No actions should remain after a moat is played on a duplicate target set."
            );
            Assert.AreEqual(
              4,
              game.CurrentPlayer.Hand.Count,
              "A card should be missing after a moat is played on a duplicate target set."
            );
            Assert.AreEqual(
              1, 
              game.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a moat is played a duplicate target set."
            );
            Assert.AreEqual(
              TestSetup.CardTypeMoat, 
              game.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a moat after one is played."
            );
        }

        #endregion
    }
}
