using System.Collections.Generic;
using System.Globalization;

namespace Llyn.Core;

public sealed record LMarkdownBlock(
    LMarkdownKind LMarkdownBlockKind,
    int LMarkdownBlockLevel,
    IReadOnlyList<LMarkdownSpan> LMarkdownBlockSpan,
    string LMarkdownBlockText,
    int LMarkdownBlockOrdinal = 0)
{
    public bool LMarkdownBlockNumbered => LMarkdownBlockKind == LMarkdownKind.LMarkdownKindNumber;

    public bool LMarkdownBlockHeaded => LMarkdownBlockKind == LMarkdownKind.LMarkdownKindHeading;

    public bool LMarkdownBlockListed =>
        LMarkdownBlockNumbered || LMarkdownBlockKind == LMarkdownKind.LMarkdownKindBullet;

    public bool LMarkdownBlockQuoted => LMarkdownBlockKind == LMarkdownKind.LMarkdownKindQuote;

    public bool LMarkdownBlockFenced => LMarkdownBlockKind == LMarkdownKind.LMarkdownKindCode;

    public bool LMarkdownBlockRuled => LMarkdownBlockKind == LMarkdownKind.LMarkdownKindRule;

    public string LMarkdownBlockMark => LMarkdownBlockKind switch
    {
        LMarkdownKind.LMarkdownKindBullet => "•",
        LMarkdownKind.LMarkdownKindNumber => LMarkdownBlockOrdinal.ToString(CultureInfo.InvariantCulture) + ".",
        _ => string.Empty,
    };
}
