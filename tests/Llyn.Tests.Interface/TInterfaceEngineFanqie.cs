using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceEngineFanqie
{
    internal static IReadOnlyList<LFanqieRow> TEngineFanqieRead(this LEngine engine, long entryId) =>
        engine.LEngineHearth.LEngineStaffHeld.LEngineStaffLanguage.LLanguageStaffFanqie.LFanqieClerkRead(entryId);

    internal static IReadOnlyList<LFanqieRow> TEngineFanqieRead(this LEngine engine, LDiwei diwei) =>
        engine.LEngineHearth.LEngineStaffHeld.LEngineStaffLanguage.LLanguageStaffDiwei.LDiweiFanqieRead(diwei);

    internal static LDiwei? TEngineDiweiFind(this LEngine engine, string language, string kind, string key) =>
        engine.LEngineHearth.LEngineStaffHeld.LEngineStaffLanguage.LLanguageStaffDiwei
            .LDiweiClerkFind(language, kind, key);

    internal static (long, bool)? TFanqieDiweiFind(this LEngine engine, string language, string kind, string key) =>
        engine.LEngineFanqie.LEngineDiweiFind(language, kind, key);

    internal static IReadOnlyList<LTally> TEngineTallyRead(this LEngine engine, LDiwei diwei) =>
        engine.LEngineHearth.LEngineStaffHeld.LEngineStaffLanguage.LLanguageStaffDiwei.LTallyRead(diwei);

    internal static void TEngineFanqieSet(this LEngine engine, long entryId, long fanqieId, int rank, bool raise) =>
        engine.LEngineFanqie.LEngineFanqieSet(entryId, fanqieId, rank, raise);

    internal static bool TEngineFanqieCheck(this LEngine engine, long entryId) =>
        engine.LEngineFanqie.LEngineFanqieCheck(entryId);

    internal static void TEngineFanqieStart(this LEngine engine, long entryId) =>
        engine.LEngineFanqie.LEngineFanqieStart(entryId);

    internal static IReadOnlyList<LFanqieGroup> TEngineFanqieDivide(this LEngine engine, long entryId) =>
        engine.LEngineFanqie.LEngineFanqieDivide(entryId);
}
