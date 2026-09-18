using System;
using System.Windows;

namespace Llyn.UIShell;

public partial class PDisplay
{
    private void PDisplayGraspShow(long id)
    {
        try
        {
            PDisplayGrasp.PGraspStep = _lEngine.LEngineGraspRead(id);
        }
        catch (Exception)
        {
            PDisplayGrasp.PGraspStep = 0;
        }

        PDisplayGraspLabel.Text = PDisplayGraspFormat(PDisplayGrasp.PGraspStep);
    }

    private string PDisplayGraspFormat(int step)
    {
        return _pDisplayVista?.LVistaChosen is null
            ? string.Empty
            : _pDisplayHost.PLocalizationTextRead(PGrasp.PGraspLabelResolve(step));
    }

    private void PDisplayHoverHandle(object sender, RoutedEventArgs e)
    {
        PDisplayGraspLabel.Text = PDisplayGraspFormat(PDisplayGrasp.PGraspHover ?? PDisplayGrasp.PGraspStep);
    }

    private void PDisplayGraspHandle(object sender, RoutedEventArgs e)
    {
        if (_pDisplayVista?.LVistaChosen is not long shown)
        {
            PDisplayGrasp.PGraspStep = 0;
            return;
        }

        try
        {
            _lEngine.LEngineGraspSave(shown, PDisplayGrasp.PGraspStep);
        }
        catch (Exception exception)
        {
            PDisplayGraspShow(shown);
            _pDisplayHost.PWindowFailureShow("Grasp.MarkFailed", exception);
        }
    }
}
