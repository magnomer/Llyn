using System;
using System.Windows;

namespace Llyn.UIVeneer;

public partial class PDisplay
{
    private void PDisplayGraspShow(long id)
    {
        try
        {
            PDisplayGrasp.PGraspStep = _lLectern.LLecternGraspRead(id);
        }
        catch (Exception)
        {
            PDisplayGrasp.PGraspStep = 0;
        }

        PDisplayGraspLabel.Text = _lLectern.LLecternGraspFormat(PDisplayGrasp.PGraspStep);
    }

    private void PDisplayHoverHandle(object sender, RoutedEventArgs e)
    {
        PDisplayGraspLabel.Text = _lLectern.LLecternGraspFormat(PDisplayGrasp.PGraspHover ?? PDisplayGrasp.PGraspStep);
    }

    private void PDisplayGraspHandle(object sender, RoutedEventArgs e)
    {
        if (_lLectern.LLecternChosen is not long shown)
        {
            PDisplayGrasp.PGraspStep = 0;
            return;
        }

        try
        {
            _lLectern.LLecternGraspSave(shown, PDisplayGrasp.PGraspStep);
        }
        catch (Exception exception)
        {
            PDisplayGraspShow(shown);
            _pDisplayHost.PWindowFailureShow("Grasp.MarkFailed", exception);
        }
    }
}
