using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame.Test
{
    [TestClass]
    public class CardSelectionTests {

        #region Test Setup

        [TestInitialize]
        public void CardSelectionTestsSetup() {
            TestSetup.InitializeTypes();
        }

        #endregion

        #region Tests

        [TestCategory("CardSelectionTest"), TestMethod]
        public void TestNoOptionCardSelection() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            TestUtilities.ForceGamePhase(game, Game.Phase.PLAY);
            game.PlayStep();

            Assert.AreEqual(
              null, 
              game.State.SelectedCard, 
              "No card should be selected if no action is eligible for selection."
            );
        }

        [TestCategory("CardSelectionTest"), TestMethod]
        public void TestEmptyScriptedCardSelection() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);
            TestUtilities.ForceGamePhase(game, Game.Phase.PLAY);
            game.CurrentStrategy.CardSelectionStrategy = new ScriptedCardSelectionStrategy(null);
            game.PlayStep();

            Assert.IsNull(
              game.State.SelectedCard, 
              "No card should be selected if the scripted choice was to play nothing."
            );
        }

        [TestCategory("CardSelectionTest"), TestMethod]
        public void TestNonActionScriptedCardSelection() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card estateCard = null;
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeEstate)) {
                    estateCard = card;
                    break;
                }
            }

            TestUtilities.ForceGamePhase(game, Game.Phase.PLAY);
            game.CurrentStrategy.CardSelectionStrategy = new ScriptedCardSelectionStrategy(estateCard);
            game.PlayStep();

            Assert.IsNull(
              game.State.SelectedCard, 
              "No card should be selected if an invalid scripted choice was selected."
            );
        }

        [TestCategory("CardSelectionTest"), TestMethod]
        public void TestSingleOptionCardSelection() {
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

            TestUtilities.ForceGamePhase(game, Game.Phase.PLAY);
            game.CurrentStrategy.CardSelectionStrategy = new ScriptedCardSelectionStrategy(villageCard);
            game.PlayStep();

            Assert.AreEqual(
              villageCard, 
              game.State.SelectedCard, 
              "The village card should be selected if it is the scripted choice."
            );
        }

        [TestCategory("CardSelectionTest"), TestMethod]
        public void TestMultipleOptionScriptedCardSelection() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeWorkshop.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeWorkshop.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card villageCard = null;
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeVillage)) {
                    villageCard = card;
                    break;
                }
            }

            TestUtilities.ForceGamePhase(game, Game.Phase.PLAY);
            game.CurrentStrategy.CardSelectionStrategy = new ScriptedCardSelectionStrategy(villageCard);
            game.PlayStep();

            Assert.AreEqual(
              villageCard, 
              game.State.SelectedCard, 
              "The village card should be selected if it is the scripted choice."
            );
        }

        #endregion
    }
}
