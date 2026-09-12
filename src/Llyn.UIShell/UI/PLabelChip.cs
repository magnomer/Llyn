namespace Llyn.UIShell;

internal sealed class PLabelChip
{
    internal PLabelChip(long id, string name)
    {
        PLabelChipId = id;
        PLabelChipName = name;
    }

    internal long PLabelChipId { get; set; }

    public string PLabelChipName { get; }
}
