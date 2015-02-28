using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame {

    public class Player : ICloneable, ICardOwner, ITargetable {

        #region Public Classes

        public class PlayerStrategy : ICloneable {

            #region Properties

            public ICardSelectionStrategy CardSelectionStrategy { get; set; }

            public ITargetSelectionStrategy TargetSelectionStrategy { get; set; }

            public IBuyingStrategy BuyingStrategy { get; set; }

            public IDiscardingStrategy DiscardingStrategy { get; set; }

            #endregion

            #region Public Methods

            public object Clone() {
                PlayerStrategy strategy = new PlayerStrategy();

                strategy.CardSelectionStrategy = CardSelectionStrategy.Clone() as ICardSelectionStrategy;
                strategy.TargetSelectionStrategy = TargetSelectionStrategy.Clone() as ITargetSelectionStrategy;
                strategy.BuyingStrategy = BuyingStrategy.Clone() as IBuyingStrategy;
                strategy.DiscardingStrategy = DiscardingStrategy.Clone() as IDiscardingStrategy;
                
                return strategy;
            }

            public override bool Equals(object obj) {
                PlayerStrategy strategy = obj as PlayerStrategy;
                if (strategy == null) {
                    return false;
                }

                return CardSelectionStrategy.Equals(strategy.CardSelectionStrategy) 
                  && TargetSelectionStrategy.Equals(strategy.TargetSelectionStrategy) 
                  && BuyingStrategy.Equals(strategy.BuyingStrategy)
                  && DiscardingStrategy.Equals(strategy.DiscardingStrategy);
            }

            public override int GetHashCode() {
                int code = CardSelectionStrategy.GetHashCode() 
                  ^ TargetSelectionStrategy.GetHashCode() 
                  ^ BuyingStrategy.GetHashCode()
                  ^ DiscardingStrategy.GetHashCode();

                return code;
            }

            #endregion

        }

        #endregion

        #region Constants

        public const int DEFAULT_HAND_SIZE = 5;

        #endregion

        #region Static Members

        private static int NextId = 1;

        #endregion

        #region Private Members

        private int _id;
        private string _name;

        private int _remainingBuys;
        private int _remainingActions;
        private int _remainingMoney;

        private Deck _hand;
        private Deck _deck;
        private Deck _discard;
        private Deck _playArea;

        private PlayerStrategy _strategy;

        #endregion

        #region Constructors

        private Player(int id, string name) {
            _id = id;
            _name = name;

            _remainingBuys = 0;
            _remainingActions = 0;
            _remainingMoney = 0;

            _hand = new Deck();
            _deck = new Deck();
            _discard = new Deck();
            _playArea = new Deck();

            _strategy = new PlayerStrategy();
            // Todo - (MT): Strategy point #1 - select best card (instead of random)
            _strategy.CardSelectionStrategy = new RandomCardSelectionStrategy();
            // Todo - (MT): Strategy point #2 - select best (or at least random) target
            _strategy.TargetSelectionStrategy = new RandomTargetSelectionStrategy();
            // Todo - (MT): Strategy point #3 - select best buy option (not just random)
            _strategy.BuyingStrategy = new RandomBuyingStrategy();
            // Todo - (MT): Strategy point #4 - select best discard option (not just random)
            _strategy.DiscardingStrategy = new RandomDiscardingStrategy();
        }

        public Player(string name) : this(Player.NextId++, name) {

        }

        private Player(Player toClone) : this(toClone.Id, toClone.Name) {
            this._remainingBuys = toClone._remainingBuys;
            this._remainingActions = toClone._remainingActions;
            this._remainingMoney = toClone._remainingMoney;

            this._hand = toClone._hand.Clone() as Deck;
            this._deck = toClone._deck.Clone() as Deck;
            this._discard = toClone._discard.Clone() as Deck;
            this._playArea = toClone._playArea.Clone() as Deck;

            this._strategy = toClone._strategy.Clone() as PlayerStrategy;
        }

        #endregion

        #region Properties

        public int Id {
            get { return _id; }
        }

        public string Name {
            get { return _name; }
        }

        public IList<Card> Hand {
            get { return _hand.Cards; }
        }

        public IList<Card> Deck {
            get { return _deck.Cards; }
        }

        public IList<Card> Discard {
            get { return _discard.Cards; }
        }

        public IList<Card> PlayArea {
            get { return _playArea.Cards; }
        }

        public int RemainingBuys {
            get { return _remainingBuys; }
            set { _remainingBuys = value; }
        }

        public int RemainingActions {
            get { return _remainingActions; }
            set { _remainingActions = value; }
        }

        public int RemainingMoney {
            get { return _remainingMoney; }
            set { _remainingMoney = value; }
        }

        public int Score {
            get {

                int score = 0;

                foreach (Card card in Hand) {
                    score += card.Score;
                }

                foreach (Card card in Deck)
                {
                    score += card.Score;
                }

                foreach (Card card in Discard)
                {
                    score += card.Score;
                }

                foreach (Card card in PlayArea)
                {
                    score += card.Score;
                }

                return score;
            }
        }

        public PlayerStrategy Strategy { get { return _strategy; } }

        public GameHistory.TargetInfo TargetInfo {
            get { return new GameHistory.PlayerInfo(this); }
        }

        #endregion

        #region Public Methods

        #region Turn Management Methods

        public void StartTurn() {
            _remainingBuys = 1;
            _remainingActions = 1;
        }

        public void EndTurn() {
            DiscardAll();
            _discard.AddRange(_playArea.RemoveRange(_playArea.Cards));

            _remainingBuys = 0;
            _remainingActions = 0;
            _remainingMoney = 0;

            DrawHand();
        }

        #endregion

        #region Card Management Methods

        public int Draw() {
            return this.Draw(1);
        }

        public int Draw(int cards) {
            int drawn;

            for (drawn = 0; drawn < cards; drawn++) {
                if(!this.DrawOne()) {
                    break;
                }
            }

            return drawn;
        }

        public int DrawHand() {
            return this.Draw(Player.DEFAULT_HAND_SIZE);
        }

        public Card PlayCard(Card card) {

            if (!_hand.Contains(card)) {
                return null;
            }

            if (_remainingActions == 0) {
                return null;
            }

            _remainingActions--;
            _hand.Remove(card);
            _playArea.Add(card);

            return card;
        }

        public Card DiscardCard(Card card) {

            if (!_hand.Contains(card)) {
                return null;
            }

            _hand.Remove(card);
            _discard.Add(card);
            _remainingMoney -= card.Value;

            return card;
        }

        public int DiscardAll() {
            int discarded = 0;

            while (_hand.Size > 0) {
                if (this.DiscardOne() != null) {
                    discarded++;
                }
            }

            return discarded;
        }

        public void AcquireCard(Card card, CardDestination destination) {
            Deck deck = null;
            switch (destination) {
                case CardDestination.PLAY_AREA:
                    deck = _playArea;
                    break;

                case CardDestination.DISCARD:
                    deck = _discard;
                    break;

                case CardDestination.HAND:
                    deck = _hand;
                    RemainingMoney += card.Value;
                    break;

                case CardDestination.DECK:
                    deck = _deck;
                    break;

                default:
                    break;
            }

            if (deck != null) {
                deck.Add(card);
            }
        }

        public bool ReleaseCard(Card card) {
            Deck deck = null;
            if (_deck.Contains(card)) {
                deck = _deck;
            } else if (_discard.Contains(card)) {
                deck = _discard;
            } else if (_hand.Contains(card)) {
                deck = _hand;
                RemainingMoney -= card.Value;
            } else if (_playArea.Contains(card)) {
                deck = _playArea;
            }

            if (deck == null) {
                return false;
            }

            deck.Remove(card);
            return true;
        }

        public void ShuffleDeck() {
            _deck.Shuffle();
        }

        #endregion

        #region Clone and Equality Methods

        public override string ToString() {
            return Name;
        }

        public object Clone() {
            return new Player(this);
        }

        public override bool Equals(object obj) {
            Player player = obj as Player;
            if (player == null) {
                return false;
            }

            return (this.Id == player.Id)
              && (this.Name.ToLower().Equals(player.Name.ToLower()))
              && (this.RemainingActions == player.RemainingActions)
              && (this.RemainingBuys == player.RemainingBuys)
              && (this.RemainingMoney == player.RemainingMoney)
              && (this._deck.Equals(player._deck))
              && (this._discard.Equals(player._discard))
              && (this._hand.Equals(player._hand))
              && (this._playArea.Equals(player._playArea))
              && (this._strategy.Equals(player._strategy));
        }

        public override int GetHashCode() {
            return this.Id.GetHashCode()
              ^ this.Name.ToLower().GetHashCode()
              ^ this.RemainingActions.GetHashCode()
              ^ this.RemainingBuys.GetHashCode()
              ^ this.RemainingMoney.GetHashCode()
              ^ this._deck.GetHashCode()
              ^ this._discard.GetHashCode()
              ^ this._hand.GetHashCode()
              ^ this._playArea.GetHashCode()
              ^ this._strategy.GetHashCode();
        }


        public string ToString(string format) {
            return string.Format("{0} ({1})", Name,  Id);
        }

        #endregion

        #endregion

        #region Utility Methods

        private bool DrawOne() {

            if (_deck.Size == 0) {
                _discard.Shuffle();
                _deck.AddRange(_discard.RemoveRangeAt(0, _discard.Size));
            }

            if (_deck.Size > 0) {
                Card drawn = _deck[0];
                _hand.Add(drawn);
                _deck.RemoveAt(0);
                _remainingMoney += drawn.Value;

                return true;
            }

            return false;
        }

        private Card DiscardOne() {
            return this.DiscardOne(true);
        }

        private Card DiscardOne(bool random) {

            if (_hand.Size == 0) {
                return null;
            }

            int discardIndex = random ? RandomNumberManager.Next(_hand.Size - 1) : 0;
            Card discardedCard = _hand[discardIndex];
            
            _hand.Remove(discardedCard);
            _discard.Add(discardedCard);
            _remainingMoney -= discardedCard.Value;

            return discardedCard;
        }

        #endregion
    }
}
