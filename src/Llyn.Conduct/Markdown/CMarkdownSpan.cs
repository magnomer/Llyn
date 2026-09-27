using System;

namespace Llyn.Conduct;

public sealed record CMarkdownSpan(
    string CMarkdownSpanText,
    bool CMarkdownSpanBold,
    bool CMarkdownSpanItalic,
    bool CMarkdownSpanCode,
    Uri? CMarkdownSpanAddress);
