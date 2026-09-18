using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace Llyn.UIShell;

public partial class PEditor
{
    private readonly ObservableCollection<PLanguageItem> _pLanguageItem = [];

    private string PSpeakerLanguageRead()
    {
        return _pEditorTenure?.LTenureLanguageRead() ?? string.Empty;
    }

    internal async void PSpeakerLoad()
    {
        _pLanguageItem.Clear();
        var languages = _lEngine.LEngineLanguageRead();

        await PEnsign.PEnsignLoad(_lEngine);

        foreach (string language in languages)
        {
            _pLanguageItem.Add(new PLanguageItem(language, PEnsign.PEnsignFind(language)));
        }

        string chosen = PSpeakerLanguageRead();
        PSpeakerName.Text = chosen;
        PHeadwordFontApply(chosen);
        PEditorContourApply(chosen);
        PEditorSilentApply(chosen);
        PSpeakerFlagUpdate();
        PLinkFlagUpdate();
        PCategoryLoad();
        PSentenceFrameLoad(chosen);
    }

    internal void PSpeakerHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PLanguageItem { PLanguageItemName: string language } })
        {
            return;
        }

        PSpeakerName.Text = language;
        PEditorLanguageSend();
        PCategoryLoad();
        PSentenceFrameLoad(language);
        PEditorExampleShow(language);
        PHeadwordFontApply(language);
        PEditorContourApply(language);
        PEditorSilentApply(language);
        PSpeakerFlagUpdate();
        PSpeaker.IsChecked = false;
    }

    private async void PSpeakerFlagUpdate()
    {
        string chosen = PSpeakerLanguageRead();
        await PEnsign.PEnsignLoad(_lEngine);

        if (chosen != PSpeakerLanguageRead())
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
