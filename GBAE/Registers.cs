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

    public class Registers
    {

        private CPSR _CPSR;
        private CPSR _SPSR_FIQ, _SPSR_SVC, _SPSR_ABT, _SPSR_IRQ, _SPSR_UND;
        private CPSR[] _SPSR;
        private GCHandle[] _R, _R_FIQ, _R_SVC, _R_ABT, _R_IRQ, _R_UND;
        private unsafe UInt32*[] _MappedR;
        public enum Mode { User=0, FIQ=2, Supervisor=3, Abort=4, IRQ=5, Undefined=6};

        public unsafe Registers()
        {
            _MappedR = new UInt32*[16];
            _CPSR = new CPSR();
            _SPSR = new CPSR[7];
            AllocRegisters(ref _R, 16, 0);
            AllocRegisters(ref _R_FIQ, 8, 1);
            AllocRegisters(ref _R_SVC, 2, 2);
            AllocRegisters(ref _R_ABT, 2, 3);
            AllocRegisters(ref _R_IRQ, 2, 4);
            AllocRegisters(ref _R_UND, 2, 5);
            CreateMap();
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

        public void SaveCPSR(Mode m)
        {
            if(m!=Mode.User)
                _SPSR[(int)m] = (CPSR) _CPSR.Clone();
        }

        public void RestoreCPSR(Mode m)
        {
            _CPSR = (CPSR)_SPSR[(int)m].Clone();
        }
/*
        public void SwitchMode(Mode m)
        {
            switch (m)
            {
                case Mode.FIQ:

                    break;
                


            }
        }
*/

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
            Switch(ref _R, 8, 7);
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

        public unsafe UInt32 SP
        {
            get { return *_MappedR[13]; }
            set { *_MappedR[13] = value;}
        }

        public unsafe UInt32 LR
        {
            get { return *_MappedR[14]; }
            set { *_MappedR[14] = value;}
        }

        public unsafe UInt32 PC
        {
            get { return *_MappedR[15]; }
            set { *_MappedR[15] = value;}
        }
    }
}
