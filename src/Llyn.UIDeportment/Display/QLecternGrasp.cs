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
        _qLecternGrasp.PGraspStar.QGraspStarRefine(
            _qLecternGraspArea.CDisplayGraspFill,
            _qLecternGraspArea.CDisplayGraspUnrated,
            _qLecternGraspArea.CDisplayGraspHit);

        _qLecternGrasp.PGraspPressed += QLecternGraspObserve;
        _qLecternGrasp.PGraspMoved += QLecternMoveObserve;
        _qLecternGrasp.PGraspHovered += QLecternHoverRefine;
        _qLecternGraspArea.CDisplayGraspChanged += QObserver.QObserverCreate<CBulletin>(surface, QLecternGraspRefine);
    }

    private void QLecternGraspObserve(int pressed)
    {
        QLecternGraspRefine(_qLecternGraspArea.CDisplayGraspSet(pressed));
    }

    private void QLecternMoveObserve(int move)
    {
        QLecternGraspRefine(_qLecternGraspArea.CDisplayGraspMove(move));
    }

    private void QLecternHoverRefine(int? hovered)
    {
        _qLecternGraspLabel.Text = hovered is int pointed
            ? _qLecternGraspArea.CDisplayGraspRead(pointed)
            : _qLecternGraspArea.CDisplayGraspRead().CGraspLabel;
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
