using System;
using System.Collections.Generic;
using System.IO;
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

    public LKeep LEngineKeepRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineKeep;
        }
    }

    public IReadOnlyDictionary<string, string> LEngineLocalizationLoad(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);

        return LLocalization.LLocalizationLoad(_lEngineLocalization, language);
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
            return _lEngineSettings.LSettingsRespelled
                && language.Trim().Length > 0
                && LEngineLanguageLoad(language).LLanguageRespellings.Count > 0;
        }
    }

    public bool LEnginePhonemicCheck(string language)
    {
        return language.Trim().Length > 0 && LEngineLanguageLoad(language).LLanguagePhonemic;
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
                LEngineInflectionClear();
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
            LEngineSettingsSave();
            return true;
        }
    }

    private void LEngineSettingsSave()
    {
        try
        {
            _lEngineSettingsVault.LSettingsSave(_lEngineSettings);
        }
        catch (IOException exception)
        {
            _lEngineAudit.LAuditRecord(exception);
        }
        catch (UnauthorizedAccessException exception)
        {
            _lEngineAudit.LAuditRecord(exception);
        }
    }
}
