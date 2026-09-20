using System;
using System.Windows;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    private void PDisplayGraspShow(long id)
    {
        try
        {
            PDisplayGrasp.PGraspStep = _lDisplay.LDisplayGraspRead(id);
        }
        catch (Exception)
        {
            PDisplayGrasp.PGraspStep = 0;
        }

        PDisplayGraspLabel.Text = _lDisplay.LDisplayGraspFormat(PDisplayGrasp.PGraspStep);
    }

    private void PDisplayHoverHandle(object sender, RoutedEventArgs e)
    {
        PDisplayGraspLabel.Text = _lDisplay.LDisplayGraspFormat(PDisplayGrasp.PGraspHover ?? PDisplayGrasp.PGraspStep);
    }

    private void PDisplayGraspHandle(object sender, RoutedEventArgs e)
    {
        if (_lDisplay.LDisplayChosen is not long shown)
        {
            PDisplayGrasp.PGraspStep = 0;
            return;
        }

        try
        {
            _lDisplay.LDisplayGraspSave(shown, PDisplayGrasp.PGraspStep);
        }
        catch (Exception exception)
        {
            PDisplayGraspShow(shown);
            _pDisplayHost.PWindowFailureShow("Grasp.MarkFailed", exception);
        }
    }
}
