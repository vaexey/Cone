using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Flow
{

    public class Result
    {
        public static Result OK
        {
            get { return new Result(true); }
        }

        public static Result<T> VAL<T>(T value)
        {
            return new Result<T>(value);
        }

        public static Result Error(Exception ex)
        {
            return new Result(ex);
        }
        public static Result<T> Error<T>(Exception ex)
        {
            return new Result<T>(ex);
        }
        public static Result Error(string text)
        {
            return new Result(text);
        }
        public static Result<T> Error<T>(string text)
        {
            return new Result<T>(text);
        }

        public static implicit operator bool(Result r)
        {
            return r.IsOK;
        }

        public Result(bool isOK, string? message = null, Exception? exception = null)
        {
            this.isOK = isOK;
            this.message = message;
            this.exception = exception;
        }

        public Result(string message) : this(false, message) { }
        public Result(Exception ex) : this(false, null, ex) { }

        public bool IsOK { get => isOK; }
        public string Message
        {
            get
            {
                if(exception is not null)
                {
                    return exception.Message;
                } 
                else if(message is not null)
                {
                    return message;
                } 
                else
                {
                    return "No message provided.";
                }
            }
        }
         

        protected string? message;
        protected Exception? exception;
        protected bool isOK;
    }

    public class Result<T>
    {
        public static implicit operator bool(Result<T> r)
        {
            return r.IsOK;
        }

        public Result(bool isOK, string? message = null, Exception? exception = null, T? value = default(T))
        {
            this.isOK = isOK;
            this.message = message;
            this.exception = exception;
            this.value = value;
        }

        public Result(T value)
        {
            this.value = value;
            isOK = true;
        }

        public Result(string message) : this(false, message) { }
        public Result(Exception ex) : this(false, null, ex) { }

        public bool IsOK { get => isOK; }
        public T? Value { get => value; }
        public string Message
        {
            get
            {
                if (exception is not null)
                {
                    return exception.Message;
                }
                else if (message is not null)
                {
                    return message;
                }
                else
                {
                    return "No message provided.";
                }
            }
        }

        protected string? message;
        protected Exception? exception;
        protected bool isOK;
        protected T? value;
    }
}
