namespace Llyn.Core;

public sealed record LFellow(
    long LFellowId,
    string LFellowName,
    int LFellowShared)
{
    public string LFellowCount => LCatalog.LCatalogUsageFormat(LFellowShared);
}
