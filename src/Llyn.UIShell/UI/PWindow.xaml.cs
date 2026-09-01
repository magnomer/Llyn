using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using Llyn.Core;
using Llyn.ShellEngine;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace Llyn.UIShell;

public partial class PWindow : Window, LReceiver, LListener
{
    // Fallback language used until a pack is chosen, and when no pack folder is present on disk.
    private const string PWindowLanguage = "English";

    private readonly ObservableCollection<PInputCard> _pSenseList = [];
    private readonly ObservableCollection<PLangcodeItem> _pLangcodeList = [];
    private readonly ObservableCollection<PInputCard> _pCollocationList = [];
    private readonly ObservableCollection<PLookupCandidate> _pLookupCandidate = [];
    private readonly ObservableCollection<PDownloaderRecording> _pDownloaderRecording = [];
    private readonly LEngine _lEngine;
    private readonly MediaPlayer _pDownloaderPlayer = new();
    private string? _pRecording;
    private string? _pRecordingSource;
    private PGhost? _pGhost;
    private FrameworkElement? _pGhostCard;
    private TextBox? _pGhostTitle;
    private Point _pGhostGrab;
    private CancellationTokenSource? _lLookupCancellation;
    private CancellationTokenSource? _lHarvestCancellation;
    private bool _lLookupSearching;
    private bool _lHarvestSearching;
    private string _pLangcodeChoice = PWindowLanguage;
    private readonly bool _pWindowReady;

    /// <summary>
    /// Opens the window on <paramref name="engine"/>, already built and bound to a workspace that
    /// opened. The engine is not constructed here: opening the workspace can fail, and a failure in a
    /// window constructor has nowhere to be shown. <see cref="LBootstrap"/> builds it and hands it
    /// over; the window owns it from here and disposes it when it closes.
    /// </summary>
    public PWindow(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);

        _lEngine = engine;

        InitializeComponent();

        PWorkspacePath.Text = _lEngine.LEngineWorkspaceRead();
        PLocalization.SelectedValue = _lEngine.LEngineSettingsRead().LSettingsLocalization;
        _pWindowReady = true;

        PSenseList.ItemsSource = _pSenseList;
        PCollocationList.ItemsSource = _pCollocationList;
        PLookupMenuList.ItemsSource = _pLookupCandidate;
        PDownloaderMenuList.ItemsSource = _pDownloaderRecording;
        PLangcodeListMenu.ItemsSource = _pLangcodeList;
        PHeadword.TextChanged += PHeadwordHandle;
        // Card drags re-parent their container on every reorder, which drops mouse capture and
        // with it the header's event routing; the window-level preview events tunnel from the root
        // on every input, so the drag loop and the drop stay reachable for the whole gesture.
        PreviewMouseMove += PCardDragHandle;
        PreviewMouseLeftButtonUp += PCardDropHandle;
        PLangcodeLoad();

        // The session the last run left behind: the form opens on the entry the workspace row names,
        // or empty when it names nothing. This also seeds the card lists, so no cards are added here.
        PStateRestore();

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

    private void PCardPressHandle(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left || e.ButtonState != MouseButtonState.Pressed)
        {
            return;
        }

        // Clears any stale drag state before arming a new one.
        PCardGhostStop();

        if (sender is not FrameworkElement { DataContext: PInputCard } header)
        {
            return;
        }

        // A press on the remove button must remove the card, not arm a drag.
        DependencyObject? origin = e.OriginalSource as DependencyObject;
        if (PCardButtonCheck(origin))
        {
            return;
        }

        FrameworkElement? card = PCardRootFind(header);
        if (card is null)
        {
            return;
        }

