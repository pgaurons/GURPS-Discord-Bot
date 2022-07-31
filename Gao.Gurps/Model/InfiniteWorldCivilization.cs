using System;
using System.Collections.Generic;
using System.Text;

namespace Gao.Gurps.Model
{
    public class InfiniteWorldCivilization
    {
        public Civilization Civilization { get; set; }
        public CivilizationUnity Unity { get; set; }
        public int NumberOfPowers { get; set; } = 1;
        public Government[] PowerGovernments { get; set; } = new Government[] { Government.Anarchy };
        public bool Fragmenting { get; internal set; }
    }
}
