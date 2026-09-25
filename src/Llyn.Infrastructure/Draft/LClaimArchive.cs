using System;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LClaimArchive : LClaimVault
{
    private readonly string _lClaimArchiveRoot;

    private const string LClaimArchiveExtension = ".json";
    private const string LClaimArchivePending = ".json.tmp";

    private static readonly TimeSpan LClaimArchiveDrift = TimeSpan.FromSeconds(1);

    private static readonly JsonSerializerOptions LClaimArchiveIndent = new() { WriteIndented = true };

    public LClaimArchive(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        _lClaimArchiveRoot = root;
    }

    public LClaim LClaimCreate(long draftId)
    {
        ArgumentOutOfRangeException.ThrowIfZero(draftId);

        using Process running = Process.GetCurrentProcess();
        return new LClaim(draftId, running.Id, new DateTimeOffset(running.StartTime.ToUniversalTime()));
    }

    public void LClaimSave(LClaim claim)
    {
        ArgumentNullException.ThrowIfNull(claim);
        ArgumentOutOfRangeException.ThrowIfZero(claim.LClaimDraft);

        string folder = LWorkspaceRoot.LWorkspaceClaimRead(_lClaimArchiveRoot);
        string stem = claim.LClaimDraft.ToString(CultureInfo.InvariantCulture);
        string pending = Path.Combine(folder, stem + LClaimArchivePending);
        string path = Path.Combine(folder, stem + LClaimArchiveExtension);

        File.WriteAllText(pending, JsonSerializer.Serialize(claim, LClaimArchiveIndent));
        LWorkspaceRoot.LWorkspacePendingCommit(pending, path);
    }

    public LClaim? LClaimRead(long draftId)
    {
        ArgumentOutOfRangeException.ThrowIfZero(draftId);

        string path = Path.Combine(
            LWorkspaceRoot.LWorkspaceClaimRead(_lClaimArchiveRoot), draftId + LClaimArchiveExtension);
        return LClaimArchiveLoad(path);
    }

    public IReadOnlyList<LClaim> LClaimScan()
    {
        string folder = LWorkspaceRoot.LWorkspaceClaimRead(_lClaimArchiveRoot);

        string[] files;
        try
        {
            files = Directory.GetFiles(folder, "*" + LClaimArchiveExtension);
        }
        catch (IOException)
        {
            return [];
        }
        catch (UnauthorizedAccessException)
        {
            return [];
        }

        List<LClaim> claims = [];
        foreach (string file in files)
        {
            if (!file.EndsWith(LClaimArchiveExtension, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (LClaimArchiveLoad(file) is LClaim claim)
            {
                claims.Add(claim);
            }
        }

        return claims;
    }

    public void LClaimDelete(long draftId)
    {
        ArgumentOutOfRangeException.ThrowIfZero(draftId);

        string path = Path.Combine(
            LWorkspaceRoot.LWorkspaceClaimRead(_lClaimArchiveRoot), draftId + LClaimArchiveExtension);
        try
        {
            File.Delete(path);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    public bool LClaimCheck(long draftId)
    {
        ArgumentOutOfRangeException.ThrowIfZero(draftId);

        LClaim? claim = LClaimRead(draftId);
        if (claim is null)
        {
            return false;
        }

        if (LClaimArchiveMatch(claim))
        {
            return true;
        }

        LClaimDelete(draftId);
        return false;
    }

    private static bool LClaimArchiveMatch(LClaim claim)
    {
        try
        {
            using Process held = Process.GetProcessById(claim.LClaimProcess);
            if (held.HasExited)
            {
                return false;
            }

            DateTimeOffset started = new(held.StartTime.ToUniversalTime());
            return (started - claim.LClaimMoment).Duration() <= LClaimArchiveDrift;
        }
        catch (ArgumentException)
        {
            return false;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (Win32Exception)
        {
            return false;
        }
    }

    private static LClaim? LClaimArchiveLoad(string path)
    {
        try
        {
            return JsonSerializer.Deserialize<LClaim>(File.ReadAllText(path), LClaimArchiveIndent);
        }
        catch (JsonException)
        {
            return null;
        }
        catch (NotSupportedException)
        {
            return null;
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }
}
