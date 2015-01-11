using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame {

    public class ScriptedDiscardingStrategy : IDiscardingStrategy {

        private IList<Card> _cardsToDiscard;

        public ScriptedDiscardingStrategy(IList<Card> cardsToDiscard) {
            _cardsToDiscard = cardsToDiscard;
        }

        public IList<Card> SelectDiscards(Game game, Player player, int cardsToDiscard) {
            List<Card> targetCards = new List<Card>();
            if ((_cardsToDiscard.Count != cardsToDiscard) || (player.Hand.Count < cardsToDiscard)) {
                return targetCards;
            }

            List<Card> remainingCardsInHand = new List<Card>(player.Hand);
            foreach (Card card in _cardsToDiscard) {
                if (!remainingCardsInHand.Contains(card)) {
                    return new List<Card>();
                }

                targetCards.Add(card);
                remainingCardsInHand.Remove(card);
            }

            return targetCards;
        }

        public object Clone() {
            return new ScriptedDiscardingStrategy(new List<Card>(_cardsToDiscard));
        }

        public override bool Equals(object obj) {
            ScriptedDiscardingStrategy strategy = obj as ScriptedDiscardingStrategy;
            if (strategy == null) {
                return false;
            }

            if (this._cardsToDiscard.Count != strategy._cardsToDiscard.Count) {
                return false;
            }

            List<int> thisOptionCardIds = 
              (this._cardsToDiscard as List<Card>).ConvertAll<int>(delegate(Card card) { return card.Id; });

            List<int> strategyOptionCardIds = 
              (strategy._cardsToDiscard as List<Card>).ConvertAll<int>(delegate(Card card) { return card.Id; });

            for (int optionIndex = 0; optionIndex < thisOptionCardIds.Count; optionIndex++) {
                if (!strategyOptionCardIds.Contains(thisOptionCardIds[optionIndex])) {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode() {
            int code = this._cardsToDiscard.Count.GetHashCode();

            for (int cardIndex = 0; cardIndex < this._cardsToDiscard.Count; cardIndex++) {
                code = code ^ this._cardsToDiscard[cardIndex].GetHashCode();
            }
            
            return code;
        }
    }
}
