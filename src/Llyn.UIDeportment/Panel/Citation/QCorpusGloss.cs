using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class QCorpus
{
    private readonly ObservableCollection<PGloss> _qTranscriptGloss = [];

    private readonly ObservableCollection<PGloss> _qExcerptGloss = [];

    private void QExcerptGlossShow(IReadOnlyList<CGlossDraft> glosses)
    {
        _qExcerptGloss.Clear();
        foreach (CGlossDraft draft in glosses)
        {
            _qExcerptGloss.Add(new PGloss(_qLanguageItem, draft));
        }

        QExcerptGlossSection.Visibility = glosses.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    private void QTranscriptGlossShow(CExample? example)
    {
        PCard.PCardRowShow(
            _qTranscriptGloss,
            example?.CExampleGloss ?? [],
            static row => row.PGlossId,
            static draft => draft.CGlossDraftId,
            draft => new PGloss(_qLanguageItem, draft),
            (row, draft) =>
            {
                row.PGlossShow(draft);
                return row;
            });

        QTranscriptSeedShow();
    }

    private void QGlossTextHandle(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox { DataContext: PGloss gloss } box)
        {
            return;
        }

        QTranscriptQuill?.LQuillGlossSet(0, 0, gloss.PGlossId, null, box.Text);
    }

    private void QGlossSpeakerHandle(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListBox { DataContext: PGloss gloss, SelectedValue: string language })
        {
            return;
        }

        QTranscriptQuill?.LQuillGlossSet(0, 0, gloss.PGlossId, language, null);
    }

    private void QGlossAddHandle(object sender, RoutedEventArgs e)
    {
        int position = QSender.QSenderItemRead<PGloss>(sender) is PGloss row
            ? _qTranscriptGloss.IndexOf(row) + 1
            : _qTranscriptGloss.Count;

        QTranscriptQuill?.LQuillGlossAdd(
            0, 0, _qCorpusHost.PWindowDeportment.LWindowWorkspace.QWorkspaceGlossRead(), position);
    }

    private void QGlossRemoveHandle(object sender, RoutedEventArgs e)
    {
        if (QSender.QSenderItemRead<PGloss>(sender) is PGloss gloss)
        {
            QTranscriptQuill?.LQuillGlossRemove(0, 0, gloss.PGlossId);
        }
    }

    private void QTranscriptGlossApply(FrameworkElement container, object item, string? change)
    {
        TextBox? field = QLook.QLookPartFind<TextBox>(container, "PGlossText");
        ListBox? list = QLook.QLookPartFind<ListBox>(container, "PGlossList");
        if (field is not null)
        {
            field.TextChanged -= QGlossTextHandle;
        }

        if (list is not null)
        {
            list.SelectionChanged -= QGlossSpeakerHandle;
        }

        PGloss.PGlossRowApply(container, item, change);

        if (field is not null)
        {
            field.TextChanged += QGlossTextHandle;
        }

        if (list is not null)
        {
            list.SelectionChanged += QGlossSpeakerHandle;
        }

        if (QLook.QLookPartFind<Button>(container, "PGlossAddition") is Button addition)
        {
            addition.Click -= QGlossAddHandle;
            addition.Click += QGlossAddHandle;
            if (addition.Content is QIconImage mark)
            {
                mark.QIconSource = QIcon.QIconResolve("add", 12);
            }
        }

        if (QLook.QLookPartFind<Button>(container, "PGlossRemoval") is Button removal)
        {
            removal.Click -= QGlossRemoveHandle;
            removal.Click += QGlossRemoveHandle;
            if (removal.Content is QIconImage mark)
            {
                mark.QIconSource = QIcon.QIconResolve("remove", 12);
            }
        }
    }

    private void QTranscriptSeedShow()
    {
        QTranscriptSeed.Visibility = _qTranscriptGloss.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void QTranscriptSeedHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (_qTranscriptGloss.Count > 0)
        {
            return;
        }

        QTranscriptQuill?.LQuillGlossAdd(
            0, 0, _qCorpusHost.PWindowDeportment.LWindowWorkspace.QWorkspaceGlossRead(), 0);

        QTranscriptGlossLine.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, () =>
        {
            if (QTranscriptGlossLine.ItemContainerGenerator.ContainerFromIndex(0) is DependencyObject container
                && PEditor.PEditorCaretFind(container) is TextBox box)
            {
                box.Focus();
            }
        });
    }
}
