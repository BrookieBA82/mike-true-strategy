using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KingdomGame;
using System.Collections.Generic;

namespace KingdomGame.Test
{
    [TestClass]
    public class CardTests {

        #region Test Setup

        [TestInitialize]
        public void CardTestsSetup() {
            TestSetup.InitializeTypes();
        }

        #endregion

        #region Tests

        #region Clone Tests

        #region Clone Equality Tests

        [TestCategory("CardObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestCardEquality()
        {
            Player player = new Player("Player");
            Card card = new Card(TestSetup.CardTypeCopper, player);
            Card clone = card.Clone() as Card;

            Assert.AreEqual(card, clone, "Card does not match its clone.");
            Assert.AreEqual(card.OwnerId, clone.OwnerId, "Card does not match on owner its clone.");
        }

        #endregion

        #region Clone Independence Tests

        #region Shallow Clone Independence Tests

        [TestCategory("CardObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestCardOwnerChangeCloneIndependence()
        {
            Player playerOne = new Player("Player 1");
            Player playerTwo = new Player("Player 2");
            Card card = new Card(TestSetup.CardTypeCopper, playerOne);
            Card clone = card.Clone() as Card;
            card.OwnerId = playerTwo.Id;

            Assert.AreEqual(card, clone, "Card should match its clone even after its owner is changed.");
            Assert.AreNotEqual(
              card.OwnerId, 
              clone.OwnerId, 
              "Card should not match its clone own owner after change."
            );
        }

        #endregion

        #endregion

        #endregion

        #endregion
    }
}
