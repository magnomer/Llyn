using System;
using System.ComponentModel;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIShell;

internal sealed class PAccentItem : INotifyPropertyChanged
{
    private readonly string _pAccentItemName;
    private ImageSource? _pAccentItemFlag;
    private string _pAccentItemIpa;
    private string _pAccentItemAudio;

    internal PAccentItem(long id, string variety, string name, ImageSource? flag, string ipa, string audio)
    {
        PAccentItemId = id;
        PAccentItemVariety = variety;
        _pAccentItemName = name;
        _pAccentItemFlag = flag;
        _pAccentItemIpa = ipa;
        _pAccentItemAudio = audio;
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

    public string PAccentItemIpa
    {
        get => _pAccentItemIpa;
        set
        {
            string text = value ?? string.Empty;
            if (string.Equals(_pAccentItemIpa, text, StringComparison.Ordinal))
            {
                return;
            }

            _pAccentItemIpa = text;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PAccentItemIpa)));
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
        PWindow host, string language, bool flagged, LPronunciationDraft spoken)
    {
        ArgumentNullException.ThrowIfNull(spoken);

        string variety = spoken.LPronunciationDraftVariety;
        return new PAccentItem(
            spoken.LPronunciationDraftId,
            variety,
            PAccentLabelFormat(host, variety),
            PAccentFlagFind(language, flagged, variety),
            spoken.LPronunciationDraftIpa,
            spoken.LPronunciationDraftAudio);
    }

    internal static string PAccentLabelFormat(PWindow host, string variety)
    {
        ArgumentNullException.ThrowIfNull(host);

        return variety.Length == 0
            ? string.Empty
            : host.PLocalizationTextFind(string.Concat("Variety.", variety)) ?? variety;
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
