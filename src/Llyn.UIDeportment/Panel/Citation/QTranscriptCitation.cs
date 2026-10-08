using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QTranscriptCitation
{
    private readonly UserControl _qTranscriptCitationScope;

    private readonly QDrawer _qDrawer;

    private string _qTranscriptCitation = string.Empty;

    private CCorpus _cCorpus = null!;

    internal QTranscriptCitation(UserControl scope)
    {
        ArgumentNullException.ThrowIfNull(scope);

        _qTranscriptCitationScope = scope;
        _qDrawer = new QDrawer(QCitationDrawer, QCitationSheet, QCitationList, QCitationPressObserve);

        QCitationField.TextChanged += QCitationTextObserve;
        QCitationField.PreviewKeyDown += QCitationKeyRefine;
        QCitationField.PreviewKeyDown += QCitationPickObserve;
        QCitationField.PreviewKeyDown += QCitationCommitObserve;
        QCitationField.PreviewKeyDown += QCitationEscapeRefine;
        QCitationField.LostKeyboardFocus += QCitationLeaveRefine;
    }

    private TextBox QCitationField => QContract.QContractFind<TextBox>(_qTranscriptCitationScope, "PCitationField");

    private Popup QCitationDrawer => QContract.QContractFind<Popup>(_qTranscriptCitationScope, "PCitationDrawer");

    private Border QCitationSheet => QContract.QContractFind<Border>(_qTranscriptCitationScope, "PCitationSheet");

    private ListBox QCitationList => QContract.QContractFind<ListBox>(_qTranscriptCitationScope, "PCitationList");

    internal void QTranscriptCitationIntroduce(CCorpus corpus)
    {
        ArgumentNullException.ThrowIfNull(corpus);

        _cCorpus = corpus;
        _cCorpus.CCorpusTranscriptChanged += QTranscriptCitationRefine;
        _cCorpus.CCorpusTranscript.CTranscriptDraftChanged += QCitationDraftRefine;
    }

    internal void QTranscriptCitationClose()
    {
        _qDrawer.QDrawerHide();
    }

    private void QTranscriptCitationRefine(CExample example)
    {
        _qTranscriptCitation = example.CExampleCitation;
        QCitationRefine();
    }

    private void QCitationDraftRefine(CExample example)
    {
        string shown = _qTranscriptCitation;
        _qTranscriptCitation = example.CExampleCitation;
        if (!string.Equals(shown, _qTranscriptCitation, StringComparison.Ordinal))
        {
            QCitationRefine();
        }
    }

    private void QCitationRefine()
    {
        QCitationField.TextChanged -= QCitationTextObserve;
        QCitationField.Text = _qTranscriptCitation;
        QCitationField.TextChanged += QCitationTextObserve;
    }

    private void QCitationShutRefine()
    {
        _qDrawer.QDrawerHide();
        QCitationRefine();
    }

    private void QCitationTextObserve(object sender, TextChangedEventArgs e)
    {
        _qDrawer.QDrawerShow(_cCorpus.CCorpusAnthology.CAnthologyCitationRead(QCitationField.Text), QCitationField);
    }

    private void QCitationKeyRefine(object sender, KeyEventArgs e)
    {
        if (!_qDrawer.QDrawerShown)
        {
            return;
        }

        if (e.Key == Key.Escape)
        {
            _qDrawer.QDrawerHide();
            e.Handled = true;
            return;
        }

        if (e.Key is Key.Down or Key.Up)
        {
            _qDrawer.QDrawerMove(e.Key == Key.Down);
            e.Handled = true;
        }
    }

    private void QCitationPickObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        if (QCitationList.SelectedItem is not QDrawerItem item)
        {
            return;
        }

        e.Handled = true;
        _cCorpus.CCorpusAnthology.CAnthologyCitationSet(item.QDrawerItemId);
        QCitationShutRefine();
    }

    private void QCitationCommitObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        e.Handled = true;
        _cCorpus.CCorpusAnthology.CAnthologyCitationSet(QCitationField.Text);
        QCitationShutRefine();
    }

    private void QCitationEscapeRefine(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Escape)
        {
            return;
        }

        e.Handled = true;
        QCitationShutRefine();
    }

    private void QCitationLeaveRefine(object sender, KeyboardFocusChangedEventArgs e)
    {
        QCitationShutRefine();
    }

    private void QCitationPressObserve(object sender, MouseButtonEventArgs e)
    {
        if (QSender.QSenderItemRead<QDrawerItem>(sender) is not QDrawerItem item)
        {
            return;
        }

        e.Handled = true;
        _cCorpus.CCorpusAnthology.CAnthologyCitationSet(item.QDrawerItemId);
        QCitationShutRefine();
    }
}
