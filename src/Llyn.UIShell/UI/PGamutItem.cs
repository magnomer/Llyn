using System.Globalization;

namespace Llyn.UIShell;

internal sealed class PGamutItem
{
    internal PGamutItem(long id, string name, int usage, bool chosen)
    {
        PGamutItemId = id;
        PGamutItemName = name;
        PGamutItemUsage = usage;
        PGamutItemChosen = chosen;
    }

    public long PGamutItemId { get; }

    public string PGamutItemName { get; }

    public int PGamutItemUsage { get; }

    public bool PGamutItemChosen { get; }

    public string PGamutItemCount => PGamutItemUsage.ToString(CultureInfo.CurrentCulture);
}
