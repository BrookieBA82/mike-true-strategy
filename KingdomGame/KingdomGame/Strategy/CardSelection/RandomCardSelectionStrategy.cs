using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame {

    public class RandomCardSelectionStrategy : ICardSelectionStrategy {

        public Card SelectCard(Game game, Deck currentHand) {
            Deck randomizedHand = currentHand.Clone() as Deck;
            randomizedHand.Shuffle();

            foreach (Card card in randomizedHand.Cards) {
                if (card.Type.Class == CardType.CardClass.ACTION) {
                    return card;
                }
            }

            return null;
        }

        public object Clone() {
            return new RandomCardSelectionStrategy();
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
