namespace Llyn.Core;

public sealed record LExample(
    string LExampleId,
    string LExampleLanguage,
    LStateValue LExampleText,
    LStateValue LExampleTranslation,
    LStateValue LExampleSource)
{
    public LStateValue LExampleText { get; init; } = LExampleText ?? LStateValue.LStateValueUnspecified;

    public LStateValue LExampleTranslation { get; init; } =
        LExampleTranslation ?? LStateValue.LStateValueUnspecified;

    public LStateValue LExampleSource { get; init; } = LExampleSource ?? LStateValue.LStateValueUnspecified;
}
