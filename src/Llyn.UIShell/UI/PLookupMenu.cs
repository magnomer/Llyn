using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PEditor : LReceiver
{
    private readonly ObservableCollection<PLookupCandidate> _pLookupCandidate = [];
    private CancellationTokenSource? _pLookupCancellation;
    private bool _pLookupSearching;

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
        _pLookupSearching = word.Length > 0;
        PLookupMenuUpdate();

        if (word.Length == 0)
        {
            return;
        }

        _pLookupCancellation = new CancellationTokenSource();

        try
        {
            await _lEngine.LEnginePronunciationFind(word, _pLangcodeChoice, this, _pLookupCancellation.Token);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception)
        {
            _pLookupSearching = false;
            PLookupMenuUpdate();
        }
    }

    private void PLookupCancel()
    {
        _pLookupCancellation?.Cancel();
        _pLookupCancellation?.Dispose();
        _pLookupCancellation = null;
    }

    private void PLookupMenuUpdate()
    {
        bool candidates = _pLookupCandidate.Count > 0;

        PLookupMenuList.Visibility = candidates ? Visibility.Visible : Visibility.Collapsed;
        PLookupMenuProgress.Visibility = _pLookupSearching ? Visibility.Visible : Visibility.Collapsed;

        if (candidates)
        {
            PLookupMenuNotice.Visibility = Visibility.Collapsed;
            return;
        }

        PLookupMenuNotice.Text = _pEditorHost.PLocalizationTextRead(
            _pLookupSearching ? "Lookup.Searching" : "Lookup.Empty");
        PLookupMenuNotice.Visibility = Visibility.Visible;
    }

    void LReceiver.LReceiverSourceStart(string source)
    {
    }

    void LReceiver.LReceiverCandidateAdd(LCandidate candidate)
    {
        Dispatcher.Invoke(() =>
        {
            _pLookupCandidate.Add(new PLookupCandidate(candidate, candidate.LCandidateSource));
            PLookupMenuUpdate();
        });
    }

    void LReceiver.LReceiverLookupFinish()
    {
        Dispatcher.Invoke(() =>
        {
            _pLookupSearching = false;
            PLookupMenuUpdate();
        });
    }
}
