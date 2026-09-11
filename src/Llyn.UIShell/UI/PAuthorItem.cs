using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PAuthorItem
{
    internal PAuthorItem(LAuthor author, int position, int total)
    {
        PAuthorItemId = author.LAuthorId;
        PAuthorItemName = author.LAuthorName;
        PAuthorItemPosition = position;
        PAuthorItemEarlier = position > 0;
        PAuthorItemLater = position < total - 1;
    }

    public long PAuthorItemId { get; }

    public string PAuthorItemName { get; }

    public int PAuthorItemPosition { get; }

    public bool PAuthorItemEarlier { get; }

    public bool PAuthorItemLater { get; }
}
