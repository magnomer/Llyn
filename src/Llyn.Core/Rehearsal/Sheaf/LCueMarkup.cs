using System;

namespace Llyn.Core;

public static class LCueMarkup
{
    public static string LCueMarkupFormat(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        return text.Replace('|', '∣');
    }
}
