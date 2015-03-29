using KingdomGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace KingdomGame.Driver {

    public class Program {

        private static Set<int> _humanPlayerIds = new Set<int>();

        private delegate void DisplayGameStatusOperation(Game game, Player player);

        private static IDictionary<string, Pair<string, DisplayGameStatusOperation>> CommonMenuOptions 
          = new Dictionary<string, Pair<string, DisplayGameStatusOperation>>() {
            {"c", new Pair<string, DisplayGameStatusOperation>("Display card text", PrintCardText)},
            {"t", new Pair<string, DisplayGameStatusOperation>("Display turn details", PrintTurnDetails)},
            {"h", new Pair<string, DisplayGameStatusOperation>("Display current hand", PrintCurrentHand)},
            {"s", new Pair<string, DisplayGameStatusOperation>("Display current score", PrintCurrentScore)},
            {"a", new Pair<string, DisplayGameStatusOperation>("Display available cards by type", PrintAvailableCardCounts)},
            {"q", new Pair<string, DisplayGameStatusOperation>("Quit current game", TerminateGame)}
        };

        public static void Main(string[] args) {
            // Todo - (MT): Make this configurable:
            string configFilePath = string.Format(@"{0}\BasicTypes.xml", Environment.CurrentDirectory);
            Game game = InitializeGame(configFilePath);

            // Todo - (MT): Force this to be a player specified by a menu option.
            SetupHumanPlayer(game, game.GetPlayerById(1));

            while (!game.IsFinished) {
                if (!IsPlayerHuman(game.State.CurrentPlayer)) {
                    ExecuteComputerPlayerTurn(game);
                }
                else {
                    ExecuteHumanPlayerTurn(game);
                }
            }

            TerminateGame(game, game.State.CurrentPlayer);
        }

        #region Helper Methods

        #region Game Config Helpers

        private static Game InitializeGame(string configFilePath) {
            // Todo - (MT): Make this block entirely read out of the config file path, including the type path:
            ActionRegistry.Instance.InitializeRegistry(configFilePath);
            IList<Player> players = new List<Player>() { new Player("Player 1"), new Player("Player 2")};
            Game game = new Game(players);
            Logger.Instance.TrackGame(game);

            int copperTypeId = ActionRegistry.Instance.GetCardTypeByName("Copper").Id;
            int estateTypeId = ActionRegistry.Instance.GetCardTypeByName("Estate").Id;
            IDictionary<int, int> playerCardCountsByTypeId = new Dictionary<int, int>();
            playerCardCountsByTypeId[copperTypeId] = 7;
            playerCardCountsByTypeId[estateTypeId] = 3;

            bool randomizePlayers = true;
            // End Todo Block

            game.StartGame(playerCardCountsByTypeId, randomizePlayers);
            return game;
        }

        private static void SetupHumanPlayer(Game game, Player player) {
            _humanPlayerIds.Add(player.Id);

            PromptedPlaySelectionStrategy playSelectionStrategy = new PromptedPlaySelectionStrategy();
            playSelectionStrategy.PlaySelectionPromptRequired += new PlaySelectionPromptEventHandler(ExecuteHumanPlayerPlay);
            player.Strategy.PlaySelectionStrategy = playSelectionStrategy;

            PromptedTargetSelectionStrategy targetSelectionStrategy = new PromptedTargetSelectionStrategy();
            targetSelectionStrategy.TargetSelectionPromptRequired += new TargetSelectionPromptEventHandler(ExecuteHumanPlayerAction);
            player.Strategy.TargetSelectionStrategy = targetSelectionStrategy;

            PromptedBuySelectionStrategy buySelectionStrategy = new PromptedBuySelectionStrategy();
            buySelectionStrategy.BuySelectionPromptRequired += new BuySelectionPromptEventHandler(ExecuteHumanPlayerBuy);
            player.Strategy.BuySelectionStrategy = buySelectionStrategy;
        }

        private static bool IsPlayerHuman(Player player) {
            return (player != null && _humanPlayerIds.Contains(player.Id));
        }

        #endregion

        #region Game Execution Helpers

        private static void ExecuteComputerPlayerTurn(Game game) {
            game.PlayTurn();
            GameHistory.Turn lastTurn = Logger.Instance.GetLastTurn(game);

            Console.WriteLine(string.Format("Turn number {0} (Current Player is {1}):\n", lastTurn.Number, lastTurn.Player.Name));

            Console.WriteLine("\tStarting hand:");
            Program.PrintHand(lastTurn.InitialHand);
            Console.WriteLine();

            PrintPlays(lastTurn.Plays);

            Console.WriteLine("\tFinal hand:");
            Program.PrintHand(lastTurn.FinalHand);
            Console.WriteLine();

            PrintBuys(lastTurn.Buy);
            Console.WriteLine();

            Console.WriteLine(string.Format("\tEnd of turn {0} score:", lastTurn.Number));
            PrintScore(lastTurn);
            Console.WriteLine("\n");
        }

        private static void ExecuteHumanPlayerTurn(Game game) {

            int startingTurn = game.State.TurnNumber;
            Console.WriteLine(string.Format("Turn number {0} (Current Player is {1}):\n", startingTurn, game.State.CurrentPlayer.Name));

            Console.WriteLine("\tStarting hand:");
            Program.PrintHand(game.State.CurrentPlayer.Hand);
            Console.WriteLine();
            
            do {

                switch (game.State.Phase) {
                    case Game.Phase.PLAY:
                        game.PlayPhase();
                        break;

                    case Game.Phase.ACTION:

                        if (game.State.SelectedPlay != null) {

                            while(game.State.HasNextPendingAction) {
                                IAction actionToPlay = game.State.NextPendingAction;

                                if (
                                  actionToPlay.TargetSelectorId.HasValue 
                                  && !IsPlayerHuman(game.GetPlayerById(actionToPlay.TargetSelectorId.Value))
                                ) {
                                    game.PlayStep();
                                    GameHistory.Action target = Logger.Instance.GetLastAction(game);
                                    PrintAction(target);
                                    Console.WriteLine();
                                }
                                else {
                                    game.PlayStep();
                                }
                            }
                        }

                        break;

                    case Game.Phase.BUY:
                        Console.WriteLine("\tFinal hand:");
                        Program.PrintHand(game.State.CurrentPlayer.Hand);
                        Console.WriteLine();

                        while (game.State.Phase == Game.Phase.BUY) {
                            game.PlayStep();
                        }

                        GameHistory.Buy lastBuy = Logger.Instance.GetLastBuy(game);
                        PrintBuys(lastBuy);
                        Console.WriteLine();

                        break;

                    case Game.Phase.ADVANCE:
                        game.PlayPhase();
                        break;

                    case Game.Phase.END:
                        break;
                }

            } while (game.State.TurnNumber == startingTurn && game.State.Phase != Game.Phase.END);

            GameHistory.Turn lastTurn = Logger.Instance.GetLastTurn(game);
            Console.WriteLine(string.Format("\tEnd of turn {0} score:", lastTurn.Number));
            PrintScore(lastTurn);
            Console.WriteLine("\n");
        }

        private static void ExecuteHumanPlayerPlay(object sender, PlaySelectionPromptEventArgs args) {
            if (!IsPlayerHuman(args.Game.State.CurrentPlayer)) {
                args.SelectedPlay = null;
                return;
            }

            int optionCounter = 2;
            IDictionary<string, string> optionPromptsByIndex = new Dictionary<string, string>() {{"1", "<no action>"}};
            IDictionary<string, Card> optionsByIndex = new Dictionary<string, Card>() { { "1", null } };

            foreach (Card card in args.Game.State.CurrentPlayer.Hand) {
                if (card.Type.Class == CardType.CardClass.ACTION) {
                    string optionPrompt = optionCounter.ToString();
                    optionsByIndex[optionPrompt] = card;
                    optionPromptsByIndex[optionPrompt] = string.Format("{0} ({1})", card.Type.Name, card.Id);
                    optionCounter++;
                }
            }

            Card selectedPlay;
            if (optionsByIndex.Count == 1) {
                Console.WriteLine("Skipping action selection because no action cards are in the hand\n");
                selectedPlay = null;
            }
            else {
                string promptMessage = "Please select a card to play from the following menu:";
                selectedPlay = optionsByIndex[PromptUserForOptionInput(
                  args.Game, 
                  args.Game.State.CurrentPlayer, 
                  promptMessage, 
                  optionPromptsByIndex
                )];
            }

            args.SelectedPlay = selectedPlay;
        }

        private static void ExecuteHumanPlayerAction(Object sender, TargetSelectionPromptEventArgs args) {

            IList<ITargetable> validTargets = args.CurrentAction.GetAllIndividuallyValidTargets(args.Game.State.SelectedPlay, args.Game);

            if (args.CurrentAction.MinTargets <= validTargets.Count) {
                int optionCounter = 1;
                Console.WriteLine("Valid target summary list:");
                foreach (ITargetable target in validTargets) {
                    Console.WriteLine(string.Format("\t{0}: {1}", optionCounter, target.ToString()));
                    optionCounter++;
                }

                Console.WriteLine();

                List<ITargetable> selectedTargets;
                if (args.CurrentAction.MinTargets == validTargets.Count) {
                    selectedTargets = new List<ITargetable>(validTargets);
                    Console.WriteLine("Automatically selected targets because only one valid option was available\n");
                }
                else if (args.CurrentAction.AllValidTargetsRequired) {
                    selectedTargets = new List<ITargetable>(validTargets);
                    Console.WriteLine("Automatically selected targets because all valid options are required\n");
                }
                else {

                    bool validTargetSpecified = false;
                    do {

                        int selectedTargetCount;
                        int maxValidTargets = Math.Min(args.CurrentAction.MaxTargets, validTargets.Count);
                        if (args.CurrentAction.MinTargets < maxValidTargets) {
                            selectedTargetCount = PromptUserForNumericInput(
                              args.Game,
                              args.Game.State.CurrentPlayer,
                              string.Format(
                                "Please enter the number of targets you wish to select for {0}:", 
                                args.CurrentAction.ActionDescription
                              ),
                              args.CurrentAction.MinTargets,
                              Math.Min(args.CurrentAction.MaxTargets, validTargets.Count)
                            );
                        }
                        else {
                            selectedTargetCount = maxValidTargets;
                        }

                        selectedTargets = new List<ITargetable>();
                        List<ITargetable> remainingTargets = new List<ITargetable>(validTargets);
                        for (int targetNumber = 1; targetNumber <= selectedTargetCount; targetNumber++) {
                            optionCounter = 1;
                            IDictionary<string, string> optionPromptsByIndex = new Dictionary<string, string>();
                            IDictionary<string, ITargetable> optionsByIndex = new Dictionary<string, ITargetable>();

                            foreach (ITargetable target in remainingTargets) {
                                string optionPrompt = optionCounter.ToString();
                                optionsByIndex[optionPrompt] = target;
                                optionPromptsByIndex[optionPrompt] = target.ToString();
                                optionCounter++;
                            }

                            ITargetable selectedTarget = optionsByIndex[PromptUserForOptionInput(
                              args.Game,
                              args.Game.State.CurrentPlayer,
                              string.Format(
                                "Please select target #{0} (of {1}):", 
                                targetNumber, 
                                selectedTargetCount
                              ),
                              optionPromptsByIndex
                            )];

                            remainingTargets.Remove(selectedTarget);
                            selectedTargets.Add(selectedTarget);
                        }

                        validTargetSpecified = args.CurrentAction.IsTargetValid(
                          selectedTargets, 
                          args.CurrentPlay, 
                          args.Game
                        );

                        if(!validTargetSpecified) {
                            Console.WriteLine(string.Format(
                              "Invalid target selection for {0}, please try again:\n", 
                              args.CurrentAction.ActionDescription)
                            );
                        }

                    } while (!validTargetSpecified);
                }

                args.SelectedTargets = selectedTargets;
            }
            else {
                Console.WriteLine(string.Format(
                  "No valid targets available for {0}, skipping action...\n", 
                  args.CurrentAction.ActionDescription
                ));

                args.SelectedTargets = new List<ITargetable>();
            }
        }

        private static void ExecuteHumanPlayerBuy(object sender, BuySelectionPromptEventArgs args) {
            int optionCounter = 2;
            IDictionary<string, string> optionPromptsByIndex = 
                new Dictionary<string, string>() { { "1", "<no buy>" } };
            IDictionary<string, CardType> optionsByIndex = 
                new Dictionary<string, CardType>() { { "1", null } };

            IList<CardType> buyOptions = GetValidBuyOptions(args.Game);
            foreach (CardType buyOption in buyOptions) {
                string optionPrompt = optionCounter.ToString();
                optionsByIndex[optionPrompt] = buyOption;
                optionPromptsByIndex[optionPrompt] = 
                    string.Format("{0} ({1} cost, {2} remaining)",
                    buyOption.Name,
                    buyOption.Cost,
                    args.Game.GetCardsByType(buyOption).Count
                );
                optionCounter++;
            }

            CardType selectedBuyOption;
            if (optionsByIndex.Count == 1) {
                Console.WriteLine("Skipping buy selection because there are no valid options for buying\n");
                selectedBuyOption = null;
            }
            else {
                string promptMessage = string.Format(
                    "Please select a buy option from the following menu ({0} money, {1} buy(s) remain):", 
                    args.Game.State.CurrentPlayer.RemainingMoney, 
                    args.Game.State.CurrentPlayer.RemainingBuys
                );

                selectedBuyOption = optionsByIndex[PromptUserForOptionInput(
                    args.Game, 
                    args.Game.State.CurrentPlayer, 
                    promptMessage, 
                    optionPromptsByIndex
                )];
            }

            args.SelectedBuy = selectedBuyOption;
        }

        #endregion

        #region Game Input Helpers

        private static string PromptUserForOptionInput(
          Game game, 
          Player player, 
          string promptMessage, 
          IDictionary<string, string> menuCommands
        ) {
            do {

                Console.WriteLine(promptMessage);
                foreach (string key in menuCommands.Keys) {
                    Console.WriteLine(string.Format("{0}: {1}", key, menuCommands[key]));
                }
                foreach (string key in Program.CommonMenuOptions.Keys) {
                    Console.WriteLine(string.Format("{0}: {1}", key, CommonMenuOptions[key].First));
                }

                Console.Write("\nSelected Option: ");
                string response = Console.ReadLine();
                response = response.Trim().ToLower();
                Console.WriteLine();

                if (menuCommands.ContainsKey(response)) {
                    return response;
                }
                else if (Program.CommonMenuOptions.ContainsKey(response)) {
                    DisplayGameStatusOperation operation = CommonMenuOptions[response].Second;
                    if (operation != null) {
                        operation.Invoke(game, player);
                    }
                }
                else {
                    Console.WriteLine(string.Format(
                      "Invalid response '{0}' specified, please enter a valid command.\n",
                      response
                    ));
                }

            } while (true);
        }

        private static int PromptUserForNumericInput(
          Game game,
          Player player,
          string promptMessage,
          int minValue,
          int maxValue
        ) {
            do {

                Console.WriteLine(promptMessage);
                Console.WriteLine(string.Format("Min Value: {0}", minValue));
                Console.WriteLine(string.Format("Max Value: {0}", maxValue));
                foreach (string key in Program.CommonMenuOptions.Keys) {
                    Console.WriteLine(string.Format("{0}: {1}", key, CommonMenuOptions[key].First));
                }

                Console.Write("\nSelected Option: ");
               string response = Console.ReadLine();
                response = response.Trim().ToLower();
                Console.WriteLine();

                int selectedValue;
                if (
                  int.TryParse(response, out selectedValue) 
                  && (selectedValue >= minValue) 
                  && (selectedValue <= maxValue)
                ) {
                    return selectedValue;
                }
                else if (Program.CommonMenuOptions.ContainsKey(response)) {
                    DisplayGameStatusOperation operation = CommonMenuOptions[response].Second;
                    if (operation != null) {
                        operation.Invoke(game, player);
                    }
                }
                else {
                    Console.WriteLine(string.Format(
                      "Invalid response '{0}' specified, please enter a valid command.\n",
                      response
                    ));
                }

            } while (true);
        }

        #endregion

        #region Game Output Helpers

        // Refactor - (MT): Make the base number of indents configurable.
        private static void PrintHand(IList<GameHistory.CardInfo> hand) {
            foreach (GameHistory.CardInfo cardInfo in hand) {
                Console.WriteLine(string.Format("\t\t{0} ({1})", cardInfo.TypeName, cardInfo.Id));
            }
        } 

        private static void PrintHand(IList<Card> hand) {
            foreach (Card card in hand) {
                Console.WriteLine(string.Format("\t\t{0} ({1})", card.Type.Name, card.Id));
            }
        } 

        private static void PrintPlays(IList<GameHistory.Play> plays) {
            int playNumber = 1;
            foreach (GameHistory.Play play in plays) {
                PrintPlay(play, playNumber);
                playNumber++;
                Console.WriteLine();
            }
        }

        private static void PrintPlay(GameHistory.Play play, int playNumber) {
            Console.WriteLine(string.Format("\tPlay #{0}: {1} ({2})", playNumber, play.Card.TypeName, play.Card.Id));

            int targetNumber = 1;
            foreach (GameHistory.Action target in play.Actions) {
                PrintAction(target, targetNumber);
                targetNumber++;
            }
        }

        private static void PrintAction(GameHistory.Action action) {
            Console.WriteLine(string.Format("\tAction summary for {0}:", action.Command.DisplayName));
            foreach (ITargetable target in action.Targets) {
                Console.WriteLine(string.Format("\t\t{0}", target.ToString(null)));
            }
        }

        private static void PrintAction(GameHistory.Action action, int targetNumber) {
            Console.WriteLine(string.Format("\t\tAction #{0}: {1}", targetNumber, action.Command.DisplayName));
            foreach (ITargetable target in action.Targets) {
                Console.WriteLine(string.Format("\t\t\t{0}", target.ToString(null)));
            }
        }

        private static void PrintActionTargets<T>(IAction action, IList<T> targets) {
            if (targets.Count > 0) {
                Console.WriteLine(string.Format("\tSelected target summary for {0}:", action.DisplayName));
                int targetCounter = 1;
                foreach (T target in targets) {
                    Console.WriteLine(string.Format("\t\t{0}: ({1})", targetCounter, target.ToString()));
                }
            }
            else {
                Console.WriteLine(string.Format("\tSelected target summary for {0}: No targets", action.DisplayName));
            }

            Console.WriteLine();
        }

        private static void PrintBuys(GameHistory.Buy buy) {
            if (buy == null || buy.CardsBought.Count == 0) {
                Console.WriteLine("\tBuy phase - 0 buy(s)");
                return;
            }

            int buyNumber = 1;
            Console.WriteLine(string.Format("\tBuy phase - {0} buy(s):", buy.Cards.Count));
            foreach (GameHistory.CardInfo cardInfo in buy.CardsBought) {
                Console.WriteLine(string.Format(
                  "\t\tBuy #{0}: {1} ({2})", 
                  buyNumber, 
                  cardInfo.TypeName, 
                  cardInfo.Id
                ));
            }
        }

        private static void PrintScore(GameHistory.Turn turn) {
            foreach (GameHistory.ScoreInfo score in turn.Scores) {
                Console.WriteLine(string.Format("\t\t{0}: {1}", score.PlayerName, score.Score));
            }
        }

        private static void PrintCurrentScore(Game game, Player player) {
            Console.WriteLine("\tCurrent score:");
            foreach (Player gamePlayer in game.Players) {
                Console.WriteLine(string.Format("\t\t{0}: {1}", gamePlayer.Name, gamePlayer.Score));
            }
            Console.WriteLine();
        }

        private static void PrintAvailableCardCounts(Game game, Player player) {
            Console.WriteLine("\tCards available:");
            foreach (CardType cardType in ActionRegistry.Instance.CardTypes) {
                Console.WriteLine(string.Format("\t\t{0}: {1}", cardType.Name, game.GetCardsByType(cardType).Count));
            }
            Console.WriteLine();
        }

        private static void PrintCurrentHand(Game game, Player player) {
            Console.WriteLine("\tCurrent hand:");
            PrintHand(player.Hand);
            Console.WriteLine();
        }

        private static void PrintCardText(Game game, Player player) {
            CardType selectedType = null;
            int optionCounter = 1;
            IDictionary<string, string> optionPromptsByIndex = new Dictionary<string, string>();
            IDictionary<string, CardType> optionsByIndex = new Dictionary<string, CardType>();

            foreach (CardType type in ActionRegistry.Instance.CardTypes) {
                if (game.GetCardsByType(type) != null) {
                    string optionPrompt = optionCounter.ToString();
                    optionsByIndex[optionPrompt] = type;
                    optionPromptsByIndex[optionPrompt] = string.Format("{0}", type.Name);
                    optionCounter++;
                }
            }

            string promptMessage = "Please select a card type to obtain text for from the following menu:";
            selectedType = optionsByIndex[PromptUserForOptionInput(
              game, 
              game.State.CurrentPlayer, 
              promptMessage, 
              optionPromptsByIndex
            )];

            if (selectedType != null) {
                Console.WriteLine(string.Format("\tCard type {0} text:", selectedType.Name));
                if (!string.IsNullOrEmpty(selectedType.CardText)) {
                    Console.WriteLine(string.Format("\t\t{0}", selectedType.CardText));
                }
                else {
                    Console.WriteLine(string.Format("\t\tNo text avaliable for {0}", selectedType.Name));
                }
                Console.WriteLine(string.Format("\t\tCost: {0}", selectedType.Cost));
                Console.WriteLine(string.Format("\t\tMonetary Value: {0}", selectedType.MonetaryValue));
                Console.WriteLine(string.Format("\t\tVictory Value: {0}", selectedType.VictoryValue));
                Console.WriteLine();
            }
        }

        private static void PrintTurnDetails(Game game, Player player) {
            Console.WriteLine(string.Format("\tTurn {0} summary:", game.State.TurnNumber));
            Console.WriteLine(string.Format("\t\tCurrent Player: {0} ({1})", game.State.CurrentPlayer.Name, game.State.CurrentPlayer.Id));
            Console.WriteLine(string.Format("\t\tRemaining Actions: {0}", game.State.CurrentPlayer.RemainingActions));
            Console.WriteLine(string.Format("\t\tRemaining Buys: {0}", game.State.CurrentPlayer.RemainingBuys));
            Console.WriteLine(string.Format("\t\tRemaining Money: {0}", game.State.CurrentPlayer.RemainingMoney));
            Console.WriteLine();
        }

        private static void SaveGameHistory(Game game, string directory, string filePrefix, string timestampFormat) {
            string outputPath = string.Format(
              @"{0}\{1}{2}.xml", 
              directory,
              filePrefix,
              DateTime.Now.ToString(timestampFormat)
            );

            Logger.Instance.SaveHistory(game, outputPath);
        }

        private static void TerminateGame(Game game, Player player) {
            // Todo - (MT): Make these path components configurable:
            SaveGameHistory(game, Environment.CurrentDirectory, "GameResults", "_yyyy-dd-MM_HHmmss");

            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
            Environment.Exit(0);
        }

        #endregion

        #region Targeting Helpers

        private static IList<CardType> GetValidBuyOptions(Game game) {
            IList<CardType> validBuyOptions = new List<CardType>();

            if (game.State.CurrentPlayer.RemainingBuys > 0) {
                foreach (CardType type in ActionRegistry.Instance.CardTypes) {
                    if (game.GetCardsByType(type).Count > 0 && type.Cost <= game.State.CurrentPlayer.RemainingMoney) {
                        validBuyOptions.Add(type);
                    }
                }
            }

            return validBuyOptions;
        }

        #endregion

        #endregion
    }
}
