using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace KingdomGame {

    public abstract class BasePlayerTargetAction : BaseAction<Player> {

        #region Enums

        public enum PlayerTargetType {
            NONE = 0,
            SELF = 1,
            OTHER = 2,
            ANY = 3
        }

        #endregion

        #region Private Members

        private PlayerTargetType _playerTargetType;

        #endregion

        #region Constructors

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

        #endregion

        #region Public Methods

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

        #endregion

        #region Protected Methods

        protected override bool IsIndividualTargetValidTypedBase(Player target, Card targetingCard, Game game) {
            if(((_playerTargetType & PlayerTargetType.SELF) == PlayerTargetType.NONE) 
              && target.Id == GetTargetSelector(game).Id) {
                return false;
            } 
            else if(((_playerTargetType & PlayerTargetType.OTHER) == PlayerTargetType.NONE) 
              && target.Id != GetTargetSelector(game).Id) {
                return false;
            }

            return true;
        }

        protected override IList<ITargetable> GetAllPossibleIndividualTargetsTypedBase(Game game) {
            return new List<ITargetable>(game.Players);
        }

        #endregion

    }
}
