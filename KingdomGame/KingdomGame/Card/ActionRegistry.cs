using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KingdomGame {

    public class ActionRegistry {

        #region Private Classes

        private class ActionLoadInfo {

            #region Properties

            public string AssemblyKey { get; private set; }
            public string ClassName { get; private set; }
            public string DisplayName { get; private set; }
            public string ActionDescription { get; private set; }

            #endregion

            #region Constructors

            public ActionLoadInfo(string assemblyKey, string className, string displayName, string actionDescription) {
                AssemblyKey = assemblyKey;
                ClassName = className;
                DisplayName = displayName;
                ActionDescription = actionDescription;
            }

            #endregion

        }

        private class CardTypeLoadInfo {

            #region Properties

            public string Name { get; private set; }
            public CardType.CardClass CardClass { get; private set; }
            public int Cost { get; private set; }
            public int VictoryValue { get; private set; }
            public int MonetaryValue { get; private set; }
            public int EndOfGameWeight { get; private set; }
            public int DefaultQuantity { get; private set; }
            public string CardText { get; private set; }
            public IList<CardProperty> Properties { get; private set; }
            public IList<ActionLoadInfo> ActionsLoadInfo { get; private set; }

            #endregion

            #region Constructors

            public CardTypeLoadInfo(
              string name, 
              CardType.CardClass cardClass, 
              int cost, 
              int victoryValue,
              int monetaryValue,
              int endOfGameWeight,
              int defaultQuantity,
              string cardText,
              IList<CardProperty> properties,
              IList<ActionLoadInfo> actionsLoadInfo
            ) {
                Name = name;
                CardClass = cardClass;
                Cost = cost;
                VictoryValue = victoryValue;
                MonetaryValue = monetaryValue;
                EndOfGameWeight = endOfGameWeight;
                DefaultQuantity = defaultQuantity;
                CardText = cardText;
                Properties = properties;
                ActionsLoadInfo = actionsLoadInfo;
            }

            #endregion

        }

        #endregion

        #region Static Members

        private static ActionRegistry _instance = null;

        #endregion

        #region Static Properties

        public static ActionRegistry Instance {
            get {
                if (_instance == null) {
                    _instance = new ActionRegistry();
                }

                return _instance;
            }
        }

        #endregion

        #region Private Members

        private IDictionary<int, CardType> _cardTypesById;
        private IDictionary<string, CardType> _cardTypesByName;
        private IDictionary<int, int> _defaultQuantitiesByTypeId;

        private IDictionary<string, IAction> _actionsByTypeName;

        #endregion

        #region Constructors

        private ActionRegistry() {
            ResetRegistry();
        }

        #endregion

        #region Properties

        public IList<CardType> CardTypes {
            get { return new List<CardType>(_cardTypesById.Values); }
        }

        public IDictionary<int, int> DefaultCardCountsByType {
            get {
                IDictionary<int, int> defaultGameCardCountsByType = new Dictionary<int, int>();
                foreach(CardType type in CardTypes) {
                    defaultGameCardCountsByType[type.Id] = _defaultQuantitiesByTypeId[type.Id];
                }

                return defaultGameCardCountsByType;
            }
        }

        #endregion

        #region Public Methods

        #region Registry Management Methods

        public void InitializeRegistry(string configFilePath) {
            ResetRegistry();

            using (XmlReader reader = XmlReader.Create(new StringReader(File.ReadAllText(configFilePath))))
            {
                IDictionary<string, Assembly> assembliesByKey;
                reader.ReadToFollowing("assemblies");
                using (XmlReader assembliesReader = reader.ReadSubtree()) {
                    assembliesByKey = ParseAssembliesData(assembliesReader);
                }

                IList<CardTypeLoadInfo> cardTypesLoadInfo = new List<CardTypeLoadInfo>();
                reader.ReadToFollowing("types");
                using (XmlReader cardTypesReader = reader.ReadSubtree()) {
                    cardTypesLoadInfo = ParseCardTypesData(cardTypesReader);
                    foreach (CardTypeLoadInfo cardTypeLoadInfo in cardTypesLoadInfo) {
                        IList<IAction> actions = new List<IAction>();
                        foreach (ActionLoadInfo actionLoadInfo in cardTypeLoadInfo.ActionsLoadInfo) {
                            Assembly assembly = assembliesByKey[actionLoadInfo.AssemblyKey];
                            IAction action = RegisterAction(
                              assembly, 
                              actionLoadInfo.ClassName, 
                              actionLoadInfo.DisplayName, 
                              actionLoadInfo.ActionDescription
                            );
                            actions.Add(action);
                        }

                        CardType cardType = RegisterCardType(   
                            cardTypeLoadInfo.Name, 
                            cardTypeLoadInfo.CardClass, 
                            cardTypeLoadInfo.Cost, 
                            cardTypeLoadInfo.VictoryValue, 
                            cardTypeLoadInfo.MonetaryValue,
                            cardTypeLoadInfo.EndOfGameWeight,
                            cardTypeLoadInfo.DefaultQuantity,
                            cardTypeLoadInfo.CardText,
                            cardTypeLoadInfo.Properties, 
                            actions
                        );

                    }            
                }

                if(reader.ReadToFollowing("actions")) {
                    using (XmlReader actionTypesReader = reader.ReadSubtree()) {
                        IList<ActionLoadInfo> actionsLoadInfo = ParseActionsData(actionTypesReader);
                        foreach (ActionLoadInfo actionLoadInfo in actionsLoadInfo) {
                            Assembly assembly = assembliesByKey[actionLoadInfo.AssemblyKey];
                            RegisterAction(assembly, actionLoadInfo.ClassName, actionLoadInfo.DisplayName, actionLoadInfo.ActionDescription);
                        }
                    }
                }
            }
        }

        public void ResetRegistry() {
            // Todo - (MT): Place a reset call to card properties as well?
            _cardTypesById = new Dictionary<int, CardType>();
            _cardTypesByName = new Dictionary<string, CardType>(StringComparer.InvariantCultureIgnoreCase);
            _defaultQuantitiesByTypeId = new Dictionary<int, int>();

            _actionsByTypeName = new Dictionary<string, IAction>(StringComparer.InvariantCultureIgnoreCase);
        }

        #endregion

        #region Registry Lookup Methods

        public CardType GetCardTypeById(int id) {
            return _cardTypesById.ContainsKey(id) ? _cardTypesById[id] : null;
        }

        public CardType GetCardTypeByName(string name) {
            return _cardTypesByName.ContainsKey(name) ? _cardTypesByName[name] : null;
        }

        public IAction GetActionByType(Type type) {
            return _actionsByTypeName.ContainsKey(type.Name) ? _actionsByTypeName[type.Name] : null;
        }

        #endregion

        #endregion

        #region Helper Methods

        #region Registration Methods

        private CardType RegisterCardType(
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

        private IAction RegisterAction(Assembly assembly, string className, string displayName, string actionDescription) {
            // Refactor - (MT): Try to make the constructors for all actions private.
            Type classType = assembly.GetType(className);
            IAction action = Activator.CreateInstance(classType) as IAction;
            if (displayName != null) {
                action.DisplayName = displayName;
            }
            if (actionDescription != null) {
                action.ActionDescription = actionDescription;
            }

            _actionsByTypeName[classType.Name] = action;
            return action;
        }

        #endregion

        #region Parsing Methods

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

        private IList<CardTypeLoadInfo> ParseCardTypesData(XmlReader cardTypesReader) {
            // Todo - (MT): Add better parsing and error handling.
            IList<CardTypeLoadInfo> cardTypesLoadInfo = new List<CardTypeLoadInfo>();
            while (cardTypesReader.ReadToFollowing("type")) {
                using (XmlReader cardTypeReader = cardTypesReader.ReadSubtree()) {
                    cardTypesReader.MoveToAttribute("key");
                    string key = cardTypesReader.Value;
                    CardTypeLoadInfo cardTypeLoadInfo = ParseCardTypeData(cardTypeReader, key);
                    cardTypesLoadInfo.Add(cardTypeLoadInfo);
                }
            }

            return cardTypesLoadInfo;
        }

        private CardTypeLoadInfo ParseCardTypeData(
          XmlReader cardTypeReader, 
          string key
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
            IList<ActionLoadInfo> actionsLoadInfo = new List<ActionLoadInfo>();

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
                                actionsLoadInfo = ParseActionsData(actionsReader);
                            }
                            break;

                        default:
                            break;

                    }
                }
            }

            return new CardTypeLoadInfo(   
                key, 
                cardClass, 
                cost, 
                victoryValue, 
                monetaryValue,
                endOfGameWeight,
                defaultQuantity,
                cardText,
                properties, 
                actionsLoadInfo
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

        private IList<ActionLoadInfo> ParseActionsData(
          XmlReader actionsReader
        ) {
            // Todo - (MT): Add better parsing and error handling.
            IList<ActionLoadInfo> actions = new List<ActionLoadInfo>();
            while (actionsReader.ReadToFollowing("action")) {
                using (XmlReader actionReader = actionsReader.ReadSubtree()) {
                    actions.Add(ParseActionData(actionReader));
                }
            }

            return actions;
        }

        private ActionLoadInfo ParseActionData(
          XmlReader actionReader
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

            return new ActionLoadInfo(assemblyKey, className, displayName, actionDescription);
        }

        #endregion

        #endregion

    }
}
