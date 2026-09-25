using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LSounding
{
    private readonly LPhonologyPort _lPhonologyPort;

    public LSounding(LPhonologyPort phonology)
    {
        ArgumentNullException.ThrowIfNull(phonology);

        _lPhonologyPort = phonology;
    }

    public event Action? LSoundingChanged;

    public event Action<string, Exception>? LSoundingFailed;

    public IReadOnlyList<LFanqieGroup> LSoundingFanqieRead(long? entry)
    {
        if (entry is not long id)
        {
            return [];
        }

        try
        {
            _lPhonologyPort.LEngineFanqieStart(id);
            return _lPhonologyPort.LEngineFanqieDivide(id);
        }
        catch (Exception)
        {
            return [];
        }
    }

    public string LSoundingReadingRead(long? entry, string headword)
    {
        return LFanqieGroup.LFanqieReadingFormat(LSoundingFanqieRead(entry), headword);
    }

    public IReadOnlyList<LFanqieRow> LSoundingAnchorRead(long? entry)
    {
        List<LFanqieRow> rows = [];
        foreach (LFanqieGroup group in LSoundingFanqieRead(entry))
        {
            rows.AddRange(group.LFanqieGroupRows);
        }

        return rows;
    }

    public void LSoundingFanqieRebuild(long? entry)
    {
        if (entry is not long id)
        {
            return;
        }

        try
        {
            _lPhonologyPort.LEngineFanqieRebuild(id);
        }
        catch (Exception exception)
        {
            LSoundingFailed?.Invoke("Display.FanqieRebuildFailed", exception);
            return;
        }

        LSoundingChanged?.Invoke();
    }

    public void LSoundingFanqieSet(long? entry, long fanqieId, int rank)
    {
        if (entry is not long id)
        {
            return;
        }

        try
        {
            _lPhonologyPort.LEngineFanqieSet(id, fanqieId, rank);
        }
        catch (Exception exception)
        {
            LSoundingFailed?.Invoke("Display.FanqieRepresentativeFailed", exception);
            return;
        }

        LSoundingChanged?.Invoke();
    }

    public IReadOnlyList<LScriptGroup> LSoundingScriptRead(long? entry)
    {
        if (entry is not long id)
        {
            return [];
        }

        try
        {
            _lPhonologyPort.LEngineScriptStart(id);
            return _lPhonologyPort.LEngineScriptDivide(id);
        }
        catch (Exception)
        {
            return [];
        }
    }

    public void LSoundingScriptRebuild(long? entry)
    {
        if (entry is not long id)
        {
            return;
        }

        try
        {
            _lPhonologyPort.LEngineScriptRebuild(id);
        }
        catch (Exception exception)
        {
            LSoundingFailed?.Invoke("Display.ScriptRebuildFailed", exception);
            return;
        }

        LSoundingChanged?.Invoke();
    }

    public IReadOnlyList<LParadigmSlot> LSoundingParadigmRead(long? entry)
    {
        if (entry is not long id)
        {
            return [];
        }

        try
        {
            return _lPhonologyPort.LEngineParadigmShow(id);
        }
        catch (Exception)
        {
            return [];
        }
    }
}
