using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
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

    public bool LAccentItemPlayable => _lAccentItemAudio.Length > 0;

    internal static void LAccentItemApply(FrameworkElement container, object item, string? _)
    {
        if (item is not LAccentItem row)
        {
            return;
        }

        if (PLook.PLookPartFind<Image>(container, "PAccentFlag") is Image flag)
        {
            flag.Source = row.LAccentItemFlag;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PAccentLabel") is TextBlock label)
        {
            label.Text = row.LAccentItemLabel;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PAccentOpener") is TextBlock opener)
        {
            opener.Text = row.LAccentItemOpener;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PAccentCloser") is TextBlock closer)
        {
            closer.Text = row.LAccentItemCloser;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PAccentText") is TextBlock text)
        {
            text.Text = row.LAccentItemText;
        }

        if (PLook.PLookPartFind<TextBlock>(container, "PAccentPrompt") is TextBlock prompt)
        {
            PLook.PLookPromptApply(prompt, row.LAccentItemText, "Input.Pronunciation", null);
            if (prompt.Parent is Grid cell)
            {
                cell.Tag = "Theme.Pronunciation.Field LAccentItemText Input.Pronunciation";
            }

            if (PLook.PLookPartFind<ContentControl>(container, "PAccentShelf") is ContentControl shelf)
            {
                shelf.Tag = "Theme.Accent.Slot";
            }
        }

        if (PLook.PLookPartFind<Button>(container, "PAccentPlayback") is Button playback)
        {
            playback.Visibility = PLook.PLookVisibleRead(row.LAccentItemPlayable);
        }
    }

    public static LAccentItem LAccentItemCreate(
        string language,
        bool flagged,
        LPronunciationDraft spoken,
        LRespellingMark respelling)
    {
        ArgumentNullException.ThrowIfNull(spoken);
        ArgumentNullException.ThrowIfNull(respelling);

        string variety = spoken.LPronunciationDraftVariety;
        return new LAccentItem(
            spoken.LPronunciationDraftId,
            variety,
            LAccentLabelFormat(variety),
            LAccentFlagFind(language, flagged, variety),
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

    public static ImageSource? LAccentFlagFind(string language, bool flagged, string variety)
    {
        ArgumentNullException.ThrowIfNull(variety);

        return flagged && variety.Length > 0
            ? LEnsignImage.LEnsignFind(LAccentVarietyFormat(language, variety))
            : null;
    }

    public void LAccentFlagUpdate(string language, bool flagged)
    {
        LAccentItemFlag = LAccentFlagFind(language, flagged, LAccentItemVariety);
    }

    public static string LAccentVarietyFormat(string language, string variety)
    {
        return string.Concat(language, "/", variety);
    }
}
