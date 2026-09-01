namespace Llyn.Core;

public sealed record LForm(
    string LFormEntryId,
    int LFormPosition,
    string LFormText,
    string? LFormLocal,
    string LFormRole);
