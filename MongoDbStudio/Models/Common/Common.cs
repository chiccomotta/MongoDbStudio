using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MongoDbStudio.Models.Common
{
    public class BaseResponse
    {
        public string RequestStatus { get; set; }
        public Error Error { get; set; }
    }

    public class Error
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ErrorMessage
    {
        public string Field { get; set; }
        public string Message { get; set; }
    }
    public enum RequestStatus
    {
        KO = 0,
        OK = 1
    }

    public class ApiResponse
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public IEnumerable<ErrorMessage> Errors { get; set; }
    }

    public class UnhandledExceptionResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string InnerExceptionMessage { get; set; }
        public ExceptionInfo ExceptionInfo { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class ExceptionInfo
    {
        private ExceptionInfo()
        {
        }

        public string Type { get; set; }
        public IEnumerable<string> StackTrace { get; set; }

        public static ExceptionInfo GetStackTrace(Exception exception)
        {
            return new ExceptionInfo
            {
                Type = exception.GetType().FullName,
                StackTrace = exception.StackTrace
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.TrimStart())
            };
        }
    }
}
