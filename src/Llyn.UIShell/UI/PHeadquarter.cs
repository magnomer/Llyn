using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace Llyn.UIShell;

public partial class PWindow
{
    private void PLogoHandle(object sender, MouseButtonEventArgs e)
    {
        PHeadquarter.IsOpen = !PHeadquarter.IsOpen;
        e.Handled = true;
    }

    private void PHeadquarterAboutHandle(object sender, MouseButtonEventArgs e)
    {
        PHeadquarter.IsOpen = false;
        e.Handled = true;

        string product = PLocalizationTextRead("Terms.Product");

        MessageBox.Show(
            this,
            $"{product}\n{PLocalizationTextRead("Headquarter.Version")} {PHeadquarterVersionRead()}",
            product,
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void PHeadquarterExitHandle(object sender, MouseButtonEventArgs e)
    {
        PHeadquarter.IsOpen = false;
        e.Handled = true;

        SystemCommands.CloseWindow(this);
    }

    private static string PHeadquarterVersionRead()
    {
        Version? version = Assembly.GetExecutingAssembly().GetName().Version;

        return version is null ? string.Empty : version.ToString(3);
    }
}
