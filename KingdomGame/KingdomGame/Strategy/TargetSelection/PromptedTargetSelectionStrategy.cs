using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame {

    public class TargetSelectionPromptEventArgs : EventArgs
    {
        public Game Game { get; set; }
        public Card CurrentPlay { get; set; }
        public IAction CurrentAction { get; set; }
        public IList<ITargetable> SelectedTargets { get; set; }
    }

    public delegate void TargetSelectionPromptEventHandler(Object sender, TargetSelectionPromptEventArgs args);

    public class PromptedTargetSelectionStrategy : ITargetSelectionStrategy {

        public event TargetSelectionPromptEventHandler TargetSelectionPromptRequired;

        public IList<ITargetable> SelectTargets(Game game, Card card, IAction action) {
            if (TargetSelectionPromptRequired != null) {
                TargetSelectionPromptEventArgs args = new TargetSelectionPromptEventArgs();
                args.Game = game;
                args.CurrentPlay = card;
                args.CurrentAction = action;
                TargetSelectionPromptRequired(this, args);
                IList<ITargetable> selectedTargets = args.SelectedTargets;
                return (action.IsTargetSetValid(selectedTargets, card, game)) ? selectedTargets : new List<ITargetable>();
            }
            else {
                return new RandomTargetSelectionStrategy().SelectTargets(game, card, action);
            }
        }

        public object Clone() {
            return new RandomTargetSelectionStrategy();
        }

        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }

            return this.GetType().Equals(obj.GetType());
        }

        public override int GetHashCode() {
            return this.GetType().GetHashCode();
        }
    }
}
