namespace Llyn.UIShell;

internal sealed class PTranslationItem
{
    internal PTranslationItem(string id, string language, string text)
    {
        PTranslationItemId = id;
        PTranslationItemLanguage = language;
        PTranslationItemText = text;
    }

    public string PTranslationItemId { get; set; }

    public string PTranslationItemLanguage { get; set; }

    public string PTranslationItemText { get; set; }
}
