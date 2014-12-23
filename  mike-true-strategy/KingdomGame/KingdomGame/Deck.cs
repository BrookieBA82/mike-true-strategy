using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame {

    public class Deck : ICloneable {

        private List<Card> _orderedDeck;
        private Dictionary<int, Card> _cardIndex;
        private Dictionary<int, IList<Card>> _cardIndexByCardType;

        public Deck() {
            _orderedDeck = new List<Card>();
            _cardIndex = new Dictionary<int, Card>();
            _cardIndexByCardType = new Dictionary<int, IList<Card>>();
        }

        public Deck(IList<Card> cards) : this() {
            AddRange(cards);
        }

        private Deck(Deck toClone)
            : this((toClone._orderedDeck as List<Card>).ConvertAll<Card>(delegate(Card card) { return (card.Clone() as Card); })) {
        }

        public int Size {
            get { return _orderedDeck.Count; }
        }

        public IList<Card> Cards {
            get { return new List<Card>(_orderedDeck); }
        }

        public void Add(Card card) {
            AddAt(card, Size);
        }

        public void AddAt(Card card, int index) {
            if (card == null) {
                throw new ArgumentNullException("Cannot insert a null card into a deck.");
            }

            if (index < 0 && index > _orderedDeck.Count) {
                throw new ArgumentOutOfRangeException(
                    "Index at which to add a card to a deck must be between zero and its size, inclusive.");
            }

            int existingIndex = IndexOf(card);
            if (existingIndex >= 0 && index < _orderedDeck.Count) {
                throw new ArgumentException("A card cannot be added to a deck if it already belongs to it.");
            }

            _orderedDeck.Insert(index, card);
            _cardIndex[card.Id] = card;

            if (!_cardIndexByCardType.ContainsKey(card.Type.Id)) {
                _cardIndexByCardType[card.Type.Id] = new List<Card>();
            }
            _cardIndexByCardType[card.Type.Id].Add(card);
        }

        public void AddRange(IList<Card> cards) {
            AddRangeAt(cards, Size);
        }

        public void AddRangeAt(IList<Card> cards, int index) {
            for (int offset = 0; offset < cards.Count; offset++) {
                AddAt(cards[offset], index + offset);
            }
        }

        public object Clone() {
            return new Deck(this);
        }

        public bool Contains(Card card) {
            return _cardIndex.ContainsKey(card.Id);
        }

        public Card this[int index] {
            get { return _orderedDeck[index]; }
        }

        public IList<Card> GetCardsByCardType(CardType type) {
            if (!_cardIndexByCardType.ContainsKey(type.Id)) {
                return new List<Card>();
            }

            return new List<Card>(_cardIndexByCardType[type.Id]);
        }

        public int IndexOf(Card card) {
            return _orderedDeck.IndexOf(card);
        }

        public Card Peek() {
            if (_orderedDeck.Count == 0) {
                return null;
            }

            return _orderedDeck[0];
        }

        public Card Remove(Card card) {
            if (!Contains(card)) {
                return null;
            }

            return RemoveAt(IndexOf(card));
        }

        public Card RemoveAt(int index) {
            if (index < 0 && index >= _orderedDeck.Count) {
                throw new ArgumentOutOfRangeException(
                    "Index to remove from a deck must be between zero (inclusive) and its size (exclusive).");
            }

            Card removed = _orderedDeck[index];

            _orderedDeck.RemoveAt(index);
            _cardIndex.Remove(removed.Id);
            _cardIndexByCardType[removed.Type.Id].Remove(removed);

            return removed;
        }

        public IList<Card> RemoveRange(IList<Card> cards) {
            List<Card> removedCards = new List<Card>();

            foreach (Card card in cards) {
                Card removedCard = Remove(card);
                if (card != null) {
                    removedCards.Add(removedCard);
                }
            }

            return removedCards;
        }

        public IList<Card> RemoveRangeAt(int index, int count) {
            List<Card> removedCards = new List<Card>();

            for (int offset = 0; offset < count; offset++) {
                Card removedCard = RemoveAt(index);
                removedCards.Add(removedCard);
            }

            return removedCards;
        }

        public void Shuffle() {
            for (int fillIndex = Size - 1; fillIndex > 0; fillIndex--) {
                int swapIndex = RandomNumberManager.Next(fillIndex + 1);
                Card card = _orderedDeck[swapIndex];
                _orderedDeck[swapIndex] = _orderedDeck[fillIndex];
                _orderedDeck[fillIndex] = card;
            }
        }

        public override bool Equals(object obj) {
            Deck deck = obj as Deck;
            if (deck == null) {
                return false;
            }

            if (this._orderedDeck.Count != deck._orderedDeck.Count) {
                return false;
            }

            for (int deckIndex = 0; deckIndex < this._orderedDeck.Count; deckIndex++) {
                if (!this._orderedDeck[deckIndex].Equals(deck._orderedDeck[deckIndex])) {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode() {
            int code = this._orderedDeck.Count.GetHashCode();

            for (int deckIndex = 0; deckIndex < this._orderedDeck.Count; deckIndex++) {
                code = code ^ this._orderedDeck[deckIndex].GetHashCode();
            }
            
            return code;
        }
    }
}
