using System;
using System.Globalization;

namespace Llyn.Application;

public static class LCase
{
    public static string LCaseUpperChange(string value, CultureInfo culture)
    {
        return LCaseChange(value, culture.TextInfo.ToUpper);
    }

    public static string LCaseLowerChange(string value, CultureInfo culture)
    {
        return LCaseChange(value, culture.TextInfo.ToLower);
    }

    private static string LCaseChange(string value, Func<string, string> changeCase)
    {
        if (value.Length == 0)
        {
            return value;
        }

        string firstElement = StringInfo.GetNextTextElement(value);
        return changeCase(firstElement) + value[firstElement.Length..];
    }
}
