using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Llyn.UIShell;

public partial class PEditor
{
    private const string PWindowLanguage = "English";

    private readonly ObservableCollection<PTongueItem> _pTongueItem = [];

    private string _pLanguageChoice = PWindowLanguage;

    private bool _pLanguageEntry;

    internal async void PLanguageLoad()
    {
        _pTongueItem.Clear();
        var languages = _lEngine.LEngineLanguageRead();

        await PEnsign.PEnsignLoad(_lEngine);

        foreach (string language in languages)
        {
            _pTongueItem.Add(new PTongueItem(language, PEnsign.PEnsignFind(language)));
        }

        if (!_pLanguageEntry && languages.Count > 0 && !languages.Contains(_pLanguageChoice))
        {
            _pLanguageChoice = languages[0];
            PEditorStateUpdate();
        }

        PLanguageName.Text = _pLanguageChoice;
        PLanguageFlagUpdate();
        PLinkFlagUpdate();
        PCategoryLoad();
    }

    internal void PLanguageHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PTongueItem { PTongueItemName: string language } })
        {
            return;
        }

        if (!string.Equals(_pLanguageChoice, language, StringComparison.Ordinal))
        {
            PRecordingClear();
            _pLanguageChoice = language;
            _pLanguageEntry = false;
            PCategoryLoad();
        }
        PLanguageName.Text = language;
        PLanguageFlagUpdate();
        PLanguage.IsChecked = false;
    }

    private async void PLanguageFlagUpdate()
    {
        string chosen = _pLanguageChoice;
        await PEnsign.PEnsignLoad(_lEngine);

        if (chosen != _pLanguageChoice)
        {
            return;
        }

        ImageSource? flag = PEnsign.PEnsignFind(chosen);
        if (flag is not null)
        {
            PLanguageFlag.Source = flag;
            PLanguageFlag.Visibility = Visibility.Visible;
            PLanguageGlobe.Visibility = Visibility.Collapsed;
        }
        else
        {
            PLanguageFlag.Source = null;
            PLanguageFlag.Visibility = Visibility.Collapsed;
            PLanguageGlobe.Visibility = Visibility.Visible;
        }
    }
}
