namespace Llyn.UIShell;

internal sealed class PRenditionItem
{
    internal PRenditionItem(string id, string language, string text)
    {
        PRenditionItemId = id;
        PRenditionItemLanguage = language;
        PRenditionItemText = text;
    }

    public string PRenditionItemId { get; set; }

    public string PRenditionItemLanguage { get; set; }

    public string PRenditionItemText { get; set; }
}
