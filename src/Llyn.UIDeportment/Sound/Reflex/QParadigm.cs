using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Shapes;

namespace Llyn.UIDeportment;

public sealed class QParadigm : Decorator
{
    public static readonly DependencyProperty QParadigmItemsProperty = DependencyProperty.Register(
        nameof(QParadigmItems),
        typeof(IReadOnlyList<QParadigmItem>),
        typeof(QParadigm),
        new FrameworkPropertyMetadata(null, QParadigmItemsRefine));

    public static readonly DependencyProperty QParadigmSheetProperty = DependencyProperty.Register(
        nameof(QParadigmSheet),
        typeof(QParadigmSheet),
        typeof(QParadigm),
        new FrameworkPropertyMetadata(null, QParadigmSheetRefine));

    private readonly ItemsControl _qParadigmList = new();
    private readonly RadioButton _qParadigmShort = new();
    private readonly RadioButton _qParadigmFull = new();
    private readonly Grid _qParadigmCollapsed = new();
    private readonly Grid _qParadigmExpanded = new();

    public QParadigm()
    {
        Visibility = Visibility.Collapsed;
        HorizontalAlignment = HorizontalAlignment.Left;

        Grid.SetIsSharedSizeScope(_qParadigmList, true);
        _qParadigmList.SetResourceReference(ItemsControl.ItemTemplateProperty, "Theme.Paradigm.Row");
        QLookItem.QLookItemAttach(_qParadigmList, QParadigmItemRefine);

        StackPanel fold = new()
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
        };
        fold.SetResourceReference(ToolTipProperty, "Paradigm.Fold");
        fold.SetResourceReference(AutomationProperties.NameProperty, "Paradigm.Fold");
        (RadioButton, string)[] buttons = [(_qParadigmShort, "Paradigm.Short"), (_qParadigmFull, "Paradigm.Full")];
        foreach ((RadioButton button, string key) in buttons)
        {
            button.SetResourceReference(ContentControl.ContentProperty, key);
            button.SetResourceReference(StyleProperty, "Theme.Paradigm.Fold");
            button.Visibility = Visibility.Collapsed;
            button.Checked += QParadigmFoldRefine;
            fold.Children.Add(button);
        }

        _qParadigmShort.IsChecked = true;
        _qParadigmCollapsed.Visibility = Visibility.Collapsed;
        _qParadigmExpanded.Visibility = Visibility.Collapsed;

        StackPanel stack = new();
        stack.Children.Add(_qParadigmList);
        stack.Children.Add(fold);
        stack.Children.Add(_qParadigmCollapsed);
        stack.Children.Add(_qParadigmExpanded);

