using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame.Test
{
    [TestClass]
    public class DiscardingTests {

        #region Test Setup

        [TestInitialize]
        public void DiscardingTestsSetup() {
            TestSetup.InitializeTypes();
        }

        #endregion

        #region Tests

        #region Random Discard Tests

        [TestCategory("DiscardingTest"), TestMethod]
        public void TestSingleOptionToDiscard() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            Card onlyCardInHand = game.State.CurrentPlayer.Hand[0];
            IList<Card> cardsToDiscard = game.CurrentStrategy.DiscardingStrategiesByPlayerId[game.State.CurrentPlayer.Id]
              .FindOptimalDiscardingStrategy(game, game.State.CurrentPlayer, 1);

            Assert.AreEqual(
              1, 
              cardsToDiscard.Count, 
              "One card should be discarded if there is one to do so with."
            );

            Assert.AreEqual(
              onlyCardInHand,
              cardsToDiscard[0],
              "The only card in the hand should be the one discarded at random."
            );
        }

        [TestCategory("DiscardingTest"), TestMethod]
        public void TestNoCardsToDiscard() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            IList<Card> cardsToDiscard = game.CurrentStrategy.DiscardingStrategiesByPlayerId[game.State.CurrentPlayer.Id]
              .FindOptimalDiscardingStrategy(game, game.State.CurrentPlayer, 1);

            Assert.AreEqual(
              0, 
              cardsToDiscard.Count, 
              "No cards should ever be discarded if a player's hand is empty."
            );
        }

        [TestCategory("DiscardingTest"), TestMethod]
        public void TestInsufficientCardsToDiscard() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            Card onlyCardInHand = game.State.CurrentPlayer.Hand[0];
            IList<Card> cardsToDiscard = game.CurrentStrategy.DiscardingStrategiesByPlayerId[game.State.CurrentPlayer.Id]
              .FindOptimalDiscardingStrategy(game, game.State.CurrentPlayer, 2);

            Assert.AreEqual(
              0, 
              cardsToDiscard.Count, 
              "No cards should be discarded if the number to discard exceeds the size of the player's hand."
            );
        }

        #endregion

        #region Scripted Discard Tests

        [TestCategory("DiscardingTest"), TestMethod]
        public void TestEmptyScriptedDiscardTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            IDiscardingStrategy strategy = new ScriptedDiscardingStrategy(new List<Card>());
            IList<Card> cardsToDiscard = strategy.FindOptimalDiscardingStrategy(game, game.State.CurrentPlayer, 0);

            Assert.AreEqual(
              0, 
              cardsToDiscard.Count, 
              "No cards should be discarded if none are specified to be discared."
            );
        }

        [TestCategory("DiscardingTest"), TestMethod]
        public void TestSingleCardScriptedDiscardTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 30;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            Card secondCardInHand = game.State.CurrentPlayer.Hand[1];
            IDiscardingStrategy strategy = new ScriptedDiscardingStrategy(new List<Card>() {secondCardInHand});
            IList<Card> cardsToDiscard = strategy.FindOptimalDiscardingStrategy(game, game.State.CurrentPlayer, 1);

            Assert.AreEqual(
              1, 
              cardsToDiscard.Count, 
              "A card should be discarded if it is available in the player's hand."
            );

            Assert.AreEqual(
              secondCardInHand,
              cardsToDiscard[0],
              "The selected card should be the only one which gets discarded."
            );
        }

        [TestCategory("DiscardingTest"), TestMethod]
        public void TestMultipleCardsScriptedDiscardTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeSilver.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 30;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeSilver.Id] = 2;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            Card secondCardInHand = game.State.CurrentPlayer.Hand[1];
            Card fourthCardInHand = game.State.CurrentPlayer.Hand[3];
            IDiscardingStrategy strategy = 
              new ScriptedDiscardingStrategy(new List<Card>() {secondCardInHand, fourthCardInHand});
            IList<Card> cardsToDiscard = strategy.FindOptimalDiscardingStrategy(game, game.State.CurrentPlayer, 2);

            Assert.AreEqual(
              2, 
              cardsToDiscard.Count, 
              "Two cards should be discarded if they are available in the player's hand."
            );

            Assert.IsTrue(
              cardsToDiscard.Contains(secondCardInHand),
              "The first card selected should be one of the cards which gets discarded."
            );

            Assert.IsTrue(
              cardsToDiscard.Contains(fourthCardInHand),
              "The second card selected should be one of the cards which gets discarded."
            );
        }

        [TestCategory("DiscardingTest"), TestMethod]
        public void TestDuplicateCardsScriptedDiscardTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeSilver.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 30;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeSilver.Id] = 2;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            Card secondCardInHand = game.State.CurrentPlayer.Hand[1];
            IDiscardingStrategy strategy = 
              new ScriptedDiscardingStrategy(new List<Card>() {secondCardInHand, secondCardInHand});
            IList<Card> cardsToDiscard = strategy.FindOptimalDiscardingStrategy(game, game.State.CurrentPlayer, 2);

            Assert.AreEqual(
              0, 
              cardsToDiscard.Count, 
              "No cards should be discarded if the specified set contains a duplicate card."
            );
        }

        [TestCategory("DiscardingTest"), TestMethod]
        public void TestInvalidCardInScriptedDiscardTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeSilver.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 30;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeSilver.Id] = 2;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            Card firstPlayerCard = game.Players[0].Hand[1];
            Card secondPlayerCard = game.Players[1].Hand[3];
            IDiscardingStrategy strategy = 
              new ScriptedDiscardingStrategy(new List<Card>() {firstPlayerCard, secondPlayerCard});
            IList<Card> cardsToDiscard = strategy.FindOptimalDiscardingStrategy(game, game.State.CurrentPlayer, 2);

            Assert.AreEqual(
              0, 
              cardsToDiscard.Count, 
              "No cards should be discarded if the target set size contains one or more invalid cards."
            );
        }

        [TestCategory("DiscardingTest"), TestMethod]
        public void TestNotEnoughCardsInScriptedDiscardTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeSilver.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 30;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeSilver.Id] = 2;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            Card secondCardInHand = game.State.CurrentPlayer.Hand[1];
            IDiscardingStrategy strategy = 
              new ScriptedDiscardingStrategy(new List<Card>() {secondCardInHand});
            IList<Card> cardsToDiscard = strategy.FindOptimalDiscardingStrategy(game, game.State.CurrentPlayer, 2);

            Assert.AreEqual(
              0, 
              cardsToDiscard.Count, 
              "No cards should be discarded if the target set size is less than the number specified to discard."
            );
        }

        [TestCategory("DiscardingTest"), TestMethod]
        public void TestTooManyCardsInScriptedDiscardTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeSilver.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 30;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeSilver.Id] = 2;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            Card secondCardInHand = game.State.CurrentPlayer.Hand[1];
            Card fourthCardInHand = game.State.CurrentPlayer.Hand[3];
            IDiscardingStrategy strategy = 
              new ScriptedDiscardingStrategy(new List<Card>() {secondCardInHand, fourthCardInHand});
            IList<Card> cardsToDiscard = strategy.FindOptimalDiscardingStrategy(game, game.State.CurrentPlayer, 1);

            Assert.AreEqual(
              0, 
              cardsToDiscard.Count, 
              "No cards should be discarded if the target set size is greater than the number specified to discard."
            );
        }

        #endregion

        #endregion
    }
}
