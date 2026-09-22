namespace Llyn.Core;

public sealed record LMarkupEtymon(string LMarkupEtymonHeadword, string LMarkupEtymonLanguage)
{
    public string LMarkupEtymonHeadword { get; init; } = LMarkupEtymonHeadword ?? string.Empty;

    public string LMarkupEtymonLanguage { get; init; } = LMarkupEtymonLanguage ?? string.Empty;
}
