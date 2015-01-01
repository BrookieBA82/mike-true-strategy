using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace KingdomGame {

    public abstract class BasePlayerTargetAction : BaseAction<Player> {

        public enum PlayerTargetType {
            NONE = 0,
            SELF = 1,
            OTHER = 2,
            ANY = 3
        }

        protected PlayerTargetType _playerTargetType;

        public BasePlayerTargetAction (
          PlayerTargetType playerTargetType, 
          int minTargets, 
          int maxTargets
        ) : this(playerTargetType, minTargets, maxTargets, false) {
        }

        public BasePlayerTargetAction (
          PlayerTargetType playerTargetType, 
          int minTargets, 
          int maxTargets,
          bool allValidTargetsRequired
        ) : base(minTargets, maxTargets, false, allValidTargetsRequired) {
            _playerTargetType = playerTargetType;
        }

        protected override bool IsTargetValidBase(
          Player target,
          Card targetingCard,
          Game game
        ) {
            if(((_playerTargetType & PlayerTargetType.SELF) == PlayerTargetType.NONE) 
              && target.Id == targetingCard.OwnerId) {
                return false;
            } 
            else if(((_playerTargetType & PlayerTargetType.OTHER) == PlayerTargetType.NONE) 
              && target.Id != targetingCard.OwnerId) {
                return false;
            }

            return true;
        }

        protected override IList<Player> GetAllPossibleTargetsBase(Game game) {
            return game.Players;
        }

        public override bool Equals(object obj) {
            BasePlayerTargetAction action = obj as BasePlayerTargetAction;
            if (action == null) {
                return false;
            }

            return base.Equals(action) && _playerTargetType == action._playerTargetType;
        }

        public override int GetHashCode() {
            return base.GetHashCode() ^ _playerTargetType.GetHashCode();
        }
    }
}
