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
        string[] InstructionSet;
        public List<InstructionMemory> iMemory;
        public List<DataMemory> dMemory;
        public List<string> InputText;
        public List<LabelData> LabelTable;
        string CurrentInstruction;
        int CurrentLine; // PC
        bool PreProcessFlag = false; // aka. compile flag
        public void InputChanged() => PreProcessFlag = true;
        public MIPSSimulator(Form1 sender)
        {
            this.gui = sender;


            Registers = new List<Register>(Register.Count);

            string[] tempRegisters = new string[] { "$zero", "$r0", "$r1", "$r2", "$r3", "$r4", "$sp", "$ra"};
            for (byte i = 0; i < Register.Count; i++)
            {
                Registers.Add(new Register(tempRegisters[i], i));
            }

            string[] tempInstructionSet = new string[] { "add", "sub", "and", "or", "slt", "mult","slti", "sll","srl","muli","lui"
                                                        , "lw", "sw"
                                                        , "beq", "bne"
                                                        , "jr","j","jal"};
            InstructionSet = new string[tempInstructionSet.Length];
            for (int i = 0; i < tempInstructionSet.Length; i++)
            {
                InstructionSet[i] = tempInstructionSet[i];
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
        }
        
    }
}
