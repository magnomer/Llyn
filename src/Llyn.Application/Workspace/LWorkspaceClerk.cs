using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LWorkspaceClerk
{
    private readonly LVault _lWorkspaceClerkVault;
    private readonly LSettingsVault _lWorkspaceClerkSettings;
    private readonly LAuditVault _lWorkspaceClerkAudit;
    private readonly LPostureVault _lWorkspaceClerkPosture;
    private readonly LWorkspaceVault _lWorkspaceClerkWorkspaces;
    private readonly LEntryVault _lWorkspaceClerkEntries;
    private readonly LPronunciationVault _lWorkspaceClerkPronunciations;
    private readonly LReflexVault _lWorkspaceClerkReflexes;
    private readonly LLocalizationVault _lWorkspaceClerkLocalization;
    private readonly LClock _lWorkspaceClerkClock;
    private readonly LLanguageCache _lWorkspaceClerkLanguages;
    private readonly LReflexClerk _lWorkspaceClerkEpithets;
    private readonly LParadigmClerk _lWorkspaceClerkParadigms;

    public LWorkspaceClerk(LRig rig, LLanguageCache languages, LReflexClerk reflexes, LParadigmClerk paradigms)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(reflexes);
        ArgumentNullException.ThrowIfNull(paradigms);
        _lWorkspaceClerkVault = rig.LRigVault;
        _lWorkspaceClerkSettings = rig.LRigSettings;
        _lWorkspaceClerkAudit = rig.LRigAudit;
        _lWorkspaceClerkPosture = rig.LRigPosture;
        _lWorkspaceClerkWorkspaces = rig.LRigWorkspaces;
        _lWorkspaceClerkEntries = rig.LRigEntries;
        _lWorkspaceClerkPronunciations = rig.LRigPronunciations;
        _lWorkspaceClerkReflexes = rig.LRigReflexes;
        _lWorkspaceClerkLocalization = rig.LRigLocalization;
        _lWorkspaceClerkClock = rig.LRigClock;
        _lWorkspaceClerkLanguages = languages;
        _lWorkspaceClerkEpithets = reflexes;
        _lWorkspaceClerkParadigms = paradigms;
    }

    public static LDoctorRescue LWorkspaceRescueCreate(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        return rig.LRigDoctor.LDoctorDatabaseCreate();
    }

    public static LRealm LWorkspaceRealmRead(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        return rig.LRigRealm.LRealmRead();
    }

    public static LSettings LWorkspaceSettingsRead(LRig rig, LSettings fallback, out bool settled)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(fallback);

        settled = rig.LRigSettings.LSettingsExist();
        return settled ? rig.LRigSettings.LSettingsRead() : fallback;
    }

    public LSettings LWorkspaceSettingsRead()
    {
        return _lWorkspaceClerkSettings.LSettingsRead();
    }

    public void LWorkspaceSettingsSave(LSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        try
        {
            _lWorkspaceClerkSettings.LSettingsSave(settings);
        }
        catch (LVaultFault exception)
        {
            _lWorkspaceClerkAudit.LAuditRecord(exception);
        }
    }

    public bool LWorkspaceClerkMigrated => _lWorkspaceClerkVault.LVaultMigrated;

    public string? LWorkspaceAuditRecord(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        return _lWorkspaceClerkAudit.LAuditRecord(exception);
    }

    public static string? LWorkspaceNoticeRead(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        return exception is LRefusal refusal
            ? refusal.LRefusalReason
            : exception.InnerException is null ? null : LWorkspaceNoticeRead(exception.InnerException);
    }

    public IReadOnlyDictionary<string, string> LWorkspaceLocalizationLoad(string language)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        return LLocalization.LLocalizationLoad(_lWorkspaceClerkLocalization, language);
    }

    public DateTimeOffset LWorkspaceClockRead()
    {
        return _lWorkspaceClerkClock.LClockRead();
    }

    public LWorkspaceState LWorkspaceStateRead()
    {
        return _lWorkspaceClerkWorkspaces.LWorkspaceStateRead();
    }

    public void LWorkspaceStateSave(LWorkspaceState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        _lWorkspaceClerkWorkspaces.LWorkspaceStateSave(state);
    }

    public bool LWorkspacePostureRead(string name, out LPostureState? state)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        try
        {
            state = _lWorkspaceClerkPosture.LPostureRead(name);
            return true;
        }
        catch (LVaultFault exception)
        {
            _lWorkspaceClerkAudit.LAuditRecord(exception);
            state = null;
            return false;
        }
    }

    public void LWorkspacePostureSave(string name, LPostureState state)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(state);

        try
        {
            _lWorkspaceClerkPosture.LPostureSave(name, state);
        }
        catch (LVaultFault exception)
        {
            _lWorkspaceClerkAudit.LAuditRecord(exception);
        }
    }

    public void LWorkspaceClerkUpdate()
    {
        using LVaultSession session = _lWorkspaceClerkVault.LVaultSessionStart();
        foreach (LEntry entry in _lWorkspaceClerkEntries.LEntryFind(string.Empty))
        {
            try
            {
                LRespellingUpdate(entry);
                _lWorkspaceClerkEpithets.LReflexEpithetSave(entry.LEntryId);
                _lWorkspaceClerkParadigms.LParadigmClerkUpdate(entry);
            }
            catch (Exception exception) when (exception is not OutOfMemoryException)
            {
                _lWorkspaceClerkAudit.LAuditRecord(exception);
            }
        }

        session.LVaultSessionCommit();
    }

    private void LRespellingUpdate(LEntry entry)
    {
        LPronunciationVault pronunciations = _lWorkspaceClerkPronunciations;
        foreach (LPronunciation row in pronunciations.LPronunciationRead(entry.LEntryId))
        {
            if (!string.IsNullOrEmpty(row.LPronunciationRespelling) || string.IsNullOrEmpty(row.LPronunciationIpa))
            {
                continue;
            }

            LPronunciationDraft spoken = _lWorkspaceClerkLanguages.LLanguageRespellingResolve(
                entry.LEntryLanguage,
                new LPronunciationDraft(
                    row.LPronunciationIpa, LPronunciationDraftVariety: row.LPronunciationVariety ?? string.Empty));
            if (spoken.LPronunciationDraftRespelling.Length > 0)
            {
                pronunciations.LPronunciationUpdate(
                    row with { LPronunciationRespelling = spoken.LPronunciationDraftRespelling });
            }
        }

        LReflexVault reflexes = _lWorkspaceClerkReflexes;
        IReadOnlyList<LReflex> stored = reflexes.LReflexRead(entry.LEntryId);
        List<LReflex> spelled = new(stored.Count);
        bool changed = false;
        foreach (LReflex row in stored)
        {
            string respelling = row.LReflexRespelling.Length > 0 || row.LReflexText.Length == 0
                ? row.LReflexRespelling
                : _lWorkspaceClerkLanguages.LLanguageRespellingResolve(
                    new LReflexDraft(row.LReflexLanguage, LReflexDraftText: row.LReflexText)).LReflexDraftRespelling;
            LAnatomy anatomy = row.LReflexText.Length == 0
                ? row.LReflexAnatomy
                : _lWorkspaceClerkLanguages.LLanguageAnatomyResolve(
                    entry.LEntryLanguage,
                    new LReflexDraft(
                        row.LReflexLanguage,
                        LReflexDraftText: row.LReflexText,
                        LReflexDraftRespelling: respelling)).LReflexDraftAnatomy;
            changed |= respelling.Length != row.LReflexRespelling.Length || anatomy != row.LReflexAnatomy;
            spelled.Add(row with { LReflexRespelling = respelling, LReflexAnatomy = anatomy });
        }

        if (changed)
        {
            reflexes.LReflexSet(entry.LEntryId, spelled);
        }
    }
}
