using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Sim
{
    public class Register
    {
        public static readonly int Count = 8;
        private string label;
        public byte id;
        private short value;
        private string binary;
        private string hex;

        public Register(string label, byte id)
        {
            this.Label = label;
            this.Value = 0;
            this.id = id;
        }

        public string Label { get => label; set => label = value; }
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
