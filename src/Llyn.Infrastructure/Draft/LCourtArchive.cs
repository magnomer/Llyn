using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LCourtArchive : LCourtVault
{
    private readonly string _lCourtArchiveRoot;

    private const string LCourtArchiveExtension = ".json";
    private const string LCourtArchivePending = ".json.tmp";

    public const int LCourtArchiveVersion = 1;

    private static readonly TimeSpan LCourtArchiveStale = TimeSpan.FromHours(1);

    private static readonly JsonSerializerOptions LCourtArchiveIndent = new() { WriteIndented = true };

    public LCourtArchive(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        _lCourtArchiveRoot = root;
    }

    public void LCourtSave(LCourt link)
    {
        ArgumentNullException.ThrowIfNull(link);
        ArgumentOutOfRangeException.ThrowIfZero(link.LCourtId);

        string folder = LWorkspaceRoot.LWorkspaceCourtRead(_lCourtArchiveRoot);
        string stem = link.LCourtId.ToString(CultureInfo.InvariantCulture);
        string pending = Path.Combine(folder, stem + LCourtArchivePending);
        string path = Path.Combine(folder, stem + LCourtArchiveExtension);

        File.WriteAllText(
            pending,
            JsonSerializer.Serialize(link with { LCourtVersion = LCourtArchiveVersion }, LCourtArchiveIndent));
        LWorkspaceRoot.LWorkspacePendingCommit(pending, path);
    }

    public IReadOnlyList<LCourt> LCourtScan()
    {
        string folder = LWorkspaceRoot.LWorkspaceCourtRead(_lCourtArchiveRoot);

        string[] files;
        try
        {
            files = Directory.GetFiles(folder, "*" + LCourtArchiveExtension);
        }
        catch (IOException)
        {
            return [];
        }
        catch (UnauthorizedAccessException)
        {
            return [];
        }

        List<LCourt> links = [];
        foreach (string file in files)
        {
            if (!file.EndsWith(LCourtArchiveExtension, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (LCourtArchiveLoad(file) is LCourt link)
            {
                links.Add(link);
            }
        }

        return links;
    }

    public void LCourtDelete(long id)
    {
        ArgumentOutOfRangeException.ThrowIfZero(id);

        string path = Path.Combine(
            LWorkspaceRoot.LWorkspaceCourtRead(_lCourtArchiveRoot),
            id.ToString(CultureInfo.InvariantCulture) + LCourtArchiveExtension);
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

    public IReadOnlyList<LCourt> LCourtSettle(long draftId)
    {
        ArgumentOutOfRangeException.ThrowIfZero(draftId);

        List<LCourt> settled = [];
        foreach (LCourt link in LCourtScan())
        {
            if (link.LCourtTargetId != draftId)
            {
                continue;
            }

            LCourtDelete(link.LCourtId);
            settled.Add(link);
        }

        return settled;
    }

    public void LCourtSweep()
    {
        string folder = LWorkspaceRoot.LWorkspaceCourtRead(_lCourtArchiveRoot);

        string[] pending;
        string[] files;
        try
        {
            pending = Directory.GetFiles(folder, "*" + LCourtArchivePending);
            files = Directory.GetFiles(folder, "*" + LCourtArchiveExtension);
        }
        catch (IOException)
        {
            return;
        }
        catch (UnauthorizedAccessException)
        {
            return;
        }

        DateTime edge = DateTime.UtcNow - LCourtArchiveStale;
        foreach (string file in pending)
        {
            if (!file.EndsWith(LCourtArchivePending, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            try
            {
                if (File.GetLastWriteTimeUtc(file) > edge)
                {
                    continue;
                }

                File.Delete(file);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        foreach (string file in files)
        {
            if (!file.EndsWith(LCourtArchiveExtension, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            LCourt? link = LCourtArchiveLoad(file, false);
            if (link is not null && link.LCourtVersion == LCourtArchiveVersion)
            {
                continue;
            }

            try
            {
                File.Delete(file);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }

    private static LCourt? LCourtArchiveLoad(string path, bool checking = true)
    {
        try
        {
            LCourt? link = JsonSerializer.Deserialize<LCourt>(File.ReadAllText(path), LCourtArchiveIndent);
            return checking && link is not null && link.LCourtVersion != LCourtArchiveVersion
                ? null
                : link;
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
