using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBAE
{
    class Emulator
    {
        ROM ROM;
        public Emulator()
        {
            ROM = new ROM();
        }

        public void LoadRom()
        {
            ROM.LoadROM(System.IO.File.ReadAllBytes("Advanced Wars  # GBA.GBA"));
            ROM.ParseROM();
            ROM.DumpInfo();
            ROM.VerifyChecksum();
            Console.WriteLine(ROM.VerifyMagic());
            int i = ROM.FindPattern(Encoding.ASCII.GetBytes("FLASH_V"));
            Console.WriteLine("A{0}",i.ToString());
        }
    }
}
