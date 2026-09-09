using System;
using System.Text;

namespace Llyn.Core;

public static class LMarkupMark
{
    public static void LMarkupMarkAppend(StringBuilder text, string mark, LStateValue value)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(value);

        if (value.LStateValueState == LState.LStateUnspecified)
        {
            return;
        }

        text.Append(' ')
            .Append(mark)
            .Append('=')
            .Append(LMarkupMarkNormalize(value.LStateValueShow()));
    }

    public static string LMarkupMarkNormalize(string value)
    {
        return value.Contains('"', StringComparison.Ordinal)
            ? "'" + value.Replace("'", string.Empty, StringComparison.Ordinal) + "'"
            : "\"" + value + "\"";
    }
}
