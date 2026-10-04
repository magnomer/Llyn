using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QAccentItem : INotifyPropertyChanged
{
    private readonly string _qAccentItemName;
    private readonly string _qAccentItemEnsign;
    private ImageSource? _qAccentItemFlag;
    private string _qAccentItemText;
    private string _qAccentItemAudio;

    private QAccentItem(
        long id,
        string variety,
        string name,
        string ensign,
        bool flagged,
        string text,
        string audio,
        CRespellingMark respelling)
    {
        ArgumentNullException.ThrowIfNull(respelling);

        QAccentItemId = id;
        QAccentItemVariety = variety;
        _qAccentItemName = name;
        _qAccentItemEnsign = ensign;
        _qAccentItemFlag = flagged ? QEnsignImage.QEnsignRead(ensign) : null;
        _qAccentItemText = text;
        _qAccentItemAudio = audio;
        QAccentItemRespelled = respelling.CRespellingMarkShown;
        QAccentItemOpener = respelling.CRespellingMarkOpener;
        QAccentItemCloser = respelling.CRespellingMarkCloser;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    internal event Action<QAccentItem, string>? QAccentItemTyped;

    public long QAccentItemId { get; }

    public string QAccentItemVariety { get; }

    public string QAccentItemLabel => _qAccentItemFlag is null ? _qAccentItemName : string.Empty;

    public ImageSource? QAccentItemFlag => _qAccentItemFlag;

    public bool QAccentItemRespelled { get; }

    public string QAccentItemOpener { get; }

    public string QAccentItemCloser { get; }

    public string QAccentItemText
    {
        get => _qAccentItemText;
        set => QAccentItemTyped?.Invoke(this, value);
    }

    public string QAccentItemAudio => _qAccentItemAudio;

    public bool QAccentItemPlayable => _qAccentItemAudio.Length > 0;

    internal static void QAccentItemRefine(FrameworkElement container, object item, string? _)
    {
        if (item is not QAccentItem row)
        {
            return;
        }

        if (QLook.QLookPartFind<Image>(container, "PAccentFlag") is Image flag)
        {
            flag.Source = row.QAccentItemFlag;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PAccentLabel") is TextBlock label)
        {
            label.Text = row.QAccentItemLabel;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PAccentOpener") is TextBlock opener)
        {
            opener.Text = row.QAccentItemOpener;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PAccentCloser") is TextBlock closer)
        {
            closer.Text = row.QAccentItemCloser;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PAccentText") is TextBlock text)
        {
            text.Text = row.QAccentItemText;
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PAccentPrompt") is TextBlock prompt)
        {
            QLook.QLookPromptApply(prompt, row.QAccentItemText, "Input.Pronunciation", null);
            if (prompt.Parent is Grid cell)
            {
                cell.SetValue(
                    QQuill.QQuillCellProperty,
                    new QQuill.QQuillCell("Theme.Pronunciation.Field", "QAccentItemText", "Input.Pronunciation"));
            }

            if (QLook.QLookPartFind<ContentControl>(container, "PAccentShelf") is ContentControl shelf)
            {
                shelf.Tag = "Theme.Accent.Slot";
            }
        }

        if (QLook.QLookPartFind<Button>(container, "PAccentPlayback") is Button playback)
        {
            playback.Visibility = QLook.QLookVisibleRead(row.QAccentItemPlayable);
        }
    }

    internal static QAccentItem QAccentItemBuild(CAccent accent, bool flagged, CRespellingMark respelling)
    {
        ArgumentNullException.ThrowIfNull(accent);

        return new QAccentItem(
            accent.CAccentId,
            accent.CAccentVariety.CVarietyName,
            QAccentLabelRefine(accent.CAccentVariety),
            accent.CAccentVariety.CVarietyEnsign,
            flagged,
            accent.CAccentText,
            accent.CAccentAudio,
            respelling);
    }

    internal static string QAccentLabelRefine(CVariety variety)
    {
        ArgumentNullException.ThrowIfNull(variety);

        return QLocalizationCatalog.QLocalizationTextFind(variety.CVarietyKey) ?? variety.CVarietyName;
    }

    internal static ImageSource? QAccentEnsignRefine(CVariety variety, bool flagged)
    {
        ArgumentNullException.ThrowIfNull(variety);

        return flagged ? QEnsignImage.QEnsignRead(variety.CVarietyEnsign) : null;
    }

    internal void QAccentFlagRefine(bool flagged)
    {
        QAccentValueRefine(
            ref _qAccentItemFlag,
            flagged ? QEnsignImage.QEnsignRead(_qAccentItemEnsign) : null,
            nameof(QAccentItemFlag),
            nameof(QAccentItemLabel));
    }

    internal void QAccentStateRefine(CAccent spoken)
    {
        ArgumentNullException.ThrowIfNull(spoken);

        QAccentValueRefine(ref _qAccentItemText, spoken.CAccentText, nameof(QAccentItemText));
        QAccentValueRefine(ref _qAccentItemAudio, spoken.CAccentAudio, nameof(QAccentItemAudio));
    }

    internal void QAccentTypeRefine(CAccentTyped typed)
    {
        ArgumentNullException.ThrowIfNull(typed);

        QAccentValueRefine(ref _qAccentItemText, typed.CAccentTypedText, nameof(QAccentItemText));
    }

    private void QAccentValueRefine<QAccentValue>(
        ref QAccentValue field,
        QAccentValue value,
        params string[] names)
    {
        if (EqualityComparer<QAccentValue>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        foreach (string name in names)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
