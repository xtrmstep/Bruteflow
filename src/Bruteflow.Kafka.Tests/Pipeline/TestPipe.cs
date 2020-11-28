using System;
using Bruteflow.Abstract;
using Bruteflow.Blocks;
using Newtonsoft.Json.Linq;

namespace Bruteflow.Kafka.Tests.Pipeline
{
    public class TestPipe : AbstractPipe<JObject>
    {
        private readonly TestRoutines _routines;

        public TestPipe(IServiceProvider serviceProvider, TestRoutines routines)
            : base(serviceProvider)
        {
            _routines = routines;

            Head
                .Process((tkn, data, meta) => _routines.AddProperty(tkn, data, meta))
                .Action((tkn, data, meta) => _routines.Send(tkn, data, meta));
        }
        
        
    }
}