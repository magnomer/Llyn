using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PFootnoteItem
{
    internal PFootnoteItem(LUsage usage, string owner, string unreadable, string unnamed)
    {
        PFootnoteItemId = usage.LUsageId;
        PFootnoteItemOwner = usage.LUsageOwner;
        PFootnoteItemKind = owner;
        PFootnoteItemName = usage.LUsageHeadword.Length > 0 ? usage.LUsageHeadword : unnamed;
        PFootnoteItemLanguage = usage.LUsageLanguage;
        PFootnoteItemTitle = usage.LUsageTitle.LStateValueState switch
        {
            LState.LStateSpecified => usage.LUsageTitle.LStateValueShow(),
            LState.LStateUnknown => unreadable,
            _ => unnamed,
        };
        PFootnoteItemFlag = PEnsign.PEnsignFind(usage.LUsageLanguage);
    }

    public string PFootnoteItemId { get; }

    public LOwner PFootnoteItemOwner { get; }

    public string PFootnoteItemKind { get; }

    public string PFootnoteItemName { get; }

    public string PFootnoteItemLanguage { get; }

    public string PFootnoteItemTitle { get; }

    public ImageSource? PFootnoteItemFlag { get; }
}
