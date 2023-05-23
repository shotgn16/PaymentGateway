using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Logger
{
    internal interface ILogger
    {
        void Debug(string message, object arg = null);
        void Info(string message, object arg = null);
        void Warning(string message, object arg = null);
        void Error(string message, object arg = null);
    }
}
