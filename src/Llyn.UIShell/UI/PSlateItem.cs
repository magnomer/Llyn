using System.Globalization;

namespace Llyn.UIShell;

internal sealed class PSlateItem
{
    internal PSlateItem(string text, string word)
    {
        PSlateItemText = text;

        int found = -1;
        int size = 0;
        if (word.Length != 0)
        {
            found = CultureInfo.CurrentCulture.CompareInfo.IndexOf(
                text, word, CompareOptions.IgnoreCase, out size);
        }

        if (found < 0)
        {
            PSlateItemLead = text;
            PSlateItemMark = string.Empty;
            PSlateItemTail = string.Empty;
            return;
        }

        PSlateItemLead = text[..found];
        PSlateItemMark = text.Substring(found, size);
        PSlateItemTail = text[(found + size)..];
    }

    public string PSlateItemText { get; }

    public string PSlateItemLead { get; }

    public string PSlateItemMark { get; }

    public string PSlateItemTail { get; }
}
