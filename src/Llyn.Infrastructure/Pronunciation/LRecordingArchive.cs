using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LRecordingArchive : LRecordingVault
{
    private const string LRecordingArchiveBucket = "audio";
    private const string LRecordingArchiveCache = "temp";
    private const string LRecordingArchiveExtension = ".mp3";
    private const int LRecordingDigestLength = 8;
    private const int LRecordingArchiveAttempts = 3;

    private static readonly TimeSpan LRecordingArchivePatience = TimeSpan.FromSeconds(5);

    private static readonly string[] LRecordingArchiveExtensions =
        [".mp3", ".ogg", ".oga", ".wav", ".m4a", ".aac", ".flac", ".opus", ".webm"];

    private static readonly HashSet<string> LRecordingArchiveDevices = new(
        ["con", "prn", "aux", "nul",
         "com1", "com2", "com3", "com4", "com5", "com6", "com7", "com8", "com9",
         "lpt1", "lpt2", "lpt3", "lpt4", "lpt5", "lpt6", "lpt7", "lpt8", "lpt9"],
        StringComparer.OrdinalIgnoreCase);

    private readonly string _lRecordingArchiveRoot;

    private readonly HttpClient _lRecordingArchiveClient;

    public LRecordingArchive(string root, HttpClient client)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(root);
        ArgumentNullException.ThrowIfNull(client);
        _lRecordingArchiveRoot = root;
        _lRecordingArchiveClient = client;
    }

    public async Task<string> LRecordingSave(
        LRecording recording, string word, string language, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(recording);
        ArgumentException.ThrowIfNullOrWhiteSpace(word);
        ArgumentException.ThrowIfNullOrWhiteSpace(language);
        ArgumentException.ThrowIfNullOrWhiteSpace(recording.LRecordingAddress);

        string address = recording.LRecordingAddress;
        byte[] audio = await LRecordingArchiveRead(address, cancellation).ConfigureAwait(false);

        string directory = Path.Combine(
            _lRecordingArchiveRoot, LRecordingArchiveBucket, LRecordingArchiveNormalize(language));
        Directory.CreateDirectory(directory);

        string path = Path.Combine(
            directory,
            LRecordingStemRead(word, recording.LRecordingVariety, address)
                + LRecordingExtensionRead(address));
        await LWorkspaceRoot.LWorkspaceFileSave(path, audio, cancellation).ConfigureAwait(false);
        return path;
    }

    public async Task<string> LRecordingPrepare(LRecording recording, CancellationToken cancellation)
    {
        ArgumentNullException.ThrowIfNull(recording);
        ArgumentException.ThrowIfNullOrWhiteSpace(recording.LRecordingAddress);

        string address = recording.LRecordingAddress;
        string directory = Path.Combine(_lRecordingArchiveRoot, LRecordingArchiveCache);
        string path = Path.Combine(directory, LRecordingNameRead(address));
        if (File.Exists(path))
        {
            return path;
        }

        byte[] audio = await LRecordingArchiveRead(address, cancellation).ConfigureAwait(false);
        Directory.CreateDirectory(directory);
        await LWorkspaceRoot.LWorkspaceFileSave(path, audio, cancellation).ConfigureAwait(false);
        return path;
    }

    private async Task<byte[]> LRecordingArchiveRead(string address, CancellationToken cancellation)
    {
        for (int attempt = 1; ; attempt++)
        {
            using HttpResponseMessage response = await _lRecordingArchiveClient
                .GetAsync(address, cancellation)
                .ConfigureAwait(false);
            if (response.StatusCode != HttpStatusCode.TooManyRequests || attempt >= LRecordingArchiveAttempts)
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync(cancellation).ConfigureAwait(false);
            }

            TimeSpan asked = response.Headers.RetryAfter?.Delta ?? TimeSpan.Zero;
            TimeSpan pause = asked > LRecordingArchivePatience ? asked : LRecordingArchivePatience;
            await Task.Delay(pause, cancellation).ConfigureAwait(false);
        }
    }

    private static string LRecordingStemRead(string word, string variety, string address)
    {
        string stem = LRecordingArchiveNormalize(word);
        if (variety.Length > 0)
        {
            stem += "." + LRecordingArchiveNormalize(variety);
        }

        return stem + "." + LRecordingDigestRead(address)[..LRecordingDigestLength];
    }

    private static string LRecordingNameRead(string address)
    {
        return LRecordingDigestRead(address)[..16] + LRecordingExtensionRead(address);
    }

    private static string LRecordingDigestRead(string address)
    {
        return Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(address)));
    }

    private static string LRecordingExtensionRead(string address)
    {
        try
        {
            string extension = Path.GetExtension(new Uri(address).AbsolutePath).ToLowerInvariant();
            return Array.IndexOf(LRecordingArchiveExtensions, extension) >= 0
                ? extension
                : LRecordingArchiveExtension;
        }
        catch (UriFormatException)
        {
            return LRecordingArchiveExtension;
        }
    }

    private static string LRecordingArchiveNormalize(string value)
    {
        foreach (char invalid in Path.GetInvalidFileNameChars())
        {
            value = value.Replace(invalid, '_');
        }

        value = value.Trim();
        int stop = value.IndexOf('.');
        string head = stop < 0 ? value : value[..stop];
        return LRecordingArchiveDevices.Contains(head) ? "_" + value : value;
    }
}
