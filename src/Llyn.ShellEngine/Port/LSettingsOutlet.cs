using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LSettingsOutlet : LSettingsPort
{
    private readonly LEngine _lSettingsOutletEngine;

    public LSettingsOutlet(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lSettingsOutletEngine = engine;
    }

    public LSettings LEngineSettingsRead() => _lSettingsOutletEngine.LEngineSettingsRead();

    public string LEngineWorkspaceRead() => _lSettingsOutletEngine.LEngineWorkspaceRead();

    public string LEngineWorkspaceFormat() => _lSettingsOutletEngine.LEngineWorkspaceFormat();

    public LWorkspaceState LEngineStateRead() => _lSettingsOutletEngine.LEngineStateRead();

    public void LEngineLeftSave(long? id) => _lSettingsOutletEngine.LEngineLeftSave(id);

    public void LEngineRightSave(long? id) => _lSettingsOutletEngine.LEngineRightSave(id);

    public IReadOnlyDictionary<string, string> LEngineLocalizationLoad(string language) =>
        _lSettingsOutletEngine.LEngineLocalizationLoad(language);

    public string LEngineLocalizationRead() => _lSettingsOutletEngine.LEngineLocalizationRead();

    public string LEngineTextRead(string key) => _lSettingsOutletEngine.LEngineTextRead(key);

    public string? LEngineTextFind(string key) => _lSettingsOutletEngine.LEngineTextFind(key);

    public void LEngineLocalizationSave(string language) => _lSettingsOutletEngine.LEngineLocalizationSave(language);

    public void LEngineEpithetSave(bool epithet) => _lSettingsOutletEngine.LEngineEpithetSave(epithet);

    public void LEngineFrequencySave(bool frequency) => _lSettingsOutletEngine.LEngineFrequencySave(frequency);

    public void LEngineMorphologySave(bool morphology) => _lSettingsOutletEngine.LEngineMorphologySave(morphology);

    public void LEngineRespellingSave(bool respelled) => _lSettingsOutletEngine.LEngineRespellingSave(respelled);

    public LDoctorRescue LEngineRescueRead() => _lSettingsOutletEngine.LEngineRescueRead();

    public string? LEngineAuditRecord(Exception exception) => _lSettingsOutletEngine.LEngineAuditRecord(exception);

    public string? LEngineNoticeRead(Exception exception) => _lSettingsOutletEngine.LEngineNoticeRead(exception);

    public LFont LEngineFontRead(string language, LFontRole role) =>
        _lSettingsOutletEngine.LEngineFontRead(language, role);

    public Task<IReadOnlyList<LEnsignRow>> LEngineEnsignLoad() => _lSettingsOutletEngine.LEngineEnsignLoad();

    public Task<IReadOnlyList<LEnsignRow>> LEngineEnsignLoad(string language, IEnumerable<string> varieties) =>
        _lSettingsOutletEngine.LEngineEnsignLoad(language, varieties);

    public void LEngineEnsignDelete(string path) => _lSettingsOutletEngine.LEngineEnsignDelete(path);

    public LEstablishment LEngineEstablishmentRead() => _lSettingsOutletEngine.LEngineEstablishmentRead();

    public IReadOnlyList<string> LEngineLanguageRead() => _lSettingsOutletEngine.LEngineLanguageRead();
}
