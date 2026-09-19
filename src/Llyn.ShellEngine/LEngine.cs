using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Threading;
using Llyn.Application;
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
    private readonly Dictionary<string, LLanguage> _lEngineLanguages = new(StringComparer.Ordinal);
    private IReadOnlyList<string>? _lEngineLanguageListed;
    private readonly Dictionary<string, LSpeechPack> _lEngineSpeechPacks = new(StringComparer.Ordinal);
    private readonly LTrove _lEngineTrove = new();
    private readonly LUsher _lEngineUsher = new LUsherFile();
    private readonly LEnsign _lEngineEnsign;
    private string _lEngineWorkspace;
    private LSettings _lEngineSettings;
    private LDatabase _lEngineDatabase;
    private LVault _lEngineVault;
    private LEntryVault _lEngineEntries;
    private LDraftVault _lEngineDrafts;
    private LRevisionVault _lEngineRevisions;
    private LWorkspaceVault _lEngineWorkspaces;
    private LTombstoneVault _lEngineTombstones;
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
        : this(workspace, client, null)
    {
    }

    internal LEngine(string workspace, HttpClient? client, LEntryVault? entries)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workspace);

        _lEngineWorkspace = workspace;
        _lEngineEnsign = new LEnsign(_lEngineUsher);
        _lEngineSettings = LSettingsLoader.LSettingsLoaderLoad(_lEngineWorkspace);

        _lEngineDatabase = new LDatabase(_lEngineWorkspace);
        _lEngineRescue = LDoctor.LDoctorDatabaseCreate(_lEngineDatabase);
        _lEngineRealm = new LRealmArchive(_lEngineDatabase).LRealmRead();
        LEngineVaultSet();
        if (entries is not null)
        {
            _lEngineEntries = entries;
        }

        _lEngineIdentity = new LIdentity(_lEngineWorkspaces);

        LEngineLanguageImport();
        LEngineDiweiApply();
        if (_lEngineDatabase.LDatabaseMigrated)
        {
            LEngineWorkspaceUpdate();
        }

        _lEngineClient = client ?? new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10),
            MaxResponseContentBufferSize = LEngineClientCeiling,
        };
        _lEngineClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) Llyn/0.0 (pronunciation lookup)");
    }

    [MemberNotNull(
        nameof(_lEngineVault),
        nameof(_lEngineEntries),
        nameof(_lEngineDrafts),
        nameof(_lEngineRevisions),
        nameof(_lEngineWorkspaces),
        nameof(_lEngineTombstones))]
    private void LEngineVaultSet()
    {
        _lEngineVault = _lEngineDatabase;
        _lEngineEntries = new LEntryArchive(_lEngineDatabase);
        _lEngineDrafts = new LDraftArchive(_lEngineWorkspace);
        _lEngineRevisions = new LRevisionArchive(_lEngineDatabase);
        _lEngineWorkspaces = new LWorkspaceArchive(_lEngineDatabase);
        _lEngineTombstones = new LTombstoneArchive(_lEngineDatabase);
    }

    private long LEngineIdentityCreate()
    {
        lock (_lEngineGate)
        {
            return _lEngineIdentity.LIdentityCreate();
        }
    }

    internal LRealm LEngineRealmRead()
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

    public string LEngineWorkspaceFormat()
    {
        string root = LEngineWorkspaceRead();
        string name = Path.GetFileName(Path.TrimEndingDirectorySeparator(root));
        return name.Length > 0 ? name : root;
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

    internal void LEngineWorkspaceOpen(string path)
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
            _lEngineLanguageListed = null;
            _lEngineEnsign.LEnsignClear();
            _lEngineSpeechPacks.Clear();
            _lEngineTrove.LTroveClear();

            _lEngineSettings = settings;
            _lEngineDatabase = database;
            _lEngineRescue = rescue;
            _lEngineRealm = realm;
            LEngineVaultSet();
            _lEngineIdentity = new LIdentity(_lEngineWorkspaces);
            LEngineSettingsSave();
            LEngineLanguageImport();
            LEngineDiweiApply();
            if (database.LDatabaseMigrated)
            {
                LEngineWorkspaceUpdate();
            }
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
