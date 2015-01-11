using System;
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
            game.PlayStep();

            Assert.AreEqual(false, game.IsFinished, "The game should not be over because no piles are empty.");
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
            game.PlayStep();

            Assert.AreEqual(false, game.IsFinished, "The game should not be over because one non-critical pile is empty.");
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
            game.PlayStep();

            Assert.AreEqual(true, game.IsFinished, "The game should be over because a critical pile is empty.");
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
            game.PlayStep();

            Assert.AreEqual(true, game.IsFinished, "The game should be over because enough non-critical piles are empty.");
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
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeVillage)) {
                    village = card;
                    break;
                }
            }

            game.State.SelectedCard = village;
            TestUtilities.ForceGamePhase(game, Game.Phase.ACTION);
            game.State.AddPendingAction(village.Type.Actions[0].Create(game.State.CurrentPlayer));
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, game.State.CurrentPlayer);
            game.PlayStep();

            Game clone = game.Clone() as Game;

            Assert.AreEqual(
              game.State, 
              clone.State, 
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
            game.PlayTurn();

            Assert.AreNotEqual(
              game.State.CurrentPlayer, 
              clone.State.CurrentPlayer, 
              "Game should not match its clone on current player after playing a turn."
            );
        }

        [TestCategory("GameObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestGameTurnNumberCloneIndependence() {
            Game game = TestSetup.GenerateSimpleGame(2);
            Game clone = game.Clone() as Game;
            game.PlayTurn();

            Assert.AreNotEqual(game, clone, "Game should not match its clone afterplaying a  turn.");
            Assert.AreNotEqual(
              game.State.TurnNumber, 
              clone.State.TurnNumber, 
              "Game's turn number should not match that of its clone after playing a  turn."
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
            game.PlayTurn();

            Assert.AreNotEqual(game, clone, "Game should not match its clone after playing a turn.");
            Assert.AreNotEqual(
              game.State.CurrentPlayer, 
              clone.State.CurrentPlayer, 
              "Game's current player should not match that of its clone after playing a turn."
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
            game.State.CurrentPlayer.Strategy.BuyingStrategy = new RandomBuyingStrategy();
            Game clone = game.Clone() as Game;
            List<CardType> gameOption = new List<CardType>();
            gameOption.Add(TestSetup.CardTypeCopper);
            game.State.CurrentPlayer.Strategy.BuyingStrategy = new ScriptedBuyingStrategy(gameOption);

            Assert.AreNotEqual(game, clone, "Game should not match its clone after changing its buying strategy.");

            List<CardType> cloneOption = new List<CardType>();
            cloneOption.Add(TestSetup.CardTypeEstate);
            clone.State.CurrentPlayer.Strategy.BuyingStrategy = new ScriptedBuyingStrategy(cloneOption);

            Assert.AreNotEqual(game, clone, "Game should not match its clone after changing its buying strategy.");
        }

        [TestCategory("GameObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestCardSelectionStrategyChangeCloneIndependence() {
            Game game = TestSetup.GenerateSimpleGame(2);
            game.State.CurrentPlayer.Strategy.CardSelectionStrategy = new RandomCardSelectionStrategy();
            Game clone = game.Clone() as Game;
            game.State.CurrentPlayer.Strategy.CardSelectionStrategy = new ScriptedCardSelectionStrategy(game.State.CurrentPlayer.Hand[0]);

            Assert.AreNotEqual(game, clone, "Game should not match its clone after changing its card selection strategy.");
 
            clone.State.CurrentPlayer.Strategy.CardSelectionStrategy = new ScriptedCardSelectionStrategy(null);
            Assert.AreNotEqual(game, clone, "Game should not match its clone after changing its card selection strategy.");
        }

        [TestCategory("GameObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestTargetSelectionStrategyChangeCloneIndependence() {
            Game game = TestSetup.GenerateSimpleGame(2);
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new RandomTargetSelectionStrategy();
            Game clone = game.Clone() as Game;
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, game.State.CurrentPlayer);

            Assert.AreNotEqual(game, clone, "Game should not match its clone after changing its target selection strategy.");

            clone.State.CurrentPlayer.Strategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<Card>(), new List<Player>());
            Assert.AreNotEqual(game, clone, "Game should not match its clone after changing its target selection strategy.");
        }

        [TestCategory("GameObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestStrategyPhaseChangeCloneIndependence() {
            Game game = TestSetup.GenerateSimpleGame(2);
            game.State.CurrentPlayer.StartTurn();
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
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeVillage)) {
                    firstVillage = card;
                    break;
                }
            }

            game.State.SelectedCard = firstVillage;
            Game clone = game.Clone() as Game;

            Card secondVillage = null;
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (!card.Equals(firstVillage) && card.Type.Equals(TestSetup.CardTypeVillage)) {
                    secondVillage = card;
                    break;
                }
            }

            game.State.SelectedCard = secondVillage;

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
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeVillage)) {
                    villageCard = card;
                    break;
                }
            }

            Game clone = game.Clone() as Game;

            game.State.CurrentPlayer.Strategy.DiscardingStrategy = new ScriptedDiscardingStrategy(new List<Card>() {villageCard});

            Assert.AreNotEqual(game, clone, "Game should not match its clone after cards to discard are changed.");
        }

        #endregion

        #region State Clone Independence Tests

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
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeCellar)) {
                    cellarCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.CardSelectionStrategy = new ScriptedCardSelectionStrategy(cellarCard);
            game.PlayStep();

            // Play a null operation which should leave the clone in a state different only by action stack:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(new List<Card>(), null);
            game.PlayStep();
            Game clone = game.Clone() as Game;

            game.PlayStep();

            Assert.AreNotEqual(
              game.State, 
              clone.State, 
              "Game state should not match that of its clone after action stack is changed."
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
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeVillage)) {
                    villageCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.CardSelectionStrategy = new ScriptedCardSelectionStrategy(villageCard);
            game.PlayStep();

            Game clone = game.Clone() as Game;
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, game.State.CurrentPlayer);
            clone.State.CurrentPlayer.Strategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<Card>(), new List<Player>());

            game.PlayStep();
            clone.PlayStep();

            // The original game will be in PLAY as the player has an extra action, while the clone will be in BUY.
            Assert.AreNotEqual(
              game.State, 
              clone.State, 
              "Game state should not match that of its clone after previous action histories diverge."
            );
        }

        #endregion

        #endregion

        #endregion

        #endregion
    }
}
