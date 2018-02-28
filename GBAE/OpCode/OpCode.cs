using System;
using GBAE;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using ExpressionBuilder;

namespace GBAE.OpCode
{
    public enum Conditions : Byte { EQ, NE, CSHS, CCLO, MI, PL, VS, VC, HI, LS, GE, LT, GT, LE, AL, NV };
    public enum ShiftTypes : Byte { LSL, LSR, ASR, ROR};

    public class RawOpcode
    {
        UInt32 _OpCode;

        public RawOpcode(UInt32 opcode)
        {
            _OpCode = opcode;
        }

        public Conditions Condition
        {
            get
            {
                return (Conditions)_OpCode.GetBitRange(28, 31);
            }
        }

        public bool IsBitSet(int offset)
        {
            return true;
        }
    }



    public abstract class OpCode
    {
        public Conditions Condition { get; private set; }
        public OpCode(UInt32 opcode)
        {
            Condition = (Conditions)opcode.GetBitRange(28, 31);
        }

        public String Name
        {
            get
            {
                return this.GetType().Name + Condition;
            }
        }

        public override string ToString()
        {
            return this.GetType().Name + Condition;
        }
    }

    public abstract class Arm7OpCode : OpCode {
        public Arm7OpCode(UInt32 opcode) : base(opcode) { }
    }
    public abstract class ThumbOpCode : OpCode {
        public ThumbOpCode(UInt32 opcode) : base(opcode) { }
    }


    public abstract class ALUOpCode : Arm7OpCode
    { 
        public Conditions Condition { get; private set; }
        public ShiftTypes ShiftType { get; private set; }
        public bool ImmediateSecond { get; private set; }
        public bool SetConditionCodes { get; private set; }
        public bool ShiftByRegister { get; private set; }
        public Byte RORShift { get; private set; }
        public Byte ShiftAmount { get; private set; }
        public Byte ShiftRegister { get; private set; }
        public Byte Rn { get; private set; }
        public Byte Rd { get; private set; }
        public Byte Rm { get; private set; }
        public Byte Op2 { get; private set; }

        public ALUOpCode(UInt32 opcode) : base(opcode)
        {
            ImmediateSecond = opcode.IsBitSet(25);
            ShiftByRegister = opcode.IsBitSet(4);
            SetConditionCodes = opcode.IsBitSet(19);
            Rn = (byte)opcode.GetBitRange(16, 19);
            Rd = (byte)opcode.GetBitRange(12, 15);
            Rm = (byte)opcode.GetBitRange(0, 3);
            ShiftAmount = (byte)opcode.GetBitRange(7, 11);
            ShiftRegister = (byte)opcode.GetBitRange(8, 11);
            ShiftType = (ShiftTypes)opcode.GetBitRange(5, 6);
            Op2 = (byte)opcode.GetBitRange(0, 7);
        }


        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append(this.Name);
            s.Append(" R" + Rd + ", R" + Rn+", ");
            if(ImmediateSecond)
            {
                s.Append("#"+Op2);
            } else
            {
                s.Append("R" + Rm);
                s.Append(", " + ShiftType + " ");
                if (ShiftByRegister)
                {
                    s.Append("R" + ShiftRegister);
                } else
                {
                    s.Append("#"+ShiftAmount);
                }
            }

            return s.ToString();
        }
    }

    public class AND : ALUOpCode
    {
        public AND(UInt32 opcode) : base(opcode) { }
    }

    public class MOV : ALUOpCode
    {
        public MOV(UInt32 opcode) : base(opcode) { }
    }

    public class B : Arm7OpCode
    {
        public Int24 RawOffset { get; private set; }
        public Int32 CorrectedOffset { get; private set; }
        //public Expression Expression { get; private set; }
        public B(UInt32 opcode) : base(opcode)
        {
            RawOffset = opcode;
            CorrectedOffset = RawOffset * 4 + 8;
        }

        public Expression<Func<UInt32>> ToExpression()
        {
            Expression<Func<UInt32>> ex = () => (UInt32)(Emulator.State.Registers[15]+CorrectedOffset); 
            //var ex = Function.Create().WithParameter<UInt32>("PC").WithParameter<Int32>("Offset")
            //    .WithBody(CodeLine.Assign(Operation.Variable("PC"),Operation.Variable("Offset"),ExpressionBuilder.Enums.AssignementOperator.SumAssign));
            return ex;
        }

    }

}
