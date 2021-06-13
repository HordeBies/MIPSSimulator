using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Sim
{
    public class LabelData
    {
        private string label;
        private byte address;
        public string Label { get => label; set => label = value; }
        public byte Address { get => address; set => address = value; }
        public LabelData()
        {

        }
    }
}
