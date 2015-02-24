using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace KingdomGame {

    public class RandomTargetSelectionStrategy : ITargetSelectionStrategy {

        public IList<ITargetable> SelectTargets(
          Game game, 
          Card card, 
          IAction action
        ) {
            List<ITargetable> validTargets = new List<ITargetable>();
            foreach(ITargetable target in action.GetAllPossibleTargets(game)) {
                if (action.IsTargetValid(new List<ITargetable>() { target }, card, game)) {
                    validTargets.Add(target);
                }
            }

            if (action.AllValidTargetsRequired) {
                return validTargets;
            }

            return Combinations.SelectRandomItemsFromList<ITargetable>(validTargets, action.MinTargets, action.MaxTargets);
        }

        public object Clone() {
            return new RandomTargetSelectionStrategy();
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
