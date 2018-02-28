using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * General Internal Memory
  00000000-00003FFF   BIOS - System Rom         (16 KBytes)
  00004000-01FFFFFF   Not used
  02000000-0203FFFF   WRAM - On-board Work RAM  (256 KBytes) 2 Wait
  02040000-02FFFFFF   Not used
  03000000-03007FFF   WRAM - On-chip Work RAM   (32 KBytes)
  03008000-03FFFFFF   Not used
  04000000-040003FE   I/O Registers
  04000400-04FFFFFF   Not used
Internal Display Memory
  05000000-050003FF   BG/OBJ Palette RAM        (1 Kbyte)
  05000400-05FFFFFF   Not used
  06000000-06017FFF   VRAM - Video RAM          (96 KBytes)
  06018000-06FFFFFF   Not used
  07000000-070003FF   OAM - OBJ Attributes      (1 Kbyte)
  07000400-07FFFFFF   Not used
External Memory (Game Pak)
  08000000-09FFFFFF   Game Pak Rom/FlashROM (max 32MB) - Wait State 0
  0A000000-0BFFFFFF   Game Pak Rom/FlashROM (max 32MB) - Wait State 1
  0C000000-0DFFFFFF   Game Pak Rom/FlashROM (max 32MB) - Wait State 2
  0E000000-0E00FFFF   Game Pak SRAM    (max 64 KBytes) - 8bit Bus width
  0E010000-0FFFFFFF   Not used
Unused Memory Area
  10000000-FFFFFFFF   Not used (upper 4bits of address bus unused)
  */

namespace GBAE
{
    class UnusedMemoryException : Exception
    {

    }

    class Memory
    {
        private byte[] _WRAM1;
        private byte[] _WRAM2;
        private byte[] _Palette;
        private byte[] _VRAM;
        private byte[] _OAM;
        public Memory()
        {
            _WRAM1  = new byte[1024 * 256];
            _WRAM2  = new byte[1024 * 32];
            _VRAM   = new byte[1024 * 96];
            _Palette= new byte[1024];
            _OAM    = new byte[1024];
        }

        public byte this[byte address]
        {
            get
            {
                return 0;
            }
        }

        public Byte Read8(int address)
        {
            return 0;
        }

        public UInt16 Read16(int address)
        {
            return 0;
        }

        public UInt32 Read32(int address)
        {
            return 0;
        }
    }
}
