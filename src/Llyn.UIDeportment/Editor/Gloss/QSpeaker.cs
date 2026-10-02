using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QSpeaker
{
    private readonly FrameworkElement _qSpeakerSurface;

    private readonly ObservableCollection<PLanguageItem> _qSpeakerLanguage;

    private readonly QLink _qSpeakerLink;

    private CEditor _cEditor = null!;

    private CAtelier _cAtelier = null!;

    internal QSpeaker(FrameworkElement surface, ObservableCollection<PLanguageItem> language, QLink link)
    {
        _qSpeakerSurface = surface;
        _qSpeakerLanguage = language;
        _qSpeakerLink = link;
        QSpeakerList.ItemsSource = _qSpeakerLanguage;
        QLookItem.QLookItemAttach(QSpeakerList, QSpeakerApply);
        QChoice.QChoiceDropperAttach(QSpeakerSwitch, QSpeakerPopup, QSpeakerSwitch);
    }

    private ToggleButton QSpeakerSwitch => QContract.QContractFind<ToggleButton>(_qSpeakerSurface, "PSpeaker");

    private Image QSpeakerFlag => QContract.QContractFind<Image>(_qSpeakerSurface, "PSpeakerFlag");

    private Ellipse QSpeakerGlobe => QContract.QContractFind<Ellipse>(_qSpeakerSurface, "PSpeakerGlobe");

    private TextBlock QSpeakerName => QContract.QContractFind<TextBlock>(_qSpeakerSurface, "PSpeakerName");

    private Popup QSpeakerPopup => QContract.QContractFind<Popup>(_qSpeakerSurface, "PLanguage");

    private ItemsControl QSpeakerList => QContract.QContractFind<ItemsControl>(_qSpeakerSurface, "PLanguageList");

    internal void QSpeakerIntroduce(CEditor editor, CAtelier atelier)
    {
        _cEditor = editor;
        _cAtelier = atelier;
        editor.CEditorDraftChanged += QSpeakerRefine;
        atelier.CAtelierWorkspace.CWorkspaceOpened += QSpeakerLanguageRefine;
    }

    private void QSpeakerRefine(CEntryDraft _)
    {
        QSpeakerName.Text = _cEditor.CEditorLanguage;
        QSpeakerFlagRefine();
    }

    private async void QSpeakerLanguageRefine()
    {
        PLanguageItem.PLanguageItemReset(
            _qSpeakerLanguage,
            await _cAtelier.CAtelierCatalog.CCatalogEnsignLoad(QEnsignImage.QEnsignDraw));
        QSpeakerEnsignRefine();
        _qSpeakerLink.QLinkFlagRefine();
    }

    private void QSpeakerObserve(object sender, RoutedEventArgs e)
    {
        _cEditor.CEditorLanguageSet(PLanguageItem.PLanguageNameRead(sender));
        QSpeakerChoiceRefine();
    }

    private void QSpeakerChoiceRefine()
    {
        QSpeakerSwitch.IsChecked = false;
    }

    private async void QSpeakerFlagRefine()
    {
        await _cAtelier.CAtelierCatalog.CCatalogEnsignLoad(QEnsignImage.QEnsignDraw);
        QSpeakerEnsignRefine();
    }

    private void QSpeakerEnsignRefine()
    {
        QEnsignImage.QEnsignFlagRefine(QSpeakerFlag, QSpeakerGlobe, _cEditor.CEditorLanguage);
    }

    private void QSpeakerApply(FrameworkElement container, object item, string? change)
    {
        PLanguageItem.PLanguageItemApply(container, item, change);

        if (item is PLanguageItem language
            && QLook.QLookPartFind<Ellipse>(container, "PSpeakerGlobe") is Ellipse globe)
        {
            globe.Visibility = QLook.QLookVisibleRead(language.PLanguageItemFlag is null);
        }

        if (QLook.QLookPartFind<Button>(container, "PSpeakerChoice") is Button choice)
        {
            choice.Click -= QSpeakerObserve;
            choice.Click += QSpeakerObserve;
        }
    }
}
