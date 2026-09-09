using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PUsageItem
{
    internal PUsageItem(LUsage usage, string owner, string unreadable, string unnamed)
    {
        PUsageItemId = usage.LUsageId;
        PUsageItemEntry = usage.LUsageEntry;
        PUsageItemHeadword = usage.LUsageHeadword;
        PUsageItemName = usage.LUsageHeadword;
        PUsageItemLanguage = usage.LUsageLanguage;
        PUsageItemOwner = owner;
        PUsageItemTitle = usage.LUsageTitle.LStateValueState switch
        {
            LState.LStateSpecified => usage.LUsageTitle.LStateValueShow(),
            LState.LStateUnknown => unreadable,
            _ => unnamed,
        };
        PUsageItemFlag = PEnsign.PEnsignFind(usage.LUsageLanguage);
    }

    public string PUsageItemId { get; }

    public string PUsageItemEntry { get; }

    public string PUsageItemHeadword { get; }

    public string PUsageItemName { get; internal set; }

    public string PUsageItemLanguage { get; }

    public string PUsageItemOwner { get; }

    public string PUsageItemTitle { get; }

    public ImageSource? PUsageItemFlag { get; }
}
