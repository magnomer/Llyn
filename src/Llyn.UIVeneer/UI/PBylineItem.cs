using System.Globalization;

namespace Llyn.UIVeneer;

internal sealed class PBylineItem
{
    internal PBylineItem(long id, string name, string word)
    {
        PBylineItemId = id;
        PBylineItemName = name;

        int found = -1;
        int size = 0;
        if (word.Length != 0)
        {
            found = CultureInfo.CurrentCulture.CompareInfo.IndexOf(
                name, word, CompareOptions.IgnoreCase, out size);
        }

        if (found < 0)
        {
            PBylineItemLead = name;
            PBylineItemMark = string.Empty;
            PBylineItemTail = string.Empty;
            return;
        }

        PBylineItemLead = name[..found];
        PBylineItemMark = name.Substring(found, size);
        PBylineItemTail = name[(found + size)..];
    }

    internal long PBylineItemId { get; }

    public string PBylineItemName { get; }

    public string PBylineItemLead { get; }

    public string PBylineItemMark { get; }

    public string PBylineItemTail { get; }
}
