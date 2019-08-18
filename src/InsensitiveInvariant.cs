using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace NoDiacriticsComparer
{
    public class InsensitiveInvariant : IComparer<string>, IEqualityComparer<string>
    {
        public static readonly InsensitiveInvariant Instance = new InsensitiveInvariant();

        private InsensitiveInvariant()
        {
        }

        public static CompareInfo CompareInfo { get; } = CultureInfo.InvariantCulture.CompareInfo;

        public static IComparer<string> Comparer => Instance;

        public static IEqualityComparer<string> EqualityComparer => Instance;

        #region IComparer

        public int Compare(string x, string y)
        {
            if (x == null && y == null)
                return 0;
            if (x == null)
                return 1;
            if (y == null)
                return -1;

            return CompareInfo.Compare(x, y, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace);
        }

        #endregion IComparer

        #region IEqualityComparer

        public bool Equals(string x, string y) => Instance.Compare(x, y) == 0;

        public int GetHashCode(string obj) => RemoveDiacritics(obj).ToUpperInvariant().GetHashCode();

        #endregion IEqualityComparer

        public static int CompareValues(string x, string y) => Instance.Compare(x, y);

        public static bool Contains(string x, string y)
        {
            if (x == null || y == null || x.Length < y.Length)
                return false;

            return CompareInfo.IndexOf(x, y, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0;
        }

        public static bool EndsWith(string x, string y)
        {
            if (x == null || y == null || x.Length < y.Length)
                return false;

            return CompareInfo.IsSuffix(x, y, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace);
        }

        public static bool EqualsValues(string x, string y) => Instance.Compare(x, y) == 0;

        public static string RemoveDiacritics(string input)
        {
            var stFormD = input.Normalize(NormalizationForm.FormD);
            var len = stFormD.Length;
            var sb = new StringBuilder();
            for (var j = 0; j < len; j++)
                if (CharUnicodeInfo.GetUnicodeCategory(stFormD[j]) != UnicodeCategory.NonSpacingMark)
                    sb.Append(stFormD[j]);

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        public static bool StartsWith(string x, string y)
        {
            if (x == null || y == null || x.Length < y.Length)
                return false;

            return CompareInfo.IsPrefix(x, y, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace);
        }
    }
}