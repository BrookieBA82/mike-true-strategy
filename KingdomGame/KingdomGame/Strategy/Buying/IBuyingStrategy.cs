﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingdomGame {

    public interface IBuyingStrategy : ICloneable {

        CardType SelectBuy(Game game);

    }
}