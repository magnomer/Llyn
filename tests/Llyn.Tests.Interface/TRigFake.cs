using System.Reflection;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;

namespace Llyn.Tests;

internal static class TRigFake
{
    internal static LEngine TRigFakeStart(LRig rig) =>
        new(rig, workspace => TRigFakeBuild(new TVaultFake(), workspace), _ => { });

    internal static LRig TRigFakeBuild() => TRigFakeBuild(new TVaultFake(), "fake");

    internal static LRig TRigFakeBuild(TVaultFake entries) => TRigFakeBuild(entries, "fake");

    internal static LRig TRigFakeBuild(TVaultFake entries, string workspace) =>
        TRigFakeBuild(entries, workspace, new TRigFakeLanguages(), TRigStubCreate<LUsher>());

    internal static LRig TRigFakeBuild(LLanguageVault languages, LUsher usher) =>
        TRigFakeBuild(new TVaultFake(), "fake", languages, usher);

    private static LRig TRigFakeBuild(TVaultFake entries, string workspace, LLanguageVault languages, LUsher usher)
    {
        return new LRig(
            new TRigFakeVault(),
            new LRigKeeping(
                new TRigFakeDoctor(),
                TRigStubCreate<LRevisionVault>(),
                TRigStubCreate<LWorkspaceVault>(),
                TRigStubCreate<LTombstoneVault>(),
                TRigStubCreate<LFavoriteVault>(),
                TRigStubCreate<LNoteVault>()),
            new TRigFakeSettings(),
            new TRigFakeAudit(),
            new TPostureFake(),
            new LRigAsset(
                TRigStubCreate<LManifestVault>(),
                TRigStubCreate<LLocalizationVault>(),
                TRigStubCreate<LMarkupVault>(),
                TRigStubCreate<LPortraitVault>(),
                TRigStubCreate<LLivery>()),
            entries,
            entries,
            new LRigLexicon(
                TRigStubCreate<LEtymologyVault>(),
                TRigStubCreate<LCollocationVault>(),
                TRigStubCreate<LGlossVault>(),
                TRigStubCreate<LInflectionVault>(),
                TRigStubCreate<LLacunaVault>(),
                TRigStubCreate<LMeaningVault>(),
                TRigStubCreate<LMorphologyVault>(),
                TRigStubCreate<LSpeechVault>()),
            new LRigDraft(
                TRigStubCreate<LDraftVault>(),
                TRigStubCreate<LClaimVault>(),
                TRigStubCreate<LCourtVault>()),
            new LRigCitation(
                TRigStubCreate<LAuthorVault>(),
                TRigStubCreate<LImageVault>(),
                TRigStubCreate<LReferenceVault>(),
                TRigStubCreate<LVideoVault>()),
            new LRigSound(
                TRigStubCreate<LDiweiVault>(),
                TRigStubCreate<LFanqieVault>(),
                TRigStubCreate<LShengfuVault>(),
                TRigStubCreate<LStemVault>(),
                TRigStubCreate<LFrequencyVault>(),
                TRigStubCreate<LPronunciationVault>(),
                TRigStubCreate<LReflexVault>(),
                TRigStubCreate<LScriptVault>(),
                TRigStubCreate<LTranscriptionVault>()),
            new LRigSentence(
                TRigStubCreate<LExampleVault>(),
                TRigStubCreate<LMentionVault>(),
                TRigStubCreate<LSentenceVault>(),
                TRigStubCreate<LTranslationVault>()),
            new LRigContext(
                TRigStubCreate<LRegisterVault>(),
                TRigStubCreate<LSituationVault>(),
                TRigStubCreate<LTagVault>()),
            new LRigSource(
                TRigStubCreate<LSourceFactory>(),
                TRigStubCreate<LFanqieSource>(),
                TRigStubCreate<LShengfuSource>(),
                TRigStubCreate<LReflexSource>(),
                TRigStubCreate<LScriptSource>(),
                TRigStubCreate<LRecordingVault>()),
            TRigStubCreate<LOutpost>(),
            languages,
            new TPress(),
            new TWarrantFake(),
            usher,
            new TPhonographFake(),
            new LTrailSystem(),
            new TClockFake(),
            1,
            workspace);
    }

    internal static LSourceFactory TRigSourceCreate() => TRigStubCreate<LSourceFactory>();

    internal static string TRigFaultRead(string? method) => $"The fake rig answers no {method}.";

    private static TRigFakePort TRigStubCreate<TRigFakePort>() where TRigFakePort : class =>
        DispatchProxy.Create<TRigFakePort, TRigFakeProxy>();

    public class TRigFakeProxy : DispatchProxy
    {
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) =>
            throw new NotSupportedException(TRigFaultRead(targetMethod?.Name));
    }

    private sealed class TRigFakeVault : LVault
    {
        public LVaultSession LVaultSessionStart() => new TRigFakeSession();

        public bool LVaultMigrated => false;
    }

    private sealed class TRigFakeSession : LVaultSession, IDisposable
    {
        public void LVaultSessionCommit()
        {
        }

        public void Dispose()
        {
        }
    }

    private sealed class TRigFakeAudit : LAuditVault
    {
        public string? LAuditRecord(Exception exception) => null;
    }

    private sealed class TRigFakeDoctor : LDoctorVault
    {
        public LDoctorRescue LDoctorDatabaseCreate() => LDoctorRescue.LDoctorRescueHealthy;
    }

    private sealed class TRigFakeSettings : LSettingsVault
    {
        private LSettings? _tRigFakeSaved;

        public bool LSettingsExist() => _tRigFakeSaved is not null;

        public LSettings LSettingsRead() => _tRigFakeSaved ?? TInterface.TSettingsCreate("en");

        public void LSettingsSave(LSettings settings)
        {
            _tRigFakeSaved = settings;
        }
    }

    private sealed class TRigFakeLanguages : LLanguageVault
    {
        public IReadOnlyList<string> LLanguageScan() => [];

        public LLanguage LLanguageRead(string language) => throw new NotSupportedException();

        public bool LLanguageNameValidate(string? language) => false;

        public Task<string?> LLanguageFlagRead(string code, CancellationToken cancellation) =>
            Task.FromResult<string?>(null);

        public string? LLanguageFlagFind(string code) => null;
    }
}
