using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LMarkdownBlock(
    LMarkdownKind LMarkdownBlockKind,
    int LMarkdownBlockLevel,
    IReadOnlyList<LMarkdownSpan> LMarkdownBlockSpan,
    string LMarkdownBlockText,
    int LMarkdownBlockOrdinal = 0)
{
    public bool LMarkdownBlockNumbered => LMarkdownBlockKind == LMarkdownKind.LMarkdownKindNumber;
}
