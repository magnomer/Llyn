using System;
using System.IO;
using System.Text.Json;

namespace Llyn.UIDeportment.Capsule;

public sealed class LCapsule
{
    private const string LCapsuleName = "capsule.json";

    private static readonly JsonSerializerOptions LCapsuleOptions = new()
    {
        WriteIndented = true,
    };

    public LCapsuleContent LCapsuleRead(string? root)
    {
        if (string.IsNullOrWhiteSpace(root))
        {
            return new LCapsuleContent();
        }

        string path = Path.Combine(root, LCapsuleName);
        try
        {
            if (!File.Exists(path))
            {
                return new LCapsuleContent();
            }

            return JsonSerializer.Deserialize<LCapsuleContent>(File.ReadAllText(path), LCapsuleOptions)
                ?? new LCapsuleContent();
        }
        catch (JsonException)
        {
            return new LCapsuleContent();
        }
        catch (IOException)
        {
            return new LCapsuleContent();
        }
        catch (UnauthorizedAccessException)
        {
            return new LCapsuleContent();
        }
    }

    public void LCapsuleSave(string? root, LCapsuleContent state)
    {
        ArgumentNullException.ThrowIfNull(state);
        if (string.IsNullOrWhiteSpace(root))
        {
            return;
        }

        string path = Path.Combine(root, LCapsuleName);
        string pending = path + ".tmp";
        File.WriteAllText(pending, JsonSerializer.Serialize(state, LCapsuleOptions));
        File.Move(pending, path, true);
    }
}
