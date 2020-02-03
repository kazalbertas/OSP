using System;
using System.Collections.Generic;
using System.Text;

namespace CoreOSP.Exceptions
{
    public class OperatorMismatchException : Exception
    {
        public OperatorMismatchException()
        {
        }

        public OperatorMismatchException(string message) : base(message)
        {
        }

        public OperatorMismatchException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
