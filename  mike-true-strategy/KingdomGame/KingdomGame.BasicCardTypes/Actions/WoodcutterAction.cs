using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;
using KingdomGame;

namespace KingdomGame.BasicCardTypes {

    public class WoodcutterAction : BasePlayerTargetAction {

        public WoodcutterAction() : base(BasePlayerTargetAction.PlayerTargetType.SELF, 1, 1) {

        }

        protected override void ApplyInternal(IList<Player> players, Game game, IList<Pair<IAction, IList<int>>> previousActions) {
            if (players.Count > 0) {
                players[0].RemainingBuys += 1;
                players[0].RemainingMoney += 2;
            }
        }
    }
}
