using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame.Test
{
    [TestClass]
    public class VillageTests {

        #region Test Setup

        [TestInitialize]
        public void VillageTestsSetup() {
            TestSetup.InitializeTypes();
        }

        #endregion

        #region Tests

        [TestCategory("VillageActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestStandardVillageActionApplication() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);
            game.PlayStep();
            game.CurrentStrategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<Card>(), new List<Player>() { game.CurrentPlayer });
            game.PlayStep();
            
            Assert.AreEqual(
              2, 
              game.CurrentPlayer.RemainingActions, 
              "Two actions should remain after a village is played."
            );
            Assert.AreEqual(
              5,
              game.CurrentPlayer.Hand.Count,
              "A full hand should remain after a village is played."
            );
            Assert.AreEqual(
              1, 
              game.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a village is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeVillage, 
              game.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a village after one is played."
            );
        }

        [TestCategory("VillageActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestEmptyDeckVillageActionApplication() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            game.PlayStep();
            game.CurrentStrategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<Card>(), new List<Player>() { game.CurrentPlayer });
            game.PlayStep();
            
            Assert.AreEqual(
              2,
              game.CurrentPlayer.RemainingActions, 
              "Two actions should remain after a village is played."
            );
            Assert.AreEqual(
              4, 
              game.CurrentPlayer.Hand.Count, 
              "A hand without a village should remain after a village is played."
            );
            Assert.AreEqual(
              1, 
              game.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a village is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeVillage, 
              game.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a village after one is played."
            );
        }

        [TestCategory("VillageActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestExistingActionsVillageActionApplication() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);
            game.CurrentPlayer.RemainingActions = 2;
            game.PlayStep();
            game.CurrentStrategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<Card>(), new List<Player>() { game.CurrentPlayer });
            game.PlayStep();
            
            Assert.AreEqual(
              3, 
              game.CurrentPlayer.RemainingActions, 
              "Three actions should remain after a village is played if two were available beforehand."
            );
            Assert.AreEqual(
              5,
              game.CurrentPlayer.Hand.Count,
              "A full hand should remain after a village is played."
            );
            Assert.AreEqual(
              1, 
              game.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a village is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeVillage, 
              game.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a village after one is played."
            );
        }

        [TestCategory("VillageActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestScriptedEmptyVillageActionTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);
            game.PlayStep();
            game.CurrentStrategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<Card>(), new List<Player>());
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.CurrentPlayer.RemainingActions, 
              "No actions should remain after a village is played on no target."
            );
            Assert.AreEqual(
              4,
              game.CurrentPlayer.Hand.Count,
              "A hand without a village should remain after a village is played."
            );
            Assert.AreEqual(
              1, 
              game.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a village is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeVillage, 
              game.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a village after one is played."
            );
        }

        [TestCategory("VillageActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestInvalidVillageTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 1;

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
              "No actions should remain after a village is played on an invalid target."
            );
            Assert.AreEqual(
              4,
              game.CurrentPlayer.Hand.Count,
              "A card should be missing after a village is played on an invalid target."
            );
            Assert.AreEqual(
              5,
              game.Players[1].Hand.Count,
              "The other player's hand should remain unchanged despite being illegally targeted by a village."
            );
            Assert.AreEqual(
              1, 
              game.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a village is played on an invalid target."
            );
            Assert.AreEqual(
              TestSetup.CardTypeVillage, 
              game.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a village after one is played."
            );
        }

        [TestCategory("VillageActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMultipleVillageTargets() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 1;

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
              "No actions should remain after a village is played on a multiple target set."
            );
            Assert.AreEqual(
              4,
              game.CurrentPlayer.Hand.Count,
              "A card should be missing after a village is played on a multiple target set."
            );
            Assert.AreEqual(
              5,
              game.Players[1].Hand.Count,
              "The other player's hand should remain unchanged despite being illegally targeted by a village."
            );
            Assert.AreEqual(
              1, 
              game.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a village is played a multiple target set."
            );
            Assert.AreEqual(
              TestSetup.CardTypeVillage, 
              game.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a village after one is played."
            );
        }

        [TestCategory("VillageActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestDuplicateVillageTargets() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 1;

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
              "No actions should remain after a village is played on a duplicate target set."
            );
            Assert.AreEqual(
              4,
              game.CurrentPlayer.Hand.Count,
              "A card should be missing after a village is played on a duplicate target set."
            );
            Assert.AreEqual(
              1, 
              game.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a village is played a duplicate target set."
            );
            Assert.AreEqual(
              TestSetup.CardTypeVillage, 
              game.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a village after one is played."
            );
        }

        #endregion
    }
}
