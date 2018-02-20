using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBAE
{
    public static class ByteExtension
    {
        public static string ToBinary(this byte b)
        {
            return Convert.ToString(b, 2);
        }
    }

    class Emulator
    {
        ROM ROM;
        CPU CPU;
        public Emulator()
        {
            ROM = new ROM();
            CPU = new CPU();
        }

        [Conditional("DEBUG")]
        public static void Log(string msg, params object[] p)
        {
            Console.WriteLine(msg, p);
        }

        [Conditional("DEBUG")]
        public static void Log(string msg)
        {
            Console.WriteLine(msg);
        }

        [Conditional("DEBUG")]
        public static void LogNoNL(string msg)
        {
            Console.Write(msg);
        }

        [Conditional("DEBUG")]
        public static void LogNoNL(string msg, params object[] p)
        {
            Console.Write(msg, p);
        }

        public void LoadRom(String pFileName)
        {
            ROM.LoadROM(System.IO.File.ReadAllBytes(pFileName));
            ROM.ParseROM();
            ROM.DumpInfo();
            ROM.VerifyChecksum();
            Log(ROM.VerifyMagic().ToString());
            //int i = ROM.FindPattern(Encoding.ASCII.GetBytes("FLASH_V"));
            ROM.BackupType t = ROM.CheckBackup();
            Log("BACKUP: {0:X}",ROM.BackupToString(t));
            //CPU.DecodeARM(0x2e0000ea);
            //CPU.DecodeARM(0xea00002e);
            CPU.AddHandlers();
            Registers r = new Registers();
            //r.DumpRegs();
            //r[13] = 16;
            //r.DumpRegs();
            //r.SwitchABT();
            //r[13] = 32;
            //r.DumpRegs();
            //r.SwitchUSR();
            //r.DumpRegs();
            //r.SwitchABT();
            //r.DumpRegs();
            //CPSR c = new CPSR();
            //c[CPSR.Flag.Carry] = true;
            //c[CPSR.Flag.User] = true;
            //c.Dump();
            //CPU.DecodeCondition(0x2);
            CPU.DecodeARM(0x2e0000ea);
            CPU.DecodeARM(0xea00002e);
            CPU.DecodeARM(0x0a82843d);
            CPU.DecodeARM(0x3d84820a);
        }
    }
}
