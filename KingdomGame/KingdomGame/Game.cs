using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace KingdomGame {

    public class Game : ICloneable, ICardOwner {

        #region Enums

        public enum Phase {
            START = 1,
            PLAY = 2,
            ACTION = 3,
            BUY = 4,
            ADVANCE = 5,
            END = 6,
            DONE = 7
        }

        #endregion

        #region Public Classes

        public class GameState : ICloneable {

            #region Private Members

            private Game _game;
            private Phase _phase;
            private int _turnNumber;
            private int _currentPlayerIndex;
            private int? _selectedPlayId;

            private Stack<IAction> _pendingActionStack;
            private Stack<Pair<IAction, IList<int>>> _executedActionStack;

            #endregion

            #region Constructors

            public GameState(Game game) {
                _game = game;
                _phase = Phase.START;
                _selectedPlayId = null;
                _turnNumber = 1;
                _pendingActionStack = new Stack<IAction>();
                _executedActionStack = new Stack<Pair<IAction, IList<int>>>();
            }

            #endregion

            #region Properties

            // Refactor - (MT): Handle backrefs more elegantly than this.
            internal Game Game { set { _game = value; } }

            public Phase Phase { get { return _phase; } }

            public Card SelectedPlay { 
                get { return (_selectedPlayId.HasValue) ? _game.GetCardById(_selectedPlayId.Value) : null; }
                set {
                    if (value != null) {
                        if (Phase != Phase.PLAY) {
                            throw new InvalidOperationException(
                              "Cannot set a selected card outside of the play phase.");
                        }

                        _selectedPlayId = (int ?) value.Id;
                    }
                    else {
                        _selectedPlayId = null;
                    }
                }
            }

            public int TurnNumber { get { return _turnNumber; } }

            public Player CurrentPlayer { get { return _game.Players[_currentPlayerIndex]; } }

            public bool HasNextPendingAction { get { return NextPendingAction != null; } }

            public IAction NextPendingAction {
                get {
                    if ((_phase == Phase.ACTION) && (_pendingActionStack.Count > 0)) {
                        return _pendingActionStack.Peek();
                    }

                    return null;
                }
            }

            #endregion

            #region Public Methods

            #region Exeuction Methods

            public void AdvanceTurn() {
                _turnNumber++;
                _currentPlayerIndex = (_currentPlayerIndex + 1) % _game._orderedPlayerList.Count;
            }

            public void TransitionPhase() {
                switch (Phase) {
                    case Phase.START:
                        if (Game.IsGameFinished(_game._cardsByTypeId)) {
                            _phase = Phase.END;
                        }
                        else {
                            _phase = Phase.PLAY;
                        }

                        break;

                    case Phase.PLAY:
                        _phase = (SelectedPlay != null) ? Phase.ACTION : Phase.BUY;
                        if (SelectedPlay != null) {
                            for (int actionIndex = SelectedPlay.Type.Actions.Count - 1; actionIndex >= 0; actionIndex--) {
                                AddPendingAction(SelectedPlay.Type.Actions[actionIndex].Create(CurrentPlayer));
                            }
                        }

                        break;

                    case Phase.ACTION:
                        if (!HasNextPendingAction) {
                            _phase = (CurrentPlayer.RemainingActions > 0) ? Phase.PLAY : Phase.BUY;

                            SelectedPlay = null;
                            _pendingActionStack = new Stack<IAction>();
                            _executedActionStack = new Stack<Pair<IAction, IList<int>>>();
                        }

                        break;

                    case Phase.BUY:
                        if (Game.IsGameFinished(_game._cardsByTypeId)) {
                            _phase = Phase.END;
                        } else if (CurrentPlayer.RemainingBuys == 0) {
                            _phase = Phase.ADVANCE;
                        }

                        break;

                    case Phase.ADVANCE:
                        _phase = Phase.PLAY;
                        break;

                    case Phase.END:
                        _phase = Phase.DONE;
                        break;

                    case Phase.DONE:
                        break;
                }
            }

            public void AddPendingAction(IAction action) {
                if(_phase != Phase.ACTION) {
                    throw new InvalidOperationException("Cannot add an action outside of the action phase.");
                }                

                if(action == null) {
                    throw new ArgumentNullException("Cannot add a null pending action.");
                }

                if (!action.TargetSelectorId.HasValue) {
                    throw new ArgumentNullException("Cannot add an action without a valid target selector.");
                }

                _pendingActionStack.Push(action);
            }

            // Refactor - (MT): Make the targets a generically typed parameter.
            public void ExecuteNextPendingAction(IList<int> targetIds) {
                if(_phase != Phase.ACTION) {
                    throw new InvalidOperationException("Cannot execute an action outside of the action phase.");
                }                

                if(_pendingActionStack.Count == 0) {
                    throw new InvalidOperationException("Cannot execute a pending action if none exist.");
                }

                IAction pendingAction = _pendingActionStack.Pop();
                _executedActionStack.Push(new Pair<IAction, IList<int>>(pendingAction, targetIds));
            }

            #endregion

            #region History Query Methods

            // Refactor - (MT): Make the returned targets a generically typed parameter.
            public IList<int> GetTargetsFromLastExecutedAction(Type actionType) {
                if (actionType == null) {
                    throw new ArgumentNullException("Cannot get the last executed action without a non-null type.");
                }

                Stack<Pair<IAction, IList<int>>> copiedStack 
                  = new Stack<Pair<IAction, IList<int>>>(_executedActionStack);

                while (copiedStack.Count > 0) {
                    Pair<IAction, IList<int>> candidateAction = copiedStack.Pop();
                    if(candidateAction.First.GetType().Equals(actionType)) {
                        return new List<int>(candidateAction.Second);
                    }
                }

                return null;
            }

            #endregion

            #region Clone and Equality Methods

            public object Clone() {
                GameState state = new GameState(_game);

                state._phase = _phase;
                state._selectedPlayId = _selectedPlayId;
                state._turnNumber = _turnNumber;
                state._currentPlayerIndex = _currentPlayerIndex;
                state._pendingActionStack = new Stack<IAction>(_pendingActionStack);
                state._executedActionStack = new Stack<Pair<IAction, IList<int>>>(_executedActionStack);

                return state;
            }

            public override bool Equals(object obj) {
                GameState state = obj as GameState;
                if (state == null) {
                    return false;
                }

                if (_executedActionStack.Count != state._executedActionStack.Count) {
                    return false;
                }

                for (int actionIndex = 0; actionIndex < _executedActionStack.Count; actionIndex++) {
                    if (!_executedActionStack.ElementAt(actionIndex).First.Equals(
                          state._executedActionStack.ElementAt(actionIndex).First)) {
                        return false;
                    }

                    List<int> thisTargets = new List<int>(state._executedActionStack.ElementAt(actionIndex).Second);
                    List<int> strategyTargets = new List<int>(state._executedActionStack.ElementAt(actionIndex).Second);

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

                if (_pendingActionStack.Count != state._pendingActionStack.Count) {
                    return false;
                }

                for (int actionIndex = 0; actionIndex < _pendingActionStack.Count; actionIndex++) {
                    if (!_pendingActionStack.ElementAt<IAction>(actionIndex).Equals(
                      state._pendingActionStack.ElementAt<IAction>(actionIndex))) {
                          return false;
                    }
                }

                return _phase == state._phase 
                  && _selectedPlayId == state._selectedPlayId
                  && _turnNumber == state._turnNumber
                  && _currentPlayerIndex == state._currentPlayerIndex;
            }

            public override int GetHashCode() {
                int code = _phase.GetHashCode() 
                  ^ _selectedPlayId.GetHashCode() 
                  ^ _turnNumber.GetHashCode() 
                  ^ _currentPlayerIndex.GetHashCode();

                code ^= _executedActionStack.Count.GetHashCode();

                foreach(Pair<IAction, IList<int>> pair in _executedActionStack) {
                    code ^= pair.First.GetType().GetHashCode();
                    code ^= pair.Second.Count.GetHashCode();

                    foreach (int targetId in pair.Second) {
                        code ^= targetId.GetHashCode();
                    }
                }

                code ^= _pendingActionStack.Count.GetHashCode();

                foreach(IAction action in _pendingActionStack) {
                    code ^= action.GetHashCode();
                }

                return code;
            }

            #endregion

            #endregion

        }

        #endregion

        #region Static Members

        private static int NextId = 1;

        #endregion

        #region Private Members

        private int _id;
        private GameState _state;

        private IList<Player> _orderedPlayerList;
        private IDictionary<int, Player> _playersById;
        private IDictionary<string, Player> _playersByName;

        private Deck _trash;
        private IDictionary<int, Card> _cardsById;
        private IDictionary<int, Deck> _cardsByTypeId;

        #endregion

        #region Constructors

        public Game(IList<Player> orderedPlayerList) : this(orderedPlayerList, ActionRegistry.Instance.DefaultCardCountsByType) {

        }

        public Game(IList<Player> orderedPlayerList, IDictionary<int, int> gameCardCountsByType) : this() {
            _state = new GameState(this);

            SetUpPlayerIndex(orderedPlayerList);

            _trash = new Deck();
            _cardsByTypeId = new Dictionary<int, Deck>();
           foreach(int cardTypeId in gameCardCountsByType.Keys) {
                _cardsByTypeId[cardTypeId] = new Deck();
                CardType cardType = ActionRegistry.Instance.GetCardTypeById(cardTypeId);

                for(int cardTypeIndex = 0; cardTypeIndex < gameCardCountsByType[cardTypeId]; cardTypeIndex++) {
                    Card card = new Card(cardType, null);
                    _cardsByTypeId[cardTypeId].Add(card);
                }
            }

            SetUpCardIndex();
        }

        private Game(Game toClone) : this() {
            // Refactor - (MT): Make an attribute-based clone factory to properly handle deep/shallow/backrefs
            _state = toClone._state.Clone() as GameState;
            _state.Game = this;

            List<Player> players = toClone._orderedPlayerList as List<Player>;
            List<Player> clonedPlayers = players.ConvertAll<Player>(delegate(Player player) { return player.Clone() as Player; });
            SetUpPlayerIndex(clonedPlayers);

            _trash = toClone._trash.Clone() as Deck;
            _cardsByTypeId = new Dictionary<int, Deck>();
            foreach(int cardTypeId in toClone._cardsByTypeId.Keys) {
                _cardsByTypeId[cardTypeId] = toClone._cardsByTypeId[cardTypeId].Clone() as Deck;
            }

            SetUpCardIndex();
        }

        private Game() {
            // Note - (MT): Games do not retain their ID on clones, by design.
            _id = Game.NextId++;
        }

        #endregion

        #region Properties

        public int Id { get { return this._id; } }

        public GameState State { get { return _state; } }

        public bool IsFinished { get { return _state.Phase == Phase.DONE; } }

        public IList<Player> Players { get { return new List<Player>(_orderedPlayerList); } }

        public IList<Card> Cards { get { return new List<Card>(_cardsById.Values); } }

        public IList<Card> Trash { get { return _trash.Cards; } }

        #endregion

        #region Public Methods

        #region Query Methods

        #region Player Query Methods

        public Player GetPlayerById(int id) {
            return (_playersById.ContainsKey(id)) ? _playersById[id] : null;
        }

        public Player GetPlayerByName(string name) {
            return (_playersByName.ContainsKey(name)) ? _playersByName[name] : null;
        }

        #endregion

        #region Card Query Methods

        public Card GetCardById(int cardId) {
            return (_cardsById.ContainsKey(cardId)) ? _cardsById[cardId] : null;
        }

        public IList<Card> GetCardsByType(CardType type) {
            return (_cardsByTypeId.ContainsKey(type.Id)) ? new List<Card>(_cardsByTypeId[type.Id].Cards) : null;
        }

        #endregion

        #region Buy Option Query Methods

        public IList<IList<CardType>> GetAllValidBuyOptions() {
            IDictionary<int, int> cardsAvailableByTypeId = new Dictionary<int, int>();
            foreach(int typeId in _cardsByTypeId.Keys) {
                cardsAvailableByTypeId[typeId] = _cardsByTypeId[typeId].Cards.Count;
            }

            IList<IList<CardType>> buyingOptions = Game.CalculateAllBuyOptions(
              State.CurrentPlayer.RemainingBuys, 
              State.CurrentPlayer.RemainingMoney,
              cardsAvailableByTypeId
            );

            return buyingOptions;
        }

        #endregion

        #endregion

        #region Initialization Methods

        public void StartGame(
          IDictionary<int, int> playerCardCountsByType,
          bool randomizePlayerOrder
        ) {
            StartGame(playerCardCountsByType, null, randomizePlayerOrder);
        }

        public void StartGame(
          IDictionary<int, int> playerCardCountsByType,
          IDictionary<int, int> handCardCountsByType,
          bool randomizePlayerOrder
        ) {
            if (_state.Phase != Phase.START) {
                throw new Exception("Cannot start a game which has already been started.");
            }

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
                CardType cardType = ActionRegistry.Instance.GetCardTypeById(cardTypeId);

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

            if (randomizePlayerOrder) {
                List<Player> newOrderedPlayerList = new List<Player>();
                while (_orderedPlayerList.Count > 0) {
                    int playerIndex = RandomNumberManager.Next(_orderedPlayerList.Count);
                    newOrderedPlayerList.Add(_orderedPlayerList[playerIndex]);
                    _orderedPlayerList.RemoveAt(playerIndex);
                }

                _orderedPlayerList = newOrderedPlayerList;
            }

            // Handle the start step here.
            PlayStep();
        }

        #endregion

        #region Execution Methods

        public void PlayGame() {
            while (State.Phase != Phase.DONE) {
                PlayTurn();
            }
        }

        public void PlayTurn() {
            while ((State.Phase != Phase.ADVANCE) && (State.Phase != Phase.END) && (State.Phase != Phase.DONE)) {
                PlayPhase();
            }

            // Once advance or end is reached, play once more to end the current turn or game, respectively:
            if ((State.Phase == Phase.ADVANCE) || (State.Phase != Phase.END)) {
                PlayPhase();
            }
        }

        public void PlayPhase() {
            Phase startPhase = State.Phase;
            while (startPhase == State.Phase && State.Phase != Phase.DONE) {
                PlayStep();
            }
        }

        public void PlayStep() {
            switch (State.Phase) {

                case Phase.START:

                    AssertCardSelected(false);
                    AssertPendingActionAvailable(false);

                    if (!Game.IsGameFinished(_cardsByTypeId)) {
                        State.CurrentPlayer.StartTurn();
                    }

                    break;

                case Phase.PLAY:

                    AssertCardSelected(false);
                    AssertPendingActionAvailable(false);
                    AssertRemainingActionsAvailable(true);

                    State.SelectedPlay = _state.CurrentPlayer.Strategy.PlaySelectionStrategy.SelectPlay(
                      this, 
                      new Deck(State.CurrentPlayer.Hand)
                    );

                    if (State.SelectedPlay != null) {               
                        State.CurrentPlayer.PlayCard(State.SelectedPlay);
                    } 
                    else {
                        // If no card is selected, skip all remaining actions.
                        State.CurrentPlayer.RemainingActions = 0;
                    }

                    Logger.Instance.RecordPlay(this, State.CurrentPlayer, State.SelectedPlay);

                    break;

                case Phase.ACTION:

                    AssertCardSelected(true);
                    AssertPendingActionAvailable(true);

                    IAction action = State.NextPendingAction;
                    Player targetSelector = GetPlayerById(action.TargetSelectorId.Value);
                    IList<ITargetable> targets = targetSelector.Strategy.TargetSelectionStrategy.SelectTargets(
                        this, 
                        State.SelectedPlay, 
                        action
                    );

                    State.ExecuteNextPendingAction(new List<ITargetable>(targets).ConvertAll<int>(
                      delegate (ITargetable target) { return target.Id;}
                    ));

                    if (action.IsTargetSetValid(targets, this)) {
                        action.Apply(targets, this);
                    }

                    Logger.Instance.RecordAction(this, GetPlayerById(action.TargetSelectorId.Value), State.SelectedPlay, action, targets);

                    break;

                case Phase.BUY:

                    AssertCardSelected(false);
                    AssertPendingActionAvailable(false);
                    AssertRemainingActionsAvailable(false);
                    AssertRemainingBuysAvailable(true);

                    CardType typeToBuy = _state.CurrentPlayer.Strategy.BuySelectionStrategy.SelectBuy(this);

                    Debug.Assert(
                      (typeToBuy == null || GetCardsByType(typeToBuy).Count > 0),
                      "No buying strategy should ever return an option with insufficient cards remaining."
                    );

                    if(typeToBuy == null || GetCardsByType(typeToBuy).Count == 0) {
                        typeToBuy = null;
                    }

                    Card cardBought = null;
                    if (typeToBuy != null) {
                        cardBought = SellCard(State.CurrentPlayer, typeToBuy);
                    } else {
                        // If no card type is selected, skip all remaining buys.
                        State.CurrentPlayer.RemainingBuys = 0;
                    }

                    Logger.Instance.RecordBuy(this, State.CurrentPlayer, cardBought);

                    break;

                case Phase.ADVANCE:

                    AssertCardSelected(false);
                    AssertPendingActionAvailable(false);
                    AssertRemainingActionsAvailable(false);
                    AssertRemainingBuysAvailable(false);

                    int currentTurn = State.TurnNumber;
                    State.CurrentPlayer.EndTurn();
                    State.AdvanceTurn();
                    State.CurrentPlayer.StartTurn();

                    Logger.Instance.RecordScoresForTurn(this, currentTurn);

                    break;

                case Phase.END:

                    AssertCardSelected(false);
                    AssertPendingActionAvailable(false);
                    AssertRemainingActionsAvailable(false);
                    AssertRemainingBuysAvailable(false);

                    Logger.Instance.RecordScoresForTurn(this, State.TurnNumber);

                    break;

                case Phase.DONE:
                    break;

                default:
                    break;
            }

            State.TransitionPhase();
        }

        #endregion

        #region Card Management Methods

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

        #endregion

        #region Clone and Equality Methods

        public object Clone() {
            return new Game(this);
        }

        public override bool Equals(object obj) {
            Game game = obj as Game;
            if (game == null) {
                return false;
            }

            if (this._orderedPlayerList.Count != game._orderedPlayerList.Count) {
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

            if (!game._state.Equals(this._state)) {
                return false;
            }

            return true;
        }

        public override int GetHashCode() {
            int code = this._orderedPlayerList.Count.GetHashCode();

            for (int playerIndex = 0; playerIndex < this._orderedPlayerList.Count; playerIndex++) {
                code = code ^ this._orderedPlayerList[playerIndex].GetHashCode();
            }

            code = code ^ this._cardsByTypeId.Count.GetHashCode();

            foreach (KeyValuePair<int, Deck> entry in this._cardsByTypeId) {
                code = code ^ entry.Key.GetHashCode();
                code = code ^ this._cardsByTypeId[entry.Key].GetHashCode();
            }

            code = code ^ this._state.GetHashCode();

            return code;
        }

        #endregion

        #endregion

        #region Utility Methods

        #region Setup Methods

        private void SetUpPlayerIndex(IList<Player> orderedPlayerList) {
            _orderedPlayerList = new List<Player>(orderedPlayerList);
            _playersById = new Dictionary<int, Player>();
            _playersByName = new Dictionary<string, Player>(StringComparer.InvariantCultureIgnoreCase);

            for (int playerIndex = 0; playerIndex < _orderedPlayerList.Count; playerIndex++) {
                _playersById[_orderedPlayerList[playerIndex].Id] = _orderedPlayerList[playerIndex];
                _playersByName[_orderedPlayerList[playerIndex].Name] = _orderedPlayerList[playerIndex];
            }
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

        #endregion

        #region Card Management Utility Methods

        private void TransferCardOwner(Card card, ICardOwner newOwner, CardDestination destination) {
            ICardOwner currentOwner = card.OwnerId.HasValue
              ? this.GetPlayerById(card.OwnerId.Value) as ICardOwner : this;

            currentOwner.ReleaseCard(card);
            newOwner.AcquireCard(card, destination);

            card.OwnerId = (newOwner != this) ? (newOwner as Player).Id : (int?) null;
        }

        #endregion

        #region Assertion Utility Methods

        private void AssertCardSelected(bool shouldCardBeSelected) {
            string assertMessage = string.Format(
              "Game should {0} start the {1} phase with a card selected.",
              shouldCardBeSelected ? "always" : "never",
              State.Phase.ToString()); 
            Debug.Assert(((State.SelectedPlay != null) == shouldCardBeSelected), assertMessage);
        }

        private void AssertPendingActionAvailable(bool shouldPendingActionBeAvailable) {
            string assertMessage = string.Format(
              "Game should {0} start the {1} phase with a next pending action.",
              shouldPendingActionBeAvailable ? "always" : "never",
              State.Phase.ToString()); 
            Debug.Assert((State.HasNextPendingAction == shouldPendingActionBeAvailable), assertMessage);

            if(shouldPendingActionBeAvailable) {
                Debug.Assert(
                  State.NextPendingAction.TargetSelectorId.HasValue, 
                  "Next pending action should always have a valid target selector."
                );
            }
        }

        private void AssertRemainingActionsAvailable(bool shouldRemainingActionsBeAvailable) {
            string assertMessage = string.Format(
              "Game should {0} start the {1} phase with one or more action(s) remaining.",
              shouldRemainingActionsBeAvailable ? "always" : "never",
              State.Phase.ToString()); 
            Debug.Assert(
              ((State.CurrentPlayer.RemainingActions > 0) == shouldRemainingActionsBeAvailable), 
              assertMessage
            );
        }

        private void AssertRemainingBuysAvailable(bool shouldRemainingBuysBeAvailable) {
            string assertMessage = string.Format(
              "Game should {0} start the {1} phase with one or more buy(s) remaining.",
              shouldRemainingBuysBeAvailable ? "always" : "never",
              State.Phase.ToString()); 
            Debug.Assert(((State.CurrentPlayer.RemainingBuys > 0) == shouldRemainingBuysBeAvailable), assertMessage);
        }

        #endregion

        #region Calculation Utility Methods

        private static bool IsGameFinished(IDictionary<int, Deck> cardsByTypeId) {
            IDictionary<int, int> emptyCardSetsByWeight = new Dictionary<int, int>();
            foreach (int cardTypeId in cardsByTypeId.Keys) {
                if (cardsByTypeId[cardTypeId].Size == 0) {
                    int weight = ActionRegistry.Instance.GetCardTypeById(cardTypeId).EndOfGameWeight;
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

                CardType type = ActionRegistry.Instance.GetCardTypeById(typeId);
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

        #endregion

        #endregion

    }
}