using System;
using System.IO;
using System.Net.Http;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LRigFactory
{
    private const long LRigFactoryCeiling = 8L * 1024 * 1024;

    private static HttpClient? _lRigFactoryClient;

    public static LRig LRigFactoryBuild(string workspace, HttpClient? client)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspace);
        if (!Path.IsPathFullyQualified(workspace))
        {
            throw new ArgumentException("The workspace path must be fully qualified.", nameof(workspace));
        }

        string root = Path.GetFullPath(workspace);
        Directory.CreateDirectory(root);

        HttpClient shared = client ?? LRigClientCreate();
        LDatabase database = new(root);

        return new LRig(
            database,
            new LDoctor(database),
            new LRealmArchive(database),
            new LSettingsLoader(root),
            new LAuditWriter(root),
            new LKeepFile(root),
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
            new LSourceFactoryHttp(shared),
            new LFanqieSourceHttp(shared),
            new LReflexSourceHttp(shared),
            new LScriptSourceHttp(shared),
            new LRecordingArchive(root, shared),
            new LLanguageLoader(root, shared),
            new LLocalizationLoader(),
            new LMarkupFile(),
            new LPortraitFile(),
            new LUsherFile(),
            root);
    }

    private static HttpClient LRigClientCreate()
    {
        if (_lRigFactoryClient is not null)
        {
            return _lRigFactoryClient;
        }

        HttpClient client = new()
        {
            Timeout = TimeSpan.FromSeconds(10),
            MaxResponseContentBufferSize = LRigFactoryCeiling,
        };
        client.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Llyn/0.0 (pronunciation lookup)");
        _lRigFactoryClient = client;
        return client;
    }
}
