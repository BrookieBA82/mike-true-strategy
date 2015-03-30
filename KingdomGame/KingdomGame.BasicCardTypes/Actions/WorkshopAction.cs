using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;
using KingdomGame;

namespace KingdomGame.BasicCardTypes {

    public class WorkshopAction : BaseCardTypeTargetAction {

        private WorkshopAction() : base(1, 1, true) {

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

        protected override bool IsTargetSetValidInternal(IList<CardType> targets, Game game) {
            if (targets.Count > 0) {
                return targets[0].Cost <= 4;
            }

            return true;
        }
    }
}
