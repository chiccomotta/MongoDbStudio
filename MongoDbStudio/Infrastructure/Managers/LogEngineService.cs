using MongoDbStudio.Interfaces;
using NLogEngine;
using System;
using System.Collections.Generic;

namespace MongoDbStudio.Infrastructure.Managers
{
    public class LogEngineService : ILogEngineService
    {
        public bool WriteLog(LogLevels logLevels, string message, string correlation, Dictionary<string, object> exceptionParams = null)
        {
            try
            {
                LogEngine.SendLog(logLevels, correlation, message, false, exceptionParams);
            }
            catch //error on logengine must be not send application in error
            {
                return false;
            }
            return true;
        }

        public bool WriteException(Exception ex, string correlation, bool writeErrorLog = false, string message = null, Dictionary<string, object> exceptionParams = null)
        {

            try
            {
                LogEngine.SendException(ex, correlation);

                if (writeErrorLog)
                    WriteLog(LogLevels.ERROR, string.IsNullOrWhiteSpace(message) ? ex.ToString() : $"{message}\n{ex}", correlation, exceptionParams);
            }
            catch //error on logengine must be not send application in error
            {
                return false;
            }
            return true;

        }
    }
}
