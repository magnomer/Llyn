using System;
using System.IO;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LKeepFile : LKeep
{
    private const string LKeepFileExtension = ".json";
    private const string LKeepFilePending = ".json.tmp";

    private readonly string _lKeepFileRoot;

    public LKeepFile(string root)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        _lKeepFileRoot = root;
    }

    public string? LKeepRead(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        string path = Path.Combine(_lKeepFileRoot, name + LKeepFileExtension);
        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            return File.ReadAllText(path);
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

    public void LKeepSave(string name, string text)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(text);

        Directory.CreateDirectory(_lKeepFileRoot);

        string pending = Path.Combine(_lKeepFileRoot, name + LKeepFilePending);
        string path = Path.Combine(_lKeepFileRoot, name + LKeepFileExtension);
        File.WriteAllText(pending, text);
        LWorkspaceRoot.LWorkspacePendingCommit(pending, path);
    }
}
