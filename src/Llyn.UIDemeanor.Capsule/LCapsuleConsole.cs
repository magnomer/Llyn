using System;
using System.IO;
using System.Text.Json;

namespace Llyn.UIDemeanor.Capsule;

public sealed class LCapsuleConsole
{
    private const string LCapsuleConsoleName = "console.json";

    private static readonly JsonSerializerOptions LCapsuleConsoleOptions = new()
    {
        WriteIndented = true,
    };

    public LCapsuleSetting LCapsuleConsoleRead(string? root)
    {
        if (string.IsNullOrWhiteSpace(root))
        {
            return new LCapsuleSetting();
        }

        string path = Path.Combine(root, LCapsuleConsoleName);
        try
        {
            if (!File.Exists(path))
            {
                return new LCapsuleSetting();
            }

            return JsonSerializer.Deserialize<LCapsuleSetting>(File.ReadAllText(path), LCapsuleConsoleOptions)
                ?? new LCapsuleSetting();
        }
        catch (JsonException)
        {
            return new LCapsuleSetting();
        }
        catch (IOException)
        {
            return new LCapsuleSetting();
        }
        catch (UnauthorizedAccessException)
        {
            return new LCapsuleSetting();
        }
    }

    public void LCapsuleConsoleSave(string? root, LCapsuleSetting state)
    {
        ArgumentNullException.ThrowIfNull(state);
        if (string.IsNullOrWhiteSpace(root))
        {
            return;
        }

        string path = Path.Combine(root, LCapsuleConsoleName);
        string pending = path + ".tmp";
        try
        {
            File.WriteAllText(pending, JsonSerializer.Serialize(state, LCapsuleConsoleOptions));
            File.Move(pending, path, true);
        }
        finally
        {
            try
            {
                File.Delete(pending);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }
}
