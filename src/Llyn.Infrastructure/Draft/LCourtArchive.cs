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

    private static readonly TimeSpan LCourtArchiveStale = TimeSpan.FromHours(1);

    private static readonly JsonSerializerOptions LCourtArchiveIndent = new() { WriteIndented = true };

    public static void LCourtArchiveSave(string root, LCourtLink link)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        ArgumentNullException.ThrowIfNull(link);
        ArgumentOutOfRangeException.ThrowIfZero(link.LCourtLinkId);

        string folder = LWorkspaceRoot.LWorkspaceCourtRead(root);
        string pending = Path.Combine(folder, link.LCourtLinkId.ToString(CultureInfo.InvariantCulture) + LCourtArchivePending);
        string path = Path.Combine(folder, link.LCourtLinkId.ToString(CultureInfo.InvariantCulture) + LCourtArchiveExtension);

        File.WriteAllText(pending, JsonSerializer.Serialize(link, LCourtArchiveIndent));
        File.Move(pending, path, true);
    }

    public static LCourtLink? LCourtArchiveRead(string root, long id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        ArgumentOutOfRangeException.ThrowIfZero(id);

        string path = Path.Combine(LWorkspaceRoot.LWorkspaceCourtRead(root), id.ToString(CultureInfo.InvariantCulture) + LCourtArchiveExtension);
        return LCourtArchiveLoad(path);
    }

    public static IReadOnlyList<LCourtLink> LCourtArchiveScan(string root)
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

        List<LCourtLink> links = [];
        foreach (string file in files)
        {
            if (!file.EndsWith(LCourtArchiveExtension, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (LCourtArchiveLoad(file) is LCourtLink link)
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

    public static IReadOnlyList<LCourtLink> LCourtArchiveSettle(string root, long draftId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        ArgumentOutOfRangeException.ThrowIfZero(draftId);

        List<LCourtLink> settled = [];
        foreach (LCourtLink link in LCourtArchiveScan(root))
        {
            if (link.LCourtLinkTarget != draftId)
            {
                continue;
            }

            LCourtArchiveDelete(root, link.LCourtLinkId);
            settled.Add(link);
        }

        return settled;
    }

    public static void LCourtArchiveSweep(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);

        string folder = LWorkspaceRoot.LWorkspaceCourtRead(root);

        string[] files;
        try
        {
            files = Directory.GetFiles(folder, "*" + LCourtArchivePending);
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
        foreach (string file in files)
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
    }

    private static LCourtLink? LCourtArchiveLoad(string path)
    {
        try
        {
            return JsonSerializer.Deserialize<LCourtLink>(File.ReadAllText(path), LCourtArchiveIndent);
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
