namespace Llyn.UIShell;

internal sealed class PDirectoryItem
{
    internal PDirectoryItem(string text, bool chosen)
    {
        PDirectoryItemText = text;
        PDirectoryItemChosen = chosen;
    }

    public string PDirectoryItemText { get; }

    public bool PDirectoryItemChosen { get; }
}
