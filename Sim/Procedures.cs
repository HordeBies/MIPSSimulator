using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Sim
{
    public partial class MIPSSimulator {
        public void Stop()
        {
            CurrentLine = 0;
        }
        public void Flush()
        {
            LabelTable = new List<LabelData>();
            iMemoryText = new List<string>();
            gui.BindCompiledText();
            iMemory.ForEach(i => i.Value = 0);
            Registers.ForEach(i => i.Value = 0);
            Registers[6].Value = 255;
            gui.ClearLog();
            gui.SelectTab(gui.MetroSetTabControl1, 0);
            gui.SelectTab(gui.MetroSetTabControl2, 0);
            gui.RefreshControls();
            CurrentLine = 0;
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
        public void ReadInstruction(int line)
        {
            CurrentInstruction = InputText[line];
            if (CurrentInstruction.Contains("#"))
            {
                CurrentInstruction = CurrentInstruction.Substring(0, CurrentInstruction.IndexOf("#"));
            }
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
            int InstructionCount = 0;
            int labelIndex = -1;
            Queue<LabelData> assignLabel = new Queue<LabelData>();
            //Second iteration, assign label adresses
            for (int currentLine = 0; currentLine < InputText.Count; currentLine++)
            {
                ReadInstruction(currentLine);
                RemoveSpaces();
                if (CurrentInstruction == "")
                {
                    continue;
                }
                if (CurrentInstruction.IndexOf(":") != -1)
                {
                    labelIndex++;
                    assignLabel.Enqueue(LabelTable[labelIndex]);
                    continue;
                }
                while (assignLabel.Count > 0)
                    assignLabel.Dequeue().Address = (byte)InstructionCount;
                InstructionCount++;
            }
            iMemory[InstructionCount].Value = 0; //End of Instructions flag in Instruction Memory
            while (assignLabel.Count > 0)
                assignLabel.Dequeue().Address = (byte)InstructionCount;

            if (LabelTable.GroupBy(x => x.Label).Any(g => g.Count() > 1))
            {
                gui.ReportError("Error: Repeated Label Found");
                return false;
            }
            InstructionCount = 0;
            //Third iteration, compile instructions into Instruction Memory
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
                    continue;
                }
                string output = "0";
                iMemoryText.Add(CurrentInstruction);
                try
                {
                    CurrentLine++;
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

                InstructionCount++;
                if(InstructionCount == 256)
                {
                    gui.ReportError("Error: Can't compile more than 256 instructions: "+ currentLine);
                    return false;
                }
            }
            LabelTable.ForEach(i =>
            {
                if (i.Address >= iMemoryText.Count)
                    iMemoryText.Add(i.Label+":");
                else
                    iMemoryText[i.Address] = i.Label+": " + iMemoryText[i.Address];
            });
            gui.SendLog("Initialized and ready to execute.");

            CurrentLine = 0;
            return true;
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
                    case "srl":
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
                        short val = (short)(FindLabel() - ((short)CurrentLine));
                        if(val < 0)
                        {
                            val = Math.Abs(val);
                            end = SignExtend("1"+Convert.ToString(val,2), 6);
                        }
                        else
                        {
                            end = Extend(Convert.ToString(val,2),6);
                        }
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
                        instruction += Extend(Convert.ToString(FindRegister().Value,2),3)+"000000000";
                        break;
                    case false:
                        instruction += Extend(Convert.ToString(FindLabel(),2),12);
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
        private short FindLabel()
        {
            int j = 0;
            while(j < CurrentInstruction.Length && CurrentInstruction[j] != ' ' && CurrentInstruction[j] != '\t')
            {
                j++;
            }
            string labelName = CurrentInstruction.Substring(0, j);
            CurrentInstruction = CurrentInstruction.Substring(j);
            return LabelTable.Find(i => i.Label == labelName).Address;
        }
        private string FindOffset()
        {
            int j = 0;
            while(CurrentInstruction[j] != '(')
            {
                if (j >= CurrentInstruction.Length)
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
                int number = int.Parse(imm.Substring(1));
                number = number > 31 ? 31 : number;
                return '1'+Convert.ToString(number, 2);
            }
            else
            {
                int number = int.Parse(imm);
                number = number > 31 ? 31 : number;
                return '0'+Convert.ToString(number, 2);
            }
        }
        public int Execute()
        {
            if (CurrentLine < iMemory.Count && iMemory[CurrentLine].Value != 0)
            {
                LastLine = CurrentLine;
                try
                {
                    ExecuteInstruction(iMemory[CurrentLine].Binary);
                    gui.RefreshControls();
                }
                catch (Exception e)
                {
                    gui.SendLog("on Line: " + CurrentLine.ToString());
                }
                CurrentLine++;
            }
            if (iMemory[CurrentLine].Value == 0)
            {
                gui.SendLog("Execution Completed");
            }
            return LastLine;
        }
        public bool ExecuteInstruction(string instruction)
        {
            string opCode = instruction.Substring(0, 4);

            switch (opCode)
            {
                case "0000":
                    string funcCode;
                    funcCode = instruction.Substring(13, 3);
                    switch (funcCode)
                    {
                        case "000":
                            Add(instruction);
                            break;
                        case "001":
                            Sub(instruction);
                            break;
                        case "010":
                            And(instruction);
                            break;
                        case "011":
                            Or(instruction);
                            break;
                        case "100":
                            Slt(instruction);
                            break;
                        case "101":
                            Mul(instruction);
                            break;
                        case "110":
                            Sll(instruction);
                            break;
                        case "111":
                            Srl(instruction);
                            break;
                        default:
                            break;
                    }
                    break;
                case "0001":
                    Jr(instruction);
                    break;
                case "0010":
                    Slti(instruction);
                    break;
                case "0011":
                    Beq(instruction);
                    break;
                case "0100":
                    Bne(instruction);
                    break;
                case "0101":
                    Muli(instruction);
                    break;
                case "0110":
                    Lui(instruction);
                    break;
                case "0111":
                    Lw(instruction);
                    break;
                case "1000":
                    Sw(instruction);
                    break;
                case "1001":
                    J(instruction);
                    break;
                case "1010":
                    Jal(instruction);
                    break;
                default:
                    break;
            }
            return true;
        }
    }
}
