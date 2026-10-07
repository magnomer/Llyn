namespace Llyn.UIDeportment;

internal sealed class QUnitItem
{
    internal QUnitItem(string key, bool taken)
    {
        QUnitItemKey = key;
        QUnitItemTaken = taken;
    }

    public string QUnitItemKey { get; }

    public bool QUnitItemTaken { get; }
}
