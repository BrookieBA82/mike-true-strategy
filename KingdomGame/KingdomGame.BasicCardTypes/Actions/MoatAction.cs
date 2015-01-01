﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;
using KingdomGame;

namespace KingdomGame.BasicCardTypes {

    public class MoatAction : BasePlayerTargetAction {

        public MoatAction() : base(BasePlayerTargetAction.PlayerTargetType.SELF, 1, 1) {

        }

        protected override void ApplyInternal(IList<Player> players, Game game, IList<Pair<IAction, IList<int>>> previousActions) {
            if (players.Count > 0) {
                players[0].Draw(2);
            }
        }
    }
}