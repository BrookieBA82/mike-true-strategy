using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace KingdomGame {

    public abstract class BaseActionTargetAction : BaseAction<IAction> {

        #region Constructors

        public BaseActionTargetAction (
          int minTargets, 
          int maxTargets
        ) : base(minTargets, maxTargets, false, false) {

        }

        #endregion

        #region Public Methods

        public override bool Equals(object obj) {
            return (obj is BaseActionTargetAction) && base.Equals(obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode() ^ GetType().GetHashCode();
        }

        // Todo - (MT): Override toString such that it can provide a user-friendy description of what this choice represents.

        #endregion

        #region Protected Methods

        protected sealed override bool IsIndividualTargetValidTypedBase(IAction target, Game game) {
            return GetAllPossibleIndividualTargets(game).Contains(target);
        }

        protected sealed override IList<ITargetable> GetAllPossibleIndividualTargets(Game game) {
            return new List<ITargetable>(game.State.PendingActions);
        }

        #endregion

    }
}