namespace Llyn.Core;

public sealed record LEstablishment(
    int LEstablishmentUnsaved,
    long LEstablishmentEntry,
    long LEstablishmentSize)
{
    public bool LEstablishmentPending => LEstablishmentUnsaved > 0;

    public bool LEstablishmentSingle => LEstablishmentEntry == 1;
}
