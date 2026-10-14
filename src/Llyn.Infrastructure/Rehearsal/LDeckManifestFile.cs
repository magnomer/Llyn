using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LDeckManifestFile : LDeckManifestVault
{
    private const string LDeckManifestKeep = "rehearsal";
    private const string LDeckManifestDecks = "decks";
    private const string LDeckManifestDigest = "digest";
    private const string LDeckManifestMedia = "media";
    private const string LDeckManifestWritten = "written";

    private readonly LKeep _lDeckManifestKeep;

    public LDeckManifestFile(LKeep keep)
    {
        ArgumentNullException.ThrowIfNull(keep);
        _lDeckManifestKeep = keep;
    }

    public LDeckManifest LDeckManifestRead()
    {
        try
        {
            return LDeckManifestParse(_lDeckManifestKeep.LKeepRead(LDeckManifestKeep));
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

    public void LDeckManifestSave(LDeckManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        try
        {
            _lDeckManifestKeep.LKeepSave(LDeckManifestKeep, LDeckManifestFormat(manifest));
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

    public static LDeckManifest LDeckManifestParse(string? text)
    {
        Dictionary<long, (string, IReadOnlyList<string>, string)> decks = [];
        if (string.IsNullOrWhiteSpace(text))
        {
            return new LDeckManifest(decks);
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(text);
            JsonElement root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object ||
                !root.TryGetProperty(LDeckManifestDecks, out JsonElement stored) ||
                stored.ValueKind != JsonValueKind.Object)
            {
                return new LDeckManifest(decks);
            }

            foreach (JsonProperty deck in stored.EnumerateObject())
            {
                if (deck.Value.ValueKind == JsonValueKind.Object &&
                    long.TryParse(deck.Name, NumberStyles.None, CultureInfo.InvariantCulture, out long id))
                {
                    decks[id] = (
                        LDeckTextRead(deck.Value, LDeckManifestDigest),
                        LDeckMediaRead(deck.Value),
                        LDeckTextRead(deck.Value, LDeckManifestWritten));
                }
            }

            return new LDeckManifest(decks);
        }
        catch (JsonException)
        {
            return new LDeckManifest(new Dictionary<long, (string, IReadOnlyList<string>, string)>());
        }
    }

    public static string LDeckManifestFormat(LDeckManifest manifest)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        SortedDictionary<string, object> decks = new(StringComparer.Ordinal);
        foreach ((long id, (string digest, IReadOnlyList<string> media, string written)) in manifest.LDeckManifestDecks)
        {
            string key = id.ToString(CultureInfo.InvariantCulture);
            decks[key] = new SortedDictionary<string, object>(StringComparer.Ordinal)
            {
                [LDeckManifestDigest] = digest,
                [LDeckManifestMedia] = media.Order(StringComparer.Ordinal).ToArray(),
                [LDeckManifestWritten] = written,
            };
        }

        Dictionary<string, object> payload = new(StringComparer.Ordinal) { [LDeckManifestDecks] = decks };
        return JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
    }

    private static string LDeckTextRead(JsonElement deck, string key) =>
        deck.TryGetProperty(key, out JsonElement value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()!
            : string.Empty;

    private static string[] LDeckMediaRead(JsonElement deck)
    {
        if (!deck.TryGetProperty(LDeckManifestMedia, out JsonElement media) || media.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        return media.EnumerateArray()
            .Where(name => name.ValueKind == JsonValueKind.String)
            .Select(name => name.GetString()!)
            .ToArray();
    }
}
