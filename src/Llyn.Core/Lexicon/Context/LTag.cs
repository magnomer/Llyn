namespace Llyn.Core;

public sealed record LTag(
    long LTagId,
    string LTagText)
{
    public string LTagText { get; init; } = LTagText ?? string.Empty;
}
