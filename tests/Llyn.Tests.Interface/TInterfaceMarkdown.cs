using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceMarkdown
{
    internal static LMarkdownPort TMarkdownPortCreate() =>
        TEngineFake.TEngineCreate<LMarkdownPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineMarkdownParse"] = args => TInterface.TMarkdownParse((string?)args![0]),
        });

    internal static IReadOnlyList<CMarkdownBlock> TMarkdownBlockRead(string? text) =>
        CMarkdown.LMarkdownParse(TMarkdownPortCreate(), text);
}
