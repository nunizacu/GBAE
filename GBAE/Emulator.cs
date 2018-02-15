﻿using System;
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

        public void LoadRom()
        {
            ROM.LoadROM(System.IO.File.ReadAllBytes("Advanced Wars  # GBA.GBA"));
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
        }
    }
}
