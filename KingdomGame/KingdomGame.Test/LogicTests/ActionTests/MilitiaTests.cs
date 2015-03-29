using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame.Test
{
    [TestClass]
    public class MilitiaTests {

        #region Test Setup

        [TestInitialize]
        public void MilitiaTestsSetup() {
            TestSetup.InitializeTypes();
        }

        #endregion

        #region Tests

        [TestCategory("MilitiaActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestStandardMilitiaActionApplication() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card militiaCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMilitia)) {
                    militiaCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.PlaySelectionStrategy = 
              new ScriptedPlaySelectionStrategy(militiaCard);
            game.PlayStep();

            // Forcing the discard action:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.Players[1] });
            game.PlayStep();

            // Performing the monetary gain action:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.State.CurrentPlayer });
            game.PlayPhase();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a militia is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card remain after a militia is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a militia is played."
            );
            Assert.AreEqual(
              militiaCard, 
              game.State.CurrentPlayer.PlayArea[0], 
              "The play area should have a militia after it's been played."
            );
            Assert.AreEqual(
              2, 
              game.State.CurrentPlayer.RemainingMoney, 
              "The player should have gained two money as a result of playing a militia."
            );
            Assert.AreEqual(
              3,
              game.Players[1].Hand.Count,
              "The other player's hand should be missing two cards after a militia is played."
            );
            Assert.AreEqual(
              2,
              game.Players[1].Discard.Count,
              "The other player's discard pile should contain two cards after a militia is played."
            );
        }

        [TestCategory("MilitiaActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMilitiaActionWithAllTargetsDefended() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card militiaCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMilitia)) {
                    militiaCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.PlaySelectionStrategy = 
              new ScriptedPlaySelectionStrategy(militiaCard);
            game.PlayStep();

            // Forcing the discard action (cannot target anyone because the opponent has a moat):
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>());
            game.PlayStep();

            // Performing the monetary gain action:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.State.CurrentPlayer });
            game.PlayPhase();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a militia is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card remain after a militia is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a militia is played."
            );
            Assert.AreEqual(
              militiaCard, 
              game.State.CurrentPlayer.PlayArea[0], 
              "The play area should have a militia after it's been played."
            );
            Assert.AreEqual(
              2, 
              game.State.CurrentPlayer.RemainingMoney, 
              "The player should have gained two money as a result of playing a militia."
            );
            Assert.AreEqual(
              5,
              game.Players[1].Hand.Count,
              "The other player's hand should not be missing two cards after a militia is played if it has a moat."
            );
            Assert.AreEqual(
              0,
              game.Players[1].Discard.Count,
              "The other player's discard pile should contain no cards after a militia is played if the hand has a moat."
            );
        }

        [TestCategory("MilitiaActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMilitiaWithSubsetOfOpponentsDefended() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 16;
            gameCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(4, gameCardCountsByTypeId, playerCardCountsByTypeId);
            game.TrashCard(game.Players[1].Hand[0]);
            game.DealCard(TestSetup.CardTypeMoat, game.Players[1], CardDestination.HAND);

            Card militiaCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMilitia)) {
                    militiaCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.PlaySelectionStrategy = 
              new ScriptedPlaySelectionStrategy(militiaCard);
            game.PlayStep();

            // Forcing the discard action (should need to specifiy both players without a moat):
            IList<Player> playersToDiscard = new List<Player>() { game.Players[2], game.Players[3] };
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, playersToDiscard);
            game.PlayStep();

            // Performing the monetary gain action:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.State.CurrentPlayer });
            game.PlayPhase();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a militia is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card remain after a militia is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a militia is played."
            );
            Assert.AreEqual(
              militiaCard, 
              game.State.CurrentPlayer.PlayArea[0], 
              "The play area should have a militia after it's been played."
            );
            Assert.AreEqual(
              2, 
              game.State.CurrentPlayer.RemainingMoney, 
              "The player should have gained two money as a result of playing a militia."
            );
            foreach (Player player in game.Players) {
                if (player.Equals(game.State.CurrentPlayer)) {
                    continue;
                }

                if (playersToDiscard.Contains(player)) {
                    Assert.AreEqual(
                      3,
                      player.Hand.Count,
                      "The opponent's hand should be missing two cards after a militia is played if it doesn't have a moat."
                    );
                    Assert.AreEqual(
                      2,
                      player.Discard.Count,
                      "The other opponent's discard pile should contain two cards after a militia is played if the hand doesn't have a moat."
                    );
                }
                else {
                    Assert.AreEqual(
                      5,
                      player.Hand.Count,
                      "The opponent's hand should not be missing two cards after a militia is played if it has a moat."
                    );
                    Assert.AreEqual(
                      0,
                      player.Discard.Count,
                      "The other opponent's discard pile should contain no cards after a militia is played if the hand has a moat."
                    );
                }
            }
        }

        [TestCategory("MilitiaActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMilitiaWithTargetHavingOnlyOneCardToDiscard() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 16;
            gameCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(4, gameCardCountsByTypeId, playerCardCountsByTypeId);
            game.TrashCard(game.Players[1].Hand[0]);
            game.DealCard(TestSetup.CardTypeMoat, game.Players[1], CardDestination.HAND);

            Player partialHandPlayer = game.Players[2];
            game.TrashCard(partialHandPlayer.Hand[0]);

            Card militiaCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMilitia)) {
                    militiaCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.PlaySelectionStrategy = 
              new ScriptedPlaySelectionStrategy(militiaCard);
            game.PlayStep();

            // Forcing the discard action (should need to specifiy both players without a moat):
            IList<Player> playersToDiscard = new List<Player>() { game.Players[2], game.Players[3] };
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, playersToDiscard);
            game.PlayStep();

            // Performing the monetary gain action:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.State.CurrentPlayer });
            game.PlayPhase();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a militia is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card remain after a militia is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a militia is played."
            );
            Assert.AreEqual(
              militiaCard, 
              game.State.CurrentPlayer.PlayArea[0], 
              "The play area should have a militia after it's been played."
            );
            Assert.AreEqual(
              2, 
              game.State.CurrentPlayer.RemainingMoney, 
              "The player should have gained two money as a result of playing a militia."
            );
            foreach (Player player in game.Players) {
                if (player.Equals(game.State.CurrentPlayer)) {
                    continue;
                }

                if (player.Equals(partialHandPlayer)) {
                    Assert.AreEqual(
                      3,
                      player.Hand.Count,
                      "The opponent's hand should be missing two cards after a militia is played if it doesn't have a moat."
                    );
                    Assert.AreEqual(
                      1,
                      player.Discard.Count,
                      "The opponent's discard pile should contain one cards after a militia is played if the hand doesn't have a moat when there were only four cards to start."
                    );
                } else if (playersToDiscard.Contains(player)) {
                    Assert.AreEqual(
                      3,
                      player.Hand.Count,
                      "The opponent's hand should be missing two cards after a militia is played if it doesn't have a moat."
                    );
                    Assert.AreEqual(
                      2,
                      player.Discard.Count,
                      "The other opponent's discard pile should contain two cards after a militia is played if the hand doesn't have a moat."
                    );
                } else {
                    Assert.AreEqual(
                      5,
                      player.Hand.Count,
                      "The opponent's hand should not be missing two cards after a militia is played if it has a moat."
                    );
                    Assert.AreEqual(
                      0,
                      player.Discard.Count,
                      "The opponent's discard pile should contain no cards after a militia is played if the hand has a moat."
                    );
                }
            }
        }

        [TestCategory("MilitiaActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMilitiaWithTargetHavingInsufficientCardsToDiscard() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 16;
            gameCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(4, gameCardCountsByTypeId, playerCardCountsByTypeId);
            game.TrashCard(game.Players[1].Hand[0]);
            game.DealCard(TestSetup.CardTypeMoat, game.Players[1], CardDestination.HAND);

            Player partialHandPlayer = game.Players[2];
            game.TrashCard(partialHandPlayer.Hand[0]);
            game.TrashCard(partialHandPlayer.Hand[0]);
            game.TrashCard(partialHandPlayer.Hand[0]);

            Card militiaCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMilitia)) {
                    militiaCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.PlaySelectionStrategy = 
              new ScriptedPlaySelectionStrategy(militiaCard);
            game.PlayStep();

            // Forcing the discard action (should need to specifiy both players without a moat):
            IList<Player> playersToDiscard = new List<Player>() { game.Players[2], game.Players[3] };
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, playersToDiscard);
            game.PlayStep();

            // Performing the monetary gain action:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.State.CurrentPlayer });
            game.PlayPhase();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a militia is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card remain after a militia is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a militia is played."
            );
            Assert.AreEqual(
              militiaCard, 
              game.State.CurrentPlayer.PlayArea[0], 
              "The play area should have a militia after it's been played."
            );
            Assert.AreEqual(
              2, 
              game.State.CurrentPlayer.RemainingMoney, 
              "The player should have gained two money as a result of playing a militia."
            );
            foreach (Player player in game.Players) {
                if (player.Equals(game.State.CurrentPlayer)) {
                    continue;
                }

                if (player.Equals(partialHandPlayer)) {
                    Assert.AreEqual(
                      2,
                      player.Hand.Count,
                      "The opponent's hand should have only two cards after a militia if that was the hand size to start."
                    );
                    Assert.AreEqual(
                      0,
                      player.Discard.Count,
                      "The opponent's discard pile should contain no cards after a militia is played if the hand wasn't large enough to discard."
                    );
                } else if (playersToDiscard.Contains(player)) {
                    Assert.AreEqual(
                      3,
                      player.Hand.Count,
                      "The opponent's hand should be missing two cards after a militia is played if it doesn't have a moat."
                    );
                    Assert.AreEqual(
                      2,
                      player.Discard.Count,
                      "The other opponent's discard pile should contain two cards after a militia is played if the hand doesn't have a moat."
                    );
                } else {
                    Assert.AreEqual(
                      5,
                      player.Hand.Count,
                      "The opponent's hand should not be missing two cards after a militia is played if it has a moat."
                    );
                    Assert.AreEqual(
                      0,
                      player.Discard.Count,
                      "The opponent's discard pile should contain no cards after a militia is played if the hand has a moat."
                    );
                }
            }
        }

        [TestCategory("MilitiaActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMilitiaWithCurrentPlayerInDiscardTargetSet() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 16;
            gameCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(4, gameCardCountsByTypeId, playerCardCountsByTypeId);
            game.TrashCard(game.Players[1].Hand[0]);
            game.DealCard(TestSetup.CardTypeMoat, game.Players[1], CardDestination.HAND);

            Card militiaCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMilitia)) {
                    militiaCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.PlaySelectionStrategy = 
              new ScriptedPlaySelectionStrategy(militiaCard);
            game.PlayStep();

            // Forcing the discard action (specifying the current player, which should fail):
            IList<Player> playersToDiscard = new List<Player>() { game.State.CurrentPlayer, game.Players[2], game.Players[3] };
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, playersToDiscard);
            game.PlayStep();

            // Performing the monetary gain action:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.State.CurrentPlayer });
            game.PlayPhase();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a militia is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card remain after a militia is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a militia is played."
            );
            Assert.AreEqual(
              militiaCard, 
              game.State.CurrentPlayer.PlayArea[0], 
              "The play area should have a militia after it's been played."
            );
            Assert.AreEqual(
              2, 
              game.State.CurrentPlayer.RemainingMoney, 
              "The player should have gained two money as a result of playing a militia."
            );
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.Discard.Count, 
              "The player should not have discarded any cards as the result of playing a militia."
            );
            foreach (Player player in game.Players) {
                if (player.Equals(game.State.CurrentPlayer)) {
                    continue;
                }

                Assert.AreEqual(
                  5,
                  player.Hand.Count,
                  "The opponent's hand should not be missing any cards after a militia is played if the discard target set was invalid."
                );
                Assert.AreEqual(
                  0,
                  player.Discard.Count,
                  "The other opponent's discard pile should contain two cards after a militia is played if the discard target set was invalid."
                );
            }
        }

        [TestCategory("MilitiaActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMilitiaWithDefendedPlayerInDiscardTargetSet() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 16;
            gameCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(4, gameCardCountsByTypeId, playerCardCountsByTypeId);
            game.TrashCard(game.Players[1].Hand[0]);
            game.DealCard(TestSetup.CardTypeMoat, game.Players[1], CardDestination.HAND);

            Card militiaCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMilitia)) {
                    militiaCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.PlaySelectionStrategy = 
              new ScriptedPlaySelectionStrategy(militiaCard);
            game.PlayStep();

            // Forcing the discard action (specifying a player which has the moat, which should fail):
            IList<Player> playersToDiscard = new List<Player>() { game.Players[1], game.Players[2], game.Players[3] };
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, playersToDiscard);
            game.PlayStep();

            // Performing the monetary gain action:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.State.CurrentPlayer });
            game.PlayPhase();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a militia is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card remain after a militia is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a militia is played."
            );
            Assert.AreEqual(
              militiaCard, 
              game.State.CurrentPlayer.PlayArea[0], 
              "The play area should have a militia after it's been played."
            );
            Assert.AreEqual(
              2, 
              game.State.CurrentPlayer.RemainingMoney, 
              "The player should have gained two money as a result of playing a militia."
            );
            foreach (Player player in game.Players) {
                if (player.Equals(game.State.CurrentPlayer)) {
                    continue;
                }

                Assert.AreEqual(
                  5,
                  player.Hand.Count,
                  "The opponent's hand should not be missing any cards after a militia is played if the discard target set was invalid."
                );
                Assert.AreEqual(
                  0,
                  player.Discard.Count,
                  "The other opponent's discard pile should contain two cards after a militia is played if the discard target set was invalid."
                );
            }
        }

        [TestCategory("MilitiaActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMilitiaWithDuplicatePlayerInDiscardTargetSet() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 16;
            gameCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(4, gameCardCountsByTypeId, playerCardCountsByTypeId);
            game.TrashCard(game.Players[1].Hand[0]);
            game.DealCard(TestSetup.CardTypeMoat, game.Players[1], CardDestination.HAND);

            Card militiaCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMilitia)) {
                    militiaCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.PlaySelectionStrategy = 
              new ScriptedPlaySelectionStrategy(militiaCard);
            game.PlayStep();

            // Forcing the discard action (specifying a duplicate player, which should fail):
            IList<Player> playersToDiscard = new List<Player>() { game.Players[2], game.Players[2], game.Players[3] };
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, playersToDiscard);
            game.PlayStep();

            // Performing the monetary gain action:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.State.CurrentPlayer });
            game.PlayPhase();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a militia is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card remain after a militia is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a militia is played."
            );
            Assert.AreEqual(
              militiaCard, 
              game.State.CurrentPlayer.PlayArea[0], 
              "The play area should have a militia after it's been played."
            );
            Assert.AreEqual(
              2, 
              game.State.CurrentPlayer.RemainingMoney, 
              "The player should have gained two money as a result of playing a militia."
            );
            foreach (Player player in game.Players) {
                if (player.Equals(game.State.CurrentPlayer)) {
                    continue;
                }

                Assert.AreEqual(
                  5,
                  player.Hand.Count,
                  "The opponent's hand should not be missing any cards after a militia is played if the discard target set was invalid."
                );
                Assert.AreEqual(
                  0,
                  player.Discard.Count,
                  "The other opponent's discard pile should contain two cards after a militia is played if the discard target set was invalid."
                );
            }
        }

        [TestCategory("MilitiaActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMilitiaWithMissingPlayerFromDiscardTargetSet() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 16;
            gameCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(4, gameCardCountsByTypeId, playerCardCountsByTypeId);
            game.TrashCard(game.Players[1].Hand[0]);
            game.DealCard(TestSetup.CardTypeMoat, game.Players[1], CardDestination.HAND);

            Card militiaCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMilitia)) {
                    militiaCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.PlaySelectionStrategy = 
              new ScriptedPlaySelectionStrategy(militiaCard);
            game.PlayStep();

            // Forcing the discard action (not specifying an opponent without a moat, which should fail):
            IList<Player> playersToDiscard = new List<Player>() { game.Players[3] };
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, playersToDiscard);
            game.PlayStep();

            // Performing the monetary gain action:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.State.CurrentPlayer });
            game.PlayPhase();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a militia is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card remain after a militia is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a militia is played."
            );
            Assert.AreEqual(
              militiaCard, 
              game.State.CurrentPlayer.PlayArea[0], 
              "The play area should have a militia after it's been played."
            );
            Assert.AreEqual(
              2, 
              game.State.CurrentPlayer.RemainingMoney, 
              "The player should have gained two money as a result of playing a militia."
            );
            foreach (Player player in game.Players) {
                if (player.Equals(game.State.CurrentPlayer)) {
                    continue;
                }

                Assert.AreEqual(
                  5,
                  player.Hand.Count,
                  "The opponent's hand should not be missing any cards after a militia is played if the discard target set was invalid."
                );
                Assert.AreEqual(
                  0,
                  player.Discard.Count,
                  "The other opponent's discard pile should contain two cards after a militia is played if the discard target set was invalid."
                );
            }
        }

        [TestCategory("MilitiaActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMilitiaExistingMoneyPreserved() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            playerCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card militiaCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMilitia)) {
                    militiaCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.PlaySelectionStrategy = 
              new ScriptedPlaySelectionStrategy(militiaCard);
            game.PlayStep();

            IList<ITargetable> cardsToDiscard = new List<ITargetable>();
            foreach (Card card in game.Players[1].Hand) {
                if (!card.Type.Equals(TestSetup.CardTypeCopper)) {
                    cardsToDiscard.Add(card);
                }
            }

            // Ensure the other player doesn't discard any copper:
            game.Players[1].Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(cardsToDiscard);

            // Forcing the discard action:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.Players[1] });
            game.PlayStep();

            // Performing the monetary gain action:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.State.CurrentPlayer });
            game.PlayPhase();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a militia is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card remain after a militia is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a militia is played."
            );
            Assert.AreEqual(
              militiaCard, 
              game.State.CurrentPlayer.PlayArea[0], 
              "The play area should have a militia after it's been played."
            );
            Assert.AreEqual(
              5, 
              game.State.CurrentPlayer.RemainingMoney, 
              "The player should have gained two additional money as a result of playing a militia."
            );
            Assert.AreEqual(
              3,
              game.Players[1].Hand.Count,
              "The other player's hand should be missing two cards after a militia is played."
            );
            Assert.AreEqual(
              2,
              game.Players[1].Discard.Count,
              "The other player's discard pile should contain two cards after a militia is played."
            );
            Assert.AreEqual(
              3, 
              game.Players[1].RemainingMoney, 
              "The other player should gain no money as the result of a militia being played."
            );
        }

        [TestCategory("MilitiaActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMilitiaInvalidMoneyGainingTarget() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card militiaCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMilitia)) {
                    militiaCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.PlaySelectionStrategy = 
              new ScriptedPlaySelectionStrategy(militiaCard);
            game.PlayStep();

            // Forcing the discard action:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.Players[1] });
            game.PlayStep();

            // Performing the monetary gain action on the wrong player:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.Players[1] });
            game.PlayPhase();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a militia is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card remain after a militia is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a militia is played."
            );
            Assert.AreEqual(
              militiaCard, 
              game.State.CurrentPlayer.PlayArea[0], 
              "The play area should have a militia after it's been played."
            );
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingMoney, 
              "The player should have gained no money if the militia's gain action is performed on an invalid target."
            );
            Assert.AreEqual(
              3,
              game.Players[1].Hand.Count,
              "The other player's hand should be missing two cards after a militia is played."
            );
            Assert.AreEqual(
              2,
              game.Players[1].Discard.Count,
              "The other player's discard pile should contain two cards after a militia is played."
            );
            Assert.AreEqual(
              0, 
              game.Players[1].RemainingMoney, 
              "The other player should gain no money if the militia's money gaining target set is invalid."
            );
        }

        [TestCategory("MilitiaActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMilitiaMultipleMoneyGainingTargetSet() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card militiaCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMilitia)) {
                    militiaCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.PlaySelectionStrategy = 
              new ScriptedPlaySelectionStrategy(militiaCard);
            game.PlayStep();

            // Forcing the discard action:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.Players[1] });
            game.PlayStep();
            game.PlayStep();

            // Performing the monetary gain action on multiple players:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>(game.Players));
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a militia is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card remain after a militia is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a militia is played."
            );
            Assert.AreEqual(
              militiaCard, 
              game.State.CurrentPlayer.PlayArea[0], 
              "The play area should have a militia after it's been played."
            );
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingMoney, 
              "The player should have gained no money if the militia's gain action is performed on an invalid target set."
            );
            Assert.AreEqual(
              3,
              game.Players[1].Hand.Count,
              "The other player's hand should be missing two cards after a militia is played."
            );
            Assert.AreEqual(
              2,
              game.Players[1].Discard.Count,
              "The other player's discard pile should contain two cards after a militia is played."
            );
            Assert.AreEqual(
              0, 
              game.Players[1].RemainingMoney, 
              "The other player should gain no money if the militia's money gaining target set is invalid."
            );
        }

        [TestCategory("MilitiaActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMilitiaDuplicateMoneyGainingTargetSet() {
            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 4;
            playerCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 1;

            Game game = TestSetup.GenerateStartingGame(2, gameCardCountsByTypeId, playerCardCountsByTypeId);

            Card militiaCard = null;
            foreach(Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(TestSetup.CardTypeMilitia)) {
                    militiaCard = card;
                    break;
                }
            }

            game.State.CurrentPlayer.Strategy.PlaySelectionStrategy = 
              new ScriptedPlaySelectionStrategy(militiaCard);
            game.PlayStep();

            // Forcing the discard action:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.Players[1] });
            game.PlayStep();
            game.PlayStep();

            // Performing the monetary gain action on duplicate players:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = 
              new ScriptedTargetSelectionStrategy(null, new List<Player>() { game.State.CurrentPlayer, game.State.CurrentPlayer });
            game.PlayStep();
            
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingActions, 
              "No actions should remain after a militia is played."
            );
            Assert.AreEqual(
              4,
              game.State.CurrentPlayer.Hand.Count,
              "The hand should be missing a card remain after a militia is played."
            );
            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              "The play area should have one card after a militia is played."
            );
            Assert.AreEqual(
              militiaCard, 
              game.State.CurrentPlayer.PlayArea[0], 
              "The play area should have a militia after it's been played."
            );
            Assert.AreEqual(
              0, 
              game.State.CurrentPlayer.RemainingMoney, 
              "The player should have gained no money if the militia's gain action is performed on an invalid target set."
            );
            Assert.AreEqual(
              3,
              game.Players[1].Hand.Count,
              "The other player's hand should be missing two cards after a militia is played."
            );
            Assert.AreEqual(
              2,
              game.Players[1].Discard.Count,
              "The other player's discard pile should contain two cards after a militia is played."
            );
            Assert.AreEqual(
              0, 
              game.Players[1].RemainingMoney, 
              "The other player should gain no money if the militia's money gaining target set is invalid."
            );
        }

        #endregion
    }
}