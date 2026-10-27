using System;
using System.IO;
using System.Net.Http;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LRigFactory : IDisposable
{
    private const long LRigFactoryCeiling = 8L * 1024 * 1024;

    private readonly HttpClient _lRigFactoryClient;
    private readonly bool _lRigFactoryOwned;
    private readonly LUsher _lRigFactoryUsher;
    private readonly LPress _lRigFactoryPress;
    private readonly LPressQueue _lRigFactoryQueue;
    private readonly LWarrant _lRigFactoryWarrant;
    private readonly LPhonograph _lRigFactoryPhonograph;

    public LRigFactory(LUsher usher, LPress press, LPressQueue queue, LWarrant warrant, LPhonograph phonograph)
        : this(LRigClientCreate(), true, usher, press, queue, warrant, phonograph)
    {
    }

    public LRigFactory(
        HttpClient client, LUsher usher, LPress press, LPressQueue queue, LWarrant warrant, LPhonograph phonograph)
        : this(client, false, usher, press, queue, warrant, phonograph)
    {
    }

    private LRigFactory(
        HttpClient client,
        bool owned,
        LUsher usher,
        LPress press,
        LPressQueue queue,
        LWarrant warrant,
        LPhonograph phonograph)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(usher);
        ArgumentNullException.ThrowIfNull(press);
        ArgumentNullException.ThrowIfNull(queue);
        ArgumentNullException.ThrowIfNull(warrant);
        ArgumentNullException.ThrowIfNull(phonograph);
        _lRigFactoryClient = client;
        _lRigFactoryOwned = owned;
        _lRigFactoryUsher = usher;
        _lRigFactoryPress = press;
        _lRigFactoryQueue = queue;
        _lRigFactoryWarrant = warrant;
        _lRigFactoryPhonograph = phonograph;
    }

    public LRig LRigFactoryBuild(string workspace)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspace);
        if (!Path.IsPathFullyQualified(workspace))
        {
            throw new ArgumentException("The workspace path must be fully qualified.", nameof(workspace));
        }

        string root = Path.GetFullPath(workspace);
        Directory.CreateDirectory(root);

        LDatabase database = new(root);
        LTheme theme = LThemeLoader.LThemeLoaderLoad();
        LClockSystem clock = new();

        return new LRig(
            database,
            LRigFactoryKeeping.LRigKeepingBuild(database),
            new LSettingsLoader(root),
            new LAuditWriter(root),
            new LPostureFile(new LKeepFile(root)),
            LRigFactoryAsset.LRigAssetBuild(theme),
            new LEntryArchive(database),
            new LEntryQueryArchive(database),
            LRigFactoryLexicon.LRigLexiconBuild(database),
            new LDossierEntrance(root),
            LRigFactoryCitation.LRigCitationBuild(database),
            LRigFactorySound.LRigSoundBuild(database),
            LRigFactorySentence.LRigSentenceBuild(database),
            LRigFactoryContext.LRigContextBuild(database),
            LRigFactorySource.LRigSourceBuild(_lRigFactoryClient, root, clock),
            LRigFactoryRehearsal.LRigRehearsalBuild(database, root, clock),
            new LEmbassyEntrance(new LKeepFile(root), new LEngraverEntrance(theme)),
            new LLanguageLoader(root, _lRigFactoryClient),
            _lRigFactoryPress,
            _lRigFactoryQueue,
            new LPressFile(new LKeepFile(root)),
            _lRigFactoryWarrant,
            _lRigFactoryUsher,
            _lRigFactoryPhonograph,
            new LTrailSystem(),
            clock,
            new LLeaseFile(root),
            root);
    }

    public void Dispose()
    {
        if (_lRigFactoryOwned)
        {
            _lRigFactoryClient.Dispose();
        }
    }

    private static HttpClient LRigClientCreate()
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
