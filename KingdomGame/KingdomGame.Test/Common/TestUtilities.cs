using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame.Test
{
    public class TestUtilities {

        public static Card SetUpCardToPlay(Game game, CardType type) {
            foreach (Card card in game.State.CurrentPlayer.Hand) {
                if (card.Type.Equals(type)) {
                    game.State.CurrentPlayer.Strategy.PlaySelectionStrategy = new ScriptedPlaySelectionStrategy(card);
                    return card;
                }
            }

            return null;
        }

        #region Assertion Helpers

        public static readonly string CARD_LOCATION_HAND = "hand";
        public static readonly string CARD_LOCATION_DISCARD = "discard pile";
        public static readonly string CARD_LOCATION_PLAY_AREA = "play area";
        public static readonly string CARD_LOCATION_DECK = "deck";

        public static void ConfirmCardTypeCount(IList<Card> cards, CardType targetType, int count) {
            ConfirmCardTypeCount(cards, targetType, count, string.Empty);
        }

        public static void ConfirmCardTypeCount(IList<Card> cards, CardType targetType, int count, string location) {
            ConfirmCardTypeCount(cards, targetType, count, location, string.Empty);
        }

        public static void ConfirmCardTypeCount(IList<Card> cards, CardType targetType, int count, string location, string description) {
            int matches = 0;
            foreach (Card card in cards) {
                if (card.Type.Equals(targetType)) {
                    matches++;
                }
            }

            Assert.AreEqual(
              count, 
              matches, 
              string.Format("The {0} should contain {1} {2}{3}.", 
                (!string.IsNullOrEmpty(location)) ? location : "card set", 
                count, 
                (count != 1) ? string.Format("{0}(s)", targetType.Name) : targetType.Name, 
                (!string.IsNullOrEmpty(description)) ? string.Format(" {0}", description) : string.Empty
              )
            );
        }

        public static void ConfirmCardCount(IList<Card> cards, int count) {
            ConfirmCardCount(cards, count, string.Empty);
        }

        public static void ConfirmCardCount(IList<Card> cards, int count, string location) {
            ConfirmCardCount(cards, count, location, string.Empty);
        }

        public static void ConfirmCardCount(IList<Card> cards, int count, string location, string description) {
            Assert.AreEqual(
              count, 
              cards.Count, 
              string.Format("The {0} should contain {1} {2}{3}.", 
                (!string.IsNullOrEmpty(location)) ? location : "card set", 
                count, 
                (count != 1) ? "cards" : "card", 
                (!string.IsNullOrEmpty(description)) ? string.Format(" {0}", description) : string.Empty
              )
            );
        }

        public static void ConfirmCardPlayed(Game game, Card card) {
            ConfirmCardPlayed(game, card, 0);
        }

        public static void ConfirmCardPlayed(Game game, Card card, int expectedActionsRemaining) {
            string description = string.Format("after a {0} is played", card.Type.Name);
            ConfirmCardPlayed(game, card, expectedActionsRemaining, new Dictionary<CardType, int>(), description);
        }

        public static void ConfirmCardPlayed(Game game, Card card, string description) {
            ConfirmCardPlayed(game, card, 0, description);
        }

        public static void ConfirmCardPlayed(Game game, Card card, int expectedActionsRemaining, string description) {
            ConfirmCardPlayed(game, card, expectedActionsRemaining, new Dictionary<CardType, int>(), description);
        }

        public static void ConfirmCardPlayed(
          Game game, 
          Card card, 
          int expectedActionsRemaining, 
          IDictionary<CardType, int> additionalCardsByType, 
          string description
        ) {
            // Todo - (MT): Make this its own series of asserts, permitting flexibility based on type of remaining (Action, Buys, etc.)
            Assert.AreEqual(
              expectedActionsRemaining, 
              game.State.CurrentPlayer.RemainingActions, 
              string.Format(
                "There should be {0} {1} remaining{2}.", 
                expectedActionsRemaining, 
                (expectedActionsRemaining != 1) ? "actions" : "action",
                (!string.IsNullOrEmpty(description)) ? string.Format(" {0}", description) : string.Empty
              )
            );


            int totalExpectedCardsPlayed = 0;
            foreach (CardType type in additionalCardsByType.Keys) {
                int expectedCardsPlayed = (type.Equals(card.Type)) ? additionalCardsByType[type] + 1 : additionalCardsByType[type];

                TestUtilities.ConfirmCardTypeCount(
                    game.State.CurrentPlayer.PlayArea,
                    type,
                    expectedCardsPlayed, 
                    TestUtilities.CARD_LOCATION_PLAY_AREA, 
                    description
                );

                totalExpectedCardsPlayed += expectedCardsPlayed;
            }

            if (!additionalCardsByType.ContainsKey(card.Type)) {
                totalExpectedCardsPlayed++;

                TestUtilities.ConfirmCardTypeCount(
                    game.State.CurrentPlayer.PlayArea, 
                    card.Type,
                    1, 
                    TestUtilities.CARD_LOCATION_PLAY_AREA, 
                    description
                );
            }

            TestUtilities.ConfirmCardCount(
                game.State.CurrentPlayer.PlayArea,
                totalExpectedCardsPlayed, 
                TestUtilities.CARD_LOCATION_PLAY_AREA, 
                description
            );

            Assert.IsTrue(
              game.State.CurrentPlayer.PlayArea.Contains(card), 
              string.Format(
                "The play area should contain the selected {0}{1}.", 
                card.Type.Name, 
                (!string.IsNullOrEmpty(description)) ? string.Format(" {0}", description) : string.Empty
              )
            );
        }

        #endregion

        public static void ForceGamePhase(Game game, Game.Phase phase) {
            if (phase == Game.Phase.BUY && game.State.CurrentPlayer.RemainingBuys == 0) {
                throw new ArgumentException("Cannot force the phase to BUY if there are no remaining buys.");
            }

            if (phase != Game.Phase.ACTION) {
                game.State.SelectedPlay = null;
            }

            if (game.State.CurrentPlayer.RemainingActions > 0
              && !(phase == Game.Phase.PLAY || phase == Game.Phase.ACTION)) {
                game.State.CurrentPlayer.RemainingActions = 0;
            }

            Type stateType = typeof(Game.GameState);
            FieldInfo phaseField = stateType.GetField("_phase", BindingFlags.NonPublic | BindingFlags.Instance);
            phaseField.SetValue(game.State, phase);
        }

        #region Search Helpers

        public static Card FindCard(IList<Card> sourceCards, CardType cardType) {
            return FindCards(sourceCards, cardType, 1)[0];
        }

        public static IList<Card> FindCards(IList<Card> sourceCards, CardType cardType, int count) {
            IList<Card> cards = new List<Card>();
            foreach(Card card in sourceCards) {
                if (card.Type.Equals(cardType)) {
                    cards.Add(card);
                    if (cards.Count >= count) {
                        break;
                    }
                }
            }

            if (cards.Count < count) {
                throw new InvalidOperationException(string.Format("Could not find enough cards of type {0} in the deck.", cardType.Name));
            }

            return cards;
        }

        #endregion
    }
}