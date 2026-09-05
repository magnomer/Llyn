using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LDraftArchive
{
    private const string LDraftArchiveExtension = ".json";
    private const string LDraftArchivePending = ".json.tmp";

    private static readonly TimeSpan LDraftArchiveStale = TimeSpan.FromHours(1);

    private static readonly JsonSerializerOptions LDraftArchiveIndent = new() { WriteIndented = true };

    public static void LDraftArchiveSave(string root, LDraft draft)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentException.ThrowIfNullOrWhiteSpace(draft.LDraftId);

        string folder = LWorkspaceRoot.LWorkspaceDraftRead(root);
        string pending = Path.Combine(folder, draft.LDraftId + LDraftArchivePending);
        string path = Path.Combine(folder, draft.LDraftId + LDraftArchiveExtension);

        File.WriteAllText(pending, JsonSerializer.Serialize(draft, LDraftArchiveIndent));
        File.Move(pending, path, true);
    }

    public static LDraft? LDraftArchiveRead(string root, string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        string path = Path.Combine(LWorkspaceRoot.LWorkspaceDraftRead(root), id + LDraftArchiveExtension);
        return LDraftArchiveLoad(path);
    }

    public static IReadOnlyList<LDraft> LDraftArchiveScan(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);

        string folder = LWorkspaceRoot.LWorkspaceDraftRead(root);

        string[] files;
        try
        {
            files = Directory.GetFiles(folder, "*" + LDraftArchiveExtension);
        }
        catch (IOException)
        {
            return [];
        }
        catch (UnauthorizedAccessException)
        {
            return [];
        }

        List<LDraft> drafts = [];
        foreach (string file in files)
        {
            if (!file.EndsWith(LDraftArchiveExtension, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (LDraftArchiveLoad(file) is LDraft draft)
            {
                drafts.Add(draft);
            }
        }

        return drafts;
    }

    public static void LDraftArchiveDelete(string root, string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        string path = Path.Combine(LWorkspaceRoot.LWorkspaceDraftRead(root), id + LDraftArchiveExtension);
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

    public static void LDraftArchiveSweep(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);

        string folder = LWorkspaceRoot.LWorkspaceDraftRead(root);

        string[] files;
        try
        {
            files = Directory.GetFiles(folder, "*" + LDraftArchivePending);
        }
        catch (IOException)
        {
            return;
        }
        catch (UnauthorizedAccessException)
        {
            return;
        }

        DateTime edge = DateTime.UtcNow - LDraftArchiveStale;
        foreach (string file in files)
        {
            if (!file.EndsWith(LDraftArchivePending, StringComparison.OrdinalIgnoreCase))
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

    private static LDraft? LDraftArchiveLoad(string path)
    {
        try
        {
            return JsonSerializer.Deserialize<LDraft>(File.ReadAllText(path), LDraftArchiveIndent);
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
