using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace KingdomGame {

    public class ForcedDiscardPromptEventArgs : EventArgs
    {
        public Game Game { get; set; }
        public Player Player { get; set; }
        public int CardsToDiscard { get; set; }
        public IList<Card> SelectedCards { get; set; }
    }

    public delegate void ForcedDiscardPromptEventHandler(Object sender, ForcedDiscardPromptEventArgs e);

    public class PromptedDiscardingStrategy : IDiscardingStrategy {

        public event ForcedDiscardPromptEventHandler ForcedDiscardPromptRequired;

        public IList<Card> FindOptimalDiscardingStrategy(
          Game game,
          Player player,
          int cardsToDiscard
        ) {
            if (ForcedDiscardPromptRequired != null) {
                ForcedDiscardPromptEventArgs args = new ForcedDiscardPromptEventArgs();
                args.Game = game;
                args.Player = player;
                args.CardsToDiscard = cardsToDiscard;
                ForcedDiscardPromptRequired(this, args);
                return new List<Card>(args.SelectedCards);
            }
            else {
                return new RandomDiscardingStrategy().FindOptimalDiscardingStrategy(game, player, cardsToDiscard);
            }
        }

        public object Clone() {
            return new PromptedDiscardingStrategy();
        }

        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }

            return this.GetType().Equals(obj.GetType());
        }

        public override int GetHashCode() {
            return this.GetType().GetHashCode();
        }
    }
}