using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QCorpus
{
    private void QCitationFind()
    {
        _qCitationCatalog.Clear();

        IReadOnlyList<CCatalogReference> read;
        try
        {
            read = _cCorpus.CCorpusAnthology.CAnthologyReferenceRead();
        }
        catch (Exception exception)
        {
            _qCorpusHost.PWindowFailureRefine("Reference.LoadFailed", exception);
            read = [];
        }

        foreach (CCatalogReference row in read)
        {
            _qCitationCatalog.Add(QCitationItem.QCitationItemCreate(row));
        }
    }

    private string QCitationNameRead(long id)
    {
        if (id == 0)
        {
            return string.Empty;
        }

        foreach (QCitationItem row in _qCitationCatalog)
        {
            if (row.QCitationItemId == id)
            {
                return row.QCitationItemName;
            }
        }

        return id.ToString(CultureInfo.InvariantCulture);
    }

    private void QCitationUpdate()
    {
        QCitationField.TextChanged -= QCitationTextHandle;
        QCitationField.Text = QCitationNameRead(_qTranscriptCitation);
        QCitationField.TextChanged += QCitationTextHandle;
    }

    private void QCitationTextHandle(object sender, TextChangedEventArgs e)
    {
        try
        {
            _qDrawer.QDrawerShow(_cCorpus.CCorpusAnthology.CAnthologyCitationRead(QCitationField.Text), QCitationField);
        }
        catch (Exception)
        {
            _qDrawer.QDrawerHide();
        }
    }

    private void QCitationKeyHandle(object sender, KeyEventArgs e)
    {
        e.Handled = QCitationKeyRun(e.Key, _qDrawer.QDrawerShown, _qDrawer.QDrawerChosenRead());
    }

    private bool QCitationKeyRun(Key key, bool shown, long? chosen)
    {
        if (shown && key == Key.Escape)
        {
            _qDrawer.QDrawerHide();
            return true;
        }

        if (shown && key is Key.Down or Key.Up)
        {
            _qDrawer.QDrawerMove(key == Key.Down);
            return true;
        }

        if (shown && key == Key.Enter && chosen is long reference)
        {
            QCitationSet(reference);
            return true;
        }

        if (key == Key.Enter)
        {
            QCitationCommit();
            return true;
        }

        if (key == Key.Escape)
        {
            QCitationHide();
            return true;
        }

        return false;
    }

    private void QCitationLeaveHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        QCitationHide();
    }

    private void QCitationPressHandle(object sender, MouseButtonEventArgs e)
    {
        if (QSender.QSenderItemRead<QDrawerItem>(sender) is not QDrawerItem item)
        {
            _qDrawer.QDrawerHide();
            return;
        }

        QCitationSet(item.QDrawerItemId);
        e.Handled = true;
    }

    private void QCitationSet(long reference)
    {
        _qDrawer.QDrawerHide();
        QTranscriptQuill?.LQuillReferenceSet(reference);
        QCitationUpdate();
    }

    private void QCitationCommit()
    {
        _qDrawer.QDrawerHide();
        try
        {
            _cCorpus.CCorpusAnthology.CAnthologyCitationSet(QCitationField.Text);
        }
        catch (Exception exception)
        {
            _qCorpusHost.PWindowFailureRefine("Reference.CreateFailed", exception);
        }

        QCitationUpdate();
    }

    private void QCitationHide()
    {
        _qDrawer.QDrawerHide();
        QCitationUpdate();
    }
}
