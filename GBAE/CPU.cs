using System;
using System.Reflection;
//using System.Reflection
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
        public OPCode(string pName, byte pHigh)
        {
            Type = Types.High;
            Name = pName;
            High = pHigh;
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


    class CPU
    {
        private delegate void OPCodeHandler();
        private OPCodeHandler[] OPCodeHandlers;
        public CPU() { }

        //[OPCode(Name="B", High=0xa0)]
        private void B()
        {
            Emulator.Log("B");
        }

        public void DecodeARM(UInt32 op)
        {
            // bits 20-27
            byte a = (byte)((op & 0xFF00000) >>20);
            // bits 4-7
            byte b = (byte)((op & 0x1E000000) >> 3);
            Emulator.Log("OP[{0:X}] BIN[{1}] HEX[{2:X}]", op, a.ToBinary(), a);
            OPCodeHandlers = new OPCodeHandler[255];
            OPCodeHandlers[0] = this.B;
            OPCodeHandlers[0]();
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
