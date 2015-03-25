using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame {

    public class ScriptedBuySelectionStrategy : IBuySelectionStrategy {

        private IList<CardType> _optionToSelect;

        public ScriptedBuySelectionStrategy(IList<CardType> optionToSelect) {
            _optionToSelect = optionToSelect;
        }

        public CardType SelectBuy(Game game) {
            IList<IList<CardType>> buyingOptions = game.GetAllValidBuyOptions();
            foreach (IList<CardType> buyingOption in buyingOptions) {
                if (buyingOption.Count != _optionToSelect.Count) {
                    continue;
                }

                bool found = true;
                for (int typeIndex = 0; typeIndex < _optionToSelect.Count; typeIndex++ ) {
                    if(!_optionToSelect[typeIndex].Equals(buyingOption[typeIndex])) {
                        found = false;
                        break;
                    }
                }

                if(found) {
                    if (_optionToSelect.Count > 0) {
                        CardType option = _optionToSelect[0];
                        _optionToSelect.RemoveAt(0);
                        return option;
                    }

                    return null;
                }
            }

            return null;
        }

        public object Clone() {
            return new ScriptedBuySelectionStrategy(new List<CardType>(_optionToSelect));
        }

        public override bool Equals(object obj) {
            ScriptedBuySelectionStrategy strategy = obj as ScriptedBuySelectionStrategy;
            if (strategy == null) {
                return false;
            }

            if (this._optionToSelect.Count != strategy._optionToSelect.Count) {
                return false;
            }

            List<int> thisOptionCardIds = 
              (this._optionToSelect as List<CardType>).ConvertAll<int>(delegate(CardType type) { return type.Id; });

            List<int> strategyOptionCardIds = 
              (strategy._optionToSelect as List<CardType>).ConvertAll<int>(delegate(CardType type) { return type.Id; });

            for (int optionIndex = 0; optionIndex < thisOptionCardIds.Count; optionIndex++) {
                if (!strategyOptionCardIds.Contains(thisOptionCardIds[optionIndex])) {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode() {
            int code = this._optionToSelect.Count.GetHashCode();

            for (int optionIndex = 0; optionIndex < this._optionToSelect.Count; optionIndex++) {
                code = code ^ this._optionToSelect[optionIndex].GetHashCode();
            }
            
            return code;
        }
    }
}
