using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QTranscriptSpeaker
{
    private readonly UserControl _qTranscriptSpeakerScope;

    private CCorpus _cCorpus = null!;

    private CAtelier _cAtelier = null!;

    private CEnvoy _qTranscriptSpeakerEnvoy = null!;

    internal QTranscriptSpeaker(UserControl scope)
    {
        ArgumentNullException.ThrowIfNull(scope);

        _qTranscriptSpeakerScope = scope;

        QChoice.QChoiceDropperAttach(QSpeaker, QLanguage, QSpeaker);
        QSpeakerIcon.QIconSource = QIcon.QIconResolve("expand", 12);
        QLookItem.QLookItemAttach(QLanguageList, QSpeakerApply);
    }

    internal ObservableCollection<PLanguageItem> QTranscriptSpeakerLanguage { get; } = [];

    private ToggleButton QSpeaker => QContract.QContractFind<ToggleButton>(_qTranscriptSpeakerScope, "PSpeaker");

    private Image QSpeakerFlag => QContract.QContractFind<Image>(_qTranscriptSpeakerScope, "PSpeakerFlag");

    private TextBlock QSpeakerName => QContract.QContractFind<TextBlock>(_qTranscriptSpeakerScope, "PSpeakerName");

    private QIconImage QSpeakerIcon =>
        QContract.QContractFind<QIconImage>(_qTranscriptSpeakerScope, "PSpeakerIcon");

    private Popup QLanguage => QContract.QContractFind<Popup>(_qTranscriptSpeakerScope, "PLanguage");

    private ItemsControl QLanguageList =>
        QContract.QContractFind<ItemsControl>(_qTranscriptSpeakerScope, "PLanguageList");

    internal void QTranscriptSpeakerIntroduce(CCorpus corpus, CAtelier atelier, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(corpus);
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(envoy);

        _cCorpus = corpus;
        _cAtelier = atelier;
        _qTranscriptSpeakerEnvoy = envoy;
        _cCorpus.CCorpusTranscriptChanged += QSpeakerShow;
        _cCorpus.CCorpusTranscript.CTranscriptDraftChanged += QSpeakerShow;
        _cCorpus.CCorpusWorkspaceChanged += QSpeakerWorkspaceRefine;

        QLanguageList.ItemsSource = QTranscriptSpeakerLanguage;
    }

    internal void QTranscriptSpeakerClose()
    {
        QLanguage.IsOpen = false;
    }

    private async void QSpeakerWorkspaceRefine()
    {
        QSpeakerRefine(
            await _cAtelier.CAtelierCatalog.CCatalogEnsignLoad(
                _qTranscriptSpeakerEnvoy, QEnsignImage.QEnsignDraw));
    }

    internal void QSpeakerRefine(IReadOnlyList<string> languages)
    {
        PLanguageItem.PLanguageItemReset(QTranscriptSpeakerLanguage, languages);
    }

    private void QSpeakerObserve(object sender, RoutedEventArgs e)
    {
        if (QSender.QSenderItemRead<PLanguageItem>(sender) is not PLanguageItem item)
        {
            return;
        }

        _cCorpus.CCorpusAnthology.CAnthologySpeakerSet(item.PLanguageItemName);
        QSpeakerDropperRefine();
    }

    private void QSpeakerDropperRefine()
    {
        QSpeaker.IsChecked = false;
    }

    private void QSpeakerApply(FrameworkElement container, object item, string? change)
    {
        PLanguageItem.PLanguageItemApply(container, item, change);

        if (QLook.QLookPartFind<Button>(container, "PSpeakerChoice") is Button choice)
        {
            choice.Click -= QSpeakerObserve;
            choice.Click += QSpeakerObserve;
        }
    }

    private void QSpeakerShow(CExample example)
    {
        QSpeakerName.Text = example.CExampleLanguage;
        QSpeakerFlag.Source = QEnsignImage.QEnsignRead(example.CExampleLanguage);
    }
}
