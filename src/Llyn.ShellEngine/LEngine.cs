using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine : IDisposable
{
    private readonly object _lEngineGate = new();
    private const long LEngineClientCeiling = 8L * 1024 * 1024;

    private readonly HttpClient _lEngineClient;
    private readonly Dictionary<string, IReadOnlyList<LSource>> _lEngineLookupSources = new(StringComparer.Ordinal);
    private readonly Dictionary<string, IReadOnlyList<LSource>> _lEngineHarvestSources = new(StringComparer.Ordinal);
    private readonly Dictionary<(string LEngineLanguage, string LEngineScheme), IReadOnlyList<LSource>>
        _lEngineSchemeSources = [];
    private readonly Dictionary<string, IReadOnlyList<LSource>> _lEngineFrequencySources = new(StringComparer.Ordinal);
    private readonly Dictionary<long, CancellationTokenSource> _lEngineFrequencyPending = [];
    private readonly HashSet<long> _lEngineFrequencyMissed = [];
    private readonly Dictionary<string, IReadOnlyList<LSource>> _lEngineInflectionSources = new(StringComparer.Ordinal);
    private readonly Dictionary<long, CancellationTokenSource> _lEngineInflectionPending = [];
    private readonly Dictionary<long, HashSet<long>> _lEngineInflectionMissed = [];
    private readonly HashSet<long> _lEngineInflectionLost = [];
    private readonly Dictionary<string, LLanguage> _lEngineLanguages = new(StringComparer.Ordinal);
    private readonly Dictionary<string, LSpeechPack> _lEngineSpeechPacks = new(StringComparer.Ordinal);
    private readonly LTrove _lEngineTrove = new();
    private string _lEngineWorkspace;
    private LSettings _lEngineSettings;
    private LDatabase _lEngineDatabase;
    private LDoctorRescue _lEngineRescue;
    private LRealm _lEngineRealm;
    private LIdentity _lEngineIdentity;

    public LEngine()
        : this(LWorkspaceRoot.LWorkspaceRootRead())
    {
    }

    public LEngine(string workspace)
        : this(workspace, null)
    {
    }

    internal LEngine(string workspace, HttpClient? client)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspace);

        _lEngineWorkspace = workspace;
        _lEngineSettings = LSettingsLoader.LSettingsLoaderLoad(_lEngineWorkspace);

        _lEngineDatabase = new LDatabase(_lEngineWorkspace);
        _lEngineRescue = LDoctor.LDoctorDatabaseCreate(_lEngineDatabase);
        _lEngineRealm = new LRealmArchive(_lEngineDatabase).LRealmRead();
        _lEngineIdentity = new LIdentity(_lEngineDatabase);

        LEngineLanguageImport();
        LEngineDiweiApply();

        _lEngineClient = client ?? new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10),
            MaxResponseContentBufferSize = LEngineClientCeiling,
        };
        _lEngineClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Llyn/0.0 (pronunciation lookup)");
    }

    private long LEngineIdentityCreate()
    {
        lock (_lEngineGate)
        {
            return _lEngineIdentity.LIdentityCreate();
        }
    }

    public LRealm LEngineRealmRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineRealm;
        }
    }

    public static bool LEngineBusyCheck(Exception fault)
    {
        return LDoctor.LDoctorBusyCheck(fault);
    }

    public LDoctorRescue LEngineRescueRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineRescue;
        }
    }

    public string LEngineWorkspaceRead()
    {
        lock (_lEngineGate)
        {
            return _lEngineWorkspace;
        }
    }

    public string? LEngineAuditRecord(Exception exception)
    {
        lock (_lEngineGate)
        {
            return LAuditWriter.LAuditWriterRecord(_lEngineWorkspace, exception);
        }
    }

    public static string LEngineWorkspaceResolve()
    {
        return LWorkspaceRoot.LWorkspaceRootRead();
    }

    public void LEngineWorkspaceChange(string path)
    {
        LEngineWorkspaceOpen(path);
        LWorkspaceRoot.LWorkspaceRootChange(LEngineWorkspaceRead());
    }

    public void LEngineWorkspaceOpen(string path)
    {
        if (!Path.IsPathFullyQualified(path))
        {
            throw new ArgumentException("The workspace path must be fully qualified.", nameof(path));
        }

        string root = Path.GetFullPath(path);

        lock (_lEngineGate)
        {
            Directory.CreateDirectory(root);

            LDatabase database = new(root);
            LDoctorRescue rescue = LDoctor.LDoctorDatabaseCreate(database);
            LRealm realm = new LRealmArchive(database).LRealmRead();
            LIdentity identity = new(database);
            LSettings settings = LSettingsLoader.LSettingsLoaderExist(root)
                ? LSettingsLoader.LSettingsLoaderLoad(root)
                : _lEngineSettings;

            _lEngineWorkspace = root;

            foreach (long held in _lEngineDraftHeld)
            {
                _lEngineDraftStale.Add(held);
            }

            _lEngineDraftHeld.Clear();
            _lEngineLookupSources.Clear();
            _lEngineHarvestSources.Clear();
            _lEngineFrequencySources.Clear();
            LEngineFrequencyClear();
            _lEngineInflectionSources.Clear();
            LEngineInflectionClear();
            LEngineScriptClear();
            LEngineFanqieClear();
            LEngineReflexClear();
            _lEngineLanguages.Clear();
            _lEngineSpeechPacks.Clear();
            _lEngineTrove.LTroveClear();

            _lEngineSettings = settings;
            _lEngineDatabase = database;
            _lEngineRescue = rescue;
            _lEngineRealm = realm;
            _lEngineIdentity = identity;
            LEngineSettingsSave();
            LEngineLanguageImport();
            LEngineDiweiApply();
        }

        LEngineBulletinRaise(LSubject.LSubjectWorkspace, 0);
    }

    private static bool LEngineOwnerCheck(LOwner owner)
    {
        return owner switch
        {
            LOwner.LOwnerMeaning => false,
            LOwner.LOwnerCollocation => true,
            _ => throw LEngineOwnerRaise(owner),
        };
    }

    private static ArgumentOutOfRangeException LEngineOwnerRaise(LOwner owner)
    {
        return new ArgumentOutOfRangeException(
            nameof(owner), owner, "This entity has no reference from that kind of row.");
    }

    public void Dispose()
    {
        lock (_lEngineGate)
        {
            LEngineFrequencyClear();
            LEngineInflectionClear();
            LEngineScriptClear();
            LEngineFanqieClear();
            LEngineReflexClear();
            _lEngineClient.Dispose();
        }
    }
}
