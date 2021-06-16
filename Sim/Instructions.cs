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
		}

		void Sub(string instruction)
		{
			Register rd = FetchRd(instruction);
			Register rt = FetchRt(instruction);
			Register rs = FetchRs(instruction);

			rd.Value = (short)(rs.Value - rt.Value);
		}

		void And(string instruction)
		{
			Register rd = FetchRd(instruction);
			Register rt = FetchRt(instruction);
			Register rs = FetchRs(instruction);

			rd.Value = (short)(rs.Value & rt.Value);
		}

		void Or(string instruction)
		{
			Register rd = FetchRd(instruction);
			Register rt = FetchRt(instruction);
			Register rs = FetchRs(instruction);

			rd.Value = (short)(rs.Value | rt.Value);
		}

		void Slt(string instruction)
		{
			Register rd = FetchRd(instruction);
			Register rs = FetchRs(instruction);
			Register rt = FetchRt(instruction);

			rd.Value = (short)(rs.Value < rt.Value ? 1 : 0);
		}

		void Mul(string instruction)
		{
			Register rd = FetchRd(instruction);
			Register rt = FetchRt(instruction);
			Register rs = FetchRs(instruction);
			var rs1 = Convert.ToUInt16(rs.Binary.Substring(8, 8),2);
			var rt1 = Convert.ToUInt16(rt.Binary.Substring(8, 8),2);
			rd.Value = (short)(rs1 * rt1);
		}

		void Sll(string instruction)
		{
			Register rd = FetchRd(instruction);
			Register rs = FetchRs(instruction);
			short imm = FetchOffset(instruction);
			rd.Value = (short)(rs.Value << imm);
		}

		void Srl(string instruction)
		{
			Register rd = FetchRd(instruction);
			Register rs = FetchRs(instruction);
			short imm = FetchOffset(instruction);
			rd.Value = (short)(rs.Value >> imm);
		}

		void Slti(string instruction)
		{
			Register rt = FetchRt(instruction);
			Register rs = FetchRs(instruction);
			short offset = FetchOffset(instruction);
			rt.Value = (short)(rs.Value < offset ? 1 : 0);
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
		}
		void Muli(string instruction)
		{
			Register rs = FetchRs(instruction);
			Register rt = FetchRt(instruction);
			short imm = FetchOffset(instruction);
			rt.Value = (short)(rs.Value * imm);
		}
		void Lui(string instruction)
		{

		}
		void Lw(string instruction)
		{
			Register rt = FetchRt(instruction);
			short offset = FetchOffset(instruction);
			rt.Value = dMemory[offset].Value;
		}
		void Sw(string instruction)
		{
			Register rt = FetchRt(instruction);
			short offset = FetchOffset(instruction);
			dMemory[offset].Value = rt.Value;
		}
		void J(string instruction)
		{
			CurrentLine = FetchJumpOffset(instruction) - 1;
		}
		void Jal(string instruction)
		{
			Registers[7].Value = (short)(CurrentLine + 1);
			CurrentLine = FetchJumpOffset(instruction) - 1;
		}


		void Jr(string instruction)
		{
			Register rs = FetchRs(instruction);
			CurrentLine = rs.Value;
		}

	}
}