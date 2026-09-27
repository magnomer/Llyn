using System;
using System.Windows;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed class QEnvoy : CEnvoy
{
    private readonly Window _qEnvoySurface;

    internal QEnvoy(Window surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        _qEnvoySurface = surface;
    }

    public bool CEnvoyConfirm(string key)
    {
        return MessageBox.Show(
            _qEnvoySurface,
            QLocalizationCatalog.QLocalizationTextRead(key),
            QLocalizationCatalog.QLocalizationTextRead("Terms.Product"),
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning) == MessageBoxResult.Yes;
    }

    public void CEnvoyFailureShow(string key)
    {
        MessageBox.Show(
            _qEnvoySurface,
            QLocalizationCatalog.QLocalizationTextRead(key),
            QLocalizationCatalog.QLocalizationTextRead("Terms.Product"),
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }
}
