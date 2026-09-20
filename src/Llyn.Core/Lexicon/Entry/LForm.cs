namespace Llyn.Core;

public sealed record LForm(
    long LFormEntryId,
    int LFormPosition,
    string LFormText,
    string? LFormLocal,
    string LFormRole);
