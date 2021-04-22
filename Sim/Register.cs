using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Sim
{
    public class Register
    {
        private string label;
        private int value;

        public Register(string label,int value)
        {
            this.Label = label;
            this.Value = value;
        }

        public string Label { get => label; set => label = value; }
        public int Value { get => value; set => this.value = value; }


    }
}
