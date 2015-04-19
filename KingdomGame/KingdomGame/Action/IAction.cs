using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wintellect.PowerCollections;

namespace KingdomGame {

    // Refactor - (MT): Make actions themselves ITargetable (with IDs).
    public interface IAction {

        IAction Create(Player targetSelector);

        bool IsTargetSetValid(IList<ITargetable> targetSet, Game game);

        IList<ITargetable> GetAllValidIndividualTargets(Game game);

        void Apply(IList<ITargetable> targetSet, Game game);

        int? TargetSelectorId { get; }

        int MinTargets { get; }

        int MaxTargets { get; }

        bool AllValidTargetsRequired { get; }

        string DisplayName { get; set; }

        string ActionDescription { get; set; }
    }
}
