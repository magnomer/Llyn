using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PAtlasItem
{
    internal PAtlasItem(LSituation situation, int usage, string unreadable, string untitled)
    {
        PAtlasItemId = situation.LSituationId;
        PAtlasItemTitle = PAtlasTextRead(situation.LSituationTitle, unreadable) ?? untitled;
        PAtlasItemKind = PAtlasTextRead(situation.LSituationKind, unreadable) ?? string.Empty;
        PAtlasItemCount = usage.ToString(System.Globalization.CultureInfo.CurrentCulture);
    }

    public string PAtlasItemId { get; }

    public string PAtlasItemTitle { get; }

    public string PAtlasItemKind { get; }

    public string PAtlasItemCount { get; }

    private static string? PAtlasTextRead(LStateValue value, string unreadable)
    {
        return value.LStateValueState switch
        {
            LState.LStateSpecified => value.LStateValueShow(),
            LState.LStateUnknown => unreadable,
            _ => null,
        };
    }
}
