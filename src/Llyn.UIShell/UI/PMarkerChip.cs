namespace Llyn.UIShell;

internal sealed class PMarkerChip
{
    internal PMarkerChip(long valueId, string name)
    {
        PMarkerChipValue = valueId;
        PMarkerChipName = name;
    }

    public long PMarkerChipValue { get; }

    public string PMarkerChipName { get; }
}
