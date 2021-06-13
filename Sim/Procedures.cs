using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Sim
{
    public partial class MIPSSimulator {
        public void ReadInstruction(int line)
        {
            CurrentInstruction = InputText[line];
            if (CurrentInstruction.Contains("#"))
            {
                CurrentInstruction = CurrentInstruction.Substring(0, CurrentInstruction.IndexOf("#"));
            }
        }
        public void RemoveSpaces()
        {
            int i = 0;
            while (i < CurrentInstruction.Length && (CurrentInstruction[i] == ' ' || CurrentInstruction[i] == '\t'))
            {
                i++;
            }
            CurrentInstruction = CurrentInstruction.Substring(i);
        }
        private bool assertLabelAllowed(string str)
        {
            if (str.Length == 0 || char.IsDigit(str.ElementAt(0)))
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
        public bool Compile(string[] input)
        {
            InputText = new List<string>(input);

            int LabelIndex;
            //First iteration. Find Labels
            for (int currentLine = 0; currentLine < InputText.Count; currentLine++)
            {
                ReadInstruction(currentLine);
                if (CurrentInstruction == "")
                {
                    continue;
                }
                RemoveSpaces();
                LabelIndex = CurrentInstruction.IndexOf(":");
                if (LabelIndex == 0)
                {
                    gui.ReportError("Error: Label name expected on Line:" + currentLine);
                    return false;
                }
                if(LabelIndex == -1)
                {
                    continue;
                }
                string LabelName = CurrentInstruction.Substring(0, LabelIndex);

                if (!assertLabelAllowed(LabelName))
                {
                    gui.ReportError("Error: Incorrect Label Format on Line:" + currentLine);
                    return false;
                }

                CurrentInstruction = CurrentInstruction.Substring(LabelIndex + 1);
                RemoveSpaces();

                if (CurrentInstruction != "")
                {
                    gui.ReportError("Error: Incorrect Instruction Format on Line:" + currentLine);
                }
                else
                {
                    LabelData tempLabel = new LabelData();
                    tempLabel.Address = 0;
                    tempLabel.Label = LabelName;
                    LabelTable.Add(tempLabel);
                }
            }
            //Second iteration, compile instructions into Instruction Memory
            if(LabelTable.GroupBy(x => x.Label).Any(g => g.Count() > 1))
            {
                gui.ReportError("Error: Repeated Label Found");
                return false;
            }

            int InstructionCount = 0;
            int labelIndex = -1;
            Queue<LabelData> assignLabel = new Queue<LabelData>();
            for (int currentLine = 0; currentLine < InputText.Count; currentLine++)
            {
                ReadInstruction(currentLine);
                RemoveSpaces();
                if(CurrentInstruction == "")
                {
                    continue;
                }
                if (CurrentInstruction.IndexOf(":") != -1)
                {
                    labelIndex++;
                    assignLabel.Enqueue(LabelTable[labelIndex]);
                    continue;
                }
                string output = "0";
                try
                {
                    output = ParseInstruction();
                }
                catch (Exception e)
                {
                    gui.ReportError("Error: On Parsing Instruction on Line:" + currentLine);
                    return false;
                }
                short value = Convert.ToInt16(output, 2);
                iMemory[InstructionCount].Value = value;
                while (assignLabel.Count > 0)
                {
                    assignLabel.Dequeue().Address = (byte)InstructionCount;
                }
                InstructionCount++;
            }

            CurrentLine = 0;
            gui.SendLog("Initialized and ready to execute.");
            gui.updateState();

            return true;
        }
        public int Execute()
        {
            if (CurrentLine < iMemory.Count && iMemory[CurrentLine].Value != 0)
            {
                CurrentLine++;
                try
                {
                    ExecuteInstruction(iMemory[CurrentLine].Value);
                }
                catch (Exception e)
                {
                    gui.SendLog("on Line: " + CurrentLine.ToString());
                }
            }
            if (CurrentLine >= iMemory.Count)
            {
                gui.SendLog("Execution Completed");
            }
            return CurrentLine;
        }
        private string ParseInstruction()
        {
            if(false)
                throw new Exception("Tried Parsing Label as Instruction");
            string instruction = "0000000000000000";
            
            //parse instruction as binary 16 bits in string

            return instruction;
            //int i = 0, j = 0; //temp vars

            //RemoveSpaces(CurrentInstruction);

            //if (CurrentInstruction.Contains(":"))
            //    return -2;

            //if (CurrentInstruction.Length < 4)
            //{
            //    gui.ReportError("Error: Unknown operation");
            //    return -2;
            //}

            //for (j = 0; j < CurrentInstruction.Length; j++)
            //{
            //    var temp = CurrentInstruction.ElementAt(j);
            //    if (temp == ' ' || temp == '\t')
            //        break;
            //}

            //string op = CurrentInstruction.Substring(0, j);

            //if (CurrentInstruction.Length > 0 && j < CurrentInstruction.Length - 1)
            //    CurrentInstruction = CurrentInstruction.Substring(j + 1);

            //int OperationID = -1;
            //for (i = 0; i < InstructionSet.Length; i++)
            //{
            //    if (op == InstructionSet[i])
            //    {
            //        OperationID = i;
            //        break;
            //    }
            //}
            //if (OperationID == -1)
            //{
            //    gui.ReportError("Error:Unknown operation");
            //    return -2;
            //}
            //if (OperationID < 13) // R Format
            //{
            //    for (int count = 0; count < 3 - (OperationID > 8 ? 1 : 0); count++)
            //    {
            //        RemoveSpaces(CurrentInstruction);
            //        if (!FindRegister(count))
            //            return -2;
            //        RemoveSpaces(CurrentInstruction);
            //        if (count == 2 - (OperationID > 8 ? 1 : 0))
            //            break;
            //        if (!assertRemoveComma())
            //            return -2;
            //    }
            //    if (CurrentInstruction != "")
            //    {
            //        gui.ReportError("Error: Extra arguments provided");
            //        return -2;
            //    }
            //}
            //else if (OperationID < 17) // I format
            //{
            //    for (int count = 0; count < 2; count++)
            //    {
            //        RemoveSpaces(CurrentInstruction);
            //        FindRegister(count);
            //        RemoveSpaces(CurrentInstruction);
            //        assertRemoveComma();
            //    }
            //    RemoveSpaces(CurrentInstruction);
            //    string tempString = findLabel();
            //    int temp;
            //    if (!int.TryParse(tempString, out temp))
            //    {
            //        gui.ReportError("Error: Not a valid Immediate argument");
            //        return -2;
            //    }
            //    args[2] = temp;
            //}
            //else if (OperationID < 21) // lw sw ldc1 sdc1
            //{
            //    string tempString = "";
            //    int offset;
            //    RemoveSpaces(CurrentInstruction);
            //    FindRegister(0);
            //    RemoveSpaces(CurrentInstruction);
            //    assertRemoveComma();
            //    RemoveSpaces(CurrentInstruction);
            //    if ((CurrentInstruction.ElementAt(0) > 47 && CurrentInstruction.ElementAt(0) < 58) || CurrentInstruction.ElementAt(0) == '-')
            //    {
            //        j = 0;
            //        while (j < CurrentInstruction.Length && CurrentInstruction.ElementAt(j) != ' '
            //            && CurrentInstruction.ElementAt(j) != '\t' && CurrentInstruction.ElementAt(j) != '(')
            //        {
            //            tempString = tempString + CurrentInstruction.ElementAt(j);
            //            j++;
            //        }
            //        if (j == CurrentInstruction.Length)
            //        {
            //            gui.ReportError("Error: '(' expected");
            //            return -2;
            //        }
            //        int temp;
            //        if (!int.TryParse(tempString, out temp))
            //        {
            //            gui.ReportError("Error: not a valid offset");
            //            return -2;
            //        }
            //        offset = temp;
            //        CurrentInstruction = CurrentInstruction.Substring(j);
            //        RemoveSpaces(CurrentInstruction);
            //        if (CurrentInstruction == "" || CurrentInstruction.ElementAt(0) != '(' || CurrentInstruction.Length < 2)
            //        {
            //            gui.ReportError("Error: '(' expected");
            //            return -2;
            //        }
            //        CurrentInstruction = CurrentInstruction.Substring(1);
            //        RemoveSpaces(CurrentInstruction);
            //        FindRegister(1);
            //        RemoveSpaces(CurrentInstruction);
            //        if (CurrentInstruction == "" || CurrentInstruction.ElementAt(0) != ')')
            //        {
            //            gui.ReportError("Error: ')' expected");
            //            return -2;
            //        }
            //        CurrentInstruction = CurrentInstruction.Substring(1);
            //        OnlySpaces(0, CurrentInstruction.Length, CurrentInstruction);
            //        args[2] = offset;
            //        if (args[2] == -1)
            //        {
            //            gui.ReportError("Error: invalid offset");
            //            return -2;
            //        }
            //    }
            //    else //label
            //    {
            //        tempString = findLabel();
            //        bool foundLocation = false;
            //        for (j = 0; j < MemoryTable.Count; j++)
            //        {
            //            if (tempString == MemoryTable[j].Label)
            //            {
            //                foundLocation = true;

            //                args[1] = j;
            //                break;
            //            }
            //        }
            //        if (!foundLocation)
            //        {
            //            gui.ReportError("Error: invalid label");
            //            return -2;
            //        }
            //        args[2] = -1;
            //    }

            //}
            //else if (OperationID < 23) // beq blt
            //{
            //    for (int count = 0; count < 2; count++)
            //    {
            //        RemoveSpaces(CurrentInstruction);
            //        FindRegister(count);
            //        RemoveSpaces(CurrentInstruction);
            //        assertRemoveComma();
            //    }
            //    RemoveSpaces(CurrentInstruction);
            //    string tempString = findLabel();
            //    bool found = false;
            //    for (j = 0; j < LabelTable.Count; j++)
            //    {
            //        if (tempString == LabelTable[j].Label)
            //        {
            //            found = true;
            //            args[2] = LabelTable[j].Address;
            //            break;
            //        }
            //    }
            //    if (!found)
            //    {
            //        gui.ReportError("Error: invalid label");
            //        return -2;
            //    }
            //}
            //else if (OperationID < 26) // j bclt bclf
            //{
            //    RemoveSpaces(CurrentInstruction);
            //    bool found = false;
            //    string tempString = findLabel();
            //    for (j = 0; j < LabelTable.Count; j++)
            //    {
            //        if (tempString == LabelTable[j].Label)
            //        {
            //            found = true;
            //            args[0] = LabelTable[j].Address;
            //        }
            //    }
            //    if (!found)
            //    {
            //        gui.ReportError("Error: invalid label");
            //        return -2;
            //    }
            //}
            //else if (OperationID < 28) // mflo mfhi
            //{
            //    RemoveSpaces(CurrentInstruction);
            //    if (!FindRegister(0))
            //        return -2;
            //    if (!OnlySpaces(0, CurrentInstruction.Length, CurrentInstruction))
            //        return -2;
            //}
        }
        private bool assertRemoveComma()
        {
            if (CurrentInstruction.Length < 2 || CurrentInstruction.ElementAt(0) != ',')
            {
                gui.ReportError("Error: Comma expected");
                return false;
            }
            CurrentInstruction = CurrentInstruction.Substring(1);
            return true;
        }
        private bool FindRegister(int num)
        {
            return true;
            //bool foundRegister = false;
            //if (CurrentInstruction.ElementAt(0) != '$' || CurrentInstruction.Length < 2)
            //{
            //    gui.ReportError("Error: Register expected");
            //    return foundRegister;
            //}
            //CurrentInstruction = CurrentInstruction.Substring(1);
            //string register = CurrentInstruction.Substring(0, 2);
            //if (register == "ze" && CurrentInstruction.Length >= 4)
            //{
            //    register += CurrentInstruction.Substring(2, 2);
            //}
            //else if (register == "ze")
            //{
            //    gui.ReportError("Error: Register expected");
            //    return foundRegister;
            //}
            //register = "$" + register;
            //for (int i = 0; i < Registers.Count; i++)
            //{
            //    if (register == Registers[i].Label)
            //    {
            //        args[num] = i;
            //        foundRegister = true;
            //        if (i != 0)
            //        {
            //            CurrentInstruction = CurrentInstruction.Substring(2);
            //        }
            //        else
            //        {
            //            CurrentInstruction = CurrentInstruction.Substring(4);
            //        }
            //    }
            //}
            //if (!foundRegister)
            //{
            //    gui.ReportError("Error: Invalid register");
            //}
            //return foundRegister;
        }
        public bool ExecuteInstruction(int instruction)
        {
            //switch (instruction)
            //{
            //    case 0:
            //        add();
            //        break;
            //    case 1:
            //        sub();
            //        break;
            //    case 2:
            //        and();
            //        break;
            //    case 3:
            //        or();
            //        break;
            //    case 4:
            //        slt();
            //        break;
            //    case 5:
            //        addd();
            //        break;
            //    case 6:
            //        subd();
            //        break;
            //    case 7:
            //        muld();
            //        break;
            //    case 8:
            //        divd();
            //        break;
            //    case 9:
            //        ceqd();
            //        break;
            //    case 10:
            //        cltd();
            //        break;
            //    case 11:
            //        mult();
            //        break;
            //    case 12:
            //        div();
            //        break;
            //    case 13:
            //        addi();
            //        break;
            //    case 14:
            //        andi();
            //        break;
            //    case 15:
            //        ori();
            //        break;
            //    case 16:
            //        slti();
            //        break;
            //    case 17:
            //        lw();
            //        break;
            //    case 18:
            //        sw();
            //        break;
            //    case 19:
            //        ldc1();
            //        break;
            //    case 20:
            //        sdc1();
            //        break;
            //    case 21:
            //        beq();
            //        break;
            //    case 22:
            //        blt();
            //        break;
            //    case 23:
            //        j();
            //        break;
            //    case 24:
            //        bc1t();
            //        break;
            //    case 25:
            //        bc1f();
            //        break;
            //    case 26:
            //        mflo();
            //        break;
            //    case 27:
            //        mfhi();
            //        break;
            //    default:
            //        return false;
            //}
            return true;
        }
        private bool OnlySpaces(int lower, int upper, string str)
        {
            for (int i = lower; i < upper; i++)
            {
                if (str.ElementAt(i) != ' ' && str[i] != '\t')
                {
                    gui.ReportError("Error: Unexpected character");
                    return false;
                }
            }
            return true;
        }
    }
}
