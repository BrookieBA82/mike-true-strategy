using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace KingdomGame {

    public abstract class BaseCardTypeTargetAction : BaseAction<CardType> {

        protected bool _requireTypesAvailableForAcquisition;

        public BaseCardTypeTargetAction (
          int minTargets, 
          int maxTargets,
          bool requireTypesAvailableForAcquisition
        ) : this(minTargets, maxTargets, requireTypesAvailableForAcquisition, true) {
        }

        public BaseCardTypeTargetAction (
          int minTargets, 
          int maxTargets,
          bool requireTypesAvailableForAcquisition,
          bool duplicateTargetsAllowed
        ) : base(minTargets, maxTargets, duplicateTargetsAllowed, false) {
            _requireTypesAvailableForAcquisition = requireTypesAvailableForAcquisition;
        }

        protected override bool IsTargetValidBase(
          CardType target,
          Card targetingCard,
          Game game
        ) {
            return !(_requireTypesAvailableForAcquisition 
              && (game.GetCardsByType(target) == null || game.GetCardsByType(target).Count == 0));
        }

        protected override IList<CardType> GetAllPossibleTargetsBase(Game game) {
            IList<CardType> types = new List<CardType>();
            foreach (CardType type in CardTypeRegistry.Instance.CardTypes) {
                IList<Card> cardsByType = game.GetCardsByType(type);
                if(cardsByType != null) {                    
                    types.Add(type);
                }
            }

            return types;
        }

        public override bool Equals(object obj) {
            BaseCardTypeTargetAction action = obj as BaseCardTypeTargetAction;
            if (action == null) {
                return false;
            }

            return base.Equals(action) 
              && _requireTypesAvailableForAcquisition == action._requireTypesAvailableForAcquisition;
        }

        public override int GetHashCode() {
            return base.GetHashCode() ^ _requireTypesAvailableForAcquisition.GetHashCode();
        }
    }
}
