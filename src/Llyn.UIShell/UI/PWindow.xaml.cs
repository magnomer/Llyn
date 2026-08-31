using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIShell;

public partial class PWindow : Window, LReceiver
{
    private readonly ObservableCollection<PInputCard> _pSenseList = [];
    private readonly ObservableCollection<PInputCard> _pCollocationList = [];
    private readonly ObservableCollection<PLookupCandidate> _pLookupCandidate = [];
    private readonly LEngine _lEngine = new();
    private CancellationTokenSource? _lLookupCancellation;
    private bool _lLookupSearching;

    public PWindow()
    {
        InitializeComponent();
        PLocalization.SelectedValue = PLocalizationLoader.PLocalizationLoaderLanguage;

        PSenseList.ItemsSource = _pSenseList;
        PCollocationList.ItemsSource = _pCollocationList;
        PLookupMenuList.ItemsSource = _pLookupCandidate;

        _pSenseList.Add(new PInputCard("Sense", 1));
        _pCollocationList.Add(new PInputCard("Collocation", 1));

        Closed += PWindowExitHandle;
    }

    private void PCaptionMinimizeHandle(object sender, RoutedEventArgs e)
    {
        SystemCommands.MinimizeWindow(this);
    }

    private void PCaptionMaximizeHandle(object sender, RoutedEventArgs e)
    {
        PWindowMaximizeToggle();
    }

    private void PCaptionExitHandle(object sender, RoutedEventArgs e)
    {
        SystemCommands.CloseWindow(this);
    }

