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
        private void Flush()
        {
            LabelTable = new List<LabelData>();
            iMemoryText = new List<string>();
            iMemory.ForEach(i => i.Value = 0);
            Registers.ForEach(i => i.Value = 0);
            Registers[6].Value = 255;
            gui.ClearLog();
            CurrentLine = 0;
        }
        public bool Compile(string[] input)
        {
            Flush();
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
                iMemoryText.Add(CurrentInstruction);
                try
                {
                    output = ParseInstruction();
                }
                catch (Exception e)
                {
                    gui.ReportError("Error: On Parsing Instruction on Line:" + currentLine);
                    gui.ReportError("\t" + e.Message);
                    return false;
                }
                ushort value = Convert.ToUInt16(output, 2);
                iMemory[InstructionCount].Value = value;

                while (assignLabel.Count > 0)
                    assignLabel.Dequeue().Address = (byte)InstructionCount;

                InstructionCount++;
                if(InstructionCount == 256)
                {
                    gui.ReportError("Error: Can't compile more than 256 instructions: "+ currentLine);
                    return false;
                }
            }
            iMemory[InstructionCount].Value = 0; //End of Instructions flag in Instruction Memory
            while (assignLabel.Count > 0)
                assignLabel.Dequeue().Address = (byte)InstructionCount;
            LabelTable.ForEach(i =>
            {
                if (i.Address >= iMemoryText.Count)
                    iMemoryText.Add(i.Label+":");
                else
                    iMemoryText[i.Address] = i.Label+": " + iMemoryText[i.Address];
            });
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

        private Register FindRegister()
        {
            RemoveSpaces();
            int j = 0;
            while (j < CurrentInstruction.Length && CurrentInstruction[j] != ',' && CurrentInstruction[j] != ' ' && CurrentInstruction[j] != '\t' && CurrentInstruction[j] != ')' )
            {
                j++;
            }
            string register = CurrentInstruction.Substring(0, j);
            CurrentInstruction = CurrentInstruction.Substring(j);
            return Registers.Find(i => i.Label == register);
        }
        private string Extend(string input, int extend)
        {
            if (input.Length == extend)
                return input;
            string result = "";
            for (int i = 0; i < extend - input.Length; i++)
            {
                result += '0';
            }
            result += input;
            return result;
        }
        private string SignExtend(string input, int extend)
        {
            if (input.Length == extend)
                return input;

            string result = "";
            result += input[0];
            for(int i = 0; i < extend - input.Length; i++)
            {
                result += '0';
            }
            result += input.Substring(1);
            return result;
        }
        private string FetchR()
        {
            try
            {
                RemoveSpaces();
                string r1 = Extend(Convert.ToString(FindRegister().id,2),3);
                RemoveSpaces();
                assertRemoveComma();
                RemoveSpaces();
                string r2 = Extend(Convert.ToString(FindRegister().id, 2), 3);
                RemoveSpaces();
                assertRemoveComma();
                RemoveSpaces();
                string r3 = Extend(Convert.ToString(FindRegister().id, 2), 3);
                RemoveSpaces();
                if (CurrentInstruction != "")
                    throw new Exception("Incorrect R Format");
                return r2+r3+r1;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        private string FindLabel()
        {
            int j = 0;
            while(j < CurrentInstruction.Length && CurrentInstruction[j] != ' ' && CurrentInstruction[j] != '\t')
            {
                j++;
            }
            string labelName = CurrentInstruction.Substring(0, j);
            CurrentInstruction = CurrentInstruction.Substring(j);
            return Convert.ToString(LabelTable.Find(i => i.Label == labelName).Address, 2);
        }
        private string FindOffset()
        {
            int j = 0;
            while(CurrentInstruction[j] != '(')
            {
                if (j > CurrentInstruction.Length)
                    throw new Exception("Invalid Offset");
                j++;
            }
            int offset = int.Parse(CurrentInstruction.Substring(0,j));
            CurrentInstruction = CurrentInstruction.Substring(j);
            if (CurrentInstruction[0] != '(')
                throw new Exception("Invalid Offset");
            CurrentInstruction = CurrentInstruction.Substring(1);
            Register r = FindRegister();
            if (CurrentInstruction[0] != ')')
                throw new Exception("Ivalid Offset");
            CurrentInstruction = CurrentInstruction.Substring(1);
            return Convert.ToString(r.Value+offset,2);
            
        }
        private string FindImmediate()
        {
            int j = 0;
            while(j < CurrentInstruction.Length && (CurrentInstruction[j] != ' ' && CurrentInstruction[j] != '\t') )
            {
                j++;
            }
            string imm = CurrentInstruction.Substring(0, j);
            CurrentInstruction = CurrentInstruction.Substring(j);
            if(imm[0] == '-')
            {
                return '1'+Convert.ToString(int.Parse(imm.Substring(1)), 2);
            }else
                return '0'+Convert.ToString(int.Parse(imm), 2);
        }
        private string FetchI(int LOI)
        {
            try
            {
                RemoveSpaces();
                string r1 = Extend(Convert.ToString(FindRegister().id, 2), 3);
                RemoveSpaces();
                assertRemoveComma();
                RemoveSpaces();
                string r2 = Extend(Convert.ToString(FindRegister().id, 2), 3);
                RemoveSpaces();
                assertRemoveComma();
                RemoveSpaces();
                string end = "";
                switch (LOI)
                {
                    case 0: //label
                         end = Extend(FindLabel(),6);
                        break;
                    case 1: //offset
                        end = Extend(FindOffset(),6);
                        break;
                    case 2: //immediate
                        end = SignExtend(FindImmediate(),6);
                        break;
                    default:
                        break;
                }
                RemoveSpaces();
                if (CurrentInstruction != "")
                    throw new Exception("Invalid I type instruction");
                return r2+r1+end;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private string FetchJ(bool jrFlag)
        {
            try
            {
                RemoveSpaces();
                string instruction = "";
                switch (jrFlag)
                {
                    case true:
                        instruction += Extend(Convert.ToString(FindRegister().Value,2),12);
                        break;
                    case false:
                        instruction += Extend(FindLabel(),12);
                        break;
                }
                RemoveSpaces();
                if(CurrentInstruction != "")
                    throw new Exception("Invalid J type Instruction");
                return instruction;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        private string ParseInstruction()
        {
            string instruction = "";
            int j;
            for (j = 0; j < CurrentInstruction.Length; j++)
            {
                var temp = CurrentInstruction[j];
                if (temp == ' ' || temp == '\t')
                    break;
            }
            string op = CurrentInstruction.Substring(0, j);

            CurrentInstruction = CurrentInstruction.Substring(j);

            if (!InstructionSet.ContainsKey(op))
                throw new Exception("'" + op + "' is not a valid instruction");

            instruction += InstructionSet[op];
            try
            {
                switch (op)
                {
                    case "add":
                        instruction += FetchR() + "000";
                        break;
                    case "sub":
                        instruction += FetchR() + "001";
                        break;
                    case "and":
                        instruction += FetchR() + "010";
                        break;
                    case "or":
                        instruction += FetchR() + "011";
                        break;
                    case "slt":
                        instruction += FetchR() + "100";
                        break;
                    case "jr":
                        instruction += FetchJ(true);
                        break;
                    case "mul":
                        instruction += FetchR() + "101";
                        break;
                    case "beq":
                    case "bne":
                        instruction += FetchI(0);
                        break;
                    case "sll":
                        instruction += FetchR() + "110";
                        break;
                    case "srl":
                        instruction += FetchR() + "111";
                        break;
                    case "muli":
                    case "lui":
                    case "slti":
                        instruction += FetchI(2);
                        break;
                    case "lw":
                    case "sw":
                        instruction += FetchI(1);
                        break;
                    case "j":
                    case "jal":
                        instruction += FetchJ(false);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {

                throw e;
            }

            return instruction;
            //int i = 0, j = 0; //temp vars

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
