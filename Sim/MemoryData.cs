﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Sim
{
    public class MemoryData
    {
        public static int StaticMemory = 40400;
        private int memory;
        private string label;
        private int value;
        public string Label { get => label; set => label = value; }
        public int Memory { get => memory; set => this.memory = value; }
        public int Value { get => value; set => this.value = value; }

        public MemoryData()
        {
            memory = StaticMemory;
            StaticMemory += 4;
        }
    }
}
