using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
/*
namespace GBAE
{
    class Interpreter
    {
        //public static Expression OpCodeToExpression(OpCode o, Int32 PC)
        //{

        //}
        public delegate void OpCodeHandler(OpCode.OpCode op, Int32 );
        private Dictionary<Type, OpCodeHandler> HandlersMap;

        public void SendToHandler(OpCode.OpCode op)
        {
            HandlersMap[op.GetType()].Invoke();
        }
        public void AddHandlers()
        {
            IEnumerable<MethodInfo> candidates = typeof(GBAE.CPU).GetRuntimeMethods();
            foreach (MethodInfo candidate in candidates)
            {
                foreach (Attribute attrib in candidate.GetCustomAttributes())
                {
                    if (attrib.GetType() == typeof(HandlerFor))
                    {
                        OPCode a = (OPCode)attrib;
                        //Emulator.Log("!");
                        Emulator.Log("{0} {1} {2:X}", candidate.Name, a.Name, a.High);


                    }
                }

            }
        }

        private void ADD ()
        {

        }
    }
}
*/