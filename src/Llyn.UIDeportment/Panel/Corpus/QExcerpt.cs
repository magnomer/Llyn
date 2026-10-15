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

    private readonly ObservableCollection<QRendition> _qExcerptGloss = [];

    private ObservableCollection<QLanguageItem> _qLanguageItem = [];

    private CCorpus _cCorpus = null!;

    internal QExcerpt(UserControl scope)
    {
        ArgumentNullException.ThrowIfNull(scope);

        _qExcerptScope = scope;

        QExcerptText.QMentionClick += QExcerptMentionObserve;
    }

    internal event Action<QMention, CMentionOffer?>? QExcerptMentionOffered;

    private Grid QExcerptView => QContract.QContractFind<Grid>(_qExcerptScope, "PExcerpt");

    private StackPanel QExcerptBody => QContract.QContractFind<StackPanel>(_qExcerptScope, "PExcerptBody");

    private QMention QExcerptText => QContract.QContractFind<QMention>(_qExcerptScope, "PExcerptText");

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

    internal void QExcerptIntroduce(CCorpus corpus, ObservableCollection<QLanguageItem> language)
    {
        ArgumentNullException.ThrowIfNull(corpus);
        ArgumentNullException.ThrowIfNull(language);

        _cCorpus = corpus;
        _qLanguageItem = language;
        _cCorpus.CCorpusExampleChanged += QExcerptRefine;
        _cCorpus.CCorpusAnthology.CAnthologyPanel.CPanelAperture.CApertureRowsChanged += QExcerptTallyRefine;

        QExcerptGloss.ItemsSource = _qExcerptGloss;
        QLookItem.QLookItemAttach(QExcerptGloss, QRendition.QRenditionRowApply);
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
        QExcerptText.QMentionPieces = example.CExampleWording is string key
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
            _qExcerptGloss.Add(new QRendition(_qLanguageItem, draft));
        }

        QExcerptGlossSection.Visibility = glosses.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    private void QExcerptCitationRefine(CExample example)
    {
        QExcerptCitation.Text = example.CExampleCitation;
        QExcerptCitationSection.Visibility = example.CExampleSource is null ? Visibility.Collapsed : Visibility.Visible;
    }

    private void QExcerptMentionObserve(object? sender, QMentionArgument e)
    {
        QExcerptMentionOffered?.Invoke(
            QExcerptText,
            _cCorpus.CCorpusMentionFind(e.QMentionArgumentText, e.QMentionArgumentUnit));
    }
}
