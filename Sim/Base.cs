using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Sim
{
    public partial class MIPSSimulator
    {
        public Form1 gui;
        public List<Register> Registers;
        Dictionary<string,string> InstructionSet;
        public List<InstructionMemory> iMemory;
        public List<DataMemory> dMemory;
        public List<string> InputText;
        public List<LabelData> LabelTable;
        public List<String> iMemoryText;
        public string CurrentInstruction;
        public int LastLine; //For Code Highlighting/Debugging
        public int CurrentLine; // PC
        public MIPSSimulator(Form1 sender)
        {
            this.gui = sender;


            Registers = new List<Register>(Register.Count);

            string[] tempRegisters = new string[] { "$zero", "$r0", "$r1", "$r2", "$r3", "$r4", "$sp", "$ra"};
            for (byte i = 0; i < Register.Count; i++)
            {
                Registers.Add(new Register(tempRegisters[i], i));
            }
            Registers[6].Value = 255;

            string[] tempInstructionKeySet = new string[]  { "add", "sub", "and", "or", "slt", "mul", "sll","srl"
                                                            ,"slti","beq","bne", "muli", "lui", "lw","sw"
                                                            , "jr","j","jal"};
            string[] tempInstructionValueSet = new string[]{ "0000", "0000", "0000", "0000", "0000", "0000", "1011","1100"
                                                            ,"0010","0011","0100", "0101", "0110", "0111","1000"
                                                            , "0001","1001","1010"};
            InstructionSet = new Dictionary<string, string>(tempInstructionKeySet.Length);
            
            for (int i = 0; i < tempInstructionKeySet.Length; i++)
            {
                InstructionSet[tempInstructionKeySet[i]] = tempInstructionValueSet[i];
            }

            iMemory = new List<InstructionMemory>(256);
            for (int i = 0; i < 256; i++)
            {
                iMemory.Add(new InstructionMemory((byte)i));
            }

            dMemory = new List<DataMemory>(256);
            for (int i = 0; i < 256; i++)
            {
                dMemory.Add(new DataMemory((byte)i));
            }

            InputText = new List<string>();
            LabelTable = new List<LabelData>();
            iMemoryText = new List<string>();
        }
        
    }
}
