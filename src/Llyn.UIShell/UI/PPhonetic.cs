using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor : LReceiver
{
    private readonly ObservableCollection<PPhoneticItem> _pPhoneticItem = [];
    private CancellationTokenSource? _pPhoneticCancellation;
    private bool _pPhoneticSearching;

    private async void PLookupCheckedHandle(object sender, RoutedEventArgs e)
    {
        await PPhoneticStart();
    }

    private void PLookupUncheckedHandle(object sender, RoutedEventArgs e)
    {
        PPhoneticCancel();
    }

    internal void PPhoneticSelectorHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PPhoneticItem candidate })
        {
            return;
        }

        PPronunciation.Text = candidate.PPhoneticItemReading;
        PLookup.IsChecked = false;
    }

    private async Task PPhoneticStart()
    {
        PPhoneticCancel();

        string word = PHeadword.Text?.Trim() ?? string.Empty;
        _pPhoneticItem.Clear();
        _pPhoneticSearching = word.Length > 0;
        PPhoneticUpdate();

        if (word.Length == 0)
        {
            return;
        }

        _pPhoneticCancellation = new CancellationTokenSource();

        try
        {
            await _lEngine.LEnginePronunciationFind(word, _pLanguageChoice, this, _pPhoneticCancellation.Token);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception)
        {
            _pPhoneticSearching = false;
            PPhoneticUpdate();
        }
    }

    private void PPhoneticCancel()
    {
        _pPhoneticCancellation?.Cancel();
        _pPhoneticCancellation?.Dispose();
        _pPhoneticCancellation = null;
    }

    private void PPhoneticUpdate()
    {
        bool candidates = _pPhoneticItem.Count > 0;

        PPhoneticList.Visibility = candidates ? Visibility.Visible : Visibility.Collapsed;
        PPhoneticProgress.Visibility = _pPhoneticSearching ? Visibility.Visible : Visibility.Collapsed;

        if (candidates)
        {
            PPhoneticNotice.Visibility = Visibility.Collapsed;
            return;
        }

        PPhoneticNotice.Text = _pEditorHost.PLocalizationTextRead(
            _pPhoneticSearching ? "Lookup.Searching" : "Lookup.Empty");
        PPhoneticNotice.Visibility = Visibility.Visible;
    }

    void LReceiver.LReceiverSourceStart(string source)
    {
    }

    void LReceiver.LReceiverCandidateAdd(LCandidate candidate)
    {
        Dispatcher.Invoke(() =>
        {
            _pPhoneticItem.Add(new PPhoneticItem(candidate, candidate.LCandidateSource));
            PPhoneticUpdate();
        });
    }

    void LReceiver.LReceiverLookupFinish()
    {
        Dispatcher.Invoke(() =>
        {
            _pPhoneticSearching = false;
            PPhoneticUpdate();
        });
    }
}
