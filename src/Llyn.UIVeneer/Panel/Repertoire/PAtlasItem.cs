using System;
using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PAtlasItem : INotifyPropertyChanged
{
    private bool _pAtlasItemChosen;

    internal PAtlasItem(LSituation situation, string name, int usage, string unknown, string untitled, bool chosen)
    {
        _pAtlasItemChosen = chosen;
        PAtlasItemId = situation.LSituationId;
        PAtlasItemTitle = name;
        PAtlasItemKind = PAtlasTextRead(situation.LSituationKind, unknown) ?? string.Empty;
        PAtlasItemCount = usage.ToString(System.Globalization.CultureInfo.CurrentCulture);
    }

    public long PAtlasItemId { get; }

    public string PAtlasItemTitle { get; }

    public string PAtlasItemKind { get; }

    public string PAtlasItemCount { get; }

    private static string? PAtlasTextRead(LStateValue value, string unknown)
    {
        return value.LStateValueUncertain ? unknown : value.LStateValueShown;
    }

    internal static bool PAtlasItemMatch(PAtlasItem held, PAtlasItem fresh)
    {
        return held.PAtlasItemId == fresh.PAtlasItemId
            && string.Equals(held.PAtlasItemTitle, fresh.PAtlasItemTitle, StringComparison.Ordinal)
            && string.Equals(held.PAtlasItemKind, fresh.PAtlasItemKind, StringComparison.Ordinal)
            && string.Equals(held.PAtlasItemCount, fresh.PAtlasItemCount, StringComparison.Ordinal);
    }

    internal static void PAtlasItemSync(PAtlasItem held, PAtlasItem fresh)
    {
        held.PAtlasItemChosen = fresh.PAtlasItemChosen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PAtlasItemChosen
    {
        get => _pAtlasItemChosen;

        set
        {
            if (_pAtlasItemChosen == value)
            {
                return;
            }

            _pAtlasItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PAtlasItemChosen)));
        }
    }
}
