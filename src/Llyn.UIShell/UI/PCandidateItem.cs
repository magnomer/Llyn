using System.Globalization;

namespace Llyn.UIShell;

internal sealed class PCandidateItem
{
    internal PCandidateItem(string id, string title, int usage, string word)
    {
        PCandidateItemId = id;
        PCandidateItemTitle = title;
        PCandidateItemUsage = usage;

        int found = -1;
        int size = 0;
        if (word.Length != 0)
        {
            found = CultureInfo.CurrentCulture.CompareInfo.IndexOf(
                title, word, CompareOptions.IgnoreCase, out size);
        }

        if (found < 0)
        {
            PCandidateItemLead = title;
            PCandidateItemMark = string.Empty;
            PCandidateItemTail = string.Empty;
            return;
        }

        PCandidateItemLead = title[..found];
        PCandidateItemMark = title.Substring(found, size);
        PCandidateItemTail = title[(found + size)..];
    }

    public string PCandidateItemId { get; }

    public string PCandidateItemTitle { get; }

    public int PCandidateItemUsage { get; }

    public string PCandidateItemLead { get; }

    public string PCandidateItemMark { get; }

    public string PCandidateItemTail { get; }

    public string PCandidateItemCount => PCandidateItemUsage > 0
        ? PCandidateItemUsage.ToString(CultureInfo.CurrentCulture)
        : string.Empty;
}
