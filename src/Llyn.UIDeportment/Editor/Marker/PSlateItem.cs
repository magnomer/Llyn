namespace Llyn.UIDeportment;

internal sealed class PSlateItem
{
    internal PSlateItem(long id, string lead, string mark, string tail)
    {
        PSlateItemId = id;
        PSlateItemLead = lead;
        PSlateItemMark = mark;
        PSlateItemTail = tail;
    }

    internal long PSlateItemId { get; }

    public string PSlateItemLead { get; }

    public string PSlateItemMark { get; }

    public string PSlateItemTail { get; }
}
