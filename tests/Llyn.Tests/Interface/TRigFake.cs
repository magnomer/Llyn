using System.Reflection;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.Tests;

internal static class TRigFake
{
    internal static LRig TRigFakeBuild() => TRigFakeBuild(new TVaultFake(), "fake");

    internal static LRig TRigFakeBuild(LEntryVault entries) => TRigFakeBuild(entries, "fake");

    internal static LRig TRigFakeBuild(LEntryVault entries, string workspace)
    {
        return new LRig(
            new TRigFakeVault(),
            new TRigFakeDoctor(),
            new TRigFakeRealm(),
            new TRigFakeSettings(),
            TRigStubCreate<LAuditVault>(),
            new TPostureFake(),
            entries,
            TRigStubCreate<LDraftVault>(),
            TRigStubCreate<LClaimVault>(),
            TRigStubCreate<LCourtVault>(),
            TRigStubCreate<LRevisionVault>(),
            TRigStubCreate<LWorkspaceVault>(),
            TRigStubCreate<LTombstoneVault>(),
            TRigStubCreate<LAuthorVault>(),
            TRigStubCreate<LCollocationVault>(),
            TRigStubCreate<LDiweiVault>(),
            TRigStubCreate<LExampleVault>(),
            TRigStubCreate<LFanqieVault>(),
            TRigStubCreate<LFavoriteVault>(),
            TRigStubCreate<LFrequencyVault>(),
            TRigStubCreate<LGlossVault>(),
            TRigStubCreate<LImageVault>(),
            TRigStubCreate<LInflectionVault>(),
            TRigStubCreate<LLacunaVault>(),
            TRigStubCreate<LMeaningVault>(),
            TRigStubCreate<LMentionVault>(),
            TRigStubCreate<LMorphologyVault>(),
            TRigStubCreate<LNoteVault>(),
            TRigStubCreate<LPronunciationVault>(),
            TRigStubCreate<LReferenceVault>(),
            TRigStubCreate<LReflexVault>(),
            TRigStubCreate<LRegisterVault>(),
            TRigStubCreate<LScriptVault>(),
            TRigStubCreate<LSentenceVault>(),
            TRigStubCreate<LSituationVault>(),
            TRigStubCreate<LSpeechVault>(),
            TRigStubCreate<LTagVault>(),
            TRigStubCreate<LTranscriptionVault>(),
            TRigStubCreate<LTranslationVault>(),
            TRigStubCreate<LVideoVault>(),
            TRigStubCreate<LSourceFactory>(),
            TRigStubCreate<LFanqieSource>(),
            TRigStubCreate<LReflexSource>(),
            TRigStubCreate<LScriptSource>(),
            TRigStubCreate<LRecordingVault>(),
            new TRigFakeLanguages(),
            TRigStubCreate<LLocalizationVault>(),
            TRigStubCreate<LMarkupVault>(),
            TRigStubCreate<LPortraitVault>(),
            TRigStubCreate<LUsher>(),
            new LTrailSystem(),
            new TClockFake(),
            1,
            workspace);
    }

    private static TRigFakePort TRigStubCreate<TRigFakePort>() where TRigFakePort : class =>
        DispatchProxy.Create<TRigFakePort, TRigFakeProxy>();

    public class TRigFakeProxy : DispatchProxy
    {
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) =>
            throw new NotSupportedException($"The fake rig answers no {targetMethod?.Name}.");
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

    private sealed class TRigFakeDoctor : LDoctorVault
    {
        public LDoctorRescue LDoctorDatabaseCreate() => LDoctorRescue.LDoctorRescueHealthy;
    }

    private sealed class TRigFakeRealm : LRealmVault
    {
        public LRealm LRealmRead() => new(Guid.Empty);
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
    }
}
