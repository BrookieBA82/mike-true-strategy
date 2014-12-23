using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame {

    // Refactor - (MT): Make this a generic choice block:
    // * Actions have "target selecting player" property
    // * Player somehow specifies targets in response to the action
    public interface IDiscardingStrategy : ICloneable {

        IList<Card> FindOptimalDiscardingStrategy(Game game, Player player, int cardsToDiscard);

    }
}