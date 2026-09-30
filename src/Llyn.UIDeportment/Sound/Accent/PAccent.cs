using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly ObservableCollection<QAccentItem> _pAccentItem = [];

    private ItemsControl PAccent => (ItemsControl)FindName(nameof(PAccent));

    private Image PPronunciationFlag => (Image)FindName(nameof(PPronunciationFlag));

    private TextBlock PPronunciationLabel => (TextBlock)FindName(nameof(PPronunciationLabel));

    private void PAccentAttach()
    {
        PAccent.ItemsSource = _pAccentItem;
        QLookItem.QLookItemAttach(PAccent, QAccentItem.QAccentItemRefine);
        QField.QFieldCellAttach(PAccent);
        PAccentControl.PAccentControlAttach(PAccent);
        CommandBinding clip = new(PAccentCommand.PAccentCommandClip);
        clip.Executed += PAccentClipRefine;
        clip.Executed += PAccentClipObserve;
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PAccentCommand.PAccentCommandAddition, PAccentAddObserve));
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PAccentCommand.PAccentCommandRemoval, PAccentRemoveObserve));
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PAccentCommand.PAccentCommandNotation, PAccentNotationObserve));
        PEditorSound.CommandBindings.Add(clip);
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PAccentCommand.PAccentCommandPlayback, PAccentPlaybackObserve));
    }

    private void PAccentAddObserve(object sender, ExecutedRoutedEventArgs e)
    {
        _qEditor.QEditorArea.CEditorTimbre.CTimbrePronunciationAdd((e.Parameter as QAccentItem)?.QAccentItemId ?? 0);
    }

    private void PAccentRemoveObserve(object sender, ExecutedRoutedEventArgs e)
    {
        _qEditor.QEditorArea.CEditorTimbre.CTimbrePronunciationRemove((e.Parameter as QAccentItem)?.QAccentItemId ?? 0);
    }

    private async void PAccentNotationObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QAccentItem row)
        {
            await PNotationOpen(PAccentAnchorRead(e), row.QAccentItemId);
        }
    }

    private void PAccentClipRefine(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QAccentItem)
        {
            PClipOpenRefine(PAccentAnchorRead(e));
        }
    }

    private void PAccentClipObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QAccentItem row)
        {
            PClipEnsignRefine(_qEditor.QEditorArea.CEditorDesk.CDeskErrand.CErrandRecordingStart(row.QAccentItemId));
        }
    }

    private void PAccentPlaybackObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QAccentItem row)
        {
            PAccentPlaybackRefine(_qEditor.QEditorArea.CEditorTimbre.CTimbreAudioStart(row.QAccentItemId));
        }
    }

    private void PAccentPlaybackRefine(Uri? address)
    {
        if (address is not null)
        {
            _pDownloaderPlayer.Open(address);
            _pDownloaderPlayer.Play();
        }
    }

    private UIElement PAccentAnchorRead(ExecutedRoutedEventArgs e)
    {
        return e.OriginalSource as UIElement ?? PAccent;
    }

    private QAccentItem? PAccentFind(long id)
    {
        foreach (QAccentItem row in _pAccentItem)
        {
            if (row.QAccentItemId == id)
            {
                return row;
            }
        }

        return null;
    }

    private void PAccentTextObserve(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not QAccentItem row
            || !string.Equals(e.PropertyName, nameof(QAccentItem.QAccentItemText), StringComparison.Ordinal))
        {
            return;
        }

        _qEditor.QEditorArea.CEditorTimbre.CTimbreAccentSet(row.QAccentItemId, row.QAccentItemText);
    }

    internal void PAccentRefine(CEntryDraft _)
    {
        PAccentRefine(_qEditor.QEditorArea.CEditorTimbre.CTimbreAccentRead());
        PAccentEnsignRefine();
    }

    private void PAccentRefine(CTimbreAccent accent)
    {
        PCard.PCardRowShow(
            _pAccentItem,
            accent.CTimbreAccentRows,
            static row => row.QAccentItemId,
            static spoken => spoken.CAccentId,
            spoken => PAccentRowRefine(spoken, accent),
            (row, spoken) => PAccentRowRefine(row, spoken, accent));

        PAccentPrimaryRefine(accent);
    }

    private QAccentItem PAccentRowRefine(CAccent spoken, CTimbreAccent accent)
    {
        QAccentItem row = QAccentItem.QAccentItemBuild(
            spoken, accent.CTimbreAccentFlagged, accent.CTimbreAccentMark);
        row.PropertyChanged += PAccentTextObserve;
        return row;
    }

    private QAccentItem PAccentRowRefine(QAccentItem row, CAccent spoken, CTimbreAccent accent)
    {
        CRespellingMark mark = accent.CTimbreAccentMark;
        if (!string.Equals(spoken.CAccentVariety.CVarietyName, row.QAccentItemVariety, StringComparison.Ordinal)
            || row.QAccentItemRespelled != mark.CRespellingMarkShown
            || !string.Equals(row.QAccentItemOpener, mark.CRespellingMarkOpener, StringComparison.Ordinal)
            || !string.Equals(row.QAccentItemCloser, mark.CRespellingMarkCloser, StringComparison.Ordinal))
        {
            row.PropertyChanged -= PAccentTextObserve;
            return PAccentRowRefine(spoken, accent);
        }

        row.QAccentItemText = spoken.CAccentText;
        row.QAccentItemAudio = spoken.CAccentAudio;
        return row;
    }

    private void PAccentPrimaryRefine(CTimbreAccent accent)
    {
        PPronunciationFlag.Source =
            QAccentItem.QAccentEnsignRefine(accent.CTimbreAccentPrimary, accent.CTimbreAccentFlagged);
        PPronunciationLabel.Text = PPronunciationFlag.Source is null
            ? QAccentItem.QAccentLabelRefine(accent.CTimbreAccentPrimary)
            : string.Empty;
    }

    private async void PAccentEnsignRefine()
    {
        PAccentFlagRefine(await LEnsignImage.LEnsignLoad(_qEditor.QEditorArea.CEditorTimbre.CTimbreFlagRead));
    }

    private void PAccentFlagRefine(CTimbreAccent? accent)
    {
        if (accent is null)
        {
            return;
        }

        foreach (QAccentItem row in _pAccentItem)
        {
            row.QAccentFlagRefine(accent.CTimbreAccentFlagged);
        }

        PAccentPrimaryRefine(accent);
    }
}
