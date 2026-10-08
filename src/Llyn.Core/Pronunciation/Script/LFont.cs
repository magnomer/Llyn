namespace Llyn.Core;

public sealed record LFont(
    string? LFontFamily,
    double LFontSize,
    string? LFontStyle = null)
{
    public string? LFontStyle { get; init; } = LFontStyle?.ToLowerInvariant() switch
    {
        "italic" => "italic",
        "oblique" => "oblique",
        _ => null,
    };

    public string? LFontFace => string.IsNullOrWhiteSpace(LFontFamily) ? null : LFontFamily;

    public double? LFontSized => double.IsFinite(LFontSize) && LFontSize > 0 ? LFontSize : null;
}
