using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KingdomGame {

    public class CardTypeRegistry {
        
        private static CardTypeRegistry _instance = null;

        public static CardTypeRegistry Instance {
            get {
                if (_instance == null) {
                    _instance = new CardTypeRegistry();
                }

                return _instance;
            }
        }

        private IDictionary<int, CardType> _cardTypesById;
        private IDictionary<string, CardType> _cardTypesByName;
        private IDictionary<int, int> _defaultQuantitiesByTypeId;

        private CardTypeRegistry() {
            ResetCardTypes();
        }

        public CardType RegisterCardType(
            string name, 
            CardType.CardClass cardClass, 
            int cost, 
            int victoryValue, 
            int monetaryValue,
            int endOfGameWeight,
            int defaultQuantity,
            string cardText,
            IEnumerable<CardProperty> properties,
            IEnumerable<IAction> actions
        ) {
            if (_cardTypesByName.ContainsKey(name)) {
                throw new ArgumentException("A card type with name " + name + " already exists.");
            }

            if (cardClass != CardType.CardClass.VICTORY && victoryValue != 0) {
                throw new ArgumentException("A card type with class " + cardClass.ToString() + " cannot have a non-zero victory value.");
            }

            if (cardClass != CardType.CardClass.TREASURE && monetaryValue != 0) {
                throw new ArgumentException("A card type with class " + cardClass.ToString() + " cannot have a non-zero monetary value.");
            }

            CardType type = new CardType(
              name, 
              cardClass, 
              cost, 
              victoryValue, 
              monetaryValue, 
              endOfGameWeight,
              cardText,
              properties, 
              actions
            );

            _cardTypesById.Add(type.Id, type);
            _cardTypesByName.Add(type.Name, type);
            _defaultQuantitiesByTypeId.Add(type.Id, defaultQuantity);

            return type;
        }

        public CardType GetCardTypeById(int id) {
            return _cardTypesById.ContainsKey(id) ? _cardTypesById[id] : null;
        }

        public CardType GetCardTypeByName(string name) {
            return _cardTypesByName.ContainsKey(name) ? _cardTypesByName[name] : null;
        }

        public int GetDefaultQuantityByTypeId(int id) {
            return _defaultQuantitiesByTypeId.ContainsKey(id) ? _defaultQuantitiesByTypeId[id] : 0;
        }

        public IList<CardType> CardTypes {
            get { return new List<CardType>(_cardTypesById.Values); }
        }

        public IDictionary<int, int> DefaultCardCountsByType {
            get {
                IDictionary<int, int> defaultGameCardCountsByType = new Dictionary<int, int>();
                foreach(CardType type in CardTypes) {
                    defaultGameCardCountsByType[type.Id] = GetDefaultQuantityByTypeId(type.Id);
                }

                return defaultGameCardCountsByType;
            }
        }

        public void InitializeCardTypes(string configFilePath) {
            ResetCardTypes();

            using (XmlReader reader = XmlReader.Create(new StringReader(File.ReadAllText(configFilePath))))
            {
                IDictionary<string, Assembly> assembliesByKey;
                reader.ReadToFollowing("assemblies");
                using (XmlReader assembliesReader = reader.ReadSubtree()) {
                    assembliesByKey = ParseAssembliesData(assembliesReader);
                }

                IDictionary<string, CardType> cardTypesByKey;
                reader.ReadToFollowing("types");
                using (XmlReader cardTypesReader = reader.ReadSubtree()) {
                    cardTypesByKey = ParseCardTypesData(cardTypesReader, assembliesByKey);
                }

                // Todo - (MT): Return the Card Types table, or something else?
            }
        }

        public void ResetCardTypes() {
            // Todo - (MT): Place a reset call to card properties as well?
            _cardTypesById = new Dictionary<int, CardType>();
            _cardTypesByName = new Dictionary<string, CardType>(StringComparer.InvariantCultureIgnoreCase);
            _defaultQuantitiesByTypeId = new Dictionary<int, int>();
        }

        private IDictionary<string, Assembly> ParseAssembliesData(XmlReader assembliesReader) {
            // Todo - (MT): Add better parsing and error handling.
            IDictionary<string, Assembly> assembliesByKey = new Dictionary<string, Assembly>();
            while (assembliesReader.ReadToFollowing("assembly")) {
                using (XmlReader pathReader = assembliesReader.ReadSubtree()) {
                    assembliesReader.MoveToAttribute("key");
                    string key = assembliesReader.Value;
                    pathReader.ReadToFollowing("path");
                    string assemblyPath = pathReader.ReadElementContentAsString();

                    Assembly assembly = Assembly.LoadFile(Path.GetFullPath(assemblyPath));
                    assembliesByKey[key] = assembly;
                }
            }

            return assembliesByKey;
        }

        private IDictionary<string, CardType> ParseCardTypesData(
          XmlReader cardTypesReader, 
          IDictionary<string, Assembly> assembliesByKey
        ) {
            // Todo - (MT): Add better parsing and error handling.
            IDictionary<string, CardType> cardTypesByKey = new Dictionary<string, CardType>();
            while (cardTypesReader.ReadToFollowing("type")) {
                using (XmlReader cardTypeReader = cardTypesReader.ReadSubtree()) {
                    cardTypesReader.MoveToAttribute("key");
                    string key = cardTypesReader.Value;
                    CardType cardType = ParseCardTypeData(cardTypeReader, key, assembliesByKey);
                    cardTypesByKey[key] = cardType;
                }
            }

            return cardTypesByKey;
        }

        private CardType ParseCardTypeData(
          XmlReader cardTypeReader, 
          string key,           
          IDictionary<string, Assembly> assembliesByKey
        ) {
            // Default values to use if not specified in configuration file:
            CardType.CardClass cardClass = CardType.CardClass.ACTION;
            int cost = 0;
            int victoryValue = 0;
            int monetaryValue = 0;
            int endOfGameWeight = 3;
            int defaultQuantity = 10;
            string cardText = null;
            IList<CardProperty> properties = new List<CardProperty>();
            IList<IAction> actions = new List<IAction>();

            while (cardTypeReader.Read()) {
                if (cardTypeReader.NodeType == XmlNodeType.Element) {
                    switch (cardTypeReader.Name.ToLower()) {

                        case "class":
                            Enum.TryParse<CardType.CardClass>(cardTypeReader.ReadElementContentAsString(), out cardClass);
                            break;

                        case "cost":
                            cost = cardTypeReader.ReadElementContentAsInt();
                            break;

                        case "victoryvalue":
                            victoryValue = cardTypeReader.ReadElementContentAsInt();
                            break;

                        case "monetaryvalue":
                            monetaryValue = cardTypeReader.ReadElementContentAsInt();
                            break;

                        case "endofgameweight":
                            endOfGameWeight = cardTypeReader.ReadElementContentAsInt();
                            break;

                        case "defaultquantity":
                            defaultQuantity = cardTypeReader.ReadElementContentAsInt();
                            break;

                        case "cardtext":
                            cardText = cardTypeReader.ReadElementContentAsString();
                            break;

                        case "properties":
                            using (XmlReader cardPropertiesReader = cardTypeReader.ReadSubtree()) {
                                properties = ParseCardPropertiesData(cardPropertiesReader);
                            }
                            break;

                        case "actions":
                            using (XmlReader actionsReader = cardTypeReader.ReadSubtree()) {
                                actions = ParseActionsData(actionsReader, assembliesByKey);
                            }
                            break;

                        default:
                            break;

                    }
                }
            }

            return RegisterCardType (   
                key, 
                cardClass, 
                cost, 
                victoryValue, 
                monetaryValue,
                endOfGameWeight,
                defaultQuantity,
                cardText,
                properties, 
                actions
            );
        }

        private IList<CardProperty> ParseCardPropertiesData(XmlReader cardPropertiesReader) {
            // Todo - (MT): Add better parsing and error handling.
            IList<CardProperty> cardProperties = new List<CardProperty>();
            while (cardPropertiesReader.ReadToFollowing("property")) {
                string propertyString = cardPropertiesReader.ReadElementContentAsString();
                CardProperty property = CardProperty.GetCardPropertyByName(propertyString);

                if (property == null) {
                    property = CardProperty.RegisterCardType(propertyString);
                }

                cardProperties.Add(property);
            }

            return cardProperties;
        }

        private IList<IAction> ParseActionsData(
          XmlReader actionsReader, 
          IDictionary<string, Assembly> assembliesByKey
        ) {
            // Todo - (MT): Add better parsing and error handling.
            IList<IAction> actions = new List<IAction>();
            while (actionsReader.ReadToFollowing("action")) {
                using (XmlReader actionReader = actionsReader.ReadSubtree()) {
                    actions.Add(ParseActionData(actionReader, assembliesByKey));
                }
            }

            return actions;
        }

        private IAction ParseActionData(
          XmlReader actionReader, 
          IDictionary<string, Assembly> assembliesByKey
        ) {
            // Todo - (MT): Add better parsing and error handling.
            string assemblyKey = null;
            string className = null;
            string displayName = null;
            string actionDescription = null;

            while (actionReader.Read()) {
                if (actionReader.NodeType == XmlNodeType.Element) {
                    switch (actionReader.Name.ToLower()) {

                        case "assemblykey":
                            assemblyKey = actionReader.ReadElementContentAsString();
                            break;

                        case "classname":
                            className = actionReader.ReadElementContentAsString();
                            break;

                        case "displayname":
                            displayName = actionReader.ReadElementContentAsString();
                            break;

                        case "actiondescription":
                            actionDescription = actionReader.ReadElementContentAsString();
                            break;

                        default:
                            break;

                    }
                }
            }

            // Refactor - (MT): Try to make the constructors for all actions private.
            Assembly assembly = assembliesByKey[assemblyKey];
            Type classType = assembly.GetType(className);
            IAction action = Activator.CreateInstance(classType) as IAction;
            if (displayName != null) {
                action.DisplayName = displayName;
            }
            if (actionDescription != null) {
                action.ActionDescription = actionDescription;
            }
            return action;
        }
    }
}
