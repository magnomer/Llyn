using System;
using System.Windows;

namespace Llyn.UIShell;

public partial class PDisplay
{
    private void PDisplayFavoriteShow(long id)
    {
        try
        {
            PDisplayFavorite.IsChecked = _lEngine.LEngineFavoriteCheck(id);
        }
        catch (Exception)
        {
            PDisplayFavorite.IsChecked = false;
        }
    }

    private void PDisplayFavoriteHandle(object sender, RoutedEventArgs e)
    {
        if (_pDisplayVista?.LVistaChosen is not long shown)
        {
            PDisplayFavorite.IsChecked = false;
            return;
        }

        bool marked = PDisplayFavorite.IsChecked == true;
        try
        {
            if (marked)
            {
                _lEngine.LEngineFavoriteSave(shown);
            }
            else
            {
                _lEngine.LEngineFavoriteDelete(shown);
            }
        }
        catch (Exception exception)
        {
            PDisplayFavorite.IsChecked = !marked;
            _pDisplayHost.PWindowFailureShow("Favorite.MarkFailed", exception);
            return;
        }

    }
}
