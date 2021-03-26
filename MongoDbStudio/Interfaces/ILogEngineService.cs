using NLogEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDbStudio.Interfaces
{
    public interface ILogEngineService
    {
        bool WriteLog(LogLevels logLevels, string message, string correlation, Dictionary<string, object> exceptionParams = null);
        bool WriteException(Exception ex, string correlation, bool writeErrorLog = false, string message = null, Dictionary<string, object> exceptionParams = null);

    }
}
