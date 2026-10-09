using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TInterfaceEngineSettings
{
    internal static LSettings TEngineSettingsRead(this LEngine engine) =>
        engine.LEngineSettings.LEngineSettingsRead();

    internal static string TEngineTextRead(this LEngine engine, string key) =>
        engine.LEngineSettings.LEngineTextRead(key);

    internal static string TEngineLocalizationRead(this LEngine engine)
    {
        return engine.LEngineSettings.LEngineLocalizationRead();
    }

    internal static void TEngineLocalizationSave(this LEngine engine, string language)
    {
        engine.LEngineSettings.LEngineLocalizationSave(language);
    }

    internal static void TEngineEpithetSave(this LEngine engine, bool epithet) =>
        engine.LEngineSettings.LEngineEpithetSave(epithet);

    internal static void TEngineRespellingSave(this LEngine engine, bool respelled)
    {
        engine.LEngineSettings.LEngineRespellingSave(respelled);
    }

    internal static void TEngineTallySave(this LEngine engine, bool respelled)
    {
        engine.LEngineSettings.LEngineTallySave(respelled);
    }

    internal static bool TEngineRespellingCheck(this LEngine engine, string language) =>
        engine.LEngineSettings.LEngineRespellingCheck(language);

    internal static bool TEnginePhonemicCheck(this LEngine engine, string language) =>
        engine.LEngineSettings.LEnginePhonemicCheck(language);

    internal static (bool, string, string) TEngineMarkRead(this LEngine engine, string language) =>
        engine.LEngineSettings.LEngineMarkRead(language);

    internal static void TEngineFrequencySave(this LEngine engine, bool frequency)
    {
        engine.LEngineSettings.LEngineFrequencySave(frequency);
    }

    internal static void TEngineMorphologySave(this LEngine engine, bool morphology)
    {
        engine.LEngineSettings.LEngineMorphologySave(morphology);
    }

    internal static bool TEngineAnalysisCheck(this LEngine engine) => engine.LEngineSettings.LEngineAnalysisCheck();

    internal static void TEngineAnalysisSave(this LEngine engine, bool analysis)
    {
        engine.LEngineSettings.LEngineAnalysisSave(analysis);
    }
}
