using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBAE
{
    public class State
    {
        public Rom Rom;
        public Registers Registers;
        public CPSR CPSR;

        public State()
        {
            Rom = new Rom();
            Registers = new Registers();
            CPSR = new CPSR();
        }
    }
}
