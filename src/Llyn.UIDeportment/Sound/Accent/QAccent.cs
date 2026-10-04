using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QAccent
{
    private readonly FrameworkElement _qAccentSurface;

    private readonly QNotation _qNotation;

    private readonly QClip _qAccentClip;

    private readonly MediaPlayer _qAccentPlayer;

    private readonly ObservableCollection<QAccentItem> _qAccentItem = [];

    private CEditor _cEditor = null!;

    internal QAccent(FrameworkElement surface, QNotation notation, QClip menu, MediaPlayer player)
    {
        _qAccentSurface = surface;
        _qNotation = notation;
        _qAccentClip = menu;
        _qAccentPlayer = player;
        QAccentList.ItemsSource = _qAccentItem;
        QLookItem.QLookItemAttach(QAccentList, QAccentItem.QAccentItemRefine);
        QQuill.QQuillIntroduce(QAccentList);
        QAccentControl.QAccentControlAttach(QAccentList);
        StackPanel sound = QContract.QContractFind<StackPanel>(_qAccentSurface, "PEditorSound");
        CommandBinding clip = new(QAccentCommand.QAccentCommandClip);
        clip.Executed += QAccentClipRefine;
        clip.Executed += QAccentClipObserve;
        sound.CommandBindings.Add(new CommandBinding(
            QAccentCommand.QAccentCommandAddition, QAccentAddObserve));
        sound.CommandBindings.Add(new CommandBinding(
            QAccentCommand.QAccentCommandRemoval, QAccentRemoveObserve));
        CommandBinding command = new(QAccentCommand.QAccentCommandNotation);
        command.Executed += QAccentNotationRefine;
        command.Executed += QAccentNotationObserve;
        sound.CommandBindings.Add(command);
        sound.CommandBindings.Add(clip);
        sound.CommandBindings.Add(new CommandBinding(
            QAccentCommand.QAccentCommandPlayback, QAccentPlaybackObserve));
    }

    private ItemsControl QAccentList => QContract.QContractFind<ItemsControl>(_qAccentSurface, "PAccent");

    private Image QAccentFlag => QContract.QContractFind<Image>(_qAccentSurface, "PPronunciationFlag");

    private TextBlock QAccentLabel => QContract.QContractFind<TextBlock>(_qAccentSurface, "PPronunciationLabel");

    internal void QAccentIntroduce(CEditor editor)
    {
        _cEditor = editor;
        editor.CEditorDraftChanged += QAccentRefine;
        editor.CEditorDraftChanged += QAccentEnsignRefine;
    }

    private void QAccentAddObserve(object sender, ExecutedRoutedEventArgs e)
    {
        _cEditor.CEditorTimbre.CTimbrePronunciationAdd((e.Parameter as QAccentItem)?.QAccentItemId);
    }

    private void QAccentRemoveObserve(object sender, ExecutedRoutedEventArgs e)
    {
        _cEditor.CEditorTimbre.CTimbrePronunciationRemove((e.Parameter as QAccentItem)?.QAccentItemId);
    }

    private void QAccentNotationRefine(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QAccentItem)
        {
            _qNotation.QNotationOpenRefine(QAccentAnchorRead(e));
        }
    }

    private void QAccentNotationObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QAccentItem row)
        {
            _qNotation.QNotationStartRefine(_cEditor.CEditorDesk.CDeskErrand.CErrandTranscriptionStart(
                row.QAccentItemId, string.Empty));
        }
    }

    private void QAccentClipRefine(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QAccentItem)
        {
            _qAccentClip.QClipOpenRefine(QAccentAnchorRead(e));
        }
    }

    private void QAccentClipObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QAccentItem row)
        {
            _qAccentClip.QClipRecordingStart(row.QAccentItemId);
        }
    }

    private void QAccentPlaybackObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QAccentItem row)
        {
            QAccentPlaybackRefine(_cEditor.CEditorPlayback.CPlaybackAccentStart(row.QAccentItemId));
        }
    }

    private void QAccentPlaybackRefine(Uri? address)
    {
        if (address is not null)
        {
            _qAccentPlayer.Open(address);
            _qAccentPlayer.Play();
        }
    }

    private UIElement QAccentAnchorRead(ExecutedRoutedEventArgs e)
    {
        return e.OriginalSource as UIElement ?? QAccentList;
    }

    private void QAccentTypeObserve(QAccentItem row, string text)
    {
        row.QAccentTypeRefine(_cEditor.CEditorTimbre.CTimbreAccentSet(row.QAccentItemId, text));
    }

    private void QAccentRefine(CEntryDraft _)
    {
        QAccentList.Visibility = QLook.QLookVisibleRead(_cEditor.CEditorTimbre.CTimbreSpoken);
        QAccentRefine(_cEditor.CEditorTimbre.CTimbreAccentRead());
    }

    private void QAccentRefine(CTimbreAccent accent)
    {
        PCard.PCardRowShow(
            _qAccentItem,
            accent.CTimbreAccentRows,
            static row => row.QAccentItemId,
            static spoken => spoken.CAccentId,
            spoken => QAccentRowRefine(spoken, accent),
            (row, spoken) => QAccentRowRefine(row, spoken, accent));

        QAccentPrimaryRefine(accent);
    }

    private QAccentItem QAccentRowRefine(CAccent spoken, CTimbreAccent accent)
    {
        QAccentItem row = QAccentItem.QAccentItemBuild(
            spoken, accent.CTimbreAccentFlagged, accent.CTimbreAccentMark);
        row.QAccentItemTyped += QAccentTypeObserve;
        return row;
    }

    private QAccentItem QAccentRowRefine(QAccentItem row, CAccent spoken, CTimbreAccent accent)
    {
        CRespellingMark mark = accent.CTimbreAccentMark;
        if (!string.Equals(spoken.CAccentVariety.CVarietyName, row.QAccentItemVariety, StringComparison.Ordinal)
            || row.QAccentItemRespelled != mark.CRespellingMarkShown
            || !string.Equals(row.QAccentItemOpener, mark.CRespellingMarkOpener, StringComparison.Ordinal)
            || !string.Equals(row.QAccentItemCloser, mark.CRespellingMarkCloser, StringComparison.Ordinal))
        {
            row.QAccentItemTyped -= QAccentTypeObserve;
            return QAccentRowRefine(spoken, accent);
        }

        row.QAccentStateRefine(spoken);
        return row;
    }

    private void QAccentPrimaryRefine(CTimbreAccent accent)
    {
        QAccentFlag.Source =
            QAccentItem.QAccentEnsignRefine(accent.CTimbreAccentPrimary, accent.CTimbreAccentFlagged);
        QAccentLabel.Text = QAccentFlag.Source is null
            ? QAccentItem.QAccentLabelRefine(accent.CTimbreAccentPrimary)
            : string.Empty;
    }

    private async void QAccentEnsignRefine(CEntryDraft _)
    {
        QAccentFlagRefine(await _cEditor.CEditorTimbre.CTimbreFlagRead(QEnsignImage.QEnsignDraw));
    }

    private void QAccentFlagRefine(CTimbreAccent? accent)
    {
        if (accent is null)
        {
            return;
        }

        foreach (QAccentItem row in _qAccentItem)
        {
            row.QAccentFlagRefine(accent.CTimbreAccentFlagged);
        }

        QAccentPrimaryRefine(accent);
    }
}
