using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame.Test
{
    public class TestSetup {

        #region Static Members

        private static bool TypesInitialized = false;

        #endregion

        #region Public Properties

        public static CardType CardTypeCopper {
            get {
                TestSetup.InitializeTypes();
                return ActionRegistry.Instance.GetCardTypeByName("Copper");
            }
        }

        public static CardType CardTypeSilver {
            get {
                TestSetup.InitializeTypes();
                return ActionRegistry.Instance.GetCardTypeByName("Silver");
            }
        }

        public static CardType CardTypeGold {
            get {
                TestSetup.InitializeTypes();
                return ActionRegistry.Instance.GetCardTypeByName("Gold");
            }
        }

        public static CardType CardTypeEstate {
            get {
                TestSetup.InitializeTypes();
                return ActionRegistry.Instance.GetCardTypeByName("Estate");
            }
        }

        public static CardType CardTypeDuchy {
            get {
                TestSetup.InitializeTypes();
                return ActionRegistry.Instance.GetCardTypeByName("Duchy");
            }
        }

        public static CardType CardTypeProvince {
            get {
                TestSetup.InitializeTypes();
                return ActionRegistry.Instance.GetCardTypeByName("Province");
            }
        }

        public static CardType CardTypeCellar {
            get {
                TestSetup.InitializeTypes();
                return ActionRegistry.Instance.GetCardTypeByName("Cellar");
            }
        }

        public static CardType CardTypeMoat {
            get {
                TestSetup.InitializeTypes();
                return ActionRegistry.Instance.GetCardTypeByName("Moat");
            }
        }

        public static CardType CardTypeVillage {
            get {
                TestSetup.InitializeTypes();
                return ActionRegistry.Instance.GetCardTypeByName("Village");
            }
        }

        public static CardType CardTypeWorkshop {
            get {
                TestSetup.InitializeTypes();
                return ActionRegistry.Instance.GetCardTypeByName("Workshop");
            }
        }

        public static CardType CardTypeWoodcutter {
            get {
                TestSetup.InitializeTypes();
                return ActionRegistry.Instance.GetCardTypeByName("Woodcutter");
            }
        }

        public static CardType CardTypeMilitia {
            get {
                TestSetup.InitializeTypes();
                return ActionRegistry.Instance.GetCardTypeByName("Militia");
            }
        }

        public static CardType CardTypeRemodel {
            get {
                TestSetup.InitializeTypes();
                return ActionRegistry.Instance.GetCardTypeByName("Remodel");
            }
        }

        public static CardType CardTypeSmithy {
            get {
                TestSetup.InitializeTypes();
                return ActionRegistry.Instance.GetCardTypeByName("Smithy");
            }
        }

        public static CardType CardTypeMine {
            get {
                TestSetup.InitializeTypes();
                return ActionRegistry.Instance.GetCardTypeByName("Mine");
            }
        }

        public static CardType CardTypeMarket {
            get {
                TestSetup.InitializeTypes();
                return ActionRegistry.Instance.GetCardTypeByName("Market");
            }
        }

        #endregion

        #region Static Setup Methods

        public static void InitializeTypes() {
            if (TypesInitialized) {
                return;
            }

            string configFilePath = string.Format(@"{0}\Common\TestTypes.xml", Environment.CurrentDirectory);
            ActionRegistry.Instance.InitializeRegistry(configFilePath);
            TypesInitialized = true;
        }

        public static Game GenerateSimpleGame(int numPlayers) {
            TestSetup.InitializeTypes();

            Dictionary<int, int> gameCardCountsByTypeId = new Dictionary<int,int>();
            gameCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 60;
            gameCardCountsByTypeId[TestSetup.CardTypeSilver.Id] = 40;
            gameCardCountsByTypeId[TestSetup.CardTypeGold.Id] = 30;
            gameCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 24;
            gameCardCountsByTypeId[TestSetup.CardTypeDuchy.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeProvince.Id] = 12;
            gameCardCountsByTypeId[TestSetup.CardTypeCellar.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeMoat.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeVillage.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeWorkshop.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeWoodcutter.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeMilitia.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeRemodel.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeSmithy.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeMine.Id] = 10;
            gameCardCountsByTypeId[TestSetup.CardTypeMarket.Id] = 10;

            Dictionary<int, int> playerCardCountsByTypeId = new Dictionary<int,int>();
            playerCardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 7;
            playerCardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;

            return TestSetup.GenerateStartingGame(numPlayers, gameCardCountsByTypeId, playerCardCountsByTypeId);
        }

        public static Game GenerateStartingGame(
          int numPlayers, 
          IDictionary<int, int> gameCardCountsByTypeId,
          IDictionary<int, int> playerCardCountsByTypeId
        ) {
            return TestSetup.GenerateStartingGame(numPlayers, gameCardCountsByTypeId, playerCardCountsByTypeId, null);
        }

        public static Game GenerateStartingGame(
          int numPlayers, 
          IDictionary<int, int> gameCardCountsByTypeId,
          IDictionary<int, int> playerCardCountsByTypeId,
          IDictionary<int, int> handCardCountsByTypeId
        ) {
            if (numPlayers < 2 || numPlayers > 4) {
                throw new ArgumentException("Game must contain between 2 and 4 players.");
            }

            IList<Player> players = new List<Player>();
            for (int playerIndex = 1; playerIndex <= numPlayers; playerIndex++) {
                players.Add(new Player("Player " + playerIndex));
            }

            Game game = new Game(players, gameCardCountsByTypeId);
            game.StartGame(playerCardCountsByTypeId, handCardCountsByTypeId, false);
            return game;
        }

        public static Deck GenerateSimpleDeck() {
            TestSetup.InitializeTypes();

            Dictionary<int, int> cardCountsByTypeId = new Dictionary<int,int>();
            cardCountsByTypeId[TestSetup.CardTypeCopper.Id] = 7;
            cardCountsByTypeId[TestSetup.CardTypeEstate.Id] = 3;

            return TestSetup.GenerateSimpleDeck(cardCountsByTypeId);
        }

        public static Deck GenerateSimpleDeck(IDictionary<int, int> cardCountsByTypeId) {
            Deck deck = new Deck();
            foreach(int cardTypeId in cardCountsByTypeId.Keys) {
                CardType cardType = ActionRegistry.Instance.GetCardTypeById(cardTypeId);

                for(int cardTypeIndex = 0; cardTypeIndex < cardCountsByTypeId[cardTypeId]; cardTypeIndex++) {
                    Card card = new Card(cardType, null);
                    deck.Add(card);
                }
            }

            return deck;
        }

        #endregion
    }
}
