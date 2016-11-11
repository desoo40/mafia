using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mafia
{
    class Player
    {
        public string Nick { get; set; }
        public bool IsWinner { get; set; }
        public Type Trump { get; set; }
        public decimal Stata { get; set; }
    }

    enum Type
    {
        Mafia,
        Polizai,
        Citizen
    }
}
