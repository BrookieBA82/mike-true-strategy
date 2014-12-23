using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace KingdomGame {

    public abstract class BaseCardTargetAction : BaseAction<Card> {

        public enum CardOwnerTargetType {
            NONE = 0,
            SELF = 1,
            OTHER = 2,
            GAME = 4,
            TRASH = 8,
            ANY = 15
        }

        protected CardOwnerTargetType _cardOwnerTargetType;

        public BaseCardTargetAction (
          CardOwnerTargetType cardOwnerTargetType, 
          int minTargets, 
          int maxTargets
        ) : this(cardOwnerTargetType, minTargets, maxTargets, false) {
        }

        public BaseCardTargetAction (
          CardOwnerTargetType cardOwnerTargetType, 
          int minTargets, 
          int maxTargets,
          bool allValidTargetsRequired
        ) : base(minTargets, maxTargets, false, allValidTargetsRequired) {
            _cardOwnerTargetType = cardOwnerTargetType;
        }

        // Todo - (MT): Support card property targeting as an AND relationship with card owner.
        protected override bool IsTargetValidBase(
          Card target,
          Card targetingCard,
          Game game,
          IList<Pair<IAction, IList<int>>> previousActions
        ) {
            if(((_cardOwnerTargetType & CardOwnerTargetType.SELF) == CardOwnerTargetType.NONE) 
              && target.OwnerId.HasValue 
              && target.OwnerId == targetingCard.OwnerId) {
                return false;
            }
            else if(((_cardOwnerTargetType & CardOwnerTargetType.OTHER) == CardOwnerTargetType.NONE) 
              && target.OwnerId.HasValue 
              && target.OwnerId != targetingCard.OwnerId) {
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

        protected override IList<Card> GetAllPossibleTargetsBase(Game game) {
            return game.Cards;
        }

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
    }
}
