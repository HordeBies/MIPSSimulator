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
        int[] Stack;
        List<string> InputText;
        List<string> InputData;
        int InputLength;
        string CurrentInstruction;
        bool section; //false = data , true = text section
        int CurrentLine;
        List<MemoryData> MemoryTable = new List<MemoryData>();
        bool isHalted = false;
        bool Mode;
        bool PreProcessFlag = false;
        public void InputChanged() => PreProcessFlag = true;

        int[] r = new int[3]; //register names,values... for the instruction.
        List<LabelData> LabelTable = new List<LabelData>();
        public MIPSSimulator(string[] text,string[] data, Form1 sender)
        {
            this.gui = sender;
            Random rand = new Random(Environment.TickCount);

            Registers = new List<Register>(32);
            string[] tempRegisters = new string[]{"$zero","$at","$v0","$v1","$a0","$a1","$a2","$a3","$t0","$t1","$t2","$t3","$t4","$t5","$t6","$t7","$s0","$s1","$s2","$s3","$s4","$s5","$s6","$s7","$t8","$t9","$k0","$k1","$gp","$sp","$s8","$ra"};
            for (int i = 0; i < tempRegisters.Length; i++)
            {
                Registers.Add(new Register(tempRegisters[i], 0));
            }

            string[] tempInstructionSet = new string[] { "add", "sub", "mul", "and", "or", "nor", "slt", "addi", "andi", "ori", "slti", "lw", "sw", "beq", "bne", "j", "halt" };
            InstructionSet = new string[tempInstructionSet.Length];
            for (int i = 0; i < tempInstructionSet.Length; i++)
            {
                InstructionSet[i] = tempInstructionSet[i];
            }

            Stack = new int[100];
            for (int i = 0; i < 100; i++)
            {
                Stack[i] = 0;
            }

            Registers[29].Value = 40296; //sp
            Registers[28].Value = 10000000; //gp


            InputText = new List<string>(text);
            InputData = new List<string>(data);
            InputLength = InputData.Count + InputText.Count;

            section = false;

        }
        public void PreLoad(string[] text, string[] data)
        {
            InputText = new List<string>(text);
            InputData = new List<string>(data);
            InputLength = InputData.Count + InputText.Count;
        }
        public int Execute()
        {
            if (!PreProcessFlag)
            {
                Preprocess();
                PreProcessFlag = true;
            }
            while(CurrentLine < InputText.Count)
            {
                ReadInstruction(CurrentLine, true);
                RemoveSpaces(CurrentInstruction);
                if(CurrentInstruction == "")
                {
                    CurrentLine++;
                    continue;
                }
                int instruction = ParseInstruction();
                ExecuteInstruction(instruction);
                if(instruction <13 || instruction > 15)
                {
                    CurrentLine++;
                }
                break;   
            }
            if(CurrentLine == InputLength)
                gui.SendLog("Execution Completed");

            return CurrentLine;
        }

        private int ParseInstruction()
        {
            return 0;
        }
        public int ExecuteInstruction(int instruction)
        {
            return 0;
        }

        public void Preprocess()
        {
            int current_section = -1;
            if(InputData.Count != 0)
                current_section = 0;

            int LabelIndex;
            if(current_section == 0)
            {
                for (int i = 0; i < InputData.Count; i++)
                {
                    ReadInstruction(i,false);
                    CurrentInstruction = RemoveSpaces(CurrentInstruction);
                    if(CurrentInstruction == "")
                    {
                        continue;
                    }
                    LabelIndex = CurrentInstruction.IndexOf(':');
                    if(LabelIndex == -1)
                    {
                        break;
                    }
                    if(LabelIndex == 0)
                    {
                        gui.ReportError("Error: Label name expected");
                    }
                    int j = LabelIndex - 1;
                    while(j>0 && (CurrentInstruction.ElementAt(j) == ' ' || CurrentInstruction.ElementAt(j) == '\t'))
                    {
                        j--;
                    }
                    string tempString = "";
                    bool doneFlag = false;
                    for (; j >= 0; j--)
                    {
                        char tempChar = CurrentInstruction.ElementAt(j);
                        if (tempChar != ' ' && tempChar != '\t' && !doneFlag)
                        {
                            tempString = tempChar + tempString;
                        }
                        else if (tempChar != ' ' && tempChar != '\t' && doneFlag)
                        {
                            gui.ReportError("Error: Unexpected text before label name");
                        }
                        else
                            doneFlag = true;
                    }

                    assertLabelAllowed(tempString);
                    MemoryData tempMemory = new MemoryData();
                    tempMemory.Label = tempString;
                    int wordIndex = CurrentInstruction.IndexOf(".word"); // TODO: Add directives for .asciiz
                    if(wordIndex == -1)
                    {
                        gui.ReportError("Error: .word not found");
                    }
                    OnlySpaces(LabelIndex + 1, wordIndex, CurrentInstruction);
                    bool foundValue = false;
                    bool doneFinding = false;
                    tempString = "";
                    for (j = wordIndex + 5; j < CurrentInstruction.Length; j++)
                    {
                        char temp = CurrentInstruction.ElementAt(j);
                        if(foundValue && (temp == ' ' || temp == '\t') && !doneFinding)
                        {
                            doneFinding = true;
                        }
                        else if(foundValue && temp != ' ' && temp != '\t' && doneFinding)
                        {
                            gui.ReportError("Error: Unexpected text after value");
                        }
                        else if(!foundValue && temp != ' ' && temp != '\t')
                        {
                            foundValue = true;
                            tempString = tempString + temp;
                        }
                        else if(foundValue && temp != ' ' && temp != '\t')
                        {
                            tempString = tempString + temp;
                        }
                    }
                    int tempValue;
                    if(!int.TryParse(tempString,out tempValue))
                    {
                        gui.ReportError("Error: Number conversion error");
                    }
                    tempMemory.Value = tempValue;
                    MemoryTable.Add(tempMemory);
                }
            }
            MemoryTable.Sort(delegate (MemoryData m1, MemoryData m2) { return m1.Label.CompareTo(m2.Label); });
            for(int i = 0; i < MemoryTable.Count - 1; i++)
            {
                if(MemoryTable.ElementAt(i) == MemoryTable.ElementAt(i + 1))
                {
                    gui.ReportError("Error: One or more labels are repeated");
                }
            }

            if (InputText.Count != 0)
                current_section = 1;

            if(current_section != 1)
            {
                gui.ReportError("Error: Text section does not exist or found unknown string");
            }
            int MainIndex = 0;
            bool FoundMain = false;
            LabelIndex = 0;
            for (int i = 0; i < InputText.Count; i++)
            {
                ReadInstruction(i, true);
                if(CurrentInstruction == "")
                {
                    continue;
                }
                LabelIndex = CurrentInstruction.IndexOf(":");
                if(LabelIndex == 0)
                {
                    gui.ReportError("Error: Label name expected");
                }
                if(LabelIndex == -1)
                {
                    continue;
                }
                int j = LabelIndex - 1;
                while(j>0 && CurrentInstruction.ElementAt(j) == ' ' && CurrentInstruction.ElementAt(j) == '\t')
                {
                    j--;
                }
                string tempString = "";
                bool isLabel = false;
                bool doneFlag = false;
                for (; j >= 0; j--)
                {
                    char temp = CurrentInstruction.ElementAt(j);
                    if(temp != ' ' && temp != '\t' && !doneFlag)
                    {
                        isLabel = true;
                        tempString = temp + tempString;
                    }
                    else if(temp != ' ' && temp != '\t' && doneFlag)
                    {
                        gui.ReportError("Error: Unexpected text before label name");
                    }
                    else if(!isLabel)
                    {
                        gui.ReportError("Error: Label name expected");
                    }
                    else
                    {
                        doneFlag = true;
                    }
                }
                assertLabelAllowed(tempString);
                OnlySpaces(LabelIndex + 1, CurrentInstruction.Length, CurrentInstruction);
                if(tempString == "main")
                {
                    FoundMain = true;
                    MainIndex = CurrentLine;
                }
                else
                {
                    LabelData tempLabel = new LabelData(); ;
                    tempLabel.Address = CurrentLine;
                    tempLabel.Label = tempString;
                    LabelTable.Add(tempLabel);
                }
            }
            LabelTable.Sort(delegate (LabelData l1, LabelData l2) { return l1.Label.CompareTo(l2.Label); });
            for (int i = 0;LabelTable.Count > 0 && i < LabelTable.Count-1; i++)
            {
                if(LabelTable[i].Label == LabelTable[i].Label)
                {
                    gui.ReportError("Error: One or more labels are repeated");
                }
            }
            if(!FoundMain)
            {
                gui.ReportError("Error: Could not find main");
            }
            CurrentLine = MainIndex;
            gui.SendLog("Initialized and ready to execute. Current state is:");
            gui.updateState();
        }

        private void assertLabelAllowed(string str)
        {
            if(str.Length == 0 || char.IsDigit(str.ElementAt(0)))
            {
                gui.ReportError("Error: Invalid Label");
            }
            for (int i = 0; i < str.Length; i++)
            {
                char temp = str.ElementAt(i);
                if (!(char.IsDigit(temp) || char.IsLetter(temp)))
                {
                    gui.ReportError("Error: Invalid Label");
                }
            }
        }
        private void OnlySpaces(int lower,int upper,string str)
        {
            for(int i = lower;i< upper; i++)
            {
                if(str.ElementAt(i)!= ' ' && str[i] != '\t')
                {
                    gui.ReportError("Error: Unexpected character");
                }
            }
        }

        private string RemoveSpaces(string str)
        {
            int t = 0;
            while(t < str.Length && (str.ElementAt(t) == ' ' || str.ElementAt(t) == '\t'))
            {
                t++;
            }
            return str.Substring(t);
        }

        public void ReadInstruction(int line,bool section)
        {
            CurrentInstruction = section?  InputText[line] : InputData[line];
            if (CurrentInstruction.Contains("#"))
            {
                CurrentInstruction = CurrentInstruction.Substring(0, CurrentInstruction.IndexOf("#"));
            }
            CurrentLine = line;
        }
    }
}
