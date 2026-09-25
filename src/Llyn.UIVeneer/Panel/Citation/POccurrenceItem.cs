using System;
using System.ComponentModel;
using System.Windows.Media;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

internal sealed class POccurrenceItem : INotifyPropertyChanged
{
    private bool _pOccurrenceItemChosen;

    internal POccurrenceItem(long id, string headword, string language, string epithet = "", bool chosen = false)
    {
        _pOccurrenceItemChosen = chosen;
        POccurrenceItemId = id;
        POccurrenceItemHeadword = headword;
        POccurrenceItemEpithet = epithet ?? string.Empty;
        POccurrenceItemLanguage = language;
        POccurrenceItemFlag = LEnsignImage.LEnsignFind(language);
    }

    public long POccurrenceItemId { get; }

    public string POccurrenceItemHeadword { get; }

    public string POccurrenceItemEpithet { get; }

    public required string POccurrenceItemName { get; init; }

    public string POccurrenceItemLanguage { get; }

    public ImageSource? POccurrenceItemFlag { get; }

    internal static bool POccurrenceItemMatch(POccurrenceItem held, POccurrenceItem fresh)
    {
        return held.POccurrenceItemId == fresh.POccurrenceItemId
            && string.Equals(held.POccurrenceItemHeadword, fresh.POccurrenceItemHeadword, StringComparison.Ordinal)
            && string.Equals(held.POccurrenceItemEpithet, fresh.POccurrenceItemEpithet, StringComparison.Ordinal)
            && string.Equals(held.POccurrenceItemName, fresh.POccurrenceItemName, StringComparison.Ordinal)
            && string.Equals(held.POccurrenceItemLanguage, fresh.POccurrenceItemLanguage, StringComparison.Ordinal);
    }

    internal static void POccurrenceItemSync(POccurrenceItem held, POccurrenceItem fresh)
    {
        held.POccurrenceItemChosen = fresh.POccurrenceItemChosen;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool POccurrenceItemChosen
    {
        get => _pOccurrenceItemChosen;

        set
        {
            if (_pOccurrenceItemChosen == value)
            {
                return;
            }

            _pOccurrenceItemChosen = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(POccurrenceItemChosen)));
        }
    }
}
