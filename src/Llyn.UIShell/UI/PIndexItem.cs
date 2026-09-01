namespace Llyn.UIShell;

/// <summary>
/// Presentation item for one entry row in <c>PIndex</c>. Carries the headword and language the row
/// shows, and the entry id the row loads through — the id is identity and never displayed, so the
/// row stays selectable after a headword is edited into something another entry also reads as.
/// </summary>
internal sealed class PIndexItem
{
    internal PIndexItem(string id, string headword, string language)
    {
        PIndexItemId = id;
        PIndexItemHeadword = headword;
        PIndexItemLanguage = language;
    }

    public string PIndexItemId { get; }

    public string PIndexItemHeadword { get; }

    public string PIndexItemLanguage { get; }
}
