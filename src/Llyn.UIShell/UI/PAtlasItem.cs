using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PAtlasItem : INotifyPropertyChanged
{
    private bool _pAtlasItemChosen;

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
