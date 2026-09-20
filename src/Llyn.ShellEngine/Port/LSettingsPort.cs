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

    void LEngineLocalizationSave(string language);

    void LEngineEpithetSave(bool epithet);

    void LEngineFrequencySave(bool frequency);

    void LEngineMorphologySave(bool morphology);

    void LEngineRespellingSave(bool respelled);

    LDoctorRescue LEngineRescueRead();

    string? LEngineAuditRecord(Exception exception);

    LFont LEngineFontRead(string language, LFontRole role);

    Task<IReadOnlyList<LEnsignRow>> LEngineEnsignLoad();

    Task<IReadOnlyList<LEnsignRow>> LEngineEnsignLoad(string language, IEnumerable<string> varieties);

    void LEngineEnsignDelete(string path);

    LEstablishment LEngineEstablishmentRead();

    IReadOnlyList<string> LEngineLanguageRead();

    void LEnginePressApply(LPress press);
}
