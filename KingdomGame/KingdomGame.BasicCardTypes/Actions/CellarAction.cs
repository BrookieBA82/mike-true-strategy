using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;
using KingdomGame;

namespace KingdomGame.BasicCardTypes {

    public class CellarDiscardingAction : BaseCardTargetAction {

        private CellarDiscardingAction() : base(BaseCardTargetAction.CardOwnerTargetType.SELF, 0, int.MaxValue) {

        }

        protected override void ApplyInternal(IList<Card> cards, Game game) {
            Player targetSelector = GetTargetSelector(game);
            foreach (Card card in cards) {
                targetSelector.DiscardCard(card);
            }
        }

        protected override bool IsTargetValidInternal(
          IList<Card> targetCards, 
          Card targetingCard, 
          Game game
        ) {
            Player targetSelector = GetTargetSelector(game);
            foreach (Card card in targetCards) {
                if (!targetSelector.Hand.Contains(card)) {
                    return false;
                }
            }

            return true;
        }
    }

    public class CellarDrawingAction : BasePlayerTargetAction {

        private CellarDrawingAction() : base(BasePlayerTargetAction.PlayerTargetType.SELF, 1, 1) {

        }

        protected override void ApplyInternal(IList<Player> players, Game game) {
            if (players.Count > 0) {
                IList<int> discardingTargetIds = game.State.GetTargetsFromLastExecutedAction(typeof(CellarDiscardingAction));

                if (discardingTargetIds != null) {
                    players[0].Draw(discardingTargetIds.Count);
                    players[0].RemainingActions++;
                }
            }
        }
    }
}
