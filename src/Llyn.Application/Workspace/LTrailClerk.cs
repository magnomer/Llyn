using System;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LTrailClerk
{
    private readonly LTrail _lTrailClerkTrail;
    private readonly LUsher _lTrailClerkUsher;
    private readonly string _lTrailClerkWorkspace;

    public LTrailClerk(LRig rig)
    {
        ArgumentNullException.ThrowIfNull(rig);
        _lTrailClerkTrail = rig.LRigTrail;
        _lTrailClerkUsher = rig.LRigUsher;
        _lTrailClerkWorkspace = rig.LRigWorkspace;
    }

    public string LTrailClerkWorkspace => _lTrailClerkWorkspace;

    public string LWorkspaceFormat()
    {
        string name = _lTrailClerkTrail.LTrailNameRead(_lTrailClerkWorkspace);
        return name.Length > 0 ? name : _lTrailClerkWorkspace;
    }

    public string LTrailNameNormalize(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        return _lTrailClerkTrail.LTrailNameNormalize(name);
    }

    public bool LTrailRootCheck(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        return _lTrailClerkTrail.LTrailRootCheck(path);
    }

    public bool LTrailPathCheck(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        return _lTrailClerkUsher.LUsherPathExist(path);
    }

    public string? LTrailPathResolve(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        return _lTrailClerkTrail.LTrailResolve(_lTrailClerkWorkspace, path);
    }

    public Uri? LTrailClerkResolve(string? location)
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

        return LTrailFileResolve(path);
    }

    public Uri? LTrailFileResolve(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        if (_lTrailClerkTrail.LTrailResolve(_lTrailClerkWorkspace, path) is not string full || full.StartsWith('\\'))
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

    public Uri? LTrailClerkRead(string? location)
    {
        Uri? address = LTrailClerkResolve(location);
        return address is null || (address.IsFile && !_lTrailClerkUsher.LUsherPathExist(address.LocalPath))
            ? null
            : address;
    }

    public void LTrailClerkOpen(string target)
    {
        ArgumentNullException.ThrowIfNull(target);
        _lTrailClerkUsher.LUsherOpen(target);
    }

    public string LRecordingResolve(string file)
    {
        ArgumentNullException.ThrowIfNull(file);
        return LTrailClerkResolve(file) is { IsFile: true } resolved ? resolved.LocalPath : file;
    }

    public bool LRecordingExist(string? file)
    {
        return LTrailClerkResolve(file) is { IsFile: true } resolved
            && _lTrailClerkUsher.LUsherPathExist(resolved.LocalPath);
    }

    public string LRecordingFormat(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        return _lTrailClerkTrail.LTrailRelativeResolve(_lTrailClerkWorkspace, path) ?? path;
    }
}
