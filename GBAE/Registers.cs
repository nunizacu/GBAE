using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections;

namespace GBAE
{
    public class CPSR
    {
        private UInt32 _data;
        public enum Flag : UInt32
        {
            Sign = 0x80000000, Zero = 0x40000000, Carry = 0x20000000, Overflow = 0x10000000,
            StickyOverflow = 0x8000000, IRQDisable = 0x80, FIQDisable = 0x40, State = 0x20,
            Mode4 = 0x10, Mode3 = 0x8, Mode2 = 0x4, Mode1 = 0x2, Mode0 = 0x1
        };

        public CPSR()
        {
            _data = 0;
        }

        public bool this[Flag f]
        {
            get
            {
                return (_data & (UInt32)f)>0;
            }

            set
            {
                _data = value ? _data |= (UInt32)f: _data &= ~(UInt32)f;
            }
        }

        public void Dump()
        {
            
            foreach (Tuple<String,UInt32> flag in Enum.GetNames(typeof(Flag)).Zip((UInt32[])(Enum.GetValues(typeof(Flag))), Tuple.Create))
            {
                Emulator.Log("{0}:{1}", flag.Item1, this[(Flag)flag.Item2]);
            }
        }
    }

    class Registers
    {

        private UInt32 _CPSR;
        private UInt32 _SPSR_FIQ, _SPSR_SVC, _SPSR_ABT, _SPSR_IRQ, _SPSR_UND;
        private GCHandle[] _R, _R_FIQ, _R_SVC, _R_ABT, _R_IRQ, _R_UND;
        private unsafe UInt32*[] _MappedR;

        public unsafe Registers()
        {
            //AllocUserRegisters();
            //_R = new GCHandle[16];
            //_R_FIQ = new GCHandle[8];
            _MappedR = new UInt32*[16];
            AllocRegisters(ref _R, 16, 0);
            AllocRegisters(ref _R_FIQ, 8, 1);
            AllocRegisters(ref _R_SVC, 2, 2);
            AllocRegisters(ref _R_ABT, 2, 3);
            AllocRegisters(ref _R_IRQ, 2, 4);
            AllocRegisters(ref _R_UND, 2, 5);
            CreateMap();
            //DumpRegs();
            //SwitchFIQ();
            //SwitchIRQ();
            //DumpRegs();
        }

        private void AllocRegister(ref GCHandle pReg, int val=0)
        {
            pReg = GCHandle.Alloc(val, GCHandleType.Pinned);
        }

        private void AllocRegisters(ref GCHandle[] pReg, int count, int val=0)
        {
            pReg = new GCHandle[count];
            for(int i = 0; i<count; i++)
            {
                AllocRegister(ref pReg[i], val);
            }
        }
            
        private unsafe void CreateMap()
        {
            for(int i = 0; i<16; i++)
            {
                _MappedR[i] = (UInt32*)_R[i].AddrOfPinnedObject();
            }
        }

        public unsafe void DumpRegs()
        {
            for(int i = 0; i<16; i++)
            {
                Emulator.LogNoNL("R{0}[{1}] ",i,*_MappedR[i]);
            }
            Emulator.LogNoNL(Environment.NewLine);
        }

        public void SwitchUSR()
        {
            Switch(ref _R, 0, 16);
        }

        public void SwitchFIQ()
        {
            Switch(ref _R_FIQ, 8, 8);
        }

        public void SwitchSVC()
        {
            SwitchWithReset(ref _R_SVC, 13, 2);
        }

        public void SwitchABT()
        {
            SwitchWithReset(ref _R_ABT, 13, 2);
        }

        public void SwitchIRQ()
        {
            SwitchWithReset(ref _R_IRQ, 13, 2);
        }

        public void SwitchUND()
        {
            SwitchWithReset(ref _R_UND, 13, 2);
        }

        private unsafe void SwitchWithReset(ref GCHandle[] target, int offset, int count)
        {
            Switch(ref _R, 8, 8);
            Switch(ref target, offset, count);
        }

        private unsafe void Switch(ref GCHandle[] target, int offset, int count)
        {
            for(int i = 0; i<count;i++,offset++)
            {
                _MappedR[offset] = (UInt32*)target[i].AddrOfPinnedObject();
            }
        }

        public unsafe UInt32 this[byte R]
        {
            get { return *_MappedR[R]; }
            set { *_MappedR[R] = value;}
        }

    }
}
