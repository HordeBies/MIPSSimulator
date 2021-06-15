using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS
{
    public enum SimStateMachine
    {
        Empty,
        Compiled,
        Running,
        Paused,
        Finished,
    }
}
