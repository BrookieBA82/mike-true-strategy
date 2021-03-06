﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;
using KingdomGame;

namespace KingdomGame.BasicCardTypes {

    public class VillageAction : BasePlayerTargetAction {

        private VillageAction() : base(BasePlayerTargetAction.PlayerTargetType.SELF, 1, 1) {

        }

        protected override void ApplyInternal(IList<Player> players, Game game) {
            if (players.Count > 0) {
                players[0].Draw();
                players[0].RemainingActions += 2;
            }
        }
    }
}
