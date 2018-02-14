using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBAE
{
    class ROM
    {
        private Byte[] _Data;

        private UInt32 _EntryPoint;
        private Byte[] _OEMLogo;
        private String _GameTitle;
        private String _GameCode;
        private String _MakerCode;
        private Byte _FixedValue; // 96h
        private Byte _MainUnitCode;
        private Byte _DeviceType;
        private Byte _SoftwareVersion;
        private Byte _ComplementCheck;

        public ROM()
        { }

        public void LoadROM(byte[] pROM)
        {
            _Data = new Byte[pROM.Length];
            Array.Copy(pROM, _Data, pROM.Length);
        }

        public void DumpInfo()
        {
            Console.WriteLine("Size  : {0} bytes", _Data.Length);
            Console.WriteLine("CHKSM : {0} {1}", _ComplementCheck, CalculateChecksum());
            Console.WriteLine("Entry : {0:X}h", _EntryPoint);
            Console.WriteLine("Title : {0}", _GameTitle);
            Console.WriteLine("Code  : {0}", _GameCode);
            Console.WriteLine("Maker : {0}", _MakerCode);
            Console.WriteLine("MUC   : {0}", _MainUnitCode);
            Console.WriteLine("Device: {0}", _DeviceType);
            Console.WriteLine("SftVer: {0}", _SoftwareVersion);
        }

        public void ParseROM()
        {
            _EntryPoint = (UInt16)((_Data[0] << 8) + _Data[1]);
            _GameTitle = ReadString(0xA0, 12);
            _GameCode = ReadString(0xAC, 4);
            _MakerCode = ReadString(0xB0, 2);
            _MainUnitCode = _Data[0xB3];
            _DeviceType = _Data[0xB4];
            _SoftwareVersion = _Data[0xBC];
            _ComplementCheck = _Data[0xBD];
        }

        public void VerifyChecksum()
        {
            if(CalculateChecksum()==_ComplementCheck)
            {
                Console.WriteLine("CHECKSUM OK");
            } else
            {
                Console.WriteLine("CHECKSUM MISMATCH");
            }
        }

        public bool VerifyMagic()
        {
            if (_Data[0xB2] != 0x96) return false;
            if (!CompareRange(0, 0xBE, 2)) return false;
            if (!CompareRange(0, 0xB5, 7)) return false;
            return true;
        }

        public int CheckFlashOldID()
        {
            return 0;
        }

        public int FindPattern(byte[] pattern)
        {
            Console.WriteLine("SEARCHING");
            for (int i = 0; i < _Data.Length; i++)
            {
                if (_Data.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
                {
                    Console.WriteLine("FOUND");
                    return i;
                    
                }
            }
            Console.WriteLine("NOT FOUND");
            return -1;

        }

        public byte CalculateChecksum()
        {
            byte Checksum = 0;
            for(int i = 0xA0; i<=0xBC;i++)
            {
                Checksum = (byte)(Checksum - _Data[i]);
            }
            Checksum = (byte)((Checksum - 0x19)& 0x0FF);
            return (byte)Checksum;
        }

        private string ReadString(int Offset, int Length)
        {
            StringBuilder sb = new StringBuilder();
            for (int i=0; i < Length; i++)
            {
                //Console.Write(_Data[Offset]);
                sb.Append(Convert.ToChar(_Data[Offset+i]), 1);
            }
            return sb.ToString();
        }

        private bool CompareRange(byte expectedValue, int offset, int length)
        {
            for(int i=0; i < length; i++)
            {
                if (_Data[offset + i] != expectedValue) return false;
            }
            return true;
        }
    }
}
