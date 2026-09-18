using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Llyn.UIShell;

internal sealed class PFootnoteItem : INotifyPropertyChanged
{
    private bool _pFootnoteItemChosen;

    internal PFootnoteItem(long id, string headword, string language, string epithet = "", bool chosen = false)
    {
        _pFootnoteItemChosen = chosen;
        PFootnoteItemId = id;
        PFootnoteItemHeadword = headword;
        PFootnoteItemEpithet = epithet ?? string.Empty;
        PFootnoteItemLanguage = language;
        PFootnoteItemFlag = PEnsign.PEnsignFind(language);
    }

    public long PFootnoteItemId { get; }

    public string PFootnoteItemHeadword { get; }

    public string PFootnoteItemEpithet { get; }

    public required string PFootnoteItemName { get; init; }

    public string PFootnoteItemLanguage { get; }

    public ImageSource? PFootnoteItemFlag { get; }

    internal static bool PFootnoteItemMatch(PFootnoteItem held, PFootnoteItem fresh)
    {
        return held.PFootnoteItemId == fresh.PFootnoteItemId
            && string.Equals(held.PFootnoteItemHeadword, fresh.PFootnoteItemHeadword, StringComparison.Ordinal)
            && string.Equals(held.PFootnoteItemEpithet, fresh.PFootnoteItemEpithet, StringComparison.Ordinal)
            && string.Equals(held.PFootnoteItemName, fresh.PFootnoteItemName, StringComparison.Ordinal)
            && string.Equals(held.PFootnoteItemLanguage, fresh.PFootnoteItemLanguage, StringComparison.Ordinal);
    }

    internal static void PFootnoteItemSync(PFootnoteItem held, PFootnoteItem fresh)
    {
        held.PFootnoteItemChosen = fresh.PFootnoteItemChosen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool PFootnoteItemChosen
    {
        get => _pFootnoteItemChosen;

        set
        {
            if (_pFootnoteItemChosen == value)
            {
                return;
            }

            _pFootnoteItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PFootnoteItemChosen)));
        }
    }
}
