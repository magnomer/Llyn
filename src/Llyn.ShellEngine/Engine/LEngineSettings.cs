using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LSettings LEngineSettingsRead()
    {
        lock (LEngineGate)
        {
            return LEngineSettingsHeld;
        }
    }

    internal bool LEnginePostureLoad(string name, out LPostureState? state)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffWorkspace.LWorkspacePostureRead(name, out state);
        }
    }

    internal void LEnginePostureSave(string name, LPostureState state)
    {
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffWorkspace.LWorkspacePostureSave(name, state);
        }
    }

    internal DateTimeOffset LEngineStampRead()
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffWorkspace.LWorkspaceClockRead();
        }
    }

    internal string LEngineTrailNormalize(string name)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffTrail.LTrailNameNormalize(name);
        }
    }

    public IReadOnlyDictionary<string, string> LEngineLocalizationLoad(string language)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffWorkspace.LWorkspaceLocalizationLoad(language);
        }
    }

    public string LEngineLocalizationRead()
    {
        return LLocalization.LLocalizationNormalize(LEngineSettingsRead().LSettingsLocalization);
    }

    public string LEngineTextRead(string key)
    {
        return LLocalization.LLocalizationTextRead(key);
    }

    public string? LEngineTextFind(string key)
    {
        return LLocalization.LLocalizationTextFind(key);
    }

    public void LEngineLocalizationSave(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        if (LEngineSettingsChange(settings => settings with { LSettingsLocalization = language }))
        {
            LEngineBulletinRaise(LSubject.LSubjectSettings, 0);
        }
    }

    public void LEngineRespellingSave(bool respelled)
    {
        if (LEngineSettingsChange(settings => settings with { LSettingsRespelled = respelled }))
        {
            LEngineBulletinRaise(LSubject.LSubjectSettings, 0);
        }
    }

    public bool LEngineRespellingCheck(string language)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffLanguage.LLanguageRespellingCheck(
                language, LEngineSettingsHeld.LSettingsRespelled);
        }
    }

    public bool LEnginePhonemicCheck(string language)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffLanguage.LLanguagePhonemicCheck(language);
        }
    }

    public void LEngineEpithetSave(bool epithet)
    {
        if (LEngineSettingsChange(settings => settings with { LSettingsEpithet = epithet }))
        {
            LEngineBulletinRaise(LSubject.LSubjectSettings, 0);
        }
    }

    public void LEngineTallySave(bool respelled)
    {
        LEngineSettingsChange(settings => settings with { LSettingsTally = respelled });
    }

    public void LEngineFrequencySave(bool frequency)
    {
        if (LEngineSettingsChange(settings => settings with { LSettingsFrequency = frequency }))
        {
            LEngineBulletinRaise(LSubject.LSubjectSettings, 0);
        }
    }

    public void LEngineMorphologySave(bool morphology)
    {
        bool changed;
        lock (LEngineGate)
        {
            changed = LEngineSettingsChange(settings => settings with { LSettingsMorphology = morphology });
            if (changed && !morphology)
            {
                _lEngineStaff.LEngineStaffLacuna.LLacunaClerkClear();
            }
        }

        if (changed)
        {
            LEngineBulletinRaise(LSubject.LSubjectSettings, 0);
        }
    }

    private bool LEngineSettingsChange(Func<LSettings, LSettings> change)
    {
        lock (LEngineGate)
        {
            LSettings changed = change(LEngineSettingsHeld);
            if (changed == LEngineSettingsHeld)
            {
                return false;
            }

            LEngineSettingsHeld = changed;
            _lEngineStaff.LEngineStaffWorkspace.LWorkspaceSettingsSave(LEngineSettingsHeld);
            return true;
        }
    }
}
