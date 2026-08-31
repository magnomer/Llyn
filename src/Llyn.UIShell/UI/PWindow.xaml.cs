using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Llyn.UIShell;

public partial class PWindow : Window
{
    public PWindow()
    {
        InitializeComponent();
        PLocalization.SelectedValue = PLocalizationLoader.DefaultLanguage;
    }

    private void PCaptionMinimize_OnClick(object sender, RoutedEventArgs e)
    {
        SystemCommands.MinimizeWindow(this);
    }

    private void PCaptionMaximize_OnClick(object sender, RoutedEventArgs e)
    {
        ToggleMaximizedState();
    }

    private void PCaptionClose_OnClick(object sender, RoutedEventArgs e)
    {
        SystemCommands.CloseWindow(this);
    }

    private void PNavigation_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button selectedButton)
        {
            return;
        }

        (Button Button, Grid Panel)[] tabs =
        [
            (PNavigationInput, PInput),
            (PNavigationList, PList),
            (PNavigationSound, PSound),
            (PNavigationTag, PTag),
            (PNavigationSituation, PSituation),
            (PNavigationFavorite, PFavorite),
            (PNavigationDualpanel, PDualPanel),
            (PNavigationSettings, PSettings)
        ];

        foreach ((Button button, Grid panel) in tabs)
        {
            bool isSelected = button == selectedButton;
            button.Style = (Style)FindResource(
                isSelected ? "Theme.Navigation.Selected" : "Theme.Navigation.Button");
            panel.Visibility = isSelected ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private void PStack_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button selectedButton)
        {
            return;
        }

        (Button Button, FrameworkElement Contents)[] tabs =
        [
            (PStackMeaning, PMeaning),
            (PStackCollocation, PCollocation),
            (PStackNote, PNote)
        ];

        foreach ((Button button, FrameworkElement contents) in tabs)
        {
            bool isSelected = button == selectedButton;
            button.Style = (Style)FindResource(
                isSelected ? "Theme.Input.Tab.Selected" : "Theme.Input.Tab");
            contents.Visibility = isSelected ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private void PCardRemove_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            FindAncestor<Border>(button)?.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
        }
    }

    private static T? FindAncestor<T>(DependencyObject child)
        where T : DependencyObject
    {
        DependencyObject? current = VisualTreeHelper.GetParent(child);

        while (current is not null)
        {
            if (current is T ancestor)
            {
                return ancestor;
            }

            current = VisualTreeHelper.GetParent(current);
        }

        return null;
    }

    private void PLocalization_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (PLocalization.SelectedValue is string language)
        {
            PLocalizationLoader.Apply(Application.Current.Resources, language);
        }
    }

    private void PRoof_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            ToggleMaximizedState();
            return;
        }

        if (WindowState == WindowState.Maximized)
        {
            RestoreWindowAtPointer(e);
        }

        DragMove();
    }

    private void ToggleMaximizedState()
    {
        if (WindowState == WindowState.Maximized)
        {
            SystemCommands.RestoreWindow(this);
        }
        else
        {
            SystemCommands.MaximizeWindow(this);
        }
    }

    private void RestoreWindowAtPointer(MouseButtonEventArgs e)
    {
        Point pointerInWindow = e.GetPosition(this);
        Point pointerOnScreen = PointToScreen(pointerInWindow);
        PresentationSource? source = PresentationSource.FromVisual(this);

        if (source?.CompositionTarget is not null)
        {
            pointerOnScreen = source.CompositionTarget.TransformFromDevice.Transform(pointerOnScreen);
        }

        double restoredWidth = RestoreBounds.Width;
        double horizontalRatio = ActualWidth > 0
            ? Math.Clamp(pointerInWindow.X / ActualWidth, 0, 1)
            : 0.5;

        SystemCommands.RestoreWindow(this);
        Left = pointerOnScreen.X - (restoredWidth * horizontalRatio);
        Top = pointerOnScreen.Y - Math.Min(pointerInWindow.Y, PRoof.ActualHeight / 2);
    }
}
