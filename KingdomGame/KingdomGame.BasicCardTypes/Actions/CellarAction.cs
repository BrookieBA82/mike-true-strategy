using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;
using KingdomGame;

namespace KingdomGame.BasicCardTypes {

    public class CellarDiscardingAction : BaseCardTargetAction {

        public CellarDiscardingAction() : base(BaseCardTargetAction.CardOwnerTargetType.SELF, 0, int.MaxValue) {

        }

        protected override void ApplyInternal(IList<Card> cards, Game game) {
            foreach (Card card in cards) {
                game.CurrentPlayer.DiscardCard(card);
            }
        }

        protected override bool IsTargetValidInternal(
          IList<Card> targetCards, 
          Card targetingCard, 
          Game game
        ) {
            foreach (Card card in targetCards) {
                if (!game.CurrentPlayer.Hand.Contains(card)) {
                    return false;
                }
            }

            return true;
        }
    }

    public class CellarDrawingAction : BasePlayerTargetAction {

        public CellarDrawingAction() : base(BasePlayerTargetAction.PlayerTargetType.SELF, 1, 1) {

        }

        protected override void ApplyInternal(IList<Player> players, Game game) {
            if (players.Count > 0) {
                players[0].Draw(game.State.PreviousActions[0].Second.Count);
                players[0].RemainingActions++;
            }
        }
    }
}
