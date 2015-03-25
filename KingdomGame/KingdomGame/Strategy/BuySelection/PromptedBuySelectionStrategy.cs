using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame {

    public class BuySelectionPromptEventArgs : EventArgs
    {
        public Game Game { get; set; }
        public CardType SelectedBuy { get; set; }
    }

    public delegate void BuySelectionPromptEventHandler(Object sender, BuySelectionPromptEventArgs args);

    public class PromptedBuySelectionStrategy : IBuySelectionStrategy {

        public event BuySelectionPromptEventHandler BuySelectionPromptRequired;

        public CardType SelectBuy(Game game) {
            if (BuySelectionPromptRequired != null) {
                BuySelectionPromptEventArgs args = new BuySelectionPromptEventArgs();
                args.Game = game;
                BuySelectionPromptRequired(this, args);
                CardType selectedBuy = args.SelectedBuy;
                return selectedBuy;
            }
            else {
                return new RandomBuySelectionStrategy().SelectBuy(game);
            }
        }

        public object Clone() {
            return new PromptedBuySelectionStrategy();
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
