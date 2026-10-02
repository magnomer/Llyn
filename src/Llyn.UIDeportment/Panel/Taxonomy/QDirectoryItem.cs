namespace Llyn.UIDeportment;

internal sealed class QDirectoryItem
{
    internal QDirectoryItem(long id, string text, bool chosen)
    {
        QDirectoryItemId = id;
        QDirectoryItemText = text;
        QDirectoryItemChosen = chosen;
    }

    internal long QDirectoryItemId { get; }

    public string QDirectoryItemText { get; }

    public bool QDirectoryItemChosen { get; }
}
