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
        if (_pDisplayEntry is null)
        {
            PDisplayFavorite.IsChecked = false;
            return;
        }

        bool marked = PDisplayFavorite.IsChecked == true;
        try
        {
            if (marked)
            {
                _lEngine.LEngineFavoriteSave(_pDisplayEntry!.Value);
            }
            else
            {
                _lEngine.LEngineFavoriteDelete(_pDisplayEntry!.Value);
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
