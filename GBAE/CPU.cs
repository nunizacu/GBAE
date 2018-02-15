using System;
using System.Reflection;
//using System.Reflection
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBAE
{
    public class OPCode:System.Attribute
    {
        public string Name;
        public byte High;
        public byte Low;
        public OPCode() { }
    }

    class CPU
    {
        private delegate void OPCodeHandler();
        private OPCodeHandler[] OPCodeHandlers;
        public CPU() { }

        [OPCode(Name="B", High=0xa0)]
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
