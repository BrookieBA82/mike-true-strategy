using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame {
    
    public interface ITargetable {

        int Id { get; }

        // Todo - (MT): Force a ToString override which accepts a format string.
    }
}
