using System.Collections.Generic;

namespace Libraries.Utilities
{
    public class GenericResult
    {
        public GenericResult()
        {
        }

        public GenericResult(bool success)
        {
            Success = success;
        }

        public GenericResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public GenericResult(bool success, object data)
        {
            Success = success;
            Data = data;
        }
        public GenericResult(bool success,string message, object data)
        {
            Message = message;
            Success = success;
            Data = data;
        }
        public GenericResult(bool success, List<object> error)
        {
            Success = success;
            Error = error;
        }

        public object Data { get; set; }

        public bool Success { get; set; }

        public string Message { get; set; }

        public List<object> Error { get; set; }
    }
    public class GenericResult<T> where T:class
    {
        public GenericResult()
        {
        }

        public GenericResult(bool success)
        {
            Success = success;
        }

        public GenericResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public GenericResult(bool success, T data)
        {
            Success = success;
            Data = data;
        }
        public GenericResult(bool success, List<T> listData)
        {
            Success = success;
            ListData = listData;
        }
        public GenericResult(bool success, string message, T data)
        {
            Message = message;
            Success = success;
            Data = data;
        }
        public GenericResult(bool success, string message, List<T> listData)
        {
            Message = message;
            Success = success;
            ListData = listData;
        }
        public GenericResult(bool success, List<object> error)
        {
            Success = success;
            Error = error;
        }

        public T Data { get; set; }
        public List<T> ListData { get; set; }

        public bool Success { get; set; }

        public string Message { get; set; }

        public List<object> Error { get; set; }
    }
}
