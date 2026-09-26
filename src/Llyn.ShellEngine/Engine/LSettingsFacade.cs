using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LSettingsFacade
{
    private readonly LEngine _lSettingsFacadeEngine;
    private readonly object _lSettingsFacadeGate;

    private LEngineStaff LSettingsFacadeStaff => _lSettingsFacadeEngine.LEngineStaffHeld;

    public LSettingsFacade(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lSettingsFacadeEngine = engine;
        _lSettingsFacadeGate = engine.LEngineGate;
    }

    internal LSettings LEngineSettingsRead()
    {
        return _lSettingsFacadeEngine.LEngineSettingsRead();
    }

    internal bool LEnginePostureLoad(string name, out LPostureState? state)
    {
        lock (_lSettingsFacadeGate)
        {
            return LSettingsFacadeStaff.LEngineStaffWorkspace.LWorkspacePostureRead(name, out state);
        }
    }

    internal void LEnginePostureSave(string name, LPostureState state)
    {
        lock (_lSettingsFacadeGate)
        {
            LSettingsFacadeStaff.LEngineStaffWorkspace.LWorkspacePostureSave(name, state);
        }
    }

    internal DateTimeOffset LEngineStampRead()
    {
        lock (_lSettingsFacadeGate)
        {
            return LSettingsFacadeStaff.LEngineStaffWorkspace.LWorkspaceClockRead();
        }
    }

    internal string LEngineTrailNormalize(string name)
    {
        lock (_lSettingsFacadeGate)
        {
            return LSettingsFacadeStaff.LEngineStaffTrail.LTrailNameNormalize(name);
        }
    }

    internal IReadOnlyDictionary<string, string> LEngineLocalizationLoad(string language)
    {
        lock (_lSettingsFacadeGate)
        {
            return LSettingsFacadeStaff.LEngineStaffWorkspace.LWorkspaceLocalizationLoad(language);
        }
    }

    internal string LEngineLocalizationRead()
    {
        return LLocalization.LLocalizationNormalize(LEngineSettingsRead().LSettingsLocalization);
    }

    internal IReadOnlyList<string> LEngineLocalizationScan() => LLocalization.LLocalizationListedRead();

    internal string LEngineTextRead(string key) => LLocalization.LLocalizationTextRead(key);

    internal string? LEngineTextFind(string key) => LLocalization.LLocalizationTextFind(key);

    internal void LEngineLocalizationSave(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        if (LEngineSettingsChange(settings => settings with { LSettingsLocalization = language }))
        {
            _lSettingsFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectSettings, 0);
        }
    }

    internal void LEngineRespellingSave(bool respelled)
    {
        if (LEngineSettingsChange(settings => settings with { LSettingsRespelled = respelled }))
        {
            _lSettingsFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectSettings, 0);
        }
    }

    internal bool LEngineRespellingCheck(string language)
    {
        lock (_lSettingsFacadeGate)
        {
            return LSettingsFacadeStaff.LEngineStaffLanguage.LLanguageRespellingCheck(
                language, _lSettingsFacadeEngine.LEngineSettingsHeld.LSettingsRespelled);
        }
    }

    internal bool LEnginePhonemicCheck(string language)
    {
        lock (_lSettingsFacadeGate)
        {
            return LSettingsFacadeStaff.LEngineStaffLanguage.LLanguagePhonemicCheck(language);
        }
    }

    internal void LEngineEpithetSave(bool epithet)
    {
        if (LEngineSettingsChange(settings => settings with { LSettingsEpithet = epithet }))
        {
            _lSettingsFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectSettings, 0);
        }
    }

    internal void LEngineTallySave(bool respelled) =>
        LEngineSettingsChange(settings => settings with { LSettingsTally = respelled });

    internal void LEngineFrequencySave(bool frequency)
    {
        if (LEngineSettingsChange(settings => settings with { LSettingsFrequency = frequency }))
        {
            _lSettingsFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectSettings, 0);
        }
    }

    internal void LEngineMorphologySave(bool morphology)
    {
        bool changed;
        lock (_lSettingsFacadeGate)
        {
            changed = LEngineSettingsChange(settings => settings with { LSettingsMorphology = morphology });
            if (changed && !morphology)
            {
                LSettingsFacadeStaff.LEngineStaffLacuna.LLacunaClerkClear();
            }
        }

        if (changed)
        {
            _lSettingsFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectSettings, 0);
        }
    }

    private bool LEngineSettingsChange(Func<LSettings, LSettings> change)
    {
        lock (_lSettingsFacadeGate)
        {
            LSettings changed = change(_lSettingsFacadeEngine.LEngineSettingsHeld);
            if (changed == _lSettingsFacadeEngine.LEngineSettingsHeld)
            {
                return false;
            }

            _lSettingsFacadeEngine.LEngineSettingsHeld = changed;
            LSettingsFacadeStaff.LEngineStaffWorkspace.LWorkspaceSettingsSave(
                _lSettingsFacadeEngine.LEngineSettingsHeld);
            return true;
        }
    }
}
