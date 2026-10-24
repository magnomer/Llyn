using System;
using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed class CDisplayGrasp
{
    private readonly CRule _cDisplayGraspRule;

    internal CDisplayGrasp(CRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);

        _cDisplayGraspRule = rule;
    }

    public event Action<CBulletin>? CDisplayFavoriteChanged;

    public event Action<CBulletin>? CDisplayGraspChanged;

    public int CDisplayGraspStep => _cDisplayGraspRule.CRuleGraspStep;

    public IReadOnlyList<IReadOnlyList<int>> CDisplayGraspFill => _cDisplayGraspRule.CRuleGraspStar.LGraspStarFill;

    public IReadOnlyList<bool> CDisplayGraspUnrated => _cDisplayGraspRule.CRuleGraspStar.LGraspStarUnrated;

    public IReadOnlyList<IReadOnlyList<int>> CDisplayGraspHit => _cDisplayGraspRule.CRuleGraspStar.LGraspStarHit;

    internal void LDisplayGraspAttach()
    {
        _cDisplayGraspRule.CRuleChosenAttach(
            CSubject.CSubjectFavorite, bulletin => CDisplayFavoriteChanged?.Invoke(bulletin));
        _cDisplayGraspRule.CRuleChosenAttach(
            CSubject.CSubjectGrasp, bulletin => CDisplayGraspChanged?.Invoke(bulletin));
    }

    public bool CDisplayFavoriteRead()
    {
        return _cDisplayGraspRule.CRuleFavoriteRead(_cDisplayGraspRule.CRuleChosen);
    }

    public bool CDisplayFavoriteToggle(bool marked)
    {
        _cDisplayGraspRule.CRuleFavoriteSave(_cDisplayGraspRule.CRuleChosen, marked);
        return CDisplayFavoriteRead();
    }

    public CGrasp CDisplayGraspRead()
    {
        int step = _cDisplayGraspRule.CRuleGraspRead(_cDisplayGraspRule.CRuleChosen);
        return new CGrasp(step, CDisplayGraspRead(step));
    }

    public string CDisplayGraspRead(int step)
    {
        return _cDisplayGraspRule.CRuleGraspFormat(_cDisplayGraspRule.CRuleChosen, step);
    }

    public CGrasp CDisplayGraspSet(int step)
    {
        _cDisplayGraspRule.CRuleGraspSave(_cDisplayGraspRule.CRuleChosen, step);
        return CDisplayGraspRead();
    }

    public CGrasp CDisplayGraspMove(int move)
    {
        long? chosen = _cDisplayGraspRule.CRuleChosen;
        int kept = _cDisplayGraspRule.CRuleGraspRead(chosen);
        int step = Math.Clamp(kept + move, 0, CDisplayGraspStep);
        return step == kept ? CDisplayGraspRead() : CDisplayGraspSet(step);
    }
}
