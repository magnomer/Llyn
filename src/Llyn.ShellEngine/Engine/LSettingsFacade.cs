using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LSettingsFacade
{
    private readonly LEngineHearth _lSettingsFacadeHearth;
    private readonly object _lSettingsFacadeGate;

    private LEngineStaff LSettingsFacadeStaff => _lSettingsFacadeHearth.LEngineStaffHeld;

    public LSettingsFacade(LEngineHearth hearth)
    {
        ArgumentNullException.ThrowIfNull(hearth);
        _lSettingsFacadeHearth = hearth;

        _lSettingsFacadeGate = _lSettingsFacadeHearth.LEngineGate;
    }

    internal event Action? LEngineFoldChanged;

    internal LSettings LEngineSettingsRead()
    {
        return _lSettingsFacadeHearth.LEngineSettingsRead();
    }

    internal bool LEnginePostureLoad(string name, out LPostureState? state)
    {
        lock (_lSettingsFacadeGate)
        {
            return LSettingsFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffWorkspace
                .LWorkspacePostureRead(name, out state);
        }
    }

    internal bool LEnginePostureSave(string name, LPostureState state, out Exception? fault)
    {
        lock (_lSettingsFacadeGate)
        {
            return LSettingsFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffWorkspace
                .LWorkspacePostureSave(name, state, out fault);
        }
    }

    internal DateTimeOffset LEngineStampRead()
    {
        lock (_lSettingsFacadeGate)
        {
            return LSettingsFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffWorkspace.LWorkspaceClockRead();
        }
    }

    internal string LEngineTrailNormalize(string name)
    {
        lock (_lSettingsFacadeGate)
        {
            return LSettingsFacadeStaff.LEngineStaffLanguage.LLanguageStaffTrail.LTrailNameNormalize(name);
        }
    }

    internal IReadOnlyDictionary<string, string> LEngineLocalizationLoad(string language)
    {
        lock (_lSettingsFacadeGate)
        {
            return LSettingsFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffWorkspace
                .LWorkspaceLocalizationLoad(language);
        }
    }

    internal string LEngineLocalizationRead()
    {
        return LLocalization.LLocalizationNormalize(LEngineSettingsRead().LSettingsLocalization);
    }

    internal IReadOnlyList<string> LEngineLocalizationScan() => LLocalization.LLocalizationListedRead();

    internal string LEngineTextRead(string key) => LLocalization.QLocalizationTextRead(key);

    internal string? LEngineTextFind(string key) => LLocalization.QLocalizationTextFind(key);

    internal IReadOnlyList<string> LEngineGroupFind(
        IReadOnlyList<(string, IReadOnlyList<string>)> groups, string? text) =>
        LLocalization.LLocalizationGroupFind(groups, text);

    internal void LEngineLocalizationSave(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        if (LEngineSettingsChange(settings => settings with { LSettingsLocalization = language }))
        {
            _lSettingsFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectSettings, 0);
        }
    }

    internal void LEngineRespellingSave(bool respelled)
    {
        if (LEngineSettingsChange(settings => settings with { LSettingsRespelled = respelled }))
        {
            _lSettingsFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectSettings, 0);
        }
    }

    internal bool LEngineRespellingCheck(string language)
    {
        lock (_lSettingsFacadeGate)
        {
            return LSettingsFacadeStaff.LEngineStaffLanguage.LLanguageStaffLanguage.LLanguageRespellingCheck(
                language, _lSettingsFacadeHearth.LEngineSettingsHeld.LSettingsRespelled);
        }
    }

    internal string LEnginePronunciationRead(LEntryDraft draft)
    {
        lock (_lSettingsFacadeGate)
        {
            return LSettingsFacadeStaff.LEngineStaffLanguage.LLanguageStaffLanguage.LLanguagePronunciationRead(
                draft, _lSettingsFacadeHearth.LEngineSettingsHeld.LSettingsRespelled);
        }
    }
    internal bool LEnginePhonemicCheck(string language)
    {
        lock (_lSettingsFacadeGate)
        {
            return LSettingsFacadeStaff.LEngineStaffLanguage.LLanguageStaffLanguage.LLanguagePhonemicCheck(language);
        }
    }

    internal (bool, string, string) LEngineMarkRead(string language)
    {
        bool respelled = LEngineRespellingCheck(language);
        bool slashed = respelled && LEnginePhonemicCheck(language);
        return (respelled, slashed ? "/" : "[", slashed ? "/" : "]");
    }

    internal void LEngineEpithetSave(bool epithet)
    {
        if (LEngineSettingsChange(settings => settings with { LSettingsEpithet = epithet }))
        {
            _lSettingsFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectSettings, 0);
        }
    }

    internal void LEngineTallySave(bool respelled) =>
        LEngineSettingsChange(settings => settings with { LSettingsTally = respelled });

    internal void LEngineFanqieSave(bool opened)
    {
        if (LEngineSettingsChange(settings => settings with { LSettingsFanqieOpened = opened }))
        {
            LEngineFoldChanged?.Invoke();
        }
    }

    internal void LEngineScriptSave(bool opened)
    {
        if (LEngineSettingsChange(settings => settings with { LSettingsScriptOpened = opened }))
        {
            LEngineFoldChanged?.Invoke();
        }
    }

    internal void LEngineFrequencySave(bool frequency)
    {
        if (LEngineSettingsChange(settings => settings with { LSettingsFrequency = frequency }))
        {
            _lSettingsFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectSettings, 0);
        }
    }

    internal bool LEngineMorphologyCheck()
    {
        return LEngineSettingsRead().LSettingsMorphology;
    }

    internal void LEngineMorphologySave(bool morphology)
    {
        bool changed;
        lock (_lSettingsFacadeGate)
        {
            changed = LEngineSettingsChange(settings => settings with { LSettingsMorphology = morphology });
            if (changed && !morphology)
            {
                LSettingsFacadeStaff.LEngineStaffLanguage.LLanguageStaffLacuna.LLacunaClerkClear();
            }
        }

        if (changed)
        {
            _lSettingsFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectSettings, 0);
        }
    }

    internal bool LEngineAnalysisCheck()
    {
        return LEngineSettingsRead().LSettingsAnalysis;
    }

    internal void LEngineAnalysisSave(bool analysis)
    {
        if (LEngineSettingsChange(settings => settings with { LSettingsAnalysis = analysis }))
        {
            _lSettingsFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectSettings, 0);
            _lSettingsFacadeHearth.LEngineBulletinRaise(LSubject.LSubjectInflection, 0);
        }
    }

    internal bool LEngineSettingsChange(Func<LSettings, LSettings> change)
    {
        lock (_lSettingsFacadeGate)
        {
            LSettings held = _lSettingsFacadeHearth.LEngineSettingsHeld;
            LSettings changed = change(held);
            if (changed == held)
            {
                return false;
            }

            _lSettingsFacadeHearth.LEngineSettingsHeld = changed;
            try
            {
                LSettingsFacadeStaff.LEngineStaffWorkspace.LWorkspaceStaffWorkspace.LWorkspaceSettingsSave(changed);
            }
            catch
            {
                _lSettingsFacadeHearth.LEngineSettingsHeld = held;
                throw;
            }

            return true;
        }
    }
}
