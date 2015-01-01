using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace KingdomGame {

    public class RandomTargetSelectionStrategy : ITargetSelectionStrategy {

        public IList<TTarget> SelectTargets<TTarget>(
          Game game, 
          Card card, 
          IAction action
        ) where TTarget : class, ITargetable {
            List<TTarget> validTargets = new List<TTarget>();
            foreach(TTarget target in action.GetAllPossibleTargets<TTarget>(game)) {
                if (action.IsTargetValid<TTarget>(new List<TTarget>() { target }, card, game)) {
                    validTargets.Add(target);
                }
            }

            if (action.AllValidTargetsRequired) {
                return validTargets;
            }

            return Combinations.SelectRandomItemsFromList<TTarget>(validTargets, action.MinTargets, action.MaxTargets);
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
