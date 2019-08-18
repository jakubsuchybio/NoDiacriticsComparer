using System.Globalization;
using System.Text;

namespace System
{
    public static class StringComparerEx
    {
        public static StringComparer CurrentCultureIgnoreDiacritics { get; } = 
            new NoDiacriticsCultureAwareComparer(CultureInfo.CurrentCulture.CompareInfo, CompareOptions.None);

        public static StringComparer CurrentCultureIgnoreCaseIgnoreDiacritics { get; } = 
            new NoDiacriticsCultureAwareComparer(CultureInfo.CurrentCulture.CompareInfo, CompareOptions.IgnoreCase);

        public static StringComparer InvariantCultureIgnoreDiacritics { get; } = 
            new NoDiacriticsCultureAwareComparer(CultureInfo.InvariantCulture.CompareInfo, CompareOptions.None);

        public static StringComparer InvariantCultureIgnoreCaseIgnoreDiacritics { get; } = 
            new NoDiacriticsCultureAwareComparer(CultureInfo.InvariantCulture.CompareInfo, CompareOptions.IgnoreCase);

        public static string RemoveDiacritics(this string input)
        {
            var stFormD = input.Normalize(NormalizationForm.FormD);
            var len = stFormD.Length;
            var sb = new StringBuilder();
            for (var j = 0; j < len; j++)
                if (CharUnicodeInfo.GetUnicodeCategory(stFormD[j]) != UnicodeCategory.NonSpacingMark)
                    sb.Append(stFormD[j]);

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }

    public class NoDiacriticsCultureAwareComparer : StringComparer
    {
        private readonly CompareInfo _compareInfo;
        private readonly CompareOptions _compareOptions;

        public NoDiacriticsCultureAwareComparer(CompareInfo compareInfo, CompareOptions compareOptions)
        {
            _compareInfo = compareInfo;
            _compareOptions = compareOptions;
        }

        #region StringComparer overrides
        public override bool Equals(string? x, string? y) => Compare(x, y) == 0;

        public override int Compare(string? x, string? y)
        {
            if (ReferenceEquals(x, y)) 
                return 0;

            if (x == null) 
                return -1;

            if (y == null) 
                return 1;

            return _compareInfo.Compare(StringComparerEx.RemoveDiacritics(x), StringComparerEx.RemoveDiacritics(y), _compareOptions);
        }

        public override int GetHashCode(string? obj)
        {            
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            return _compareInfo.GetHashCode(StringComparerEx.RemoveDiacritics(obj), _compareOptions);
        }
        #endregion StringComparer overrides
    }
}