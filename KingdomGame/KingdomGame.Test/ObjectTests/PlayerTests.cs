using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KingdomGame;
using System.Collections.Generic;

namespace KingdomGame.Test
{
    [TestClass]
    public class PlayerTests {

        #region Test Setup

        [TestInitialize]
        public void PlayerTestsSetup() {
            TestSetup.InitializeTypes();
        }

        #endregion

        #region Tests

        #region Clone Tests

        #region Clone Equality Tests

        [TestCategory("PlayerObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestPlayerEquality()
        {
            Game game = TestSetup.GenerateSimpleGame(2);
            Player player = game.Players[0];
            Player clone = player.Clone() as Player;

            Assert.AreEqual(player, clone, "Player does not match its clone.");
        }

        #endregion

        #region Clone Independence Tests

        #region Shallow Clone Independence Tests

        [TestCategory("PlayerObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestPlayerRemainingActionsCloneIndependence()
        {
            Game game = TestSetup.GenerateSimpleGame(2);
            Player player = game.Players[0];
            Player clone = player.Clone() as Player;
            player.RemainingActions++;

            Assert.AreNotEqual(player, clone, "Player should not match its clone after changing remaining actions.");
        }

        [TestCategory("PlayerObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestPlayerRemainingBuysCloneIndependence()
        {
            Game game = TestSetup.GenerateSimpleGame(2);
            Player player = game.Players[0];
            Player clone = player.Clone() as Player;
            player.RemainingBuys++;

            Assert.AreNotEqual(player, clone, "Player should not match its clone after changing remaining buys.");
        }

        [TestCategory("PlayerObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestPlayerRemainingMoneyCloneIndependence()
        {
            Game game = TestSetup.GenerateSimpleGame(2);
            Player player = game.Players[0];
            Player clone = player.Clone() as Player;
            player.RemainingMoney++;

            Assert.AreNotEqual(player, clone, "Player should not match its clone after changing remaining money.");
        }

        #endregion

        #region Deep Clone Independence Tests

        [TestCategory("PlayerObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestPlayerDiscardCloneIndependence()
        {
            Game game = TestSetup.GenerateSimpleGame(2);
            Player player = game.Players[0];
            Player clone = player.Clone() as Player;
            player.DiscardAll();

            Assert.AreNotEqual(player, clone, "Player should not match its clone after hand discard.");
        }

        [TestCategory("PlayerObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestPlayerDrawCloneIndependence()
        {
            Game game = TestSetup.GenerateSimpleGame(2);
            Player player = game.Players[0];
            Player clone = player.Clone() as Player;
            player.Draw();

            Assert.AreNotEqual(player, clone, "Player should not match its clone after drawing a card.");
        }

        [TestCategory("PlayerObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestPlayerEndTurnCloneIndependence()
        {
            Game game = TestSetup.GenerateSimpleGame(2);
            Player player = game.Players[0];
            Player clone = player.Clone() as Player;
            player.EndTurn();

            Assert.AreNotEqual(player, clone, "Player should not match its clone after ending a turn.");
        }

        [TestCategory("PlayerObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestPlayerCardAcquisitionCloneIndependence()
        {
            Game game = TestSetup.GenerateSimpleGame(2);
            Player player = game.Players[0];
            Player clone = player.Clone() as Player;
            game.DealCard(TestSetup.CardTypeCopper, player, CardDestination.PLAY_AREA);

            Assert.AreNotEqual(player, clone, "Player should not match its clone after card acquisition.");
        }

        [TestCategory("GameObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestPlayerCardTrashingCloneIndependence() {
            Game game = TestSetup.GenerateSimpleGame(2);
            Player player = game.Players[0];
            Player clone = player.Clone() as Player;
            game.TrashCard(player.Hand[0]);

            Assert.AreNotEqual(player, clone, "Player should not match its clone after a card is trashed.");
        }

        #endregion

        #endregion

        #endregion

        #endregion
    }
}
