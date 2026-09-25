using System;
using System.ComponentModel;
using System.Windows.Media;
using Llyn.Core;

namespace Llyn.UIDeportment;

public sealed class LAccentItem : INotifyPropertyChanged
{
    private readonly string _lAccentItemName;
    private ImageSource? _lAccentItemFlag;
    private string _lAccentItemText;
    private string _lAccentItemAudio;

    public LAccentItem(
        long id, string variety, string name, ImageSource? flag, string text, string audio, LRespellingMark respelling)
    {
        ArgumentNullException.ThrowIfNull(respelling);

        LAccentItemId = id;
        LAccentItemVariety = variety;
        _lAccentItemName = name;
        _lAccentItemFlag = flag;
        _lAccentItemText = text;
        _lAccentItemAudio = audio;
        LAccentItemOpener = respelling.LRespellingMarkOpener;
        LAccentItemCloser = respelling.LRespellingMarkCloser;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public long LAccentItemId { get; }

    public string LAccentItemVariety { get; }

    public string LAccentItemLabel => _lAccentItemFlag is null ? _lAccentItemName : string.Empty;

    public ImageSource? LAccentItemFlag
    {
        get => _lAccentItemFlag;
        private set
        {
            if (ReferenceEquals(_lAccentItemFlag, value))
            {
                return;
            }

            _lAccentItemFlag = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LAccentItemFlag)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LAccentItemLabel)));
        }
    }

    public string LAccentItemOpener { get; }

    public string LAccentItemCloser { get; }

    public string LAccentItemText
    {
        get => _lAccentItemText;
        set
        {
            string text = value ?? string.Empty;
            if (string.Equals(_lAccentItemText, text, StringComparison.Ordinal))
            {
                return;
            }

            _lAccentItemText = text;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LAccentItemText)));
        }
    }

    public string LAccentItemAudio
    {
        get => _lAccentItemAudio;
        set
        {
            string path = value ?? string.Empty;
            if (string.Equals(_lAccentItemAudio, path, StringComparison.Ordinal))
            {
                return;
            }

            _lAccentItemAudio = path;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LAccentItemAudio)));
        }
    }

    public static LAccentItem LAccentItemCreate(
        string language,
        bool flagged,
        LPronunciationDraft spoken,
        LRespellingMark respelling,
        Func<string, ImageSource?> flagSeam)
    {
        ArgumentNullException.ThrowIfNull(spoken);
        ArgumentNullException.ThrowIfNull(respelling);

        string variety = spoken.LPronunciationDraftVariety;
        return new LAccentItem(
            spoken.LPronunciationDraftId,
            variety,
            LAccentLabelFormat(variety),
            LAccentFlagFind(language, flagged, variety, flagSeam),
            respelling.LRespellingMarkResolve(spoken),
            spoken.LPronunciationDraftAudio,
            respelling);
    }

    public static string LAccentLabelFormat(string variety)
    {
        return variety.Length == 0
            ? string.Empty
            : LLocalizationCatalog.LLocalizationTextFind(string.Concat("Variety.", variety)) ?? variety;
    }

    public static ImageSource? LAccentFlagFind(
        string language, bool flagged, string variety, Func<string, ImageSource?> flagSeam)
    {
        ArgumentNullException.ThrowIfNull(flagSeam);

        return flagged && variety.Length > 0
            ? flagSeam(LAccentVarietyFormat(language, variety))
            : null;
    }

    public void LAccentFlagUpdate(string language, bool flagged, Func<string, ImageSource?> flagSeam)
    {
        LAccentItemFlag = LAccentFlagFind(language, flagged, LAccentItemVariety, flagSeam);
    }

    public static string LAccentVarietyFormat(string language, string variety)
    {
        return string.Concat(language, "/", variety);
    }
}
