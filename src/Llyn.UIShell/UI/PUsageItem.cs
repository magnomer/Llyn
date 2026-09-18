using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIShell;

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
        PUsageItemKind = usage.LUsageOwner;
        PUsageItemTitle = PStateConverter.PStateConverterCheck(usage.LUsageTitle)
            ? unknown
            : usage.LUsageTitle.LStateValueShow() is { Length: > 0 } shown ? shown : unnamed;
        PUsageItemFlag = PEnsign.PEnsignFind(usage.LUsageLanguage);
    }

    public long PUsageItemId { get; }

    public long PUsageItemEntry { get; }

    public string PUsageItemHeadword { get; }

    public string PUsageItemEpithet { get; }

    public string PUsageItemName { get; }

    public string PUsageItemLanguage { get; }

    public string PUsageItemOwner { get; }

    public LOwner PUsageItemKind { get; }

    public string PUsageItemTitle { get; }

    public ImageSource? PUsageItemFlag { get; }
}
