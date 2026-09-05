using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LCourtArchive
{
    private const string LCourtArchiveExtension = ".json";
    private const string LCourtArchivePending = ".json.tmp";

    private static readonly JsonSerializerOptions LCourtArchiveIndent = new() { WriteIndented = true };

    public static void LCourtArchiveSave(string root, LCourtLink link)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        ArgumentNullException.ThrowIfNull(link);
        ArgumentException.ThrowIfNullOrWhiteSpace(link.LCourtLinkId);

        string folder = LWorkspaceRoot.LWorkspaceCourtRead(root);
        string pending = Path.Combine(folder, link.LCourtLinkId + LCourtArchivePending);
        string path = Path.Combine(folder, link.LCourtLinkId + LCourtArchiveExtension);

        File.WriteAllText(pending, JsonSerializer.Serialize(link, LCourtArchiveIndent));
        File.Move(pending, path, true);
    }

    public static LCourtLink? LCourtArchiveRead(string root, string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        string path = Path.Combine(LWorkspaceRoot.LWorkspaceCourtRead(root), id + LCourtArchiveExtension);
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

    public static void LCourtArchiveDelete(string root, string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        string path = Path.Combine(LWorkspaceRoot.LWorkspaceCourtRead(root), id + LCourtArchiveExtension);
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

    public static IReadOnlyList<LCourtLink> LCourtArchiveResolve(string root, string draftId, string realId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(realId);
        return LCourtArchiveRemove(root, draftId);
    }

    public static IReadOnlyList<LCourtLink> LCourtArchiveCancel(string root, string draftId)
    {
        return LCourtArchiveRemove(root, draftId);
    }

    private static IReadOnlyList<LCourtLink> LCourtArchiveRemove(string root, string draftId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        ArgumentException.ThrowIfNullOrWhiteSpace(draftId);

        List<LCourtLink> settled = [];
        foreach (LCourtLink link in LCourtArchiveScan(root))
        {
            if (!string.Equals(link.LCourtLinkTarget, draftId, StringComparison.Ordinal))
            {
                continue;
            }

            LCourtArchiveDelete(root, link.LCourtLinkId);
            settled.Add(link);
        }

        return settled;
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
