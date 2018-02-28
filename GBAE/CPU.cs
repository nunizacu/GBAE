using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace GBAE
{
    class CPU
    {
        private delegate void OPCodeHandler();
        private OPCodeHandler[] OPCodeHandlers;
        public enum Condition : Byte { EQ, NE, CSHS, CCLO, MI, PL, VS, VC, HI, LS, GE, LT, GT, LE, AL, NV }
        public CPU() { }


        private void B()
        {
            Emulator.Log("B");
        }

        public void DecodeARM(UInt32 op)
        {
            // bits 27-31
            byte Cond = (byte)((op & 0xFF000000) >> 31);

            // bits 20-27
            byte High = (byte)((op & 0xFF00000) >>20);
            // bits 4-7
            byte Low = (byte)((op & 0x1E000000) >> 3);
            // bits 0-23
            Int32 Offset = (Int32)(op & 0x00FFFFFF);

            Offset = ((Offset & 0x800000) > 0) ?  (int)(Offset | 0xFF000000): (int)(Offset & 0x00FFFFFF);
            Offset = Offset * 4 + 8;
            Emulator.Log("OP[{0:X8}] HIGH[{1:X}] LOW[{2:X}] COND[{3:X}] OFFSET[{4:X}]", op, High, Low, Cond, Offset);
            //OPCodeHandlers = new OPCodeHandler[255];
            //OPCodeHandlers[0] = this.B;
            //OPCodeHandlers[0]();
        }

        public void DecodeCondition(byte condition)
        {
            Emulator.Log(((Condition)condition).ToString());
        }


    }
}
