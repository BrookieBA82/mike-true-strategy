﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame {

    public class RandomBuySelectionStrategy : IBuySelectionStrategy {

        public CardType SelectBuy(Game game) {
            IList<IList<CardType>> buyingOptions = game.GetAllValidBuyOptions();
            if (buyingOptions.Count == 0) {
                return null;
            }

            IList<CardType> option = buyingOptions[RandomNumberManager.Next(buyingOptions.Count)];
            return (option.Count > 0) ? option[0] : null;
        }

        public object Clone() {
            return new RandomBuySelectionStrategy();
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
