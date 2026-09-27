using System;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QEsteem
{
    private readonly LDesk _qEsteemDesk;

    private readonly LDisplay _qEsteemDisplay;

    internal QEsteem(LDesk desk, LDisplay display)
    {
        ArgumentNullException.ThrowIfNull(desk);
        ArgumentNullException.ThrowIfNull(display);

        _qEsteemDesk = desk;
        _qEsteemDisplay = display;
    }

    public event Action? QEsteemFavoriteChanged;

    public event Action? QEsteemGraspChanged;

    public bool QEsteemFavorite => _qEsteemDisplay.LDisplayFavoriteRead(QEsteemEntry);

    public int QEsteemGrasp => _qEsteemDisplay.LDisplayGraspRead(QEsteemEntry);

    public int QEsteemGraspStep => _qEsteemDisplay.LDisplayGraspStep;

    private long? QEsteemEntry => _qEsteemDesk.LDeskStoredRead();

    public void QEsteemFavoriteSet(bool marked)
    {
        _qEsteemDisplay.LDisplayFavoriteSave(QEsteemEntry, marked);
        QEsteemFavoriteChanged?.Invoke();
    }

    public void QEsteemGraspSet(int step)
    {
        _qEsteemDisplay.LDisplayGraspSave(QEsteemEntry, step);
        QEsteemGraspChanged?.Invoke();
    }

    public string QEsteemGraspFormat(int step)
    {
        return _qEsteemDisplay.LDisplayGraspFormat(QEsteemEntry, step);
    }

    public CFrequency? QEsteemFrequencyRead(string once)
    {
        if (QEsteemEntry is null)
        {
            return null;
        }

        return LSounding.LSoundingFrequencyRead(_qEsteemDisplay.LDisplayFrequencyRead(QEsteemEntry), once);
    }
}
