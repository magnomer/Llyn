using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed record CMarkdownBlock(
    bool CMarkdownBlockHeaded,
    bool CMarkdownBlockListed,
    bool CMarkdownBlockQuoted,
    bool CMarkdownBlockFenced,
    bool CMarkdownBlockRuled,
    int CMarkdownBlockLevel,
    string CMarkdownBlockMark,
    string CMarkdownBlockText,
    IReadOnlyList<CMarkdownSpan> CMarkdownBlockSpan);
