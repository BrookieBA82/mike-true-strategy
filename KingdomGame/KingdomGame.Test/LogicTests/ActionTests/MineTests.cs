using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame.Test
{
    [TestClass]
    public class MineTests {

        #region Test Setup

        [TestInitialize]
        public void MineTestsSetup() {
            TestSetup.InitializeTypes();
        }

        #endregion

        #region Tests

        [TestCategory("MineActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestStandardMineActionApplication() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeSilver.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card mineCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMine)) {
                    mineCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(mineCard);
            game.PlayStep();

            Card copperCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeCopper)) {
                    copperCard = card;
                    break;
                }
            }

            // Trashing the copper card:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(copperCard, null);
            game.PlayStep();

            // Acquiring the silver card:
            Card silverCard = game.GetCardsByType(TestSetup.CardTypeSilver)[0];
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>() { TestSetup.CardTypeSilver } );
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a mine is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card remain after a mine is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a mine is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeMine, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a mine after it's been played."
            );
            Assert.IsTrue(
              game.State.CurrentPlayer.Hand.Contains(silverCard),
              "The hand should contain the gained card (a silver) after a mine is played."
            );
            Assert.AreEqual(
              1, 
              game.Trash.Count, 
              "The trash should have one card after a mine is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeCopper, 
              game.Trash[0].Type, 
              "The trash should have the trashed card (a copper) after a mine is played."
            );
            Assert.AreEqual(
              2, 
              game.State.CurrentPlayer.RemainingMoney, 
              "The player should have gained one money as a result of playing a mine."
            );
        }

        [TestCategory("MineActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMineNonUpgradeActionApplication() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card mineCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMine)) {
                    mineCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(mineCard);
            game.PlayStep();

            Card copperCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeCopper)) {
                    copperCard = card;
                    break;
                }
            }

            // Trashing the copper card:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(copperCard, null);
            game.PlayStep();

            // Acquiring another copper card:
            copperCard = game.GetCardsByType(TestSetup.CardTypeCopper)[0];
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>() { TestSetup.CardTypeCopper } );
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a mine is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card remain after a mine is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a mine is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeMine, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a mine after it's been played."
            );
            Assert.IsTrue(
              game.State.CurrentPlayer.Hand.Contains(copperCard),
              "The hand should contain the gained card (a copper) after a mine is played."
            );
            Assert.AreEqual(
              1, 
              game.Trash.Count, 
              "The trash should have one card after a mine is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeCopper, 
              game.Trash[0].Type, 
              "The trash should have the trashed card (a copper) after a mine is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.RemainingMoney, 
              "The player should have not gained any money as a result of playing a mine to get the same card."
            );
        }

        [TestCategory("MineActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMineNoTreasureToTrash() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeSilver.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card mineCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMine)) {
                    mineCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(mineCard);
            game.PlayStep();

            // Attempt to trash as the first action (no valid card):
            game.PlayStep();

            // Attempting to acquire a silver card:
            Card silverCard = game.GetCardsByType(TestSetup.CardTypeSilver)[0];
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>() { TestSetup.CardTypeSilver } );
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a mine is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card remain after a mine is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a mine is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeMine, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a mine after it's been played."
            );
            Assert.IsFalse(
              game.State.CurrentPlayer.Hand.Contains(silverCard),
              "The hand should not contain the gained card (a silver) after a mine is played."
            );
            Assert.AreEqual(
              0, 
              game.Trash.Count, 
              "The trash should not have any cards after a mine is played without a valid target."
            );
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingMoney, 
              "The player should have not gained any money if the mine had no valid target."
            );
        }

        [TestCategory("MineActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMineNonTreasureSelectedToTrash() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeSilver.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card mineCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMine)) {
                    mineCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(mineCard);
            game.PlayStep();

            // Attempt to trash a non-valid card the first action:
            Card estateCard = null;
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeEstate)) {
                    estateCard = card;
                    break;
                }
            }
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(estateCard, null);
            game.PlayStep();

            // Attempting to acquire a silver card:
            Card silverCard = game.GetCardsByType(TestSetup.CardTypeSilver)[0];
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>() { TestSetup.CardTypeSilver } );
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a mine is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card after a mine is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a mine is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeMine, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a mine after it's been played."
            );
            Assert.IsFalse(
              game.State.CurrentPlayer.Hand.Contains(silverCard),
              "The hand should not contain the gained card (a silver) after a mine is played."
            );
            Assert.AreEqual(
              0, 
              game.Trash.Count, 
              "The trash should not have any cards after a mine is played without a valid target."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.RemainingMoney, 
              "The player should have not gained any money if the mine had no valid target."
            );
        }

        [TestCategory("MineActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMineMultipleTargetsToTrash() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeSilver.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card mineCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMine)) {
                    mineCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(mineCard);
            game.PlayStep();

            // Attempt to trash multiple the first action:
            IList<Card> copperCards = new List<Card>();
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeCopper)) {
                    copperCards.Add(card);
                }
            }

            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(copperCards, null);
            game.PlayStep();

            // Attempting to acquire a silver card:
            Card silverCard = game.GetCardsByType(TestSetup.CardTypeSilver)[0];
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>() { TestSetup.CardTypeSilver } );
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a mine is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card after a mine is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a mine is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeMine, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a mine after it's been played."
            );
            Assert.IsFalse(
              game.State.CurrentPlayer.Hand.Contains(silverCard),
              "The hand should not contain the gained card (a silver) after a mine is played after multiple discards."
            );
            Assert.AreEqual(
              0, 
              game.Trash.Count, 
              "The trash should not have any cards after a mine is played after multiple discards."
            );
            Assert.AreEqual(
              2, 
              game.State.CurrentPlayer.RemainingMoney, 
              "The player should have not gained any money if the mine was played with multiple discards."
            );
        }

        [TestCategory("MineActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMineDuplicateTargetsToTrash() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeSilver.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card mineCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMine)) {
                    mineCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(mineCard);
            game.PlayStep();

            // Attempt to trash duplicate cards the first action:
            IList<Card> copperCards = new List<Card>();
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeCopper)) {
                    copperCards.Add(card);
                    copperCards.Add(card);
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(copperCards, null);
            game.PlayStep();

            // Attempting to acquire a silver card:
            Card silverCard = game.GetCardsByType(TestSetup.CardTypeSilver)[0];
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>() { TestSetup.CardTypeSilver } );
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a mine is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card after a mine is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a mine is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeMine, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a mine after it's been played."
            );
            Assert.IsFalse(
              game.State.CurrentPlayer.Hand.Contains(silverCard),
              "The hand should not contain the gained card (a silver) after a mine is played after duplicate discards."
            );
            Assert.AreEqual(
              0, 
              game.Trash.Count, 
              "The trash should not have any cards after a mine is played after duplicate discards."
            );
            Assert.AreEqual(
              2, 
              game.State.CurrentPlayer.RemainingMoney, 
              "The player should have not gained any money if the mine was played with duplicate discards."
            );
        }

        [TestCategory("MineActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMineTreasureToTrashNotInHand() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeSilver.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 6;
            playerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 4;
            handCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
              (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);

            Card mineCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMine)) {
                    mineCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(mineCard);
            game.PlayStep();

            // Attempt to trash a card from the deck as the first action:
            Card copperCard = null;
            foreach (Card card in game.State.CurrentPlayer.Deck) {
                if (card.Type.Equals(TestSetup.CardTypeCopper)) {
                    copperCard = card;
                    break;
                }
            }
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(copperCard, null);
            game.PlayStep();

            // Attempting to acquire a silver card:
            Card silverCard = game.GetCardsByType(TestSetup.CardTypeSilver)[0];
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>() { TestSetup.CardTypeSilver } );
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a mine is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card remain after a mine is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a mine is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeMine, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a mine after it's been played."
            );
            Assert.IsFalse(
              game.State.CurrentPlayer.Hand.Contains(silverCard),
              "The hand should not contain the gained card (a silver) after a mine is played."
            );
            Assert.AreEqual(
              0, 
              game.Trash.Count, 
              "The trash should not have any cards after a mine is played without a valid target."
            );
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingMoney, 
              "The player should have not gained any money if the mine had no valid target."
            );
        }

        [TestCategory("MineActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMineUpgradeTooExpensive() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeGold.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card mineCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMine)) {
                    mineCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(mineCard);
            game.PlayStep();

            Card copperCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeCopper)) {
                    copperCard = card;
                    break;
                }
            }

            // Trashing the copper card:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(copperCard, null);
            game.PlayStep();

            // Attempting to acquire the gold card:
            Card goldCard = game.GetCardsByType(TestSetup.CardTypeGold)[0];
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>() { TestSetup.CardTypeGold } );
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a mine is played."
            );
            Assert.AreEqual(
              3,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing two cards after a mine is played without a valid acquisition target."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a mine is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeMine, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a mine after it's been played."
            );
            Assert.IsFalse(
              game.State.CurrentPlayer.Hand.Contains(goldCard),
              "The hand should not contain the gained card (a gold) after a mine is played."
            );
            Assert.AreEqual(
              1, 
              game.Trash.Count, 
              "The trash should have one card after a mine is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeCopper, 
              game.Trash[0].Type, 
              "The trash should have the trashed card (a copper) after a mine is played."
            );
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingMoney, 
              "The player should have not gained any money if the mine had no valid acquisition target."
            );
        }

        [TestCategory("MineActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMineMultipleUpgradeTargets() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeSilver.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card mineCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMine)) {
                    mineCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(mineCard);
            game.PlayStep();

            Card copperCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeCopper)) {
                    copperCard = card;
                    break;
                }
            }

            // Trashing the copper card:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(copperCard, null);
            game.PlayStep();

            // Attempting to acquire multiple treasure cards:
            Card firstCard = game.GetCardsByType(TestSetup.CardTypeSilver)[0];
            Card secondCard = game.GetCardsByType(TestSetup.CardTypeCopper)[0];
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(
              new List<ITargetable>() { TestSetup.CardTypeSilver, TestSetup.CardTypeCopper } );
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a mine is played."
            );
            Assert.AreEqual(
              3,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing two cards after a mine is played without a valid acquisition target."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a mine is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeMine, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a mine after it's been played."
            );
            Assert.IsFalse(
              game.State.CurrentPlayer.Hand.Contains(firstCard),
              "The hand should not contain the gained card (a silver) after a mine is played on multiple targets."
            );
            Assert.IsFalse(
              game.State.CurrentPlayer.Hand.Contains(secondCard),
              "The hand should not contain the gained card (a silver) after a mine is played on multiple targets."
            );
            Assert.AreEqual(
              1, 
              game.Trash.Count, 
              "The trash should have one card after a mine is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeCopper, 
              game.Trash[0].Type, 
              "The trash should have the trashed card (a copper) after a mine is played."
            );
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingMoney, 
              "The player should have not gained any money if the mine had no valid acquisition target."
            );
        }

        [TestCategory("MineActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMineActionUpgradeTargetNotAvailable() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeSilver.Id] = 2;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeSilver.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card mineCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMine)) {
                    mineCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(mineCard);
            game.PlayStep();

            Card copperCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeCopper)) {
                    copperCard = card;
                    break;
                }
            }

            // Trashing the copper card:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(copperCard, null);
            game.PlayStep();

            // Attempting to acquire a silver card card:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>() { TestSetup.CardTypeSilver } );
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a mine is played."
            );
            Assert.AreEqual(
              3,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing two cards after a mine is played without a valid acquisition target."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a mine is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeMine, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a mine after it's been played."
            );
            Assert.AreEqual(
              1, 
              game.Trash.Count, 
              "The trash should have one card after a mine is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeCopper, 
              game.Trash[0].Type, 
              "The trash should have the trashed card (a copper) after a mine is played."
            );
            Assert.AreEqual(
              2, 
              game.State.CurrentPlayer.RemainingMoney, 
              "The player should have lost money if the mine had no valid acquisition target."
            );
        }

        [TestCategory("MineActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMineActionNonTreasureUpgradeTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card mineCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMine)) {
                    mineCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.CardSelectionStrategy = 
              new ScriptedCardSelectionStrategy(mineCard);
            game.PlayStep();

            Card copperCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeCopper)) {
                    copperCard = card;
                    break;
                }
            }

            // Trashing the copper card:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(copperCard, null);
            game.PlayStep();

            // Attempting to acquire a village card:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>() { TestSetup.CardTypeVillage } );
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a mine is played."
            );
            Assert.AreEqual(
              3,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing two cards after a mine is played without a valid acquisition target."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a mine is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeMine, 
              game.State.CurrentPlayer.PlayArea[0].Type, 
              "The play area should have a mine after it's been played."
            );
            Assert.AreEqual(
              1, 
              game.Trash.Count, 
              "The trash should have one card after a mine is played."
            );
            Assert.AreEqual(
              TestSetup.CardTypeCopper, 
              game.Trash[0].Type, 
              "The trash should have the trashed card (a copper) after a mine is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.RemainingMoney, 
              "The player should have lost money if the mine had no valid acquisition target."
            );
        }

        #endregion
    }
}
