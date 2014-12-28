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

        protected override void ApplyInternal(IList<Card> cards, Game game, IList<Pair<IAction, IList<int>>> previousActions) {
            if (cards.Count > 0) {
                game.TrashCard(cards[0]);
            }
        }

        protected override bool IsTargetValidInternal(
          IList<Card> targetCards, 
          Card targetingCard, 
          Game game,
          IList<Pair<IAction, IList<int>>> previousActions
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
          Game game, 
          IList<Pair<IAction, IList<int>>> previousActions
        ) {
            if (types.Count > 0) {
                game.DealCard(types[0], game.CurrentPlayer, CardDestination.HAND);
            }
        }

        protected override bool IsTargetValidInternal(
          IList<CardType> targetTypes, 
          Card targetingCard, 
          Game game,
          IList<Pair<IAction, IList<int>>> previousActions
        ) {
            if (targetTypes.Count > 0) {
                if (targetTypes[0].Class != CardType.CardClass.TREASURE) {
                    return false;
                }

                bool trashFound = false;
                int trashedCardCost = 0;
                foreach (Pair<IAction, IList<int>> action in previousActions) {
                    if (action.First is MineTrashingAction && action.Second.Count > 0) {
                        trashedCardCost = game.GetCardById(action.Second[0]).Type.Cost;
                        trashFound = true;
                        break;
                    }
                }

                return trashFound && targetTypes[0].Cost <= trashedCardCost + 3;
            }

            return true;
        }
    }

}
