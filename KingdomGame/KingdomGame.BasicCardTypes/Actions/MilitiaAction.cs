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

        public MilitiaDiscardTargetSelectionAction() 
          : base(BasePlayerTargetAction.PlayerTargetType.OTHER, 0, int.MaxValue, true) {

        }

        protected override void ApplyInternal(
          IList<Player> players, 
          Game game, 
          IList<Pair<IAction, IList<int>>> previousActions
        ) {
            foreach (Player player in players) {
                IAction forcedDiscardAction = new MilitiaForcedDiscardAction(player.Id);
                forcedDiscardAction.DisplayName = "Militia Forced Discard Phase";
                forcedDiscardAction.ActionDescription = string.Format(
                  "discarding {0} cards for Militia",
                  Math.Max(player.Hand.Count - 3, 0)
                );
                game.State.AddPendingAction(forcedDiscardAction);
            }
        }

        protected override bool IsTargetValidInternal(
          IList<Player> players, 
          Card targetingCard,
          Game game,
          IList<Pair<IAction, IList<int>>> previousActions
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

        public MilitiaForcedDiscardAction(int playerId) : base(BasePlayerTargetAction.PlayerTargetType.ANY, 1, 1) {
            _executingPlayerId = playerId;
        }

        protected override void ApplyInternal(
          IList<Player> players, 
          Game game, 
          IList<Pair<IAction, IList<int>>> previousActions
        ) {
            Player player = game.GetPlayerById(_executingPlayerId.Value);
            IList<Card> cardsToDiscard = game.CurrentStrategy.DiscardingStrategiesByPlayerId[player.Id].
                FindOptimalDiscardingStrategy(game, player, (player.Hand.Count > 3) ? player.Hand.Count - 3 : 0);

            foreach (Card card in cardsToDiscard) {
                player.DiscardCard(card);
            }

            // Modify the history so the effective target for this action are the discarded cards:
            Logger.Instance.UpdateLastAction(game, null, cardsToDiscard);
        }

        protected override bool IsTargetValidInternal(
          IList<Player> players, 
          Card targetingCard,
          Game game,
          IList<Pair<IAction, IList<int>>> previousActions
        ) {
            return true;
        }
    }

    public class MilitiaMoneyGainAction : BasePlayerTargetAction {

        public MilitiaMoneyGainAction() : base(BasePlayerTargetAction.PlayerTargetType.SELF, 1, 1) {

        }

        protected override void ApplyInternal(
          IList<Player> players,
          Game game, 
          IList<Pair<IAction, IList<int>>> previousActions
        ) {
            if (players.Count > 0) {
                players[0].RemainingMoney += 2;
            }
        }
    }
}
