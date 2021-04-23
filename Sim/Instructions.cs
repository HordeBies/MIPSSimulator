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
		void add()
		{
			if (args[0] == 29)
			{
				if (!checkStackBounds(int.Parse(Registers[args[1]].Value) + int.Parse(Registers[args[1]].Value)))
					return;
			} 
			if(args[0] != 0 && args[0] != 1 &&  args[1] != 1 && args[2] != 1)
            {
				Registers[args[0]].Value = (int.Parse(Registers[args[1]].Value) + int.Parse(Registers[args[2]].Value)).ToString();
            }
            else
            {
				OpError();
            }
		}
		void addi()
		{
			bool spFlag = false;
			if(args[0] == 29)
            {
				spFlag = true;
				if (!checkStackBounds(int.Parse(Registers[args[1]].Value) + args[2]))
					return;
            }
			if(args[0] != 0 && args[0] != 1 && args[1] != 1)
            {
				Registers[args[0]].Value = (int.Parse(Registers[args[1]].Value) + (spFlag?args[2]*4:args[2])).ToString();
            }
            else
            {
				OpError();
				return;
            }
		}
		void sub()
		{
			if (args[0] == 29)
			{
				if (!checkStackBounds(int.Parse(Registers[args[1]].Value) - int.Parse(Registers[args[1]].Value)))
					return;
			}
			if (args[0] != 0 && args[0] != 1 && args[1] != 1 && args[2] != 1)
			{
				Registers[args[0]].Value = (int.Parse(Registers[args[1]].Value) - int.Parse(Registers[args[2]].Value)).ToString();
			}
			else
			{
				OpError();
			}
		}
		void and()
		{
			if (args[0] == 29)
			{
				if (!checkStackBounds(int.Parse(Registers[args[1]].Value) & int.Parse(Registers[args[1]].Value)))
					return;
			}
			if (args[0] != 0 && args[0] != 1 && args[1] != 1 && args[2] != 1)
			{
				Registers[args[0]].Value = (int.Parse(Registers[args[1]].Value) & int.Parse(Registers[args[2]].Value)).ToString();
			}
			else
			{
				OpError();
			}
		}
		void andi()
		{
			if (args[0] == 29)
			{
				if (!checkStackBounds(int.Parse(Registers[args[1]].Value) & args[2]))
					return;
			}
			if (args[0] != 0 && args[0] != 1 && args[1] != 1)
			{
				Registers[args[0]].Value = (int.Parse(Registers[args[1]].Value) & args[2]).ToString();
			}
			else
			{
				OpError();
				return;
			}

		}
		void or()
		{
			if (args[0] == 29)
			{
				if (!checkStackBounds(int.Parse(Registers[args[1]].Value) | int.Parse(Registers[args[1]].Value)))
					return;
			}
			if (args[0] != 0 && args[0] != 1 && args[1] != 1 && args[2] != 1)
			{
				Registers[args[0]].Value = (int.Parse(Registers[args[1]].Value) | int.Parse(Registers[args[2]].Value)).ToString();
			}
			else
			{
				OpError();
			}
		}
		void ori()
		{
			if (args[0] == 29)
			{
				if (!checkStackBounds(int.Parse(Registers[args[1]].Value) | args[2]))
					return;
			}
			if (args[0] != 0 && args[0] != 1 && args[1] != 1)
			{
				Registers[args[0]].Value = (int.Parse(Registers[args[1]].Value) | args[2]).ToString();
			}
			else
			{
				OpError();
				return;
			}
		}
		void slt()
		{
			if(args[0] != 0 && args[0] != 1 && args[1] != 1 && args[2] != 1)
            {
				Registers[args[0]].Value =(int.Parse(Registers[args[1]].Value) < int.Parse(Registers[args[2]].Value)) ? "1":"0";
            }
            else
            {
				OpError();
				return;
            }
		}
		void slti()
		{
			if (args[0] != 0 && args[0] != 1 && args[1] != 1)
			{
				Registers[args[0]].Value = (int.Parse(Registers[args[1]].Value) < args[2]) ? "1" : "0";
			}
			else
			{
				OpError();
				return;
			}
		}
		void lw()
		{
			if(args[0] == 29)
            {
				if (!checkStackBounds(args[1]))
					return;
            }
			if(args[0] != 0 && args[0] != 1 && args[2] == -1)
            {
				Registers[args[0]].Value = args[1].ToString();
            }
			else if(args[0] != 0 && args[0] != 1) 
			{
				if (!checkStackBounds(int.Parse(Registers[args[1] + args[2]].Value)))
				{
					OpError();
					return;
				}
				int offsetPos = ((int.Parse(Registers[args[1]].Value) - 40000) / 4 + args[2] + 1);
				if (offsetPos > 99 || offsetPos < 0)
				{
					OpError();
					return;
				}
				Registers[args[0]].Value = Stack[offsetPos].Value;
			}
            else
            {
				OpError();
				return;
            }
		}
        void sw()
		{
			if(args[0] != 1 && args[2] == -1)
            {
				MemoryTable[args[1]].Value = Registers[args[0]].Value.ToString();
            }else if(args[0] != 1)
            {
				if (!checkStackBounds(int.Parse(Registers[args[1]].Value) + args[2] * 4))
                {
					OpError();
					return;
                }
				int offsetPos = ((int.Parse(Registers[args[1]].Value) -40000)/4 + args[2] +1);
				if (offsetPos > 99 || offsetPos < 0)
                {
					OpError();
					return;
                }
				Stack[offsetPos].Value = Registers[args[0]].Value;
            }
            else
            {
				OpError();
				return;
            }
		}
		void beq()
		{
			if (args[0] != 1 && args[1] != 1)
			{
				LastLine = CurrentLine;
				if (Registers[args[0]] != Registers[args[1]])
				{
					CurrentLine = args[2];
				}
				else
					CurrentLine++;
            }
            else
            {
				OpError();
            }
		}
		void bne()
		{
			if(args[0] != 1 && args[1] != 1)
            {
				LastLine = CurrentLine;
				if (Registers[args[0]] != Registers[args[1]])
				{
					CurrentLine = args[2];
				}
				else
					CurrentLine++;
            }
			else
			{
				OpError();
			}
		}
		void j()
		{
			LastLine = CurrentLine;
			CurrentLine = args[0];
		}
		void jal()
		{
			LastLine = CurrentLine;
			Registers[31].Value = (CurrentLine + 1).ToString();
			CurrentLine = args[0];
		}
		void ldc1()
		{
			if(args[0] > 31 && args[2] == -1)
            {
                if (!MemoryTable[args[1]].floatFlag)
                {
					OpError();
					return;
                }
				Registers[args[0]].Value = MemoryTable[args[1]].Value;
			}else if (args[0] > 31)
            {
				if (!checkStackBounds(int.Parse(Registers[args[1] + args[2]].Value)))
				{
					OpError();
					return;
				}
				int offsetPos = ((int.Parse(Registers[args[1]].Value) - 40000) / 4 + args[2] + 1);
				if (offsetPos > 99 || offsetPos < 0)
				{
					OpError();
					return;
				}
				Registers[args[0]].Value = Stack[offsetPos].Value;
			}
			else
			{
				OpError();
				return;
			}
			gui.SendLog(args.ToString());
		}
		void sdc1()
		{
			throw new NotImplementedException();
		}
		void addd()
		{
            if (args[0] > 31 && args[1] > 31 && args[2] > 31)
            {
				Registers[args[0]].Value = (double.Parse(Registers[args[1]].Value) + double.Parse(Registers[args[2]].Value)).ToString();
			}
            else
            {
				OpError();
				return;
			}
		}
		void subd()
		{
			gui.SendLog(args.ToString());
			throw new NotImplementedException();
		}
		void ceqd()
		{
			throw new NotImplementedException();
		}
		void cltd()
		{
			throw new NotImplementedException();
		}
		void cled()
		{
			throw new NotImplementedException();
		}
		void bc1t()
		{
			throw new NotImplementedException();
		}
		void bc1f()
		{
			throw new NotImplementedException();
		}
		private bool checkStackBounds(int idx)
        {

			if (!( idx< 40400 && idx >= 40000))
            {
				gui.ReportError("Error: Invalid address for stack pointer");
				return false;
            }
			return true;
        }
	}
}
