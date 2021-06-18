using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPS.Sim
{
	public partial class MIPSSimulator
	{
		private short ConvertSignedNumber(string number)
        {
			short result;
			if(number[0] == '1')
            {
				result = (short)(0 - Convert.ToInt16(number.Substring(1), 2));
            }
            else
            {
				result = Convert.ToInt16(number.Substring(1), 2);
			}
			return result;
        }
		private Register FetchRd(string instruction)
		{
			return Registers[Convert.ToInt16(instruction.Substring(10, 3), 2)];
		}

		private Register FetchRt(string instruction)
		{
			return Registers[Convert.ToInt16(instruction.Substring(7, 3),2)];
		}

		private Register FetchRs(string instruction)
		{
			return Registers[Convert.ToInt16(instruction.Substring(4, 3),2)];
		}
		private short FetchOffset(string instruction)
		{
			return ConvertSignedNumber(instruction.Substring(10, 6));
		}

		private short FetchJumpOffset(string instruction)
		{
			return ConvertSignedNumber(instruction.Substring(4, 12));
		}

		void Add(string instruction)
		{
			Register rd = FetchRd(instruction);
			Register rt = FetchRt(instruction);
			Register rs = FetchRs(instruction);

			rd.Value = (short)(rt.Value + rs.Value);
			gui.SelectTab(gui.MetroSetTabControl2,2,Registers.IndexOf(rd));
		}

		void Sub(string instruction)
		{
			Register rd = FetchRd(instruction);
			Register rt = FetchRt(instruction);
			Register rs = FetchRs(instruction);

			rd.Value = (short)(rs.Value - rt.Value);
			gui.SelectTab(gui.MetroSetTabControl2, 2, Registers.IndexOf(rd));
		}

		void And(string instruction)
		{
			Register rd = FetchRd(instruction);
			Register rt = FetchRt(instruction);
			Register rs = FetchRs(instruction);

			rd.Value = (short)(rs.Value & rt.Value);
			gui.SelectTab(gui.MetroSetTabControl2, 2, Registers.IndexOf(rd));
		}

		void Or(string instruction)
		{
			Register rd = FetchRd(instruction);
			Register rt = FetchRt(instruction);
			Register rs = FetchRs(instruction);

			rd.Value = (short)(rs.Value | rt.Value);
			gui.SelectTab(gui.MetroSetTabControl2, 2, Registers.IndexOf(rd));
		}

		void Slt(string instruction)
		{
			Register rd = FetchRd(instruction);
			Register rs = FetchRs(instruction);
			Register rt = FetchRt(instruction);

			rd.Value = (short)(rs.Value < rt.Value ? 1 : 0);
			gui.SelectTab(gui.MetroSetTabControl2, 2, Registers.IndexOf(rd));
		}

		void Mul(string instruction)
		{
			Register rd = FetchRd(instruction);
			Register rt = FetchRt(instruction);
			Register rs = FetchRs(instruction);
			var rs1 = rs.Value;
			var rt1 = rt.Value;
			if (rs.Value > 127 || rs.Value < -128)
            {
				rs1 = (short)(rs.Value << 8);
				rs1 = (short)(rs1 >> 8);
            }
			if(rt.Value > 127 || rt.Value < -128)
            {
				rt1 = (short)(rt.Value << 8);
				rt1 = (short)(rt1 >> 8);
			}
			rd.Value = (short)(rs1 * rt1);
			gui.SelectTab(gui.MetroSetTabControl2, 2, Registers.IndexOf(rd));
		}

		void Sll(string instruction)
		{
			Register rt = FetchRt(instruction);
			Register rs = FetchRs(instruction);
			short imm = FetchOffset(instruction);
			rt.Value = (short)(rs.Value << imm);
			gui.SelectTab(gui.MetroSetTabControl2, 2, Registers.IndexOf(rt));
		}

		void Srl(string instruction)
		{
			Register rt = FetchRt(instruction);
			Register rs = FetchRs(instruction);
			short imm = FetchOffset(instruction);
			rt.Value = (short)(rs.Value >> imm);
			gui.SelectTab(gui.MetroSetTabControl2, 2, Registers.IndexOf(rt));
		}

		void Slti(string instruction)
		{
			Register rt = FetchRt(instruction);
			Register rs = FetchRs(instruction);
			short offset = FetchOffset(instruction);
			rt.Value = (short)(rs.Value < offset ? 1 : 0);
			gui.SelectTab(gui.MetroSetTabControl2, 2, Registers.IndexOf(rt));
		}
		void Beq(string instruction)
		{
			Register rt = FetchRt(instruction);
			Register rs = FetchRs(instruction);
			int offset = FetchOffset(instruction);

			if (rs.Value == rt.Value)
			{
				CurrentLine += offset;
			}
			gui.SelectTab(gui.MetroSetTabControl2, 2, -1);
		}
		void Bne(string instruction)
		{
			Register rt = FetchRt(instruction);
			Register rs = FetchRs(instruction);
			int offset = FetchOffset(instruction);

			if (rs.Value != rt.Value)
			{
				CurrentLine += offset;
			}
			gui.SelectTab(gui.MetroSetTabControl2, 2, -1);
		}
		void Muli(string instruction)
		{
			Register rs = FetchRs(instruction);
			Register rt = FetchRt(instruction);
			short imm = FetchOffset(instruction);
			short rs1 = rs.Value;
			if (rs.Value > 127 || rs.Value < -128)
			{
				rs1 = (short)(rs.Value << 8);
				rs1 = (short)(rs1 >> 8);
			}
			rt.Value = (short)(rs1 * imm);
			gui.SelectTab(gui.MetroSetTabControl2, 2, Registers.IndexOf(rt));
		}
		void Lui(string instruction)
		{
			Register rt = FetchRt(instruction);
			short imm = FetchOffset(instruction);
			rt.Value = (short)(imm << 8);
			gui.SelectTab(gui.MetroSetTabControl2, 2, Registers.IndexOf(rt));
		}
		void Lw(string instruction)
		{
			Register rt = FetchRt(instruction);
			Register rs = FetchRs(instruction);
			short offset = FetchOffset(instruction);
			rt.Value = dMemory[rs.Value + offset].Value;
			gui.SelectTab(gui.MetroSetTabControl2, 2, Registers.IndexOf(rt));
		}
		void Sw(string instruction)
		{
			Register rt = FetchRt(instruction);
			Register rs = FetchRs(instruction);
			short offset = FetchOffset(instruction);
			dMemory[rs.Value + offset].Value = rt.Value;
			gui.SelectTab(gui.MetroSetTabControl2, 1, rs.Value+offset);
		}
		void J(string instruction)
		{
			CurrentLine = FetchJumpOffset(instruction) - 1;
		}
		void Jal(string instruction)
		{
			Registers[7].Value = (short)(CurrentLine + 1);
			CurrentLine = FetchJumpOffset(instruction) - 1;
			gui.SelectTab(gui.MetroSetTabControl2, 2, 7);
		}


		void Jr(string instruction)
		{
			Register rs = FetchRs(instruction);
			CurrentLine = rs.Value - 1;
			gui.SelectTab(gui.MetroSetTabControl2, 2, Registers.IndexOf(rs));
		}

	}
}