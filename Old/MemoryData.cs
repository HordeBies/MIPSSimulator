using System;
using System.Collections.Generic;
using System.Text;

namespace CORG_MT
{
    class MemoryData
    {
        private string label;
        private int value;
        public string Label { get => label; }
        public int Value { get => value; }
        public MemoryData(string label, int value)
        {
            this.label = label;
            this.value = value;
        }
    }
}
