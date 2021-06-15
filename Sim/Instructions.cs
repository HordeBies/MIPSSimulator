using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Sim
{
	public partial class MIPSSimulator
	{
		private void OpError()
        {
			gui.ReportError("Error: invalid usage of instruction");
			return;
		}
		private bool FindRegister(string id)
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
		//void add()
		//{
		//	if (args[0] == 29)
		//	{
		//		if (!checkStackBounds(int.Parse(Registers[args[1]].Value) + int.Parse(Registers[args[1]].Value)))
		//			return;
		//	} 
		//	if(args[0] != 0 && args[0] != 1 &&  args[1] != 1 && args[2] != 1)
		//          {
		//		Registers[args[0]].Value = (int.Parse(Registers[args[1]].Value) + int.Parse(Registers[args[2]].Value)).ToString();
		//          }
		//          else
		//          {
		//		OpError();
		//          }
		//}
		//void addi()
		//{
		//	bool spFlag = false;
		//	if(args[0] == 29)
		//          {
		//		spFlag = true;
		//		if (!checkStackBounds(int.Parse(Registers[args[1]].Value) + args[2]*4))
		//			return;
		//          }
		//	if(args[0] != 0 && args[0] != 1 && args[1] != 1)
		//          {
		//		Registers[args[0]].Value = (int.Parse(Registers[args[1]].Value) + (spFlag?args[2]*4:args[2])).ToString();
		//          }
		//          else
		//          {
		//		OpError();
		//		return;
		//          }
		//}
		//void sub()
		//{
		//	if (args[0] == 29)
		//	{
		//		if (!checkStackBounds(int.Parse(Registers[args[1]].Value) - int.Parse(Registers[args[1]].Value)))
		//			return;
		//	}
		//	if (args[0] != 0 && args[0] != 1 && args[1] != 1 && args[2] != 1)
		//	{
		//		Registers[args[0]].Value = (int.Parse(Registers[args[1]].Value) - int.Parse(Registers[args[2]].Value)).ToString();
		//	}
		//	else
		//	{
		//		OpError();
		//	}
		//}
		//void and()
		//{
		//	if (args[0] == 29)
		//	{
		//		if (!checkStackBounds(int.Parse(Registers[args[1]].Value) & int.Parse(Registers[args[1]].Value)))
		//			return;
		//	}
		//	if (args[0] != 0 && args[0] != 1 && args[1] != 1 && args[2] != 1)
		//	{
		//		Registers[args[0]].Value = (int.Parse(Registers[args[1]].Value) & int.Parse(Registers[args[2]].Value)).ToString();
		//	}
		//	else
		//	{
		//		OpError();
		//	}
		//}
		//void andi()
		//{
		//	if (args[0] == 29)
		//	{
		//		if (!checkStackBounds(int.Parse(Registers[args[1]].Value) & args[2]))
		//			return;
		//	}
		//	if (args[0] != 0 && args[0] != 1 && args[1] != 1)
		//	{
		//		Registers[args[0]].Value = (int.Parse(Registers[args[1]].Value) & args[2]).ToString();
		//	}
		//	else
		//	{
		//		OpError();
		//		return;
		//	}

		//}
		//void or()
		//{
		//	if (args[0] == 29)
		//	{
		//		if (!checkStackBounds(int.Parse(Registers[args[1]].Value) | int.Parse(Registers[args[1]].Value)))
		//			return;
		//	}
		//	if (args[0] != 0 && args[0] != 1 && args[1] != 1 && args[2] != 1)
		//	{
		//		Registers[args[0]].Value = (int.Parse(Registers[args[1]].Value) | int.Parse(Registers[args[2]].Value)).ToString();
		//	}
		//	else
		//	{
		//		OpError();
		//	}
		//}
		//void ori()
		//{
		//	if (args[0] == 29)
		//	{
		//		if (!checkStackBounds(int.Parse(Registers[args[1]].Value) | args[2]))
		//			return;
		//	}
		//	if (args[0] != 0 && args[0] != 1 && args[1] != 1)
		//	{
		//		Registers[args[0]].Value = (int.Parse(Registers[args[1]].Value) | args[2]).ToString();
		//	}
		//	else
		//	{
		//		OpError();
		//		return;
		//	}
		//}
		//void slt()
		//{
		//	if(args[0] != 0 && args[0] != 1 && args[1] != 1 && args[2] != 1)
		//          {
		//		Registers[args[0]].Value =(int.Parse(Registers[args[1]].Value) < int.Parse(Registers[args[2]].Value)) ? "1":"0";
		//          }
		//          else
		//          {
		//		OpError();
		//		return;
		//          }
		//}
		//void slti()
		//{
		//	if (args[0] != 0 && args[0] != 1 && args[1] != 1)
		//	{
		//		Registers[args[0]].Value = (int.Parse(Registers[args[1]].Value) < args[2]) ? "1" : "0";
		//	}
		//	else
		//	{
		//		OpError();
		//		return;
		//	}
		//}
		//void lw()
		//{
		//	if(args[0] == 29)
		//          {
		//		if (!checkStackBounds(int.Parse(Registers[args[1]].Value + args[2]*4)))
		//			return;
		//          }
		//	if(args[0] != 0 && args[0] != 1 && args[2] == -1)
		//          {
		//		Registers[args[0]].Value = MemoryTable[args[1]].Value;
		//          }
		//	else if(args[0] != 0 && args[0] != 1) 
		//	{
		//		int offsetPos = ((int.Parse(Registers[args[1]].Value) - 40000) / 4 + args[2] + 1);
		//		if (offsetPos > 99 || offsetPos < 0)
		//		{
		//			OpError();
		//			return;
		//		}
		//		Registers[args[0]].Value = Stack[offsetPos].Value;
		//	}
		//          else
		//          {
		//		OpError();
		//		return;
		//          }
		//}
		//      void sw()
		//{
		//	if(args[0] != 1 && args[2] == -1)
		//          {
		//		MemoryTable[args[1]].Value = Registers[args[0]].Value.ToString();
		//          }else if(args[0] != 1)
		//          {
		//		if (!checkStackBounds(int.Parse(Registers[args[1]].Value) + args[2] * 4))
		//              {
		//			OpError();
		//			return;
		//              }
		//		int offsetPos = ((int.Parse(Registers[args[1]].Value) -40000)/4 + args[2] +1);
		//		if (offsetPos > 99 || offsetPos < 0)
		//              {
		//			OpError();
		//			return;
		//              }
		//		Stack[offsetPos].Value = Registers[args[0]].Value;
		//          }
		//          else
		//          {
		//		OpError();
		//		return;
		//          }
		//}
		//void beq()
		//{
		//	if (args[0] != 1 && args[1] != 1)
		//	{
		//		LastLine = CurrentLine;
		//		if (int.Parse(Registers[args[0]].Value) == int.Parse(Registers[args[1]].Value))
		//		{
		//			CurrentLine = args[2];
		//		}
		//		else
		//			CurrentLine++;
		//          }
		//          else
		//          {
		//		OpError();
		//          }
		//}
		//void blt()
		//{
		//	if(args[0] != 1 && args[1] != 1)
		//          {
		//		LastLine = CurrentLine;
		//		if ((int.Parse((Registers[args[0]].Value))) < (int.Parse(Registers[args[1]].Value)))
		//		{
		//			CurrentLine = args[2];
		//		}
		//		else
		//			CurrentLine++;
		//          }
		//	else
		//	{
		//		OpError();
		//	}
		//}
		//void j()
		//{
		//	LastLine = CurrentLine;
		//	CurrentLine = args[0];
		//}
		//void ldc1()
		//{
		//	if(args[0] > 31 && args[2] == -1)
		//          {
		//              if (!MemoryTable[args[1]].floatFlag)
		//              {
		//			OpError();
		//			return;
		//              }
		//		Registers[args[0]].Value = MemoryTable[args[1]].Value;
		//		Registers[args[0]+1].Value = MemoryTable[args[1]].Value;
		//	}
		//	else if (args[0] > 31)
		//          {
		//		if (!checkStackBounds(int.Parse(Registers[args[1]].Value) + args[2]*4))
		//		{
		//			OpError();
		//			return;
		//		}
		//		int offsetPos = ((int.Parse(Registers[args[1]].Value) - 40000) / 4 + args[2] + 1);
		//		if (offsetPos > 99 || offsetPos < 0)
		//		{
		//			OpError();
		//			return;
		//		}
		//		if(Stack[offsetPos].Value == Stack[offsetPos - 1].Value)
		//              {
		//			Registers[args[0]].Value = Stack[offsetPos].Value;
		//			Registers[args[0]+1].Value = Stack[offsetPos].Value;
		//		}
		//		else
		//              {
		//                  OpError();
		//              }

		//	}
		//	else
		//	{
		//		OpError();
		//		return;
		//	}
		//}
		//void sdc1()
		//{
		//	if (args[0] > 31 && args[2] == -1)
		//          {
		//		if (!MemoryTable[args[1]].floatFlag)
		//		{
		//			OpError();
		//			return;
		//		}
		//		MemoryTable[args[1]].Value = Registers[args[0]].Value ;
		//	}
		//	else if (args[0] > 31)
		//	{
		//		if (!checkStackBounds(int.Parse(Registers[args[1]].Value) + args[2]*4))
		//		{
		//			OpError();
		//			return;
		//		}
		//		int offsetPos = ((int.Parse(Registers[args[1]].Value) - 40000) / 4 + args[2] + 1);
		//		if (offsetPos > 99 || offsetPos < 0 || offsetPos-1 <0 || offsetPos -1 > 99)
		//		{
		//			OpError();
		//			return;
		//		}
		//		Stack[offsetPos].Value = Registers[args[0]].Value;
		//		Stack[offsetPos-1].Value = Registers[args[0]].Value;
		//	}
		//}
		//void addd()
		//{
		//          if (args[0] > 31 && args[1] > 31 && args[2] > 31 && args[0] % 2 == 0 && args[1] % 2 == 0 && args[2] % 2 == 0)
		//          {
		//		string res = (double.Parse(Registers[args[1]].Value) + double.Parse(Registers[args[2]].Value)).ToString();
		//		Registers[args[0]].Value = res;
		//		Registers[args[0]+1].Value = res;
		//	}
		//	else
		//          {
		//		OpError();
		//		return;
		//	}
		//}
		//void subd()
		//{

		//	if (args[0] > 31 && args[1] > 31 && args[2] > 31 && args[0] % 2 == 0 && args[1] % 2 == 0 && args[2] % 2 == 0)
		//	{
		//		string res = (double.Parse(Registers[args[1]].Value) - double.Parse(Registers[args[2]].Value)).ToString();
		//		Registers[args[0]].Value = res;
		//		Registers[args[0]+1].Value = res;
		//	}
		//	else
		//	{
		//		OpError();
		//		return;
		//	}

		//}
		//void divd()
		//{
		//	if (args[0] > 31 && args[1] > 31 && args[2] > 31 && args[0] % 2 == 0 && args[1] % 2 == 0 && args[2] % 2 == 0)
		//	{
		//		string res = (double.Parse(Registers[args[1]].Value) / double.Parse(Registers[args[2]].Value)).ToString();
		//		Registers[args[0]].Value = res;
		//		Registers[args[0] + 1].Value = res;
		//	}
		//	else
		//	{
		//		OpError();
		//		return;
		//	}
		//}
		//void muld()
		//{
		//	if (args[0] > 31 && args[1] > 31 && args[2] > 31 && args[0] % 2 == 0 && args[1] % 2 == 0 && args[2] % 2 == 0)
		//	{
		//		string res = (double.Parse(Registers[args[1]].Value) * double.Parse(Registers[args[2]].Value)).ToString();
		//		Registers[args[0]].Value = res;
		//		Registers[args[0] + 1].Value = res;
		//	}
		//	else
		//	{
		//		OpError();
		//		return;
		//	}
		//}
		//void ceqd()
		//{
		//	if (args[0] > 31 && args[1] > 31 && args[0] % 2 == 0 && args[1] % 2 == 0)
		//	{
		//              FPA = double.Parse(Registers[args[0]].Value).Equals(double.Parse(Registers[args[1]].Value));
		//          }
		//	else
		//	{
		//		OpError();
		//		return;
		//	}

		//}
		//void cltd()
		//{
		//	if (args[0] > 31 && args[1] > 31 && args[0] % 2 == 0 && args[1] % 2 == 0)
		//	{
		//		FPA = double.Parse(Registers[args[0]].Value) < double.Parse(Registers[args[1]].Value);
		//	}
		//	else
		//	{
		//		OpError();
		//		return;
		//	}
		//}
		//void bc1t()
		//{
		//	LastLine = CurrentLine;
		//	if(FPA == true)
		//          {
		//		CurrentLine = args[0];
		//          }
		//          else
		//          {
		//		CurrentLine++;
		//          }
		//}
		//void bc1f()
		//{
		//	LastLine = CurrentLine;
		//	if (FPA == false)
		//	{
		//		CurrentLine = args[0];
		//          }
		//          else
		//          {
		//		CurrentLine++;
		//          }
		//}
		//void mfhi()
		//      {
		//	if(args[0] < 31 && args[0] > 1)
		//          {
		//		Registers[args[0]].Value = MFHI.ToString();
		//          }
		//      }
		//void mflo()
		//{
		//	if (args[0] < 31 && args[0] > 1)
		//	{
		//		Registers[args[0]].Value = MFLO.ToString();
		//	}
		//}
		//void mult()
		//{
		//	if (args[0] != 0 && args[0] != 1 && args[1] != 1)
		//	{
		//		long a = int.Parse(Registers[args[0]].Value);
		//              long b = int.Parse(Registers[args[1]].Value);
		//		long c = a * b;
		//		long hi = (c >> 32);
		//		MFLO = (int)(c);
		//		MFHI = (int)(hi);
		//	}
		//	else
		//	{
		//		OpError();
		//	}
		//}
		//void div()
		//{
		//	if (args[0] != 0 && args[0] != 1 && args[1] != 1)
		//	{
		//		MFLO = (int.Parse(Registers[args[0]].Value) / int.Parse(Registers[args[1]].Value));
		//		MFHI = (int.Parse(Registers[args[0]].Value) % int.Parse(Registers[args[1]].Value));
		//	}
		//	else
		//	{
		//		OpError();
		//	}
		//}

	}
}
