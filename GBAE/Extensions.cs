using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Mono.Cecil;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Newtonsoft.Json;
using SDILReader;

namespace GBAE
{
    public static class Extensions
    {
        public static bool IsBitSet(this UInt32 x, int offset)
        {
            return (x & (1 << offset)) > 0;
        }

        public static UInt32 GetBitRange(this UInt32 x, int start, int end)
        {
            return (x & (0xFFFFFFFF >> (32 - (end - start + 1))) << start) >> start;
        }

        public static Int24 ToInt24(UInt32 i)
        {
            return new Int24(i);
        }

        public static string ToBinary(this byte b)
        {
            return Convert.ToString(b, 2);
        }

        public static string ToBinary(this Int32 i)
        {
            return Convert.ToString(i, 2);
        }

        public static string ToBinary(this Int24 i)
        {
            return Convert.ToString(i, 2);
        }

        public static void RoL (ref this Byte b, int count)
        {
            var mask = count & 0x07;
            b = (byte)((b << count) | (b >> (8 - count)));
        }

        public static void RoR(ref this Byte b, int count)
        {
            var mask = count & 0x07;
            b = (byte)((b >> count) | (b << (8 - count)));
        }

        public static void RoL(this Int24 i, int count)
        {
            var mask = count & (0x07 * 3);
            i.Value = (Int24)((i.Value << count) | (i.Value >> (24 - count)));
        }

        public static void RoR(this Int24 i, int count)
        {
            var mask = count & (0x07 * 3);
            i.Value = (Int24)((i.Value >> count) | (i.Value << (24 - count)));
        }

        public static String ObjDump(this Object o)
        {
            return JsonConvert.SerializeObject(o, Formatting.Indented);
        }

        public static DynamicMethod GetDynamicMethod (this Delegate action)
        {
            var mtype = action.Method.GetType();
            var fiOwner = mtype.GetField("m_owner", BindingFlags.Instance | BindingFlags.NonPublic);
            var dynMethod = fiOwner.GetValue(action.Method) as DynamicMethod;
            return dynMethod;
        }

        public static Byte[] GetILAsByteArray (this DynamicMethod dyn)
        {
            var ilgen = dyn.GetILGenerator();
            var fiBytes = ilgen.GetType().BaseType.GetField("m_ILStream", BindingFlags.Instance | BindingFlags.NonPublic);
            var fiLength = ilgen.GetType().BaseType.GetField("m_length", BindingFlags.Instance | BindingFlags.NonPublic);
            int cnt = (int)fiLength.GetValue(ilgen);
            Byte[] bytes = fiBytes.GetValue(ilgen) as Byte[];
            Array.Resize(ref bytes, cnt);

            return bytes;
        }

        public static String GetILAsReadableString(this DynamicMethod dyn)
        {
            return new MethodBodyReader(dyn).GetBodyCode();
        }
    }
}
