using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QExcerpt
{
    private readonly UserControl _qExcerptScope;

    private readonly ObservableCollection<PGloss> _qExcerptGloss = [];

    private ObservableCollection<PLanguageItem> _qLanguageItem = [];

    private CCorpus _cCorpus = null!;

    internal QExcerpt(UserControl scope)
    {
        ArgumentNullException.ThrowIfNull(scope);

        _qExcerptScope = scope;

        QExcerptText.PMentionClick += QExcerptMentionObserve;
    }

    internal event Action<PMention, CMentionOffer?>? QExcerptMentionOffered;

    private Grid QExcerptView => QContract.QContractFind<Grid>(_qExcerptScope, "PExcerpt");

    private StackPanel QExcerptBody => QContract.QContractFind<StackPanel>(_qExcerptScope, "PExcerptBody");

    private PMention QExcerptText => QContract.QContractFind<PMention>(_qExcerptScope, "PExcerptText");

    private Image QExcerptFlag => QContract.QContractFind<Image>(_qExcerptScope, "PExcerptFlag");

    private TextBlock QExcerptLanguage => QContract.QContractFind<TextBlock>(_qExcerptScope, "PExcerptLanguage");

    private TextBlock QExcerptTally => QContract.QContractFind<TextBlock>(_qExcerptScope, "PExcerptTally");

    private StackPanel QExcerptGlossSection =>
        QContract.QContractFind<StackPanel>(_qExcerptScope, "PExcerptGlossSection");

    private ItemsControl QExcerptGloss => QContract.QContractFind<ItemsControl>(_qExcerptScope, "PExcerptGloss");

    private StackPanel QExcerptCitationSection =>
        QContract.QContractFind<StackPanel>(_qExcerptScope, "PExcerptCitationSection");

    private TextBlock QExcerptCitation => QContract.QContractFind<TextBlock>(_qExcerptScope, "PExcerptCitation");

    private TextBlock QExcerptUnselected =>
        QContract.QContractFind<TextBlock>(_qExcerptScope, "PExcerptUnselected");

    internal void QExcerptIntroduce(CCorpus corpus, ObservableCollection<PLanguageItem> language)
    {
        ArgumentNullException.ThrowIfNull(corpus);
        ArgumentNullException.ThrowIfNull(language);

        _cCorpus = corpus;
        _qLanguageItem = language;
        _cCorpus.CCorpusExampleChanged += QExcerptRefine;
        _cCorpus.CCorpusAnthology.CAnthologyPanel.CPanelAperture.CApertureRowsChanged += QExcerptTallyRefine;

        QExcerptGloss.ItemsSource = _qExcerptGloss;
        QLookItem.QLookItemAttach(QExcerptGloss, PGloss.PGlossRowApply);
    }

    internal void QExcerptVisibleRefine()
    {
        QExcerptView.Visibility = QLook.QLookVisibleRead(_cCorpus.CCorpusDiptych.CDiptychParentShown);
        QExcerptBody.Visibility = QLook.QLookVisibleRead(_cCorpus.CCorpusExcerptHeld);
        QExcerptUnselected.Visibility = QLook.QLookVisibleRead(_cCorpus.CCorpusExcerptBlank);
    }

    private void QExcerptTallyRefine()
    {
        QExcerptTally.Text = _cCorpus.CCorpusAnthology.CAnthologyPanel.CPanelAperture.CApertureTallyRead();
    }

    private void QExcerptRefine(CExample example)
    {
        QExcerptSentenceRefine(example);
        QExcerptLanguage.Text = example.CExampleLanguage;
        QExcerptFlag.Source = QEnsignImage.QEnsignRead(example.CExampleLanguage);
        QExcerptGlossShow(example.CExampleGloss);
        QExcerptCitationRefine(example);
        QExcerptTally.Text = example.CExampleTally;
    }

    private void QExcerptSentenceRefine(CExample example)
    {
        QExcerptText.PMentionPiece = example.CExampleWording is string key
            ? QMentionPiece.QMentionPieceCreate(QLocalizationCatalog.QLocalizationTextRead(key))
            : QMentionPiece.QMentionPieceCreate(example.CExamplePiece);
        QExcerptText.SetResourceReference(
            TextBlock.ForegroundProperty,
            example.CExampleMuted ? "Theme.Muted" : "Theme.Ink");
    }

    private void QExcerptGlossShow(IReadOnlyList<CGlossDraft> glosses)
    {
        _qExcerptGloss.Clear();
        foreach (CGlossDraft draft in glosses)
        {
            _qExcerptGloss.Add(new PGloss(_qLanguageItem, draft));
        }

        QExcerptGlossSection.Visibility = glosses.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    private void QExcerptCitationRefine(CExample example)
    {
        QExcerptCitation.Text = example.CExampleCitation;
        QExcerptCitationSection.Visibility = example.CExampleSource is null ? Visibility.Collapsed : Visibility.Visible;
    }

    private void QExcerptMentionObserve(object? sender, PMentionArgument e)
    {
        QExcerptMentionOffered?.Invoke(
            QExcerptText,
            _cCorpus.CCorpusMentionFind(e.PMentionArgumentText, e.PMentionArgumentUnit));
    }
}
