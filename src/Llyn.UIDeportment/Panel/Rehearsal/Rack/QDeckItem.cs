namespace Llyn.UIDeportment;

internal sealed class QDeckItem
{
    internal QDeckItem(long id, string text, bool folder, bool chosen)
    {
        QDeckItemId = id;
        QDeckItemText = text;
        QDeckItemFolder = folder;
        QDeckItemChosen = chosen;
    }

    internal long QDeckItemId { get; }

    public string QDeckItemText { get; }

    internal bool QDeckItemFolder { get; }

    public bool QDeckItemChosen { get; }
}
