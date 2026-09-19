using System;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LDisplay
{
    private readonly LEngine _lEngine;

    private bool _lDisplayOpened;

    public LDisplay(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);

        _lEngine = engine;
    }

    public bool LDisplayFoldOpened => _lDisplayOpened;

    public void LDisplayFoldSet(bool opened)
    {
        _lDisplayOpened = opened;
    }

    public bool LDisplayFanqieCheck(long? id)
    {
        return LDisplayPendingRead(_lEngine.LEngineFanqieCheck, id);
    }

    public bool LDisplayScriptCheck(long? id)
    {
        return LDisplayPendingRead(_lEngine.LEngineScriptCheck, id);
    }

    private static bool LDisplayPendingRead(Func<long, bool> check, long? id)
    {
        if (id is not long shown)
        {
            return false;
        }

        try
        {
            return check(shown);
        }
        catch (Exception)
        {
            return false;
        }
    }
}
