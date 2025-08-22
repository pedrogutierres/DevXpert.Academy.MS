using System;
using System.Text;

namespace DevXpert.Academy.Core.Domain.Extensions
{
    public static class CoreExtensions
    {
        public static DateTime ToFirstHourOfDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
        }

        public static DateTime ToLastHourOfDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);
        }

        public static DateTime ToFirstDayOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0);
        }
        public static DateTime ToLastDayOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1, 23, 59, 59).AddMonths(1).AddDays(-1);
        }

        public static string AgruparTodasAsMensagens(this Exception exception)
        {
            var msg = new StringBuilder(exception.Message);

            var tmp = exception;
            while (tmp.InnerException != null)
            {
                tmp = tmp.InnerException;
                msg.AppendLine();
                msg.Append(tmp.Message);
            }

            return msg.ToString();
        }

        public static string AgruparTodasAsException(this Exception exception)
        {
            var msg = new StringBuilder(exception.ToString());

            var tmp = exception;
            while (tmp.InnerException != null)
            {
                tmp = tmp.InnerException;

                msg.AppendLine();
                msg.Append(tmp.ToString());
            }

            return msg.ToString();
        }

        public static bool SwitchInline<T>(this T obj, params T[] values)
        {
            if (obj == null)
                throw new NullReferenceException(nameof(obj));

            if (values == null)
                return false;

            for (int i = 0; i < values.Length; i++)
                if (obj.Equals(values[i]))
                    return true;
           
            return false;
        }

        public static string ToMD5(this string texto)
        {
            if (string.IsNullOrEmpty(texto)) return null;
            return ToMD5(Encoding.ASCII.GetBytes(texto));
        }
        public static string ToMD5(this byte[] bytes)
        {
            if (bytes == null) return null;

            using var md5 = System.Security.Cryptography.MD5.Create();
            return Convert.ToHexString(md5.ComputeHash(bytes));
        }
    }
}
