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

        IAction Create(Player targetSelector);

        bool IsTargetSetValid(IList<ITargetable> targetSet, Card targetingCard, Game game);

        IList<ITargetable> GetAllValidIndividualTargets(Card targetingCard, Game game);

        void Apply(IList<ITargetable> targetSet, Game game);

        // Refactor - (MT): Get the player from the game backreference.
        int? TargetSelectorId { get; }

        // Refactor - (MT): Add a method for getting a target object from the game via ID.
        // ITargetable GetTargetObjectById(int id);

        int MinTargets { get; }

        int MaxTargets { get; }

        bool AllValidTargetsRequired { get; }

        string DisplayName { get; set; }

        string ActionDescription { get; set; }
    }
}
