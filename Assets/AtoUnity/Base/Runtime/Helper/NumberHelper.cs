using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace AtoGame.Base.Helper
{
    /// <summary> Extensions for various number types. </summary>
    public static class NumberHelper
    {
        /// <summary>
        ///     Formats a large number to 5 digits (2 decimal places) with a suffix. (e.g. 123,456,789,123 (123 Billion) ->
        ///     123.45B)
        /// </summary>
        /// <param name="numberToFormat"> The number to format. </param>
        /// <param name="roundType"> Specifies the strategy that mathematical rounding methods should use to round a number</param>
        /// <param name="decimalPlaces"> The number of decimal places to include - <i> defaults to <c> 2 </c> </i> (only with <see cref="RoundType.Round"/> and <see cref="RoundType.RoundAwayFromZero"/>) </param>
        /// <returns> A <see cref="string" />. </returns>
        public static string FormatNumber(this long numberToFormat, RoundType roundType = RoundType.Round, int decimalPlaces = 2)
        {
            // Get the default string representation of the numberToFormat.
            string numberString = numberToFormat.ToString();

            foreach (NumberSuffix suffix in Enum.GetValues(typeof(NumberSuffix)))
            {
                // Assign the amount of digits to base 10.
                double currentValue = 1 * Math.Pow(10, (int)suffix * 3);

                // Get the suffix value.
                string? suffixValue = Enum.GetName(typeof(NumberSuffix), (int)suffix);

                // If the suffix is the placeholder, set it to an empty string.
                if ((int)suffix == 0) { suffixValue = string.Empty; }

                // Set the return value to a rounded value with the suffix.
                if (numberToFormat >= currentValue)
                {
                    double roundedNumber = numberToFormat / currentValue;
                    switch(roundType)
                    {
                        case RoundType.Round:
                            roundedNumber = Math.Round(roundedNumber, decimalPlaces, MidpointRounding.ToEven);
                            break;
                        case RoundType.RoundAwayFromZero:
                            roundedNumber = Math.Round(roundedNumber, decimalPlaces, MidpointRounding.AwayFromZero);
                            break;
                        case RoundType.Ceiling:
                            roundedNumber = Math.Ceiling(roundedNumber);
                            break;
                        case RoundType.Floor:
                            roundedNumber = Math.Floor(roundedNumber);
                            break;
                    }    
                    numberString = $"{roundedNumber}{suffixValue}";
                }
            }

            return numberString;
        }



        /// <summary> Suffixes for numbers based on how many digits they have left of the decimal point. </summary>
        /// <remarks> The order of the suffixes matters! </remarks>
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private enum NumberSuffix
        {
            /// <summary> A placeholder if the value is under 1 thousand </summary>
            P,

            /// <summary> Thousand </summary>
            K,

            /// <summary> Million </summary>
            M,

            /// <summary> Billion </summary>
            B,

            /// <summary> Trillion </summary>
            T,

            /// <summary> Quadrillion </summary>
            Q
        }

        public enum RoundType
        {
            /// <summary>
            /// Rounds a decimal value to the nearest integral value, and rounds midpoint values to the nearest even number.
            /// (e.g 3.45 => 3.4)
            /// </summary>
            Round,
            /// <summary>
            /// Rounds a decimal value to the nearest integral value, and rounds midpoint values to the nearest number that's away from zero.
            /// (e.g 3.45 => 3.5)
            /// </summary>
            RoundAwayFromZero,
            /// <summary>
            /// Returns the smallest integral value that is greater than or equal to the specified decimal number.
            /// </summary>
            Ceiling,
            /// <summary>
            /// Returns the largest integral value less than or equal to the specified decimal number.
            /// </summary>
            Floor
        }
    }
}
