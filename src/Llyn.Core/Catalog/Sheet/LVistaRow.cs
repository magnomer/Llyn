namespace Llyn.Core;

public sealed record LVistaRow(
    long LVistaRowId,
    string LVistaRowHeadword,
    string LVistaRowLanguage,
    string? LVistaRowEpithet,
    string LVistaRowName,
    bool LVistaRowChosen);
