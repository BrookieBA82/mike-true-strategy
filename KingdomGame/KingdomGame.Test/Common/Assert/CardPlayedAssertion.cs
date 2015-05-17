using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame.Test {

    public class CardPlayedAssertion : ITestAssertion {

        public delegate Card SelectCardDelegate(Game game);

        private Card _card = null;

        private string _key;
        private int _expectedActionsRemaining;
        private SelectCardDelegate _cardLocator;
        private IDictionary<CardType, int> _additionalCardsPlayed;

        public CardPlayedAssertion(
            string key,
            SelectCardDelegate cardLocator, 
            int expectedActionsRemaining = 0, 
            IDictionary<CardType, int> additionalCardsPlayed = null
        ) {
            _key = key;
            _cardLocator = cardLocator;
            _expectedActionsRemaining = expectedActionsRemaining;
            _additionalCardsPlayed = additionalCardsPlayed ?? new Dictionary<CardType, int>();
        }

        public string Key { get { return string.Format("CardPlayed-{0}", _key); } }

        public string Description { get; set; }

        public void Bind(Game game) {
            Debug.Assert(_card == null, "An assertion shouldn't be bound to a game more than once.");
            _card = _cardLocator(game);
        }

        public void Assert(Game game) {
            Debug.Assert(_card != null, "An assertion shouldn't checked until it is bound.");
            TestUtilities.ConfirmCardPlayed(game, _card, _expectedActionsRemaining, _additionalCardsPlayed, Description);
        }
    }
}
