using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor : LReceiver
{
    private readonly ObservableCollection<PNotationItem> _pNotationItem = [];
    private CancellationTokenSource? _pNotationCancellation;
    private bool _pNotationSearching;

    private async void PTranscriberCheckedHandle(object sender, RoutedEventArgs e)
    {
        await PNotationStart();
    }

    private void PTranscriberUncheckedHandle(object sender, RoutedEventArgs e)
    {
        PNotationCancel();
    }

    internal void PNotationSelectorHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PNotationItem candidate })
        {
            return;
        }

        if (!candidate.PNotationItemReady)
        {
            return;
        }

        PPronunciation.Text = candidate.PNotationItemReading;
        PTranscriber.IsChecked = false;
    }

    private async Task PNotationStart()
    {
        PNotationCancel();

        string word = PHeadword.Text?.Trim() ?? string.Empty;
        _pNotationItem.Clear();
        _pNotationSearching = word.Length > 0;
        PNotationUpdate();

        if (word.Length == 0)
        {
            return;
        }

        _pNotationCancellation = new CancellationTokenSource();

        try
        {
            await _lEngine.LEnginePronunciationFind(
                _pEditorDraft, word, _pSpeakerChoice, this, _pNotationCancellation.Token);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception)
        {
            _pNotationSearching = false;
            PNotationUpdate();
        }
    }

    private void PNotationCancel()
    {
        _pNotationCancellation?.Cancel();
        _pNotationCancellation?.Dispose();
        _pNotationCancellation = null;
    }

    private PNotationItem PNotationPlace(string source, int order)
    {
        int position = 0;
        while (position < _pNotationItem.Count &&
            _pNotationItem[position].PNotationItemOrder < order)
        {
            position++;
        }

        if (position < _pNotationItem.Count && _pNotationItem[position].PNotationItemOrder == order)
        {
            return _pNotationItem[position];
        }

        PNotationItem row = new(source, order, _pEditorHost.PLocalizationTextRead("Transcriber.Searching"));
        _pNotationItem.Insert(position, row);
        return row;
    }

    private void PNotationUpdate()
    {
        bool candidates = _pNotationItem.Count > 0;

        PNotationList.Visibility = candidates ? Visibility.Visible : Visibility.Collapsed;
        PNotationProgress.Visibility = _pNotationSearching ? Visibility.Visible : Visibility.Collapsed;

        if (candidates)
        {
            PNotationNotice.Visibility = Visibility.Collapsed;
            return;
        }

        PNotationNotice.Text = _pEditorHost.PLocalizationTextRead(
            _pNotationSearching ? "Transcriber.Searching" : "Transcriber.Empty");
        PNotationNotice.Visibility = Visibility.Visible;
    }

    void LReceiver.LReceiverSourceStart(string source, int order)
    {
        Dispatcher.BeginInvoke(() =>
        {
            PNotationPlace(source, order);
            PNotationUpdate();
        });
    }

    void LReceiver.LReceiverCandidateAdd(LCandidate candidate)
    {
        Dispatcher.BeginInvoke(() =>
        {
            PNotationPlace(candidate.LCandidateSource, candidate.LCandidateOrder).PNotationItemShow(
                candidate,
                _pEditorHost.PLocalizationTextRead("Transcriber.Missing"),
                _pEditorHost.PLocalizationTextRead("Transcriber.Broken"));
            PNotationUpdate();
        });
    }

    void LReceiver.LReceiverLookupFinish()
    {
        Dispatcher.BeginInvoke(() =>
        {
            _pNotationSearching = false;
            PNotationUpdate();
        });
    }
}
