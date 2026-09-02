namespace Llyn.Core;

public sealed record LTag(
    string LTagId,
    LStateValue LTagText)
{
    public LStateValue LTagText { get; init; } = LTagText ?? LStateValue.LStateValueUnspecified;
}

