using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame.Test
{
    [TestClass]
    public class CellarTests {

        #region Test Setup

        [TestInitialize]
        public void CellarTestsSetup() {
            TestSetup.InitializeTypes();
        }

        #endregion

        #region Tests

        [TestCategory("CellarActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestStandardCellarActionApplication() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            handCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
              (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);

            Card cellarCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeCellar)) {
                    cellarCard = card;
                    break;
                }
            }

            game.CurrentStrategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(cellarCard);
            game.PlayStep();

            Card estateCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeEstate)) {
                    estateCard = card;
                    break;
                }
            }

            // Discarding the estate card:
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(estateCard, null);
            game.PlayStep();

            // Drawing the replacement card (mine):
            Card mineCard = game.State.CurrentPlayer.Deck[0];
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, game.State.CurrentPlayer);
            game.PlayStep();
            
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.RemainingActions, 
              "One action should remain after a cellar is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card after a cellar is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a cellar is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeCellar, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a cellar after it's been played."
            );
            Assert.IsTrue(
              game.State.CurrentPlayer.Hand.Contains(mineCard),
              "The hand should contain the drawn card (a mine) after a cellar is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.Discard.Count, 
              "The discard pile should have one card after a cellar is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeEstate, 
              game.State.CurrentPlayer.Discard[0].Type, 
              "The discard pile should have the discarded card (an estate) after a cellar is played."
            );
        }

        [TestCategory("CellarActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMultipleDiscardCellarAction() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 3;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            handCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
              (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);

            Card cellarCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeCellar)) {
                    cellarCard = card;
                    break;
                }
            }

            game.CurrentStrategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(cellarCard);
            game.PlayStep();

            IList<Card> estateCards = new List<Card>();
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeEstate)) {
                    estateCards.Add(card);
                }
            }

            // Discarding the estate cards:
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(estateCards, null);
            game.PlayStep();

            // Drawing the replacement cards (mines):
            IList<Card> mineCards = new List<Card>(game.State.CurrentPlayer.Deck);
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, game.State.CurrentPlayer);
            game.PlayStep();
            
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.RemainingActions, 
              "One action should remain after a cellar is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card after a cellar is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a cellar is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeCellar, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a cellar after it's been played."
            );
            foreach (Card card in mineCards) {
                Assert.IsTrue(
                  game.State.CurrentPlayer.Hand.Contains(card),
                  "The hand should contain each of the drawn cards (mines) after a cellar is played."
                );
            }
            Assert.AreEqual(
              3, 
              game.State.CurrentPlayer.Discard.Count, 
              "The discard pile should have three cards after a cellar is played."
            );
            foreach (Card card in game.State.CurrentPlayer.Discard) {
                Assert.AreEqual(
                TestSetup.CardTypeEstate, 
                  card.Type, 
                  "The discard pile should have the discarded card (an estate) after a cellar is played."
                );
            }
        }

        [TestCategory("CellarActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestNotEnoughCardsToDrawOnCellarAction() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 2;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            handCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
              (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);

            Card cellarCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeCellar)) {
                    cellarCard = card;
                    break;
                }
            }

            game.CurrentStrategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(cellarCard);
            game.PlayStep();

            IList<Card> estateCards = new List<Card>();
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeEstate)) {
                    estateCards.Add(card);
                }
            }

            // Discarding the estate cards:
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(estateCards, null);
            game.PlayStep();

            // Drawing the replacement cards (mines):
            IList<Card> mineCards = new List<Card>(game.State.CurrentPlayer.Deck);
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, game.State.CurrentPlayer);
            game.PlayStep();
            
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.RemainingActions, 
              "One action should remain after a cellar is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing one card after a cellar is played (as an estate will be reshuffled)."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a cellar is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeCellar, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a cellar after it's been played."
            );
            foreach (Card card in mineCards) {
                Assert.IsTrue(
                  game.State.CurrentPlayer.Hand.Contains(card),
                  "The hand should contain each of the drawn cards (mines) after a cellar is played."
                );
            }
            Assert.AreEqual(
              2, 
              game.State.CurrentPlayer.Deck.Count, 
              "The deck should have two cards after a cellar is played (due to reshuffling)."
            );

            Card missingEstate = null;
            foreach (Card card in estateCards) {
                Assert.IsTrue(
                  game.State.CurrentPlayer.Deck.Contains(card) || missingEstate == null, 
                  "The deck should have all but one of the discarded cards (estates) after a cellar is played and reshuffling occurs."
                );

                if (!game.State.CurrentPlayer.Deck.Contains(card)) {
                    missingEstate = card; 
                }
            }
            Assert.IsTrue(
              game.State.CurrentPlayer.Hand.Contains(missingEstate),
              "The hand should contain the final estate after cellar is played and reshuffling occurs."
            );
        }

        [TestCategory("CellarActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestNoDiscardCellarAction() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 3;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            handCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
              (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);

            Card cellarCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeCellar)) {
                    cellarCard = card;
                    break;
                }
            }

            game.CurrentStrategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(cellarCard);
            game.PlayStep();

            // Discarding no cards:
            IList<Card> cardsToDiscard = new List<Card>();
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(cardsToDiscard, null);
            game.PlayStep();

            // Drawing no replacement cards:
            IList<Card> mineCards = new List<Card>(game.State.CurrentPlayer.Deck);
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, game.State.CurrentPlayer);
            game.PlayStep();
            
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.RemainingActions, 
              "One action should remain after a cellar is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card after a cellar is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a cellar is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeCellar, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a cellar after it's been played."
            );
            foreach (Card card in mineCards) {
                Assert.IsFalse(
                  game.State.CurrentPlayer.Hand.Contains(card),
                  "The hand should not contain any of the deck cards (mines) after a cellar is played."
                );
            }
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.Discard.Count, 
              "The discard pile should have no cards after a cellar is played."
            );
        }

        [TestCategory("CellarActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestCellarDuplicateDiscardTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 2;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            handCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
              (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);

            Card cellarCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeCellar)) {
                    cellarCard = card;
                    break;
                }
            }

            game.CurrentStrategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(cellarCard);
            game.PlayStep();

            IList<Card> estateCards = new List<Card>();
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeEstate)) {
                    estateCards.Add(card);
                    estateCards.Add(card);
                    break;
                }
            }

            // Discarding the duplicate estate cards:
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(estateCards, null);
            game.PlayStep();

            // Drawing the replacement cards (mines):
            Card mineCard = game.State.CurrentPlayer.Deck[0];
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, game.State.CurrentPlayer);
            game.PlayStep();
            
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.RemainingActions, 
              "One action should remain after a cellar is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card after a cellar is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a cellar is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeCellar, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a cellar after it's been played."
            );
            Assert.IsFalse(
              game.State.CurrentPlayer.Hand.Contains(mineCard),
              "The hand should not contain the drawn card after an illegal cellar discard target is played."
            );
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.Discard.Count, 
              "The discard pile should have no cards after an illegal cellar discard target is played."
            );
            Assert.IsTrue(
              game.State.CurrentPlayer.Hand.Contains(estateCards[0]),
              "The hand should still contain the discard target card after an illegal target is played."
            );
        }

        [TestCategory("CellarActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestInvalidCellarTargetPlayerAction() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            handCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
              (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);

            Card cellarCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeCellar)) {
                    cellarCard = card;
                    break;
                }
            }

            game.CurrentStrategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(cellarCard);
            game.PlayStep();

            Card estateCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeEstate)) {
                    estateCard = card;
                    break;
                }
            }

            // Discarding the estate card:
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(estateCard, null);
            game.PlayStep();

            // Attempting to draw the replacement card (mine) for the wrong player:
            Card mineCard = game.State.CurrentPlayer.Deck[0];
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, game.Players[1]);
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a cellar is played on the wrong player."
            );
            Assert.AreEqual(
              3,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing two cards after a cellar is played on the wrong player."
            );
            Assert.AreEqual(
              5,
              game.Players[1].Hand.Count,
              "The other player's hand should remain unchanged after a cellar is played on the wrong player."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a cellar is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeCellar, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a cellar after it's been played."
            );
            Assert.IsFalse(
              game.State.CurrentPlayer.Hand.Contains(mineCard),
              "The current player's hand should not contain the drawn card (a mine) after a cellar is played invalidly."
            );
            Assert.IsFalse(
              game.Players[1].Hand.Contains(mineCard),
              "The other player's hand should not contain the drawn card (a mine) after a cellar is played invalidly."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.Discard.Count, 
              "The discard pile should have one card after a cellar is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeEstate, 
              game.State.CurrentPlayer.Discard[0].Type, 
              "The discard pile should have the discarded card (an estate) after a cellar is played."
            );
        }

        [TestCategory("CellarActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMultipleCellarTargetPlayersAction() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            handCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
              (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);

            Card cellarCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeCellar)) {
                    cellarCard = card;
                    break;
                }
            }

            game.CurrentStrategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(cellarCard);
            game.PlayStep();

            Card estateCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeEstate)) {
                    estateCard = card;
                    break;
                }
            }

            // Discarding the estate card:
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(estateCard, null);
            game.PlayStep();

            // Attempting to draw the replacement card (mine) for multiple players:
            Card mineCard = game.State.CurrentPlayer.Deck[0];
            game.CurrentStrategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(null, new List<Player>(game.Players));
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a cellar is played on multiple players."
            );
            Assert.AreEqual(
              3,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing two cards after a cellar is played on multiple players."
            );
            Assert.AreEqual(
              5,
              game.Players[1].Hand.Count,
              "The other player's hand should remain unchanged after a cellar is played on multiple players."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a cellar is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeCellar, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a cellar after it's been played."
            );
            Assert.IsFalse(
              game.State.CurrentPlayer.Hand.Contains(mineCard),
              "The current player's hand should not contain the drawn card (a mine) after a cellar is played invalidly."
            );
            Assert.IsFalse(
              game.Players[1].Hand.Contains(mineCard),
              "The other player's hand should not contain the drawn card (a mine) after a cellar is played invalidly."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.Discard.Count, 
              "The discard pile should have one card after a cellar is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeEstate, 
              game.State.CurrentPlayer.Discard[0].Type, 
              "The discard pile should have the discarded card (an estate) after a cellar is played."
            );
        }

        [TestCategory("CellarActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestCellarDiscardTargetDiscardedAction() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            handCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
              (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);

            Card cellarCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeCellar)) {
                    cellarCard = card;
                    break;
                }
            }

            game.CurrentStrategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(cellarCard);
            game.PlayStep();

            Card estateCard = game.GetCardsByType(TestSetup.CardTypeEstate)[0];
            game.TrashCard(estateCard);

            // Attempting to discard the trashed estate card:
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(estateCard, null);
            game.PlayStep();

            // Drawing the replacement card (mine):
            Card mineCard = game.State.CurrentPlayer.Deck[0];
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, game.State.CurrentPlayer);
            game.PlayStep();
            
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.RemainingActions, 
              "An action should remain even though a cellar is played with an invalid discard target."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card after a cellar is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a cellar is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeCellar, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a cellar after it's been played."
            );
            Assert.IsFalse(
              game.State.CurrentPlayer.Hand.Contains(mineCard),
              "The hand should not contain the drawn card (a mine) after a cellar is played with an invalid discard target."
            );
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.Discard.Count, 
              "The discard pile should have no cards after a cellar is played with an invalid discard target."
            );
        }

        [TestCategory("CellarActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestCellarDiscardTargetTrashedAction() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            handCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
              (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);

            Card cellarCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeCellar)) {
                    cellarCard = card;
                    break;
                }
            }

            game.CurrentStrategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(cellarCard);
            game.PlayStep();

            Card estateCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeEstate)) {
                    estateCard = card;
                    break;
                }
            }
            game.State.CurrentPlayer.DiscardCard(estateCard);

            // Attempting to discard an already discarded estate card:
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(estateCard, null);
            game.PlayStep();

            // Drawing the replacement card (mine):
            Card mineCard = game.State.CurrentPlayer.Deck[0];
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, game.State.CurrentPlayer);
            game.PlayStep();
            
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.RemainingActions, 
              "An action should remain even though a cellar is played with an invalid discard target."
            );
            Assert.AreEqual(
              3,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing two cards after a cellar is played following a previous discard."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a cellar is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeCellar, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a cellar after it's been played."
            );
            Assert.IsFalse(
              game.State.CurrentPlayer.Hand.Contains(mineCard),
              "The hand should not contain the drawn card (a mine) after a cellar is played with an invalid discard target."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.Discard.Count, 
              "The discard pile should have one card from before when the cellar was played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeEstate, 
              game.State.CurrentPlayer.Discard[0].Type, 
              "The discard pile should have the previously discarded card (an estate) after a cellar is played."
            );
        }

        #endregion
    }
}