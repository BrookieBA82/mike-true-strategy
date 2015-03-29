using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace KingdomGame.Test
{
    [TestClass]
    public class TargetSelectionTests {

        #region Test Setup

        [TestInitialize]
        public void TargetSelectionTestsSetup() {
            TestSetup.InitializeTypes();
        }

        #endregion

        #region Tests

        [TestCategory("TargetSelectionTest"), TestMethod]
        public void TestEmptyScriptedPlayerTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card village = null;
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeVillage)) {
                    village = card;
                    break;
                }
            }

            game.State.SelectedPlay = village;
            TestUtilities.ForceGamePhase(game, Game.Phase.ACTION);
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<Card>(), new List<Player>());
            IList<ITargetable> targets = game.State.CurrentPlayer.Strategy.TargetSelectionStrategy.SelectTargets(
              game, 
              village, 
              village.Type.Actions[0].Create(game.State.CurrentPlayer)
            );

            Assert.AreEqual(0, targets.Count, "It should be possible to opt out of targeting a player.");
        }

        [TestCategory("TargetSelectionTest"), TestMethod]
        public void TestSingleOptionPlayerTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card village = null;
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeVillage)) {
                    village = card;
                    break;
                }
            }

            game.State.SelectedPlay = village;
            TestUtilities.ForceGamePhase(game, Game.Phase.ACTION);            IList<ITargetable> targets = game.State.CurrentPlayer.Strategy.TargetSelectionStrategy.SelectTargets(
              game, 
              village, 
              village.Type.Actions[0].Create(game.State.CurrentPlayer)
            );

            Assert.AreEqual(1, targets.Count, "Villages can only target the current player.");
            Assert.AreEqual(game.State.CurrentPlayer, targets[0], "Villages can only target the current player.");
        }

        [TestCategory("TargetSelectionTest"), TestMethod]
        public void TestInvalidScriptedPlayerTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card village = null;
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeVillage)) {
                    village = card;
                    break;
                }
            }

            game.State.SelectedPlay = village;
            TestUtilities.ForceGamePhase(game, Game.Phase.ACTION);            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, game.Players[1]);
            IList<ITargetable> targets = game.State.CurrentPlayer.Strategy.TargetSelectionStrategy.SelectTargets(
              game, 
              village, 
              village.Type.Actions[0].Create(game.State.CurrentPlayer)
            );

            Assert.AreEqual(0, targets.Count, "Non-current player selection should fail for villages.");
        }

        [TestCategory("TargetSelectionTest"), TestMethod]
        public void TestScriptedMultiplePlayerTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(3, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card militiaCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMilitia)) {
                    militiaCard = card;
                    break;
                }
            }

            ITargetSelectionStrategy strategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.Players[1], game.Players[2] });
            IList<ITargetable> targets = strategy.SelectTargets(
              game, 
              militiaCard, 
              TestSetup.CardTypeMilitia.Actions[0].Create(game.State.CurrentPlayer)
            );

            Assert.AreEqual(2, targets.Count, "The two selected players should be the ones which end up targeted.");
            Assert.IsTrue(targets.Contains(game.Players[1]), "Each selected player should be amongst the ones which end up targeted.");
            Assert.IsTrue(targets.Contains(game.Players[2]), "Each selected player should be amongst the ones which end up targeted.");
        }

        [TestCategory("TargetSelectionTest"), TestMethod]
        public void TestInvalidScriptedMultiplePlayerTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(3, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card militiaCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMilitia)) {
                    militiaCard = card;
                    break;
                }
            }

            ITargetSelectionStrategy strategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.Players[0], game.Players[2] });
            IList<ITargetable> targets = strategy.SelectTargets(
              game, 
              militiaCard, 
              TestSetup.CardTypeMilitia.Actions[0].Create(game.State.CurrentPlayer)
            );

            Assert.AreEqual(0, targets.Count, "No players should be targeted if any player in the scripted set is invalid.");
        }

        [TestCategory("TargetSelectionTest"), TestMethod]
        public void TestDuplicateScriptedPlayerTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card village = null;
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeVillage)) {
                    village = card;
                    break;
                }
            }

            game.State.SelectedPlay = village;
            TestUtilities.ForceGamePhase(game, Game.Phase.ACTION);
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(
              new List<Card>(), 
              new List<Player>() {game.State.CurrentPlayer, game.State.CurrentPlayer}
            );
            IList<ITargetable> targets = game.State.CurrentPlayer.Strategy.TargetSelectionStrategy.SelectTargets(
              game, 
              village, 
              village.Type.Actions[0].Create(game.State.CurrentPlayer)
            );

            Assert.AreEqual(0, targets.Count, "Duplicate player selection should fail for villages.");
        }

        [TestCategory("TargetSelectionTest"), TestMethod]
        public void TestEmptyScriptedCardTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeWorkshop.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeWorkshop.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card workshop = null;
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeWorkshop)) {
                    workshop = card;
                    break;
                }
            }

            game.State.SelectedPlay = workshop;
            TestUtilities.ForceGamePhase(game, Game.Phase.ACTION);
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<Card>(), new List<Player>());
            IList<ITargetable> targets = game.State.CurrentPlayer.Strategy.TargetSelectionStrategy.SelectTargets(
              game, 
              workshop, 
              workshop.Type.Actions[0].Create(game.State.CurrentPlayer)
            );

            Assert.AreEqual(0, targets.Count, "It should be possible to opt out of targeting a card.");
        }

        [TestCategory("TargetSelectionTest"), TestMethod]
        public void TestSingleOptionScriptedCardTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card cellar = null;
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeCellar)) {
                    cellar = card;
                    break;
                }
            }

            Card estate = null;
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeEstate)) {
                    estate = card;
                    break;
                }
            }

            game.State.SelectedPlay = cellar;
            TestUtilities.ForceGamePhase(game, Game.Phase.ACTION);
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>() { estate });
            IList<ITargetable> targets = game.State.CurrentPlayer.Strategy.TargetSelectionStrategy.SelectTargets(
              game, 
              cellar, 
              cellar.Type.Actions[0].Create(game.State.CurrentPlayer)
            );

            Assert.AreEqual(1, targets.Count, "The single selected card should be the one which ends up targeted.");
            Assert.AreEqual(estate, targets[0], "The selected card should be the one which ends up targeted.");
        }

        [TestCategory("TargetSelectionTest"), TestMethod]
        public void TestMultipleOptionScriptedCardTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card cellar = null;
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeCellar)) {
                    cellar = card;
                    break;
                }
            }

            Card estate = null;
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeEstate)) {
                    estate = card;
                    break;
                }
            }

            game.State.SelectedPlay = cellar;
            TestUtilities.ForceGamePhase(game, Game.Phase.ACTION);
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>() { estate });
            IList<ITargetable> targets = game.State.CurrentPlayer.Strategy.TargetSelectionStrategy.SelectTargets(
              game, 
              cellar, 
              cellar.Type.Actions[0].Create(game.State.CurrentPlayer)
            );

            Assert.AreEqual(1, targets.Count, "The single selected card should be the one which ends up targeted.");
            Assert.AreEqual(estate, targets[0], "The selected card should be the one which ends up targeted.");
        }

        [TestCategory("TargetSelectionTest"), TestMethod]
        public void TestSingleOptionScriptedCardTypeTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeWorkshop.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeWorkshop.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card workshop = null;
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeWorkshop)) {
                    workshop = card;
                    break;
                }
            }

            game.State.SelectedPlay = workshop;
            TestUtilities.ForceGamePhase(game, Game.Phase.ACTION);
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>() { TestSetup.CardTypeEstate });
            IList<ITargetable> targets = game.State.CurrentPlayer.Strategy.TargetSelectionStrategy.SelectTargets(
              game, 
              workshop, 
              workshop.Type.Actions[0].Create(game.State.CurrentPlayer)
            );

            Assert.AreEqual(1, targets.Count, "The single selected card should be the one which ends up targeted.");
            Assert.AreEqual(TestSetup.CardTypeEstate, targets[0], "The selected card should be the one which ends up targeted.");
        }

        [TestCategory("TargetSelectionTest"), TestMethod]
        public void TestMultipleOptionScriptedCardTypeTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeWorkshop.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeWorkshop.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card workshop = null;
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeWorkshop)) {
                    workshop = card;
                    break;
                }
            }

            game.State.SelectedPlay = workshop;
            TestUtilities.ForceGamePhase(game, Game.Phase.ACTION);
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<ITargetable>() { TestSetup.CardTypeEstate });
            IList<ITargetable> targets = game.State.CurrentPlayer.Strategy.TargetSelectionStrategy.SelectTargets(
              game, 
              workshop, 
              workshop.Type.Actions[0].Create(game.State.CurrentPlayer)
            );

            Assert.AreEqual(1, targets.Count, "The single selected card should be the one which ends up targeted.");
            Assert.AreEqual(TestSetup.CardTypeEstate, targets[0], "The selected card should be the one which ends up targeted.");
        }

        [TestCategory("TargetSelectionTest"), TestMethod]
        public void TestInvalidScriptedCardTarget() {
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

            Card workshop = null;
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeWorkshop)) {
                    workshop = card;
                    break;
                }
            }

            game.State.SelectedPlay = workshop;
            TestUtilities.ForceGamePhase(game, Game.Phase.ACTION);
            Card goldCard = game.GetCardsByType(TestSetup.CardTypeGold)[0];
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(goldCard, null);
            IList<ITargetable> targets = game.State.CurrentPlayer.Strategy.TargetSelectionStrategy.SelectTargets(
              game, 
              workshop, 
              workshop.Type.Actions[0].Create(game.State.CurrentPlayer)
            );

            Assert.AreEqual(0, targets.Count, "Too expensive card acquisition target should fail for workshops.");
        }

        [TestCategory("TargetSelectionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestScriptedMultipleCardTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 1;

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

            IList<Card> estateCards = new List<Card>();
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeEstate)) {
                    estateCards.Add(card);
                }
            }

            game.State.SelectedPlay = cellarCard;
            TestUtilities.ForceGamePhase(game, Game.Phase.ACTION);
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(estateCards, null);
            IList<ITargetable> targets = game.State.CurrentPlayer.Strategy.TargetSelectionStrategy.SelectTargets(
              game, 
              cellarCard, 
              cellarCard.Type.Actions[0].Create(game.State.CurrentPlayer)
            );

            Assert.AreEqual(3, targets.Count, "All estates from the hand should be a valid targeting option.");
            foreach (Card estateCard in estateCards) {
                Assert.IsTrue(targets.Contains(estateCard), "All estates from the hand should be valid targets.");
            }
        }

        [TestCategory("TargetSelectionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestInvalidScriptedMultipleCardTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 1;

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

            Card estateCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeEstate)) {
                    estateCard = card;
                    break;
                }
            }

            Card gameCard = game.GetCardsByType(TestSetup.CardTypeEstate)[0];

            game.State.SelectedPlay = cellarCard;
            TestUtilities.ForceGamePhase(game, Game.Phase.ACTION);
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(
              new List<Card>() {estateCard, gameCard},
              null
            );
            IList<ITargetable> targets = game.State.CurrentPlayer.Strategy.TargetSelectionStrategy.SelectTargets(
              game, 
              cellarCard, 
              cellarCard.Type.Actions[0].Create(game.State.CurrentPlayer)
            );

            Assert.AreEqual(0, targets.Count, "Target card set should be empty if one or more cards are invalid.");
        }

        [TestCategory("TargetSelectionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestDuplicateScriptedCardTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 1;

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

            Card estateCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeEstate)) {
                    estateCard = card;
                    break;
                }
            }

            game.State.SelectedPlay = cellarCard;
            TestUtilities.ForceGamePhase(game, Game.Phase.ACTION);
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(
              new List<Card>() {estateCard, estateCard},
              null
            );
            IList<ITargetable> targets = game.State.CurrentPlayer.Strategy.TargetSelectionStrategy.SelectTargets(
              game, 
              cellarCard, 
              cellarCard.Type.Actions[0].Create(game.State.CurrentPlayer)
            );

            Assert.AreEqual(0, targets.Count, "Target card set should be empty if one or more cards are duplicates.");

        }

        #endregion
    }
}
