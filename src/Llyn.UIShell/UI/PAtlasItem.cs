using System.ComponentModel;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PAtlasItem : INotifyPropertyChanged
{
    private bool _pAtlasItemChosen;

    internal PAtlasItem(LSituation situation, int usage, string unknown, string untitled)
    {
        PAtlasItemId = situation.LSituationId;
        PAtlasItemTitle = PAtlasTextRead(situation.LSituationTitle, unknown) ?? untitled;
        PAtlasItemKind = PAtlasTextRead(situation.LSituationKind, unknown) ?? string.Empty;
        PAtlasItemCount = usage.ToString(System.Globalization.CultureInfo.CurrentCulture);
    }

    public long PAtlasItemId { get; }

    public string PAtlasItemTitle { get; }

    public string PAtlasItemKind { get; }

    public string PAtlasItemCount { get; }

    private static string? PAtlasTextRead(LStateValue value, string unknown)
    {
        return PStateConverter.PStateConverterCheck(value)
            ? unknown
            : value.LStateValueShow() is { Length: > 0 } shown ? shown : null;
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
