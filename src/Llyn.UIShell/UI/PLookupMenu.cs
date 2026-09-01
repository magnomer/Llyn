using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

/// <summary>
/// Pronunciation lookup as the input panel shows it: opening the menu starts a search for the
/// headword, candidates stream in from the engine and fill the list, and picking one writes it into
/// the pronunciation field. This is the shell side of <see cref="LReceiver"/> — the engine calls back
/// on a worker thread, so every arrival is marshalled onto the dispatcher here.
/// </summary>
public partial class PInput : LReceiver
{
    private readonly ObservableCollection<PLookupCandidate> _pLookupCandidate = [];
    private CancellationTokenSource? _lLookupCancellation;
    private bool _lLookupSearching;

    private async void PLookupCheckedHandle(object sender, RoutedEventArgs e)
    {
        await PLookupStart();
    }

    private void PLookupUncheckedHandle(object sender, RoutedEventArgs e)
    {
        PLookupCancel();
    }

    internal void PLookupMenuHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PLookupCandidate candidate })
        {
            return;
        }

        PPronunciation.Text = candidate.PLookupCandidatePhonetic;
        PLookup.IsChecked = false;
    }

    private async Task PLookupStart()
    {
        PLookupCancel();

        string word = PHeadword.Text?.Trim() ?? string.Empty;
        _pLookupCandidate.Clear();
        _lLookupSearching = word.Length > 0;
        PLookupStatusUpdate();

        if (word.Length == 0)
        {
            return;
        }

        _lLookupCancellation = new CancellationTokenSource();
        CancellationToken token = _lLookupCancellation.Token;

        try
        {
            await _lEngine.LEnginePronunciationFind(word, _pLangcodeChoice, this, token);
        }
        catch (OperationCanceledException)
        {
            // Superseded by a newer lookup or the window closed; ignore.
        }
        finally
        {
            if (!token.IsCancellationRequested)
            {
                _lLookupSearching = false;
                PLookupStatusUpdate();
            }
        }
    }

    private void PLookupCancel()
    {
        _lLookupCancellation?.Cancel();
        _lLookupCancellation?.Dispose();
        _lLookupCancellation = null;
    }

    private void PLookupStatusUpdate()
    {
        bool hasCandidates = _pLookupCandidate.Count > 0;
        PLookupMenuList.Visibility = hasCandidates ? Visibility.Visible : Visibility.Collapsed;

        if (_lLookupSearching && !hasCandidates)
        {
            PLookupMenuStatus.Text = _pInputHost.PLocalizationTextRead("Lookup.Searching");
            PLookupStatusCard.Visibility = Visibility.Visible;
            PLookupProgress.Visibility = Visibility.Visible;
        }
        else if (!_lLookupSearching && !hasCandidates)
        {
            PLookupMenuStatus.Text = _pInputHost.PLocalizationTextRead("Lookup.Empty");
            PLookupStatusCard.Visibility = Visibility.Visible;
            PLookupProgress.Visibility = Visibility.Collapsed;
        }
        else
        {
            PLookupStatusCard.Visibility = Visibility.Collapsed;
        }
    }

    void LReceiver.LReceiverSourceStart(string source)
    {
        // Each source's arrival is surfaced through LReceiverCandidateAdd; the shared "Searching…" status is
        // already shown while the lookup runs, so no per-source UI update is needed here.
    }

    void LReceiver.LReceiverCandidateAdd(LCandidate candidate)
    {
        Dispatcher.Invoke(() =>
        {
            _pLookupCandidate.Add(new PLookupCandidate(candidate, candidate.LCandidateSource));
            PLookupStatusUpdate();
        });
    }

    void LReceiver.LReceiverLookupFinish()
    {
        Dispatcher.Invoke(() =>
        {
            _lLookupSearching = false;
            PLookupStatusUpdate();
        });
    }
}
