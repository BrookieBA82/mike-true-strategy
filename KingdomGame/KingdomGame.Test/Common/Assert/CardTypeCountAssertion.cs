using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame.Test {

    public class CardTypeCountAssertion : ITestAssertion {

        public delegate IList<Card> SelectDeckDelegate(Game game);

        private string _key;
        private string _location;
        private CardType _cardType;
        private int _expectedCount;
        private SelectDeckDelegate _deckLocator;

        public CardTypeCountAssertion(
            string key,
            SelectDeckDelegate deckLocator,
            string location,
            CardType cardType,
            int expectedCount
        ) {
            _key = key;
            _deckLocator = deckLocator;
            _location = location;
            _cardType = cardType;
            _expectedCount = expectedCount;
        }

        public string Key { get { return string.Format("CardTypeCount-{0}", _key); } }

        public string Description { get; set; }

        public void Bind(Game game) {

        }

        public void Assert(Game game) {
            TestUtilities.ConfirmCardTypeCount(_deckLocator(game), _cardType, _expectedCount, _location, Description);
        }
    }
}
