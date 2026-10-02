using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QSentence
{
    private readonly ObservableCollection<PCard> _qSentenceMeaning;

    private readonly ObservableCollection<PCard> _qSentenceCollocation;

    private QWindow _pSentenceHost = null!;

    private CEditor _cEditor = null!;

    private QProspect _qSentenceProspect = null!;

    private QGloss _qSentenceGloss = null!;

    private QCitation _qSentenceCitation = null!;

    internal QSentence(ObservableCollection<PCard> meaning, ObservableCollection<PCard> collocation)
    {
        _qSentenceMeaning = meaning;
        _qSentenceCollocation = collocation;
    }

    internal ObservableCollection<string> QSentenceParticle { get; } = [];

    internal ObservableCollection<string> QSentenceDependence { get; } = [];

    internal CSentenceOrder? QSentenceOrder { get; private set; }

    internal void QSentenceIntroduce(
        CEditor editor, QWindow host, QProspect prospect, QGloss gloss, QCitation citation)
    {
        _cEditor = editor;
        _pSentenceHost = host;
        _qSentenceProspect = prospect;
        _qSentenceGloss = gloss;
        _qSentenceCitation = citation;
    }

    internal PCard? QSentenceCardFind(PSentence row)
    {
        foreach (PCard card in _qSentenceMeaning)
        {
            if (card.PCardSentence.Contains(row))
            {
                return card;
            }
        }

        foreach (PCard card in _qSentenceCollocation)
        {
            if (card.PCardSentence.Contains(row))
            {
                return card;
            }
        }

        return null;
    }

    internal void QSentenceFrameRefine(CEntryDraft _)
    {
        CSentenceFrame frame = _cEditor.CEditorSentence.CSentenceFrameRead();
        QSentenceOrder = frame.CSentenceFrameOrder;
        QSentenceListRefine(QSentenceParticle, frame.CSentenceFrameParticle);
        QSentenceListRefine(QSentenceDependence, frame.CSentenceFrameDependence);

        foreach (PCard card in _qSentenceMeaning)
        {
            card.PCardSentenceApply(frame.CSentenceFrameOrder);
        }

        foreach (PCard card in _qSentenceCollocation)
        {
            card.PCardSentenceApply(frame.CSentenceFrameOrder);
        }
    }

    internal void QSentenceMentionRefine(CEntryDraft _)
    {
        IReadOnlyDictionary<long, IReadOnlyList<CMentionLabel>> lines =
            _cEditor.CEditorSentence.CSentenceMentionRead();
        QSentenceChipRefine(_qSentenceMeaning, lines);
        QSentenceChipRefine(_qSentenceCollocation, lines);
    }

    internal void QSentenceApply(FrameworkElement container, object item, string? _)
    {
        if (item is not PSentence row)
        {
            return;
        }

        PSentence.PSentenceRowApply(container, row);
        if (QLook.QLookPartFind<Grid>(container, "PSentenceReach") is Grid reach && reach.CommandBindings.Count == 0)
        {
            reach.CommandBindings.Add(new CommandBinding(
                PMentionCommand.PMentionCommandLink, QSentenceLinkRefine, QSentenceSpanRefine));
            reach.CommandBindings.Add(new CommandBinding(
                PMentionCommand.PMentionCommandChoose, QSentenceMeaningRefine, QSentenceSenseRefine));
            reach.CommandBindings.Add(new CommandBinding(
                PMentionCommand.PMentionCommandSilence, QSentenceSilenceObserve, QSentenceSpanRefine));
            reach.CommandBindings.Add(new CommandBinding(
                PMentionCommand.PMentionCommandUnlink, QSentenceUnlinkObserve, QSentenceUnlinkRefine));
            reach.CommandBindings.Add(new CommandBinding(
                PGlossCommand.PGlossCommandRemoval, _qSentenceGloss.QGlossRemoveObserve));
        }

        if (QLook.QLookPartFind<ToggleButton>(container, "PSentenceOpening") is ToggleButton opening)
        {
            opening.Click -= QSentenceOpeningRefine;
            opening.Click += QSentenceOpeningRefine;
        }

        if (QLook.QLookPartFind<Button>(container, "PSentenceAdder") is Button adder)
        {
            adder.Click -= QSentenceAddObserve;
            adder.Click += QSentenceAddObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PSentenceEraser") is Button eraser)
        {
            eraser.Click -= QSentenceRemoveObserve;
            eraser.Click += QSentenceRemoveObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PSentenceGlossChooser") is Button gloss)
        {
            gloss.Click -= _qSentenceGloss.QGlossAddObserve;
            gloss.Click += _qSentenceGloss.QGlossAddObserve;
        }

        if (QLook.QLookPartFind<TextBox>(container, "PSentenceText") is TextBox text)
        {
            text.TextChanged -= QSentenceTextObserve;
            text.TextChanged += QSentenceTextObserve;
        }

        if (QLook.QLookPartFind<ComboBox>(container, "PSentenceParticle") is ComboBox particle)
        {
            particle.RemoveHandler(TextBoxBase.TextChangedEvent, (TextChangedEventHandler)QSentenceParticleObserve);
            particle.AddHandler(TextBoxBase.TextChangedEvent, (TextChangedEventHandler)QSentenceParticleObserve);
        }

        if (QLook.QLookPartFind<ComboBox>(container, "PSentenceDependence") is ComboBox dependence)
        {
            dependence.RemoveHandler(
                TextBoxBase.TextChangedEvent, (TextChangedEventHandler)QSentenceDependenceObserve);
            dependence.AddHandler(TextBoxBase.TextChangedEvent, (TextChangedEventHandler)QSentenceDependenceObserve);
        }

        if (QLook.QLookPartFind<TextBox>(container, "PSentenceCitation") is TextBox citation)
        {
            citation.TextChanged -= QSentenceCitationRefine;
            citation.TextChanged += QSentenceCitationRefine;
        }

        _qSentenceCitation.QCitationApply(container);

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

        if (ItemsControl.ItemsControlFromItemContainer(container) is ItemsControl list)
        {
            QSentenceRevealRefine(list);
        }
    }

    internal void QSentenceGlossObserve(PCard card, PSentence row, PGloss gloss, string language)
    {
        _cEditor.CEditorSentence.CSentenceLanguageSet(card.PCardId, row.PSentenceRow, gloss.PGlossId, language);
    }

    private void QSentenceTextObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: PSentence row } box
            && QSentenceCardFind(row) is PCard card)
        {
            _cEditor.CEditorSentence.CSentenceTextSet(card.PCardId, row.PSentenceRow, box.Text);
        }
    }

    private void QSentenceParticleObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is ComboBox { IsKeyboardFocusWithin: true, DataContext: PSentence row }
            && e.OriginalSource is TextBox box
            && QSentenceCardFind(row) is PCard card)
        {
            _cEditor.CEditorSentence.CSentenceParticleSet(card.PCardId, row.PSentenceRow, box.Text);
        }
    }

    private void QSentenceDependenceObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is ComboBox { IsKeyboardFocusWithin: true, DataContext: PSentence row }
            && e.OriginalSource is TextBox box
            && QSentenceCardFind(row) is PCard card)
        {
            _cEditor.CEditorSentence.CSentenceDependenceSet(card.PCardId, row.PSentenceRow, box.Text);
        }
    }

    private void QSentenceCitationRefine(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocusWithin: true, DataContext: PSentence row } box
            && QSentenceCardFind(row) is PCard card)
        {
            _qSentenceCitation.QCitationFieldRefine(card, row, box);
        }
    }

    internal static void QSentenceRevealAttach(ItemsControl list)
    {
        list.MouseEnter -= QSentenceRevealRefine;
        list.MouseEnter += QSentenceRevealRefine;
        list.MouseLeave -= QSentenceRevealRefine;
        list.MouseLeave += QSentenceRevealRefine;
        list.IsKeyboardFocusWithinChanged -= QSentenceRevealRefine;
        list.IsKeyboardFocusWithinChanged += QSentenceRevealRefine;
        QSentenceRevealRefine(list);
    }

    private static void QSentenceListRefine(ObservableCollection<string> catalog, IReadOnlyList<string> values)
    {
        catalog.Clear();
        foreach (string value in values)
        {
            catalog.Add(value);
        }
    }

    private static void QSentenceChipRefine(
        IReadOnlyList<PCard> cards, IReadOnlyDictionary<long, IReadOnlyList<CMentionLabel>> lines)
    {
        foreach (PCard card in cards)
        {
            foreach (PSentence row in card.PCardSentence)
            {
                if (lines.TryGetValue(row.PSentenceRow, out IReadOnlyList<CMentionLabel>? labels))
                {
                    row.PSentenceChip.PMentionLineRefine(QMentionChip.QMentionChipCreate(labels));
                }
            }
        }
    }

    private static void QSentenceRevealRefine(object sender, MouseEventArgs e)
    {
        QSentenceRevealRefine((ItemsControl)sender);
    }

    private static void QSentenceRevealRefine(object sender, DependencyPropertyChangedEventArgs e)
    {
        QSentenceRevealRefine((ItemsControl)sender);
    }

    private static void QSentenceRevealRefine(ItemsControl list)
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

    private void QSentenceOpeningRefine(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton { DataContext: PSentence row } opening)
        {
            row.PSentenceFrameVisible = QLook.QLookCheckedRead(opening.IsChecked);
        }
    }

    private void QSentenceAddObserve(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PSentence row }
            || QSentenceCardFind(row) is not PCard card)
        {
            return;
        }

        _cEditor.CEditorSentence.CSentenceAdd(card.PCardId, card.PCardSentenceFind(row));
    }

    private void QSentenceRemoveObserve(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PSentence row }
            || QSentenceCardFind(row) is not PCard card)
        {
            return;
        }

        _cEditor.CEditorSentence.CSentenceRemove(card.PCardId, row.PSentenceRow);
    }

    private void QSentenceLinkRefine(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox { DataContext: PSentence } box)
        {
            return;
        }

        _qSentenceProspect.QProspectPlaceRefine(box, PMentionSelection.PMentionSelectionPlace(box));
        _qSentenceProspect.QProspectOpenRefine(_cEditor.CEditorCard.CCardMentionRead(box.SelectedText));
    }

    private void QSentenceMeaningRefine(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox { DataContext: PSentence row } box
            || QSentenceCardFind(row) is not PCard card)
        {
            return;
        }

        if (_cEditor.CEditorSentence.CSentenceSenseRead(
                card.PCardId, row.PSentenceRow, box.Text, box.SelectionStart, box.SelectionLength)
            is CMentionSense sense)
        {
            _pSentenceHost.QMentionMeaningRefine(
                box, PMentionSelection.PMentionSelectionPlace(box), sense, QSentenceSenseObserve);
        }
    }

    private void QSentenceSenseObserve(FrameworkElement anchor, long sense)
    {
        if (anchor is not TextBox { DataContext: PSentence row } box
            || QSentenceCardFind(row) is not PCard card)
        {
            return;
        }

        _cEditor.CEditorSentence.CSentenceSenseSet(
            card.PCardId, row.PSentenceRow, box.Text, box.SelectionStart, box.SelectionLength, sense);
    }

    private void QSentenceSilenceObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source is not TextBox { DataContext: PSentence row } box
            || QSentenceCardFind(row) is not PCard card)
        {
            return;
        }

        _cEditor.CEditorSentence.CSentenceMentionAdd(
            card.PCardId, row.PSentenceRow, box.Text, box.SelectionStart, box.SelectionLength, 0);
    }

    private void QSentenceUnlinkObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PSentence row }
            || QSentenceCardFind(row) is not PCard card)
        {
            return;
        }

        if (e.Parameter is PMentionChip chip)
        {
            _cEditor.CEditorSentence.CSentenceMentionRemove(card.PCardId, row.PSentenceRow, chip.PMentionChipId);
        }
        else if (e.Source is TextBox box)
        {
            _cEditor.CEditorSentence.CSentenceMentionRemove(
                card.PCardId, row.PSentenceRow, box.Text, box.SelectionStart, box.SelectionLength);
        }
    }

    private void QSentenceSpanRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Source is TextBox box
            && _pSentenceHost.QWindowAtelier.CAtelierMention.CMentionSpanCheck(
                box.Text, box.SelectionStart, box.SelectionLength);
    }

    private void QSentenceSenseRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Source is TextBox { DataContext: PSentence row } box
            && QSentenceCardFind(row) is PCard card
            && _cEditor.CEditorSentence.CSentenceSenseCheck(
                card.PCardId, row.PSentenceRow, box.Text, box.SelectionStart, box.SelectionLength);
    }

    private void QSentenceUnlinkRefine(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = e.Parameter is PMentionChip
            || (e.Source is TextBox { DataContext: PSentence row } box
                && QSentenceCardFind(row) is PCard card
                && _cEditor.CEditorSentence.CSentenceMentionCheck(
                    card.PCardId, row.PSentenceRow, box.Text, box.SelectionStart, box.SelectionLength));
    }
}
