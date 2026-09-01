using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Llyn.UIShell;

public partial class PEditor
{
    private const string PWindowLanguage = "English";

    private readonly ObservableCollection<PLangcodeItem> _pLangcodeItem = [];

    private string _pLangcodeChoice = PWindowLanguage;

    private bool _pLangcodeEntry;

    internal async void PLangcodeLoad()
    {
        _pLangcodeItem.Clear();
        var languages = _lEngine.LEngineLanguageRead();

        string?[] paths = await Task.WhenAll(
            languages.Select(language => _lEngine.LEngineFlagRead(language, CancellationToken.None)));

        for (int index = 0; index < languages.Count; index++)
        {
            string? path = paths[index];
            ImageSource? flag = path is not null && File.Exists(path)
                ? PLangcodeIndicator.PLangcodeIndicatorResolve(path)
                : null;
            _pLangcodeItem.Add(new PLangcodeItem(languages[index], flag));
        }

        if (!_pLangcodeEntry && languages.Count > 0 && !languages.Contains(_pLangcodeChoice))
        {
            _pLangcodeChoice = languages[0];
        }

        PLangcodeBaseName.Text = _pLangcodeChoice;
        PLangcodeFlagUpdate();
        PSpeechLoad();
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
            _pLangcodeEntry = false;
            PSpeechLoad();
        }
        PLangcodeBaseName.Text = language;
        PLangcodeFlagUpdate();
        PLangcodeBase.IsChecked = false;
    }

    private async void PLangcodeFlagUpdate()
    {
        string chosen = _pLangcodeChoice;
        string? path = await _lEngine.LEngineFlagRead(chosen, CancellationToken.None);

        if (chosen != _pLangcodeChoice)
        {
            return;
        }

        DrawingImage? flag = path is not null && File.Exists(path)
            ? PLangcodeIndicator.PLangcodeIndicatorResolve(path)
            : null;
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
}
