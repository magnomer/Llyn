namespace Llyn.UIShell;

internal sealed class PCategoryItem
{
    internal PCategoryItem(string name, bool taken)
    {
        PCategoryItemName = name;
        PCategoryItemTaken = taken;
    }

    public string PCategoryItemName { get; }

    public bool PCategoryItemTaken { get; }
}
