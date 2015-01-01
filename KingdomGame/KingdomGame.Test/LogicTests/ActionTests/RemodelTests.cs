using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame.Test
{
    [TestClass]
    public class RemodelTests {

        #region Test Setup

        [TestInitialize]
        public void RemodelTestsSetup() {
            TestSetup.InitializeTypes();
        }

        #endregion

        #region Tests


        [TestCategory("RemodelActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestStandardRemodelActionApplication() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeRemodel.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeSmithy.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeRemodel.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card remodelCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeRemodel)) {
                    remodelCard = card;
                    break;
                }
            }

            game.CurrentStrategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(remodelCard);
            game.PlayStep();

            Card estateCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeEstate)) {
                    estateCard = card;
                    break;
                }
            }

            // Trashing the estate card:
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(estateCard, null);
            game.PlayStep();

            // Acquiring the smithy card:
            Card smithyCard = game.GetCardsByType(TestSetup.CardTypeSmithy)[0];
            game.CurrentStrategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>() { TestSetup.CardTypeSmithy });
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a remodel is played."
            );
            Assert.AreEqual(
              3,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing two cards after a remodel is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a remodel is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeRemodel, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a remodel after it's been played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.Discard.Count, 
              "The discard pile should have one card after a remodel is played."
            );
            Assert.AreEqual(
              smithyCard,
              game.State.CurrentPlayer.Discard[0],
              "The discard pile should contain the gained card (a smithy) after a remodel is played."
            );
            Assert.AreEqual(
              1, 
              game.Trash.Count, 
              "The trash should have one card after a remodel is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeEstate, 
              game.Trash[0].Type, 
              "The trash should have the trashed card (a estate) after a remodel is played."
            );
        }

        [TestCategory("RemodelActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestRemodelNonUpgradeActionApplication() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeRemodel.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeRemodel.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card remodelCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeRemodel)) {
                    remodelCard = card;
                    break;
                }
            }

            game.CurrentStrategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(remodelCard);
            game.PlayStep();

            Card estateCardToTrash = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeEstate)) {
                    estateCardToTrash = card;
                    break;
                }
            }

            // Trashing the estate card:
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(estateCardToTrash, null);
            game.PlayStep();

            // Acquiring another estate card:
            Card estateCardToGain = game.GetCardsByType(TestSetup.CardTypeEstate)[0];
            game.CurrentStrategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>() { TestSetup.CardTypeEstate });
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a remodel is played."
            );
            Assert.AreEqual(
              3,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing two cards after a remodel is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a remodel is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeRemodel, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a remodel after it's been played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.Discard.Count, 
              "The discard pile should have one card after a remodel is played."
            );
            Assert.AreEqual(
              estateCardToGain,
              game.State.CurrentPlayer.Discard[0],
              "The discard pile should contain the gained card (an estate) after a remodel is played."
            );
            Assert.AreEqual(
              1, 
              game.Trash.Count, 
              "The trash should have one card after a remodel is played."
            );
            Assert.AreEqual(
              estateCardToTrash, 
              game.Trash[0], 
              "The trash should have the trashed card (an estate) after a remodel is played."
            );
        }

        [TestCategory("RemodelActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestRemodelNoCardToTrash() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeRemodel.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeRemodel.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card remodelCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeRemodel)) {
                    remodelCard = card;
                    break;
                }
            }

            game.CurrentStrategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(remodelCard);
            game.PlayStep();

            // Attempt to trash as the first action (no valid card):
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(new List<Card>(), null);
            game.PlayStep();

            // Attempting to acquire an estate card:
            Card estateCard = game.GetCardsByType(TestSetup.CardTypeEstate)[0];
            game.CurrentStrategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>() { TestSetup.CardTypeEstate });
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a remodel is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card after a remodel is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a remodel is played."
            );
            Assert.AreEqual(
              remodelCard, 
              game.State.CurrentPlayer.PlayArea[0], 
              "The play area should have a remodel after it's been played."
            );
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.Discard.Count, 
              "The discard pile should have no cards after a remodel is played without trashing a card."
            );
            Assert.AreEqual(
              0, 
              game.Trash.Count, 
              "The trash should not have any cards after a remodel is played without a valid target."
            );
        }

        [TestCategory("RemodelActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestRemodelMultipleTargetsToTrash() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeRemodel.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeSmithy.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeRemodel.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card remodelCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeRemodel)) {
                    remodelCard = card;
                    break;
                }
            }

            game.CurrentStrategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(remodelCard);
            game.PlayStep();

            // Attempt to trash multiple the first action:
            IList<Card> estateCards = new List<Card>();
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeEstate)) {
                    estateCards.Add(card);
                }
            }

            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(estateCards, null);
            game.PlayStep();

            // Attempting to acquire a smithy card:
            Card smithyCard = game.GetCardsByType(TestSetup.CardTypeSmithy)[0];
            game.CurrentStrategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>() { TestSetup.CardTypeSmithy });
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a remodel is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card after a remodel is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a remodel is played."
            );
            Assert.AreEqual(
              remodelCard, 
              game.State.CurrentPlayer.PlayArea[0], 
              "The play area should have a remodel after it's been played."
            );
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.Discard.Count, 
              "The discard pile should have no cards after a remodel is played on multiple targets."
            );
            Assert.AreEqual(
              0, 
              game.Trash.Count, 
              "The trash should not have any cards after a remodel is played after multiple discards."
            );
        }

        [TestCategory("RemodelActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestRemodelDuplicateTargetsToTrash() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeRemodel.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeSmithy.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeRemodel.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card remodelCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeRemodel)) {
                    remodelCard = card;
                    break;
                }
            }

            game.CurrentStrategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(remodelCard);
            game.PlayStep();

            // Attempt to trash duplicate cards as the first action:
            Card estateCard = null;
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeEstate)) {
                    estateCard = card;
                }
            }

            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(new List<Card>() { estateCard, estateCard}, null);
            game.PlayStep();

            // Attempting to acquire a smithy card:
            Card smithyCard = game.GetCardsByType(TestSetup.CardTypeSmithy)[0];
            game.CurrentStrategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>() { TestSetup.CardTypeSmithy });
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a remodel is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card after a remodel is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a remodel is played."
            );
            Assert.AreEqual(
              remodelCard, 
              game.State.CurrentPlayer.PlayArea[0], 
              "The play area should have a remodel after it's been played."
            );
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.Discard.Count, 
              "The discard pile should have no cards after a remodel is played on duplicate targets."
            );
            Assert.AreEqual(
              0, 
              game.Trash.Count, 
              "The trash should not have any cards after a remodel is played after duplicate discards."
            );
        }

        [TestCategory("RemodelActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestRemodelCardToTrashNotInHand() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 24;
            gameCardCountsByTypeId[TestSetup.CardTypeRemodel.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 6;
            playerCardCountsByTypeId[TestSetup.CardTypeRemodel.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 4;
            handCardCountsByTypeId[TestSetup.CardTypeRemodel.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
              (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);

            Card remodelCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeRemodel)) {
                    remodelCard = card;
                    break;
                }
            }

            game.CurrentStrategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(remodelCard);
            game.PlayStep();

            // Attempt to trash a card from the deck as the first action:
            Card copperCard = null;
            foreach (Card card in game.State.CurrentPlayer.Deck) {
                if (card.Type.Equals(TestSetup.CardTypeCopper)) {
                    copperCard = card;
                    break;
                }
            }
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(copperCard, null);
            game.PlayStep();

            // Attempting to acquire an estate card:
            Card estateCard = game.GetCardsByType(TestSetup.CardTypeEstate)[0];
            game.CurrentStrategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>() { TestSetup.CardTypeEstate });
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a remodel is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card remain after a remodel is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a remodel is played."
            );
            Assert.AreEqual(
              remodelCard, 
              game.State.CurrentPlayer.PlayArea[0], 
              "The play area should have a remodel after it's been played."
            );
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.Discard.Count, 
              "The discard pile should not have any cards after a remodel is played without a valid target."
            );
            Assert.AreEqual(
              0, 
              game.Trash.Count, 
              "The trash should not have any cards after a remodel is played without a valid target."
            );
        }

        [TestCategory("RemodelActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestRemodelUpgradeTooExpensive() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeGold.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeRemodel.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeRemodel.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card remodelCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeRemodel)) {
                    remodelCard = card;
                    break;
                }
            }

            game.CurrentStrategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(remodelCard);
            game.PlayStep();

            Card estateCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeEstate)) {
                    estateCard = card;
                    break;
                }
            }

            // Trashing the estate card:
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(estateCard, null);
            game.PlayStep();

            // Attempting to acquire the gold card:
            Card goldCard = game.GetCardsByType(TestSetup.CardTypeGold)[0];
            game.CurrentStrategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>() { TestSetup.CardTypeGold });
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a remodel is played."
            );
            Assert.AreEqual(
              3,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing two cards after a remodel is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a remodel is played."
            );
            Assert.AreEqual(
              remodelCard, 
              game.State.CurrentPlayer.PlayArea[0], 
              "The play area should have a remodel after it's been played."
            );
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.Discard.Count, 
              "The discard pile should have no one cards after a remodel is played without a valid acquisition target."
            );
            Assert.AreEqual(
              1, 
              game.Trash.Count, 
              "The trash should have one card after a remodel is played."
            );
            Assert.AreEqual(
              estateCard, 
              game.Trash[0], 
              "The trash should have the trashed card (an estate) after a remodel is played."
            );
        }

        [TestCategory("RemodelActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestRemodelMultipleUpgradeTargets() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeRemodel.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeSmithy.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeRemodel.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card remodelCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeRemodel)) {
                    remodelCard = card;
                    break;
                }
            }

            game.CurrentStrategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(remodelCard);
            game.PlayStep();

            Card estateCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeEstate)) {
                    estateCard = card;
                    break;
                }
            }

            // Trashing the estate card:
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(estateCard, null);
            game.PlayStep();

            // Attempting to acquire multiple smithy cards:
            Card firstCard = game.GetCardsByType(TestSetup.CardTypeCopper)[0];
            Card secondCard = game.GetCardsByType(TestSetup.CardTypeEstate)[1];
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(
              new List<ITargetable>() {TestSetup.CardTypeCopper, TestSetup.CardTypeEstate});
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a remodel is played."
            );
            Assert.AreEqual(
              3,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing two cards after a remodel is played without a valid acquisition target."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a remodel is played."
            );
            Assert.AreEqual(
              remodelCard, 
              game.State.CurrentPlayer.PlayArea[0], 
              "The play area should have a remodel after it's been played."
            );
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.Discard.Count, 
              "The discard pile should have no cards after a remodel is played on an invalid acquisition target."
            );
            Assert.AreEqual(
              1, 
              game.Trash.Count, 
              "The trash should have one card after a remodel is played."
            );
            Assert.AreEqual(
              estateCard, 
              game.Trash[0], 
              "The trash should have the trashed card (a estate) after a remodel is played."
            );
        }

        [TestCategory("RemodelActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestRemodelActionUpgradeTargetNotAvailable() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeRemodel.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeSmithy.Id] = 2;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeRemodel.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeSmithy.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card remodelCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeRemodel)) {
                    remodelCard = card;
                    break;
                }
            }

            game.CurrentStrategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(remodelCard);
            game.PlayStep();

            Card estateCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeEstate)) {
                    estateCard = card;
                    break;
                }
            }

            // Trashing the estate card:
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(estateCard, null);
            game.PlayStep();

            // Attempting to acquire the smithy card:           
            game.CurrentStrategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>() {TestSetup.CardTypeSmithy });
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a remodel is played."
            );
            Assert.AreEqual(
              3,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing two cards after a remodel is played without a valid acquisition target."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a remodel is played."
            );
            Assert.AreEqual(
              remodelCard, 
              game.State.CurrentPlayer.PlayArea[0], 
              "The play area should have a remodel after it's been played."
            );
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.Discard.Count, 
              "The discard pile should have no cards after a remodel is played with an invalid acquisition target."
            );
            Assert.AreEqual(
              1, 
              game.Trash.Count, 
              "The trash should have one card after a remodel is played."
            );
        }

        #endregion

    }
}