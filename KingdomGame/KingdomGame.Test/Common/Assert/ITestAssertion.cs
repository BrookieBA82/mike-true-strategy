using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame.Test {

    public interface ITestAssertion {

        string Key { get; }

        string Description { get; set; }

        void Bind(Game game);

        void Assert(Game game);

    }
}