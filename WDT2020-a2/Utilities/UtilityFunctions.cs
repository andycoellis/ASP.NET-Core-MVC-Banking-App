using System;
namespace WDT2020_a2.Utilities
{
    public static class UtilityFunctions
    {
        public static string GenerateStringID(int length)
        {
            int number;

            //Create a boundary where there will always be a min where 1 is the
            //lead digit and max where 9 is the lead
            var min = Convert.ToInt32(Math.Pow(10, length) - (.9 * Math.Pow(10, length)));
            var max = Convert.ToInt32(Math.Pow(10, length) - 1);

            number = new Random().Next(min, max);

            return number.ToString();
        }


        // EXTENSION METHODS //

        // Adapted from Week 7 Tutorial Example Code, thanks Matthew
        public static bool MoreThan2DecimalPlaces(this double value) => decimal.Round(Convert.ToDecimal(value), 2) != Convert.ToDecimal(value);
    }
}
