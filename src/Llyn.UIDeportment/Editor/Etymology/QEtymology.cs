using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shapes;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QEtymology
{
    private readonly FrameworkElement _qEtymologySurface;

    private readonly ObservableCollection<QEtymologyItem> _qEtymologyItem = [];

    private readonly QProspect _qEtymologyProspect;

    private CEtymology _cEtymology = null!;

    internal QEtymology(FrameworkElement surface, QProspect prospect)
    {
        _qEtymologySurface = surface;
        _qEtymologyProspect = prospect;
        QEtymologySources.ItemsSource = _qEtymologyItem;
        QLookItem.QLookItemAttach(QEtymologySources, QEtymologyApply);
        QContract.QContractFind<Rectangle>(_qEtymologySurface, "PEtymologyTrunk").SetBinding(
            UIElement.VisibilityProperty,
            new Binding(nameof(ItemsControl.HasItems))
            {
                Source = QEtymologySources,
                Converter = new BooleanToVisibilityConverter(),
            });
        QEtymologyInsertion.Click += QEtymologyHeadObserve;
        QEtymologyField.CommandBindings.Add(new CommandBinding(
            QMentionCommand.QMentionCommandPick, QEtymologyPickObserve));
    }

    private FrameworkElement QEtymologyField =>
        QContract.QContractFind<FrameworkElement>(_qEtymologySurface, "PEtymology");

    private ItemsControl QEtymologySources =>
        QContract.QContractFind<ItemsControl>(_qEtymologySurface, "PEtymologySources");

    private TextBlock QEtymologyHeadword =>
        QContract.QContractFind<TextBlock>(_qEtymologySurface, "PEtymologyHeadword");

    private Button QEtymologyInsertion => QContract.QContractFind<Button>(_qEtymologySurface, "PEtymologyInsertion");

    internal void QEtymologyIntroduce(CEtymology etymology, CEntry entry)
    {
        _cEtymology = etymology;
        etymology.CEtymologyChanged += QEtymologyRefine;
        entry.CEntryDraftChanged += QEtymologyRefine;
    }

    private void QEtymologyRefine(CEntryDraft draft)
    {
        QEtymologyHeadword.Text = draft.CEntryDraftHeadword;
        QEtymologyRefine();
    }

    private void QEtymologyRefine()
    {
        QEtymologyItem.QEtymologyItemShow(_qEtymologyItem, _cEtymology.CEtymologyRead());
    }

    private void QEtymologyApply(FrameworkElement container, object item, string? _)
    {
        if (item is not QEtymologyItem row)
        {
            return;
        }

        CAncestor ancestor = row.QEtymologyItemAncestor;
        if (QLook.QLookPartFind<ItemsControl>(container, "PEtymologySources") is ItemsControl sources)
        {
            sources.ItemsSource = row.QEtymologyItemSources;
            QLookItem.QLookItemAttach(sources, QEtymologyApply);

            if (QLook.QLookPartFind<Rectangle>(container, "PEtymologyTrunk") is Rectangle trunk)
            {
                trunk.SetBinding(UIElement.VisibilityProperty, new Binding(nameof(ItemsControl.HasItems))
                {
                    Source = sources,
                    Converter = new BooleanToVisibilityConverter(),
                });
            }
        }

        if (QLook.QLookPartFind<Rectangle>(container, "PEtymologyPrior") is Rectangle prior)
        {
            prior.Visibility = QLook.QLookVisibleRead(!row.QEtymologyItemFirst);
        }

        if (QLook.QLookPartFind<Rectangle>(container, "PEtymologyNext") is Rectangle next)
        {
            next.Visibility = QLook.QLookVisibleRead(!row.QEtymologyItemLast);
        }

        QEtymologyBoxApply(container, ancestor);
        QEtymologyToolApply(container, ancestor);
        if (QLook.QLookPartFind<ComboBox>(container, "PEtymologyLanguage") is ComboBox language)
        {
            language.Visibility = QLook.QLookVisibleRead(!ancestor.CAncestorLinked);
            language.IsEnabled = !ancestor.CAncestorBorrowed;
            QEtymologyChoiceApply(
                language,
                row.QEtymologyItemLanguages,
                ancestor.CAncestorLanguage.Length == 0
                    ? null
                    : row.QEtymologyItemLanguages.FirstOrDefault(choice =>
                        choice.QEtymologyChoiceAncestor.CAncestorChoiceName == ancestor.CAncestorLanguage),
                QEtymologyLanguageObserve,
                QEtymologyLanguageApply);
        }

        if (QLook.QLookPartFind<ComboBox>(container, "PEtymologyRelation") is ComboBox relation)
        {
            relation.IsEnabled = !ancestor.CAncestorBorrowed;
            QEtymologyChoiceApply(
                relation,
                row.QEtymologyItemRelations,
                ancestor.CAncestorRelation is null
                    ? null
                    : row.QEtymologyItemRelations.FirstOrDefault(choice =>
                        choice.QEtymologyChoiceAncestor.CAncestorChoiceId == ancestor.CAncestorRelation),
                QEtymologyRelationObserve,
                QEtymologyRelationApply);
        }
    }

    private void QEtymologyBoxApply(FrameworkElement container, CAncestor ancestor)
    {
        if (QLook.QLookPartFind<TextBlock>(container, "PEtymologyTongue") is TextBlock tongue)
        {
            tongue.Text = ancestor.CAncestorLanguage;
            tongue.Visibility = QLook.QLookVisibleRead(ancestor.CAncestorLinked);
        }

        if (QLook.QLookPartFind<TextBox>(container, "PEtymologyText") is TextBox text)
        {
            text.TextChanged -= QEtymologyTextObserve;
            QField.QFieldTextShow(text, ancestor.CAncestorText);
            text.TextChanged += QEtymologyTextObserve;
            text.IsReadOnly = ancestor.CAncestorBorrowed;
            text.SetResourceReference(QField.QFieldHintProperty, "Input.AncestorHint");
            text.SetResourceReference(
                Control.ForegroundProperty, ancestor.CAncestorLinked ? "Theme.Accent" : "Theme.Ink");
            text.PreviewKeyDown -= _qEtymologyProspect.QProspectKeyRefine;
            text.PreviewKeyDown -= _qEtymologyProspect.QProspectKeyObserve;
            text.PreviewKeyDown += _qEtymologyProspect.QProspectKeyRefine;
            text.PreviewKeyDown += _qEtymologyProspect.QProspectKeyObserve;
        }

        if (QLook.QLookPartFind<TextBox>(container, "PEtymologyGloss") is TextBox gloss)
        {
            QField.QFieldTextShow(gloss, ancestor.CAncestorGloss);
            gloss.IsReadOnly = ancestor.CAncestorBorrowed;
            gloss.SetResourceReference(QField.QFieldHintProperty, "Input.AncestorGlossHint");
            gloss.TextChanged -= QEtymologyGlossObserve;
            gloss.TextChanged += QEtymologyGlossObserve;
        }

        if (QLook.QLookPartFind<Border>(container, "PEtymologyFrame") is Border frame)
        {
            frame.Visibility = QLook.QLookVisibleRead(!ancestor.CAncestorBorrowed);
            frame.SetResourceReference(
                Border.BorderBrushProperty, ancestor.CAncestorLinked ? "Theme.Accent" : "Theme.Line");
        }

        if (QLook.QLookPartFind<Rectangle>(container, "PEtymologyTrace") is Rectangle trace)
        {
            trace.Visibility = QLook.QLookVisibleRead(ancestor.CAncestorBorrowed);
        }

        if (QLook.QLookPartFind<TextBlock>(container, "PEtymologyMark") is TextBlock mark)
        {
            mark.Visibility = QLook.QLookVisibleRead(ancestor.CAncestorLinked);
        }
    }

    private void QEtymologyToolApply(FrameworkElement container, CAncestor ancestor)
    {
        if (QLook.QLookPartFind<Panel>(container, "PEtymologyTools") is Panel tools)
        {
            tools.Visibility = QLook.QLookVisibleRead(!ancestor.CAncestorBorrowed);
        }

        if (QLook.QLookPartFind<Button>(container, "PEtymologyInsertion") is Button insertion)
        {
            insertion.Visibility = QLook.QLookVisibleRead(!ancestor.CAncestorLinked);
            insertion.Click -= QEtymologyInsertObserve;
            insertion.Click += QEtymologyInsertObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PEtymologyAddition") is Button addition)
        {
            addition.Click -= QEtymologyAddObserve;
            addition.Click += QEtymologyAddObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PEtymologyRemoval") is Button removal)
        {
            removal.Click -= QEtymologyRemoveObserve;
            removal.Click += QEtymologyRemoveObserve;
        }

        if (QLook.QLookPartFind<Button>(container, "PEtymologyRestoration") is Button restoration)
        {
            restoration.Visibility = QLook.QLookVisibleRead(ancestor.CAncestorRestorable);
            restoration.Click -= QEtymologyRestoreObserve;
            restoration.Click += QEtymologyRestoreObserve;
        }
    }

    private static void QEtymologyChoiceApply(
        ComboBox choice,
        IReadOnlyList<QEtymologyChoice> choices,
        QEtymologyChoice? chosen,
        SelectionChangedEventHandler observe,
        Action<FrameworkElement, object, string?> menu)
    {
        choice.SelectionChanged -= observe;
        choice.ItemsSource = choices;
        choice.SelectedItem = chosen;
        choice.SelectionChanged += observe;
        QLookItem.QLookItemAttach(choice, menu);
    }

    private void QEtymologyLanguageApply(FrameworkElement container, object item, string? _)
    {
        QEtymologyMenuApply(container, item, QEtymologyLanguageDelete);
    }

    private void QEtymologyRelationApply(FrameworkElement container, object item, string? _)
    {
        QEtymologyMenuApply(container, item, QEtymologyRelationDelete);
    }

    private static void QEtymologyMenuApply(FrameworkElement container, object item, RoutedEventHandler delete)
    {
        if (item is not QEtymologyChoice { QEtymologyChoiceAncestor.CAncestorChoiceCustom: true })
        {
            container.ClearValue(FrameworkElement.ContextMenuProperty);
            return;
        }

        MenuItem deletion = new();
        deletion.SetResourceReference(HeaderedItemsControl.HeaderProperty, "Input.AncestorChoiceDelete");
        deletion.Click += delete;
        container.ContextMenu = new ContextMenu { Items = { deletion } };
    }

    private void QEtymologyHeadObserve(object sender, RoutedEventArgs e)
    {
        _cEtymology.CEtymologyInsert(null);
    }

    private void QEtymologyInsertObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: QEtymologyItem row })
        {
            _cEtymology.CEtymologyInsert(row.QEtymologyItemAncestor.CAncestorId);
        }
    }

    private void QEtymologyAddObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: QEtymologyItem row })
        {
            _cEtymology.CEtymologyAdd(row.QEtymologyItemAncestor.CAncestorId);
        }
    }

    private void QEtymologyRemoveObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: QEtymologyItem row })
        {
            _cEtymology.CEtymologyRemove(row.QEtymologyItemAncestor.CAncestorId);
        }
    }

    private void QEtymologyRestoreObserve(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: QEtymologyItem row })
        {
            _cEtymology.CEtymologyRestore(row.QEtymologyItemAncestor.CAncestorId);
        }
    }

    private void QEtymologyTextObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox { DataContext: QEtymologyItem row } box)
        {
            return;
        }

        CProspect prospect = _cEtymology.CEtymologyTextSet(row.QEtymologyItemAncestor.CAncestorId, box.Text);
        _qEtymologyProspect.QProspectPlaceRefine(box, QMentionSelection.QMentionSelectionPlace(box));
        _qEtymologyProspect.QProspectOpenRefine(prospect);
    }

    private void QEtymologyGlossObserve(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox { IsKeyboardFocused: true, DataContext: QEtymologyItem row } box)
        {
            _cEtymology.CEtymologyGlossSet(row.QEtymologyItemAncestor.CAncestorId, box.Text);
        }
    }

    private void QEtymologyPickObserve(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.OriginalSource is TextBox { DataContext: QEtymologyItem row } && e.Parameter is long entry)
        {
            _cEtymology.CEtymologyTargetSet(row.QEtymologyItemAncestor.CAncestorId, entry);
        }
    }

    private void QEtymologyLanguageObserve(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox { DataContext: QEtymologyItem row, SelectedItem: QEtymologyChoice choice })
        {
            _cEtymology.CEtymologyLanguageSet(row.QEtymologyItemAncestor.CAncestorId, choice.QEtymologyChoiceAncestor);
        }
    }

    private void QEtymologyRelationObserve(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox { DataContext: QEtymologyItem row, SelectedItem: QEtymologyChoice choice })
        {
            _cEtymology.CEtymologyRelationSet(row.QEtymologyItemAncestor.CAncestorId, choice.QEtymologyChoiceAncestor);
        }
    }

    private void QEtymologyLanguageDelete(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: QEtymologyChoice choice })
        {
            _cEtymology.CEtymologyLanguageDelete(choice.QEtymologyChoiceAncestor);
        }
    }

    private void QEtymologyRelationDelete(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement { DataContext: QEtymologyChoice choice })
        {
            _cEtymology.CEtymologyRelationDelete(choice.QEtymologyChoiceAncestor);
        }
    }
}
