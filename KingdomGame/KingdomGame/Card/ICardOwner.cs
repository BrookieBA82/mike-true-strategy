using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame {

    public enum CardDestination {
        DISCARD = 1,
        PLAY_AREA = 2,
        HAND = 3,
        DECK = 4,
        TRASH = 5,
        GAME = 6
    }

    public interface ICardOwner {

        void AcquireCard(Card card, CardDestination destination);

        bool ReleaseCard(Card card);

    }
}
