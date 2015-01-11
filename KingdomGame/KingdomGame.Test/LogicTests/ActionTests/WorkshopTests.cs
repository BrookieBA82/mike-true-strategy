using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame.Test
{
    [TestClass]
    public class WorkshopTests {

        #region Test Setup

        [TestInitialize]
        public void WorkshopTestsSetup() {
            TestSetup.InitializeTypes();
        }

        #endregion

        #region Tests

        [TestCategory("WorkshopActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestStandardWorkshopActionApplication() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeWorkshop.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeWorkshop.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            game.PlayStep();

            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>() { TestSetup.CardTypeVillage });
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a workshop is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card remain after a workshop is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a workshop is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeWorkshop, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a workshop after it's been played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.Discard.Count, 
              "The discard pile should have one card after a workshop is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeVillage,
              game.State.CurrentPlayer.Discard[0].Type,  
              "The discard pile should have the gained card (a village) after a workshop is played."
            );
        }

        [TestCategory("WorkshopActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestWorkshopActionCostLimit() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeGold.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeWorkshop.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeWorkshop.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            game.PlayStep();

            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>() { TestSetup.CardTypeGold });
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a workshop is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card remain after a workshop is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a workshop is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeWorkshop, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a workshop after it's been played."
            );
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.Discard.Count, 
              "The discard pile should have no cards if the target card of the workshop costs too much."
            );
        }

        [TestCategory("WorkshopActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMultipleWorkshopTargets() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeSilver.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeWorkshop.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeWorkshop.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            game.PlayStep();

            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(
              new List<ITargetable>() { TestSetup.CardTypeVillage, TestSetup.CardTypeSilver });
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a workshop is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card remain after a workshop is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a workshop is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeWorkshop, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a workshop after it's been played."
            );
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.Discard.Count, 
              "The discard pile should have no cards if the multiple workshop acquisition targets were specified."
            );
        }

        [TestCategory("WorkshopActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestWorkshopActionUpgradeTargetNotAvailable() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeGold.Id] = 2;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeWorkshop.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeGold.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeWorkshop.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            game.PlayStep();

            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>() { TestSetup.CardTypeGold });
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a workshop is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card remain after a workshop is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a workshop is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeWorkshop, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a workshop after it's been played."
            );
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.Discard.Count, 
              "The discard pile should have no cards if the target type of the workshop has no cards remaining."
            );
        }

        [TestCategory("WorkshopActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestScriptedEmptyWorkshopActionTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeGold.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeWorkshop.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeWorkshop.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            game.PlayStep();

            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>());
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a workshop is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card remain after a workshop is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a workshop is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeWorkshop, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a workshop after it's been played."
            );
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.Discard.Count, 
              "The discard pile should have no cards if no target card is specified."
            );
        }

        #endregion
    }
}
