using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LManifestFile : LManifestVault
{
    private const string LManifestFileKeep = "joplin";
    private const string LManifestFileNotes = "notes";
    private const string LManifestFileRealm = "realm";

    private readonly LKeep _lManifestFileKeep;

    public LManifestFile(LKeep keep)
    {
        ArgumentNullException.ThrowIfNull(keep);
        _lManifestFileKeep = keep;
    }

    public LManifest LManifestRead()
    {
        try
        {
            return LManifestFileParse(_lManifestFileKeep.LKeepRead(LManifestFileKeep));
        }
        catch (IOException exception)
        {
            throw new LVaultFault(exception);
        }
        catch (UnauthorizedAccessException exception)
        {
            throw new LVaultFault(exception);
        }
    }

    public void LManifestSave(LManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        try
        {
            _lManifestFileKeep.LKeepSave(LManifestFileKeep, LManifestFileFormat(manifest));
        }
        catch (IOException exception)
        {
            throw new LVaultFault(exception);
        }
        catch (UnauthorizedAccessException exception)
        {
            throw new LVaultFault(exception);
        }
    }

    public static LManifest LManifestFileParse(string? text)
    {
        Dictionary<string, string> digest = new(StringComparer.Ordinal);
        if (string.IsNullOrWhiteSpace(text))
        {
            return new LManifest(digest, string.Empty);
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(text);
            JsonElement root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object ||
                !root.TryGetProperty(LManifestFileNotes, out JsonElement notes) ||
                notes.ValueKind != JsonValueKind.Object)
            {
                return new LManifest(digest, string.Empty);
            }

            foreach (JsonProperty note in notes.EnumerateObject())
            {
                if (note.Value.ValueKind == JsonValueKind.String && LOutpostSeal.LOutpostSealMatch(note.Name))
                {
                    digest[note.Name] = note.Value.GetString()!;
                }
            }

            string realm = root.TryGetProperty(LManifestFileRealm, out JsonElement stamp)
                && stamp.ValueKind == JsonValueKind.String
                ? stamp.GetString()!
                : string.Empty;
            return new LManifest(digest, realm);
        }
        catch (JsonException)
        {
            return new LManifest(new Dictionary<string, string>(StringComparer.Ordinal), string.Empty);
        }
    }

    public static string LManifestFileFormat(LManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        Dictionary<string, object> payload = new(StringComparer.Ordinal)
        {
            [LManifestFileRealm] = manifest.LManifestRealm ?? string.Empty,
            [LManifestFileNotes] = new SortedDictionary<string, string>(
                new Dictionary<string, string>(manifest.LManifestDigest, StringComparer.Ordinal),
                StringComparer.Ordinal)
        };

        return JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
    }
}
