using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KingdomGame {

    public class CardType : ITargetable {

        #region Enums

        public enum CardClass {
            ACTION = 0,
            VICTORY = 1,
            TREASURE = 2,
        }

        #endregion

        #region Static Members

        private static int NextId = 1;

        #endregion

        #region Private Members

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

        #endregion

        #region Constructors

        public CardType(string name, 
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

        #endregion

        #region Properties

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

        public object Serializable {
            get { return new GameHistory.CardTypeInfo(this); }
        }

        #endregion

        #region Public Methods

        public bool IsProperty(CardProperty property) {
            return _propertiesByName.ContainsKey(property.Name);
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

        public override string ToString() {
            return Name;
        }

        public string ToString(string format) {
            return ToString();
        }

        #endregion

    }
}
