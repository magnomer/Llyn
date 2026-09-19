namespace Llyn.Core;

public sealed record LFont(
    string? LFontFamily,
    double LFontSize,
    string? LFontStyle = null)
{
    public double? LFontSized => LFontSize > 0 ? LFontSize : null;
}
