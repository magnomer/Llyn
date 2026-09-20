using System;
using System.Windows;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    private void PDisplayFavoriteShow(long id)
    {
        try
        {
            PDisplayFavorite.IsChecked = _lDisplay.LDisplayFavoriteCheck(id);
        }
        catch (Exception)
        {
            PDisplayFavorite.IsChecked = false;
        }
    }

    private void PDisplayFavoriteHandle(object sender, RoutedEventArgs e)
    {
        if (_lDisplay.LDisplayChosen is not long shown)
        {
            PDisplayFavorite.IsChecked = false;
            return;
        }

        bool marked = PDisplayFavorite.IsChecked == true;
        try
        {
            if (marked)
            {
                _lDisplay.LDisplayFavoriteSave(shown);
            }
            else
            {
                _lDisplay.LDisplayFavoriteDelete(shown);
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
