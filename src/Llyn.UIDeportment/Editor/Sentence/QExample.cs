using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Llyn.UIDeportment;

internal sealed class QExample
{
    private readonly QSentence _qExampleSentence;

    private readonly QGloss _qExampleGloss;

    private readonly QCitation _qExampleCitation;

    internal QExample(QSentence sentence, QGloss gloss, QCitation citation)
    {
        _qExampleSentence = sentence;
        _qExampleGloss = gloss;
        _qExampleCitation = citation;
    }

    internal void QExampleRefine(FrameworkElement container, object item)
    {
        if (item is not PSentence row)
        {
            return;
        }

        PSentence.PSentenceRowApply(container, row);
        if (ItemsControl.ItemsControlFromItemContainer(container) is ItemsControl list)
        {
            QExampleRevealRefine(list);
        }
    }

    internal void QExampleIntroduce(FrameworkElement container, object item)
    {
        if (item is not PSentence row)
        {
            return;
        }

        QSentence sentence = _qExampleSentence;
        if (QLook.QLookPartFind<Grid>(container, "PSentenceReach") is Grid reach && reach.CommandBindings.Count == 0)
        {
            reach.CommandBindings.Add(new CommandBinding(
                PMentionCommand.PMentionCommandLink, sentence.QSentenceLinkRefine, sentence.QSentenceSpanRefine));
            reach.CommandBindings.Add(new CommandBinding(
                PMentionCommand.PMentionCommandChoose, sentence.QSentenceMeaningRefine, sentence.QSentenceSenseRefine));
            reach.CommandBindings.Add(new CommandBinding(
                PMentionCommand.PMentionCommandSilence,
                sentence.QSentenceSilenceObserve,
                sentence.QSentenceSpanRefine));
            reach.CommandBindings.Add(new CommandBinding(
                PMentionCommand.PMentionCommandUnlink,
                sentence.QSentenceUnlinkObserve,
                sentence.QSentenceUnlinkRefine));
            reach.CommandBindings.Add(new CommandBinding(
                PGlossCommand.PGlossCommandRemoval, _qExampleGloss.QGlossRemoveObserve));
        }

        if (QLook.QLookPartFind<ToggleButton>(container, "PSentenceOpening") is ToggleButton opening)
        {
            opening.Click -= QExampleOpeningRefine;
            opening.Click += QExampleOpeningRefine;
        }

        if (QLook.QLookPartFind<Button>(container, "PSentenceAdder") is Button adder)
        {
            adder.Click -= sentence.QSentenceAddObserve;
            adder.Click += sentence.QSentenceAddObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PSentenceEraser") is Button eraser)
        {
            eraser.Click -= sentence.QSentenceRemoveObserve;
            eraser.Click += sentence.QSentenceRemoveObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PSentenceGlossChooser") is Button gloss)
        {
            gloss.Click -= _qExampleGloss.QGlossAddObserve;
            gloss.Click += _qExampleGloss.QGlossAddObserve;
        }

        if (QLook.QLookPartFind<TextBox>(container, "PSentenceText") is TextBox text)
        {
            text.TextChanged -= sentence.QSentenceTextObserve;
            text.TextChanged += sentence.QSentenceTextObserve;
        }

        if (QLook.QLookPartFind<ComboBox>(container, "PSentenceParticle") is ComboBox particle)
        {
            particle.RemoveHandler(
                TextBoxBase.TextChangedEvent, (TextChangedEventHandler)sentence.QSentenceParticleObserve);
            particle.AddHandler(
                TextBoxBase.TextChangedEvent, (TextChangedEventHandler)sentence.QSentenceParticleObserve);
        }

        if (QLook.QLookPartFind<ComboBox>(container, "PSentenceDependence") is ComboBox dependence)
        {
            dependence.RemoveHandler(
                TextBoxBase.TextChangedEvent, (TextChangedEventHandler)sentence.QSentenceDependenceObserve);
            dependence.AddHandler(
                TextBoxBase.TextChangedEvent, (TextChangedEventHandler)sentence.QSentenceDependenceObserve);
        }

        if (QLook.QLookPartFind<TextBox>(container, "PSentenceCitation") is TextBox citation)
        {
            citation.TextChanged -= sentence.QSentenceCitationRefine;
            citation.TextChanged += sentence.QSentenceCitationRefine;
        }

        _qExampleCitation.QCitationApply(container);

        if (QLook.QLookPartFind<ItemsControl>(container, "PSentenceMentionLine") is ItemsControl mention)
        {
            mention.ItemsSource = row.PSentenceChip.PMentionLineChip;
            QLookItem.QLookItemAttach(mention, PMentionChip.PMentionChipRefine);
        }

        if (QLook.QLookPartFind<ItemsControl>(container, "PSentenceGlossLine") is ItemsControl glosses)
        {
            glosses.ItemsSource = row.PSentenceGloss;
            QLookItem.QLookItemAttach(glosses, PGloss.PGlossRowApply);
        }
    }

    internal static void QExampleRevealIntroduce(ItemsControl list)
    {
        list.MouseEnter -= QExampleRevealRefine;
        list.MouseEnter += QExampleRevealRefine;
        list.MouseLeave -= QExampleRevealRefine;
        list.MouseLeave += QExampleRevealRefine;
        list.IsKeyboardFocusWithinChanged -= QExampleRevealRefine;
        list.IsKeyboardFocusWithinChanged += QExampleRevealRefine;
    }

    private static void QExampleRevealRefine(object sender, MouseEventArgs e)
    {
        QExampleRevealRefine((ItemsControl)sender);
    }

    private static void QExampleRevealRefine(object sender, DependencyPropertyChangedEventArgs e)
    {
        QExampleRevealRefine((ItemsControl)sender);
    }

    internal static void QExampleRevealRefine(ItemsControl list)
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

            if (item is PSentence row
                && QLook.QLookPartFind<TextBox>(container, "PSentenceCitation") is TextBox citation)
            {
                citation.Opacity = QLook.QLookFirstRead(shown || row.PSentenceCited, 1.0, 0.0);
            }
        }
    }

    private void QExampleOpeningRefine(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton { DataContext: PSentence row } opening)
        {
            row.PSentenceFrameVisible = QLook.QLookCheckedRead(opening.IsChecked);
        }
    }
}
