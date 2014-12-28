using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace KingdomGame {

    public class Logger {

        private static Logger _instance = new Logger();

        private IDictionary<int, GameHistory> _historyByGame = new Dictionary<int, GameHistory>();

        private Logger() {

        }

        public static Logger Instance {
            get { return _instance; }
        }

        public void TrackGame(Game game) {
            if (!_historyByGame.ContainsKey(game.Id)) {
                _historyByGame[game.Id] = new GameHistory();
            }
        }

        public void UntrackGame(Game game) {
            if (_historyByGame.ContainsKey(game.Id)) {
                _historyByGame.Remove(game.Id);
            }
        }

        public void RecordCardSelection(Game game, Player player, Card card) {
            if (_historyByGame.ContainsKey(game.Id)) {
                GameHistory history = _historyByGame[game.Id];
                if (!history.TurnsByNumber.ContainsKey(game.TurnNumber)) {
                    this.RecordStartOfTurn(game);
                }

                GameHistory.Turn turn = history.TurnsByNumber[game.TurnNumber];
                turn.Plays.Add(new GameHistory.Play(player, card));
            }
        }

        // Refactor - (MT): Turn these three functions into a single generic one.
        public void RecordTargetSelection(Game game, Player player, Card card, IAction action, IList<Player> playerTargets) {
            if (_historyByGame.ContainsKey(game.Id)) {
                GameHistory history = _historyByGame[game.Id];
                if (!history.TurnsByNumber.ContainsKey(game.TurnNumber)) {
                    this.RecordStartOfTurn(game);
                }

                GameHistory.Turn turn = history.TurnsByNumber[game.TurnNumber];
                if (turn.Plays.Count > 0) {
                    GameHistory.Play lastPlay = turn.Plays[turn.Plays.Count - 1];
                    lastPlay.Targets.Add(new GameHistory.Target(player, card, action, playerTargets, null, null));
                }
            }
        }

        public void RecordTargetSelection(Game game, Player player, Card card, IAction action, IList<Card> cardTargets) {
            if (_historyByGame.ContainsKey(game.Id)) {
                GameHistory history = _historyByGame[game.Id];
                if (!history.TurnsByNumber.ContainsKey(game.TurnNumber)) {
                    this.RecordStartOfTurn(game);
                }

                GameHistory.Turn turn = history.TurnsByNumber[game.TurnNumber];
                if (turn.Plays.Count > 0) {
                    GameHistory.Play lastPlay = turn.Plays[turn.Plays.Count - 1];
                    lastPlay.Targets.Add(new GameHistory.Target(player, card, action, null, cardTargets, null));
                }
            }
        }

        public void RecordTargetSelection(
          Game game, 
          Player player, 
          Card card, 
          IAction action, 
          IList<CardType> typeTargets
        ){
            if (_historyByGame.ContainsKey(game.Id)) {
                GameHistory history = _historyByGame[game.Id];
                if (!history.TurnsByNumber.ContainsKey(game.TurnNumber)) {
                    this.RecordStartOfTurn(game);
                }

                GameHistory.Turn turn = history.TurnsByNumber[game.TurnNumber];
                if (turn.Plays.Count > 0) {
                    GameHistory.Play lastPlay = turn.Plays[turn.Plays.Count - 1];
                    lastPlay.Targets.Add(new GameHistory.Target(player, card, action, null, null, typeTargets));
                }
            }
        }
        // End Refactor Block

        public void RecordBuy(Game game, Player player, Card card) {
            if (_historyByGame.ContainsKey(game.Id)) {
                GameHistory history = _historyByGame[game.Id];
                if (!history.TurnsByNumber.ContainsKey(game.TurnNumber)) {
                    this.RecordStartOfTurn(game);
                }

                GameHistory.Turn turn = history.TurnsByNumber[game.TurnNumber];
                if (turn.Buy == null) {
                    turn.Buy = new GameHistory.Buy(player);
                }

                turn.Buy.Cards.Add(card);
            }
        }

        public void RecordEndOfTurn(Game game) {
            if (_historyByGame.ContainsKey(game.Id)) {
                GameHistory history = _historyByGame[game.Id];
                if (history.TurnsByNumber.ContainsKey(game.TurnNumber)) {
                    GameHistory.Turn lastTurn = history.TurnsByNumber[game.TurnNumber];
                    lastTurn.Scores.Clear();
                    foreach (Player player in game.Players) {
                        lastTurn.Scores.Add(new GameHistory.ScoreInfo(player));
                    }
                }
            }
        }

        public void UpdateLastTarget(Game game, IList<Player> playerTargets, IList<Card> cardTargets) {
            if (_historyByGame.ContainsKey(game.Id)) {
                GameHistory history = _historyByGame[game.Id];
                if (history.TurnsByNumber.Count > 0) {
                    int maxTurn = history.TurnsByNumber.Keys.Max();
                    GameHistory.Turn lastTurn = history.TurnsByNumber[maxTurn];
                    if (lastTurn.Plays.Count > 0) {
                        GameHistory.Play lastPlay = lastTurn.Plays[lastTurn.Plays.Count - 1];
                        if (lastPlay.Targets.Count > 0) {
                             GameHistory.Target target = lastPlay.Targets[lastPlay.Targets.Count - 1];
                            target.SetTargets(
                              (playerTargets != null) ? new List<Player>(playerTargets) : new List<Player>(),
                              (cardTargets!= null) ? new List<Card>(cardTargets) : new List<Card>()
                            );
                        }
                    }
                }
            }
        }

        public bool SaveHistory(Game game, string outputPath) {
            GameHistory history = GetHistory(game);
            if (history == null) {
                return false;
            }

            XmlSerializer serializer = new XmlSerializer(history.GetType());
            using(StreamWriter fileWriter = new StreamWriter(outputPath)) {
                serializer.Serialize(fileWriter, history);
            }

            return true;
        }

        public GameHistory GetHistory(Game game) {
            return (_historyByGame.ContainsKey(game.Id)) ? new GameHistory(_historyByGame[game.Id]) : null;
        }

        public GameHistory.Turn GetLastTurn(Game game) {
            if (_historyByGame.ContainsKey(game.Id)) {
                GameHistory history = _historyByGame[game.Id];
                if (history.TurnsByNumber.Count > 0) {
                    int maxTurn = history.TurnsByNumber.Keys.Max();
                    return new GameHistory.Turn(history.TurnsByNumber[maxTurn]);
                }
            }

            return null;
        }

        public GameHistory.Play GetLastPlay(Game game) {
            GameHistory.Turn lastTurn = this.GetLastTurn(game);
            if (lastTurn != null) {
                if (lastTurn.Plays.Count > 0) {
                    return new GameHistory.Play(lastTurn.Plays[lastTurn.Plays.Count - 1]);
                }
            }

            return null;
        }

        public GameHistory.Target GetLastTarget(Game game) {
            GameHistory.Play lastPlay = this.GetLastPlay(game);
            if (lastPlay != null) {
                if (lastPlay.Targets.Count > 0) {
                    return new GameHistory.Target(lastPlay.Targets[lastPlay.Targets.Count - 1]);
                }
            }

            return null;
        }

        public GameHistory.Buy GetLastBuy(Game game) {
            GameHistory.Turn lastTurn = this.GetLastTurn(game);
            if (lastTurn != null) {
                return lastTurn.Buy;
            }

            return null;
        }

        private void RecordStartOfTurn(Game game) {
            if (_historyByGame.ContainsKey(game.Id)) {
                GameHistory history = _historyByGame[game.Id];
                if (!history.TurnsByNumber.ContainsKey(game.TurnNumber)) {
                    history.TurnsByNumber[game.TurnNumber] = new GameHistory.Turn(game.TurnNumber, game.CurrentPlayer);
                }
            }
        }
    }
}
