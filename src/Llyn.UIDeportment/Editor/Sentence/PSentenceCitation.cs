using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Application;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    internal void PCitationKeyHandle(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox { DataContext: PSentence row } box || PCardSentenceFind(row) is not PCard card)
        {
            return;
        }

        if (PCandidate.IsOpen && PCandidateHandle(e.Key))
        {
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Enter)
        {
            PSentenceCitationCommit(card, row, box);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Escape)
        {
            PCandidateHide();
            PSentenceCitationReset(box);
            e.Handled = true;
        }
    }

    internal void PCitationLeaveHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is not TextBox { DataContext: PSentence row } box)
        {
            return;
        }

        if (ReferenceEquals(_pCandidateSentence, row))
        {
            PCandidateHide();
        }

        PSentenceCitationReset(box);
    }

    private static void PSentenceCitationReset(TextBox box)
    {
        if (box.DataContext is PSentence row)
        {
            box.Text = PSentence.PSentenceCitationFind(row.PSentenceCitationCatalog, row.PSentenceCitation);
        }
    }

    private static void PSentenceRevealAttach(ItemsControl list)
    {
        list.MouseEnter -= PSentenceRevealHandle;
        list.MouseEnter += PSentenceRevealHandle;
        list.MouseLeave -= PSentenceRevealHandle;
        list.MouseLeave += PSentenceRevealHandle;
        list.IsKeyboardFocusWithinChanged -= PSentenceRevealHandle;
        list.IsKeyboardFocusWithinChanged += PSentenceRevealHandle;
        PSentenceRevealApply(list);
    }

    private static void PSentenceRevealHandle(object sender, MouseEventArgs e)
    {
        PSentenceRevealApply((ItemsControl)sender);
    }

    private static void PSentenceRevealHandle(object sender, DependencyPropertyChangedEventArgs e)
    {
        PSentenceRevealApply((ItemsControl)sender);
    }

    private static void PSentenceRevealApply(ItemsControl list)
    {
        bool shown = list.IsMouseOver || list.IsKeyboardFocusWithin;
        foreach (object item in list.Items)
        {
            if (list.ItemContainerGenerator.ContainerFromItem(item) is not FrameworkElement container)
            {
                continue;
            }

            foreach (string name in new[] { "PSentenceControl", "PSentenceGlossControl" })
            {
                if (PLook.PLookPartFind<FrameworkElement>(container, name) is not FrameworkElement control)
                {
                    continue;
                }

                if (shown)
                {
                    control.Opacity = 1;
                    control.IsHitTestVisible = true;
                }
                else
                {
                    control.ClearValue(UIElement.OpacityProperty);
                    control.ClearValue(UIElement.IsHitTestVisibleProperty);
                }
            }

            if (PLook.PLookPartFind<TextBox>(container, "PSentenceCitation") is TextBox citation)
            {
                citation.Opacity = PLook.PLookFirstRead(!shown && citation.Text.Length == 0, 0.0, 1.0);
            }
        }
    }

    private void PSentenceCitationCommit(PCard card, PSentence row, TextBox box)
    {
        PCandidateHide();
        try
        {
            _lEditor.LEditorCard.LCardCitationSet(card.PCardId, row.PSentenceRow, box.Text);
        }
        catch (Exception exception)
        {
            _pEditorHost.PWindowFailureShow("Reference.CreateFailed", exception);
        }

        PSentenceCitationReset(box);
    }

    private void PSentenceCitationSend(PCard card, PSentence row, long reference)
    {
        PEditorRequestSend(new LRequestSentenceReference(PEditorDraft, card.PCardId, row.PSentenceRow, reference));
    }

    private void PSentenceCitationShow()
    {
        foreach (PCard card in _pMeaningList)
        {
            PSentenceCitationShow(card);
        }

        foreach (PCard card in _pCollocationList)
        {
            PSentenceCitationShow(card);
        }
    }

    private static void PSentenceCitationShow(PCard card)
    {
        foreach (PSentence row in card.PCardSentence)
        {
            row.PSentenceCitationShow();
        }
    }
}
