using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame.Test
{
    [TestClass]
    public class MarketTests {

        #region Test Setup

        [TestInitialize]
        public void MarketTestsSetup() {
            TestSetup.InitializeTypes();
        }

        #endregion

        #region Tests

        [TestCategory("MarketActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestStandardMarketActionApplication() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMarket.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeMarket.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 4;
            handCardCountsByTypeId[TestSetup.CardTypeMarket.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);

            Card marketCard = TestUtilities.SetUpCardToPlay(game, TestSetup.CardTypeMarket);
            game.PlayStep();

            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.State.CurrentPlayer });
            game.PlayStep();

            TestUtilities.ConfirmCardPlayed(game, marketCard, 1);
            Assert.AreEqual(
              5,
              game.State.CurrentPlayer.Hand.Count,
              "A full hand should remain after a market is played."
            );
            Assert.AreEqual(
              2,
              game.State.CurrentPlayer.RemainingBuys,
              "The player should have two buys after a market is played."
            );
            Assert.AreEqual(
              2,
              game.State.CurrentPlayer.RemainingMoney,
              "The player should have two money after a market is played and a copper is drawn."
            );
        }

        [TestCategory("MarketActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMarketExistingMoneyPreserved() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMarket.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeMarket.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeMarket.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);
            game.State.CurrentPlayer.RemainingMoney += 3;

            Card marketCard = TestUtilities.SetUpCardToPlay(game, TestSetup.CardTypeMarket);
            game.PlayStep();

            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.State.CurrentPlayer });
            game.PlayStep();

            TestUtilities.ConfirmCardPlayed(game, marketCard, 1);
            Assert.AreEqual(
              5,
              game.State.CurrentPlayer.Hand.Count,
              "A full hand should remain after a market is played."
            );
            Assert.AreEqual(
              2,
              game.State.CurrentPlayer.RemainingBuys,
              "The player should have two buys after a market is played."
            );
            Assert.AreEqual(
              6,
              game.State.CurrentPlayer.RemainingMoney,
              "The player should have six money after a market is played."
            );
        }

        [TestCategory("MarketActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMarketExistingActionsPreserved() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMarket.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeMarket.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeMarket.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);
            game.State.CurrentPlayer.RemainingActions += 2;

            Card marketCard = TestUtilities.SetUpCardToPlay(game, TestSetup.CardTypeMarket);
            game.PlayStep();

            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.State.CurrentPlayer });
            game.PlayStep();

            TestUtilities.ConfirmCardPlayed(game, marketCard, 3);
            Assert.AreEqual(
              5,
              game.State.CurrentPlayer.Hand.Count,
              "A full hand should remain after a market is played."
            );
            Assert.AreEqual(
              2,
              game.State.CurrentPlayer.RemainingBuys,
              "The player should have two buys after a market is played."
            );
            Assert.AreEqual(
              3,
              game.State.CurrentPlayer.RemainingMoney,
              "The player should have three money after a market is played."
            );
        }

        [TestCategory("MarketActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMarketExistingBuysPreserved() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMarket.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeMarket.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeMarket.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);
            game.State.CurrentPlayer.RemainingBuys += 1;

            Card marketCard = TestUtilities.SetUpCardToPlay(game, TestSetup.CardTypeMarket);
            game.PlayStep();

            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.State.CurrentPlayer });
            game.PlayStep();

            TestUtilities.ConfirmCardPlayed(game, marketCard, 1);
            Assert.AreEqual(
              5,
              game.State.CurrentPlayer.Hand.Count,
              "A full hand should remain after a market is played."
            );
            Assert.AreEqual(
              3,
              game.State.CurrentPlayer.RemainingBuys,
              "The player should have three buys after a market is played."
            );
            Assert.AreEqual(
              3,
              game.State.CurrentPlayer.RemainingMoney,
              "The player should have three money after a market is played."
            );
        }

        [TestCategory("MarketActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMarketInsufficientCardsToDraw() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMarket.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeMarket.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeMarket.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);

            Card marketCard = TestUtilities.SetUpCardToPlay(game, TestSetup.CardTypeMarket);
            game.PlayStep();

            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.State.CurrentPlayer });
            game.PlayStep();

            TestUtilities.ConfirmCardPlayed(game, marketCard, 1);
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card after a market is played without any cards to draw."
            );
            Assert.AreEqual(
              2,
              game.State.CurrentPlayer.RemainingBuys,
              "The player should have two buys after a market is played."
            );
            Assert.AreEqual(
              3,
              game.State.CurrentPlayer.RemainingMoney,
              "The player should have three money after a market is played."
            );
        }

        [TestCategory("MarketActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMarketInvalidPlayerTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMarket.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeMarket.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeMarket.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);

            Card marketCard = TestUtilities.SetUpCardToPlay(game, TestSetup.CardTypeMarket);
            game.PlayStep();

            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.Players[1] });
            game.PlayStep();

            TestUtilities.ConfirmCardPlayed(game, marketCard, 0);
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card after a market is played on an invalid target player."
            );
            Assert.AreEqual(
              1,
              game.State.CurrentPlayer.RemainingBuys,
              "The player should have one buy after a market is played."
            );
            Assert.AreEqual(
              2,
              game.State.CurrentPlayer.RemainingMoney,
              "The player should have two money after a market is played."
            );
            Assert.AreEqual(
              5,
              game.Players[1].Hand.Count,
              "The other player's hand should be unaffected when invalidly targeted by a market."
            );
        }

        [TestCategory("MarketActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMarketMultiplePlayerTargets() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMarket.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeMarket.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeMarket.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);

            Card marketCard = TestUtilities.SetUpCardToPlay(game, TestSetup.CardTypeMarket);
            game.PlayStep();

            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>(game.Players));
            game.PlayStep();

            TestUtilities.ConfirmCardPlayed(game, marketCard, 0);
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card after a market is played on an multiple players."
            );
            Assert.AreEqual(
              1,
              game.State.CurrentPlayer.RemainingBuys,
              "The player should have one buy after a market is played."
            );
            Assert.AreEqual(
              2,
              game.State.CurrentPlayer.RemainingMoney,
              "The player should have two money after a market is played."
            );
            Assert.AreEqual(
              5,
              game.Players[1].Hand.Count,
              "The other player's hand should be unaffected when invalidly targeted by a market."
            );
        }

        [TestCategory("MarketActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMarketDuplicatePlayerTargets() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMarket.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeMarket.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeMarket.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);

            Card marketCard = TestUtilities.SetUpCardToPlay(game, TestSetup.CardTypeMarket);
            game.PlayStep();

            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.State.CurrentPlayer, game.State.CurrentPlayer });
            game.PlayStep();

            TestUtilities.ConfirmCardPlayed(game, marketCard, 0);
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card after a market is played on an duplicate players."
            );
            Assert.AreEqual(
              1,
              game.State.CurrentPlayer.RemainingBuys,
              "The player should have one buy after a market is played."
            );
            Assert.AreEqual(
              2,
              game.State.CurrentPlayer.RemainingMoney,
              "The player should have two money after a market is played."
            );
        }

        #endregion

    }
}