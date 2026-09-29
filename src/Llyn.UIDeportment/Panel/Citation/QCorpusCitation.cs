using System.Windows.Controls;
using System.Windows.Input;

namespace Llyn.UIDeportment;

internal sealed partial class QCorpus
{
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
