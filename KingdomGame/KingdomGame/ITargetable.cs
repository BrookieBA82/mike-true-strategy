using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame {

    public interface ITargetable {

        int Id { get; }

        GameHistory.TargetInfo TargetInfo { get; }

        string ToString(string format);

    }
}
