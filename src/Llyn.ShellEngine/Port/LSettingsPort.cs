using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public interface LSettingsPort
{
    LSettings LEngineSettingsRead();

    string LEngineWorkspaceRead();

    string LEngineWorkspaceFormat();

    void LEngineWorkspaceChange(string path);

    LWorkspaceState LEngineStateRead();

    LWorkspaceState LEngineWorkspaceStart();

    IReadOnlyDictionary<string, string> LEngineLocalizationLoad(string language);

    string LEngineLocalizationRead();

    IReadOnlyList<string> LEngineLocalizationScan();

    string LEngineTextRead(string key);

    string? LEngineTextFind(string key);

    void LEngineLocalizationSave(string language);

    void LEngineEpithetSave(bool epithet);

    void LEngineFrequencySave(bool frequency);

    bool LEngineMorphologyCheck();

    void LEngineMorphologySave(bool morphology);

    void LEngineRespellingSave(bool respelled);

    (string LEngineFailureNotice, string? LEngineFailureLabel, string? LEngineFailurePath) LEngineFailureRead(
        Exception exception, string unexpected, string recorded);

    LFont LEngineFontRead(string language, LFontRole role);

    Task<IReadOnlyList<string>> LEngineEnsignLoad(
        Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store);

    Task LEngineEnsignLoad(
        string language,
        IEnumerable<string> varieties,
        Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store);

    LEstablishment LEngineEstablishmentRead();

    IReadOnlyList<string> LEngineLanguageRead();

    string LEngineGlossRead();

    static string LEngineEnsignFormat(string language, string variety)
    {
        ArgumentNullException.ThrowIfNull(variety);

        return variety.Length == 0 ? string.Empty : LEnsign.LEnsignKeyFormat(language, variety);
    }
}
