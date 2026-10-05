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

    bool LEngineWorkspaceCheck(string chosen);

    LWorkspaceState LEngineWorkspaceChange(string chosen);

    LWorkspaceState LEngineWorkspaceStart();

    void LEngineFolderOpen();

    IReadOnlyDictionary<string, string> LEngineLocalizationLoad(string language);

    string LEngineLocalizationRead();

    IReadOnlyList<string> LEngineLocalizationScan();

    string LEngineTextRead(string key);

    string? LEngineTextFind(string key);

    IReadOnlyList<string> LEngineGroupFind(IReadOnlyList<(string, IReadOnlyList<string>)> groups, string? text);

    void LEngineLocalizationSave(string language);

    void LEngineEpithetSave(bool epithet);

    void LEngineFrequencySave(bool frequency);

    void LEngineOutpostSave(string text);

    bool LEngineMorphologyCheck();

    void LEngineMorphologySave(bool morphology);

    void LEngineRespellingSave(bool respelled);

    void LEngineFanqieSave(bool opened);

    void LEngineScriptSave(bool opened);

    event Action? LEngineFoldChanged;

    (string LEngineFailureNotice, string? LEngineFailureLabel, string? LEngineFailurePath) LEngineFailureRead(
        Exception exception, string unexpected, string recorded);

    LFont LEngineFontRead(string language, LFontRole role);

    Task<IReadOnlyList<string>> LEngineEnsignLoad(
        Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store);

    LEstablishment LEngineEstablishmentRead();

    IReadOnlyList<string> LEngineLanguageRead();


    static string LEngineEnsignFormat(string language, string variety)
    {
        ArgumentNullException.ThrowIfNull(variety);

        return variety.Length == 0 ? string.Empty : LEnsign.LEnsignKeyFormat(language, variety);
    }
}
