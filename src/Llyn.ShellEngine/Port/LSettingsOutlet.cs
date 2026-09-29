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

    public LSettings LEngineSettingsRead() => _lSettingsOutletEngine.LEngineSettings.LEngineSettingsRead();

    public string LEngineWorkspaceRead() => _lSettingsOutletEngine.LEngineWorkspaceRead();

    public string LEngineWorkspaceFormat() => _lSettingsOutletEngine.LEngineWorkspaceFormat();

    public bool LEngineWorkspaceCheck(string chosen) => _lSettingsOutletEngine.LEngineWorkspaceCheck(chosen);

    public LWorkspaceState LEngineWorkspaceChange(string chosen) =>
        _lSettingsOutletEngine.LEngineWorkspace.LEngineWorkspaceChange(chosen);

    public LWorkspaceState LEngineWorkspaceStart() =>
        _lSettingsOutletEngine.LEngineWorkspace.LEngineWorkspaceStart();

    public IReadOnlyDictionary<string, string> LEngineLocalizationLoad(string language) =>
        _lSettingsOutletEngine.LEngineSettings.LEngineLocalizationLoad(language);

    public string LEngineLocalizationRead() => _lSettingsOutletEngine.LEngineSettings.LEngineLocalizationRead();

    public IReadOnlyList<string> LEngineLocalizationScan() =>
        _lSettingsOutletEngine.LEngineSettings.LEngineLocalizationScan();

    public string LEngineTextRead(string key) => _lSettingsOutletEngine.LEngineSettings.LEngineTextRead(key);

    public string? LEngineTextFind(string key) => _lSettingsOutletEngine.LEngineSettings.LEngineTextFind(key);

    public void LEngineLocalizationSave(string language) =>
        _lSettingsOutletEngine.LEngineSettings.LEngineLocalizationSave(language);

    public void LEngineEpithetSave(bool epithet) => _lSettingsOutletEngine.LEngineSettings.LEngineEpithetSave(epithet);

    public void LEngineFrequencySave(bool frequency) =>
        _lSettingsOutletEngine.LEngineSettings.LEngineFrequencySave(frequency);

    public bool LEngineMorphologyCheck() => _lSettingsOutletEngine.LEngineSettings.LEngineMorphologyCheck();

    public void LEngineMorphologySave(bool morphology) =>
        _lSettingsOutletEngine.LEngineSettings.LEngineMorphologySave(morphology);

    public void LEngineRespellingSave(bool respelled) =>
        _lSettingsOutletEngine.LEngineSettings.LEngineRespellingSave(respelled);

    public (string LEngineFailureNotice, string? LEngineFailureLabel, string? LEngineFailurePath) LEngineFailureRead(
        Exception exception, string unexpected, string recorded) =>
        _lSettingsOutletEngine.LEngineFailureRead(exception, unexpected, recorded);

    public LFont LEngineFontRead(string language, LFontRole role) =>
        _lSettingsOutletEngine.LEngineLanguage.LEngineFontRead(language, role);

    public Task<IReadOnlyList<string>> LEngineEnsignLoad(
        Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store) =>
        _lSettingsOutletEngine.LEngineLanguage.LEngineEnsignLoad(store);

    public Task LEngineEnsignLoad(
        string language,
        IEnumerable<string> varieties,
        Func<IReadOnlyList<LEnsignRow>, Action<string, Exception>, Action> store) =>
        _lSettingsOutletEngine.LEngineLanguage.LEngineEnsignLoad(language, varieties, store);


    public LEstablishment LEngineEstablishmentRead() => _lSettingsOutletEngine.LEngineEntry.LEngineEstablishmentRead();

    public IReadOnlyList<string> LEngineLanguageRead() => _lSettingsOutletEngine.LEngineLanguage.LEngineLanguageRead();

    public string LEngineGlossRead() => _lSettingsOutletEngine.LEngineLanguage.LEngineGlossRead();
}
