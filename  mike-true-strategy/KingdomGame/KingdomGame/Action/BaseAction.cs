using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace KingdomGame {

    public abstract class BaseAction<TTarget> : IAction where TTarget : class, ITargetable {

        protected Type _targetType;
        protected int _minTargets;
        protected int _maxTargets;
        protected int? _executingPlayerId = null;
        protected bool _allValidTargetsRequired;

        protected bool _duplicateTargetsAllowed;

        protected string _displayName;
        protected string _actionDescription;

        public BaseAction (
          int minTargets, 
          int maxTargets,
          bool duplicateTargetsAllowed
        ) : this(minTargets, maxTargets, duplicateTargetsAllowed, false) {

        }

        public BaseAction (
          int minTargets, 
          int maxTargets,
          bool duplicateTargetsAllowed,
          bool allValidTargetsRequired
        ) {
            if (minTargets > maxTargets) {
                throw new ArgumentException("Cannot specify a minimum number of targets larger than the maximum.");
            }

            _targetType = typeof(TTarget);
            _minTargets = minTargets;
            _maxTargets = maxTargets;
            _duplicateTargetsAllowed = duplicateTargetsAllowed;
            _allValidTargetsRequired = allValidTargetsRequired;
        }

        public bool IsTargetValid<TType>(
          IList<TType> targetSet, 
          Card targetingCard, 
          Game game, 
          IList<Pair<IAction, IList<int>>> previousActions
        ) where TType : class, ITargetable {
            if(typeof(TType) == _targetType  || typeof(TType) == typeof(ITargetable)) {
                IList<TTarget> typedTargetSet = new List<TTarget>();
                foreach(TType target in targetSet) {
                    if(!(target is TTarget)) {
                        return false;
                    }

                    typedTargetSet.Add(target as TTarget);
                }

                if ((typedTargetSet.Count < _minTargets) || (typedTargetSet.Count > _maxTargets)) {
                    return false;
                }

                if (!_duplicateTargetsAllowed) {
                    Set<TTarget> targets = new Set<TTarget>();
                    foreach (TTarget target in typedTargetSet) {
                        if (targets.Contains(target)) {
                            return false;
                        }

                        targets.Add(target);
                    }
                }

                IList<TTarget> eligibleTargets = new List<TTarget>();
                IList<TTarget> allTypedTargets = GetAllPossibleTargetsBase(game);
                foreach(TTarget target in allTypedTargets) {
                    if (IsTargetValidBase(
                      target,
                      targetingCard, 
                      game, 
                      previousActions
                    )) {
                        eligibleTargets.Add(target);
                    }
                    else {
                        if (typedTargetSet.Contains(target)) {
                            return false;
                        }
                    }
                }

                if(_allValidTargetsRequired && !AreAllValidTargetsIncluded(
                  typedTargetSet, 
                  eligibleTargets, 
                  allTypedTargets, 
                  targetingCard, 
                  game, 
                  previousActions
                )) {
                    return false;
                }

                return IsTargetValidInternal(typedTargetSet, targetingCard, game, previousActions);
            }

            return false;
        }

        public void Apply<TType>(
          IList<TType> targetSet,
          Game game, 
          IList<Pair<IAction, IList<int>>> previousActions
        ) where TType : class, ITargetable {
            if(typeof(TType) == _targetType) {
                IList<TTarget> typedTargetSet = (targetSet != null && targetSet.Count > 0) 
                  ? targetSet as IList<TTarget> 
                  : new List<TTarget>();

                ApplyInternal(typedTargetSet, game, previousActions);
            }
            else {
                throw new InvalidOperationException(string.Format(
                  "Cannot apply this action to {0} because it can only be applied to {1}.",
                  typeof(TType).ToString(),
                  _targetType.ToString()
                ));
            }
        }

        public IList<ITargetable> GetAllValidTargets(
          Card targetingCard, 
          Game game, 
          IList<Pair<IAction, IList<int>>> previousActions
        ) {
            IList<ITargetable> validTargets = new List<ITargetable>();
            foreach (ITargetable target in GetAllPossibleTargets<ITargetable>(game)) {
                if (IsTargetValid<ITargetable>(new List<ITargetable>() { target }, targetingCard, game, previousActions)) {
                    validTargets.Add(target);
                }
            }

            return validTargets;
        }

        public IList<TType> GetAllPossibleTargets<TType>(Game game) where TType : class, ITargetable {
            if(typeof(TType) == _targetType || typeof(TType) == typeof(ITargetable)) {
                IList<TTarget> allPossibleTargets = GetAllPossibleTargetsBase(game);
                IList<TType> allTypedTargets = new List<TType>();
                foreach (TTarget target in allPossibleTargets) {
                    allTypedTargets.Add(target as TType);
                }

                return allTypedTargets;
            }

            return new List<TType>();
        }

        public int MinTargets { get { return _minTargets; } }

        public int MaxTargets { get { return _maxTargets; } }

        public int? ExecutingPlayerId { get { return _executingPlayerId; } }

        public bool AllValidTargetsRequired { get { return _allValidTargetsRequired; } }

        public string DisplayName { 
            get {return (!string.IsNullOrEmpty(_displayName)) ? _displayName : GetType().Name; }
            set { _displayName = value; }
        }

        public string ActionDescription { 
            get {return (!string.IsNullOrEmpty(_actionDescription)) ? _actionDescription : GetType().Name; }
            set { _actionDescription = value; }
        }

        public override bool Equals(object obj) {
            BaseAction<TTarget> action = obj as BaseAction<TTarget>;
            if (action == null) {
                return false;
            }

            return GetType() == action.GetType()
              && _minTargets == action._minTargets 
              && _maxTargets == action._maxTargets
              && _duplicateTargetsAllowed == action._duplicateTargetsAllowed
              && _allValidTargetsRequired == action._allValidTargetsRequired
              && _executingPlayerId.HasValue == action._executingPlayerId.HasValue
              && (!_executingPlayerId.HasValue || (_executingPlayerId.Value == action._executingPlayerId.Value));
        }

        public override int GetHashCode() {
            return GetType().GetHashCode() 
              ^ _minTargets.GetHashCode()
              ^ _maxTargets.GetHashCode()
              ^ _duplicateTargetsAllowed.GetHashCode()
              ^ _allValidTargetsRequired.GetHashCode()
              ^ ((_executingPlayerId.HasValue) ? _executingPlayerId.Value.GetHashCode() : false.GetHashCode());
        }

        protected abstract void ApplyInternal(
          IList<TTarget> target, 
          Game game, 
          IList<Pair<IAction, 
          IList<int>>> previousActions
        );

        protected abstract IList<TTarget> GetAllPossibleTargetsBase(Game game);

        protected virtual bool IsTargetValidBase(
          TTarget target,
          Card targetingCard,
          Game game,
          IList<Pair<IAction, IList<int>>> previousActions
        ) {
            return true;
        }

        protected virtual bool IsTargetValidInternal(
          IList<TTarget> targets, 
          Card targetingCard,
          Game game,
          IList<Pair<IAction, IList<int>>> previousActions
        ) {
            return true;
        }

        private bool AreAllValidTargetsIncluded(
          IList<TTarget> targetSetToCheck,
          IList<TTarget> eligibleTargets,
          IList<TTarget> allTargets,
          Card targetingCard,
          Game game,
          IList<Pair<IAction, IList<int>>> previousActions
        ) {
            foreach (TTarget target in allTargets) {
                bool isValidTarget = IsTargetValidInternal(
                  new List<TTarget>() { target }, 
                  targetingCard, 
                  game, 
                  previousActions
                );

                if (eligibleTargets.Contains(target) && (isValidTarget != targetSetToCheck.Contains(target))) {
                    return false;
                }
            }

            return true;
        }
    }
}