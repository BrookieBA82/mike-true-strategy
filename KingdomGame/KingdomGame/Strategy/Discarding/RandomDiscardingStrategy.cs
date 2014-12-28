using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame {

    public class RandomDiscardingStrategy : IDiscardingStrategy {

        public IList<Card> FindOptimalDiscardingStrategy(Game game, Player player, int cardsToDiscard) {
            List<Card> targetCards = new List<Card>();
            if (player.Hand.Count < cardsToDiscard) {
                return targetCards;
            }

            List<Card> remainingCardsInHand = new List<Card>(player.Hand);
            for (int cardsSelected = 0; cardsSelected < cardsToDiscard; cardsSelected++) {
                Card card = remainingCardsInHand[RandomNumberManager.Next(remainingCardsInHand.Count)];
                remainingCardsInHand.Remove(card);
                targetCards.Add(card);
            }

            return targetCards;
        }

        public object Clone() {
            return new RandomDiscardingStrategy();
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
