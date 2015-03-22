using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame {

    public class PlaySelectionPromptEventArgs : EventArgs
    {
        public Game Game { get; set; }
        public Deck CurrentHand { get; set; }
        public Card SelectedPlay { get; set; }
    }

    public delegate void PlaySelectionPromptEventHandler(Object sender, PlaySelectionPromptEventArgs e);

    public class PromptedPlaySelectionStrategy : IPlaySelectionStrategy {

        public event PlaySelectionPromptEventHandler PlaySelectionPromptRequired;

        public Card SelectPlay(Game game, Deck currentHand) {
            if (PlaySelectionPromptRequired != null) {
                PlaySelectionPromptEventArgs args = new PlaySelectionPromptEventArgs();
                args.Game = game;
                args.CurrentHand = currentHand.Clone() as Deck;
                PlaySelectionPromptRequired(this, args);
                Card selectedPlay = args.SelectedPlay;
                return (selectedPlay != null && currentHand.Contains(selectedPlay)) ? selectedPlay : null;
            }
            else {
                return new RandomPlaySelectionStrategy().SelectPlay(game, currentHand);
            }
        }

        public object Clone() {
            return new PromptedPlaySelectionStrategy();
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
