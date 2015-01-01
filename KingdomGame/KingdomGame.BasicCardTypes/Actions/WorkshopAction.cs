using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;
using KingdomGame;

namespace KingdomGame.BasicCardTypes {

    public class WorkshopAction : BaseCardTypeTargetAction {

        public WorkshopAction() : base(1, 1, true, true) {

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
                return targets[0].Cost <= 4;
            }

            return true;
        }
    }
}
