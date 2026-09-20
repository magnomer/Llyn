using System;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public Uri? LEngineLocationResolve(string? location)
    {
        string trimmed = (location ?? string.Empty).Trim();
        if (trimmed.Length == 0 || trimmed.StartsWith('\\') || trimmed.StartsWith('/'))
        {
            return null;
        }

        if (trimmed.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return Uri.TryCreate(trimmed, UriKind.Absolute, out Uri? web) ? web : null;
        }

        string path = trimmed;
        if (Uri.TryCreate(trimmed, UriKind.Absolute, out Uri? parsed) && trimmed.Contains(':'))
        {
            if (!parsed.IsFile || parsed.Host.Length > 0 || parsed.LocalPath.StartsWith('\\'))
            {
                return null;
            }

            path = parsed.LocalPath;
        }

        string workspace;
        lock (_lEngineGate)
        {
            workspace = _lEngineWorkspace;
        }

        return LEngineLocationResolve(path, workspace);
    }

    private Uri? LEngineLocationResolve(string path, string workspace)
    {
        if (LEngineTrailRead().LTrailResolve(workspace, path) is not string full || full.StartsWith('\\'))
        {
            return null;
        }

        try
        {
            return new Uri(full, UriKind.Absolute);
        }
        catch (Exception exception) when (exception is ArgumentException or UriFormatException)
        {
            return null;
        }
    }

    public Uri? LEngineLocationRead(string? location)
    {
        Uri? address = LEngineLocationResolve(location);
        return address is null || (address.IsFile && !_lEngineUsher.LUsherPathExist(address.LocalPath))
            ? null
            : address;
    }

    public void LEngineLocationOpen(string target)
    {
        _lEngineUsher.LUsherOpen(target);
    }

    private string LEngineRecordingResolve(string file)
    {
        return LEngineLocationResolve(file) is { IsFile: true } resolved ? resolved.LocalPath : file;
    }

    public bool LEngineRecordingExist(string? file)
    {
        return LEngineLocationResolve(file) is { IsFile: true } resolved
            && _lEngineUsher.LUsherPathExist(resolved.LocalPath);
    }
}
