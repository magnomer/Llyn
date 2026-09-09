using System;
using System.Text;

namespace Llyn.Core;

public static class LMarkupLeaf
{
    public static void LMarkupLeafAppend(
        StringBuilder text, int depth, string name, LStateValue value)
    {
        LMarkupLeafAppend(text, depth, name, value, string.Empty, null);
    }

    public static void LMarkupLeafAppend(
        StringBuilder text, int depth, string name, LStateValue value, string mark, string? marked)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(value);

        if (value.LStateValueState == LState.LStateUnspecified)
        {
            return;
        }

        text.Append(' ', depth * 2).Append('<').Append(name);

        if (!string.IsNullOrEmpty(marked))
        {
            text.Append(' ')
                .Append(mark)
                .Append('=')
                .Append(LMarkupMark.LMarkupMarkNormalize(marked));
        }

        text.Append('>');

        if (value.LStateValueState == LState.LStateSpecified)
        {
            text.Append(LMarkupLeafNormalize(value.LStateValueShow(), name));
        }

        text.Append("</").Append(name).Append(">\n");
    }

    public static string LMarkupLeafNormalize(string value, string name)
    {
        return value.Replace("</" + name + ">", string.Empty, StringComparison.Ordinal);
    }
}
