using System;
using System.Collections.Generic;
using System.Text;

namespace CORG_MT.MIPS
{
    public partial class MIPSSimulator
    {
        Dictionary<string, int> Registers;
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
        MIPSSimulator(string input,bool mode)
        {
            Registers = new Dictionary<string, int>(32);
            Input = new List<string>();
        }
    }
}
