namespace Llyn.UIDeportment;

internal sealed class QProfferItem
{
    internal QProfferItem(long id, string lead, string mark, string tail, string count)
    {
        QProfferItemId = id;
        QProfferItemLead = lead;
        QProfferItemMark = mark;
        QProfferItemTail = tail;
        QProfferItemCount = count;
    }

    public long QProfferItemId { get; }

    public string QProfferItemLead { get; }

    public string QProfferItemMark { get; }

    public string QProfferItemTail { get; }

    public string QProfferItemCount { get; }
}
