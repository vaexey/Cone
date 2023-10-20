using ConeEngine.Model.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Util
{
    public static class ExWrapper
    {
        public delegate void WrapDelegate(ref Result r);
        public delegate void WrapDelegate<T>(out Result<T> r);

        public static Result Wrap(Func<Result> action)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                return Result.Error(ex);
            }
        }
        public static Result Wrap(WrapDelegate action)
        {
            try
            {
                var r = Result.OK;

                action(ref r);

                return r;
            }
            catch (Exception ex)
            {
                return Result.Error(ex);
            }
        }

        public static Result<T> Wrap<T>(Func<Result<T>> action)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                return Result.Error<T>(ex);
            }
        }
        public static Result<T> Wrap<T>(WrapDelegate<T> action)
        {
            try
            {
                action(out Result<T> r);

                return r;
            }
            catch (Exception ex)
            {
                return Result.Error<T>(ex);
            }
        }
    }
}