        _pGhostCard = card;
        _pGhostGrab = e.GetPosition(card);
        _pGhostTitle = PCardTitleFind(origin);
    }

    private void PCardDragHandle(object sender, MouseEventArgs e)
    {
        if (_pGhostCard is null)
        {
            return;
        }

        if (e.LeftButton != MouseButtonState.Pressed)
        {
            PCardGhostStop();
            return;
        }

        if (Content is not FrameworkElement surface)
        {
            return;
        }

        Point cursor = e.GetPosition(surface);
        if (_pGhost is null)
        {
            Point current = e.GetPosition(_pGhostCard);
            if (Math.Abs(current.X - _pGhostGrab.X) < SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(current.Y - _pGhostGrab.Y) < SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }

            PCardGhostStart(surface, cursor);
            return;
        }

        _pGhost.PGhostPlace(new Point(cursor.X - _pGhostGrab.X, cursor.Y - _pGhostGrab.Y));
        PCardOrderPlace(e, surface);
    }

    private void PCardDropHandle(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton != MouseButton.Left)
        {
            return;
        }

        PCardGhostStop();
    }

    private void PCardGhostStart(FrameworkElement surface, Point cursor)
    {
        if (_pGhostCard is null)
        {
            return;
        }

        // A press inside the title starts a text selection before the drag threshold is reached;
        // collapse it so the drag does not carry the highlight along.
        if (_pGhostTitle is not null)
        {
            _pGhostTitle.Select(_pGhostTitle.CaretIndex, 0);
        }

        AdornerLayer? layer = AdornerLayer.GetAdornerLayer(surface);
        if (layer is null)
        {
            return;
        }

        _pGhost = new PGhost(surface, _pGhostCard);
        layer.Add(_pGhost);
        _pGhost.PGhostPlace(new Point(cursor.X - _pGhostGrab.X, cursor.Y - _pGhostGrab.Y));
        _pGhostCard.Opacity = 0.35;
        // Capture on the window itself: unlike the card header it is never re-parented mid-drag,
        // so the capture survives every reorder.
        Mouse.Capture(this);
    }

    // The cleanup re-enters: WPF re-raises mouse-button events, and the adorner removal, capture
    // release, and binding refresh below can dispatch queued input mid-cleanup. Clear the state
    // first so any re-entered call sees nothing to stop.
    private void PCardGhostStop()
    {
        if (_pGhostCard is null)
        {
            return;
        }

        FrameworkElement card = _pGhostCard;
        PGhost? ghost = _pGhost;
        _pGhost = null;
        _pGhostCard = null;
        _pGhostTitle = null;

        if (ghost is not null)
        {
            if (Content is Visual surface)
            {
                AdornerLayer.GetAdornerLayer(surface)?.Remove(ghost);
            }

            if (PCardListFind(card) is { } list)
            {
                PInputOrderUpdate(list);
            }

            Mouse.Capture(null);
        }

        card.Opacity = 1;
    }

    // Moves the dragged card within its own list each time the pointer crosses another card's
    // midpoint, so the cards rearrange live under the ghost.
    private void PCardOrderPlace(MouseEventArgs e, FrameworkElement surface)
    {
        if (_pGhostCard is null)
        {
            return;
        }

        ObservableCollection<PInputCard>? list = PCardListFind(_pGhostCard);
        if (list is null || list.Count <= 1)
        {
            return;
        }

        ScrollViewer scroll = list == _pSenseList ? PMeaning : PCollocation;
        PCardScrollMove(scroll, e.GetPosition(scroll));

        if (_pGhostCard.DataContext is not PInputCard card ||
            VisualTreeHelper.GetParent(_pGhostCard) is not Panel host)
        {
            return;
        }

        int current = list.IndexOf(card);
        if (current < 0)
        {
            return;
        }

        // The pointer grabs the card by its title, so the ghost hangs below the cursor; comparing
        // the raw cursor to the sibling midpoints would swap far too early upward and far too late
        // downward. Compare the dragged card's own center instead.
        Point cursor = e.GetPosition(surface);
        double probe = cursor.Y - _pGhostGrab.Y + (_pGhostCard.ActualHeight / 2);
        int target = 0;
        foreach (UIElement sibling in host.Children)
        {
            if (sibling is not FrameworkElement element || element.DataContext == card)
            {
                continue;
            }

            Point middle = element.TransformToAncestor(surface).Transform(new Point(0, element.ActualHeight / 2));
            if (middle.Y < probe)
            {
                target++;
            }
        }

        if (target != current)
        {
            list.Move(current, target);
        }
    }

    // Nudges the list scroll while the ghost nears the viewport edge, so cards outside the
    // visible range stay reachable.
    private static void PCardScrollMove(ScrollViewer scroll, Point cursor)
    {
        const double edge = 56;
        double step = 0;

        if (cursor.Y < edge)
        {
            step = -(edge - cursor.Y) / 4;
        }
        else if (cursor.Y > scroll.ActualHeight - edge)
        {
            step = (edge - (scroll.ActualHeight - cursor.Y)) / 4;
        }

        if (step != 0 && scroll.ScrollableHeight > 0)
        {
            scroll.ScrollToVerticalOffset(scroll.VerticalOffset + step);
        }
    }

    private ObservableCollection<PInputCard>? PCardListFind(FrameworkElement card)
    {
        if (card.DataContext is not PInputCard inputCard)
        {
            return null;
        }

        return _pSenseList.Contains(inputCard) ? _pSenseList
            : _pCollocationList.Contains(inputCard) ? _pCollocationList
            : null;
    }

    // The card visual is the template-root element sitting directly under the list's items panel;
    // walk up from the header until the element is found whose parent is that StackPanel (a Grid
    // is a Panel too, so the items-host type, not Panel, is what marks the card root).
    private static FrameworkElement? PCardRootFind(FrameworkElement header)
    {
        FrameworkElement? root = null;
        for (DependencyObject? current = header; current is not null; current = VisualTreeHelper.GetParent(current))
        {
            if (current is ItemsControl)
            {
                return root;
            }

            if (current is FrameworkElement candidate && VisualTreeHelper.GetParent(current) is StackPanel)
            {
                root ??= candidate;
            }
        }

        return null;
    }

    private static bool PCardButtonCheck(DependencyObject? origin)
    {
        for (DependencyObject? current = origin; current is not null; current = VisualTreeHelper.GetParent(current))
        {
            if (current is Button)
            {
                return true;
            }
        }

        return false;
    }

    private static TextBox? PCardTitleFind(DependencyObject? origin)
    {
        for (DependencyObject? current = origin; current is not null; current = VisualTreeHelper.GetParent(current))
        {
            if (current is TextBox box)
            {
                return box;
            }
        }

        return null;
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
        if (PLocalization.SelectedValue is not string language)
        {
            return;
        }

        PLocalizationLoader.PLocalizationLoaderApply(System.Windows.Application.Current.Resources, language);

        // Skip persistence while the constructor is applying the stored choice; only user changes save.
        if (_pWindowReady)
        {
            _lEngine.LEngineSettingsSave(new LSettings(language));
        }
    }

    private void PWorkspaceBrowseHandle(object sender, RoutedEventArgs e)
    {
        Microsoft.Win32.OpenFolderDialog dialog = new()
        {
            Title = PLocalizationTextRead("Settings.Workspace"),
            InitialDirectory = PWorkspacePath.Text
        };

        if (dialog.ShowDialog(this) == true)
        {
            PWorkspacePath.Text = dialog.FolderName;
            PWorkspaceApply();
        }
    }

    private void PWorkspacePathHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        PWorkspaceApply();
    }

    private void PWorkspaceApply()
    {
        string path = PWorkspacePath.Text?.Trim() ?? string.Empty;
        if (path.Length == 0 || string.Equals(path, _lEngine.LEngineWorkspaceRead(), StringComparison.Ordinal))
        {
            return;
        }

        try
        {
            _lEngine.LEngineWorkspaceChange(path);
        }
        catch (Exception)
        {
            // An unusable path (permission, invalid characters) leaves the previous workspace in place;
            // restore the field so it keeps showing the folder actually in use.
            PWorkspacePath.Text = _lEngine.LEngineWorkspaceRead();
            return;
        }

        // The new workspace has its own database, so everything on screen came from a database that is
        // no longer open and carries ids that mean nothing here. The form moves onto the new
        // workspace's own session, and the list and display are re-read from it.
        PStateRestore();
        PDisplayClear();
        PIndexFind(PInquiry.Text ?? string.Empty);
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
            PLookupMenuStatus.Text = PLocalizationTextRead("Lookup.Searching");
            PLookupStatusCard.Visibility = Visibility.Visible;
            PLookupProgress.Visibility = Visibility.Visible;
        }
        else if (!_lLookupSearching && !hasCandidates)
        {
            PLookupMenuStatus.Text = PLocalizationTextRead("Lookup.Empty");
            PLookupStatusCard.Visibility = Visibility.Visible;
            PLookupProgress.Visibility = Visibility.Collapsed;
        }
        else
        {
            PLookupStatusCard.Visibility = Visibility.Collapsed;
        }
    }

    // Builds the language menu. Each row carries its own flag, resolved once here so the dropdown
    // paints ready. The flag download may await a first-time fetch, hence async.
    private async void PLangcodeLoad()
    {
        _pLangcodeList.Clear();
        var languages = _lEngine.LEngineLanguageRead();
        foreach (string language in languages)
        {
            string? path = await _lEngine.LEngineFlagRead(language, CancellationToken.None);
            ImageSource? flag = path is not null && File.Exists(path) ? PLangcodeFlagResolve(path) : null;
            _pLangcodeList.Add(new PLangcodeItem(language, flag));
        }

        if (languages.Count > 0 && !languages.Contains(_pLangcodeChoice))
        {
            _pLangcodeChoice = languages[0];
        }

        PLangcodeBaseName.Text = _pLangcodeChoice;
        PLangcodeFlagUpdate();
    }

    private void PLangcodeHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PLangcodeItem { PLangcodeItemName: string language } })
        {
            return;
        }

        if (!string.Equals(_pLangcodeChoice, language, StringComparison.Ordinal))
        {
            PRecordingClear();
            _pLangcodeChoice = language;
        }
        PLangcodeBaseName.Text = language;
        PLangcodeFlagUpdate();
        PLangcodeBase.IsChecked = false;
    }

    // Shows the selected language's flag beside its name. A pack declares an ISO country code; the
    // engine downloads and caches the matching flag-icons SVG, so this may await a first-time fetch.
    // When the pack declares no flag or the download fails, a neutral globe stands in.
    private async void PLangcodeFlagUpdate()
    {
        string chosen = _pLangcodeChoice;
        string? path = await _lEngine.LEngineFlagRead(chosen, CancellationToken.None);

        // The choice may have changed while the flag downloaded; only paint the still-current one.
        if (chosen != _pLangcodeChoice)
        {
            return;
        }

        DrawingImage? flag = path is not null && File.Exists(path) ? PLangcodeFlagResolve(path) : null;
        if (flag is not null)
        {
            PLangcodeBaseFlag.Source = flag;
            PLangcodeBaseFlag.Visibility = Visibility.Visible;
            PLangcodeBaseGlobe.Visibility = Visibility.Collapsed;
        }
        else
        {
            PLangcodeBaseFlag.Source = null;
            PLangcodeBaseFlag.Visibility = Visibility.Collapsed;
            PLangcodeBaseGlobe.Visibility = Visibility.Visible;
        }
    }

    // Rasterizes a flag SVG into a frozen drawing the Image can paint. Flag-icons ship as SVG, which
    // WPF's bitmap decoders can't read, so SharpVectors renders it into a WPF drawing here.
    private static DrawingImage? PLangcodeFlagResolve(string path)
    {
        try
        {
            FileSvgReader reader = new(new WpfDrawingSettings { IncludeRuntime = false, TextAsGeometry = true });
            DrawingGroup drawing = reader.Read(path);
            if (drawing is null)
            {
                return null;
            }

            DrawingImage image = new(drawing);
            image.Freeze();
            return image;
        }
        catch (Exception exception) when (exception is IOException or XmlException or NotSupportedException)
        {
            return null;
        }
    }

    private async void PDownloaderCheckedHandle(object sender, RoutedEventArgs e)
    {
        await PDownloaderStart();
    }

    private void PDownloaderUncheckedHandle(object sender, RoutedEventArgs e)
    {
        PDownloaderCancel();
    }

    private async Task PDownloaderStart()
    {
        PDownloaderCancel();

        string word = PHeadword.Text?.Trim() ?? string.Empty;
        _pDownloaderRecording.Clear();
        _lHarvestSearching = word.Length > 0;
        PDownloaderStatusUpdate();

        if (word.Length == 0)
        {
            return;
        }

        _lHarvestCancellation = new CancellationTokenSource();
        CancellationToken token = _lHarvestCancellation.Token;

        try
        {
            await _lEngine.LEngineRecordingFind(word, _pLangcodeChoice, this, token);
        }
        catch (OperationCanceledException)
        {
            // Superseded by a newer discovery or the window closed; ignore.
        }
        finally
        {
            if (!token.IsCancellationRequested)
            {
                _lHarvestSearching = false;
                PDownloaderStatusUpdate();
            }
        }
    }

    private void PDownloaderCancel()
    {
        _lHarvestCancellation?.Cancel();
        _lHarvestCancellation?.Dispose();
        _lHarvestCancellation = null;
    }

    private void PDownloaderStatusUpdate()
    {
        bool hasRecordings = _pDownloaderRecording.Count > 0;
        PDownloaderMenuList.Visibility = hasRecordings ? Visibility.Visible : Visibility.Collapsed;

        if (_lHarvestSearching && !hasRecordings)
        {
            PDownloaderMenuStatus.Text = PLocalizationTextRead("Downloader.Searching");
            PDownloaderStatusCard.Visibility = Visibility.Visible;
            PDownloaderProgress.Visibility = Visibility.Visible;
        }
        else if (!_lHarvestSearching && !hasRecordings)
        {
            PDownloaderMenuStatus.Text = PLocalizationTextRead("Downloader.Empty");
            PDownloaderStatusCard.Visibility = Visibility.Visible;
            PDownloaderProgress.Visibility = Visibility.Collapsed;
        }
        else
        {
            PDownloaderStatusCard.Visibility = Visibility.Collapsed;
        }
    }

    private async void PDownloaderPlayHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PDownloaderRecording recording })
        {
            return;
        }

        try
        {
            // Streaming the remote, token-bearing URL through the media stack is unreliable; fetch it
            // to a local temp file first, then play that.
            string path = await _lEngine.LEngineRecordingPrepare(recording.PDownloaderRecordingModel, CancellationToken.None);
            _pDownloaderPlayer.Open(new Uri(path));
            _pDownloaderPlayer.Play();
        }
        catch (Exception)
        {
            // Preview is best-effort; a failed fetch leaves the menu untouched.
        }
    }

    private async void PDownloaderMenuHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PDownloaderRecording recording })
        {
            return;
        }

        string word = PHeadword.Text?.Trim() ?? string.Empty;
        if (word.Length == 0)
        {
            return;
        }

        string language = _pLangcodeChoice;
        PDownloaderMenuStatus.Text = PLocalizationTextRead("Downloader.Saving");
        PDownloaderStatusCard.Visibility = Visibility.Visible;
        PDownloaderProgress.Visibility = Visibility.Visible;

        try
        {
            string path = await _lEngine.LEngineRecordingSave(recording.PDownloaderRecordingModel, word, language, CancellationToken.None);
            PDownloaderMenuStatus.Text = PLocalizationTextRead("Downloader.Saved");
            PDownloaderProgress.Visibility = Visibility.Collapsed;

            if (string.Equals(PHeadword.Text?.Trim(), word, StringComparison.Ordinal) &&
                string.Equals(_pLangcodeChoice, language, StringComparison.Ordinal))
            {
                _pRecording = path;
                _pRecordingSource = recording.PDownloaderRecordingSource;
                PPlayback.Visibility = Visibility.Visible;
            }
        }
        catch (Exception)
        {
            // A failed download leaves the menu open with a failure notice; the user can retry.
            PDownloaderMenuStatus.Text = PLocalizationTextRead("Downloader.Failed");
            PDownloaderProgress.Visibility = Visibility.Collapsed;
        }
    }

    private void PPlaybackHandle(object sender, RoutedEventArgs e)
    {
        if (_pRecording is null || !File.Exists(_pRecording))
        {
            PRecordingClear();
            return;
        }

        _pDownloaderPlayer.Open(new Uri(_pRecording));
        _pDownloaderPlayer.Play();
    }

    private void PHeadwordHandle(object sender, TextChangedEventArgs e)
    {
        PRecordingClear();
    }

    private void PRecordingClear()
    {
        _pRecording = null;
        _pRecordingSource = null;
        _pDownloaderPlayer.Stop();
        PPlayback.Visibility = Visibility.Collapsed;
    }

    private string PLocalizationTextRead(string key)
    {
        return TryFindResource(key) as string ?? key;
    }

    // Presents a request that failed, under the localized headline the given key names.
    // A deliberate refusal carries a reason key, which resolves through the same catalog as the rest
    // of the interface; anything unexpected is a fault rather than a refusal, so it keeps its own
    // message, which stays diagnosable even though it is not translated.
    private void PWindowFailureShow(string key, Exception exception)
    {
        string detail = exception is LRefusal refusal
            ? PLocalizationTextRead(refusal.LRefusalReason)
            : exception.Message;

        MessageBox.Show(
            this,
            $"{PLocalizationTextRead(key)}\n\n{detail}",
            PLocalizationTextRead("Terms.Product"),
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }

    private void PWindowExitHandle(object? sender, EventArgs e)
    {
        PLookupCancel();
        PDownloaderCancel();
        _pDownloaderPlayer.Close();
        // The session is recorded before the engine goes: the save runs through it.
        PStateSave();
        _lEngine.Dispose();
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

    void LListener.LListenerSourceStart(string source)
    {
        // Handled the same way as lookup: the shared "Searching…" status already covers per-source starts.
    }

    void LListener.LListenerRecordingAdd(LRecording recording)
    {
        Dispatcher.Invoke(() =>
        {
            _pDownloaderRecording.Add(new PDownloaderRecording(recording));
            PDownloaderStatusUpdate();
        });
    }

    void LListener.LListenerFinish()
    {
        Dispatcher.Invoke(() =>
        {
            _lHarvestSearching = false;
            PDownloaderStatusUpdate();
        });
    }
}
