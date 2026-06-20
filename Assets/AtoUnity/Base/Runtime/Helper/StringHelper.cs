using UnityEngine;


namespace AtoGame.Base.Helper
{
    public static class StringHelper
    {
        public static string ToCurrencyFormat(this int number) // 1,000,000
        {
            return System.String.Format("{0:N0}", number);
        }

        public static string ToCurrencyFormat(this long number) // 1,000,000
        {
            return System.String.Format("{0:N0}", number);
        }

        public static string ToShortCurrencyFormat(this int number) // 100M
        {
            if (number < 1000)
                return number.ToString();

            if (number < 10000)
                return System.String.Format("{0:#,.##}K", number - 5);

            if (number < 100000)
                return System.String.Format("{0:#,.#}K", number - 50);

            if (number < 1000000)
                return System.String.Format("{0:#,.}K", number - 500);

            if (number < 10000000)
                return System.String.Format("{0:#,,.##}M", number - 5000);

            if (number < 100000000)
                return System.String.Format("{0:#,,.#}M", number - 50000);

            if (number < 1000000000)
                return System.String.Format("{0:#,,.}M", number - 500000);

            return System.String.Format("{0:#,,,.##}B", number - 5000000);
        }
    }
}
