using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GBAE.OpCode;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Linq.Expressions;
using ExpressionToCodeLib;
using ExpressionBuilder;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using System.Threading;
using SDILReader;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Reflection.Emit;
using SR = System.Reflection;
using System.Reflection;

namespace GBAE
{
    public static class Emulator
    {
        public static State State;

        static Emulator()
        {
            State = new State();
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

        public static void LoadRom(String pFileName)
        {
            Console.WriteLine(pFileName);
            State.Rom.LoadROM(System.IO.File.ReadAllBytes(pFileName));
            State.Rom.ParseROM();
            State.Rom.DumpInfo();
            State.Rom.VerifyChecksum();
            Log(State.Rom.VerifyMagic().ToString());
            //int i = Rom.FindPattern(Encoding.ASCII.GetBytes("FLASH_V"));
            Rom.BackupType t = State.Rom.CheckBackup();
            Log("BACKUP: {0:X}",State.Rom.BackupToString(t));
            Registers r = new Registers();
            B test = new B(0xea00002e);
            //B test2 = new B(0x0a82843d);
            //Int24 tt = 25;
            //tt += 2;
            //AND test = new AND(0x00004682);
            //int i = test.CorrectedOffset.Value;
            //Console.WriteLine(tt);
            //Console.WriteLine("{0:X8}",test.RawOffset);

            //Console.WriteLine("andeq r4, r0, r2, lsl #13");
            //Console.WriteLine(test);
            //  ec54:       0000e079        andeq   lr, r0, r9, ror r0
            //AND test2 = new AND(0x0000e079);
            //Console.WriteLine("andeq lr, r0, r9, ror r0");
            //Console.WriteLine(test2);
            //Console.WriteLine(JsonConvert.SerializeObject(test2));
            //  020100b0        andeq   r0, r1, #176    ; 0xb0
            //AND test3 = new AND(0x00001432);
            //Console.WriteLine(test3);
            //Expression<Func<int,int>> teste = e => e + e;
            //Console.WriteLine(ExpressionToCode.ToCode(test.ToExpression()));
            //Console.WriteLine("{0:X}",test.ToExpression().Compile().Invoke());
            Int24 b = 0b110101111111111111111111;
            Console.WriteLine(b.ToBinary());
            b.RoR(4);  //b.RoL(4);
            Console.WriteLine(b.ToBinary());
            State.Registers.DumpRegs();
            State.Registers[0] = 1;
            var param = Expression.Parameter(typeof(State), "curState");
            var aa = Expression.Field(param, "Registers");
            //var ab = Expression.ArrayAccess(aa, Expression.Constant(0));
            var ab = Expression.Call(aa, typeof(Registers).GetMethod("get_Item"), Expression.Constant((Byte)0,typeof(Byte)));
            Expression bc = Expression.Call(typeof(Console).GetMethod("WriteLine",new Type[] { typeof(UInt32) }), ab);
            Console.WriteLine(ExpressionToCode.ToCode(bc));
            var vv = Expression.Lambda<Action<State>>(bc, param);
            Action<State> vvv = vv.Compile();
            //var ii = xyz.GetILAsByteArray();
            //ii.GetMethodBody();
            vvv.Invoke(State);
            //byte[] smh = xxx.GetILAsByteArray();
            //Console.WriteLine(xxx);
            Globals.LoadOpCodes();
            //MethodBodyReader m = new MethodBodyReader(dynMethod);
            //ICSharpCode.Decompiler.TypeSystem;
            Console.WriteLine(vvv.GetDynamicMethod().GetILAsReadableString());
            var v = typeof(Registers).GetMethods();
            //Console.WriteLine(JsonConvert.SerializeObject(v,Formatting.Indented));
            

           
        }
    }
}
