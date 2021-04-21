using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Sim
{
    public partial class MIPSSimulator
    {
        public List<Register> Registers;
        string[] InstructionSet;
        bool Mode;
        List<string> Input;
        uint NumberOfInstructions;
        uint CurrentLine;
        uint InputLength;
        int[] r = new int[3]; //register names,values... for the instruction.
        List<LabelData> LabelTable = new List<LabelData>();
        List<MemoryData> MemoryTable = new List<MemoryData>();
        int[] Stack = new int[100];
        public MIPSSimulator(string input)
        {
            Random rand = new Random(Environment.TickCount);

            Registers = new List<Register>(32);
            Registers.Add(new Register("$zero", 0));
            Registers.Add(new Register("$at", rand.Next(int.MaxValue)));
            for (int i = 0; i < 2; i++)
            {
                Registers.Add(new Register("$v" + i, 0));
            }
            for (int i = 0; i < 4; i++)
            {
                Registers.Add(new Register("$a" + i, 0));
            }
            for (int i = 0; i < 8; i++)
            {
                Registers.Add(new Register("$t" + i, 0));
            }
            for (int i = 0; i < 8; i++)
            {
                Registers.Add(new Register("$s" + i, 0));
            }
            for (int i = 8; i < 10; i++)
            {
                Registers.Add(new Register("$t" + i, 0));
            }
            Registers.Add(new Register("$k0", rand.Next(int.MaxValue)));
            Registers.Add(new Register("$k0", rand.Next(100,int.MaxValue)));
            Registers.Add(new Register("$k1", rand.Next(100,int.MaxValue)));
            Registers.Add(new Register("$k0", rand.Next(int.MaxValue)));

            Input = new List<string>();
        }
    }
}
