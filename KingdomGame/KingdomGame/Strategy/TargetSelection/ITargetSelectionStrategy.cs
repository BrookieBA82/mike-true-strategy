using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace KingdomGame {

    public interface ITargetSelectionStrategy : ICloneable {

        IList<TTarget> SelectTargets<TTarget>(
          Game game, 
          Card card, 
          IAction action
        ) where TTarget : class, ITargetable;

    }
}