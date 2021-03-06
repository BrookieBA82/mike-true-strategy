﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame.Test
{
    [TestClass]
    public class CellarTests {

        #region Test Setup

        [TestInitialize]
        public void CellarTestsSetup() {
            TestSetup.InitializeTypes();
        }

        #endregion

        #region Base Specifications

        private static readonly string ASSERTION_KEY_CARD_PLAYED = "LibraryPlayed";
        private static readonly string ASSERTION_KEY_HAND_SIZE = "HandSize";
        private static readonly string ASSERTION_KEY_DISCARD_SIZE = "DiscardSize";
        private static readonly string ASSERTION_KEY_DECK_SIZE = "DeckSize";
        private static readonly string ASSERTION_KEY_HAND_MINE_COUNT = "HandMineCount";
        private static readonly string ASSERTION_KEY_HAND_ESTATE_COUNT = "HandEstateCount";
        private static readonly string ASSERTION_KEY_DECK_ESTATE_COUNT = "DeckEstateCount";
        private static readonly string ASSERTION_KEY_DISCARD_ESTATE_COUNT = "DiscardEstateCount";

        private static ActionTestSpecification GetBaseSpecification(int expectedHandSize = 4, int expectedDiscardSize = 1) {
            ActionTestSpecification baseSpec = new ActionTestSpecification();

            baseSpec.GameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 30;
            baseSpec.GameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 12;
            baseSpec.GameCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 10;
            baseSpec.GameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;

            baseSpec.PlayerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            baseSpec.PlayerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            baseSpec.PlayerCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;
            baseSpec.PlayerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 1;

            baseSpec.HandCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 3;
            baseSpec.HandCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 1;
            baseSpec.HandCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;

            baseSpec.Play = TestSetup.CardTypeCellar;
            baseSpec.ActionDescription = "after a cellar is played";

            // Todo - (MT): Find way to pass in custom assert description.
            ITestAssertion playedAssertion = new CardPlayedAssertion(
              CellarTests.ASSERTION_KEY_CARD_PLAYED,
              game => TestUtilities.FindCard(game.State.CurrentPlayer.Hand, baseSpec.Play),
              expectedActionsRemaining: 1
            );

            ITestAssertion handSizeAssertion = new CardCountAssertion(
              CellarTests.ASSERTION_KEY_HAND_SIZE,
              game => game.State.CurrentPlayer.Hand,
              TestUtilities.CARD_LOCATION_HAND,
              expectedHandSize
            );

            ITestAssertion discardSizeAssertion = new CardCountAssertion(
              CellarTests.ASSERTION_KEY_DISCARD_SIZE,
              game => game.State.CurrentPlayer.Discard,
              TestUtilities.CARD_LOCATION_DISCARD,
              expectedDiscardSize
            );

            ITestAssertion handMineCountAssertion = new CardTypeCountAssertion(
                CellarTests.ASSERTION_KEY_HAND_MINE_COUNT,
                game => game.State.CurrentPlayer.Hand, 
                TestUtilities.CARD_LOCATION_HAND,
                TestSetup.CardTypeMine, 
                expectedDiscardSize
            );

            ITestAssertion discardEstateCountAssertion = new CardTypeCountAssertion(
                CellarTests.ASSERTION_KEY_DISCARD_ESTATE_COUNT,
                game => game.State.CurrentPlayer.Discard, 
                TestUtilities.CARD_LOCATION_DISCARD,
                TestSetup.CardTypeEstate, 
                expectedDiscardSize
            );

            baseSpec.AssertionsByKey[playedAssertion.Key] = playedAssertion;
            baseSpec.AssertionsByKey[handSizeAssertion.Key] = handSizeAssertion;
            baseSpec.AssertionsByKey[discardSizeAssertion.Key] = discardSizeAssertion;
            baseSpec.AssertionsByKey[handMineCountAssertion.Key] = handMineCountAssertion;
            baseSpec.AssertionsByKey[discardEstateCountAssertion.Key] = discardEstateCountAssertion;

            return baseSpec;
        }

        #endregion

        #region Tests

        [TestCategory("CellarActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestSingleDiscardCellarAction() {
            ActionTestSpecification testSpec = CellarTests.GetBaseSpecification();

            Game game = testSpec.CreateAndBindGame();

            Card cellarCard = TestUtilities.FindCard(game.State.CurrentPlayer.Hand, testSpec.Play);

            game.State.CurrentPlayer.Strategy.PlaySelectionStrategy = new ScriptedPlaySelectionStrategy(cellarCard);
            game.PlayStep();

            Card estateCard = TestUtilities.FindCard(game.State.CurrentPlayer.Hand, TestSetup.CardTypeEstate);

            // Discarding the estate card:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(estateCard, null);
            game.PlayStep();

            // Drawing the replacement card (mine):
            Card mineCard = TestUtilities.FindCard(game.State.CurrentPlayer.Deck, TestSetup.CardTypeMine);
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, game.State.CurrentPlayer);
            game.PlayStep();

            testSpec.AssertAll(game);
        }

        [TestCategory("CellarActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMultipleDiscardCellarAction() {
            ActionTestSpecification customSpec = new ActionTestSpecification();

            customSpec.PlayerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            customSpec.PlayerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            customSpec.PlayerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 3;

            customSpec.HandCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            customSpec.HandCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;

            ActionTestSpecification testSpec 
              = ActionTestSpecification.ApplyOverrides(CellarTests.GetBaseSpecification(expectedDiscardSize: 3), customSpec);

            Game game = testSpec.CreateAndBindGame();

            Card cellarCard = TestUtilities.FindCard(game.State.CurrentPlayer.Hand, testSpec.Play);

            game.State.CurrentPlayer.Strategy.PlaySelectionStrategy = 
              new ScriptedPlaySelectionStrategy(cellarCard);
            game.PlayStep();

            IList<Card> estateCards = TestUtilities.FindCards(game.State.CurrentPlayer.Hand, TestSetup.CardTypeEstate, 3);

            // Discarding the estate cards:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(estateCards, null);
            game.PlayStep();

            // Drawing the replacement cards (mines):
            IList<Card> mineCards = TestUtilities.FindCards(game.State.CurrentPlayer.Deck, TestSetup.CardTypeMine, 3);
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, game.State.CurrentPlayer);
            game.PlayStep();
            
            testSpec.AssertAll(game);
        }

        [TestCategory("CellarActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestNotEnoughCardsToDrawOnCellarAction() {
            ActionTestSpecification customSpec = new ActionTestSpecification();

            customSpec.PlayerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            customSpec.PlayerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;
            customSpec.PlayerCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 1;
            customSpec.PlayerCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 2;

            customSpec.HandCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 1;
            customSpec.HandCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;

            ITestAssertion deckSizeAssertion = new CardCountAssertion(
              CellarTests.ASSERTION_KEY_DECK_SIZE,
              targetGame => targetGame.State.CurrentPlayer.Deck,
              TestUtilities.CARD_LOCATION_DECK,
              2
            );

            ITestAssertion discardEstateCountAssertion = new CardTypeCountAssertion(
              CellarTests.ASSERTION_KEY_DISCARD_ESTATE_COUNT,
              targetGame => targetGame.State.CurrentPlayer.Discard,
              TestUtilities.CARD_LOCATION_DISCARD,
              TestSetup.CardTypeEstate,
              0
            );

            ITestAssertion handEstateCountAssertion = new CardTypeCountAssertion(
              CellarTests.ASSERTION_KEY_HAND_ESTATE_COUNT,
              targetGame => targetGame.State.CurrentPlayer.Hand,
              TestUtilities.CARD_LOCATION_HAND,
              TestSetup.CardTypeEstate,
              1
            );

            ITestAssertion deckEstateCountAssertion = new CardTypeCountAssertion(
              CellarTests.ASSERTION_KEY_DECK_ESTATE_COUNT,
              targetGame => targetGame.State.CurrentPlayer.Deck,
              TestUtilities.CARD_LOCATION_DECK,
              TestSetup.CardTypeEstate,
              2
            );

            ITestAssertion handMineCountAssertion = new CardTypeCountAssertion(
              CellarTests.ASSERTION_KEY_HAND_MINE_COUNT,
              targetGame => targetGame.State.CurrentPlayer.Hand,
              TestUtilities.CARD_LOCATION_HAND,
              TestSetup.CardTypeMine,
              2
            );

            customSpec.AssertionsByKey[deckSizeAssertion.Key] = deckSizeAssertion;
            customSpec.AssertionsByKey[deckEstateCountAssertion.Key] = deckEstateCountAssertion;
            customSpec.AssertionsByKey[handEstateCountAssertion.Key] = handEstateCountAssertion;
            customSpec.AssertionsByKey[discardEstateCountAssertion.Key] = discardEstateCountAssertion;
            customSpec.AssertionsByKey[handMineCountAssertion.Key] = handMineCountAssertion;

            ActionTestSpecification testSpec 
              = ActionTestSpecification.ApplyOverrides(CellarTests.GetBaseSpecification(expectedDiscardSize: 0), customSpec);

            Game game = testSpec.CreateAndBindGame();

            Card cellarCard = TestUtilities.FindCard(game.State.CurrentPlayer.Hand, testSpec.Play);

            game.State.CurrentPlayer.Strategy.PlaySelectionStrategy = new ScriptedPlaySelectionStrategy(cellarCard);
            game.PlayStep();

            // Discarding the estate cards:
            IList<Card> estateCards = TestUtilities.FindCards(game.State.CurrentPlayer.Hand, TestSetup.CardTypeEstate, 3);
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(estateCards, null);
            game.PlayStep();

            // Drawing the replacement cards (mines):
            IList<Card> mineCards = TestUtilities.FindCards(game.State.CurrentPlayer.Deck, TestSetup.CardTypeMine, 1);
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, game.State.CurrentPlayer);
            game.PlayStep();
            
            testSpec.AssertAll(game);
        }

        [TestCategory("CellarActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestNoDiscardCellarAction() {
            ActionTestSpecification customSpec = new ActionTestSpecification();

            ITestAssertion handEstateCountAssertion = new CardTypeCountAssertion(
              CellarTests.ASSERTION_KEY_HAND_ESTATE_COUNT,
              targetGame => targetGame.State.CurrentPlayer.Hand,
              TestUtilities.CARD_LOCATION_HAND,
              TestSetup.CardTypeEstate,
              1
            );

            customSpec.AssertionsByKey[handEstateCountAssertion.Key] = handEstateCountAssertion;

            ActionTestSpecification testSpec 
              = ActionTestSpecification.ApplyOverrides(CellarTests.GetBaseSpecification(expectedDiscardSize: 0), customSpec);

            // Todo - (MT): Add a better test description here.

            Game game = testSpec.CreateAndBindGame();

            Card cellarCard = TestUtilities.FindCard(game.State.CurrentPlayer.Hand, testSpec.Play);

            game.State.CurrentPlayer.Strategy.PlaySelectionStrategy = 
              new ScriptedPlaySelectionStrategy(cellarCard);
            game.PlayStep();

            // Discarding no cards:
            IList<Card> cardsToDiscard = new List<Card>();
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(cardsToDiscard, null);
            game.PlayStep();

            // Drawing no replacement cards:
            IList<Card> mineCards = new List<Card>(game.State.CurrentPlayer.Deck);
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, game.State.CurrentPlayer);
            game.PlayStep();
            
            testSpec.AssertAll(game);
        }

        [TestCategory("CellarActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestCellarDuplicateDiscardTarget() {
            ActionTestSpecification customSpec = new ActionTestSpecification();

            ITestAssertion handEstateCountAssertion = new CardTypeCountAssertion(
              CellarTests.ASSERTION_KEY_HAND_ESTATE_COUNT,
              targetGame => targetGame.State.CurrentPlayer.Hand,
              TestUtilities.CARD_LOCATION_HAND,
              TestSetup.CardTypeEstate,
              1
            );

            customSpec.AssertionsByKey[handEstateCountAssertion.Key] = handEstateCountAssertion;

            ActionTestSpecification testSpec 
              = ActionTestSpecification.ApplyOverrides(CellarTests.GetBaseSpecification(expectedDiscardSize: 0), customSpec);

            Game game = testSpec.CreateAndBindGame();

            Card cellarCard = TestUtilities.FindCard(game.State.CurrentPlayer.Hand, testSpec.Play);

            game.State.CurrentPlayer.Strategy.PlaySelectionStrategy = 
              new ScriptedPlaySelectionStrategy(cellarCard);
            game.PlayStep();

            Card estateCard = TestUtilities.FindCard(game.State.CurrentPlayer.Hand, TestSetup.CardTypeEstate);
            IList<Card> estateCards = new List<Card>() { estateCard, estateCard };

            // Discarding the duplicate estate cards:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(estateCards, null);
            game.PlayStep();

            // Drawing the replacement card (mine):
            Card mineCard = TestUtilities.FindCard(game.State.CurrentPlayer.Deck, TestSetup.CardTypeMine);
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, game.State.CurrentPlayer);
            game.PlayStep();
            
            testSpec.AssertAll(game);
        }

        [TestCategory("CellarActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestInvalidCellarTargetPlayerAction() {
            ActionTestSpecification baseSpec = CellarTests.GetBaseSpecification(expectedHandSize: 3);
            ActionTestSpecification customSpec = new ActionTestSpecification();

            ITestAssertion playedAssertion = new CardPlayedAssertion(
              CellarTests.ASSERTION_KEY_CARD_PLAYED,
              targetGame => TestUtilities.FindCard(targetGame.State.CurrentPlayer.Hand, baseSpec.Play),
              expectedActionsRemaining: 0
            );

            ITestAssertion handMineCountAssertion = new CardTypeCountAssertion(
              CellarTests.ASSERTION_KEY_HAND_MINE_COUNT,
              targetGame => targetGame.State.CurrentPlayer.Hand,
              TestUtilities.CARD_LOCATION_HAND,
              TestSetup.CardTypeMine,
              0
            );

            ITestAssertion handEstateCountAssertion = new CardTypeCountAssertion(
              CellarTests.ASSERTION_KEY_HAND_ESTATE_COUNT,
              targetGame => targetGame.State.CurrentPlayer.Hand,
              TestUtilities.CARD_LOCATION_HAND,
              TestSetup.CardTypeEstate,
              0
            );

            customSpec.AssertionsByKey[playedAssertion.Key] = playedAssertion;
            customSpec.AssertionsByKey[handMineCountAssertion.Key] = handMineCountAssertion;
            customSpec.AssertionsByKey[handEstateCountAssertion.Key] = handEstateCountAssertion;

            ActionTestSpecification testSpec = ActionTestSpecification.ApplyOverrides(baseSpec, customSpec);

            Game game = testSpec.CreateAndBindGame();

            Card cellarCard = TestUtilities.FindCard(game.State.CurrentPlayer.Hand, testSpec.Play);

            game.State.CurrentPlayer.Strategy.PlaySelectionStrategy = new ScriptedPlaySelectionStrategy(cellarCard);
            game.PlayStep();

            Card estateCard = TestUtilities.FindCard(game.State.CurrentPlayer.Hand, TestSetup.CardTypeEstate);

            // Discarding the estate card:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(estateCard, null);
            game.PlayStep();

            // Attempting to draw the replacement card (mine) for the wrong player:
            Card mineCard = TestUtilities.FindCard(game.State.CurrentPlayer.Deck, TestSetup.CardTypeMine);
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, game.Players[1]);
            game.PlayStep();

            testSpec.AssertAll(game);
            Assert.AreEqual(
              5,
              game.Players[1].Hand.Count,
              "The other player's hand should remain unchanged after a cellar is played on multiple players."
            );
            Assert.IsFalse(
              game.Players[1].Hand.Contains(mineCard),
              "The other player's hand should not contain the drawn card (a mine) after a cellar is played invalidly."
            );
        }

        [TestCategory("CellarActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestMultipleCellarTargetPlayersAction() {
            ActionTestSpecification baseSpec = CellarTests.GetBaseSpecification(expectedHandSize: 3);
            ActionTestSpecification customSpec = new ActionTestSpecification();

            ITestAssertion playedAssertion = new CardPlayedAssertion(
              CellarTests.ASSERTION_KEY_CARD_PLAYED,
              targetGame => TestUtilities.FindCard(targetGame.State.CurrentPlayer.Hand, baseSpec.Play),
              expectedActionsRemaining: 0
            );

            ITestAssertion handMineCountAssertion = new CardTypeCountAssertion(
              CellarTests.ASSERTION_KEY_HAND_MINE_COUNT,
              targetGame => targetGame.State.CurrentPlayer.Hand,
              TestUtilities.CARD_LOCATION_HAND,
              TestSetup.CardTypeMine,
              0
            );

            ITestAssertion handEstateCountAssertion = new CardTypeCountAssertion(
              CellarTests.ASSERTION_KEY_HAND_ESTATE_COUNT,
              targetGame => targetGame.State.CurrentPlayer.Hand,
              TestUtilities.CARD_LOCATION_HAND,
              TestSetup.CardTypeEstate,
              0
            );

            customSpec.AssertionsByKey[playedAssertion.Key] = playedAssertion;
            customSpec.AssertionsByKey[handMineCountAssertion.Key] = handMineCountAssertion;
            customSpec.AssertionsByKey[handEstateCountAssertion.Key] = handEstateCountAssertion;

            ActionTestSpecification testSpec = ActionTestSpecification.ApplyOverrides(baseSpec, customSpec);

            Game game = testSpec.CreateAndBindGame();

            Card cellarCard = TestUtilities.FindCard(game.State.CurrentPlayer.Hand, testSpec.Play);

            game.State.CurrentPlayer.Strategy.PlaySelectionStrategy = new ScriptedPlaySelectionStrategy(cellarCard);
            game.PlayStep();

            Card estateCard = TestUtilities.FindCard(game.State.CurrentPlayer.Hand, TestSetup.CardTypeEstate);

            // Discarding the estate card:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(estateCard, null);
            game.PlayStep();

            // Attempting to draw the replacement card (mine) for multiple players:
            Card mineCard = TestUtilities.FindCard(game.State.CurrentPlayer.Deck, TestSetup.CardTypeMine);
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy 
              = new ScriptedTargetSelectionStrategy(null, new List<Player>(game.Players));
            game.PlayStep();

            testSpec.AssertAll(game);
            Assert.AreEqual(
              5,
              game.Players[1].Hand.Count,
              "The other player's hand should remain unchanged after a cellar is played on multiple players."
            );
            Assert.IsFalse(
              game.Players[1].Hand.Contains(mineCard),
              "The other player's hand should not contain the drawn card (a mine) after a cellar is played invalidly."
            );
        }

        [TestCategory("CellarActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestCellarDiscardTargetDiscardedAction() {
            ActionTestSpecification baseSpec = CellarTests.GetBaseSpecification(expectedHandSize: 3);
            ActionTestSpecification customSpec = new ActionTestSpecification();

            ITestAssertion handMineCountAssertion = new CardTypeCountAssertion(
              CellarTests.ASSERTION_KEY_HAND_MINE_COUNT,
              targetGame => targetGame.State.CurrentPlayer.Hand,
              TestUtilities.CARD_LOCATION_HAND,
              TestSetup.CardTypeMine,
              0
            );

            ITestAssertion handEstateCountAssertion = new CardTypeCountAssertion(
              CellarTests.ASSERTION_KEY_HAND_ESTATE_COUNT,
              targetGame => targetGame.State.CurrentPlayer.Hand,
              TestUtilities.CARD_LOCATION_HAND,
              TestSetup.CardTypeEstate,
              0
            );

            customSpec.AssertionsByKey[handMineCountAssertion.Key] = handMineCountAssertion;
            customSpec.AssertionsByKey[handEstateCountAssertion.Key] = handEstateCountAssertion;

            ActionTestSpecification testSpec = ActionTestSpecification.ApplyOverrides(baseSpec, customSpec);

            Game game = testSpec.CreateAndBindGame();

            Card cellarCard = TestUtilities.FindCard(game.State.CurrentPlayer.Hand, testSpec.Play);

            game.State.CurrentPlayer.Strategy.PlaySelectionStrategy = new ScriptedPlaySelectionStrategy(cellarCard);
            game.PlayStep();

            Card estateCard  = TestUtilities.FindCard(game.State.CurrentPlayer.Hand, TestSetup.CardTypeEstate);
            game.State.CurrentPlayer.DiscardCard(estateCard);

            // Attempting to discard the discarded estate card:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(estateCard, null);
            game.PlayStep();

            // Drawing the replacement card (mine):
            Card mineCard = TestUtilities.FindCard(game.State.CurrentPlayer.Deck, TestSetup.CardTypeMine);
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, game.State.CurrentPlayer);
            game.PlayStep();
            
            testSpec.AssertAll(game);
        }

        [TestCategory("CellarActionTest"), TestCategory("ActionLogicTest"), TestMethod]
        public void TestCellarDiscardTargetTrashedAction() {
            ActionTestSpecification baseSpec = CellarTests.GetBaseSpecification(expectedDiscardSize: 0);
            ActionTestSpecification customSpec = new ActionTestSpecification();

            ITestAssertion handEstateCountAssertion = new CardTypeCountAssertion(
              CellarTests.ASSERTION_KEY_HAND_ESTATE_COUNT,
              targetGame => targetGame.State.CurrentPlayer.Hand,
              TestUtilities.CARD_LOCATION_HAND,
              TestSetup.CardTypeEstate,
              1
            );

            customSpec.AssertionsByKey[handEstateCountAssertion.Key] = handEstateCountAssertion;

            ActionTestSpecification testSpec = ActionTestSpecification.ApplyOverrides(baseSpec, customSpec);

            Game game = testSpec.CreateAndBindGame();

            Card cellarCard = TestUtilities.FindCard(game.State.CurrentPlayer.Hand, testSpec.Play);

            game.State.CurrentPlayer.Strategy.PlaySelectionStrategy = new ScriptedPlaySelectionStrategy(cellarCard);
            game.PlayStep();

            Card estateCard  = game.GetCardsByType(TestSetup.CardTypeEstate)[0];
            game.TrashCard(estateCard);

            // Attempting to discard the trashed estate card:
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(estateCard, null);
            game.PlayStep();

            // Drawing the replacement card (mine):
            Card mineCard = TestUtilities.FindCard(game.State.CurrentPlayer.Deck, TestSetup.CardTypeMine);
            game.State.CurrentPlayer.Strategy.TargetSelectionStrategy = new ScriptedTargetSelectionStrategy(null, game.State.CurrentPlayer);
            game.PlayStep();
            
            testSpec.AssertAll(game);
        }

        #endregion
    }
}