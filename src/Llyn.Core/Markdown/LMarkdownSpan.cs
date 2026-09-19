using System;

namespace Llyn.Core;

public sealed record LMarkdownSpan(
    string LMarkdownSpanText,
    bool LMarkdownSpanBold,
    bool LMarkdownSpanItalic,
    bool LMarkdownSpanCode,
    string LMarkdownSpanLink)
{
    public Uri? LMarkdownSpanAddress =>
        LMarkdownSpanLink.Length > 0
        && Uri.TryCreate(LMarkdownSpanLink, UriKind.Absolute, out Uri? target)
        && (target.Scheme == Uri.UriSchemeHttp || target.Scheme == Uri.UriSchemeHttps)
            ? target
            : null;
}
