using System.Collections.Generic;
using System.Net;

namespace SharedKernel
{
    public class OperationResult
    {
        public OperationResult(bool success)
        {
            Success = success;
            MessageList = new List<string>();
        }

        public bool Success { get; set; }

        public List<string> MessageList { get; private set; }

        public HttpStatusCode StatusCode { get; private set; }

        public void AddMessage(string message, HttpStatusCode statusCode)
        {
            MessageList.Add(message);
            StatusCode = statusCode;
        }

        public static implicit operator OperationResult(bool success)
        {
            return new OperationResult(success);
        }
    }
}
