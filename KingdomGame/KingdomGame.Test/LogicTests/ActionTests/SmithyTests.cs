using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame.Test
{
    [TestClass]
    public class SmithyTests {

        #region Test Setup

        [TestInitialize]
        public void SmithyTestsSetup() {
            TestSetup.InitializeTypes();
        }

        #endregion

        #region Tests


        [TestCategory("SmithyTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestStandardSmithyActionApplication() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeSmithy.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 5;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeSmithy.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeSmithy.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);
            Card smithyCard = TestUtilities.SetUpCardToPlay(game, TestSetup.CardTypeSmithy);
            game.PlayStep();
            game.CurrentStrategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<Card>(), new List<Player>() { game.CurrentPlayer });
            game.PlayStep();

            TestUtilities.ConfirmCardPlayed(game, smithyCard);
            Assert.AreEqual(
              7,
              game.CurrentPlayer.Hand.Count,
              "A hand with two extra cards should remain after a smithy is played."
            );
        }

        [TestCategory("SmithyTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestInsufficentCardsSmithyActionApplication() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeSmithy.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeSmithy.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeSmithy.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);
            Card smithyCard = TestUtilities.SetUpCardToPlay(game, TestSetup.CardTypeSmithy);
            game.PlayStep();
            game.CurrentStrategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<Card>(), new List<Player>() { game.CurrentPlayer });
            game.PlayStep();
            
            TestUtilities.ConfirmCardPlayed(game, smithyCard);
            Assert.AreEqual(
              6,
              game.CurrentPlayer.Hand.Count,
              "A hand with an extra card should remain after a smithy is played with only two cards left to draw."
            );
        }

        [TestCategory("SmithyTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestScriptedEmptySmithyActionTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeSmithy.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 5;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeSmithy.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeSmithy.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);
            Card smithyCard = TestUtilities.SetUpCardToPlay(game, TestSetup.CardTypeSmithy);
            game.PlayStep();
            game.CurrentStrategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(new List<Card>(), new List<Player>());
            game.PlayStep();
            
            TestUtilities.ConfirmCardPlayed(game, smithyCard);
            Assert.AreEqual(
              4,
              game.CurrentPlayer.Hand.Count,
              "A hand without a smithy should remain after a smithy is played with no target."
            );
        }

        [TestCategory("SmithyTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestInvalidSmithyTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeSmithy.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 5;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeSmithy.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeSmithy.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);
            Card smithyCard = TestUtilities.SetUpCardToPlay(game, TestSetup.CardTypeSmithy);
            game.PlayStep();

            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(
              null, 
              new List<Player>() {game.Players[1]}
            );
            game.PlayStep();
            
            TestUtilities.ConfirmCardPlayed(game, smithyCard);
            Assert.AreEqual(
              4,
              game.CurrentPlayer.Hand.Count,
              "A card should be missing after a smithy is played on an invalid target."
            );
            Assert.AreEqual(
              5,
              game.Players[1].Hand.Count,
              "The other player's hand should remain unchanged despite being illegally targeted by a smithy."
            );
        }

        [TestCategory("SmithyTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMultipleSmithyTargets() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeSmithy.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 5;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeSmithy.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeSmithy.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);
            Card smithyCard = TestUtilities.SetUpCardToPlay(game, TestSetup.CardTypeSmithy);
            game.PlayStep();

            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(
              null, 
              new List<Player>(game.Players)
            );
            game.PlayStep();
            
            TestUtilities.ConfirmCardPlayed(game, smithyCard);
            Assert.AreEqual(
              4,
              game.CurrentPlayer.Hand.Count,
              "A card should be missing after a smithy is played on a multiple target set."
            );
            Assert.AreEqual(
              5,
              game.Players[1].Hand.Count,
              "The other player's hand should remain unchanged despite being illegally targeted by a smithy."
            );
        }

        [TestCategory("SmithyTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestDuplicateSmithyTargets() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeSmithy.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 5;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            playerCardCountsByTypeId[TestSetup.CardTypeSmithy.Id] = 1;

            Dictionary<int, int> handCardCountsByTypeId = new Dictionary<int,int>();
            handCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 2;
            handCardCountsByTypeId[TestSetup.CardTypeSmithy.Id] = 1;

            Game game = TestSetup.GenerateStartingGame
                (2, gameCardCountsByTypeId, playerCardCountsByTypeId, handCardCountsByTypeId);
            Card smithyCard = TestUtilities.SetUpCardToPlay(game, TestSetup.CardTypeSmithy);
            game.PlayStep();

            game.CurrentStrategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(
              null,
              new List<Player>() { game.CurrentPlayer, game.CurrentPlayer}
            );
            game.PlayStep();
            
            TestUtilities.ConfirmCardPlayed(game, smithyCard);
            Assert.AreEqual(
              4,
              game.CurrentPlayer.Hand.Count,
              "A card should be missing after a smithy is played on a duplicate target set."
            );
        }

        #endregion

    }
}