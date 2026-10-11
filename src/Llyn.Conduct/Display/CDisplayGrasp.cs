using System;
using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed class CDisplayGrasp
{
    private readonly LDisplay _cDisplayGraspRule;

    internal CDisplayGrasp(LDisplay rule)
    {
        ArgumentNullException.ThrowIfNull(rule);

        _cDisplayGraspRule = rule;
    }

    public event Action<CBulletin>? CDisplayGraspChanged;

    public int CDisplayGraspStep => _cDisplayGraspRule.LDisplayGraspStep;

    public IReadOnlyList<IReadOnlyList<int>> CDisplayGraspFill => _cDisplayGraspRule.LDisplayGraspStar.LGraspStarFill;

    public IReadOnlyList<bool> CDisplayGraspUnrated => _cDisplayGraspRule.LDisplayGraspStar.LGraspStarUnrated;

    public IReadOnlyList<IReadOnlyList<int>> CDisplayGraspHit => _cDisplayGraspRule.LDisplayGraspStar.LGraspStarHit;

    internal void LDisplayGraspAttach()
    {
        _cDisplayGraspRule.LDisplayChosenAttach(
            CSubject.CSubjectGrasp, bulletin => CDisplayGraspChanged?.Invoke(bulletin));
    }

    public CGrasp CDisplayGraspRead()
    {
        int step = _cDisplayGraspRule.LDisplayGraspRead(_cDisplayGraspRule.LDisplayChosen);
        return new CGrasp(step, CDisplayGraspRead(step));
    }

    public string CDisplayGraspRead(int step)
    {
        return _cDisplayGraspRule.LDisplayGraspFormat(_cDisplayGraspRule.LDisplayChosen, step);
    }

    public CGrasp CDisplayGraspSet(int step)
    {
        _cDisplayGraspRule.LDisplayGraspSave(_cDisplayGraspRule.LDisplayChosen, step);
        return CDisplayGraspRead();
    }
}
