using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame {

    public class CardSelectionPromptEventArgs : EventArgs
    {
        public Game Game { get; set; }
        public Deck CurrentHand { get; set; }
        public Card SelectedCard { get; set; }
    }

    public delegate void CardSelectionPromptEventHandler(Object sender, CardSelectionPromptEventArgs e);

    public class PromptedCardSelectionStrategy : ICardSelectionStrategy {

        public event CardSelectionPromptEventHandler CardSelectionPromptRequired;

        public Card SelectCard(Game game, Deck currentHand) {
            if (CardSelectionPromptRequired != null) {
                CardSelectionPromptEventArgs args = new CardSelectionPromptEventArgs();
                args.Game = game;
                args.CurrentHand = currentHand.Clone() as Deck;
                CardSelectionPromptRequired(this, args);
                Card selectedCard = args.SelectedCard;
                return (selectedCard != null && currentHand.Contains(selectedCard)) ? selectedCard : null;
            }
            else {
                return new RandomCardSelectionStrategy().SelectCard(game, currentHand);
            }
        }

        public object Clone() {
            return new PromptedCardSelectionStrategy();
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
