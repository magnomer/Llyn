using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Infrastructure;

/// <summary>
/// The user's workspace on disk. Downloads remote assets into the workspace and returns their local
/// paths: a chosen recording's bytes saved under a per-language folder, and a language's flag image
/// fetched by country code and cached for reuse. The engine calls these once a source or language is
/// chosen; nothing about which source or language is baked in here.
/// </summary>
public static class LWorkspace
{
    private const string LWorkspaceBucket = "audio";
    private const string LWorkspaceCache = "temp";
    private const string LWorkspaceExtension = ".mp3";
    private const string LWorkspaceFlagFolder = "flags";
    private const string LWorkspaceFlagExtension = ".svg";

    // The flag-icons set (github.com/lipis/flag-icons), served over jsDelivr's CDN of the repo. The
    // 4x3 SVGs match the flag box's aspect; the leaf is the lowercased ISO 3166-1 alpha-2 code.
    private const string LWorkspaceFlagHost = "https://cdn.jsdelivr.net/gh/lipis/flag-icons/flags/4x3/";

    public static async Task<string> LWorkspaceRecordingSave(
        LRecording recording,
        string word,
        string language,
        string root,
        HttpClient client,
        CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(recording);
        ArgumentNullException.ThrowIfNull(client);
        ArgumentException.ThrowIfNullOrWhiteSpace(word);
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentException.ThrowIfNullOrWhiteSpace(root);

        byte[] audio = await client.GetByteArrayAsync(recording.LRecordingAddress, cancellation).ConfigureAwait(false);

        // Saved audio is workspace data, so it lives under the chosen workspace root, never elsewhere.
        string directory = Path.Combine(root, LWorkspaceBucket, LWorkspaceNormalize(language));
        Directory.CreateDirectory(directory);

        string path = Path.Combine(directory, LWorkspaceNormalize(word) + LWorkspaceExtensionRead(recording.LRecordingAddress));
        await File.WriteAllBytesAsync(path, audio, cancellation).ConfigureAwait(false);
        return path;
    }

    /// <summary>
    /// Downloads a recording to a temporary cache file for immediate playback and returns its path.
    /// Playing a local file is reliable, whereas streaming a remote, token-bearing URL through the
    /// media stack is not. The file name derives from the audio's own name, so replaying reuses it.
    /// </summary>
    public static async Task<string> LWorkspaceRecordingPrepare(
        LRecording recording,
        string root,
        HttpClient client,
        CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(recording);
        ArgumentNullException.ThrowIfNull(client);
        ArgumentException.ThrowIfNullOrWhiteSpace(root);

        byte[] audio = await client.GetByteArrayAsync(recording.LRecordingAddress, cancellation).ConfigureAwait(false);

        // Temporary files also stay inside the workspace, under its own temp folder.
        string directory = Path.Combine(root, LWorkspaceCache);
        Directory.CreateDirectory(directory);

        string path = Path.Combine(directory, LWorkspaceNameRead(recording.LRecordingAddress));
        await File.WriteAllBytesAsync(path, audio, cancellation).ConfigureAwait(false);
        return path;
    }

    /// <summary>
    /// Returns the local path to the flag image for <paramref name="code"/> (an ISO 3166-1 alpha-2
    /// country code), downloading it from the flag-icons set into the workspace cache on first use and
    /// serving the cached copy thereafter. Returns <c>null</c> when the download fails, so a missing
    /// flag never blocks the UI.
    /// </summary>
    public static async Task<string?> LWorkspaceFlagRead(
        string code,
        string root,
        HttpClient client,
        CancellationToken cancellation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        ArgumentNullException.ThrowIfNull(client);

        string key = LWorkspaceNormalize(code).ToLowerInvariant();
        string directory = Path.Combine(root, LWorkspaceFlagFolder);
        Directory.CreateDirectory(directory);

        string path = Path.Combine(directory, key + LWorkspaceFlagExtension);
        if (File.Exists(path))
        {
            return path;
        }

        try
        {
            byte[] svg = await client
                .GetByteArrayAsync(LWorkspaceFlagHost + key + LWorkspaceFlagExtension, cancellation)
                .ConfigureAwait(false);
            await File.WriteAllBytesAsync(path, svg, cancellation).ConfigureAwait(false);
            return path;
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    private static string LWorkspaceNameRead(string address)
    {
        try
        {
            string leaf = Path.GetFileName(new Uri(address).AbsolutePath);
            if (leaf.Length > 0)
            {
                return LWorkspaceNormalize(leaf);
            }
        }
        catch (UriFormatException)
        {
            // Fall through to the default name below.
        }

        return "audio" + LWorkspaceExtension;
    }

    private static string LWorkspaceExtensionRead(string address)
    {
        try
        {
            string extension = Path.GetExtension(new Uri(address).AbsolutePath);
            return extension.Length is > 1 and <= 5 ? extension : LWorkspaceExtension;
        }
        catch (UriFormatException)
        {
            return LWorkspaceExtension;
        }
    }

    private static string LWorkspaceNormalize(string value)
    {
        foreach (char invalid in Path.GetInvalidFileNameChars())
        {
            value = value.Replace(invalid, '_');
        }

        return value.Trim();
    }
}
