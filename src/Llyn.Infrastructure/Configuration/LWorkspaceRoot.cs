using System;
using System.IO;

namespace Llyn.Infrastructure;

/// <summary>
/// Resolves and persists the user's workspace folder — the single location that owns the user's
/// settings and database. The chosen folder path is the one piece of state that cannot itself live
/// in the workspace (it is what tells the program where the workspace is), so it is kept in a small
/// pointer file under the user's application-data folder. Everything else the program persists goes
/// inside the resolved workspace, never beside this pointer.
/// </summary>
public static class LWorkspaceRoot
{
    private const string LWorkspaceRootFolder = "Llyn";
    private const string LWorkspaceRootPointer = "workspace.txt";
    private const string LWorkspaceRootDatabase = "llyn.db";

    /// <summary>
    /// Returns the current workspace folder, creating it if needed. When no folder has been chosen
    /// yet, a default under the user profile is used and recorded so later runs are stable.
    /// </summary>
    public static string LWorkspaceRootRead()
    {
        string? stored = LWorkspacePointerLoad();
        string root = string.IsNullOrWhiteSpace(stored) ? LWorkspaceDefaultRead() : stored;

        Directory.CreateDirectory(root);
        return root;
    }

    /// <summary>
    /// Records <paramref name="path"/> as the workspace folder and creates it. Subsequent settings
    /// and database access resolve against this folder.
    /// </summary>
    public static void LWorkspaceRootChange(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        Directory.CreateDirectory(path);
        LWorkspacePointerSave(path);
    }

    /// <summary>The database file path within <paramref name="root"/>. The database lives only here.</summary>
    public static string LWorkspaceDatabaseRead(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        return Path.Combine(root, LWorkspaceRootDatabase);
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
