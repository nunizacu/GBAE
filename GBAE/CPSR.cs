using System;
using System.Linq;

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
