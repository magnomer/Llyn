namespace Llyn.UIDeportment;

internal sealed class PCategoryItem
{
    internal PCategoryItem(long valueId, string name, bool taken)
    {
        PCategoryItemValue = valueId;
        PCategoryItemName = name;
        PCategoryItemTaken = taken;
    }

    public long PCategoryItemValue { get; }

    public string PCategoryItemName { get; }

    public bool PCategoryItemTaken { get; }
}
