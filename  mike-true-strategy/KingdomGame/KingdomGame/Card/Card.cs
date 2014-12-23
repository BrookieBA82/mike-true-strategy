using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame
{
    public class Card : ICloneable, ITargetable {

        #region Static Members

        private static int NextId = 1;

        #endregion

        #region Private Members

        private int _id;
        private CardType _type;
        private int? _ownerId;
        private bool _isTrashed;

        #endregion

        #region Constructors

        #region Public Constructor

        public Card(CardType type, Player owner) 
          : this(Card.NextId++, type, (owner == null) ? new Nullable<int>() : owner.Id, false) {

        }

        #endregion

        #region Private Constructors

        private Card(int id, CardType type, int? ownerId, bool isTrashed) {
            _id = id;
            _type = type;
            _ownerId = ownerId;
            _isTrashed = isTrashed;
        }

        private Card(Card toClone) : this(toClone._id, toClone._type, toClone._ownerId, toClone._isTrashed) {

        }

        #endregion

        #endregion

        #region Properties

        public int Id {
            get { return _id; }
        }

        public int Cost {
            get { return Type.Cost; }
        }

        public int Score { 
            get { return Type.VictoryValue; }
        }

        public int Value { 
            get { return Type.MonetaryValue; }
        }

        public CardType Type {
            get { return _type; }
        }

        public int? OwnerId {
            get { return _ownerId; }
            set { _ownerId = value; }
        }

        public bool IsTrashed {
            get { return _isTrashed; }
            set { _isTrashed = value; }
        }

        public object Clone() {
            return new Card(this);
        }

        #endregion

        #region Public Methods

        public override string ToString() {
            return Type.Name;
        }

        public override bool Equals(object obj) {
            Card card = obj as Card;
            if (card == null) {
                return false;
            }

            return (this.Id == card.Id) && (this.Type.Equals(card.Type));
        }

        public override int GetHashCode() {
            return Id ^ Type.Id;
        }

        #endregion
    }
}
