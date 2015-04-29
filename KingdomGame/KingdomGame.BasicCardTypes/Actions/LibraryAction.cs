using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;
using KingdomGame;

namespace KingdomGame.BasicCardTypes {

    public class LibraryDrawingAction : BasePlayerTargetAction {

        private LibraryDrawingAction() : base(BasePlayerTargetAction.PlayerTargetType.SELF, 1, 1) {

        }

        protected override void ApplyInternal(
          IList<Player> players, 
          Game game
        ) {
            // Todo - (MT): Check card counts in hand.

            // If action, add a discard choice here.  If not and can draw more, add another library action.
            /*
            foreach (Player player in players) {
                IAction forcedDiscardAction = ActionRegistry.Instance.GetActionByType(typeof(MilitiaForcedDiscardAction)).Create(player);
                game.State.AddPendingAction(forcedDiscardAction);
            }
            */
        }

        protected override bool IsTargetSetValidInternal(IList<Player> players, Game game) {
            // Todo - (MT): Ensure the player who plays this card is the singular target.
            return true;
        }
    }

    public class LibraryDiscardChoiceAction : BaseActionTargetAction {

        private LibraryDiscardChoiceAction() : base(1, 1) {

        }

        protected override void ApplyInternal(
          IList<IAction> actions, 
          Game game
        ) {
            // Todo - (MT): On yes, add discarding action.  On no, add library action.
        }

        protected override bool IsTargetSetValidInternal(IList<IAction> actions, Game game) {
            // Todo - (MT): Ensure exactly one of the two actions is targeted.
            return true;
        }

        // Todo - (MT): Pass object parameters into post construction.
        protected override void CreatePostProcess(Player targetSelector) {
            /*
            Debug.Assert(targetSelector != null, "Target selectors cannot be null.");
            int cardsToDiscard = (targetSelector.Hand.Count > 3) ? targetSelector.Hand.Count - 3 : 0;
            MinTargets = cardsToDiscard;
            MaxTargets = cardsToDiscard;
             */
        }
    }

    public class LibraryDiscardingAction : BaseCardTargetAction {
        
        private LibraryDiscardingAction() : base(CardOwnerTargetType.SELF, 1, 1) {

        }

        protected override void ApplyInternal(
          IList<Card> cards, 
          Game game
        ) {
            // Todo - (MT): When done, add another library action.
        }

        protected override bool IsTargetSetValidInternal(IList<Card> cards, Game game) {
            // Todo - (MT): Ensure the drawn card is the singular target.
            return true;
        }

        // Todo - (MT): Pass object parameters into post construction.
        protected override void CreatePostProcess(Player targetSelector) {
            /*
            Debug.Assert(targetSelector != null, "Target selectors cannot be null.");
            int cardsToDiscard = (targetSelector.Hand.Count > 3) ? targetSelector.Hand.Count - 3 : 0;
            MinTargets = cardsToDiscard;
            MaxTargets = cardsToDiscard;
             */
        }
    }
}
