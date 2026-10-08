using System;
using System.Windows;
using System.Windows.Controls;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QLecternGrasp
{
    private readonly CDisplayGrasp _qLecternGraspArea;

    private readonly PGrasp _qLecternGrasp;

    private readonly TextBlock _qLecternGraspLabel;

    public QLecternGrasp(FrameworkElement surface, CDisplayGrasp area)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(area);

        _qLecternGraspArea = area;
        _qLecternGrasp = QContract.QContractFind<PGrasp>(surface, "PDisplayGrasp");
        _qLecternGraspLabel = QContract.QContractFind<TextBlock>(surface, "PDisplayGraspLabel");
        _qLecternGrasp.PGraspLimit = _qLecternGraspArea.CDisplayGraspStep;

        _qLecternGrasp.PGraspChanged += QLecternGraspObserve;
        _qLecternGrasp.PGraspHovered += QLecternHoverRefine;
        _qLecternGraspArea.CDisplayGraspChanged += QObserver.QObserverCreate<CBulletin>(surface, QLecternGraspRefine);
    }

    private void QLecternGraspObserve(object sender, RoutedEventArgs e)
    {
        QLecternGraspRefine(_qLecternGraspArea.CDisplayGraspSet(_qLecternGrasp.PGraspStep));
    }

    private void QLecternHoverRefine(object sender, RoutedEventArgs e)
    {
        _qLecternGraspLabel.Text = _qLecternGraspArea.CDisplayGraspRead(_qLecternGrasp.PGraspPointed);
    }

    public void QLecternGraspRefine()
    {
        QLecternGraspRefine(_qLecternGraspArea.CDisplayGraspRead());
    }

    private void QLecternGraspRefine(CGrasp grasp)
    {
        _qLecternGrasp.PGraspStep = grasp.CGraspStep;
        _qLecternGraspLabel.Text = grasp.CGraspLabel;
    }
}
