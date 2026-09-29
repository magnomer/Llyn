namespace Llyn.UIDeportment;

internal sealed class PProfferItem
{
    internal PProfferItem(long id, string lead, string mark, string tail, string count)
    {
        PProfferItemId = id;
        PProfferItemLead = lead;
        PProfferItemMark = mark;
        PProfferItemTail = tail;
        PProfferItemCount = count;
    }

    public long PProfferItemId { get; }

    public string PProfferItemLead { get; }

    public string PProfferItemMark { get; }

    public string PProfferItemTail { get; }

    public string PProfferItemCount { get; }
}
