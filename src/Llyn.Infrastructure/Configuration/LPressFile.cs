using System;
using System.IO;
using System.Text.Json;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LPressFile : LPressVault
{
    private const string LPressFileName = "press";

    private readonly LKeep _lPressFileKeep;

    public LPressFile(LKeep keep)
    {
        ArgumentNullException.ThrowIfNull(keep);
        _lPressFileKeep = keep;
    }

    public LPressSetting? LPressVaultRead()
    {
        string? text;
        try
        {
            text = _lPressFileKeep.LKeepRead(LPressFileName);
        }
        catch (IOException exception)
        {
            throw new LVaultFault(exception);
        }
        catch (UnauthorizedAccessException exception)
        {
            throw new LVaultFault(exception);
        }

        return text is null ? null : LPressFileParse(text);
    }

    public void LPressVaultSave(LPressSetting setting)
    {
        ArgumentNullException.ThrowIfNull(setting);

        try
        {
            _lPressFileKeep.LKeepSave(LPressFileName, LPressFileFormat(setting));
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

    public static LPressSetting? LPressFileParse(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<LPressSetting>(text);
        }
        catch (JsonException)
        {
            return null;
        }
        catch (NotSupportedException)
        {
            return null;
        }
    }

    public static string LPressFileFormat(LPressSetting setting)
    {
        ArgumentNullException.ThrowIfNull(setting);

        return JsonSerializer.Serialize(setting);
    }
}
