using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace KingdomGame {

    // Refactor - (MT): Make actions themselves ITargetable (with IDs).
    // Refactor - (MT): Add a target selecting player action to determine whose strategy should get used.
    public interface IAction {

        bool IsTargetValid<TTarget>(
          IList<TTarget> targets, 
          Card targetingCard, 
          Game game
        ) where TTarget : class, ITargetable;

        void Apply(
          IList<ITargetable> targets,
          Game game
        );

        IList<ITargetable> GetAllValidTargets(
          Card targetingCard, 
          Game game
        );

        IList<TTarget> GetAllPossibleTargets<TTarget>(Game game) where TTarget : class, ITargetable;

        int MinTargets { get; }

        int MaxTargets { get; }

        Type TargetType { get; }

        int? ExecutingPlayerId { get; }

        bool AllValidTargetsRequired { get; }

        string DisplayName { get; set; }

        string ActionDescription { get; set; }
    }
}
