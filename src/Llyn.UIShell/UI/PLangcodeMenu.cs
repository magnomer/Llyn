using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace Llyn.UIShell;

/// <summary>
/// The language an entry is written in, as the input panel chooses it: the dropdown of installed
/// packs, the pill that shows the chosen one, and the flag beside it. The choice is what lookup,
/// audio download, and the saved entry are all carried out in.
/// </summary>
public partial class PInput
{
    // Fallback language used until a pack is chosen, and when no pack folder is present on disk.
    private const string PWindowLanguage = "English";

    private readonly ObservableCollection<PLangcodeItem> _pLangcodeList = [];

    private string _pLangcodeChoice = PWindowLanguage;

    // Whether the current language came from a loaded entry. A pack that is no longer on disk is
    // still the language the entry was written in, so the start-up fallback to the first pack leaves
    // it alone; only a form standing on no entry may be moved off its language.
    private bool _pLangcodeEntry;

    // Builds the language menu. Each row carries its own flag, resolved once here so the dropdown
    // paints ready. The flag download may await a first-time fetch, hence async.
    internal async void PLangcodeLoad()
    {
        _pLangcodeList.Clear();
        var languages = _lEngine.LEngineLanguageRead();

        // Every flag is asked for at once rather than one after the next: each is a first-time
        // download with its own ten-second timeout, and awaiting them in turn made the menu wait for
        // the sum of them. The rows are added afterwards, in the order the packs were read.
        string?[] paths = await Task.WhenAll(
            languages.Select(language => _lEngine.LEngineFlagRead(language, CancellationToken.None)));

        for (int index = 0; index < languages.Count; index++)
        {
            string? path = paths[index];
            ImageSource? flag = path is not null && File.Exists(path) ? PLangcodeFlagResolve(path) : null;
            _pLangcodeList.Add(new PLangcodeItem(languages[index], flag));
        }

        // A form standing on an entry keeps that entry's language even when no pack answers to it;
        // the fallback is for a form that stands on nothing and would otherwise name a pack that is
        // not installed.
        if (!_pLangcodeEntry && languages.Count > 0 && !languages.Contains(_pLangcodeChoice))
        {
            _pLangcodeChoice = languages[0];
        }

        PLangcodeBaseName.Text = _pLangcodeChoice;
        PLangcodeFlagUpdate();
    }

    internal void PLangcodeHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PLangcodeItem { PLangcodeItemName: string language } })
        {
            return;
        }

        if (!string.Equals(_pLangcodeChoice, language, StringComparison.Ordinal))
        {
            PRecordingClear();
            _pLangcodeChoice = language;
            // Chosen by hand, so the language is no longer the loaded entry's.
            _pLangcodeEntry = false;
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
}
