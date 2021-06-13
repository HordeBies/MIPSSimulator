using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Sim
{
    public delegate void StatusUpdateHandler();

    public partial class MIPSSimulator
    {
        public event StatusUpdateHandler Compiled;
        public event StatusUpdateHandler Executed;
        public event StatusUpdateHandler Paused;
        public event StatusUpdateHandler Stopped;
        public event StatusUpdateHandler Finished;
    }
}
