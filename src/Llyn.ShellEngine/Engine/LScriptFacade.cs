using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LScriptFacade : LScriptPort
{
    private readonly LEngineHearth _lScriptFacadeHearth;

    internal LScriptFacade(LEngineHearth hearth)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        _lScriptFacadeHearth = hearth;
    }

    public IReadOnlyList<LScriptStyle> LEngineStyleRead(string language)
    {
        lock (_lScriptFacadeHearth.LEngineGate)
        {
            return _lScriptFacadeHearth.LEngineStaffHeld.LEngineStaffLanguage.LLanguageStaffScript
                .LScriptStyleRead(language);
        }
    }

    public bool LEngineStyleCheck(string language)
    {
        return LEngineStyleRead(language).Count > 0;
    }

    public void LEngineScriptStart(long entryId)
    {
        _lScriptFacadeHearth.LEngineStaffHeld.LEngineStaffLanguage.LLanguageStaffScript.LScriptClerkStart(entryId);
    }

    public void LEngineScriptRebuild(long entryId)
    {
        _lScriptFacadeHearth.LEngineStaffHeld.LEngineStaffLanguage.LLanguageStaffScript.LScriptClerkRebuild(entryId);
    }

    public IReadOnlyList<LScriptGroup> LEngineScriptDivide(long entryId)
    {
        return _lScriptFacadeHearth.LEngineStaffHeld.LEngineStaffLanguage.LLanguageStaffScript
            .LScriptClerkDivide(entryId);
    }

    public IReadOnlyList<LScriptGroup> LEngineScriptRead(long entryId)
    {
        LEngineScriptStart(entryId);
        return LEngineScriptDivide(entryId);
    }

    public bool LEngineScriptCheck(long entryId)
    {
        return _lScriptFacadeHearth.LEngineStaffHeld.LEngineStaffLanguage.LLanguageStaffScript
            .LScriptClerkCheck(entryId);
    }
}
