using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame {

    public interface ICardSelectionStrategy : ICloneable {

        Card FindOptimalCardSelectionStrategy(Game game, Deck currentHand);

    }
}