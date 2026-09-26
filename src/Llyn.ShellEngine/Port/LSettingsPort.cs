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

    LWorkspaceState LEngineStateRead();

    void LEngineLeftSave(long? id);

    void LEngineRightSave(long? id);

    IReadOnlyDictionary<string, string> LEngineLocalizationLoad(string language);

    string LEngineLocalizationRead();

    IReadOnlyList<string> LEngineLocalizationScan();

    string LEngineTextRead(string key);

    string? LEngineTextFind(string key);

    void LEngineLocalizationSave(string language);

    void LEngineEpithetSave(bool epithet);

    void LEngineFrequencySave(bool frequency);

    void LEngineMorphologySave(bool morphology);

    void LEngineRespellingSave(bool respelled);

    string? LEngineAuditRecord(Exception exception);

    string? LEngineNoticeRead(Exception exception);

    LFont LEngineFontRead(string language, LFontRole role);

    Task LEngineEnsignLoad(Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store);

    Task LEngineEnsignLoad(
        string language,
        IEnumerable<string> varieties,
        Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store);

    LEstablishment LEngineEstablishmentRead();

    IReadOnlyList<string> LEngineLanguageRead();
}
