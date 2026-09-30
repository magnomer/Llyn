using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Application;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly ObservableCollection<QAccentItem> _pAccentItem = [];
    private string _pAccentLanguage = string.Empty;
    private bool _pAccentFlagged;
    private string _pAccentPrimary = string.Empty;
    private CRespellingMark? _pAccentRespelling;

    private ItemsControl PAccent => (ItemsControl)FindName(nameof(PAccent));

    private Image PPronunciationFlag => (Image)FindName(nameof(PPronunciationFlag));

    private TextBlock PPronunciationLabel => (TextBlock)FindName(nameof(PPronunciationLabel));

    private void PAccentAttach()
    {
        PAccent.ItemsSource = _pAccentItem;
        QLookItem.QLookItemAttach(PAccent, QAccentItem.QAccentItemRefine);
        QField.QFieldCellAttach(PAccent);
        PAccentControl.PAccentControlAttach(PAccent);
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PAccentCommand.PAccentCommandAddition, PAccentAddHandle));
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PAccentCommand.PAccentCommandRemoval, PAccentRemoveHandle));
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PAccentCommand.PAccentCommandNotation, PAccentNotationHandle));
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PAccentCommand.PAccentCommandClip, PAccentClipHandle));
        PEditorSound.CommandBindings.Add(new CommandBinding(
            PAccentCommand.PAccentCommandPlayback, PAccentPlaybackHandle));
    }

    private LRequest PAccentRequestCreate(QAccentItem row)
    {
        return row.QAccentItemRespelled
            ? new LRequestPronunciationRespelling(PEditorDraft, row.QAccentItemId, row.QAccentItemText)
            : new LRequestPronunciationIpa(PEditorDraft, row.QAccentItemId, row.QAccentItemText);
    }

    internal void PAccentAddHandle(object sender, ExecutedRoutedEventArgs e)
    {
        int position = e.Parameter is QAccentItem row ? _pAccentItem.IndexOf(row) + 2 : 1;
        PEditorRequestSend(new LRequestPronunciationAddition(PEditorDraft, string.Empty, position));
    }

    internal void PAccentRemoveHandle(object sender, ExecutedRoutedEventArgs e)
    {
        PEditorRequestSend(
            new LRequestPronunciationRemoval(PEditorDraft, (e.Parameter as QAccentItem)?.QAccentItemId ?? 0));
    }

    internal async void PAccentNotationHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QAccentItem row)
        {
            await PNotationOpen(PAccentAnchorRead(e), row.QAccentItemId);
        }
    }

    internal void PAccentClipHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is QAccentItem row)
        {
            PClipOpenRefine(PAccentAnchorRead(e));
            PClipEnsignRefine(_qEditor.QEditorArea.CEditorDesk.CDeskErrand.CErrandRecordingStart(row.QAccentItemId));
        }
    }

    internal void PAccentPlaybackHandle(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Parameter is not QAccentItem row)
        {
            return;
        }

        if (!_pEditorHost.PWindowAtelier.CAtelierRecordingExist(row.QAccentItemAudio))
        {
            PEditorRequestSend(new LRequestPronunciationAudio(PEditorDraft, row.QAccentItemId, string.Empty, null));
            return;
        }

        _pDownloaderPlayer.Open(new Uri(row.QAccentItemAudio));
        _pDownloaderPlayer.Play();
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

    private void PAccentChangeHandle(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not QAccentItem row
            || !string.Equals(e.PropertyName, nameof(QAccentItem.QAccentItemText), StringComparison.Ordinal))
        {
            return;
        }

        PEditorRequestDefer(PAccentRequestCreate(row));
    }

    internal void PAccentShow(CEntryDraft draft)
    {
        string language = draft.CEntryDraftLanguage;
        bool flagged = _qEditor.QEditorArea.CEditorTimbre.CTimbreFlagged;
        _pAccentLanguage = language;
        _pAccentFlagged = flagged;
        _pAccentPrimary = draft.CEntryDraftPronunciation?.CPronunciationDraftVariety ?? string.Empty;
        CRespellingMark respelling =
            _pEditorHost.PWindowAtelier.CAtelierRespelling.CRespellingMarkRead(language);
        if (respelling != _pAccentRespelling)
        {
            _pAccentRespelling = respelling;
            PAccentRowClear();
        }

        PCard.PCardRowShow(
            _pAccentItem,
            draft.CEntryDraftAccents,
            static row => row.QAccentItemId,
            static spoken => spoken.CPronunciationDraftId,
            PAccentCreate,
            PAccentUpdate);

        PAccentPrimaryShow();
        _ = PAccentFlagLoad(language, flagged);
    }

    private QAccentItem PAccentCreate(CPronunciationDraft spoken)
    {
        QAccentItem row = QAccentItem.QAccentItemBuild(
            CRespelling.CRespellingAccentRead(_pAccentRespelling!, _pAccentLanguage, spoken),
            _pAccentFlagged,
            _pAccentRespelling!);
        row.PropertyChanged += PAccentChangeHandle;
        return row;
    }

    private QAccentItem PAccentUpdate(QAccentItem row, CPronunciationDraft spoken)
    {
        if (!string.Equals(spoken.CPronunciationDraftVariety, row.QAccentItemVariety, StringComparison.Ordinal))
        {
            row.PropertyChanged -= PAccentChangeHandle;
            return PAccentCreate(spoken);
        }

        row.QAccentItemText = CRespelling.CRespellingResolve(_pAccentRespelling!, spoken);

        row.QAccentItemAudio = spoken.CPronunciationDraftAudio;
        return row;
    }

    private void PAccentPrimaryShow()
    {
        CVariety primary = CSounding.CSoundingVarietyRead(_pAccentLanguage, _pAccentPrimary);
        PPronunciationFlag.Source = QAccentItem.QAccentEnsignRefine(primary, _pAccentFlagged);
        PPronunciationLabel.Text = PPronunciationFlag.Source is null
            ? QAccentItem.QAccentLabelRefine(primary)
            : string.Empty;
    }

    private async Task PAccentFlagLoad(string language, bool flagged)
    {
        if (!flagged)
        {
            return;
        }

        List<string> varieties = _pAccentItem.Select(static row => row.QAccentItemVariety).ToList();
        varieties.Add(_pAccentPrimary);

        try
        {
            await LEnsignImage.LEnsignVarietyLoad(
                _pEditorHost.PWindowAtelier, language, varieties.Where(static variety => variety.Length > 0));
        }
        catch (Exception)
        {
            return;
        }

        if (!string.Equals(_pAccentLanguage, language, StringComparison.Ordinal) || !_pAccentFlagged)
        {
            return;
        }

        foreach (QAccentItem row in _pAccentItem)
        {
            row.QAccentFlagRefine(flagged);
        }

        PAccentPrimaryShow();
    }

    private void PAccentRowClear()
    {
        foreach (QAccentItem row in _pAccentItem)
        {
            row.PropertyChanged -= PAccentChangeHandle;
        }

        _pAccentItem.Clear();
    }
}
