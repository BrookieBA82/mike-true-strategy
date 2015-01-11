using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KingdomGame {

    // Refactor - (MT): Break the static interface out into its own registry class?
    public class CardType : ITargetable {

        public enum CardClass {
            ACTION = 0,
            VICTORY = 1,
            TREASURE = 2,
        }

        private static int NextId = 1;

        private static IDictionary<int, CardType> CardTypesById;
        private static IDictionary<string, CardType> CardTypesByName;
        private static IDictionary<int, int> DefaultQuantitiesByTypeId;

        private int _id;
        private string _name;

        private CardType.CardClass _class;
        private int _cost;
        private int _victoryValue;
        private int _monetaryValue;
        private int _endOfGameWeight;
        private string _cardText;

        private IList<CardProperty> _properties;
        private IDictionary<string, CardProperty> _propertiesByName;

        private IList<IAction> _actions;

        static CardType() {
            ResetCardTypes();
        }

        private CardType(string name, 
            CardType.CardClass cardClass, 
            int cost, 
            int victoryValue, 
            int monetaryValue, 
            int endOfGameWeight,
            string cardText,
            IEnumerable<CardProperty> properties,
            IEnumerable<IAction> actions
        ) {
            _id = CardType.NextId++;
            _name = name;
            _class = cardClass;
            _cost = cost;
            _victoryValue = victoryValue;
            _monetaryValue = monetaryValue;
            _endOfGameWeight = endOfGameWeight;
            _cardText = cardText;

            _properties = new List<CardProperty>(properties);
            _propertiesByName = new Dictionary<string, CardProperty>(StringComparer.InvariantCultureIgnoreCase);

            foreach (CardProperty property in properties) {
                if (this.IsProperty(property)) {
                    throw new ArgumentException("A duplicate property with name " + property.Name + " was specified.");
                }

                _propertiesByName[property.Name] = property;
            }

            _actions = new List<IAction>(actions);
        }

        public static CardType RegisterCardType(
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
            if (CardTypesByName.ContainsKey(name)) {
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

            CardTypesById.Add(type.Id, type);
            CardTypesByName.Add(type.Name, type);
            DefaultQuantitiesByTypeId.Add(type.Id, defaultQuantity);

            return type;
        }

        public static CardType GetCardTypeById(int id) {
            return CardTypesById.ContainsKey(id) ? CardTypesById[id] : null;
        }

        public static CardType GetCardTypeByName(string name) {
            return CardTypesByName.ContainsKey(name) ? CardTypesByName[name] : null;
        }

        public static int GetDefaultQuantityByTypeId(int id) {
            return DefaultQuantitiesByTypeId.ContainsKey(id) ? DefaultQuantitiesByTypeId[id] : 0;
        }

        public static IList<CardType> CardTypes {
            get { return new List<CardType>(CardTypesById.Values); }
        }

        public static IDictionary<int, int> DefaultCardCountsByType {
            get {
                IDictionary<int, int> defaultGameCardCountsByType = new Dictionary<int, int>();
                foreach(CardType type in CardType.CardTypes) {
                    defaultGameCardCountsByType[type.Id] = CardType.GetDefaultQuantityByTypeId(type.Id);
                }

                return defaultGameCardCountsByType;
            }
        }

        public static void InitializeCardTypes(string configFilePath) {
            ResetCardTypes();

            using (XmlReader reader = XmlReader.Create(new StringReader(File.ReadAllText(configFilePath))))
            {
                IDictionary<string, Assembly> assembliesByKey;
                reader.ReadToFollowing("assemblies");
                using (XmlReader assembliesReader = reader.ReadSubtree()) {
                    assembliesByKey = CardType.ParseAssembliesData(assembliesReader);
                }

                IDictionary<string, CardType> cardTypesByKey;
                reader.ReadToFollowing("types");
                using (XmlReader cardTypesReader = reader.ReadSubtree()) {
                    cardTypesByKey = CardType.ParseCardTypesData(cardTypesReader, assembliesByKey);
                }

                // Todo - (MT): Return the Card Types table, or something else?
            }
        }

        public static void ResetCardTypes() {
            // Todo - (MT): Place a reset call to card properties as well?
            CardTypesById = new Dictionary<int, CardType>();
            CardTypesByName = new Dictionary<string, CardType>(StringComparer.InvariantCultureIgnoreCase);
            DefaultQuantitiesByTypeId = new Dictionary<int, int>();
        }

        public int Id {
            get { return _id; }
        }

        public string Name {
            get { return _name; }
        }

        public CardType.CardClass Class {
            get { return _class; }
        }

        public int Cost {
            get { return _cost; }
        }

        public int VictoryValue {
            get { return _victoryValue; }
        }

        public int MonetaryValue {
            get { return _monetaryValue; }
        }

        public int EndOfGameWeight {
            get { return _endOfGameWeight; }
        }

        public string CardText {
            get { return _cardText; }
        }

        public IList<CardProperty> Properties {
            get { return new List<CardProperty>(_properties); }
        }

        public IList<IAction> Actions {
            get { return new List<IAction>(_actions); }
        }

        public bool IsProperty(CardProperty property) {
            return _propertiesByName.ContainsKey(property.Name);
        }

        public override string ToString() {
            return Name;
        }

        public override bool Equals(object obj) {
            CardType type = obj as CardType;
            if (type == null) {
                return false;
            }

            return (this.Id == type.Id);
        }

        public override int GetHashCode() {
            return this.Id.GetHashCode();
        }


        private static IDictionary<string, Assembly> ParseAssembliesData(XmlReader assembliesReader) {
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

        private static IDictionary<string, CardType> ParseCardTypesData(
          XmlReader cardTypesReader, 
          IDictionary<string, Assembly> assembliesByKey
        ) {
            // Todo - (MT): Add better parsing and error handling.
            IDictionary<string, CardType> cardTypesByKey = new Dictionary<string, CardType>();
            while (cardTypesReader.ReadToFollowing("type")) {
                using (XmlReader cardTypeReader = cardTypesReader.ReadSubtree()) {
                    cardTypesReader.MoveToAttribute("key");
                    string key = cardTypesReader.Value;
                    CardType cardType = CardType.ParseCardTypeData(cardTypeReader, key, assembliesByKey);
                    cardTypesByKey[key] = cardType;
                }
            }

            return cardTypesByKey;
        }

        private static CardType ParseCardTypeData(
          XmlReader cardTypeReader, 
          string key,           
          IDictionary<string, Assembly> assembliesByKey
        ) {
            // Default values to use if not specified in configuration file:
            CardClass cardClass = CardClass.ACTION;
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
                            Enum.TryParse<CardClass>(cardTypeReader.ReadElementContentAsString(), out cardClass);
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
                                properties = CardType.ParseCardPropertiesData(cardPropertiesReader);
                            }
                            break;

                        case "actions":
                            using (XmlReader actionsReader = cardTypeReader.ReadSubtree()) {
                                actions = CardType.ParseActionsData(actionsReader, assembliesByKey);
                            }
                            break;

                        default:
                            break;

                    }
                }
            }

            return CardType.RegisterCardType (   
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

        private static IList<CardProperty> ParseCardPropertiesData(XmlReader cardPropertiesReader) {
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

        private static IList<IAction> ParseActionsData(
          XmlReader actionsReader, 
          IDictionary<string, Assembly> assembliesByKey
        ) {
            // Todo - (MT): Add better parsing and error handling.
            IList<IAction> actions = new List<IAction>();
            while (actionsReader.ReadToFollowing("action")) {
                using (XmlReader actionReader = actionsReader.ReadSubtree()) {
                    actions.Add(CardType.ParseActionData(actionReader, assembliesByKey));
                }
            }

            return actions;
        }

        private static IAction ParseActionData(
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
