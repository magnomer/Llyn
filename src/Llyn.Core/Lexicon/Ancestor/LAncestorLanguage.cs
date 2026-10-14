namespace Llyn.Core;

public sealed record LAncestorLanguage(
    string LAncestorLanguageName,
    long? LAncestorLanguageTongue = null,
    int LAncestorLanguageUsage = 0)
{
    public string LAncestorLanguageName { get; init; } = LAncestorLanguageName ?? string.Empty;
}
