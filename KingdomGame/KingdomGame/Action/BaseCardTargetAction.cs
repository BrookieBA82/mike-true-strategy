using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace KingdomGame {

    public abstract class BaseCardTargetAction : BaseAction<Card> {

        #region Enums

        public enum CardOwnerTargetType {
            NONE = 0,
            SELF = 1,
            OTHER = 2,
            GAME = 4,
            TRASH = 8,
            ANY = 15
        }

        #endregion

        #region Private Members

        private CardOwnerTargetType _cardOwnerTargetType;

        #endregion

        #region Constructors

        public BaseCardTargetAction (
          CardOwnerTargetType cardOwnerTargetType, 
          int minTargets, 
          int maxTargets
        ) : base(minTargets, maxTargets, false, false) {
            _cardOwnerTargetType = cardOwnerTargetType;
        }

        #endregion

        #region Public Methods

        public override bool Equals(object obj) {
            BaseCardTargetAction action = obj as BaseCardTargetAction;
            if (action == null) {
                return false;
            }

            return base.Equals(action) && _cardOwnerTargetType == action._cardOwnerTargetType;
        }

        public override int GetHashCode() {
            return base.GetHashCode() ^ _cardOwnerTargetType.GetHashCode();
        }

        #endregion

        #region Protected Methods

        // Todo - (MT): Support card property targeting as an AND relationship with card owner.
        protected override bool IsIndividualTargetValidTypedBase(Card target, Card targetingCard, Game game) {
            Player targetSelector = GetTargetSelector(game);
            if(((_cardOwnerTargetType & CardOwnerTargetType.SELF) == CardOwnerTargetType.NONE) 
              && target.OwnerId.HasValue 
              && target.OwnerId.Value == targetSelector.Id) {
                return false;
            }
            else if(((_cardOwnerTargetType & CardOwnerTargetType.OTHER) == CardOwnerTargetType.NONE) 
              && target.OwnerId.HasValue 
              && target.OwnerId.Value != targetSelector.Id) {
                return false;
            }
            else if(((_cardOwnerTargetType & CardOwnerTargetType.GAME) == CardOwnerTargetType.NONE) 
              && !target.OwnerId.HasValue
              && !target.IsTrashed) {
                return false;
            }
            else if(((_cardOwnerTargetType & CardOwnerTargetType.TRASH) == CardOwnerTargetType.NONE)
              && !target.OwnerId.HasValue
              && target.IsTrashed) {
                return false;
            }

            return true;
        }

        protected override IList<ITargetable> GetAllPossibleIndividualTargetsTypedBase(Game game) {
            return new List<ITargetable>(game.Cards);
        }

        #endregion

    }
}
