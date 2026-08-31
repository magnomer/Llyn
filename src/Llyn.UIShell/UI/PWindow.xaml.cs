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

public partial class PWindow : Window, IPronunciationReceiver
{
    private readonly ObservableCollection<PInputCard> _senses = [];
    private readonly ObservableCollection<PInputCard> _collocations = [];
    private readonly ObservableCollection<PLookupCandidate> _lookupCandidates = [];
    private readonly LEngine _engine = new();
    private CancellationTokenSource? _lookupCancellation;
    private bool _lookupSearching;

    public PWindow()
    {
        InitializeComponent();
        PLocalization.SelectedValue = PLocalizationLoader.DefaultLanguage;

        PSenseList.ItemsSource = _senses;
        PCollocationList.ItemsSource = _collocations;
        PLookupMenuList.ItemsSource = _lookupCandidates;

        _senses.Add(new PInputCard("Sense", 1));
        _collocations.Add(new PInputCard("Collocation", 1));

        Closed += PWindow_OnClosed;
    }

    private void PCaptionMinimize_OnClick(object sender, RoutedEventArgs e)
    {
        SystemCommands.MinimizeWindow(this);
    }

    private void PCaptionMaximize_OnClick(object sender, RoutedEventArgs e)
    {
        ToggleMaximizedState();
    }

    private void PCaptionClose_OnClick(object sender, RoutedEventArgs e)
    {
        SystemCommands.CloseWindow(this);
    }

    private void PNavigation_OnClick(object sender, RoutedEventArgs e)
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
            (PNavigationDualpanel, PDualPanel),
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

    private void PStack_OnClick(object sender, RoutedEventArgs e)
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

    private void PCardRemove_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PInputCard card })
        {
            return;
        }

        ObservableCollection<PInputCard>? list =
            _senses.Contains(card) ? _senses :
            _collocations.Contains(card) ? _collocations :
            null;

        if (list is null || list.Count <= 1)
        {
            return;
        }

        list.Remove(card);
        Renumber(list);
    }

    private void PAddSense_OnClick(object sender, RoutedEventArgs e)
    {
        _senses.Add(new PInputCard("Sense", _senses.Count + 1));
    }

    private void PAddCollocation_OnClick(object sender, RoutedEventArgs e)
    {
        _collocations.Add(new PInputCard("Collocation", _collocations.Count + 1));
    }

    private static void Renumber(ObservableCollection<PInputCard> list)
    {
        for (int index = 0; index < list.Count; index++)
        {
            list[index].Order = index + 1;
        }
    }

    private void PNoteContents_OnTextChanged(object sender, TextChangedEventArgs e)
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

    private void PLocalization_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (PLocalization.SelectedValue is string language)
        {
            PLocalizationLoader.Apply(System.Windows.Application.Current.Resources, language);
        }
    }

    private void PRoof_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            ToggleMaximizedState();
            return;
        }

        if (WindowState == WindowState.Maximized)
        {
            RestoreWindowAtPointer(e);
        }

        DragMove();
    }

    private void ToggleMaximizedState()
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

    private void RestoreWindowAtPointer(MouseButtonEventArgs e)
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

    private async void PLookup_OnChecked(object sender, RoutedEventArgs e)
    {
        await StartLookup();
    }

    private void PLookup_OnUnchecked(object sender, RoutedEventArgs e)
    {
        CancelLookup();
    }

    private void PLookupMenuSelector_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PLookupCandidate candidate })
        {
            return;
        }

        PPronunciation.Text = candidate.Phonetic;
        PLookup.IsChecked = false;
    }

    private async Task StartLookup()
    {
        CancelLookup();

        string word = PHeadword.Text?.Trim() ?? string.Empty;
        _lookupCandidates.Clear();
        _lookupSearching = word.Length > 0;
        UpdateLookupStatus();

        if (word.Length == 0)
        {
            return;
        }

        _lookupCancellation = new CancellationTokenSource();
        CancellationToken token = _lookupCancellation.Token;

        try
        {
            await _engine.PronunciationFindAsync(word, this, token);
        }
        catch (OperationCanceledException)
        {
            // Superseded by a newer lookup or the window closed; ignore.
        }
        finally
        {
            if (!token.IsCancellationRequested)
            {
                _lookupSearching = false;
                UpdateLookupStatus();
            }
        }
    }

    private void CancelLookup()
    {
        _lookupCancellation?.Cancel();
        _lookupCancellation?.Dispose();
        _lookupCancellation = null;
    }

    private void UpdateLookupStatus()
    {
        bool hasCandidates = _lookupCandidates.Count > 0;
        PLookupMenuList.Visibility = hasCandidates ? Visibility.Visible : Visibility.Collapsed;

        if (_lookupSearching && !hasCandidates)
        {
            PLookupMenuStatus.Text = ReadText("Lookup.Searching");
            PLookupMenuStatus.Visibility = Visibility.Visible;
        }
        else if (!_lookupSearching && !hasCandidates)
        {
            PLookupMenuStatus.Text = ReadText("Lookup.Empty");
            PLookupMenuStatus.Visibility = Visibility.Visible;
        }
        else
        {
            PLookupMenuStatus.Visibility = Visibility.Collapsed;
        }
    }

    private string ReadSourceLabel(LookupSource source)
    {
        return ReadText("Lookup.Source" + source);
    }

    private string ReadText(string key)
    {
        return TryFindResource(key) as string ?? key;
    }

    private void PWindow_OnClosed(object? sender, EventArgs e)
    {
        CancelLookup();
        _engine.Dispose();
    }

    void IPronunciationReceiver.SourceStart(LookupSource source)
    {
        // Each source's arrival is surfaced through CandidateAdd; the shared "Searching…" status is
        // already shown while the lookup runs, so no per-source UI update is needed here.
    }

    void IPronunciationReceiver.CandidateAdd(PronunciationCandidate candidate)
    {
        Dispatcher.Invoke(() =>
        {
            _lookupCandidates.Add(new PLookupCandidate(candidate, ReadSourceLabel(candidate.Source)));
            UpdateLookupStatus();
        });
    }

    void IPronunciationReceiver.LookupStop()
    {
        Dispatcher.Invoke(() =>
        {
            _lookupSearching = false;
            UpdateLookupStatus();
        });
    }
}
