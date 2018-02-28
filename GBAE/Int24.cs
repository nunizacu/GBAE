using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBAE
{
    public class Int24
    {
        public Int32 Value;

        public Int24(UInt32 from)
        {
         //   Int32 Offset = (Int32)(o& 0x00FFFFFF);

       // Offset = ((Offset & 0x800000) > 0) ? (int)(Offset| 0xFF000000) : (int)(Offset & 0x00FFFFFF);
            Int32 Tmp = (Int32)from & 0x00FFFFFF;
            Value = ((Tmp & 0x800000) > 0) ? (Int32)(Tmp | 0xFF000000) : (Int32)(Tmp &  0x00FFFFFF);
        }

        public Int24(Int32 from)
        {
            Value = from & 0x00FFFFFF;
        }

        public static implicit operator Int32(Int24 i)
        {
            //Console.WriteLine(i.Value);
            return (Int32)i.Value;
            //return 0;
        }

        public static Int24 operator << (Int24 i, int count)
        {
            //i.Value = i.Value << count;
            //i.Value &= 0x00FFFFFF;
            return new Int24(i.Value << count);
        }

        public static Int32 operator * (Int24 a, Int32 b)
        {
            return a.Value * b;
        }


        public static Int24 operator + (Int24 a, Int32 b)
        {
            return a.Value + b;
        }

        public static implicit operator Int24(UInt32 i)
        {
            return new Int24(i);
        }

        public static implicit operator Int24(Int32 i)
        {
            Console.WriteLine("Hello");
            return new Int24(i);
        }

        public override String ToString()
        {
            return String.Format("{0:X8}",Value);
        }

    }
}
