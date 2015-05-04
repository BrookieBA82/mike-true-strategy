using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame.Test {

    public class CardPlayedAssertion : ITestAssertion {

        public delegate Card SelectCardDelegate(Game game);

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

        public void Assert(Game game) {
            TestUtilities.ConfirmCardPlayed(game, _cardLocator(game), _expectedActionsRemaining, _additionalCardsPlayed, Description);
        }
    }
}
