namespace Llyn.UIShell;

internal sealed class PDirectoryItem
{
    internal PDirectoryItem(long id, string text, bool chosen)
    {
        PDirectoryItemId = id;
        PDirectoryItemText = text;
        PDirectoryItemChosen = chosen;
    }

    internal long PDirectoryItemId { get; }

    public string PDirectoryItemText { get; }

    public bool PDirectoryItemChosen { get; }
}
