﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace KingdomGame {

    public class Game : ICloneable, ICardOwner {

        // Refactor - (MT): Push this into the new Game State object.
        public enum Phase {
            ACTION = 1,
            TARGET = 2,
            BUY = 3,
            ADVANCE = 4,
            END = 5
        }

        // Refactor - (MT): Push this object down to the player level.
        public class Strategy : ICloneable {

            public ICardSelectionStrategy CardSelectionStrategy { get; set; }

            public ITargetSelectionStrategy TargetSelectionStrategy { get; set; }

            public IBuyingStrategy BuyingStrategy { get; set; }

            public IDictionary<int, IDiscardingStrategy> DiscardingStrategiesByPlayerId { get; private set; }

            // Refactor - (MT): Push this game state into its own subclass.
            public Phase Phase { get; set; }

            public Stack<IAction> ActionStack { get; set; }

            public int? SelectedCardId { get; set; }

            public IList<Pair<IAction, IList<int>>> PreviousActions { get; set; }
            // End Refactor Block

            public Strategy(IList<int> playerIds) {
                // Todo - (MT): Strategy point #4 - select best discard option (not just random)
                DiscardingStrategiesByPlayerId = new Dictionary<int, IDiscardingStrategy>();
                foreach(int playerId in playerIds) {
                    DiscardingStrategiesByPlayerId[playerId] = new RandomDiscardingStrategy();
                }

                ActionStack = new Stack<IAction>();
            }

            public object Clone() {
                Strategy strategy = new Strategy(new List<int>(DiscardingStrategiesByPlayerId.Keys));

                strategy.CardSelectionStrategy = CardSelectionStrategy.Clone() as ICardSelectionStrategy;
                strategy.TargetSelectionStrategy = TargetSelectionStrategy.Clone() as ITargetSelectionStrategy;
                strategy.BuyingStrategy = BuyingStrategy.Clone() as IBuyingStrategy;
                foreach (int playerId in DiscardingStrategiesByPlayerId.Keys) {
                    strategy.DiscardingStrategiesByPlayerId[playerId] = 
                      DiscardingStrategiesByPlayerId[playerId].Clone() as IDiscardingStrategy;
                }

                strategy.Phase = Phase;
                strategy.ActionStack = new Stack<IAction>(ActionStack);
                strategy.SelectedCardId = SelectedCardId;
                strategy.PreviousActions = new List<Pair<IAction, IList<int>>>(PreviousActions);

                return strategy;
            }

            public override bool Equals(object obj) {
                Strategy strategy = obj as Strategy;
                if (strategy == null) {
                    return false;
                }

                if (PreviousActions.Count != strategy.PreviousActions.Count) {
                    return false;
                }

                for (int actionIndex = 0; actionIndex < PreviousActions.Count; actionIndex++) {
                    if (!PreviousActions[actionIndex].Equals(strategy.PreviousActions[actionIndex].First)) {
                        return false;
                    }

                    List<int> thisTargets = new List<int>(PreviousActions[actionIndex].Second);
                    List<int> strategyTargets = new List<int>(strategy.PreviousActions[actionIndex].Second);

                    if (thisTargets.Count != strategyTargets.Count) {
                        return false;
                    }

                    thisTargets.Sort();
                    strategyTargets.Sort();
                    
                    for (int targetIndex = 0; targetIndex < thisTargets.Count; targetIndex++) {
                        if (thisTargets[targetIndex] != strategyTargets[targetIndex]) {
                            return false;
                        }
                    }
                }

                if (ActionStack.Count != strategy.ActionStack.Count) {
                    return false;
                }

                for (int actionIndex = 0; actionIndex < ActionStack.Count; actionIndex++) {
                    if (!ActionStack.ElementAt<IAction>(actionIndex).Equals(
                      strategy.ActionStack.ElementAt<IAction>(actionIndex))) {
                          return false;
                    }
                }

                if (DiscardingStrategiesByPlayerId.Count != strategy.DiscardingStrategiesByPlayerId.Count) {
                    return false;
                }

                foreach(int playerId in DiscardingStrategiesByPlayerId.Keys) {
                    if (!strategy.DiscardingStrategiesByPlayerId.ContainsKey(playerId)) {
                        return false;
                    }

                    if (!DiscardingStrategiesByPlayerId[playerId].Equals(
                      strategy.DiscardingStrategiesByPlayerId[playerId])) {
                        return false;
                    }
                }

                return CardSelectionStrategy.Equals(strategy.CardSelectionStrategy) 
                  && TargetSelectionStrategy.Equals(strategy.TargetSelectionStrategy) 
                  && BuyingStrategy.Equals(strategy.BuyingStrategy)
                  && Phase == strategy.Phase
                  && SelectedCardId == strategy.SelectedCardId;
            }

            public override int GetHashCode() {
                int code = CardSelectionStrategy.GetHashCode() 
                  ^ TargetSelectionStrategy.GetHashCode() 
                  ^ BuyingStrategy.GetHashCode()
                  ^ Phase.GetHashCode()
                  ^ SelectedCardId.GetHashCode();

                code ^= PreviousActions.Count;

                foreach(Pair<IAction, IList<int>> pair in PreviousActions) {
                    code ^= pair.First.GetType().GetHashCode();
                    code ^= pair.Second.Count;

                    foreach (int targetId in pair.Second) {
                        code ^= targetId.GetHashCode();
                    }
                }

                code ^= ActionStack.Count;

                foreach(IAction action in ActionStack) {
                    code ^= action.GetHashCode();
                }

                code ^= DiscardingStrategiesByPlayerId.Keys.Count;

                foreach (int playerId in DiscardingStrategiesByPlayerId.Keys) {
                    code ^= playerId;
                    code ^= DiscardingStrategiesByPlayerId[playerId].GetHashCode();
                }

                return code;
            }
        }

        public const int MAX_EMPTY_CARD_SETS = 3;

        private static int NextId = 1;

        private IList<Player> _orderedPlayerList;
        private IDictionary<int, Player> _playersById;
        private IDictionary<string, Player> _playersByName;

        private IDictionary<int, Card> _cardsById;
        private IDictionary<int, Deck> _cardsByTypeId;

        private Deck _trash;

        private int _id;
        private int _currentPlayerIndex;
        private int _turnNumber;

        private Strategy _strategy;

        public Game(IList<Player> orderedPlayerList) : this(orderedPlayerList, Game.GetDefaultCardCountsByType()) {

        }

        public Game(IList<Player> orderedPlayerList, IDictionary<int, int> gameCardCountsByType) {
            SetUpPlayerIndexes(orderedPlayerList);
            SetUpBasicGameDetails();

            _cardsByTypeId = new Dictionary<int, Deck>();
            foreach(int cardTypeId in gameCardCountsByType.Keys) {
                _cardsByTypeId[cardTypeId] = new Deck();
                CardType cardType = CardType.GetCardTypeById(cardTypeId);

                for(int cardTypeIndex = 0; cardTypeIndex < gameCardCountsByType[cardTypeId]; cardTypeIndex++) {
                    Card card = new Card(cardType, null);
                    _cardsByTypeId[cardTypeId].Add(card);
                }
            }

            SetUpCardIndex();
        }

        private Game(Game toClone) {
            // Note - (MT): Games do not have their ID copied over on clones, by design
            List<Player> players = toClone._orderedPlayerList as List<Player>;
            List<Player> clonedPlayers = players.ConvertAll<Player>(delegate(Player player) { return player.Clone() as Player; });
            SetUpPlayerIndexes(clonedPlayers);
            SetUpBasicGameDetails();

            _trash = toClone._trash.Clone() as Deck;
            _cardsByTypeId = new Dictionary<int, Deck>();
            foreach(int cardTypeId in toClone._cardsByTypeId.Keys) {
                _cardsByTypeId[cardTypeId] = toClone._cardsByTypeId[cardTypeId].Clone() as Deck;
            }

            _currentPlayerIndex = toClone._currentPlayerIndex;
            _strategy = toClone._strategy.Clone() as Strategy;

            SetUpCardIndex();
        }

        public object Clone() {
            return new Game(this);
        }

        public Player GetPlayerById(int id) {
            return _playersById.ContainsKey(id) ? _playersById[id] : null;
        }

        public Player GetPlayerByName(string name) {
            return _playersByName.ContainsKey(name) ? _playersByName[name] : null;
        }

        public int Id {
            get { return this._id; }
        }

        public Strategy CurrentStrategy { get { return _strategy; } }

        public IList<Player> Players {
            get { return new List<Player>(_orderedPlayerList); }
        }

        public Player CurrentPlayer {
            get { return _orderedPlayerList[_currentPlayerIndex]; }
        }

        public int TurnNumber {
            get { return this._turnNumber; }
        }

        public IList<Card> Cards {
            get { return new List<Card>(_cardsById.Values); }
        }

        public IList<Card> Trash {
            get { return _trash.Cards; }
        }

        public IList<Card> GetCardsByType(CardType type) {
            if (!_cardsByTypeId.ContainsKey(type.Id)) {
                return null;
            }

            return new List<Card>(_cardsByTypeId[type.Id].Cards);
        }

        public IList<IList<CardType>> GetAllValidBuyOptions() {
            IDictionary<int, int> cardsAvailableByTypeId = new Dictionary<int, int>();
            foreach(int typeId in _cardsByTypeId.Keys) {
                cardsAvailableByTypeId[typeId] = _cardsByTypeId[typeId].Cards.Count;
            }

            IList<IList<CardType>> buyingOptions = Game.CalculateAllBuyOptions(
              CurrentPlayer.RemainingBuys, 
              CurrentPlayer.RemainingMoney,
              cardsAvailableByTypeId
            );

            return buyingOptions;
        }

        public Card GetCardById(int cardId) {
            if (!_cardsById.ContainsKey(cardId)) {
                return null;
            }

            return _cardsById[cardId];
        }

        public void StartGame() {
            StartGame(new Dictionary<int, int>(), null, false);
        }

        public void StartGame(IDictionary<int, int> playerCardCountsByType) {
            StartGame(playerCardCountsByType, null, false);
        }

        public void StartGame(
          IDictionary<int, int> playerCardCountsByType,
          IDictionary<int, int> handCardCountsByType
        ) {
            StartGame(playerCardCountsByType, handCardCountsByType, false);
        }

        public void StartGame(
          IDictionary<int, int> playerCardCountsByType,
          IDictionary<int, int> handCardCountsByType,
          bool randomizeFirstPlayer
        ) {
            foreach (int cardTypeId in playerCardCountsByType.Keys) {
                if (!_cardsByTypeId.ContainsKey(cardTypeId)) {
                    throw new ArgumentException(
                      "Cannot assign cards to a player if their type doesn't belong to the game.");
                }
                
                if (_cardsByTypeId[cardTypeId].Size < _orderedPlayerList.Count * playerCardCountsByType[cardTypeId]) {
                    throw new ArgumentException(
                      "Cannot assign cards to a player if enough of the type don't belong to the game.");
                }
            }

            if (handCardCountsByType != null) {
                foreach (int cardTypeId in handCardCountsByType.Keys) {
                    if (!_cardsByTypeId.ContainsKey(cardTypeId)) {
                        throw new ArgumentException(
                          "Cannot place cards in a player's hand the player doesn't have any cards of that type.");
                    }
                
                    if (playerCardCountsByType[cardTypeId] < handCardCountsByType[cardTypeId]) {
                        throw new ArgumentException(
                          "Cannot place cards in a player's hand the player doesn't have enough cards of that type.");
                    }
                }
            }

            foreach(int cardTypeId in playerCardCountsByType.Keys) {
                CardType cardType = CardType.GetCardTypeById(cardTypeId);

                foreach (Player player in _orderedPlayerList) {
                    for(int cardTypeIndex = 0; cardTypeIndex < playerCardCountsByType[cardTypeId]; cardTypeIndex++) {
                        CardDestination location = (handCardCountsByType != null 
                          && handCardCountsByType.ContainsKey(cardTypeId) 
                          && cardTypeIndex < handCardCountsByType[cardTypeId]) 
                          ? CardDestination.HAND : CardDestination.DECK;
                        this.DealCard(cardType, player, location);
                    }
                }
            }

            if (handCardCountsByType == null) {
                foreach (Player player in _orderedPlayerList) {
                    player.ShuffleDeck();
                    player.DrawHand();
                }
            }

            _currentPlayerIndex = (randomizeFirstPlayer) ? RandomNumberManager.Next(_orderedPlayerList.Count) : 0;
            CurrentPlayer.StartTurn();
        }

        public void PlayGame() {
            while (!IsGameOver()) {
                PlayTurn();
            }
        }

        public void PlayTurn() {
            while (CurrentStrategy.Phase != Phase.ADVANCE && CurrentStrategy.Phase != Phase.END) {
                PlayPhase();
            }

            // Once the advance phase is reached, play one more phase to end the current turn:
            if (CurrentStrategy.Phase != Phase.END) {
                PlayPhase();
            }
        }

        public void PlayPhase() {
            Phase startPhase = CurrentStrategy.Phase;
            while (startPhase == CurrentStrategy.Phase && CurrentStrategy.Phase != Phase.END) {
                PlayStep();
            }
        }

        public void PlayStep() {
            switch (CurrentStrategy.Phase) {

                case Phase.ACTION:

                    bool advanceToBuy = true;
                    if (CurrentPlayer.RemainingActions > 0) {
                        Card cardToPlay =  _strategy.CardSelectionStrategy.FindOptimalCardSelectionStrategy
                            (this, new Deck(CurrentPlayer.Hand));

                        if (cardToPlay != null) {
                            CurrentStrategy.SelectedCardId = (int?) cardToPlay.Id;
                            Logger.Instance.RecordCardSelection(this, CurrentPlayer, cardToPlay);

                            CurrentStrategy.Phase = Phase.TARGET;
                            CurrentStrategy.ActionStack = new Stack<IAction>();
                            for (int actionIndex = cardToPlay.Type.Actions.Count - 1; actionIndex >= 0; actionIndex--) {
                                CurrentStrategy.ActionStack.Push(cardToPlay.Type.Actions[actionIndex]);
                            }
                                
                            CurrentPlayer.PlayCard(cardToPlay);
                            CurrentPlayer.RemainingActions--;
                            advanceToBuy = false;
                        } else {
                            // If no card is selected, skip all remaining actions.
                            CurrentPlayer.RemainingActions = 0;
                        }
                    }

                    if (advanceToBuy) {
                        CurrentStrategy.SelectedCardId = null;
                        CurrentStrategy.Phase = Phase.BUY;
                        CurrentStrategy.ActionStack = new Stack<IAction>();
                    }

                    CurrentStrategy.PreviousActions = new List<Pair<IAction, IList<int>>>();

                    break;

                case Phase.TARGET:

                    if (CurrentStrategy.SelectedCardId != null) {

                        Card cardToPlay = _cardsById[CurrentStrategy.SelectedCardId.Value];
                        IAction actionToPlay = CurrentStrategy.ActionStack.Pop();

                        if (actionToPlay != null) {

                            // Refactor - (MT): Find a way to make this generic enough to have a single call.
                            IList<Player> targetPlayers = _strategy.TargetSelectionStrategy.SelectTargets<Player>(
                                this, 
                                cardToPlay, 
                                actionToPlay,
                                CurrentStrategy.PreviousActions
                            );

                            IList<Card> targetCards = _strategy.TargetSelectionStrategy.SelectTargets<Card>(
                                this,
                                cardToPlay,
                                actionToPlay,
                                CurrentStrategy.PreviousActions
                            );

                            IList<CardType> targetTypes = _strategy.TargetSelectionStrategy.SelectTargets<CardType>(
                                this,
                                cardToPlay,
                                actionToPlay,
                                CurrentStrategy.PreviousActions
                            );

                            List<int> targetIds = new List<int>();
                            if (targetPlayers.Count > 0) {
                                Logger.Instance.RecordTargetSelection(
                                  this, 
                                  CurrentPlayer, 
                                  cardToPlay, 
                                  actionToPlay, 
                                  targetPlayers
                                );

                                actionToPlay.Apply<Player>(targetPlayers, this, CurrentStrategy.PreviousActions);
                                targetIds.AddRange(new List<Player>(targetPlayers)
                                  .ConvertAll<int>(delegate (Player player) { return player.Id;}));
                            } else if (targetCards.Count > 0) {
                                Logger.Instance.RecordTargetSelection(
                                  this, 
                                  CurrentPlayer, 
                                  cardToPlay, 
                                  actionToPlay, 
                                  targetCards
                                );

                                actionToPlay.Apply<Card>(targetCards, this, CurrentStrategy.PreviousActions);
                                targetIds.AddRange(new List<Card>(targetCards)
                                  .ConvertAll<int>(delegate (Card card) { return card.Id;}));
                            } else if (targetTypes.Count > 0) {
                                Logger.Instance.RecordTargetSelection(
                                  this, 
                                  CurrentPlayer, 
                                  cardToPlay, 
                                  actionToPlay, 
                                  targetTypes
                                );

                                actionToPlay.Apply<CardType>(targetTypes, this, CurrentStrategy.PreviousActions);
                                targetIds.AddRange(new List<CardType>(targetTypes)
                                  .ConvertAll<int>(delegate (CardType type) { return type.Id;}));
                            }
                            // End Refactor Block

                            CurrentStrategy.PreviousActions.Add(new Pair<IAction, IList<int>>(actionToPlay, targetIds));
                        }

                        if (CurrentStrategy.ActionStack.Count == 0) {
                            if (CurrentPlayer.RemainingActions > 0) {
                                CurrentStrategy.Phase = Phase.ACTION;
                            } else {
                                CurrentStrategy.Phase = Phase.BUY;
                            }

                            CurrentStrategy.SelectedCardId = null;
                            CurrentStrategy.PreviousActions = new List<Pair<IAction, IList<int>>>();
                        }
                    }
                    else {
                        // No actions left to play should short-circuit attempts at finding another.
                        CurrentPlayer.RemainingActions = 0;
                        CurrentStrategy.Phase = Phase.BUY;
                        CurrentStrategy.ActionStack = new Stack<IAction>();
                        CurrentStrategy.SelectedCardId = null;
                        CurrentStrategy.PreviousActions = new List<Pair<IAction, IList<int>>>();
                    }

                    break;

                case Phase.BUY:

                    bool isTurnOver = false;
                    if (CurrentPlayer.RemainingBuys > 0) {
                        IList<IList<CardType>> buyingOptions = GetAllValidBuyOptions();

                        CardType typeToBuy = _strategy.BuyingStrategy.FindOptimalBuyingStrategy(this, buyingOptions);

                        if (typeToBuy != null && GetCardsByType(typeToBuy).Count > 0) {
                            Card cardBought = SellCard(CurrentPlayer, typeToBuy);
                            Logger.Instance.RecordBuy(this, CurrentPlayer, cardBought);
                            if (CurrentPlayer.RemainingBuys == 0) {
                                isTurnOver = true;
                            }
                        } else {
                            // If no card is selected, assume the turn is over.
                            isTurnOver = true;
                        }
                    }
                    else {
                        isTurnOver = true;
                    }

                    if (IsGameOver()) {
                        isTurnOver = true;
                        CurrentStrategy.Phase = Phase.END;
                        Logger.Instance.RecordEndOfTurn(this);
                    }
                    else if (isTurnOver) {
                        CurrentStrategy.Phase = Phase.ADVANCE;
                    }

                    if (isTurnOver) {
                        CurrentStrategy.ActionStack = new Stack<IAction>();
                        CurrentStrategy.SelectedCardId = null;
                        CurrentStrategy.PreviousActions = new List<Pair<IAction, IList<int>>>();
                    }

                    break;

                case Phase.ADVANCE:

                    Logger.Instance.RecordEndOfTurn(this);

                    AdvanceTurn();
                    CurrentStrategy.Phase = Phase.ACTION;
                    CurrentStrategy.ActionStack = new Stack<IAction>();
                    CurrentStrategy.SelectedCardId = null;
                    CurrentStrategy.PreviousActions = new List<Pair<IAction, IList<int>>>();

                    break;

                case Phase.END:
                    break;

                default:
                    break;
            }
        }

        public void AdvanceTurn() {
            CurrentPlayer.EndTurn();
            _turnNumber++;
            _currentPlayerIndex = (_currentPlayerIndex + 1) % _orderedPlayerList.Count;
            CurrentPlayer.StartTurn();
        }

        public bool IsGameOver() {
            IDictionary<int, int> emptyCardSetsByWeight = new Dictionary<int, int>();
            foreach (int cardTypeId in _cardsByTypeId.Keys) {
                if (_cardsByTypeId[cardTypeId].Size == 0) {
                    int weight = CardType.GetCardTypeById(cardTypeId).EndOfGameWeight;
                    if (weight == 0) {
                        continue;
                    }

                    if (!emptyCardSetsByWeight.ContainsKey(weight)) {
                        emptyCardSetsByWeight[weight] = 0;
                    }

                    emptyCardSetsByWeight[weight]++;
                    if (emptyCardSetsByWeight[weight] >= weight) {
                        return true;
                    }
                }
            }

            return false;
        }

        public void AcquireCard(Card card, CardDestination destination) {
            Deck deck = null;
            switch (destination) {
                case CardDestination.GAME:
                    deck = _cardsByTypeId[card.Type.Id];
                    break;

                case CardDestination.TRASH:
                    deck = _trash;
                    break;

                default:
                    break;
            }

            if (deck != null) {
                deck.Add(card);
                if (deck == _trash) {
                    card.IsTrashed = true;
                }
            }
        }

        public bool ReleaseCard(Card card) {
            Deck deck = (!card.IsTrashed) ? _cardsByTypeId[card.Type.Id] : _trash;
            if (!deck.Contains(card)) {
                return false;
            }

            card.IsTrashed = false;
            deck.Remove(card);
            return true;
        }

        public Card SellCard(Player buyer, CardType type) {
            Card card = DealCard(type, buyer, CardDestination.PLAY_AREA);
            buyer.RemainingBuys--;
            buyer.RemainingMoney -= type.Cost;
            return card;
        }

        public Card DealCard(CardType type, Player player, CardDestination destination) {
            Deck deck = _cardsByTypeId[type.Id];
            Card card = deck.Peek();
            DealCard(card, player, destination);
            return card;
        }

        public Card DealCard(Card card, Player player, CardDestination destination) {
            TransferCardOwner(card, player, destination);
            return card;
        }

        public Card TrashCard(Card card) {
            TransferCardOwner(card, this, CardDestination.TRASH);
            return card;
        }

        private void SetUpPlayerIndexes(IList<Player> orderedPlayerList) {
            _orderedPlayerList = new List<Player>(orderedPlayerList);
            _playersById = new Dictionary<int, Player>();
            _playersByName = new Dictionary<string, Player>(StringComparer.InvariantCultureIgnoreCase);

            for (int playerIndex = 0; playerIndex < _orderedPlayerList.Count; playerIndex++) {
                _playersById[_orderedPlayerList[playerIndex].Id] = _orderedPlayerList[playerIndex];
                _playersByName[_orderedPlayerList[playerIndex].Name] = _orderedPlayerList[playerIndex];
            }
        }

        private void SetUpBasicGameDetails() {
            _id = Game.NextId++;
            _currentPlayerIndex = 0;
            _turnNumber = 1;
            _trash = new Deck();

            _strategy = new Strategy(new List<int>(_playersById.Keys));
            
            // Todo - (MT): Strategy point #1 - select best card (instead of random)
            _strategy.CardSelectionStrategy = new RandomCardSelectionStrategy();
            // Todo - (MT): Strategy point #2 - select best (or at least random) target
            _strategy.TargetSelectionStrategy = new RandomTargetSelectionStrategy();
            // Todo - (MT): Strategy point #3 - select best buy option (not just random)
            _strategy.BuyingStrategy = new RandomBuyingStrategy();
            _strategy.Phase = Phase.ACTION;
            _strategy.SelectedCardId = null;
            _strategy.PreviousActions = new List<Pair<IAction, IList<int>>>();

            _cardsById = new Dictionary<int, Card>();
        }

        private void SetUpCardIndex() {
            _cardsById = new Dictionary<int, Card>();

            foreach (Player player in _orderedPlayerList) {
                foreach (Card card in player.Hand) {
                    _cardsById[card.Id] = card;
                }

                foreach (Card card in player.Discard) {
                    _cardsById[card.Id] = card;
                }

                foreach (Card card in player.PlayArea) {
                    _cardsById[card.Id] = card;
                }

                foreach (Card card in player.Deck) {
                    _cardsById[card.Id] = card;
                }
            }

            foreach (Deck cardsToBuy in _cardsByTypeId.Values) {
                foreach (Card card in cardsToBuy.Cards) {
                    _cardsById[card.Id] = card;
                }
            }

            foreach (Card card in Trash) {
                _cardsById[card.Id] = card;
            }
        }

        private void TransferCardOwner(Card card, ICardOwner newOwner, CardDestination destination) {
            ICardOwner currentOwner = card.OwnerId.HasValue
              ? this.GetPlayerById(card.OwnerId.Value) as ICardOwner : this;

            currentOwner.ReleaseCard(card);
            newOwner.AcquireCard(card, destination);

            card.OwnerId = (newOwner != this) ? (newOwner as Player).Id : (int?) null;
        }

        private static IDictionary<int, int> GetDefaultCardCountsByType() {
            IDictionary<int, int> defaultGameCardCountsByType = new Dictionary<int, int>();
            foreach(CardType type in CardType.CardTypes) {
                defaultGameCardCountsByType[type.Id] = CardType.GetDefaultQuantityByTypeId(type.Id);
            }

            return defaultGameCardCountsByType;
        }

        private static IList<IList<CardType>> CalculateAllBuyOptions(
          int buysRemaining, 
          int moneyRemaining, 
          IDictionary<int,int> cardsRemainingByTypeId) 
        {
            List<IList<CardType>> buyOptions = new List<IList<CardType>>(){new List<CardType>()}; 
            if(buysRemaining == 0) {
                return buyOptions;
            }

            foreach (int typeId in cardsRemainingByTypeId.Keys) {
                int cardsRemaining = cardsRemainingByTypeId[typeId];
                if (cardsRemaining == 0) {
                    continue;
                }

                CardType type = CardType.GetCardTypeById(typeId);
                int cardCost = type.Cost;
                if(cardCost > moneyRemaining) {
                    continue;
                }

                IDictionary<int, int> cardsRemainingByTypeIdAfterBuy 
                  = new Dictionary<int, int>(cardsRemainingByTypeId);
                cardsRemainingByTypeIdAfterBuy[typeId]--;
                IList<IList<CardType>> optionsAfterBuy = CalculateAllBuyOptions(
                  buysRemaining - 1, 
                  moneyRemaining - cardCost, 
                  cardsRemainingByTypeIdAfterBuy
                );

                foreach(IList<CardType> optionAfterBuy in optionsAfterBuy) {
                    IList<CardType> fullOption = new List<CardType>(optionAfterBuy);
                    fullOption.Add(type);
                    buyOptions.Add(fullOption);
                }
            }

            return buyOptions;
        }

        public override bool Equals(object obj) {
            Game game = obj as Game;
            if (game == null) {
                return false;
            }

            if (this._orderedPlayerList.Count != game._orderedPlayerList.Count) {
                return false;
            }

            if (this._currentPlayerIndex != game._currentPlayerIndex) {
                return false;
            }

            if (this._turnNumber != game._turnNumber) {
                return false;
            }

            for (int playerIndex = 0; playerIndex < this._orderedPlayerList.Count; playerIndex++) {
                if (!this._orderedPlayerList[playerIndex].Equals(game._orderedPlayerList[playerIndex])) {
                    return false;
                }
            }

            if(this._cardsByTypeId.Count != game._cardsByTypeId.Count) {
                return false;
            }

            foreach (KeyValuePair<int, Deck> entry in this._cardsByTypeId) {
                if (!game._cardsByTypeId.ContainsKey(entry.Key)) {
                    return false;
                }

                if (!game._cardsByTypeId[entry.Key].Equals(this._cardsByTypeId[entry.Key])) {
                    return false;
                }
            }

            if (!game._strategy.Equals(this._strategy)) {
                return false;
            }

            return true;
        }

        public override int GetHashCode() {
            int code = this._orderedPlayerList.Count.GetHashCode();
            code = code ^ this._currentPlayerIndex;
            code = code ^ this._turnNumber;

            for (int playerIndex = 0; playerIndex < this._orderedPlayerList.Count; playerIndex++) {
                code = code ^ this._orderedPlayerList[playerIndex].GetHashCode();
            }

            code = code ^ this._cardsByTypeId.Count.GetHashCode();

            foreach (KeyValuePair<int, Deck> entry in this._cardsByTypeId) {
                code = code ^ entry.Key.GetHashCode();
                code = code ^ this._cardsByTypeId[entry.Key].GetHashCode();
            }

            code = code ^ this._strategy.GetHashCode();

            return code;
        }
    }
}