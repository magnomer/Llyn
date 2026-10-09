using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceEngineLanguage
{
    internal static async Task<IReadOnlyList<LEnsignRow>> TEngineEnsignLoad(this LEngine engine)
    {
        List<LEnsignRow> kept = [];
        await engine.LEngineLanguage.LEngineEnsignLoad((rows, _) => () => kept.AddRange(rows));
        return kept;
    }

    internal static async Task<IReadOnlyList<LEnsignRow>> TEngineEnsignLoad(
        this LEngine engine, string language, IEnumerable<string> varieties)
    {
        List<LEnsignRow> kept = [];
        await engine.LEngineLanguage.LEngineEnsignLoad(language, varieties, (rows, _) => () => kept.AddRange(rows));
        return kept;
    }

    internal static Task<IReadOnlyList<string>> TEngineEnsignLoad(
        this LEngine engine, Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store) =>
        engine.LEngineLanguage.LEngineEnsignLoad(store);

    internal static IReadOnlyList<LFrequency> TEngineFrequencyRead(this LEngine engine, long entryId) =>
        engine.LEngineHearth.LEngineStaffHeld.LEngineStaffLanguage.LLanguageStaffFrequency.LFrequencyClerkRead(entryId);

    internal static LFrequencyGauge? TEngineFrequencyResolve(this LEngine engine, long entryId, string once) =>
        engine.LEnginePronunciation.LEngineFrequencyResolve(entryId, once);

    internal static void TEngineFrequencyStart(this LEngine engine, long entryId)
    {
        engine.LEnginePronunciation.LEngineFrequencyStart(entryId);
    }

    internal static void TEngineInflectionStart(this LEngine engine, long entryId)
    {
        engine.LEngineVocabulary.LEngineInflectionStart(entryId);
    }

    internal static void TEngineSoundStart(this LEngine engine, long entryId)
    {
        engine.LEngineLanguage.LEngineSoundStart(entryId);
    }

    internal static bool TEngineInflectionCheck(this LEngine engine, long entryId) =>
        engine.LEngineVocabulary.LEngineInflectionCheck(entryId);

    internal static LParadigmView? TEngineInflectionRead(
        this LEngine engine, long entryId, bool pending, bool enabled) =>
        engine.LEngineVocabulary.LEngineInflectionRead(entryId, pending, enabled);

    internal static IReadOnlyList<LParadigmRow> TEngineParadigmScan(this LEngine engine, long entryId) =>
        engine.LEngineVocabulary.LEngineParadigmScan(entryId);

    internal static string TEngineLanguageResolve(this LEngine engine, long entryId) =>
        engine.LEngineVocabulary.LEngineLanguageResolve(entryId);

    internal static IReadOnlyList<LScriptImage> TEngineScriptRead(this LEngine engine, long entryId) =>
        engine.LEngineHearth.LEngineStaffHeld.LEngineStaffLanguage.LLanguageStaffScript.LScriptClerkRead(entryId);

    internal static bool TEngineScriptCheck(this LEngine engine, long entryId) =>
        engine.LEngineScript.LEngineScriptCheck(entryId);

    internal static void TEngineScriptStart(this LEngine engine, long entryId) =>
        engine.LEngineScript.LEngineScriptStart(entryId);

    internal static void TEngineScriptRebuild(this LEngine engine, long entryId) =>
        engine.LEngineScript.LEngineScriptRebuild(entryId);

    internal static IReadOnlyList<LReflexRule> TEngineReflexRead(this LEngine engine, string language) =>
        engine.LEngineReflex.LEngineReflexRead(language);

    internal static IReadOnlyList<LReflexGuise> TEngineGuiseRead(
        this LEngine engine, string language, IReadOnlyList<string> reflexes) =>
        engine.LEngineReflex.LEngineGuiseRead(language, reflexes);

    internal static bool TEngineReflexCheck(this LEngine engine, long entryId) =>
        engine.LEngineReflex.LEngineReflexCheck(entryId);

    internal static void TEngineReflexStart(this LEngine engine, long entryId) =>
        engine.LEngineReflex.LEngineReflexStart(entryId);

    internal static void TEngineReflexRebuild(this LEngine engine, long entryId) =>
        engine.LEngineReflex.LEngineReflexRebuild(entryId);

    internal static long? TEngineStemFind(this LEngine engine, string language, string? key) =>
        engine.LEngineStem.LEngineStemFind(language, key);

    internal static LStem? TEngineStemRead(this LEngine engine, long? id) =>
        engine.LEngineStem.LEngineStemRead(id);

    internal static string? TEngineStemFind(this LEngine engine) =>
        engine.LEngineStem.LEngineStemFind();

    internal static LStemPage TEngineStemResolve(this LEngine engine, long? id) =>
        engine.LEngineStem.LEngineStemResolve(id);

    internal static IReadOnlyList<LVistaRow> TEngineKindredFind(
        this LEngine engine, string language, IReadOnlyList<long> stemIds, string query, LVista? vista = null) =>
        engine.LEngineStem.LEngineKindredFind(language, stemIds, query, vista);
}
