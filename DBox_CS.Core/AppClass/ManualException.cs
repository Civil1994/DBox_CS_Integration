using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBox_CS.Core.AppClass
{
    public class ManualException : Exception
    {
        public string InnerErrorDetails { get; }
        public ManualException() { }

        public ManualException(string message) : base(message) { }

        public ManualException(string message, Exception inner) : base(message, inner) { }

        public ManualException(string message, string innerErrorDetails)
        : base(message)
        {
            InnerErrorDetails = innerErrorDetails;  
        }
    }
}
