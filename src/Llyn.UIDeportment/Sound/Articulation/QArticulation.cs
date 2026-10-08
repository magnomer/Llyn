using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QArticulation
{
    private const double QArticulationGap = 12;

    private readonly UserControl _qArticulationSurface;

    private readonly List<TextBox> _qArticulationTarget = [];

    private TextBox? _qArticulationField;

    internal QArticulation(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qArticulationSurface = surface;
        QLook.QLookStyleAttach(surface);

        QArticulationLane.SizeChanged += QArticulationLaneRefine;
    }

    private ScrollViewer QArticulationLane =>
        QContract.QContractFind<ScrollViewer>(_qArticulationSurface, "PArticulationLane");

    private Grid QArticulationRack => QContract.QContractFind<Grid>(_qArticulationSurface, "PArticulationRack");

    private Border QVowelChart => QContract.QContractFind<Border>(_qArticulationSurface, "PVowelChart");

    private Border QConsonantChart => QContract.QContractFind<Border>(_qArticulationSurface, "PConsonantChart");

    private Grid QVowel => QContract.QContractFind<Grid>(_qArticulationSurface, "PVowel");

    private Grid QConsonant => QContract.QContractFind<Grid>(_qArticulationSurface, "PConsonant");

    private void QArticulationLaneRefine(object sender, SizeChangedEventArgs e)
    {
        double lane = e.NewSize.Width;
        double vowel = QVowelChart.DesiredSize.Width;
        double consonant = QConsonantChart.DesiredSize.Width;

        if (vowel <= 0 || consonant <= 0)
        {
            return;
        }

        bool beside = vowel + QArticulationGap + consonant <= lane;

        Grid.SetRow(QConsonantChart, beside ? 0 : 1);
        Grid.SetColumn(QConsonantChart, beside ? 1 : 0);
        QConsonantChart.Margin = beside
            ? new Thickness(QArticulationGap, 0, 0, 0)
            : new Thickness(0, QArticulationGap, 0, 0);
    }

    internal void QArticulationIntroduce(CCatalog catalog, params TextBox[] fields)
    {
        ArgumentNullException.ThrowIfNull(catalog);

        QArticulationChartIntroduce(QVowel, catalog.CCatalogVowelRead());
        QArticulationChartIntroduce(QConsonant, catalog.CCatalogConsonantRead());
        foreach (TextBox field in fields)
        {
            _qArticulationTarget.Add(field);
            _qArticulationField ??= field;
            field.GotKeyboardFocus += QArticulationFocusRefine;
        }
    }

    private void QArticulationFocusRefine(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is TextBox field)
        {
            _qArticulationField = field;
        }
    }

    private void QArticulationGlyphRefine(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Content: string character })
        {
            return;
        }

        TextBox? field = QArticulationTargetFind();

        if (field is null)
        {
            return;
        }

        int caret = field.SelectionStart;
        string text = (field.Text ?? string.Empty).Remove(caret, field.SelectionLength);

        field.Text = text.Insert(caret, character);
        field.SelectionStart = caret + character.Length;
        field.SelectionLength = 0;
        field.Focus();
    }

    private TextBox? QArticulationTargetFind()
    {
        if (_qArticulationField is { IsEnabled: true, IsVisible: true })
        {
            return _qArticulationField;
        }

        return _qArticulationTarget.Find(field => field is { IsEnabled: true, IsVisible: true });
    }

    private void QArticulationChartIntroduce(Grid table, CArticulation chart)
    {
        QArticulationTableBuild(table, chart.CArticulationHeaders.Count, chart.CArticulationSides.Count);

        for (int column = 0; column < chart.CArticulationHeaders.Count; column++)
        {
            QArticulationHeaderPlace(table, chart.CArticulationHeaders[column], column);
        }

        for (int row = 0; row < chart.CArticulationSides.Count; row++)
        {
            QArticulationSidePlace(table, chart.CArticulationSides[row], row);

            for (int column = 0; column < chart.CArticulationCells[row].Count; column++)
            {
                QArticulationCellPlace(table, chart.CArticulationCells[row][column], column, row);
            }
        }
    }

    private Grid QArticulationTableBuild(Grid table, int columns, int rows)
    {
        table.ColumnDefinitions.Clear();
        table.RowDefinitions.Clear();

        table.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        for (int column = 0; column < columns; column++)
        {
            table.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        }

        table.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        for (int row = 0; row < rows; row++)
        {
            table.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        return table;
    }

    private void QArticulationHeaderPlace(Grid table, string key, int column)
    {
        TextBlock header = new();
        header.Style = (Style)QArticulationRack.FindResource("Articulation.Header");
        header.SetResourceReference(TextBlock.TextProperty, key);

        Grid.SetColumn(header, column + 1);
        Grid.SetRow(header, 0);
        table.Children.Add(header);
    }

    private void QArticulationSidePlace(Grid table, string key, int row)
    {
        TextBlock side = new();
        side.Style = (Style)QArticulationRack.FindResource("Articulation.Side");
        side.SetResourceReference(TextBlock.TextProperty, key);

        Grid.SetColumn(side, 0);
        Grid.SetRow(side, row + 1);
        table.Children.Add(side);
    }

    private void QArticulationCellPlace(Grid table, IReadOnlyList<string> characters, int column, int row)
    {
        StackPanel cell = new()
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
        };

        foreach (string character in characters)
        {
            Button glyph = new()
            {
                Content = character,
                Style = (Style)QArticulationRack.FindResource("Articulation.Glyph")
            };
            glyph.Click += QArticulationGlyphRefine;
            cell.Children.Add(glyph);
        }

        Grid.SetColumn(cell, column + 1);
        Grid.SetRow(cell, row + 1);
        table.Children.Add(cell);
    }
}
