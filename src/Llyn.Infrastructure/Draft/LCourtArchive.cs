using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LCourtArchive
{
    private const string LCourtArchiveExtension = ".json";
    private const string LCourtArchivePending = ".json.tmp";

    public const int LCourtArchiveVersion = 1;

    private static readonly TimeSpan LCourtArchiveStale = TimeSpan.FromHours(1);

    private static readonly JsonSerializerOptions LCourtArchiveIndent = new() { WriteIndented = true };

    public static void LCourtArchiveSave(string root, LCourt link)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        ArgumentNullException.ThrowIfNull(link);
        ArgumentOutOfRangeException.ThrowIfZero(link.LCourtId);

        string folder = LWorkspaceRoot.LWorkspaceCourtRead(root);
        string pending = Path.Combine(folder, link.LCourtId.ToString(CultureInfo.InvariantCulture) + LCourtArchivePending);
        string path = Path.Combine(folder, link.LCourtId.ToString(CultureInfo.InvariantCulture) + LCourtArchiveExtension);

        File.WriteAllText(
            pending,
            JsonSerializer.Serialize(link with { LCourtVersion = LCourtArchiveVersion }, LCourtArchiveIndent));
        File.Move(pending, path, true);
    }

    public static LCourt? LCourtArchiveRead(string root, long id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        ArgumentOutOfRangeException.ThrowIfZero(id);

        string path = Path.Combine(LWorkspaceRoot.LWorkspaceCourtRead(root), id.ToString(CultureInfo.InvariantCulture) + LCourtArchiveExtension);
        return LCourtArchiveLoad(path);
    }

    public static IReadOnlyList<LCourt> LCourtArchiveScan(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);

        string folder = LWorkspaceRoot.LWorkspaceCourtRead(root);

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

    public static void LCourtArchiveDelete(string root, long id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        ArgumentOutOfRangeException.ThrowIfZero(id);

        string path = Path.Combine(LWorkspaceRoot.LWorkspaceCourtRead(root), id.ToString(CultureInfo.InvariantCulture) + LCourtArchiveExtension);
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

    public static IReadOnlyList<LCourt> LCourtArchiveSettle(string root, long draftId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        ArgumentOutOfRangeException.ThrowIfZero(draftId);

        List<LCourt> settled = [];
        foreach (LCourt link in LCourtArchiveScan(root))
        {
            if (link.LCourtTargetId != draftId)
            {
                continue;
            }

            LCourtArchiveDelete(root, link.LCourtId);
            settled.Add(link);
        }

        return settled;
    }

    public static void LCourtArchiveSweep(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);

        string folder = LWorkspaceRoot.LWorkspaceCourtRead(root);

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
