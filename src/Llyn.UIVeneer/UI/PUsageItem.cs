using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PUsageItem
{
    internal PUsageItem(LUsage usage, string owner, string unknown, string unnamed, string epithet = "")
    {
        PUsageItemId = usage.LUsageId;
        PUsageItemEntry = usage.LUsageEntry;
        PUsageItemHeadword = usage.LUsageHeadword;
        PUsageItemName = usage.LUsageName;
        PUsageItemEpithet = epithet ?? string.Empty;
        PUsageItemLanguage = usage.LUsageLanguage;
        PUsageItemOwner = owner;
        PUsageItemQuoted = usage.LUsageQuoted;
        PUsageItemTitle = usage.LUsageTitle.LStateValueUncertain
            ? unknown
            : usage.LUsageTitle.LStateValueShown ?? unnamed;
        PUsageItemFlag = PEnsign.PEnsignFind(usage.LUsageLanguage);
    }

    public long PUsageItemId { get; }

    public long PUsageItemEntry { get; }

    public string PUsageItemHeadword { get; }

    public string PUsageItemEpithet { get; }

    public string PUsageItemName { get; }

    public string PUsageItemLanguage { get; }

    public string PUsageItemOwner { get; }

    public bool PUsageItemQuoted { get; }

    public string PUsageItemTitle { get; }

    public ImageSource? PUsageItemFlag { get; }
}
