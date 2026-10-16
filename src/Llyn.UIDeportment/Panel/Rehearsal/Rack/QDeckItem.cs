namespace Llyn.UIDeportment;

internal sealed class QDeckItem
{
    internal QDeckItem(long id, string text, bool folder, bool chosen, string count)
    {
        QDeckItemId = id;
        QDeckItemText = text;
        QDeckItemFolder = folder;
        QDeckItemChosen = chosen;
        QDeckItemCount = count;
    }

    internal long QDeckItemId { get; }

    public string QDeckItemText { get; }

    internal bool QDeckItemFolder { get; }

    public bool QDeckItemChosen { get; }

    public string QDeckItemCount { get; }
}
