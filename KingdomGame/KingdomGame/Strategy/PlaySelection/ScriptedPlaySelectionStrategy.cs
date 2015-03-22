using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame {

    public class ScriptedPlaySelectionStrategy : IPlaySelectionStrategy {

        private Card _cardToSelect;

        public ScriptedPlaySelectionStrategy(Card cardToSelect) {
            _cardToSelect = cardToSelect;
        }

        public Card SelectPlay(Game game, Deck currentHand) {
            foreach (Card card in currentHand.Cards) {
                if (card.Type.Class == CardType.CardClass.ACTION && card.Equals(_cardToSelect)) {
                    return card;
                }
            }

            return null;
        }

        public object Clone() {
            Card card = (_cardToSelect != null) ? _cardToSelect.Clone() as Card : null;
            return new ScriptedPlaySelectionStrategy(card);
        }

        public override bool Equals(object obj) {
            ScriptedPlaySelectionStrategy strategy = obj as ScriptedPlaySelectionStrategy;
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
