using System;
using System.Windows;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    private void PDisplayFavoriteShow(long id)
    {
        try
        {
            PDisplayFavorite.IsChecked = _lLectern.LLecternFavoriteCheck(id);
        }
        catch (Exception)
        {
            PDisplayFavorite.IsChecked = false;
        }
    }

    private void PDisplayFavoriteHandle(object sender, RoutedEventArgs e)
    {
        if (_lLectern.LLecternChosen is not long shown)
        {
            PDisplayFavorite.IsChecked = false;
            return;
        }

        bool marked = PDisplayFavorite.IsChecked == true;
        try
        {
            if (marked)
            {
                _lLectern.LLecternFavoriteSave(shown);
            }
            else
            {
                _lLectern.LLecternFavoriteDelete(shown);
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