    private void PNavigationHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not Button selectedButton)
        {
            return;
        }

        (Button Button, Grid Panel)[] tabs =
        [
            (PNavigationInput, PInput),
            (PNavigationList, PList),
            (PNavigationSound, PSound),
            (PNavigationTag, PTag),
            (PNavigationSituation, PSituation),
            (PNavigationFavorite, PFavorite),
            (PNavigationDuplex, PDuplex),
            (PNavigationSettings, PSettings)
        ];

        foreach ((Button button, Grid panel) in tabs)
        {
            bool isSelected = button == selectedButton;
            button.Style = (Style)FindResource(
                isSelected ? "Theme.Navigation.Selected" : "Theme.Navigation.Button");
            panel.Visibility = isSelected ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private void PStackHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not Button selectedButton)
        {
            return;
        }

        (Button Button, FrameworkElement Contents)[] tabs =
        [
            (PStackMeaning, PMeaning),
            (PStackCollocation, PCollocation),
            (PStackNote, PNote)
        ];

        foreach ((Button button, FrameworkElement contents) in tabs)
        {
            bool isSelected = button == selectedButton;
            button.Style = (Style)FindResource(
                isSelected ? "Theme.Input.Tab.Selected" : "Theme.Input.Tab");
            contents.Visibility = isSelected ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private void PCardHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PInputCard card })
        {
            return;
        }

        ObservableCollection<PInputCard>? list =
            _pSenseList.Contains(card) ? _pSenseList :
            _pCollocationList.Contains(card) ? _pCollocationList :
            null;

        if (list is null || list.Count <= 1)
        {
            return;
        }

        list.Remove(card);
        PInputOrderUpdate(list);
    }

    private void PSenseHandle(object sender, RoutedEventArgs e)
    {
        _pSenseList.Add(new PInputCard("Sense", _pSenseList.Count + 1));
    }

    private void PCollocationHandle(object sender, RoutedEventArgs e)
    {
        _pCollocationList.Add(new PInputCard("Collocation", _pCollocationList.Count + 1));
    }

    private static void PInputOrderUpdate(ObservableCollection<PInputCard> list)
    {
        for (int index = 0; index < list.Count; index++)
        {
            list[index].PInputCardOrder = index + 1;
        }
    }

    private void PNoteContentsHandle(object sender, TextChangedEventArgs e)
    {
        if (PNotePlaceholder is null || PNoteContents is null)
        {
            return;
        }

        TextRange contents = new(PNoteContents.Document.ContentStart, PNoteContents.Document.ContentEnd);
        PNotePlaceholder.Visibility = string.IsNullOrWhiteSpace(contents.Text)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void PLocalizationHandle(object sender, SelectionChangedEventArgs e)
    {
        if (PLocalization.SelectedValue is string language)
        {
            PLocalizationLoader.PLocalizationLoaderApply(System.Windows.Application.Current.Resources, language);
        }
    }

    private void PRoofHandle(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            PWindowMaximizeToggle();
            return;
        }

        if (WindowState == WindowState.Maximized)
        {
            PWindowPointerRestore(e);
        }

        DragMove();
    }

    private void PWindowMaximizeToggle()
    {
        if (WindowState == WindowState.Maximized)
        {
            SystemCommands.RestoreWindow(this);
        }
        else
        {
            SystemCommands.MaximizeWindow(this);
        }
    }

    private void PWindowPointerRestore(MouseButtonEventArgs e)
    {
        Point pointerInWindow = e.GetPosition(this);
        Point pointerOnScreen = PointToScreen(pointerInWindow);
        PresentationSource? source = PresentationSource.FromVisual(this);

        if (source?.CompositionTarget is not null)
        {
            pointerOnScreen = source.CompositionTarget.TransformFromDevice.Transform(pointerOnScreen);
        }

        double restoredWidth = RestoreBounds.Width;
        double horizontalRatio = ActualWidth > 0
            ? Math.Clamp(pointerInWindow.X / ActualWidth, 0, 1)
            : 0.5;

        SystemCommands.RestoreWindow(this);
        Left = pointerOnScreen.X - (restoredWidth * horizontalRatio);
        Top = pointerOnScreen.Y - Math.Min(pointerInWindow.Y, PRoof.ActualHeight / 2);
    }

    private async void PLookupCheckedHandle(object sender, RoutedEventArgs e)
    {
        await PLookupStart();
    }

    private void PLookupUncheckedHandle(object sender, RoutedEventArgs e)
    {
        PLookupCancel();
    }

    private void PLookupMenuHandle(object sender, RoutedEventArgs e)
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
            await _lEngine.LEnginePronunciationFind(word, this, token);
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
            PLookupMenuStatus.Text = PLocalizationTextRead("Lookup.Searching");
            PLookupMenuStatus.Visibility = Visibility.Visible;
        }
        else if (!_lLookupSearching && !hasCandidates)
        {
            PLookupMenuStatus.Text = PLocalizationTextRead("Lookup.Empty");
            PLookupMenuStatus.Visibility = Visibility.Visible;
        }
        else
        {
            PLookupMenuStatus.Visibility = Visibility.Collapsed;
        }
    }

    private string PLookupSourceRead(LOrigin source)
    {
        string name = source switch
        {
            LOrigin.LOriginWikipedia => "Wikipedia",
            LOrigin.LOriginCambridge => "Cambridge",
            _ => source.ToString()
        };
        return PLocalizationTextRead("Lookup.Source" + name);
    }

    private string PLocalizationTextRead(string key)
    {
        return TryFindResource(key) as string ?? key;
    }

    private void PWindowExitHandle(object? sender, EventArgs e)
    {
        PLookupCancel();
        _lEngine.Dispose();
    }

    void LReceiver.LReceiverSourceStart(LOrigin source)
    {
        // Each source's arrival is surfaced through LReceiverCandidateAdd; the shared "Searching…" status is
        // already shown while the lookup runs, so no per-source UI update is needed here.
    }

    void LReceiver.LReceiverCandidateAdd(LCandidate candidate)
    {
        Dispatcher.Invoke(() =>
        {
            _pLookupCandidate.Add(new PLookupCandidate(candidate, PLookupSourceRead(candidate.LCandidateSource)));
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
