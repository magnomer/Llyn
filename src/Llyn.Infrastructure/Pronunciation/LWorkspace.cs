using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
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
    private const string LWorkspacePending = ".tmp";
    private const int LWorkspaceDigestLength = 8;

    private static readonly string[] LWorkspaceExtensions =
        [".mp3", ".ogg", ".oga", ".wav", ".m4a", ".aac", ".flac", ".opus", ".webm"];

    private static readonly HashSet<string> LWorkspaceDevices = new(
        ["con", "prn", "aux", "nul",
         "com1", "com2", "com3", "com4", "com5", "com6", "com7", "com8", "com9",
         "lpt1", "lpt2", "lpt3", "lpt4", "lpt5", "lpt6", "lpt7", "lpt8", "lpt9"],
        StringComparer.OrdinalIgnoreCase);

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
        ArgumentException.ThrowIfNullOrWhiteSpace(recording.LRecordingAddress);

        string address = recording.LRecordingAddress;
        byte[] audio = await client.GetByteArrayAsync(address, cancellation).ConfigureAwait(false);

        string directory = Path.Combine(root, LWorkspaceBucket, LWorkspaceNormalize(language));
        Directory.CreateDirectory(directory);

        string path = Path.Combine(
            directory,
            LWorkspaceStemRead(word, recording.LRecordingVariety, address) + LWorkspaceExtensionRead(address));
        await LWorkspaceFileSave(path, audio, cancellation).ConfigureAwait(false);
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
        ArgumentException.ThrowIfNullOrWhiteSpace(recording.LRecordingAddress);

        string address = recording.LRecordingAddress;
        string directory = Path.Combine(root, LWorkspaceCache);
        string path = Path.Combine(directory, LWorkspaceNameRead(address));
        if (File.Exists(path))
        {
            return path;
        }

        byte[] audio = await client.GetByteArrayAsync(address, cancellation).ConfigureAwait(false);
        Directory.CreateDirectory(directory);
        await LWorkspaceFileSave(path, audio, cancellation).ConfigureAwait(false);
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
            await LWorkspaceFileSave(path, svg, cancellation).ConfigureAwait(false);
            return path;
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    private static async Task LWorkspaceFileSave(string path, byte[] content, CancellationToken cancellation)
    {
        string pending = path + LWorkspacePending;
        await File.WriteAllBytesAsync(pending, content, cancellation).ConfigureAwait(false);
        LWorkspaceRoot.LWorkspacePendingCommit(pending, path);
    }

    private static string LWorkspaceStemRead(string word, string variety, string address)
    {
        string stem = LWorkspaceNormalize(word);
        if (variety.Length > 0)
        {
            stem += "." + LWorkspaceNormalize(variety);
        }

        return stem + "." + LWorkspaceDigestRead(address)[..LWorkspaceDigestLength];
    }

    private static string LWorkspaceNameRead(string address)
    {
        return LWorkspaceDigestRead(address)[..16] + LWorkspaceExtensionRead(address);
    }

    private static string LWorkspaceDigestRead(string address)
    {
        return Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(address)));
    }

    private static string LWorkspaceExtensionRead(string address)
    {
        try
        {
            string extension = Path.GetExtension(new Uri(address).AbsolutePath).ToLowerInvariant();
            return Array.IndexOf(LWorkspaceExtensions, extension) >= 0 ? extension : LWorkspaceExtension;
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

        value = value.Trim();
        int stop = value.IndexOf('.');
        string head = stop < 0 ? value : value[..stop];
        return LWorkspaceDevices.Contains(head) ? "_" + value : value;
    }
}
