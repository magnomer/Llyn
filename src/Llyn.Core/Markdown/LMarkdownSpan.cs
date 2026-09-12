namespace Llyn.Core;

public sealed record LMarkdownSpan(
    string LMarkdownSpanText,
    bool LMarkdownSpanBold,
    bool LMarkdownSpanItalic,
    bool LMarkdownSpanCode,
    string LMarkdownSpanLink);
