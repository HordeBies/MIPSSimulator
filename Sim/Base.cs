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
        public List<StackData> Stack;
        public List<string> InputText;
        public List<string> InputData;
        string CurrentInstruction;
        int CurrentLine;
        public int LastLine;
        public List<MemoryData> MemoryTable = new List<MemoryData>();
        List<LabelData> LabelTable = new List<LabelData>();
        public bool isHalted = false;
        public bool DoneFlag = false;
        bool PreProcessFlag = false;
        public int[] args = new int[3]; //register names,values... for the instruction.
        bool FPA = false;
        public int MFLO = 0;
        public int MFHI = 0;
        public void InputChanged() => PreProcessFlag = true;

        public MIPSSimulator(string[] text,string[] data, Form1 sender)
        {
            this.gui = sender;
            Random rand = new Random(Environment.TickCount);

            Registers = new List<Register>(32);
            string[] tempRegisters = new string[]{"$zero","$at","$v0","$v1","$a0","$a1","$a2","$a3","$t0","$t1","$t2","$t3","$t4","$t5","$t6","$t7","$s0","$s1","$s2","$s3","$s4","$s5","$s6","$s7","$t8","$t9","$k0","$k1","$gp","$sp","$fp","$ra"};
            for (int i = 0; i < tempRegisters.Length; i++)
            {
                Registers.Add(new Register(tempRegisters[i], "0"));
            }
            for(int i = 0; i< 32; i++)
            {
                Registers.Add(new Register("$"+ (i < 10 ? "0" : "") + i, "0"));
            }

            string[] tempInstructionSet = new string[] { "add", "sub", "and", "or", "slt", "add.d","sub.d","mul.d","div.d","c.eq.d","c.lt.d","mult","div"
                                                        ,"addi", "andi", "ori", "slti"
                                                        ,"lw", "sw","ldc1","sdc1"
                                                        , "beq", "blt"
                                                        , "j","bc1t","bc1f"
                                                        ,"mflo" ,"mfhi" };
            InstructionSet = new string[tempInstructionSet.Length];
            for (int i = 0; i < tempInstructionSet.Length; i++)
            {
                InstructionSet[i] = tempInstructionSet[i];
            }

            Stack = new List<StackData>(100);
            for (int i = 0; i < 100; i++)
            {
                Stack.Add(new StackData("0"));
            }
            Registers[28].Value = "10000000"; //gp
            Registers[29].Value = "40396"; //sp
            Registers[30].Value = "40396"; //fp


            InputText = new List<string>(text);
            InputData = new List<string>(data);

        }
        public void PreLoad(string[] text, string[] data)
        {
            isHalted = false;
            InputText = new List<string>(text);
            InputData = new List<string>(data);
            MemoryTable = new List<MemoryData>();
            MemoryData.StaticMemory = 40400;
            Registers[29].Value = "40396"; //sp
            LabelTable = new List<LabelData>();
            PreProcessFlag = false;
            DoneFlag = false;
            CurrentLine = 0;
            CurrentInstruction = "";
        }
        public int Execute()
        {
            if (isHalted)
            {
                gui.SendLog("on Line: "+CurrentLine.ToString());
                return CurrentLine;
            }
            if (!PreProcessFlag)
            {
                DoneFlag = false;
                Preprocess();
                if (isHalted)
                {
                    gui.SendLog("on Line: " + CurrentLine.ToString());
                    return CurrentLine;
                }
                PreProcessFlag = true;
            }
            while(CurrentLine < InputText.Count)
            {
                ReadInstruction(CurrentLine, true);
                RemoveSpaces(CurrentInstruction);
                if(CurrentInstruction == "")
                {
                    CurrentLine++;
                    LastLine = CurrentLine - 1;
                    continue;
                }
                int instruction = ParseInstruction();
                if (isHalted)
                {
                    gui.SendLog("on Line: " + CurrentLine.ToString());
                    return CurrentLine;
                }
                ExecuteInstruction(instruction);
                if (isHalted)
                {
                    gui.SendLog("on Line: " + CurrentLine.ToString());
                    return CurrentLine;
                }
                if (instruction <26 || instruction > 20)
                {
                    CurrentLine++;
                    LastLine = CurrentLine - 1;
                }
                break;   
            }
            if(CurrentLine >= InputText.Count)
            {
                gui.SendLog("Execution Completed");
                DoneFlag = true;
            }

            return CurrentLine;
        }

        private int ParseInstruction()
        {
            int i = 0, j = 0; //temp vars

            RemoveSpaces(CurrentInstruction);

            if (CurrentInstruction.Contains(":"))
                return -2;

            if(CurrentInstruction.Length < 4)
            {
                gui.ReportError("Error: Unknown operation");
                return -2;
            }

            for(j = 0; j< CurrentInstruction.Length; j++)
            {
                var temp = CurrentInstruction.ElementAt(j);
                if(temp == ' ' || temp == '\t')
                    break;
            }

            string op = CurrentInstruction.Substring(0, j);

            if (CurrentInstruction.Length > 0 && j < CurrentInstruction.Length - 1)
                CurrentInstruction = CurrentInstruction.Substring(j + 1);

            int OperationID = -1;
            for(i = 0; i< InstructionSet.Length; i++)
            {
                if(op == InstructionSet[i])
                {
                    OperationID = i;
                    break;
                }
            }
            if(OperationID == -1)
            {
                gui.ReportError("Error:Unknown operation");
                return -2;
            }
            if(OperationID < 13) // R Format
            {
                for(int count = 0; count < 3 - (OperationID > 8 ? 1: 0); count++)
                {
                    RemoveSpaces(CurrentInstruction);
                    if (!FindRegister(count ))
                        return -2;
                    RemoveSpaces(CurrentInstruction);
                    if (count == 2 - (OperationID > 8 ? 1 : 0))
                        break;
                    if (!assertRemoveComma())
                        return -2;
                }
                if(CurrentInstruction != "")
                {
                    gui.ReportError("Error: Extra arguments provided");
                    return -2;
                }
            }else if(OperationID < 17) // I format
            {
                for(int count = 0; count < 2; count++)
                {
                    RemoveSpaces(CurrentInstruction);
                    FindRegister(count);
                    RemoveSpaces(CurrentInstruction);
                    assertRemoveComma();
                }
                RemoveSpaces(CurrentInstruction);
                string tempString = findLabel();
                int temp;
                if(!int.TryParse(tempString,out temp))
                {
                    gui.ReportError("Error: Not a valid Immediate argument");
                    return -2;
                }
                args[2] = temp;
            }else if (OperationID < 21) // lw sw ldc1 sdc1
            {
                string tempString = "";
                int offset;
                RemoveSpaces(CurrentInstruction);
                FindRegister(0);
                RemoveSpaces(CurrentInstruction);
                assertRemoveComma();
                RemoveSpaces(CurrentInstruction);
                if((CurrentInstruction.ElementAt(0) > 47 && CurrentInstruction.ElementAt(0) < 58) || CurrentInstruction.ElementAt(0) == '-')
                {
                    j = 0;
                    while (j < CurrentInstruction.Length && CurrentInstruction.ElementAt(j) != ' ' 
                        && CurrentInstruction.ElementAt(j) != '\t' && CurrentInstruction.ElementAt(j) != '(')
                    {
                        tempString = tempString + CurrentInstruction.ElementAt(j);
                        j++;
                    }
                    if(j == CurrentInstruction.Length)
                    {
                        gui.ReportError("Error: '(' expected");
                        return -2;
                    }
                    int temp;
                    if(!int.TryParse(tempString,out temp))
                    {
                        gui.ReportError("Error: not a valid offset");
                        return -2;
                    }
                    offset = temp;
                    CurrentInstruction = CurrentInstruction.Substring(j);
                    RemoveSpaces(CurrentInstruction);
                    if(CurrentInstruction == "" || CurrentInstruction.ElementAt(0) != '(' || CurrentInstruction.Length < 2)
                    {
                        gui.ReportError("Error: '(' expected");
                        return -2;
                    }
                    CurrentInstruction = CurrentInstruction.Substring(1);
                    RemoveSpaces(CurrentInstruction);
                    FindRegister(1);
                    RemoveSpaces(CurrentInstruction);
                    if (CurrentInstruction == "" || CurrentInstruction.ElementAt(0) != ')')
                    {
                        gui.ReportError("Error: ')' expected");
                        return -2;
                    }
                    CurrentInstruction = CurrentInstruction.Substring(1);
                    OnlySpaces(0, CurrentInstruction.Length, CurrentInstruction);
                    args[2] = offset;
                    if(args[2] == -1)
                    {
                        gui.ReportError("Error: invalid offset");
                        return -2;
                    }
                }
                else //label
                {
                    tempString = findLabel();
                    bool foundLocation = false;
                    for (j = 0; j < MemoryTable.Count; j++)
                    {
                        if(tempString == MemoryTable[j].Label)
                        {
                            foundLocation = true;
                            if(OperationID == 14)
                            {
                                args[1] = int.Parse(MemoryTable[j].Value);
                            }
                            else
                            {
                                args[1] = j;
                            }
                            break;
                        }
                    }
                    if (!foundLocation)
                    {
                        gui.ReportError("Error: invalid label");
                        return -2;
                    }
                    args[2] = -1;
                }
                
            }else if(OperationID < 23) // beq blt
            {
                for(int count = 0; count <2; count++)
                {
                    RemoveSpaces(CurrentInstruction);
                    FindRegister(count);
                    RemoveSpaces(CurrentInstruction);
                    assertRemoveComma();
                }
                RemoveSpaces(CurrentInstruction);
                string tempString = findLabel();
                bool found = false;
                for(j=0;j< LabelTable.Count; j++)
                {
                    if(tempString == LabelTable[j].Label)
                    {
                        found = true;
                        args[2] = LabelTable[j].Address;
                        break;
                    }
                }
                if (!found)
                {
                    gui.ReportError("Error: invalid label");
                    return -2;
                }
            }else if(OperationID <26) // j bclt bclf
            {
                RemoveSpaces(CurrentInstruction);
                bool found = false;
                string tempString = findLabel();
                for(j = 0; j< LabelTable.Count; j++)
                {
                    if(tempString == LabelTable[j].Label)
                    {
                        found = true;
                        args[0] = LabelTable[j].Address;
                    }
                }
                if (!found)
                {
                    gui.ReportError("Error: invalid label");
                    return -2;
                }
            }else if(OperationID < 28) // mflo mfhi
            {
                RemoveSpaces(CurrentInstruction);
                if (!FindRegister(0))
                    return -2;
                if (!OnlySpaces(0, CurrentInstruction.Length, CurrentInstruction))
                    return -2;
            }

            return OperationID;
        }

        private string findLabel()
        {
            RemoveSpaces(CurrentInstruction);
            string tempString = "";
            bool foundValue = false;
            bool doneFinding = false;

            for(int j=0;j< CurrentInstruction.Length; j++)
            {
                char temp = CurrentInstruction.ElementAt(j);
                if(foundValue && (temp == ' ' || temp == '\t') && !doneFinding)
                {
                    doneFinding = true;
                }else if(foundValue && temp != ' ' && temp != '\t' && doneFinding)
                {
                    gui.ReportError("Error: Unexpected text after value");
                }else if(!foundValue && temp != ' ' && temp != '\t')
                {
                    foundValue = true;
                    tempString = tempString + temp;
                }else if(foundValue && temp != ' ' && temp != '\t')
                {
                    tempString = tempString + temp;
                }
            }

            return tempString;
        }

        private bool assertRemoveComma()
        {
            if(CurrentInstruction.Length <2 || CurrentInstruction.ElementAt(0) != ',')
            {
                gui.ReportError("Error: Comma expected");
                return false;
            }
            CurrentInstruction = CurrentInstruction.Substring(1);
            return true;
        }

        private bool FindRegister(int num)
        {
            bool foundRegister = false;
            if(CurrentInstruction.ElementAt(0) != '$' || CurrentInstruction.Length < 2)
            {
                gui.ReportError("Error: Register expected");
                return foundRegister;
            }
            CurrentInstruction = CurrentInstruction.Substring(1);
            string register = CurrentInstruction.Substring(0, 2);
            if(register == "ze" && CurrentInstruction.Length >= 4)
            {
                register += CurrentInstruction.Substring(2, 2);
            }
            else if(register == "ze")
            {
                gui.ReportError("Error: Register expected");
                return foundRegister;
            }
            register = "$" + register;
            for(int i =0;i< Registers.Count; i++)
            {
                if(register == Registers[i].Label)
                {
                    args[num] = i;
                    foundRegister = true;
                    if(i != 0)
                    {
                        CurrentInstruction = CurrentInstruction.Substring(2);
                    }
                    else
                    {
                        CurrentInstruction = CurrentInstruction.Substring(4);
                    }
                }
            }
            if (!foundRegister)
            {
                gui.ReportError("Error: Invalid register");
            }
            return foundRegister;
        }

        public bool ExecuteInstruction(int instruction)
        {
            switch (instruction)
            {
                case 0:
                    add();
                    break;
                case 1:
                    sub();
                    break;
                case 2:
                    and();
                    break;
                case 3:
                    or();
                    break;
                case 4:
                    slt();
                    break;
                case 5:
                    addd();
                    break;
                case 6:
                    subd();
                    break;
                case 7:
                    muld();
                    break;
                case 8:
                    divd();
                    break;
                case 9:
                    ceqd();
                    break;
                case 10:
                    cltd();
                    break;
                case 11:
                    mult();
                    break;
                case 12:
                    div();
                    break;
                case 13:
                    addi();
                    break;
                case 14:
                    andi();
                    break;
                case 15:
                    ori();
                    break;
                case 16:
                    slti();
                    break;
                case 17:
                    lw();
                    break;
                case 18:
                    sw();
                    break;
                case 19:
                    ldc1();
                    break;
                case 20:
                    sdc1();
                    break;
                case 21:
                    beq();
                    break;
                case 22:
                    blt();
                    break;
                case 23:
                    j();
                    break;
                case 24:
                    bc1t();
                    break;
                case 25:
                    bc1f();
                    break;
                case 26:
                    mflo();
                    break;
                case 27:
                    mfhi();
                    break;
                default:
                    return false;
            }
            return true;
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
                        return;
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
                            return;
                        }
                        else
                            doneFlag = true;
                    }

                    if (!assertLabelAllowed(tempString))
                        return;
                    MemoryData tempMemory = new MemoryData();
                    tempMemory.Label = tempString;
                    int wordIndex = CurrentInstruction.IndexOf(".word"); // TODO: Add directives for .float
                    int floatIndex = CurrentInstruction.IndexOf(".float");

                    if (!(floatIndex != -1 || wordIndex != -1))
                    {
                        gui.ReportError("Error: .word or .float not found");
                        return;
                    }
                    bool floatFlag;
                    int Index;
                    if (wordIndex == -1)
                    {
                        floatFlag = true;
                        Index = floatIndex;
                    }
                    else
                    {
                        floatFlag = false;
                        Index = wordIndex;
                    }
                    tempMemory.floatFlag = floatFlag;
                    if (tempMemory.floatFlag)
                        MemoryData.StaticMemory += 4;
                    if (!OnlySpaces(LabelIndex + 1, Index, CurrentInstruction))
                        return;
                    bool foundValue = false;
                    bool doneFinding = false;
                    tempString = "";
                    for (j = Index + (floatFlag ? 6:5); j < CurrentInstruction.Length; j++)
                    {
                        char temp = CurrentInstruction.ElementAt(j);
                        if(foundValue && (temp == ' ' || temp == '\t') && !doneFinding)
                        {
                            doneFinding = true;
                        }
                        else if(foundValue && temp != ' ' && temp != '\t' && doneFinding)
                        {
                            gui.ReportError("Error: Unexpected text after value");
                            return;
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
                    if (floatFlag)
                    {
                        double tempValue;
                        if (!double.TryParse(tempString, out tempValue))
                        {
                            gui.ReportError("Error: Float conversion error");
                            return;
                        }
                        tempMemory.Value = tempString;

                    }
                    else
                    {
                        int tempValue;
                        if(!int.TryParse(tempString,out tempValue))
                        {
                            gui.ReportError("Error: Int conversion error");
                            return;
                        }
                        tempMemory.Value = tempString;
                    }
                    MemoryTable.Add(tempMemory);
                }
            }
            MemoryTable.Sort(delegate (MemoryData m1, MemoryData m2) { return m1.Label.CompareTo(m2.Label); });
            for(int i = 0; i < MemoryTable.Count - 1; i++)
            {
                if(MemoryTable.ElementAt(i) == MemoryTable.ElementAt(i + 1))
                {
                    gui.ReportError("Error: One or more labels are repeated");
                    return;
                }
            }

            if (InputText.Count != 0)
                current_section = 1;

            if(current_section != 1)
            {
                gui.ReportError("Error: Text section does not exist or found unknown string");
                return;
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
                    return;
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
                        return;
                    }
                    else if(!isLabel)
                    {
                        gui.ReportError("Error: Label name expected");
                        return;
                    }
                    else
                    {
                        doneFlag = true;
                    }
                }
                if (!assertLabelAllowed(tempString))
                    return;
                if (!OnlySpaces(LabelIndex + 1, CurrentInstruction.Length, CurrentInstruction))
                    return;
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
                if(LabelTable[i].Label == LabelTable[i+1].Label)
                {
                    gui.ReportError("Error: One or more labels are repeated");
                    return;
                }
            }
            if(!FoundMain)
            {
                gui.ReportError("Error: Could not find main");
                return;
            }
            CurrentLine = MainIndex;
            gui.SendLog("Initialized and ready to execute.");
            gui.updateState();
        }

        private bool assertLabelAllowed(string str)
        {
            if(str.Length == 0 || char.IsDigit(str.ElementAt(0)))
            {
                gui.ReportError("Error: Invalid Label");
                return false;
            }
            for (int i = 0; i < str.Length; i++)
            {
                char temp = str.ElementAt(i);
                if (!(char.IsDigit(temp) || char.IsLetter(temp)))
                {
                    gui.ReportError("Error: Invalid Label");
                    return false;
                }
            }
            return true;
        }
        private bool OnlySpaces(int lower,int upper,string str)
        {
            for(int i = lower;i< upper; i++)
            {
                if(str.ElementAt(i)!= ' ' && str[i] != '\t')
                {
                    gui.ReportError("Error: Unexpected character");
                    return false;
                }
            }
            return true;
        }

        private string RemoveSpaces(string str)
        {
            int t = 0;
            while(t < str.Length && (str.ElementAt(t) == ' ' || str.ElementAt(t) == '\t'))
            {
                t++;
            }
            CurrentInstruction = str.Substring(t);
            return CurrentInstruction;
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

        public void Stop(string[] text, string[] data) 
        {
            PreLoad(text, data);
        }
        public void Reset(string[] text, string[] data)
        {
            Stop(text, data);
        }
    }
}
