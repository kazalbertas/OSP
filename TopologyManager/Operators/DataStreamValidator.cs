using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopologyManagerOSP.Operators
{
    static internal class DataStreamValidator
    {
        public static bool ValidateType<T>(Type t) 
        {
            var validationResult = t.GetInterfaces().Contains(typeof(T)) ? true : false;
            return validationResult;
        }
    }
}
