using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LWorkspace
{
    private const string LWorkspaceBucket = "audio";
    private const string LWorkspaceCache = "temp";
    private const string LWorkspaceExtension = ".mp3";
    private const string LWorkspaceFlagFolder = "flags";
    private const string LWorkspaceFlagExtension = ".svg";

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

        string directory = Path.Combine(root, LWorkspaceBucket, LWorkspaceNormalize(language));
        Directory.CreateDirectory(directory);

        string path = Path.Combine(directory, LWorkspaceNormalize(word) + LWorkspaceExtensionRead(recording.LRecordingAddress));
        await File.WriteAllBytesAsync(path, audio, cancellation).ConfigureAwait(false);
        return path;
    }

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

        string directory = Path.Combine(root, LWorkspaceCache);
        Directory.CreateDirectory(directory);

        string path = Path.Combine(directory, LWorkspaceNameRead(recording.LRecordingAddress));
        await File.WriteAllBytesAsync(path, audio, cancellation).ConfigureAwait(false);
        return path;
    }

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
