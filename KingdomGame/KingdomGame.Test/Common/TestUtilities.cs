﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                    game.State.CurrentPlayer.Strategy.CardSelectionStrategy = new ScriptedCardSelectionStrategy(card);
                    return card;
                }
            }

            return null;
        } 

        public static void ConfirmCardPlayed(Game game, Card card) {
            ConfirmCardPlayed(game, card, 0);
        }

        public static void ConfirmCardPlayed(Game game, Card card, int expectedActionsRemaining) {
            Assert.AreEqual(
              expectedActionsRemaining, 
              game.State.CurrentPlayer.RemainingActions, 
              string.Format(
                "There should be {0} action(s) remaining after a {1} is played.", 
                expectedActionsRemaining, 
                card.Type.Name
              )
            );

            Assert.AreEqual(
              1, 
              game.State.CurrentPlayer.PlayArea.Count, 
              string.Format("The play area should have one card after a {0} is played.", card.Type.Name)
            );

            Assert.AreEqual(
              card, 
              game.State.CurrentPlayer.PlayArea[0], 
              string.Format("The play area should have a {0} after one is played.", card.Type.Name)
            );
        }

        public static void ForceGamePhase(Game game, Game.Phase phase) {
            if (phase == Game.Phase.BUY && game.State.CurrentPlayer.RemainingBuys == 0) {
                throw new ArgumentException("Cannot force the phase to BUY if there are no remaining buys.");
            }

            if (phase != Game.Phase.ACTION) {
                game.State.SelectedCard = null;
            }

            if (game.State.CurrentPlayer.RemainingActions > 0
              && !(phase == Game.Phase.PLAY || phase == Game.Phase.ACTION)) {
                game.State.CurrentPlayer.RemainingActions = 0;
            }

            Type stateType = typeof(Game.GameState);
            FieldInfo phaseField = stateType.GetField("_phase", BindingFlags.NonPublic | BindingFlags.Instance);
            phaseField.SetValue(game.State, phase);
        }
    }
}