namespace Llyn.UIDeportment;

internal sealed class QLabelChip
{
    internal QLabelChip(long id, string name)
    {
        QLabelChipId = id;
        QLabelChipName = name;
    }

    internal long QLabelChipId { get; }

    public string QLabelChipName { get; }
}
