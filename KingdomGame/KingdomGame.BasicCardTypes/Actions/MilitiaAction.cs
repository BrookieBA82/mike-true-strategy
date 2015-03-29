﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;
using KingdomGame;

namespace KingdomGame.BasicCardTypes {

    public class MilitiaDiscardTargetSelectionAction : BasePlayerTargetAction {

        // Todo - (MT): Parameterize this into the config file.
        private static readonly string ATTACK_DEFENSE_PROPERTY = "Attack_Defense";

        private MilitiaDiscardTargetSelectionAction() 
          : base(BasePlayerTargetAction.PlayerTargetType.OTHER, 0, int.MaxValue, true) {

        }

        protected override void ApplyInternal(
          IList<Player> players, 
          Game game
        ) {
            foreach (Player player in players) {
                IAction forcedDiscardAction = ActionRegistry.Instance.GetActionByType(typeof(MilitiaForcedDiscardAction)).Create(player);
                game.State.AddPendingAction(forcedDiscardAction);
            }
        }

        protected override bool IsTargetValidInternal(
          IList<Player> players, 
          Card targetingCard,
          Game game
        ) {
            foreach (Player player in players) {
                foreach (Card card in player.Hand) {
                    if (card.Type.Properties.Contains(
                      CardProperty.GetCardPropertyByName(MilitiaDiscardTargetSelectionAction.ATTACK_DEFENSE_PROPERTY))) {
                        return false;
                    }
                }
            }

            return true;
        }
    }

    public class MilitiaForcedDiscardAction : BaseCardTargetAction {

        private MilitiaForcedDiscardAction() : base(BaseCardTargetAction.CardOwnerTargetType.SELF, 0, int.MaxValue) {

        }

        protected override void ApplyInternal(
          IList<Card> cards, 
          Game game
        ) {
            Player targetSelector = game.GetPlayerById(TargetSelectorId.Value);
            foreach (Card card in cards) {
                targetSelector.DiscardCard(card);
            }
        }

        protected override bool IsTargetValidInternal(
          IList<Card> cards, 
          Card targetingCard,
          Game game
        ) {
            if(!TargetSelectorId.HasValue) {
                return false;
            }

            Player targetSelector = game.GetPlayerById(TargetSelectorId.Value);
            int cardsToDiscard = (targetSelector.Hand.Count > 3) ? targetSelector.Hand.Count - 3 : 0;
            if (cards.Count != cardsToDiscard) {
                return false;
            }

            foreach (Card card in cards) {
                if (!targetSelector.Hand.Contains(card)) {
                    return false;
                }
            }

            return true;
        }

        protected override bool IsTargetIndividuallyValid(
          Card target, 
          Card targetingCard,
          Game game
        ) {
            if(!TargetSelectorId.HasValue) {
                return false;
            }

            Player targetSelector = game.GetPlayerById(TargetSelectorId.Value);
            return targetSelector.Hand.Contains(target);
        }

        protected override void CreateInternal(Player targetSelector) {
            if(TargetSelectorId.HasValue) {
                int cardsToDiscard = (targetSelector.Hand.Count > 3) ? targetSelector.Hand.Count - 3 : 0;
                _minTargets = cardsToDiscard;
                _maxTargets = cardsToDiscard;
            }
        }
    }

    public class MilitiaMoneyGainAction : BasePlayerTargetAction {

        private MilitiaMoneyGainAction() : base(BasePlayerTargetAction.PlayerTargetType.SELF, 1, 1) {

        }

        protected override void ApplyInternal(
          IList<Player> players,
          Game game
        ) {
            if (players.Count > 0) {
                players[0].RemainingMoney += 2;
            }
        }
    }
}
