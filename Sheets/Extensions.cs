using System.Collections.Generic;

namespace Sheets
{
    public static class Extensions
    {
        public static IEnumerable<string> PadTo(this IEnumerable<string> input, int padLength)
        {
            var numReturned = 0;

            foreach (var item in input)
            {
                yield return item;
                numReturned++;
            }

            for (int i = numReturned; i < padLength; i++)
            {
                yield return string.Empty;
            }
        }
    }
}
