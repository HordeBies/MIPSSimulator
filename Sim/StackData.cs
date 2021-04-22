using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Sim
{
    public class StackData
    {
        private static int StaticMemory = 40000;
        private int value;
        private int memory;

        public StackData(int value)
        {
            Value = value;
            Memory = StaticMemory;
            StaticMemory += 4;
        }

        public int Memory { get => memory; set => memory = value; }
        public int Value { get => value; set => this.value = value; }
    }
}
