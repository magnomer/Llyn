namespace Llyn.Core;

public sealed record LMarkupMention(
    int LMarkupMentionOffset,
    int LMarkupMentionLength,
    string LMarkupMentionHeadword,
    string LMarkupMentionLanguage,
    string LMarkupMentionSense = "")
{
    public string LMarkupMentionHeadword { get; init; } = LMarkupMentionHeadword ?? string.Empty;

    public string LMarkupMentionLanguage { get; init; } = LMarkupMentionLanguage ?? string.Empty;

    public string LMarkupMentionSense { get; init; } = LMarkupMentionSense ?? string.Empty;
}
