using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace KingdomGame {

    public abstract class BaseActionTypeTargetAction : BaseAction<IAction> {

        #region Constructors

        public BaseActionTypeTargetAction (
          int minTargets, 
          int maxTargets
        ) : base(minTargets, maxTargets, false, false) {

        }

        #endregion

        #region Public Methods

        public override bool Equals(object obj) {
            return (obj is BaseActionTypeTargetAction) && base.Equals(obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode() ^ GetType().GetHashCode();
        }

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