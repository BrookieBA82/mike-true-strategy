using System;
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

    public class MilitiaForcedDiscardAction : BasePlayerTargetAction {

        private MilitiaForcedDiscardAction() : base(BasePlayerTargetAction.PlayerTargetType.ANY, 1, 1) {

        }

        protected override void ApplyInternal(
          IList<Player> players, 
          Game game
        ) {
            Player player = game.GetPlayerById(_targetSelectorId.Value);
            IList<Card> cardsToDiscard = player.Strategy.DiscardingStrategy.
                SelectDiscards(game, player, (player.Hand.Count > 3) ? player.Hand.Count - 3 : 0);

            foreach (Card card in cardsToDiscard) {
                player.DiscardCard(card);
            }

            // Modify the history so the effective target for this action are the discarded cards:
            Logger.Instance.UpdateLastAction(game, null, cardsToDiscard);
        }

        protected override bool IsTargetValidInternal(
          IList<Player> players, 
          Card targetingCard,
          Game game
        ) {
            return true;
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
