using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame.Test
{
    [TestClass]
    public class BuyingTests {

        #region Test Setup

        [TestInitialize]
        public void BuyingTestsSetup() {
            TestSetup.InitializeTypes();
        }

        #endregion

        #region Tests

        #region Buying Success Tests

        [TestCategory("BuyingTest"), TestMethod]
        public void TestEmptyScriptedCardBuy() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            TestUtilities.ForceGamePhase(game, Game.Phase.BUY);
            game.State.CurrentPlayer.Strategy.BuyingStrategy = new ScriptedBuySelectionStrategy(new List<CardType>());
            game.PlayPhase();

            Assert.AreEqual(0, game.State.CurrentPlayer.RemainingBuys, "No buys should remain if the decision to make none is made.");
            Assert.AreEqual(3, game.State.CurrentPlayer.RemainingMoney, "All money should remain if no buy was made.");
            Assert.AreEqual(0, game.State.CurrentPlayer.Discard.Count, "No cards should be in discard pile without a buy.");
        }

        [TestCategory("BuyingTest"), TestMethod]
        public void TestSingleScriptedCardBuy() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            TestUtilities.ForceGamePhase(game, Game.Phase.BUY);
            game.State.CurrentPlayer.Strategy.BuyingStrategy 
                = new ScriptedBuySelectionStrategy(new List<CardType>() {TestSetup.CardTypeEstate});
            game.PlayPhase();

            Assert.AreEqual(0, game.State.CurrentPlayer.RemainingBuys, "No buys should remain after one has been made.");
            Assert.AreEqual(1, game.State.CurrentPlayer.RemainingMoney, "Balance should remain if a buy was made.");
            Assert.AreEqual(1, game.State.CurrentPlayer.PlayArea.Count, "One card should be in play area after a buy.");
            Assert.AreEqual(
              TestSetup.CardTypeEstate, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "Estate should be the single card in the play area pile after a buy."
            );
        }

        [TestCategory("BuyingTest"), TestMethod]
        public void TestMultipleScriptedCardBuys() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            TestUtilities.ForceGamePhase(game, Game.Phase.BUY);
            game.State.CurrentPlayer.Strategy.BuyingStrategy = new ScriptedBuySelectionStrategy(
                new List<CardType>() {TestSetup.CardTypeEstate, TestSetup.CardTypeEstate});
            game.State.CurrentPlayer.RemainingBuys = 2;
            game.PlayPhase();

            Assert.AreEqual(0, game.State.CurrentPlayer.RemainingBuys, "No buys should remain after one has been made.");
            Assert.AreEqual(0, game.State.CurrentPlayer.RemainingMoney, "No money should remain if it was all spent on buys.");
            Assert.AreEqual(2, game.State.CurrentPlayer.PlayArea.Count, "Two cards should be in play area after two buys.");
            Assert.AreEqual(
              TestSetup.CardTypeEstate, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "Estate should be the first card in the play area after a scripted buy."
            );
            Assert.AreEqual(
              TestSetup.CardTypeEstate, 
              game.State.CurrentPlayer.PlayArea[1].Type, 
              "Estate should be the second card in the play area after a scripted buy."
            );
        }

        #endregion

        #region Buying Failure Tests

        [TestCategory("BuyingTest"), TestMethod]
        public void TestInsufficientMoneyForCardBuy() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 5;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            TestUtilities.ForceGamePhase(game, Game.Phase.BUY);
            game.PlayPhase();

            // Note - (MT): The only option to buy is an estate, which can't be done with zero gold
            Assert.AreEqual(0, game.State.CurrentPlayer.RemainingBuys, "No buys should remain once an invalid selection is made.");
            Assert.AreEqual(0, game.State.CurrentPlayer.RemainingMoney, "No money should remain if it wasn't there to begin with.");
            Assert.AreEqual(0, game.State.CurrentPlayer.Discard.Count, "No cards should be in discard pile without a buy.");
        }

        [TestCategory("BuyingTest"), TestMethod]
        public void TestInsufficientBuysForCardBuys() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 8;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 4;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            TestUtilities.ForceGamePhase(game, Game.Phase.BUY);
            game.State.CurrentPlayer.Strategy.BuyingStrategy 
              = new ScriptedBuySelectionStrategy(new List<CardType>() {TestSetup.CardTypeEstate, TestSetup.CardTypeEstate});
            game.PlayPhase();

            // Note - (MT): There are not enough estates left to buy one, so no buy should occur
            Assert.AreEqual(0, game.State.CurrentPlayer.RemainingBuys, "No buys should remain once an invalid selection is made.");
            Assert.AreEqual(4, game.State.CurrentPlayer.RemainingMoney, "All money should remain if no buy was made.");
            Assert.AreEqual(0, game.State.CurrentPlayer.Discard.Count, "No cards should be in discard pile without a buy.");
        }

        [TestCategory("BuyingTest"), TestMethod]
        public void TestInsufficientCardsForCardBuy() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 5;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            TestUtilities.ForceGamePhase(game, Game.Phase.BUY);
            game.PlayPhase();

            // Note - (MT): There are no cards left to buy anything
            Assert.AreEqual(0, game.State.CurrentPlayer.RemainingBuys, "No buys should remain once an invalid selection is made.");
            Assert.AreEqual(5, game.State.CurrentPlayer.RemainingMoney, "All money should remain if no buy was made.");
            Assert.AreEqual(0, game.State.CurrentPlayer.Discard.Count, "No cards should be in discard pile without a buy.");
        }

        [TestCategory("BuyingTest"), TestMethod]
        public void TestSingleScriptedCardBuyNotPossible() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            TestUtilities.ForceGamePhase(game, Game.Phase.BUY);
            game.State.CurrentPlayer.Strategy.BuyingStrategy = new ScriptedBuySelectionStrategy(new List<CardType>() {TestSetup.CardTypeEstate});
            game.PlayPhase();

            // Note - (MT): There are not enough estates left to buy one, so no buy should occur
            Assert.AreEqual(0, game.State.CurrentPlayer.RemainingBuys, "No buys should remain once an invalid selection is made.");
            Assert.AreEqual(4, game.State.CurrentPlayer.RemainingMoney, "All money should remain if no buy was made.");
            Assert.AreEqual(0, game.State.CurrentPlayer.Discard.Count, "No cards should be in discard pile without a buy.");
        }

        [TestCategory("BuyingTest"), TestMethod]
        public void TestMultipleScriptedCardBuyNotPossible() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            TestUtilities.ForceGamePhase(game, Game.Phase.BUY);
            game.State.CurrentPlayer.Strategy.BuyingStrategy = new ScriptedBuySelectionStrategy(
                new List<CardType>() {TestSetup.CardTypeEstate, TestSetup.CardTypeEstate});
            game.State.CurrentPlayer.RemainingBuys = 2;
            game.PlayPhase();

            // Note - (MT): There are not enough cards left to buy two estates, so no buy should occur
            Assert.AreEqual(0, game.State.CurrentPlayer.RemainingBuys, "No buys should remain once an invalid selection is made.");
            Assert.AreEqual(4, game.State.CurrentPlayer.RemainingMoney, "All money should remain if no buy was made.");
            Assert.AreEqual(0, game.State.CurrentPlayer.Discard.Count, "No cards should be in discard pile without a buy.");
        }

        #endregion

        #region Buying Option Generation Tests

        [TestCategory("BuyingOptionTest"), TestMethod]
        public void TestSingleBuyNoOptions() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 8;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Assert.IsTrue(
              BuyingTests.IsBuyingOptionValid(game, new List<CardType>(), 1), 
              "Buying nothing should always be an option."
            );

            Assert.IsFalse(
              BuyingTests.IsBuyingOptionValid(game, new List<CardType>() {TestSetup.CardTypeCopper}, 1), 
              "Buying a card should never be an option when none are available."
            );
        }

        [TestCategory("BuyingOptionTest"), TestMethod]
        public void TestSingleBuySingleOption() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 11;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 0;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 5;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Assert.IsTrue(
              BuyingTests.IsBuyingOptionValid(game, new List<CardType>(), 1), 
              "Buying nothing should always be an option."
            );

            Assert.IsTrue(
              BuyingTests.IsBuyingOptionValid(game, new List<CardType>() {TestSetup.CardTypeCopper}, 1), 
              "Buying a card should be permitted if it is available and can be afforded."
            );

            Assert.IsFalse(
              BuyingTests.IsBuyingOptionValid(game, new List<CardType>() {TestSetup.CardTypeEstate}, 1), 
              "Buying a card should not be permitted if it cannot be afforded."
            );
        }

        [TestCategory("BuyingOptionTest"), TestMethod]
        public void TestSingleBuyMultipleOptions() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 5;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 7;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Assert.IsTrue(
              BuyingTests.IsBuyingOptionValid(game, new List<CardType>(), 1), 
              "Buying nothing should always be an option."
            );

            Assert.IsTrue(
              BuyingTests.IsBuyingOptionValid(game, new List<CardType>() {TestSetup.CardTypeCopper}, 1), 
              "Buying a card should be permitted if it is available and can be afforded."
            );

            Assert.IsTrue(
              BuyingTests.IsBuyingOptionValid(game, new List<CardType>() {TestSetup.CardTypeEstate}, 1), 
              "Buying a card should be permitted if it is available and can be afforded."
            );

            Assert.IsFalse(
              BuyingTests.IsBuyingOptionValid(game, new List<CardType>() {TestSetup.CardTypeVillage}, 1), 
              "Buying a card should not be permitted if it is not available."
            );

            Assert.IsFalse(
              BuyingTests.IsBuyingOptionValid(
                game, 
                new List<CardType>() {TestSetup.CardTypeCopper, TestSetup.CardTypeEstate},
                1), 
              "Buying two cards should not be permitted if only one buy remains."
            );
        }

        [TestCategory("BuyingOptionTest"), TestMethod]
        public void TestMultipleBuysNoOptions() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 8;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Assert.IsTrue(
              BuyingTests.IsBuyingOptionValid(game, new List<CardType>(), 2), 
              "Buying nothing should always be an option."
            );

            Assert.IsFalse(
              BuyingTests.IsBuyingOptionValid(game, new List<CardType>() {TestSetup.CardTypeCopper}, 2), 
              "Buying a card should never be an option when none are available."
            );
        }

        [TestCategory("BuyingOptionTest"), TestMethod]
        public void TestMultipleBuysSingleOption() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 11;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 0;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 5;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Assert.IsTrue(
              BuyingTests.IsBuyingOptionValid(game, new List<CardType>(), 2), 
              "Buying nothing should always be an option."
            );

            Assert.IsTrue(
              BuyingTests.IsBuyingOptionValid(game, new List<CardType>() {TestSetup.CardTypeCopper}, 2), 
              "Buying a card should be permitted if it is available and can be afforded."
            );

            Assert.IsFalse(
              BuyingTests.IsBuyingOptionValid(game, new List<CardType>() {TestSetup.CardTypeEstate}, 2), 
              "Buying a card should not be permitted if it cannot be afforded."
            );

            Assert.IsFalse(
              BuyingTests.IsBuyingOptionValid(
                game, 
                new List<CardType>() {TestSetup.CardTypeCopper, TestSetup.CardTypeEstate},
                2), 
              "Buying two cards should not be permitted if only enouch cards are available for one buy."
            );
        }

        [TestCategory("BuyingOptionTest"), TestMethod]
        public void TestMultipleBuysMultipleOptions() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Assert.IsTrue(
              BuyingTests.IsBuyingOptionValid(game, new List<CardType>(), 2), 
              "Buying nothing should always be an option."
            );

            Assert.IsTrue(
              BuyingTests.IsBuyingOptionValid(game, new List<CardType>() {TestSetup.CardTypeCopper}, 2), 
              "Buying a card should be permitted if it is available and can be afforded."
            );

            Assert.IsTrue(
              BuyingTests.IsBuyingOptionValid(game, new List<CardType>() {TestSetup.CardTypeEstate}, 2), 
              "Buying a card should be permitted if it is available and can be afforded."
            );

            Assert.IsTrue(
              BuyingTests.IsBuyingOptionValid(
                game, 
                new List<CardType>() {TestSetup.CardTypeCopper, TestSetup.CardTypeEstate}, 
                2),
              "Buying two cards should be permitted if they are available and can be afforded."
            );

            Assert.IsTrue(
              BuyingTests.IsBuyingOptionValid(
                game, 
                new List<CardType>() {TestSetup.CardTypeEstate, TestSetup.CardTypeCopper}, 
                2),
              "Buying two cards should be permitted if the reverse order can be bought."
            );

            Assert.IsTrue(
              BuyingTests.IsBuyingOptionValid(
                game, 
                new List<CardType>() {TestSetup.CardTypeCopper, TestSetup.CardTypeCopper}, 
                2),
              "Buying two of the same card type should be permitted if they are available and can be afforded."
            );

            Assert.IsFalse(
              BuyingTests.IsBuyingOptionValid(
                game, 
                new List<CardType>() {TestSetup.CardTypeEstate, TestSetup.CardTypeEstate}, 
                2), 
              "Buying two of the same card type should not be permitted if only one is available."
            );
        }

        #endregion

        #endregion

        #region Utility Functions

        private static bool IsBuyingOptionValid(Game game, IList<CardType> buyingOption, int maxBuys) {
            Game clone = game.Clone() as Game;
            TestUtilities.ForceGamePhase(clone, Game.Phase.BUY);
            clone.State.CurrentPlayer.Strategy.BuyingStrategy = new ScriptedBuySelectionStrategy(new List<CardType>(buyingOption));
            clone.State.CurrentPlayer.RemainingBuys = maxBuys;
            clone.PlayPhase();

            return (clone.State.CurrentPlayer.PlayArea.Count == buyingOption.Count);
        }

        #endregion
    }
}
