using System;
using System.IO;
using System.Net.Http;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LRigFactory
{
    private const long LRigFactoryCeiling = 8L * 1024 * 1024;

    public static LRig LRigFactoryBuild(
        string workspace, HttpClient client, LUsher usher, LPress press, LWarrant warrant, LPhonograph phonograph)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspace);
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(usher);
        ArgumentNullException.ThrowIfNull(press);
        ArgumentNullException.ThrowIfNull(warrant);
        ArgumentNullException.ThrowIfNull(phonograph);
        if (!Path.IsPathFullyQualified(workspace))
        {
            throw new ArgumentException("The workspace path must be fully qualified.", nameof(workspace));
        }

        string root = Path.GetFullPath(workspace);
        Directory.CreateDirectory(root);

        LDatabase database = new(root);
        LTheme theme = LThemeLoader.LThemeLoaderLoad();

        return new LRig(
            database,
            LRigFactoryKeeping.LRigKeepingBuild(database),
            new LSettingsLoader(root),
            new LAuditWriter(root),
            new LPostureFile(new LKeepFile(root)),
            LRigFactoryAsset.LRigAssetBuild(root, theme),
            new LEntryArchive(database),
            new LEntryQueryArchive(database),
            LRigFactoryLexicon.LRigLexiconBuild(database),
            LRigFactoryDraft.LRigDraftBuild(root),
            LRigFactoryCitation.LRigCitationBuild(database),
            LRigFactorySound.LRigSoundBuild(database),
            LRigFactorySentence.LRigSentenceBuild(database),
            LRigFactoryContext.LRigContextBuild(database),
            LRigFactorySource.LRigSourceBuild(client, root),
            new LOutpostHttp(),
            new LLanguageLoader(root, client),
            press,
            warrant,
            usher,
            phonograph,
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
