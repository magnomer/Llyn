using System;
using System.IO;

namespace Llyn.Infrastructure;

public static class LWorkspaceRoot
{
    private const string LWorkspaceRootFolder = "Llyn";
    private const string LWorkspaceRootPointer = "workspace.txt";
    private const string LWorkspaceRootDatabase = "llyn.db";
    private const string LWorkspaceRootDrafts = "drafts";
    private const string LWorkspaceRootCourt = "court";
    private const string LWorkspaceRootClaim = "claim";

    public static string LWorkspaceRootRead()
    {
        string? stored = LWorkspacePointerLoad();
        string root = string.IsNullOrWhiteSpace(stored) ? LWorkspaceDefaultRead() : stored;

        Directory.CreateDirectory(root);
        return root;
    }

    public static void LWorkspaceRootChange(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        Directory.CreateDirectory(path);
        LWorkspacePointerSave(path);
    }

    public static string LWorkspaceDatabaseRead(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        return Path.Combine(root, LWorkspaceRootDatabase);
    }

    public static string LWorkspaceDraftRead(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);

        string folder = Path.Combine(root, LWorkspaceRootDrafts);
        Directory.CreateDirectory(folder);
        return folder;
    }

    public static string LWorkspaceCourtRead(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);

        string folder = Path.Combine(LWorkspaceDraftRead(root), LWorkspaceRootCourt);
        Directory.CreateDirectory(folder);
        return folder;
    }

    public static string LWorkspaceClaimRead(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);

        string folder = Path.Combine(LWorkspaceDraftRead(root), LWorkspaceRootClaim);
        Directory.CreateDirectory(folder);
        return folder;
    }

    private static string LWorkspaceDefaultRead()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            LWorkspaceRootFolder);
    }

    private static string LWorkspacePointerRead()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            LWorkspaceRootFolder,
            LWorkspaceRootPointer);
    }

    private static string? LWorkspacePointerLoad()
    {
        string pointer = LWorkspacePointerRead();
        try
        {
            return File.Exists(pointer) ? File.ReadAllText(pointer).Trim() : null;
        }
        catch (IOException)
        {
            return null;
        }
    }

    private static void LWorkspacePointerSave(string path)
    {
        string pointer = LWorkspacePointerRead();
        Directory.CreateDirectory(Path.GetDirectoryName(pointer)!);
        File.WriteAllText(pointer, path);
    }
}
