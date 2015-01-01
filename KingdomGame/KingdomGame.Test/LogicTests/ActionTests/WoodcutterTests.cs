using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame.Test
{
    [TestClass]
    public class WoodcutterTests {

        #region Test Setup

        [TestInitialize]
        public void WoodcutterTestsSetup() {
            TestSetup.InitializeTypes();
        }

        #endregion

        #region Tests

        [TestCategory("WoodcutterActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestStandardWoodcutterActionApplication() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeWoodcutter.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeWoodcutter.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card woodcutterCard = TestUtilities.SetUpCardToPlay(game, TestSetup.CardTypeWoodcutter);
            game.PlayStep();

            game.CurrentStrategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.State.CurrentPlayer });
            game.PlayStep();

            TestUtilities.ConfirmCardPlayed(game, woodcutterCard);
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card after a woodcutter is played."
            );
            Assert.AreEqual(
              2,
              game.State.CurrentPlayer.RemainingBuys,
              "The player should have two buys after a woodcutter is played."
            );
            Assert.AreEqual(
              2,
              game.State.CurrentPlayer.RemainingMoney,
              "The player should have two money after a woodcutter is played."
            );
        }

        [TestCategory("WoodcutterActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestWoodcutterExistingMoneyPreserved() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeWoodcutter.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeWoodcutter.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            game.State.CurrentPlayer.RemainingMoney += 3;

            Card woodcutterCard = TestUtilities.SetUpCardToPlay(game, TestSetup.CardTypeWoodcutter);
            game.PlayStep();

            game.CurrentStrategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.State.CurrentPlayer });
            game.PlayStep();

            TestUtilities.ConfirmCardPlayed(game, woodcutterCard);
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card after a woodcutter is played."
            );
            Assert.AreEqual(
              2,
              game.State.CurrentPlayer.RemainingBuys,
              "The player should have two buys after a woodcutter is played."
            );
            Assert.AreEqual(
              7,
              game.State.CurrentPlayer.RemainingMoney,
              "The player should have seven money after a woodcutter is played."
            );
        }

        [TestCategory("WoodcutterActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestWoodcutterExistingBuysPreserved() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeWoodcutter.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeWoodcutter.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            game.State.CurrentPlayer.RemainingBuys += 1;

            Card woodcutterCard = TestUtilities.SetUpCardToPlay(game, TestSetup.CardTypeWoodcutter);
            game.PlayStep();

            game.CurrentStrategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.State.CurrentPlayer });
            game.PlayStep();

            TestUtilities.ConfirmCardPlayed(game, woodcutterCard);
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card after a woodcutter is played."
            );
            Assert.AreEqual(
              3,
              game.State.CurrentPlayer.RemainingBuys,
              "The player should have three buys after a woodcutter is played."
            );
            Assert.AreEqual(
              2,
              game.State.CurrentPlayer.RemainingMoney,
              "The player should have two money after a woodcutter is played."
            );
        }

        [TestCategory("WoodcutterActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestWoodcutterInvalidPlayerTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeWoodcutter.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeWoodcutter.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeWoodcutter.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);

            Card woodcutterCard = TestUtilities.SetUpCardToPlay(game, TestSetup.CardTypeWoodcutter);
            game.PlayStep();

            game.CurrentStrategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.Players[1] });
            game.PlayStep();

            TestUtilities.ConfirmCardPlayed(game, woodcutterCard, 0);
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card after a woodcutter is played."
            );
            Assert.AreEqual(
              1,
              game.State.CurrentPlayer.RemainingBuys,
              "The player should have one buy after a woodcutter is played."
            );
            Assert.AreEqual(
              2,
              game.State.CurrentPlayer.RemainingMoney,
              "The player should have two money after a woodcutter is played."
            );
        }

        [TestCategory("WoodcutterActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestWoodcutterMultiplePlayerTargets() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeWoodcutter.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeWoodcutter.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeWoodcutter.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);

            Card woodcutterCard = TestUtilities.SetUpCardToPlay(game, TestSetup.CardTypeWoodcutter);
            game.PlayStep();

            game.CurrentStrategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>(game.Players));
            game.PlayStep();

            TestUtilities.ConfirmCardPlayed(game, woodcutterCard, 0);
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card after a woodcutter is played on an multiple players."
            );
            Assert.AreEqual(
              1,
              game.State.CurrentPlayer.RemainingBuys,
              "The player should have one buy after a woodcutter is played."
            );
            Assert.AreEqual(
              2,
              game.State.CurrentPlayer.RemainingMoney,
              "The player should have two money after a woodcutter is played."
            );
        }

        [TestCategory("WoodcutterActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestWoodcutterDuplicatePlayerTargets() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeWoodcutter.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeWoodcutter.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeWoodcutter.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);

            Card woodcutterCard = TestUtilities.SetUpCardToPlay(game, TestSetup.CardTypeWoodcutter);
            game.PlayStep();

            game.CurrentStrategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.State.CurrentPlayer, game.State.CurrentPlayer });
            game.PlayStep();

            TestUtilities.ConfirmCardPlayed(game, woodcutterCard, 0);
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card after a woodcutter is played on an duplicate players."
            );
            Assert.AreEqual(
              1,
              game.State.CurrentPlayer.RemainingBuys,
              "The player should have one buy after a woodcutter is played."
            );
            Assert.AreEqual(
              2,
              game.State.CurrentPlayer.RemainingMoney,
              "The player should have two money after a woodcutter is played."
            );
        }

        #endregion

    }
}