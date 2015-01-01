using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;
using KingdomGame;

namespace KingdomGame.BasicCardTypes {

    public class MineTrashingAction : BaseCardTargetAction {

        public MineTrashingAction() : base(BaseCardTargetAction.CardOwnerTargetType.SELF, 0, 1) {

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
                return targetCards[0].Type.Class == CardType.CardClass.TREASURE 
                    && game.CurrentPlayer.Hand.Contains(targetCards[0]);
            }

            return true;
        }
    }

    public class MineGainingAction : BaseCardTypeTargetAction {

        public MineGainingAction() : base(1, 1, true, true) {

        }

        protected override void ApplyInternal(
          IList<CardType> types, 
          Game game
        ) {
            if (types.Count > 0) {
                game.DealCard(types[0], game.CurrentPlayer, CardDestination.HAND);
            }
        }

        protected override bool IsTargetValidInternal(
          IList<CardType> targetTypes, 
          Card targetingCard, 
          Game game
        ) {
            if (targetTypes.Count > 0) {
                if (targetTypes[0].Class != CardType.CardClass.TREASURE) {
                    return false;
                }

                IList<int> trashingTargetIds = game.State.GetTargetsFromLastExecutedAction(typeof(MineTrashingAction));

                if ((trashingTargetIds != null) && (trashingTargetIds.Count > 0)) {
                    int trashedCardCost = game.GetCardById(trashingTargetIds[0]).Type.Cost;
                    return targetTypes[0].Cost <= trashedCardCost + 3;
                }

                return false;
            }

            return true;
        }
    }

}
