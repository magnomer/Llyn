using System.Globalization;

namespace Llyn.UIShell;

internal sealed class PGamutItem
{
    internal PGamutItem(string id, string name, string language, int usage, bool shipped, bool chosen)
    {
        PGamutItemId = id;
        PGamutItemName = name;
        PGamutItemLanguage = language;
        PGamutItemUsage = usage;
        PGamutItemShipped = shipped;
        PGamutItemChosen = chosen;
    }

    public string PGamutItemId { get; }

    public string PGamutItemName { get; }

    public string PGamutItemLanguage { get; }

    public int PGamutItemUsage { get; }

    public bool PGamutItemShipped { get; }

    public bool PGamutItemChosen { get; }

    public string PGamutItemCount => PGamutItemUsage > 0
        ? PGamutItemUsage.ToString(CultureInfo.CurrentCulture)
        : string.Empty;
}
