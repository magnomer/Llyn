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
    private readonly LEngine _lEngine = new();
    private readonly MediaPlayer _pDownloaderPlayer = new();
    private CancellationTokenSource? _lLookupCancellation;
    private CancellationTokenSource? _lHarvestCancellation;
    private bool _lLookupSearching;
    private bool _lHarvestSearching;
    private string _pLangcodeChoice = PWindowLanguage;
    private readonly bool _pWindowReady;

    public PWindow()
    {
        InitializeComponent();

        PWorkspacePath.Text = _lEngine.LEngineWorkspaceRead();
        PLocalization.SelectedValue = _lEngine.LEngineSettingsRead().LSettingsLocalization;
        _pWindowReady = true;

        PSenseList.ItemsSource = _pSenseList;
        PCollocationList.ItemsSource = _pCollocationList;
        PLookupMenuList.ItemsSource = _pLookupCandidate;
        PDownloaderMenuList.ItemsSource = _pDownloaderRecording;
        PLangcodeListMenu.ItemsSource = _pLangcodeList;
        PLangcodeLoad();

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

        _pLangcodeChoice = language;
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
            PDownloaderMenuStatus.Visibility = Visibility.Visible;
        }
        else if (!_lHarvestSearching && !hasRecordings)
        {
            PDownloaderMenuStatus.Text = PLocalizationTextRead("Downloader.Empty");
            PDownloaderMenuStatus.Visibility = Visibility.Visible;
        }
        else
        {
            PDownloaderMenuStatus.Visibility = Visibility.Collapsed;
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

        PDownloaderMenuStatus.Text = PLocalizationTextRead("Downloader.Saving");
        PDownloaderMenuStatus.Visibility = Visibility.Visible;

        try
        {
            await _lEngine.LEngineRecordingSave(recording.PDownloaderRecordingModel, word, _pLangcodeChoice, CancellationToken.None);
            PDownloaderMenuStatus.Text = PLocalizationTextRead("Downloader.Saved");
        }
        catch (Exception)
        {
            // A failed download leaves the menu open with a failure notice; the user can retry.
            PDownloaderMenuStatus.Text = PLocalizationTextRead("Downloader.Failed");
        }
    }

    private string PLocalizationTextRead(string key)
    {
        return TryFindResource(key) as string ?? key;
    }

    private void PWindowExitHandle(object? sender, EventArgs e)
    {
        PLookupCancel();
        PDownloaderCancel();
        _pDownloaderPlayer.Close();
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
