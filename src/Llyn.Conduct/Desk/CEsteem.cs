using System;

namespace Llyn.Conduct;

public sealed class CEsteem
{
    private readonly CDesk _cEsteemDesk;

    private readonly LDisplay _cEsteemDisplay;

    internal CEsteem(CDesk desk, LDisplay display)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(display);

        _cEsteemDesk = desk;
        _cEsteemDisplay = display;
    }

    public event Action? CEsteemFavoriteChanged;

    public event Action? CEsteemGraspChanged;

    public bool CEsteemFavorite => _cEsteemDisplay.LDisplayFavoriteRead(LEsteemEntry);

    public int CEsteemGrasp => _cEsteemDisplay.LDisplayGraspRead(LEsteemEntry);

    public int CEsteemGraspStep => _cEsteemDisplay.LDisplayGraspStep;

    private long? LEsteemEntry => _cEsteemDesk.CDeskStoredRead();

    public void CEsteemFavoriteSet(bool marked)
    {
        _cEsteemDisplay.LDisplayFavoriteSave(LEsteemEntry, marked);
        CEsteemFavoriteChanged?.Invoke();
    }

    public void CEsteemGraspSet(int step)
    {
        _cEsteemDisplay.LDisplayGraspSave(LEsteemEntry, step);
        CEsteemGraspChanged?.Invoke();
    }

    public string CEsteemGraspRead(int step)
    {
        return _cEsteemDisplay.LDisplayGraspFormat(LEsteemEntry, step);
    }

    public CFrequency? CEsteemFrequencyRead(string once)
    {
        return _cEsteemDisplay.LDisplayFrequencyRead(LEsteemEntry, once);
    }
}
