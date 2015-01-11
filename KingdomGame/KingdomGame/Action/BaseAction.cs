using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace KingdomGame {

    // Refactor - (MT): Make actions themselves ITargetable (with IDs).
    // Refactor - (MT): Consider a base class for making a general choice (between actions).
    public abstract class BaseAction<TTarget> : IAction where TTarget : class, ITargetable {

        protected Type _targetType;
        protected int _minTargets;
        protected int _maxTargets;
        protected int? _targetSelectorId = null;
        protected bool _allValidTargetsRequired;

        protected bool _duplicateTargetsAllowed;

        protected string _displayName;
        protected string _actionDescription;

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
          Game game
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
                      game
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
                  game
                )) {
                    return false;
                }

                return IsTargetValidInternal(typedTargetSet, targetingCard, game);
            }

            return false;
        }

        public void Apply(
          IList<ITargetable> targetSet,
          Game game
        ) {
            IList<TTarget> typedTargetSet = new List<TTarget>();
            foreach (ITargetable target in targetSet) {
                if(target is TTarget) {
                    typedTargetSet.Add(target as TTarget);
                }
                else {
                    throw new InvalidOperationException(string.Format(
                    "Cannot apply this action to a target set containing {0} because it can only be applied to {1}.",
                        target.GetType().ToString(),
                        _targetType.ToString()
                    ));
                }
            }

            ApplyInternal(typedTargetSet, game);
        }

        public IList<ITargetable> GetAllValidTargets(
          Card targetingCard, 
          Game game
        ) {
            IList<ITargetable> validTargets = new List<ITargetable>();
            foreach (ITargetable target in GetAllPossibleTargets<ITargetable>(game)) {
                if (IsTargetValid<ITargetable>(new List<ITargetable>() { target }, targetingCard, game)) {
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

        public Type TargetType { get { return _targetType; } }

        public int? TargetSelectorId { get { return _targetSelectorId; } }

        public bool AllValidTargetsRequired { get { return _allValidTargetsRequired; } }

        public string DisplayName { 
            get {return (!string.IsNullOrEmpty(_displayName)) ? _displayName : GetType().Name; }
            set { _displayName = value; }
        }

        public string ActionDescription { 
            get {return (!string.IsNullOrEmpty(_actionDescription)) ? _actionDescription : GetType().Name; }
            set { _actionDescription = value; }
        }

        public IAction Create(Player targetSelector) {
            if (targetSelector == null) {
                throw new ArgumentNullException("A valid player must be specified as the target selector for an action.");
            }

            BaseAction<TTarget> action = this.MemberwiseClone() as BaseAction<TTarget>;
            action._targetSelectorId = targetSelector.Id;
            return action;
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
              && _targetSelectorId.HasValue == action._targetSelectorId.HasValue
              && (!_targetSelectorId.HasValue || (_targetSelectorId.Value == action._targetSelectorId.Value));
        }

        public override int GetHashCode() {
            return GetType().GetHashCode() 
              ^ _minTargets.GetHashCode()
              ^ _maxTargets.GetHashCode()
              ^ _duplicateTargetsAllowed.GetHashCode()
              ^ _allValidTargetsRequired.GetHashCode()
              ^ ((_targetSelectorId.HasValue) ? _targetSelectorId.Value.GetHashCode() : false.GetHashCode());
        }

        protected abstract void ApplyInternal(
          IList<TTarget> target, 
          Game game
        );

        protected abstract IList<TTarget> GetAllPossibleTargetsBase(Game game);

        protected virtual bool IsTargetValidBase(
          TTarget target,
          Card targetingCard,
          Game game
        ) {
            return true;
        }

        protected virtual bool IsTargetValidInternal(
          IList<TTarget> targets, 
          Card targetingCard,
          Game game
        ) {
            return true;
        }

        private bool AreAllValidTargetsIncluded(
          IList<TTarget> targetSetToCheck,
          IList<TTarget> eligibleTargets,
          IList<TTarget> allTargets,
          Card targetingCard,
          Game game
        ) {
            foreach (TTarget target in allTargets) {
                bool isValidTarget = IsTargetValidInternal(
                  new List<TTarget>() { target }, 
                  targetingCard, 
                  game
                );

                if (eligibleTargets.Contains(target) && (isValidTarget != targetSetToCheck.Contains(target))) {
                    return false;
                }
            }

            return true;
        }
    }
}