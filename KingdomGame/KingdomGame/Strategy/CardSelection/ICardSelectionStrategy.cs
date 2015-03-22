using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame {

    public interface IPlaySelectionStrategy : ICloneable {

        Card SelectPlay(Game game, Deck currentHand);

    }
}