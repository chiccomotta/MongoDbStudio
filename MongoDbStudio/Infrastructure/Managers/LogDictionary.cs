using System.Collections.Generic;

namespace MongoDbStudio.Infrastructure.Managers
{
    public class LogDictionary
    {
        private readonly Dictionary<string, object> logs = new Dictionary<string, object>();

        public static LogDictionary New()
        {
            return new LogDictionary();
        }

        public LogDictionary AddParameter(string key, object value)
        {
            logs.Add(key, value);
            return this;
        }

        public Dictionary<string, object> Build()
        {
            return logs;
        }
    }
}