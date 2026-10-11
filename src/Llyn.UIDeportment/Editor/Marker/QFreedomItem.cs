namespace Llyn.UIDeportment;

internal sealed class QFreedomItem
{
    internal QFreedomItem(string key, bool taken)
    {
        QFreedomItemKey = key;
        QFreedomItemTaken = taken;
    }

    public string QFreedomItemKey { get; }

    public bool QFreedomItemTaken { get; }
}
