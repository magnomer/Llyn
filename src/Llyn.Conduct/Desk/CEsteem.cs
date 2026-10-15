using System;
using System.Collections.Generic;

namespace Llyn.Conduct;

public sealed class CEsteem
{
    private readonly CDesk _cEsteemDesk;

    private readonly CRule _cEsteemDisplay;

    internal CEsteem(CDesk desk, CRule display)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(display);

        _cEsteemDesk = desk;
        _cEsteemDisplay = display;
    }

    public event Action? CEsteemFavoriteChanged;

    public event Action? CEsteemGraspChanged;

    public event Action? CEsteemFrequencyChanged;

    public bool CEsteemFavorite => _cEsteemDisplay.CRuleFavoriteRead(LEsteemEntry);

    public int CEsteemGrasp => _cEsteemDisplay.CRuleGraspRead(LEsteemEntry);

    public int CEsteemGraspStep => _cEsteemDisplay.CRuleGraspStep;

    public IReadOnlyList<IReadOnlyList<int>> CEsteemGraspFill => _cEsteemDisplay.CRuleGraspStar.LGraspStarFill;

    public IReadOnlyList<bool> CEsteemGraspUnrated => _cEsteemDisplay.CRuleGraspStar.LGraspStarUnrated;

    public IReadOnlyList<IReadOnlyList<int>> CEsteemGraspHit => _cEsteemDisplay.CRuleGraspStar.LGraspStarHit;

    private long? LEsteemEntry => _cEsteemDesk.CDeskStoredRead();

    internal void LEsteemObserverAttach(Action<Action> marshal)
    {
        _cEsteemDesk.CDeskVigil.CVigilEntryAttach(
            CSubject.CSubjectFrequency, _ => marshal(() => CEsteemFrequencyChanged?.Invoke()));
        _cEsteemDesk.CDeskVigil.CVigilEntryAttach(
            CSubject.CSubjectGrasp, _ => marshal(() => CEsteemGraspChanged?.Invoke()));
    }

    public void CEsteemFavoriteSet(bool marked)
    {
        _cEsteemDisplay.CRuleFavoriteSave(LEsteemEntry, marked);
        CEsteemFavoriteChanged?.Invoke();
    }

    public void CEsteemGraspSet(int step)
    {
        _cEsteemDisplay.CRuleGraspSave(LEsteemEntry, step);
        CEsteemGraspChanged?.Invoke();
    }

    public void CEsteemGraspMove(int move)
    {
        int kept = CEsteemGrasp;
        int step = Math.Clamp(kept + move, 0, CEsteemGraspStep);
        if (step != kept)
        {
            CEsteemGraspSet(step);
        }
    }

    public CGrasp CEsteemGraspRead()
    {
        int step = CEsteemGrasp;
        return new CGrasp(step, CEsteemGraspRead(step));
    }

    public string CEsteemGraspRead(int step)
    {
        return _cEsteemDisplay.CRuleGraspFormat(LEsteemEntry, step);
    }

    public CFrequency? CEsteemFrequencyRead(string once)
    {
        return _cEsteemDisplay.CRuleFrequencyRead(LEsteemEntry, once);
    }
}
