using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace GBAE
{
    public class OPCode : System.Attribute
    {
        public string Name;
        public byte High;
        public byte Low;

        public byte HighStart;
        public byte HighEnd;

        public enum Types { High, HighRanged, HighAndLow }
        public Types Type;
        public OPCode(string pName)
        {
            Type = Types.High;
            Name = pName;
            //High = pHigh;
            //Low = pLow;
        }
        public OPCode(string pName, byte pHighStart, byte pHighEnd)
        {
            Type = Types.HighRanged;
            Name = pName;
            HighStart = pHighStart;
            HighEnd = pHighEnd;
        }
        public OPCode(string pName, byte High, sbyte Low)
        {
            Type =  Types.HighRanged;
        }
    }
    /*  Code Suffix Flags         Meaning
  0:   EQ     Z=1           equal (zero) (same)
  1:   NE     Z=0           not equal (nonzero) (not same)
  2:   CS/HS  C=1           unsigned higher or same (carry set)
  3:   CC/LO  C=0           unsigned lower (carry cleared)
  4:   MI     N=1           negative (minus)
  5:   PL     N=0           positive or zero (plus)
  6:   VS     V=1           overflow (V set)
  7:   VC     V=0           no overflow (V cleared)
  8:   HI     C=1 and Z=0   unsigned higher
  9:   LS     C=0 or Z=1    unsigned lower or same
  A:   GE     N=V           greater or equal
  B:   LT     N<>V          less than
  C:   GT     Z=0 and N=V   greater than
  D:   LE     Z=1 or N<>V   less or equal
  E:   AL     -             always (the "AL" suffix can be omitted)
  F:   NV     -             never (ARMv1,v2 only) (Reserved ARMv3 and up)*/

    class CPU
    {
        private delegate void OPCodeHandler();
        private OPCodeHandler[] OPCodeHandlers;
        public enum Condition : Byte { EQ, NE, CSHS, CCLO, MI, PL, VS, VC, HI, LS, GE, LT, GT, LE, AL, NV }
        public CPU() { }

        [OPCode("B", High=0xa0)]
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
            Offset = Offset & ~0x800000;
            Offset *= 4;
            Emulator.Log("OP[{0:X}] HIGH[{1:X}] LOW[{2:X}] COND[{3:X}] OFFSET[{4:X}]", op, High, Low, Cond, Offset);
            OPCodeHandlers = new OPCodeHandler[255];
            OPCodeHandlers[0] = this.B;
            OPCodeHandlers[0]();
        }

        public void DecodeCondition(byte condition)
        {
            Emulator.Log(((Condition)condition).ToString());
        }

        public void AddHandlers()
        {
            IEnumerable<MethodInfo> candidates = typeof(GBAE.CPU).GetRuntimeMethods();
            foreach(MethodInfo candidate in candidates)
            {
                foreach(Attribute attrib in candidate.GetCustomAttributes())
                {
                    if (attrib.GetType() == typeof(OPCode))
                    {
                        OPCode a = (OPCode)attrib;
                        //Emulator.Log("!");
                        Emulator.Log("{0} {1} {2:X}", candidate.Name, a.Name, a.High);


                    }
                }

            }
        }
    }
}

//_Mapped = stackalloc 
//_Mapped = stackalloc int[6];
//_Mapped = (_Mapped*)new UInt32[16];
//fixed (_Mapped[0)
//{
//    _Mapped[0] = 
//}
//fixed (int* a = new int()) {
//_Mapped[0] = _R1; //}
//fixed{
//   _Mapped *[0] = &_R0;
//}
/*
GCHandle a = GCHandle.Alloc(5, GCHandleType.Pinned);
GCHandle b = a;
int* pa = (int*)a.AddrOfPinnedObject();
int* pb = (int*)b.AddrOfPinnedObject();

Emulator.Log("A {0} {1}", *pa, *pb);
*pb  = 6;
Emulator.Log("B {0} {1}", *pa, *pb);
_R[0] = GCHandle.Alloc(0, GCHandleType.Pinned);
_R[13] = GCHandle.Alloc(0, GCHandleType.Pinned); 
_R = new int*[16];
GCHandle _R13 = GCHandle.Alloc(0, GCHandleType.Pinned);
GCHandle _R13_PIQ = GCHandle.Alloc(5, GCHandleType.Pinned);
_R[13] = (int*)_R13.AddrOfPinnedObject();
Emulator.Log("{0}",*_R[13]);
_R[13] = (int*)_R13_PIQ.AddrOfPinnedObject();
Emulator.Log("{0}", *_R[13]); */
