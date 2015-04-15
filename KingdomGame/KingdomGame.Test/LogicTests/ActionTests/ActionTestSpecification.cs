﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame.Test {

    public class ActionTestSpecification {

        private IDictionary<int, int> _gameCardCountsByTypeId = new Dictionary<int, int>();
        private IDictionary<int, int> _playerCardCountsByTypeId = new Dictionary<int, int>();
        private IDictionary<int, int> _handCardCountsByTypeId = new Dictionary<int, int>();

        private CardType _play = null;
        private IList<ITargetable> _targets = null;

        public ActionTestSpecification() {

        }

        private ActionTestSpecification(ActionTestSpecification toClone) {
            _gameCardCountsByTypeId = new Dictionary<int, int>(toClone._gameCardCountsByTypeId);
            _playerCardCountsByTypeId = new Dictionary<int, int>(toClone._playerCardCountsByTypeId);
            _handCardCountsByTypeId = new Dictionary<int, int>(toClone._handCardCountsByTypeId);

            _play = (toClone._play != null) ? toClone._play : null;
            _targets = (toClone._targets != null) ? new List<ITargetable>(toClone._targets) : null;
        }

        public IDictionary<int, int> GameCardCountsByTypeId { get { return _gameCardCountsByTypeId; } }

        public IDictionary<int, int> PlayerCardCountsByTypeId { get { return _playerCardCountsByTypeId; } }

        public IDictionary<int, int> HandCardCountsByTypeId { get { return _handCardCountsByTypeId; } }

        public CardType Play {
            get { return _play; } 
            set { _play = value; } 
        }

        public IList<ITargetable> Targets { 
            get { return _targets; }
            set { _targets = value; }
        }

        public static ActionTestSpecification ApplyOverrides(ActionTestSpecification baseTest, ActionTestSpecification overrides) {
            ActionTestSpecification mergedTest = new ActionTestSpecification(baseTest);

            foreach (int typeId in overrides.GameCardCountsByTypeId.Keys) {
                mergedTest.GameCardCountsByTypeId[typeId] = overrides.GameCardCountsByTypeId[typeId];
            }

            foreach (int typeId in overrides.PlayerCardCountsByTypeId.Keys) {
                mergedTest.PlayerCardCountsByTypeId[typeId] = overrides.PlayerCardCountsByTypeId[typeId];
            }

            foreach (int typeId in overrides.HandCardCountsByTypeId.Keys) {
                mergedTest.HandCardCountsByTypeId[typeId] = overrides.HandCardCountsByTypeId[typeId];
            }

            foreach (int typeId in overrides.HandCardCountsByTypeId.Keys) {
                mergedTest.HandCardCountsByTypeId[typeId] = overrides.HandCardCountsByTypeId[typeId];
            }

            if (overrides.Play != null) {
                mergedTest.Play = overrides.Play;
            }

            if (overrides.Targets != null) {
                mergedTest.Targets = new List<ITargetable>(overrides.Targets);
            }

            return mergedTest;
        }
    }
}