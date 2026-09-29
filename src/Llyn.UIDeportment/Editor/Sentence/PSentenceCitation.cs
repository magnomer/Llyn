using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public partial class PEditor
{
    private readonly ObservableCollection<QCitationItem> _pEditorCitation = [];

    private void PCitationCommitObserve(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter
            || sender is not TextBox { DataContext: PSentence row } box
            || PCardSentenceFind(row) is not PCard card)
        {
            return;
        }

        e.Handled = true;
        _qEditor.QEditorArea.CEditorCard.CCardCitationSet(card.PCardId, row.PSentenceRow, box.Text);
        PProfferShutRefine();
        PCitationTextRefine(box);
    }

    private void PCitationEscapeRefine(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Escape || sender is not TextBox { DataContext: PSentence } box)
        {
            return;
        }

        e.Handled = true;
        PCitationTextRefine(box);
    }

    private void PCitationLeaveRefine(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is not TextBox { DataContext: PSentence } box)
        {
            return;
        }

        PProfferShutRefine();
        PCitationTextRefine(box);
    }

    private static void PCitationTextRefine(TextBox box)
    {
        if (box.DataContext is PSentence row)
        {
            box.Text = PSentence.PSentenceCitationFind(row.PSentenceCitationCatalog, row.PSentenceCitation);
        }
    }

    private static void PSentenceRevealAttach(ItemsControl list)
    {
        list.MouseEnter -= PSentenceRevealRefine;
        list.MouseEnter += PSentenceRevealRefine;
        list.MouseLeave -= PSentenceRevealRefine;
        list.MouseLeave += PSentenceRevealRefine;
        list.IsKeyboardFocusWithinChanged -= PSentenceRevealRefine;
        list.IsKeyboardFocusWithinChanged += PSentenceRevealRefine;
        PSentenceRevealRefine(list);
    }

    private static void PSentenceRevealRefine(object sender, MouseEventArgs e)
    {
        PSentenceRevealRefine((ItemsControl)sender);
    }

    private static void PSentenceRevealRefine(object sender, DependencyPropertyChangedEventArgs e)
    {
        PSentenceRevealRefine((ItemsControl)sender);
    }

    internal static void PSentenceRevealRefine(ItemsControl list)
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
                if (QLook.QLookPartFind<FrameworkElement>(container, name) is not FrameworkElement control)
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

            if (QLook.QLookPartFind<TextBox>(container, "PSentenceCitation") is TextBox citation)
            {
                citation.Opacity = QLook.QLookFirstRead(!shown && citation.Text.Length == 0, 0.0, 1.0);
            }
        }
    }

    internal void PSentenceCitationRefine()
    {
        _pEditorCitation.Clear();
        foreach (CCatalogReference row in _qEditor.QEditorArea.CEditorCard.CCardReferenceRead())
        {
            _pEditorCitation.Add(QCitationItem.QCitationItemCreate(row));
        }

        foreach (PCard card in _pMeaningList.Concat(_pCollocationList))
        {
            foreach (PSentence row in card.PCardSentence)
            {
                row.PSentenceCitationShow();
            }
        }
    }
}
