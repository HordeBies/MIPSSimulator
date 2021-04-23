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
				if (!checkStackBounds(Registers[args[1]].Value + Registers[args[1]].Value))
					return;
			} 
			if(args[0] != 0 && args[0] != 1 &&  args[1] != 1 && args[2] != 1)
            {
				Registers[args[0]].Value = Registers[args[1]].Value + Registers[args[2]].Value;
            }
            else
            {
				OpError();
            }
		}
		void addi()
		{
			if(args[0] == 29)
            {
				if (!checkStackBounds(Registers[args[1] + args[2]].Value))
					return;
            }
			if(args[0] != 0 && args[0] != 1 && args[1] != 1)
            {
				Registers[args[0]].Value = Registers[args[1]].Value + args[2];
            }
            else
            {
				OpError();
				return;
            }
		}
		void sub()
		{
			throw new NotImplementedException();
		}
		void and()
		{
			throw new NotImplementedException();
		}
		void andi()
		{
			throw new NotImplementedException();
		}
		void or()
		{
			throw new NotImplementedException();
		}
		void ori()
		{
			throw new NotImplementedException();
		}
		void slt()
		{
			throw new NotImplementedException();
		}
		void slti()
		{
			throw new NotImplementedException();
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
				Registers[args[0]].Value = args[1];
            }
            else
            {
				OpError();
				return;
            }
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

        void sw()
		{
			if(args[0] != 1 && args[2] == -1)
            {
				MemoryTable[args[1]].Value = Registers[args[0]].Value;
            }else if(args[0] != 1)
            {
				if (!checkStackBounds(Registers[args[1]].Value + args[2] * 4))
                {
					OpError();
					return;
                }
				int offsetPos = ((Registers[args[1]].Value -40000)/4 + args[2] +1);
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
			throw new NotImplementedException();
		}
		void bne()
		{
			throw new NotImplementedException();
		}
		void j()
		{
			LastLine = CurrentLine;
			CurrentLine = args[0];
		}
		void addd()
		{
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
		void subd()
		{
			throw new NotImplementedException();
		}
		void jal()
		{
			LastLine = CurrentLine;
			Registers[31].Value = CurrentLine + 1;
			CurrentLine = args[0];
		}
		void bc1t()
		{
			throw new NotImplementedException();
		}
		void bc1f()
		{
			throw new NotImplementedException();
		}
	}
}
