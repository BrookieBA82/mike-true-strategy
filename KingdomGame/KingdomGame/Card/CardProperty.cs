using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame
{
    public class CardProperty {

        private static int NextId = 1;

        private static IDictionary<int, CardProperty> CardPropertiesById;
        private static IDictionary<string, CardProperty> CardPropertiesByName;

        private int _id;
        private string _name;

        static CardProperty() {
            CardPropertiesById = new Dictionary<int, CardProperty>();
            CardPropertiesByName = new Dictionary<string, CardProperty>(StringComparer.InvariantCultureIgnoreCase);
        }

        private CardProperty(string name) {
            _id = CardProperty.NextId++;
            _name = name;
        }

        public static CardProperty RegisterCardType(string name) {
            if (CardPropertiesByName.ContainsKey(name)) {
                throw new ArgumentException("A card property with name " + name + " already exists.");
            }

            CardProperty property = new CardProperty(name);

            CardPropertiesById.Add(property.Id, property);
            CardPropertiesByName.Add(property.Name, property);

            return property;
        }

        public static CardProperty GetCardPropertyById(int id) {
            return CardPropertiesById.ContainsKey(id) ? CardPropertiesById[id] : null;
        }

        public static CardProperty GetCardPropertyByName(string name) {
            return CardPropertiesByName.ContainsKey(name) ? CardPropertiesByName[name] : null;
        }

        public int Id {
            get { return this._id; }
        }

        public string Name {
            get { return this._name; }
        }

        public override bool Equals(object obj) {
            CardProperty property = obj as CardProperty;
            if (property == null) {
                return false;
            }

            return (this.Id == property.Id) && (this.Name.ToLower().Equals(property.Name.ToLower()));
        }

        public override int GetHashCode() {
            return this.Id.GetHashCode() ^ this.Name.ToLower().GetHashCode();
        }
    }
}
