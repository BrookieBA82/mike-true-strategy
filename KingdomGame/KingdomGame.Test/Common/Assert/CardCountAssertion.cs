using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame.Test {

    public class CardCountAssertion : ITestAssertion {

        public delegate IList<Card> SelectDeckDelegate(Game game);

        private string _key;
        private string _location;
        private int _expectedCount;
        private SelectDeckDelegate _deckLocator;

        public CardCountAssertion(
            string key,
            SelectDeckDelegate deckLocator,
            string location,
            int expectedCount
        ) {
            _key = key;
            _deckLocator = deckLocator;
            _location = location;
            _expectedCount = expectedCount;
        }

        public string Key { get { return string.Format("CardCount-{0}", _key); } }

        public string Description { get; set; }

        public void Bind(Game game) {

        }

        public void Assert(Game game) {
            TestUtilities.ConfirmCardCount(_deckLocator(game), _expectedCount, _location, Description);
        }
    }
}
