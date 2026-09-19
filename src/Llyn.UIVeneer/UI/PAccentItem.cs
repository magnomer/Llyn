using System;
using System.ComponentModel;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIVeneer;

internal sealed class PAccentItem : INotifyPropertyChanged
{
    private readonly string _pAccentItemName;
    private ImageSource? _pAccentItemFlag;
    private string _pAccentItemText;
    private string _pAccentItemAudio;

    internal PAccentItem(
        long id, string variety, string name, ImageSource? flag, string text, string audio, PRespelling respelling)
    {
        PAccentItemId = id;
        PAccentItemVariety = variety;
        _pAccentItemName = name;
        _pAccentItemFlag = flag;
        _pAccentItemText = text;
        _pAccentItemAudio = audio;
        PAccentItemOpener = respelling.PRespellingOpener;
        PAccentItemCloser = respelling.PRespellingCloser;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public long PAccentItemId { get; }

    public string PAccentItemVariety { get; }

    public string PAccentItemLabel => _pAccentItemFlag is null ? _pAccentItemName : string.Empty;

    public ImageSource? PAccentItemFlag
    {
        get => _pAccentItemFlag;
        private set
        {
            if (ReferenceEquals(_pAccentItemFlag, value))
            {
                return;
            }

            _pAccentItemFlag = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PAccentItemFlag)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PAccentItemLabel)));
        }
    }

    public string PAccentItemOpener { get; }

    public string PAccentItemCloser { get; }

    public string PAccentItemText
    {
        get => _pAccentItemText;
        set
        {
            string text = value ?? string.Empty;
            if (string.Equals(_pAccentItemText, text, StringComparison.Ordinal))
            {
                return;
            }

            _pAccentItemText = text;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PAccentItemText)));
        }
    }

    public string PAccentItemAudio
    {
        get => _pAccentItemAudio;
        set
        {
            string path = value ?? string.Empty;
            if (string.Equals(_pAccentItemAudio, path, StringComparison.Ordinal))
            {
                return;
            }

            _pAccentItemAudio = path;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PAccentItemAudio)));
        }
    }

    internal static PAccentItem PAccentItemCreate(
        PWindow host, string language, bool flagged, LPronunciationDraft spoken, PRespelling respelling)
    {
        ArgumentNullException.ThrowIfNull(spoken);
        ArgumentNullException.ThrowIfNull(respelling);

        string variety = spoken.LPronunciationDraftVariety;
        return new PAccentItem(
            spoken.LPronunciationDraftId,
            variety,
            PAccentLabelFormat(host, variety),
            PAccentFlagFind(language, flagged, variety),
            respelling.PRespellingTextRead(spoken),
            spoken.LPronunciationDraftAudio,
            respelling);
    }

    internal static string PAccentLabelFormat(PWindow host, string variety)
    {
        ArgumentNullException.ThrowIfNull(host);

        return variety.Length == 0
            ? string.Empty
            : PLocalizationCatalog.PLocalizationTextFind(string.Concat("Variety.", variety)) ?? variety;
    }

    internal static ImageSource? PAccentFlagFind(string language, bool flagged, string variety)
    {
        return flagged && variety.Length > 0
            ? PEnsign.PEnsignFind(PEnsign.PEnsignVarietyFormat(language, variety))
            : null;
    }

    internal void PAccentFlagUpdate(string language, bool flagged)
    {
        PAccentItemFlag = PAccentFlagFind(language, flagged, PAccentItemVariety);
    }
}
