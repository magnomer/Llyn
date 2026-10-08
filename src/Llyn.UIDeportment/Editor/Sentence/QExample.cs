using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Llyn.Conduct;

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

        QExampleRowRefine(container, row);
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
            row.PSentenceFrame.PSentenceFrameVisible = QLook.QLookCheckedRead(opening.IsChecked);
        }
    }

    private static void QExampleRowRefine(FrameworkElement container, PSentence row)
    {
        ArgumentNullException.ThrowIfNull(row);

        PSentenceFrame frame = row.PSentenceFrame;
        if (QLook.QLookPartFind<ToggleButton>(container, "PSentenceOpening") is ToggleButton opening)
        {
            opening.IsChecked = frame.PSentenceFrameVisible;
            opening.Visibility = QLook.QLookVisibleRead(!frame.PSentenceFrameWritten);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PSentenceSign") is TextBlock sign)
        {
            sign.Text = QLook.QLookFirstRead(frame.PSentenceFrameVisible, "−", "+");
        }

        if (QLook.QLookPartFind<StackPanel>(container, "PSentenceFrame") is StackPanel panel)
        {
            panel.Visibility = QLook.QLookVisibleRead(frame.PSentenceFrameVisible);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PSentenceGap") is TextBlock gap)
        {
            gap.Text = frame.PSentenceFrameGap;
        }

        QExampleChoiceRefine(
            container, "PSentenceParticle", frame.PSentenceFrameParticle, row.PSentenceParticleCatalog);
        QExampleChoiceRefine(
            container,
            "PSentenceDependence",
            frame.PSentenceFrameDependence,
            row.PSentenceDependenceCatalog);
        if (QLook.QLookPartFind<Grid>(container, "PSentenceParticleField") is Grid particle)
        {
            Grid.SetColumn(particle, frame.PSentenceFrameOrder.CSentenceOrderParticle * 2);
        }

        if (QLook.QLookPartFind<Grid>(container, "PSentenceDependenceField") is Grid dependence)
        {
            Grid.SetColumn(dependence, frame.PSentenceFrameOrder.CSentenceOrderDependence * 2);
        }

        if (QLook.QLookPartFind<TextBox>(container, "PSentenceText") is TextBox text)
        {
            text.Text = row.PSentenceText.CStateWordingText;
            QStateConverter.QStateHintRefine(text, QField.QFieldHintProperty, row.PSentenceText);
            QExampleLayoutRefine(container, text);
        }

        if (QLook.QLookPartFind<TextBox>(container, "PSentenceCitation") is TextBox citation)
        {
            citation.Text = PSentence.PSentenceCitationFind(row.PSentenceCitationCatalog, row.PSentenceCitation);
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PSentenceGlossIcon") is QIconImage gloss)
        {
            gloss.QIconSource = QIcon.QIconResolve("gloss", 12);
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PSentenceAddIcon") is QIconImage add)
        {
            add.QIconSource = QIcon.QIconResolve("add", 12);
        }

        if (QLook.QLookPartFind<QIconImage>(container, "PSentenceRemoveIcon") is QIconImage remove)
        {
            remove.QIconSource = QIcon.QIconResolve("remove", 12);
        }
    }

    private static void QExampleChoiceRefine(
        FrameworkElement container, string name, CStateWording value, ObservableCollection<string> catalog)
    {
        string shown = value.CStateWordingText;
        if (QLook.QLookPartFind<ComboBox>(container, name) is ComboBox choice)
        {
            choice.ItemsSource = catalog;
            choice.Text = shown;
            QStateConverter.QStateHintRefine(choice, QField.QFieldHintProperty, value);
        }

        if (QLook.QLookPartFind<TextBlock>(container, name + "Ghost") is TextBlock ghost)
        {
            ghost.Text = shown;
        }

        if (QLook.QLookPartFind<TextBlock>(container, name + "Hint") is TextBlock prompt)
        {
            QStateConverter.QStateHintRefine(prompt, TextBlock.TextProperty, value);
            prompt.Visibility = QLook.QLookVisibleRead(shown.Length == 0);
        }
    }

    private static void QExampleLayoutRefine(FrameworkElement container, TextBox text)
    {
        if (QLook.QLookPartFind<TextBlock>(container, "PSentenceLead") is not TextBlock lead
            || QLook.QLookPartFind<Grid>(container, "PSentenceBody") is not Grid body
            || QLook.QLookPartFind<TextBox>(container, "PSentenceCitation") is not TextBox citation
            || BindingOperations.IsDataBound(body, FrameworkElement.MarginProperty))
        {
            return;
        }

        foreach (FrameworkElement target in new FrameworkElement[] { body, citation })
        {
            MultiBinding inset = new() { Converter = new QFontConverter() };
            inset.Bindings.Add(new Binding(nameof(TextBlock.FontFamily)) { Source = lead });
            inset.Bindings.Add(new Binding(nameof(TextBlock.FontSize)) { Source = lead });
            inset.Bindings.Add(new Binding(nameof(TextBox.FontFamily)) { Source = text });
            inset.Bindings.Add(new Binding(nameof(TextBox.FontSize)) { Source = text });
            target.SetBinding(FrameworkElement.MarginProperty, inset);
        }

        citation.SetBinding(Control.FontFamilyProperty, new Binding(nameof(TextBox.FontFamily)) { Source = text });
        citation.SetBinding(Control.FontSizeProperty, new Binding(nameof(TextBox.FontSize)) { Source = text });
        foreach (string name in new[] { "PSentenceParticle", "PSentenceDependence" })
        {
            if (QLook.QLookPartFind<ComboBox>(container, name) is ComboBox choice
                && QLook.QLookPartFind<Grid>(container, name + "Field") is Grid field)
            {
                choice.SetBinding(
                    FrameworkElement.WidthProperty, new Binding(nameof(Grid.ActualWidth)) { Source = field });
                choice.SetBinding(
                    FrameworkElement.HeightProperty, new Binding(nameof(Grid.ActualHeight)) { Source = field });
            }
        }
    }
}
