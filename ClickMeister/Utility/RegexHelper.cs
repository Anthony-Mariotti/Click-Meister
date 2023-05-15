using System.Text.RegularExpressions;

namespace ClickMeister.Utility;
public static partial class RegexHelper
{
    [GeneratedRegex("[^0-9]+")]
    public static partial Regex NumericOnly();
}
