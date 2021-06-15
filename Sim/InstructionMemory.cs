﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Sim
{
    public class InstructionMemory
    {
        private byte memory;
        private ushort value;
        private string binary;
        private string hex;

        public InstructionMemory(byte memory)
        {
            Memory = memory;
            Value = 0;
        }
        public byte Memory { get => memory; set => memory = value; }
        public ushort Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                this.hex = value.ToString("X");
                this.binary = Extend(Convert.ToString(value, 2),16);
            }
        }
        public string Binary { get => binary; }
        public string Hex { get => hex; }
        private string Extend(string input, int extend)
        {
            if (input.Length == extend)
                return input;
            string result = "";
            for (int i = 0; i < extend - input.Length; i++)
            {
                result += '0';
            }
            result += input;
            return result;
        }
    }
}
