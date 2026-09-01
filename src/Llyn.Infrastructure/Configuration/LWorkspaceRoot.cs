using System;
using System.IO;

namespace Llyn.Infrastructure;

public static class LWorkspaceRoot
{
    private const string LWorkspaceRootFolder = "Llyn";
    private const string LWorkspaceRootPointer = "workspace.txt";
    private const string LWorkspaceRootDatabase = "llyn.db";

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
