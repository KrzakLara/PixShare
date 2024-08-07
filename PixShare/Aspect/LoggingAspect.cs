using PostSharp.Aspects;
using PostSharp.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixShare.Aspect
{
    [PSerializable]
    public class LoggingAspect : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            Console.WriteLine($"Entering {args.Method.Name}");
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            Console.WriteLine($"Exiting {args.Method.Name}");
        }
    }

}