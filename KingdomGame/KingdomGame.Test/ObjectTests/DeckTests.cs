using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KingdomGame;
using System.Collections.Generic;

namespace KingdomGame.Test
{
    [TestClass]
    public class DecksTest {

        #region Test Setup

        [TestInitialize]
        public void DeckTestsSetup() {
            TestSetup.InitializeTypes();
        }

        [TestCleanup]
        public void DeckTestsCleanup() {
            RandomNumberManager.SetRandomNumberGenerator(new RandomNumberGenerator());
        }

        #endregion

        #region Tests

        #region Clone Tests

        #region Clone Equality Tests

        [TestCategory("DeckObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestDeckEquality()
        {
            Deck deck = TestSetup.GenerateSimpleDeck();
            Deck clone = deck.Clone() as Deck;

            Assert.AreEqual(deck, clone, "Deck does not match its clone.");
        }

        [TestCategory("DeckObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestDeckSearchEquality()
        {
            Deck deck = TestSetup.GenerateSimpleDeck();
            Deck clone = deck.Clone() as Deck;
            Card card = deck.Cards[0];

            Assert.AreEqual(
              0, 
              clone.IndexOf(card), 
              "Cards within a deck should match cards at corresponding indexes within its clone"
            );
        }

        #endregion

        #region Clone Independence Tests

        #region Deep Clone Independence Tests

        [TestCategory("DeckObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestDeckAddCloneIndependence()
        {
            Deck deck = TestSetup.GenerateSimpleDeck();
            Deck clone = deck.Clone() as Deck;
            Card card = new Card(TestSetup.CardTypeCopper, null);
            deck.Add(card);

            Assert.AreNotEqual(deck, clone, "Deck should not match its clone when a card is added.");
        }

        [TestCategory("DeckObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestDeckAddAtCloneIndependence()
        {
            Deck deck = TestSetup.GenerateSimpleDeck();
            Deck clone = deck.Clone() as Deck;
            Card card = new Card(TestSetup.CardTypeCopper, null);
            deck.AddAt(card, deck.Size);

            Assert.AreNotEqual(deck, clone, "Deck should not match its clone when a card is added.");
        }

        [TestCategory("DeckObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestDeckRemoveCloneIndependence()
        {
            Deck deck = TestSetup.GenerateSimpleDeck();
            Deck clone = deck.Clone() as Deck;
            Card card = deck.Peek();
            deck.Remove(card);

            Assert.AreNotEqual(deck, clone, "Deck should not match its clone when a card is removed.");
        }

        [TestCategory("DeckObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestDeckRemoveAtCloneIndependence()
        {
            Deck deck = TestSetup.GenerateSimpleDeck();
            Deck clone = deck.Clone() as Deck;
            deck.RemoveAt(deck.Size - 1);

            Assert.AreNotEqual(deck, clone, "Deck should not match its clone when a card is removed.");
        }

        [TestCategory("DeckObjectTest"), TestCategory("CloneTest"), TestMethod]
        public void TestDeckShuffleCloneIndependence()
        {
            // Guarantee that we will actually perform a shuffle by controlling randomness
            List<decimal> scriptedValues = new List<decimal>() { 0.0m, 1.0m };
            IRandomNumberGenerator scriptedGenerator = new ScriptedNumberGenerator(scriptedValues);

            Deck deck = TestSetup.GenerateSimpleDeck();
            Deck clone = deck.Clone() as Deck;
            deck.Shuffle();

            Assert.AreNotEqual(deck, clone, "Deck should not match its clone after shuffling.");
        }

        #endregion

        #endregion

        #endregion

        #endregion
    }
}
