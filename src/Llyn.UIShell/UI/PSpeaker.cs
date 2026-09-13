using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Llyn.UIShell;

public partial class PEditor
{
    private const string PWindowLanguage = "English";

    private readonly ObservableCollection<PLanguageItem> _pLanguageItem = [];

    private string _pSpeakerChoice = PWindowLanguage;

    private bool _pSpeakerEntry;

    internal async void PSpeakerLoad()
    {
        _pLanguageItem.Clear();
        var languages = _lEngine.LEngineLanguageRead();

        await PEnsign.PEnsignLoad(_lEngine);

        foreach (string language in languages)
        {
            _pLanguageItem.Add(new PLanguageItem(language, PEnsign.PEnsignFind(language)));
        }

        bool switched = false;
        if (!_pSpeakerEntry && languages.Count > 0 && !languages.Contains(_pSpeakerChoice))
        {
            _pSpeakerChoice = languages[0];
            switched = true;
        }

        PSpeakerName.Text = _pSpeakerChoice;
        PHeadwordFontApply(_pSpeakerChoice);
        PSpeakerFlagUpdate();
        PLinkFlagUpdate();
        PCategoryLoad();
        PSentenceFrameLoad(_pSpeakerChoice);

        if (switched)
        {
            PEditorExampleShow(_pSpeakerChoice);
            PEditorLanguageSend();
        }
    }

    internal void PSpeakerHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PLanguageItem { PLanguageItemName: string language } })
        {
            return;
        }

        if (!string.Equals(_pSpeakerChoice, language, StringComparison.Ordinal))
        {
            PRecordingClear();
            PAccentFreshClear();
            _pSpeakerChoice = language;
            _pSpeakerEntry = false;
            PCategoryLoad();
            PSentenceFrameLoad(language);
            PEditorExampleShow(language);
            PEditorLanguageSend();
            PEditorAudioSend();
        }
        PSpeakerName.Text = language;
        PHeadwordFontApply(language);
        PSpeakerFlagUpdate();
        PSpeaker.IsChecked = false;
    }

    private async void PSpeakerFlagUpdate()
    {
        string chosen = _pSpeakerChoice;
        await PEnsign.PEnsignLoad(_lEngine);

        if (chosen != _pSpeakerChoice)
        {
            return;
        }

        ImageSource? flag = PEnsign.PEnsignFind(chosen);
        if (flag is not null)
        {
            PSpeakerFlag.Source = flag;
            PSpeakerFlag.Visibility = Visibility.Visible;
            PSpeakerGlobe.Visibility = Visibility.Collapsed;
        }
        else
        {
            PSpeakerFlag.Source = null;
            PSpeakerFlag.Visibility = Visibility.Collapsed;
            PSpeakerGlobe.Visibility = Visibility.Visible;
        }
    }
}
