using System;
using System.Text;

namespace Sky_Health.Helpers
{
    public static class ArabicNumberHelper
    {
  
        public static string ToHindi(this decimal number)
        {
            return ConvertToHindi(number.ToString("N2"));
        }

        public static string ToHindi(this int number)
        {
            return ConvertToHindi(number.ToString());
        }


        public static string ToHindi(this long number)
        {
            return ConvertToHindi(number.ToString());
        }


        public static string ToHindi(this double number)
        {
            return ConvertToHindi(number.ToString("N2"));
        }

        public static string ToHindi(this DateTime date, string format)
        {
            string westernDateString = date.ToString(format);
            return ConvertToHindi(westernDateString);
        }


        public static string ToHindi(this string text)
        {
            return ConvertToHindi(text);
        }

        private static string ConvertToHindi(string westernNumber)
        {
            if (string.IsNullOrEmpty(westernNumber))
                return string.Empty;

            var hindi = new StringBuilder();
            foreach (char c in westernNumber)
            {
                switch (c)
                {
                    case '0': hindi.Append('٠'); break;
                    case '1': hindi.Append('١'); break;
                    case '2': hindi.Append('٢'); break;
                    case '3': hindi.Append('٣'); break;
                    case '4': hindi.Append('٤'); break;
                    case '5': hindi.Append('٥'); break;
                    case '6': hindi.Append('٦'); break;
                    case '7': hindi.Append('٧'); break;
                    case '8': hindi.Append('٨'); break;
                    case '9': hindi.Append('٩'); break;
                    default: hindi.Append(c); break;
                }
            }
            return hindi.ToString();
        }
    }
}