        Border box = new() { Child = stack };
        box.SetResourceReference(StyleProperty, "Theme.Paradigm.Box");
        Child = box;
    }

    public IReadOnlyList<QParadigmItem>? QParadigmItems
    {
        get => (IReadOnlyList<QParadigmItem>?)GetValue(QParadigmItemsProperty);
        set => SetValue(QParadigmItemsProperty, value);
    }

    public QParadigmSheet? QParadigmSheet
    {
        get => (QParadigmSheet?)GetValue(QParadigmSheetProperty);
        set => SetValue(QParadigmSheetProperty, value);
    }

    private static void QParadigmItemRefine(FrameworkElement container, object item, string? _)
    {
        if (container is not ContentPresenter presenter || item is not QParadigmItem row)
        {
            return;
        }

        presenter.ApplyTemplate();
        if (presenter.ContentTemplate?.FindName("PParadigmPart", presenter) is not TextBlock part
            || presenter.ContentTemplate.FindName("PParadigmName", presenter) is not TextBlock name
            || presenter.ContentTemplate.FindName("PParadigmText", presenter) is not TextBlock text)
        {
            return;
        }

        part.Text = row.QParadigmItemPart;
        name.Text = row.QParadigmItemName;
        text.Text = row.QParadigmItemText;
        if (row.QParadigmItemTip is not string tip)
        {
            text.ClearValue(TextBlock.ForegroundProperty);
            text.ClearValue(ToolTipProperty);
            return;
        }

        text.SetResourceReference(TextBlock.ForegroundProperty, "Theme.Muted");
        text.SetResourceReference(ToolTipProperty, tip);
    }

    private static void QParadigmItemsRefine(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        QParadigm box = (QParadigm)sender;
        IReadOnlyList<QParadigmItem>? items = e.NewValue as IReadOnlyList<QParadigmItem>;
        box._qParadigmList.ItemsSource = items;
        box.Visibility = QLook.QLookVisibleRead(items is { Count: > 0 } || box.QParadigmSheet is not null);
    }

    private static void QParadigmSheetRefine(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        QParadigm box = (QParadigm)sender;
        QParadigmSheet? sheet = e.NewValue as QParadigmSheet;
        box._qParadigmCollapsed.Children.Clear();
        box._qParadigmExpanded.Children.Clear();
        if (sheet is not null)
        {
            QParadigmTableRefine(box._qParadigmCollapsed, sheet.QParadigmSheetCollapsed);
            QParadigmTableRefine(box._qParadigmExpanded, sheet.QParadigmSheetExpanded);
        }

        box._qParadigmShort.Visibility = QLook.QLookVisibleRead(sheet is not null);
        box._qParadigmFull.Visibility = QLook.QLookVisibleRead(sheet is not null);
        box.QParadigmFoldRefine(box, new RoutedEventArgs());
        box.Visibility = QLook.QLookVisibleRead(box.QParadigmItems is { Count: > 0 } || sheet is not null);
    }

    private void QParadigmFoldRefine(object sender, RoutedEventArgs e)
    {
        bool shown = QParadigmSheet is not null;
        bool expanded = QLook.QLookCheckedRead(_qParadigmFull.IsChecked);
        _qParadigmCollapsed.Visibility = QLook.QLookVisibleRead(shown && !expanded);
        _qParadigmExpanded.Visibility = QLook.QLookVisibleRead(shown && expanded);
    }

    private static void QParadigmTableRefine(Grid grid, QParadigmTable table)
    {
        ArgumentNullException.ThrowIfNull(table);

        grid.Children.Clear();
        grid.ColumnDefinitions.Clear();
        grid.RowDefinitions.Clear();
        int width = table.QParadigmTableLines.Select(line => line.QParadigmLineForms.Count)
            .Append(table.QParadigmTableHeaders.Count)
            .Max();
        for (int column = 0; column < width + 2; column++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        }

        int row = 0;
        if (table.QParadigmTableHeaders.Count > 0)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            for (int column = 0; column < table.QParadigmTableHeaders.Count; column++)
            {
                if (table.QParadigmTableHeaders[column] is not { Length: > 0 } key)
                {
                    continue;
                }

                TextBlock header = new();
                header.SetResourceReference(StyleProperty, "Theme.Paradigm.Header");
                header.SetResourceReference(TextBlock.TextProperty, key);
                Grid.SetColumn(header, column + 2);
                grid.Children.Add(header);
            }

            QParadigmRuleDraw(grid, row, true);
            row++;
        }

        for (int index = 0; index < table.QParadigmTableLines.Count; index++)
        {
            QParadigmLine line = table.QParadigmTableLines[index];
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            if (line.QParadigmLineGroup is { Length: > 0 })
            {
                TextBlock group = new();
                group.SetResourceReference(StyleProperty, "Theme.Paradigm.Group");
                group.SetResourceReference(TextBlock.TextProperty, line.QParadigmLineGroup);
                Grid.SetRow(group, row);
                grid.Children.Add(group);
            }

            if (line.QParadigmLineLabel is { Length: > 0 })
            {
                TextBlock label = new();
                label.SetResourceReference(StyleProperty, "Theme.Paradigm.Label");
                label.SetResourceReference(TextBlock.TextProperty, line.QParadigmLineLabel);
                Grid.SetRow(label, row);
                Grid.SetColumn(label, 1);
                grid.Children.Add(label);
            }

            for (int column = 0; column < line.QParadigmLineForms.Count; column++)
            {
                TextBlock form = new();
                form.SetResourceReference(StyleProperty, "Theme.Paradigm.Form");
                QParadigmFormRefine(form, line.QParadigmLineForms[column]);
                Grid.SetRow(form, row);
                Grid.SetColumn(form, column + 2);
                grid.Children.Add(form);
            }

            if (index + 1 < table.QParadigmTableLines.Count)
            {
                QParadigmRuleDraw(
                    grid, row, table.QParadigmTableLines[index + 1].QParadigmLineGroup is { Length: > 0 });
            }

            row++;
        }
    }

    private static void QParadigmRuleDraw(Grid grid, int row, bool closed)
    {
        int start = closed ? 0 : 1;
        Rectangle rule = new();
        rule.SetResourceReference(StyleProperty, closed ? "Theme.Paradigm.Close" : "Theme.Paradigm.Rule");
        Grid.SetRow(rule, row);
        Grid.SetColumn(rule, start);
        Grid.SetColumnSpan(rule, Math.Max(1, grid.ColumnDefinitions.Count - start));
        grid.Children.Add(rule);
    }

    private static void QParadigmFormRefine(TextBlock block, QParadigmForm form)
    {
        ArgumentNullException.ThrowIfNull(form);

        string text = form.QParadigmFormText;
        int split = form.QParadigmFormSplit;
        block.Inlines.Clear();
        bool[] marked = new bool[text.Length];
        foreach (QParadigmMark mark in form.QParadigmFormMarks)
        {
            int offset = Math.Clamp(mark.QParadigmMarkOffset, 0, text.Length);
            int end = Math.Clamp(mark.QParadigmMarkOffset + mark.QParadigmMarkLength, offset, text.Length);
            Array.Fill(marked, true, offset, end - offset);
        }

        int start = 0;
        for (int index = 1; index <= text.Length; index++)
        {
            if (index < text.Length && index != split && marked[index] == marked[start])
            {
                continue;
            }

            Run run = new(text[start..index]);
            if (marked[start])
            {
                run.SetResourceReference(FrameworkContentElement.StyleProperty, "Theme.Paradigm.Marked");
            }

            block.Inlines.Add(run);
            if (index == split && split < text.Length)
            {
                Run cut = new("-");
                cut.SetResourceReference(FrameworkContentElement.StyleProperty, "Theme.Paradigm.Cut");
                block.Inlines.Add(cut);
            }

            start = index;
        }

        if (form.QParadigmFormTip is not string tip)
        {
            block.ClearValue(TextBlock.ForegroundProperty);
            block.ClearValue(ToolTipProperty);
            return;
        }

        block.SetResourceReference(TextBlock.ForegroundProperty, "Theme.Muted");
        block.SetResourceReference(ToolTipProperty, tip);
    }
}
