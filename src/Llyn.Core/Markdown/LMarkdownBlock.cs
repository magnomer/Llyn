using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LMarkdownBlock(
    LMarkdownKind LMarkdownBlockKind,
    int LMarkdownBlockLevel,
    IReadOnlyList<LMarkdownSpan> LMarkdownBlockSpan,
    string LMarkdownBlockText);
