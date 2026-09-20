using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LMarkupNode(
    string LMarkupNodeName,
    IReadOnlyDictionary<string, string> LMarkupNodeAttribute,
    IReadOnlyList<LMarkupNode> LMarkupNodeChild,
    string LMarkupNodeText,
    int LMarkupNodeLine)
{
    private static readonly IReadOnlyDictionary<string, string> LMarkupNodeBare = new Dictionary<string, string>();

    public static LMarkupNode LMarkupNodeCreate(string name, string text = "")
    {
        return new LMarkupNode(name, LMarkupNodeBare, [], text, 0);
    }

    public static LMarkupNode LMarkupNodeCreate(string name, IReadOnlyList<LMarkupNode> children)
    {
        return new LMarkupNode(name, LMarkupNodeBare, children, string.Empty, 0);
    }
}
