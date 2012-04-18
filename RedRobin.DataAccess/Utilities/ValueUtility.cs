using System;

namespace RedRobin.DataAccess.Utilities
{
    public class ValueUtility
    {
        public static string ConvertNull(string value, string ifNullOrError)
        {
            string result = ifNullOrError;
            return (string.IsNullOrEmpty(value) ? ifNullOrError : value);
        }

        public static bool IsNull(object value)
        {
            return ((value == DBNull.Value) || (value == null));
        }

        public static int? ToInteger(object value, int? ifNullOrFailure)
        {
            int? result = ifNullOrFailure;
            if (IsNull(value))
            {
                return ifNullOrFailure;
            }
            try
            {
                result = new int?(Convert.ToInt32(value.ToString()));
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static int ToInteger(object value, int ifFailure)
        {
            int result = ifFailure;
            try
            {
                result = Convert.ToInt32(value.ToString());
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}
