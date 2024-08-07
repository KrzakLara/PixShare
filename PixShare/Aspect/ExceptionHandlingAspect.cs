using PostSharp.Aspects;
using PostSharp.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PixShare.Aspect
{
    [PSerializable]
    public class ExceptionHandlingAspect : OnExceptionAspect
    {
        public override void OnException(MethodExecutionArgs args)
        {
            Console.WriteLine($"Exception in {args.Method.Name}: {args.Exception.Message}");
        }
    }
}