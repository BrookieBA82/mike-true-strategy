using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KingdomGame {

    [XmlRoot("game")]
    public class GameHistory {
        // Todo - (MT): Accept both metadata and events as XML/JSON for serialization
        // Todo - (MT): Log selection strategies for better ability to back out decision making for improvement

        [XmlType("turn")]
        public class Turn {

            private List<Play> _plays;
            private List<ScoreInfo> _scores;

            [XmlAttribute("number")]
            public int Number { get; set; }

            // Todo - (MT): Add an element for the current player ("PlayerInfo")

            [XmlIgnore()]
            public Player Player { get; private set; }

            [XmlElement("player")]
            public PlayerInfo PlayerInfo {
                get { return new PlayerInfo(Player); }
                set { throw new NotSupportedException("History deserialization is not presently supported." ); } 
            }

            [XmlArray("initialHand")]
            public List<CardInfo> InitialHand { 
                get {
                    List<CardInfo> cards = new List<CardInfo>();
                    foreach (Card card in Player.Hand) {
                        cards.Add(new CardInfo(card));
                    }

                    return cards; 
                }

                set { throw new NotSupportedException("History deserialization is not presently supported." ); } 
            }

            [XmlArray("plays")]
            public List<Play> Plays { 
                get { return _plays; }
                set { throw new NotSupportedException("History deserialization is not presently supported." ); } 
            }

            [XmlArray("finalHand")]
            public List<CardInfo> FinalHand { 
                get {
                    List<CardInfo> cards = new List<CardInfo>();
                    if (Buy != null) {
                        foreach (Card card in Buy.Player.Hand) {
                            cards.Add(new CardInfo(card));
                        }
                    }

                    return cards; 
                }

                set { throw new NotSupportedException("History deserialization is not presently supported." );  } 
            }

            [XmlElement("buy")]
            public Buy Buy { get; set; }

            [XmlArray("scores")]
            public List<ScoreInfo> Scores { 
                get { return _scores; }
                set { throw new NotSupportedException("History deserialization is not presently supported." ); } 
            }

            public Turn() {

            }

            public Turn(Turn toClone) : this(toClone.Number, toClone.Player) {
                Buy = (toClone.Buy != null) ? new Buy(toClone.Buy) : null;
                foreach (Play play in toClone.Plays) {
                    _plays.Add(new Play(play));
                }

                foreach (ScoreInfo score in toClone.Scores) {
                    _scores.Add(new ScoreInfo(score));
                }
            }

            public Turn(int number, Player player) {
                Number = number;
                Player = player.Clone() as Player;
                _plays = new List<Play>();
                Buy = null;
                _scores = new List<ScoreInfo>();
            }
        }

        [XmlType("play")]
        public class Play {

            [XmlIgnore()]
            public Player Player { get; private set; }

            [XmlElement("cardPlayed")]
            public CardInfo Card { get; set; }

            [XmlArray("targets")]
            public List<Target> Targets { get; private set; }

            public Play() {

            }

            public Play(Play toClone) : this(toClone.Player, toClone.Card) {
                foreach (Target target in toClone.Targets) {
                    Targets.Add(new Target(target));
                }
            }

            public Play(Player player, Card card) : this (player, new CardInfo(card)) {

            }

            public Play(Player player, CardInfo card) {
                Player = player.Clone() as Player;
                Card = new CardInfo(card);
                Targets = new List<Target>();
            }
        }

        [XmlType("action")]
        public class Target {
            
            private IList<Player> _playerTargets;
            private IList<Card> _cardTargets;
            private IList<CardType> _typeTargets;

            [XmlIgnore()]
            public Player Player { get; private set; }

            [XmlIgnore()]
            public Card Card { get; private set; }

            [XmlIgnore()]
            public IAction Action { get; private set; }

            [XmlAttribute("name")]
            public string Name  { 
                get { return Action.DisplayName; }
                set { throw new NotSupportedException("History deserialization is not presently supported." ); } 
            }

            [XmlArray("cardTargets")]
            public List<CardInfo> CardTargets { 
                get {
                    List<CardInfo> cards = new List<CardInfo>();
                    foreach (Card card in _cardTargets) {
                        cards.Add(new CardInfo(card));
                    }

                    return cards; 
                }

                set { throw new NotSupportedException("History deserialization is not presently supported." ); } 
            }

            [XmlArray("playerTargets")]
            public List<PlayerInfo> PlayerTargets { 
                get {
                    List<PlayerInfo> players = new List<PlayerInfo>();
                    foreach (Player player in _playerTargets) {
                        players.Add(new PlayerInfo(player));
                    }

                    return players; 
                }

                set { throw new NotSupportedException("History deserialization is not presently supported." ); } 
            }

            [XmlArray("cardTypeTargets")]
            public List<CardTypeInfo> TypeTargets { 
                get {
                    List<CardTypeInfo> cardTypes = new List<CardTypeInfo>();
                    foreach (CardType type in _typeTargets) {
                        cardTypes.Add(new CardTypeInfo(type));
                    }

                    return cardTypes; 
                }

                set { throw new NotSupportedException("History deserialization is not presently supported." ); } 
            }

            public Target() {

            }

            public Target(Target toClone) : this(
              toClone.Player,
              toClone.Card,
              toClone.Action,
              new List<Player>(toClone._playerTargets).ConvertAll<Player>(delegate(Player player) { return player.Clone() as Player; }),
              new List<Card>(toClone._cardTargets),
              new List<CardType>(toClone._typeTargets)
            ) {

            }

            public Target(
              Player player, 
              Card card, 
              IAction action, 
              IList<Player> playerTargets, 
              IList<Card> cardTargets,
              IList<CardType> typeTargets
            ){
                Player = player.Clone() as Player;
                Card = card;
                Action = action;
                _playerTargets = playerTargets ?? new List<Player>();
                _cardTargets = cardTargets ?? new List<Card>();
                _typeTargets = typeTargets ?? new List<CardType>();
            }

            internal void SetTargets(List<Player> playerTargets, List<Card> cardTargets) {
                _playerTargets = playerTargets;
                _cardTargets = cardTargets;
            }
        }

        public class Buy {
            
            [XmlIgnore()]
            public Player Player { get; private set; }

            [XmlIgnore()]
            public IList<Card> Cards { get; private set; }

            [XmlArray("cardsBought")]
            public List<CardInfo> CardsBought { 
                get {
                    List<CardInfo> cards = new List<CardInfo>();
                    foreach (Card card in Cards) {
                        cards.Add(new CardInfo(card));
                    }

                    return cards; 
                }

                set { throw new NotSupportedException("History deserialization is not presently supported." ); } 
            }

            public Buy() {

            }

            public Buy(Buy toClone) : this(toClone.Player, new List<Card>(toClone.Cards)) { 

            }

            public Buy(Player player) : this(player, null) {

            }

            public Buy(Player player, IList<Card> cards) {
                Player = player.Clone() as Player;
                Cards = new List<Card>(cards ?? new List<Card>());
            }
        }

        [XmlType("player")]
        public class PlayerInfo {

            [XmlAttribute("id")]
            public int Id { get; set; }

            [XmlAttribute("name")]
            public string Name { get; set; }

            public PlayerInfo() {

            }

            public PlayerInfo(PlayerInfo toClone) : this (toClone.Id, toClone.Name) {

            }

            public PlayerInfo(Player player) : this(player.Id, player.Name) {

            }

            private PlayerInfo(int id, string name) {
                Id = id;
                Name = name;
            }
        }

        [XmlType("card")]
        public class CardInfo {

            [XmlAttribute("id")]
            public int Id { get; set; }

            [XmlAttribute("type")]
            public string TypeName { get; set; }

            public CardInfo() {

            }

            public CardInfo(CardInfo toClone) : this(toClone.Id, toClone.TypeName) {

            }

            public CardInfo(Card card) : this(card.Id, card.Type.Name) {

            }

            private CardInfo(int id, string typeName) {
                Id = id;
                TypeName = typeName;
            }
        }

        [XmlType("cardType")]
        public class CardTypeInfo {

            [XmlAttribute("id")]
            public int Id { get; set; }

            [XmlAttribute("type")]
            public string Name { get; set; }

            public CardTypeInfo() {

            }

            public CardTypeInfo(CardTypeInfo toClone) : this(toClone.Id, toClone.Name) {

            }

            public CardTypeInfo(CardType type) : this(type.Id, type.Name) {

            }

            private CardTypeInfo(int id, string name) {
                Id = id;
                Name = name;
            }
        }

        [XmlType("score")]
        public class ScoreInfo {

            [XmlAttribute("id")]
            public int PlayerId { get; set; }
            
            [XmlAttribute("name")]
            public string PlayerName { get; set; }

            [XmlAttribute("score")]
            public int Score { get; set; }

            public ScoreInfo() {

            }

            public ScoreInfo(ScoreInfo toClone) : this(toClone.PlayerId, toClone.PlayerName, toClone.Score) {

            }

            public ScoreInfo(Player player) : this(player.Id, player.Name, player.Score) {

            }

            private ScoreInfo(int playerId, string playerName, int score) {
                PlayerId = playerId;
                PlayerName = playerName;
                Score = score;
            }
        }

        [XmlArray("turns")]
        public List<Turn> Turns { 
            get { return new List<Turn>(TurnsByNumber.Values); }
            set { throw new NotSupportedException("History deserialization is not presently supported." ); }
        }

        [XmlIgnore()]
        public IDictionary<int, Turn> TurnsByNumber { get; private set; }

        public GameHistory() {
            TurnsByNumber = new Dictionary<int, Turn>();
        }

        public GameHistory(GameHistory toClone) : this() {
            foreach (int turnId in toClone.TurnsByNumber.Keys) {
                TurnsByNumber[turnId] = new Turn(toClone.TurnsByNumber[turnId]);
            }
        }
    }
}
