﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KingdomGame;
using System.Collections.Generic;

namespace KingdomGame.Test
{
    [TestClass]
    public class GameTests {

        #region Test Setup

        [TestInitialize]
        public void GameTestsSetup() {
            TestSetup.InitializeTypes();
        }

        #endregion

        #region Tests

        #region End of Game Tests

        [TestCategory("GameObjectTest"), TestCategory("EndOfGameTest"), TestMethod]
        public void TestNoPilesEmptyEndOfGameCheck() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 1;

            Game game = TestSetup.GenerateStartingGame (2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Assert.AreEqual(false, game.IsGameOver(), "The game should not be over because no piles are empty.");
        }

        [TestCategory("GameObjectTest"), TestCategory("EndOfGameTest"), TestMethod]
        public void TestSinglePileEmptyEndOfGameCheck() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 2;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 1;

            Game game = TestSetup.GenerateStartingGame (2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Assert.AreEqual(false, game.IsGameOver(), "The game should not be over because one non-critical pile is empty.");
        }

        [TestCategory("GameObjectTest"), TestCategory("EndOfGameTest"), TestMethod]
        public void TestCriticalPileEmptyEndOfGameCheck() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeProvince.Id] = 2;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeProvince.Id] = 1;

            Game game = TestSetup.GenerateStartingGame (2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Assert.AreEqual(true, game.IsGameOver(), "The game should be over because a critical pile is empty.");
        }

        [TestCategory("GameObjectTest"), TestCategory("EndOfGameTest"), TestMethod]
        public void TestEnoughPilesEmptyEndOfGameCheck() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            gameCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 2;
            gameCardCountsByTypeId[TestSetup.CardTypeWorkshop.Id] = 2;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeWorkshop.Id] = 1;

            Game game = TestSetup.GenerateStartingGame (2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Assert.AreEqual(true, game.IsGameOver(), "The game should be over because enough non-critical piles are empty.");
        }

        #endregion

        #region Clone Tests

        #region Clone Equality Tests

        [TestCategory("GameObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestGameCloneEquality()
        {
            Game game = TestSetup.GenerateSimpleGame(2);
            Game clone = game.Clone() as Game;

            Assert.AreEqual(game, clone, "Game does not match its clone.");
        }

        [TestCategory("GameObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestPreviousActionsCloneEquality() {
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
            foreach (Card card in game.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeVillage)) {
                    village = card;
                    break;
                }
            }

            game.CurrentStrategy.Phase = Game.Phase.TARGET;
            game.CurrentStrategy.ActionStack.Push(village.Type.Actions[0]);
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, game.CurrentPlayer);
            game.CurrentStrategy.SelectedCardId = village.Id;
            game.PlayStep();

            Game clone = game.Clone() as Game;

            Assert.AreEqual(
              game.CurrentStrategy, 
              clone.CurrentStrategy, 
              "Game strategy should match that of its clone after sharing a common initial previous action history."
            );
        }

        #endregion

        #region Clone Independence Tests

        #region Shallow Clone Independence Tests
        
        [TestCategory("GameObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestGameCurrentPlayerChangeCloneIndependence() {
            Game game = TestSetup.GenerateSimpleGame(2);
            Game clone = game.Clone() as Game;
            game.AdvanceTurn();

            Assert.AreNotEqual(
              game.CurrentPlayer, 
              clone.CurrentPlayer, 
              "Game should not match its clone on current player after advancing the turn."
            );
        }

        [TestCategory("GameObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestGameTurnNumberCloneIndependence() {
            Game game = TestSetup.GenerateSimpleGame(2);
            Game clone = game.Clone() as Game;
            game.AdvanceTurn();

            Assert.AreNotEqual(game, clone, "Game should not match its clone after advancing the turn.");
            Assert.AreNotEqual(
              game.TurnNumber, 
              clone.TurnNumber, 
              "Game's turn number should not match that of its clone after advancing the turn."
            );
        }

        #endregion

        #region Deep Clone Independence Tests

        [TestCategory("GameObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestGameDiscardCloneIndependence() {
            Game game = TestSetup.GenerateSimpleGame(2);
            Game clone = game.Clone() as Game;
            Player player = game.Players[0];
            player.DiscardAll();

            Assert.AreNotEqual(game, clone, "Game should not match its clone after player hand discard.");
        }

        [TestCategory("GameObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestGameDrawCloneIndependence() {
            Game game = TestSetup.GenerateSimpleGame(2);
            Game clone = game.Clone() as Game;
            Player player = game.Players[0];
            player.Draw();

            Assert.AreNotEqual(game, clone, "Game should not match its clone after player drawing a card.");
        }

        [TestCategory("GameObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestGameAdvanceTurnCloneIndependence() {
            Game game = TestSetup.GenerateSimpleGame(2);
            Game clone = game.Clone() as Game;
            game.AdvanceTurn();

            Assert.AreNotEqual(game, clone, "Game should not match its clone after advancing the turn.");
            Assert.AreNotEqual(
              game.CurrentPlayer, 
              clone.CurrentPlayer, 
              "Game's current player should not match that of its clone after advancing the turn."
            );
        }

        [TestCategory("GameObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestGameCardAcquisitionCloneIndependence() {
            Game game = TestSetup.GenerateSimpleGame(2);
            Game clone = game.Clone() as Game;
            Player player = game.Players[0];
            game.DealCard(TestSetup.CardTypeCopper, player, CardDestination.PLAY_AREA);

            Assert.AreNotEqual(game, clone, "Game should not match its clone after card acquisition.");
        }

        [TestCategory("GameObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestGameCardTrashingCloneIndependence() {
            Game game = TestSetup.GenerateSimpleGame(2);
            Game clone = game.Clone() as Game;
            Player player = game.Players[0];
            game.TrashCard(player.Hand[0]);

            Assert.AreNotEqual(game, clone, "Game should not match its clone after a card is trashed.");
        }

        #endregion

        #region Strategy Clone Independence Tests

        [TestCategory("GameObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestBuyingStrategyChangeCloneIndependence() {
            Game game = TestSetup.GenerateSimpleGame(2);
            game.CurrentStrategy.BuyingStrategy = new RandomBuyingStrategy();
            Game clone = game.Clone() as Game;
            List<CardType> gameOption = new List<CardType>();
            gameOption.Add(TestSetup.CardTypeCopper);
            game.CurrentStrategy.BuyingStrategy = new ScriptedBuyingStrategy(gameOption);

            Assert.AreNotEqual(game, clone, "Game should not match its clone after changing its buying strategy.");

            List<CardType> cloneOption = new List<CardType>();
            cloneOption.Add(TestSetup.CardTypeEstate);
            clone.CurrentStrategy.BuyingStrategy = new ScriptedBuyingStrategy(cloneOption);

            Assert.AreNotEqual(game, clone, "Game should not match its clone after changing its buying strategy.");
        }

        [TestCategory("GameObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestCardSelectionStrategyChangeCloneIndependence() {
            Game game = TestSetup.GenerateSimpleGame(2);
            game.CurrentStrategy.CardSelectionStrategy = new RandomCardSelectionStrategy();
            Game clone = game.Clone() as Game;
            game.CurrentStrategy.CardSelectionStrategy = new ScriptedCardSelectionStrategy(game.CurrentPlayer.Hand[0]);

            Assert.AreNotEqual(game, clone, "Game should not match its clone after changing its card selection strategy.");
 
            clone.CurrentStrategy.CardSelectionStrategy = new ScriptedCardSelectionStrategy(null);
            Assert.AreNotEqual(game, clone, "Game should not match its clone after changing its card selection strategy.");
        }

        [TestCategory("GameObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestTargetSelectionStrategyChangeCloneIndependence() {
            Game game = TestSetup.GenerateSimpleGame(2);
            game.CurrentStrategy.TargetSelectionStrategy = new RandomTargetSelectionStrategy();
            Game clone = game.Clone() as Game;
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, game.CurrentPlayer);

            Assert.AreNotEqual(game, clone, "Game should not match its clone after changing its target selection strategy.");

            clone.CurrentStrategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<Card>(), new List<Player>());
            Assert.AreNotEqual(game, clone, "Game should not match its clone after changing its target selection strategy.");
        }

        [TestCategory("GameObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestStrategyPhaseChangeCloneIndependence() {
            Game game = TestSetup.GenerateSimpleGame(2);
            game.CurrentPlayer.StartTurn();
            Game clone = game.Clone() as Game;

            Assert.AreEqual(game, clone, "Game should match its clone after each have started a turn.");

            game.PlayStep();
            Assert.AreNotEqual(game, clone, "Game should not match its clone after a phase has been played.");
        }

        [TestCategory("GameObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestStrategySelectedCardChangeCloneIndependence() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 2;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card firstVillage = null;
            foreach (Card card in game.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeVillage)) {
                    firstVillage = card;
                    break;
                }
            }

            game.CurrentStrategy.Phase = Game.Phase.TARGET;
            game.CurrentStrategy.SelectedCardId = firstVillage.Id;

            Game clone = game.Clone() as Game;

            Card secondVillage = null;
            foreach (Card card in game.CurrentPlayer.Hand) {
                if (!card.Equals(firstVillage) && card.Type.Equals(TestSetup.CardTypeVillage)) {
                    secondVillage = card;
                    break;
                }
            }

            game.CurrentStrategy.SelectedCardId = secondVillage.Id;

            Assert.AreNotEqual(game, clone, "Game should not match its clone after selected card is changed.");
        }

        [TestCategory("GameObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestStrategyDiscardingCardsChangedCloneIndependence() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card villageCard = null;
            foreach (Card card in game.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeVillage)) {
                    villageCard = card;
                    break;
                }
            }

            Game clone = game.Clone() as Game;

            game.CurrentStrategy.DiscardingStrategiesByPlayerId[game.CurrentPlayer.Id] = 
              new ScriptedDiscardingStrategy(new List<Card>() {villageCard});

            Assert.AreNotEqual(game, clone, "Game should not match its clone after cards to discard are changed.");
        }

        [TestCategory("GameObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestActionStackCloneIndependence() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card cellarCard = null;
            foreach (Card card in game.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeCellar)) {
                    cellarCard = card;
                    break;
                }
            }

            game.CurrentStrategy.CardSelectionStrategy = new ScriptedCardSelectionStrategy(cellarCard);
            game.PlayStep();

            // Play a null operation which should leave the clone in a state different only by action stack:
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(new List<Card>(), null);
            game.PlayStep();
            Game clone = game.Clone() as Game;

            game.PlayStep();

            Assert.AreNotEqual(
              game.CurrentStrategy, 
              clone.CurrentStrategy, 
              "Game strategy should not match that of its clone after action stack is changed."
            );
        }

        [TestCategory("GameObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestPreviousActionsCloneIndependence() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card villageCard = null;
            foreach (Card card in game.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeVillage)) {
                    villageCard = card;
                    break;
                }
            }

            game.CurrentStrategy.CardSelectionStrategy = new ScriptedCardSelectionStrategy(villageCard);
            game.PlayStep();

            Game clone = game.Clone() as Game;
            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, game.CurrentPlayer);
            clone.CurrentStrategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<Card>(), new List<Player>());

            game.PlayStep();
            clone.PlayStep();

            // Clear out the strategy difference so it doesn't trigger a failed comparison on its own:
            game.CurrentStrategy.TargetSelectionStrategy = new RandomTargetSelectionStrategy();
            clone.CurrentStrategy.TargetSelectionStrategy = new RandomTargetSelectionStrategy();

            Assert.AreNotEqual(
              game.CurrentStrategy, 
              clone.CurrentStrategy, 
              "Game strategy should not match that of its clone after previous action histories diverge."
            );
        }

        #endregion

        #endregion

        #endregion

        #endregion
    }
}