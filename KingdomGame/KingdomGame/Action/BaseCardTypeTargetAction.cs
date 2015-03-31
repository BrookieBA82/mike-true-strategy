using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace KingdomGame {

    public abstract class BaseCardTypeTargetAction : BaseAction<CardType> {

        #region Constructors

        public BaseCardTypeTargetAction (
          int minTargets, 
          int maxTargets,
          bool duplicateTargetsAllowed
        ) : base(minTargets, maxTargets, duplicateTargetsAllowed, false) {

        }

        #endregion

        #region Public Methods

        public override bool Equals(object obj) {
            return (obj is BaseCardTypeTargetAction) && base.Equals(obj);
        }

        public override int GetHashCode() {
            return base.GetHashCode() ^ GetType().GetHashCode();
        }

        #endregion

        #region Protected Methods

        protected sealed override bool IsIndividualTargetValidTypedBase(CardType target, Game game) {
            return !(game.GetCardsByType(target) == null || game.GetCardsByType(target).Count == 0);
        }

        protected sealed override IList<ITargetable> GetAllPossibleIndividualTargets(Game game) {
            IList<CardType> types = new List<CardType>();
            foreach (CardType type in ActionRegistry.Instance.CardTypes) {
                IList<Card> cardsByType = game.GetCardsByType(type);
                if(cardsByType != null) {                    
                    types.Add(type);
                }
            }

            return new List<ITargetable>(types);
        }

        #endregion

    }
}