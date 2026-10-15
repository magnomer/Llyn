namespace Llyn.UIDeportment;

internal sealed class QSlateItem
{
    internal QSlateItem(long id, string lead, string mark, string tail)
    {
        QSlateItemId = id;
        QSlateItemLead = lead;
        QSlateItemMark = mark;
        QSlateItemTail = tail;
    }

    internal long QSlateItemId { get; }

    public string QSlateItemLead { get; }

    public string QSlateItemMark { get; }

    public string QSlateItemTail { get; }
}
