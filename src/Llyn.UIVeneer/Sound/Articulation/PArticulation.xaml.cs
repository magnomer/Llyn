using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Llyn.UIVeneer;

public partial class PArticulation : UserControl
{
    private const double PArticulationGap = 12;

    private readonly List<TextBox> _pArticulationTarget = [];

    private TextBox? _pArticulationField;

    public PArticulation()
    {
        InitializeComponent();

        PVowelBuild();
        PConsonantBuild();
    }

    private void PArticulationLaneHandle(object sender, SizeChangedEventArgs e)
    {
        PArticulationPlace(e.NewSize.Width);
    }

    private void PArticulationPlace(double lane)
    {
        double vowel = PVowelChart.DesiredSize.Width;
        double consonant = PConsonantChart.DesiredSize.Width;

        if (vowel <= 0 || consonant <= 0)
        {
            return;
        }

        bool beside = vowel + PArticulationGap + consonant <= lane;

        Grid.SetRow(PConsonantChart, beside ? 0 : 1);
        Grid.SetColumn(PConsonantChart, beside ? 1 : 0);
        PConsonantChart.Margin = beside
            ? new Thickness(PArticulationGap, 0, 0, 0)
            : new Thickness(0, PArticulationGap, 0, 0);
    }

    internal void PArticulationAttach(params TextBox[] fields)
    {
        foreach (TextBox field in fields)
        {
            _pArticulationTarget.Add(field);
            _pArticulationField ??= field;
            field.GotKeyboardFocus += PArticulationFocusHandle;
        }
    }

    private void PArticulationFocusHandle(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is TextBox field)
        {
            _pArticulationField = field;
        }
    }

    private void PArticulationHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Content: string character })
        {
            return;
        }

        PArticulationInsert(character);
    }

    private void PArticulationInsert(string character)
    {
        TextBox? field = PArticulationTargetFind();

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

    private TextBox? PArticulationTargetFind()
    {
        if (_pArticulationField is { IsEnabled: true, IsVisible: true })
        {
            return _pArticulationField;
        }

        return _pArticulationTarget.Find(field => field is { IsEnabled: true, IsVisible: true });
    }

    private Grid PArticulationTableBuild(Grid table, int columns, int rows)
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

    private void PArticulationHeaderPlace(Grid table, string key, int column)
    {
        TextBlock header = new();
        header.Style = (Style)FindResource("Articulation.Header");
        header.SetResourceReference(TextBlock.TextProperty, key);

        Grid.SetColumn(header, column + 1);
        Grid.SetRow(header, 0);
        table.Children.Add(header);
    }

    private void PArticulationSidePlace(Grid table, string key, int row)
    {
        TextBlock side = new();
        side.Style = (Style)FindResource("Articulation.Side");
        side.SetResourceReference(TextBlock.TextProperty, key);

        Grid.SetColumn(side, 0);
        Grid.SetRow(side, row + 1);
        table.Children.Add(side);
    }

    private void PArticulationCellPlace(Grid table, string characters, int column, int row)
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
                Style = (Style)FindResource("Articulation.Glyph")
            };
            glyph.Click += PArticulationHandle;
            cell.Children.Add(glyph);
        }

        Grid.SetColumn(cell, column + 1);
        Grid.SetRow(cell, row + 1);
        table.Children.Add(cell);
    }
}
