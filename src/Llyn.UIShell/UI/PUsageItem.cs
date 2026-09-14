using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PUsageItem
{
    internal PUsageItem(LUsage usage, string owner, string unknown, string unnamed)
    {
        PUsageItemId = usage.LUsageId;
        PUsageItemEntry = usage.LUsageEntry;
        PUsageItemHeadword = usage.LUsageHeadword;
        PUsageItemName = usage.LUsageHeadword;
        PUsageItemLanguage = usage.LUsageLanguage;
        PUsageItemOwner = owner;
        PUsageItemKind = usage.LUsageOwner;
        PUsageItemTitle = usage.LUsageTitle.LStateValueState switch
        {
            _ when usage.LUsageTitle.LStateValueUnreadable => usage.LUsageTitle.LStateValueShow(),
            LState.LStateSpecified => usage.LUsageTitle.LStateValueShow(),
            LState.LStateUnknown => unknown,
            _ => unnamed,
        };
        PUsageItemFlag = PEnsign.PEnsignFind(usage.LUsageLanguage);
    }

    public long PUsageItemId { get; }

    public long PUsageItemEntry { get; }

    public string PUsageItemHeadword { get; }

    public string PUsageItemName { get; internal set; }

    public string PUsageItemLanguage { get; }

    public string PUsageItemOwner { get; }

    public LOwner PUsageItemKind { get; }

    public string PUsageItemTitle { get; }

    public ImageSource? PUsageItemFlag { get; }
}
