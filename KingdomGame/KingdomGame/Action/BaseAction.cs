using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace KingdomGame {

    // Refactor - (MT): Make actions themselves ITargetable (with IDs).
    // Refactor - (MT): Consider a base class for making a general choice (between actions).
    public abstract class BaseAction<TTarget> : IAction where TTarget : class, ITargetable {

        #region Private Members

        private int? _targetSelectorId = null;

        private int _minTargets;
        private int _maxTargets;
        private bool _duplicateTargetsAllowed;
        private bool _allValidTargetsRequired;

        private bool _lockedForUpdates;

        private string _displayName;
        private string _actionDescription;

        #endregion

        #region Constructors

        public BaseAction (int minTargets, int maxTargets, bool duplicateTargetsAllowed, bool allValidTargetsRequired) {
            if (minTargets > maxTargets) {
                throw new ArgumentException("Cannot specify a minimum number of targets larger than the maximum.");
            }

            _minTargets = minTargets;
            _maxTargets = maxTargets;
            _duplicateTargetsAllowed = duplicateTargetsAllowed;
            _allValidTargetsRequired = allValidTargetsRequired;

            _lockedForUpdates = false;
        }

        #endregion

        #region Properties

        public int? TargetSelectorId { get { return _targetSelectorId; } }

        public int MinTargets { 
            get { return _minTargets; }
            protected set { 
                Debug.Assert(!_lockedForUpdates, "Action properties should not be altered once Create has been called.");
                _minTargets = value; 
            }
        }

        public int MaxTargets { 
            get { return _maxTargets; }
            protected set {
                Debug.Assert(!_lockedForUpdates, "Action properties should not be altered once Create has been called.");
                _maxTargets = value; 
            }
        }

        public bool AllValidTargetsRequired { get { return _allValidTargetsRequired; } }

        public string DisplayName { 
            get {return (!string.IsNullOrEmpty(_displayName)) ? _displayName : GetType().Name; }
            set { 
                Debug.Assert(!_lockedForUpdates, "Action properties should not be altered once Create has been called.");
                _displayName = value; 
            }
        }

        public string ActionDescription { 
            get {return (!string.IsNullOrEmpty(_actionDescription)) ? _actionDescription : GetType().Name; }
            set { 
                Debug.Assert(!_lockedForUpdates, "Action properties should not be altered once Create has been called.");
                _actionDescription = value; 
            }
        }

        #endregion

        #region Public Methods

        #region Action Interface Methods

        public IAction Create(Player targetSelector) {
            if (targetSelector == null) {
                throw new ArgumentNullException("A valid player must be specified as the target selector for an action.");
            }

            BaseAction<TTarget> action = this.MemberwiseClone() as BaseAction<TTarget>;
            action._targetSelectorId = targetSelector.Id;
            action.CreatePostProcess(targetSelector);

            action._lockedForUpdates = true;

            return action;
        }

        public bool IsTargetSetValid(IList<ITargetable> targetSet, Card targetingCard, Game game) {
            if (targetSet == null) {
                return false;
            }

            if (GetTypedTargetList(targetSet).Contains(null)) {
                return false;
            }

            if ((targetSet.Count < _minTargets) || (targetSet.Count > _maxTargets)) {
                return false;
            }

            if (!_duplicateTargetsAllowed && (GetUniqueTargetList(targetSet).Count < targetSet.Count)) {
                return false;
            }

            if(_allValidTargetsRequired && !AreAllValidTargetsIncluded(targetSet, targetingCard, game)) {
                return false;
            }

            return IsTargetSetValidSubclass(targetSet, targetingCard, game);
        }

        public IList<ITargetable> GetAllValidIndividualTargets(
          Card targetingCard, 
          Game game
        ) {
            IList<ITargetable> validTargets = new List<ITargetable>();
            foreach (ITargetable target in GetAllPossibleIndividualTargetsTypedBase(game)) {
                if (IsIndividualTargetValid(target as TTarget, targetingCard, game)) {
                    validTargets.Add(target);
                }
            }

            return validTargets;
        }

        public void Apply(IList<ITargetable> targetSet, Game game) {
            if (IsTargetSetValid(targetSet, game.State.SelectedPlay, game)) {
                ApplyInternal(GetTypedTargetList(targetSet), game);
            }
            else {
                throw new InvalidOperationException("Cannot apply this action to an invalid target set.");
            }
        }

        #endregion

        #region Equality Methods

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

        #endregion

        #endregion

        #region Protected Methods

        #region Abstract Methods

        protected abstract void ApplyInternal(IList<TTarget> targetSet, Game game);

        protected abstract IList<ITargetable> GetAllPossibleIndividualTargetsTypedBase(Game game);

        #endregion

        #region Virtual Methods

        protected virtual void CreatePostProcess(Player targetSelector) {

        }

        protected virtual bool IsIndividualTargetValidTypedBase(TTarget targetSet, Card targetingCard, Game game) {
            return true;
        }

        protected virtual bool IsTargetSetValidInternal(IList<TTarget> targetSet, Card targetingCard, Game game) {
            return true;
        }

        protected virtual bool IsIndividualTargetValid(TTarget target, Card targetingCard, Game game) {
            return IsIndividualTargetValidSubclass(target, targetingCard, game);
        }

        #endregion

        #region Utility Methods

        protected Player GetTargetSelector(Game game) {
            Debug.Assert(_targetSelectorId.HasValue, "Actions should always have a target selector specified.");
            Player targetSelector = game.GetPlayerById(_targetSelectorId.Value);
            Debug.Assert(targetSelector != null, "Target selectors should always be a valid player.");
            return targetSelector;
        }

        #endregion

        #endregion

        #region Utility Methods

        #region Target Validity Methods

        private bool AreAllValidTargetsIncluded(IList<ITargetable> targetSet, Card targetingCard, Game game) {
            List<ITargetable> eligibleTargets = new List<ITargetable>(GetAllValidIndividualTargets(targetingCard, game));
            foreach (ITargetable eligibleTarget in eligibleTargets) {
                if (!targetSet.Contains(eligibleTarget)) {
                    return false;
                }
            }

            return true;
        }

        private bool IsIndividualTargetValidSubclass(ITargetable target, Card targetingCard, Game game) {
            return IsTargetSetValidInternal(new List<TTarget>() { target as TTarget }, targetingCard, game)
                && IsIndividualTargetValidTypedBase(target as TTarget, targetingCard, game);
        }

        private bool IsTargetSetValidSubclass(IList<ITargetable> targetSet, Card targetingCard, Game game) {
            List<TTarget> typedTargetSet = GetTypedTargetList(targetSet);
            return IsTargetSetValidInternal(typedTargetSet, targetingCard, game)
                && !typedTargetSet.Exists(delegate(TTarget target) { return !IsIndividualTargetValidTypedBase(target, targetingCard, game); });
        }

        #endregion

        #region List Methods

        private static List<TTarget> GetTypedTargetList(IList<ITargetable> untypedList) {
            return new List<ITargetable>(untypedList).ConvertAll<TTarget>(delegate(ITargetable target) { return target as TTarget; });
        }

        private static List<TTarget> GetUniqueTargetList(IList<ITargetable> untypedList) {
            return new List<TTarget>(GetTypedTargetList(untypedList).Distinct<TTarget>());
        }

        #endregion

        #endregion
    }
}