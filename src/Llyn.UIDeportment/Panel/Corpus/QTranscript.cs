using System;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QTranscript
{
    private readonly UserControl _qTranscriptScope;

    private CCorpus _cCorpus = null!;

    internal QTranscript(UserControl scope)
    {
        ArgumentNullException.ThrowIfNull(scope);

        _qTranscriptScope = scope;

        QTranscriptAttach();
    }

    private Grid QTranscriptView => QContract.QContractFind<Grid>(_qTranscriptScope, "PTranscript");

    private TextBox QTranscriptText => QContract.QContractFind<TextBox>(_qTranscriptScope, "PTranscriptText");

    private TextBlock QTranscriptTally => QContract.QContractFind<TextBlock>(_qTranscriptScope, "PTranscriptTally");

    internal void QTranscriptDeskIntroduce(CCorpus corpus)
    {
        ArgumentNullException.ThrowIfNull(corpus);

        _cCorpus = corpus;
        _cCorpus.CCorpusTranscriptChanged += QTranscriptRefine;
        _cCorpus.CCorpusTranscript.CTranscriptDraftChanged += QTranscriptDraftRefine;
        _cCorpus.CCorpusAnthology.CAnthologyPanel.CPanelAperture.CApertureRowsChanged += QTranscriptTallyRefine;
    }

    internal void QTranscriptVisibleRefine()
    {
        QTranscriptView.Visibility = QLook.QLookVisibleRead(_cCorpus.CCorpusDiptych.CDiptychParentEditing);
        QTranscriptView.IsEnabled = _cCorpus.CCorpusTranscriptEnabled;
    }

    private void QTranscriptAttach()
    {
        QTranscriptText.TextChanged += QTranscriptTextObserve;
    }

    private void QTranscriptTeardown()
    {
        QTranscriptText.TextChanged -= QTranscriptTextObserve;
    }

    private void QTranscriptTextObserve(object sender, TextChangedEventArgs e)
    {
        QTranscriptHintRefine(_cCorpus.CCorpusAnthology.CAnthologyTextSet(QTranscriptText.Text));
    }

    private void QTranscriptHintRefine(string hint)
    {
        QTranscriptText.SetValue(QField.QFieldHintProperty, QLocalizationCatalog.QLocalizationTextRead(hint));
    }

    private void QTranscriptRefine(CExample example)
    {
        QTranscriptTeardown();

        QTranscriptText.Text = example.CExampleText.CStateValueText;
        QTranscriptHintRefine(example.CExampleTextHint);
        QTranscriptTally.Text = example.CExampleTally;

        QTranscriptAttach();
    }

    private void QTranscriptDraftRefine(CExample example)
    {
        QTranscriptTeardown();

        if (!example.CExampleTextKept)
        {
            QTranscriptText.Text = example.CExampleText.CStateValueText;
        }

        QTranscriptHintRefine(example.CExampleTextHint);

        QTranscriptAttach();
    }

    private void QTranscriptTallyRefine()
    {
        QTranscriptTally.Text = _cCorpus.CCorpusAnthology.CAnthologyPanel.CPanelAperture.CApertureTallyRead();
    }
}
