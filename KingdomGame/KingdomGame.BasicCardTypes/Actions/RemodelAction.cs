using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;
using KingdomGame;

namespace KingdomGame.BasicCardTypes {

    public class RemodelTrashingAction : BaseCardTargetAction {

        public RemodelTrashingAction() : base(BaseCardTargetAction.CardOwnerTargetType.SELF, 0, 1) {

        }

        protected override void ApplyInternal(IList<Card> cards, Game game) {
            if (cards.Count > 0) {
                game.TrashCard(cards[0]);
            }
        }

        protected override bool IsTargetValidInternal(
          IList<Card> targetCards, 
          Card targetingCard, 
          Game game
        ) {
            if (targetCards.Count > 0) {
                return game.CurrentPlayer.Hand.Contains(targetCards[0]);
            }

            return true;
        }
    }

    public class RemodelGainingAction : BaseCardTypeTargetAction {

        public RemodelGainingAction() : base(1, 1, true, true) {

        }

        protected override void ApplyInternal(
          IList<CardType> types, 
          Game game
        ) {
            if (types.Count > 0) {
                game.DealCard(types[0], game.CurrentPlayer, CardDestination.DISCARD);
            }
        }

        protected override bool IsTargetValidInternal(
          IList<CardType> targets, 
          Card targetingCard, 
          Game game
        ) {
            if (targets.Count > 0) {
                bool trashFound = false;
                int trashedCardCost = 0;
                foreach (Pair<IAction, IList<int>> action in game.State.PreviousActions) {
                    if (action.First is RemodelTrashingAction && action.Second.Count > 0) {
                        trashedCardCost = game.GetCardById(action.Second[0]).Type.Cost;
                        trashFound = true;
                        break;
                    }
                }

                return trashFound && targets[0].Cost <= trashedCardCost + 2;
            }

            return true;
        }
    }
}
