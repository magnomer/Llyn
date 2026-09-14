namespace Llyn.Core;

public sealed record LMarkupTranslation(
    string LMarkupTranslationHeadword,
    string LMarkupTranslationLanguage)
{
    public string LMarkupTranslationHeadword { get; init; } = LMarkupTranslationHeadword ?? string.Empty;

    public string LMarkupTranslationLanguage { get; init; } = LMarkupTranslationLanguage ?? string.Empty;
}
