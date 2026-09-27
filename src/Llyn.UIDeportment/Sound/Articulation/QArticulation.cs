using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Llyn.UIDeportment;

internal sealed partial class QArticulation
{
    private const double QArticulationGap = 12;

    private readonly UserControl _qArticulationSurface;

    private readonly List<TextBox> _qArticulationTarget = [];

    private TextBox? _qArticulationField;

    internal QArticulation(UserControl surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qArticulationSurface = surface;
        QLook.QLookStyleAttach(surface.Resources);

        QArticulationLane.SizeChanged += QArticulationLaneHandle;

        QVowelBuild();
        QConsonantBuild();
    }

    private ScrollViewer QArticulationLane =>
        QContract.QContractFind<ScrollViewer>(_qArticulationSurface, "PArticulationLane");

    private Grid QArticulationRack => QContract.QContractFind<Grid>(_qArticulationSurface, "PArticulationRack");

    private Border QVowelChart => QContract.QContractFind<Border>(_qArticulationSurface, "PVowelChart");

    private Border QConsonantChart => QContract.QContractFind<Border>(_qArticulationSurface, "PConsonantChart");

    private Grid QVowel => QContract.QContractFind<Grid>(_qArticulationSurface, "PVowel");

    private Grid QConsonant => QContract.QContractFind<Grid>(_qArticulationSurface, "PConsonant");

    private void QArticulationLaneHandle(object sender, SizeChangedEventArgs e)
    {
        QArticulationPlace(e.NewSize.Width);
    }

    private void QArticulationPlace(double lane)
    {
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

    internal void QArticulationAttach(params TextBox[] fields)
    {
        foreach (TextBox field in fields)
        {
            _qArticulationTarget.Add(field);
            _qArticulationField ??= field;
            field.GotKeyboardFocus += QArticulationFocusHandle;
        }
    }

    private void QArticulationFocusHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is TextBox field)
        {
            _qArticulationField = field;
        }
    }

    private void QArticulationHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Content: string character })
        {
            return;
        }

        QArticulationInsert(character);
    }

    private void QArticulationInsert(string character)
    {
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

    private void QArticulationCellPlace(Grid table, string characters, int column, int row)
    {
        StackPanel cell = new()
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
        };

        foreach (string character in characters.Split(' ', System.StringSplitOptions.RemoveEmptyEntries))
        {
            Button glyph = new()
            {
                Content = character,
                Style = (Style)QArticulationRack.FindResource("Articulation.Glyph")
            };
            glyph.Click += QArticulationHandle;
            cell.Children.Add(glyph);
        }

        Grid.SetColumn(cell, column + 1);
        Grid.SetRow(cell, row + 1);
        table.Children.Add(cell);
    }
}
