using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBAE
{
    public class Rom
    {
        private Byte[] _Data;

        private UInt32 _EntryPoint;
        private Byte[] _OEMLogo;
        private String _GameTitle;
        private String _GameCode;
        private String _MakerCode;
        private Byte _MainUnitCode;
        private Byte _DeviceType;
        private Byte _SoftwareVersion;
        private Byte _ComplementCheck;
        public enum BackupType
        {
            EEPROM = 0, SRAM = 1, Flash = 2, Flash512 = 3, Flash1m = 4, None = -1
        }
        private String[] _BackupMap =
        {
            "EEPROM_V", "SRAM_V", "FLASH_V", "FLASH512_V", "FLASH1M_V", "NONE"
        };

        public Rom()
        { }

        public void LoadROM(byte[] pROM)
        {
            _Data = new Byte[pROM.Length];
            Array.Copy(pROM, _Data, pROM.Length);
        }

        public BackupType CheckBackup()
        {
            //string[] backupTypes = Enum.GetNames(Backup);
            for(int i = 0; i < _BackupMap.Length; i++)
            {
                if (-1 != FindPattern(Encoding.ASCII.GetBytes(_BackupMap[i])))
                    return (BackupType)i;
            }
            return (BackupType) (-1);
        }

        public string BackupToString(BackupType type)
        {
            return Enum.GetName(typeof(BackupType), type);
        }

        public void DumpInfo()
        {
            Emulator.Log("Size  : {0} bytes", _Data.Length);
            Emulator.Log("CHKSM : {0} {1}", _ComplementCheck, CalculateChecksum());
            Emulator.Log("Entry : {0:X}h", _EntryPoint);
            Emulator.Log("Title : {0}", _GameTitle);
            Emulator.Log("Code  : {0}", _GameCode);
            Emulator.Log("Maker : {0}", _MakerCode);
            Emulator.Log("MUC   : {0}", _MainUnitCode);
            Emulator.Log("Device: {0}", _DeviceType);
            Emulator.Log("SftVer: {0}", _SoftwareVersion);
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

        public int FindPattern(byte[] pattern)
        {
            for (int i = 0; i < _Data.Length; i++)
            {
                if (CompareRange(pattern,i,pattern.Length)) return i;
            }
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

        private bool CompareRange(byte[] pattern, int offset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (_Data[offset + i] != pattern[i]) return false;
            }
            return true;
        }
    }
}
