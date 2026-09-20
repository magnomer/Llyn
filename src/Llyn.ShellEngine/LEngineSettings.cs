using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LSettings LEngineSettingsRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineSettings;
        }
    }

    internal bool LEnginePostureLoad(string name, out LPostureState? state)
    {
        lock (_lEngineGate)
        {
            return _lEngineWorkspaceClerk.LWorkspacePostureRead(name, out state);
        }
    }

    internal void LEnginePostureSave(string name, LPostureState state)
    {
        lock (_lEngineGate)
        {
            _lEngineWorkspaceClerk.LWorkspacePostureSave(name, state);
        }
    }

    internal DateTimeOffset LEngineStampRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineWorkspaceClerk.LWorkspaceClockRead();
        }
    }

    internal string LEngineTrailNormalize(string name)
    {
        lock (_lEngineGate)
        {
            return _lEngineTrailClerk.LTrailNameNormalize(name);
        }
    }

    public IReadOnlyDictionary<string, string> LEngineLocalizationLoad(string language)
    {
        lock (_lEngineGate)
        {
            return _lEngineWorkspaceClerk.LWorkspaceLocalizationLoad(language);
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
        lock (_lEngineGate)
        {
            return _lEngineLanguageClerk.LLanguageRespellingCheck(language, _lEngineSettings.LSettingsRespelled);
        }
    }

    public bool LEnginePhonemicCheck(string language)
    {
        lock (_lEngineGate)
        {
            return _lEngineLanguageClerk.LLanguagePhonemicCheck(language);
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
        lock (_lEngineGate)
        {
            changed = LEngineSettingsChange(settings => settings with { LSettingsMorphology = morphology });
            if (changed && !morphology)
            {
                _lEngineLacunaClerk.LLacunaClerkClear();
            }
        }

        if (changed)
        {
            LEngineBulletinRaise(LSubject.LSubjectSettings, 0);
        }
    }

    private bool LEngineSettingsChange(Func<LSettings, LSettings> change)
    {
        lock (_lEngineGate)
        {
            LSettings changed = change(_lEngineSettings);
            if (changed == _lEngineSettings)
            {
                return false;
            }

            _lEngineSettings = changed;
            _lEngineWorkspaceClerk.LWorkspaceSettingsSave(_lEngineSettings);
            return true;
        }
    }
}
