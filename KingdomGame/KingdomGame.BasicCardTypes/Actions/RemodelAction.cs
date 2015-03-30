using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;
using KingdomGame;

namespace KingdomGame.BasicCardTypes {

    public class RemodelTrashingAction : BaseCardTargetAction {

        private RemodelTrashingAction() : base(BaseCardTargetAction.CardOwnerTargetType.SELF, 0, 1) {

        }

        protected override void ApplyInternal(IList<Card> cards, Game game) {
            if (cards.Count > 0) {
                game.TrashCard(cards[0]);
            }
        }

        protected override bool IsTargetSetValidInternal(
          IList<Card> targetCards, 
          Card targetingCard, 
          Game game
        ) {
            if (targetCards.Count > 0) {
                Player targetSelector = GetTargetSelector(game);
                return targetSelector.Hand.Contains(targetCards[0]);
            }

            return true;
        }
    }

    public class RemodelGainingAction : BaseCardTypeTargetAction {

        private RemodelGainingAction() : base(1, 1, true) {

        }

        protected override void ApplyInternal(
          IList<CardType> types, 
          Game game
        ) {
            if (types.Count > 0) {
                Player targetSelector = GetTargetSelector(game);
                game.DealCard(types[0], targetSelector, CardDestination.DISCARD);
            }
        }

        protected override bool IsTargetSetValidInternal(
          IList<CardType> targets, 
          Card targetingCard, 
          Game game
        ) {
            if (targets.Count > 0) {

                IList<int> trashingTargetIds = game.State.GetTargetsFromLastExecutedAction(typeof(RemodelTrashingAction));

                if ((trashingTargetIds != null) && (trashingTargetIds.Count > 0)) {
                    int trashedCardCost = game.GetCardById(trashingTargetIds[0]).Type.Cost;
                    return targets[0].Cost <= trashedCardCost + 2;
                }

                return false;
            }

            return true;
        }
    }
}
