using System;
using System.IO;
using System.Net.Http;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LRigFactory
{
    private const long LRigFactoryCeiling = 8L * 1024 * 1024;

    public static LRig LRigFactoryBuild(string workspace, HttpClient client, LUsher usher, LPress press)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspace);
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(usher);
        ArgumentNullException.ThrowIfNull(press);
        if (!Path.IsPathFullyQualified(workspace))
        {
            throw new ArgumentException("The workspace path must be fully qualified.", nameof(workspace));
        }

        string root = Path.GetFullPath(workspace);
        Directory.CreateDirectory(root);

        LDatabase database = new(root);

        return new LRig(
            database,
            new LDoctor(database),
            new LRealmArchive(database),
            new LSettingsLoader(root),
            new LAuditWriter(root),
            new LPostureFile(new LKeepFile(root)),
            new LEntryArchive(database),
            new LDraftArchive(root),
            new LClaimArchive(root),
            new LCourtArchive(root),
            new LRevisionArchive(database),
            new LWorkspaceArchive(database),
            new LTombstoneArchive(database),
            new LAuthorArchive(database),
            new LCollocationArchive(database),
            new LDiweiArchive(database),
            new LExampleArchive(database),
            new LFanqieArchive(database),
            new LFavoriteArchive(database),
            new LFrequencyArchive(database),
            new LGlossArchive(database),
            new LImageArchive(database),
            new LInflectionArchive(database),
            new LLacunaArchive(database),
            new LMeaningArchive(database),
            new LMentionArchive(database),
            new LMorphologyArchive(database),
            new LNoteArchive(database),
            new LPronunciationArchive(database),
            new LReferenceArchive(database),
            new LReflexArchive(database),
            new LRegisterArchive(database),
            new LScriptArchive(database),
            new LSentenceArchive(database),
            new LSituationArchive(database),
            new LSpeechArchive(database),
            new LTagArchive(database),
            new LTranscriptionArchive(database),
            new LTranslationArchive(database),
            new LVideoArchive(database),
            new LSourceFactoryHttp(client),
            new LFanqieSourceHttp(client),
            new LReflexSourceHttp(client),
            new LScriptSourceHttp(client),
            new LRecordingArchive(root, client),
            new LLanguageLoader(root, client),
            new LLocalizationLoader(),
            new LMarkupFile(),
            new LPortraitFile(LThemeLoader.LThemeLoaderLoad()),
            press,
            usher,
            new LTrailSystem(),
            new LClockSystem(),
            Environment.ProcessId,
            root);
    }

    public static HttpClient LRigClientCreate()
    {
        HttpClient client = new()
        {
            Timeout = TimeSpan.FromSeconds(10),
            MaxResponseContentBufferSize = LRigFactoryCeiling,
        };
        client.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Llyn/0.0 (pronunciation lookup)");
        return client;
    }
}
