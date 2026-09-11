namespace Llyn.Core;

public sealed record LExample(
    long LExampleId,
    string LExampleLanguage,
    LStateValue LExampleText,
    LStateValue LExampleTranslation,
    LStateAnchor LExampleSource)
{
    public LStateValue LExampleText { get; init; } = LExampleText ?? LStateValue.LStateValueUnspecified;

    public LStateValue LExampleTranslation { get; init; } =
        LExampleTranslation ?? LStateValue.LStateValueUnspecified;

    public LStateAnchor LExampleSource { get; init; } = LExampleSource ?? LStateAnchor.LStateAnchorUnspecified;
}
