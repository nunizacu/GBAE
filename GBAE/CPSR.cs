using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBAE
{
    public class CPSR : ICloneable
    {
        private UInt32 _data;
        public enum Flag : UInt32
        {
            Sign = 0x80000000, Zero = 0x40000000, Carry = 0x20000000, Overflow = 0x10000000,
            StickyOverflow = 0x8000000, IRQDisable = 0x80, FIQDisable = 0x40, State = 0x20,
            //Mode4 = 0x10, Mode3 = 0x8, Mode2 = 0x4, Mode1 = 0x2, Mode0 = 0x1,
            User = 0x10, FIQ = 0x11, IRQ = 0x12, Supervisor = 0x13, Abort = 0x17, Undefined = 0x1B, System = 0x1F
        };
        /*
         * The Mode Bits M4-M0 contain the current operating mode.
         * Binary Hex Dec  Expl.
         * 0xx00b 00h 0  - Old User       ;\26bit Backward Compatibility modes
         * 0xx01b 01h 1  - Old FIQ        ; (supported only on ARMv3, except ARMv3G,
         * 0xx10b 02h 2  - Old IRQ        ; and on some non-T variants of ARMv4)
         * 0xx11b 03h 3  - Old Supervisor ;/
         * 10000b 10h 16 - User (non-privileged)
         * 10001b 11h 17 - FIQ
         * 10010b 12h 18 - IRQ
         * 10011b 13h 19 - Supervisor (SWI)
         * 10111b 17h 23 - Abort
         * 11011b 1Bh 27 - Undefined
         * 11111b 1Fh 31 - System (privileged 'User' mode) (ARMv4 and up 
         
        public enum Mode : UInt32
        {
            User = 0x00000010, FIQ = 0x00000011, IRQ = 0x00000012, Supervisor = 0x00000013,
            Abort = 0x00000017, Undefined = 0x0000001B, System = 0x000001F
        }
        */
        public CPSR()
        {
            _data = 0;
        }

        private CPSR(UInt32 data)
        {
            _data = data;
        }

        public object Clone()
        {
            return new CPSR(_data);
        }

        public bool this[Flag f]
        {
            get
            {
                return ((_data & (UInt32)f) == (UInt32)f);
            }

            set
            {
                _data = value ? _data |= (UInt32)f : _data &= ~(UInt32)f;
            }
        }

        public UInt32 ToUInt32()
        {
            return _data;
        }

        public void Dump()
        {
            Emulator.Log("RAW: {0}", _data);
            foreach (Tuple<String, UInt32> flag in Enum.GetNames(typeof(Flag)).Zip((UInt32[])(Enum.GetValues(typeof(Flag))), Tuple.Create))
            {
                Emulator.Log("{0}:{1}", flag.Item1, this[(Flag)flag.Item2]);
            }
        }
    }

}
