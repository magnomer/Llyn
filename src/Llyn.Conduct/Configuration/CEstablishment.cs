namespace Llyn.Conduct;

public sealed record CEstablishment(
    int CEstablishmentUnsaved,
    long CEstablishmentEntry,
    bool CEstablishmentPending,
    string CEstablishmentEntryKey,
    string CEstablishmentSizeKey,
    string CEstablishmentAmount);
