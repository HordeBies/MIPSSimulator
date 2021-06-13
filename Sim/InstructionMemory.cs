using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Sim
{
    public class InstructionMemory
    {
        private byte memory;
        private short value;
        private string binary;
        private string hex;

        public InstructionMemory(byte memory)
        {
            Memory = memory;
            Value = 0;
        }
        public byte Memory { get => memory; set => memory = value; }
        public short Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                this.hex = value.ToString("X");
                this.binary = Convert.ToString(value, 2);
            }
        }
        public string Binary { get => binary; }
        public string Hex { get => hex; }
    }
}
