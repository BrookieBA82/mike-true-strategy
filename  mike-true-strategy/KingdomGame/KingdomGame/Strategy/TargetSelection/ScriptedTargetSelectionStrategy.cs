using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace KingdomGame {

    public class ScriptedTargetSelectionStrategy : ITargetSelectionStrategy {

        private Type _targetType;
        private IList<ITargetable> _targets;

        public ScriptedTargetSelectionStrategy(IList<ITargetable> targets) {
            _targetType = null;
            foreach (ITargetable target in targets) {
                if (_targetType == null) {
                    _targetType = target.GetType();
                }
                else if (!_targetType.Equals(target.GetType())) {
                    _targetType = null;
                    _targets = new List<ITargetable>();
                    return;
                }
            }

            _targets = new List<ITargetable>(targets);
        }

        // Todo - (MT): Deprecate these two constructors
        public ScriptedTargetSelectionStrategy(Card cardToTarget, Player playerToTarget) : this(
          (cardToTarget != null) ? new List<Card>() {cardToTarget} : new List<Card>(),
          (playerToTarget != null) ? new List<Player>() {playerToTarget} : new List<Player>()
        ){

        }

        public ScriptedTargetSelectionStrategy(IList<Card> cardsToTarget, IList<Player> playersToTarget)
          : this((cardsToTarget != null && cardsToTarget.Count > 0) 
              ? new List<ITargetable>(cardsToTarget)
              : (playersToTarget != null) ? new List<ITargetable>(playersToTarget) : new List<ITargetable>()) {

        }
        // End Todo Block

        public IList<TTarget> SelectTargets<TTarget>(
          Game game, 
          Card card, 
          IAction action, 
          IList<Pair<IAction, IList<int>>> previousActions
        ) where TTarget : class, ITargetable {
            if (typeof(TTarget).Equals(_targetType)) {
                IList<TTarget> targets = new List<TTarget>();
                foreach (ITargetable target in _targets) {
                    targets.Add(target as TTarget);
                }
                return action.IsTargetValid<TTarget>(targets, card, game, previousActions)
                  ? targets
                  : new List<TTarget>();
            }

            return new List<TTarget>();
        }

        public object Clone() {
            IList<ITargetable> targets = new List<ITargetable>();
            foreach (ITargetable target in _targets) {
                targets.Add(target);
            }

            return new ScriptedTargetSelectionStrategy(targets);
        }

        public override bool Equals(object obj) {
            ScriptedTargetSelectionStrategy strategy = obj as ScriptedTargetSelectionStrategy;
            if(strategy == null) {
                return false;
            }

            if (this._targets.Count != strategy._targets.Count) {
                return false;
            }

            for (int targetIndex = 0; targetIndex < _targets.Count; targetIndex++) {
                if (!this._targets[targetIndex].Equals(strategy._targets[targetIndex])) {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode() {
            int code = _targets.Count.GetHashCode();

            for (int targetIndex = 0; targetIndex < _targets.Count; targetIndex++) {
                code ^= _targets[targetIndex].GetHashCode();
            }

            return code;
        }
    }
}
