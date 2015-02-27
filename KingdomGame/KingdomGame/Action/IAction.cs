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

        bool IsTargetValid(
          IList<ITargetable> targets, 
          Card targetingCard, 
          Game game
        );

        void Apply(
          IList<ITargetable> targets,
          Game game
        );

        IList<ITargetable> GetAllValidTargets(
          Card targetingCard, 
          Game game
        );

        IList<ITargetable> GetAllPossibleTargets(Game game);

        IAction Create(Player targetSelector);

        // Refactor - (MT): Get the player from the game backreference.
        int? TargetSelectorId { get; }

        // Refactor - (MT): Add a method for getting a target object from the game via ID.
        // IList<ITargetable> GetTargetObjectById(int id);

        int MinTargets { get; }

        int MaxTargets { get; }

        bool AllValidTargetsRequired { get; }

        string DisplayName { get; set; }

        string ActionDescription { get; set; }
    }
}
