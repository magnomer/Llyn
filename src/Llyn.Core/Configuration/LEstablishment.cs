namespace Llyn.Core;

public sealed record LEstablishment(
    int LEstablishmentUnsaved,
    long LEstablishmentEntry,
    long LEstablishmentSize)
{
    private const long LEstablishmentMegabyte = 1024L * 1024;

    public bool LEstablishmentPending => LEstablishmentUnsaved > 0;

    public bool LEstablishmentSingle => LEstablishmentEntry == 1;

    public bool LEstablishmentLarge => LEstablishmentSize >= LEstablishmentMegabyte;

    public double LEstablishmentAmount => LEstablishmentLarge
        ? (double)LEstablishmentSize / LEstablishmentMegabyte
        : (LEstablishmentSize + 1023) / 1024;
}
