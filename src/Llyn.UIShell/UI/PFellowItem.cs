using System.Globalization;

namespace Llyn.UIShell;

internal sealed class PFellowItem
{
    internal PFellowItem(long id, string name, int shared)
    {
        PFellowItemId = id;
        PFellowItemName = name;
        PFellowItemCount = shared.ToString(CultureInfo.CurrentCulture);
    }

    public long PFellowItemId { get; }

    public string PFellowItemName { get; }

    public string PFellowItemCount { get; }
}
