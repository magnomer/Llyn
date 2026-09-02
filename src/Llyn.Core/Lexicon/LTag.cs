namespace Llyn.Core;

public sealed record LTag(
    string LTagText)
{
    public string LTagText { get; init; } = LTagText ?? string.Empty;
}
