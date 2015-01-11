using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame {

    public class ScriptedCardSelectionStrategy : ICardSelectionStrategy {

        private Card _cardToSelect;

        public ScriptedCardSelectionStrategy(Card cardToSelect) {
            _cardToSelect = cardToSelect;
        }

        public Card SelectCard(Game game, Deck currentHand) {
            foreach (Card card in currentHand.Cards) {
                if (card.Type.Class == CardType.CardClass.ACTION && card.Equals(_cardToSelect)) {
                    return card;
                }
            }

            return null;
        }

        public object Clone() {
            Card card = (_cardToSelect != null) ? _cardToSelect.Clone() as Card : null;
            return new ScriptedCardSelectionStrategy(card);
        }

        public override bool Equals(object obj) {
            ScriptedCardSelectionStrategy strategy = obj as ScriptedCardSelectionStrategy;
            if(strategy == null) {
                return false;
            }

            if (this._cardToSelect == null) {
                return (strategy._cardToSelect == null);
            }

            return this._cardToSelect.Equals(strategy._cardToSelect);
        }

        public override int GetHashCode() {
            if (this._cardToSelect == null) {
                return 0;
            }

            return this._cardToSelect.GetHashCode();
        }
    }
}
