namespace Llyn.Conduct;

public sealed record CEstablishment(
    int CEstablishmentUnsaved,
    long CEstablishmentEntry,
    long CEstablishmentSize,
    bool CEstablishmentPending,
    bool CEstablishmentSingle);
